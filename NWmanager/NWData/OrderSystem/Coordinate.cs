using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Coordinate
    {
        #region "Private fields"
        private double _latitudeRad;
        private double _longitudeRad;
        
        #endregion

        #region "Constructors"
        public Coordinate()
        {
            
        }

        public Coordinate(double latitudeDeg, double longitudeDeg) 
            : this()
        {
            LatitudeDeg = latitudeDeg;
            LongitudeDeg = longitudeDeg;
        }

        public Coordinate(Coordinate oldCoordinate)
            : this()
        {
            LatitudeDeg = oldCoordinate.LatitudeDeg;
            LongitudeDeg = oldCoordinate.LongitudeDeg;
        }

        public Coordinate(PositionInfo info) : this()
        {
            LatitudeDeg = info.Latitude;
            LongitudeDeg = info.Longitude;
        }

        #endregion

        #region "Public properties"
        /// <summary>
        /// Latitude (Y) in degrees. -90 to 90 
        /// </summary>
        private double _latitudeDeg;
        public double LatitudeDeg
        {
            get { return _latitudeDeg; }
            set
            {
                _latitudeDeg = value;
                _latitudeRad = _latitudeDeg.ToRadian();
            }
        }

        /// <summary>
        /// Longitude (X) in degree. -180 to 180
        /// </summary>
        private double _longitudeDeg;
        public double LongitudeDeg
        {
            get { return _longitudeDeg; }
            set
            {
                _longitudeDeg = value;
                _longitudeRad = _longitudeDeg.ToRadian();
            }
        }

        /// <summary>
        /// Sets and returns latitude (north-south) in Radians
        /// </summary>
        public double LatitudeRad
        {
            get 
            { 
                return _latitudeRad;  
            }
            set
            {
                _latitudeRad = value;
                _latitudeDeg = _latitudeRad.ToDegreeSignedLongitude();
            }
        }

        /// <summary>
        /// Sets and returns longitude (east-west) in Radians
        /// </summary>
        public double LongitudeRad
        {
            get
            {
                return _longitudeRad;
            }
            set
            {
                _longitudeRad = value;
                _longitudeDeg = _longitudeRad.ToDegreeSignedLongitude();
            }
        }

        #endregion

        #region "Public static methods"
        /// <summary>
        /// Takes a string in the space separated format "LA MM SS LO MM SS" and returns a coordinate. Use negative
        /// numbers for southern latitudes and western longitudes.
        /// </summary>
        /// <param name="formattedCoordinate"></param>
        /// <returns></returns>
        public static Coordinate ParseFromString(string formattedCoordinate)
        { 
            string[] parsed = formattedCoordinate.Split(' ');
            if (parsed.Length != 6)
            {
                throw new ArgumentOutOfRangeException("formattedCoordinate", "Must contain six double values separated by space.");
            }
            try
            {
                double Lat = double.Parse(parsed[0]);
                double LatMin = double.Parse(parsed[1]);
                double LatSec = double.Parse(parsed[2]);
                double Lon = double.Parse(parsed[3]);
                double LonMin = double.Parse(parsed[4]);
                double LonSec = double.Parse(parsed[5]);
                //Given a DMS (Degrees, Minutes, Seconds) coordinate such as W87°43'41", 
                //it's trivial to convert it to a number of decimal degrees using the following method:
                //Calculate the total number of seconds, 43'41" = (43*60 + 41) = 2621 seconds.
                //The fractional part is total number of seconds divided by 3600. 2621 / 3600 = ~0.728056
                //Add fractional degrees to whole degrees to produce the final result: 87 + 0.728056 = 87.728056
                //Since it is a West longitude coordinate, negate the result.
                //The final result is -87.728056.
                double TmpLat = ((LatMin * 60) + LatSec) / 3600;
                double TmpLon = ((LonMin * 60) + LonSec) / 3600;

                if (Lat < 0)
                {
                    Lat = Lat - TmpLat;
                }
                else
                {
                    Lat = Lat + TmpLat;
                }
                if (Lon < 0)
                {
                    Lon = Lon - TmpLon;
                }
                else
                {
                    Lon = Lon + TmpLon;
                }
                return new Coordinate(Lat, Lon);
            }
            catch (Exception ex)
            {
                throw new Exception("Must contain six double values separated by space.", ex);
            }
        }

        #endregion

        #region "Public methods"

        public double DistanceToM(Coordinate otherCoordinate)
        {
            return MapHelper.CalculateDistanceM(this, otherCoordinate);
        }

        public Coordinate Clone()
        {
            Coordinate coord = new Coordinate(this.LatitudeDeg, this.LongitudeDeg);
            return coord;
        }

        public override string ToString()
        {
            string lat = LatitudeDeg.ToDegMinSecString();
            lat += (LatitudeDeg < 0) ? "S" : "N";
            string lon = LongitudeDeg.ToDegMinSecString();
            lon += (LongitudeDeg > 0) ? "E" : "W";
            return string.Format("{0}, {1}", lat, lon);
        }

        #endregion

        // Test for intersection
        public double X { get; set; }
        public double Y { get; set; }

        
        public void SetFromXY(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.LatitudeDeg = MapHelper.yToLat(y);
            this.LongitudeDeg = MapHelper.xToLong(x);
        }


    }
}
