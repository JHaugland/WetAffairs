using System;
using System.Collections.Generic;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitClass :IMarshallable, IGameDataObject
    {
        private List<UnitClassWeaponLoads> _WeaponLoads = new List<UnitClassWeaponLoads>();
        private List<GameConstants.Role> _UnitRoles = new List<GameConstants.Role>();

        private float _UnitModelScaleFactor = 1.0f;

        //UnitClass is the production type of unit, like "F-22 Raptor" or "Ashleigh Burke"

        #region "Constructors"
        public UnitClass() : base()
        {
            UnitModelScaleFactor = 1.0f;
            AgilityFactor = 1;
            VesselNames = new List<UnitClassVesselName>();
            SensorClassIdList = new List<string>();
            CarriedUnitClassses = new List<UnitClassStorage>();
            MaxSimultanousTakeoffs = 1;
            NoiseLevel = GameConstants.SonarNoiseLevel.Noisy;
        }

        public UnitClass(string unitClassCode, GameConstants.UnitType unitType)
            : this()
        {
            Id = unitClassCode;
            UnitType = unitType;
        }
        #endregion

        #region "Public properties"

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Short name for this UnitClass
        /// </summary>
        public string UnitClassShortName { get; set; }

        public string UnitClassLongName { get; set; }

        public string UnitModelFileName { get; set; }

        public float UnitModelScaleFactor 
        { 
            get
            {
                if (_UnitModelScaleFactor == 0)
                {
                    _UnitModelScaleFactor = 1;
                }
                return _UnitModelScaleFactor;
            }
            set
            {
                _UnitModelScaleFactor = value;
            }
        }

        public string UnitIconSatMap { get; set; }

        public string UnitIconSatMapGroup { get; set; }

        public Vector2 UnitIconSatMapSize { get; set; }

        public string UnitIcon3D { get; set; }

        public string UnitIconSide { get; set; }

        public GameConstants.UnitType UnitType { get; set; }

        public GameConstants.DetectionClassification DetectionClassification { get; set; }

        public GameConstants.RadarCrossSectionSize RadarCrossSectionSize { get; set; }

        /// <summary>
        /// The unit's ability to evade and avoid weapon fire. If higher that weapon's ReferenceAgilityFactor,
        /// the chance of weapon hit is reduced dramatically. If difference is >= 2, weapon can't target at all. Default is 1.
        /// </summary>
        public byte AgilityFactor { get; set; }

        public bool IsEsmShielded { get; set; }

        public bool IsIrShielded { get; set; }

        /// <summary>
        /// Coating for submarines - reduces detection range for ACTIVE sonar
        /// </summary>
        public bool IsSonarShielded { get; set; }

        public GameConstants.SonarNoiseLevel NoiseLevel { get; set; }

        public GameConstants.PropulsionSystem PropulsionSystem { get; set; }

        public string CountryId { get; set; }

        //internal Country Country { get; set; } //TODO: Get from Id

        /// <summary>
        /// For units with limited lifespans, like sonobuoys. Will self-destruct after set time in seconds. Ignored
        /// if 0.
        /// </summary>
        public int TimeToLiveSec { get; set; }
        
        //see http://www.gcblue2.com/wiki/index.php?title=Game_database

        public bool IsFixed { get; set; }  //Unmovable

        public bool IsLandbased { get; set; }

        public bool IsAircraft { get; set; }

        public bool IsSurfaceShip { get; set; }

        public bool IsSubmarine { get; set; }

        public bool IsMissileOrTorpedo { get; set; }

        public bool IsBallistic { get; set; }

        public bool IsAlwaysVisibleForEnemy { get; set; } //for land installations like airfields and ports

        public bool CanBeTargeted { get; set; }

        public bool IsDecoy { get; set; }

        public double TotalMassLoadedKg { get; set; }

        public double TotalMassEmptyKg { get; set; }

        public double LengthM { get; set; }

        public double WidthM { get; set; }

        public double HeightM { get; set; }

        public double DraftM { get; set; }

        public double MaxSpeedKph { get; set; }

        public double CruiseSpeedKph { get; set; }

        /// <summary>
        /// If MilitaryMaxSpeedKph is less than MaxSpeedKph it follows that the
        /// unit has afterburner.
        /// </summary>
        public double MilitaryMaxSpeedKph { get; set; }

        public List<UnitClassVesselName> VesselNames { get; set; }

        public List<UnitClassWeaponLoads> WeaponLoads
        {
            get
            {
                return _WeaponLoads;
            }
            set
            {
                _WeaponLoads = value;
            }
        }

        /// <summary>
        /// List of SensorClassID references.
        /// </summary>
        public List<string> SensorClassIdList { get; set; }

        public List<GameConstants.Role> UnitRoles
        {
            get
            {
                return _UnitRoles;
            }
            set
            {
                _UnitRoles = value;
            }
        }
        /// <summary>
        /// Minimum speed in kph. Relevant for airplanes (effectively stall speed), otherwise 0.
        /// </summary>
        public double MinSpeedKph { get; set; }

        public double MaxAccelerationKphSec { get; set; }

        /// <summary>
        /// Expects a negative number
        /// </summary>
        public double MaxDecelerationKphSec { get; set; }

        /// <summary>
        /// Maximum range in meters at cruise speed. This is (normally) only used for 
        /// aircraft, missiles and torpedoes. When exhausted, munitions disappear and
        /// aircraft crashes.
        /// </summary>
        public double MaxRangeCruiseM { get; set; }

        //public virtual double MaxRangeMaxM { get; set; }

        public double MaxClimbrateMSec { get; set; }

        public double MaxFallMSec { get; set; }

        public double TurnRangeDegrSec { get; set; }

        public double HighestOperatingHeightM { get; set; }

        /// <summary>
        /// Lowest operating height (ie depth) in meters. For anything but a submarine, 0.
        /// Note that since this value is the height over sea level, it should be a negative number 
        /// for subs.
        /// </summary>
        public double LowestOperatingHeightM { get; set; } //

        /// <summary>
        /// Relevant for aircraft, set true for those that can be refueled in-air (by tanker aircraft)
        /// </summary>
        public bool CanRefuelInAir { get; set; }

        public bool CanCarryUnits 
        { 
            get
            {
                return this.MaxCarriedUnitsTotal > 0;
            }
        }

        /// <summary>
        /// The runway style this unit has (on aircrafthangar)
        /// </summary>
        public GameConstants.RunwayStyle CarriedRunwayStyle { get; set; }

        /// <summary>
        /// For aircraft, the required runway this unit needs to land and take off
        /// on another unit.
        /// </summary>
        public GameConstants.RunwayStyle RequiredRunwayStyle { get; set; }
        //Storage,
        //TankingAndRefitting,
        //ReadyForTakeoff,
        //public virtual int MaxCarriedUnitsStorage { get; set; }

        //public virtual int MaxCarriedUnitsRefitting { get; set; }

        //public virtual int MaxCarriedUnitsReadyForTakeoff { get; set; }

        public int MaxCarriedUnitsTotal { get; set; }

        public int MaxSimultanousTakeoffs { get; set; }

        public List<UnitClassStorage> CarriedUnitClassses { get; set; }

        public int CrewTotal { get; set; }

        public int MaxHitpoints { get; set; }

        /// <summary>
        /// The effective sea state for a unit is the actual sea state with StabilityBonus
        /// deducted.
        /// </summary>
        public int StabilityBonus { get; set; }

        public double EstimatedUnitPriceMillionUSD { get; set; }

        public int AcquisitionCostCredits { get; set; }

        public GameConstants.DomainType DomainType
        {
            get
            {
                switch ( this.UnitType )
                {
                    case GameConstants.UnitType.SurfaceShip:
                        return GameConstants.DomainType.Surface;
                    case GameConstants.UnitType.FixedwingAircraft:
                    case GameConstants.UnitType.Helicopter:
                    case GameConstants.UnitType.Missile:
                    case GameConstants.UnitType.BallisticProjectile:
                    case GameConstants.UnitType.Bomb:
                        return GameConstants.DomainType.Air;

                    case GameConstants.UnitType.Submarine:
                    case GameConstants.UnitType.Torpedo:
                    case GameConstants.UnitType.Mine:
                    case GameConstants.UnitType.Sonobuoy:
                        return GameConstants.DomainType.Subsea;
                    case GameConstants.UnitType.Decoy:
                        return GameConstants.DomainType.Unknown; //TODO: Resolve what type of decoy it attempts to be
                    case GameConstants.UnitType.LandInstallation:
                        return GameConstants.DomainType.Land;
                    default:
                        return GameConstants.DomainType.Unknown;
                }
            }
        }

        /// <summary>
        /// Returns a List of UnitSpeedType for this unit class, outlining all possible speed types
        /// available.
        /// </summary>
        /// <returns></returns>
        public List<GameConstants.UnitSpeedType> GetPossibleSpeedTypes()
        {
            List<GameConstants.UnitSpeedType> list = new List<GameConstants.UnitSpeedType>();
            double maxSpeedKph = this.MaxSpeedKph;
            if ( maxSpeedKph == 0 || this.DomainType == GameConstants.DomainType.Land )
            {
                return list;
            }
            if ( this.MinSpeedKph == 0 )
            {
                list.Add( GameConstants.UnitSpeedType.Stop );
            }
            if ( this.DomainType != GameConstants.DomainType.Air )
            {
                list.Add( GameConstants.UnitSpeedType.Crawl );
            }
            list.Add( GameConstants.UnitSpeedType.Slow );
            list.Add( GameConstants.UnitSpeedType.Half );
            list.Add( GameConstants.UnitSpeedType.Cruise );
            if ( this.MaxSpeedKph > this.CruiseSpeedKph )
            {
                list.Add( GameConstants.UnitSpeedType.Military );
            }

            if ( this.MaxSpeedKph > this.MilitaryMaxSpeedKph && this.DomainType == GameConstants.DomainType.Air )
            {
                list.Add( GameConstants.UnitSpeedType.Afterburner );
            }
            return list;
        }

        /// <summary>
        /// Returns a list of HeightDepthPoints for this unit class, specifying all heights and depths
        /// this unit can be moved to.
        /// </summary>
        /// <returns></returns>
        public List<GameConstants.HeightDepthPoints> GetPossibleHeightDepth()
        {
            List<GameConstants.HeightDepthPoints> list = new List<GameConstants.HeightDepthPoints>();
            double minHeightM = this.LowestOperatingHeightM;
            double maxHeightM = this.HighestOperatingHeightM;
            if ( minHeightM < 0 )
            {
                if ( minHeightM <= GameConstants.DEPTH_SURFACE_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.MaxDepth );
                }
                if ( minHeightM <= GameConstants.DEPTH_VERY_DEEP_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.VeryDeep );
                }
                if ( minHeightM <= GameConstants.DEPTH_DEEP_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.Deep );
                }
                if ( minHeightM <= GameConstants.DEPTH_MEDIUM_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.MediumDepth );
                }
                if ( minHeightM < GameConstants.DEPTH_SHALLOW_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.ShallowDepth );
                }
                if ( minHeightM < GameConstants.DEPTH_PERISCOPE_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.PeriscopeDepth );
                }
                if ( maxHeightM >= 0 )
                {
                    list.Add( GameConstants.HeightDepthPoints.Surface );
                }
            }
            if ( maxHeightM == minHeightM )
            {
                list.Add( GameConstants.HeightDepthPoints.Surface );
            }
            else
            {
                if (maxHeightM > 0) //GameConstants.HEIGHT_VERY_HIGH_MIN_M
                {
                    list.Add( GameConstants.HeightDepthPoints.MaxHeight );
                }
                if ( maxHeightM >= GameConstants.HEIGHT_VERY_HIGH_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.VeryHigh );
                }
                if ( maxHeightM >= GameConstants.HEIGHT_HIGH_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.High );
                }
                if ( maxHeightM >= GameConstants.HEIGHT_MEDIUM_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.MediumHeight );
                }
                if ( maxHeightM >= GameConstants.HEIGHT_LOW_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.Low );
                }
                if ( maxHeightM >= GameConstants.HEIGHT_VERY_LOW_MIN_M && minHeightM <= GameConstants.HEIGHT_VERY_LOW_MIN_M )
                {
                    list.Add( GameConstants.HeightDepthPoints.VeryLow );
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the speed type for this unit class given an actual speed in kph.
        /// </summary>
        /// <param name="speedKph"></param>
        /// <returns></returns>
        public GameConstants.UnitSpeedType GetSpeedTypeFromKph(double speedKph)
        {
            GameConstants.UnitSpeedType SpeedType = GameConstants.UnitSpeedType.Stop;
            if (speedKph > 0)
            {
                SpeedType = GameConstants.UnitSpeedType.Crawl;
            }
            if (speedKph > GameConstants.DEFAULT_CRAWL_SPEED && speedKph <= GameConstants.DEFAULT_SLOW_SPEED)
            {
                SpeedType = GameConstants.UnitSpeedType.Slow;
            }
            else if (speedKph > GameConstants.DEFAULT_CRAWL_SPEED)
            {
                SpeedType = GameConstants.UnitSpeedType.Cruise;
                if (speedKph < this.CruiseSpeedKph * 0.9)
                {
                    SpeedType = GameConstants.UnitSpeedType.Half;
                }
            }
            if (speedKph > this.CruiseSpeedKph)
            {
                SpeedType = GameConstants.UnitSpeedType.Military;
            }
            if (this.MaxSpeedKph > this.MilitaryMaxSpeedKph && speedKph > this.MilitaryMaxSpeedKph) // has afterburners
            {
                SpeedType = GameConstants.UnitSpeedType.Afterburner;
            }
            return SpeedType;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(UnitClassShortName))
            {
                return UnitClassShortName;
            }
            else
            {
                return "Unnamed UnitClass";
            }
            
        }

        #endregion

        #region "Public methods"

        public UnitClass Clone()
        {
            var serhelper = new TTG.NavalWar.NWComms.CommsSerializationHelper<UnitClass>();
            byte[] bytes = serhelper.SerializeToBytes(this);

            UnitClass newUnitClass = serhelper.DeserializeFromBytes(bytes);
            return newUnitClass;
        }
        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.UnitClass; }
        }

        #endregion


    }
}
