using System;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class SensorClass: IMarshallable, IGameDataObject
    {
        public SensorClass()
        {
            MinimumTargetSurfaceSizeArcSec = 60; //human eye
            MinimumTargetAirSizeArcSec = 30;
            EsmDetectionOverHorizonPercent = 100; //default
            AESAfactorPercent = 100;
            IdentifyDetectionStrength = GameConstants.DEFAULT_IDENTIFY_DETECTION_STRENGTH;
            SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF;
        }

        private string _sensorClassName;

        #region "Public properties"

        public string Id { get; set; }

        public string SensorClassName 
        { 
            get
            {
                return _sensorClassName;
            }
            set
            {
                _sensorClassName = value;
                if (string.IsNullOrEmpty(this.SensorClassShortName))
                {
                    SensorClassShortName = _sensorClassName;
                }
            }
        }

        public string SensorClassShortName { get; set; }

        public GameConstants.SensorType SensorType { get; set; }

        public GameConstants.SonarType SonarType { get; set; }

        public bool CanTargetAir { get; set; }

        public bool CanTargetSurface { get; set; }
        
        public bool CanTargetSubmarine { get; set; }

        /// <summary>
        /// The detection strength where a target is identified.
        /// </summary>
        public double IdentifyDetectionStrength { get; set; }

        /// <summary>
        /// The number of seconds taken to deploy (set operational) this sensor.
        /// </summary>
        public double TimeToDeploySec { get; set; }

        /// <summary>
        /// The maximum speed of the platform where this sensor can be deployed and used. 0 means no limit.
        /// </summary>
        public double MaxSpeedDeployedKph { get; set; }

        /// <summary>
        /// The highest altitude from which this sensor can be used. 0 means no limit.
        /// </summary>
        public double MaxHeightDeployedM { get; set; }

        /// <summary>
        /// ESM is Electromagnetic Support Measures. All radars and radio-based communications emit radiation, 
        /// which can be detected by sensor which has this setting set to true.
        /// </summary>
        public bool IsEsmDetector { get; set; }

        /// <summary>
        /// For AESA (Active Electronically Scanned Array) radar, active emissions are camouflaged and may not be detected
        /// at longer range. 100 is default (not AESA). 50 would mean radar emissions are only detected at 2x the strenght
        /// of normal, etc.
        /// </summary>
        public double AESAfactorPercent { get; set; }

        /// <summary>
        /// All ESM detectors can see radiation within the line of sight. More advanced sensors can also see 
        /// over the horizon. 100% (default) means it can not see beyond horizon, 150 can detect 
        /// radiation from 150% of the distance to the horizon, etc.
        /// </summary>
        public double EsmDetectionOverHorizonPercent { get; set; }

        /// <summary>
        /// Whether this sensor is a seeker radar/sonar for missile or torpedo. If true, 
        /// detection information is *not* communicated to player as a target detection.
        /// </summary>
        public bool IsTargetingSensorOnly { get; set; }

        /// <summary>
        /// If true, this sensor can be set to active and passive.
        /// </summary>
        public bool IsPassiveActiveSensor { get; set; }

        /// <summary>
        /// If true, this sensor must be actively deployed, like a dipping sonar or
        /// towed sonar array. Otherwise, it will always be deployed. The setting of
        /// IsOperational determines whether this sensor is deployed.
        /// </summary>
        public bool IsDeployableSensor { get; set; }

        /// <summary>
        /// Applicable to sonar. If true, this sensor can be deployed to both shallow and intermediate 
        /// depth. Otherwise the sonar will operate in shallow depth (surface or air), or if on a sub,
        /// at the sub's depth.
        /// </summary>
        public bool IsVariableDepthSensor { get; set; }

        /// <summary>
        /// Radiated power in decibel Watts, for emission detectable by other sensors. 
        /// Ex: Transmitter power of 15,000 W corresponds to 10·log10(15000) = 41.8 dBW. 
        /// Not currently used!
        /// </summary>
        public double EffectiveRadiatedPower_dBW 
        { 
            get 
            {
                return 10 * Math.Log10(TransmitterPowerW);
            } 
        }

        /// <summary>
        /// Transmitter power in Watts. Not currently used.
        /// </summary>
        public double TransmitterPowerW { get; set; }

        /// <summary>
        /// If true, this sensor will only report bearing (direction) to sensor, not its actual position.
        /// Typical for passive sonars. Detection will then be reported as a bearing and a range from 
        /// sensor platform to max (?) detection range.
        /// </summary>
        public bool IsBearingDetectorOnly { get; set; }

        /// <summary>
        /// Max range in meters where this sensor can possibly detect a target.
        /// </summary>
        public double MaxRangeM { get; set; }

        /// <summary>
        /// For sonars, the range at which this sensor will (barely) passively detect a QUIET target in
        /// otherwise ideal circumstances.
        /// </summary>
        public double SonarPassiveReferenceRangeM { get; set; }

        /// <summary>
        /// For sonars, the range at which this active sensor will detect a non-stealthy target in
        /// otherwise ideal circumstances.
        /// </summary>
        public double SonarActiveReferenceRangeM { get; set; }
        /// <summary>
        /// For sonars: The frequency band(s) where a this sonar operates
        /// </summary>
        public GameConstants.SonarFrequencyBands SonarFrequencyBand { get; set; }

        /// <summary>
        /// The smallest target on the surface this sensor can see, measured in ArcSec. The smallest object 
        /// (which does not itself radiate light) that can be seen by the human eye is around one
        /// arc minute, ie 60 arc seconds. 60 is the default value of this setting.
        /// </summary>
        public double MinimumTargetSurfaceSizeArcSec { get; set; }

        /// <summary>
        /// The smallest target in the air this sensor can see, measured in ArcSec. The smallest object 
        /// (which does not itself radiate light) that can be seen by the human eye is around one
        /// arc minute, ie 60 arc seconds. 60 is the default value of this setting.
        /// </summary>
        public double MinimumTargetAirSizeArcSec { get; set; }

        /// <summary>
        /// Range of sensor, relative to SensorBearingDeg. 360 means it can see in any
        /// direction. E.g. 30 means 15 degrees to each side of sensor bearing (set in BaseSensor).
        /// </summary>
        public virtual double SensorBearingRangeDeg { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(SensorClassName))
            {
                return  Id + ": " + SensorClassName;
            }
            else
            {
                return "Unnamed SensorClass " + Id;
            }

        }

        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.SensorClass; }
        }

        #endregion
    }
}
