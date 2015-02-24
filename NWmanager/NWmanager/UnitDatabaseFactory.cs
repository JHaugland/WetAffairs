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
using TTG.NavalWar.NWComms.NonCommEntities;
using System.IO;

namespace NWmanager
{
    public static partial class UnitDatabaseFactory
    {
        public delegate void MessageEventHandler(string message);
        public static event MessageEventHandler MessageCallback;

        private static void Message( string message )
        {
            if ( MessageCallback != null )
            {
                MessageCallback( message );
            }
        }


        public static List<WeaponClass> CreateWeaponClasses()
        {
            #region "AA missile weapons"

            List<WeaponClass> list = new List<WeaponClass>();

            #region "Nato AA missile weapons"

            WeaponClass wc = new WeaponClass()
            {
                Id = "rim66standard",
                WeaponClassName = "RIM-66 Standard MR SM-2",
                WeaponClassShortName = "SM-2",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true, //actually, it can
                CanBeTargetted = true,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 167000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 4287,
                HighestOperatingHeightM=24400,
                LowestOperatingHeightM=10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                MaxWeaponRangeM = 167000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemiir",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "aster15",
                WeaponClassName = "Aster 15 SAM",
                WeaponClassShortName = "Aster 15",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false, 
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 29000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 1700,
                MaxSpeedKph = 3675,
                HighestOperatingHeightM = 13000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "aster30",
                WeaponClassName = "Aster 30 SAM",
                WeaponClassShortName = "Aster 30",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 110000,
                MaxWeaponRangeM = 130000,
                MinWeaponRangeM = 3000,
                MaxSpeedKph = 5500,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "idassam",
                WeaponClassName = "IDAS SAM",
                WeaponClassShortName = "IDAS",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 18000,
                MaxWeaponRangeM = 20000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3675,
                HighestOperatingHeightM = 13000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "mim104fpatriot",
                WeaponClassName = "MIM-104F Patriot SAM",
                WeaponClassShortName = "Patriot",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false, 
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 240000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 2400,
                HighestOperatingHeightM = 24200,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 16 * 4,
                MaxSimultanousShots = 4,
                MaxWeaponRangeM = 240000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mim23hawk",
                WeaponClassName = "MIM-23 Hawk SAM",
                WeaponClassShortName = "Hawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 45000,
                MaxWeaponRangeM = 45000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 2940,
                HighestOperatingHeightM = 24200,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 16 * 4,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 20,
                MaxSeaState = 9,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rbs23bamse",
                WeaponClassName = "RBS-23 BAMSE SAM",
                WeaponClassShortName = "RBS-23",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 45000,
                MaxWeaponRangeM = 45000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 2940,
                HighestOperatingHeightM = 24200,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 3,
                IsECM = false,
                MaxAmmunition = 16 * 4,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, 
                HitPercent = 90,
                ReferenceAgilityFactor = 4,
                EwCounterMeasureResistancePercent = 20,
                MaxSeaState = 9,

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "nasams2amraam",
                WeaponClassName = "NASAMS2 Amraam",
                WeaponClassShortName = "AMRAAM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 25000,
                MaxWeaponRangeM = 26000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 4900,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 12 * 6,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rim7seasparrow",
                WeaponClassName = "RIM-7 Sea Sparrow",
                WeaponClassShortName = "Sea Sparrow",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 19000,
                MaxWeaponRangeM = 19000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 4287,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "seawolf",
                WeaponClassName = "Sea Wolf SAM",
                WeaponClassShortName = "Sea Wolf",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 10000,
                MaxWeaponRangeM = 11000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3675,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, 
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 8,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "umkhontoir",
                WeaponClassName = "Umkhonto-IR SAM",
                WeaponClassShortName = "Umkhonto-IR",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 15000,
                MaxWeaponRangeM = 15000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 2450,
                HighestOperatingHeightM = 8000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rim116ram",
                WeaponClassName = "RIM-116 Rolling Airframe Missile",
                WeaponClassShortName = "RAM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 9000,
                MaxWeaponRangeM = 9000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3000,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemiir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rim162essm",
                WeaponClassName = "RIM-162 Evolved Sea Sparrow Missile",
                WeaponClassShortName = "ESSM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 50000,
                MaxWeaponRangeM = 50000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 5000,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rim67standard",
                WeaponClassName = "RIM-67 Standard ER Missile",
                WeaponClassShortName = "Standard",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 150000,
                MaxWeaponRangeM = 190000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 5000,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mistral",
                WeaponClassName = "Mistral AA Missile",
                WeaponClassShortName = "Mistral",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 4000,
                MaxWeaponRangeM = 5000,
                MinWeaponRangeM = 200,
                MaxSpeedKph = 3100,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "fim92cstinger",
                WeaponClassName = "FIM-92C Stinger SAM",
                WeaponClassShortName = "Stinger",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 8000,
                MaxWeaponRangeM = 8000,
                MinWeaponRangeM = 200,
                MaxSpeedKph = 1700,
                HighestOperatingHeightM = 8000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "thaleslmm",
                WeaponClassName = "Thales Lightweight Multi-role Missile",
                WeaponClassShortName = "Thales LMM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 7800,
                MaxWeaponRangeM = 8000,
                MinWeaponRangeM = 200,
                MaxSpeedKph = 1700,
                HighestOperatingHeightM = 8000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 8,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true, 
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 7,

            };
            list.Add(wc);



            wc = new WeaponClass()
            {
                Id = "aim120amraam",
                WeaponClassName = "AIM-120D AMRAAM Missile",
                WeaponClassShortName = "AMRAAM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 170000,
                MaxWeaponRangeM = 180000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 4900,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 100,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 7,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rapier2sam",
                WeaponClassName = "RAPIER II CAMM(L) SAM",
                WeaponClassShortName = "Rapier 2",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 30000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 2,
                WeaponBearingRangeDeg = 100,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 3,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "aim9sidewinder",
                WeaponClassName = "AIM-9X Sidewinder Missile",
                WeaponClassShortName = "Sidewinder",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 34000,
                MaxWeaponRangeM = 35400,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 24400,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 100,
                IncreasesMassByKg = 1000,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false, //hmm
                ReferenceAgilityFactor = 3,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            #endregion //Nato aa missile weapons

            #region "Russian AA missile weapons"
            
            wc = new WeaponClass()
            {
                Id = "3k87kortik",
                WeaponClassName = "3K87 Kortik SA-19 SAM",
                WeaponClassShortName = "Kortik",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 10000,
                MaxWeaponRangeM = 10000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3240,
                HighestOperatingHeightM=3500,
                LowestOperatingHeightM=10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "3k95kinzhal",
                WeaponClassName = "Tor 3K95 Kinzhal 'SA-N-9 Gauntlet' SAM",
                WeaponClassShortName = "Kinzhal",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 12000,
                MaxWeaponRangeM = 12000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 6000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "9k33osam",
                WeaponClassName = "9K33M OSA-M 'SA-N-4 Gecko' SAM",
                WeaponClassShortName = "Osa-M",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 14000,
                MaxWeaponRangeM = 15000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 3670,
                HighestOperatingHeightM = 12000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "s300fm",
                WeaponClassName = "S-300fm Fort M 'SA-N-20 Grumble' SAM",
                WeaponClassShortName = "S-300",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 140000,
                MaxWeaponRangeM = 150000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 7350,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "s400trimuf",
                WeaponClassName = "S-400 Triumf 'SA-21 Growler' SAM",
                WeaponClassShortName = "Triumf",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 120000,
                MaxWeaponRangeM = 120000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 3600,
                HighestOperatingHeightM = 30000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "9k38igla",
                WeaponClassName = "9K38 Igla 'SA-18 Grouse' SAM",
                WeaponClassShortName = "Igla",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 5200,
                MaxWeaponRangeM = 5200,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 2800,
                HighestOperatingHeightM = 3500,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                MaxSeaState = 8,
            };
            list.Add(wc);
            wc = new WeaponClass()
            {
                Id = "vympelr37",
                WeaponClassName = "Vympel R-37 'AA-13 Arrow' AA Missile",
                WeaponClassShortName = "Vympel R-37",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 380000,
                MaxWeaponRangeM = 398000,
                MinWeaponRangeM = 3000,
                MaxSpeedKph = 5500,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "vympelr77m1",
                WeaponClassName = "Vympel R-77M1 'AA-12 Adder' AA Missile",
                WeaponClassShortName = "Vympel R-77M1",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 70000,
                MaxWeaponRangeM = 80000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 5500,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "vympelr77",
                WeaponClassName = "Vympel R-77-AE-PD 'AA-12 Adder' AA Missile",
                WeaponClassShortName = "Vympel R-77-AE-PD",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 150000,
                MaxWeaponRangeM = 160000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 5500,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissile",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "vympelr73",
                WeaponClassName = "Vympel R-73 'AA-11 Archer' AA Missile",
                WeaponClassShortName = "Vympel R-73",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = true,
                EffectiveWeaponRangeM = 28000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "vympelr74",
                WeaponClassName = "Vympel R-74M 'AA-11 Archer B' AA Missile",
                WeaponClassShortName = "Vympel R-74M",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 38000,
                MaxWeaponRangeM = 40000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissileir",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "vympelr27",
                WeaponClassName = "Vympel R-27 'AA-10 Alamo' AA Missile",
                WeaponClassShortName = "Vympel R-27",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AntiAirTracking,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanRetargetAfterLaunch = false,
                EffectiveWeaponRangeM = 120000,
                MaxWeaponRangeM = 130000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 4900,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aamissilesemi",
                UnitModelFileName = "aamissile_AMRAAM",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 3,
                EwCounterMeasureResistancePercent = 20,
                MaxSeaState = 8,
            };
            list.Add(wc);


            #endregion //Russian AA missile weapons

            #endregion //AA missile weapons

            #region "Guns and point defence"

            #region "Nato Guns"
            wc = new WeaponClass()
            {
                Id = "phalanxciws",
                WeaponClassName = "Phalanx CIWS Gatling Gun",
                WeaponClassShortName = "Phalanx",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 3600,
                MaxWeaponRangeM = 4000,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 100000,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 110,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "oerlikon35mm",
                WeaponClassName = "Oerlikon 35mm CIWS gun",
                WeaponClassShortName = "Oerlikon",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 3500,
                MaxWeaponRangeM = 3800,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 100000,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 350,
                IncreasesMassByKg = 0,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "oerlikon30mm",
                WeaponClassName = "Oerlikon 30mm twin gun",
                WeaponClassShortName = "Oerlikon 30mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 8000,
                MaxWeaponRangeM = 10000,
                MinWeaponRangeM = 80,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 1000,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 0,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 3,
                HitPercent = 80
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mk45gun",
                WeaponClassName = "Mk-45 5 inch gun", //5 inch (127 mm/62) Mk-45 mod 4 
                WeaponClassShortName = "Mk-45",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 24000,
                MaxWeaponRangeM = 24000,
                MinWeaponRangeM = 100,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 500,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "ags155mm", //Zumwalt
                WeaponClassName = "AGS 155mm gun", //155mm Advanced Gun System
                WeaponClassShortName = "AGS 155",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                MaxWeaponRangeM = 109000,
                MinWeaponRangeM = 2000,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 500,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "bae45mk8gun",
                WeaponClassName = "BAE 4.5 inch Mk8 gun", 
                WeaponClassShortName = "BAE 4.5\" gun",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 22000,
                MaxWeaponRangeM = 27500,
                MinWeaponRangeM = 100,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 500,
                MaxSimultanousShots = 16,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "m134minigun",
                WeaponClassName = "M134 Minigun",
                WeaponClassShortName = "Minigun",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1000,
                MaxWeaponRangeM = 1000,
                MinWeaponRangeM = 0,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 20,
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "bofors5770",
                WeaponClassName = "Bofors 57 mm/70 SAK Mk3", 
                WeaponClassShortName = "Bofors 57 mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 15500,
                MaxWeaponRangeM = 17500,
                MinWeaponRangeM = 0,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 2,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "bofors4060",
                WeaponClassName = "Bofors 44 mm/60 gun",
                WeaponClassShortName = "Bofors 44 mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 15500,
                MaxWeaponRangeM = 17500,
                MinWeaponRangeM = 100,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 2,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "nsv127mm",
                WeaponClassName = "NSV 12.7 mm heavy machine gun", 
                WeaponClassShortName = "NSV 12.7 mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1500,
                MaxWeaponRangeM = 2000,
                MinWeaponRangeM = 0,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 12,
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "otobreda76mm",
                WeaponClassName = "Otobreda 76 mm gun",
                WeaponClassShortName = "Otobreda 76mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 28000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 100,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 85,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "otobreda127mm",
                WeaponClassName = " Otobreda 127mm/54 Compact gun",
                WeaponClassShortName = "Otobreda 127mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 28000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 100,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 4,
                IsECM = false,
                MaxAmmunition = 85,
                MaxSimultanousShots = 16,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 300,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mauserbk27",
                WeaponClassName = "Mauser BK 27",
                WeaponClassShortName = "Mauser Bk27",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 3000, //unknown
                MaxWeaponRangeM = 3500,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 200,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "m61a2gatling",
                WeaponClassName = "M61A2 Vulcan Gatling Gun",
                WeaponClassShortName = "M61A2 Vulcan",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 2000,
                MaxWeaponRangeM = 2500,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 200,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 3,
                HitPercent = 80

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gau12gun",
                WeaponClassName = "GAU-12/U Equalizer 25mm Gatling Gun",
                WeaponClassShortName = "GAU-12",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 2000,
                MaxWeaponRangeM = 2500,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 300,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 3,
                HitPercent = 80

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gau8agatling",
                WeaponClassName = "GAU-8/A Avenger 30mm Gatling Gun",
                WeaponClassShortName = "GAU-8/A",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 2000,
                MaxWeaponRangeM = 2100,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 3,
                HitPercent = 80

            };
            list.Add(wc);

            #endregion //Nato guns

            #region "Russian guns"

            wc = new WeaponClass()
            {
                Id = "ak630gun",
                WeaponClassName = "AK-630 CIWS Gatling Gun",
                WeaponClassShortName = "AK-630",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 3600,
                MaxWeaponRangeM = 4000,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 4000,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 220,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "dual10070", //Udaloy 1
                WeaponClassName = " dual 100mm/70cal DP Gun",
                WeaponClassShortName = "100mm/70cal",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 20000,
                MaxWeaponRangeM = 22000,
                MinWeaponRangeM = 80,
                MaxSeaState = 8,
                DamageHitPoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.18),
                IsECM = false,
                MaxAmmunition = 100,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 220,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "30mmaa",
                WeaponClassName = "30mm AA Gun",
                WeaponClassShortName = "30mm",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 3600,
                MaxWeaponRangeM = 4000,
                MinWeaponRangeM = 0,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 4000,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 8,
                WeaponBearingRangeDeg = 220,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 4,
                HitPercent = 90
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "ak130gun",
                WeaponClassName = "AK-130 130 mm/L70 Gun",
                WeaponClassShortName = "AK-130",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 26000,
                MaxWeaponRangeM = 28000,
                MinWeaponRangeM = 80,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 5,
                IsECM = false,
                MaxAmmunition = 300,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 220,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 80
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "a190gun",
                WeaponClassName = "Arsenal A-190 100mm gun",
                WeaponClassShortName = "A-190",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 21000,
                MaxWeaponRangeM = 22000,
                MinWeaponRangeM = 80,
                MaxSeaState = 9,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 300,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 220,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 80
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gsh30",
                WeaponClassName = "Gryazev-Shipunov GSh-30-1 30mm Cannon",
                WeaponClassShortName = "GSh-30-1",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1200,
                MaxWeaponRangeM = 1800,
                MinWeaponRangeM = 80,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 200,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 1,
                HitPercent = 80

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gsh23",
                WeaponClassName = "Gryazev-Shipunov 23mm Cannon",
                WeaponClassShortName = "GSh-23",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = true,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1200,
                MaxWeaponRangeM = 1800,
                MinWeaponRangeM = 80,
                MaxSeaState = 8,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2,
                IsECM = false,
                MaxAmmunition = 200,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = "",
                TimeBetweenShotsSec = 4,
                WeaponBearingRangeDeg = 80,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                ReferenceAgilityFactor = 2,
                HitPercent = 80
            };
            list.Add(wc);


            #endregion

            #endregion //Guns

            #region "ASuW and Land attack missile weapons"

            #region "NATO ASuW and LA missiles"

            #region "Harpoons"
           
            wc = new WeaponClass()
            {
                Id = "rgm84harpoon",
                WeaponClassName = "RGM-84 Harpoon", //ship launched
                WeaponClassShortName = "Harpoon",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 260000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 278000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm84harpoon",
                WeaponClassName = "AGM-84 Harpoon", //air launched
                WeaponClassShortName = "Harpoon",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                EffectiveWeaponRangeM = 315000,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 315000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm84harpoonslam",
                WeaponClassName = "AGM-84 Harpoon SLAM", //air launched, land target
                WeaponClassShortName = "Harpoon",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 315000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 315000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "ugm84harpoon",
                WeaponClassName = "UGM-84 Harpoon",
                WeaponClassShortName = "Harpoon",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 140000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 140000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            #endregion //harpoons

            #region "Tomahawks"
            
            wc = new WeaponClass()
            {
                Id = "bgm109tomahawktlam",
                WeaponClassName = "BGM-109 Tomahawk TLAM-C", //ship launched, land attack
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                EffectiveWeaponRangeM = 1500000,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 1500000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm109tomahawktlam",
                WeaponClassName = "AGM-109 Tomahawk TLAM-C", //air launched, land attack
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                EffectiveWeaponRangeM = 1500000,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 1500000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "rgm109tomahawktasm",
                WeaponClassName = "RGM/UGM-109B Tomahawk TASM", //surface launched anti ship 
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 450000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 450000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm109tomahawktasm",
                WeaponClassName = "AGM-109B Tomahawk TASM", //air launched, anti ship missile
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 450000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 450000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "ugm109tomahawktlam",
                WeaponClassName = "UGM-109B Tomahawk TLAM", //sub launched, land attack
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 1500000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 1000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 1500000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "ugm109tacticaltomahawk",
                WeaponClassName = "UGM-109 Tactical Tomahawk", //sub launched, land and ship attack
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 1600000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 5000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 1700000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rgm109tacticaltomahawk",
                WeaponClassName = "RGM-109 Tactical Tomahawk", //surface launched, land and ship attack
                WeaponClassShortName = "Tomahawk",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 1600000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 850,
                MinWeaponRangeM = 5000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                MaxWeaponRangeM = 1700000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            #endregion //tomahawks

            wc = new WeaponClass()
            {
                Id = "nsm",
                WeaponClassName = "Kongsberg Naval Strike Missile",
                WeaponClassShortName = "Kongsberg NSM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 175000,
                MaxWeaponRangeM = 185000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "jsm",
                WeaponClassName = "Kongsberg Joint Strike Missile",
                WeaponClassShortName = "Kongsberg JSM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 175000,
                MaxWeaponRangeM = 185000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "seaskua",
                WeaponClassName = "MBDA Sea Skua Mk 1",
                WeaponClassShortName = "Sea Skua",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 25000,
                MaxWeaponRangeM = 25500,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 980,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 6,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissilesemi",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "seaskuamk2", //air to sea, does not exist, but candidate for future
                WeaponClassName = "MBDA Sea Skua Mk 2",
                WeaponClassShortName = "Sea Skua 2",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 40000,
                MaxWeaponRangeM = 40500,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 980,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 4,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 40,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "jassm",
                WeaponClassName = "AGM-158 JASSM",
                WeaponClassShortName = "JASSM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 350000,
                MaxWeaponRangeM = 370000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "jassmer",
                WeaponClassName = "AGM-158B JASSM-ER",
                WeaponClassShortName = "JASSM-ER",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 920000,
                MaxWeaponRangeM = 926000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm88harm",
                WeaponClassName = "AGM-88 HARM",
                WeaponClassShortName = "HARM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 100000,
                MaxWeaponRangeM = 106000,
                MinWeaponRangeM = 3000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 2280,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissilesemi",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                IsAntiRadiationWeapon = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "lrasma",
                WeaponClassName = "Long Range Anti-Ship Missile-A", //ship launched subsonic
                WeaponClassShortName = "LRASM-A",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 920000,
                MaxWeaponRangeM = 926000,
                MinWeaponRangeM = 2000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "lrasmb",
                WeaponClassName = "Long Range Anti-Ship Missile-B", //ship launched supersonic
                WeaponClassShortName = "LRASM-B",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseHighAltitude,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 920000,
                MaxWeaponRangeM = 926000,
                MinWeaponRangeM = 5000,
                HighestOperatingHeightM = 20000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 4050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                TerminalSpeedRangeM = 30000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rbs15mk3",
                WeaponClassName = "Bofors RBS-15 Mk 3",
                WeaponClassShortName = "RBS-15",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 240000,
                MaxWeaponRangeM = 250000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 950,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 4,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm154jsow",
                WeaponClassName = "AGM-154 Joint Standoff Weapon",
                WeaponClassShortName = "JSOW",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 100000, //low altitude 22km, high altitude 130km
                MaxWeaponRangeM = 130000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm154jsower",
                WeaponClassName = "AGM-154 Joint Standoff Weapon ER",
                WeaponClassShortName = "JSOW-ER",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 550000, //555 km
                MaxWeaponRangeM = 555600,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gbu38jdam",
                WeaponClassName = "GBU-38 Joint Direct Attack Munition",
                WeaponClassShortName = "JDAM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 28000, 
                MaxWeaponRangeM = 29000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 800,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "bomb",
                UnitModelFileName = "bomb",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "gbu53bsdb2",
                WeaponClassName = "GBU-53/B Small Diameter Bomb II",
                WeaponClassShortName = "SMB2",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 28000,
                MaxWeaponRangeM = 29000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 800,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "bomb",
                UnitModelFileName = "bomb",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "gbu12",
                WeaponClassName = "GBU-12 Paveway II Bomb",
                WeaponClassShortName = "Paveway II",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 16000,
                MaxWeaponRangeM = 18000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 800,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "bomb",
                UnitModelFileName = "bomb",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm114n",
                WeaponClassName = "AGM-114N Hellfire II",
                WeaponClassShortName = "Hellfire II",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 8000, 
                MaxWeaponRangeM = 9000,
                MinWeaponRangeM = 500,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 5,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "slamer",
                WeaponClassName = "AGM-84K Standoff Land Attack Missile - ER",
                WeaponClassShortName = "SLAM-ER",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 250000,
                MaxWeaponRangeM = 270000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 950,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "agm65",
                WeaponClassName = "AGM-65 Maverick",
                WeaponClassShortName = "Maverick",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot, //hmm
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 28000, 
                MaxWeaponRangeM = 28000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1200,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "aamissile_rim66",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kepd350", //gripen, typhoon, f/a-18
                WeaponClassName = "Taurus KEPD-350",
                WeaponClassShortName = "KEPD-350",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming, 
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 480000,
                MaxWeaponRangeM = 500000,
                MinWeaponRangeM = 10000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1050,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissileir",
                UnitModelFileName = "asumissile_kongsberg",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 30,
                MaxSeaState = 6,
            };
            list.Add(wc);

            #endregion

            #region "Russian ASuW and LA missiles"

            wc = new WeaponClass()
            {
                Id = "p700granit",
                WeaponClassName = "P-700 Granit 'SS-N-19 Shipwreck' Missile", 
                WeaponClassShortName = "Granit",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 550000,
                TerminalSpeedRangeM = 30000,
                MaxWeaponRangeM = 625000,
                MinWeaponRangeM = 5000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 3000,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "p500bazalt",
                WeaponClassName = "P-500 Bazalt 'SS-N-12 Sandbox' Missile",
                WeaponClassShortName = "Bazalt",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 540000,
                TerminalSpeedRangeM = 30000,
                MaxWeaponRangeM = 550000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 5512,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "p1000vulcan",
                WeaponClassName = "P-1000 Vulcan Missile",
                WeaponClassShortName = "Vulcan",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 680000,
                MaxWeaponRangeM = 700000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 2080,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                TerminalSpeedRangeM = 40000,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "p270moskit",
                WeaponClassName = "P-270 Moskit 'SS-N-22 Sunburn' Missile",
                WeaponClassShortName = "Moskit",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 120000,
                MaxWeaponRangeM = 120000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 3060,
                HighestOperatingHeightM = 200,
                LowestOperatingHeightM = 10,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 30,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 120,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                TerminalSpeedRangeM = 20000,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 70,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "p800oniks",
                WeaponClassName = "P-800 3M55 Oniks 'SS-N-26' Missile",
                WeaponClassShortName = "Oniks",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 280000,
                MaxWeaponRangeM = 300000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 3060,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                TerminalSpeedRangeM = 20000,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 70,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "3m54klub", //Anti-ship
                WeaponClassName =  "3M-54E Klub 'SS-N-27' Missile",
                WeaponClassShortName = "54E Klub",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 190000,
                MaxWeaponRangeM = 200000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 3060,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                TerminalSpeedRangeM = 20000,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "3m14eklub", //Anti-land
                WeaponClassName = "3M-14E Klub 'SS-N-27' Missile",
                WeaponClassShortName = "14E Klub",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 270000,
                MaxWeaponRangeM = 275000,
                MinWeaponRangeM = 3000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 980,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            
            wc = new WeaponClass()
            {
                Id = "brahmos",
                WeaponClassName = "PJ-10 BrahMos Missile",
                WeaponClassShortName = "BrahMos",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 250000,
                MaxWeaponRangeM = 290000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 910,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 3430,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 70,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh15",
                WeaponClassName = "Kh-15 'AS-16 Kickback' Missile", //air to surface
                WeaponClassShortName = "Kh-15",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseHighAltitude,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 300000,
                MaxWeaponRangeM = 300000,
                MinWeaponRangeM = 4000,
                HighestOperatingHeightM = 40000,
                TerminalSpeedRangeM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph =  6125, // terminal speed only
                DamageHitPoints = (int) (GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.6),
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "kh31",
                WeaponClassName = "Kh-31 'AS-17 Krypton' Missile", //air to surface
                WeaponClassShortName = "Kh-31",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 110000,
                MaxWeaponRangeM = 110000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 2150,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                IsAntiRadiationWeapon = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh58",
                WeaponClassName = "Kh-58U 'AS-11 Kilter' Missile", //PAK FA anti-radiation
                WeaponClassShortName = "Kh-58U",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 240000,
                MaxWeaponRangeM = 250000,
                MinWeaponRangeM = 3000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 2150,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                IsAntiRadiationWeapon = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh35",
                WeaponClassName = "Zvezda Kh-35U 'AS-20 Kayak' Missile", //air to surface
                WeaponClassShortName = "Kh-35U",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 130000,
                MaxWeaponRangeM = 130000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 900,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh22m",
                WeaponClassName = "Raduga KH-22M 'AS-4 Kitchen' Missile", //air to surface
                WeaponClassShortName = "Kh-22M",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseHighAltitude,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 460000,
                MaxWeaponRangeM = 500000,
                MinWeaponRangeM = 10000,
                HighestOperatingHeightM = 25000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 4000,
                DamageHitPoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.75),
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                TerminalSpeedRangeM = 60000,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh65",
                WeaponClassName = "Raduga Kh-65SE 'AS-15 Kent' Missile", //air to surface
                WeaponClassShortName = "Kh-65SE",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 600000,
                MaxWeaponRangeM = 600000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 900,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "s10granat", //yasen, akula
                WeaponClassName = "S-10 Granat 'SS-N-21 Sampson' Missile", //sub to land, very long range
                WeaponClassShortName = "S-10 Granat",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 3000000,
                MaxWeaponRangeM = 3000000,
                MinWeaponRangeM = 5000,
                HighestOperatingHeightM = 10000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 720,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_tomahawk",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh29", //russian maverick
                WeaponClassName = "Kh-29 'AS-14 Kedge' Missile",
                WeaponClassShortName = "Kh-29",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.DirectShot, //hmm
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 30000,
                MaxWeaponRangeM = 30000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 1200,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissilesemi",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh555",
                WeaponClassName = "Raduga Kh-555 'Kent-C' Missile", //air launched land attack missile
                WeaponClassShortName = "Kh-555",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = true,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 2000000,
                MaxWeaponRangeM = 2500000,
                MinWeaponRangeM = 5000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 900,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissilesemi",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "kh37",
                WeaponClassName = "Raduga Kh-37 Uran Missile", //land attack
                WeaponClassShortName = "Uran",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.CruiseSeaSkimming,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = true,
                EffectiveWeaponRangeM = 230000,
                MaxWeaponRangeM = 250000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 20000,
                LowestOperatingHeightM = 10,
                MaxSpeedKph = 900,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 3,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asumissile",
                UnitModelFileName = "asumissile_harpoon",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                MaxSeaState = 6,
            };
            list.Add(wc);   
            //
            wc = new WeaponClass()
            {
                Id = "kab500kr",
                WeaponClassName = "KAB-500KR Bomb",
                WeaponClassShortName = "KAB-500",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.GravityBomb,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 17000,
                MaxWeaponRangeM = 19000,
                MinWeaponRangeM = 1000,
                HighestOperatingHeightM = 22000,
                LowestOperatingHeightM = 20,
                MaxSpeedKph = 900,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2,
                IsECM = false,
                MaxAmmunition = 16,
                MaxSimultanousShots = 99,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "bomb",
                UnitModelFileName = "bomb",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = true,
                HitPercent = 90,
                ReferenceAgilityFactor = 2,
                EwCounterMeasureResistancePercent = 80,
                MaxSeaState = 6,
            };
            list.Add(wc);

            #endregion
            #endregion //ASuW and Land attack missile weapons"

            #region "Torpedoes and ASW weapons"

            #region "Nato torpedoes and asw weapons"

            wc = new WeaponClass()
            {
                Id = "mark48torpedo",
                WeaponClassName = "Mark 48 ADCAP Heavyweight Torpedo",
                WeaponClassShortName = "Mark 48",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 38000,
                MaxWeaponRangeM = 50000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 100,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 6,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "torpedo62", //gotland
                WeaponClassName = "Saab Bofors Heavyweight Torpedo 62",
                WeaponClassShortName = "Torpedo 62",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 38000,
                MaxWeaponRangeM = 40000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 74,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 6,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "type43x2torpedo", //gotland
                WeaponClassName = "Saab Bofors Lightweight Torpedo Type 43x2",
                WeaponClassShortName = "Type 43x2",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 25000,
                MaxWeaponRangeM = 29000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 64.8,
                DamageHitPoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.7),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 85,
                MaxSeaState = 6,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "spearfish",
                WeaponClassName = "Spearfish Heavyweight Torpedo",
                WeaponClassShortName = "Spearfish",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 50000,
                MaxWeaponRangeM = 54000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 150,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "tp45",
                WeaponClassName = "Type 45 Torpedo",
                WeaponClassShortName = "Tp 45",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 30000, //guess
                MaxWeaponRangeM = 35000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 90,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "aegdm2a3torpedo",
                WeaponClassName = "AEG DM2A3 Heavyweight Torpedo",
                WeaponClassShortName = "AEG DM",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 12000,
                MaxWeaponRangeM = 12000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 62,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "dm2a4torpedo", //Type 212
                WeaponClassName = "DM2A4 Seehecht Heavyweight Torpedo",
                WeaponClassShortName = "DM2A4 Seehecht",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 50000,
                MaxWeaponRangeM = 52000,
                MinWeaponRangeM = 2000,
                MaxSpeedKph = 92,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 8,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mark46torpedo",
                WeaponClassName = "Mark 46 Lightweight Torpedo",
                WeaponClassShortName = "Mark 46",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 11000,
                MaxWeaponRangeM = 12000,
                MinWeaponRangeM = 100,
                MaxSpeedKph = 83,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "mark54torpedo",
                WeaponClassName = "Mark 54 Lightweight Hybrid Torpedo",
                WeaponClassShortName = "Mark 54",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 11000,
                MaxWeaponRangeM = 12000,
                MinWeaponRangeM = 100,
                MaxSpeedKph = 85,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mu90impact",
                WeaponClassName = "MU90 IMPACT Lightweight Torpedo",
                WeaponClassShortName = "MU90/Impact",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 25000,
                MaxWeaponRangeM = 20000,
                MinWeaponRangeM = 100,
                MaxSpeedKph = 93,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);


            wc = new WeaponClass()
            {
                Id = "stingraytorpedo",
                WeaponClassName = "Sting Ray Lightweight Torpedo",
                WeaponClassShortName = "Sting Ray",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 10000,
                MaxWeaponRangeM = 11000,
                MinWeaponRangeM = 100,
                MaxSpeedKph = 83,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);
            
            #endregion //Nato torpedoes and asw weapons
            
            #region "Russian torpedoes and asw weapons"

            wc = new WeaponClass()
            {
                Id = "type5365torpedo",
                WeaponClassName = "Type 53-65M Heavyweight Torpedo", //wake homing
                WeaponClassShortName = "Type 53-65",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 20000,
                MaxWeaponRangeM = 22000,
                MinWeaponRangeM = 500,
                MaxSpeedKph = 81,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 6,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "type6576torpedo",
                WeaponClassName = "Type 65-76 Heavyweight Torpedo",
                WeaponClassShortName = "Type 65-76",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoHeavyweight,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 70000,
                MaxWeaponRangeM = 100000,
                MinWeaponRangeM = 1000,
                MaxSpeedKph = 93,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 4,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "asutorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  //hmm
                HitPercent = 90,
                MaxSeaState = 6,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "apr3etorpedo",
                WeaponClassName = "APR-3E Lightweight Torpedo",
                WeaponClassShortName = "APR-3E",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 4000,
                MaxWeaponRangeM = 5000,
                MinWeaponRangeM = 100,
                MaxSpeedKph = 83,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "va111etorpedo",
                WeaponClassName = "VA-111 Shkval Lightweight Rocket Torpedo",
                WeaponClassShortName = "Shkval",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.TorpedoLight,
                CanFireOnPosition = true,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = true,
                EffectiveWeaponRangeM = 9000,
                MaxWeaponRangeM = 13000,
                MinWeaponRangeM = 400,
                MaxSpeedKph = 370,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "rockettorpedo",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            #endregion //Russian torpedoes and asw weapons

            #region "Rocket Torpedo (ASROC) Weapons"


            wc = new WeaponClass()
            {
                Id = "rum139vlasroc", //Arleigh Burke, other US warships
                WeaponClassName = "RUM-139 VL-ASROC",
                WeaponClassShortName = "RUM-139",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AsRoc,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 21000,
                EffectRangeM = 12000, //for AsRoc, this is used to determine range of torpedo component
                MaxWeaponRangeM = 22000,
                MinWeaponRangeM = 5000,
                MaxSpeedKph = 75,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,  
                HitPercent = 90,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rpk9medvedka", //Gorshkov, Steregushchiy 
                WeaponClassName = "RPK-9 Medvedka 'SS-N-29' AsRoc",
                WeaponClassShortName = "RPK-9 Medvedka",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AsRoc,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 20000,
                EffectRangeM = 12000, //for AsRoc, this is used to determine range of torpedo component
                MaxWeaponRangeM = 20000,
                MinWeaponRangeM = 4000,
                MaxSpeedKph = 75,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 7,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rpk2viyuga", //Kirov, Slava, others
                WeaponClassName = "RPK-2 Viyuga 'SS-N-15' AsRoc",
                WeaponClassShortName = "RPK-2 Viyuga",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AsRoc,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 32000,
                EffectRangeM = 12000, //for AsRoc, this is used to determine range of torpedo component
                MaxWeaponRangeM = 35000,
                MinWeaponRangeM = 4000,
                MaxSpeedKph = 75,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 80,
                MaxSeaState = 7,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rpk7veter", //Lada (sub)
                WeaponClassName = "RPK-7 Veter 'SS-N-16' AsRoc",
                WeaponClassShortName = "RPK-7 Veter",
                WeaponTrajectory = GameConstants.WeaponTrajectoryType.AsRoc,
                CanFireOnPosition = false,
                CanFireOnUnit = true,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = true,
                CanTargetSurface = false,
                CanBeTargetted = false,
                CanFireBearingOnly = false,
                EffectiveWeaponRangeM = 32000,
                EffectRangeM = 12000, //for AsRoc, this is used to determine range of torpedo component
                MaxWeaponRangeM = 35000,
                MinWeaponRangeM = 4000,
                MaxSpeedKph = 75,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                IsECM = false,
                MaxAmmunition = 8,
                MaxSimultanousShots = 2,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "aswtorpedo",
                TimeBetweenShotsSec = 1,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 100,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = false,
                RequiresPlatformFireControl = false,
                HitPercent = 80,
                MaxSeaState = 7,

            };
            list.Add(wc);

            #endregion //ASROC
            #endregion //torpedoes and asw weapons

            #region "Special launchers (non-weapons)"

            wc = new WeaponClass()
            {
                Id = "dicass",
                WeaponClassName = "DICASS AN/SSQ-62 Sonobuoy",
                WeaponClassShortName = "DICASS",
                SpecialOrders = GameConstants.SpecialOrders.DropSonobuoy,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 10,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 12000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "dicass",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 5,

            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "rgbnm",
                WeaponClassName = "RGB-NM Sonobuoy",
                WeaponClassShortName = "RGB-NM",
                SpecialOrders = GameConstants.SpecialOrders.DropSonobuoy,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 10,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 12000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "rgbnm",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 5,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "alq214",
                WeaponClassName = "ALQ-214 RFCM Electronic Countermeasures",
                WeaponClassShortName = "ALQ-214 RFCM",
                SpecialOrders = GameConstants.SpecialOrders.JammerRadarDegradation,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                EffectRangeM = 100000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 2,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 100000,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 600,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 90,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "alq99",
                WeaponClassName = "ALQ-99 Jamming Pod",
                WeaponClassShortName = "ALQ-99 Jammer",
                SpecialOrders = GameConstants.SpecialOrders.JammerRadarDegradation,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                EffectRangeM = 100000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 2,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 100000,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 600,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 90,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "alq227",
                WeaponClassName = "AN/ALQ-227 Comms Countermeasures",
                WeaponClassShortName = "AN/ALQ-227 Comms Jammer",
                SpecialOrders = GameConstants.SpecialOrders.JammerRadarDegradation,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                EffectRangeM = 100000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 2,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 100000,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 600,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 90,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "genericradarjammer",
                WeaponClassName = "Radar Jamming Pod",
                WeaponClassShortName = "Jammer",
                SpecialOrders = GameConstants.SpecialOrders.JammerRadarDegradation,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                EffectRangeM = 100000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 2,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 100000,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 600,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 75,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 9,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "genericcommsjammer",
                WeaponClassName = "Comms Jamming Pod",
                WeaponClassShortName = "Comms Jammer",
                SpecialOrders = GameConstants.SpecialOrders.JammerCommunicationDegradation,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 100000,
                EffectRangeM = 100000,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 2,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 100000,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 600,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 75,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 9,
            };
            list.Add(wc);


            //Chaff
            wc = new WeaponClass()
            {
                Id = "genericchaff", 
                WeaponClassName = "Chaff",
                WeaponClassShortName = "Chaff",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.Chaff,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 5,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 40,
                MaxSeaState = 9,
            };
            list.Add(wc);

            //Flare
            wc = new WeaponClass()
            {
                Id = "genericflare", 
                WeaponClassName = "Flares",
                WeaponClassShortName = "Flares",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.Flare,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 5,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 40,
                MaxSeaState = 9,
            };
            list.Add(wc);

            //Torpedo decoy
            wc = new WeaponClass()
            {
                Id = "generictorpedodecoy", 
                WeaponClassName = "Anti-torpedo decoy",
                WeaponClassShortName = "Torpedo decoy",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.TorpedoDecoy,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 5,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 20,
                MaxSeaState = 9,
            };
            list.Add(wc);
            //Chaff
            wc = new WeaponClass()
            {
                Id = "mk36srboc", 
                WeaponClassName = "MK 36 Super Rapid Bloom Offboard Countermeasures",
                WeaponClassShortName = "MK 36 CounterMeasures",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.Chaff,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 6,
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 30,
                MaxSeaState = 9,
            };
            list.Add(wc);

            //Torp countermeasures
            wc = new WeaponClass()
            {
                Id = "slq25", 
                WeaponClassName = "AN/SLQ-25 NIXIE Torpedo Countermeasures",
                WeaponClassShortName = "AN/SLQ-25 Nixie",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.TorpedoDecoy,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 0, //0 must mean unlimited
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 30,
                MaxSeaState = 9,
            };
            list.Add(wc);

            //Flares
            wc = new WeaponClass()
            {
                Id = "mju40", //F-22
                WeaponClassName = "Chemring MJU-39/40 Flares",
                WeaponClassShortName = "MJU-40 Flares",
                SpecialOrders = GameConstants.SpecialOrders.None,
                EwCounterMeasures = GameConstants.EwCounterMeasuresType.Flare,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                MaxWeaponRangeM = 0,
                EffectiveWeaponRangeM = 0,
                EffectRangeM = 0,
                MinWeaponRangeM = 0,
                DamageHitPoints = 0, //GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = true,
                MaxAmmunition = 6, 
                MaxSimultanousShots = 1,
                SpawnsUnitOnFire = false,
                SpawnUnitClassId = string.Empty,
                TimeBetweenShotsSec = 0,
                WeaponBearingRangeDeg = 360,
                MaxEffectDurationSec = 0,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                StrengthPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 40,
                MaxSeaState = 9,
            };
            list.Add(wc);


            //Naval Mines: MK56 (us) and MDM-5 (ru)
            wc = new WeaponClass()
            {
                Id = "mk56mine",
                WeaponClassName = "MK56 Naval Mine",
                WeaponClassShortName = "MK56",
                SpecialOrders = GameConstants.SpecialOrders.DropMine,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1000,
                MinWeaponRangeM = 0,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 10,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 12000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "mk56mine",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 5,
            };
            list.Add(wc);

            wc = new WeaponClass()
            {
                Id = "mdm5mine",
                WeaponClassName = "MDM-5 Naval Mine",
                WeaponClassShortName = "MDM-5",
                SpecialOrders = GameConstants.SpecialOrders.DropMine,
                CanFireOnPosition = true,
                CanFireOnUnit = false,
                CanTargetAir = false,
                CanTargetLand = false,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                CanBeTargetted = false,
                EffectiveWeaponRangeM = 1000,
                MinWeaponRangeM = 0,
                DamageHitPoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                IsECM = false,
                MaxAmmunition = 10,
                MaxSimultanousShots = 1,
                MaxWeaponRangeM = 12000,
                SpawnsUnitOnFire = true,
                SpawnUnitClassId = "mdm5mine",
                TimeBetweenShotsSec = 10,
                WeaponBearingRangeDeg = 360,
                IncreasesMassByKg = 20,
                IncreasesRangeByPercent = 0,
                IsNotWeapon = true,
                RequiresPlatformFireControl = false,
                HitPercent = 90,
                MaxSeaState = 5,
            };
            list.Add(wc);

            #endregion

            return list;
        }

        public static List<SensorClass> CreateSensorClasses()
        {
            List<SensorClass> list = new List<SensorClass>();

            #region "Radars"
            #region "Nato Radars"
            SensorClass sc = new SensorClass()
            {
                Id = "anspy1d",
                SensorClassName = "AN/SPY-1D Radar",
                SensorClassShortName = "SPY-1D",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 10,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC, 
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anspy1f",
                SensorClassName = "AN/SPY-1F Radar",
                SensorClassShortName = "SPY-1F",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anspy3",//Zumwalt
                SensorClassName = "AN/SPY-3 Radar",
                SensorClassShortName = "SPY-3",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mpq64sentinel",
                SensorClassName = "AN/MPQ-64 Sentinel Radar",
                SensorClassShortName = "MPQ-64 Sentinel",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mpq65patriot",
                SensorClassName = "AN/MPQ-65 Patriot Radar",
                SensorClassShortName = "MPQ-65 Patriot",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_EXCELLENT_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ocfcs2000",
                SensorClassName = "Oerlikon Contraves FCS2000 Radar",
                SensorClassShortName = "OC FCS2000",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_EXCELLENT_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "sampson",
                SensorClassName = "SAMPSON Radar",
                SensorClassShortName = "SAMPSON",
                AESAfactorPercent = 5,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 500000,
                EsmDetectionOverHorizonPercent = 180,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_EXCELLENT_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_EXCELLENT_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "s1850m",
                SensorClassName = "S1850M Radar",
                SensorClassShortName = "S1850M Radar",
                AESAfactorPercent = 100,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);



            sc = new SensorClass()
            {
                Id = "baetype996",
                SensorClassName = " BAE Type 996 Md 1 3D Radar",
                SensorClassShortName = "BAE Type 996",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "selexgalileo5000", //AW101
                SensorClassName = "Selex Galileo Blue Kestrel 5000",
                SensorClassShortName = "Selex Galileo 5000",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ravenasea", //gripen
                SensorClassName = "Saab Selex Galileo Raven",
                SensorClassShortName = "Raven",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ansps48e",
                SensorClassName = "AN/SPS-48E Air Search Radar",
                SensorClassShortName = "SPS-48E",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ansps67",
                SensorClassName = "AN/SPS-67 Surface Radar",
                SensorClassShortName = "SPS-48E",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                //AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "captor",
                SensorClassName = "Euroradar CAPTOR",
                SensorClassShortName = "Euroradar Captor",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "seagiraffe",
                SensorClassName = "Ericsson Sea Giraffe AMB 3D radar",
                SensorClassShortName = "Sea Giraffe",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ericsongiraffe",
                SensorClassName = "Ericsson Giraffe 3D radar",
                SensorClassShortName = "Giraffe",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "amdaggerradar",
                SensorClassName = "Alenia Marconi Dagger Radar",
                SensorClassShortName = "AM Dagger",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapq181", //B-2 Spirit
                SensorClassName = "AN/APQ-181 Radar",
                SensorClassShortName = "APQ-181",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ansps49",
                SensorClassName = "AN/SPS-49 Air Search Radar",
                SensorClassShortName = "SPS-49",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapg66",
                SensorClassName = "AN/APG-66 Radar",
                SensorClassShortName = "APG-66",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapg77",
                SensorClassName = "AN/APG-77 Radar",
                SensorClassShortName = "APG-77",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapg79",
                SensorClassName = "AN/APG-79 Radar",
                SensorClassShortName = "APG-79",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapg81",
                SensorClassName = "AN/APG-81 Radar",
                SensorClassShortName = "APG-81",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 15,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "enrthales",
                SensorClassName = "Thales European Navy Radar",
                SensorClassShortName = "ThalesENR",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "thaleslw08",
                SensorClassName = "Thales LW08 Air Search Radar",
                SensorClassShortName = "Thales LW08",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "smarts",
                SensorClassName = "Thales SMART-S Radar",
                SensorClassShortName = "SMART-S",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                //AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "smartl",
                SensorClassName = "Thales SMART-L Radar",
                SensorClassShortName = "SMART-L",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anaps124",
                SensorClassName = "AN/APS-124 Radar",
                SensorClassShortName = "APS-124",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 80,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapy8", //mq9 mariner, pegasus
                SensorClassName = "AN/APY-8 Lynx II Radar",
                SensorClassShortName = "APY-8 Radar",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 40,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapy10", //Poseidon
                SensorClassName = "AN/APY-10 Radar",
                SensorClassShortName = "APY-10",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 600000,
                EsmDetectionOverHorizonPercent = 200,
                AESAfactorPercent = 15,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anaps137", //P-3 Orion
                SensorClassName = "AN/APS-137(V) ",
                SensorClassShortName = "APS-137",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 600000,
                EsmDetectionOverHorizonPercent = 200,
                AESAfactorPercent = 15,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anaps145",
                SensorClassName = "AN/APS-145 Radar",
                SensorClassShortName = "APS-145",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 600000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anapy1",
                SensorClassName = "AN/APY-1/2 Radar",
                SensorClassShortName = "APY-1/2",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 600000,
                EsmDetectionOverHorizonPercent = 200,
                AESAfactorPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "analr94",
                SensorClassName = "AN/ALR-94 RWR",
                SensorClassShortName = "ALR-94",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "roresm",
                SensorClassName = "Racal Orange Reaper ESM",
                SensorClassShortName = "ROR ESM",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "analr73",
                SensorClassName = "AN/ALR-73 RWR",
                SensorClassShortName = "ALR-73",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "analq142",
                SensorClassName = "AN/ALQ-142 ESM system ",
                SensorClassShortName = "ALQ-142",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "kelvinhughes1007",
                SensorClassName = "Kelvin Hughes 1007 Surface Search Radar",
                SensorClassShortName = "KH 1007",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 125,
                TransmitterPowerW = 400000,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "sar",  //Camcopter
                SensorClassName = "Synthetic Aperture Radar",
                SensorClassShortName = "SAR",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 400000,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_WEAK_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_WEAK_ARC_SEC,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);
            sc = new SensorClass()
            {
                Id = "eadstrs3d", //hamina
                SensorClassName = "EADS TRS-3D/16-ES 3D Radar",
                SensorClassShortName = "EADS 3D",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 400000,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "matildarwr",
                SensorClassName = "Thales Matilda RWR",
                SensorClassShortName = "Matilda",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "thalesmrr3d",
                SensorClassName = "Thales MRR/3D NG Radar",
                SensorClassShortName = "ThalesMRR",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 400000,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anfps117",
                SensorClassName = "AN/FPS-117 Radar",
                SensorClassShortName = "FPS-117",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 200,
                TransmitterPowerW = 400000,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            #endregion

            #region "Russian radars"

            sc = new SensorClass()
            {
                Id = "mr710",
                SensorClassName = "MR-710 Fregat-MA 'Top Plate' 3D Radar",
                SensorClassShortName = "MR-710",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 10,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mr320",
                SensorClassName = "MR-320 Topaz 'Strut Pair' 2D Radar",
                SensorClassShortName = "MR-320",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mr800",
                SensorClassName = "MR-800 Voskhod 'Top Pair' 3D Radar",
                SensorClassShortName = "MR-800",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mae4k", //Project 21956
                SensorClassName = "MAE-4K air search radar",
                SensorClassShortName = "MAE-4K ",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 10,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mineralme", //Project 21956
                SensorClassName = "Mineral-ME surface search radar",
                SensorClassShortName = "Mineral-ME",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 10,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "s400triumfradar",
                SensorClassName = "S-400 Triumf Radar",
                SensorClassShortName = "S-400",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "torradar",
                SensorClassName = "TOR Radar",
                SensorClassShortName = "TOR",
                AESAfactorPercent = 10,
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "furkee",
                SensorClassName = "Furke-E 3D Radar",
                SensorClassShortName = "Furke-E",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "garpunb",
                SensorClassName = "Garpun-B 'Plank Shave' Radar",
                SensorClassShortName = "Garpun-B",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "chiblis", //akula
                SensorClassName = "Chiblis 'Park Lamp' Radar",
                SensorClassShortName = "Chiblis",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false, //esm only
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "myedvyeditsa971", //yasen
                SensorClassName = "Myedvyeditsa-971 Radar",
                SensorClassShortName = "Myedvyeditsa",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true, 
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "irbise", //PAK FA
                SensorClassName = "EGSP-27 Irbis-E 3D Radar",
                SensorClassShortName = "Irbis-E",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "obzork", //Su-160
                SensorClassName = "Obzor-K Radar",
                SensorClassShortName = "Obzor-K",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                //AESAfactorPercent = 20,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "e801", //Ka-27
                SensorClassName = "E-801 Oko Radar",
                SensorClassShortName = "E-801 Oko",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_WEAK_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_WEAK_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "pnad",
                SensorClassName = "Leninets PNA-D Radar",
                SensorClassShortName = "PNA-D",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "zhukme", //Mig29K
                SensorClassName = "Zhuk-ME Radar",
                SensorClassShortName = "Zhuk-ME",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "pnab", //Tu95
                SensorClassName = "Leninets Rubin PNA-B 'Down Beat' Radar",
                SensorClassShortName = " PNA-B",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "shmelm", //Beriev
                SensorClassName = "Vega Shmel-M Radar",
                SensorClassShortName = " Shmel-M",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 150,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_STRONG_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            #endregion

            #region "Other radars"

            sc = new SensorClass()
            {
                Id = "ruttersigmas6",
                SensorClassName = "Rutter Sigma S6 Radar",
                SensorClassShortName = "Rutter Sigma S6",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 200000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = false,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "missileradar",
                SensorClassName = "Missile Guidance Radar",
                SensorClassShortName = "Missile Radar",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 100000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false, //true!
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "missileradarsemi",
                SensorClassName = "Missile Guidance Semi-Active Radar",
                SensorClassShortName = "Missile Radar",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 200000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false, //true!
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = true,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_MEDIUM_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_MEDIUM_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "civilianradar",
                SensorClassName = "Civilian Radar",
                SensorClassShortName = "Civilian Radar",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 200000,
                EsmDetectionOverHorizonPercent = 100,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = false,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_WEAK_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_WEAK_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "airportradar",
                SensorClassName = "Airport Radar",
                SensorClassShortName = "Airport Radar",
                SensorType = GameConstants.SensorType.Radar,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 400000,
                EsmDetectionOverHorizonPercent = 200,
                TransmitterPowerW = 4000000,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsEsmDetector = false,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                MinimumTargetSurfaceSizeArcSec = GameConstants.RADAR_TARGET_SURFACE_VERY_WEAK_ARC_SEC,
                MinimumTargetAirSizeArcSec = GameConstants.RADAR_TARGET_AIR_VERY_STRONG_ARC_SEC,
                SensorBearingRangeDeg = 360,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            
            #endregion //other radars
            #endregion //radars

            #region "Sonars"

            #region "Nato sonars"
            sc = new SensorClass()
            {
                Id = "ansqs53", //Burke, Tico
                SensorClassName = "AN/SQS-53 Sonar",
                SensorClassShortName = "SQS-53",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anbqq6", //Ohio
                SensorClassName = "AN/BQQ-6 Sonar",
                SensorClassShortName = "BQQ-53",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anbqs13", //Ohio
                SensorClassName = "AN/BQS-13 Sonar",
                SensorClassShortName = "BQS-13",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ansqs510", //Halifax
                SensorClassName = "AN/SQS-510 Sonar",
                SensorClassShortName = "SQS-510",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mfs7000", //Daring 
                SensorClassName = "UE/EDO MFS-7000 sonar",
                SensorClassShortName = "MFS-7000",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "type2079", //Astute
                SensorClassName = "Thales Sonar Type 2079",
                SensorClassShortName = "Thales Sonar 2079",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveExcellent),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveExcellent),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "type2050", //Type 23 Frigate
                SensorClassName = "Thales Sonar Type 2050",
                SensorClassShortName = "Thales Sonar 2050",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "hydrosciencetowed", //visby
                SensorClassName = "Hydroscience Technologies Towed array sonar",
                SensorClassShortName = "HT Towed",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayLow),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayLow),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "gdc",  //Visby
                SensorClassName = "GDC Hull-Mounted Sonar",
                SensorClassShortName = "GDC Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveMedium),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveMedium),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "type2016", //unknown...?
                SensorClassName = "Thales Sonar Type 2016",
                SensorClassShortName = "Thales Sonar 2016",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "type2076", //Astute
                SensorClassName = "Type 2076 Flank Array",
                SensorClassShortName = "Thales Sonar 2076",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "type2087", //Type 23 Frigate
                SensorClassName = "Type 2087 Towed Sonar",
                SensorClassShortName = "Type 2087 Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "ansqr501", //Halifax
                SensorClassName = "AN/SQR-501 CANTASS Towed Sonar Array",
                SensorClassShortName = "SQR-501 Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "toadfish", //hamina
                SensorClassName = "Simrad Subsea Toadfish sonar",
                SensorClassShortName = "Toadfish Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveMedium),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveMedium),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "tas3towed", //Type 212
                SensorClassName = "TAS-3 Towed array sonar",
                SensorClassShortName = "TAS-3 Towed",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "fas3hull", //Type 212
                SensorClassName = "FAS-3 Flank Sonar",
                SensorClassShortName = "FAS-3 Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveExcellent),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "sonacpta", //hamina
                SensorClassName = "Sonac/PTA Towed Array Sonar",
                SensorClassShortName = "Sonac/PTA",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayLow),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayLow),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "ansqr19", //Tico
                SensorClassName = "AN/SQR-19 Towed Array Sonar",
                SensorClassShortName = "SQR-19",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,  //na, passive only
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),  
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "type2065", //Astute
                SensorClassName = "Type 2065 Towed Array Sonar",
                SensorClassShortName = "SQR-19",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,  //na, passive only
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "csu83", //ula
                SensorClassName = "Atlas Elektronik CSU83 Sonar",
                SensorClassShortName = "Atlas CSU83",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveMedium),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood), 
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "csu902bow", //gotland
                SensorClassName = "Atlas Elektronik CSU 90-2 Bow Sonar",
                SensorClassShortName = "CSU 90-2 Bow",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "csu902flank", //gotland
                SensorClassName = "Atlas Elektronik CSU 90-2 Flank Sonar",
                SensorClassShortName = "CSU 90-2 Flank",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0, 
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "csu902intercept", //gotland
                SensorClassName = "Atlas Elektronik CSU 90-2 Intercept Sonar",
                SensorClassShortName = "CSU 90-2 Icpt",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "aso94", //iver huitfeldt
                SensorClassName = "Atlas Elektronik ASO94 Sonar",
                SensorClassShortName = "Atlas ASO94",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveMedium),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveMedium),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "dsqs24b", //sachsen, others
                SensorClassName = "Atlas Elektronik DSQS-24B Sonar",
                SensorClassShortName = "Atlas ASO94",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "lfasstowed", //brandenburg
                SensorClassName = "STN Atlas LFASS Towed Array Sonar",
                SensorClassShortName = "LFASS Towed",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,   //na, passive only
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "tsm2253", //ula
                SensorClassName = "TSM 2253 Large Array Flank Sonar",
                SensorClassShortName = "TSM 2253",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 10,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "anbqq5",
                SensorClassName = "AN/BQQ-5 Bow-Mounted Spherical Array Sonar ", //virginia
                SensorClassShortName = "BQQ-5",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "tb29",
                SensorClassName = "TB-29 Towed Array Sonar", //Virginia class
                SensorClassShortName = "TB-29",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,   //na, passive only
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "tb16",
                SensorClassName = "TB-16 Towed Array Sonar", //Ohio
                SensorClassShortName = "TB-16",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = 0,  //na, passive only
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 360,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mrs2000", //nansen
                SensorClassName = "Spherion MRS 2000 Sonar",
                SensorClassShortName = "MRS 2000",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "captasmk2", //nansen
                SensorClassName = "CAPTAS Mk2V1 Towed Array Sonar", //note, this towed array is active/passive
                SensorClassShortName = "Captas Mk2V1",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.VHF_LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 360,
            };
            list.Add(sc);



            sc = new SensorClass()
            {
                Id = "anssq62",
                SensorClassName = "AN/SSQ-62 DICASS Sonobuoy Sonar",
                SensorClassShortName = "SSQ-62",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.DippingOrSonobuoy,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveSononbuoy),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveSonobuoy),  
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 60,

            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "aqs18a",
                SensorClassName = "AN/AQS-18A Dipping Sonar",
                SensorClassShortName = "AQS-18A",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.DippingOrSonobuoy,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsVariableDepthSensor = true,
                IsDeployableSensor = true,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),  
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 20,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 60,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "sintraflash",
                SensorClassName = "Thomson Sintra FLASH Dipping Sonar",
                SensorClassShortName = "Sintra Flash",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.DippingOrSonobuoy,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsVariableDepthSensor = true,
                IsDeployableSensor = true,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),  
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 20,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 60,

            };
            list.Add(sc);


            #endregion //nato sonars

            #region "Russian sonars"

            sc = new SensorClass()
            {
                Id = "mgk355bow", //Kirov
                SensorClassName = "MGK-355 Polinom 'Horse Jaw' Bow-Mounted Sonar",
                SensorClassShortName = "MGK-355 Bow",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mg332bow", //Slava
                SensorClassName = "MG-332 Tigan-2T 'Bull Nose' Bow-Mounted Sonar",
                SensorClassShortName = "MG-332 Bow",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mgk355tail", //Slava
                SensorClassName = "MGK-355 Polinom 'Horse Tail' VD Sonar",
                SensorClassShortName = "MGK-355 VS",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedKeel,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                IdentifyDetectionStrength = 2,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 120,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mgk345bow", //Kuznetsov
                SensorClassName = "MGK-345 Bronza 'Ox Jaw' Bow-Mounted Sonar",
                SensorClassShortName = "MGK-345 Bow",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                IdentifyDetectionStrength = 2,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveMedium),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveMedium),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mgk345tail", //Kuznetsov, Udaloy
                SensorClassName = "MGK-345 Bronza 'Ox Tail' VD Sonar",
                SensorClassShortName = "MGK-345 VD",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedKeel,
                CanTargetAir = false,
                IdentifyDetectionStrength = 2,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 120,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "zaryame", //Steregushcky
                SensorClassName = "Zarya-ME Bow-Mounted Sonar",
                SensorClassShortName = "Zarya-ME",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "vinyetka", //Steregushchiy
                SensorClassName = "Vinyetka LF Towed Array Sonar",
                SensorClassShortName = "Vinyetka",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 120,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "lirabow", //lada
                SensorClassName = "Lira Bow-Mounted Sonar",
                SensorClassShortName = "Lira Bow",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "liratail", //Lada
                SensorClassName = "Lira Flank Array Sonar",
                SensorClassShortName = "Lira Tail",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 120,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "liratowed", //Lada
                SensorClassName = "Lira Towed Array Sonar",
                SensorClassShortName = "Lira Towed",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 30,
                TimeToDeploySec = 120,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "mgk540bow", //AkulaII
                SensorClassName = "MGK-540 Bow-Mounted Sonar",
                SensorClassShortName = "MGK-540 Bow",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mgk540atail", //Akula
                SensorClassName = "MGK-540 Flank Array Sonar",
                SensorClassShortName = "MGK-540 Flank",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 120,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mgk500bow", //Yasen
                SensorClassName = "Irtysh/Amfora MGK-500 'Shark Gill' Sonar",
                SensorClassShortName = "MGK-500 Bow",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "mg519flank", //yasen
                SensorClassName = "MG-519 Arfa 'Mouse Roar' Flank Sonar",
                SensorClassShortName = "MG-519",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedFlankArray,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = true,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "pelamida", //akula
                SensorClassName = "Pelamida Towed Array Sonar",
                SensorClassShortName = "Palemida",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveTowedArrayHigh),
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 24,
                TimeToDeploySec = 120,
            };
            list.Add(sc);
            
            sc = new SensorClass()
            {
                Id = "skat3towed", //Yasen
                SensorClassName = "Irtysh/Amfora Skat 3 Towed Array Sonar",
                SensorClassShortName = "Skat 3",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.TowedArray,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveTowedArrayHigh),
                SonarPassiveReferenceRangeM = 0,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 24,
                TimeToDeploySec = 120,
            };
            list.Add(sc);


            sc = new SensorClass()
            {
                Id = "vgs-3", //KA27
                SensorClassName = "VGS-3 'Lamb Tail' Dipping Sonar",
                SensorClassShortName = "VGS-3",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.DippingOrSonobuoy,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                IdentifyDetectionStrength = 2,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = true,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveSononbuoy),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveSonobuoy),
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 20,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 60,

            };
            list.Add(sc);
            
            sc = new SensorClass()
            {
                Id = "rgbnm", //russian
                SensorClassName = "RGB-NM Sonobuoy Sonar",
                SensorClassShortName = "RGB-NM",
                IdentifyDetectionStrength = 2,
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.DippingOrSonobuoy,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.LF_MF,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = true,
                IsVariableDepthSensor = true,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveSononbuoy),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveSonobuoy),
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 20,
                MaxSpeedDeployedKph = 40,
                TimeToDeploySec = 60,

            };
            list.Add(sc);
            #endregion //russian sonars

            #region "Other sonars"
            sc = new SensorClass()
            {
                Id = "torpedosonar", 
                SensorClassName = "Torpedo Sonar",
                SensorClassShortName = "Torpedo Sonar",
                SensorType = GameConstants.SensorType.Sonar,
                SonarType = GameConstants.SonarType.HullMountedBow,
                SonarFrequencyBand = GameConstants.SonarFrequencyBands.MF,
                CanTargetAir = false,
                CanTargetSubmarine = true,
                CanTargetSurface = true,
                IdentifyDetectionStrength = 1.1,
                MaxRangeM = 50000,
                TransmitterPowerW = 40000,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = true,
                IsTargetingSensorOnly = false, //true?
                IsEsmDetector = false,
                SonarActiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.ActiveVeryGood),
                SonarPassiveReferenceRangeM = GameConstants.GetSonarReferenceRangeM(GameConstants.SonarStrengthTypes.PassiveVeryGood), 
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 60,

            };
            list.Add(sc);

            #endregion //Other sonars
            #endregion //Sonars

            #region "Visual, IR and MAD"

            sc = new SensorClass()
            {
                Id = "visual",
                SensorClassName = "Visual",
                SensorClassShortName = "Visual",
                SensorType = GameConstants.SensorType.Visual,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 900,
                MinimumTargetAirSizeArcSec = 450,
                IdentifyDetectionStrength = 5,
                IsDeployableSensor=false,
                IsVariableDepthSensor=false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "aar56",
                SensorClassName = "AN/AAR 56 Infra-Red", //F-22
                SensorClassShortName = "AAR 56",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 80000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 5,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "pirateir",
                SensorClassName = "PIRATE Infrared", //Typhoon
                SensorClassShortName = "Pirate IR",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 5,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "genericir",
                SensorClassName = "Infrared Seeker", //Generic IR
                SensorClassShortName = "Infrared",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 5,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = false,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "schiebelir",
                SensorClassName = "Schiebel IR Seeker",
                SensorClassShortName = "Schiebel IR",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 2,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = true,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,
            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "irasu",
                SensorClassName = "ASu IR Seeker",
                SensorClassShortName = "ASu IR",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = false,
                CanTargetSubmarine = false,
                CanTargetSurface = true,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 2,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = true,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            sc = new SensorClass()
            {
                Id = "iraa",
                SensorClassName = "AA IR Seeker",
                SensorClassShortName = "AA IR",
                SensorType = GameConstants.SensorType.Infrared,
                CanTargetAir = true,
                CanTargetSubmarine = false,
                CanTargetSurface = false,
                MaxRangeM = 40000,
                TransmitterPowerW = 0,
                MinimumTargetSurfaceSizeArcSec = 3600,
                MinimumTargetAirSizeArcSec = 1800,
                IdentifyDetectionStrength = 2,
                IsDeployableSensor = false,
                IsVariableDepthSensor = false,
                IsBearingDetectorOnly = false,
                IsPassiveActiveSensor = false,
                IsTargetingSensorOnly = true,
                IsEsmDetector = false,
                SensorBearingRangeDeg = 360,
                MaxHeightDeployedM = 0,
                MaxSpeedDeployedKph = 0,
                TimeToDeploySec = 0,

            };
            list.Add(sc);

            #endregion

            return list;
        }

        public static List<Country> CreateCountries()
        {
            List<Country> countryList = new List<Country>();
            Country country = new Country()
            {
                Id = "no",
                CountryNameShort = "Norway",
                CountryNameLong = "Kingdom of Norway"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "ru",
                CountryNameShort = "Russia",
                CountryNameLong = "Russian Federation"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "us",
                CountryNameShort = "USA",
                CountryNameLong = "United States of America"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "uk",
                CountryNameShort = "UK",
                CountryNameLong = "United Kingdom"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "se",
                CountryNameShort = "Sweden",
                CountryNameLong = "Kingdom of Sweden"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "dk",
                CountryNameShort = "Denmark",
                CountryNameLong = "Kingdom of Denmark"
            };
            countryList.Add(country);

            country = new Country()
            {
                Id = "de",
                CountryNameShort = "Germany",
                CountryNameLong = "Federal Republic of Germany"
            };
            countryList.Add(country);

            return countryList;
        }

        public static List<Formation> CreateFormations()
        {
            List<Formation> FormationList = new List<Formation>()
            {
                new Formation()
                {
                    Id="aircraftdefault",
                    Description="Default formation for aircraft",
                    IsAircraftFormation = true,
                    FormationPositions = new List<FormationPosition>()
                    {
                        new FormationPosition()
                        {
                            Id="center",
                            PositionOffset = new PositionOffset(0,0,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left1",
                            PositionOffset = new PositionOffset(-50,-20, 0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right1",
                            PositionOffset = new PositionOffset(50,-20,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left2",
                            PositionOffset = new PositionOffset(-100,-40,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right2",
                            PositionOffset = new PositionOffset(100,-40,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left3",
                            PositionOffset = new PositionOffset(-150,-60,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right3",
                            PositionOffset = new PositionOffset(150,-60,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="behindleft1",
                            PositionOffset = new PositionOffset(-50,-160,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="behindright1",
                            PositionOffset = new PositionOffset(50,-160,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="behindleft2",
                            PositionOffset = new PositionOffset(-100,-160,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="behindright2",
                            PositionOffset = new PositionOffset(100,-160,0),
                            UnitType = null,
                        },

                        new FormationPosition()
                        {
                            Id="left4",
                            PositionOffset = new PositionOffset(-200,-60,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right4",
                            PositionOffset = new PositionOffset(200,-60,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left5",
                            PositionOffset = new PositionOffset(-250,-60,0),
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right5",
                            PositionOffset = new PositionOffset(250,-60,0),
                            UnitType = null,
                        },

                    }
                },
                new Formation()
                {
                    Id="fighters1",
                    Description="Default formation for fighters",
                    IsAircraftFormation = true,
                    FormationPositions = new List<FormationPosition>()
                    {
                        new FormationPosition()
                        {
                            Id="center",
                            PositionOffset = new PositionOffset(0,0,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left1",
                            PositionOffset = new PositionOffset(-50,-20, 0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right1",
                            PositionOffset = new PositionOffset(50,-20,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left2",
                            PositionOffset = new PositionOffset(-100,-40,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right2",
                            PositionOffset = new PositionOffset(100,-40,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="left3",
                            PositionOffset = new PositionOffset(-150,-60,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },
                        new FormationPosition()
                        {
                            Id="right3",
                            PositionOffset = new PositionOffset(150,-60,0),
                            Role = GameConstants.Role.InterceptAircraft,
                            UnitType = null,
                        },


                    }
                },
                new Formation()
                {
                    Id="sagdefault",
                    Description="Surface action group",
                    IsAircraftFormation = false,
                    FormationPositions = new List<FormationPosition>()
                    {
                        new FormationPosition()
                        {
                            Id="center",
                            PositionOffset = new PositionOffset(0,0,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="followright",
                            PositionOffset = new PositionOffset(700,-500,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="followleft",
                            PositionOffset = new PositionOffset(-700,-500,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="leadright",
                            PositionOffset = new PositionOffset(600,1000,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="leadleft",
                            PositionOffset = new PositionOffset(-600,1000,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="closeright",
                            PositionOffset = new PositionOffset(500,-50,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="closeleft",
                            PositionOffset = new PositionOffset(-500,-50,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="aswright",
                            PositionOffset = new PositionOffset(2000,500,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="aswleft",
                            PositionOffset = new PositionOffset(-2000,500,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="aswlead",
                            PositionOffset = new PositionOffset(0,2000,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 
                        new FormationPosition()
                        {
                            Id="aswfollow",
                            PositionOffset = new PositionOffset(0,-1000,0),
                            Role = GameConstants.Role.IsSurfaceCombattant,
                            UnitType = null,
                        }, 


                    }
                }
            };
            return FormationList;
        }
        
        public static List<GameScenario> CreateScenarios()
        {
            return ScenarioDatabaseFactory.CreateScenarios();
        }

        //public static List<User> CreateUsers()
        //{
        //    var userList = new List<User>()
        //    {
        //        new User()
        //        {
        //            UserId = "Jan",

        //        }
        //    };

        //    return userList;
        //}

        public static List<Campaign> CreateCampaigns()
        {
            var campaignList = new List<Campaign>()
            {
#region Arctic War - NATO
                new Campaign()
                {
                    Id="aw_nato",
                    Name="Arctic War - NATO",
                    Description="CAMPAIGN_01_DESCRIPTION",
                    PreviewImage = "ScenarioPreview/campaign_nato",
                    Icon = "Icons/icon_nato",
                    EndNews = new NewsReport()
                    {
                        Header = "NEWS_C01_END_HEADER",
                        SubHeader = "NEWS_C01_END_SUBHEADER",
                        Body = "NEWS_C01_END_BODY",
                        Image = "Illustrations/news_epilogue"
                    },
                    CampaignScenarios = new List<CampaignScenario>()
                    {
#region Tutorial 1
                        new CampaignScenario()
                        {
                            ScenarioId="tutorialtest",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,15)),
                                Header = "NEWS_TUTORIAL1_HEADER",
                                SubHeader = "NEWS_TUTORIAL1_SUBHEADER",
                                Body = "NEWS_TUTORIAL1_BODY",
                                Image = "Illustrations/news_tutorial01"
                            },
                            DialogBackgroundImage = "Backgrounds/kossels_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL1_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL1_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL1_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL1_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL1_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL1_6"
                                },
                            }
                        },
#endregion
#region Tutorial 2
                        new CampaignScenario()
                        {
                            ScenarioId="tutorial2",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_TUTORIAL2_HEADER",
                                SubHeader = "NEWS_TUTORIAL2_SUBHEADER",
                                Body = "NEWS_TUTORIAL2_BODY",
                                Image = "Illustrations/news_tutorial02",
                                Date = new NWDateTime(new DateTime(2030,6,21)),
                            },
                            DialogBackgroundImage = "Backgrounds/kossels_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL2_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL2_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL2_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL2_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL2_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL2_6"
                                },
                            }
                        },
#endregion
#region Tutorial 3
                        new CampaignScenario()
                        {
                            ScenarioId="tutorial3",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_TUTORIAL3_HEADER",
                                SubHeader = "NEWS_TUTORIAL3_SUBHEADER",
                                Body = "NEWS_TUTORIAL3_BODY",
                                Image = "Illustrations/news_tutorial03",
                                Date = new NWDateTime(new DateTime(2030,6,29)),
                            },
                            DialogBackgroundImage = "Backgrounds/kossels_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL3_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL3_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_TUTORIAL3_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL3_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_TUTORIAL3_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_TUTORIAL3_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_TUTORIAL3_7"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_TUTORIAL3_8"
                                },
                            }
                        },
#endregion

#region Scenario 1
                        new CampaignScenario()
                        {
                            ScenarioId="c01s01",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S01_HEADER",
                                SubHeader = "NEWS_C01S01_SUBHEADER",
                                Body = "NEWS_C01S01_BODY",
                                Image = "Illustrations/news_C01S01",
                                Date = new NWDateTime(new DateTime(2030,6,21)),
                            },
                            DialogBackgroundImage = "Backgrounds/vessel_interior",
                            DialogBackgroundSound = "Sound/155 ship office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_C01S01_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S01_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_C01S01_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S01_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_C01S01_5"
                                }
                            }
                        },
#endregion
#region Scenario 2
                        new CampaignScenario()
                        {
                            ScenarioId="c01s02",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S02_HEADER",
                                SubHeader = "NEWS_C01S02_SUBHEADER",
                                Body = "NEWS_C01S02_BODY",
                                Image = "Illustrations/news_C01S02"
                            },
                            DialogBackgroundImage = "Backgrounds/oslo_office",
                            DialogBackgroundSound = "Sound/150 london office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S02_1"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik1",
                                    Dialog = "DIALOG_C01S02_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S02_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S02_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S02_5"
                                }
                            }
                        },
#endregion
#region Scenario 3
                        new CampaignScenario()
                        {
                            ScenarioId="c01s03",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S03_HEADER",
                                SubHeader = "NEWS_C01S03_SUBHEADER",
                                Body = "NEWS_C01S03_BODY",
                                Image = "Illustrations/news_C01S03"
                            },
                            DialogBackgroundImage = "Backgrounds/kossels_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S03_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S03_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S03_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S03_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S03_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S03_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S03_7"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S03_8"
                                }
                            }
                        },
#endregion
#region Scenario 4
                        new CampaignScenario()
                        {
                            ScenarioId="c01s04",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S04_HEADER",
                                SubHeader = "NEWS_C01S04_SUBHEADER",
                                Body = "NEWS_C01S04_BODY",
                                Image = "Illustrations/news_C01S04"
                            },
                            DialogBackgroundImage = "Backgrounds/vessel_interior",
                            DialogBackgroundSound = "Sound/155 ship office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S04_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S04_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S04_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S04_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S04_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S04_6"
                                },
                            }
                        },
#endregion
#region Scenario 5
                        new CampaignScenario()
                        {
                            ScenarioId="c01s05",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S05_HEADER",
                                SubHeader = "NEWS_C01S05_SUBHEADER",
                                Body = "NEWS_C01S05_BODY",
                                Image = "Illustrations/news_C01S05"
                            },
                            DialogBackgroundImage = "Backgrounds/oslo_office",
                            DialogBackgroundSound = "Sound/150 london office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S05_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S05_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S05_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S05_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S05_5"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S05_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S05_7"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S05_8"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S05_9"
                                },
                            }
                        },
#endregion
#region Scenario 6
                        new CampaignScenario()
                        {
                            ScenarioId="c01s06",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S06_HEADER",
                                SubHeader = "NEWS_C01S06_SUBHEADER",
                                Body = "NEWS_C01S06_BODY",
                                Image = "Illustrations/news_C01S06"
                            },
                            DialogBackgroundImage = "Backgrounds/cochranes_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S06_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S06_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S06_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S06_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S06_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S06_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S06_7"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S06_8"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S06_9"
                                },
                            }
                        },
#endregion
#region Scenario 7
                        new CampaignScenario()
                        {
                            ScenarioId="c01s07",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S07_HEADER",
                                SubHeader = "NEWS_C01S07_SUBHEADER",
                                Body = "NEWS_C01S07_BODY",
                                Image = "Illustrations/news_C01S07"
                            },
                            DialogBackgroundImage = "Backgrounds/vessel_interior",
                            DialogBackgroundSound = "Sound/155 ship office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S07_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S07_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S07_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S07_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "moltevik2",
                                    Dialog = "DIALOG_C01S07_5"
                                },
                            }
                        },
#endregion
#region Scenario 8
                        new CampaignScenario()
                        {
                            ScenarioId="c01s08",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S08_HEADER",
                                SubHeader = "NEWS_C01S08_SUBHEADER",
                                Body = "NEWS_C01S08_BODY",
                                Image = "Illustrations/news_C01S08"
                            },
                            DialogBackgroundImage = "Backgrounds/cochranes_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S08_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S08_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S08_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S08_4"
                                },
                            }
                        },
#endregion
#region Scenario 9
                        new CampaignScenario()
                        {
                            ScenarioId="c01s09",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S09_HEADER",
                                SubHeader = "NEWS_C01S09_SUBHEADER",
                                Body = "NEWS_C01S09_BODY",
                                Image = "Illustrations/news_C01S09"
                            },
                            DialogBackgroundImage = "Backgrounds/london_office",
                            DialogBackgroundSound = "Sound/150 london office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S09_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S09_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S09_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S09_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S09_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S09_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S09_7"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S09_8"
                                },
                            }
                        },
#endregion
#region Scenario 10
                        new CampaignScenario()
                        {
                            ScenarioId="c01s10",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S10_HEADER",
                                SubHeader = "NEWS_C01S10_SUBHEADER",
                                Body = "NEWS_C01S10_BODY",
                                Image = "Illustrations/news_C01S10"
                            },
                            DialogBackgroundImage = "Backgrounds/london_office",
                            DialogBackgroundSound = "Sound/150 london office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S10_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S10_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S10_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S10_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "barth_westerdahl",
                                    Dialog = "DIALOG_C01S10_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S10_6"
                                },
                            }
                        },
#endregion
#region Scenario 11
                        new CampaignScenario()
                        {
                            ScenarioId="c01s11",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S11_HEADER",
                                SubHeader = "NEWS_C01S11_SUBHEADER",
                                Body = "NEWS_C01S11_BODY",
                                Image = "Illustrations/news_C01S11"
                            },
                            DialogBackgroundImage = "Backgrounds/sea_night",
                            DialogBackgroundSound = "Sound/153 sea at night",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S11_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S11_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S11_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S11_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "cochrane",
                                    Dialog = "DIALOG_C01S11_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S11_6"
                                },
                            }
                        },
#endregion
#region Scenario 12
                        new CampaignScenario()
                        {
                            ScenarioId="c01s12",
                            GameScenarioPlayerId = "NATO",
                            News = new NewsReport()
                            {
                                Header = "NEWS_C01S12_HEADER",
                                SubHeader = "NEWS_C01S12_SUBHEADER",
                                Body = "NEWS_C01S12_BODY",
                                Image = "Illustrations/news_C01S12"
                            },
                            DialogBackgroundImage = "Backgrounds/london_office",
                            DialogBackgroundSound = "Sound/150 london office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S12_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S12_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S12_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S12_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S12_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S12_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S12_7"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S12_8"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "kossel",
                                    Dialog = "DIALOG_C01S12_9"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C01S12_10"
                                },
                            }
                        },
#endregion
                    }
                },
#endregion

#region Arctic War - Russia

                new Campaign()
                {
                    Id = "aw_russia",
                    Name="Arctic War - Russia",
                    Description="CAMPAIGN_02_DESCRIPTION",
                    PreviewImage = "ScenarioPreview/campaign_russia",
                    Icon = "Icons/icon_russia",
                    EndNews = new NewsReport()
                    {
                        Header = "NEWS_C02_END_HEADER",
                        SubHeader = "NEWS_C02_END_SUBHEADER",
                        Body = "NEWS_C02_END_BODY",
                        Image = "Illustrations/news_epilogue"
                    },
                    CampaignScenarios = new List<CampaignScenario>()
                    {
#region Scenario 1
                        new CampaignScenario()
                        {
                            ScenarioId="c02s01",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S01_HEADER",
                                SubHeader = "NEWS_C02S01_SUBHEADER",
                                Body = "NEWS_C02S01_BODY",
                                Image = "Illustrations/news_C02S01"
                            },
                            DialogBackgroundImage = "Backgrounds/berlinskys_office",
                            DialogBackgroundSound = "Sound/145 berlinsky office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S01_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S01_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S01_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S01_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S01_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S01_6"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S01_7"
                                },
                            }
                        },
#endregion
#region Scenario 2
                        new CampaignScenario()
                        {
                            ScenarioId="c02s02",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S02_HEADER",
                                SubHeader = "NEWS_C02S02_SUBHEADER",
                                Body = "NEWS_C02S02_BODY",
                                Image = "Illustrations/news_C02S02"
                            },
                            DialogBackgroundImage = "Backgrounds/berlinskys_office",
                            DialogBackgroundSound = "Sound/145 berlinsky office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S02_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S02_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S02_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S02_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S02_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S02_6"
                                },
                            }
                        },
#endregion
#region Scenario 3
                        new CampaignScenario()
                        {
                            ScenarioId="c02s03",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S03_HEADER",
                                SubHeader = "NEWS_C02S03_SUBHEADER",
                                Body = "NEWS_C02S03_BODY",
                                Image = "Illustrations/news_C02S03"
                            },
                            DialogBackgroundImage = "Backgrounds/berlinskys_office",
                            DialogBackgroundSound = "Sound/145 berlinsky office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S03_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S03_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S03_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S03_4"
                                },
                            }
                        },
#endregion
#region Scenario 4
                        new CampaignScenario()
                        {
                            ScenarioId="c02s04",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S04_HEADER",
                                SubHeader = "NEWS_C02S04_SUBHEADER",
                                Body = "NEWS_C02S04_BODY",
                                Image = "Illustrations/news_C02S04"
                            },
                            DialogBackgroundImage = "Backgrounds/moscow_office",
                            DialogBackgroundSound = "Sound/151 moscow office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S04_1"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "tutko",
                                    Dialog = "DIALOG_C02S04_2"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S04_3"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "tutko",
                                    Dialog = "DIALOG_C02S04_4"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S04_5"
                                },
                            }
                        },
#endregion
#region Scenario 5
                        new CampaignScenario()
                        {
                            ScenarioId="c02s05",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S05_HEADER",
                                SubHeader = "NEWS_C02S05_SUBHEADER",
                                Body = "NEWS_C02S05_BODY",
                                Image = "Illustrations/news_C02S05"
                            },
                            DialogBackgroundImage = "Backgrounds/berlinskys_office",
                            DialogBackgroundSound = "Sound/145 berlinsky office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S05_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S05_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S05_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S05_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S05_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S05_6"
                                },
                            }
                        },
#endregion
#region Scenario 6
                        new CampaignScenario()
                        {
                            ScenarioId="c02s06",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S06_HEADER",
                                SubHeader = "NEWS_C02S06_SUBHEADER",
                                Body = "NEWS_C02S06_BODY",
                                Image = "Illustrations/news_C02S06"
                            },
                            DialogBackgroundImage = "Backgrounds/tutkos_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "tutko",
                                    Dialog = "DIALOG_C02S06_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S06_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "tutko",
                                    Dialog = "DIALOG_C02S06_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S06_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "tutko",
                                    Dialog = "DIALOG_C02S06_5"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S06_6"
                                },
                            }
                        },
#endregion
#region Scenario 7
                        new CampaignScenario()
                        {
                            ScenarioId="c02s07",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S07_HEADER",
                                SubHeader = "NEWS_C02S07_SUBHEADER",
                                Body = "NEWS_C02S07_BODY",
                                Image = "Illustrations/news_C02S07"
                            },
                            DialogBackgroundImage = "Backgrounds/moscow_office",
                            DialogBackgroundSound = "Sound/151 moscow office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S07_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S07_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "wolfsfeld",
                                    Dialog = "DIALOG_C02S07_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S07_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "wolfsfeld",
                                    Dialog = "DIALOG_C02S07_5"
                                },
                            }
                        },
#endregion
#region Scenario 8
                        new CampaignScenario()
                        {
                            ScenarioId="c02s08",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S08_HEADER",
                                SubHeader = "NEWS_C02S08_SUBHEADER",
                                Body = "NEWS_C02S08_BODY",
                                Image = "Illustrations/news_C02S08"
                            },
                            DialogBackgroundImage = "Backgrounds/tutkos_office",
                            DialogBackgroundSound = "Sound/148 cochranes office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S08_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S08_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S08_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S08_4"
                                },
                            }
                        },
#endregion
#region Scenario 9
                        new CampaignScenario()
                        {
                            ScenarioId="c02s09",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S09_HEADER",
                                SubHeader = "NEWS_C02S09_SUBHEADER",
                                Body = "NEWS_C02S09_BODY",
                                Image = "Illustrations/news_C02S09"
                            },
                            DialogBackgroundImage = "Backgrounds/moscow_office",
                            DialogBackgroundSound = "Sound/151 moscow office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S09_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S09_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S09_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S09_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S09_5"
                                },                            
                            }
                        },
#endregion
#region Scenario 10
                        new CampaignScenario()
                        {
                            ScenarioId="c02s10",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S10_HEADER",
                                SubHeader = "NEWS_C02S10_SUBHEADER",
                                Body = "NEWS_C02S10_BODY",
                                Image = "Illustrations/news_C02S10"
                            },
                            DialogBackgroundImage = "Backgrounds/wolfsfeld_meeting",
                            DialogBackgroundSound = "Sound/156 wolfsfeld",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "wolfsfeld",
                                    Dialog = "DIALOG_C02S10_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S10_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "wolfsfeld",
                                    Dialog = "DIALOG_C02S10_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S10_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "wolfsfeld",
                                    Dialog = "DIALOG_C02S10_5"
                                },                            
                            }
                        },
#endregion
#region Scenario 11
                        new CampaignScenario()
                        {
                            ScenarioId="c02s11",
                            GameScenarioPlayerId = "Russia",
                            News = new NewsReport()
                            {
                                Date = new NWDateTime(new DateTime(2030,6,2)),
                                Header = "NEWS_C02S11_HEADER",
                                SubHeader = "NEWS_C02S11_SUBHEADER",
                                Body = "NEWS_C02S11_BODY",
                                Image = "Illustrations/news_C02S11"
                            },
                            DialogBackgroundImage = "Backgrounds/berlinskys_office",
                            DialogBackgroundSound = "Sound/145 berlinsky office",
                            DialogTree = new List<DialogEntry>()
                            {
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S11_1"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S11_2"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S11_3"
                                },
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S11_4"
                                },
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S11_5"
                                },                            
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S11_6"
                                },                            
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S11_7"
                                },                            
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S11_8"
                                },                            
                                new DialogEntry()
                                {
                                    CharacterId = "berlinsky",
                                    Dialog = "DIALOG_C02S11_9"
                                },                            
                                new DialogEntry()
                                {
                                    Dialog = "DIALOG_C02S11_10"
                                },                            
                            }
                        },
#endregion

                    },
                    
                },

#endregion
            };
            return campaignList;
        }

        public static List<DialogCharacter> CreateDialogCharacters()
        {
            var characterList = new List<DialogCharacter>()
            {
                new DialogCharacter()
                {
                    Id = "kossel",
                    Name = "CHARACTER_KOSSEL_NAME",
                    Image = "kossel",
                    TextColor = new Color(0,255,255),
                },
                new DialogCharacter()
                {
                    Id = "moltevik1",
                    Name = "CHARACTER_MOLTEVIK1_NAME",
                    Image = "moltevik",
                    TextColor = new Color(255,60,128),
                },
                new DialogCharacter()
                {
                    Id = "moltevik2",
                    Name = "CHARACTER_MOLTEVIK2_NAME",
                    Image = "moltevik",
                    TextColor = new Color(255,60,128),
                },
                new DialogCharacter()
                {
                    Id = "cochrane",
                    Name = "CHARACTER_COCHRANE_NAME",
                    Image = "cochrane",
                    TextColor = new Color(0,128,255),
                },
                new DialogCharacter()
                {
                    Id = "barth_westerdahl",
                    Name = "CHARACTER_BARTH_WESTERDAHL_NAME",
                    Image = "barth_westerdahl",
                    TextColor = new Color(255,255,0),
                },
                                
                new DialogCharacter()
                {
                    Id = "berlinsky",
                    Name = "CHARACTER_BERLINSKY_NAME",
                    Image = "berlinsky",
                    TextColor = new Color(255,255,0),
                },
                new DialogCharacter()
                {
                    Id = "tutko",
                    Name = "CHARACTER_TUTKO_NAME",
                    Image = "tutko",
                    TextColor = new Color(0,128,0),
                },
                new DialogCharacter()
                {
                    Id = "wolfsfeld",
                    Name = "CHARACTER_WOLFSFELD_NAME",
                    Image = "wolfsfeld",
                    TextColor = new Color(255,60,128),
                },            
            };
            return characterList;
        }
    }
}
