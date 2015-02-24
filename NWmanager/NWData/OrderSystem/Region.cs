using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Region
    {
        private List<Coordinate> _CoordinateList = new List<Coordinate>();

        #region "Constructors"
        
        public Region()
        {

        }

        public Region(RegionInfo regionInfo)
        { 
            foreach(var pos in regionInfo.Coordinates)
            {
                Coordinates.Add(new Coordinate(pos));
            }
        }

        #endregion


        #region "Public properties"

        public List<Coordinate> Coordinates
        {
            get { return _CoordinateList; }
        }

        public int Count
        {
            get
            {
                return _CoordinateList.Count;
            }
        }

        #endregion



        #region "Public methods"
        
        /// <summary>
        /// Determines whether a coordinate falls within the boundary specified by a 
        /// polygon region
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool IsWithinRegion(Coordinate coordinate)
        {
            return MapHelper.IsWithinRegion(this, coordinate);
        }

        public RegionInfo GetRegionInfo()
        {
            RegionInfo info = new RegionInfo();
            foreach (var c in Coordinates)
            {
                var position = new Position(c);
                info.Coordinates.Add(position.GetPositionInfo());
            }
            return info;
        }

        public override string ToString()
        {
            string temp = "Region: ";
            foreach (var c in Coordinates)
            {
                temp += "\n" + c.ToString();
            }
            return temp;
        }

        #endregion

        #region "Public static methods"
        
        public static Region FromCircle(Coordinate coordinateCenter, double radiusM)
        {
            Region reg = new Region();
            for (int i = 1; i <= 16; i++)
            {
                double angleDeg = 360.0 * ((double)i / 16.0);
                Coordinate coord = new Position(coordinateCenter).Offset(angleDeg, radiusM).Coordinate;
                reg.Coordinates.Add(coord);
            }
            return reg;
        }

        public static Region FromCoordinates(Coordinate[] coords)
        {
            Region reg = new Region();
            foreach (var c in coords)
            {
                reg.Coordinates.Add(c);
            }
            return reg;
        }

        public static Region FromRectangle(Coordinate coordinateCenter, double widthEastWestM, double heightNorthSouthM)
        {
            var reg = new Region();
            Coordinate nw = new Position(coordinateCenter).Offset(0, (heightNorthSouthM / 2.0)).Coordinate;
            nw = new Position(nw).Offset(270, (widthEastWestM / 2.0)).Coordinate;
            Coordinate ne = new Position(nw).Offset(90, widthEastWestM).Coordinate;
            Coordinate se = new Position(ne).Offset(180, heightNorthSouthM).Coordinate;
            Coordinate sw = new Position(se).Offset(270, widthEastWestM).Coordinate;
            reg.Coordinates.Add(nw);
            reg.Coordinates.Add(ne);
            reg.Coordinates.Add(se);
            reg.Coordinates.Add(sw);
            reg.Coordinates.Add(nw);
            return reg;
        }

        #endregion

    }
}
