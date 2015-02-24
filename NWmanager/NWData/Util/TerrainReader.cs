using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Util.MemoryMapping;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace TTG.NavalWar.NWData.Util
{
    public static class TerrainReader
    {
        public const string TERRAIN_FILE_NAME = "terrain.bin";
        public static string TerrainFilePath = "";
        public static string MemoryFileName = "mmData";

        private const int WIDTH = 36423; //36120;
        private const int HEIGHT = 21849; //21852;
        private const float PIXEL_FACTOR = 4.9979f;
        private const float NORTHPOLE_OFFSET = 26033;

        private static MemoryMappedFile _memoryMap;
        private static MapViewStream _mapViewSteam;

        public static Vector2 CalculatePixelvalue(ProjCoordinate OrthoCoordinate)
        {          
            Vector2 res = new Vector2();
            res.x = (float)((((OrthoCoordinate.Longitude * PIXEL_FACTOR) )+ NORTHPOLE_OFFSET));
            res.y = (float)(OrthoCoordinate.Latitude * PIXEL_FACTOR);
            //Coordinate pco = PixelValueToCoordinate(res);
            return res;
        }

        public static Coordinate PixelValueToCoordinate(Vector2 pixvalue)
        {
            const float a = PIXEL_FACTOR;
            const float b = -PIXEL_FACTOR;

            double latOrtho = -(Math.Pow(b, -1) + Math.Pow(b, -1) * pixvalue.y);
            double longOrtho = -(NORTHPOLE_OFFSET) * Math.Pow(a, -1) + Math.Pow(a, -1) * pixvalue.x;
            var pro = new ProjCoordinate(latOrtho, longOrtho);
            Coordinate  c = pro.FromOrthoProjectedCoordinate();
            return c;
        }

        public static void LoadMemoryMap()
        {
            if (_memoryMap != null && _mapViewSteam != null)
            {
                return;
            }

            if (string.IsNullOrEmpty(TerrainFilePath))
            {
                SetTerrainDataPath();
            }

            string memoryMapFile = Path.Combine(TerrainFilePath, TERRAIN_FILE_NAME);
            if (!System.IO.File.Exists(memoryMapFile))
            {
                memoryMapFile = "C:\\terrain.bin"; //hack for unit tests!
            }

            try
            {
                GameManager.Instance.Log.LogDebug("TerrainReader->LoadMemoryMap initializing. Reading terrain file: <" + memoryMapFile + "> .");
                _memoryMap = MemoryMappedFile.Create(memoryMapFile, MapProtection.PageReadOnly);
                _mapViewSteam = _memoryMap.MapAsStream();
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("TerrainReader->LoadMemoryMap failed to load terrain from <" + memoryMapFile + "> ." + ex.Message);
                GameManager.Instance.Log.LogError( ex.ToString() );
            }
        }

        public static void CloseMemoryMap()
        {
            try
            {
                if (_mapViewSteam != null)
                {
                    _mapViewSteam.Close();
                    _mapViewSteam = null;
                }
                if (_memoryMap != null)
                {
                    _memoryMap.Close();
                    _memoryMap = null;
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private static void SetTerrainDataPath()
        {
            TerrainFilePath = typeof(TerrainReader).Assembly.Location;
            var pathOnly = Path.GetDirectoryName(TerrainFilePath);
            TerrainFilePath = pathOnly + "\\Data";
        }

        public static int GetHeightM(Coordinate coordinate)
        {
            try
            {
                return ConvertToHeightInMeters(ReadHeight(coordinate));
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("TerrainReader->GetHeightM failed. " + ex.Message + "\n" + ex.StackTrace);
                return 0;
            }
        }

        public static int GetMaxTerrainHeightAreaM(Coordinate coordinate, double sideLengthM)
        {
            try
            {
                var coords = new List<Coordinate>();
                coords.Add(coordinate);
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 0, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 45, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 90, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 135, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 180, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 225, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 270, sideLengthM / 2.0));
                coords.Add(MapHelper.CalculateNewPosition2(coordinate, 305, sideLengthM / 2.0));
                var heights = GetHeightArrayM(coords);
                return heights.Max();
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("TerrainReader->GetMaxTerrainHeightAreaM failed. " + ex.Message + "\n" + ex.StackTrace);
                return 0;
            }
        }

        public static int[] GetHeightArrayM(List<Coordinate> coordinates)
        {
            if (coordinates == null || coordinates.Count == 0)
            {
                GameManager.Instance.Log.LogError("TerrainReader->GetHeightArrayM called with empty or null list.");
                return new int[0];
            }
            int[] heightsM = new int[coordinates.Count];
            for (int i = 0; i < coordinates.Count; i++)
            {
                heightsM[i] = GetHeightM(coordinates[i]);
            }
            return heightsM;
        }

        /// <summary>
        /// Returns a list of Coordinate, forming a straight (geodesic) line from start to end point, consisting of noOfPoint samples.
        /// </summary>
        /// <param name="startCoordinate"></param>
        /// <param name="endCoordinate"></param>
        /// <param name="noOfPoints"></param>
        /// <returns></returns>
        public static List<Coordinate> GetCoordinateLine(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints, out double bearingDeg, out double distanceM)
        {
            if (startCoordinate == null || endCoordinate == null || noOfPoints < 1)
            {
                GameManager.Instance.Log.LogError("TerrainReader->GetCoordinateLine called with null coordinate(s) or less than 1 points.");
                bearingDeg = 0;
                distanceM = 0;
                return null;
            }
            var coordList = new List<Coordinate>();
            var lengthM = MapHelper.CalculateDistanceApproxM(startCoordinate, endCoordinate);
            distanceM = lengthM;
            bearingDeg = MapHelper.CalculateBearingDegrees(startCoordinate, endCoordinate);
            //var lengthToExtendM = lengthM * 0.25;
            //if (lengthToExtendM > 5000)
            //{
            //    lengthToExtendM = 5000;
            //}
            //endCoordinate = MapHelper.CalculateNewPositionApprox(endCoordinate, bearingDeg, lengthToExtendM);
            for (int i = 1; i <= noOfPoints; i++)
            {
                var coord = MapHelper.CalculateNewPositionApprox(startCoordinate, bearingDeg, i * ((double)lengthM / (double)noOfPoints));
                coordList.Add(coord);
            }
            return coordList;
        }

        public static int[] GetHeightArrayM(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints, out double bearingDeg, out double distanceM)
        {
            var coords = GetCoordinateLine(startCoordinate, endCoordinate, noOfPoints, out bearingDeg, out distanceM);
            return GetHeightArrayM(coords);
        }

        //public static int GetMaxHeightM(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints)
        //{
        //    return GetHeightArrayM(startCoordinate, endCoordinate, noOfPoints).Max();
        //}

        //public static int GetMinHeightM(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints)
        //{
        //    return GetHeightArrayM(startCoordinate, endCoordinate, noOfPoints).Min();
        //}

        public static TerrainLineSummary GetTerrainHeightSummary(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints)
        {
            double bearingDeg;
            double distanceM;
            var heightsM = GetHeightArrayM(startCoordinate, endCoordinate, noOfPoints, out bearingDeg, out distanceM);
            //bearingDeg = MapHelper.CalculateBearingDegrees(startCoordinate, endCoordinate);
            double distanceBehindTargetM = distanceM / 10.0;
            if (distanceBehindTargetM > 5000)
            {
                distanceBehindTargetM = 5000;
            }
            var coordBehind = MapHelper.CalculateNewPositionApprox(endCoordinate, bearingDeg, distanceBehindTargetM);
            double bearingBehDeg;
            double distBehM;
            var heightsBehindM = GetHeightArrayM(endCoordinate, coordBehind, 3, out bearingBehDeg, out distBehM);
            TerrainLineSummary summary = new TerrainLineSummary();
            summary.MaxHeightM = heightsM.Max();
            summary.MinHeightM = heightsM.Min();
            if (summary.MaxHeightM > summary.MinHeightM)
            {
                summary.HeightVarianceM = summary.MaxHeightM - summary.MinHeightM;
            }
            else
            {
                summary.HeightVarianceM = summary.MinHeightM - summary.MaxHeightM;
            }
            summary.MinHeightBehindM = heightsBehindM.Min();
            summary.MaxHeightBehindM = heightsBehindM.Max();
            if (summary.MaxHeightBehindM > summary.MinHeightBehindM)
            {
                summary.HeightVarianceBehindM = summary.MaxHeightBehindM - summary.MinHeightBehindM;
            }
            else
            {
                summary.HeightVarianceBehindM = summary.MinHeightBehindM - summary.MaxHeightBehindM;
            }

            return summary;
        }

        public static int GetVariationHeightM(Coordinate startCoordinate, Coordinate endCoordinate, int noOfPoints)
        {
            double bearingDeg;
            double distanceM;
            var heightsM = GetHeightArrayM(startCoordinate, endCoordinate, noOfPoints, out bearingDeg, out distanceM);
            var max = heightsM.Max();
            var min = heightsM.Min();
            if (max > min)
            {
                return max - min;
            }
            else
            {
                return min - max;
            }
        }

        public static BitArray LoadTraversableNodeMap()
        {
            BitArray ba = new BitArray((WIDTH * HEIGHT), false);
           
            // Save data to file
            FileStream stream = new FileStream("C:\\Traversable.bin", FileMode.OpenOrCreate);

            BinaryFormatter bf = new BinaryFormatter();
            ba = (BitArray)bf.Deserialize(stream);
            
            stream.Close();
            return ba;
        }
        
        public static void CreateTraversableNodeMap()
        {
            BitArray ba = new BitArray((WIDTH * HEIGHT),false);
            byte[] buff = new byte[1];
            
            for (int j = 0; j < WIDTH; j++)
            {
                for (int i = 0; i < HEIGHT; i++ )
                {
                    _mapViewSteam.Position = ((WIDTH * i) + (j));
                    int offset = 0;
                    int read =  _mapViewSteam.Read(buff, offset, 1);
                    if (buff[0] > 4)
                    {
                        ba[((WIDTH * i) + (j))] = false;
                    }
                    else
                    {
                        ba[((WIDTH * i) + (j))] = true;
                    }
                }
            }

            // Save data to file
            FileStream stream = new FileStream("C:\\Traversable.bin", FileMode.OpenOrCreate);
            
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream,ba);
            stream.Close();
        }

        public static byte ReadHeight(Coordinate coordinate)
        {
            var cor = coordinate.ToOrthoProjectedCoordinate();
            var pixelValue = CalculatePixelvalue(cor);
           
            var buff = new byte[1];

            _mapViewSteam.Position = (((WIDTH * (int)Math.Floor(pixelValue.y))) + ((int)Math.Floor(pixelValue.x)));
            
            _mapViewSteam.Read(buff, 0, 1);

            return buff[0];
        }
        
        public static int ConvertToHeightInMeters(byte data)
        {
            if (data < 5)
            {
                if (data == 0)
                {
                    return -1000;
                }
                if (data == 1)
                {
                    return -500;
                }
                if (data == 3)
                {
                    return -100;
                }
                else
                {
                    return -50;
                }
            }
            else
            {
                float step = 3700.0f / (255.0f);
                int d = (Convert.ToInt32(data));
                return Convert.ToInt32(Math.Round( step * (float)(d)));
            }
        }       

        public static void TestSomeCoordinates()
        {
            List<Coordinate> cords = new List<Coordinate>() 
                { 
                   new Coordinate(61.63713, 8.31401), 
                   new Coordinate(61.63407, 8.31187), 
                   new Coordinate(64.88613, -16.36963), 
                   new Coordinate(78.66022, -38.43018),
                   new Coordinate(58.95, -18.896), //open sea
                   new Coordinate(62.021528,-26.279297), //shallow waters?
                   new Coordinate(56.944974,2.197266), //north sea
                   new Coordinate(67.67, 11) //far north, on an island?
 
                };
            LoadMemoryMap();
            float[] heights = new float[cords.Count];
            int i = 0;
            foreach (Coordinate c in cords)
            {
               heights[i] = ConvertToHeightInMeters(ReadHeight(c));
                i++;
            }
            //CreateMemoryFileFromBmp();
            //TestPoint();
        }
    }

    public class Vector2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2()
        {

        }
        
        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }
        
        public override string ToString()
        {
            return x + "," + y;
        }
    }
}
