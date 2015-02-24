using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class DetectedUnitInfo : IMarshallable
    {
        #region "Constructors"

        public DetectedUnitInfo()
        {
            DetectionSensors = new List<DetecedUnitSensorInfo>();
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public GameConstants.DetectionClassification DetectionClassification { get; set; }

        public GameConstants.ThreatClassification ThreatClassification { get; set; }

        public GameConstants.FriendOrFoe FriendOrFoeClassification { get; set; }

        public string RefersToUnitName { get; set; }

        public string DetectedGroupId { get; set; }

        public string RefersToUnitClassId { get; set; }

        //public string DetectedUnitDescription { get; set; }

        public double DetectedGameTimeMs { get; set; }

        public PositionInfo Position { get; set; }

        public RegionInfo PositionRegion { get; set; }

        public bool IsFixed { get; set; }

        public bool IsIdentified { get; set; }

        public bool IsKnownToUseActiveRadar { get; set; }

        public bool IsKnownToUseActiveSonar { get; set; }

        public bool IsKnownToBeCivilianUnit { get; set; }

        public bool IsBearingOnly { get; set; }

        public bool IsFiredUpon { get; set; }

        public bool IsTargetted { get; set; }

        public string OwnerPlayerId { get; set; }

        public string OwnerPlayerName { get; set; }

        public int KnownDamagePercent { get; set; }

        public GameConstants.FireLevel KnownFireLevel { get; set; }

        public bool HasLightingOn { get; set; }

        public List<DetecedUnitSensorInfo> DetectionSensors { get; set; }

        public double UncertaintyRangeM { get; set; }

        //public string DetectionSensorId { get; set; }

        //public string DetectionPlatformId { get; set; }

        //public double DistanceDetectorToTargetM { get; set; }

        //public double BearingDetectorToTargetDeg { get; set; }

        public GameConstants.DomainType DomainType { get; set; }

        public WeatherSystemInfo WeatherSystemInfo { get; set; }
        
        #endregion



        #region "Public methods"

        public override string ToString()
        {
            if (Position == null)
            {
                return "Invalid contact " + Id;
            }
            string Info = string.Empty;
            if (IsIdentified)
            {
                Info = "[" + Id + "] " + RefersToUnitName + " (" + RefersToUnitClassId + ")";
            }
            else
            {
                Info = "[" + Id + "] " + DetectionClassification.ToString();
            }
            if (this.FriendOrFoeClassification != GameConstants.FriendOrFoe.Undetermined)
            {
                Info += " [" + this.FriendOrFoeClassification.ToString() + "]";
            }
            else
            {
                Info += " [FoF Undetermined]";
            }

            Info += "  @ " + Position.ToString();
            if (KnownDamagePercent > 0)
            {
                Info += string.Format(" dmg: {0:F}%", KnownDamagePercent);
            }
            return Info;
        }

        public string ToLongString()
        {
            string Info = ToString() + "\n";
            if (IsFixed)
            {
                Info += "Fixed. ";
            }
            if (IsIdentified)
            {
                Info += "Identified.";
            }
            Info += "\n";
            if (!string.IsNullOrEmpty(OwnerPlayerId))
            {
                Info += "Owner: " + OwnerPlayerName + "\n";
            }
            
            if (KnownFireLevel != GameConstants.FireLevel.NoFire)
            {
                Info += "Fire: " + KnownFireLevel + "  ";
            }
            Info += "\nSensors: (" + this.DetectionSensors.Count + ")\n";
            if (this.DetectionSensors.Count == 0)
            {
                Info += "Intelligence\n";
            }
            else
            {
                int maxCount = 3;
                int currentCount = 0;
                foreach (var sensor in this.DetectionSensors)
                {
                    Info += sensor.ToString() + "\n";
                    currentCount++;
                    if (currentCount >= maxCount)
                    {
                        break;
                    }
                }
            }
            return Info;
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.DetectedUnitInfo; }
        }

        #endregion
    }
}
