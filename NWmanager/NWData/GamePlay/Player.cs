using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.Ai;
using System.Diagnostics;


namespace TTG.NavalWar.NWData.GamePlay
{
	[Serializable]
	public class Player : GameObject
	{
		private List<BaseUnit> _units = new List<BaseUnit>();
		private readonly List<Group> _groups;
		private bool _hasBeenDefeated = false;
		private List<Message> _messageToPlayer;
		private List<Message> _messageFromPlayer;
		private List<DetectedUnit> _detectedUnits;
		private List<DetectedGroup> _detectedGroups;
        //private List<BattleDamageReport> _NewBattleDamageReports = new List<BattleDamageReport>();
        private double _timeNextEventTriggerCheck = 0;
		
        public const string TAG_MARKER = "tag:";

		public Player()
			: base()
		{
			_groups = new List<Group>();
            DefaultWeaponOrders = GameConstants.WeaponOrders.FireInSelfDefenceOnly;
			BattleDamageReports = new List<BattleDamageReport>();
			//DefeatConditionSets = new List<DefeatConditionSet>();
			HighLevelOrders = new FlexQueue<HighLevelOrder>();
			EventTriggers = new List<EventTrigger>();
			AcquirableUnitClasses = new List<string>();
            IsAutomaticallyEvadingAttacks = true;
			InitialGameUiControls = new List<GameUiControl>();
            
			IsCompetitivePlayer = true;
			Enemies = new List<Player>();
			Allies = new List<Player>();
            IsAllUnknownContactsHostile = false;
            IsAutomaticallyEngagingHighValueTargets = true;
            IsAutomaticallyEngagingOpportunityTargets = true;
            IsAutomaticallySettingHighValueDefence = true;
            IsAutomaticallyChangingTimeOnDetection = true;
            IsAutomaticallyChangingTimeOnBattleReport = true;
            IsAutomaticallyChangingTimeOnNoOrder = true;

            TimeCompressionOnDetection = 1;
            TimeCompressionOnBattleReport = 1;
            TimeCompressionOnNoOrder = 1;
            _RequestedTimeCompression = 1;

			Name = Id; //In case it is not overruled			
		}

		#region "Public properties"

        private bool _HasLoadedGameScene = false;
        public bool HasLoadedGameScene
        {
            get
            {
                return _HasLoadedGameScene || this.IsComputerPlayer;        // Computer player should always return true
            }
            set
            {
                _HasLoadedGameScene = value;
            }
        }

		/// <summary>
		/// Index of player Tcp comms. 0 is unassigned; first available is 1.
		/// </summary>
		public int TcpPlayerIndex { get; set; }

		public bool IsAdministrator { get; set; }

        private int _RequestedTimeCompression;

        public int RequestedTimeCompression 
        {
            get
            {
                return _RequestedTimeCompression;
            }
            set
            {
                _RequestedTimeCompression = value;
                if (GameManager.Instance.Game != null)
                {
                    GameManager.Instance.Game.RealTimeCompressionChanged();
                }
            }


        }

        public User CurrentUser { get; set; }

		/// <summary>
		/// Which GameScenarioPlayer id this player is corresponding to
		/// </summary>
		public string ScenarioPlayerId { get; set; }

        private GameConstants.RulesOfEngagement _CurrentRulesOfEngagement { get; set; }
        public GameConstants.RulesOfEngagement CurrentRulesOfEngagement 
        {
            get
            {
                return _CurrentRulesOfEngagement;
            }
            set
            {
                if (value != _CurrentRulesOfEngagement)
                {
                    if (TcpPlayerIndex > 0)
                    {
                        Send(GetPlayerInfo());
                    }
                }
                _CurrentRulesOfEngagement = value;
            }
        }

        private bool _HasPlayerBeenFiredUpon;
        public bool HasPlayerBeenFiredUpon 
        { 
            get
            {
                return _HasPlayerBeenFiredUpon;
            }
            set
            {
                if (value != _HasPlayerBeenFiredUpon)
                {
                    if (TcpPlayerIndex > 0)
                    {
                        Send(GetPlayerInfo());
                    }
                }
                _HasPlayerBeenFiredUpon = value;
            }
        }

        public bool IsPlayerPermittedToOpenFire 
        {
            get
            {
                switch (CurrentRulesOfEngagement)
                {
                    case GameConstants.RulesOfEngagement.FireOnClearedTargets:
                        return true;
                    case GameConstants.RulesOfEngagement.FireOnlyInSelfDefence:
                        return (HasPlayerBeenFiredUpon);
                    case GameConstants.RulesOfEngagement.HoldFire:
                        return false;
                }
                return true;
            }
        }

		public List<Player> Enemies { get; set; }

		public List<Player> Allies { get; set; }

		public List<string> AcquirableUnitClasses { get; set; }

		public List<BattleDamageReport> BattleDamageReports { get; set; }

		public FlexQueue<HighLevelOrder> HighLevelOrders { get; set; }

		public List<EventTrigger> EventTriggers { get; set; }

		public List<GameUiControl> InitialGameUiControls { get; set; }

		public List<DetectedUnit> DetectedUnits
		{
			get
			{
				if (_detectedUnits == null)
				{
					_detectedUnits = new List<DetectedUnit>();
				}
				return _detectedUnits;
			}
			set
			{
				_detectedUnits = value;
			}
		}

		public List<DetectedGroup> DetectedGroups
		{
			get
			{
				if (_detectedGroups == null)
				{
					_detectedGroups = new List<DetectedGroup>();
				}
				return _detectedGroups;
			}
			set
			{
				_detectedGroups = value;
			}
		}

		public List<BaseUnit> Units
		{
			get
			{
				return _units;
			}
			set
			{
				_units = value;
			}
		}

		public IList<Group> Groups
		{
			get
			{
				return _groups.AsReadOnly();
			}
		}


		public List<Message> MessageToPlayer
		{
			get
			{
				if (_messageToPlayer == null)
				{
					_messageToPlayer = new List<Message>();
				}
				return _messageToPlayer;
			}
			set
			{
				_messageToPlayer = value;
			}
		}
		public List<Message> MessageFromPlayer
		{
			get
			{
				if (_messageFromPlayer == null)
				{
					_messageFromPlayer = new List<Message>();
				}
				return _messageFromPlayer;
			}
			set { _messageFromPlayer = value; }
		}

		//public List<DefeatConditionSet> DefeatConditionSets { get; set; }

		public BaseAIHandler AIHandler { get; set; }

		public Country Country { get; set; }

		public GameConstants.WeaponOrders DefaultWeaponOrders { get; set; }

		public int Credits { get; set; }

		public bool IsAllUnknownContactsHostile { get; set; }

		public bool IsAutomaticallyEvadingAttacks { get; set; }

		public bool IsAutomaticallyRespondingToActiveSensor { get; set; }

        /// <summary>
        /// For AI players. If true, tries to automatically find targets based on victory conditions and engage them.
        /// </summary>
        public bool IsAutomaticallyEngagingHighValueTargets { get; set; }

        /// <summary>
        /// For AI players. If true, may engage all targets of opportunity when they are detected. 
        /// </summary>
        public bool IsAutomaticallyEngagingOpportunityTargets { get; set; }

        /// <summary>
        /// For AI players. If true, sets up AEW and ASW for high value units automatically.
        /// </summary>
        public bool IsAutomaticallySettingHighValueDefence { get; set; }

        /// <summary>
        /// Should the player request a changed time compression on new detections?
        /// </summary>
        public bool IsAutomaticallyChangingTimeOnDetection { get; set; }

        /// <summary>
        /// Should the player request a changed time compression on new battle reports?
        /// </summary>
        public bool IsAutomaticallyChangingTimeOnBattleReport { get; set; }

        /// <summary>
        /// Should the player request a changed time compression when a unit has no order?
        /// </summary>
        public bool IsAutomaticallyChangingTimeOnNoOrder { get; set; }

        public double TimeCompressionOnDetection { get; set; }
        public double TimeCompressionOnBattleReport { get; set; }
        public double TimeCompressionOnNoOrder { get; set; }

		public bool IsComputerPlayer { get; set; }

		/// <summary>
		/// Used for scenario loading.
		/// </summary>
		public string AllianceId { get; set; }

		/// <summary>
		/// Determines whether this is a player that can win or lose game. Players controlling neutral units,
		/// observers, etc, will have this property set to false.
		/// </summary>
		public bool IsCompetitivePlayer { get; set; }

		/// <summary>
		/// Used for identifying game player as specific scenario player, 
		/// for triggers, identifyers, AI, etc.
		/// </summary>
		public string Tag { get; set; }

		public bool HasWonGame { get; private set; }


		public bool HasBeenDefeated 
		{
			get
			{
				if (!IsCompetitivePlayer)
				{
					return false; //Not competing player (observer, civilians, neutral units, etc) cannot be defeated
				}
				if (_hasBeenDefeated) //already determined earlier
				{
					return true;
				}
                //_HasBeenDefeated = CheckForVictoryConditions();
                //foreach (var condset in DefeatConditionSets)
                //{
                //    if (condset.HasConditionBeenMet)
                //    {
                //        _HasBeenDefeated = true;
                //        AIHandler.GameLossOrVictoryAchieved();
                //        GameManager.Instance.Game.PlayerHasLostGame(this);
                //        return _HasBeenDefeated;
                //    }
                //}
                //if (_HasBeenDefeated)
                //{
                //    AIHandler.GameLossOrVictoryAchieved();
                //}
				return _hasBeenDefeated;
			}
			set
			{
                if (value != _hasBeenDefeated && value == true)
                {
                    _hasBeenDefeated = true;
                    AIHandler.GameLossOrVictoryAchieved();
                }
                else
                {
                    _hasBeenDefeated = value;
                }
				
			}
		}

		#endregion

		#region "Public methods"

		/// <summary>
		/// Sends an object to the corresponding player client over tcp/ip. Ignored if player
		/// has no connection.
		/// </summary>
		/// <param name="obj"></param>
		public void Send(IMarshallable obj)
		{
			if (TcpPlayerIndex == 0)
			{
				return;
			}

			GameManager.Instance.SendToClient(TcpPlayerIndex, obj);
		}


		/// <summary>
		/// Handles a message sent from a player to one or more other players.
		/// </summary>
		/// <param name="req"></param>
		public void HandleSendMessageToPlayers(GameControlRequest req)
		{
			if (req == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(req.Id)) //message to several players
			{
				var players = GetPlayersByCriteria(req.SendMessageTo);
				players.Remove(this); //do not send message to self
				foreach (var p in players)
				{
					var msg = p.CreateNewMessage(req.ControlParameterString);
					msg.FromPlayer = this;
					msg.Priority = GameConstants.Priority.Urgent;
				}
			}
			else //message to one player
			{
				var player = GameManager.Instance.Game.GetPlayerById(req.Id);
				if (player != null)
				{
					var msg = player.CreateNewMessage(req.ControlParameterString);
					msg.FromPlayer = this;
					msg.Priority = GameConstants.Priority.Urgent;
				}
			}
		}
		/// <summary>
		/// Accepts a SendMessageTo enum parameterm and returns a List of Player including
		/// those players in the game that corresponds to the parameter.
		/// </summary>
		/// <param name="sendMessageTo"></param>
		/// <returns></returns>
		public List<Player> GetPlayersByCriteria(GameConstants.SendMessageTo sendMessageTo)
		{
			var players = from p in GameManager.Instance.Game.Players
						  where p.Id != this.Id
						  select p;
			switch (sendMessageTo)
			{
				case GameConstants.SendMessageTo.All:
					//as above
					break;
				case GameConstants.SendMessageTo.Allies:
					players = from p in GameManager.Instance.Game.Players
							  where this.IsAlly(p)
							  select p;
					break;
				case GameConstants.SendMessageTo.Enemies:
					players = from p in GameManager.Instance.Game.Players
							  where this.IsEnemy(p)
							  select p;
					break;
				case GameConstants.SendMessageTo.CompetetivePlayers:
					players = from p in GameManager.Instance.Game.Players
							  where p.IsCompetitivePlayer
							  select p;

					break;
				case GameConstants.SendMessageTo.Observers:
					players = from p in GameManager.Instance.Game.Players
							  where !p.IsCompetitivePlayer && !p.IsComputerPlayer
							  select p;
					break;
				default:
					break;
			}
			return players.ToList<Player>();
		}

		/// <summary>
		/// Handles a request and sends back a list of units in an area (within radius from specified center)
		/// in a GameStateInfo object.
		/// </summary>
		/// <param name="req"></param>
		public void HandleGetAllUnitsInArea(GameControlRequest req)
		{
			if (req.Position == null || req.ControlParameterValue <= 0.0)
			{
				return;
			}
            var gameStateInfo = new GameStateInfo(GameConstants.GameStateInfoType.UnitsWithinArea, string.Empty);
			gameStateInfo.Position = req.Position;
			gameStateInfo.RadiusM = req.ControlParameterValue;
			var units = GetAllUnitsInArea(new Coordinate(gameStateInfo.Position.Latitude, gameStateInfo.Position.Longitude), gameStateInfo.RadiusM);
			foreach (var u in units)
			{
				if (!u.IsMarkedForDeletion)
				{ 
					gameStateInfo.IdList.Add(u.Id);
				}
			}
			Send(gameStateInfo);
		}

        public void HandleTriggerSignal(GameControlRequest req)
        {
            GameManager.Instance.Log.LogDebug("Player->HandleTriggerSignal: received signal with ParameterString=" + req.ControlParameterString);
            var triggers = from t in this.EventTriggers
                           where (t.TimeElapsedSec == 0 || t.TimeElapsedSec >= GameManager.Instance.Game.GameWorldTimeSec) 
                           && t.EventTriggerType == GameConstants.EventTriggerType.TriggeredBySignalFromGui
                           select t;
            //bool hasBeenHandled = false;
            foreach (var tr in triggers)
            {
                if (tr.Id == req.ControlParameterString)
                {
                    ExecuteEventTriggers(tr);
                    //hasBeenHandled = true;
                }   
            }
            //if (!hasBeenHandled)
            //{
            //    GameManager.Instance.Log.LogWarning("HandleTriggerSignal received signal but there is no triggers of type TriggeredBySignalFromGui. Parameter =" 
            //        + req.ControlParameterString);    
            //}
        }


		/// <summary>
		/// Handles all orders and requests from game client to the game engine.
		/// </summary>
		/// <param name="dataReceived"></param>
		public void HandleMessageFromClient(IMarshallable dataReceived)
		{
			//TODO: Handle all orders from client!

			switch (dataReceived.ObjectTypeToken)
			{
				case CommsMarshaller.ObjectTokens.NoObject:
					break;
				case CommsMarshaller.ObjectTokens.Enq:
					break;
				case CommsMarshaller.ObjectTokens.Ack:
					break;
				case CommsMarshaller.ObjectTokens.ClientInfoRequest:
					ClientInfoRequest infoReq = dataReceived as ClientInfoRequest;
					if (infoReq != null)
					{
						HandleInfoRequest(infoReq);
					}

					break;

				case CommsMarshaller.ObjectTokens.GameControlRequest:
					GameManager.Instance.Game.HandleGameControlMessage(dataReceived, this);
					break;

				case CommsMarshaller.ObjectTokens.MessageString:
					break;
				case CommsMarshaller.ObjectTokens.GameInfo:
					break;
				case CommsMarshaller.ObjectTokens.PlayerInfo:
					break;
				case CommsMarshaller.ObjectTokens.PositionInfo:
					break;
				case CommsMarshaller.ObjectTokens.BaseUnitInfo:
					break;
				case CommsMarshaller.ObjectTokens.OrderInfo:
					break;
				case CommsMarshaller.ObjectTokens.UnitOrder:
					HandleUnitOrderFromClient(dataReceived);
					break;
				case CommsMarshaller.ObjectTokens.UnitMovementOrder:
					HandleUnitMovementOrder(dataReceived);
					break;
				case CommsMarshaller.ObjectTokens.UnitEngagementOrder:
					UnitEngagementOrder unitOrder = dataReceived as UnitEngagementOrder;
					
					if (unitOrder != null)
					{
						BaseUnit unitToExecuteOrder = GetUnitById(unitOrder.Id);
						if (unitToExecuteOrder != null)
						{
							if (unitToExecuteOrder.IsCommunicationJammingCurrentlyInEffect())
							{
                                SendCommunicationJammingMessageToPlayer(unitToExecuteOrder);
								return;
							}
							EngagementOrder engagementOrder = new EngagementOrder();
							engagementOrder.Priority = GameConstants.Priority.Critical;
							if (unitOrder.IsTargetAGroup)
							{
								engagementOrder.TargetDetectedGroup = GetDetectedGroupById(unitOrder.TargetId);

							}
							else if (unitOrder.Position != null && !string.IsNullOrEmpty(unitOrder.WeaponClassID))
							{
								engagementOrder.Position = new Position(unitOrder.Position);
							}
							else
							{
								engagementOrder.TargetDetectedUnit = GetDetectedUnitById(unitOrder.TargetId);
								if (engagementOrder.TargetDetectedUnit == null)
								{
									GameManager.Instance.Log.LogError(string.Format(
										"HandleMessageFromClient: UnitEngagementOrder received for Unit {0} where Target was not found or blank, or there was no position or weaponclass.",
										unitOrder.Id));
									return;
								}
							}
							engagementOrder.WeaponClassId = unitOrder.WeaponClassID;
							engagementOrder.EngagementStrength = unitOrder.EngagementStrength;
							engagementOrder.IsGroupAttack = unitOrder.IsGroupAttack;
							engagementOrder.OwnerPlayer = this;
							engagementOrder.EngagementOrderType = unitOrder.EngagementType;
							engagementOrder.RoundCount = unitOrder.RoundCount;
							engagementOrder.IsComputerGenerated = false;
							//unitToExecuteOrder.Orders.Enqueue(engagementOrder);
							unitToExecuteOrder.Orders.AddFirst(engagementOrder); //Add attack orders from player to top
							GameManager.Instance.Log.LogDebug(string.Format(
								"HandleMessageFromClient: UnitEngagementOrder received for Unit {0} to engage target {1}.",
								unitOrder.Id, unitOrder.TargetId));
						}
					}
					else
					{
						GameManager.Instance.Log.LogError(
							"HandleMessageFromClient: UnitEngagementOrder received for unit "
							+ Id + " which could not be executed.");

					}
					break;
				case CommsMarshaller.ObjectTokens.UnitClass:
					break;
				case CommsMarshaller.ObjectTokens.WeaponClass:
					break;
				case CommsMarshaller.ObjectTokens.SensorClass:
					break;
				case CommsMarshaller.ObjectTokens.HighLevelOrder:
					var hlo = dataReceived as HighLevelOrder;
					if (hlo != null)
					{
						hlo.Id = GameManager.GetUniqueCode();
						AddHighLevelOrder(hlo);
					}
					break;
				default:
					break;
			}
		}



		/// <summary>
		/// Used by many order handlers to inform the player that the order has been suppressed
		/// by enemy jamming.
		/// </summary>
        /// <param name="unit"></param>
        public void SendCommunicationJammingMessageToPlayer(BaseUnit unit)
		{
            //string unitName = "the unit";
            //if (unit != null)
            //{
            //    unitName = unit.ToShortString();
			//}
            Send(new GameStateInfo(GameConstants.GameStateInfoType.CommunicationJammingBlockedOrder, unit.Id));
            //var msg = CreateNewMessage(
            //    string.Format("Sir, your orders do not reach {0}. Communication jamming is in effect.", unitName));
            //msg.Priority = GameConstants.Priority.Urgent;
            //if (unit != null && unit.Position != null)
            //{
            //    msg.Position = unit.Position.Clone();
            //}
		}


		/// <summary>
		/// Adds a new HighLevelOrder to queue for this player. No error or sanity checking is performed.
		/// </summary>
		/// <param name="highLevelOrder"></param>
		public void AddHighLevelOrder(HighLevelOrder highLevelOrder)
		{
			if (highLevelOrder == null)
			{
				return;
			}
			HighLevelOrders.Enqueue(highLevelOrder);
		}

		/// <summary>
		/// Handles all unitmovement type orders.
		/// </summary>
		/// <param name="dataReceived"></param>
		private void HandleUnitMovementOrder(IMarshallable dataReceived)
		{
			var order = (UnitMovementOrder)dataReceived;
			var unit = this.GetUnitById(order.Id);
			if (unit != null)
			{
				if (unit.UnitClass.IsFixed
					|| unit.UnitClass.UnitType == GameConstants.UnitType.BallisticProjectile
					|| unit.UnitClass.UnitType == GameConstants.UnitType.Bomb
					|| unit.UnitClass.UnitType == GameConstants.UnitType.LandInstallation
					|| unit.UnitClass.UnitType == GameConstants.UnitType.Mine
					|| unit.UnitClass.UnitType == GameConstants.UnitType.Missile
					|| unit.UnitClass.UnitType == GameConstants.UnitType.Sonobuoy
					|| unit.UnitClass.UnitType == GameConstants.UnitType.Torpedo)
				{
					GameManager.Instance.Log.LogError(
						"Attempt to send movement order to Unit Id=" + order.Id + " of type " + unit.UnitClass.UnitType + ".");
					return;
				}

                if (unit.IsCommunicationJammingCurrentlyInEffect())
                {
                    SendCommunicationJammingMessageToPlayer(unit);
                    return;
                }

                CheckEventTriggerUnitOrderReceived(unit, order);

                var movementOrder = new MovementOrder();
                foreach (var pos in order.Waypoints)
                {
                    movementOrder.AddWaypoint(pos);
                }
                movementOrder.IsComputerGenerated = false;
                movementOrder.RemoveAllExistingWaypoints = order.RemoveAllExistingWaypoints;
                movementOrder.UnitSpeedType = order.UnitSpeedType;
                movementOrder.IsRecurring = order.IsParameter;

                // When player orders unit to move, remove any existing engagement orders.
                // If unit is group main, then remove from all units in group as well.
                if (unit.Group != null && unit.IsGroupMainUnit())
                {
                    foreach (var groupUnit in unit.Group.Units)
                    {
                        groupUnit.RemoveExistingEngagementOrders();
                    }
                }
                else
                {
                    unit.RemoveExistingEngagementOrders();
                }

                unit.Orders.AddFirst(movementOrder);    //Add move orders from player to top
			}
			else
			{
				GameManager.Instance.Log.LogError(
					"HandleMessageFromServer: UnitOrder received for unit "
					+ Id + " which does not exist.");
			}
		}

		/// <summary>
		/// Sets active weapon orders for a specified unit.
		/// </summary>
		/// <param name="weaponOrders"></param>
		/// <param name="unitId"></param>
		public void SetWeaponOrders(GameConstants.WeaponOrders weaponOrders, string unitId)
		{
			var unit = GetUnitById(unitId);
			if (unit != null)
			{
				unit.WeaponOrders = weaponOrders;
				unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                //var msg = CreateNewMessage(
                //    string.Format("{0} weapon orders set to: {1}", unit.ToShortString(), unit.WeaponOrders));
                //if (unit.Position != null)
                //{
                //    msg.Position = unit.Position.Clone();
                //}
			}
			else
			{ 
				GameManager.Instance.Log.LogError(
					string.Format("SetWeaponOrders: Unit {0} not found.", unitId));
			}
		}

		/// <summary>
		/// Sets global weapon orders, applicable to all new units and launched air. Optionally
		/// also sets new weapon orders to all existing units.
		/// </summary>
		/// <param name="weaponOrders"></param>
		/// <param name="changeForAllUnits"></param>
		public void SetWeaponOrders(GameConstants.WeaponOrders weaponOrders, bool changeForAllUnits)
		{
			this.DefaultWeaponOrders = weaponOrders;
            //var msg = CreateNewMessage(
            //    string.Format("Global weapon orders set to: {0}.", weaponOrders));
			if (changeForAllUnits)
			{
				foreach (var unit in Units)
				{
					if (unit.Position != null)
					{
						unit.WeaponOrders = weaponOrders;
					}
				}
			}
		}
		/// <summary>
		/// Handles all orders received from client that is directed at a specific unit (or group).
		/// </summary>
		/// <param name="dataReceived"></param>
		private void HandleUnitOrderFromClient(IMarshallable dataReceived)
		{
			UnitOrder order = dataReceived as UnitOrder;
			if (order == null)
			{
				return;
			}
			BaseUnit unit = GetUnitById(order.Id);
			if (unit == null)
			{ 
				GameManager.Instance.Log.LogError(string.Format(
					"HandleUnitOrderFromClient error. No unit with Id={0} to handle order {1}", 
					order.Id, order));
				return;
			}
			if(unit.IsCommunicationJammingCurrentlyInEffect())
			{
                SendCommunicationJammingMessageToPlayer(unit);
				return;
			}
			
			CheckEventTriggerUnitOrderReceived(unit, order);
			if (order.UnitOrderType == GameConstants.UnitOrderType.AcquireAmmo )
			{
				AcquireMoreAmmo(order);
				return;
			}
			if (order.UnitOrderType == GameConstants.UnitOrderType.AcquireNewUnit)
			{
				AcquireNewUnit(order);
				return;
			}

			BaseOrder baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(order);
			if (baseOrder != null)
			{
                baseOrder.IsComputerGenerated = false;
                if (baseOrder is EngagementOrder) //make sure new attack order from user is handled directly and not lost in the queue
                {
                    var engageOrder = baseOrder as EngagementOrder;
                    if (engageOrder.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage)
                    {
                        unit.RemoveExistingEngagementOrders();
                    }
                }
                unit.Orders.Enqueue(baseOrder);
			}
		}

		/// <summary>
		/// Player using credits to acquire new unit on airport or other designated carrying unit.
		/// </summary>
		/// <param name="order"></param>
		public bool AcquireNewUnit(UnitOrder order)
		{
			var carrierUnit = GetUnitById(order.Id);
			if (carrierUnit == null)
			{
				GameManager.Instance.Log.LogError("AcquireNewUnit failed: No unit " + order.Id);
				return false;
			}
			if (carrierUnit.AircraftHangar == null || !carrierUnit.UnitClass.IsLandbased)
			{
				GameManager.Instance.Log.LogError(
					string.Format("AcquireNewUnit failed: Unit {0} is not a landbased airfield.", carrierUnit));
				return false;
			}
			var unitClass = GameManager.Instance.GameData.UnitClasses.FirstOrDefault(u => u.Id == order.SecondId);
			if (unitClass == null)
			{
				GameManager.Instance.Log.LogError("AcquireNewUnit failed: No UnitClass " + order.SecondId);
				return false;
			}
			return AcquireNewUnit(carrierUnit, unitClass.Id, order.StringParameter);
		}

		/// <summary>
		///  Player using credits to acquire new unit on airport or other designated carrying unit.
		/// </summary>
		/// <param name="carrierUnit"></param>
		/// <param name="unitClassId"></param>
		/// <param name="weaponLoadName"></param>
		/// <returns></returns>
		public bool  AcquireNewUnit(BaseUnit carrierUnit, string unitClassId, string weaponLoadName)
		{
			if(carrierUnit == null)
			{
				return false;
			}
			var unitClass = GameManager.Instance.GameData.UnitClasses.FirstOrDefault(u => u.Id == unitClassId);
			if (unitClass == null)
			{
				GameManager.Instance.Log.LogError("AcquireNewUnit failed: No UnitClass " + unitClassId);
				return false;
			}
			if (carrierUnit.UnitClass.CarriedRunwayStyle < unitClass.RequiredRunwayStyle)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireUnit, carrierUnit.Id);
                errMsg.UnitClassId = unitClassId;
                Send(errMsg);
				GameManager.Instance.Log.LogError(
                    string.Format("AcquireNewUnit: {0} cannot acquire {1}. Requires {2}.",
					carrierUnit,unitClass.UnitClassShortName, unitClass.RequiredRunwayStyle));
				return false;
			}
			if (AcquirableUnitClasses.Count > 0) //if there is a list of available unit classes, use it
			{
				if (!AcquirableUnitClasses.Contains(unitClassId))
				{
                    var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireUnit, carrierUnit.Id);
                    errMsg.UnitClassId = unitClassId;
                    Send(errMsg);

					GameManager.Instance.Log.LogError(
                        string.Format("AcquireNewUnit: Cannot acquire a {0} in this game.",
						unitClass.UnitClassShortName));
					return false;
				}
			}
			else //otherwise, assume all units for the player's country is available
			{
				if (Country != null && Country.Id != unitClass.CountryId)
				{
                    var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireUnit, carrierUnit.Id);
                    errMsg.UnitClassId = unitClassId;
                    Send(errMsg);

					GameManager.Instance.Log.LogError(
                        string.Format("AcquireNewUnit: Cannot acquire a {0} in this game. Only available for country {1}.",
						unitClass.UnitClassShortName, unitClass.CountryId));
					return false;
 
				}
			}
			if (unitClass.AcquisitionCostCredits > Credits)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireUnit, carrierUnit.Id);
                errMsg.UnitClassId = unitClassId;
                Send(errMsg);

				GameManager.Instance.Log.LogError(
                    string.Format("AcquireNewUnit: Insufficient funds to acquire a {0}. Cost is {1}, and only {2} available now.",
					unitClass.UnitClassShortName, unitClass.AcquisitionCostCredits, Credits));
				return false;
			}
			//ok, now we should be good to go.
			var acqUnit = carrierUnit.AddCarriedUnit(unitClassId);
			if (acqUnit == null)
			{
				GameManager.Instance.Log.LogError(
					string.Format("AcquireNewUnit failed to add new unit {0} to {1}.", unitClass.UnitClassShortName, carrierUnit.Name));
				return false;
			}
			Credits -= unitClass.AcquisitionCostCredits;
			Send(GetPlayerInfo());
			acqUnit.SetWeaponLoad(weaponLoadName);
			var wpnLoads = GameManager.Instance.GameData.GetWeaponLoadByName(unitClassId, acqUnit.CurrentWeaponLoadName);
			if (wpnLoads != null)
			{
				acqUnit.ReadyInSec = wpnLoads.TimeToChangeLoadoutHour * 60.0 * 60.0;
			}
			var timeSp = TimeSpan.FromSeconds(acqUnit.ReadyInSec);
            //var msgOk = CreateNewMessage(
            //    string.Format("A {0} successfully aqcuired. Ready in {1}.",
            //    unitClass.UnitClassShortName, timeSp.ToString()));
            //msgOk.Priority = GameConstants.Priority.Urgent;
			return true;
		}

		/// <summary>
		/// Used to acquire more ammo for a weapon store on a carrier / airfield. Returns errors to user if error or 
		/// insufficient funds.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public bool AcquireMoreAmmo(UnitOrder order)
		{
			var unit = GetUnitById(order.Id);
			if(unit == null)
			{
				GameManager.Instance.Log.LogError(
					string.Format("AcquireMoreAmmo failed. No unit '{0}'.", order.Id));
				return false;
			}
			var weaponClass = GameManager.Instance.GetWeaponClassById(order.SecondId);
			if (weaponClass == null)
			{
				GameManager.Instance.Log.LogError(
					string.Format("AcquireMoreAmmo failed. No WeaponClass '{0}'.", order.SecondId));
				return false;
				
			}
			var store = unit.GetWeaponStoreEntry(weaponClass.Id);
			if (store == null)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireAmmo, unit.Id);
                errMsg.WeaponClassId = weaponClass.Id;
                Send(errMsg);
				GameManager.Instance.Log.LogError(
                    string.Format("AcquireMoreAmmo: Cannot acquire ammo for {0} on {1}. No storage for this weapon.",
					weaponClass.WeaponClassName, unit.ToShortString()));
				return false;
			}
			int count = Convert.ToInt32(order.ValueParameter);
			if (count < 1)
			{
				count = 1;
			}
			return AcquireMoreAmmo(unit, weaponClass, count, true);
		}

		/// <summary>
		/// Used to acquire more ammo for a weapon store on a carrier / airfield. Returns errors to user if error or 
		/// insufficient funds.
		/// </summary>
		/// <param name="carrierUnit"></param>
		/// <param name="weaponClass"></param>
		/// <param name="count"></param>
		/// <param name="reportErrors"></param>
		/// <returns></returns>
		public bool AcquireMoreAmmo(BaseUnit carrierUnit, WeaponClass weaponClass, int count, bool reportErrors)
		{
			if (carrierUnit == null || weaponClass == null || count < 1)
			{
				return false;
			}
			var store = carrierUnit.GetWeaponStoreEntry(weaponClass.Id);
			if (store == null)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireAmmo, carrierUnit.Id);
                errMsg.WeaponClassId = weaponClass.Id;
                Send(errMsg);
				GameManager.Instance.Log.LogError(
                    string.Format("AcquireMoreAmmo: Cannot acquire ammo for {0} on {1}. No storage for this weapon.",
					weaponClass.WeaponClassName, carrierUnit.ToShortString()));
				return false;
			}
			var cost = weaponClass.AcquisitionCostAmmoCredits * count;
			if (cost > Credits)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotAcquireAmmo, carrierUnit.Id);
                errMsg.WeaponClassId = weaponClass.Id;
                Send(errMsg);
				GameManager.Instance.Log.LogError(
                    string.Format("AcquireMoreAmmo: Insufficient funds to acquire {0} rounds of {1} ammo. {2} credits required; {3} available.",
					count, weaponClass.WeaponClassName, cost, Credits));
				return false;
			}
			store.Count += count;
            carrierUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
			Credits -= cost;
			Send(GetPlayerInfo());

			return true;
		}

		/// <summary>
		/// When unit order is received for a specific unit, checks if 
		/// a trigger exists for this event, and executes it if available.
		/// </summary>
		/// <param name="unit"></param>
        /// <param name="unitOrder"></param>
		public void CheckEventTriggerUnitOrderReceived(BaseUnit unit, UnitOrder unitOrder)
		{
            try
            {
                var eventTriggerUnitOrders = from t in EventTriggers
                                             where t.EventTriggerType == GameConstants.EventTriggerType.OrderReceivedFromPlayer
                                             && !t.HasBeenTriggered
                                             && (t.TimeElapsedSec == 0
                                                || t.TimeElapsedSec
                                                >= GameManager.Instance.Game.GameWorldTimeSec)
                                                && t.UnitOrderType == unitOrder.UnitOrderType
                                             select t;
                var tempTriggerList = new List<EventTrigger>();
                foreach (var t in eventTriggerUnitOrders)
                {
                    GameManager.Instance.Log.LogDebug("CheckEventTriggerUnitOrderReceived: Trigger " + t.ToString() + " condition met.");
                    tempTriggerList.Add(t);
                }

                foreach (var t in tempTriggerList)
                {
                    if (!string.IsNullOrEmpty(t.Tag))
                    {
                        if (unit.Tag == t.Tag)
                        {
                            switch ( t.UnitOrderType )
                            {
                                case GameConstants.UnitOrderType.LaunchAircraft:
                                    if (!string.IsNullOrEmpty(t.UnitClassId))
                                    {
                                        if (unitOrder.ParameterList.Select(GetUnitById).Any(launchUnit => launchUnit != null &&
                                                                                                            launchUnit.UnitClass != null && 
                                                                                                            launchUnit.UnitClass.Id == t.UnitClassId))
                                        {
                                            ExecuteEventTriggers(t);
                                        }
                                    }
                                    else
                                    {
                                        ExecuteEventTriggers(t);
                                    }
                                    break;

                                default:
                                    if (!string.IsNullOrEmpty(t.UnitClassId))
                                    {
                                        if (unit.UnitClass != null && unit.UnitClass.Id == t.UnitClassId)
                                        {
                                            ExecuteEventTriggers(t);
                                        }
                                    }
                                    else
                                    {
                                        ExecuteEventTriggers(t);
                                    }
                                    break;
                            }
                        }
                    }
                    else //first order any unit will trigger event
                    {
                        ExecuteEventTriggers(t);
                    }
                }
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError(
                    string.Format("CheckEventTriggerUnitOrderReceived: Error Checking {0}. " + ex.Message,
                    unitOrder.UnitOrderType) );

            }
		}

		/// <summary>
		/// Returns a BaseUnit based on its Id. Also supports "TAG:nn" format in parameter for tags.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public BaseUnit GetUnitById(string id)
		{
			try 
			{
				if (id.Length > TAG_MARKER.Length && id.StartsWith(TAG_MARKER, StringComparison.CurrentCultureIgnoreCase))
				{
					string tag = id.Substring(TAG_MARKER.Length);
					return Units.Single<BaseUnit>(g => g.Tag == tag);
				}
				else
				{
					return Units.Single<BaseUnit>(g => g.Id == id);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Tests whether a specific unit is known to this player, returning true if it
		/// exists in its list of DetectedUnits
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public bool CanCurrentlySeeEnemyUnit(BaseUnit unit)
		{
			if (unit == null)
			{
				return false;
			}
			else
			{
				var detUnit = GetDetectedUnitByUnitId(unit.Id);
				return (detUnit != null);
			}
		}

		/// <summary>
		/// Gets DetectedUnit, if known, based on the Id of the unit it is referring to.
		/// </summary>
		/// <param name="unitId"></param>
		/// <returns></returns>
		public DetectedUnit GetDetectedUnitByUnitId(string unitId)
		{
			try 
			{
				if (unitId.Length > TAG_MARKER.Length && unitId.StartsWith(TAG_MARKER, StringComparison.CurrentCultureIgnoreCase))
				{
					string tag = unitId.Substring(TAG_MARKER.Length);
					return _detectedUnits.Single<DetectedUnit>(g => g.RefersToUnit.Tag == tag);
				}
				else
				{
					return _detectedUnits.Single<DetectedUnit>(g => g.RefersToUnit.Id == unitId);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

        /// <summary>
        /// Get Group from Id.
        /// </summary>
        /// <param name="id">Id of group to get.</param>
        /// <returns></returns>
        public Group GetGroupById(string id)
        {
            try
            {
                return _groups.Find(g => g.Id == id);
            }
            catch (Exception)
            {

                return null; //not found
            }
        }

        public void AddGroup(Group group)
        {
            _groups.Add(group);
        }

        public void RemoveGroup(Group group)
        {
            _groups.Remove(group);
        }

		/// <summary>
		/// Returns DetectedGroup from its Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DetectedGroup GetDetectedGroupById(string id)
		{
			try
			{
				return this.DetectedGroups.Single<DetectedGroup>(g => g.Id == id);
			}
			catch (Exception)
			{
				return null;
			}
			
		}
		/// <summary>
		/// Returns a DetectedUnit based on its Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DetectedUnit GetDetectedUnitById(string id)
		{
			try
			{

                if (!string.IsNullOrEmpty(id) && id.Length > TAG_MARKER.Length && id.StartsWith(TAG_MARKER, StringComparison.CurrentCultureIgnoreCase))
                {
                    string tag = id.Substring(TAG_MARKER.Length);
                    return DetectedUnits.Single<DetectedUnit>(g => g.Tag == tag);
                }
				return DetectedUnits.Single<DetectedUnit>(g => g.Id == id);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a list of BattleDamageReports where this player is the attacking party
		/// </summary>
		/// <returns></returns>
		public IList<BattleDamageReport> BattleReportsAttacking()
		{
			return BattleDamageReports.Where(p => p.PlayerInflictingDamageId 
				== this.Id).ToList<BattleDamageReport>();
		}

		/// <summary>
		/// Returns a list of BattleDamageReports where this player is the defending (attacked) party
		/// </summary>
		/// <returns></returns>
		public IList<BattleDamageReport> BattleReportsDefending()
		{
			return BattleDamageReports.Where(p => p.PlayerSustainingDamageId 
				== this.Id).ToList<BattleDamageReport>();
		}

		/// <summary>
		/// Returns a list of BattleDamageReports where this player's unit with the specificied role has been destroyed
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public IList<BattleDamageReport> BattleReportsLostUnit(TTG.NavalWar.NWComms.GameConstants.Role role)
		{
			return BattleDamageReports.Where(p => p.PlayerSustainingDamageId == this.Id 
				&& p.TargetPlatformRoles.Contains(role)
				&& p.IsTargetPlatformDestroyed).ToList<BattleDamageReport>();
		}

		/// <summary>
		/// Returns a list of BattleDamageReports where this player's unit of the specificied UnitClass has been destroyed
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <returns></returns>
		public IList<BattleDamageReport> BattleReportsLostUnit(string unitClassId)
		{
			return BattleDamageReports.Where(p => p.PlayerSustainingDamageId == this.Id
				&& p.TargetPlatformClassId == unitClassId
				&& p.IsTargetPlatformDestroyed).ToList<BattleDamageReport>();
		}

		/// <summary>
		/// Returns a count of units remaining in the specifid class 
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <returns></returns>
		public int CountUnitsByClass(string unitClassId)
		{
			return Units.Count(u => u.UnitClass.Id == unitClassId 
				&& u.HitPoints > 0 && !u.IsMarkedForDeletion);
		}

		/// <summary>
		/// Returns a count of the units remaining supporting the specified role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public int CountUnitsByRole(TTG.NavalWar.NWComms.GameConstants.Role role)
		{
            return Units.Count(u => u.HitPoints > 0 && !u.IsMarkedForDeletion && u.SupportsRole(role));
		}

		/// <summary>
		/// Returns all the player's units whose position is within the specific radius from the coordinate.
		/// Omits units that are carried.
		/// </summary>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetAllUnitsInArea(Coordinate coordinateCenter, double areaRadiusM)
		{
			return Units.Where<BaseUnit>(u => !u.IsMarkedForDeletion && u.Position != null && u.Position.Coordinate != null 
				&& MapHelper.CalculateDistanceM(coordinateCenter, u.Position.Coordinate) 
				<= areaRadiusM).ToList<BaseUnit>();
		}

		public IList<BaseUnit> GetUnitsInAreaByClassId(string unitClassId, Coordinate coordinateCenter, double areaRadiusM)
		{
			return Units.Where<BaseUnit>(u => !u.IsMarkedForDeletion && u.UnitClass.Id == unitClassId 
				&& MapHelper.CalculateDistanceM(coordinateCenter, 
				u.Position.Coordinate) <= areaRadiusM).ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all of a player's units in the specified area of a certain UnitClass
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetSortedUnitsInAreaByClassId(string unitClassId, Coordinate coordinateCenter, double areaRadiusM)
		{
			var list = from o in Units
					   where !o.IsMarkedForDeletion && o.UnitClass.Id == unitClassId && o.Position != null &&
						  MapHelper.CalculateDistanceM(coordinateCenter, o.Position.Coordinate) <= areaRadiusM
					   orderby MapHelper.CalculateDistanceM(coordinateCenter, o.Position.Coordinate) ascending
					   select o;
			return list.ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all of a player's units in the specified area that meets the specified Role
		/// </summary>
		/// <param name="role"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetUnitsInAreaByRole(TTG.NavalWar.NWComms.GameConstants.Role role,
			Coordinate coordinateCenter, double areaRadiusM)
		{
			return Units.Where<BaseUnit>(u => u.Position != null && !u.IsMarkedForDeletion && u.SupportsRole(role)
				&& MapHelper.CalculateDistanceM(coordinateCenter, 
				u.Position.Coordinate) <= areaRadiusM).ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all of a player's units in the specified area that meets the specified UnitType
		/// </summary>
		/// <param name="unitType"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetUnitsInAreaByUnitType(GameConstants.UnitType unitType,
			Coordinate coordinateCenter, double areaRadiusM)
		{
			return Units.Where(u => !u.IsMarkedForDeletion && u.UnitType == unitType
                && u.Position != null && MapHelper.CalculateDistanceM(coordinateCenter,
				u.Position.Coordinate) <= areaRadiusM).ToList();
		}

		/// <summary>
		/// Returns all of a player's units in the specified area that meets the specified role, sorted by distance to center.
		/// If isAvailableOnly is True, only units with a patrol order are returned.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <param name="isAvailableOnly"></param>
		/// <returns></returns>
		public IOrderedEnumerable<BaseUnit> GetSortedUnitsInAreaByRole(TTG.NavalWar.NWComms.GameConstants.Role role,
			Coordinate coordinateCenter, double areaRadiusM, bool isAvailableOnly)
		{
			var list = from o in Units
					   where !o.IsMarkedForDeletion && o.Position != null && 
                          (!isAvailableOnly || o.MissionType == GameConstants.MissionType.Patrol || o.UnitClass.IsLandbased) &&
                          o.SupportsRole(role) &&
						  MapHelper.CalculateDistanceM(coordinateCenter, o.Position.Coordinate) <= areaRadiusM
                       orderby MapHelper.CalculateDistanceM( coordinateCenter, o.Position.Coordinate ) ascending
                       select o;
            return list;
        }

		/// <summary>
		/// Returns a list of a player's unitclasses that supports the specified Role.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public IList<string> GetActiveUnitClassesByRole(TTG.NavalWar.NWComms.GameConstants.Role role)
		{
			var classList = new List<string>();
			var list = from o in Units
                       where !o.IsMarkedForDeletion && o.SupportsRole(role)
					   select o;
			foreach (var u in list)
			{
				if (!classList.Contains(u.UnitClass.Id))
				{
					classList.Add(u.UnitClass.Id);
				}
			}
			return classList;
		}

        /// <summary>
        /// Returns a list of all detections within a set radius of a map coordinate, sorted by its set ValueScore (highest first).
        /// 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="rangeM"></param>
        /// <returns></returns>
        public IList<DetectedUnit> GetDetectedUnitsInArea(Coordinate coordinate, double rangeM, bool onlyFoes)
        {
            var detectedUnits = from d in this.DetectedUnits
                                where !d.IsMarkedForDeletion && (!onlyFoes || d.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe)
                                && MapHelper.CalculateDistanceRoughM(coordinate, d.Position.Coordinate) <= rangeM
                                orderby d.ValueScore descending
                                select d;
            return detectedUnits.ToList<DetectedUnit>();
        }

		/// <summary>
		/// Returns a list of all units owned by enemy players currently in play, that is, excluding units
		/// that are carried by others (no position) or destroyed.
		/// </summary>
		/// <returns></returns>
		public IList<BaseUnit> GetAllEnemyUnits()
		{
		    return (from enemy in Enemies from unit in enemy.Units where unit.Position != null && 
                                                                            unit.CarriedByUnit == null && 
                                                                            !unit.IsMarkedForDeletion select unit).ToList();
		}

	    /// <summary>
		/// Returns a list of all operational unitclasses for all the player's enemies
		/// that meets specified role.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public IList<string> GetActiveEnemyUnitClassesByRole(GameConstants.Role role)
		{
			var unitList = GetAllEnemyUnits();
			var list = from u in unitList
                       where !u.IsMarkedForDeletion && u.SupportsRole(role)
					   select u;
			var classList = new List<string>();
			foreach (var u in list)
			{
				if (!classList.Contains(u.UnitClass.Id))
				{
					classList.Add(u.UnitClass.Id);
				}
			}
			return classList;
		}

		/// <summary>
		/// Returns a list of all units from all the player's enemies which is of the specified UnitClass
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsByUnitClass(string unitClassId)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where(u => u.UnitClass.Id == unitClassId).ToList<BaseUnit>();

		}

		/// <summary>
		/// Returns a list of all units from all the player's enemies which supports specified Role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsByRole(TTG.NavalWar.NWComms.GameConstants.Role role)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where(u => u.SupportsRole(role)).ToList();
		}

		/// <summary>
		/// Returns a list of all units from all the player's enemies which is within a specific area
		/// </summary>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsInArea(Coordinate coordinateCenter, double areaRadiusM)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where<BaseUnit>(u => 
				MapHelper.CalculateDistanceM(u.Position.Coordinate, coordinateCenter) 
				<= areaRadiusM).ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all enemy units within area that is the specified UnitClass
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsInAreaByUnitClass(string unitClassId, 
			Coordinate coordinateCenter, double areaRadiusM)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where<BaseUnit>(u => u.UnitClass.Id == unitClassId &&
				MapHelper.CalculateDistanceM(u.Position.Coordinate, coordinateCenter)
				<= areaRadiusM).ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all enemy units within area that supports the specified Role
		/// </summary>
		/// <param name="role"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsInAreaByRole(TTG.NavalWar.NWComms.GameConstants.Role role, 
			Coordinate coordinateCenter, double areaRadiusM)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where<BaseUnit>(u => u.SupportsRole(role) &&
				MapHelper.CalculateDistanceM(u.Position.Coordinate, coordinateCenter)
				<= areaRadiusM).ToList<BaseUnit>();
		}

		/// <summary>
		/// Returns all enemy units within area that is the specified UnitType
		/// </summary>
		/// <param name="unitType"></param>
		/// <param name="coordinateCenter"></param>
		/// <param name="areaRadiusM"></param>
		/// <returns></returns>
		public IList<BaseUnit> GetEnemyUnitsInAreaByUnitType(GameConstants.UnitType unitType,
			Coordinate coordinateCenter, double areaRadiusM)
		{
			IList<BaseUnit> unitList = GetAllEnemyUnits();
			return unitList.Where<BaseUnit>(u => u.UnitType == unitType &&
				MapHelper.CalculateDistanceM(u.Position.Coordinate, coordinateCenter)
				<= areaRadiusM).ToList<BaseUnit>();
		}

        /// <summary>
        /// Returns a unit (optionally that is a group MainUnit or unaffiliated) that fulfills 
        /// all roles listed. Nearest unit is returned in case several meets criteria.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="rolesList"></param>
        /// <returns></returns>
        public BaseUnit FindNearestAvailableUnitRole( Coordinate coordinate,
            List<GameConstants.Role> rolesList, string unitClass, bool groupLeadersOnly )
        {
            BaseUnit nearestUnit = null;
            double nearestUnitDistanceM = 100000000;
            foreach ( var unit in Units )
            {
                if ( !unit.IsMarkedForDeletion && !unit.IsOrderedToReturnToBase && unit.Position != null && unit.SupportsRole( rolesList ) )
                {
                    if ( string.IsNullOrEmpty( unitClass ) || unit.UnitClass.Id == unitClass )
                    {
                        if ( !groupLeadersOnly || ( unit.IsGroupMainUnit() || !unit.IsInGroupWithOthers() ) )
                        {
                            double distanceM = MapHelper.CalculateDistance3DM( new Position( coordinate ), unit.Position );
                            if ( nearestUnit == null || nearestUnitDistanceM > distanceM )
                            {
                                nearestUnit = unit;
                                nearestUnitDistanceM = distanceM;
                            }
                        }
                    }
                }
            }
            return nearestUnit;
        }

        public List<BaseUnit> FindAllAvailableUnitRole( Coordinate coordinate,
            List<GameConstants.Role> rolesList, string unitClassId, bool groupLeadersOnly, bool isAvailableOnly )
        {
            var unitList = Units.Where( unit => unit.Position != null && !unit.IsOrderedToReturnToBase &&
                                                    unit.SupportsRole( rolesList ) && ( unit.UnitClass.Id == unitClassId || string.IsNullOrEmpty( unitClassId ) ) &&
                                                    ( !groupLeadersOnly || ( unit.IsGroupMainUnit() || !unit.IsInGroupWithOthers() ) ) &&
                                                    ( !isAvailableOnly || unit.MissionType == GameConstants.MissionType.Patrol ) ).ToList();

            var listSorted = from u in unitList
                             where u.Position != null && u.CarriedByUnit == null
                             orderby MapHelper.CalculateDistance3DM( new Position( coordinate ), u.Position ) ascending
                             select u;

            return listSorted.ToList<BaseUnit>();
        }

		/// <summary>
		/// Sends a BattleDamageReport to client over tcpip (if applicable) and notifies AI.
		/// </summary>
		/// <param name="report"></param>
		public void SendNewBattleDamageReport(BattleDamageReport report)
		{
			GameManager.Instance.Log.LogDebug(string.Format("New BattleDamageReport: {0}", report.ToString()));
			if (IsCompetitivePlayer && !IsComputerPlayer && TcpPlayerIndex > 0)
			{
                //_NewBattleDamageReports.Add(report);
				Send(report);
			}
			if (AIHandler != null)
			{
				AIHandler.BattleDamageReportReceived(report);
			}
		}

		/// <summary>
		/// Initially at game start, makes sure that all players have detected those
		/// units of those unitclasses that IsAlwaysVisibleForEnemy. This mostly applies to
		/// airports and seaports.
		/// </summary>
		public void AddAllAutomaticDetections()
		{
			foreach (Player player in GameManager.Instance.Game.Players)
			{
				if (player.Id != this.Id)
				{
					var units = from u in player.Units
								where u.UnitClass.IsAlwaysVisibleForEnemy && !u.IsMarkedForDeletion
								select u;
					foreach (var unit in units)
					{
						AddAutomaticDetection(unit);
					}
				}
			}
		}

		/// <summary>
		/// Adds specified unit to the DetectedUnits list permanently. Also handles DetectedGroups.
		/// </summary>
		/// <param name="unit"></param>
		public void AddAutomaticDetection(BaseUnit unit)
		{
			if (!DetectedUnits.Exists(u => u.RefersToUnit.Id == unit.Id))
			{
				DetectedUnit det = new DetectedUnit();
				det.RefersToUnit = unit;
				det.IsFixed = true;
				det.IsIdentified = true;
				det.Position = unit.Position;
				det.DetectionClassification = unit.UnitClass.DetectionClassification;
				det.IsAlwaysVisibleForEnemy = true;
                det.OwnerPlayer = unit.OwnerPlayer;
				if (unit.OwnerPlayer.IsEnemy(this))
				{
					det.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
				}
				else if (unit.OwnerPlayer.IsAlly(this))
				{
					det.FriendOrFoeClassification = GameConstants.FriendOrFoe.Friend;
				}
				det.SetDirty(GameConstants.DirtyStatus.NewlyCreated);
				GameManager.Instance.Log.LogDebug(
					string.Format("AddAutomaticDetection added persistent detection {0} for player {1}", 
					det.ToLongString(), this.ToString()));
				DetectedUnits.Add(det);
			}
		}

		/// <summary>
		/// Ensures that contact with DetectedUnits that are not renewed is lost.
		/// </summary>
		/// <param name="timer"></param>
		public void ForgetOldDetections(double deltaGameTimeSec)
		{
            //DateTime forgetTimeThreshold = GameManager.Instance.Game.GameCurrentTime.Subtract(
            //    TimeSpan.FromSeconds(GameConstants.TIME_DETECTION_CONTACT_LOST_SEC));
            var worldTimeForgetOlder = GameManager.Instance.Game.GameWorldTimeSec - GameConstants.TIME_DETECTION_CONTACT_LOST_SEC;
			foreach (var det in DetectedUnits)
			{
				try
				{
					if (GameManager.Instance.Game.GameWorldTimeSec > GameConstants.TIME_DETECTION_CONTACT_LOST_SEC)
					{
						if (!det.IsAlwaysVisibleForEnemy)
						{
							for (int i = det.DetectionSensors.Count - 1; i >= 0; i--)
							{
								var sens = det.DetectionSensors[i];
								if (sens.DetectedWorldGameTimeSec < worldTimeForgetOlder)
								{
									det.DetectionSensors.RemoveAt(i);
									det.SetDirty(GameConstants.DirtyStatus.UnitChanged);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					GameManager.Instance.Log.LogError("ForgetOldDetections error removing DetectionSensor. " + ex.ToString());
				}
				if ((!det.IsAlwaysVisibleForEnemy && det.DetectionSensors.Count == 0)
					|| (det.RefersToUnit != null && det.RefersToUnit.IsMarkedForDeletion))
				{
					det.IsMarkedForDeletion = true;
					AIHandler.DetectionLost(det);
                    //det.RefersToUnit = null;
				}
			}
			foreach (var det in DetectedUnits.Where<DetectedUnit>(d=>d.IsMarkedForDeletion))
			{ 
				if(!string.IsNullOrEmpty(det.DetectedGroupId))
				{
					DetectedGroup detGroup = det.GetDetectedGroup();
					if (detGroup != null)
					{
						detGroup.RemoveUnit(det);
					}
				}
			}
			foreach (var det in DetectedGroups)
			{
				if (det.DetectedUnits.Count < 1)
				{
					det.IsMarkedForDeletion = true;
				}
			}
		}

		/// <summary>
		/// Executes HighLevelOrders at the right time, based on queue.
		/// </summary>
		public void ExecuteHighLevelOrders()
		{
			if (HighLevelOrders.Count > 0)
			{
				var order = HighLevelOrders.Dequeue();
				DateTime currentTime = GameManager.Instance.Game.GameCurrentTime;
				if (order.TriggerNextTime == null) //not initialized with actual trigger time, do that first
				{
					currentTime = currentTime.AddSeconds(order.FirstTriggerInSec);
					order.TriggerNextTime = new NWDateTime(currentTime);
					HighLevelOrders.Enqueue(order); //ready for next time
				}
				else
				{
					if (order.TriggerNextTime.GetDateTime() <= currentTime)
					{
						ExecuteHighLevelOrders(order);
					}
					else
					{
						HighLevelOrders.Enqueue(order);
					}
				}
			}
		}

		/// <summary>
		/// Executes specified HighLevelOrder
		/// </summary>
		/// <param name="order"></param>
		public void ExecuteHighLevelOrders(HighLevelOrder order)
		{ 
			//this method assumes that it is determined that the order is to be executed at this time
			Debug.Assert(this.AIHandler != null, "AIHandler must not be null.");
			if (order.IsForComputerPlayerOnly && !this.IsComputerPlayer)
			{
				return;
			}
			GameManager.Instance.Log.LogDebug(
				string.Format("ExecuteHighLevelOrders for player {0}: Order {1}", 
				ToString(), order.ToString()));
			if (order.GameUiControls != null && order.GameUiControls.Count > 0)
			{
				foreach (var guic in order.GameUiControls)
				{
					Send(guic);
				}
			}
			switch (order.HighLeverOrderType)
			{
				case GameConstants.HighLevelOrderType.SetAswPatrol:
					AIHandler.SetAswAirPatrol(order.Clone());
					break;
				case GameConstants.HighLevelOrderType.SetAewPatrol:
					AIHandler.SetAewAirPatrol(order.Clone());
					break;
				case GameConstants.HighLevelOrderType.LaunchAircraft:
					this.AIHandler.LaunchAircraft(order.Clone());
					break;
				case GameConstants.HighLevelOrderType.SendGameUiControl:
					//already done, above
					break;
				case GameConstants.HighLevelOrderType.TurnOffUnnecessaryActiveSensors:
					if (this.AIHandler != null)
					{ 
						this.AIHandler.TurnOffActiveRadars();
						this.AIHandler.TurnOffActiveSonars();
					}
					break;
				case GameConstants.HighLevelOrderType.EngageLandStructures:
					if (this.AIHandler != null)
					{
						this.AIHandler.EngageSurfaceOrLandTargets(order);
					}
					break;
				case GameConstants.HighLevelOrderType.EngagePrimaryTargets:
					if (this.AIHandler != null)
					{
						this.AIHandler.EngageHighValueTargets();
					}

					break;
				case GameConstants.HighLevelOrderType.EngageSurfaceTargets:
					if (this.AIHandler != null)
					{
						this.AIHandler.EngageSurfaceOrLandTargets(order);
					}
					
					break;
				case GameConstants.HighLevelOrderType.DefendHighValueUnits:
					if (this.AIHandler != null)
					{
						this.AIHandler.DefendHighValueUnits();
					}
					break;
                case GameConstants.HighLevelOrderType.ChangeOrdersOfEngagement:
                    CurrentRulesOfEngagement = order.NewRulesOfEngagement;
                    break;
				default:
					break;
			}
			order.RecurringCount--;
			if (order.RecurringCount > 0)
			{
				DateTime currentTime = new DateTime(GameManager.Instance.Game.GameCurrentTime.Ticks);
				currentTime = currentTime.AddSeconds(order.TriggerEverySec);
				order.TriggerNextTime = new NWDateTime(currentTime);
				HighLevelOrders.Enqueue(order);
			}

		}

        public void PreTick(double deltaGameTimeSec)
        {
            foreach (var group in Groups.Where(group => group.Units.Count > 1))
            {
                group.CheckIfGroupIsStaging();
            }
        }

	    /// <summary>
		/// Main game tick for player. Calls ticks for all units and executes for other functionality.
		/// </summary>
		/// <param name="timer"></param>
		public void Tick(double deltaGameTimeSec)
		{
			// TODO: The whole freaking game!
			//* Sensors must detect enemy units within range and create DetectedUnit objects
			//*   - Expired DetectedUnit objects must be deleted +
			//*   - Player must be notified +
			//*   - For AI especially, newly detected units must be prioritized
			//* Units movement (in progress)
			//*   - Speed and bearing calculated based on ActiveWayPoint, or MovementFormationOrder (or other orders)
			//*   - Unit position updated (done)
			//*   - Projectile units should be checked for impact, damage as appropriate
			//*   - Units under attack should take evasive action
			//* Weapons
			//*   - Units must fire on cleared targets within range, according to Order of Engagement
			//*   - Automatic AA fire a priority
			//* Network
			//*   - Send info on all changed (dirty) objects to tcpip clients ...
			//GameManager.Instance.Log.LogDebug("Player " + ToString() + " tick.");

			ForgetOldDetections(deltaGameTimeSec);

			AIHandler.Tick(GameManager.Instance.Game.GameCurrentTime);

			ExecuteHighLevelOrders();

            if (_timeNextEventTriggerCheck == 0 || _timeNextEventTriggerCheck >= GameManager.Instance.Game.GameWorldTimeSec)
            {
                ExecuteEventTriggers();
                _timeNextEventTriggerCheck = GameManager.Instance.Game.GameWorldTimeSec + GameConstants.TIME_BETWEEN_TRIGGER_CHECKS_SEC;
            }			
		}

        public void SendUpdates()
        {
            int Idx = 0;
            while ( Units.Count > 0 && Idx < Units.Count )
            {
                var unit = Units[ Idx ];
                if ( this.IsCompetitivePlayer && !IsComputerPlayer && !unit.IsCarried && !unit.IsMarkedForDeletion )
                {
                    switch ( unit.DirtySetting )
                    {
                        case GameConstants.DirtyStatus.Clean:
                            break;
                        case GameConstants.DirtyStatus.PositionOnlyChanged:
                            PositionInfo pos = unit.GetPositionInfo();
                            Send( pos );
                            break;
                        case GameConstants.DirtyStatus.UnitChanged:
                        case GameConstants.DirtyStatus.NewlyCreated: //hmm. Should these be treated differently?
                            BaseUnitInfo unitinfo = unit.GetBaseUnitInfo();
                            Send( unitinfo );
                            break;
                        default:
                            break;
                    }
                }
                unit.SetDirty( GameConstants.DirtyStatus.Clean );
                Idx++;
            } //while

            //if ( TcpPlayerIndex > 0 && _NewBattleDamageReports.Count > 0 )
            //{
            //    foreach ( var report in _NewBattleDamageReports )
            //    {
            //        Send( report );
            //    }
            //    _NewBattleDamageReports.Clear();
            //}

            Idx = 0;
            while ( Units.Count > 0 && Idx < Units.Count )
            {
                var unit = Units[ Idx ];
                if ( unit.IsMarkedForDeletion )
                {
                    if ( unit.AircraftHangar != null && unit.AircraftHangar.Aircraft.Count > 0 )
                    {
                        foreach ( var u in unit.AircraftHangar.Aircraft )
                        {
                            u.IsMarkedForDeletion = true;
                        }
                    }
                    if (unit.Group != null)
                    {
                        unit.Group.RemoveUnit(unit);
                    }
                    if ( IsCompetitivePlayer && !IsComputerPlayer && TcpPlayerIndex > 0 && !unit.IsCarried )
                    {
                        GameStateInfo info = new GameStateInfo( GameConstants.GameStateInfoType.UnitIsDestroyed, unit.Id );
                        Send( info );
                    }
                    Units.Remove( unit );
                }
                else
                {
                    Idx++;
                }
            } //while

            foreach ( var group in Groups )
            {
                if ( group.DirtySetting != GameConstants.DirtyStatus.Clean && !group.IsMarkedForDeletion )
                {
                    Send( group.GetGroupInfo() );
                    group.DirtySetting = GameConstants.DirtyStatus.Clean;
                }
            }

            if ( IsCompetitivePlayer && !IsComputerPlayer )
            {
                foreach ( DetectedUnit detUnit in DetectedUnits )
                {
                    if ( detUnit.IsMarkedForDeletion )
                    {
                        GameStateInfo info = null;
                        if ( detUnit.KnownDamagePercent >= 100 )
                        {
                            info = new GameStateInfo(
                                   GameConstants.GameStateInfoType.DetectedContactIsDestroyed, detUnit.Id );
                        }
                        else
                        {
                            info = new GameStateInfo(
                                   GameConstants.GameStateInfoType.DetectedContactIsLost, detUnit.Id );
                        }
                        Send( info );
                    }
                    else if ( detUnit.DirtySetting == GameConstants.DirtyStatus.PositionOnlyChanged )
                    {
                        Send( detUnit.GetPositionInfo() );
                        detUnit.SetDirty( GameConstants.DirtyStatus.Clean );
                    }
                    else if ( detUnit.DirtySetting != GameConstants.DirtyStatus.Clean )
                    {
                        Send( detUnit.GetDetectedUnitInfo() );
                        detUnit.SetDirty( GameConstants.DirtyStatus.Clean );
                    }
                    else
                    {
                        if ( detUnit.RefersToUnit != null && detUnit.IsFixed )
                        {
                            if ( detUnit.RefersToUnit.ActualSpeedKph > 0 )
                            {
                                Send( detUnit.GetPositionInfo() );
                            }
                        }
                    }
                }

                foreach ( var detGroup in DetectedGroups )
                {
                    if ( detGroup.IsMarkedForDeletion )
                    {
                        GameStateInfo info = new GameStateInfo(
                                GameConstants.GameStateInfoType.DetectedContactGroupIsLost, detGroup.Id );
                        Send( info );
                    }
                    else if ( detGroup.DirtySetting != GameConstants.DirtyStatus.Clean )
                    {
                        Send( detGroup.GetDetectedGroupInfo() );
                        detGroup.SetDirty( GameConstants.DirtyStatus.Clean );
                    }
                }
            }

            DetectedUnits.RemoveAll( d => d.IsMarkedForDeletion );
            DetectedGroups.RemoveAll( d => d.IsMarkedForDeletion );

            if ( TcpPlayerIndex > 0 ) //Send unread Messages to player client
            {
                if ( MessageToPlayer.Count > 0 )
                {
                    foreach ( var msg in GetAllNewMessages() )
                    {
                        msg.HasBeenSentToClient = true;
                        msg.IsRead = true;
                        Send( msg.GetMessageInfo() );
                    }
                }

            }
        }

		/// <summary>
		/// Executes the eventtriggers when conditions are met. Deletes executed triggers to improve performance.
		/// </summary>
		public void ExecuteEventTriggers()
		{
			if (EventTriggers == null || EventTriggers.Count == 0)
			{
				return;
			}
            if (!GameManager.Instance.Game.IsGamePlayStarted || !GameManager.Instance.Game.IsGameLoopStarted)
            {
                return;
            }
            bool hasAnyChangesOccurred = false;
			var triggerList = from t in this.EventTriggers
							  where !t.HasBeenTriggered && (t.TimeElapsedSec == 0 || t.TimeElapsedSec 
								<= GameManager.Instance.Game.GameWorldTimeSec)
							  select t;
            var tempTriggerList = triggerList.ToList();
		    foreach (var t in tempTriggerList.Where(IsEventTriggerConditionMet))
		    {
		        hasAnyChangesOccurred = true;
		        ExecuteEventTriggers(t);
		    }
            //Remove all triggers that have been met, except those that are listed as a Player Objective
			try
			{
				this.EventTriggers.RemoveAll(t => t.HasBeenTriggered && !t.IsPlayerObjective);
                if (hasAnyChangesOccurred)
                {
                    //GameManager.Instance.Game.SendGameInfoToAllPlayers();
                    CheckForVictoryConditions();
                }
			}
			catch (Exception)
			{
				//ignore
			}
		}

        /// <summary>
        /// Checks if victory conditions have been met for current player. Executes necessary functions to notify all players 
        /// if true. Returns bool.
        /// </summary>
        /// <returns></returns>
        public bool CheckForVictoryConditions()
        {
            if (this.HasWonGame)
            {
                return true;
            }
            if (this._hasBeenDefeated)
            {
                return false;
            }
            var vicCond = from v in this.EventTriggers
                          where (v is PlayerObjective) &&  (v as PlayerObjective).IsVictoryCondition
                          select v;
            var vicCondExclusive = from v in this.EventTriggers
                          where (v is PlayerObjective) &&  (v as PlayerObjective).IsVictoryCondition && (v as PlayerObjective).IsExclusiveVictoryCondition
                          select v;
            var vicCondMet = from v in this.EventTriggers
                             where (v is PlayerObjective) && (v as PlayerObjective).IsVictoryCondition && v.HasBeenTriggered
                             select v;
            var vicCondExclusiveMet = from v in this.EventTriggers
                                      where (v is PlayerObjective) && (v as PlayerObjective).IsVictoryCondition && (v as PlayerObjective).IsExclusiveVictoryCondition && v.HasBeenTriggered
                                      select v;
            var countAllVicCon = vicCond.Count<EventTrigger>();
            var countVicConMet = vicCondMet.Count<EventTrigger>();
            var countVicConExclusive = vicCondExclusive.Count<EventTrigger>();
            var countVicConExclsuiveMet = vicCondExclusiveMet.Count<EventTrigger>();
            if ((countAllVicCon > 0 &&  countVicConMet >= countAllVicCon) || (countVicConExclusive > 0 && countVicConExclsuiveMet > 0))
            {
                GameManager.Instance.Game.PlayerHasMetVictoryConditions(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveTargettingForDetectedUnitAfterImpact(MissileUnit missileUnit, bool isTargetHit)
        {
            if (missileUnit.TargetDetectedUnit == null)
            {
                //no idea what unit missile was targetting
                return;
            }
            var targetUnit = missileUnit.TargetDetectedUnit;
            if (isTargetHit)
            {
                var remove1 = targetUnit.TargettingList.RemoveAll( t => t.WeaponFired != null && t.WeaponFired.Id == missileUnit.LaunchWeapon.Id );
                var remove2 = targetUnit.TargettingList.RemoveAll( t => t.UnitTargetting != null && t.UnitTargetting.Id == missileUnit.LaunchPlatform.Id );
                GameManager.Instance.Log.LogDebug(
                    string.Format("RemoveTargettingForDetectedUnitAfterImpact-> Removed {0} from targettinglist after miss on target {1}. {2} still in list.",
                    remove1 + remove2, targetUnit.ToString(), targetUnit.TargettingList.Count));
                
                //hmm. This may make the AI think all missiles have missed, not only one
            }
        }

		/// <summary>
		/// Executes the specified EventTrigger.
		/// </summary>
		/// <param name="eventTrigger"></param>
		public void ExecuteEventTriggers(EventTrigger eventTrigger)
		{
			GameManager.Instance.Log.LogDebug("ExecuteEventTriggers: Trigger " + eventTrigger.ToString());
            try
            {
                if (eventTrigger.GameUiControls != null)
                {
                    foreach (var ui in eventTrigger.GameUiControls)
                    {
                        if (!string.IsNullOrEmpty(ui.Tag)) //tag will be a tag from scenario. Set Id
                        {
                            var unit = GetUnitById("tag:" + ui.Tag);
                            if (unit != null)
                            {
                                ui.Id = unit.Id;
                            }
                        }
                        Send(ui);
                    }
                }
                foreach (var hlo in eventTrigger.HighLevelOrders)
                {
                    if (hlo.IsForComputerPlayerOnly && !this.IsComputerPlayer)
                    {
                        this.HighLevelOrders.Enqueue(hlo);
                    }

                }
                try
                {
                    var enemy = Enemies[0]; //assume one and only one enemy
                    foreach (var hlo in eventTrigger.HighLevelOrdersEnemy)
                    {
                        if (hlo.IsForComputerPlayerOnly && !enemy.IsComputerPlayer)
                        {
                            enemy.HighLevelOrders.Enqueue(hlo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GameManager.Instance.Log.LogError(
                      string.Format("ExecuteEventTriggers: error while executing Enemy HLOs for {0}. Error message: " + ex.Message, eventTrigger.ToString()));
                }
                if (eventTrigger.InnerTriggers != null)
                {
                    foreach (var trigger in eventTrigger.InnerTriggers)
                    {
                        if (trigger.TimeElapsedSec > 0)
                        {
                            trigger.TimeElapsedSec += (long)GameManager.Instance.Game.GameWorldTimeSec;
                        }
                        GameManager.Instance.Log.LogDebug(
                            "ExecuteEventTriggers: Adding inner trigger: " + trigger.ToString());
                        this.EventTriggers.Add(trigger);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError(
                    string.Format("ExecuteEventTriggers: error while executing {0}. Error message: " + ex.Message, eventTrigger.ToString()));
            }
            finally
            {
                eventTrigger.HasBeenTriggered = true;
                if (eventTrigger is PlayerObjective)
                {
                    CheckForVictoryConditions();
                }
                Send(GetPlayerInfo());
            }
			
		}

		/// <summary>
		/// Returns true if specified EventTrigger's condition is met, otherwise False.
		/// </summary>
		/// <param name="eventTrigger"></param>
		/// <returns></returns>
		public bool IsEventTriggerConditionMet(EventTrigger eventTrigger)
		{
			if (eventTrigger == null || eventTrigger.HasBeenTriggered)
			{
				return false;
			}
            var isConditionMet = false;
            var oldCountCurrent = eventTrigger.CountCurrent;

			switch (eventTrigger.EventTriggerType)
			{
				case GameConstants.EventTriggerType.PlayerUnitInRegion:
					isConditionMet = IsEventTriggerConditionUnitInRegionMet(eventTrigger);
                    break;

				case GameConstants.EventTriggerType.PlayerDetectsEnemy:
					//Handled in AIBaseHandler
					break;
				case GameConstants.EventTriggerType.TimeHasElapsed:
					if(GameManager.Instance.Game.GameWorldTimeSec >= eventTrigger.TimeElapsedSec)
					{
                        isConditionMet = true;
					}
					break;
                case GameConstants.EventTriggerType.UnitTagIsDestroyed:
                    isConditionMet =(Units.Where<BaseUnit>(u => u.Tag == eventTrigger.Tag && !u.IsMarkedForDeletion).Count<BaseUnit>() == 0);
                    break;
                case GameConstants.EventTriggerType.CountUnitClassInSectorEqualsOrHigher:
                    try
                    {
                        var unitsInSector = this.GetUnitsInAreaByClassId(eventTrigger.UnitClassId,
                            new Coordinate(eventTrigger.Position.Latitude, eventTrigger.Position.Longitude),
                            eventTrigger.AreaRadiusM);
                        eventTrigger.CountCurrent = unitsInSector.Count;
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountUnitClassInSectorEqualsOrHigher error " + ex.Message);
                    }
                    break;
                case GameConstants.EventTriggerType.CountUnitRoleInSectorEqualsOrHigher:
                    try
                    {
                        var unitsInSector = this.GetUnitsInAreaByRole(eventTrigger.Role,
                            new Coordinate(eventTrigger.Position.Latitude, eventTrigger.Position.Longitude),
                            eventTrigger.AreaRadiusM);
                        eventTrigger.CountCurrent = unitsInSector.Count;
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountUnitRoleInSectorEqualsOrHigher error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyClassEqualsOrLower:
                    try
                    {
                        var units = this.GetEnemyUnitsByUnitClass(eventTrigger.UnitClassId);
                        eventTrigger.CountCurrent = units.Count;
                        isConditionMet = (eventTrigger.CountCurrent <= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyClassEqualsOrLower error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyUnitClassDestroyedEqualsOrHigher:
                    try
                    {
                        var btdmg = from b in this.BattleDamageReports
                                    where b.IsTargetPlatformDestroyed && b.PlayerInflictingDamageId == this.Id 
                                    && b.TargetPlatformClassId == eventTrigger.UnitClassId
                                    && !b.IsTargetNeutralUnit
                                    select b;
                        eventTrigger.CountCurrent = btdmg.Count<BattleDamageReport>();
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyUnitClassDestroyedEqualsOrHigher error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyUnitRoleEqualsOrLower:
                    try
                    {
                        var units = this.GetEnemyUnitsByRole(eventTrigger.Role);
                        eventTrigger.CountCurrent = units.Count;
                        isConditionMet = (eventTrigger.CountCurrent <= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyUnitRoleEqualsOrLower error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyRoleDestroyedEqualsOrHigher:
                    try
                    {
                        var btdmg = from b in this.BattleDamageReports
                                    where b.IsTargetPlatformDestroyed && b.PlayerInflictingDamageId == this.Id 
                                    && b.TargetPlatformRoles != null && b.TargetPlatformRoles.Contains(eventTrigger.Role)
                                    && !b.IsTargetNeutralUnit
                                    select b;
                        eventTrigger.CountCurrent = btdmg.Count<BattleDamageReport>();
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyRoleDestroyedEqualsOrHigher error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyDestroyedNeutralUnits:
                    try
                    {
                        var enemyPlayer = this.Enemies[0]; //hmm: note that this must be changed for more than 2 players
                        var btdmg = from b in enemyPlayer.BattleDamageReports
                                    where b.IsTargetPlatformDestroyed && b.PlayerInflictingDamageId == enemyPlayer.Id && b.IsTargetNeutralUnit
                                    select b;
                        eventTrigger.CountCurrent = btdmg.Count<BattleDamageReport>();
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyCivilianUnitsDestroyed error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.CountEnemyDestroyedCivilianUnits:
                    try
                    {
                        var enemyPlayer = this.Enemies[0]; //hmm: note that this must be changed for more than 2 players
                        var btdmg = from b in enemyPlayer.BattleDamageReports
                                    where b.IsTargetPlatformDestroyed && b.PlayerInflictingDamageId == enemyPlayer.Id && b.IsTargetCivilianUnit
                                    select b;
                        eventTrigger.CountCurrent = btdmg.Count<BattleDamageReport>();
                        isConditionMet = (eventTrigger.CountCurrent >= eventTrigger.CountDesired);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet CountEnemyCivilianUnitsDestroyed error " + ex.Message);
                    }
                    break;

                case GameConstants.EventTriggerType.EnemyUnitTagIsDestroyed:
                    try
                    {
                        var units = from u in this.GetAllEnemyUnits()
                                    where u.Tag == eventTrigger.Tag
                                    select u;
                        eventTrigger.CountCurrent = units.Count<BaseUnit>();
                        isConditionMet = (eventTrigger.CountCurrent < 1);
                    }
                    catch (Exception ex)
                    {
                        GameManager.Instance.Log.LogError("IsEventTriggerConditionMet EnemyUnitTagIsDestroyed error " + ex.Message);
                    }
                    break;

				default:
					break;
			}
            if (!isConditionMet && oldCountCurrent != eventTrigger.CountCurrent)
            {
                Send(GetPlayerInfo()); //victory conditions will have been updated
                foreach (var player in this.Enemies)
                {
                    if (player.TcpPlayerIndex > 0)
                    {
                        player.Send(player.GetPlayerInfo());
                    }
                }
            }
			return isConditionMet;
		}

		/// <summary>
		/// Tests if eventTrigger of type PlayerUnitInRegion is met. 
		/// </summary>
		/// <param name="eventTrigger"></param>
		/// <returns></returns>
		private bool IsEventTriggerConditionUnitInRegionMet(EventTrigger eventTrigger)
		{
			if (eventTrigger == null 
				|| eventTrigger.EventTriggerType != GameConstants.EventTriggerType.PlayerUnitInRegion
				|| eventTrigger.Region == null)
			{
				return false;
			}
			Region region = new Region(eventTrigger.Region);
			if (!string.IsNullOrEmpty(eventTrigger.Tag))
			{
				var unit = GetUnitById("tag:" + eventTrigger.Tag);
				if (unit != null && unit.Position != null)
				{
					return region.IsWithinRegion(unit.Position.Coordinate);
				}
			}
			else //no specific unit set
			{
			    return (from unit in Units where unit.Position != null && unit.CarriedByUnit == null 
                        where string.IsNullOrEmpty(eventTrigger.UnitClassId) || unit.UnitClass.Id == eventTrigger.UnitClassId 
                        where eventTrigger.Role == GameConstants.Role.NoOrAnyRole || unit.SupportsRole(eventTrigger.Role) 
                        select (region.IsWithinRegion(unit.Position.Coordinate))).FirstOrDefault();
			}
			return false;
		}

		/// <summary>
		/// Called when this player has achieved victory in current game.
		/// </summary>
		public virtual void GameVictoryAchieved()
		{
            GameManager.Instance.Log.LogDebug(string.Format("GameVictoryAchieved: Player {0} has achived victory", ToString()));
			HasWonGame = true;
            if (AIHandler != null)
            {
                AIHandler.GameLossOrVictoryAchieved();    
            }

            // Update played campaign scenario for local user
            if ( this.IsAdministrator
                && GameManager.Instance.Game.CurrentCampaign != null
                && GameManager.Instance.Game.CurrentGameScenario != null
                && this.CurrentUser != null )
            {
                CurrentUser.UserPlayedScenarios.RemoveAll(
                    u => u.ScenarioId == GameManager.Instance.Game.CurrentGameScenario.Id
                        && u.CampaignId == GameManager.Instance.Game.CurrentCampaign.Id ); //remove old entries
                CurrentUser.UserPlayedScenarios.Add(
                    new TTG.NavalWar.NWComms.NonCommEntities.UserPlayedScenario()
                    {
                        CampaignId = GameManager.Instance.Game.CurrentCampaign.Id,
                        ScenarioId = GameManager.Instance.Game.CurrentGameScenario.Id,
                    } );
                GameManager.Instance.GameData.SaveUpdatedUserData(CurrentUser);
            }
			//GameManager.Instance.CreateNewMessage(this, "Congratulations. You have won!", GameConstants.Priority.Urgent);

		}

		/// <summary>
		/// Creates a new Message to this player.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public Message CreateNewMessage(string message)
		{
			return GameManager.Instance.CreateNewMessage(this, message, GameConstants.Priority.Normal);
		}

		/// <summary>
		/// Creates a new message to specified player.
		/// </summary>
		/// <param name="toPlayer"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public Message CreateNewMessage(Player toPlayer, string message)
		{
			return GameManager.Instance.CreateNewMessage(toPlayer, this, message, GameConstants.Priority.Elevated);
		}


		/// <summary>
		/// Returns a list of all unread messages to this player.
		/// </summary>
		/// <returns></returns>
		public List<Message> GetAllNewMessages()
		{
			return _messageToPlayer.FindAll(m => m.IsRead == false);
		}

		/// <summary>
		/// Returns true if supplied player is an enemy of this player.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool IsEnemy(Player player)
		{
			if (!player.IsCompetitivePlayer || !this.IsCompetitivePlayer)
			{
				return false;
			}
			if (Enemies.Find(p => p.Id == player.Id) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns true if supplied player is an ally of this player.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool IsAlly(Player player)
		{
			if (Allies.Find(p => p.Id == player.Id) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public List<PlayerObjective> GetPlayerObjectives()
        {
            var playerObjectives = (from trigger in EventTriggers where trigger.IsPlayerObjective select trigger as PlayerObjective).ToList();

            foreach (var player in this.Enemies)
            {
                foreach (var trigger in player.EventTriggers)
                {
                    if ((trigger is PlayerObjective) && (trigger as PlayerObjective).IsVictoryCondition)
                    {
                        var newTrigger = (PlayerObjective)trigger.Clone();
                        if (!string.IsNullOrEmpty(trigger.DescriptionEnemy))
                        {
                            newTrigger.DescriptionPlayer = trigger.DescriptionEnemy;    
                        }
                        newTrigger.IsPassiveObjective = true;
                        playerObjectives.Add(newTrigger);
                    }
                }                
            }
            //Log it:
            //var tempDesc = "";
            //foreach (var plObj  in playerObjectives)
            //{
            //    tempDesc += plObj.ToString() + "\n";
		 
            //}
            //GameManager.Instance.Log.LogDebug("GetPlayerObjectives returns:\n" + tempDesc);
            return playerObjectives;
        }

		/// <summary>
		/// Returns a fully populated PlayerInfo for current player (used to send to client).
		/// </summary>
		/// <returns></returns>
		public PlayerInfo GetPlayerInfo()
		{
			PlayerInfo info = new PlayerInfo();
			info.Id = this.Id;
			info.Name = this.Name;
			info.IsComputerPlayer = this.IsComputerPlayer;
			info.IsCompetitivePlayer = this.IsCompetitivePlayer;
			info.HasPlayerWonGame = this.HasWonGame;
            info.ScenarioPlayerId = this.ScenarioPlayerId;
			info.HasPlayerLostGame = this.HasBeenDefeated;
            info.RequestedTimeCompression = this.RequestedTimeCompression;
			info.IsAllUnknownContactsHostile = this.IsAllUnknownContactsHostile;
			info.IsAutomaticallyEvadingAttacks = this.IsAutomaticallyEvadingAttacks;
			info.IsAutomaticallyRespondingToActiveSensor = this.IsAutomaticallyRespondingToActiveSensor;
			info.DefaultWeaponOrders = this.DefaultWeaponOrders;
			info.HighLevelOrders = this.HighLevelOrders.ToList<HighLevelOrder>();
			info.Credits = Credits;
            info.IsAdministrator = this.IsAdministrator;
            info.IsPlayerPermittedToOpenFire = this.IsPlayerPermittedToOpenFire;
            info.CurrentRulesOfEngagement = this.CurrentRulesOfEngagement;
            info.IsAutomaticallyChangingTimeOnDetection = this.IsAutomaticallyChangingTimeOnDetection;
            info.IsAutomaticallyChangingTimeOnBattleReport = this.IsAutomaticallyChangingTimeOnBattleReport;
            info.IsAutomaticallyChangingTimeOnNoOrder = this.IsAutomaticallyChangingTimeOnNoOrder;
            info.TimeCompressionOnDetection = (float)this.TimeCompressionOnDetection;
            info.TimeCompressionOnBattleReport = (float)this.TimeCompressionOnBattleReport;
            info.TimeCompressionOnNoOrder = (float)this.TimeCompressionOnNoOrder;

            if (this.TcpPlayerIndex > 0)
            {
                info.PlayerObjectives = GetPlayerObjectives();
            }
            //if (this.CurrentUser != null)
            //{
                info.CurrentUser = this.CurrentUser;
                //if (GameManager.Instance.Game.CurrentCampaign != null)
                //{
                    //info.CurrentCampaign = GameManager.Instance.Game.CurrentCampaign;
                    //info.AvailableGameScenarios = GetAvailableGameScenarios();
                    //info.CurrentCampaignId = GameManager.Instance.Game.CurrentCampaign.Id;
                //}

            //}
			return info;
		}

		/// <summary>
		/// Returns a list of all available game scenarios for the current (active) campaign. Takes into account prerequisits
		/// for playing a scenario.
		/// </summary>
		/// <returns></returns>
        //public List<GameScenario> GetAvailableGameScenarios()
        //{
        //    if (this.CurrentUser == null || GameManager.Instance.Game.CurrentCampaign == null)
        //    {
        //        return new List<GameScenario>();
        //    }
        //    var list = new List<GameScenario>();
        //    foreach (var scenario in GameManager.Instance.Game.CurrentCampaign.CampaignScenarios)
        //    {
        //        if (scenario.RequiresCompletedScenarioIds == null || scenario.RequiresCompletedScenarioIds.Count == 0)
        //        {
        //            list.Add(GameManager.Instance.GameData.GetGameScenarioById(scenario.ScenarioId));
        //        }
        //        else
        //        {
        //            try
        //            {
        //                var played =
        //                    CurrentUser.UserPlayedScenarios.FindAll(
        //                        u => u.CampaignId == GameManager.Instance.Game.CurrentCampaign.Id && scenario.RequiresCompletedScenarioIds.Contains(u.ScenarioId));
        //                if (played != null && played.Count == scenario.RequiresCompletedScenarioIds.Count)
        //                {
        //                    list.Add(GameManager.Instance.GameData.GetGameScenarioById(scenario.ScenarioId));
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                //not found. ignore
        //            }
        //        }
        //    }
        //    return list;
        //}

		/// <summary>
		/// Returns the next available vessel name for a unitClassId, based on which names are already used. If none available, returns null.
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <returns></returns>
		public UnitClassVesselName GetNextAvailableVesselName(string unitClassId)
		{
			var units = from u in Units
							  where u.UnitClass.Id == unitClassId
							  select u;
			try
			{
				UnitClass unitClass = GameManager.Instance.GameData.UnitClasses.Single<UnitClass>(c => c.Id == unitClassId);
				if (unitClass == null)
				{
					return null;
				}
				foreach (var vesselname in unitClass.VesselNames)
				{
					bool IsTaken = false;
					foreach (var unit in units)
					{
						if (unit.Name == vesselname.UnitName)
						{
							IsTaken = true;
						}
					}
					if (!IsTaken)
					{
						return vesselname;
					}
				}
				return null;

			}
			catch (Exception)
			{
				return null; //not found
			}
		}

		/// <summary>
		/// Player selects user from Users data.
		/// </summary>
		/// <param name="req"></param>
        public void SelectUser(GameControlRequest req)
        {
            if (string.IsNullOrEmpty(req.Id))
            {
                GameManager.Instance.Log.LogError( "SelectUser: User ID not set." );
            }
            if ( GameManager.Instance.Game != null && GameManager.Instance.Game.IsGamePlayStarted )
            {
                if ( this.TcpPlayerIndex > 0 )
                {
                    GameManager.Instance.Log.LogError( "SelectUser: Cannot select user when gameplay has started." );
                }
                return;
            }

            var user = GameManager.Instance.GameData.GetUserById(req.Id);
            if ( user == null )   // Create user if not found
            {
                user = new User()
                {
                    UserId = req.Id
                };
            }
            if (user != null)
            {
                this.CurrentUser = user;
                GameManager.Instance.Game.SendGameInfoToAllPlayers();
            }
            else
            {
                if (this.TcpPlayerIndex > 0)
                {
                    GameManager.Instance.Log.LogError("SelectUser: User not found: " + req.Id);
                    //Hmm: Error message to frontend?
                }
            }
        }

		/// <summary>
		/// User selects campaign.
		/// </summary>
		/// <param name="req"></param>
		public void SelectCampaign(GameControlRequest req)
		{
			if (GameManager.Instance.Game != null && GameManager.Instance.Game.IsGamePlayStarted)
			{
				if (this.TcpPlayerIndex > 0)
				{
                    GameManager.Instance.Log.LogError("SelectCampaign: Cannot select campaign when gameplay has started.");
				}
				return;
			}
			var campaign = GameManager.Instance.GameData.GetCampaignById(req.Id);
			if (campaign != null)
			{
				GameManager.Instance.Game.CurrentCampaign = campaign;
				GameManager.Instance.Game.SendGameInfoToAllPlayers();
			}
			else
			{
                GameManager.Instance.Log.LogError("SelectCampaign: Campaign not found:" + req.Id);

			}


		}

		/// <summary>
		/// Executes cheat associated with cheat code. Will not be executed in competitive multiplayer.
		/// </summary>
		/// <param name="code"></param>
        /// <param name="unitId"></param>
		public void Cheat(string code, string parameter)
		{
			if (GameManager.Instance.Game.CountNonComputerPlayers > 1)
			{
				foreach (var p in GameManager.Instance.Game.Players)
				{
					if (p.TcpPlayerIndex > 0)
					{
						var msg = p.CreateNewMessage(
							string.Format("Player {0} attempted to CHEAT in comptetitive play: {1}",
							this.ToString(), code));
						msg.Priority = GameConstants.Priority.Urgent;
					}
				}
				return;
			}
			switch (code.ToLower())
			{
				case GameConstants.CHEAT_CODE_REVEAL_ORDERS:
					CheatRevealOrders();
					break;
                case GameConstants.CHEAT_CODE_DETECT_ALL:
                    CheatDetectAll();
                    break;
                case GameConstants.CHEAT_CODE_GIVE_BADASS_AIRCRAFT:
                    CheatGiveBadassAircraft();
                    break;
                case GameConstants.CHEAT_CODE_READY_ALL_AIRCRAFT:
                    CheatReadyAllAircraft();
                    break;
                case GameConstants.CHEAT_CODE_REARM_UNITS:
                    CheatRearmUnits();
                    break;
                case GameConstants.CHEAT_CODE_START_FIRE:
                    CheatStartFire(parameter);
                    break;
                case GameConstants.CHEAT_CODE_STOP_FIRE:
                    CheatStopFire(parameter);
                    break;
                case GameConstants.CHEAT_CODE_WIN_GAME:
                    CheatWinGame();
                    break;
                case GameConstants.CHEAT_CODE_LOSE_GAME:
                    CheatLoseGame();
                    break;
				default:
                    CheatCreateNewUnit(code);
					break;
			}
		}

        private void CheatCreateNewUnit(string unitClassId)
        {
            var isGameStarted = GameManager.Instance.Game.IsGamePlayStarted;
            GameManager.Instance.Game.IsGamePlayStarted = false; //necessary to bypass checks! reset at end
            try
            {
                var unitClass = GameManager.Instance.GameData.UnitClasses.FirstOrDefault<UnitClass>(c => c.Id == unitClassId);
                if (unitClass != null)
                {
                    var pos = GetCheatBestPosition();
                    var deg = GameManager.Instance.GetRandomNumber(360);
                    var distM = GameManager.Instance.GetRandomNumber(5000);
                    pos = pos.Offset(deg, distM);
                    var unit = GameManager.Instance.GameData.CreateUnit(this, new Group(), unitClass.Id, "CHEAT " + unitClass.UnitClassShortName, pos, true);
                    if (unit != null)
                    {
                        switch (unit.DomainType)
                        {
                            case GameConstants.DomainType.Surface:
                                if (TerrainReader.GetHeightM(unit.Position.Coordinate) > 0)
                                {
                                    unit.Position = new Position(new Coordinate(60, 0)); //that's sea
                                    unit.Position.HeightOverSeaLevelM = 0;
                                    unit.Position.BearingDeg = 90;
                                }
                                break;
                            case GameConstants.DomainType.Air:
                                unit.Position.HeightOverSeaLevelM = 1000;
                                unit.UserDefinedElevation = GameConstants.HeightDepthPoints.Low;
                                break;
                            case GameConstants.DomainType.Subsea:
                                if (TerrainReader.GetHeightM(unit.Position.Coordinate) > 0)
                                {
                                    unit.Position = new Position(new Coordinate(60, 0)); //that's sea
                                    unit.Position.BearingDeg = 90;
                                }
                                unit.Position.HeightOverSeaLevelM = -100;
                                unit.UserDefinedElevation = GameConstants.HeightDepthPoints.MediumDepth;
                                break;
                            case GameConstants.DomainType.Land:
                                break;
                            case GameConstants.DomainType.Unknown:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("CheatCreateNewUnit failed. " + ex.Message);

            }
            finally
            {
                GameManager.Instance.Game.IsGamePlayStarted = isGameStarted;
            }
        }

        private void CheatRearmUnits()
        {
            foreach (var unit in Units)
            {
                if (!unit.IsMarkedForDeletion && unit.Position != null)
                {
                    if (unit.MaxRangeCruiseM > 0)
                    {
                        unit.FuelDistanceCoveredSinceRefuelM = 0;
                    }
                    unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    foreach (var wpn in unit.Weapons)
                    {
                        wpn.ReadyInSec = 0;
                        wpn.AmmunitionRemaining = wpn.MaxAmmunition;
                    }
                }
            }
        }

        private void CheatReadyAllAircraft()
        {
            foreach (var unit in Units)
            {
                if (!unit.IsMarkedForDeletion && unit.Position == null && unit.CarriedByUnit != null)
                {
                    unit.ReadyInSec = 0;
                    unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    unit.CarriedByUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }
            }
        }

        private void CheatGiveBadassAircraft()
        {
            if (Units.Count < 1)
	        {
		         return;
	        }
            var isGameStarted = GameManager.Instance.Game.IsGamePlayStarted;
            GameManager.Instance.Game.IsGamePlayStarted = false; //necessary to bypass checks! reset at end
            try
            {
                var pos = GetCheatBestPosition();
                var grpFighters = new Group();
                string unitClassId = "f22";
                if (this.Country != null && this.Country.CountryNameShort.Contains("russia"))
                {
                    unitClassId = "pakfa";
                }
                for (int i = 0; i < 5; i++)
                {
                    var plane = GameManager.Instance.GameData.CreateUnit(this, grpFighters, unitClassId, string.Empty, pos, true);
                    if (plane != null)
                    {
                        plane.Position.HeightOverSeaLevelM = 100;
                        plane.SetHomeToNewCarrier();
                    }
                }
                grpFighters.AutoAssignUnitsToFormation();
                pos = pos.Offset(90, 1000);

                var grpLandAttack = new Group();
                unitClassId = "p8poseidon";
                if (this.Country != null && this.Country.CountryNameShort.Contains("russia"))
                {
                    unitClassId = "tu22m3";
                }
                for (int i = 0; i < 5; i++)
                {
                    var plane = GameManager.Instance.GameData.CreateUnit(this, grpLandAttack, unitClassId, string.Empty, pos, true);
                    if (plane != null)
                    {
                        plane.SetWeaponLoad("Land attack");
                        plane.Position.HeightOverSeaLevelM = 100;
                        plane.SetHomeToNewCarrier();
                    }
                }
                grpLandAttack.AutoAssignUnitsToFormation();

                pos = pos.Offset(150, 3000);
                var grpNavalStrike = new Group();
                unitClassId = "p8poseidon";
                if (this.Country != null && this.Country.CountryNameShort.Contains("russia"))
                {
                    unitClassId = "tu22m3";
                }
                for (int i = 0; i < 5; i++)
                {
                    var plane = GameManager.Instance.GameData.CreateUnit(this, grpNavalStrike, unitClassId, string.Empty, pos, true);
                    if (plane != null)
                    {
                        plane.SetWeaponLoad("Naval strike");
                        plane.Position.HeightOverSeaLevelM = 100;
                        plane.SetHomeToNewCarrier();
                    }
                }
                grpNavalStrike.AutoAssignUnitsToFormation();
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("CheatGiveBadassAircraft failed. " + ex.Message);
            }
            finally
            {
                GameManager.Instance.Game.IsGamePlayStarted = isGameStarted;
            }
        }

        private void CheatStartFire(string unitId)
        {
            var unit = GetUnitById(unitId);
            if (unit != null)
            {
                // Random fire level
                GameConstants.FireLevel fireLevel = (GameConstants.FireLevel)GameManager.Instance.GetRandomNumber(1, 3);
                unit.FireLevel = fireLevel;
                unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
        }

        private void CheatStopFire( string unitId )
        {
            var unit = GetUnitById( unitId );
            if (unit != null && unit.FireLevel != GameConstants.FireLevel.NoFire)
            {
                unit.FireLevel = GameConstants.FireLevel.NoFire;
                unit.SetDirty( GameConstants.DirtyStatus.UnitChanged );
            }
        }

        private void CheatWinGame()
        {
            GameManager.Instance.Game.PlayerHasMetVictoryConditions(this);
            GameManager.Instance.Game.SendGameInfoToAllPlayers();
        }

        private void CheatLoseGame()
        {
            foreach (var p in GameManager.Instance.Game.Players)
            {
                if (p != this)
                {
                    GameManager.Instance.Game.PlayerHasMetVictoryConditions(p);
                    break;
                }
            }
            GameManager.Instance.Game.SendGameInfoToAllPlayers();
        }

        private Position GetCheatBestPosition()
        {
            var pos = new Position(new Coordinate(60, 5));
            var roleList = new List<GameConstants.Role>();
            roleList.Add(GameConstants.Role.IsSurfaceCombattant);
            var mainUnit = this.FindAllAvailableUnitRole(pos.Coordinate, roleList, string.Empty, true, false);
            if (mainUnit.Count == 0)
            {
                roleList.Clear();
                roleList.Add(GameConstants.Role.LaunchFixedWingAircraft);
                mainUnit = this.FindAllAvailableUnitRole(pos.Coordinate, roleList, string.Empty, true, false);
            }
            if (mainUnit.Count > 0)
            {
                pos = mainUnit.FirstOrDefault<BaseUnit>().Position;
            }
            return pos;
        }

        private void CheatDetectAll()
        {
            foreach (var player in GameManager.Instance.Game.Players)
            {
                if (player.Id != this.Id)
                {
                    foreach (var unit in player.Units)
                    {
                        if (unit.Position != null && !unit.IsMarkedForDeletion)
                        {
                            this.AddAutomaticDetection(unit);
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Executes the CHEAT to reveal enemy orders.
		/// </summary>
		public void CheatRevealOrders()
		{
			string feedback = string.Empty;
			foreach (var player in this.Enemies)
			{
				feedback += "\nPLAYER: " + player.ToString() + "\n";
				feedback += "\nDETECTIONS:";
				foreach (var det in player.DetectedUnits)
				{
					feedback += "\n* " + det.ToLongString();
					if (det.RefersToUnit != null)
					{
						feedback += "\n --Refers to " + det.RefersToUnit.ToString();
					}
					foreach (var tar in det.TargettingList)
					{
						feedback += "\n  >> " + tar.ToString();
					}
				}
				feedback += "\n\nAIRCRAFT:";
				foreach (var unit in player.Units)
				{
					if (unit.Position != null && unit.UnitClass.IsAircraft)
					{
						feedback += "\n* Unit: " + unit.ToLongString();
						feedback += "\nMission: " + unit.MissionType + " " + unit.MissionTargetType + "\n";
						if (unit.TargetDetectedUnit != null)
						{
							feedback += "Target " + unit.TargetDetectedUnit.ToString() + "\n";
						}
						if (unit.MovementOrder != null && unit.MovementOrder is MovementFormationOrder)
						{
							feedback += "Formation order. At position: " + unit.IsAtFormationPosition;
						}
						else if (unit.MovementOrder != null)
						{
							feedback+="\n  - Waypoints:";
							if (unit.GetActiveWaypoint() != null)
							{
								feedback += "\n ** Active WP: " + unit.GetActiveWaypoint().ToString() + "\n";
								if (unit.GetActiveWaypoint().Orders.Count > 0)
								{
									foreach (var order in unit.GetActiveWaypoint().Orders)
									{
										feedback += "\n # Order " + order.ToString();
										if (order is EngagementOrder)
										{
											var engOrder = order as EngagementOrder;
											feedback += "  EngagementOrder Target: " + engOrder.TargetDetectedUnit;
										}
									}
								}


							}
							foreach (var wp in unit.MovementOrder.GetWaypoints())
							{
								feedback += "\n" + wp.ToString();
								if (wp.Orders.Count > 0)
								{
									foreach (var order in wp.Orders)
									{
										feedback += "\n # Order " + order.ToString();
										if (order is EngagementOrder)
										{
											var engOrder = order as EngagementOrder;
											feedback += "  EngagementOrder Target: " + engOrder.TargetDetectedUnit;
										}
									}
								}
							}
						}
					}
				}
			}
			MessageString info = new MessageString();
			info.Message = "CHEAT:\n" + feedback;
			Send(info);
		}

		/// <summary>
		/// Forcibly designates a specific contact as friend or foe.
		/// </summary>
		/// <param name="detectedContactId"></param>
		/// <param name="friendOrFoe"></param>
		public void DesignateContactFriendOrFoe(string detectedContactId, GameConstants.FriendOrFoe friendOrFoe)
		{
			var detectedContact = GetDetectedUnitById(detectedContactId);
			if (detectedContact != null)
			{
				if (detectedContact.FriendOrFoeClassification != friendOrFoe)
				{
					detectedContact.FriendOrFoeClassification = friendOrFoe;
					//if (friendOrFoe == GameConstants.FriendOrFoe.Foe)
					//{
                        //var msg = CreateNewMessage(
                        //    string.Format("Contact {0} is now designated as HOSTILE.", detectedContact.ToString()));
                        //if (detectedContact.Position != null)
                        //{
                        //    msg.Position = detectedContact.Position.Clone();
                        //}
					//}
					detectedContact.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    var detGroup = detectedContact.GetDetectedGroup();
                    if (detGroup != null)
                    {
                        detGroup.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    }
				}

			}
		}

		public virtual string ToLongString()
		{
			string temp = ToString();
			temp += string.Format(
				"\nUnit Count: {0}  Detected Unit Count: {1}  IsComputer: {2} IsCompetitive: {3} TcpIndex: {4}", 
				Units.Count, DetectedUnits.Count, IsComputerPlayer, IsCompetitivePlayer, TcpPlayerIndex);
			return temp;
		}

		public override string ToString()
		{
			return string.Format("[{0}] {1}", Id, Name);
		}

		#endregion

		#region "Public static methods"
		
		/// <summary>
		/// Gets player by Id from current game.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Player GetById(string id)
		{
			try
			{
				return GameManager.Instance.Game.Players.Find(p => p.Id == id);
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError(string.Format("Player->GetById failed for id {0}: {1}", id, ex.ToString()));
				return null;
			}
		}
		
		#endregion

		#region "Private methods"

		/// <summary>
		/// Handles ClientInfoRequest received from player client and returns the appropriate data
		/// </summary>
		/// <param name="infoReq"></param>
		private void HandleInfoRequest(ClientInfoRequest infoReq)
		{
			switch (infoReq.RequestObjectType)
			{
				case CommsMarshaller.ObjectTokens.NoObject:
					break;
				case CommsMarshaller.ObjectTokens.Enq:
					GameManager.Instance.SendToClient(TcpPlayerIndex, new AckObject());
					break;
				case CommsMarshaller.ObjectTokens.Ack:
					break;
				case CommsMarshaller.ObjectTokens.ClientInfoRequest:
					break;
				case CommsMarshaller.ObjectTokens.GameControlRequest:
					break;
				case CommsMarshaller.ObjectTokens.MessageString:
					break;
				case CommsMarshaller.ObjectTokens.GameInfo:
					GameInfo info = GameManager.Instance.Game.GetGameInfo();
					GameManager.Instance.SendToClient(TcpPlayerIndex, info);
					break;
				case CommsMarshaller.ObjectTokens.PlayerInfo:
					PlayerInfo pinfo;
					if (string.IsNullOrEmpty(infoReq.Id))
					{
						pinfo = GetPlayerInfo();
						Send(pinfo);
					}
					else
					{
						Player playerToSend = Player.GetById(infoReq.Id);
						if (playerToSend != null)
						{
							Send(playerToSend.GetPlayerInfo());
						}
						else
						{
							Send(new MessageString("There is no player " + infoReq.Id));
						}
					}

					break;
				case CommsMarshaller.ObjectTokens.PositionInfo:
					break;
				case CommsMarshaller.ObjectTokens.BaseUnitInfo:
					BaseUnit unit = GetUnitById(infoReq.Id);
					if (unit != null)
					{
						Send(unit.GetBaseUnitInfo());
					}
					break;
				case CommsMarshaller.ObjectTokens.OrderInfo:
					break;
				case CommsMarshaller.ObjectTokens.UnitMovementOrder:
					break;
				case CommsMarshaller.ObjectTokens.UnitClass:
					break;
				case CommsMarshaller.ObjectTokens.WeaponClass:
					break;
				case CommsMarshaller.ObjectTokens.SensorClass:
					break;
				case CommsMarshaller.ObjectTokens.GameScenario:
					break;
				case CommsMarshaller.ObjectTokens.GameScenarioAlliance:
					break;
				case CommsMarshaller.ObjectTokens.GameScenarioPlayer:
					break;
				case CommsMarshaller.ObjectTokens.GameScenarioGroup:
					break;
				case CommsMarshaller.ObjectTokens.GameScenarioUnit:
					break;
				default:
					break;
			}
		}


		#endregion


    }
}
