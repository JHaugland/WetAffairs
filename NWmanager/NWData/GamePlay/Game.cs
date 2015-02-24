using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.Ai;
using System.Diagnostics;

namespace TTG.NavalWar.NWData.GamePlay
{
	public class Game
	{
		private Player _GameOwner = null;
		private readonly List<Player> _Players = new List<Player>();
		//private Timer _gameTimer = null;
		private double _GameWorldTimeSec = 0;
		private double _LastTickTimeMs;
        private double _gameTickDeltaSec = 0;
        private double _NetSendTimerMs = 0;
	    private readonly Stopwatch _gameTimer;

		private bool _IsNetworkEnabled;

		//private List<GameObject> _GameObjects = new List<GameObject>();
        private IGameServer _GameServer = null;
		
		public const double DEFAULT_REAL_TIME_COMPRESSION_FACTOR = 1;

        private const int NET_SEND_INTERVAL_MS = 1000;          // Time between each net update

        private const double GAME_TICK_MAX_TIME = 2000;          // Maximum time to run the game tick.

        private const double GAME_TICK_MIN_LENGTH_SEC = 1;
        private const double GAME_TICK_MAX_LENGHT_SEC = 10;     // Each tick is max 10s of game time

        public event ConnectionStatusChangedDelegate ConnectionStatusChanged;
        public event ConnectionDelegate ClientAdded;
        public event ConnectionDelegate ClientRemoved;
        public event ServerDataReceivedDelegate ServerDataReceived;

		#region "Constructors"
		
		public Game()
		{
            MinNonComputerPlayersToStartGame = 1;    // default
			MaxNonComputerPlayers = 1; //default
			RealTimeCompressionFactor = DEFAULT_REAL_TIME_COMPRESSION_FACTOR;
			IsNetworkEnabled = true;
			SkillLevel = 2;
			GameStartTime = new DateTime(2030, 6, 30, 12, 0, 0); //default
			GameCurrentTime = new DateTime(2030, 6, 30, 12, 0, 0); //default
			Id = GameManager.GetUniqueCode();
			GameManager.Instance.Log.LogDebug("Game() object " + Id + " instantiated!");
			//_gameTimer = new Timer(NET_SEND_INTERVAL_MS);
			//_LastTickTime = DateTime.Now;
			//_gameTimer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _gameTimer = new Stopwatch();
		}
		
		
		public Game(int port)
			: this()
		{
			Port = port;
		}

		public Game(Player owner, string gameName)
			: this()
		{
			GameName = gameName;
			GameOwnerPlayer = owner;
		}

		public Game(Player owner, string gameName, int port)
			: this()
		{
			GameName = gameName;
			GameOwnerPlayer = owner;
			Port = port;
		}

		#endregion

		#region "Public properties"
		
		public string Id { get; set; }

		public string GameName { get; set; }

		//public string MapName { get; set; }

		public int SkillLevel { get; set; }

		public GameScenario CurrentGameScenario { get; set; }

		public Campaign CurrentCampaign { get; set; }

		public string GameOwnerPlayerCode { get; set; }

		public int MaxNonComputerPlayers { get; set; }

		/// <summary>
		/// If non-zero, game will terminate after set number of seconds. Used for debugging and
		/// test purposes only.
		/// </summary>
		public int RunGameInSec { get; set; }

		public bool IsNetworkEnabled
		{
			get
			{
				return _IsNetworkEnabled;
			}
			set
			{
				_IsNetworkEnabled = value;
				if (_GameServer != null)
				{
					_GameServer.IsOpenForNewConnecions = value;
				}
			}
		}

		public int CountNonComputerPlayers
		{
			get
			{
			    return Players.Count(p => !p.IsComputerPlayer);
			}
		}

        /// <summary>
        /// Minimum numbers of non computer players needed for this game to start.
        /// </summary>
        public int MinNonComputerPlayersToStartGame { get; set; }

		public int Port { get; set; }

		public bool IsGameLoopStarted { get; set; }

		public bool IsGamePlayStarted { get; set; }

		public List<Player> Players
		{
			get
			{
				return _Players;
			}
		}

		public Player GameOwnerPlayer
		{
			get
			{
				return _GameOwner;
			}
			set
			{
				_GameOwner = value;
				if (_Players.Count == 0 || (_Players.Find(p => p.Equals(_GameOwner)) == null))
				{
					_Players.Add(_GameOwner);
				}
			}
		}
		/// <summary>
		/// Returns total game running time (from system clock) in milliseconds
		/// </summary>
		public double GameEngineTimeMs
		{
			get
			{
				return _gameTimer.ElapsedMilliseconds;
			}
		}



		public double GameWorldTimeSec
		{
			get
			{
				return _GameWorldTimeSec;
			}
            set
            {
                _GameWorldTimeSec = value;
            }
		}

		/// <summary>
		/// Counter is increased by 1 every tick count, regardless of time compression.
		/// </summary>
		public long TickCounter { get; set; }

		public GameConstants.GameEconoomicModel EconomicModel { get; set; }

		public int CreditsPerMinute { get; set; }

		public double RealTimeCompressionFactor { get; set; }

		public Coordinate UpperLeftCorner { get; set; }
		
		public Coordinate LowerRightCorner { get; set; }

		private DateTime _GameStartTime;
		public DateTime GameStartTime
		{
			get
			{
				return _GameStartTime;
			}
			set
			{
				if (GameCurrentTime == null)
				{
					GameCurrentTime = value;
				}
				_GameStartTime = value;
			}
		}

		public DateTime GameCurrentTime { get; set; }

		#endregion

		#region "Public methods"

		public virtual void StartGameServer()
		{
			if (IsNetworkEnabled)
			{
				if (Port == 0)
				{
					Port = GameManager.DEFAULT_TCPIP_PORT;
				}
                _GameServer = CreateGameServer();
				_GameServer.ConnectionStatusChanged += new ConnectionStatusChangedDelegate(_GameServer_ConnectionStatusChanged);
				_GameServer.ClientAdded += new ConnectionDelegate(_GameServer_ClientAdded);
                _GameServer.ClientRemoved += _GameServer_ClientRemoved;
				_GameServer.DataReceived += new ServerDataReceivedDelegate(_GameServer_DataReceived);
				_GameServer.IsOpenForNewConnecions = true;
				_GameServer.Init();
			}
		}

        protected virtual IGameServer CreateGameServer()
        {
            IGameServer server = new GameServer( Port );
            return server;
        }

		public void StartGamePlay()
		{
			GameManager.Instance.Log.LogDebug("**************************************************************************");
			GameManager.Instance.Log.LogDebug(
				string.Format("Game->StartGamePlay. GameName: {0}   Net?: {1}   PlayerCount: {2}  Max human players: {3}  TimeLimit: {4}",
				GameName, IsNetworkEnabled, this.Players.Count, this.MaxNonComputerPlayers, this.RunGameInSec));
			GameManager.Instance.Log.LogDebug("**************************************************************************");
			foreach (var player in Players)
			{
				if (player.TcpPlayerIndex == 0)
				{
					player.IsComputerPlayer = true; //hmm: no chance for late join?
				}
				if (player.IsComputerPlayer)
				{
					player.AIHandler = (BaseAIHandler)new ComputerAIHandler();
				}
				else
				{
					player.AIHandler = (BaseAIHandler)new HumanAIHandler();
				}
				player.AIHandler.OwnerPlayer = player;
                player.AIHandler.GamePlayHasStarted();
			}
			IsGamePlayStarted = true;
			_GameWorldTimeSec = 0;
			GameCurrentTime = DateTime.FromBinary(GameStartTime.ToBinary());
			//SendInitialGameUiControlsToAllPlayers(); //Do later

            _NetSendTimerMs = 0;
            _LastTickTimeMs = 0;
            _gameTimer.Start();
 
			if (!IsGameLoopStarted)
			{
				StartGameLoop();
			}
		}

		public void SendInitialGameUiControlsToAllPlayers()
		{
			foreach (var pl in Players)
			{
				if (pl.TcpPlayerIndex > 0 && !pl.IsComputerPlayer && pl.IsCompetitivePlayer)
				{
					foreach (var uiContr in pl.InitialGameUiControls)
					{
						if (uiContr.Tag != null) //tag will be a tag from scenario. Set Id
						{
                            var unit = pl.GetUnitById( "tag:" + uiContr.Tag );
							if (unit != null)
							{
								uiContr.Id = unit.Id;
							}
						}
						pl.Send(uiContr);
					}
				}
			}
		}

		public void StartGameLoop()
		{
			TickCounter = 0;
			this.IsGameLoopStarted = true;
			if (GameStartTime.Year < 2000)
			{
				GameStartTime = new DateTime(2030, 6, 30);
                GameStartTime = GameStartTime.AddHours( 12 );
			}
			GameManager.Instance.Log.LogDebug("**************************************************************************");
			GameManager.Instance.Log.LogDebug(
				string.Format("Game->StartGameLoop. GameName: {0}   Net?: {1}   PlayerCount: {2}  Max human players: {3}  TimeLimit: {4}", 
				GameName, IsNetworkEnabled, this.Players.Count, this.MaxNonComputerPlayers, this.RunGameInSec ));
			GameManager.Instance.Log.LogDebug("**************************************************************************");

			StartGameServer();
			SendGameInfoToAllPlayers();
		   //_gameTimer.Start();
#if !DEBUG
		    try
		    {
#endif
                MainGameEventLoop();
#if !DEBUG
		    }
		    catch (Exception ex)
		    {
                GameManager.Instance.Log.LogError( string.Format( "Unhandled exception in MainGameEventLoop: {0}\n{1}", ex.ToString(), ex.StackTrace ) );
		    }
#endif
        }

        /// <summary>
        /// Used to notify the game that a change has been made in the requested time compression factor by one of the players.
        /// Sets the lowest time value requested by human players as the game's time compression.
        /// </summary>
        public void RealTimeCompressionChanged()
        {
            var compressionFactor = int.MaxValue;
            foreach (var p in Players)
            {
                if (p.IsCompetitivePlayer && p.TcpPlayerIndex > 0)
                {
                    if (p.RequestedTimeCompression < compressionFactor)
                    {
                        compressionFactor = p.RequestedTimeCompression;
                    }
                }
            }
            if (compressionFactor == int.MaxValue)
            {
                compressionFactor = 10;
            }
            this.RealTimeCompressionFactor = compressionFactor;
        }

		public void SendToClient(int clientId, IMarshallable message)
		{
			if (_GameServer != null && message != null)
			{
				_GameServer.Send(clientId, message);
			}
		}
		/// <summary>
		/// Handles Game Control Messages (orders) from network (human) players to the game
		/// </summary>
		/// <param name="dataReceived">Order. Should be a GameControlRequest</param>
		/// <param name="player">Player sending the order</param>
		public void HandleGameControlMessage(IMarshallable dataReceived, Player player)
		{
			GameManager.Instance.Log.LogDebug(
				string.Format("Game->HandleGameControlMessage for player {0}: Token {1}", 
				player.ToString(), dataReceived.ObjectTypeToken));
			if (dataReceived is GameControlRequest && player != null)
			{
				GameControlRequest req = (GameControlRequest)dataReceived;
                
				switch (req.ControlRequestType)
				{
                    case CommsMarshaller.GameControlRequestType.LoadGameScene:
                        if ( player.IsAdministrator )
                        {
                            foreach ( var p in Players )
                            {
                                if ( p != player )      // Don't send back
                                {
                                    SendToClient( p.TcpPlayerIndex, req );
                                }
                            }
                        }
                        break;
                    case CommsMarshaller.GameControlRequestType.GameSceneLoaded:
                        if ( CountNonComputerPlayers < MinNonComputerPlayersToStartGame )
                        {
                            return;
                        }

				        player.HasLoadedGameScene = true;
				        bool loadGameScenario = Players.All(p => p.HasLoadedGameScene);
				        if ( loadGameScenario && this.LoadGameScenario() )
                        {
                            // Start game when all players has loaded in multiplayer
                            if ( this.MinNonComputerPlayersToStartGame > 1 )
                            {
                                this.Start();
                            }
                        }
                        break;
					case CommsMarshaller.GameControlRequestType.StartGame:
                        if ( player.IsAdministrator && !IsGamePlayStarted )
                        {
                            this.Start();
                        }
						break;
					case CommsMarshaller.GameControlRequestType.TerminateGame:
						foreach (var p in Players)
						{
							SendToClient(p.TcpPlayerIndex, req);
							//Terminate(); 

						}
						Terminate();
						break;
					case CommsMarshaller.GameControlRequestType.SetTimeCompressionRatio:
						if (req.ControlParameterValue >= 0)
						{
                            player.RequestedTimeCompression = (int)req.ControlParameterValue;
							//this.RealTimeCompressionFactor = req.ControlParameterValue;
							SendGameInfoToAllPlayers();
						}

						break;
					case CommsMarshaller.GameControlRequestType.PlayerSelectScenario:
						if (this.IsGamePlayStarted)
						{
							GameManager.Instance.Log.LogError("Game already started.");
							return;
						}

						GameScenario scenario = GameManager.Instance.GameData.GetGameScenarioById(req.Id);

						if (!player.IsAdministrator || scenario == null)
						{
                            player.Send(new GameStateInfo(GameConstants.GameStateInfoType.FailedToLoadGameScenario, req.Id));
							//Message message = player.CreateNewMessage("Failed to load game " + req.Id);
							return;
						}
						else
						{
                            if (req.ControlParameterValue > 0.0 && req.ControlParameterValue < 4.0)
                            {
                                SkillLevel = (int)req.ControlParameterValue;
                            }
							CurrentGameScenario = scenario;
							MaxNonComputerPlayers = scenario.MaxNonComputerPlayers;
                            GameManager.Instance.Log.LogDebug(
                                string.Format("HandleGameControlMessage: Player {0} selects scenario {1} at skill level {2}", 
                                player.ToString(), scenario.GameName, this.SkillLevel));
							foreach (var p in Players)
							{
								if (p.TcpPlayerIndex > 0)
								{
									p.Send(scenario);
								}
							}
							SendGameInfoToAllPlayers();
						}
						break;
					case CommsMarshaller.GameControlRequestType.PlayerSelectScenarioPlayer:
						player.ScenarioPlayerId = req.Id;
                        player.Send( player.GetPlayerInfo() );
						break;
					case CommsMarshaller.GameControlRequestType.PlayerSelectCampaign:
						player.SelectCampaign(req);
						break;
                    case CommsMarshaller.GameControlRequestType.PlayerSelectUser:
                        player.SelectUser(req);
                        break;
                    case CommsMarshaller.GameControlRequestType.PlayerSetName:
                        player.Name = req.Id;
                        SendGameInfoToAllPlayers();
                        break;
					case CommsMarshaller.GameControlRequestType.DesignateContactFriendOrFoe:
						player.DesignateContactFriendOrFoe(req.Id, req.FriendOrFoeDesignation);
						break;
					case CommsMarshaller.GameControlRequestType.SetUnknownContactFoFDesignation:
						player.IsAllUnknownContactsHostile = req.IsParameter;
                        if (player.IsAllUnknownContactsHostile)
                        {
                            foreach (var det in player.DetectedUnits) //apply retroactively when setting all unkknowns hostile
                            {
                                if (det.FriendOrFoeClassification == GameConstants.FriendOrFoe.Undetermined && !det.IsIdentified)
                                {
                                    det.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                                    det.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                                    var detGroup = det.GetDetectedGroup();
                                    if ( detGroup != null)
                                    {
                                        detGroup.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                                    }
                                }
                            }
                        }
						player.Send(player.GetPlayerInfo());
						break;
					case CommsMarshaller.GameControlRequestType.SetAutomaticActiveSensorResponse:
						player.IsAutomaticallyRespondingToActiveSensor = req.IsParameter;
						player.Send(player.GetPlayerInfo());
						break;
					case CommsMarshaller.GameControlRequestType.SetAutomaticUnitEvasion:
						player.IsAutomaticallyEvadingAttacks = req.IsParameter;
						player.Send(player.GetPlayerInfo());
						break;
					case CommsMarshaller.GameControlRequestType.SetWeaponOrdersUnit:
						player.SetWeaponOrders(req.WeaponOrders, req.Id);
						break;
					case CommsMarshaller.GameControlRequestType.SetWeaponOrdersGlobal:
						player.SetWeaponOrders(req.WeaponOrders, req.IsParameter);
                        player.Send(player.GetPlayerInfo());
						break;
                    case CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnDetection:
                        player.IsAutomaticallyChangingTimeOnDetection = req.IsParameter;
                        player.TimeCompressionOnDetection = req.ControlParameterValue;
                        player.Send(player.GetPlayerInfo());
                        break;
                    case CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnBattleReport:
                        player.IsAutomaticallyChangingTimeOnBattleReport = req.IsParameter;
                        player.TimeCompressionOnBattleReport = req.ControlParameterValue;
                        player.Send(player.GetPlayerInfo());
                        break;
                    case CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnNoOrder:
                        player.IsAutomaticallyChangingTimeOnNoOrder = req.IsParameter;
                        player.TimeCompressionOnNoOrder = req.ControlParameterValue;
                        player.Send(player.GetPlayerInfo());
                        break;
					case CommsMarshaller.GameControlRequestType.CheatCode:
                        player.Cheat(req.Id, req.ControlParameterString);
						break;
					case CommsMarshaller.GameControlRequestType.RemoveHighLevelOrder:
						try
						{
							var hlo = player.HighLevelOrders.First(h => h.Id == req.Id);
							player.HighLevelOrders.Remove(hlo);
						}
						catch (Exception)
						{
							//ignore for now
							
						}
						player.Send(player.GetPlayerInfo());
						break;
					case CommsMarshaller.GameControlRequestType.RenameUnit:
						var unit = player.GetUnitById(req.Id);
						if (unit != null)
						{
							unit.Name = req.ControlParameterString;
							unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
						}
						break;
					case CommsMarshaller.GameControlRequestType.RenameGroup:
						var group = player.GetGroupById(req.Id);
						if (group != null)
						{
							group.Name = req.ControlParameterString;
							group.SetDirty(GameConstants.DirtyStatus.UnitChanged);
						}
						break;
					case CommsMarshaller.GameControlRequestType.SendMessageToPlayers:
						player.HandleSendMessageToPlayers(req);
						break;
					case CommsMarshaller.GameControlRequestType.GetAllUnitsInArea:
						player.HandleGetAllUnitsInArea(req);
						break;
                    case CommsMarshaller.GameControlRequestType.TriggerSignal:
                        player.HandleTriggerSignal(req);
                        break;
					default:
						break;
				}
			}
		}

        private bool LoadGameScenario()
        {
            if ( CurrentGameScenario == null )
            {
                GameManager.Instance.Log.LogError( "No game scenario selected." );
                return false;
            }
            if ( !GameManager.Instance.GameData.LoadGameScenario(
                CurrentGameScenario.Id, this ) )
            {
                GameManager.Instance.Log.LogError( "Failed to load scenario!" );
                return false;
            }

            SendGameInfoToAllPlayers();
            foreach ( var p in Players )
            {
                p.SendUpdates();
            }
            return true;
        }

        private bool Start()
        {
            if ( CurrentGameScenario == null )
            {
                GameManager.Instance.Log.LogError( "No game scenario selected." );
                return false;
            }
            if ( CountNonComputerPlayers < MinNonComputerPlayersToStartGame )
            {
                GameManager.Instance.Log.LogError( "Not enough players to start game." );
                return false;
            }
            SendInitialGameUiControlsToAllPlayers();
            StartGamePlay();
            SendGameInfoToAllPlayers();
            return true;
        }
		//private void InvokeTick()
		//{
		//}


		public void SetAllPlayersEnemies()
		{
			foreach (var player in Players)
			{
				foreach (var playerOther in Players)
				{
					if (player != playerOther && player.IsCompetitivePlayer && playerOther.IsCompetitivePlayer)
					{
						if (!player.Enemies.Contains(playerOther))
						{
							player.Enemies.Add(playerOther);
						}
					}
				}
			}
		}


		public Player GetPlayerById(string playerId)
		{
			try
			{
				return Players.Single<Player>(p => p.Id == playerId);
			}
			catch (Exception)
			{
				GameManager.Instance.Log.LogError(
					"Game->GetPlayerById failed to find player for id=" + playerId);
				return null;
			}
		}

		public Player GetPlayerByClientIndex(int tcpPlayerIndex)
		{
			try
			{
				return Players.Single<Player>(p => p.TcpPlayerIndex == tcpPlayerIndex);
			}
			catch (Exception)
			{
				GameManager.Instance.Log.LogError(
					"Game->GetPlayerByClientIndex failed to find player for index=" + tcpPlayerIndex);
				return null;
			}
		}

		public void AddPlayer(Player player)
		{
			if (Players.Find(p => p.Id == player.Id) == null)
			{
				Players.Add(player);
				if (GameOwnerPlayer == null)
				{
					GameOwnerPlayer = player;
				}
			}
		}

		public bool RemovePlayer(Player player)
		{
			return Players.Remove(player);
		}

        ///// <summary>
        ///// Tells the game that one defeat condition has been met in the game. Used to signal competitive players about this fact.
        ///// </summary>
        ///// <param name="player"></param>
        ///// <param name="defeatCondition"></param>
        //public void DefeatConditionHasBeenMet(Player player, DefeatCondition defeatCondition)
        //{
        //    if (player == null || defeatCondition == null)
        //    {
        //        return;
        //    }
        //    SendGameInfoToAllPlayers();

        //}

        public void PlayerHasMetVictoryConditions(Player player)
        {
            if (player.Enemies.Any(p => p.HasWonGame))
            {
                return; //if another player has already won the game, ignore this player meeting victory conditions later
            }

            player.GameVictoryAchieved();
            foreach (var p in player.Enemies)
            {
                p.HasBeenDefeated = true;
            }
            IsGamePlayStarted = false;  // Stop game as it's finished
            SendGameInfoToAllPlayers();
        }

		/// <summary>
		/// Used to send information to game that a player has been defeated, to 
		/// check whether if a player has won and the game is completed.
		/// </summary>
		/// <param name="player">The player that has met defeat conditions</param>
        //public void PlayerHasLostGame(Player player)
        //{
        //    if (!player.HasBeenDefeated)
        //    {
        //        return; //ignore call. It's a lie.
        //    }
        //    int CountOfNonDefeatedActivePlayers = 0;
        //    Player PotentialWinner = null;
        //    foreach (var enemy in this.Players)
        //    {
        //        if (enemy.IsCompetitivePlayer && !enemy.HasBeenDefeated)
        //        {
        //            CountOfNonDefeatedActivePlayers++;
        //            PotentialWinner = enemy;
        //        }
        //    }
        //    if (CountOfNonDefeatedActivePlayers == 1 && PotentialWinner != null)
        //    {
        //        PotentialWinner.GameVictoryAchieved();
        //    }
        //    else
        //    { 
        //        //TODO: Allied victory?
        //    }
        //    SendGameInfoToAllPlayers();
        //}

		public double GetRealSecondsFromGameTime(double gameTimeMs)
		{
			return (gameTimeMs * RealTimeCompressionFactor) / 1000.0;
		}

		public virtual void Terminate()
		{
			IsGameLoopStarted = false;
			//TODO: Send terminate to clients, if any
			//_gameTimer.Stop();
			if (_GameServer != null && _GameServer.ConnectionStatus == GameConstants.ConnectionStatusEnum.Connected)
			{
				_GameServer.Disconnect(); //TODO CHECK!
			    _GameServer = null;
			}
		}

		public GameInfo GetGameInfo()
		{
			GameInfo info = new GameInfo();
			info.IsGamePlayStarted = this.IsGamePlayStarted;
			info.CountNonComputerPlayers = this.CountNonComputerPlayers;
            info.MinNonComputerPlayersToStartGame = this.MinNonComputerPlayersToStartGame;
            //info.GameName = this.GameName;
			if (GameOwnerPlayer != null)
			{
				info.GameOwnerPlayerCode = this.GameOwnerPlayer.Id;
			}
			info.Id = this.Id;
			if (this.CurrentGameScenario != null)
			{
				info.ScenarioId = this.CurrentGameScenario.Id;
			}
            if (this.CurrentCampaign != null)
            {
                info.CampaignId = this.CurrentCampaign.Id;
            }
			info.GameStartTime = new NWDateTime(GameStartTime);
			info.GameCurrentTime = new NWDateTime(GameCurrentTime);
            info.GameWorldTimeSec = this.GameWorldTimeSec;
            info.GameEngineTimeMs = this.GameEngineTimeMs;
			info.EconomicModel = EconomicModel;
			info.CreditsPerMinute = CreditsPerMinute;
			if (LowerRightCorner != null)
			{
				info.LowerRightCorner = new PositionInfo()
				{
					Latitude = this.LowerRightCorner.LatitudeDeg,
					Longitude = this.LowerRightCorner.LongitudeDeg
				};
			}
			if (UpperLeftCorner != null)
			{
				info.UpperLeftCorner = new PositionInfo()
				{
					Latitude = this.UpperLeftCorner.LatitudeDeg,
					Longitude = this.UpperLeftCorner.LongitudeDeg
				};
			}
			//info.MapName = this.MapName;
			info.MaxNonComputerPlayers = this.MaxNonComputerPlayers;
			info.RealTimeCompressionFactor = this.RealTimeCompressionFactor;
			info.SkillLevel = this.SkillLevel;
			foreach (var player in Players)
			{
				var pinfo = player.GetPlayerInfo(); //hide secret info
				pinfo.HighLevelOrders = new List<HighLevelOrder>();
				pinfo.IsAutomaticallyRespondingToActiveSensor = false;
				pinfo.IsAutomaticallyEvadingAttacks = false;
				pinfo.IsAllUnknownContactsHostile = false;
				info.Players.Add(pinfo);
			}
			return info;
		}

		/// <summary>
		/// Initiates a tick that moves events in the game forward. This is called by the game event loop.
		/// </summary>
        /// <param name="deltaTimeMs">The number of real time milliseconds since the last tick.</param>
		public void Tick(double deltaTimeMs)
		{
			if (IsGamePlayStarted)
			{
				TickCounter++;
                //if (TickCounter % 60 == 0) //TODO: Const
                //{
                //    SendGameInfoToAllPlayers();
                //}
                //if (TickCounter == 2)
                //{
                //    SendInitialGameUiControlsToAllPlayers();
                //}

                _gameTickDeltaSec += GetRealSecondsFromGameTime(deltaTimeMs);

                double timeCompressionFactor = this.RealTimeCompressionFactor;

                // Grab the time we started ticking game
			    var gameTickTimer = new Stopwatch();
                gameTickTimer.Start();

                while (IsGamePlayStarted && _gameTickDeltaSec >= GAME_TICK_MIN_LENGTH_SEC)
                {
                    // Cap maximum game time to tick
                    double gameTickSec = _gameTickDeltaSec;
                    if (gameTickSec > GAME_TICK_MAX_LENGHT_SEC)
                    {
                        gameTickSec = GAME_TICK_MAX_LENGHT_SEC;
                    }

                    // Poll for updates from client
                    if (_GameServer != null)
                    {
                        _GameServer.PollNetwork();
                    }

                    // Tick the game
                    GameTick(gameTickSec);

                    // Did the compression factor change?
                    if (timeCompressionFactor != this.RealTimeCompressionFactor)
                    {
                        _gameTickDeltaSec = 0;
                        break;
                    }

                    _gameTickDeltaSec -= gameTickSec;

                    // Break out if we've spent too long ticking game
                    if (gameTickTimer.ElapsedMilliseconds > GAME_TICK_MAX_TIME)
                    {
                        break;
                    }
                }

			    _NetSendTimerMs += deltaTimeMs + gameTickTimer.ElapsedMilliseconds;

                // Finally send the updates for this tick to each player
                if ( _NetSendTimerMs >= NET_SEND_INTERVAL_MS )
                {
                    foreach (Player player in Players)
                    {
                        player.SendUpdates();
                    }
                    _NetSendTimerMs = 0;
                }

                // Did game end or time compression change?
                if (!IsGamePlayStarted || timeCompressionFactor != this.RealTimeCompressionFactor)
                {
                    SendGameInfoToAllPlayers();
                }

                gameTickTimer.Stop();
			}
		}

        private void GameTick( double deltaGameTimeSec )
        {
            _GameWorldTimeSec += deltaGameTimeSec;

            this.GameCurrentTime = this.GameCurrentTime.AddSeconds(deltaGameTimeSec);

            // List of all units in the game
            List<BaseUnit> units = new List<BaseUnit>();

            foreach ( Player player in Players )
            {
                player.PreTick( deltaGameTimeSec );
                units.AddRange( player.Units );
            }

            // Update unit positions
            foreach ( BaseUnit unit in units )
            {
                unit.MoveToNewCoordinate( deltaGameTimeSec );
            }

            // Do sensor sweeps
            foreach ( BaseUnit unit in units )
            {
                unit.SensorSweep( deltaGameTimeSec );
            }

            // Execute orders for each unit
            foreach ( BaseUnit unit in units )
            {
                unit.ExecuteOrders();
            }

            // Tick each unit
            foreach ( BaseUnit unit in units )
            {
                unit.Tick( deltaGameTimeSec );
            }

            // Remove area effects AFTER we ticked our objects
            var areaEffects = from a in BlackboardController.Objects
                              where a is AreaEffect
                              select a;

            foreach ( var eff in areaEffects )
            {
                var areaEffect = eff as AreaEffect;
                if ( areaEffect.TimeToLiveSec > 0 )
                {
                    areaEffect.TimeToLiveSec -= deltaGameTimeSec;
                }
            }

            int removeCount = BlackboardController.Objects.RemoveAll(
                a => ( a is AreaEffect ) && ( a as AreaEffect ).TimeToLiveSec <= 0 );

            //var aiHints = from a in BlackboardController.Objects
            //              where a is AIHint
            //              select a;
            //foreach (var hint in aiHints)
            //{
            //    var aiHint = hint as AIHint;
            //    if (aiHint.TimeToLiveSec > 0)
            //    {
            //        aiHint.TimeToLiveSec -= gameTimeElapsedSec;
            //    }
            //}

            //int removeAiCount = BlackboardController.Objects.RemoveAll(
            //    a => (a is AIHint) && (a as AIHint).TimeToLiveSec <= 0);

            foreach ( Player player in Players )
            {
                player.Tick( deltaGameTimeSec );
            }
        }

		public void MainGameEventLoop()
		{
			bool IsRunning = true;
			do
			{
                if ( _GameServer != null )
                {
                    _GameServer.PollNetwork();
                }

				if (IsGamePlayStarted)
				{
				    double deltaTimeMs = _gameTimer.ElapsedMilliseconds - _LastTickTimeMs;
					_LastTickTimeMs = _gameTimer.ElapsedMilliseconds;

                    Tick(deltaTimeMs);
                }
  
				if (!IsGameLoopStarted)
				{
					IsRunning = false;
				}
				else if (RunGameInSec > 0 && RunGameInSec < GameWorldTimeSec)
				{
					IsRunning = false;
					GameManager.Instance.Log.LogDebug("Game ends after RunGameInSec seconds (" + RunGameInSec + ").");
				}
				System.Threading.Thread.Sleep(1);
				
			} while (IsRunning);

            _gameTimer.Stop();
		}

		public void SendGameInfoToAllPlayers()
		{
			GameInfo info = GetGameInfo();
			foreach (var p in Players)
			{
				p.AddAllAutomaticDetections(); //make sure all automatic detections are added
				if (p.TcpPlayerIndex > 0)
				{
                    p.Send(info);
                    p.Send(p.GetPlayerInfo());
				}
			}
		}

		public BaseUnit GetUnitById(string id)
		{
		    return Players.SelectMany(p => p.Units).FirstOrDefault(u => u.Id == id);
		}

	    public DetectedUnit GetDetectedUnitById(string id)
	    {
	        return Players.SelectMany(p => p.DetectedUnits).FirstOrDefault(u => u.Id == id);
	    }

	    /// <summary>
		/// Returns jamming degradation in area in percent, ignoring only jammers that are 
		/// owned by the player.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public double GetJammingDegradationPercent(Coordinate coordinate, Player player)
		{
			BlackboardFinder<AreaEffect> finder = new BlackboardFinder<AreaEffect>();
			var allAreaEffects = finder.GetAllSortedByCoordinateAndType(coordinate, GameConstants.MAX_DETECTION_DISTANCE_M);
			var relevantEffects = allAreaEffects.Where<IBlackboardObject>(
				j => (j as AreaEffect).TimeToLiveSec >= 0
				&& ((j as AreaEffect).AreaEffectType == GameConstants.AreaEffectType.JammerRadarDegradation)
				&& j.OwnerId != player.Id 
				&& j.DistanceToM(coordinate) <= (j as AreaEffect).RadiusM);
			if (relevantEffects.Count<IBlackboardObject>() == 0)
			{
				return 0.0;
			}
			var maxValue = relevantEffects.Max<IBlackboardObject>(j => (j as AreaEffect).Strength);
			return maxValue;
		}

        public bool IsUnitIncludedForPlayer(Player player, GameConstants.SkillLevelInclusion skillInclusion)
        {
            switch (skillInclusion)
            {
                case GameConstants.SkillLevelInclusion.IncludeAlways:
                    return true;
                case GameConstants.SkillLevelInclusion.IncludeToIncreasedLevel:
                    if (!player.IsComputerPlayer) //player is human
                    {
                        return false;
                        //if (SkillLevel > 2)
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                        //    return true;
                        //}
                    }
                    else //computer player
                    {
                        if (SkillLevel > 2)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case GameConstants.SkillLevelInclusion.IncludeToReducedLevel:
                    if (!player.IsComputerPlayer) //human
                    {
                        if (SkillLevel < 2)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else //computer player
                    {
                        return false;
                    }
                default:
                    break;
            }
            return true;
        }

		public BaseOrder GetBaseOrderFromUnitOrder(UnitOrder order)
		{

			//GameManager.Instance.Log.LogDebug("GetBaseOrderFromUnitOrder handling UnitOrder: " + order.ToString());
			BaseOrder baseOrder = new BaseOrder();
            baseOrder.RemoveAllExistingWaypoints = order.RemoveAllExistingWaypoints;
			switch (order.UnitOrderType)
			{
				case GameConstants.UnitOrderType.LaunchAircraft:
					baseOrder.OrderType = GameConstants.OrderType.LaunchOrder;
					try
					{
						LaunchAircraftOrder launchOrder = new LaunchAircraftOrder();
						foreach (var id in order.ParameterList)
						{
							var launchUnit = (AircraftUnit)GetUnitById(id);
							if (launchUnit != null)
							{
								launchOrder.UnitList.Add(launchUnit);
							}
							else
							{
								GameManager.Instance.Log.LogError(
									string.Format("GetBaseOrderFromUnitOrder: {0} given to non-existant unit Id {1}",
									order.ToString(), id));
							}
						}
						foreach (var unitOrder in order.UnitOrders)
						{
							BaseOrder subOrder = GetBaseOrderFromUnitOrder(unitOrder);
							if (subOrder != null)
							{
								launchOrder.Orders.Add(subOrder);
							}
							else
							{
								GameManager.Instance.Log.LogError(
									string.Format(
									"GetBaseOrderFromUnitOrder: LaunchAircraft order to {0} contained subOrder that could not be converted,", 
									order.Id));
							}
						}
						baseOrder = launchOrder;
						//unit.LaunchAircraft(order.ParameterList, null, string.Empty); //move to order execution

					}
					catch (Exception ex)
					{
						GameManager.Instance.Log.LogError(
							"GetBaseOrderFromUnitOrder failed for " + order.UnitOrderType + ". " + ex.ToString());
					}
					break;
				case GameConstants.UnitOrderType.ReturnToBase:
					baseOrder.OrderType = GameConstants.OrderType.ReturnToBase;
                    baseOrder.SecondId = order.SecondId;
					break;
				case GameConstants.UnitOrderType.SensorActivationOrder:
					baseOrder.OrderType = GameConstants.OrderType.SensorActivationOrder;
					baseOrder.SecondId = order.SecondId;
					baseOrder.SensorType = order.SensorType;
					baseOrder.IsParameter = order.IsParameter;
					baseOrder.ValueParameter = order.ValueParameter;
					break;
				case GameConstants.UnitOrderType.SensorDeploymentOrder:
					baseOrder.OrderType = GameConstants.OrderType.SensorDeploymentOrder;
					baseOrder.SensorType = order.SensorType;
					baseOrder.SecondId = order.SecondId;
                    baseOrder.RadiusM = order.RadiusM;
					baseOrder.IsParameter = order.IsParameter;
					baseOrder.ValueParameter = order.ValueParameter; //non-zero means deep deployment for sonar
					break;
				case GameConstants.UnitOrderType.SetSpeed:
					baseOrder.OrderType = GameConstants.OrderType.SetSpeed;
					baseOrder.UnitSpeedType = order.UnitSpeedType;
					baseOrder.ValueParameter = order.ValueParameter;
                    baseOrder.IsParameter = order.IsParameter;
					break;
				case GameConstants.UnitOrderType.ChangeAircraftLoadout:
					baseOrder.OrderType = GameConstants.OrderType.ChangeAircraftLoadout;
					baseOrder.SecondId = order.SecondId;
                    baseOrder.WeaponLoadType = order.WeaponLoadType;
                    baseOrder.WeaponLoadModifier = order.WeaponLoadModifier;
					baseOrder.StringParameter = order.StringParameter;
					break;
				case GameConstants.UnitOrderType.SetNewGroupFormation:
					baseOrder.OrderType = GameConstants.OrderType.SetNewGroupFormation;
					baseOrder.Formation = order.Formation;
					break;

				case GameConstants.UnitOrderType.SetUnitFormationOrder:
					baseOrder.OrderType = GameConstants.OrderType.SetUnitFormationOrder;
					baseOrder.SecondId = order.SecondId;
					break;
				case GameConstants.UnitOrderType.SetElevation:
					baseOrder.OrderType = GameConstants.OrderType.SetElevation;
					baseOrder.ValueParameter = order.ValueParameter;
					baseOrder.HeightDepthPoints = order.HeightDepthPoints;
					break;
				case GameConstants.UnitOrderType.MovementOrder:
					//baseOrder.OrderType = GameConstants.OrderType.MovementOrder;
					
					UnitMovementOrder unitMoveOrder = order as UnitMovementOrder;
					if (unitMoveOrder != null)
					{
						if (unitMoveOrder.Position == null) //special case: if pos is null, then it is an engagement order
						{
							if (!string.IsNullOrEmpty(unitMoveOrder.SecondId))
							{
								var detect = GetDetectedUnitById(unitMoveOrder.SecondId);
								if (detect != null)
								{
									EngagementOrder engageOrder = new EngagementOrder(detect,GameConstants.EngagementOrderType.CloseAndEngage);
									return engageOrder;
								}
							}
						}
                        
						MovementOrder moveOrder = new MovementOrder();
						foreach (var pos in unitMoveOrder.Waypoints)
						{
							moveOrder.AddWaypoint(pos);
						}
                        moveOrder.OrderType = GameConstants.OrderType.MovementOrder;
                        moveOrder.IsRecurring = unitMoveOrder.IsParameter;
						baseOrder = moveOrder;
					}
					break;
				case GameConstants.UnitOrderType.SplitGroup:
					baseOrder.OrderType = GameConstants.OrderType.SplitGroup;
					baseOrder.ParameterList = order.ParameterList;
					break;
				case GameConstants.UnitOrderType.JoinGroups:
					baseOrder.OrderType = GameConstants.OrderType.JoinGroups;
					baseOrder.SecondId = order.SecondId;
					baseOrder.ParameterList = order.ParameterList;
					break;
				case GameConstants.UnitOrderType.SpecialOrders:
					baseOrder.OrderType = GameConstants.OrderType.SpecialOrders;
					baseOrder.SpecialOrders = order.SpecialOrder;
                    baseOrder.RadiusM = order.RadiusM;
					baseOrder.IsParameter = order.IsParameter; //Active
					baseOrder.ValueParameter = order.ValueParameter; // >0 is intermediate depth
					if (order.Position != null)
					{
						baseOrder.Position = new Position(order.Position);
					}
					break;
                case GameConstants.UnitOrderType.RefuelInAir:
                    baseOrder.OrderType = GameConstants.OrderType.RefuelInAir;
			        baseOrder.SecondId = order.SecondId;
                    break;
				default:
					GameManager.Instance.Log.LogWarning(
						"GetBaseOrderFromUnitOrder: Case falls to default for OrderType " + order.UnitOrderType);
					break;
			}
			//GameManager.Instance.Log.LogDebug("GetBaseOrderFromUnitOrder returning BaseOrder: " + baseOrder.ToString());
			return baseOrder;
		}


		#endregion

		#region "Private methods"

		private void _GameServer_DataReceived(int clientID, IMarshallable dataReceived)
		{
			ServerDataReceived(clientID, dataReceived);

			Player player = GetPlayerByClientIndex(clientID);
			if (player != null)
			{
				player.HandleMessageFromClient(dataReceived);
			}
			else
			{
				GameManager.Instance.Log.LogError(
					"GameServer_DataReceived receives message from unknown clientID = "
					+ clientID + " Data token: " + dataReceived.ObjectTypeToken);
			}
		}

		private void _GameServer_ConnectionStatusChanged(GameConstants.ConnectionStatusEnum status)
		{
			GameManager.Instance.Log.LogDebug("Game connection status is set to " + status.ToString());
			if (ConnectionStatusChanged != null)
			{
				ConnectionStatusChanged(status);
			}
            if ( status == GameConstants.ConnectionStatusEnum.Disconnected )
			{
				Terminate();
			}
		}

		private void _GameServer_ClientAdded(int clientId)
		{
			if (CountNonComputerPlayers < MaxNonComputerPlayers)
			{
				Player player = new Player();
				player.IsComputerPlayer = false;
				player.IsCompetitivePlayer = true;
				player.ScenarioPlayerId = string.Empty;
				player.TcpPlayerIndex = clientId;
				if (clientId == 1)
				{
					//Hmm. First connection will always be game admin
					player.IsAdministrator = true;
				}

                AddPlayer(player);
                player.Send(player.GetPlayerInfo());
                if ( clientId != 1 )
                {
                    SendGameInfoToAllPlayers();
                }

				GameManager.Instance.Log.LogDebug("Game: New connection. ClientId= " + clientId + ". Player=" + player.ToString());				
			}
			else
			{
				GameManager.Instance.Log.LogWarning("Game: New connection from client ignored since game is full.");
			}

            // If game is full, prevent server from accepting more connections.
            if (_GameServer != null && CountNonComputerPlayers >= MaxNonComputerPlayers)
            {
                _GameServer.IsOpenForNewConnecions = false;
            }

			if (ClientAdded != null)
			{
				ClientAdded(clientId);
			}
		}

        private void _GameServer_ClientRemoved( int clientId )
        {
            // If a player leaves in PVP the other player wins the game
            if ( CountNonComputerPlayers == 2 )
            {
                Player winningPlayer = this.Players.Find( p => !p.IsComputerPlayer && p.TcpPlayerIndex != clientId );
                if ( winningPlayer != null )
                {
                    GameManager.Instance.Log.LogWarning( "Client left in PVP - other player has won game!" );
                    PlayerHasMetVictoryConditions( winningPlayer );
                }
            }

            RemovePlayer( GetPlayerByClientIndex( clientId ) );

            if ( ClientRemoved != null )
            {
                ClientRemoved( clientId );
            }
        }

		//private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		//{
		//    InvokeTick();
			
		//}


		
		#endregion










	}
}
