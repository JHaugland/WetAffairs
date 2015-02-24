using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.OrderSystem
{
    public class ProjCoordinate
    {
        public ProjCoordinate()
        {

        }

        public ProjCoordinate(double latitudeDeg, double longitudeDeg) 
            : this()
        {
            Latitude = latitudeDeg;
            Longitude = longitudeDeg;
        }

        public ProjCoordinate(Coordinate oldCoordinate)
            : this()
        {
            Latitude = oldCoordinate.LatitudeDeg;
            Longitude = oldCoordinate.LongitudeDeg;
        }

        private double latitude, longitude;

        /// <summary>
        /// Latitude (Y) in coordinate system
        /// </summary>
        public Double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
            }
        }

        /// <summary>
        /// Longitude (X) in coordinate system
        /// </summary>
        public Double Longitude
        {
            get { return longitude; }
            set
            {
              longitude = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Latitude, Longitude);
        }

    }
}
