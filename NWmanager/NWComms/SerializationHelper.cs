using System;
using System.Collections.Generic;
using System.IO;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;
using TTG.NavalWar.NWComms.Properties;

namespace TTG.NavalWar.NWComms

{
    public static class SerializationHelper
    {
        public const string UNIT_CLASS_EXTENSION = ".nwunit";
        public const string SENSOR_CLASS_EXTENSION = ".nwsensor";
        public const string WEAPON_CLASS_EXTENSION = ".nwweapon";
        public const string COUNTRY_EXTENSION = ".nwcountry";
        public const string SCENARIO_EXTENSION = ".nwscenario";
        public const string FORMATION_EXTENSION = ".nwformation";
        public const string CAMPAIGN_EXTENSION = ".nwcampaigns";
        public const string CHARACTER_EXTENSION = ".nwcharacter";
        public const string USERS_EXTENSION = ".nwusers";

        //public const string UNIT_CLASSES_FILENAME = "unitclasses" + UNIT_CLASS_EXTENSION;
        //public const string SENSOR_CLASSES_FILENAME = "sensorclasses" + SENSOR_CLASSES_EXTENSION;
        //public const string WEAPON_CLASSES_FILENAME = "weaponclasses" + WEAPON_CLASS_EXTENSION;
        //public const string COUNTRIES_FILENAME = "countries" + COUNTRY_EXTENSION;
        //public const string SCENARIOS_FILENAME = "scenarios" + SCENARIO_EXTENSION;
        //public const string FORMATIONS_FILENAME = "formations" + FORMATION_EXTENSION;
        //public const string CAMPAIGNS_FILENAME = "campaigns" + CAMPAIGN_EXTENSION;
        //public const string CHARACTERS_FILENAME = "characters" + CHARACTERS_EXTENSION;
        public const string USERS_FILENAME = "users" + USERS_EXTENSION;

        private static string _dataFolder = string.Empty;
        private static string _userDataFolder = string.Empty;
        private static string _scenariosFolder = string.Empty;

        private static readonly List<string> CustomDataFolders = new List<string>();

        public static List<WeaponClass> LoadWeaponClassFromXML()
        {
            return LoadFromXML<WeaponClass>(WEAPON_CLASS_EXTENSION);
        }

        public static List<SensorClass> LoadSensorClassFromXML()
        {
            return LoadFromXML<SensorClass>(SENSOR_CLASS_EXTENSION);
        }

        public static List<UnitClass> LoadUnitClassesFromXML()
        {
            return LoadFromXML<UnitClass>(UNIT_CLASS_EXTENSION);
        }

        public static List<Country> LoadCountriesFromXML()
        {
            return LoadFromXML<Country>(COUNTRY_EXTENSION);
        }

        public static List<GameScenario> LoadScenariosFromXML()
        {
            List<GameScenario> scenarios = LoadFromXML<GameScenario>(SCENARIO_EXTENSION);

            if (!string.IsNullOrEmpty(GetScenariosFolder()))
            {
                var serializer = new Serializer<GameScenario>();
                List<GameScenario> customScenarios =
                    serializer.LoadFromXML(GetAllFilesForExtension(GetScenariosFolder(), SCENARIO_EXTENSION));

                foreach (var customScenario in customScenarios)
                {
                    if (string.IsNullOrEmpty(customScenario.Id))
                    {
                        customScenario.Id = Guid.NewGuid().ToString();
                    }

                    // Resolve relative path
                    if (customScenario.PreviewImage.StartsWith("."))
                    {
                        customScenario.PreviewImage = GetScenariosFolder() + customScenario.PreviewImage.Substring(1);
                    }
                }

                // Remove any custom scenario with conflicting Id
                customScenarios.RemoveAll(s1 => scenarios.Find(s2 => s2.Id == s1.Id) != null);

                scenarios.AddRange(customScenarios);
            }

            return scenarios;
        }

        public static List<Formation> LoadFormationsFromXML()
        {
            return LoadFromXML<Formation>(FORMATION_EXTENSION);
        }

        public static List<Campaign> LoadCampaignsFromXML()
        {
            return LoadFromXML<Campaign>(CAMPAIGN_EXTENSION);
        }

        public static List<DialogCharacter> LoadDialogCharactersFromXML()
        {
            return LoadFromXML<DialogCharacter>(CHARACTER_EXTENSION);
        }

        public static List<User> LoadUsersFromXML()
        {
            var serializer = new Serializer<User>();
            List<User> list = serializer.LoadListFromXML(
                GetAllFilesForExtension(GetUserDataFolder(), USERS_EXTENSION));
            return list;
        }

        private static List<T> LoadFromXML<T>(string fileExtension) where T : class, IGameDataObject
        {
            var serializer = new Serializer<T>();

            // Load items sequencially while checking for duplicates.
            // When a duplicate is found, we remove the old one and replace with new
            // version. This is to allow mods to override.
            var list = new List<T>();

            FileInfo[] files = GetAllFilesForExtension(fileExtension);
            foreach (var file in files)
            {
                T item = serializer.LoadFromXML(file);
                if (item != null)
                {
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        item.Id = Guid.NewGuid().ToString();
                    }

                    // Remove old before adding.
                    list.RemoveAll(s => s.Id == item.Id);
                    list.Add(item);
                }
            }

            return list;
        }

        public static FileInfo[] GetAllFilesForExtension(string path, string extension)
        {
            var di = new DirectoryInfo(path);
            return di.GetFiles("*" + extension, SearchOption.AllDirectories);
            //return Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories);
        }

        public static FileInfo[] GetAllFilesForExtension(string extension)
        {
            var files = new List<FileInfo>(GetAllFilesForExtension(GetDataFolder(), extension));

            foreach (var customDataFolder in CustomDataFolders)
            {
                var customFiles = new List<FileInfo>(GetAllFilesForExtension(customDataFolder, extension));
                files.AddRange(customFiles);
            }

            return files.ToArray();
        }

        public static string GetDataFolder()
        {
            if (string.IsNullOrEmpty(_dataFolder))
            {
                _dataFolder = Settings.Default.DataFolder;
                if (string.IsNullOrEmpty(_dataFolder))
                {
                    _dataFolder = @"\Turbo Tape Games\Naval War Arctic Circle\Data\";
                }
                _dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _dataFolder;
            }
            return _dataFolder;
        }

        public static void SetDataFolder(string path)
        {
            _dataFolder = path;
        }

        public static string GetUserDataFolder()
        {
            if (string.IsNullOrEmpty(_userDataFolder))
            {
                _userDataFolder = GetDataFolder();
            }
            return _userDataFolder;
        }

        public static void SetUserDataFolder(string path)
        {
            _userDataFolder = path;
        }

        public static string GetScenariosFolder()
        {
            return _scenariosFolder;
        }

        public static void SetScenariosFolder(string path)
        {
            _scenariosFolder = path;
        }

        public static void AddCustomDataFolder(string path)
        {
            if (!CustomDataFolders.Contains(path))
            {
                CustomDataFolders.Add(path);
            }
        }

        public static void ClearCustomDataFolders()
        {
            CustomDataFolders.Clear();
            CustomDataFolders.TrimExcess();
        }

        public static string GetServerFolder()
        {
            string dataFolder = @"\Turbo Tape Games\Naval War Arctic Circle\Server client\";
            
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + dataFolder;
            return temp;
        }
    }
}
