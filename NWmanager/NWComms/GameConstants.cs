using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms
{
    [Serializable]
    public class GameConstants
    {
        #region "Public constants"
        
        public const double TIME_DETECTION_CONTACT_LOST_SEC = 300;
        public const double TIME_DETECION_TARGETTED_TOO_OLD_SEC = 100;
        public const double TIME_BETWEEN_SENSOR_SWEEPS_SEC = 10;    // in game time
        public const double TIME_DEPLOY_SONOBUOY_SEC = 60;
        public const double TIME_BETWEEN_FORMATION_POSITION_CHECKS_SEC = 60;    // in game time
        public const double TIME_BETWEEN_TRIGGER_CHECKS_SEC = 33;
        public const double TIME_BETWEEN_WEATHER_SWEEPS_SEC = 300;  // in game time
        public const double MAX_MOVEMENT_TIME_PER_TICK_SEC = 1;
        public const double IDENTIFY_TARGET_SURFACE_SUB_M = 500; //hmm
        public const double IDENTIFY_TARGET_SUB_SUB_M = 250;
        public const double DISTANCE_TO_TARGET_IS_HIT_M = 50; 
        public const double DISTANCE_TO_FORMATION_POSITION_IS_THERE_M = 200;
        public const double MAX_MINE_IMPACT_RANGE_M = 250; //hmm: reduce?
        public const double MAX_MINE_DETECTION_RANGE_M = 500;
        public const double MINE_FIELD_DISTANCE_BETWEEN_MINES_M = 300;
        public const double BINGO_FUEL_FACTOR = 1.1;
        public const double DEFAULT_SLOW_SPEED = 10;
        public const double DEFAULT_CRAWL_SPEED = 5;
        public const double DEFAULT_AA_DEFENSE_RANGE_M = 200000;
        public const double DEFAULT_AA_INTERCEPT_RANGE_M = 1000000;
        public const double DEFAULT_ASu_ATTACK_RANGE_M = 350000;
        public const double DEFAULT_AA_ATTACK_RANGE_M = 100000;
        public const double DEFAULT_AIR_AA_STRIKE_RANGE_M = 75000; //used by AI to evaulate potency of air threats, based on amraam
        public const double DEFAULT_AIR_ASu_STRIKE_RANGE_M = 200000; 
        public const double DEFAULT_AIR_ASW_STRIKE_RANGE_M = 100000;
        public const double DEFAULT_CIWS_RANGE_M = 6000;
        public const double DEFAULT_TORPEDO_STRIKE_RANGE_M = 38000;
        public const double DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC = 60;
        public const double DEFAULT_IDENTIFY_DETECTION_STRENGTH = 10;

        public const double DEFAULT_NON_TERMINAL_SPEED_KPH = 1050;

        public const int DEFAULT_NO_OF_POINTS_TERRAIN_LINE = 10;
        public const int MAX_HEIGHT_TERRAIN_M = 3600;

        public const double MAX_DISTANCE_JOIN_M = 10000;
        public const double KPH_TO_MS_CONVERSION_FACTOR = 0.277777778;
        public const double MS_TO_KPH_CONVERSION_FACTOR = 3.6;
        public const double MAX_DETECTION_DISTANCE_M = 500000; //NOTE: No sensors in game will be permitted to reach further
        public const int AIRCRAFT_TAKEOFF_MAX_SEA_STATE = 6;
        public const int AIRCRAFT_LANDING_MAX_SEA_STATE = 6;

        public const int DEFAULT_DEPTH_THERMAL_LAYER_M = -50;

        //Minimum radar target sizes

        public const double RADAR_TARGET_SURFACE_EXCELLENT_ARC_SEC = 6;
        public const double RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC = 8;
        public const double RADAR_TARGET_SURFACE_STRONG_ARC_SEC = 10;
        public const double RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC = 12;
        public const double RADAR_TARGET_SURFACE_WEAK_ARC_SEC = 16;
        public const double RADAR_TARGET_SURFACE_VERY_WEAK_ARC_SEC = 20;

        public const double RADAR_TARGET_AIR_EXCELLENT_ARC_SEC = 6;
        public const double RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC = 6;
        public const double RADAR_TARGET_AIR_STRONG_ARC_SEC = 10;
        public const double RADAR_TARGET_AIR_MEDIUM_ARC_SEC = 12;
        public const double RADAR_TARGET_AIR_WEAK_ARC_SEC = 16;
        public const double RADAR_TARGET_AIR_VERY_WEAK_ARC_SEC = 20;

        //Turn rate defaults

        public const double TURN_RATE_SHIP_VERY_LARGE_DEG_SEC = 1;
        public const double TURN_RATE_SHIP_LARGE_DEG_SEC = 2;
        public const double TURN_RATE_SHIP_MED_DEG_SEC = 4;
        public const double TURN_RATE_SHIP_SMALL_DEG_SEC = 8;

        public const double TURN_RATE_MISSILES_DEG_SEC = 30;

        public const double TURN_RATE_HELO_DEG_SEC = 20;
        public const double TURN_RATE_AIR_AGILE_EXCELLENT_DEG_SEC = 30;
        public const double TURN_RATE_AIR_AGILE_VERY_GOOD_DEG_SEC = 24;
        public const double TURN_RATE_AIR_AGILE_GOOD_DEG_SEC = 18;
        public const double TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC = 12;
        public const double TURN_RATE_AIR_AGILE_LOW_DEG_SEC = 8;

        //Height and depth
        public const int DEFAULT_AMMO_COST = 10;
        public const int DEFAULT_UNIT_COST = 1000;
        public const double DEPTH_MAXIMUM_M = -5000;
        public const double DEPTH_VERY_DEEP_MIN_M = -1000;
        public const double DEPTH_DEEP_MIN_M = -700;
        public const double DEPTH_MEDIUM_MIN_M = -100;
        public const double DEPTH_SHALLOW_MIN_M = -50;
        public const double DEPTH_PERISCOPE_MIN_M = -10;
        public const double DEPTH_SURFACE_MIN_M = -1;
        public const double HEIGHT_VERY_LOW_MIN_M = 10;
        public const double HEIGHT_LOW_MIN_M = 100;
        public const double HEIGHT_MEDIUM_MIN_M = 2000;
        public const double HEIGHT_HIGH_MIN_M = 7500;
        public const double HEIGHT_VERY_HIGH_MIN_M = 10500;
        public const double HEIGHT_MAXIUMUM_M = 1000000;
        public const int DEFAULT_READY_TIME_SEC = 60 * 60;
        public const string FORMATION_SURFACE_DEFAULT = "sagdefault"; //these formations must exist in xml
        public const string FORMATION_AIRCRAFT_DEFAULT = "aircraftdefault";

        public const double AIRCRAFT_RANGE_INCREASE_FACTOR = 1.2; //assume 20% increase due to better technology

        public const string CHEAT_CODE_REVEAL_ORDERS = "reveal";
        public const string CHEAT_CODE_DETECT_ALL = "detectall";
        public const string CHEAT_CODE_REARM_UNITS = "rearmallunits";
        public const string CHEAT_CODE_READY_ALL_AIRCRAFT = "readyallaircraft";
        public const string CHEAT_CODE_GIVE_BADASS_AIRCRAFT = "givebadassaircraft";
        public const string CHEAT_CODE_START_FIRE = "startfire";
        public const string CHEAT_CODE_STOP_FIRE = "stopfire";
        public const string CHEAT_CODE_WIN_GAME = "wingame";
        public const string CHEAT_CODE_LOSE_GAME = "losegame";

        #endregion

        #region "Public static methods"
        
        /// <summary>
        /// Converts neutical miles to meters.
        /// </summary>
        /// <param name="nm"></param>
        /// <returns></returns>
        public static double ToMetersFromNmiles(double nm)
        {
            return nm * 1852.0;
        }

        public static double GetSonarReferenceRangeM (GameConstants.SonarStrengthTypes strengthType)
        {
            switch (strengthType)
            {
                case SonarStrengthTypes.PassiveTowedArrayHigh:
                    return ToMetersFromNmiles(12.0);
                case SonarStrengthTypes.PassiveTowedArrayLow:
                    return ToMetersFromNmiles(9.0);
                case SonarStrengthTypes.PassiveSonobuoy:
                    return ToMetersFromNmiles(6.0);
                case SonarStrengthTypes.PassiveExcellent:
                    return ToMetersFromNmiles(6.0);
                case SonarStrengthTypes.PassiveVeryGood:
                    return ToMetersFromNmiles(4.0);
                case SonarStrengthTypes.PassiveMedium:
                    return ToMetersFromNmiles(3.0);
                case SonarStrengthTypes.PassiveWeak:
                    return ToMetersFromNmiles(2.0);
                case SonarStrengthTypes.ActiveTowedArrayHigh:
                    return ToMetersFromNmiles(12.0);
                case SonarStrengthTypes.ActiveTowedArrayLow:
                    return ToMetersFromNmiles(9.0);
                case SonarStrengthTypes.ActiveSononbuoy:
                    return ToMetersFromNmiles(6.0);
                case SonarStrengthTypes.ActiveExcellent:
                    return ToMetersFromNmiles(6.0);
                case SonarStrengthTypes.ActiveVeryGood:
                    return ToMetersFromNmiles(5.0);
                case SonarStrengthTypes.ActiveMedium:
                    return ToMetersFromNmiles(3.0);
                case SonarStrengthTypes.ActiveWeak:
                    return ToMetersFromNmiles(2.0);
                default:
                    break;
            }
            return ToMetersFromNmiles(2.0);
        }

        /// <summary>
        /// This static method returns a PositionInfo based on the passed PresetGeographicalPositions enum. For simpler scenario development.
        /// </summary>
        /// <param name="presetPosition"></param>
        /// <returns></returns>
        public static PositionInfo GetPresetGeographicalPosition(GameConstants.PresetGeographicalPositions presetPosition)
        {
            switch (presetPosition)
            {
                case PresetGeographicalPositions.BergenFlesland:
                    return new PositionInfo("60 17 32 05 13 19", 0, 350);
                case PresetGeographicalPositions.Keflavik:
                    return new PositionInfo("64 01 00 -22 34 00", 0, 350);
                case PresetGeographicalPositions.RAFLossiemouth:
                    return new PositionInfo("57 42 19 -03 20 21", 0, 350);
                case PresetGeographicalPositions.OlenyaAirBase:
                    return new PositionInfo("68 09 06 33 28 12", 0, 350);
                case PresetGeographicalPositions.Andoya:
                    return new PositionInfo("69 17 33 16 08 39", 0, 350);
                case PresetGeographicalPositions.Bardufoss:
                    return new PositionInfo("69 03 22 18 32 25", 0, 350);
                case PresetGeographicalPositions.Gardermoen:
                    return new PositionInfo("60 11 38 11 06 02", 0, 350);
                case PresetGeographicalPositions.Banak:
                    return new PositionInfo("70 03 50 24 58 26", 0, 350);
                case PresetGeographicalPositions.Narvik:
                    return new PositionInfo(68.447662, 17.420197, 0, 350);
                case PresetGeographicalPositions.Stavanger:
                    return new PositionInfo(58.98346, 5.718384, 0, 350);
                case PresetGeographicalPositions.Orland:
                    return new PositionInfo(63.698908, 9.604003, 0, 350); //63.698908,9.604003
                case PresetGeographicalPositions.JanMayensfield:
                    return new PositionInfo("70 58 30 -08 35 59", 0, 350); //70° 57′ 40″ N, 8° 34′ 33″ W - tweaked for map
                case PresetGeographicalPositions.Bodo:
                    return new PositionInfo(67.269167,14.365278, 0, 350); //67.269167,14.365278
                case PresetGeographicalPositions.Skrydstrup: 
                    return new PositionInfo("55 13 9 9 16 4", 0, 350);
                case PresetGeographicalPositions.CopenhagenKastrup:
                    return new PositionInfo(55.628611, 12.646944, 0, 350);
                case PresetGeographicalPositions.Lulea:
                    return new PositionInfo("65 32 37 22 07 19", 0, 350);
                case PresetGeographicalPositions.StockholmArlanda:
                    return new PositionInfo(59.671944, 17.918611, 0, 350); //59.651944,17.918611
                case PresetGeographicalPositions.Ronneby:
                    return new PositionInfo("56 16 00 15 15 54", 0, 350);
                case PresetGeographicalPositions.Lidkoping:
                    return new PositionInfo("58 27 55 13 10 27", 0, 350);
                case PresetGeographicalPositions.MalborkPoland:
                    return new PositionInfo(54.027133, 19.134064, 0, 350);
                case PresetGeographicalPositions.SiauliaiLithuania:
                    return new PositionInfo(55.897743, 23.375301, 0, 350);
                case PresetGeographicalPositions.TalinnEstonia:
                    return new PositionInfo(59.416389, 24.799167, 0, 350);
                case PresetGeographicalPositions.TartuEstonia:
                    return new PositionInfo(58.3075, 26.686944, 0, 350);
                case PresetGeographicalPositions.SiverskyLeningrad:
                    return new PositionInfo("59 21 24 30 2 12", 0, 350);
                case PresetGeographicalPositions.PskovAirport:
                    return new PositionInfo(57.785, 28.398333, 0, 350);
                case PresetGeographicalPositions.SmolenskNorthAirport:
                    return new PositionInfo(54.825, 32.025);
                case PresetGeographicalPositions.KaliningradChkalovsk:
                    return new PositionInfo(54.766667, 20.396667, 0, 350);
                case PresetGeographicalPositions.TampereFinland:
                    return new PositionInfo(61.415278, 23.587778, 0, 350);
                case PresetGeographicalPositions.VaasaFinland:
                    return new PositionInfo(63.045278, 21.764167, 0, 350);
                case PresetGeographicalPositions.CFBGander:
                    return new PositionInfo(48.936944, -54.568056, 0, 350);
                case PresetGeographicalPositions.CFBGooseBay:
                    return new PositionInfo(53.319167, -60.425833, 0, 350);
                case PresetGeographicalPositions.CFBNorthBay:
                    return new PositionInfo(46.357117, -79.415058, 0, 350);
                case PresetGeographicalPositions.CFBShearwater:
                    return new PositionInfo(44.637222, -63.502222, 0, 350);
                case PresetGeographicalPositions.USAFThule:
                    return new PositionInfo(76.531111, -68.703056, 0, 350);
                case PresetGeographicalPositions.RAFLakenheath:
                    return new PositionInfo(52.408333, 0.556667, 0, 350); //52.408333,0.556667
                case PresetGeographicalPositions.RAFMolesworth:
                    return new PositionInfo(52.383814, -0.41705, 0, 350); 
                default:
                    break;
            }
            return null;
        }



        #endregion

        #region "Public enums"

        public enum SonarStrengthTypes
        {
            PassiveTowedArrayHigh,
            PassiveTowedArrayLow,
            PassiveSonobuoy,
            PassiveExcellent,
            PassiveVeryGood,
            PassiveMedium,
            PassiveWeak,
            ActiveTowedArrayHigh,
            ActiveTowedArrayLow,
            ActiveSononbuoy,
            ActiveExcellent,
            ActiveVeryGood,
            ActiveMedium,
            ActiveWeak,
        }


        public enum BeaufortScale
        {
            Calm,
            LightAir,
            LightBreeze,
            GentleBreeze,
            ModerateBreeze,
            FreshBreeze,
            StrongBreeze,
            NearGale,
            Gale,
            StrongGale,
            Storm,
            ViolentStorm,
            Hurricane
        }

        public enum WeatherSystemTypes
        { 
            Random,
            Fine,
            Fair,
            Rough,
            Severe
        }

        public enum WeatherSystemSeasonTypes
        { 
            Summer,
            Spring,
            Autumn,
            Winter,
            ArcticWinter
        }

        public enum PrecipitationLevel
        {
            None,
            Drizzle,
            Light,
            Intermediate,
            Heavy
        }

        public enum PrecipitationType
        {
            None,
            Rain,
            Snow,
            Hail
        }

        public enum RiseSetType { Solar, Lunar };

        public enum TideEvent
        {
            MoonRise,
            MoonSet,
            SunRise,
            SunSet
        }

        public enum TideEventType
        {
            NewMoon,
            FirstQuarter,
            FullMoon,
            LastQuarter
        }

        public enum PresetGeographicalPositions
        {
            BergenFlesland,
            Keflavik,
            RAFLossiemouth,
            OlenyaAirBase,
            Andoya,
            Bardufoss,
            Bodo, 
            Gardermoen,
            Orland, //Ørland Main Air Station, Trondheim
            Banak, //Lakselv, Finnmark
            Narvik, //Port, No
            Stavanger, //Port
            JanMayensfield,
            Skrydstrup, //DK
            CopenhagenKastrup, //DK
            Lulea, //SE
            StockholmArlanda, //SE
            Ronneby, //SE
            Lidkoping, //SE
            MalborkPoland,
            SiauliaiLithuania,
            TalinnEstonia,
            TartuEstonia,
            SiverskyLeningrad, //RU, Leningrad
            KaliningradChkalovsk, //RU
            PskovAirport, //RU, Pskov
            SmolenskNorthAirport, //RU, Smolensk
            TampereFinland, 
            VaasaFinland,
            CFBGander, //CA, Newfoundland
            CFBGooseBay, //CA, Newfoundland *
            CFBShearwater, //CA, Nova Scotia (only helos currently)
            CFBNorthBay, //CA, Ontario 
            USAFThule, //Greenland, USAF
            RAFLakenheath, //England, RAF & USAF
            RAFMolesworth, //England, USAF
            //more
        }

        public enum UnitsOfLength { Mile, NauticalMiles, Kilometer, Meter }
        public enum DirectionCardinalPoints { N, E, W, S, NE, NW, SE, SW }
        public enum HeightDepthPoints
        {
            MaxDepth = -6,
            VeryDeep = -5,
            Deep = -4,
            MediumDepth = -3,
            ShallowDepth = -2,
            PeriscopeDepth = -1,
            Surface = 0,
            VeryLow = 1,
            Low = 3,
            MediumHeight = 4,
            High = 5,
            VeryHigh = 6,
            MaxHeight = 7,
        }

        public enum RelativeBearing
        { 
            UnknownOrUndefined,
            MovingParallel,
            MovingTowards,
            MovingAway,
        }


        public enum UnitType
        {
            SurfaceShip,
            FixedwingAircraft,
            Helicopter,
            Submarine,
            Missile,
            Torpedo,
            Mine,
            Decoy,
            Sonobuoy,
            BallisticProjectile,
            Bomb,
            LandInstallation
        }

        public enum UnitSubType
        {
            AircraftCarrier,
            SurfaceShipCombattant,
            SurfaceShipSupport,
            LittoralPatrol,
            FixedWingFighter,
            FixedWingStrike,
            FixedWingSupport,
            HelicopterAsw,
            HelicopterStrike,
            HelicopterOther,
            Submarine,
            Missile,
            Torpedo,
            LandAirport,
            LandSeaport,
            LandOther,
            Other
        }



        public enum RunwayStyle
        { 
            NoRunway,
            Helicopter,
            ShortRunway,
            LongRunway,
            VeryLongRunway
        }

        public enum FriendOrFoe
        {
            Undetermined,
            Friend,
            Foe
        }

        public enum DirtyStatus //must be listed in increasing level of change
        {
            Clean,
            PositionOnlyChanged,
            UnitChanged,
            NewlyCreated
        }

        public enum DetectionClassification
        {
            Unknown,
            //Surface,
            SmallSurface,
            MediumSurface,
            LargeSurface,
            //Aircraft,
            FixedWingAircraft,
            FixedWingAircraftLarge,
            Helicopter,
            Missile,
            Subsurface,
            Submarine,
            Torpedo,
            Mine,
            Sonobuoy,
            BallisticMissile,
            LandInstallation
        }

        public enum ThreatClassification
        {
            U_Undecided,
            A_PotentAndImmediate,
            B_ImmediateOnly,
            C_PotentOnly,
            D_NeitherPotentNorImmediate,
            Z_NoThreat
        }

        public enum PropulsionSystem
        {
            Unmovable,
            ShipConventional, //ie propeller
            Hovercraft,
            UnderwaterPropeller,
            UnderwaterJet,
            TurboJet,
            TurboFan,
            TurboProp,
            TurboShaft,
            Piston,
            MissileJet,
            Rocket,
            BallisticGuided,
            BallisticUnguided,
            LandBasedMoving,
        }

        public enum Role
        {
            NoOrAnyRole, //specific role for queries
            InterceptAircraft,
            AttackSurface,
            AttackSurfaceStandoff,
            AttackAir,
            AttackSubmarine,
            AttackLand,
            AttackLandStandoff,
            Scouting,
            AEW,
            AWACS,
            AAW, //AAW, ASW and ASuW should be used only for units that have these as their primary role. 
            ASW, //Used for formations and AI.
            ASuW,
            OffensiveJamming,
            DefensiveJamming,
            DeployMines,
            MineCountermeasures,
            TransportSupplies,
            ResupplyShips,
            RefuelAircaft,
            IsAircraft,
            IsFixedWingAircraft,
            IsRotaryWingAircraft,
            LaunchFixedWingAircraft,
            LaunchRotaryWingAircraft,
            IsSurfaceShip,
            IsSurfaceCombattant,
            IsAmphibiousAssault,
            IsLittoralPatrol,
            IsCoastGuard,
            IsSubmarine,
            IsLandInstallation,
            IsLandDefensiveStructure, //SAM batteries, missile batteries, radar stations
        }

        public enum DomainType
        { 
            Surface,
            Air,
            Subsea,
            Land,
            Unknown,
        }

        public enum Priority
        {
            ExtremelyCritical,
            Critical,
            Urgent,
            Elevated,
            Normal,
            Low
        }

        public enum RulesOfEngagement
        {
            FireOnClearedTargets,
            FireOnlyInSelfDefence,
            HoldFire,
        }

        public enum WeaponOrders
        {
            FireOnAllClearedTargets,
            FireInSelfDefenceOnly,
            HoldFire
        }

        public enum WeaponTrajectoryType
        {
            DirectShot, //not spawning units
            AntiAirTracking,
            CruiseSeaSkimming,
            CruiseHighAltitude,
            GravityBomb,
            BallisticMissile,
            TorpedoHeavyweight,
            TorpedoLight,
            AsRoc, //anti-sub rocket
        }

        public enum CriticalDamageType
        {
            NoDamage,
            WeaponDamaged,
            SensorDamaged,
            AircraftHangarDamaged,
            Fire,
        }

        public enum SendMessageTo
        { 
            All,
            Allies,
            Enemies,
            CompetetivePlayers,
            Observers,
        }

        public enum UnitOrderType
        {
            LaunchAircraft,
            ReturnToBase,
            SetSpeed,
            SetElevation,
            SensorActivationOrder, 
            SensorDeploymentOrder,
            ChangeAircraftLoadout, //others...
            SetNewGroupFormation,
            SetUnitFormationOrder,
            MovementOrder,
            EngagementOrder,
            SplitGroup,
            JoinGroups,
            SpecialOrders,
            AcquireNewUnit,
            AcquireAmmo,
            RefuelInAir
        }

        public enum SpecialOrders
        { 
            None,
            DropSonobuoy,
            DropMine,
            JammerRadarDegradation,
            JammerCommunicationDegradation,
            JammerRadarDistortion,
        }

        public enum AreaEffectType
        {
            JammerRadarDegradation,
            JammerCommunicationDegradation,
            JammerRadarDistortion
        }

        public enum AIHintType
        { 
            ThreatAxisAir,
            ThreatAxisSurface,
            ThreatAxisSub,
        }

        public enum OrderType
        {
            UnknownOrder,
            MovementOrder,
            MovementFormationOrder,
            EngagementOrder,
            ReturnToBase,
            SensorActivationOrder,
            SensorDeploymentOrder,
            SetSpeed,
            SetElevation,
            LaunchOrder,
            ChangeAircraftLoadout,
            SetNewGroupFormation,
            SetUnitFormationOrder,
            SplitGroup,
            JoinGroups,
            ScheduledOrder,
            MissileSearchForTarget,
            SpecialOrders,
            RefuelInAir
        }

        public enum EwCounterMeasuresType
        {
            None,
            Flare, //against IR-guided
            Chaff, //against radar-guided
            TorpedoDecoy
        }

        public enum SkillLevelInclusion
        {
            IncludeAlways,
            IncludeToIncreasedLevel,
            IncludeToReducedLevel,
        }

        public enum HighLevelOrderType
        { 
            SetAswPatrol,
            SetAewPatrol,
            LaunchAircraft,
            SendGameUiControl,
            TurnOffUnnecessaryActiveSensors,
            EngagePrimaryTargets,
            EngageSurfaceTargets,
            EngageLandStructures,
            DefendHighValueUnits,
            ChangeOrdersOfEngagement,
            //many more
        }

        public enum DefeatConditionType
        {
            CountUnitClassIdLowerThan,
            CountUnitClassLostHigherThan,
            CountUnitRoleLowerThan,
            CountUnitRoleLostHigherThan, //MORE!
            TimeElapsedLongerThanSec,
            CountUnitClassIdInSectorLowerThan,
            CountUnitRoleInSectorLowerThan, //use these two in combination
            CountEnemyClassHigherThan,      //with TimeElapsedLongerThanSec
            CountEnemyRoleHigherThan,
            CountEnemyClassIdInSectorHigherThan,
            CountEnemyRoleInSectorHigherThan
        }

        //For EventTrigger
        public enum EventTriggerType
        { 
            PlayerUnitInRegion,
            PlayerDetectsEnemy,
            TimeHasElapsed,
            OrderReceivedFromPlayer,
            UnitTagIsDestroyed,
            CountUnitClassInSectorEqualsOrHigher,
            CountUnitRoleInSectorEqualsOrHigher,
            CountEnemyClassEqualsOrLower,
            CountEnemyUnitRoleEqualsOrLower,
            CountEnemyRoleDestroyedEqualsOrHigher,
            CountEnemyUnitClassDestroyedEqualsOrHigher,
            EnemyUnitTagIsDestroyed,
            TriggeredBySignalFromGui,
            CountEnemyDestroyedCivilianUnits,
            CountEnemyDestroyedNeutralUnits,
        }

        public enum MissionType
        { 
            Patrol,
            Attack,
            Ferry,
            Other,
        }

        public enum MissionTargetType
        { 
            Undefined,
            Air,
            Surface,
            Sub,
            Land,
            Mines
        }

        public enum UnitSpeedType
        { 
            UnchangedDefault,
            Stop,
            Crawl,
            Slow,
            Half,
            Cruise,
            Military,
            Afterburner,
        }

        public enum EngagementOrderType
        { 
            CloseAndEngage,
            EngageNotClose,
            BearingOnlyAttack
        }

        public enum EngagementStrength
        { 
            MinimalAttack,
            DefaultAttack,
            OverkillAttack
        }

        public enum EngagementStatusResultType
        { 
            Undetermined,
            ReadyToEngage,
            MustCloseToEngage,
            TooCloseToEngage,
            OutOfSectorRange,
            NoWeapon,
            CannotFireAtThisTarget,
            OutOfAmmo,
            WeaponDamaged,
        }

        public enum WeaponLoadType
        {
            Undefined,
            Ferry,
            AirSuperiority,
            Strike, //land and sea
            NavalStrike,
            LandAttack,
            ASW,
            AEW,
            DeployMines,
            CounterMine,
            ElectronicWarfare,
            RefuellingPlane,
            Other, //
        }

        public enum WeaponLoadModifier
        {
            None,
            LongRange,
            ShortRange,
            NoStealth,
            LongRangeNoStealth,
            ShortRangeNoStealth,
            LongRangeWeapon,
            AntiRadiation,
        }

        public enum AircraftDockingStatus
        {
            Storage,
            TankingAndRefitting,
            ReadyForTakeoff,
            Flying
        }

        public enum SensorType
        {
            Radar,
            Sonar,
            Visual,
            MAD, //magnetic anomaly detector
            Infrared //other?
        }

        public enum RadarCrossSectionSize
        {
            VeryStealthy,
            Stealthy,
            Reduced,
            Normal,
            Large
        }

        public enum FireLevel
        { 
            NoFire,
            MinorFire,
            MajorFire,
            SevereFire
        }

        public enum SonarNoiseLevel
        { 
            ExtremelyQuiet,
            VeryQuiet,
            Quiet,
            Noisy,
            Loud,
            VeryLoud
        }

        public enum SonarFrequencyBands
        { 
            HF,
            MF,
            LF_MF,
            LF,
            VHF_LF
        }

        public enum SonarType
        {
            HullMountedBow,
            HullMountedKeel,
            HullMountedFlankArray,
            DippingOrSonobuoy,
            TowedArray,
        }

        public enum EsmRadiationLevel
        { 
            EsmNone,
            EsmLow,
            EsmMedium,
            EsmHigh
        }

        
        public enum GameStateInfoType
        {
            UnitIsDestroyed,
            DetectedContactIsDestroyed,
            DetectedContactIsLost,
            DetectedContactGroupIsLost,
            AircraftIsLanded,
            AircraftTakeoff,
            MissileLaunch,
            GunFired, //Gatling or gun fired. Id = unit firing, WeaponClassId, BearingDeg is bearing deg from unit to target unit, Count = no of rounds, SecondaryId = target det id
            EnemyMissileLaunch, //id = detected unit fired, WeaponClassId, BearingDeg and Count set
            EnemyGunFired, //id = detected unit, WeaponClassId and BearingDeg set
            UnitUsesCountermeasures, //id = unit id, WeaponClassId = Weapon used for countermeasures
            EnemyUnitUsesCountermeasures, //id = detection id, WeaponClassId = Weapon used for countermeasures
            SonobuoyDropped,
            MineDropped,
            UnitsWithinArea,
            FailedToLoadGameScenario, //PlayerSelectScenario failed. Scenario does not exist or player is not admin
            CommunicationJammingBlockedOrder, //Order given to unit under communication jamming. Order discarded. Id = UnitId
            CannotAcquireUnit, //Unit not available in this game, too short runway on carrier unit, or not enough funds. Id = carrier. UnitClassId set
            CannotAcquireAmmo, //Could not acquire more ammo. No stores for that weapon, or not enough funds. Id = unit, weaponclassId set
            AircraftLandingFailed, //Aircraft failed to land: carrier damaged, rough weather or wrong runway type. Id = aircraft, SecondaryId = carrier
            AircraftTakeoffFailed, //Aircraft could not take off due to rough weather. Id = carrier
            AircraftCrashedOnTakeoffRoughWeather, //Id = aircraft, SecondaryId = carrier
            AircraftCrashedOnLandingRoughWeather, //Id = aircraft, SecondaryId = carrier
            AircraftNotFindingNewCarrier, //Id = unit. Cannot find an airfield or new carrier that can host aircraft. It's so fucked.
            AircraftAssignedNewCarrier, //Id = unit, SecondaryId = new carrier unit id.
            AircraftCarrierNotReady, //Id=unit, SecondaryId = alleged carrier unit id. Unit is not carrier, air fac. damaged, or hangar is full
            AircraftNotFuelToReachThisCarrier, //Id=unit, SecondaryId = alleged carrier unit id. Not enough fuel to reach this carrier
            AircraftCarrierWrongRunwayStyle, //Id=unit, SecondaryId = alleged carrier unit id. Wrong type of runway on suggested carrier unit
            AircraftRefuelInAirFailed, //Id=aircraft, SecondaryId=refueling aircraft
            CannotChangeWeaponload, //Weapon stores for required weapon is too low. Id = aircraft, WeaponClassId =  weaponclass id
            UnitCrashedOutOfFuel, //id = crashed unit id
            UnitBingoFuelReturningToBase, //id = unit id, SecondaryId = carrier id
            CannotLaunchAircraftHangarDamaged, //id = carrier id
            JammingOrderExecuted, //Id = jammer unit id. WeaponClassId (jammer), RadiusM, and Position set for info about where and what
            DropSonobuoyFailedRoughWeather, //Cannot drop sonobuoy because the sea is too rough
            UnitHasNoWeaponToEngageTarget, //Id = unit, SecondaryId = target id
            UnitMustCloseToEngage, //Id is unit, RangeM is how many meters it must close to engage, SecondaryId is Target Id
            UnitHasNoOrders, //Unit has reached its final waypoint and continues on same course at reduced speed. Id = unit
            AircraftIsReady, //Carried unit ready for launch. Id = unit Id, SecondaryId=Carrier Id.
            UnitMovingToJoinGroup, //unit is moving closer to fulfill join group order. Id = unit, SecondaryId = new group id
            UnitJoinGroupInvalid, //unit type mismatch (ir air with surface). Id=Unit id, SecondaryId = group Id
            UnitHasBeenDestroyedByFire, //unit burned up. Id = unit id
            EnemyUnitHasBeenDestroyedByFire, //Id = detected Id if known. Otherwise UnitClassId set to tell which enemy unit type was lost to fire
            UnitDetectedByActiveSensor, //Id is unit, SecondaryId is DetectedUnit using active sensor
            WeaponCantFireTooClose, //Id is unit, SecondaryId is DetectedUnit, UnitClassId is WeaponClassId
            WeaponCantFireRoughWeather, //Id is unit, SecondaryId is DetectedUnit, UnitClassId is WeaponClassId
            GroupHasBeenSplit, //Id is old GroupId, SecondaryId is Id of new group
            MineDestroyedByEnemy, //Id is mine id
            MineDestroyedByUs, //Id is countermine vessel
            UnitStoppedForImpassibleTerrain, //Id is unitId. Sent when surf/sub unit is moving into land terrain
            UnitIsEvadingEnemyFire, //Id is unitId, SecondaryId is threatening DetectedUnitId
            NotPermittedToEngageUnderOoE, //Order of Engagement for player does not currently permit opening fire. Id, if set, is unit receiving order
        }

        public enum GameUiControlType
        { 
            SetCameraFocus,
            PlayAudio,
            PlayVideo,
            ShowTextInfo,
            SetMapPositionFlag,
            ClearMapPositionFlag,
            HighlightGUIElement,
            SelectUnit,
            TriggerEventOnUnitClassSelected,
        }

        public enum GameScenarioType
        { 
            PlayerVsComputer,
            PlayerVsComputerTutorial,
            PlayerVsPlayerOnline,
            MultiplayerOnline  //supports more than 2 human players
        }

        public enum GameEconoomicModel
        { 
            NoEconomy,
            TimeOnly,
            RecoupLosses,
            RewardKills,
        }

        public enum ConnectionStatusEnum
        {
            Disconnected,
            Connected,            
        }

        #endregion


    }
}
