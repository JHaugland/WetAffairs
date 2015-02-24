using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;


public static class CoordinateHelper
{
    private static double tiles;
    private static double circumrefrence;
    private static double radius;

    private static double XTileSize = 100;
    private static double YTileSize = 50;


    private const float WGS84SEMIMAJOR = 6378137.0f;
    private const float ONEOVERWGS84SEMIMAJOR = 1.0f / WGS84SEMIMAJOR;

    public const float ORTHO_LATITUDE_CENTER_DEG = 90;
    public const float ORTHO_LONGITUDE_CENTER_DEG = 0;
    public const float RADIUS_OF_EARTH_KM = 6371;
    public const float MILES_TO_METERS = 1609.344f;
    public const float MILES_TO_KILOMETERS = 1.609344f;
    public const float MILES_TO_NEUTICAL = 0.8684f;
    


    #region Coordinates

    public static double LongitudeToXvalue(double longitude, int zoomLevel)
    {
        tiles = Mathf.Pow(2, zoomLevel);
        circumrefrence = XTileSize * tiles;
        radius = circumrefrence / (2 * Mathf.PI);

        double Longitude = longitude * (Mathf.PI / 180);
        return (radius * Longitude);
    }

    public static double LatitudeToYvalue(double Latitude, int ZoomLevel)
    {
        // REFRANCE: http://cfis.savagexi.com/2006/05/03/google-maps-deconstructed
        tiles = Mathf.Pow(2, ZoomLevel);
        circumrefrence = YTileSize * tiles;
        radius = circumrefrence / (2 * Mathf.PI);
        double latitude = Latitude * (Mathf.PI / 180);
        double Y = radius / 2.0f * Mathf.Log((float)((1.0f + Mathf.Sin((float)latitude)) / (1.0 - Mathf.Sin((float)latitude))));
        return Y;
    }

    public static double XToLong(double x)
    {
        // REFRANCE: http://cfis.savagexi.com/2006/05/03/google-maps-deconstructed
        tiles = Mathf.Pow(2, 1);
        circumrefrence = XTileSize * tiles;
        radius = circumrefrence / (2 * Mathf.PI);
        double longRadians = x / radius;
        double longDegrees = ToDegreeSigned(longRadians);

        /* The user could have panned around the world a lot of times.
        Lat long goes from -180 to 180.  So every time a user gets 
        to 181 we want to subtract 360 degrees.  Every time a user
        gets to -181 we want to add 360 degrees. */

        double rotations = Mathf.Floor((float)(longDegrees + 180) / 360);
        double longitude = longDegrees - (rotations * 360);
        return longitude;

    }

    public static double YToLat(double y)
    {
        tiles = Mathf.Pow(2, 1);
        circumrefrence = YTileSize * tiles;
        radius = circumrefrence / (2 * Mathf.PI);
        float latitude = (Mathf.PI / 2) - (2 * Mathf.Atan(Mathf.Exp((float)(-1.0 * y / radius))));
        return ToDegreeSigned(latitude);
    }

    public static double ToDegreeSigned(double radian)
    {
        return (radian * 180) / Mathf.PI;
        //return ((radian / Math.PI * 180.0) + 360) % 360;
    }
    public static float ToDegreeSigned(float radian)
    {
        return (radian * 180) / Mathf.PI;
        //return ((radian / Math.PI * 180.0) + 360) % 360;
    }

    public static Vector3 LatLongToWorldPoint(PositionInfo position)
    {
        float z = (float)LatitudeToYvalue(position.Latitude, 1);
        float x = (float)LongitudeToXvalue(position.Longitude, 1);

        return new Vector3(x, 10, z);
    }

    public static Vector3 LatLongOrthoToWorldPoint(PositionInfo position)
    {
        float x = (float)position.LatitudeOrthoProjected;
        float y = (float)position.LongitudeOrthoProjected;

        return new Vector3(x, 10, 100);
    }

    public static Coordinate FromOrthoProjectedCoordinate(float latOrtho, float longOrtho)
    {
        float latCenterRad = ToRadian(ORTHO_LATITUDE_CENTER_DEG);
        float longCenterRad = ToRadian(ORTHO_LONGITUDE_CENTER_DEG);
        float y = latOrtho * -1;
        float x = longOrtho; // swap y,x?
        float rho = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        float c = Mathf.Asin(rho / RADIUS_OF_EARTH_KM);

        float latRad = Mathf.Asin((Mathf.Cos(c) * Mathf.Sin(latCenterRad))
            + ((y * Mathf.Sin(c) * Mathf.Cos(latCenterRad)) / rho));
        float longRad = longCenterRad + Mathf.Atan(
            (x * Mathf.Sin(c)) / ((rho * Mathf.Cos(latCenterRad) * Mathf.Cos(c)) - y * Mathf.Sin(latCenterRad) * Mathf.Sin(c)));

        Coordinate result = new Coordinate(ToDegreeSigned(latRad), ToDegreeSigned(longRad));
        return result;
    }

    

    public static float ToRadian(float degree)
    {
        return (degree * Mathf.PI / 180.0f);
    }

    public static Vector3 GetPositionByPositionInfo(PositionInfo positionInfo)
    {
        //float latTo = (float)positionInfo.Latitude;
        //float lngTo = (float)positionInfo.Longitude;


        //float R = 6371; // km
        //float dLat = ToRadian(latTo - ORTHO_LATITUDE_CENTER_DEG);
        //float dLon = ToRadian(lngTo - ORTHO_LONGITUDE_CENTER_DEG);
        //float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
        //        Mathf.Cos(ToRadian(ORTHO_LATITUDE_CENTER_DEG)) * Mathf.Cos(ToRadian(latTo)) *
        //        Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        //float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        //float d = R * c;


        //float bearing = Mathf.Atan2(Mathf.Cos(ORTHO_LATITUDE_CENTER_DEG) * Mathf.Sin(latTo) - Mathf.Sin(ORTHO_LATITUDE_CENTER_DEG) * Mathf.Cos(latTo) * Mathf.Cos(lngTo - ORTHO_LONGITUDE_CENTER_DEG),
        //                Mathf.Sin(lngTo - ORTHO_LONGITUDE_CENTER_DEG) * Mathf.Cos(latTo));

        //float x = d * Mathf.Cos(bearing);
        //float z = d * Mathf.Sin(bearing);
        if ( GameManager.Instance.Origin != null )
        {
            float distanceLat = Mathf.Abs(( float ) positionInfo.Latitude - GameManager.Instance.Origin.Latitude);
            float distanceLng = Mathf.Abs(( float ) positionInfo.Longitude - GameManager.Instance.Origin.Longitude);

            if ( distanceLat < 1 && distanceLng < 1 )
            {
                //calculate distance and bearing
                //Coordinate origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                Coordinate position = new Coordinate(( float ) positionInfo.Latitude, ( float ) positionInfo.Longitude);

                Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                Vector3 worldPos = new Vector3(coord.Longitude, ( float ) positionInfo.HeightOverSeaLevelM, coord.Latitude);

                //Debug.Log(worldPos);
                //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                return worldPos;
            }
            else
            {
                //set to repository
                return GameManager.Instance.UnitRepository;
            }
        }
        return Vector3.zero;
    }

    public static Vector3 GetVectorByLatLng(float lat, float lng, float height, float bearing)
    {
        //lat *= Mathf.PI / 180.0f;
        //lng *= Mathf.PI / 180.0f;
        //Vector3 pos = Vector3.zero;
        //pos.z = Mathf.Sin(-lat);
        //pos.x = Mathf.Cos(lat) * Mathf.Sin(-lng);
        //pos.y = Mathf.Cos(lat) * Mathf.Cos(-lng);

        //Vector3 pos2d = GameManager.Instance.CameraManager.MainCamera.WorldToScreenPoint(pos) * GameManager.Instance.UnitManager.x3DMulti;

        return Vector3.zero;
    }

    public static float CalculateDistanceUnit(Coordinate coordinate1, Coordinate coordinate2, float unitLength)
    {
        float theta = coordinate1.Longitude - coordinate2.Longitude;
        float distance = Mathf.Sin(coordinate1.Latitude * Mathf.Deg2Rad) * Mathf.Sin(coordinate2.Latitude * Mathf.Deg2Rad) +
                       Mathf.Cos(coordinate1.Latitude * Mathf.Deg2Rad) * Mathf.Cos(coordinate2.Latitude * Mathf.Deg2Rad) *
                       Mathf.Cos(theta * Mathf.Deg2Rad);

        distance = Mathf.Acos(distance);
        distance = ToDegreeBearing(distance);
        distance = distance * 60 * 1.1515f;

        
        return distance * unitLength;

    }

    public static float CalculateDistanceInKm(Coordinate coord1, Coordinate coord2, float unitLength)
    {
        float R = RADIUS_OF_EARTH_KM; // km

        float dLat = (coord2.Latitude - coord1.Latitude)* Mathf.Deg2Rad;
        float dLon = (coord2.Longitude - coord1.Longitude)* Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                Mathf.Cos(coord1.Latitude* Mathf.Deg2Rad) * Mathf.Cos(coord2.Latitude * Mathf.Deg2Rad) *
                Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c;

        return d * unitLength;
    }

    public static float CalculateBearingDegrees(Coordinate coordinate1, Coordinate coordinate2)
    {
        float latitude1 = coordinate1.Latitude * Mathf.Deg2Rad;
        float latitude2 = coordinate2.Latitude * Mathf.Deg2Rad;

        float longitudeDifference = coordinate2.Longitude * Mathf.Deg2Rad - coordinate1.Longitude * Mathf.Deg2Rad;

        float y = Mathf.Sin(longitudeDifference) * Mathf.Cos(latitude2);
        float x = Mathf.Cos(latitude1) * Mathf.Sin(latitude2) -
                Mathf.Sin(latitude1) * Mathf.Cos(latitude2) *
                Mathf.Cos(longitudeDifference);
        float result = ToDegreeBearing(Mathf.Atan2(y, x));
        return (result + 360) % 360;
    }

    public static float CalculateBearing(Coordinate coordinate1, Coordinate coordinate2)
    {
        return Mathf.Atan2(Mathf.Cos(coordinate1.Latitude) * Mathf.Sin(coordinate2.Latitude) -
                             Mathf.Sin(coordinate1.Latitude) * Mathf.Cos(coordinate2.Latitude) *
                             Mathf.Cos(coordinate2.Longitude - coordinate1.Longitude),
                             Mathf.Sin(coordinate2.Longitude - coordinate1.Longitude) *
                             Mathf.Cos(coordinate2.Latitude)) * Mathf.Rad2Deg;

    }

    public static Coordinate CalculateCoordinateFromBearingAndDistance(Coordinate coordinate1, Coordinate coordinate2)
    {
        float bearing = CalculateBearing(coordinate1, coordinate2);
        //float distance = CalculateDistanceUnit(coordinate1, coordinate2, MILES_TO_METERS);
        float distance = CalculateDistanceInKm(coordinate1, coordinate2, 1000);
        distance *= GameManager.Instance.GlobalScale.x;
        //Debug.Log(string.Format("bearing : {0} - - - distance : {1}", bearing, distance));
         
        return new Coordinate(distance * Mathf.Sin(bearing * Mathf.PI / 180), (distance * Mathf.Cos(bearing * Mathf.PI / 180)) * -1);

    }
    public static float ToDegreeBearing(float radian)
    {
        return (radian * Mathf.Rad2Deg + 360) % 360;
    }

    

    #endregion
}

