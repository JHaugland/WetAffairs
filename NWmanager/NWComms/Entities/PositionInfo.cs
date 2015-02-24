using System;
using System.Collections.Generic;

using System.Text;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class PositionInfo : IMarshallable
    {
        #region "Private constants, enums and fields"
        #endregion

        #region "Constructors"
        
        public PositionInfo()
        {

        }

        public PositionInfo(double latitude, double longitude)
            : this()
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public PositionInfo(double latitude, double longitude, float heightM)
            : this()
        {
            Longitude = longitude;
            Latitude = latitude;
            HeightOverSeaLevelM = heightM;
        }

        public PositionInfo(double latitude, double longitude, float heigthM, float bearingDeg)
            : this()
        {
            Longitude = longitude;
            Latitude = latitude;
            BearingDeg = bearingDeg;
            HeightOverSeaLevelM = heigthM;
        }

        public PositionInfo(string formattedCoordinate, float heigthM, float bearingDeg)
            : this()
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
                Latitude = Lat;
                Longitude = Lon;
                BearingDeg = bearingDeg;
                HeightOverSeaLevelM = heigthM;
            }
            catch (Exception ex)
            {
                throw new Exception("Must contain six valid double values separated by space.", ex);
            }
        }
  
        #endregion


        #region "Public properties"

        public string UnitId { get; set; }

        /// <summary>
        /// If true, this PositionInfo refers to a DetectedUnit, not a BaseUnit. Otherwise a (own) unit.
        /// </summary>
        public bool IsDetection { get; set; }

        public double Latitude { get; set; }
        
        public double Longitude { get; set; }

        //public double DestinationLatitude { get; set; }

        //public double DestinationLongitude { get; set; }

        public double LatitudeOrthoProjected { get; set; }

        public double LongitudeOrthoProjected { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool HasHeightOverSeaLevel { get; set; }

        private float _HeightOverSeaLevelM;
        public float HeightOverSeaLevelM
        {
            get
            {
                return _HeightOverSeaLevelM;
            }
            set
            {
                _HeightOverSeaLevelM = value;
                this.HasHeightOverSeaLevel = true;
            }
        }

        public GameConstants.HeightDepthPoints HeightDepth { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool HasBearing { get; set; }

        private float _BearingDeg;
        public float BearingDeg
        {
            get
            {
                return _BearingDeg;
            }
            set
            {
                _BearingDeg = value;
                this.HasBearing = true;
            }
        }

        //public float DesiredBearingDeg { get; set; }

        //public float BearingToTargetPosDeg { get; set; }

        //public float DistanceToTargetPosM { get; set; }

        //public bool IsFormationMovementOrder { get; set; }

        public bool IsAtFormationPosition { get; set; }

        public int BingoFuelPercent { get; set; }

        public float FuelDistanceRemainingM { get; set; } //will be 0.0 for units with no MaxRange

        //public GameConstants.DirectionCardinalPoints Bearing { get; set; }

        public float ActualSpeedKph { get; set; }

        //public GameConstants.UnitSpeedType ActualSpeed { get; set; }

        //public float MostRecentDistanceToTargetM { get; set; }

        //public string TargetDetectedUnitId { get; set; }

        //public string ReturningToUnitId { get; set; }

        public float EtaCurrentWaypointSec { get; set; }

        public TimeSpan EtaCurrentWaypoint
        {
            get
            {
                if (EtaCurrentWaypointSec > 0)
                {
                    return TimeSpan.FromSeconds(EtaCurrentWaypointSec);
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        public float EtaAllWaypointsSec { get; set; }

        public TimeSpan EtaAllWaypoints
        {
            get
            {
                if (EtaAllWaypointsSec > 0)
                {
                    return TimeSpan.FromSeconds(EtaAllWaypointsSec);
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        //public string Description { get; set; }

        #endregion

        #region "Public methods"
        public string ToLongString()
        {
            //if (string.IsNullOrEmpty(Description))
            //{
            //    return string.Format("Lat:{0:F}, Lon:{1:F}", Latitude, Longitude);
            //}
            //else
            //{
            //    return Description;
            //}
            string lat = Latitude.ToDegMinSecString();
            lat += (Latitude < 0) ? "S" : "N";
            string lon = Longitude.ToDegMinSecString();
            lon += (Longitude > 0) ? "E" : "W";
            string temp = string.Format("{0}, {1} \nHeight: {2:F}m,  Bearing: {3:F}  Speed: {4:F}kph", 
                lat, lon, HeightOverSeaLevelM, BearingDeg, ActualSpeedKph);
            //if (DestinationLongitude != 0 && DestinationLatitude != 0)
            //{
            //    lat = DestinationLatitude.ToDegMinSecString();
            //    lat += (DestinationLatitude < 0) ? "S" : "N";
            //    lon = DestinationLongitude.ToDegMinSecString();
            //    lon += (DestinationLongitude > 0) ? "E" : "W";
            //    temp += string.Format("\nDest: {0}, {1}", lat, lon);
            //}
            return temp;
            
        }
        public override string ToString()
        {
            string lat = Latitude.ToDegMinSecString();
            lat += (Latitude < 0) ? "S" : "N";
            string lon = Longitude.ToDegMinSecString();
            lon += (Longitude > 0) ? "E" : "W";
            string temp = string.Format("{0}, {1}",
                lat, lon);
            return temp;
            
        }
        #endregion

        #region "Public static methods"
        /// <summary>
        /// Takes a string in the space separated format "LA MM SS LO MM SS" and returns a coordinate. Use negative
        /// numbers for southern latitudes and western longitudes.
        /// </summary>
        /// <param name="formattedCoordinate"></param>
        /// <returns></returns>
      #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.PositionInfo; }
        }

        #endregion
    }
}
