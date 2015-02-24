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

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class DetectedUnitSensor : GameObject
    {
        #region "Constructors"

        public DetectedUnitSensor() : base()
        {
        }

        #endregion


        #region "Public properties"

        public DetectedUnit DetectedUnit { get; set; }

        public BaseUnit DetectionPlatform { get; set; }

        public BaseSensor DetectionSensor { get; set; }

        public bool IsBearingOnly { get; set; }

        public double UncertaintyRangeM { get; set; }

        public double UncertaintyRangeDeg { get; set; }

        public double DistanceDetectorToTargetM { get; set; }

        public double BearingDetectorToTargetDeg { get; set; }

        public double DetectedWorldGameTimeSec { get; set; }

        public DateTime DetectedGameTime
        {
            get
            {
                return GameManager.Instance.Game.GameStartTime.AddSeconds(
                    GameManager.Instance.Game.GameWorldTimeSec);
            }
        }

        public double ApparentSizeArcSec { get; set; }

        public double DetectionStrength { get; set; }

        #endregion



        #region "Public methods"

        public DetecedUnitSensorInfo GetDetectedUnitSensorInfo()
        {
            DetecedUnitSensorInfo info = new DetecedUnitSensorInfo();
            info.Id = Id;
            info.DetectedGameWorldTimeSec = DetectedWorldGameTimeSec;
            info.PlatformId = DetectionPlatform.Id;
            info.PlatformName = DetectionPlatform.Name;
            info.BearingToTargetDeg = BearingDetectorToTargetDeg;
            info.DistanceToTargetM = DistanceDetectorToTargetM;
            info.SensorId = DetectionSensor.Id;
            info.SensorName = DetectionSensor.Name;
            info.SensorDescription = DetectionSensor.Description;

            return info;

        }

        public override string ToString()
        {
            return string.Format("Sensor [{0}] {1} on unit {2} [{3}]: Distance {4:F}m at {5:F}deg. Strength: {6:F}. Size: {7:F} ArcSec",
                this.DetectionSensor.Id, this.DetectionSensor.Name, 
                this.DetectionPlatform.Name, this.DetectionPlatform.UnitClass.UnitClassShortName,
                this.DistanceDetectorToTargetM, this.BearingDetectorToTargetDeg, 
                this.DetectionStrength, this.ApparentSizeArcSec);
        }

        #endregion


    }
}
