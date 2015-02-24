using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.Util
{
    public static class MapProjection
    {
        //uses Equidistant Cylindrical projection method

        private const double WGS84SEMIMAJOR = 6378137.0;
        private const double ONEOVERWGS84SEMIMAJOR = 1.0 / WGS84SEMIMAJOR;

        public const double ORTHO_LATITUDE_CENTER_DEG = 90;
        public const double ORTHO_LONGITUDE_CENTER_DEG = 0;

        public const double ORTHO_LATITUDE_CENTER_RAD = ORTHO_LATITUDE_CENTER_DEG * Math.PI / 180.0;
        public const double ORTHO_LONGITUDE_CENTER_RAD = ORTHO_LONGITUDE_CENTER_DEG * Math.PI / 180.0;

        #region "Constructors"



        #endregion


        #region "Public properties"

        #endregion



        #region "Public methods"

        public static ProjCoordinate ToOrthoProjectedCoordinate(this Coordinate geograpicCoordinate)
        {
            double latRad = geograpicCoordinate.LatitudeRad;
            double longRad = geograpicCoordinate.LongitudeRad;
            double latCenterRad = ORTHO_LATITUDE_CENTER_RAD;
            double longCenterRad = ORTHO_LONGITUDE_CENTER_RAD;

            double x = MapHelper.RADIUS_OF_EARTH_KM * Math.Cos(latRad) * Math.Sin(longRad - longCenterRad);
            double y = MapHelper.RADIUS_OF_EARTH_KM * (
                (Math.Cos(latCenterRad) * Math.Sin(latRad)) - (Math.Sin(latCenterRad) * Math.Cos(latRad) * 
                Math.Cos(longRad - longCenterRad)));
            ProjCoordinate result = new ProjCoordinate(y * -1,x); 
            //note: Does not clip results that are "behind" the globe
            return result;
        }

        public static Coordinate FromOrthoProjectedCoordinate(this ProjCoordinate projectedCoordinate)
        {
            double latCenterRad = ORTHO_LATITUDE_CENTER_RAD;
            double longCenterRad = ORTHO_LONGITUDE_CENTER_RAD;
            double y = -projectedCoordinate.Latitude;
            double x = projectedCoordinate.Longitude; 
            double rho = Math.Sqrt(Math.Pow(x,2) + Math.Pow(y, 2));
            double c = Math.Asin(rho / MapHelper.RADIUS_OF_EARTH_KM);

            double cCos = Math.Cos( c );
            double cSin = Math.Sin( c );

            double latCenterRadCos = Math.Cos( latCenterRad );
            double latCenterRadSin = Math.Sin( latCenterRad );

            double latRad = Math.Asin( ( cCos * latCenterRadSin ) + ( ( y * cSin * latCenterRadCos ) / rho ) );
            double longRad = longCenterRad + Math.Atan( ( x * cSin ) / ( ( rho * latCenterRadCos * cCos ) - y * latCenterRadSin * cSin ) );

            Coordinate result = new Coordinate(latRad.ToDegreeSignedLatitude(), longRad.ToDegreeSignedLongitude());
            return result;
        }

        public static ProjCoordinate ToEquiProjectedCoordinate(this Coordinate geographicCoordinate)
        {
            // First, convert the geographic coordinate to radians
            double radianX = geographicCoordinate.LongitudeDeg * (Math.PI / 180);
            double radianY = geographicCoordinate.LatitudeDeg * (Math.PI / 180);

            // Make a new Point object
            ProjCoordinate result = new ProjCoordinate();

            // Calculate the projected X coordinate
            result.Longitude = (float)(radianX * Math.Cos(0));

            // Calculate the projected Y coordinate
            result.Latitude = (float)radianY;

            // Return the result
            return result;

            //result.Latitude = geographicCoordinate.Latitude.ToRadian() * Math.Cos(0) * WGS84SEMIMAJOR;//Hmm...
            //result.Longitude = geographicCoordinate.Longitude.ToRadian()* WGS84SEMIMAJOR; //
            //return result;
        }
        public static Coordinate FromEquiCoordinate(this ProjCoordinate projectedCoordinate)
        {
            Coordinate result = new Coordinate();
            result.LongitudeDeg = (float)(projectedCoordinate.Longitude / Math.Cos(0) /
                         (Math.PI / 180.0));

            // Calculate the geographic Y coordinate (latitude)
            result.LatitudeDeg = (float)(projectedCoordinate.Latitude / (Math.PI / 180.0));

            //result.Latitude = projectedCoordinate.Latitude.ToRadian() * Math.Cos(0) * ONEOVERWGS84SEMIMAJOR;
            //result.Longitude = projectedCoordinate.Longitude.ToRadian() * ONEOVERWGS84SEMIMAJOR;
            return result;

        }
        #endregion


    }
}
