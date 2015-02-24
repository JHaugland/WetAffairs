using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;
using TTG.NavalWar.NWData.Ai;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;

namespace TTG.NavalWar.NWData.GamePlay
{
	[Serializable]
	public class GameData
	{
		#region "Private Fields and Constructors"

		private List<UnitClass> _UnitClasses = new List<UnitClass>();
		private List<WeaponClass> _WeaponClasses = new List<WeaponClass>();
		private List<SensorClass> _SensorClasses = new List<SensorClass>();
		private List<Country> _Countries = new List<Country>();
		private List<GameScenario> _Scenarios = new List<GameScenario>();
		private List<Formation> _Formations = new List<Formation>();
		private List<Campaign> _Campaigns = new List<Campaign>();
        private List<DialogCharacter> _DialogCharacters = new List<DialogCharacter>();
        private List<User> _Users = new List<User>();

		private WeatherSystem[,] _WeatherArray;
        private DateTime _GameTimeLastWeatherUpdate;

        private bool _isClassDataInitialized;

		//private string _GameDataFolder;

		public GameData()
		{
		}

		#endregion

		#region "Public properties"

		public IList<UnitClass> UnitClasses
		{
			get
			{
				return _UnitClasses.AsReadOnly();
			}
		}

        public IList<WeaponClass> WeaponClasses
		{
			get
			{
                return _WeaponClasses.AsReadOnly();
			}
		}
        public IList<SensorClass> SensorClasses
		{
			get
			{
                return _SensorClasses.AsReadOnly();
			}
		}

        public IList<Country> Countries
		{
			get
			{
                return _Countries.AsReadOnly();
			}
		}

        public IList<GameScenario> Scenarios
		{
			get
			{
                return _Scenarios.AsReadOnly();
			}
		}

        public IList<Formation> Formations
		{
			get
			{
                return _Formations.AsReadOnly();
			}
		}

        public IList<Campaign> Campaigns
		{
			get
			{
                return _Campaigns.AsReadOnly();
			}
		}

        public IList<DialogCharacter> DialogCharacters
        {
            get
            {
                return _DialogCharacters.AsReadOnly();
            }
        }

        public IList<User> Users
        {
            get
            {
                return _Users.AsReadOnly();
            }
        }

		#endregion

		#region "Public methods"

		public void RecreateWeatherDataFromMain()
		{
			Game game = GameManager.Instance.Game;
            _GameTimeLastWeatherUpdate = DateTime.FromBinary(game.GameCurrentTime.ToBinary());
            GameManager.Instance.Log.LogDebug("GameData->RecreateWeatherDataFromMain called.");
			int lowestLong = (int)Math.Min(game.UpperLeftCorner.LongitudeDeg, game.LowerRightCorner.LongitudeDeg);
			int highestLong = (int)Math.Max(game.UpperLeftCorner.LongitudeDeg, game.LowerRightCorner.LongitudeDeg);
			int lowestLat = (int)Math.Min(game.UpperLeftCorner.LatitudeDeg, game.LowerRightCorner.LatitudeDeg);
			int highestLat = (int)Math.Max(game.UpperLeftCorner.LatitudeDeg, game.LowerRightCorner.LatitudeDeg);

			int CountEastWest = (int)(highestLong - lowestLong);
			int CountNorthSouth = (int)(highestLat - lowestLat);
			_WeatherArray = new WeatherSystem[CountEastWest,CountNorthSouth];
			//InitMainWeatherSystems(upperLeftCorner, lowerRightCorner, weatherType, season);
			for (int LongIdx = 0; LongIdx <= _WeatherArray.GetUpperBound(0); LongIdx++)
			{
				for (int LatIdx = 0; LatIdx <= _WeatherArray.GetUpperBound(1); LatIdx++)
				{
					Coordinate coord = new Coordinate(LatIdx+lowestLat, LongIdx+lowestLong);
					WeatherSystem system = WeatherSystem.CreateInterpolatedWeatherSystem(coord);
                    if (system != null)
                    {
                        system.GameTime = DateTime.FromBinary(game.GameCurrentTime.ToBinary());
                        system.RecalculateMoonAndSunInformation();
                        system.RecalculateSunshine();
                        _WeatherArray[LongIdx, LatIdx] = system;
                    }

				}
			}
		}

		public WeatherSystem GetWeather(Coordinate coordinate)
		{
			Game game = GameManager.Instance.Game;
			int lowestLong = (int)Math.Min(game.UpperLeftCorner.LongitudeDeg, game.LowerRightCorner.LongitudeDeg);
			int highestLong = (int)Math.Max(game.UpperLeftCorner.LongitudeDeg, game.LowerRightCorner.LongitudeDeg);
			int lowestLat = (int)Math.Min(game.UpperLeftCorner.LatitudeDeg, game.LowerRightCorner.LatitudeDeg);
			int highestLat = (int)Math.Max(game.UpperLeftCorner.LatitudeDeg, game.LowerRightCorner.LatitudeDeg);

			int Long = (int)Math.Floor(coordinate.LongitudeDeg).Clamp(lowestLong, highestLong);
			int Lat = (int)Math.Floor(coordinate.LatitudeDeg).Clamp(lowestLat, highestLat);
            if (game.GameCurrentTime.Subtract(_GameTimeLastWeatherUpdate) > TimeSpan.FromMinutes(30))
            {
                RecreateWeatherDataFromMain();
            }
			try
			{
				
				int LongIdx = Long - Math.Abs(lowestLong);
				if (lowestLong < 0)
				{
					LongIdx = Long + Math.Abs(lowestLong);
				}
				int LatIdx = Lat - Math.Abs(lowestLat);
				if (lowestLat < 0)
				{
					LatIdx = Lat + Math.Abs(lowestLat);
				}
				WeatherSystem system =_WeatherArray[LongIdx, LatIdx];
				system.Coordinate = coordinate;
                system.GameTime = GameManager.Instance.Game.GameCurrentTime;
				return system;
			}
			catch (Exception)
			{
				//GameManager.Instance.Log.LogError("GetWeather fails. " + ex.ToString());
				return WeatherSystem.CreateInterpolatedWeatherSystem(coordinate);
			}
		}

		public void InitMainWeatherSystems(GameConstants.WeatherSystemTypes weatherType, 
			GameConstants.WeatherSystemSeasonTypes season)
		{
			Game game = GameManager.Instance.Game;
			int countRemove = BlackboardController.Objects.RemoveAll(o => o is WeatherSystem);
			double DiagonalDistance = game.UpperLeftCorner.DistanceToM(game.LowerRightCorner);
			double BearingDeg = MapHelper.CalculateBearingDegrees(game.UpperLeftCorner, game.LowerRightCorner);
			Coordinate coord1 = MapHelper.CalculateNewPosition2(game.UpperLeftCorner, BearingDeg, DiagonalDistance / 10);
			Coordinate coord2 = MapHelper.CalculateNewPosition2(game.UpperLeftCorner, BearingDeg, (DiagonalDistance / 10) * 9);
			Coordinate coord3 = MapHelper.CalculateNewPosition2(game.LowerRightCorner, BearingDeg, (DiagonalDistance / 10) * 9);
			Coordinate coord4 = MapHelper.CalculateNewPosition2(game.LowerRightCorner, BearingDeg, (DiagonalDistance / 10));
			WeatherSystem MainWeatherSystem1 = WeatherSystem.CreateRandomWeatherSystem(weatherType, season, coord1.LatitudeDeg);
			MainWeatherSystem1.Coordinate = coord1;
			WeatherSystem MainWeatherSystem2 = WeatherSystem.CreateRandomWeatherSystem(weatherType, season, coord2.LatitudeDeg);
			MainWeatherSystem2.Coordinate = coord2;
			WeatherSystem MainWeatherSystem3 = WeatherSystem.CreateRandomWeatherSystem(weatherType, season, coord3.LatitudeDeg);
			MainWeatherSystem3.Coordinate = coord3;
			WeatherSystem MainWeatherSystem4 = WeatherSystem.CreateRandomWeatherSystem(weatherType, season, coord4.LatitudeDeg);
			MainWeatherSystem4.Coordinate = coord4;

			BlackboardController.Objects.Add(MainWeatherSystem1);
			BlackboardController.Objects.Add(MainWeatherSystem2);
			BlackboardController.Objects.Add(MainWeatherSystem3);
			BlackboardController.Objects.Add(MainWeatherSystem4);
		}

        public void InitAllClassData()
        {
            if (_isClassDataInitialized)
            {
                return;
            }

            _Countries = SerializationHelper.LoadCountriesFromXML();
            _SensorClasses = SerializationHelper.LoadSensorClassFromXML();
            _WeaponClasses = SerializationHelper.LoadWeaponClassFromXML();
            _UnitClasses = SerializationHelper.LoadUnitClassesFromXML();
            _Scenarios = SerializationHelper.LoadScenariosFromXML();
            _Formations = SerializationHelper.LoadFormationsFromXML();
            _Campaigns = SerializationHelper.LoadCampaignsFromXML();
            _DialogCharacters = SerializationHelper.LoadDialogCharactersFromXML();
            _Users = SerializationHelper.LoadUsersFromXML();

            _isClassDataInitialized = true;
        }

        public void RefreshAllClassData()
        {
            _isClassDataInitialized = false;
            InitAllClassData();
        }

		public void InitAllData()
		{
            InitAllClassData();
            TerrainReader.LoadMemoryMap();
		}

        public BaseUnit CreateUnit(Player playerOwner, Group group, string unitClassCode, string unitName,
            Position position, bool isReady)
        {
            return CreateUnit(playerOwner, group, unitClassCode, unitName, position, isReady, true);
        }

        public BaseUnit CreateUnit(Player playerOwner, Group group, string unitClassCode, string unitName, 
			Position position, bool isReady, bool includeCarriedUnits)
		{
			BaseUnit unit;// = new BaseUnit();
			UnitClass unitClass = GetUnitClassById(unitClassCode);
            if (unitClass == null)
            {
                GameManager.Instance.Log.LogError("CreateUnit: No existing unitclass found for Id=" + unitClassCode);
                return null;
            }
			if (unitClass.UnitType == GameConstants.UnitType.Mine)
			{
				unit = new Mine();
			}
			else if (unitClass.IsMissileOrTorpedo)
			{
				unit = new MissileUnit();
			}
			else if (unitClass.IsAircraft)
			{
				unit = new AircraftUnit();
			}
			else if (unitClass.IsLandbased)
			{
				unit = new LandInstallationUnit();
			}
			else if (unitClass.UnitType == GameConstants.UnitType.Sonobuoy)
			{
				unit = new Sonobuoy();
			}

			else
			{
				unit = new BaseUnit();
			}
			if (unitClass.TimeToLiveSec > 0)
			{
				unit.TimeToLiveSec = unitClass.TimeToLiveSec;
			}
			unit.DirtySetting = GameConstants.DirtyStatus.NewlyCreated;
			unit.UnitClass = unitClass;
			unit.OwnerPlayer = playerOwner;
			unit.WeaponOrders = playerOwner.DefaultWeaponOrders;
			if (position != null)
			{
				unit.Position = position.Clone();
                if (!unit.Position.HasHeightOverSeaLevel)
                {
                    unit.Position.HeightOverSeaLevelM = 0;
                    unit.UserDefinedElevation = GameConstants.HeightDepthPoints.Surface;
                }
                else
                {
                    unit.UserDefinedElevation = unit.Position.HeightOverSeaLevelM.Value.ToHeightDepthMark();
                }
				if (!unit.Position.HasBearing)
				{
					unit.Position.BearingDeg = 90;
				}
                switch (unit.DomainType)
                {
                    case GameConstants.DomainType.Surface:
                    case GameConstants.DomainType.Subsea:
                        if (unit.TerrainHeightAtPosM > 0)
	                    {
                            GameManager.Instance.Log.LogWarning(
                                string.Format("GameData->CreateUnit: Unit {0} spawned on illegal terrain. Height {1}m", unit, unit.TerrainHeightAtPosM));
	                    };
                        break;
                    case GameConstants.DomainType.Air:
                        if (unit.TerrainHeightAtPosM > unit.Position.HeightOverSeaLevelM)
                        {
                            unit.Position.HeightOverSeaLevelM = unit.TerrainHeightAtPosM + 10;
                        }
                    
                        break;
                    case GameConstants.DomainType.Land:
                        
                        var maxHeightM = TerrainReader.GetMaxTerrainHeightAreaM(unit.Position.Coordinate, MapHelper.GetMaxValue(unit.UnitClass.LengthM, unit.UnitClass.WidthM) * 1.5);
                        if (maxHeightM > unit.Position.HeightOverSeaLevelM)
                        {
                            unit.Position.HeightOverSeaLevelM = maxHeightM;
                            GameManager.Instance.Log.LogWarning(
                                string.Format("GameData->CreateUnit: Unit {0} spawned on low ground and is moved up. Height {1}m", unit, maxHeightM));
                        }
                        break;
                }
			}
			else
			{
				unit.Position = null;
			}
			if (group == null)
			{
				unit.Group = null;
			}
			else
			{
				//Unit.GroupId = group.Id;
				group.AddUnit(unit);
			}
			
			unit.HitPoints = unitClass.MaxHitpoints;
			unit.MaxRangeCruiseM = unitClass.MaxRangeCruiseM; //TODO: Modify for extra weapons, etc
			unit.ReCalculateMaxRange();
			if (!isReady)
			{
				unit.ReadyInSec = GameConstants.DEFAULT_READY_TIME_SEC;
			}
			unit.Name = unitName;
			if (string.IsNullOrEmpty(unitName))
			{ 
				UnitClassVesselName VesselName = playerOwner.GetNextAvailableVesselName(unitClassCode);
				if (VesselName != null)
				{
					unit.Name = VesselName.UnitName;
					unit.UnitDesignation = VesselName.UnitDesignation;
				}
				else
				{
					unit.Name = unitClass.UnitClassShortName + " " + unit.Id;
				}
			}

			foreach (string sensorCode in unitClass.SensorClassIdList)
			{
				SensorClass SensorClass = GetSensorClassById(sensorCode);
				Debug.Assert(SensorClass != null, "Invalid sensor class " + sensorCode);
				if (SensorClass != null)
				{
					AddSensorToUnit(unit, playerOwner, SensorClass);
				}
			}
			unit.SetWeaponLoad(string.Empty); //Default
			unit.ReadyInSec = 0;
			if (unitClass.CanCarryUnits)
			{
				AircraftHangar hangar = new AircraftHangar();
				hangar.MaxAircraft = unitClass.MaxCarriedUnitsTotal;
				hangar.RunwayStyle = unitClass.CarriedRunwayStyle;
				unit.RegisterComponent(hangar, hangar.Id);
                if (includeCarriedUnits)
                {
                    foreach (UnitClassStorage storage in unitClass.CarriedUnitClassses)
                    {
                        for (int i = 1; i <= storage.NumberOfUnits; i++)
                        {
                            BaseUnit carriedUnit = unit.AddCarriedUnit(storage.UnitClassId);
                            AircraftUnit craft = (AircraftUnit)carriedUnit;
                            if (craft != null)
                            {
                                if (!string.IsNullOrEmpty(storage.WeaponLoadName))
                                {
                                    craft.SetWeaponLoad(storage.WeaponLoadName);
                                }
                                else
                                {
                                    craft.SetWeaponLoad(storage.WeaponLoadType, storage.WeaponLoadModifier);
                                }
                                craft.ReadyInSec = 0;
                            }
                            Debug.Assert(craft != null, "CreateUnit failed to add craft " + storage.UnitClassId + " to " + unitClass.UnitClassShortName);
                        }
                    }
                }
			}
			unit.ReCalculateMaxRange();
			playerOwner.Units.Add(unit);
			return unit;

		}

		public void AddSensorToUnit(BaseUnit unit, Player playerOwner, SensorClass sensorClass)
		{
			Debug.Assert(sensorClass != null, "SensorClass should not be null.");
			Debug.Assert(playerOwner != null, "PlayerOwner should not be null.");
			Debug.Assert(unit != null, "Unit should not be null.");
			BaseSensor sensor; // = new BaseSensor();
			switch (sensorClass.SensorType)
			{
				case GameConstants.SensorType.Radar:
					sensor = new Radar(); //RADAR
					break;
				case GameConstants.SensorType.Sonar:
					sensor = new Sonar(); //SONAR
					break;
				case GameConstants.SensorType.Visual:
				case GameConstants.SensorType.MAD:
				case GameConstants.SensorType.Infrared:
					sensor = new BaseSensor();
					break;
				default:
					sensor = new BaseSensor();
					break;
			}
			sensor.OwnerPlayer = playerOwner;
			sensor.SensorClass = sensorClass;
			sensor.OwnerUnit = unit;
			sensor.Name = sensorClass.SensorClassName;
			sensor.SensorBearingDeg = 0; 
			sensor.IsOperational = true; 
			if (sensorClass.MaxSpeedDeployedKph > 0 || sensorClass.MaxHeightDeployedM > 0)
			{
				sensor.IsOperational = false; //if sensor has height/speed restrictions, set non-operational
			}
			sensor.IsActive = false;
			if (!sensorClass.IsPassiveActiveSensor)
			{
				sensor.IsActive = true;
			}
			unit.RegisterComponent(sensor, sensor.Name);
		}

        public void SaveUpdatedUserData(User user)
        {
            if (user == null)
            {
                return;
            }

            if (_Users == null)
            {
                _Users = new List<User>();
            }

            //_Users.RemoveAll(u => u.UserId == user.UserId); //remove old
            _Users.Clear(); // remove old
            _Users.Add(user);
            try
            {
                Serializer<User> userXml = new Serializer<User>();
                userXml.SaveListToXML(_Users, SerializationHelper.USERS_FILENAME, SerializationHelper.GetUserDataFolder());
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError(
                    string.Format("SaveUpdatedUserData failed for user {0}: {1}", user.UserId, ex.Message));
            }
        }

		public bool LoadGameScenario(string gameScenarioId, Game game)
		{
			GameScenario scenario = GetGameScenarioById(gameScenarioId);
			if (scenario == null)
			{
				GameManager.Instance.Log.LogError(
					"LoadGameScenario could not find scenario id=" + gameScenarioId);
				return false;
			}
			if (game.Players.Count < 1)
			{
				GameManager.Instance.Log.LogError(
					"LoadGameScenario needs at least one current player to start game.");
				return false;
			}
			game.GameName = scenario.GameName;
			if (scenario.StartDateTime != null)
			{
				game.GameStartTime = scenario.StartDateTime.GetDateTime();
				game.GameCurrentTime = scenario.StartDateTime.GetDateTime();
			}
            game.IsNetworkEnabled = true; // scenario.IsNetworkEnabled; //hmm...? 
			game.RealTimeCompressionFactor = scenario.InitialRealTimeCompressionFactor;
			game.EconomicModel = scenario.EconomicModel;
			game.CreditsPerMinute = scenario.CreditsPerMinute;
			game.MaxNonComputerPlayers = scenario.MaxNonComputerPlayers;
			//game.SkillLevel = scenario.SkillLevel;
			if (scenario.UpperLeftCorner == null || scenario.LowerRightCorner == null)
			{
				game.UpperLeftCorner = new Coordinate(70, -10); //set defaults
				game.LowerRightCorner = new Coordinate(40, 10);
			}
			else
			{
				game.UpperLeftCorner = new Coordinate(scenario.UpperLeftCorner);
				game.LowerRightCorner = new Coordinate(scenario.LowerRightCorner);
			}
			game.CurrentGameScenario = scenario;
			GameManager.Instance.GameData.InitMainWeatherSystems(scenario.WeatherType, scenario.Season);
			GameManager.Instance.GameData.RecreateWeatherDataFromMain();

			foreach (var alliance in scenario.Alliences)
			{
				foreach (var splayer in alliance.ScenarioPlayers)
				{
					Player player;
					player = GetAvailableGamePlayer(game, splayer);
					player.ScenarioPlayerId = splayer.Id;
					splayer.AssignedPlayerId = player.Id;
					player.IsCompetitivePlayer = alliance.IsCompetitivePlayer;
                    player.IsAutomaticallyEngagingHighValueTargets = splayer.IsAutomaticallyEngagingHighValueTargets;
                    player.IsAutomaticallyEngagingOpportunityTargets = splayer.IsAutomaticallyEngagingOpportunityTargets;
                    player.IsAutomaticallySettingHighValueDefence = splayer.IsAutomaticallySettingHighValueDefence;
                    if (player.IsComputerPlayer)
                        player.IsAllUnknownContactsHostile = splayer.IsAllUnknownContactsHostile;
                    player.CurrentRulesOfEngagement = splayer.InitialRulesOfEngagement;

					player.Tag = splayer.Tag;
					player.Credits = scenario.InitialCreditsPerPlayer;
					if (splayer.InitialCredits > 0)
					{
						player.Credits = splayer.InitialCredits;
					}
					player.AllianceId = alliance.Id;
					player.Country = GetCountryById(splayer.CountryId);
					foreach (var ucid in splayer.AcquirableUnitClasses)
					{
						player.AcquirableUnitClasses.Add(ucid);
					}

					if (splayer.EventTriggers != null && splayer.EventTriggers.Count > 0)
					{
						foreach (var trigger in splayer.EventTriggers)
						{
							if (trigger.IsForComputerPlayerOnly && !player.IsComputerPlayer)
							{
								continue;
							}
							player.EventTriggers.Add(trigger);
						}
					}
					if (splayer.InitialGameUiControls != null && splayer.InitialGameUiControls.Count > 0)
					{
						foreach (var contr in splayer.InitialGameUiControls)
						{
							var contrClone = contr.Clone();
							player.InitialGameUiControls.Add(contr);
						}
					}
                    //player.DefeatConditionSets = new List<DefeatConditionSet>();
                    //if (splayer.DefeatConditionSets != null && splayer.DefeatConditionSets.Count > 0)
                    //{
                    //    foreach (var defset in splayer.DefeatConditionSets)
                    //    {
                    //        player.DefeatConditionSets.Add(new DefeatConditionSet(player, defset));
                    //    }
                    //}
					if (splayer.HighLevelOrders != null && splayer.HighLevelOrders.Count > 0)
					{

						foreach (var hlo in splayer.HighLevelOrders)
						{
							if (!hlo.IsForComputerPlayerOnly || player.IsComputerPlayer)
							{
                                if (game.IsUnitIncludedForPlayer(player, hlo.SkillLevel))
                                {
								    var hloClone = hlo.Clone();
								    hloClone.Id = GameManager.GetUniqueCode();
								    player.HighLevelOrders.Enqueue(hloClone);
                                }
							}
						}
					}
					if (splayer.AIHints != null)
					{
						foreach (var hint in splayer.AIHints)
						{
							var aiHint = new AIHint(hint);
							aiHint.OwnerId = player.Id;
							BlackboardController.Objects.Add(aiHint);
						}
					}
					foreach (var sgroup in splayer.Groups)
					{
						Group group = new Group();
						
						group.Tag = sgroup.Tag;
						group.Name = sgroup.Name;
						Formation formation = GetFormationById(sgroup.FormationId);
						if (formation != null)
						{
							group.Formation = formation.Clone();
						}
                        foreach (var sunit in sgroup.Units)
                        {
                            if (game.IsUnitIncludedForPlayer(player, sunit.SkillLevel))
                            {
                                BaseUnit unit = CreateUnit(player, group, sunit.UnitClassId,
                                    sunit.UnitName, new Position(sgroup.PositionInfo), true, sunit.IncludeDefaultCarriedUnits);
                                if (unit == null)
                                {
                                    GameManager.Instance.Log.LogError( 
                                        string.Format("LoadGameScenario failed: Unit with UnitClassId='{0}' could not be created for game {1}.", 
                                        sunit.UnitClassId, gameScenarioId ));
                                    continue;
                                }
                                if (sgroup.AlternatePositions != null && sgroup.AlternatePositions.Count > 0)
                                {
                                    var posChoice = GameManager.Instance.GetRandomNumber(sgroup.AlternatePositions.Count + 1);
                                    if (posChoice > 0)
                                    {
                                        try
                                        {
                                            unit.Position = new Position(sgroup.AlternatePositions[posChoice-1]);
                                        }
                                        catch (Exception ex)
                                        {
                                            GameManager.Instance.Log.LogError(
                                                string.Format("LoadGameScenario Unit with UnitClassId='{0}' could not set alternate position. {1}",
                                                sunit.UnitClassId, ex.Message));
                                        }
                                    }
                                }
                                if (sgroup.PositionVariationRadiusM > 0)
                                {
                                    double randomDistanceM = GameManager.Instance.GetRandomNumber((int)sgroup.PositionVariationRadiusM);
                                    double randomBearingDeg = GameManager.Instance.GetRandomNumber(360);
                                    Position pos = unit.Position.Offset(randomBearingDeg, randomDistanceM);
                                    unit.Position = pos.Clone();
                                }
                                unit.Tag = sunit.Tag;
                                unit.IsCivilianUnit = sunit.IsCivilianUnit;
                                if (!string.IsNullOrEmpty(sunit.WeaponLoad))
                                {
                                    unit.SetWeaponLoad(sunit.WeaponLoad);
                                }
                                else if (sunit.WeaponLoadType != GameConstants.WeaponLoadType.Undefined)
                                {
                                    unit.SetWeaponLoad(sunit.WeaponLoadType, sunit.WeaponLoadModifier);
                                }
                                unit.ReadyInSec = 0;
                                if (sunit.InitialOrders != null && sunit.InitialOrders.Count > 0)
                                {
                                    foreach (var order in sunit.InitialOrders)
                                    {
                                        order.Id = unit.Id;
                                        var baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(order);
                                        baseOrder.OwnerPlayer = player;
                                        unit.Orders.Enqueue(baseOrder);
                                    }
                                }
                                if (!string.IsNullOrEmpty(sgroup.FormationId) && group.Formation != null)
                                {
                                    if (!string.IsNullOrEmpty(sunit.FormationPositionId))
                                    {
                                        FormationPosition formPos = group.Formation.GetFormationPositionById(
                                            sunit.FormationPositionId);
                                        unit.FormationPositionId = sunit.FormationPositionId;
                                        formPos.AssignedUnitId = unit.Id;
                                        if (formPos != null)
                                        {
                                            unit.Position = unit.Position.Offset(formPos);

                                        }
                                    }
                                    else if (sunit.PositionOffset != null)
                                    {
                                        unit.Position = unit.Position.Offset(sunit.PositionOffset);
                                    }
                                }
                                if (!string.IsNullOrEmpty(sgroup.ReturnToUnitTag))
                                {
                                    BaseUnit returnTo = player.GetUnitById("tag:" + sgroup.ReturnToUnitTag);
                                    if (returnTo != null)
                                    {
                                        unit.LaunchedFromUnit = returnTo;
                                    }
                                    else
                                    {
                                        GameManager.Instance.Log.LogWarning(
                                            string.Format("Unit {0} return unit not found. Unknow tag '{1}'.",
                                            unit.ToShortString(), sgroup.ReturnToUnitTag));
                                    }
                                }
                                unit.SetActualSpeed(unit.GetSpeedInKphFromSpeedType(sgroup.SpeedType));
                                unit.UserDefinedSpeed = sgroup.SpeedType;

                                if (sgroup.PatrolPatternLengthM > 0 && unit.IsGroupMainUnit())
                                {
                                    //Create patrol pattern
                                    List<Coordinate> coordList = MapHelper.CreateCoordinateSquare(unit.Position.Coordinate, sgroup.PatrolPatternLengthM);
                                    Waypoint firstWp = new Waypoint(coordList[0]);
                                    unit.Position.BearingDeg = MapHelper.CalculateBearingDegrees(unit.Position.Coordinate, firstWp.Position.Coordinate);
                                    foreach (var coord in coordList)
                                    {
                                        unit.MovementOrder.AddWaypoint(new Waypoint(coord));
                                    }
                                    unit.MovementOrder.IsRecurring = true;
                                }
                                else if (unit.IsGroupMainUnit() && sgroup.InitialWaypointList != null && sgroup.InitialWaypointList.Count > 0)
                                {
                                    Waypoint firstWp = new Waypoint(sgroup.InitialWaypointList[0]);
                                    unit.Position.BearingDeg = MapHelper.CalculateBearingDegrees(unit.Position.Coordinate, firstWp.Position.Coordinate);
                                    foreach (var wp in sgroup.InitialWaypointList)
                                    {
                                        unit.MovementOrder.AddWaypoint(wp);
                                    }
                                }

                                else if (unit.IsInGroupWithOthers() && !unit.IsGroupMainUnit())
                                {
                                    MovementFormationOrder formOrder = new MovementFormationOrder(group.MainUnit,
                                            sunit.FormationPositionId,
                                            sgroup.FormationId,
                                            sunit.PositionOffset);
                                    unit.MovementOrder = formOrder;
                                    //unit.Orders.Enqueue(formOrder);
                                    if (formOrder != null && formOrder.PositionOffset != null)
                                    {
                                        unit.Position = MapHelper.CalculatePositionFromOffset2(
                                            group.MainUnit.Position, formOrder.PositionOffset);
                                        unit.Position.BearingDeg = group.MainUnit.Position.BearingDeg;
                                    }
                                }
                                unit.ReCalculateEta();
                                foreach (var carriedUnit in sunit.CarriedUnits)
                                {
                                    if (game.IsUnitIncludedForPlayer(player, carriedUnit.SkillLevel))
                                    {
                                        for (int i = 1; i <= carriedUnit.UnitCount; i++)
                                        {
                                            AircraftUnit carried = unit.AddCarriedUnit(carriedUnit.UnitClassId);
                                            if (!string.IsNullOrEmpty(carriedUnit.WeaponLoad))
                                            {
                                                carried.SetWeaponLoad(carriedUnit.WeaponLoad);
                                            }
                                            else
                                            {
                                                carried.SetWeaponLoad(carriedUnit.WeaponLoadType, carriedUnit.WeaponLoadModifier);
                                            }
                                            carried.ReadyInSec = carriedUnit.ReadyInSec;
                                            GameManager.Instance.Log.LogDebug(
                                                string.Format("LoadGameScenario: Unit {0} has added carried unit {1}, Weapon-load {2}", unit, carried, carried.CurrentWeaponLoadName));
                                        }
                                    }
                                }
                                foreach (var wpnStore in sunit.CarriedWeaponStore)
                                {
                                    unit.CarriedWeaponStores.Add(wpnStore);
                                }
                            }
                        } //foreach sunit in group.units
						if (!group.RemoveIfSingleUnit())
						{
							player.AddGroup(group);
							if (string.IsNullOrEmpty(group.Name))
							{
								group.Name = group.MainUnit.Name + " group";
							}
						}
					}//foreach group in groups
				}//foreach player in alliances.players
			}//foreach alliance in alliances
			foreach (var pl in game.Players)
			{
				foreach (var otherpl in game.Players)
				{
					if (pl.IsCompetitivePlayer && otherpl.IsCompetitivePlayer && pl.Id != otherpl.Id)
					{
						if (pl.AllianceId == otherpl.AllianceId)
						{
							pl.Allies.Add(otherpl);
						}
						else
						{
							pl.Enemies.Add(otherpl);
						}
					}
				}
			}
			
			return true;
		}

		private Player GetAvailableGamePlayer(Game game, GameScenarioPlayer scenarioPlayer)
		{
			try
			{
				
				Player player = game.Players.Single<Player>(p => p.ScenarioPlayerId == scenarioPlayer.Id);
				if (player != null)
				{
					return player;
				}
			}
			catch (Exception)
			{
				//Ignore
			}
			
			foreach (var player in game.Players)
			{
				if (string.IsNullOrEmpty(player.ScenarioPlayerId)  
					&& player.IsCompetitivePlayer == scenarioPlayer.IsCompetitivePlayer)
				{
					return player;
				}
			}
			//Not found; create new
			Player newPlayer = new Player();
			newPlayer.IsCompetitivePlayer = scenarioPlayer.IsCompetitivePlayer;
            if (!scenarioPlayer.IsPlayableAsHuman)
            {
                newPlayer.IsComputerPlayer = true;    
            }
            if (newPlayer.TcpPlayerIndex == 0)
            {
                newPlayer.IsComputerPlayer = true;
            }
			game.AddPlayer(newPlayer);
			GameManager.Instance.Log.LogDebug(string.Format("GetAvailableGamePlayer adding player {0}   IsComputerPlayer: {1}", 
				newPlayer.ToLongString(), newPlayer.IsComputerPlayer));
			return newPlayer;

		}

        public void AddOrUpdateScenario(GameScenario scenario)
        {
            GameScenario oldScenario = GetGameScenarioById(scenario.Id);
            if (oldScenario != null)
            {
                _Scenarios.Remove(oldScenario);
            }
            _Scenarios.Add(scenario);
        }

		public GameScenario GetGameScenarioById(string id)
		{
			try
			{
				return _Scenarios.Find(s => s.Id == id);
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError(
					"GetGameScenarioById failed for Id=" + id + ". " + ex.ToString());
				return null;
			}
		}

		public UnitClass GetOrGenerateUnitClassFromWeapon(WeaponClass weaponClass)
		{
			if (weaponClass == null || !weaponClass.SpawnsUnitOnFire)
			{
				return null;
			}
            //does it already exist? then return it
            UnitClass baseUnitClass = GetUnitClassById(weaponClass.Id);
            if (baseUnitClass != null)
            {
                return baseUnitClass;
            }
			
			//if it does not already exist, create it from SpawnUnitClassId base
			baseUnitClass = GameManager.Instance.GetUnitClassById(weaponClass.SpawnUnitClassId);
			Debug.Assert(baseUnitClass != null,
				string.Format("Unable to get SpawnUnitClassId '{0}' for weaponClass {1}",
				weaponClass.SpawnUnitClassId, weaponClass.Id));
			UnitClass newUnitClass = baseUnitClass.Clone();
			newUnitClass.Id = weaponClass.Id;
			newUnitClass.MaxRangeCruiseM = weaponClass.MaxWeaponRangeM;
			newUnitClass.MaxSpeedKph = weaponClass.MaxSpeedKph;
            newUnitClass.CruiseSpeedKph = GameConstants.DEFAULT_NON_TERMINAL_SPEED_KPH; // weaponClass.MaxSpeedKph;
            newUnitClass.MilitaryMaxSpeedKph = GameConstants.DEFAULT_NON_TERMINAL_SPEED_KPH; // weaponClass.MaxSpeedKph;
            newUnitClass.UnitClassShortName = weaponClass.WeaponClassShortName;
			newUnitClass.UnitClassLongName = weaponClass.WeaponClassName;
            newUnitClass.CanBeTargeted = weaponClass.CanBeTargetted;
            if (!string.IsNullOrEmpty(weaponClass.UnitModelFileName))
            {
                newUnitClass.UnitModelFileName = weaponClass.UnitModelFileName;
            }
			if (weaponClass.HighestOperatingHeightM != 0)
			{
				newUnitClass.HighestOperatingHeightM = weaponClass.HighestOperatingHeightM;
			}
			if (weaponClass.LowestOperatingHeightM != 0)
			{
				newUnitClass.LowestOperatingHeightM = weaponClass.LowestOperatingHeightM;
			}
			_UnitClasses.Add(newUnitClass);
			foreach (var player in GameManager.Instance.Game.Players)
			{
				if (player.TcpPlayerIndex > 0)
                {
					player.Send(newUnitClass);
				}
			}
			return newUnitClass;
		}

		public Formation GetFormationById(string id)
		{
			try
			{
				return _Formations.Find(c => c.Id == id);
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError("GetFormationById failed for Id=" + id + ". " + ex.ToString());
				return null;
			}
		}
		/// <summary>
		/// Returns the UnitClassWeaponLoad associated with a UnitClassId and a WeaponLoadName. If name is
		/// left blank, the default (first) weaponload is returned. Returns null if id or name is invalid.
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <param name="weaponLoadName"></param>
		/// <returns></returns>
		public UnitClassWeaponLoads GetWeaponLoadByName(string unitClassId, string weaponLoadName)
		{
			try 
			{	        
				UnitClass unitClass = _UnitClasses.First<UnitClass>(u=>u.Id == unitClassId);
				if (unitClass.WeaponLoads.Count == 0)
				{
					return null;
				}
				UnitClassWeaponLoads load = unitClass.WeaponLoads[0]; //first entry loaded by default
				if (!string.IsNullOrEmpty(weaponLoadName))
				{
					foreach (UnitClassWeaponLoads loadlist in unitClass.WeaponLoads)
					{
						if (loadlist.Name.Trim().ToLower() == weaponLoadName.Trim().ToLower())
						{
							load = loadlist;
							break;
						}
					}
				}
				
				return load;
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError(
					string.Format("GetWeaponLoadByName failed for UnitClassId={0}, WeaponLoadName={1}. {2}",
					unitClassId, weaponLoadName, ex.ToString()));
				return null;
			}
		}

        public UnitClassWeaponLoads GetWeaponLoadByType(string unitClassId, GameConstants.WeaponLoadType weaponLoadType)
        {
            try
            {
                UnitClass unitClass = _UnitClasses.First<UnitClass>(u=>u.Id == unitClassId);
				if (unitClass.WeaponLoads.Count == 0)
				{
					return null;
				}
                var wld = from l in unitClass.WeaponLoads
                          where l.WeaponLoadType == weaponLoadType
                          select l;
                return wld.FirstOrDefault<UnitClassWeaponLoads>();
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError(
                    string.Format("GetWeaponLoadByType failed for UnitClassId={0}, Type={1}. {2}",
                    unitClassId, weaponLoadType, ex.ToString()));
                return null;
            }
        }

        /// <summary>
        /// Selects a weapon load for the provided UnitClass based on type and modifyer. If no weapon load can be found satisfying both type and modifier, it will return
        /// the first weapon load of the right type.
        /// If naval strike or land attack loads are requested, and none are available, this method will return a strike weaponload if available.
        /// If Undefined is given as the weapon load type, the first weapon load is returned.
        /// </summary>
        /// <param name="unitClassId"></param>
        /// <param name="weaponLoadType"></param>
        /// <param name="weaponLoadModifier"></param>
        /// <returns></returns>
        public UnitClassWeaponLoads GetWeaponLoadByType(string unitClassId, GameConstants.WeaponLoadType weaponLoadType, GameConstants.WeaponLoadModifier weaponLoadModifier)
        {
            try
            {
                UnitClass unitClass = _UnitClasses.First<UnitClass>(u => u.Id == unitClassId);
                if (weaponLoadType == GameConstants.WeaponLoadType.Undefined)
                {
                    if (unitClass.WeaponLoads.Count == 0)
                    {
                        return null;
                    }
                    else
                    { 
                        return unitClass.WeaponLoads[0];
                    }
                }
                var wld = from l in unitClass.WeaponLoads
                          where l.WeaponLoadType == weaponLoadType && l.WeaponLoadModifyer == weaponLoadModifier
                          select l;
                if (!wld.Any<UnitClassWeaponLoads>())
                {
                    wld = from l in unitClass.WeaponLoads
                          where l.WeaponLoadType == weaponLoadType
                          select l;
                }
                if (!wld.Any<UnitClassWeaponLoads>() //if naval strike or land attack request but unavailable, check if Strike exists
                    && (weaponLoadType == GameConstants.WeaponLoadType.NavalStrike || weaponLoadType == GameConstants.WeaponLoadType.LandAttack))
                {
                    wld = from l in unitClass.WeaponLoads
                          where l.WeaponLoadType == GameConstants.WeaponLoadType.Strike
                          select l;
                }
                if (!wld.Any<UnitClassWeaponLoads>())
                {
                    GameManager.Instance.Log.LogWarning(
                        string.Format("GetWeaponLoadByType did not find a load for UnitClassId={0}, Type={1}, Modifier={2}. {3}",
                        unitClassId, weaponLoadType, weaponLoadModifier));

                }
                return wld.FirstOrDefault<UnitClassWeaponLoads>();
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError(
                    string.Format("GetWeaponLoadByType failed for UnitClassId={0}, Type={1}, Modifier={2}. {3}",
                    unitClassId, weaponLoadType, weaponLoadModifier, ex.Message));
                return null;
            }
        }
		public Country GetCountryById(string id)
		{
			try 
			{
				return _Countries.Find(c => c.Id == id);
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError("GetCountryById failed for Id=" + id + ". " + ex.ToString());
				return null;
			}
		}

        public User GetUserById( string userId )
        {
            try
            {
                // We only care about one user
                if (_Users.Count > 0)
                {
                    return _Users[0];
                }
                return null;
                //return _Users.Find( u => u.UserId == userId );
            }
            catch ( Exception ex )
            {
                GameManager.Instance.Log.LogError( "GetUserById failed for Id=" + userId + ". " + ex.Message );
                return null;
            }
        }

		public Campaign GetCampaignById(string id)
		{
			try
			{
				return _Campaigns.Find(c => c.Id == id);
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError("GetCampaignById failed for Id=" + id + ". " + ex.Message);
				return null;
			}
		}

        public UnitClass GetUnitClassById(string unitClassId)
        {
            try
            {
                return _UnitClasses.Find(c => c.Id == unitClassId);
            }
            catch ( Exception ex )
            {
                GameManager.Instance.Log.LogError("GetUnitClassById failed for Id=" + unitClassId + ". " + ex.Message);
                return null;
            }
        }

        public WeaponClass GetWeaponClassById(string weaponClassId)
        {
            try
            {
                return _WeaponClasses.Find(c => c.Id == weaponClassId);
            }
            catch ( Exception ex )
            {
                GameManager.Instance.Log.LogError("GetWeaponClassById failed for Id=" + weaponClassId + ". " + ex.Message);
                return null;
            }
        }

        public SensorClass GetSensorClassById(string sensorClassId)
        {
            try
            {
                return _SensorClasses.Find(c => c.Id == sensorClassId);
            }
            catch ( Exception ex )
            {
                GameManager.Instance.Log.LogError("GetSensorClassById failed for Id=" + sensorClassId + ". " + ex.Message);
                return null;
            }
        }

        public DialogCharacter GetDialogCharacterById(string characterId)
        {
            try
            {
                return _DialogCharacters.Find(c => c.Id == characterId);
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("GetDialogCharacterById failed for Id=" + characterId + ". " + ex.Message);
                return null;
            }
        }

		#endregion
	}
}
