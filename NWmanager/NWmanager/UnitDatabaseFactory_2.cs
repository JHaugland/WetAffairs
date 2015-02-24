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

        public static void CreateUnitDatabase(MessageEventHandler callback)
        {
            MessageCallback = callback;

            var countryXml = new Serializer<Country>();
            List<Country> countryList = CreateCountries();
            string countryPath = SerializationHelper.GetDataFolder() + "\\Countries\\";

            foreach (var country in countryList)
            {
                string fileName = country.CountryNameShort.ToLower() + SerializationHelper.COUNTRY_EXTENSION;
                countryXml.SaveToXML(country, fileName, countryPath);
                Message(fileName + " created.");
            }

            var unitClassXml = new Serializer<UnitClass>();
            List<UnitClass> unitClassList = CreateUnitClasses();
            var ucGroups = from u in unitClassList
                           group u by u.UnitType into g
                           select new { UnitType = g.Key, UnitClass = g };
            foreach (var uc in ucGroups)
            {
                var ucThis = from u in unitClassList
                             where u.UnitType == uc.UnitType
                             select u;

                string path = SerializationHelper.GetDataFolder() + "\\Units\\" + uc.UnitType.ToString();

                foreach (var u in ucThis)
                {
                    string fileName = u.Id + SerializationHelper.UNIT_CLASS_EXTENSION;
                    unitClassXml.SaveToXML(u, fileName, path);
                    Message(fileName + " created.");
                }

            }
            //unitClassXml.SaveListToXML(unitClassList, SerializationHelper.UNIT_CLASSES_FILENAME);

            var sensorClassXml = new Serializer<SensorClass>();
            List<SensorClass> sensorClassList = CreateSensorClasses();
            string sensorPath = SerializationHelper.GetDataFolder() + "\\Sensors\\";

            foreach (var sensorClass in sensorClassList)
            {
                string fileName = sensorClass.Id + SerializationHelper.SENSOR_CLASS_EXTENSION;
                sensorClassXml.SaveToXML(sensorClass, fileName, sensorPath);
                Message(fileName + " created.");
            }

            var weaponClassXml = new Serializer<WeaponClass>();
            List<WeaponClass> weaponClassList = CreateWeaponClasses();
            string weaponPath = SerializationHelper.GetDataFolder() + "\\Weapons\\";

            foreach (var weaponClass in weaponClassList)
            {
                string fileName = weaponClass.Id + SerializationHelper.WEAPON_CLASS_EXTENSION;
                weaponClassXml.SaveToXML(weaponClass, fileName, weaponPath);
                Message(fileName + " created.");
            }

            var formationXml = new Serializer<Formation>();
            List<Formation> formationList = CreateFormations();
            string formationPath = SerializationHelper.GetDataFolder() + "\\Formations\\";

            foreach (var formation in formationList)
            {
                string fileName = formation.Id + SerializationHelper.FORMATION_EXTENSION;
                formationXml.SaveToXML(formation, fileName, formationPath);
                Message(fileName + " created.");
            }

            var scenarioXml = new Serializer<GameScenario>();
            List<GameScenario> ScenarioList = UnitDatabaseFactory.CreateScenarios();

            var scenFolder = SerializationHelper.GetDataFolder();

            try
            {
                Array.ForEach(Directory.GetFiles(scenFolder, "*" + SerializationHelper.SCENARIO_EXTENSION), delegate(string path) { File.Delete(path); });
            }
            catch (Exception ex)
            {
                Message("*** Error deleting old scenarios:  " + ex.Message);
            }

            foreach (var scenario in ScenarioList)
            {
                string fileName = "Scenario_" + scenario.Id + SerializationHelper.SCENARIO_EXTENSION;
                scenarioXml.SaveToXML(scenario, fileName, SerializationHelper.GetDataFolder() + "\\Scenarios\\");
                Message(fileName + " created.");
            }

            var campaignXml = new Serializer<Campaign>();
            var campaignList = CreateCampaigns();
            string campaignPath = SerializationHelper.GetDataFolder() + "\\Campaigns\\";

            foreach (var campaign in campaignList)
            {
                string fileName = campaign.Id + SerializationHelper.CAMPAIGN_EXTENSION;
                campaignXml.SaveToXML(campaign, fileName, campaignPath);
                Message(fileName + " created.");
            }

            var characterXml = new Serializer<DialogCharacter>();
            var characterList = CreateDialogCharacters();
            string characterPath = SerializationHelper.GetDataFolder() + "\\Characters\\";

            foreach (var character in characterList)
            {
                string fileName = character.Id + SerializationHelper.CHARACTER_EXTENSION;
                characterXml.SaveToXML(character, fileName, characterPath);
                Message(fileName + " created.");
            }

            //Serializer<User> userXml = new Serializer<User>();
            //var userList = UnitDatabaseFactory.CreateUsers();
            //userXml.SaveListToXML(userList, SerializationHelper.USERS_FILENAME);
            //ShowMessage(SerializationHelper.USERS_FILENAME + " created.");

            //ScenarioXml.SaveListToXML(ScenarioList, SerializationHelper.SCENARIOS_FILENAME);
            //ShowMessage(SerializationHelper.SCENARIOS_FILENAME + " created.");
        }

        public static List<UnitClass> CreateUnitClasses()
        {
            List<UnitClass> list = new List<UnitClass>();

            #region "Fixed wing aircraft"

            #region "F22"
            UnitClass uc = new UnitClass()
            {
                Id = "f22",
                UnitClassShortName = "F-22",
                UnitClassLongName = "F-22 Raptor",
                UnitModelFileName = "f22",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2410.0,
                AgilityFactor = 3,
                MilitaryMaxSpeedKph = 1963.0,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 18.9,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 13.56,
                HeightM = 7,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 2960000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,  //1518 km in combat
                MinSpeedKph = 400,
                TotalMassEmptyKg = 19700,
                TotalMassLoadedKg = 29300,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_EXCELLENT_DEG_SEC,
                CountryId = "us",

                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = GameConstants.DEFAULT_UNIT_COST * 2,
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                    GameConstants.Role.AEW
                },
                SensorClassIdList = new List<string> { "visual", "anapg77", "analr94", "aar56" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType= GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=6, IsPrimaryWeapon=true, WeaponBearingDeg = 0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },


                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType= GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jassmer", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                         }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=10, IsPrimaryWeapon=true, WeaponBearingDeg = 0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack (JDAM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                         }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack (Non-Stealth)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.NoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true, WeaponBearingDeg = 0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack (SMB)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu53bsdb2", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                         }
                    },
                },

            };
            list.Add(uc);
            #endregion //F22

            #region "F35a"
            uc = new UnitClass()
            {
                Id = "f35a",
                UnitClassShortName = "F-35A",
                UnitClassLongName = "F-35A Lightning II",
                UnitModelFileName = "f35",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1940,
                MilitaryMaxSpeedKph = 1200.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 15.67,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 10.7,
                HeightM = 4.33,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 500,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2222000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 13300,
                TotalMassLoadedKg = 20100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",

                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg81", "aar56" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=10, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JDAM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JDAM SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                },
            };

            list.Add(uc);
            #endregion //F35a

            #region "F35b"
            uc = new UnitClass()
            {
                Id = "f35b",
                UnitClassShortName = "F-35B",
                UnitClassLongName = "F-35B Lightning II",
                UnitModelFileName = "f35",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1940,
                MilitaryMaxSpeedKph = 1200.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 15.67,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 10.7,
                HeightM = 4.33,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 500,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 1667000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 13300,
                TotalMassLoadedKg = 20100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg81", "aar56" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JDAM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                },
            };

            list.Add(uc);
            #endregion //F35b

            #region "F35c"
            uc = new UnitClass()
            {
                Id = "f35c",
                UnitClassShortName = "F-35C",
                UnitClassLongName = "F-35C Lightning II",
                UnitModelFileName = "f35",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1940,
                MilitaryMaxSpeedKph = 1200.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 15.67,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 10.7,
                HeightM = 4.33,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 500,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2593000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 13300,
                TotalMassLoadedKg = 20100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg81", "aar56" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=10, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JDAM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JDAM SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="In-Air Refuelling",
                        WeaponLoadType = GameConstants.WeaponLoadType.RefuellingPlane,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 150,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gau12gun", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mju40", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 

                },
            };

            list.Add(uc);
            #endregion //F35c

            #region "Boeing F/A-18E Super Hornet"
            uc = new UnitClass()
            {
                Id = "fa18sh",
                UnitClassShortName = "F/A-18 Super Hornet",
                UnitClassLongName = "Boeing F/A-18E Super Hornet",
                UnitModelFileName = "superhornet",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Reduced, //has reduced rcs ...
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1900,
                MilitaryMaxSpeedKph = 1050.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 15000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 18.31,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 13.62,
                HeightM = 4.88,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 228,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2346000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 13300,
                TotalMassLoadedKg = 20100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg79", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm84harpoon", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm84harpoon", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="In-Air Refuelling",
                        WeaponLoadType = GameConstants.WeaponLoadType.RefuellingPlane,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 250,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Maverick)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm65", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Maverick SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm65", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JSOW)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (JSOW SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="m61a2gatling", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                },
            };

            list.Add(uc);
            #endregion //Boeing F/A-18E/F Super Hornet

            #region "Boeing EA-18G Growler"
            uc = new UnitClass()
            {
                Id = "ea18ggrowler",
                UnitClassShortName = "EA-18G Growler",
                UnitClassLongName = "Boeing EA-18G Growler",
                UnitModelFileName = "superhornet",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Reduced, //has reduced rcs ...
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1900,
                MilitaryMaxSpeedKph = 1050.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 2,
                DraftM = 0,
                HighestOperatingHeightM = 15000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 18.31,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 13.62,
                HeightM = 4.88,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 228,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2346000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 13300,
                TotalMassLoadedKg = 20100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 361,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg79", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Electronic Warfare",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq227", MaxAmmunition=2, IsPrimaryWeapon=false //comms jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq99", MaxAmmunition=4, IsPrimaryWeapon=false //radar jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Electronic Warfare (Strike)",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq227", MaxAmmunition=2, IsPrimaryWeapon=false //comms jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq99", MaxAmmunition=4, IsPrimaryWeapon=false //radar jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Electronic Warfare (SR Strike)",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq227", MaxAmmunition=2, IsPrimaryWeapon=false //comms jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq99", MaxAmmunition=4, IsPrimaryWeapon=false //radar jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                },
            };

            list.Add(uc);
            #endregion //Boeing Growler

            #region "Eurofighter Typhoon"
            uc = new UnitClass()
            {
                Id = "typhoon",
                UnitClassShortName = "Eurofighter Typhoon",
                UnitClassLongName = "Eurofighter Typhoon",
                UnitModelFileName = "typhoon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2495,
                MilitaryMaxSpeedKph = 1350,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 15.69,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 10.95,
                HeightM = 5.28,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 500,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2900000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 11150,
                TotalMassLoadedKg = 16100,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_VERY_GOOD_DEG_SEC,
                CountryId = "uk",
                EstimatedUnitPriceMillionUSD = 123,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                SensorClassIdList = new List<string> { "visual", "captor", "pirateir" },
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (JSM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jsm", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Standoff Land Attack (JSOW)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsow", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu12", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (JDAM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Maverick)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm65", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (HARM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm88harm", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Standoff Land Attack (KEPD)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kepd350", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                },
            };

            list.Add(uc);
            #endregion //typhoon

            #region "JAS 39 Gripen"
            uc = new UnitClass()
            {
                Id = "jas39gripen",
                UnitClassShortName = "JAS 39E Gripen",
                UnitClassLongName = "Saab JAS 39E Gripen",
                UnitModelFileName = "gripen",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2200,
                MilitaryMaxSpeedKph = 1340,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 15240,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 14.1,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 8.4,
                HeightM = 4.5,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 500,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 1300000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 6800,
                TotalMassLoadedKg = 8500,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "se",
                EstimatedUnitPriceMillionUSD = 50,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                {
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "ravenasea", "pirateir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim120amraam", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Standoff Strike (RBS-15)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="rbs15mk3", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu12", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Maverick)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm65", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Standoff Land Attack (KEPD)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mauserbk27", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kepd350", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="aim9sidewinder", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                },

            };

            list.Add(uc);
            #endregion //JAS 39 Gripen

            #region "Mikoyan MiG-29K Fulcrum-D"
            uc = new UnitClass()
            {
                Id = "mig29k",
                UnitClassShortName = "MiG-29K",
                UnitClassLongName = "Mikoyan MiG-29K 'Fulcrum-D'",
                UnitModelFileName = "mig29k",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850.0,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2200,
                MilitaryMaxSpeedKph = 1300.0,
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 17500,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 17.3,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 12,
                HeightM = 6.05,
                MaxAccelerationKphSec = 200,
                MaxClimbrateMSec = 120,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 2000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 18550,
                TotalMassLoadedKg = 24500,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_VERY_GOOD_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "zhukme", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr27", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh31", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Standoff Land Attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh29", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                },

            };

            list.Add(uc);
            #endregion //"Mikoyan MiG-29K Fulcrum-D"

            #region "Sukhoi Su-27 Flanker"
            uc = new UnitClass()
            {
                Id = "su27",
                UnitClassShortName = "Su-27SM",
                UnitClassLongName = "Sukhoi Su-27SM 'Flanker'",
                UnitModelFileName = "flanker", //same model used for Su-35
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850.0,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2500,
                MilitaryMaxSpeedKph = 1000.0, //no supercruise, unlike Su35
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18500,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 21.9,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 14.7,
                HeightM = 6.05,
                MaxAccelerationKphSec = 200,
                MaxClimbrateMSec = 300,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 3530000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 16380,
                TotalMassLoadedKg = 23430,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "irbise", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },

                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (Anti-Rad)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh31", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (Kh-35)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (Kh-29)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh29", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },                    
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (Bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kab500kr", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },                
                },

            };

            list.Add(uc);
            #endregion //Su-27 Flanker

            #region "Sukhoi Su-35 Flanker E"
            uc = new UnitClass()
            {
                Id = "su35",
                UnitClassShortName = "Su-35",
                UnitClassLongName = "Sukhoi Su-35 'Flanker E'",
                UnitModelFileName = "flanker", //same model used for Su-37
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850.0,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2390,
                MilitaryMaxSpeedKph = 1350.0, //Su35 can supercruise
                AgilityFactor = 3,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 18500,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 21.9,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 15.3,
                HeightM = 5.9,
                MaxAccelerationKphSec = 200,
                MaxClimbrateMSec = 300,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 400,
                MaxRangeCruiseM = 3600000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 18400,
                TotalMassLoadedKg = 25300,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_VERY_GOOD_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 65,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "irbise", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr27", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr27", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (RW-EE)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=12, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (LR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = +50,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (Anti-Rad)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh31", MaxAmmunition=6, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (Kh-35)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=6, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (Kh-29)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh29", MaxAmmunition=6, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },                    
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (Bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kab500kr", MaxAmmunition=4, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },                
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (Bomb SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=20, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr73", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kab500kr", MaxAmmunition=6, IsPrimaryWeapon=true
                            },    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },                    
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },                
                },

            };

            list.Add(uc);
            #endregion //Su-35 Flanker E

            #region "Sukhoi PAK FA A"
            uc = new UnitClass()
            {
                Id = "sukhoipakfaa",
                UnitClassShortName = "Sukhoi T-50A",
                UnitClassLongName = "Sukhoi T-50A PAK FA",
                UnitModelFileName = "sukhoipakfa",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2600,
                AgilityFactor = 3,
                MilitaryMaxSpeedKph = 1300.0,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 20000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 22,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 12.2,
                HeightM = 6.05,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 350,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 3000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 19700,
                TotalMassLoadedKg = 29300,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_EXCELLENT_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "irbise", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },

                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (R-37)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.LongRangeWeapon,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr37", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (No Stealth)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=8, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Anti-Rad)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh58", MaxAmmunition=4, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh37", MaxAmmunition=2, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh37", MaxAmmunition=4, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },

                },

            };

            list.Add(uc);

            #endregion //"Sukhoi PAK FA A"

            #region "Sukhoi PAK FA K"
            uc = new UnitClass()
            {
                Id = "sukhoipakfa",
                UnitClassShortName = "Sukhoi T-50K",
                UnitClassLongName = "Sukhoi T-50K PAK FA",
                UnitModelFileName = "sukhoipakfa",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2600,
                AgilityFactor = 3,
                MilitaryMaxSpeedKph = 1300.0,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 1,
                DraftM = 0,
                HighestOperatingHeightM = 20000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 22,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 12.2,
                HeightM = 6.05,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 350,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 3000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 19700,
                TotalMassLoadedKg = 29300,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_EXCELLENT_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "irbise", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },

                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (R-37)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr37", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Air superiority (No Stealth)",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr77", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRangeNoStealth,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=1,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=4, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Strike (Anti-Rad)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh58", MaxAmmunition=2, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh37", MaxAmmunition=2, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=2,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh30", MaxAmmunition=18, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="vympelr74", MaxAmmunition=2, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh37", MaxAmmunition=4, IsPrimaryWeapon=true
                            },                        
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                        }
                    },

                },

            };

            list.Add(uc);

            #endregion //"Sukhoi PAK FA K"

            #region "Boeing E-3 Sentry"

            uc = new UnitClass()
            {
                Id = "e3sentry",
                UnitClassShortName = "E-3 Sentry",
                UnitClassLongName = "Boeing E-3 Sentry AWACS",
                UnitModelFileName = "e3sentry",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 882,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 955,
                MilitaryMaxSpeedKph = 955,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                CrewTotal = 14,
                DraftM = 0,
                HighestOperatingHeightM = 13106,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 46.61,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 44.42,
                HeightM = 12.6,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 8046000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 83990,
                TotalMassLoadedKg = 151636,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "us",
                AgilityFactor = 2,
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.AWACS,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>() 
                { 
                    new UnitClassWeaponLoads()
                    {
                        Name = "Default",
                        TimeToChangeLoadoutHour = 1,
                        TimeToReloadHour = 0.5,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericchaff", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericflare", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                        },
                    
                    },
                },
                SensorClassIdList = new List<string> { "visual", "anapy1", "genericir" }
            };

            list.Add(uc);

            #endregion //Boeing E-3 Sentry

            #region "Beriev A-50U Shmel 'Mainstay'"

            uc = new UnitClass()
            {
                Id = "berieva50u",
                UnitClassShortName = "Beriev A-50U Shmel",
                UnitClassLongName = "Beriev A-50U Shmel 'Mainstay' AWACS",
                UnitModelFileName = "berieva50u",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 700,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 800,
                MilitaryMaxSpeedKph = 800,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 15,
                DraftM = 0,
                HighestOperatingHeightM = 12000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 49.59,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 50.5,
                HeightM = 17.76,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 6400000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 75000,
                TotalMassLoadedKg = 170000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.AWACS,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>() 
                { 
                    new UnitClassWeaponLoads()
                    {
                        Name = "Default",
                        TimeToChangeLoadoutHour = 1,
                        TimeToReloadHour = 0.5,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericchaff", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericflare", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                        },
                    
                    },
                },
                SensorClassIdList = new List<string> { "visual", "shmelm", "genericir" }
            };

            list.Add(uc);

            #endregion //Beriev A-50U Shmel 'Mainstay'

            #region "Ilyushin Il-78 'Midas' Tanker Plane"

            uc = new UnitClass()
            {
                Id = "il78",
                UnitClassShortName = "Ilyushin Il-78",
                UnitClassLongName = "Ilyushin Il-78 'Midas' Tanker Plane",
                UnitModelFileName = "il78",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 750,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 850,
                MilitaryMaxSpeedKph = 850,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 6,
                DraftM = 0,
                HighestOperatingHeightM = 12000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 46.59,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 50.5,
                HeightM = 14.76,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 30000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 72000,
                TotalMassLoadedKg = 210000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.RefuelAircaft,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>() 
                { 
                    new UnitClassWeaponLoads()
                    {
                        Name = "In-Air Refuelling",
                        TimeToChangeLoadoutHour = 1,
                        TimeToReloadHour = 0.5,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericchaff", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericflare", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                        },
                    
                    },
                },
                SensorClassIdList = new List<string> { "visual", "civilianradar" }
            };

            list.Add(uc);

            #endregion //Ilyushin Il-78 tanker plane

            #region "Boeing KC-767 aerial refueling"

            uc = new UnitClass()
            {
                Id = "kc767",
                UnitClassShortName = "Boeing KC-767",
                UnitClassLongName = "Boeing KC-767 Aerial Refueling Aircraft",
                UnitModelFileName = "kc767",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 851,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 915,
                MilitaryMaxSpeedKph = 915,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 3,
                DraftM = 0,
                HighestOperatingHeightM = 12200,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 48.5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 47.6,
                HeightM = 15.8,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 30000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 82377,
                TotalMassLoadedKg = 181000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.RefuelAircaft,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>() 
                { 
                    new UnitClassWeaponLoads()
                    {
                        Name = "In-Air Refuelling",
                        TimeToChangeLoadoutHour = 1,
                        TimeToReloadHour = 0.5,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericchaff", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                            new UnitClassWeaponLoad()
                            {
                                WeaponClassId = "genericflare", IsPrimaryWeapon = false, MaxAmmunition = 5,
                            },
                        },
                    
                    },
                },
                SensorClassIdList = new List<string> { "visual", "civilianradar" }
            };

            list.Add(uc);

            #endregion //Boeing KC-767  aerial refueling

            #region "E-2 Hawkeye"

            uc = new UnitClass()
            {
                Id = "e2hawkeye",
                UnitClassShortName = "E-2 Hawkeye",
                UnitClassLongName = "Grumman E-2 Hawkeye",
                UnitModelFileName = "e2hawkeye",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CanRefuelInAir = true,
                IsAircraft = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 550,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 604,
                MilitaryMaxSpeedKph = 604,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboJet,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 5,
                DraftM = 0,
                HighestOperatingHeightM = 9300,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 17.56,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 24.58,
                HeightM = 5.58,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 2583000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 17090,
                TotalMassLoadedKg = 23391,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.AWACS,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>(),
                SensorClassIdList = new List<string> { "visual", "anaps145", "analr73", "genericir" }
            };

            list.Add(uc);

            #endregion //E-2 Hawkeye

            #region "Lockheed P-3C Orion"

            uc = new UnitClass()
            {
                Id = "p3orion",
                UnitClassShortName = "Lockheed P-3C Orion",
                UnitClassLongName = "Lockheed P-3C Orion",
                UnitModelFileName = "orion",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 610,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 750,
                MilitaryMaxSpeedKph = 750,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboProp,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 11,
                DraftM = 0,
                HighestOperatingHeightM = 10400,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 35.6,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 30.4,
                HeightM = 11.8,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 16,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 200,
                MaxRangeCruiseM = 3833000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 35000,
                TotalMassLoadedKg = 64400,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 36,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anaps137", },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads() 
                    { 
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mark54torpedo", MaxAmmunition=6, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            { 
                                WeaponClassId="dicass", MaxAmmunition=87, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm84harpoon", MaxAmmunition=4, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Strike (SLAM-ER)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slamer", MaxAmmunition=4, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Strike (Maverick)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm65", MaxAmmunition=4, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Deploy mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk56mine", MaxAmmunition=15, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                },
            };

            list.Add(uc);

            #endregion //Lockheed P-3C Orion

            #region "Boeing P-8A Poseidon"

            uc = new UnitClass()
            {
                Id = "p8poseidon",
                UnitClassShortName = "Boeing P-8A Poseidon",
                UnitClassLongName = "Boeing P-8A Poseidon Multimission Maritime Aircraft",
                UnitModelFileName = "p8poseidon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 815,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 907,
                MilitaryMaxSpeedKph = 907,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboJet,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 7,
                DraftM = 0,
                HighestOperatingHeightM = 12500,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 49.5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 51.1,
                HeightM = 12.12,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 250,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 15000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR, //TODO: find out
                MinSpeedKph = 200,
                TotalMassEmptyKg = 62730,
                TotalMassLoadedKg = 85730,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapy10", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm84harpoon", MaxAmmunition=11, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slamer", MaxAmmunition=11, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mark54torpedo", MaxAmmunition=11, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            { 
                                WeaponClassId="dicass", MaxAmmunition=200, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Deploy mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk56mine", MaxAmmunition=30, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Electronic warfare",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="alq214", MaxAmmunition=4, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },

                    

                },
            };

            list.Add(uc);

            #endregion //Boeing P-8A Poseidon

            #region "Rockwell B-1B Lancer"

            uc = new UnitClass()
            {
                Id = "b1lancer",
                UnitClassShortName = "B1 Lancer",
                UnitClassLongName = "Rockwell B-1B Lancer",
                UnitModelFileName = "lancer",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1340,
                MilitaryMaxSpeedKph = 940,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboJet,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 4,
                DraftM = 0,
                HighestOperatingHeightM = 18000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 44.5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 24.1,
                HeightM = 10.4,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 250,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 5543000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 87100,
                TotalMassLoadedKg = 148000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 283.1,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapg66", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jassmer", MaxAmmunition=24, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads()
                    { 
                        Name="Strike (JSOW)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="agm154jsower", MaxAmmunition=12, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=48, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Deploy mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk56mine", MaxAmmunition=84, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                },
            };

            list.Add(uc);

            #endregion //Rockwell B-1 Lancer

            #region "B-2 Spirit"

            uc = new UnitClass()
            {
                Id = "b2spirit",
                UnitClassShortName = "B-2 Spirit",
                UnitClassLongName = "Northrop Grumman B-2 Spirit",
                UnitModelFileName = "b2spirit",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1010,
                MilitaryMaxSpeedKph = 1010,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 2,
                DraftM = 0,
                HighestOperatingHeightM = 11100,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 21,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 52.4,
                HeightM = 5.18,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 250,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 11100000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 71700,
                TotalMassLoadedKg = 152200,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 1070,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "anapq181", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jassmer", MaxAmmunition=16, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (JASSM)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="jassm", MaxAmmunition=16, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land Attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gbu38jdam", MaxAmmunition=80, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                },
            };

            list.Add(uc);

            #endregion //B-2 Spirit

            #region "Tu-22M3 Backfire C"

            uc = new UnitClass()
            {
                Id = "tu22m3",
                UnitClassShortName = "Tupolev Tu-22M3",
                UnitClassLongName = "Tupolev Tu-22M3 'Backfire C'",
                UnitModelFileName = "tu22m3",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 820,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2327,
                MilitaryMaxSpeedKph = 920,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboProp,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 3,
                CrewTotal = 4,
                DraftM = 0,
                HighestOperatingHeightM = 13300,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 42.4,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 34.28,
                HeightM = 11.05,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 15,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 7000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 58000,
                TotalMassLoadedKg = 112000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "pnad", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh15", MaxAmmunition=10, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike (Kh-22M)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh22m", MaxAmmunition=3, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval Strike (Kh-65)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh65", MaxAmmunition=8, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh37", MaxAmmunition=10, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Electronic warfare",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericradarjammer", MaxAmmunition=4, IsPrimaryWeapon=false, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },

                },
            };

            list.Add(uc);

            #endregion //Tu-22M3 "Backfire C"

            #region "Tu95 Bear"

            uc = new UnitClass()
            {
                Id = "tu95",
                UnitClassShortName = "Tupolev Tu-95",
                UnitClassLongName = "Tupolev Tu-95 'Bear'",
                UnitModelFileName = "tu95",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 820,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 920,
                MilitaryMaxSpeedKph = 920,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboProp,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 7,
                DraftM = 0,
                HighestOperatingHeightM = 13716,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 49.5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 51.1,
                HeightM = 12.12,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 250,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 500,
                MaxRangeCruiseM = 15000000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 90000,
                TotalMassLoadedKg = 171000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "ru",

                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh35", MaxAmmunition=16, IsPrimaryWeapon=true, WeaponBearingDeg=180
                            },

                        }
                    }, 
                    new UnitClassWeaponLoads() 
                    { 
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="apr3etorpedo", MaxAmmunition=12, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            { 
                                WeaponClassId="rgbnm", MaxAmmunition=176, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads() 
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh555", MaxAmmunition=12, IsPrimaryWeapon=true
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },

                    new UnitClassWeaponLoads() 
                    { 
                        Name="Deploy Mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        { 
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="gsh23", MaxAmmunition=230, IsPrimaryWeapon=false, WeaponBearingDeg=180
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mdm5mine", MaxAmmunition=50, IsPrimaryWeapon=false
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },

                },
                SensorClassIdList = new List<string> { "visual", "pnab" }
            };

            list.Add(uc);

            #endregion //Tu95 Bear

            #region "Tu-160 Blackjack"

            uc = new UnitClass()
            {
                Id = "tu160",
                UnitClassShortName = "Tupolev Tu-160",
                UnitClassLongName = "Tupolev Tu-160 'Blackjack'",
                UnitModelFileName = "blackjack",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 960,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 2220,
                MilitaryMaxSpeedKph = 1040,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboProp,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 3,
                CrewTotal = 4,
                DraftM = 0,
                HighestOperatingHeightM = 15000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 54.1,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 35.6,
                HeightM = 13.10,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 70,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 12300000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 110000,
                TotalMassLoadedKg = 267600,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 1.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "obzork", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh15", MaxAmmunition=24, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh555", MaxAmmunition=12, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack (Bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kab500kr", MaxAmmunition=24, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                },
            };

            list.Add(uc);

            #endregion //Tu-160 'Blackjack'


            #region "PAK DA"

            uc = new UnitClass()
            {
                Id = "pakda",
                UnitClassShortName = "Tu-1000 PAK DA",
                UnitClassLongName = "Tupolev Tu-1000 'PAK DA'",
                UnitModelFileName = "pakda",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                IsAircraft = true,
                CanRefuelInAir = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 960,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 1200,
                MilitaryMaxSpeedKph = 1200,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 3,
                CrewTotal = 4,
                DraftM = 0,
                HighestOperatingHeightM = 20000,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 54.1,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 35.6,
                HeightM = 13.10,
                MaxAccelerationKphSec = 300,
                MaxClimbrateMSec = 70,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 12300000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 110000,
                TotalMassLoadedKg = 267600,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 1000,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 2.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                SensorClassIdList = new List<string> { "visual", "obzork", "genericir" },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Naval strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh15", MaxAmmunition=14, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kh555", MaxAmmunition=6, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    { 
                        Name="Land attack (Bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="kab500kr", MaxAmmunition=12, IsPrimaryWeapon=true, WeaponBearingDeg=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                },
            };

            list.Add(uc);

            #endregion //PAK-DA

            #region "Boeing 767 Passenger airliner"

            uc = new UnitClass()
            {
                Id = "boeing767",
                UnitClassShortName = "Boeing 767",
                UnitClassLongName = "Boeing 767 Passenger airliner",
                UnitModelFileName = "boeing767",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                IsAircraft = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraftLarge,
                CruiseSpeedKph = 850,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 913,
                MilitaryMaxSpeedKph = 913,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 2,
                DraftM = 0,
                HighestOperatingHeightM = 12000,
                IsEsmShielded = false,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 48.5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 47.6,
                HeightM = 15,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 12200000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 82380,
                TotalMassLoadedKg = 179170,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_LOW_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>(),
                SensorClassIdList = new List<string> { "visual", "civilianradar" }
            };

            list.Add(uc);

            #endregion //Boeing 767 Passenger airliner

            #region "Dassault Falcon 20"

            uc = new UnitClass()
            {
                Id = "falcon20",
                UnitClassShortName = "Dassault Falcon 20",
                UnitClassLongName = "Dassault Falcon 20",
                UnitModelFileName = "falcon20",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                IsAircraft = true,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                CruiseSpeedKph = 750,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft),
                MaxSpeedKph = 862,
                MilitaryMaxSpeedKph = 862,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                MaxCarriedUnitsTotal = 0,
                AgilityFactor = 2,
                CrewTotal = 2,
                DraftM = 0,
                HighestOperatingHeightM = 12800,
                IsEsmShielded = true,
                StabilityBonus = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 17.15,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 16.3,
                HeightM = 5.32,
                MaxAccelerationKphSec = 100,
                MaxClimbrateMSec = 13,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 3350000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 152,
                TotalMassEmptyKg = 7539,
                TotalMassLoadedKg = 13000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_MEDIUM_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 5,
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>()
                {
                    new UnitClassWeaponLoads()
                    { 
                        Name="Electronic Warfare",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad>()
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericradarjammer", MaxAmmunition=4, IsPrimaryWeapon=false //radar jamming
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                        }
                    }, 

                },
                SensorClassIdList = new List<string> { "visual", "civilianradar" }
            };

            list.Add(uc);

            #endregion //Dassault Falcon 20

            #endregion //Fixed wing aircraft

            #region "UAV"

            #region "MQ 9 Mariner UAV"

            uc = new UnitClass()
            {
                Id = "mq9mariner", //naval version of Reaper
                UnitClassShortName = "MQ-9 Mariner",
                UnitClassLongName = "MQ-9 Mariner UAV",
                UnitModelFileName = "mq9reaper",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                CruiseSpeedKph = 300,
                MaxSpeedKph = 480,
                MilitaryMaxSpeedKph = 480,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboProp,
                AgilityFactor = 2,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 15000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 11,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 20,
                HeightM = 3.6, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 800000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 200,
                TotalMassEmptyKg = 2223,
                TotalMassLoadedKg = 4760,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "anapy8" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land Attack Bomb",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                  
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu12", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land Attack JDAM",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu38jdam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Strike (AA)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu38jdam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="fim92cstinger",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        },

                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Air attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="fim92cstinger",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        },

                    },

                }
            };
            list.Add(uc);

            #endregion //MQ 9 Mariner

            #region "Pegasus UAV"

            uc = new UnitClass()
            {
                Id = "pegasus", //speculative final version of X-47B
                UnitClassShortName = "M-47B Pegasus",
                UnitClassLongName = "Northrop Grumman M-47B Pegasus UCAV",
                UnitModelFileName = "pegasus",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                CruiseSpeedKph = 551,
                MaxSpeedKph = 850,
                MilitaryMaxSpeedKph = 850,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                AgilityFactor = 2,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 12190,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 11.63,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 18.92,
                HeightM = 3.1, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 3889000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 400,
                TotalMassEmptyKg = 2223,
                TotalMassLoadedKg = 4760,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "anapy8", "genericir" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="jsm",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land Attack JDAM",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu38jdam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Strike (AA)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu38jdam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="fim92cstinger",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                        },

                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Air attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,

                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="fim92cstinger",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                        },
                    },

                }
            };
            list.Add(uc);

            #endregion //Pegasus

            #region "Schiebel Camcopter S-100"
            uc = new UnitClass()
            {
                Id = "camcopters100",
                UnitClassShortName = "Camcopter S-100",
                UnitClassLongName = "Schiebel Camcopter S-100",
                UnitModelFileName = "camcopters100",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                CruiseSpeedKph = 180,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 220,
                MilitaryMaxSpeedKph = 220,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                AgilityFactor = 2,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 5500,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 4,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 4,
                HeightM = 3, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 1200000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 160,
                TotalMassLoadedKg = 200,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "at",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "schiebelir", "sar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="AEW",
                        WeaponLoadType = GameConstants.WeaponLoadType.AEW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Multirole Missile",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=1,
                        IncreasesCruiseRangePercent = -50,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="thaleslmm",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Counter Mine Operations",
                        WeaponLoadType = GameConstants.WeaponLoadType.CounterMine,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 0,
                        IncreasesLoadRangeByLevels = 0,
                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {

                        }
                    },

                }
            };
            list.Add(uc);
            #endregion //Schiebel Camcopter S-100

            #region "Skat"

            uc = new UnitClass()
            {
                Id = "migskat", //speculative final version of Mig Skat
                UnitClassShortName = "Sukhoi Skat",
                UnitClassLongName = "Sukhoi Skat UCAV",
                UnitModelFileName = "migskat",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                CruiseSpeedKph = 550,
                MaxSpeedKph = 800,
                MilitaryMaxSpeedKph = 800,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                AgilityFactor = 2,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 12000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 10.25,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 11.5,
                HeightM = 3.1, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 3889000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 350,
                TotalMassEmptyKg = 2223,
                TotalMassLoadedKg = 4760,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "e801", "genericir" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike (antirad)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.AntiRadiation,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="kh31",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land Attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="kh29",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="kab500kr",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Electronic Warfare",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="genericcommsjammer",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        },
                    },


                }
            };
            list.Add(uc);

            #endregion //Skat UCAV

            #region "Taranis UCAV"

            uc = new UnitClass()
            {
                Id = "taranis", //speculative final version of Taranis (UK)
                UnitClassShortName = "BAE Taranis",
                UnitClassLongName = "BAE Taranis UCAV",
                UnitModelFileName = "migskat",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.FixedwingAircraft,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                DetectionClassification = GameConstants.DetectionClassification.FixedWingAircraft,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                CruiseSpeedKph = 550,
                MaxSpeedKph = 800,
                MilitaryMaxSpeedKph = 800,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboFan,
                AgilityFactor = 2,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 12000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 11.35,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 9.1,
                HeightM = 4,
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 3889000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 350,
                TotalMassEmptyKg = 8000,
                TotalMassLoadedKg = 8000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_AIR_AGILE_GOOD_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "anapy8", "genericir" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsFixedWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seaskuamk2",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike (light)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seaskua",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Strike (light)",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="thaleslmm",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land Attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0,
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land attack (bomb)",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=0.5,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gbu38jdam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                        },
                    },

                }
            };
            list.Add(uc);

            #endregion //Taranis UCAV

            #endregion

            #region "Helicopters"

            #region "NH90 NFH"
            uc = new UnitClass()
            {
                Id = "nh90nhf",
                UnitClassShortName = "NH90 NFH",
                UnitClassLongName = "NHIndustries NH90",
                UnitModelFileName = "nh90nhf",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CanRefuelInAir = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 260,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 300,
                MilitaryMaxSpeedKph = 300,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                AgilityFactor = 2,
                CrewTotal = 3,
                DraftM = 0,
                HighestOperatingHeightM = 6000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 16.13,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 16.30,
                HeightM = 9, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 800000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6400,
                TotalMassLoadedKg = 10600,
                TurnRangeDegrSec = GameConstants.TURN_RATE_HELO_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "thalesmrr3d", "aqs18a", "analq142" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                    
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=3,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dicass",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },

                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Counter Mine Operations",
                        WeaponLoadType = GameConstants.WeaponLoadType.CounterMine,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 0,
                        IncreasesLoadRangeByLevels = 0,
                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },

                        }
                    },

                }
            };
            list.Add(uc);
            #endregion //NH90 NFH

            #region "AW101"
            uc = new UnitClass()
            {
                Id = "aw101",
                UnitClassShortName = "AW101 HM2",
                UnitClassLongName = "AgustaWestland AW101 Merlin HM2",
                UnitModelFileName = "aw101", //beware of all caps in old version
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CanRefuelInAir = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 278,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 309,
                MilitaryMaxSpeedKph = 309,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                AgilityFactor = 2,
                CrewTotal = 3,
                DraftM = 0,
                HighestOperatingHeightM = 4575,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 19.53,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 18.59,
                HeightM = 6.62, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 800000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6400,
                TotalMassLoadedKg = 10600,
                TurnRangeDegrSec = GameConstants.TURN_RATE_HELO_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 21,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "selexgalileo5000", "roresm", "sintraflash" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dicass",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seaskuamk2", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Light Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="thaleslmm", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Counter Mine Operations",
                        WeaponLoadType = GameConstants.WeaponLoadType.CounterMine,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 0,
                        IncreasesLoadRangeByLevels = 0,
                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                }
            };
            list.Add(uc);
            #endregion //AW101

            #region "AW159 Lynx Wildcat"
            uc = new UnitClass()
            {
                Id = "aw159",
                UnitClassShortName = "AW159 Lynx Wildcat",
                UnitClassLongName = "AgustaWestland AW159 Lynx Wildcat",
                UnitModelFileName = "aw159",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CanRefuelInAir = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 278,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 291,
                MilitaryMaxSpeedKph = 309,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                AgilityFactor = 2,
                CrewTotal = 2,
                DraftM = 0,
                HighestOperatingHeightM = 4575,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 15.24,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 12.8,
                HeightM = 3.73, //guess
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 777000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4000,
                TotalMassLoadedKg = 6000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_HELO_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 21,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "selexgalileo5000", "roresm", "sintraflash" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                    
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dicass",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Ferry",
                        WeaponLoadType = GameConstants.WeaponLoadType.Ferry,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 25,
                    
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seaskuamk2", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike (SR)",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoadModifyer = GameConstants.WeaponLoadModifier.ShortRange,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        IncreasesCruiseRangePercent = -30,                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seaskuamk2", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Light Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="thaleslmm", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=5, IsPrimaryWeapon=false //chaff
                            },
                        }
                    },
                }
            };
            list.Add(uc);
            #endregion //Lynx Wildcat

            #region "SH-60B"
            uc = new UnitClass()
            {
                Id = "sh60b",
                UnitClassShortName = "SH-60B Seahawk",
                UnitClassLongName = "Sikorsky SH-60B Seahawk",
                UnitModelFileName = "sh60b",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CanRefuelInAir = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 300,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 333,
                MilitaryMaxSpeedKph = 333,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                AgilityFactor = 2,
                CrewTotal = 3,
                DraftM = 0,
                HighestOperatingHeightM = 5790,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 19.76,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 16.36,
                HeightM = 9, //guess
                MaxAccelerationKphSec = 50,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 704000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6190,
                TotalMassLoadedKg = 9575,
                TurnRangeDegrSec = GameConstants.TURN_RATE_HELO_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "anaps124", "aqs18a", "analq142" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark54torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=3,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gau12gun",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=18,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dicass",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=14,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                    
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gau12gun",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=18,
                                YPosition=0,
                            },

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="agm114n", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Deploy mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                   
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gau12gun",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=18,
                                YPosition=0,
                            },

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk56mine",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Counter Mine Operations",
                        WeaponLoadType = GameConstants.WeaponLoadType.CounterMine,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 0,
                        IncreasesLoadRangeByLevels = 0,
                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },

                }
            };
            list.Add(uc);
            #endregion //SH-60B

            #region "Kamov Ka-27"
            uc = new UnitClass()
            {
                Id = "ka27",
                UnitClassShortName = "Kamov Ka-27",
                UnitClassLongName = " Kamov Ka-27 'Helix'",
                UnitModelFileName = "ka27",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Helicopter,
                IsAircraft = true,
                CanRefuelInAir = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                DetectionClassification = GameConstants.DetectionClassification.Helicopter,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 205,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter),
                MaxSpeedKph = 270,
                MilitaryMaxSpeedKph = 270,
                PropulsionSystem = GameConstants.PropulsionSystem.TurboShaft,
                CrewTotal = 3,
                AgilityFactor = 2,
                DraftM = 0,
                HighestOperatingHeightM = 5000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                IsEsmShielded = true,
                StabilityBonus = 0,
                LengthM = 11.3,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 15.8,
                HeightM = 9, //guess
                MaxAccelerationKphSec = 50,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -50,
                MaxFallMSec = 100,
                MaxRangeCruiseM = 980000 * GameConstants.AIRCRAFT_RANGE_INCREASE_FACTOR,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6500,
                TotalMassLoadedKg = 11000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_HELO_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = Convert.ToInt32(GameConstants.DEFAULT_UNIT_COST * 0.5),
                EstimatedUnitPriceMillionUSD = 28,
                MaxCarriedUnitsTotal = 0,
                SensorClassIdList = new List<string> { "visual", "e801", "vgs-3" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.Scouting,
                    GameConstants.Role.AEW,
                    GameConstants.Role.IsRotaryWingAircraft,
                    GameConstants.Role.IsAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="ASW",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgbnm",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=36,
                                YPosition=0,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Naval Strike",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                       
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="kh35", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Deploy Mines",
                        WeaponLoadType = GameConstants.WeaponLoadType.DeployMines,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesLoadRangeByLevels = 0,
                      
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mdm5mine", 
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Counter Mine Operations",
                        WeaponLoadType = GameConstants.WeaponLoadType.CounterMine,
                        TimeToReloadHour=0.5,
                        TimeToChangeLoadoutHour=1,
                        IncreasesRadarCrossSection=0,
                        IncreasesCruiseRangePercent = 0,
                        IncreasesLoadRangeByLevels = 0,
                     
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=5, IsPrimaryWeapon=false //flares
                            },
                        }
                    },

                }
            };
            list.Add(uc);
            #endregion //Kamov Ka-27

            #endregion //Helicopters

            #region "Missiles and torpedoes"

            uc = new UnitClass()
            {
                Id = "asumissile", //Asu missile with active homing radar
                UnitClassShortName = "AntiShip Cruise Missile",
                UnitClassLongName = "Generic AntiShip Cruise Missile Active Homing",
                UnitModelFileName = "asumissile_harpoon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 800.0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 800.0,
                MilitaryMaxSpeedKph = 800.0,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 10000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 7,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 1,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 120000, //too low
                MinSpeedKph = 800,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 3,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "missileradar" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "bomb",
                UnitClassShortName = "Bomb",
                UnitClassLongName = "Generic Gravity or Glide Bomb",
                UnitModelFileName = "mk56mine",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Bomb,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 800.0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 800.0,
                MilitaryMaxSpeedKph = 800.0,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 20000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 6,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 120000, //too low
                MinSpeedKph = 800,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 3,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "missileradarsemi", "irasu" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "asumissileir",
                UnitClassShortName = "AntiShip Cruise Missile IR",
                UnitClassLongName = "Generic AntiShip Cruise Missile IR",
                UnitModelFileName = "asumissile_harpoon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 800.0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 800.0,
                MilitaryMaxSpeedKph = 800.0,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 10000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 6,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 1,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 120000, //too low
                MinSpeedKph = 800,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 3,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "irasu" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "asumissilesemi", //ASu Missile semi-active homing
                UnitClassShortName = "AntiShip Cruise Missile",
                UnitClassLongName = "Generic AntiShip Cruise Missile Semi-Active",
                UnitModelFileName = "asumissile_harpoon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 800.0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 800.0,
                MilitaryMaxSpeedKph = 800.0,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 10000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 7,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 1,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 120000, //too low
                MinSpeedKph = 800,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 3,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { } //"missileradarsemi"
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "asumissilesemiir", //ASu Missile semi-active homing AND IR
                UnitClassShortName = "AntiShip Cruise Missile",
                UnitClassLongName = "Generic AntiShip Cruise Missile Semi-Active IR",
                UnitModelFileName = "asumissile_harpoon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 800.0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 800.0,
                MilitaryMaxSpeedKph = 800.0,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 10000,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 7,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 1,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 120000, //too low
                MinSpeedKph = 800,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 3,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "irasu" } // "missileradarsemi"
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "aamissile",
                UnitClassShortName = "Generic AA Missile",
                UnitClassLongName = "Generic AA Missile ",
                UnitModelFileName = "aamissile_AMRAAM",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 3000,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 3000,
                MilitaryMaxSpeedKph = 3000,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 24400,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 4,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 0.5,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 100000, //too low
                MinSpeedKph = 1000,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                AgilityFactor = 4,
                EstimatedUnitPriceMillionUSD = 1,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "missileradar" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "aamissilesemi", //AA missile semi-active homing, non-active radar
                UnitClassShortName = "AA Missile",
                UnitClassLongName = "Generic AA Missile SemiActive",
                UnitModelFileName = "aamissile_AMRAAM",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 3000,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 3000,
                MilitaryMaxSpeedKph = 3000,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 24400,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 0.5,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 100000, //too low
                MinSpeedKph = 1000,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                AgilityFactor = 4,
                EstimatedUnitPriceMillionUSD = 1,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { } //"missileradarsemi" 
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "aamissilesemiir", //AA missile semi-active homing, non-active radar AND IR
                UnitClassShortName = "AA Missile",
                UnitClassLongName = "Generic AA Missile SemiActive IR",
                UnitModelFileName = "aamissile_AMRAAM",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 3000,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 3000,
                MilitaryMaxSpeedKph = 3000,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 24400,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 0.5,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 100000, //too low
                MinSpeedKph = 1000,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                AgilityFactor = 4,
                EstimatedUnitPriceMillionUSD = 1,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "iraa" } //"missileradarsemi", 
            };
            list.Add(uc);


            uc = new UnitClass()
            {
                Id = "aamissileir",
                UnitClassShortName = "AA Missile IR",
                UnitClassLongName = "Generic AA Missile IR",
                UnitModelFileName = "aamissile_AMRAAM",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Missile,
                IsAircraft = true,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Missile,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 3000,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 3000,
                MilitaryMaxSpeedKph = 3000,
                PropulsionSystem = GameConstants.PropulsionSystem.MissileJet,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = true,
                CrewTotal = 0,
                DraftM = 0,
                HighestOperatingHeightM = 24400,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 5,
                LowestOperatingHeightM = GameConstants.HEIGHT_VERY_LOW_MIN_M,
                WidthM = 0.5,
                HeightM = 0.5,
                MaxAccelerationKphSec = 500,
                MaxClimbrateMSec = 1000,
                MaxDecelerationKphSec = -100,
                MaxFallMSec = 1000,
                MaxRangeCruiseM = 100000, //too low
                MinSpeedKph = 1000,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                AgilityFactor = 4,
                CountryId = "us",

                EstimatedUnitPriceMillionUSD = 1,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "iraa" }
            };
            list.Add(uc);


            uc = new UnitClass()
            {
                Id = "aswtorpedo",
                UnitClassShortName = "Lightweight Torpedo",
                UnitClassLongName = "Lightweight Torpedo ",
                UnitModelFileName = "aswtorpedo",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Torpedo,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Torpedo,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 60,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.UnderwaterPropeller,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = GameConstants.DEPTH_PERISCOPE_MIN_M,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 9,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 20,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 20,
                MaxRangeCruiseM = 100000, //too low
                MinSpeedKph = 20,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "torpedosonar" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "asutorpedo",
                UnitClassShortName = "Heavyweight Torpedo",
                UnitClassLongName = "Heavyweight Torpedo ",
                UnitModelFileName = "asutorpedo",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Torpedo,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Torpedo,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 60,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.UnderwaterPropeller,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = GameConstants.DEPTH_PERISCOPE_MIN_M,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 3,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 20,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 20,
                MaxRangeCruiseM = 100000, //too low
                NoiseLevel = GameConstants.SonarNoiseLevel.Quiet,
                MinSpeedKph = 20,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "torpedosonar" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "rockettorpedo",
                UnitClassShortName = "Lightweight Rocket Torpedo",
                UnitClassLongName = "Lightweight Rocket Torpedo",
                UnitModelFileName = "rockettorpedo",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Torpedo,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Torpedo,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 300,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 300,
                MilitaryMaxSpeedKph = 300,
                PropulsionSystem = GameConstants.PropulsionSystem.UnderwaterPropeller,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = GameConstants.DEPTH_PERISCOPE_MIN_M,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = true,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 3,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 20,
                MaxClimbrateMSec = 20,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 20,
                MaxRangeCruiseM = 100000, //too low
                NoiseLevel = GameConstants.SonarNoiseLevel.Quiet,
                MinSpeedKph = 20,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_MISSILES_DEG_SEC,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "torpedosonar" }
            };
            list.Add(uc);

            #endregion

            #region "Surface ships"

            #region "Arleigh Burke"
            uc = new UnitClass()
            {
                Id = "arleighburke",
                UnitClassShortName = "Arleigh Burke",
                UnitClassLongName = "DDG Arleigh Burke",
                UnitModelFileName = "arleighburke",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 273,
                DraftM = 9.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 154,
                LowestOperatingHeightM = 0,
                WidthM = 18,
                HeightM = 22, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 2
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Winston S. Churchill",
                        UnitDesignation="DDG-81"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Lassen",
                        UnitDesignation="DDG-82"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Howard",
                        UnitDesignation="DDG-83"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Bulkeley",
                        UnitDesignation="DDG-84"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="McCampbell",
                        UnitDesignation="DDG-85"
                    },


                },
                SensorClassIdList = new List<string> { "visual", "anspy1d", "ansqs53" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim66standard",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=56,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bgm109tomahawktlam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rum139vlasroc",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk45gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8000,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark54torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="lrasma",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim66standard",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="lrasma",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bgm109tomahawktlam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk45gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8000,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark54torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    },
                }
            };
            list.Add(uc);
            #endregion //Arleigh Burke


            #region "Zumwalt"
            uc = new UnitClass()
            {
                Id = "zumwalt",
                UnitClassShortName = "Zumwalt",
                UnitClassLongName = "DDG Zumwalt",
                UnitModelFileName = "zumwalt",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 140,
                DraftM = 8.4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsIrShielded = true,
                IsSonarShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 180,
                LowestOperatingHeightM = 0,
                WidthM = 24.6,
                HeightM = 22, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 14789000,
                TotalMassLoadedKg = 14789000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 3450,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 2
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Zumwalt",
                        UnitDesignation="DDG-1000"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Michael Monsoor",
                        UnitDesignation="DDG-1001"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Lyndon B. Johnson",
                        UnitDesignation="DDG-1002"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "anspy3", "ansqs53" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=80,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm109tacticaltomahawk",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=40,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rum139vlasroc",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ags155mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ags155mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=12, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="lrasma",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=80,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm109tacticaltomahawk",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="lrasma",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rum139vlasroc",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ags155mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ags155mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=12, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    },
                }
            };
            list.Add(uc);
            #endregion //Zumwalt



            #region "Ticondorega"
            uc = new UnitClass()
            {
                Id = "ticonderoga",
                UnitClassShortName = "CG Ticonderoga",
                UnitClassLongName = "CG Ticonderoga",
                UnitModelFileName = "ticonderoga",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 1.2),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 390,
                DraftM = 10.2,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 173,
                LowestOperatingHeightM = 0,
                WidthM = 16.8,
                HeightM = 22, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9600000,
                TotalMassLoadedKg = 9750000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 3,
                NoiseLevel = GameConstants.SonarNoiseLevel.Noisy,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 2
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="San Jacinto",
                        UnitDesignation="CG-56"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Lake Champlain",
                        UnitDesignation="CG-57"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Philippine Sea",
                        UnitDesignation="CG-58"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Princeton",
                        UnitDesignation="CG-59"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Normandy",
                        UnitDesignation="CG-60"
                    },


                },
                SensorClassIdList = new List<string> { "visual", "anspy1d", "ansqs53", "ansqr19" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim66standard",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=56,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bgm109tomahawktlam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rum139vlasroc",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark54torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk45gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="lrasma",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim66standard",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=50,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="lrasma",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bgm109tomahawktlam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=50,
                            },

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark54torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk45gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //"Ticondorega"

            #region "Gorshkov"
            uc = new UnitClass()
            {
                Id = "gorshkov",
                UnitClassShortName = "Admiral Gorshkov",
                UnitClassLongName = "FFG Admiral Gorshkov",
                UnitModelFileName = "gorshkov",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 56,
                MilitaryMaxSpeedKph = 56,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 200,
                DraftM = 16,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsSonarShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 130,
                LowestOperatingHeightM = 0,
                WidthM = 16,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4000000,
                TotalMassLoadedKg = 4500000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Sergey Gorshkov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Kasatonov",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mr710", "mr320", "mgk345bow", "mgk345tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gsh30",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gsh30",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=30,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk9medvedka",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p800oniks",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Gorshkov

            #region "Steregushchy"
            uc = new UnitClass()
            {
                Id = "steregushchy",
                UnitClassShortName = "Steregushchiy",
                UnitClassLongName = "Steregushchiy",
                UnitModelFileName = "steregushchy",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 48,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 52,
                MilitaryMaxSpeedKph = 52,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 90,
                DraftM = 3.7,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsSonarShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 104.5,
                LowestOperatingHeightM = 0,
                WidthM = 11,
                HeightM = 16, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 2200000,
                TotalMassLoadedKg = 2200000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 1,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Steregushchiy",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Soobrazitelnyy",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Boiky",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Sovershennyy",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Stoiky",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Gromky",
                        UnitDesignation=""
                    },

                },
                SensorClassIdList = new List<string> { "visual", "furkee", "garpunb", "zaryame", "vinyetka" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="a190gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="9k38igla",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=12,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk9medvedka",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p800oniks",  //or 6 klub OR 8 kh35
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Steregushchy

            #region "Hamina"
            uc = new UnitClass()
            {
                Id = "hamina",
                UnitClassShortName = "Hamina",
                UnitClassLongName = "Hamina Missile Boat",
                UnitModelFileName = "hamina",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.3),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 26,
                DraftM = 1.7,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 51,
                LowestOperatingHeightM = 0,
                WidthM = 8.5,
                HeightM = 8, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 250000,
                TotalMassLoadedKg = 250000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_SMALL_DEG_SEC,
                CountryId = "fi",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Hamina",
                        UnitDesignation="FNS80"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Tornio",
                        UnitDesignation="FNS81"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Hanko",
                        UnitDesignation="FNS82"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Pori",
                        UnitDesignation="FNS83"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "eadstrs3d", "matildarwr", "toadfish", "sonacpta" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=6

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nsv127mm",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nsv127mm",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="umkhontoir",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs15mk3",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Hamina

            #region "Visby"
            uc = new UnitClass()
            {
                Id = "visby",
                UnitClassShortName = "Visby",
                UnitClassLongName = "Visby Corvette",
                UnitModelFileName = "visby",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.3),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 26,
                DraftM = 2.4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 72.7,
                LowestOperatingHeightM = 0,
                WidthM = 10.4,
                HeightM = 11, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 640000,
                TotalMassLoadedKg = 640000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_SMALL_DEG_SEC,
                CountryId = "se",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 1,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Visby",
                        UnitDesignation="K31"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Helsingborg",
                        UnitDesignation="K32"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Härnösand",
                        UnitDesignation="K33"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Nyköping",
                        UnitDesignation="K34"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Karlstad",
                        UnitDesignation="K35"
                    },

                },
                SensorClassIdList = new List<string> { "visual", "seagiraffe", "gdc", "hydrosciencetowed" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=6

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs15mk3",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="tp45",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Visby

            #region "Skjold"
            uc = new UnitClass()
            {
                Id = "skjold",
                UnitClassShortName = "Skjold",
                UnitClassLongName = "Skjold",
                UnitModelFileName = "skjold",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.VeryStealthy,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 83,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.3),
                MaxSpeedKph = 111,
                MilitaryMaxSpeedKph = 111,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 16,
                DraftM = 1,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 47.5,
                LowestOperatingHeightM = 0,
                WidthM = 13.5,
                HeightM = 8, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 220000,
                TotalMassLoadedKg = 274000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_SMALL_DEG_SEC,
                CountryId = "no",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Skjold",
                        UnitDesignation="P960"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Storm",
                        UnitDesignation="P961"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Skudd",
                        UnitDesignation="P962"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Steil",
                        UnitDesignation="P963"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Glimt",
                        UnitDesignation="P964"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Gnist",
                        UnitDesignation="P965"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "thalesmrr3d" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=6

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nsm",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mistral",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=0
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Skjold

            #region "Fridtjof Nansen"
            uc = new UnitClass()
            {
                Id = "fridtjofnansen",
                UnitClassShortName = "Fridtjof Nansen",
                UnitClassLongName = "FFG Fridtjof Nansen",
                UnitModelFileName = "fridtjofnansen",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 30,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 120,
                DraftM = 7.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 134,
                LowestOperatingHeightM = 0,
                WidthM = 16.8,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 5000000,
                TotalMassLoadedKg = 5290000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "no",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Fridtjof Nansen",
                        UnitDesignation="F310"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Roald Amundsen",
                        UnitDesignation="F311"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Otto Sverdrup",
                        UnitDesignation="F312"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Helge Ingstad",
                        UnitDesignation="F313"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Thor Heyerdahl",
                        UnitDesignation="F314"
                    },


                },
                SensorClassIdList = new List<string> { "visual", "anspy1f", "mrs2000", "captasmk2" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=32,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nsm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=80,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Fridtjof Nansen"

            #region "KV Harstad"
            uc = new UnitClass()
            {
                Id = "kvharstad",
                UnitClassShortName = "Harstad",
                UnitClassLongName = "NoCGV Harstad",
                UnitModelFileName = "kvharstad",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 30,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 120,
                DraftM = 7.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 134,
                LowestOperatingHeightM = 0,
                WidthM = 16.8,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 5000000,
                TotalMassLoadedKg = 5290000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "no",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 500,
                MaxCarriedUnitsTotal = 1,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="KV Harstad",
                    },

                },
                SensorClassIdList = new List<string> { "visual", "ruttersigmas6", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors4060",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=32,
                                HeightPosition=20
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"KV Harstad"

            #region "Type 23 Frigate (Duke class)"
            uc = new UnitClass()
            {
                Id = "type23frigate",
                UnitClassShortName = "Type 23 Frigate",
                UnitClassLongName = "Type 23 Frigate (Duke)",
                UnitModelFileName = "type23frigate",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 51,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 63,
                MilitaryMaxSpeedKph = 63,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 185,
                DraftM = 7.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 133,
                LowestOperatingHeightM = 0,
                WidthM = 16.1,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4900000,
                TotalMassLoadedKg = 4900000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101", 
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Richmond",
                        UnitDesignation="F239"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Somerset",
                        UnitDesignation="F82"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Sutherland",
                        UnitDesignation="F81"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kent",
                        UnitDesignation="F78"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Portland",
                        UnitDesignation="F79"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="St Albans",
                        UnitDesignation="F83"
                    },

                },
                SensorClassIdList = new List<string> { "visual", "baetype996", "kelvinhughes1007", "type2050", "type2087" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bae45mk8gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20

                            },

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="seawolf",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=32,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Type 23 Frigate"

            #region "Type 45 Destroyer (Daring class)"
            uc = new UnitClass()
            {
                Id = "type45destroyer",
                UnitClassShortName = "Type 45 Destroyer",
                UnitClassLongName = "Type 45 Destroyer (Daring)",
                UnitModelFileName = "type25destroyer",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 51,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 63,
                MilitaryMaxSpeedKph = 63,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 190,
                DraftM = 7.4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 152.4,
                LowestOperatingHeightM = 0,
                WidthM = 21.2,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 8000000,
                TotalMassLoadedKg = 8000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw159",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Daring",
                        UnitDesignation="D32"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Dauntless",
                        UnitDesignation="D33"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Diamond",
                        UnitDesignation="D34"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Dragon",
                        UnitDesignation="D35"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Defender",
                        UnitDesignation="D36"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Duncan",
                        UnitDesignation="D37"
                    },

                },
                SensorClassIdList = new List<string> { "visual", "sampson", "s1850m", "mfs7000" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bae45mk8gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="oerlikon30mm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster15",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster30",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=36, IsPrimaryWeapon=false //flare
                            },

                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="tomahawk",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bae45mk8gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="oerlikon30mm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster15",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster30",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bgm109tomahawktlam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=36, IsPrimaryWeapon=false //flare
                            },

                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="warload",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bae45mk8gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="oerlikon30mm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster15",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aster30",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=32,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="m134minigun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            

                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="stingraytorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=36, IsPrimaryWeapon=false //flare
                            },

                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        },
                    },

                }
            };
            list.Add(uc);
            #endregion //"Type 45 Destroyer (Daring)"

            #region "Iver Huitfeldt"
            uc = new UnitClass()
            {
                Id = "iverhuitfeldt",
                UnitClassShortName = "Iver Huitfeldt",
                UnitClassLongName = "FFG Iver Huitfeldt",
                UnitModelFileName = "iverhuitfeldt",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 38,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 52,
                MilitaryMaxSpeedKph = 52,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 101,
                DraftM = 5.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 138.7,
                LowestOperatingHeightM = 0,
                WidthM = 19.75,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 5000000,
                TotalMassLoadedKg = 5290000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "dk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101", 
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Iver Huitfeldt",
                        UnitDesignation="F361"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Peter Willemoes",
                        UnitDesignation="F362"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Niels Juel",
                        UnitDesignation="F363"
                    },

                },
                SensorClassIdList = new List<string> { "visual", "aso94", "smartl", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=24,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="oerlikon35mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=252,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim66standard",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=32,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mu90impact",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Ivar Huitfeldt"

            #region "Absalon"
            uc = new UnitClass()
            {
                Id = "absalon",
                UnitClassShortName = "Absalon",
                UnitClassLongName = "Absalon flexible support ship",
                UnitModelFileName = "absalon",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 32,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 44,
                MilitaryMaxSpeedKph = 44,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 100,
                DraftM = 6.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 137.6,
                LowestOperatingHeightM = 0,
                WidthM = 19.5,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6000000,
                TotalMassLoadedKg = 6600000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "dk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 1
                    },                
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Absalon",
                        UnitDesignation="L16"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Esbern Snare",
                        UnitDesignation="L17"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "aso94", "smarts", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=36,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="oerlikon35mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="fim92cstinger",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=6,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mk45gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mu90impact",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Absalon"

            #region "Sachsen"
            uc = new UnitClass()
            {
                Id = "sachsen",
                UnitClassShortName = "Sachsen",
                UnitClassLongName = "FFG Sachsen",
                UnitModelFileName = "sachsen",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 38,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 54,
                MilitaryMaxSpeedKph = 54,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 230,
                DraftM = 5,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 143,
                LowestOperatingHeightM = 0,
                WidthM = 17.44,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 5690000,
                TotalMassLoadedKg = 6000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "de",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf", 
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf", 
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 1
                    },

                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Sachsen",
                        UnitDesignation="F219"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Hamburg",
                        UnitDesignation="F220"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Hessen",
                        UnitDesignation="F221"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Thüringen",
                        UnitDesignation="F222"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "dsqs24b", "smartl", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=32,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim67standard",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=24,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mu90impact",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Sachsen"

            #region "F125 Baden-Württemberg"
            uc = new UnitClass()
            {
                Id = "f125frigate",
                UnitClassShortName = "Baden-Württemberg",
                UnitClassLongName = "FFG Baden-Württemberg",
                UnitModelFileName = "f125",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 38,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 54,
                MilitaryMaxSpeedKph = 54,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 190,
                DraftM = 5,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 149.52,
                LowestOperatingHeightM = 0,
                WidthM = 18.8,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 7200000,
                TotalMassLoadedKg = 7200000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "de",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf", 
                        NumberOfUnits = 1,
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf", 
                        NumberOfUnits = 1,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "camcopters100", 
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Baden-Württemberg",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Nordrhein-Westfalen",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Sachsen-Anhalt",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Rheinland-Pfalz",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "dsqs24b", "smartl", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim116ram",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=42,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda127mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs15mk3",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"F125 Baden-Württemberg Class Frigate"

            #region "Brandenburg"
            uc = new UnitClass()
            {
                Id = "brandenburg",
                UnitClassShortName = "Brandenburg",
                UnitClassLongName = "FFG Brandenburg",
                UnitModelFileName = "brandenburg",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 38,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 54,
                MilitaryMaxSpeedKph = 54,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 230,
                DraftM = 6.8,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 138.9,
                LowestOperatingHeightM = 0,
                WidthM = 16.7,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4900000,
                TotalMassLoadedKg = 4900000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "de",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw159", 
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw159", 
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 1,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "camcopters100",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        NumberOfUnits = 1,
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Brandenburg",
                        UnitDesignation="F215"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Schleswig-Holstein",
                        UnitDesignation="F216"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Bayern",
                        UnitDesignation="F217"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Mecklenburg-Vorpommern",
                        UnitDesignation="F218"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "thaleslw08", "smarts", "dsqs24b", "lfasstowed" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=16,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mauserbk27",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim116ram",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=21,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs15mk3",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark46torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"Brandenburg"

            #region "Halifax"
            uc = new UnitClass()
            {
                Id = "halifax",
                UnitClassShortName = "Halifax",
                UnitClassLongName = "FFH Halifax",
                UnitModelFileName = "halifax",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 54,
                MilitaryMaxSpeedKph = 54,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 225,
                DraftM = 4.9,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 134.1,
                LowestOperatingHeightM = 0,
                WidthM = 16.4,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4770000,
                TotalMassLoadedKg = 4770000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ca",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Montreál",
                        UnitDesignation="FFH 336"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Fredericton",
                        UnitDesignation="FFH 337"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Winnipeg",
                        UnitDesignation="FFH 338"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Charlottetown",
                        UnitDesignation="FFH 339"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="St. John's",
                        UnitDesignation="FFH 340"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ottawa",
                        UnitDesignation="FFH 341"
                    },


                },
                SensorClassIdList = new List<string> { "visual", "seagiraffe", "ansps49", "ansqs510", "ansqr501" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="bofors5770",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=16,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark46torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=24,
                                YPosition=50,
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericflare", MaxAmmunition=6, IsPrimaryWeapon=false //flare
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Halifax

            #region "Gawron"
            uc = new UnitClass()
            {
                Id = "gawron",
                UnitClassShortName = "Gawron",
                UnitClassLongName = "Gawron Corvette",
                UnitModelFileName = "gawron",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.4),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 48, //guess
                DraftM = 3.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 90.1,
                LowestOperatingHeightM = 0,
                WidthM = 12.8,
                HeightM = 11, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1650000,
                TotalMassLoadedKg = 2000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "pl",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 1,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Ślązak",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kujawiak",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Krakowiak",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Mazur",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kurp",
                        UnitDesignation=""
                    },

                },
                SensorClassIdList = new List<string> { "visual", "smarts", "dsqs24b", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda76mm",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=6

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs15mk3",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=32,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mu90impact",
                                HeightPosition=6,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },                        
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Gawron

            #region "De Zeven Provinciën"
            uc = new UnitClass()
            {
                Id = "zeven",
                UnitClassShortName = "De Zeven Provinciën",
                UnitClassLongName = "De Zeven Provinciën",
                UnitModelFileName = "zeven",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 38,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 54,
                MilitaryMaxSpeedKph = 54,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 232,
                DraftM = 5.18,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 144.24,
                LowestOperatingHeightM = 0,
                WidthM = 18.8,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 5740000,
                TotalMassLoadedKg = 6050000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "nl",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 267,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "nh90nhf", 
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="De Zeven Provinciën",
                        UnitDesignation="F802"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Tromp",
                        UnitDesignation="F803"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="De Ruyter",
                        UnitDesignation="F804"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Evertsen",
                        UnitDesignation="F805"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "dsqs24b", "smartl", "genericir" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim162essm",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=32,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="otobreda127mm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim67standard",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=32,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gau8agatling",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=270,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="gau8agatling",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },                            
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark46torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rgm84harpoon",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=50,
                            },                            
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=36, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //"De Zeven Provinciën"

            #region "Kirov"
            uc = new UnitClass()
            {
                Id = "kirov",
                UnitClassShortName = "Kirov",
                UnitClassLongName = "CCG Kirov",
                UnitModelFileName = "kirov",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 32,
                MaxHitpoints = Convert.ToInt32(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 1.2),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 36,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 710,
                DraftM = 9.1,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 252,
                LowestOperatingHeightM = 0,
                WidthM = 28.5,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 24300000,
                TotalMassLoadedKg = 28000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 3
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Pyotr Veliky",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Lazarev",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Nakhimov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Ushakov",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mr800", "mr710", "mgk355bow", "mgk355tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=44,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p700granit",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s300fm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=96,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=192,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=192,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk2viyuga",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Kirov

            #region "Mistral class"

            uc = new UnitClass()
            {
                Id = "mistralassault",
                UnitClassShortName = "Mistral",
                UnitClassLongName = "Mistral amphibious assault ship",
                UnitModelFileName = "mistral",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 32,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 2600,
                DraftM = 6.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 199,
                LowestOperatingHeightM = 0,
                WidthM = 32,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 16500000,
                TotalMassLoadedKg = 21300000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 600,
                MaxCarriedUnitsTotal = 35,
                MaxSimultanousTakeoffs = 4,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 6,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        WeaponLoadName = "Naval Strike",
                        NumberOfUnits = 6,
                    },

                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Vladivostok",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Sevastopol",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mr800", "torradar", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.IsAmphibiousAssault,
                    GameConstants.Role.LaunchRotaryWingAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=192,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //Mistral

            #region "Slava"
            uc = new UnitClass()
            {
                Id = "slava",
                UnitClassShortName = "Slava",
                UnitClassLongName = "CCG Slava",
                UnitModelFileName = "slava",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 32,
                MaxHitpoints = Convert.ToInt32(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 1.2),
                MaxSpeedKph = 36,
                MilitaryMaxSpeedKph = 36,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 485,
                DraftM = 8.4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 186.4,
                LowestOperatingHeightM = 0,
                WidthM = 20.8,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000000,
                TotalMassLoadedKg = 1250000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 800,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 1,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 1
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Moskva",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Marshal Ustinov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Varyag",
                        UnitDesignation=""
                    },

                },
                SensorClassIdList = new List<string> { "visual", "mr800", "mr710", "mg332bow", "mgk355tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="vulcan",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=44,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p1000vulcan",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s300fm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=64,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=96,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="9k33osam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk2viyuga",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Bazalt",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=44,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p500bazalt",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s300fm",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=64,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=96,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="9k33osam",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Slava

            #region "Project 21956"
            uc = new UnitClass()
            {
                Id = "project21956",
                UnitClassShortName = "Project 21956",
                UnitClassLongName = "DDG Project 21956",
                UnitModelFileName = "project21956",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Reduced, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 36,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 1.3),
                MaxSpeedKph = 55,
                MilitaryMaxSpeedKph = 55,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 300,
                DraftM = 5.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsSonarShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 163,
                LowestOperatingHeightM = 0,
                WidthM = 19,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 4000000,
                TotalMassLoadedKg = 4500000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 3,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 1
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 1
                    },

                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Opasnyy", //made up, based on old Skoryy class
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ostryy",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ognenny",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ostorozhnyy",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mae4k", "mineralme", "zaryame", "vinyetka" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=60,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s300fm",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=48,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk9medvedka",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=16,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3m54klub",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Project 21956

            #region "Nimitz"
            uc = new UnitClass()
            {
                Id = "nimitz",
                UnitClassShortName = "Nimitz",
                UnitClassLongName = "USS Nimitz",
                UnitModelFileName = "nimitz",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 56,
                MilitaryMaxSpeedKph = 56,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 5680,
                DraftM = 11.3,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 333,
                LowestOperatingHeightM = 0,
                WidthM = 76.8,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 24300000,
                TotalMassLoadedKg = 28000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 90,
                MaxSimultanousTakeoffs = 2,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 8,
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 6,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 1,
                        WeaponLoadType = GameConstants.WeaponLoadType.RefuellingPlane,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "fa18sh",
                        NumberOfUnits = 4,
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "fa18sh",
                        NumberOfUnits = 4,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ea18ggrowler",
                        NumberOfUnits = 2,
                        WeaponLoadName = "Electronic Warfare (Strike)"
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "e2hawkeye",
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "mq9mariner",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "pegasus",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2,
                    },
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                   new UnitClassVesselName()
                    {
                        UnitName="Theodore Roosevelt",
                        UnitDesignation="CVN-71"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Abraham Lincoln",
                        UnitDesignation="CVN-72"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="George Washington",
                        UnitDesignation="CVN-73"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="John C. Stennis",
                        UnitDesignation="CVN-74"
                    },

                },
                SensorClassIdList = new List<string> { "visual", "ansps48e", "ansps49" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.LaunchFixedWingAircraft,
                    GameConstants.Role.LaunchRotaryWingAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim116ram",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=196,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim7seasparrow",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=24,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //Nimitz

            #region "Wasp class"
            uc = new UnitClass()
            {
                Id = "wasp",
                UnitClassShortName = "Wasp",
                UnitClassLongName = "LHD Wasp",
                UnitModelFileName = "wasp",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 1.6),
                MaxSpeedKph = 41,
                MilitaryMaxSpeedKph = 41,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 1208,
                DraftM = 8.1,
                HighestOperatingHeightM = 0,
                StabilityBonus = 2,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 253.2,
                LowestOperatingHeightM = 0,
                WidthM = 31.8,
                HeightM = 40, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 41150000,
                TotalMassLoadedKg = 41150000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 42,
                NoiseLevel = GameConstants.SonarNoiseLevel.Noisy,
                MaxSimultanousTakeoffs = 2,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 6,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 6,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35b",
                        NumberOfUnits = 6,
                    },
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Bonhomme Richard",
                        UnitDesignation="LHD-6"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Iwo Jima",
                        UnitDesignation="LHD-7"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Makin Island",
                        UnitDesignation="LHD-8"
                    },
                },
                SensorClassIdList = new List<string> { "visual", "ansps48e", "ansps67", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackLand, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchRotaryWingAircraft,
                    GameConstants.Role.IsAmphibiousAssault,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim116ram",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=92,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rim7seasparrow",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=24,
                                HeightPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="mk36srboc", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="slq25", MaxAmmunition=0, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //"Wasp class"

            #region "Queen Elizabeth"
            uc = new UnitClass()
            {
                Id = "queenelizabeth",
                UnitClassShortName = "Queen Elizabeth",
                UnitClassLongName = "Queen Elizabeth",
                UnitModelFileName = "queenelizabeth",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 56,
                MilitaryMaxSpeedKph = 56,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 600,
                DraftM = 11,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 284,
                LowestOperatingHeightM = 0,
                WidthM = 73,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 65000000,
                TotalMassLoadedKg = 65000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 3000,
                MaxCarriedUnitsTotal = 40,
                MaxSimultanousTakeoffs = 2,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 10,
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 6,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35c",
                        NumberOfUnits = 1,
                        WeaponLoadType = GameConstants.WeaponLoadType.RefuellingPlane,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ea18ggrowler",
                        NumberOfUnits = 2,
                        WeaponLoadName="Electronic Warfare (Strike)",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "e2hawkeye",
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw101",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "aw159",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "taranis",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2
                    },
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                   new UnitClassVesselName()
                    {
                        UnitName="HMS Queen Elizabeth",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="HMS Price of Wales",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "smartl" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.LaunchFixedWingAircraft,
                    GameConstants.Role.LaunchRotaryWingAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="phalanxciws",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=12, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //Queen Elizabeth

            #region "Admiral Kuznetsov"

            uc = new UnitClass()
            {
                Id = "kuznetsov",
                UnitClassShortName = "Admiral Kuznetsov",
                UnitClassLongName = "TAKR Admiral Kuznetsov",
                UnitModelFileName = "kuznetsov",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.ShortRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 32,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 2600,
                DraftM = 9.1,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 306,
                LowestOperatingHeightM = 0,
                WidthM = 72.3,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 60000000,
                TotalMassLoadedKg = 65000000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 60,
                MaxSimultanousTakeoffs = 2,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sukhoipakfa",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        NumberOfUnits = 8,
                    },                    
                    new UnitClassStorage()
                    {
                        UnitClassId = "sukhoipakfa",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 4,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "mig29k",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        NumberOfUnits = 4,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "mig29k",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack, 
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "mig29k",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "migskat",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike, 
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "migskat",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack, 
                        NumberOfUnits = 2,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "migskat",
                        WeaponLoadType = GameConstants.WeaponLoadType.ElectronicWarfare, 
                        NumberOfUnits = 1,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 4,
                    },
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Kuznetsov",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mr710", "mr320", "mgk345bow", "mgk345tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                    GameConstants.Role.LaunchFixedWingAircraft,
                    GameConstants.Role.LaunchRotaryWingAircraft,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k87kortik",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=16,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=0
                            },
                            //new UnitClassWeaponLoad
                            //{
                            //    WeaponClassId="p700granit",
                            //    HeightPosition=20,
                            //    XPosition=0,
                            //    WeaponBearingDeg=0,
                            //    WeaponPitchDeg=90,
                            //    IsPrimaryWeapon=true,
                            //    MaxAmmunition=12,
                            //    YPosition=0
                            //},
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=192,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //kuznetsov

            #region "Udaloy 2"
            uc = new UnitClass()
            {
                Id = "udaloy2",
                UnitClassShortName = "Udaloy II",
                UnitClassLongName = "DDG Uldaloy II",
                UnitModelFileName = "udaloy2", //TODO: COMPLETE
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 35,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 300,
                DraftM = 6.2,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 163,
                LowestOperatingHeightM = 0,
                WidthM = 19.3,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6200000,
                TotalMassLoadedKg = 7900000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 2
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Chabanenko",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Vice-Admiral Kulakov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Kucherov",
                        UnitDesignation=""
                    },
                },
                SensorClassIdList = new List<string> { "visual", "mr710", "mr320", "mgk345bow", "mgk345tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p270moskit",
                                YPosition = -10,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=8,
                                HeightPosition=20

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=64,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k87kortik",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak130gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=30,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=80,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=80,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=-90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=80,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=-90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=80,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk2viyuga",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //UdaloyII

            #region "Udaloy 1"
            uc = new UnitClass()
            {
                Id = "udaloy1",
                UnitClassShortName = "Udaloy I",
                UnitClassLongName = "DDG Uldaloy I",
                UnitModelFileName = "udaloy1",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.MediumSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 35,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 300,
                DraftM = 6.2,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 163,
                LowestOperatingHeightM = 0,
                WidthM = 19.3,
                HeightM = 32, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 6200000,
                TotalMassLoadedKg = 7900000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 1100,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                IsAlwaysVisibleForEnemy = false,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        NumberOfUnits = 2
                    }
                },
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Levchenko",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Vinogradov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Kharlamov",
                        UnitDesignation=""
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Admiral Panteleyev",
                        UnitDesignation=""
                    },                
                },
                SensorClassIdList = new List<string> { "visual", "mr710", "mr320", "mgk345bow", "mgk345tail" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=64,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k87kortik",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=2,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dual10070",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=60,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo", //hmm
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=30,
                                YPosition=0
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="30mmaa",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="30mmaa",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="30mmaa",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="30mmaa",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=-90,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=20,
                                YPosition=50
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk2viyuga",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=10,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="genericchaff", MaxAmmunition=6, IsPrimaryWeapon=false //chaff
                            },
                            new UnitClassWeaponLoad 
                            {
                                WeaponClassId="generictorpedodecoy", MaxAmmunition=6, IsPrimaryWeapon=false //antitorp
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Udaloy 1

            #region "Project 206MR Vikhr 'Matka'"
            uc = new UnitClass()
            {
                Id = "206mrvikhr",
                UnitClassShortName = "Project 206MR Vikhr",
                UnitClassLongName = "Project 206MR Vikhr 'Matka' Hydrofoil Missile Boat",
                UnitModelFileName = "206mrvikhr",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 70,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.3),
                MaxSpeedKph = 77.78,
                MilitaryMaxSpeedKph = 77.78,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 30,
                DraftM = 3.26,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 38.6,
                LowestOperatingHeightM = 0,
                WidthM = 7.6,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 230000,
                TotalMassLoadedKg = 257000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_SMALL_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                SensorClassIdList = new List<string> { "visual", "garpunb" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsSurfaceCombattant,
                    GameConstants.Role.AttackSurface,
                    GameConstants.Role.ASuW,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ak630gun",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = false,
                                MaxAmmunition=20,
                                HeightPosition=6

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="kh35",
                                HeightPosition=3,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=0
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Project 206MR Vikhr ”Matka”

            #region "Civilian ships"

            #region "Containership M/S Pamela"
            uc = new UnitClass()
            {
                Id = "pamela",
                UnitClassShortName = "Containership",
                UnitClassLongName = "Commercial Containership",
                UnitModelFileName = "pamela",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 15,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip)),
                MaxSpeedKph = 27,
                MilitaryMaxSpeedKph = 27,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 30,
                DraftM = 4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 45.54,
                LowestOperatingHeightM = 0,
                WidthM = 9.5,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 385000, //guess
                TotalMassLoadedKg = 585000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                SensorClassIdList = new List<string> { "visual", "civilianradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.TransportSupplies,
                    GameConstants.Role.IsSurfaceShip,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>()

            };
            list.Add(uc);
            #endregion //Containership Pamela

            #region "Commercial Fishing Vessel"
            uc = new UnitClass()
            {
                Id = "fishingboat",
                UnitClassShortName = "Fishing Boat",
                UnitClassLongName = "Commercial Fishing Boat",
                UnitModelFileName = "fishingboat",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.SmallSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 15,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 0.2),
                MaxSpeedKph = 27,
                MilitaryMaxSpeedKph = 27,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 6,
                DraftM = 4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 22,
                LowestOperatingHeightM = 0,
                WidthM = 6,
                HeightM = 12, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 38500, //guess
                TotalMassLoadedKg = 58500,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                SensorClassIdList = new List<string> { "visual", "civilianradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.TransportSupplies,
                    GameConstants.Role.IsSurfaceShip,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>()

            };
            list.Add(uc);
            #endregion //Fishing boat

            #region "MV Sirius Star Oil Tanker"
            uc = new UnitClass()
            {
                Id = "siriusstar",
                UnitClassShortName = "Oil Tanker",
                UnitClassLongName = "Commercial Oil Tanker",
                UnitModelFileName = "siriusstar",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.SurfaceShip,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LargeSurface,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 15,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip)),
                MaxSpeedKph = 27,
                MilitaryMaxSpeedKph = 27,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                CrewTotal = 25,
                DraftM = 22.5,
                HighestOperatingHeightM = 0,
                StabilityBonus = 0,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = true,
                IsSubmarine = false,
                LengthM = 332,
                LowestOperatingHeightM = 0,
                WidthM = 60,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 120000, //guess
                TotalMassLoadedKg = 162252,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_VERY_LARGE_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 20,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                SensorClassIdList = new List<string> { "visual", "civilianradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.TransportSupplies,
                    GameConstants.Role.IsSurfaceShip,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>()

            };
            list.Add(uc);
            #endregion //Oil Tanker

            #endregion

            #endregion //Surface ships

            #region "Submarines"

            #region "Ula"
            uc = new UnitClass()
            {
                Id = "ula",
                UnitClassShortName = "Ula",
                UnitClassLongName = "Ula",
                UnitModelFileName = "ula",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 30,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 43,
                MilitaryMaxSpeedKph = 30,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 20,
                DraftM = 4.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 59,
                LowestOperatingHeightM = -500,
                WidthM = 5.4,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1150000,
                TotalMassLoadedKg = 1150000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "no",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 700,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Ula",
                        UnitDesignation="S300"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Utsira",
                        UnitDesignation="S301"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Utstein",
                        UnitDesignation="S302"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Utvær",
                        UnitDesignation="S303"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Uthaug",
                        UnitDesignation="S304"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Uredd",
                        UnitDesignation="S305"
                    },


                },
                SensorClassIdList = new List<string> { "kelvinhughes1007", "csu83" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="aegdm2a3torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=14,
                                YPosition=20
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Ula

            #region "Virginia"
            uc = new UnitClass()
            {
                Id = "virginia",
                UnitClassShortName = "Virginia",
                UnitClassLongName = "Virginia attack submarine",
                UnitModelFileName = "virginia",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 39,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 134,
                DraftM = 4.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 115,
                LowestOperatingHeightM = -610,
                WidthM = 10.36,
                HeightM = 25, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 7900000,
                TotalMassLoadedKg = 7900000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="USS Virginia",
                        UnitDesignation="SSN-774"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS Texas",
                        UnitDesignation="SSN-775"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS Hawaii",
                        UnitDesignation="SSN-776"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS North Carolina",
                        UnitDesignation="SSN-777"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS New Hampshire",
                        UnitDesignation="SSN-778"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS New Mexico",
                        UnitDesignation="SSN-779"
                    },


                },
                SensorClassIdList = new List<string> { "anbqq5", "tb29" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Tactical Tomahawk",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ugm109tacticaltomahawk",
                                HeightPosition=10,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark48torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=26,
                                YPosition=20
                            },

                        },

                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="ASM",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ugm84harpoon",
                                HeightPosition=10,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark48torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=26,
                                YPosition=20
                            },

                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //Virginia

            #region "Ohio SSGN"
            uc = new UnitClass()
            {
                Id = "ohio",
                UnitClassShortName = "Ohio",
                UnitClassLongName = "Ohio SSGN",
                UnitModelFileName = "ohio",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 30,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 60,
                MilitaryMaxSpeedKph = 60,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 155,
                DraftM = 10.8,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 170,
                LowestOperatingHeightM = -500,
                WidthM = 13,
                HeightM = 18, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 16499000,
                TotalMassLoadedKg = 18750000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_LARGE_DEG_SEC,
                CountryId = "us",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="USS Ohio",
                        UnitDesignation="SSGN 726"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS Michigan",
                        UnitDesignation="SSGN-727"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS Florida",
                        UnitDesignation="SSGN-728"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="USS Georgia",
                        UnitDesignation="SSGN-729"
                    },
                },
                SensorClassIdList = new List<string> { "anbqq6", "anbqs13", "tb29" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.AttackLand,
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Tactical Tomahawk",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ugm109tacticaltomahawk",
                                HeightPosition=10,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=154,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mark48torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=26,
                                YPosition=20
                            },

                        },

                    },
                }
            };
            list.Add(uc);

            #endregion //Ohio SSGN

            #region "Astute"
            uc = new UnitClass()
            {
                Id = "astute",
                UnitClassShortName = "Astute",
                UnitClassLongName = "Astute submarine",
                UnitModelFileName = "astute",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 30,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 56,
                MilitaryMaxSpeedKph = 56,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 98,
                DraftM = 4.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 97,
                LowestOperatingHeightM = -350,
                WidthM = 11.3,
                HeightM = 25, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 7400000,
                TotalMassLoadedKg = 7400000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "uk",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Astute",
                        UnitDesignation="S119"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ambush",
                        UnitDesignation="S120"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Artful",
                        UnitDesignation="S121"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Audacious",
                        UnitDesignation="S122"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Anson",
                        UnitDesignation="S123"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Agamemnon",
                        UnitDesignation="S124"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Ajax",
                        UnitDesignation="S125"
                    },
                },
                SensorClassIdList = new List<string> { "type2079", "type2065", "type2076" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.AttackLand,
                    GameConstants.Role.AttackLandStandoff,
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="Tactical Tomahawk",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ugm109tacticaltomahawk",
                                HeightPosition=10,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="spearfish",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=28,
                                YPosition=20
                            },

                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="TLAM",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="ugm109tomahawktlam",
                                HeightPosition=10,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="spearfish",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=28,
                                YPosition=20
                            },
                        },
                    },
                }
            };
            list.Add(uc);

            #endregion //Astute

            #region "Akula"
            uc = new UnitClass()
            {
                Id = "akula",
                UnitClassShortName = "Shchuka-B",
                UnitClassLongName = "Project 971 Shchuka-B 'Akula II'",
                UnitModelFileName = "akula",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 65,
                MilitaryMaxSpeedKph = 65,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 73,
                DraftM = 9.7,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 115,
                LowestOperatingHeightM = -600,
                WidthM = 10.36,
                HeightM = 25, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 7900000,
                TotalMassLoadedKg = 12770000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Vepr",
                        UnitDesignation="K-157"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Gepard",
                        UnitDesignation="K-335"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kuguar",
                        UnitDesignation="K-337"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Rys",
                        UnitDesignation="K-333"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Nerpa",
                        UnitDesignation="K-152"
                    },



                },
                SensorClassIdList = new List<string> { "chiblis", "mgk540bow", "mgk540atail", "pelamida" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type6576torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=8,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3m54klub", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk2viyuga",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=10,
                                YPosition=36
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="generictorpedodecoy", IsPrimaryWeapon=false, MaxAmmunition=6, //torpedo decoy
                            },

                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Akula

            #region "Yasen"
            uc = new UnitClass()
            {
                Id = "yasen",
                UnitClassShortName = "Yasen",
                UnitClassLongName = "Project 885 Yasen submarine",
                UnitModelFileName = "yasen",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 50,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 65,
                MilitaryMaxSpeedKph = 65,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 95,
                DraftM = 8.4,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 120,
                LowestOperatingHeightM = -600,
                WidthM = 15,
                HeightM = 25, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9500000,
                TotalMassLoadedKg = 11800000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Severodvinsk",
                        UnitDesignation="K-329"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kazan",
                    },
                },
                SensorClassIdList = new List<string> { "mgk500bow", "mg519flank", "skat3towed", "myedvyeditsa971" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type6576torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="p800oniks", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s10granat", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="generictorpedodecoy", IsPrimaryWeapon=false, MaxAmmunition=6, //torpedo decoy
                            },

                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Klub",
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=6,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type6576torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3m54klub", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3m14eklub", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="generictorpedodecoy", IsPrimaryWeapon=false, MaxAmmunition=6, //torpedo decoy
                            },

                        },
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="Land attack",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=6,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type6576torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="va111etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s10granat", 
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=24,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="generictorpedodecoy", IsPrimaryWeapon=false, MaxAmmunition=6, //torpedo decoy
                            },

                        },

                    }
                }
            };
            list.Add(uc);
            #endregion //Yasen

            #region "Lada"
            uc = new UnitClass()
            {
                Id = "lada",
                UnitClassShortName = "Lada",
                UnitClassLongName = "Project 677 Lada",
                UnitModelFileName = "lada",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 37,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 40,
                MilitaryMaxSpeedKph = 40,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 38,
                DraftM = 6.5,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 72,
                LowestOperatingHeightM = -300,
                WidthM = 7,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 2700000,
                TotalMassLoadedKg = 2700000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "ru",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 2800000000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Sankt Peterburg",
                        UnitDesignation="B-585"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Kronshtadt",
                        UnitDesignation="B-586"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Petrozavodsk",
                        UnitDesignation="B-587"
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Sevastopol",
                        UnitDesignation="B-588"
                    },

                },
                SensorClassIdList = new List<string> { "lirabow", "liratail", "liratowed" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3m54klub",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=6,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=6,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=18,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="9k38igla",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=3,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk7veter",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=6,
                                YPosition=36
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="noasm", //no anti-ship missiles
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="apr3etorpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=6,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type5365torpedo",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=18,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="9k38igla",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=3,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rpk7veter",
                                HeightPosition=20,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=6,
                                YPosition=36
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Lada

            #region "Type 212 Sub"
            uc = new UnitClass()
            {
                Id = "type212",
                UnitClassShortName = "Type 212",
                UnitClassLongName = "Type 212 Submarine",
                UnitModelFileName = "type212",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 24,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 37,
                MilitaryMaxSpeedKph = 37,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 27,
                DraftM = 6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 56,
                LowestOperatingHeightM = -500,
                WidthM = 7,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1830000,
                TotalMassLoadedKg = 1830000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "de",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 700,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="S181",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="S182",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="S183",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="S184",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="S185",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="S186",
                    },

                },
                SensorClassIdList = new List<string> { "tas3towed", "fas3hull", "kelvinhughes1007", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="dm2a4torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=13,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="idassam",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=false,
                                MaxAmmunition=4,
                                YPosition=20
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Type 212

            #region "Gotland Sub"
            uc = new UnitClass()
            {
                Id = "gotland",
                UnitClassShortName = "Gotland",
                UnitClassLongName = "Gotland Submarine",
                UnitModelFileName = "gotland",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.Submarine,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.Submarine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Stealthy, //stealthy?
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 20,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine),
                MaxSpeedKph = 37,
                MilitaryMaxSpeedKph = 37,
                PropulsionSystem = GameConstants.PropulsionSystem.ShipConventional,
                IsSonarShielded = true,
                NoiseLevel = GameConstants.SonarNoiseLevel.VeryQuiet,
                CrewTotal = 24,
                DraftM = 5.6,
                HighestOperatingHeightM = 0,
                StabilityBonus = 1,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = true,
                LengthM = 60.4,
                LowestOperatingHeightM = -500,
                WidthM = 6.2,
                HeightM = 9, //guess
                MaxAccelerationKphSec = 10,
                MaxClimbrateMSec = 2,
                MaxDecelerationKphSec = -10,
                MaxFallMSec = 2,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1599000,
                TotalMassLoadedKg = 1599000,
                TurnRangeDegrSec = GameConstants.TURN_RATE_SHIP_MED_DEG_SEC,
                CountryId = "se",
                AcquisitionCostCredits = 1000,
                EstimatedUnitPriceMillionUSD = 700,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                IsAlwaysVisibleForEnemy = false,
                VesselNames = new List<UnitClassVesselName>()
                {
                    new UnitClassVesselName()
                    {
                        UnitName="Gotland",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Uppland",
                    },
                    new UnitClassVesselName()
                    {
                        UnitName="Halland",
                    },
                },
                SensorClassIdList = new List<string> { "csu902bow", "csu902intercept", "csu902flank", },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackSubmarine, 
                    GameConstants.Role.AttackSurface, 
                    GameConstants.Role.IsSubmarine,
                    GameConstants.Role.ASW,
                    GameConstants.Role.ASuW,

                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="torpedo62",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=16,
                                YPosition=20
                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="type43x2torpedo",
                                HeightPosition=0,
                                XPosition=0,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=0,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=4,
                                YPosition=20
                            },
                        }
                    }
                }
            };
            list.Add(uc);
            #endregion //Gotland sub

            #endregion //Submarines

            #region "Airports, SAM Batteries, and other land installations"

            #region "Airports"

            uc = new UnitClass()
            {
                Id = "ukairportlarge",
                UnitClassShortName = "NATO Airport",
                UnitClassLongName = "NATO Airport (large runway)",
                UnitModelFileName = "airport_large",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 500,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1500,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "uk",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 1000,
                MaxCarriedUnitsTotal = 200,
                MaxSimultanousTakeoffs = 2,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "f22",
                        NumberOfUnits = 4
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35a",
                        NumberOfUnits = 4,
                        WeaponLoadName = ""
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35a",
                        NumberOfUnits = 2,
                        WeaponLoadName = "Naval strike"
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "f35a",
                        NumberOfUnits = 2,
                        WeaponLoadName = "Standoff Land Attack"
                    },                    
                    new UnitClassStorage()
                    {
                        UnitClassId = "typhoon",
                        NumberOfUnits = 4,
                        WeaponLoadName = ""
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "sh60b",
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "e3sentry",
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "kc767",
                        NumberOfUnits = 3
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "p8poseidon",
                        NumberOfUnits = 2,
                        WeaponLoadName = "ASW",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "p8poseidon",
                        NumberOfUnits = 2,
                        WeaponLoadName = "Naval strike",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "p8poseidon",
                        NumberOfUnits = 2,
                        WeaponLoadName = "Land attack",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "p8poseidon",
                        NumberOfUnits = 1,
                        WeaponLoadName = "Electronic warfare",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "p8poseidon",
                        NumberOfUnits = 1,
                        WeaponLoadName = "Deploy mines",
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "mq9mariner",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                    },                
                    new UnitClassStorage()
                    {
                        UnitClassId = "taranis",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.Strike,
                    },                
                    new UnitClassStorage()
                    {
                        UnitClassId = "pegasus",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },                
                },
                SensorClassIdList = new List<string> { "visual", "mpq64sentinel" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.LaunchFixedWingAircraft, 
                    GameConstants.Role.LaunchRotaryWingAircraft, 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="amraam",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nasams2amraam",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12 * 6,
                                YPosition=0
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="patriot",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mim104fpatriot",
                                YPosition = -200,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=12 * 6,
                                HeightPosition=10

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nasams2amraam",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=0,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=12 * 6,
                                YPosition=0
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="none",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                        }
                    },
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "russianairportlarge",
                UnitClassShortName = "Russian Airport",
                UnitClassLongName = "Russian Airport (large runway)",
                UnitModelFileName = "airport_large",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.LongRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 500,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1500,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "ru",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 1000,
                MaxCarriedUnitsTotal = 200,
                MaxSimultanousTakeoffs = 2,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                    new UnitClassStorage()
                    {
                        UnitClassId = "sukhoipakfaa",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        NumberOfUnits = 6,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "sukhoipakfaa",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "ka27",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 4
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "su35",
                        WeaponLoadType = GameConstants.WeaponLoadType.AirSuperiority,
                        NumberOfUnits = 4
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "su35",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 4,
                    },                    
                    new UnitClassStorage()
                    {
                        UnitClassId = "berieva50u",
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "il78",
                        NumberOfUnits = 3
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "tu22m3",
                        NumberOfUnits = 4,
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "tu22m3",
                        NumberOfUnits = 4,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "tu95",
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "tu95",
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                        NumberOfUnits = 4
                    },

                    new UnitClassStorage()
                    {
                        UnitClassId = "tu95",
                        WeaponLoadType = GameConstants.WeaponLoadType.ASW,
                        NumberOfUnits = 2
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "migskat",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.NavalStrike,
                    },
                    new UnitClassStorage()
                    {
                        UnitClassId = "migskat",
                        NumberOfUnits = 2,
                        WeaponLoadType = GameConstants.WeaponLoadType.LandAttack,
                    },

                },
                SensorClassIdList = new List<string> { "visual", "torradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.LaunchFixedWingAircraft, 
                    GameConstants.Role.LaunchRotaryWingAircraft, 
                    GameConstants.Role.IsLandInstallation,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="kinzhal",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=64,
                                YPosition=0
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="triumf",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s400trimuf",
                                YPosition = -200,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=64,
                                HeightPosition=10

                            },
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                HeightPosition=20,
                                XPosition=10,
                                WeaponBearingDeg=90,
                                WeaponPitchDeg=45,
                                IsPrimaryWeapon=true,
                                MaxAmmunition=64,
                                YPosition=0
                            },
                        }
                    },
                    new UnitClassWeaponLoads()
                    {
                        Name="none",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                        }
                    },

                }
            };
            list.Add(uc);

            #endregion //airports

            #region "Seaports and oil installations"

            uc = new UnitClass()
            {
                Id = "oilplatform",
                UnitClassShortName = "Oil Platform",
                UnitClassLongName = "Oil Production Platform",
                UnitModelFileName = "oilplatform",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation, //hmm
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true, //try false?
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation) * .2),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 100,
                DraftM = 40,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 100,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 100,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "no",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 4000,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                },
                SensorClassIdList = new List<string> { "visual", "civilianradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.LaunchRotaryWingAircraft, 
                    GameConstants.Role.IsLandInstallation, //at sea, obviously!
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {

                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "seaport",
                UnitClassShortName = "Sea Port",
                UnitClassLongName = "Sea Port",
                UnitModelFileName = "seaport",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.Helicopter,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 1000,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 500,
                LowestOperatingHeightM = 0,
                WidthM = 500,
                HeightM = 60,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "no",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 4000,
                MaxCarriedUnitsTotal = 2,
                MaxSimultanousTakeoffs = 1,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                },
                SensorClassIdList = new List<string> { "visual", "airportradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.LaunchRotaryWingAircraft, 
                    GameConstants.Role.IsLandInstallation,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {

                }
            };
            list.Add(uc);
            #endregion //sea ports and oil installations

            #region "SAM Battery Systems"

            #region "NATO SAM BAttery Systems"

            uc = new UnitClass()
            {
                Id = "mim23hawkbattery",
                UnitClassShortName = "MIM-23 Hawk SAM",
                UnitClassLongName = "MIM-23 Hawk SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "mpq64sentinel" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mim23hawk",
                                YPosition = -0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=20,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "mim104patriotbattery",
                UnitClassShortName = "MIM-104 Patriot SAM",
                UnitClassLongName = "MIM-104 Patriot SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "mpq65patriot" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="mim104fpatriot",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition = 16 * 4,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "nasams2battery",
                UnitClassShortName = "NASAMS 2 SAM",
                UnitClassLongName = "NASAMS 2 AMRAAM SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "ocfcs2000" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="nasams2amraam",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition = 16 * 4,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "rbs23sambattery",
                UnitClassShortName = "RBS 23 SAM",
                UnitClassLongName = "RBS 23 BAMSE SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "ericsongiraffe" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rbs23bamse",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition = 16 * 4,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "rapier2sambattery",
                UnitClassShortName = "Rapier 2",
                UnitClassLongName = "Rapier II CAMM(L) SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "amdaggerradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="rapier2sam",
                                YPosition = 0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition = 16 * 4,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);


            #endregion //NATO SAM Battery systems

            #region "Russian SAM Battery Systems"
            uc = new UnitClass()
            {
                Id = "s400triumfbattery",
                UnitClassShortName = "S-400 Triumf",
                UnitClassLongName = "S-400 Triumf SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "ru",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "s400triumfradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="s400trimuf",
                                YPosition = -0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=48,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "torsambattery",
                UnitClassShortName = "TOR SAM",
                UnitClassLongName = "TOR SAM Battery",
                UnitModelFileName = "sambattery",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true,
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * .25),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 10,
                DraftM = 0,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = true,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 20,
                LowestOperatingHeightM = 0,
                WidthM = 10,
                HeightM = 10, //guess
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 10000,
                TotalMassLoadedKg = 10000,
                TurnRangeDegrSec = 0,
                CountryId = "ru",
                AcquisitionCostCredits = 100,
                EstimatedUnitPriceMillionUSD = 2,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                SensorClassIdList = new List<string> { "visual", "torradar" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.AttackAir,
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                    new UnitClassWeaponLoads()
                    {
                        Name="default",
                        WeaponLoads = new List<UnitClassWeaponLoad> 
                        {
                            new UnitClassWeaponLoad
                            {
                                WeaponClassId="3k95kinzhal",
                                YPosition = -0,
                                WeaponPitchDeg=90,
                                IsPrimaryWeapon = true,
                                MaxAmmunition=48,
                                HeightPosition=10
                            },
                        }
                    }
                }
            };
            list.Add(uc);

            #endregion //Russian SAM Battery Systems

            #endregion //SAM Battery Systems

            #region "Radar Stations"

            uc = new UnitClass()
            {
                Id = "sentinelradarstation",
                UnitClassShortName = "Sentinel Radar",
                UnitClassLongName = "Sentinel Radar Station",
                UnitModelFileName = "radarstation",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true, //try false?
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation) * .2),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 100,
                DraftM = 40,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 100,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 100,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "us",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 4000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                },
                SensorClassIdList = new List<string> { "visual", "mpq64sentinel" },
                UnitRoles = new List<GameConstants.Role>() 
                { 
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {

                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "torradarstation",
                UnitClassShortName = "TOR Radar",
                UnitClassLongName = "TOR Radar Station",
                UnitModelFileName = "radarstation",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true, //try false?
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation) * .2),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 100,
                DraftM = 40,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 100,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 100,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "ru",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 4000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                },
                SensorClassIdList = new List<string> { "visual", "torradar" },
                UnitRoles = new List<GameConstants.Role>()
                {
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {

                }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "girafferadarstation",
                UnitClassShortName = "Giraffe Radar",
                UnitClassLongName = "GIRAFFE Radar Station",
                UnitModelFileName = "radarstation",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = true,
                UnitType = GameConstants.UnitType.LandInstallation,
                IsAircraft = false,
                DetectionClassification = GameConstants.DetectionClassification.LandInstallation,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Large,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                CruiseSpeedKph = 0,
                IsAlwaysVisibleForEnemy = true, //try false?
                MaxHitpoints = (int)(GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation) * .2),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                CrewTotal = 100,
                DraftM = 40,
                HighestOperatingHeightM = 0,
                StabilityBonus = 3,
                IsEsmShielded = false,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = true,
                IsLandbased = true,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 100,
                LowestOperatingHeightM = 0,
                WidthM = 100,
                HeightM = 100,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 0,
                MaxDecelerationKphSec = 0,
                MaxFallMSec = 0,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 9200000,
                TotalMassLoadedKg = 9200000,
                TurnRangeDegrSec = 0,
                CountryId = "se",
                AcquisitionCostCredits = 10000,
                EstimatedUnitPriceMillionUSD = 4000,
                MaxCarriedUnitsTotal = 0,
                MaxSimultanousTakeoffs = 0,
                CarriedUnitClassses = new List<UnitClassStorage>()
                {
                },
                SensorClassIdList = new List<string> { "visual", "ericsongiraffe" },
                UnitRoles = new List<GameConstants.Role>()
                {
                    GameConstants.Role.IsLandInstallation,
                    GameConstants.Role.IsLandDefensiveStructure,
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {

                }
            };
            list.Add(uc);


            #endregion //Radar stations

            #endregion //Airports and other land installations

            #region "Sonobyous and mines"

            uc = new UnitClass()
            {
                Id = "dicass",
                UnitClassShortName = "DICASS AN/SSQ-62 Sonobuoy",
                UnitClassLongName = "DICASS AN/SSQ-62 Sonobuoy",
                UnitModelFileName = "dicass",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Sonobuoy,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Sonobuoy,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                TimeToLiveSec = 60 * 60,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 10,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 50,
                TotalMassLoadedKg = 50,
                TurnRangeDegrSec = 1,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 0.2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "anssq62" }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "rgbnm",
                UnitClassShortName = "RGB-NM Sonobuoy",
                UnitClassLongName = "RGB-NM Sonobuoy",
                UnitModelFileName = "dicass",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Sonobuoy,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Sonobuoy,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = 0,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                TimeToLiveSec = 60 * 60,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 10,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 50,
                TotalMassLoadedKg = 50,
                TurnRangeDegrSec = 1,
                CountryId = "ru",
                EstimatedUnitPriceMillionUSD = 0.2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { "rgbnm" }
            };
            list.Add(uc);

            //Naval Mines: MK56 (us) and MDM-5 (ru)
            uc = new UnitClass()
            {
                Id = "mk56mine",
                UnitClassShortName = "MK56 Naval Mine",
                UnitClassLongName = "MK56 Naval Mine",
                UnitModelFileName = "mk56mine",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Mine,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Mine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = -20,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                TimeToLiveSec = 0,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 10,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = 1,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 0.2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { }
            };
            list.Add(uc);

            uc = new UnitClass()
            {
                Id = "mdm5mine",
                UnitClassShortName = "MDM-5 Naval Mine",
                UnitClassLongName = "MDM-5 Naval Mine",
                UnitModelFileName = "mk56mine",
                UnitModelScaleFactor = 1.0f,
                CanBeTargeted = false,
                UnitType = GameConstants.UnitType.Mine,
                IsAircraft = false,
                CarriedRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                RequiredRunwayStyle = GameConstants.RunwayStyle.NoRunway,
                DetectionClassification = GameConstants.DetectionClassification.Mine,
                RadarCrossSectionSize = GameConstants.RadarCrossSectionSize.Normal,
                CruiseSpeedKph = 0,
                MaxHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile),
                MaxSpeedKph = 0,
                MilitaryMaxSpeedKph = 0,
                PropulsionSystem = GameConstants.PropulsionSystem.Unmovable,
                MaxCarriedUnitsTotal = 0,
                IsEsmShielded = false,
                CrewTotal = 0,
                DraftM = 1,
                HighestOperatingHeightM = -20,
                IsBallistic = false,
                IsDecoy = false,
                IsFixed = false,
                IsLandbased = false,
                IsMissileOrTorpedo = false,
                IsSurfaceShip = false,
                IsSubmarine = false,
                LengthM = 1,
                LowestOperatingHeightM = GameConstants.DEPTH_DEEP_MIN_M,
                TimeToLiveSec = 0,
                WidthM = 1,
                HeightM = 1,
                MaxAccelerationKphSec = 0,
                MaxClimbrateMSec = 10,
                MaxDecelerationKphSec = -20,
                MaxFallMSec = 10,
                MaxRangeCruiseM = 0,
                MinSpeedKph = 0,
                TotalMassEmptyKg = 1000,
                TotalMassLoadedKg = 1000,
                TurnRangeDegrSec = 1,
                CountryId = "us",
                EstimatedUnitPriceMillionUSD = 0.2,
                AcquisitionCostCredits = 1,
                UnitRoles = new List<GameConstants.Role>()
                {
                },
                WeaponLoads = new List<UnitClassWeaponLoads>
                {
                },
                SensorClassIdList = new List<string> { }
            };
            list.Add(uc);

            #endregion

            return list;
        }


    }
}
