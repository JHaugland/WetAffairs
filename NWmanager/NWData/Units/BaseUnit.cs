using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System.Diagnostics;
using TTG.NavalWar.NWData.Ai;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWData.Units
{
	[Serializable]
	public class BaseUnit : BaseMovableObject
	{
		#region "Constants, Private and protected fields"

		private readonly Dictionary<string, BaseComponent> _components = null;
        private readonly List<BaseSensor> _Sensors = new List<BaseSensor>();
        private readonly List<BaseWeapon> _Weapons = new List<BaseWeapon>();
		private readonly FlexQueue<BaseOrder> _Orders = new FlexQueue<BaseOrder>();

		private MovementOrder _MovementOrder = null;
		private WeatherSystem _WeatherSystem = null; //local weather
        private double _ActualSpeedKph;
        private double? _GameWorldTimeSinceLastSweepSec = null;
        private double _GameWorldTimeWeatherReleadSec = 0;
        private double _GameWorldTimeLastTerrainCheck = 0;
        private double _GameWorldTimeNextTerrainCheck = -1;
        private double _ReservedUntilGameWorldTimeSec = 0;

        protected bool _hasSentActiveDetectionMessage = false;
        protected bool _hasCheckedForNearestCarrier = false;

        private int _TerrainHeightAtPosM;
        private int _TerrainHeight10SecForwardM;
        private int _TerrainHeight30SecForwardM;

		private BaseUnit _CarriedByUnit = null;
        private readonly HashSet<GameConstants.Role> _RoleList = new HashSet<GameConstants.Role>();
        private readonly List<DetectedUnit> _DetectedUnitsTargettingThis = null;

        // Used to guard against recursive movement calls doing recursive calls.
	    private bool _isRecursiveMovementFlag = false;

		#endregion

		#region "Constructors"

		public BaseUnit() : base()
		{
			DirtySetting =  GameConstants.DirtyStatus.NewlyCreated;
			ReadyInSec = 0;
			_components = new Dictionary<string, BaseComponent>();
			CarriedWeaponStores = new List<WeaponStoreEntry>();
			_UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
			ExtraordinaryNoiseModifyerPercent = 100;
            _DetectedUnitsTargettingThis = new List<DetectedUnit>();
		    BingoFuelPercent = 0.0;
		    FuelDistanceCoveredSinceRefuelM = 0.0;
		}
		
		#endregion

		#region "Public properties"

		//public virtual string UnitName { get; set; }


        /// <summary>
        /// The terrain height (depth) in meters at the unit's actual position.
        /// </summary>
        public int TerrainHeightAtPosM
        {
            get
            {
                CheckForTerrainUpdates();
                return _TerrainHeightAtPosM;
            }
            set
            {
                _TerrainHeightAtPosM = value;
            }
        }


        /// <summary>
        /// The terrain height (depth) in meters at the position the unit will (normally) be in 10 seconds of world time.
        /// </summary>
        public int TerrainHeight10SecForwardM
        {
            get
            {
                CheckForTerrainUpdates();
                return _TerrainHeight10SecForwardM;
            }
            set
            {
                _TerrainHeight10SecForwardM = value;
            }
        }

        /// <summary>
        /// The terrain height (depth) in meters at the position the unit will (normally) be in 30 seconds of world time.
        /// </summary>
        public int TerrainHeight30SecForwardM
        {
            get
            {
                CheckForTerrainUpdates();
                return _TerrainHeight30SecForwardM;
            }
            set
            {
                _TerrainHeight30SecForwardM = value;
            }
        }

		public string UnitDesignation { get; set; }

		public UnitClass UnitClass { get; set; }

        public GameConstants.UnitType UnitType
        {
            get
            {
                return UnitClass.UnitType;
            }
        }

		public virtual bool IsCarried 
		{
			get
			{
				return (_CarriedByUnit != null);
			}
		}

		/// <summary>
		/// When carried on a unit, returns true if a queued launchorder alreay exists reserving 
		/// this unit for a later launch. Used to avoid AI creating several launch orders with
		/// the same units.
		/// </summary>
		public bool IsReserved
		{
			get
			{
                if (_ReservedUntilGameWorldTimeSec > 0)
                {
                    return (_ReservedUntilGameWorldTimeSec > GameManager.Instance.Game.GameWorldTimeSec);
                }
				if (CarriedByUnit == null)
				{
					return true; //in fact, unit has already taken off
				}
			    return CarriedByUnit.Orders.Where(order => order.OrderType == GameConstants.OrderType.LaunchOrder).Any(order => order.ParameterList.Contains(Id));
			}
		}

		/// <summary>
		/// Used primarily for bingo fuel / return to base. Can also be used to order a unit
		/// to not return to base on bingo fuel!
		/// </summary>
		public bool IsOrderedToReturnToBase { get; set; }

        /// <summary>
        /// Destroying even an enemy unit set as civilian may carry a penalty, depending on victory conditions. Taken from GameScenarioUnit
        /// </summary>
        public bool IsCivilianUnit { get; set; }

		private GameConstants.MissionType _missionType;
		public virtual GameConstants.MissionType MissionType
		{
			get
			{
				if (IsInGroupWithOthers() && !IsGroupMainUnit())
				{
					_missionType = Group.MainUnit.MissionType; //TODO: Try catch
				}
				return _missionType;
			}
			set
			{
				if(_missionType != value)
				{

                    if (IsInGroupWithOthers() && IsGroupMainUnit())
                    {
                        foreach (var u in Group.Units)
                        {
                            u.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                        }
                        Group.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    }
                    else
                    {
                        SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    }
				}
				_missionType = value;
			}
		}

		private GameConstants.MissionTargetType _missionTargetType;
		public virtual GameConstants.MissionTargetType MissionTargetType
		{
			get
			{
				if (IsInGroupWithOthers() && !IsGroupMainUnit())
				{
					_missionTargetType = Group.MainUnit.MissionTargetType; //TODO: Try catch
				}
				return _missionTargetType;
			}
			set
			{
				if (IsInGroupWithOthers() && IsGroupMainUnit() && _missionTargetType != value)
				{
					Group.SetDirty(GameConstants.DirtyStatus.UnitChanged);
				}
				_missionTargetType = value;
			}
		}

		private DetectedUnit _targetDetectedUnit;
		public virtual DetectedUnit TargetDetectedUnit
		{
			get
			{
				if (_targetDetectedUnit != null && _targetDetectedUnit.IsMarkedForDeletion)
				{
					_targetDetectedUnit = null;
				}
                if (_targetDetectedUnit == null && MissionType == GameConstants.MissionType.Attack)
                {
                    MissionType = GameConstants.MissionType.Patrol;
                }
				return _targetDetectedUnit;
			}
			set
			{
				_targetDetectedUnit = value;
				if (_targetDetectedUnit != null)
				{
					MissionType = GameConstants.MissionType.Attack;
					MissionTargetType = GetMissionTargetTypeFromDetectedUnit(_targetDetectedUnit);
				}
				else
				{
					if (MissionType == GameConstants.MissionType.Attack)
					{
						MissionType = GameConstants.MissionType.Patrol;
					}
				}
			}
		}

		public BaseUnit CarriedByUnit 
		{
			get
			{
				return _CarriedByUnit;
			}
			set
			{
				_CarriedByUnit = value;
			}
		}

		private double _MaxSensorDetectionDistanceM;
		public double MaxSensorDetectionDistanceM
		{
			get
			{
				if (_MaxSensorDetectionDistanceM <= 0)
				{
					foreach (var sensor in Sensors)
					{
						if (sensor.SensorClass.MaxRangeM > _MaxSensorDetectionDistanceM)
						{
							_MaxSensorDetectionDistanceM = sensor.SensorClass.MaxRangeM;
						}
					}
				}
				return _MaxSensorDetectionDistanceM;
			}
			set
			{
				_MaxSensorDetectionDistanceM = value;
			}
		}

		/// <summary>
		/// Used by AI: Assigned high level order. Clear at landing.
		/// </summary>
		public HighLevelOrder AssignedHighLevelOrder { get; set; }

		public int HitPoints { get; set; }

		public string FormationPositionId { get; set; }

		/// <summary>
		/// If true, the unit (plane or ship) has lights on, and is visible at much greater distances, 
		/// especially at night. Never on in wartime. Would be set for civilian units at night.
		/// </summary>
		public bool HasLightingOn { get; set; }

		/// <summary>
		/// Current WeaponLoad for this unit
		/// </summary>
		public string CurrentWeaponLoadName { get; set; }

		/// <summary>
		/// Is 0 for clean (unloaded), 1 for loaded and 2 for heavy loaded. Higher values conceivable, but 
		/// not presently implemented. This value is used to calculated MaxCruiseRangeM.
		/// </summary>
		public int LoadLevel { get; set; } 

		/// <summary>
		/// How many levels of load the loaded weapons increases the load
		/// of the unit. Influences range. Set by SetWeaponLoad.
		/// </summary>
		public int LoadIncreasesLoadLevelBy { get; set; }

		/// <summary>
		/// The rate with which this load increases the radar cross section of the unit. 0 is none,
		/// 1 goes from stealth to small, from small to medium, etc. Set by SetWeaponLoad.
		/// </summary>
		public int LoadIncreasesRadarCrossSection { get; set; }

		/// <summary>
		/// The maxium distance unit can cover on full fuel tanks (both ways). Copied from 
		/// UnitClass, but then takes into account weight of weapons, etc. Set by
		/// ReCalculateMaxRange()
		/// </summary>
		public double MaxRangeCruiseM { get; set; }

		/// <summary>
		/// Current cruise range reduction in percent based on LoadLevel and HeightAboveSeaLevelM only,
		/// not taking ActualSpeedKph into account. Set by SetWeaponLoad.
		/// </summary>
		public double CurrentRangeReductionPercent { get; set; }

		/// <summary>
		/// The fuel-usage distance in meters covered by this unit since it was launched. 
		/// May differ from actual distance moved, since aircraft uses as much fuel at
		/// slow speeds as at cruise. This calculated value
		/// is compared to MaxRangeCruiseM/MaxRangeMaxM to determine bingo fuel and fuel 
		/// exhausted.
		/// </summary>
		public double FuelDistanceCoveredSinceRefuelM { get; set; }

		public double FuelDistanceRemainingM
		{
			get
			{
				if (MaxRangeCruiseM > 0)
				{
					var remaining = MaxRangeCruiseM - FuelDistanceCoveredSinceRefuelM;
                    if ( remaining < 0 )
                        remaining = 0;
				    return remaining;
				}
				else
				{
					return double.MaxValue;
				}
			}
		}

        /// <summary>
        /// The current bingo fuel level of the unit.
        /// </summary>
        public double BingoFuelPercent { get; private set; }

		//public virtual double RemainingMovementRangeCruiseM { get; set; }

		public TimeSpan EtaCurrentWaypoint { get; set; }

		public TimeSpan EtaAllWaypoints { get; set; }

		public GameConstants.DirtyStatus DirtySetting { get; set; }

        private GameConstants.WeaponOrders _WeaponOrders = GameConstants.WeaponOrders.FireInSelfDefenceOnly;
		public GameConstants.WeaponOrders WeaponOrders 
        {
            get
            {
                return _WeaponOrders;
            }
            set
            {
                if (_WeaponOrders != value)
                {
                    _WeaponOrders = value;
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }
            }
        }

        //public virtual bool TakesElevationFromTargetWaypoint { get; set; }

		/// <summary>
		/// Used by AI for prioritizing attacks
		/// </summary>
		public int ValueScore { get; set; }

		public bool IsReady 
		{
			get
			{
				return (ReadyInSec == 0);
			}
				
		}

		public double TimeToLiveSec { get; set; }

		public bool HasActiveSonar
		{
			get
			{
			    return Sensors.Any(sensor => sensor.SensorClass.SensorType == GameConstants.SensorType.Sonar && sensor.IsActive && sensor.IsOperational);
			}
		}

		public GameConstants.FireLevel FireLevel { get; set; }

		public double ReadyInSec { get; set; }

		public double ActualSpeedKph 
		{ 
			get
			{
				return _ActualSpeedKph;
			}
			set 
			{
				DesiredSpeedKph = value;
                _GameWorldTimeNextTerrainCheck = 0;
				ResolveUnitAndGroupSpeed();
			}
		}

		private double _ExtraordinaryNoiseModifyerPercent;
		public double ExtraordinaryNoiseModifyerPercent
		{
			get
			{
				if (ExtraOrdinaryNoiseModifyerRemoveInSec <= 0)
				{
					_ExtraordinaryNoiseModifyerPercent = 100;
				}
				return _ExtraordinaryNoiseModifyerPercent;
			}
			set
			{
				_ExtraordinaryNoiseModifyerPercent = value;
			}
		}

		public double ExtraOrdinaryNoiseModifyerRemoveInSec { get; set; }

		public void ResolveUnitAndGroupSpeed()
		{
			if (IsInGroupWithOthers() && (IsGroupMainUnit() || MovementOrder is MovementFormationOrder))
			{
                Debug.Assert(Group != null);
				double speedKph = GetGroupSpeedFromSpeedType(UserDefinedSpeed);
				if (IsGroupMainUnit())
				{
					if (Group.IsStaging)
					{
						DesiredSpeedKph = speedKph * 0.5; //this.GetGroupSpeedFromSpeedType(GameConstants.UnitSpeedType.Slow);
					}
					else
					{
						DesiredSpeedKph = speedKph;
						foreach (var unit in Group.Units)
						{
							if (unit.Id != Id && DomainType == unit.DomainType && unit.MovementOrder != null)
							{
								unit.DesiredSpeedKph = speedKph;
							}
						}
					}
				}
				else //non-main units
				{
					if (DomainType == Group.MainUnit.DomainType) //ignore formation staging for e.g. air units in surface group
					{
						if (Group.IsStaging)
						{
							if (IsAtFormationPositionFlag)
							{
								DesiredSpeedKph = Group.MainUnit.DesiredSpeedKph;
							}
							else
							{
								DesiredSpeedKph = speedKph;
							}
						}
						else
						{
							DesiredSpeedKph = Group.MainUnit.DesiredSpeedKph;
						}
					}
				}
			}
			else //not in a group
			{
				DesiredSpeedKph = this.GetSpeedInKphFromSpeedType(UserDefinedSpeed);
			}
			//if (oldDesiredSpeed != DesiredSpeedKph)
			//{ 
			//    GameManager.Instance.Log.LogDebug(
			//        string.Format("ResolveUnitAndGroupSpeed: Unit {0} desired speed changed from {1} to {2} kph. User speed= {3}",
			//        ToShortString(),oldDesiredSpeed, DesiredSpeedKph, UserDefinedSpeed));
			//}
		}

		public double DesiredSpeedKph { get; set; }

		private GameConstants.UnitSpeedType _UserDefinedSpeed;
		public GameConstants.UnitSpeedType UserDefinedSpeed
		{
			get
			{
				return _UserDefinedSpeed;
			}
			set
			{
                if ( _UserDefinedSpeed != value )
                {
                    _UserDefinedSpeed = value;
                    _GameWorldTimeNextTerrainCheck = 0;
                    SetDirty( GameConstants.DirtyStatus.UnitChanged );
                }
				ResolveUnitAndGroupSpeed();
			}
		}

        private GameConstants.HeightDepthPoints? _UserDefinedElevation;
        public GameConstants.HeightDepthPoints? UserDefinedElevation
        {
            get
            {
                return _UserDefinedElevation;
            }
            set
            {
                if (_UserDefinedElevation != value)
                {
                    _UserDefinedElevation = value;
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    if (_UserDefinedElevation != null)
                    {
                        DesiredHeightOverSeaLevelM =  GetElevationMFromHeightDepthMark(_UserDefinedElevation.Value);
                    }
                }
            }
        }

		public FlexQueue<BaseOrder> Orders 
		{
			get
			{
				return _Orders;
			}
		}

		//public virtual MovementOrders MovementOrders
		//{
		//    get 
		//    {
		//        return _MovementOrders;
		//    }
		//}

		public MovementOrder MovementOrder
		{
			get
			{
				if (_MovementOrder == null)
				{
					_MovementOrder = new MovementOrder();
				}
				return _MovementOrder;
			}
			set
			{
				_MovementOrder = value;
			}
		}

		public double FormationPositionFlagGameTimeSec { get; set; }

		private bool _IsAtFormationPositionFlag;

		/// <summary>
		/// Returns true if unit has a FormationPositionOrder and is at its proper position, 
		/// false otherwise. To reduce overhead, this flag is only automatically recalculated
		/// at certain times.
		/// </summary>
		public bool IsAtFormationPositionFlag
		{
			get
			{
                if (IsMarkedForDeletion || GameManager.Instance.Game == null)
                {
                    return false;
                }
				double gameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
				if (FormationPositionFlagGameTimeSec < 1
                    || gameTimeSec > FormationPositionFlagGameTimeSec + GameConstants.TIME_BETWEEN_FORMATION_POSITION_CHECKS_SEC)
				{
                    FormationPositionFlagGameTimeSec = gameTimeSec;
					_IsAtFormationPositionFlag = IsAtFormationPosition;
				}
				return _IsAtFormationPositionFlag;
			}
			set
			{
                if (_IsAtFormationPositionFlag != value)
                {
                    SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
                }
				_IsAtFormationPositionFlag = value;
                FormationPositionFlagGameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
			}
		}

		/// <summary>
		/// This property calculates positions to determine whether the unit is in its proper 
		/// position. Sets IsAtFormationPositionFlag.
		/// </summary>
		public bool IsAtFormationPosition
		{
			get
			{
                if (this.Position == null)
                {
                    IsAtFormationPositionFlag = false;
                }
				else if (MovementOrder is MovementFormationOrder)
				{
                    var movementFormOrder = MovementOrder as MovementFormationOrder;
                    var wp = movementFormOrder.GetActiveWaypoint();
                    if (wp != null)
                    {
                        double distanceToFormatinPosM = MapHelper.CalculateDistanceM(
                            this.Position.Coordinate, wp.Position.Coordinate);

                        IsAtFormationPositionFlag = (distanceToFormatinPosM <= GameConstants.DISTANCE_TO_FORMATION_POSITION_IS_THERE_M);
                    }
                    else
                    {
                        IsAtFormationPositionFlag = false;
                    }
				}
				else
				{
					IsAtFormationPositionFlag = false;
				}
				return _IsAtFormationPositionFlag;
			}
		}

	    public double MostRecentDistanceToTargetM { get; set; }

		public BaseUnit LaunchedFromUnit { get; set; }

		public List<WeaponStoreEntry> CarriedWeaponStores { get; set; }

		public string GroupId 
		{
			get
			{
                if ( this.Group != null )
                {
                    return this.Group.Id;
                }
                return string.Empty;
			}
		}

        public Group Group { get; set; }

		public BaseOrder ActiveOrder { get; set; }

		public Player OwnerPlayer { get; set; }

		public AircraftHangar AircraftHangar
		{
			get 
			{
				try
				{
					return (AircraftHangar)_components.Values.First<BaseComponent>(h => h is AircraftHangar);
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

        public IList<BaseWeapon> Weapons
		{
			get
			{
                return _Weapons.AsReadOnly();
			}
		}

		public IList<BaseSensor> Sensors
		{
			get
			{
                return _Sensors.AsReadOnly();
			}
		}

        public HashSet<GameConstants.Role> RoleList
		{
			get
			{
				return _RoleList;
			}
		}

		public GameConstants.DomainType DomainType
		{
			get
			{
                return UnitClass.DomainType;
			}
	 
		}
		#endregion

		#region "Public static methods"

		public static BaseUnit GetUnitById(Player ownerPlayer, string unitId)
		{
			return ownerPlayer.Units.Find(p => p.Id == unitId);
		}

		#endregion


		#region "Public methods"

        /// <summary>
        /// The unit will be marked as reserved (that is, not available for launch orders) until the specified timeout has expired.
        /// </summary>
        /// <param name="timeOutInSec"></param>
        public void SetReservation(double timeOutInSec)
        {
            if (GameManager.Instance.Game != null)
            {
                _ReservedUntilGameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec + timeOutInSec;    
            }
        }

        public virtual Waypoint GetActiveWaypoint()
        {
            //If unit has a formation order, its waypoint is first its formation position. If
            //it is already at or very near that position, calculate its waypoint as the group's
            //destination, offset by the formation Offset.
            if (MovementOrder is MovementFormationOrder && IsAtFormationPosition)
            {
                try
                {
                    var movementFormOrder = MovementOrder as MovementFormationOrder;
                    var wp = Group.MainUnit.MovementOrder.GetActiveWaypoint();
                    if (wp != null)
                    {
                        wp = new Waypoint(wp.Position.Offset(movementFormOrder.PositionOffset));
                        return wp;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            // normal MovementOrder
            return (MovementOrder != null ? MovementOrder.GetActiveWaypoint() : null);
        }

		/// <summary>
		/// Causes the unit to emit extraordinary noise for a period of time. Normally called by a missile launch or explosion. 
		/// Extraordinary noise means sonars will more easily detect unit.
		/// </summary>
		/// <param name="multiplierPercent">The value the units noise is multiplied with, as a percentage. 200 means a doubling of the noise.</param>
		/// <param name="timeSec">The time the noise will last</param>
		public void MakeExtraordinaryNoise(double multiplierPercent, double timeSec)
		{
			ExtraOrdinaryNoiseModifyerRemoveInSec = timeSec;
			ExtraordinaryNoiseModifyerPercent = multiplierPercent;
		}

		/// <summary>
		/// Signals to the unit it is being detected by ACTIVE sonar or radar, thus
		/// turning on its own sensors as appropriate if set to do that.
		/// </summary>
		/// <param name="detectionSensor">The (enemy) sensor doing the detection</param>
		public void DetectionWithActiveSensor(BaseSensor detectionSensor, double detectionStrength)
		{
			//Only if unit has sensors of same type
			if (this.UnitClass.IsMissileOrTorpedo)
			{
				return;
			}
			if (!detectionSensor.SensorClass.IsPassiveActiveSensor)
			{
				return;
			}
			var sensors = Sensors.Where<BaseSensor>(s => s.SensorClass.SensorType == detectionSensor.SensorClass.SensorType 
				&& s.IsOperational && s.IsReady);
            if (detectionSensor.SensorClass.SensorType == GameConstants.SensorType.Radar) //radars with no ESM capability cannot counterdetect
            {
                sensors = sensors.Where<BaseSensor>(s => s.SensorClass.IsEsmDetector);
            }
			if (!sensors.Any())
			{
				return;
			}
			//For AESA (Active Electronically Scanned Array) there may not be reverse detection!
			if (detectionSensor.SensorClass.AESAfactorPercent > 0 && detectionSensor.SensorClass.AESAfactorPercent < 100)
			{
				double effectiveStrength = detectionStrength * (detectionSensor.SensorClass.AESAfactorPercent / 100.0);
				if (effectiveStrength < 1.0)
				{
					return;
				}
			}
			if (OwnerPlayer.IsCompetitivePlayer && OwnerPlayer.IsAutomaticallyRespondingToActiveSensor)
			{
				if (detectionSensor.SensorClass.SensorType == GameConstants.SensorType.Radar)
				{
					if (!HasActiveSensors(detectionSensor.SensorClass.SensorType))
					{ 
						this.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
					}
				}
				else if (detectionSensor.SensorClass.SensorType == GameConstants.SensorType.Sonar)
				{
					if (!HasActiveSensors(detectionSensor.SensorClass.SensorType))
					{ 
						this.SetSensorsActivePassive(GameConstants.SensorType.Sonar, true);
					}
				}
			}
			else
			{
				if (!OwnerPlayer.IsCompetitivePlayer || HasActiveSensors(detectionSensor.SensorClass.SensorType) || _hasSentActiveDetectionMessage)
				{
					return;
				}
                DetectedUnit unit = null;
				if (detectionSensor.OwnerUnit != null)
				{ 
					unit = OwnerPlayer.GetDetectedUnitByUnitId(detectionSensor.OwnerUnit.Id);
				}
                var detMsg = new GameStateInfo(GameConstants.GameStateInfoType.UnitDetectedByActiveSensor, this.Id);
                if (unit != null)
                {
                    detMsg.SecondaryId = unit.Id;
                }
                OwnerPlayer.Send(detMsg);
				_hasSentActiveDetectionMessage = true;
			}
		}

		/// <summary>
		/// Tests if this unit has any sensors of the relevant type (
		/// </summary>
		/// <param name="sensorType"></param>
		/// <returns></returns>
		public bool HasActiveSensors(GameConstants.SensorType sensorType)
		{
			var sensors = from s in Sensors
						  where s.SensorClass.SensorType == sensorType &&
						  s.IsActive && s.SensorClass.IsPassiveActiveSensor
						  select s;
			return (sensors.Any());
		}

		/// <summary>
		/// Turn on or off all sensors of a particular type
		/// </summary>
		/// <param name="sensorType">Sensor type, e.g. sonar or radar</param>
		/// <param name="turnOn">True to turn sensor on, False to turn sensor off</param>
		public void SetSensorsActivePassive(GameConstants.SensorType sensorType, bool turnOn)
		{
			foreach (BaseSensor sensor in Sensors)
			{
				if (sensor.SensorClass.IsPassiveActiveSensor 
					&& sensor.SensorClass.SensorType == sensorType)
				{
					if (sensor.IsOperational)
					{
						sensor.IsActive = turnOn;
						SetDirty(GameConstants.DirtyStatus.UnitChanged);
					}
				}
			}
		}

		/// <summary>
		/// Turn on or off sensor as specified in the UnitOrder
		/// </summary>
		/// <param name="order">UnitOrder specifying which sensor to turn on or off</param>
		public void SetSensorsActivePassive(UnitOrder order)
		{
			try
			{
				BaseSensor sensor = Sensors.Single(s => s.Id == order.SecondId);
				if (sensor != null)
				{
					if (sensor.SensorClass.IsPassiveActiveSensor)
					{
						sensor.IsActive = order.IsParameter;
						SetDirty(GameConstants.DirtyStatus.UnitChanged);
					}
				}
			}
			catch (Exception ex)
			{
				GameManager.Instance.Log.LogError("SetSensorsActivePassive(order) failed. " + ex.Message);
			}
		}

		/// <summary>
		/// Returns local weather at position of unit. 
		/// </summary>
		/// <returns></returns>
		public virtual WeatherSystem GetWeatherSystem()
		{
			Coordinate coord;
			if (Position != null)
			{
				coord = Position.Coordinate;
			}
			else if (CarriedByUnit != null && CarriedByUnit.Position != null)
			{
				coord = CarriedByUnit.Position.Coordinate;
			}
			else
			{
				return null;
			}
			if (_WeatherSystem != null)
			{ 
				//Reload on expire
				if (_GameWorldTimeWeatherReleadSec + GameConstants.TIME_BETWEEN_WEATHER_SWEEPS_SEC 
					< GameManager.Instance.Game.GameWorldTimeSec)
				{
					_GameWorldTimeWeatherReleadSec = GameManager.Instance.Game.GameWorldTimeSec;
					_WeatherSystem = GameManager.Instance.GameData.GetWeather(coord);
				}
				return _WeatherSystem;
			}

            _GameWorldTimeWeatherReleadSec = GameManager.Instance.Game.GameWorldTimeSec;
			_WeatherSystem = GameManager.Instance.GameData.GetWeather(coord);
			return _WeatherSystem;
		}

		/// <summary>
		/// This method returns the side area of the object in square meters, very roughly calculated
		/// based on height, width and length, assuming the object is rectangular. 
		/// </summary>
		/// <param name="viewAngle">Angle from which unit is viewed, expressed in 
		/// CardinalPoints (N, SE, W, etc)</param>
		/// <returns></returns>
		public virtual double CalculateSizeInSqM(GameConstants.DirectionCardinalPoints viewAngle)
		{
			Debug.Assert(UnitClass != null, "UnitClass must never be null.");
			switch (viewAngle)
			{
				case GameConstants.DirectionCardinalPoints.S:
				case GameConstants.DirectionCardinalPoints.N:
					return UnitClass.WidthM * UnitClass.HeightM;
				case GameConstants.DirectionCardinalPoints.E:
				case GameConstants.DirectionCardinalPoints.W:
					return UnitClass.LengthM * UnitClass.HeightM;
				case GameConstants.DirectionCardinalPoints.NE:
				case GameConstants.DirectionCardinalPoints.NW:
				case GameConstants.DirectionCardinalPoints.SE:
				case GameConstants.DirectionCardinalPoints.SW:
					return ((UnitClass.LengthM * UnitClass.HeightM) + (UnitClass.WidthM * UnitClass.HeightM)) / 2;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Calculates the apparent size of this unit in ArcSeconds. Used to determine visibility of unit. 
		/// Does not correct for stealth.
		/// </summary>
		/// <param name="viewAngle"></param>
		/// <param name="distanceM"></param>
		/// <returns></returns>
		public virtual double CalculateApparentSizeArcSec(
			GameConstants.DirectionCardinalPoints viewAngle, double distanceM)
		{
			double SizeInSqM = CalculateSizeInSqM(viewAngle);
			double SqrSizeM = Math.Sqrt(SizeInSqM);
			double AngDiamRad = 2 * Math.Atan(0.5 * SqrSizeM / distanceM);
			return AngDiamRad.ToDegreeBearing() * 3600;
		}

		/// <summary>
		/// Calculates the apparent size of unit in ArcSeconds, taking into account stealth degradation 
		/// </summary>
		/// <param name="viewAngle"></param>
		/// <param name="distanceM"></param>
		/// <returns></returns>
		public virtual double CalculateRadarCorrectedSizeArcSec(
			GameConstants.DirectionCardinalPoints viewAngle, double distanceM)
		{
			double SizeArcSec = CalculateApparentSizeArcSec(viewAngle, distanceM);
			if((UnitClass.RadarCrossSectionSize == GameConstants.RadarCrossSectionSize.VeryStealthy))
			{
				SizeArcSec *= 0.1; //hmm
			}
			else if (UnitClass.RadarCrossSectionSize == GameConstants.RadarCrossSectionSize.Stealthy)
			{
				SizeArcSec *= 0.15; //hmm
			}
            else if (UnitClass.RadarCrossSectionSize == GameConstants.RadarCrossSectionSize.Reduced)
            {
                SizeArcSec *= 0.3;
            }
            else if (UnitClass.RadarCrossSectionSize == GameConstants.RadarCrossSectionSize.Large)
            {
                SizeArcSec *= 1.5; //hmm
            }

			return SizeArcSec;
		}

        ///// <summary>
        ///// Returns the unit's radar cross section as a percentage of Large (= 100)
        ///// </summary>
        ///// <returns></returns>
        //public virtual double CalculateRadarCrossSectionPercentageOfLarge()
        //{
        //    return GameManager.Instance.CalculateRadarCrossSectionPercentageOfLarge(
        //        this.GetRadarCrossSectionSize());
        //}

		/// <summary>
		/// When setting ActualSpeedKph, really DesiredSpeedKph is set, 
		/// and the game updates ActualSpeed based on max acceleration. This 
		/// provides a method to set the actual speed in Kph directly. Used for
		/// start of game and initial launch of missiles or planes from catapult.
		/// </summary>
		/// <param name="newSpeedKph"></param>
		public virtual void SetActualSpeed(double newSpeedKph)
		{
            _GameWorldTimeNextTerrainCheck = 0;
			DesiredSpeedKph = newSpeedKph;
			_ActualSpeedKph = newSpeedKph;
		}

        public virtual void ClearAllWaypoints()
        {
            MovementOrder.ClearAllWaypoints();
        }

        public bool IsValidWaypointForUnit(WaypointInfo waypointInfo)
        {
            var pos = new Position(waypointInfo.Position);
            return IsValidWaypointForUnit(pos);
        }

        public bool IsValidWaypointForUnit(Position position)
        {
            if (position == null)
                return false;

            switch (DomainType)
            {
                case GameConstants.DomainType.Surface:
                case GameConstants.DomainType.Subsea:
                    return (TerrainReader.GetHeightM(position.Coordinate) < 0);
                case GameConstants.DomainType.Air:
                    return true;
                case GameConstants.DomainType.Land:
                    return (TerrainReader.GetHeightM(position.Coordinate) > 0);
            }
            return true; //hmm
        }

		/// <summary>
		/// Returns the set group speed in kph from speedType (ie cruise) based on the slowest unit in the group.
		/// </summary>
		/// <param name="speedType">SpeedType</param>
		/// <returns></returns>
		public double GetGroupSpeedFromSpeedType(GameConstants.UnitSpeedType speedType)
		{
			if (IsInGroupWithOthers())
			{
			    return Group.Units.Select(u => u.GetSpeedInKphFromSpeedType(speedType)).Concat(new double[] {double.MaxValue}).Min();
			}
			return GetSpeedInKphFromSpeedType(speedType);
		}

		/// <summary>
		/// Sets unit dirty, deciding what data is sent to the frontend/client. Seeting Clean will clear the dirty setting. Other
		/// settings are only changed if they elevate the dirty setting
		/// </summary>
		/// <param name="newDirtySetting">New dirty setting</param>
		public virtual void SetDirty(GameConstants.DirtyStatus newDirtySetting)
		{
			if(newDirtySetting == GameConstants.DirtyStatus.Clean) //when set clean we mean it
			{
				DirtySetting = GameConstants.DirtyStatus.Clean;
			}  
			else if ((int)newDirtySetting > (int)DirtySetting) //only change dirty setting if it increases dirtyness
			{
				//GameManager.Instance.Log.LogDebug("SetDirty: Unit" + ToShortString() + " set " + newDirtySetting);
				DirtySetting = newDirtySetting;
			}
		}

		/// <summary>
		/// Returns radar cross section size as an enum, accounting for increases caused by weaponload
		/// </summary>
		/// <returns></returns>
		public virtual GameConstants.RadarCrossSectionSize GetRadarCrossSectionSize()
		{
			int size = (int)UnitClass.RadarCrossSectionSize;
			size += LoadIncreasesRadarCrossSection;
			if (size > (int)GameConstants.RadarCrossSectionSize.Large)
			{
				size = (int)GameConstants.RadarCrossSectionSize.Large;
			}
			return (GameConstants.RadarCrossSectionSize)size;
		}

		/// <summary>
		/// Recalculates MaxRange for unit, typically called after change of weaponload.
		/// </summary>
		public virtual void ReCalculateMaxRange()
		{
			MaxRangeCruiseM = UnitClass.MaxRangeCruiseM;
			double deductRangeM =  (CurrentRangeReductionPercent * MaxRangeCruiseM / 100);
			MaxRangeCruiseM -= deductRangeM;
			double increaseWeaponLoadRangePercent = GetCurrentWeaponLoadRangeIncreasePercent(this.CurrentWeaponLoadName);
			MaxRangeCruiseM *= 1.0 + (increaseWeaponLoadRangePercent /100.0) ;
		}

		/// <summary>
		/// Returns how much (in meters) the specified weapon load increases range. Negative for decrease.
		/// </summary>
		/// <param name="weaponLoadName">A string specifying a valid weapon load name for this unit</param>
		/// <returns></returns>
		public int GetCurrentWeaponLoadRangeIncreasePercent(string weaponLoadName)
		{
			var load = GameManager.Instance.GameData.GetWeaponLoadByName(this.UnitClass.Id, weaponLoadName);
			if (load != null)
			{
				return load.IncreasesCruiseRangePercent;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Removes all weapons from unit. Internally called when new weaponload is set.
		/// </summary>
		public virtual void RemoveAllWeapons()
		{
			List<string> toremovelist = new List<string>();
			foreach (BaseWeapon weapon in this.Weapons)
			{
				AddAmmoToWeaponStores(weapon.WeaponClass.Id, weapon.AmmunitionRemaining);
				toremovelist.Add(weapon.Id);
			}
			foreach (var id in toremovelist)
			{
				UnregisterComponent(id);
			}
		}

		/// <summary>
		/// Changes weapon loadout for a unit; setting up ready weapons as specified in the load. 
		/// After gameplay has started, only units on carrier/airport can change weapon load.
		/// The type of load specifies how long it will take before unit is ready after a load change.
		/// Note: If an invalid on empty string is given, the default (first) weaponload will be
		/// loaded.
		/// </summary>
		/// <param name="weaponLoadName">A string specifying a valid weapon load for this unit. </param>
		public virtual void SetWeaponLoad(string weaponLoadName)
		{
			UnitClassWeaponLoads load;
            if (UnitClass.WeaponLoads == null || UnitClass.WeaponLoads.Count == 0)
            {
                RoleList.Clear();
                foreach (GameConstants.Role role in UnitClass.UnitRoles)
                {
                    RoleList.Add(role);
                }
                return;
            }
			if (GameManager.Instance.Game.IsGamePlayStarted)
			{
				if (CarriedByUnit == null)
				{
					GameManager.Instance.Log.LogError(
						string.Format("SetWeaponLoad: Unit {0} cannot change its loadout as it is not on a carrier or airport.", 
						ToShortString()));
					return;
				}
			}
			
			load = GameManager.Instance.GameData.GetWeaponLoadByName(UnitClass.Id, weaponLoadName);
            SetWeaponLoad(load);
		}

        public void SetWeaponLoad(UnitClassWeaponLoads load)
        {
            if (load == null)
            {
				GameManager.Instance.Log.LogError(
					string.Format("SetWeaponLoad: Load was null."));
				return;
            }
            if (!CanChangeToWeaponLoad(load, true))
            {
                return;
            }

            RemoveAllWeapons();
            Debug.Assert(load != null, "Load should not be null.");

            LoadIncreasesRadarCrossSection = load.IncreasesRadarCrossSection;
            LoadIncreasesLoadLevelBy = load.IncreasesLoadRangeByLevels;
            LoadLevel = LoadIncreasesLoadLevelBy; //for now, these are the same.
            RoleList.Clear(); //reload rolelist from default, then add those relevant for this weaponload
            foreach (GameConstants.Role role in UnitClass.UnitRoles)
            {
                RoleList.Add(role);
            }
            if (UnitClass.UnitType == GameConstants.UnitType.SurfaceShip)
            {
                if (!RoleList.Contains(GameConstants.Role.IsSurfaceShip))
                {
                    RoleList.Add(GameConstants.Role.IsSurfaceShip);
                }
            }
            foreach (GameConstants.Role role in this.CreateRolesListFromWeaponLoads(load))
            {
                if (!this.RoleList.Contains(role))
                {
                    this.RoleList.Add(role);
                }
            }
            if (CurrentWeaponLoadName == null)
            {
                CurrentWeaponLoadName = string.Empty;
            }
            if (this.CarriedByUnit != null && load != null)
            {
                if (CurrentWeaponLoadName.Trim() != load.Name.Trim())
                {
                    this.ReadyInSec = load.TimeToChangeLoadoutHour * 60 * 60;
                }
            }
            CurrentWeaponLoadName = load.Name;
            foreach (UnitClassWeaponLoad WeaponLoad in load)
            {
                GameData gd = GameManager.Instance.GameData;
                BaseWeapon weapon = new BaseWeapon();
                WeaponClass WeaponClass = gd.GetWeaponClassById(WeaponLoad.WeaponClassId);
                Debug.Assert(WeaponClass != null, "Invalid weapon class " + WeaponLoad.WeaponClassId);
                weapon.WeaponClass = WeaponClass;
                weapon.Name = WeaponClass.WeaponClassName;
                weapon.OwnerPlayer = this.OwnerPlayer;
                weapon.OwnerUnit = this;
                weapon.AmmunitionRemaining = WeaponLoad.MaxAmmunition;
                weapon.IsOperational = true;
                weapon.IsPrimaryWeapon = WeaponLoad.IsPrimaryWeapon;
                weapon.LastFiredGameWorldTimeSec = 0;
                weapon.MaxAmmunition = WeaponLoad.MaxAmmunition;
                weapon.Name = WeaponClass.WeaponClassName;
                //weapon.WeaponOrders = OwnerPlayer.DefaultWeaponOrders;
                weapon.WeaponBearingDeg = WeaponLoad.WeaponBearingDeg;
                this.RegisterComponent(weapon, weapon.Name);
                AddAmmoToWeaponStores(WeaponLoad.WeaponClassId, (WeaponLoad.MaxAmmunition * -1));
            }
            CurrentRangeReductionPercent = CalculateCurrentRangeReductionPercent();
            ReCalculateMaxRange();
            if (CarriedByUnit != null)
            {
                CarriedByUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
            else
            {
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
        }

        public void SetWeaponLoad(GameConstants.WeaponLoadType weaponLoadType, GameConstants.WeaponLoadModifier weaponLoadModifier)
        {
            UnitClassWeaponLoads load;
            if (UnitClass.WeaponLoads == null || UnitClass.WeaponLoads.Count == 0)
            {
                RoleList.Clear();
                foreach (GameConstants.Role role in UnitClass.UnitRoles)
                {
                    RoleList.Add(role);
                }
                return;
            }
            if (GameManager.Instance.Game.IsGamePlayStarted)
            {
                if (CarriedByUnit == null)
                {
                    GameManager.Instance.Log.LogError(
                        string.Format("SetWeaponLoad: Unit {0} cannot change its loadout as it is not on a carrier or airport.",
                        ToShortString()));
                    return;
                }
            }

            load = GameManager.Instance.GameData.GetWeaponLoadByType(UnitClass.Id, weaponLoadType, weaponLoadModifier);
            SetWeaponLoad(load);

        }

        public List<GameConstants.Role> CreateRolesListFromWeaponLoads(UnitClassWeaponLoads weaponLoads)
        {
            var roleList = new List<GameConstants.Role>();
            switch (weaponLoads.WeaponLoadType)
            {
                case GameConstants.WeaponLoadType.Undefined:
                    break;
                case GameConstants.WeaponLoadType.Ferry:
                    break;
                case GameConstants.WeaponLoadType.AirSuperiority:
                    roleList.Add(GameConstants.Role.InterceptAircraft);
                    roleList.Add(GameConstants.Role.AttackAir);
                    break;
                case GameConstants.WeaponLoadType.Strike:
                    roleList.Add(GameConstants.Role.AttackLand);
                    roleList.Add(GameConstants.Role.AttackSurface);
                    break;
                case GameConstants.WeaponLoadType.NavalStrike:
                    roleList.Add(GameConstants.Role.AttackSurface);
                    break;
                case GameConstants.WeaponLoadType.LandAttack:
                    roleList.Add(GameConstants.Role.AttackLand);
                    break;
                case GameConstants.WeaponLoadType.ASW:
                    roleList.Add(GameConstants.Role.ASW);
                    break;
                case GameConstants.WeaponLoadType.AEW:
                    roleList.Add(GameConstants.Role.AEW);
                    break;
                case GameConstants.WeaponLoadType.DeployMines:
                    roleList.Add(GameConstants.Role.DeployMines);
                    break;
                case GameConstants.WeaponLoadType.CounterMine:
                    roleList.Add(GameConstants.Role.MineCountermeasures);
                    break;
                case GameConstants.WeaponLoadType.ElectronicWarfare:
                    roleList.Add(GameConstants.Role.OffensiveJamming);
                    break;
                case GameConstants.WeaponLoadType.RefuellingPlane:
                    roleList.Add(GameConstants.Role.RefuelAircaft);
                    break;
                case GameConstants.WeaponLoadType.Other:
                    break;
                default:
                    break;
            }
            foreach (var wld in weaponLoads)
            {
                var wpnClass = GameManager.Instance.GetWeaponClassById(wld.WeaponClassId);
                if (wpnClass.CanTargetAir)
                {
                    if (wld.IsPrimaryWeapon && SupportsRole(GameConstants.Role.IsAircraft))
                    {
                        if (!roleList.Contains(GameConstants.Role.AttackAir))
                        {
                            roleList.Add(GameConstants.Role.AttackAir);
                        }
                    }
                }
                if (wpnClass.CanTargetLand)
                {
                    if (wld.IsPrimaryWeapon && SupportsRole(GameConstants.Role.IsAircraft))
                    {
                        if (!roleList.Contains(GameConstants.Role.AttackLand))
                        {
                            roleList.Add(GameConstants.Role.AttackLand);
                        }
                        if (wpnClass.MaxWeaponRangeM >= GameConstants.DEFAULT_AA_DEFENSE_RANGE_M)
                        {
                            if (!roleList.Contains(GameConstants.Role.AttackLandStandoff))
                            {
                                roleList.Add(GameConstants.Role.AttackLandStandoff);
                            }
                        }
                    }
                } 
                if (wpnClass.CanTargetSurface)
                {
                    if (wld.IsPrimaryWeapon && SupportsRole(GameConstants.Role.IsAircraft))
                    {
                        if (!roleList.Contains(GameConstants.Role.AttackSurface))
                        {
                            roleList.Add(GameConstants.Role.AttackSurface);
                        }
                        if (wpnClass.MaxWeaponRangeM >= GameConstants.DEFAULT_AA_DEFENSE_RANGE_M)
                        {
                            if (!roleList.Contains(GameConstants.Role.AttackSurfaceStandoff))
                            {
                                roleList.Add(GameConstants.Role.AttackSurfaceStandoff);
                            }
                        }
                    }
                }
                if (wpnClass.CanTargetSubmarine)
                {
                    if (!roleList.Contains(GameConstants.Role.AttackSubmarine))
                    {
                        roleList.Add(GameConstants.Role.AttackSubmarine);
                    }
                    if (!roleList.Contains(GameConstants.Role.ASW))
                    {
                        roleList.Add(GameConstants.Role.ASW);
                    }
                }
                if (wpnClass.IsNotWeapon)
                {
                    if (wpnClass.SpecialOrders != GameConstants.SpecialOrders.None)
                    {
                        switch (wpnClass.SpecialOrders)
                        {
                            case GameConstants.SpecialOrders.None:
                                break;
                            case GameConstants.SpecialOrders.DropSonobuoy:
                                if (!roleList.Contains(GameConstants.Role.ASW))
                                {
                                    roleList.Add(GameConstants.Role.ASW);
                                }
                                break;
                            case GameConstants.SpecialOrders.DropMine:
                                if (!roleList.Contains(GameConstants.Role.DeployMines))
                                {
                                    roleList.Add(GameConstants.Role.DeployMines);
                                }

                                break;
                            case GameConstants.SpecialOrders.JammerRadarDegradation:
                            case GameConstants.SpecialOrders.JammerCommunicationDegradation:
                            case GameConstants.SpecialOrders.JammerRadarDistortion:
                                if (!roleList.Contains(GameConstants.Role.OffensiveJamming))
                                {
                                    roleList.Add(GameConstants.Role.OffensiveJamming);
                                }
                                break;
                            default:
                                break;
                        } //switch
                    } //if specialorders
                } //if isnotweapon
            } //foreach weaponload
            return roleList;
        }

		public bool CanChangeToWeaponLoad(UnitClassWeaponLoads weaponLoads, bool reportErrorToPlayer)
		{
			if (GameManager.Instance.Game == null || !GameManager.Instance.Game.IsGamePlayStarted)
			{
				return true; //if gameplay has not started, any weapon load change is ok.
			}
            if (weaponLoads == null)
            {
                return false;
            }
			foreach (var weaponload in weaponLoads)
			{
				var store = GetWeaponStoreEntry(weaponload.WeaponClassId);
				if (store != null)
				{
					var maxAmmo = weaponload.MaxAmmunition; //Note: this requires that each aircraft only has one weapon of each type that uses stores
					var weaponClass = GameManager.Instance.GetWeaponClassById(weaponload.WeaponClassId);
					if (maxAmmo == 0)
					{
						maxAmmo = weaponClass.MaxAmmunition;
					}
					if (store.Count < maxAmmo)
					{
						if (reportErrorToPlayer && !OwnerPlayer.IsComputerPlayer)
						{
                            var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.CannotChangeWeaponload, this.Id);
                            errMsg.WeaponClassId = weaponClass.Id;
                            OwnerPlayer.Send(errMsg);
							GameManager.Instance.Log.LogError(
								string.Format("CanChangeToWeaponLoad: Can not change weapon load on {0} to {1}. No more ammunition for {2} in stores.",
								ToShortString(), weaponLoads.Name, weaponClass.WeaponClassName));
						}
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Used to add or remove ammunition from weapon stores for this carrying unit. If unit is carried, operation is carried out
		/// on carrier/airfield unit. Note that if no weapon store exists for this WeaponClassId, nothing happens. 
		/// Use negative number to remove ammo. If store is reduced to below 0, it is trunctated to 0.
		/// </summary>
		/// <param name="weaponClassId"></param>
		/// <param name="count"></param>
		public void AddAmmoToWeaponStores(string weaponClassId, int count)
		{
            //if (GameManager.Instance.Game != null && !GameManager.Instance.Game.IsGamePlayStarted)
            //{
            //    return; //Ignore before game has started
            //}
			var store = GetWeaponStoreEntry(weaponClassId);
			if (store != null)
			{
				store.Count += count;
				if (store.Count < 0)
				{
					store.Count = 0;
				}
			}
		}

		public WeaponStoreEntry GetWeaponStoreEntry(string weaponClassId)
		{
			var unit = this;
			if (this.CarriedByUnit != null)
			{
				unit = this.CarriedByUnit;
			}
			return unit.CarriedWeaponStores.FirstOrDefault(s => s.WeaponClassId == weaponClassId);
		}

		/// <summary>
		/// Adds role for this unit. A role is important for AI usage of this unit.
		/// </summary>
		/// <param name="role"></param>
		public virtual void AddRole(GameConstants.Role role)
		{
			if (!_RoleList.Contains(role))
			{
				_RoleList.Add(role);    
			}
			
		}
		/// <summary>
		/// Removes a role for this unit.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public virtual bool RemoveRole(GameConstants.Role role)
		{
			return _RoleList.Remove(role);
		}

		/// <summary>
		/// Returns true only if this unit supports the specified role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public bool SupportsRole(GameConstants.Role role)
		{
            return _RoleList.Contains(role);
		}

		/// <summary>
		/// Returns true only if both specified roles are supported by this unit.
		/// </summary>
		/// <param name="role1"></param>
		/// <param name="role2"></param>
		/// <returns></returns>
		public bool SupportsRole(GameConstants.Role role1, GameConstants.Role role2)
		{
            return _RoleList.Contains(role1) && _RoleList.Contains(role2); 
		}

		/// <summary>
		/// Takes a list of roles and returns true if all roles in list are supported by the unit,
		/// otherwise false.
		/// </summary>
		/// <param name="roles">List of Role</param>
		/// <returns></returns>
		public bool SupportsRole(List<GameConstants.Role> roles)
		{
			if (roles == null)
			{
				return true;
			}
		    return roles.All(SupportsRole);
		}
		
		/// <summary>
		/// Updates the current speed of the unit if DesiredSpeed is different from the current speed, based on
		/// how much it can accelerate in the elapsed time.
		/// </summary>
		/// <param name="deltaGameTimeSec"></param>
		public virtual void UpdateActualSpeed(double deltaGameTimeSec)
		{
			Debug.Assert(UnitClass != null, "BaseUnit.UpdateActualSpeed: UnitClass should never be null.");

            if (this is MissileUnit)
            {
                var missile = this as MissileUnit;
                if (missile != null
                    && missile.LaunchWeapon != null 
                    && missile.LaunchWeapon.WeaponClass.TerminalSpeedRangeM > 0
                    && DesiredSpeedKph < UnitClass.MaxSpeedKph 
                    && missile.TargetDetectedUnit != null 
                    && !missile.TargetDetectedUnit.IsMarkedForDeletion
                    && missile.TargetDetectedUnit.Position != null)
                {
                    var distanceTargetM = MapHelper.CalculateDistanceApproxM(this.Position.Coordinate, TargetDetectedUnit.Position.Coordinate);
                    if (distanceTargetM <= missile.LaunchWeapon.WeaponClass.TerminalSpeedRangeM)
                    {
                        //SetActualSpeed(missile.LaunchWeapon.WeaponClass.MaxSpeedKph);
                        UserDefinedSpeed = GameConstants.UnitSpeedType.Afterburner;
                    }
                }
            }

            var maxIncreaseKph = deltaGameTimeSec * this.UnitClass.MaxAccelerationKphSec;
            var maxDecreaseKph = deltaGameTimeSec * this.UnitClass.MaxDecelerationKphSec;
            if (maxDecreaseKph > 0) //should be negative, but if it is positive, then flip it
			{
				maxDecreaseKph *= -1;
			}

			DesiredSpeedKph = DesiredSpeedKph.Clamp(UnitClass.MinSpeedKph, UnitClass.MaxSpeedKph);
			
            if (_ActualSpeedKph > DesiredSpeedKph)
			{
				_ActualSpeedKph = _ActualSpeedKph + maxDecreaseKph;
				if (_ActualSpeedKph < DesiredSpeedKph)
				{
					_ActualSpeedKph = DesiredSpeedKph;
				}
				ReCalculateEta();
			}
			else if (_ActualSpeedKph < DesiredSpeedKph)
			{
				_ActualSpeedKph = _ActualSpeedKph + maxIncreaseKph;
				if (_ActualSpeedKph > DesiredSpeedKph)
				{
					_ActualSpeedKph = DesiredSpeedKph;
				}
				ReCalculateEta();
			}
		}

        private double GetTurnRangeDegrSec()
        {
            if (IsInGroupWithOthers() && IsGroupMainUnit() && Group.Units.Any(u => u.IsAtFormationPositionFlag))
            {
                var val = Group.Units.Min(u => u.UnitClass.TurnRangeDegrSec);
                if (UnitClass.UnitType == GameConstants.UnitType.SurfaceShip)
                {
                    val *= .2;
                }
                return val;
            }

            return this.UnitClass.TurnRangeDegrSec;
        }

		/// <summary>
		/// Changes the actual bearing of the unit, if DesiredBearing is different from ActualBearing. 
		/// The change depends on the unitclass' TurnRangeDegrSec and the elapsed game time.
		/// </summary>
		/// <param name="elapsedGameTimeSec"></param>
		public virtual void UpdateActualBearing(double elapsedGameTimeSec)
		{
			Debug.Assert(UnitClass != null, "BaseUnit.UpdateActualSpeed: UnitClass should never be null.");
			if (ActualBearingDeg != null && DesiredBearingDeg != null && ActualBearingDeg != DesiredBearingDeg)
			{
			    var maxChangeRad = (elapsedGameTimeSec * GetTurnRangeDegrSec()).ToRadian();

				DesiredBearingDeg = DesiredBearingDeg.Value.Clamp(0, 360);
				double actualBearingRad = ((double)ActualBearingDeg).ToRadian();
				double desiredBearingRad = ((double)DesiredBearingDeg).ToRadian();
				double differenceRad = desiredBearingRad - actualBearingRad;
				if (Math.Abs(differenceRad) <= maxChangeRad)
				{
					ActualBearingDeg = DesiredBearingDeg;
					return;
				}
				if (differenceRad < 0 && Math.Abs(differenceRad) > Math.PI)
				{
					actualBearingRad += maxChangeRad;
				}
				else if (differenceRad > 0 && Math.Abs(differenceRad) > Math.PI)
				{
					actualBearingRad -= maxChangeRad;
				}
				else if (differenceRad > 0 && Math.Abs(differenceRad) < Math.PI * 2)
				{
					actualBearingRad += maxChangeRad;
				}
				else
				{
					actualBearingRad -= maxChangeRad;
				}
				ActualBearingDeg = actualBearingRad.ToDegreeBearing();
			}
		}

		/// <summary>
		/// Returns true if the unit has sufficient fuel to reach the specified coordinate, 
		/// and return to its base, at cruise speed.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <returns></returns>
		public bool HasEnoughFuelToReachTarget(Coordinate coordinate, bool alsoCalculateReturnToBase)
		{
			return HasEnoughFuelToReachTarget(coordinate, GameConstants.UnitSpeedType.Cruise, alsoCalculateReturnToBase);
		}

		/// <summary>
		/// Returns true if the unit has sufficient fuel to reach the specified coordinate, 
		/// and return to its base, at the designated speed.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <returns></returns>
        public bool HasEnoughFuelToReachTarget(Coordinate coordinate, GameConstants.UnitSpeedType speedType, bool alsoCalculateReturnToBase)
		{
			if (MaxRangeCruiseM <= 0)
			{
				return true;
			}
			if (IsOrderedToReturnToBase)
			{
				return false;
			}
			Position currentPosition = this.Position;
			if (this.Position == null && CarriedByUnit != null && CarriedByUnit.Position != null)
			{
				currentPosition = CarriedByUnit.Position;
			}
            var enduranceModifier = GetFuelEnduranceModifier(speedType);
            var distanceToTargetM = MapHelper.CalculateDistanceM( currentPosition.Coordinate, coordinate ) * enduranceModifier;
			if (distanceToTargetM > FuelDistanceRemainingM)
			{
				return false;
			}
			double distanceToHomeFromTargetM = 0;
			if (alsoCalculateReturnToBase && LaunchedFromUnit != null
				&& !LaunchedFromUnit.IsMarkedForDeletion && LaunchedFromUnit.Position != null)
			{
                distanceToHomeFromTargetM = MapHelper.CalculateDistanceM(coordinate, LaunchedFromUnit.Position.Coordinate) * enduranceModifier;
			}

            var canReachTarget = (distanceToHomeFromTargetM + distanceToTargetM) < FuelDistanceRemainingM;
			return canReachTarget;
		}

		/// <summary>
		/// Returns the percentage of bingo fuel, that is the fuel level needed for the unit (aircraft) to successfully
		/// return to base. At 100, the unit must (and will) return to the base.
		/// </summary>
		/// <returns></returns>
		public double CalculateBingoFuelPercent()
		{
            // MaxRangeCruiseM of 0 means there is no limit.
            // Missiles wont be returning home and never check bingo fuel, so we can avoid doing much calculation.
            // If a unit is carried by another one, it's always at 0 bingo fuel
            if (MaxRangeCruiseM <= 0 || UnitClass.IsMissileOrTorpedo || Position == null)
			{
				return 0.0;
			}
			if (IsOrderedToReturnToBase || FuelDistanceRemainingM <= 0)
			{ 
				return 100.0; //To avoid confusion because of margin of error
			}
		    double distanceToHomeM = GetDistanceToHomeM();
			if (distanceToHomeM == 0)
			{ 
				//TODO: Reallocate new home?
				return 0;
			}

            // Take turn distance into account. We'll account for a full 180 turn.
		    const double turnDeg = 180.0; // MapHelper.CalculateBearingDegrees(Position.Coordinate, LaunchedFromUnit.Position.Coordinate);
            var turnTimeSec = turnDeg / UnitClass.TurnRangeDegrSec;
            
            // Account for unit having to decelerate to cruise after going to bingo fuel
            if (ActualSpeedKph > UnitClass.CruiseSpeedKph)
            {
                var speedDiffKph = ActualSpeedKph - UnitClass.CruiseSpeedKph;
                var decelerationTimeSec = speedDiffKph / Math.Abs( UnitClass.MaxDecelerationKphSec );
                
                var speedKph = ActualSpeedKph;
                while (decelerationTimeSec > 0)
                {
                    var deltaTimeSec = Math.Min(1.0, decelerationTimeSec);
                    var speedInMs = speedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
                    var decelerationDistanceM = speedInMs * deltaTimeSec;

                    // Subtract deceleration distance from distance to home before adding it as the distance itself
                    // has already been accounted for with the pure distance to home.
                    // In other words we're just interested in how much more distance will be added due to fuel consumption during deceleration.
                    distanceToHomeM -= decelerationDistanceM;
                    distanceToHomeM += decelerationDistanceM * GetFuelEnduranceModifier(speedKph);

                    decelerationTimeSec -= deltaTimeSec;

                    // Update turn distance
                    if (turnTimeSec > 0)
                    {
                        var deltaTurnTimeSec = Math.Min( 1.0, deltaTimeSec );
                        var turnDistanceM = speedInMs * deltaTurnTimeSec;

                        distanceToHomeM += turnDistanceM;
                        turnTimeSec -= deltaTurnTimeSec;
                    }

                    speedKph -= Math.Abs(UnitClass.MaxDecelerationKphSec);
                }
            }

            // Add remaining turn distance at cruise speed
            if ( turnTimeSec > 0 )
            {
                var cruiseSpeedInMs = UnitClass.CruiseSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
                var turnDistanceM = cruiseSpeedInMs * turnTimeSec;

                distanceToHomeM += turnDistanceM;
            }

            // Scale distance with factor to add error margin
            return ((distanceToHomeM * GameConstants.BINGO_FUEL_FACTOR) / FuelDistanceRemainingM) * 100.0;
		}

        /// <summary>
        /// Get fuel endurance modifier (a number to be multiplied by cruise speed fuel use) for 
        /// propulsion system and speed type. This method contains hard-coded values!
        /// </summary>
        /// <param name="unitSpeedType"></param>
        /// <returns></returns>
        protected double GetFuelEnduranceModifier(GameConstants.UnitSpeedType unitSpeedType)
        {
            //TODO: Should definately take altitude into account!
            double enduranceModifier = 1.0;

            //only relevant for higher than cruise speed.
            if (unitSpeedType != GameConstants.UnitSpeedType.Military
                && unitSpeedType != GameConstants.UnitSpeedType.Afterburner)
            {
                return enduranceModifier;
            }

            switch (UnitClass.PropulsionSystem)
            {
                case GameConstants.PropulsionSystem.TurboJet:
                    if (unitSpeedType == GameConstants.UnitSpeedType.Afterburner)
                    {
                        enduranceModifier = 6;
                    }
                    else //full military power
                    {
                        enduranceModifier = 2;
                    }
                    break;

                case GameConstants.PropulsionSystem.TurboFan:
                    if (unitSpeedType == GameConstants.UnitSpeedType.Afterburner)
                    {
                        enduranceModifier = 12; //penalty reduced by half across the board.
                    }
                    else //full military power
                    {
                        enduranceModifier = 2;
                    }

                    break;
                case GameConstants.PropulsionSystem.TurboProp: //does not support afterburner
                case GameConstants.PropulsionSystem.TurboShaft:
                case GameConstants.PropulsionSystem.Piston:
                    enduranceModifier = 1.5;
                    break;
            }
            return enduranceModifier;
        }

        protected double GetFuelEnduranceModifier(double unitSpeed)
        {
            return GetFuelEnduranceModifier(GetSpeedTypeFromKph(unitSpeed));
        }

        protected double GetFuelEnduranceModifier()
        {
            return GetFuelEnduranceModifier(ActualSpeedKph);
        }

        public double GetDistanceToHomeM()
        {
            if ( this.Position != null && LaunchedFromUnit != null
                && !LaunchedFromUnit.IsMarkedForDeletion && LaunchedFromUnit.Position != null )
            {
                //hmm. or, distance3d?
                return MapHelper.CalculateDistanceM(
                    this.Position.Coordinate, LaunchedFromUnit.Position.Coordinate );
            }
            return 0.0;
        }

		/// <summary>
		/// Checks whether unit has reached bingo fuel, and also if it has run out of fuel. If the unit has run 
		/// out of fuel, it will be deleted. If it has reached bingo fuel, it will automatically return to base.
		/// </summary>
		protected void CheckForBingoFuel()
		{
			if (IsMarkedForDeletion)
			{
				return;
			}
			if (UnitClass.MaxRangeCruiseM > 0) //0 means there is no range limit
			{
                // Update bingo fuel level
			    BingoFuelPercent = CalculateBingoFuelPercent();

                // Check if we've travelled too far
				if (FuelDistanceCoveredSinceRefuelM >= MaxRangeCruiseM)
				{
					IsMarkedForDeletion = true;
					HitPoints = 0;
					GameManager.Instance.Log.LogDebug(
						string.Format("Unit {0} has a max range of {1}m, has moved {2}m and is being deleted. Distance to target: {3}m.",
						ToString(), MaxRangeCruiseM, FuelDistanceCoveredSinceRefuelM, MostRecentDistanceToTargetM));
					if (UnitClass.UnitType == GameConstants.UnitType.FixedwingAircraft ||
						UnitClass.UnitType == GameConstants.UnitType.Helicopter)
					{
                        OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitCrashedOutOfFuel, this.Id));
					}
					else if (UnitClass.UnitType == GameConstants.UnitType.Bomb
						|| UnitClass.UnitType == GameConstants.UnitType.Missile
						|| UnitClass.UnitType == GameConstants.UnitType.Torpedo)
					{
						CreateBattleDamageReportOutOfFuel();
					}
				}
				else if(!IsOrderedToReturnToBase && BingoFuelPercent >= 100.0) 
				{
					GameManager.Instance.Log.LogDebug(
						string.Format("CheckForBingoFuel() : Unit {0} has reached BINGO FUEL.\nMaxCruiseRangeM={1:F}," +
						"FuelDistanceCoveredM={2:F}, DistanceToHomeM={3:F}",
						ToShortString(), MaxRangeCruiseM, FuelDistanceCoveredSinceRefuelM, GetDistanceToHomeM()));

                    // If the unit supports in air refueling and not out of ammo yet, check for refueling aircraft nearby
                    var airUnit = this as AircraftUnit;
                    if (airUnit != null && Position != null && UnitClass != null && UnitClass.CanRefuelInAir && !IsPrimaryWeaponsOutOfAmmo())
                    {
                        var refuelingAircrafts = OwnerPlayer.FindAllAvailableUnitRole(Position.Coordinate,
                                                                                         new List<GameConstants.Role>() { GameConstants.Role.RefuelAircaft },
                                                                                         string.Empty, false, true);

                        if (refuelingAircrafts.Any())
                        {
                            foreach (var refuelingAircraft in refuelingAircrafts.OfType<AircraftUnit>())
                            {
                                // Don't attempt to refuel with ourself
                                if (refuelingAircraft.Id != this.Id)
                                {
                                    // If air unit succedes to refuel with air, send message to player and return
                                    if (airUnit.RefuelInAir(refuelingAircraft))
                                    {
                                        OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitBingoFuelReturningToBase, this.Id, refuelingAircraft.Id));
                                        return;
                                    }
                                    else
                                    {
                                        OwnerPlayer.AIHandler.RefuelInAirFailed(airUnit, refuelingAircraft);

                                        // Message player of failing to refuel
                                        OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftRefuelInAirFailed, this.Id, refuelingAircraft.Id));
                                    }
                                }
                            }
                        }
                    }
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitBingoFuelReturningToBase, this.Id, LaunchedFromUnit.Id));
                    ReturnToBase();
				}
			}
		}

        /// <summary>
        /// Sets a specific carrier unit as new home for this unit.
        /// </summary>
        /// <param name="newCarrierUnitId"></param>
        public void SetHomeToNewCarrier(string newCarrierUnitId)
        {
            var carrierUnit = GetUnitById(OwnerPlayer, newCarrierUnitId);
            if (carrierUnit == null || carrierUnit.Position == null || carrierUnit.IsMarkedForDeletion)
            {
                return;
            }
            if (carrierUnit.AircraftHangar == null || carrierUnit.AircraftHangar.ReadyInSec > GameConstants.DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftCarrierNotReady, this.Id, newCarrierUnitId));
                GameManager.Instance.Log.LogError("SetHomeToNearestCarrier: desired unit is not a carrier or aircraft facilities are damaged");
                return;
            }
            if (carrierUnit.UnitClass.CarriedRunwayStyle < this.UnitClass.RequiredRunwayStyle)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftCarrierWrongRunwayStyle, this.Id, newCarrierUnitId));
                GameManager.Instance.Log.LogError("SetHomeToNearestCarrier: desired carrier unit has the wrong runway type.");
                return;
            }
            if (!this.HasEnoughFuelToReachTarget(carrierUnit.Position.Coordinate, false))
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftNotFuelToReachThisCarrier, this.Id, newCarrierUnitId));
                GameManager.Instance.Log.LogError("SetHomeToNearestCarrier: unit does not have enough fuel to reach designated carrier.");
                return;
            }
            if (carrierUnit.AircraftHangar.Aircraft.Count >= carrierUnit.UnitClass.MaxCarriedUnitsTotal)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftCarrierNotReady, this.Id, newCarrierUnitId));
                GameManager.Instance.Log.LogError("SetHomeToNearestCarrier: carrier is full.");
                return;
            }
            //AircraftCarrierNotReady, //Id=unit, SecondaryId = alleged carrier unit id. Unit is not carrier, air fac. damaged, or hangar is full
            //AircraftNotFuelToReachThisCarrier, //Id=unit, SecondaryId = alleged carrier unit id. Not enough fuel to reach this carrier
            //AircraftCarrierWrongRunwayStyle, //Id=unit, SecondaryId = alleged carrier unit id. Wrong type of runway on suggested carrier unit

            _hasCheckedForNearestCarrier = false;
            LaunchedFromUnit = carrierUnit;
            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftAssignedNewCarrier, this.Id, LaunchedFromUnit.Id));
            GameManager.Instance.Log.LogDebug(string.Format(
                "SetHomeToNearestCarrier: {0} is now set to return to {1}", ToShortString(), LaunchedFromUnit.ToShortString()));
            if (IsOrderedToReturnToBase)
            {
                ReturnToBase();
            }

        }

        /// <summary>
        /// Find nearest carrier able to support this unit and assign it has the place to return home to.
        /// </summary>
		public void SetHomeToNewCarrier()
		{
			if (_hasCheckedForNearestCarrier)
			{
				return;
			}
			_hasCheckedForNearestCarrier = true;
			if (IsMarkedForDeletion || Position == null || this.UnitClass.IsMissileOrTorpedo)
			{
				return;
			}
			GameConstants.Role roleCarrier = GameConstants.Role.LaunchRotaryWingAircraft;
			if(SupportsRole(GameConstants.Role.IsFixedWingAircraft))
			{
				roleCarrier = GameConstants.Role.LaunchFixedWingAircraft;
			}
			var maxRange = MaxRangeCruiseM - FuelDistanceCoveredSinceRefuelM;

			var carriers = OwnerPlayer.GetUnitsInAreaByRole(roleCarrier, this.Position.Coordinate, maxRange * 1.5);
			var carrierList = carriers.Where<BaseUnit>(u => !u.IsMarkedForDeletion && u.AircraftHangar != null 
                && u.AircraftHangar.ReadyInSec < GameConstants.DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC 
                && u.UnitClass.CarriedRunwayStyle >= this.UnitClass.RequiredRunwayStyle);
			if (carrierList.Count() == 0)
			{
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftNotFindingNewCarrier, this.Id));
				GameManager.Instance.Log.LogDebug(string.Format(
					"SetHomeToNearestCarrier: {0} cannot find a suitable carrier or airfield to return to.", ToShortString()));
				return;
 
			}
			_hasCheckedForNearestCarrier = false;
			LaunchedFromUnit = carrierList.FirstOrDefault();
            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftAssignedNewCarrier, this.Id, LaunchedFromUnit.Id));
			GameManager.Instance.Log.LogDebug(string.Format(
				"SetHomeToNearestCarrier: {0} is now set to return to {1}", ToShortString(),LaunchedFromUnit.ToShortString()));
            if (IsOrderedToReturnToBase)
            {
                ReturnToBase();
            }
		}

		/// <summary>
		/// Return to base.
		/// </summary>
        /// <returns>True if the order was successful and the unit is now returning to base.</returns>
		public virtual bool ReturnToBase()
		{
			if (IsOrderedToReturnToBase || this.Position == null)
			{
				return false;
			}
			MissionType = GameConstants.MissionType.Ferry;
			MissionTargetType = GameConstants.MissionTargetType.Undefined;
			TargetDetectedUnit = null;
			if(LaunchedFromUnit == null || LaunchedFromUnit.IsMarkedForDeletion)
			{
				//GameManager.Instance.Log.LogError(
				//    string.Format("Unit {0} can not return to base since LaunchedFromUnit is null or marked for deletion.", 
				//    ToShortString()));
				SetHomeToNewCarrier();
				if (LaunchedFromUnit == null || LaunchedFromUnit.IsMarkedForDeletion)
				{
                    return false; //no airbase available
				}
			}
			SetDirty(GameConstants.DirtyStatus.UnitChanged);
			IsOrderedToReturnToBase = true;
			MovementOrder = new MovementOrder();
			Waypoint wp = new Waypoint(LaunchedFromUnit);
			ActualSpeedKph = GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Cruise);
            SetActualSpeed(ActualSpeedKph); //force afterburners off immediately
			double distanceHomeM = MapHelper.CalculateDistanceM(this.Position.Coordinate, LaunchedFromUnit.Position.Coordinate);
            if (distanceHomeM < wp.DesiredDistanceToTargetM)
            {
                var airUnit = this as AircraftUnit;
                if (airUnit != null && airUnit.LandOnBase())
                {
                    return true;
                }
            }
            //else
            //{
                //double fuelDistanceNeccessary = (MaxRangeCruiseM - distanceHomeM); //cheat! make sure there is enough fuel to get home!
                //if (fuelDistanceNeccessary < this.FuelDistanceRemainingM && fuelDistanceNeccessary > this.FuelDistanceRemainingM * 0.3)
                //{
                //FuelDistanceCoveredSinceRefuelM = (MaxRangeCruiseM - (distanceHomeM * 2)) - 20000; // GameConstants.BINGO_FUEL_FACTOR);
                //if (FuelDistanceCoveredSinceRefuelM < 0)
                //    FuelDistanceCoveredSinceRefuelM = 0;
                //}
            //}
			UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
			wp.Orders.Add(new BaseOrder() { OrderType = GameConstants.OrderType.ReturnToBase });
			MovementOrder.AddWaypoint(wp);
            ReCalculateEta();
			if (IsInGroupWithOthers()) 
			{
				if (IsGroupMainUnit()) //other units in group also return to base
				{
					if (Group != null)
					{
						try
						{
                            for (int i = 0; i < Group.Units.Count; i++)
                            {
                                var u = Group.Units[i];
                                if (u.MaxRangeCruiseM > 0)
                                {
                                    if (u.Id != this.Id)
                                    {
                                        u.ReturnToBase();    
                                    }
                                }
                                else
                                {
                                    Group.RemoveUnit(u);
                                }
							}
						}
						catch (Exception ex)
						{
							GameManager.Instance.Log.LogError(
								string.Format("ReturnToBase: Return all units to base failed. {0}", ex.Message));
						}
					}
				}
				else if(!Group.MainUnit.IsOrderedToReturnToBase)
				{ 
					//hmm. what to do if a non-leader has to return to base? For now, leave group
					Group.RemoveUnit(this);
				}
			}
		    return true;
		}

		/// <summary>
		/// Updates Estimated Time of Arrival for unit
		/// </summary>
		/// <param name="elapsedGameTimeSec"></param>
		public void UpdateEta(double elapsedGameTimeSec)
		{
			if (EtaAllWaypoints.TotalSeconds > elapsedGameTimeSec)
			{
				EtaAllWaypoints = EtaAllWaypoints.Subtract(TimeSpan.FromSeconds(elapsedGameTimeSec));
			}
			else
			{
				EtaAllWaypoints = TimeSpan.FromSeconds(0);
			}
			if (EtaCurrentWaypoint.TotalSeconds > elapsedGameTimeSec)
			{
				EtaCurrentWaypoint = EtaCurrentWaypoint.Subtract(TimeSpan.FromSeconds(elapsedGameTimeSec));
			}
			else
			{
				EtaCurrentWaypoint = TimeSpan.FromSeconds(0);
			}

		}

		/// <summary>
		/// The major method that determines the unit's destination and moves it the correct distance based
		/// on the elapsed time.
		/// </summary>
        /// <param name="deltaGameTimeSec">Game time elapsed since last tick in seconds.</param>
		public virtual void MoveToNewPosition3D(double deltaGameTimeSec)
		{
            // 1: Unit has a waypoint to move towards
            // 1a: Unit has arrived at waypoint, get next if available, otherwise warning or formation
            //     Call method recursively with remaining time to use moving towards new waypoint.
            //     Missile/torpedo: if at end target, detonate. If more waypoints, activate them.
            // 1b: Not yet arrived (no action)

            // 2: Unit does not have a waypoint: continue on present course. Warn player. If missile, destruct

			if (deltaGameTimeSec <= 0)
			{
				return;
			}
			if (UnitClass == null || UnitClass.IsFixed)
			{
				return;
			}
			if (CarriedByUnit != null || IsMarkedForDeletion)
			{
				return;
			}
            if (this.DomainType == GameConstants.DomainType.Subsea || this.DomainType == GameConstants.DomainType.Surface)
            {
                if (UserDefinedSpeed != GameConstants.UnitSpeedType.Stop && TerrainHeight10SecForwardM > 0)
                {
                    UserDefinedSpeed = GameConstants.UnitSpeedType.Stop;
                    ActualSpeedKph = 0;
                    GameManager.Instance.Log.LogWarning(
                        string.Format("MoveToNewPosition3D: Unit {0} STOPPED for impassible terrain: {1}m height", this, TerrainHeight10SecForwardM));
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitStoppedForImpassibleTerrain, this.Id));
                }
            }

			// Units that are in formation and at position, is moved along by the mainunit
			if (MovementOrder != null && MovementOrder is MovementFormationOrder 
				&& this.Group != null && !this.IsGroupMainUnit() 
				&& IsAtFormationPositionFlag)
			{
				return; 
			}

            UpdateEta(deltaGameTimeSec);

            ResolveUnitAndGroupSpeed();
            UpdateActualSpeed(deltaGameTimeSec);

            var elevationChangeM = UpdateElevation(deltaGameTimeSec, ActualSpeedKph);

            var speedInMs = ActualSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
            var distanceMeters = speedInMs * deltaGameTimeSec;          // The distance the unit CAN move this tick
            var distanceMetersToMove = distanceMeters;                  // The distance the unit ACTUALLY moved in this tick

            // Clamp distance to max remaining fuel distance
            if (distanceMetersToMove > FuelDistanceRemainingM)
            {
                distanceMetersToMove = FuelDistanceRemainingM;
                if (distanceMetersToMove < 0)
                    distanceMetersToMove = 0;
            }

            // For air units make sure we wont run out of fuel before reaching the target
		    if (!IsOrderedToReturnToBase && (UnitClass.UnitType == GameConstants.UnitType.FixedwingAircraft ||
                    UnitClass.UnitType == GameConstants.UnitType.Helicopter) && LaunchedFromUnit != null && !LaunchedFromUnit.IsMarkedForDeletion)
            {
                var fuelDistanceCoveredToTarget = GetFuelDistanceCovered(distanceMetersToMove, deltaGameTimeSec);
                if (fuelDistanceCoveredToTarget > MaxRangeCruiseM)
                {
                    distanceMetersToMove = MaxRangeCruiseM - FuelDistanceCoveredSinceRefuelM;
                    if (ActualSpeedKph >= 10)
                    {
                        distanceMetersToMove /= GetFuelEnduranceModifier();
                    }
                    if (distanceMetersToMove < 0)
                        distanceMetersToMove = 0;
                }
            }

		    // Get the active waypoint
            var activeWaypoint = this.GetActiveWaypoint();

            if (activeWaypoint!= null && activeWaypoint.Position != null)
			{
				if (ActualSpeedKph > 0)
				{
					SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
				}

                // Get distance to the target. NOTE: 2D distance!
                MostRecentDistanceToTargetM = MapHelper.CalculateDistanceM(this.Position.Coordinate, activeWaypoint.Position.Coordinate);
                if (double.IsNaN(MostRecentDistanceToTargetM))
                {
                    MostRecentDistanceToTargetM = 0;
                }

                // Make sure we haven't already reached the waypoint before moving
                if (MostRecentDistanceToTargetM > activeWaypoint.DesiredDistanceToTargetM)
                {
                    // Get desired bearing
                    DesiredBearingDeg = MapHelper.CalculateBearingDegrees(this.Position.Coordinate,
                                       activeWaypoint.Position.Coordinate);

                    // Check if we can reach the waypoint.
                    // Calculate turn circle from speed and turn range.
                    if (DesiredBearingDeg != ActualBearingDeg)
                    {
                        var turnDeg = Math.Abs(DesiredBearingDeg.Value - ActualBearingDeg.Value);

                        // Make sure we can't reach it this tick
                        var turnRangeDegrSec = GetTurnRangeDegrSec();
                        if (turnDeg > (turnRangeDegrSec * deltaGameTimeSec))
                        {
                            // How long does it take to do a full 360?
                            var turnTimeSec = 360.0 / turnRangeDegrSec;

                            // distance to target is diameter of max turn circle
                            var maxTurnRadiusM = MostRecentDistanceToTargetM / 2;

                            // Get the turn radius
                            // r = (vT / 2PI)
                            var turnRadiusM = (speedInMs * turnTimeSec) / (2 * Math.PI);
                            if (turnRadiusM * 2 > MostRecentDistanceToTargetM)
                            {
                                // Get max speed we should be travelling at.
                                // v = (2PI * r) / T
                                var maxSpeedMs = (2 * Math.PI * maxTurnRadiusM) / turnTimeSec;
                                //if (maxSpeedMs < UnitClass.MinSpeedKph)
                                //    maxSpeedMs = UnitClass.MinSpeedKph;

                                // Cap the speed?
                                if (speedInMs > maxSpeedMs)
                                {
                                    speedInMs = maxSpeedMs;

                                    SetActualSpeed(speedInMs * GameConstants.MS_TO_KPH_CONVERSION_FACTOR);

                                    // Recalculate how long we're moving now that speed changed
                                    distanceMeters = speedInMs * deltaGameTimeSec;
                                    if (distanceMeters < distanceMetersToMove)
                                        distanceMetersToMove = distanceMeters;
                                }
                            }
                        }
                    }

                    // Update bearing
                    UpdateActualBearing(deltaGameTimeSec);

                    // Clamp distance to move
                    if (distanceMetersToMove > MostRecentDistanceToTargetM)
                    {
                        distanceMetersToMove = MostRecentDistanceToTargetM;
                    }

                    double distanceToMoveHorizontally = distanceMetersToMove;
                    //if (Math.Abs(elevationChangeM - 0) > double.Epsilon)
                    //{
                    //    //Pythagoras: find b from a and c
                    //    //a^2 = c^2 - b^2
                    //    if (Math.Abs(distanceMetersToMove) > Math.Abs(elevationChangeM))
                    //    {
                    //        distanceToMoveHorizontally =
                    //            Math.Sqrt(Math.Pow(distanceMetersToMove, 2) - Math.Pow(elevationChangeM, 2));
                    //    }
                    //    else
                    //    {
                    //        distanceToMoveHorizontally =
                    //            Math.Sqrt(Math.Pow(elevationChangeM, 2) - Math.Pow(distanceMetersToMove, 2));
                    //    }
                    //}
                    //if (double.IsNaN(distanceToMoveHorizontally))
                    //{
                    //    distanceToMoveHorizontally = 0;
                    //}

                    // Calculate and set new coordinate
                    var newCoordinate = MapHelper.CalculateNewPosition2(Position.Coordinate, (double) ActualBearingDeg,
                                                                        distanceToMoveHorizontally);
                    Position.SetNewCoordinate(newCoordinate.LatitudeDeg, newCoordinate.LongitudeDeg);

                    // Update distance to target. NOTE: 2D distance!
                    MostRecentDistanceToTargetM = MapHelper.CalculateDistance3DM(this.Position, activeWaypoint.Position);
                    if (double.IsNaN(MostRecentDistanceToTargetM))
                    {
                        MostRecentDistanceToTargetM = 0;
                    }
                }
                else
                {
                    distanceMetersToMove = 0;   // Already reached target, so we didn't move
                }

                // Update ETA
                if (this.ActualSpeedKph > 0)
                {
                    EtaCurrentWaypoint = TimeSpan.FromSeconds(MostRecentDistanceToTargetM /
                        (this.ActualSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR));
                }
                else
                {
                    EtaCurrentWaypoint = TimeSpan.Zero;
                }

                // Check if we've arrived at the waypoint
                if (MostRecentDistanceToTargetM <= activeWaypoint.DesiredDistanceToTargetM)
                {
                    GameManager.Instance.Log.LogDebug(
                        string.Format("MoveToNewPosition3D: Unit {0} has met desired distance {1}m to target.",
                        ToShortString(), activeWaypoint.DesiredDistanceToTargetM));

                    //force arrived.
                    MostRecentDistanceToTargetM = 0;

                    // Activate next waypoint
                    var nextWaypoint = MovementOrder.ActivateNextWaypoint();

                    // Returning to a unit?
                    if (activeWaypoint.TargetBaseUnit != null)
                    {
                        // Land aircraft if target is the home base
                        if (IsOrderedToReturnToBase && LaunchedFromUnit != null &&
                            LaunchedFromUnit.Id == activeWaypoint.TargetBaseUnit.Id)
                        {
                            var aircraft = this as AircraftUnit;
                            if (aircraft != null && aircraft.LandOnBase())
                            {
                                return;
                            }
                        }
                    }

                    // Execute any orders in the waypoint
                    if (!activeWaypoint.HasOrdersBeenExecuted)
                    {
                        foreach (var order in activeWaypoint.Orders)
                        {
                            Orders.Enqueue(order);
                        }
                        activeWaypoint.HasOrdersBeenExecuted = true;
                        ExecuteOrders(); //make sure at least first order is executed immediately, in case it is a engagement order
                    }

                    SetDirty(GameConstants.DirtyStatus.UnitChanged);

                    if (MovementOrder == null || !MovementOrder.HasMoreWaypoints)
                    {
                        // Make sure that missiles impact at their last waypoint
                        if (UnitClass.IsMissileOrTorpedo && this is MissileUnit)
                        {
                            var missileUnit = this as MissileUnit;
                            if (missileUnit != null)
                            {
                                missileUnit.Impact();
                            }
                        }
                        else
                        {
                            // Engage any target if we have one
                            if (TargetDetectedUnit != null && TargetDetectedUnit.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe)
                            {
                                var engagementType = GameConstants.EngagementOrderType.CloseAndEngage;
                                if (this.DomainType != GameConstants.DomainType.Air)
                                {
                                    engagementType = GameConstants.EngagementOrderType.EngageNotClose;
                                }

                                EngageDetectedUnit(TargetDetectedUnit,
                                        engagementType, string.Empty,
                                        IsGroupMainUnit(), true);
                            }

                            if (!IsOrderedToReturnToBase)
                            {
                                if (IsInGroupWithOthers() && !IsGroupMainUnit())
                                {
                                    // if unit has no more movement orders but is part of a formation,
                                    // attempt to set a MovementFormationOrder.
                                    SetUnitFormationOrder();
                                    //CheckIfGroupIsStaging();
                                }

                                // Set half speed on unit with no movement order
                                if (OwnerPlayer != null
                                    && !HasQueuedMovementOrders()
                                    && !IsOrderedToReturnToBase
                                    && Position != null
                                    && !UnitClass.IsMissileOrTorpedo
                                    && !(MovementOrder is MovementFormationOrder))
                                {
                                    if (OwnerPlayer.AIHandler != null)
                                    {
                                        OwnerPlayer.AIHandler.UnitHasNoMovementOrders(this);
                                    }
                                    UserDefinedSpeed = GameConstants.UnitSpeedType.Half;
                                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitHasNoOrders, this.Id));
                                }
                            }
                        }
                    }
                }
			}
			else //does NOT have ActiveWaypoint
			{
				if (IsMarkedForDeletion)
				{
					return;
				}
                if (MovementOrder != null)
                {
                    MovementOrder.ActivateNextWaypoint();
                }

				if (UnitClass.IsMissileOrTorpedo)
				{
					if (this.Sensors.Any())
					{
						MissileUnit missile = this as MissileUnit;
						if (missile != null)
						{
							missile.SearchForTarget();
							if (missile.TargetPosition == null && missile.TargetDetectedUnit == null)
							{
								IsMarkedForDeletion = true;
								return;
							}
						}
					}
					else
					{
						IsMarkedForDeletion = true;
					}
				}
				if (GetMaxHeightOverSeaLevelM() > 0 && this.DomainType == GameConstants.DomainType.Air)
				{
					if (DesiredHeightOverSeaLevelM < GameConstants.HEIGHT_LOW_MIN_M)
					{
						DesiredHeightOverSeaLevelM = GameConstants.HEIGHT_LOW_MIN_M;
					}
				}

				MostRecentDistanceToTargetM = 0;
                if (distanceMetersToMove > 0 || elevationChangeM > 0)
				{
					SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
				}

                // Update position
                if (Position != null)
                {
                    Coordinate newCoordinate = MapHelper.CalculateNewPosition2(Position.Coordinate,
                                                                               (double) ActualBearingDeg,
                                                                               distanceMetersToMove);

                    Position.SetNewCoordinate(newCoordinate.LatitudeDeg, newCoordinate.LongitudeDeg);
                }

                // Reset ETA as we have no waypoint
			    EtaAllWaypoints = TimeSpan.Zero;
				EtaCurrentWaypoint = TimeSpan.Zero;
			}

            if (IsMarkedForDeletion)
            {
                return;
            }

            // Check if we could have travelled longer on this tick
            var distanceRemainingM = distanceMeters - distanceMetersToMove;
            var timeRemainingSec = deltaGameTimeSec * (distanceRemainingM / distanceMeters);

            UpdateFuelDistanceCovered(distanceMetersToMove, deltaGameTimeSec - timeRemainingSec);

            // Update movement on group members last
            UpdateGroupMovement(distanceMetersToMove, deltaGameTimeSec - timeRemainingSec);

            if (distanceMetersToMove <= 0 && ActualSpeedKph > 0)
            {
                GameManager.Instance.Log.LogDebug(string.Format("BaseUnit::MoveToNewPosition3D -> distance to move is <= 0, but actual speed is > 0! Unit:{0} Distance:{1}M, Speed:{2}KPH", Name, distanceMetersToMove, ActualSpeedKph));
            }

            // Call self if we have more time remaining.
            // Check against flag to avoid the recursive call doing more recursive calls.
            if (!_isRecursiveMovementFlag && !IsMarkedForDeletion && timeRemainingSec > 0)
            {
                _isRecursiveMovementFlag = true;
                MoveToNewPosition3D(timeRemainingSec);
            }
            _isRecursiveMovementFlag = false;
		}

        private void UpdateGroupMovement(double distanceMetersToMove, double deltaGameTimeSec)
        {
            // If we're in a group and group leader, move other units
            if ( this.Group != null && this.IsGroupMainUnit() && this.IsInGroupWithOthers() )
            {
                // Get units from group. Convert to list to get a copy as the Units list in Group might get modified while we run this loop
                var units = this.Group.Units.Where(unit => unit.Id != this.Id).ToList();
                foreach (var unit in units)
                {
                    unit.MoveToNewPosition3DWithFormation( this, distanceMetersToMove, deltaGameTimeSec );
                }
            }
        }

		/// <summary>
		/// Method called by a group's main unit to move this unit along with formation. If group is not
		/// near formation position or is not a non-main-unit in a group, do nothing.
		/// </summary>
        public void MoveToNewPosition3DWithFormation(BaseUnit mainUnit, double distanceMovedM, double deltaGameTimeSec)
		{
            if (Position == null || CarriedByUnit != null || IsMarkedForDeletion )
            {
                return;
            }
			if(!(MovementOrder is MovementFormationOrder) || 
                !this.IsAtFormationPositionFlag || 
				!this.IsInGroupWithOthers() || 
				this.IsGroupMainUnit())
			{
				return;
			}
            try
            {
                var posOffset = (MovementOrder as MovementFormationOrder).PositionOffset;
                if (posOffset != null && mainUnit != null && mainUnit.Position != null)
                {
                    var formPosWp = MapHelper.CalculatePositionFromOffset2(mainUnit.Position, posOffset);
                    Position.SetNewCoordinate(formPosWp.Coordinate.LatitudeDeg, formPosWp.Coordinate.LongitudeDeg);
                    if (this.MaxRangeCruiseM > 0)
                    {
                        UpdateFuelDistanceCovered(distanceMovedM, deltaGameTimeSec);
                    }
                    this.ActualHeightOverSeaLevelM = mainUnit.ActualHeightOverSeaLevelM;
                    this.DesiredHeightOverSeaLevelM = mainUnit.DesiredHeightOverSeaLevelM;
                    //if ( this.Position != null )
                    //{
                    //    this.Position.HeightOverSeaLevelM = this.DesiredHeightOverSeaLevelM;
                    //}                
                    //this.IsAtFormationPositionFlag = true;
                    this.UserDefinedElevation = mainUnit.UserDefinedElevation;
                    this.UserDefinedSpeed = mainUnit.UserDefinedSpeed;
                    this.ActualBearingDeg = mainUnit.ActualBearingDeg;
                    this.DesiredBearingDeg = mainUnit.DesiredBearingDeg;
                    this.SetActualSpeed(mainUnit.ActualSpeedKph);
                    this.DesiredSpeedKph = mainUnit.DesiredSpeedKph;

                    UpdateEta(deltaGameTimeSec);

                    SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
                }
            }
            catch (Exception ex)
            {
                var unitDesc = this.ToShortString();
                if (mainUnit != null)
                {
                    unitDesc += " (Main unit " + mainUnit.ToShortString() + ")";
                }
                GameManager.Instance.Log.LogError("MoveToNewPosition3DWithFormation failed for unit " + unitDesc + ". " + ex.ToString());
            }
		}

		/// <summary>
		/// The method that performs the moving of a unit from one map coordinate to the next, based on its destination and speed.
		/// </summary>
		/// <param name="gameTime">The time, in milliseconds, since last movement.</param>
		public virtual void MoveToNewCoordinate(double deltaGameTimeSec)
		{
			if (deltaGameTimeSec <= 0)
			{
				return;
			}
			if(CarriedByUnit != null || IsMarkedForDeletion)
			{
				return;
			}

            MoveToNewPosition3D(deltaGameTimeSec);
		}

		/// <summary>
		/// Returns true if there are any queued movement orders in the Orders queue for this unit.
		/// </summary>
		/// <returns></returns>
		public virtual bool HasQueuedMovementOrders()
		{
			foreach (var order in Orders)
			{
				if (order is MovementFormationOrder || order is MovementOrder)
				{
					return true;
				}
				if (order is EngagementOrder)
				{
					EngagementOrder engagementOrder = order as EngagementOrder;
					if (engagementOrder != null 
						&& engagementOrder.EngagementOrderType != GameConstants.EngagementOrderType.EngageNotClose)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true if orders to engage the specified DetectedUnit already exist in the unit's order queue.
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public virtual bool HasAnyEngagementOrders(DetectedUnit detectedUnit)
		{
			try
			{
				if (detectedUnit == null || detectedUnit.IsMarkedForDeletion)
				{
					return false;
				}
                //if (CurrentEngagementOrder != null && CurrentEngagementOrder.TargetDetectedUnit != null && CurrentEngagementOrder.TargetDetectedUnit.Id == detectedUnit.Id)
                //{
                //    return true;
                //}
				var engOrders = from o in this.Orders
								where o is EngagementOrder && (o as EngagementOrder).TargetDetectedUnit.Id == detectedUnit.Id
								select o;
				
				if (engOrders.Any())
				{
					return true;
				}
				if (MovementOrder != null)
				{
					foreach (var wp in MovementOrder.GetWaypoints())
					{
						if (wp.Orders != null)
						{
							var engWpOrders = from o in wp.Orders
											  where o is EngagementOrder 
												&& (o as EngagementOrder).TargetDetectedUnit.Id == detectedUnit.Id
											  select o;
							if (engWpOrders.Any())
							{
								return true;
							}
							if (wp.Orders.Any(ord => ord is EngagementOrder && (ord as EngagementOrder).TargetDetectedUnit.Id == detectedUnit.Id))
							{
							    return true;
							}
						}
					}
				}
				//Note: this doesn't find even more deeply nested EngagementOrders!
				return false;
			}
			catch (Exception)
			{

				return false;
			}

		}

		/// <summary>
		/// Returns true if any engagement orders exists in the unit's order queue.
		/// </summary>
		/// <returns></returns>
		public virtual bool HasAnyEngagementOrders()
		{
			try
			{
				var engOrders = from o in this.Orders
								where o is EngagementOrder
								select o;
				if (engOrders.Any())
				{
					return true;
				}
				if (MovementOrder != null)
				{
				    return (from wp in MovementOrder.GetWaypoints()
				            where wp.Orders != null
				            select (from o in wp.Orders
				                    where o is EngagementOrder
				                    select o)).Any(engWpOrders => engWpOrders.Any());
				}
				//Note: this doesn't find even more deeply nested EngagementOrders!
				return false;

			}
			catch (Exception)
			{

				return false;
			}
		}

		/// <summary>
		/// Recalculates the expected time of arrival in seconds based on all the unit's waypoints
		/// </summary>
		public virtual void ReCalculateEta()
		{
			if (Position == null || MovementOrder == null 
				|| MovementOrder is MovementFormationOrder 
				|| MovementOrder.CountWaypoints == 0
                || GetActiveWaypoint() == null)
			{
				EtaAllWaypoints = TimeSpan.Zero;
				return;
			}
			if (ActualSpeedKph <= 0)
			{
				EtaAllWaypoints = TimeSpan.Zero;
				return;
			}
			double tempEtaSec = 0;
			Coordinate oldpos = this.Position.Coordinate;

		    var waypoints = MovementOrder.GetWaypoints().Where(w => w != null && w.Position != null);
            var actualSpeedMs = this.ActualSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
            foreach (var wp in waypoints)
		    {
                var distanceM = MapHelper.CalculateDistanceM(oldpos, wp.Position.Coordinate);
                tempEtaSec += distanceM / actualSpeedMs;
                oldpos = wp.Position.Coordinate;
		    }
			EtaAllWaypoints = (!double.IsNaN(tempEtaSec) ? TimeSpan.FromSeconds(tempEtaSec) : TimeSpan.Zero);
		}

		/// <summary>
		/// Called by GameEventLoop every tick.
		/// </summary>
		/// <param name="timer">Milliseconds since last invocation of GameEventLoop tick</param>
		public virtual void Tick(double deltaGameTimeSec)
		{
			if (deltaGameTimeSec == 0)
			{
				return;
			}

            //double gameTimerMs = GameManager.Instance.Game.GameWorldTimeSec;
            //double gameWorldTimeLastSweep = _GameWorldTimeLastSweepSec;
            //if (_GameTimeLastSweep + GameConstants.TIME_BETWEEN_SENSOR_SWEEPS_MS > GameManager.Instance.Game.GameTimerMs)
            //{
            //SensorSweep();
            //_GameWorldTimeLastSweepSec = GameManager.Instance.Game.GameWorldTimeSec;
            //}

            foreach (BaseComponent comp in _components.Values)
            {
                comp.Tick(deltaGameTimeSec);
            }

            InflictDamageFromFires(deltaGameTimeSec);

			if (ExtraOrdinaryNoiseModifyerRemoveInSec > 0)
			{
				ExtraOrdinaryNoiseModifyerRemoveInSec -= deltaGameTimeSec;
				if (ExtraOrdinaryNoiseModifyerRemoveInSec <= 0)
				{
					ExtraOrdinaryNoiseModifyerRemoveInSec = 0;
				}
			}
			if (TimeToLiveSec > 0)
			{
				TimeToLiveSec -= deltaGameTimeSec;
				if (TimeToLiveSec <= 0)
				{
					TimeToLiveSec = 0;
					HitPoints = 0;
					IsMarkedForDeletion = true;
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
				}
			}
			if (ReadyInSec > 0)
			{
				ReadyInSec -= deltaGameTimeSec;
				if (ReadyInSec <= 0)
				{
					ReadyInSec = 0;
					SetDirty(GameConstants.DirtyStatus.UnitChanged);

					if (CarriedByUnit != null) //when carried unit is ready, the parent BaseUnitInfo should also be sent to client
					{
                        var readyMsg = new GameStateInfo(GameConstants.GameStateInfoType.AircraftIsReady, this.Id);
                        readyMsg.SecondaryId = CarriedByUnit.Id;
                        OwnerPlayer.Send(readyMsg);
						CarriedByUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
					}
				}
			}
		}

        public void SensorSweep( double deltaGameTimeSec )
        {
            // Always do a sensor sweep the first time.
            if ( _GameWorldTimeSinceLastSweepSec == null )
            {
                _GameWorldTimeSinceLastSweepSec = GameConstants.TIME_BETWEEN_SENSOR_SWEEPS_SEC;
            }
            else
            {
                _GameWorldTimeSinceLastSweepSec += deltaGameTimeSec;
            }

            // Check if enough time has passed since last sweep
            if ( _GameWorldTimeSinceLastSweepSec >= GameConstants.TIME_BETWEEN_SENSOR_SWEEPS_SEC )
            {
                SensorSweep();
                _GameWorldTimeSinceLastSweepSec = 0;
            }
        }

		/// <summary>
		/// For operating units, performs a sensor sweep for all sensors to detect units.
		/// </summary>
		/// <returns>True if any unit was detected during the sensor sweep. False otherwise.</returns>
		public virtual bool SensorSweep()
		{
            if (IsMarkedForDeletion || CarriedByUnit != null || Position == null)
			{
				return false;
			}
            if (UnitClass.IsMissileOrTorpedo)
            {
                if (IsInGroupWithOthers() && !IsGroupMainUnit())
                {
                    return false;    
                }
                if (TargetDetectedUnit != null && TargetDetectedUnit.RefersToUnit != null) //missiles with a target unit should not look for other targets (optimization)
                {
                    return AttemptDetectUnit(TargetDetectedUnit.RefersToUnit);
                }
            }
		    var didDetectedAnyUnit = false;
			foreach (var player in GameManager.Instance.Game.Players)
			{
				if (player.Id != OwnerPlayer.Id) //no need to detect own units
				{
					foreach (var unit in player.Units)
					{
						if (unit.IsOperational
							&& unit.CarriedByUnit == null
							&& unit.Position != null
							&& !unit.IsMarkedForDeletion
							&& !unit.UnitClass.IsAlwaysVisibleForEnemy)
						{
							if (AttemptDetectUnit(unit))
							{
                                didDetectedAnyUnit = true;
							}
						}
					}
				}
			}
		    return didDetectedAnyUnit;
		}

		/// <summary>
		/// This method will employ all sensors on a unit in attempting to detect a second unit
		/// </summary>
		/// <param name="unit"></param>
		protected virtual bool AttemptDetectUnit(BaseUnit unit)
		{
		    var didDetectUnit = false;
			double distanceToUnitM;
			if(MapHelper.IsDistanceShorterThan(
				this.Position, unit.Position, MaxSensorDetectionDistanceM, out distanceToUnitM))
			{
				foreach (BaseSensor sensor in this.Sensors)
				{
                    if (sensor.IsReady && sensor.IsOperational)
					{
						if (sensor.AttemptDetectUnit(unit, distanceToUnitM))
						{
						    didDetectUnit = true;
						}
					}
				}
			}

		    return didDetectUnit;
		}

		/// <summary>
		/// Registers a component on this unit. Components are weapons, sensors and aircraft hangars.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="name"></param>
		public virtual void RegisterComponent(BaseComponent component, string name)
		{
            if (component == null || string.IsNullOrEmpty(name))
            {
                return;
            }
			_components.Add(component.Id, component);
			component.Name = name;
		    component.OwnerUnit = this;

            if (component is BaseSensor)
            {
                _Sensors.Add(component as BaseSensor);
            }
            else if (component is BaseWeapon)
            {
                _Weapons.Add(component as BaseWeapon);
            }
		}

		/// <summary>
		/// Removes a component from a unit.
		/// </summary>
		/// <param name="id"></param>
		public virtual void UnregisterComponent(string id)
		{
            if ( _components.ContainsKey( id ) )
            {
                BaseComponent component = _components[ id ];

                if (component is BaseSensor)
                {
                    _Sensors.Remove(component as BaseSensor);
                }
                else if (component is BaseWeapon)
                {
                    _Weapons.Remove(component as BaseWeapon);
                }

                _components.Remove( id );
            }
		}

		/// <summary>
		/// Returns a unit by registration name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual BaseComponent GetComponent(string name)
		{
            try
            {
                return _components[name];
            }
            catch (Exception)
            {

                return null;
            }
		}

		/// <summary>
		/// Adds a carried unit (typically an aircraft) to a unit which has an aircraft hangar.
		/// </summary>
		/// <param name="unitClassId"></param>
		/// <param name="preferredAircraftDockingStatus"></param>
		/// <returns></returns>
		public virtual AircraftUnit AddCarriedUnit(string unitClassId)
		{
			if (AircraftHangar == null || UnitClass.MaxCarriedUnitsTotal == 0)
			{
				return null;
			}
			GameData gd = GameManager.Instance.GameData;
			AircraftUnit carriedUnit = (AircraftUnit)gd.CreateUnit(OwnerPlayer, null, unitClassId, string.Empty, null, true, false);
			carriedUnit.CarriedByUnit = this;
            carriedUnit.Position = null;
            SetDirty(GameConstants.DirtyStatus.UnitChanged);
			this.AircraftHangar.Aircraft.Add(carriedUnit);
			return carriedUnit;
		}

		/// <summary>
		/// Launches one or more carried aircraft from a unit.
		/// </summary>
		/// <param name="listUnits">A list of carried aircraft to be launched</param>
		/// <param name="groupId">If non-empty, the id of the group the launched units will join</param>
		/// <param name="groupName">If non-empty, specifies name of group</param>
		/// <param name="orders">A List of orders to be gives to first unit in group</param>
		/// <param name="tag">An optional tag for the units</param>
		/// <returns></returns>
		public virtual bool LaunchAircraft(List<AircraftUnit> listUnits, 
			string groupId, string groupName, 
			List<BaseOrder> orders, string tag)
		{
			Group group;
            if (!this.IsReady)
            {
                if (this.ReadyInSec > 100)
                {
                    TimeSpan time = TimeSpan.FromSeconds(this.ReadyInSec);
                    GameManager.Instance.Log.LogWarning(string.Format(
                        "LaunchAircraft: {0} could not launch aircraft. Unit ready in {1}.",
                        ToShortString(), time.ToString()));
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.CannotLaunchAircraftHangarDamaged, this.Id));
                }
                return false;
            }
			if (string.IsNullOrEmpty(groupId))
			{
				group = new Group();
			}
			else
			{
				group = OwnerPlayer.GetGroupById(groupId);
				if (group == null)
				{
					GameManager.Instance.Log.LogWarning(
						"LaunchAircraft: No group found for Id=" + groupId + ". New group created.");
					group = new Group();
				}
			}
			if(AircraftHangar == null)
			{
				GameManager.Instance.Log.LogWarning(
					"LaunchAircraft: Unit " + ToShortString() + " has no AircraftHangar.");

				return false;
			}
			if (!AircraftHangar.IsReady)
			{
                if (AircraftHangar.ReadyInSec > GameConstants.DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC)
				{
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.CannotLaunchAircraftHangarDamaged, this.Id));
					TimeSpan time = TimeSpan.FromSeconds(AircraftHangar.ReadyInSec);
					GameManager.Instance.Log.LogWarning(string.Format(
						"LaunchAircraft: {0} could not launch aircraft. Hangar and/or runway ready in {1}.",
						ToShortString(), time.ToString()));
					return false;
				}
				else // enqueue launch order for later
				{
					LaunchAircraftOrder launchOrder = new LaunchAircraftOrder(OwnerPlayer);
					foreach (var u in listUnits)
					{
						launchOrder.UnitList.Add(u);
					}
					launchOrder.GroupId = group.Id;
					if (orders != null)
					{
						launchOrder.Orders = orders;
					}
					Orders.Enqueue(launchOrder);
					return true; 
				}
			}
			GameManager.Instance.Log.LogDebug(
				string.Format("LaunchAircraft: Unit {0} to launch {1} aircraft.", ToShortString(), listUnits.Count));
			if(orders != null && orders.Count > 0)
			{
				foreach(var order in orders)
				{
					GameManager.Instance.Log.LogDebug("     Order: " + order.ToString());
				}
			} 
			else
			{
				GameManager.Instance.Log.LogDebug("-- Launch order has no orders.");
			}
 
			double crashChancePercent = 0;
			int maxSimultanousTakeoffs = UnitClass.MaxSimultanousTakeoffs;
			if (GetEffectiveSeaState() > GameConstants.AIRCRAFT_TAKEOFF_MAX_SEA_STATE)
			{
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftTakeoffFailed, this.Id));
				GameManager.Instance.Log.LogDebug(string.Format(
					"Unit {0} could not launch aircraft due to rough sea. Max sea state is {1}," 
					+ "effective sea state is {2}", 
					ToShortString(), GameConstants.AIRCRAFT_TAKEOFF_MAX_SEA_STATE, 
					GetEffectiveSeaState()));
				return false;
			}
			else if (GetEffectiveSeaState() == GameConstants.AIRCRAFT_TAKEOFF_MAX_SEA_STATE)
			{
				crashChancePercent = 25.0;
			}
			int takeOffCount = 0;
			DetectedUnit targetDetectedUnit = null;
			if (orders != null && orders.Count > 0)
			{
				foreach (var o in orders)
				{
					if (o is EngagementOrder)
					{
						var engOrder = o as EngagementOrder;
						if (engOrder != null)
						{
							targetDetectedUnit = engOrder.TargetDetectedUnit;
						}
					}
				}
			}

			foreach(AircraftUnit craft in listUnits)
			{
				if (craft.IsReady && craft.CarriedByUnit == this)
				{
					MakeExtraordinaryNoise(200, 20);
					AircraftHangar.Aircraft.Remove(craft);
					craft.SetActualSpeed(UnitClass.CruiseSpeedKph); //at launch, units are up to speed at once
                    craft._GameWorldTimeNextTerrainCheck = -1; //force terrain check immediately
					craft.Position = Position.Clone();
					craft.Position.HeightOverSeaLevelM = Position.HeightOverSeaLevelM + (UnitClass.HeightM * 0.2);
                    if (this.DomainType == GameConstants.DomainType.Surface && craft.UnitClass.UnitType == GameConstants.UnitType.Helicopter && this.Position.HasBearing)
                    {
                        var bearingDeg = (double)this.Position.BearingDeg;
                        bearingDeg = (bearingDeg.ToRadian() + Math.PI).ToDegreeBearing();
                        var distM = this.UnitClass.LengthM / 3.0;
                        var newCoord = MapHelper.CalculateNewPosition2(this.Position.Coordinate, bearingDeg, distM);
                        craft.Position.Coordinate = newCoord.Clone();
                    }
					craft.CarriedByUnit = null;
					craft.WeaponOrders = OwnerPlayer.DefaultWeaponOrders;
					craft.LaunchedFromUnit = this;
					craft.MissionType = GameConstants.MissionType.Patrol; //default
					if (targetDetectedUnit != null)
					{
						craft.MissionType = GameConstants.MissionType.Attack;
						craft.TargetDetectedUnit = targetDetectedUnit;
					}
                    if (craft.UnitClass.UnitType != GameConstants.UnitType.Helicopter)
                    {
                        Waypoint wp = new Waypoint(craft.Position.Offset(new PositionOffset(Position.BearingDeg.Value, 1500))); //initially move forward
                        wp.IsNotRecurring = true;
                        if (craft.DomainType == GameConstants.DomainType.Air)
                        {
                            wp.Position.HeightOverSeaLevelM = GameConstants.HEIGHT_LOW_MIN_M;
                        }
                        craft.MovementOrder.AddWaypoint(wp);
                    }
                    craft.UserDefinedElevation = GameConstants.HeightDepthPoints.Low;
                    if ( crashChancePercent > 0 && GameManager.Instance.ThrowDice( crashChancePercent ) )
                    {
                        var dmgMsg = new GameStateInfo( GameConstants.GameStateInfoType.AircraftCrashedOnTakeoffRoughWeather, craft.Id, this.Id );
                        dmgMsg.SecondaryId = this.Id;
                        OwnerPlayer.Send( dmgMsg );
                        craft.IsMarkedForDeletion = true;
                        GameManager.Instance.Log.LogDebug( string.Format(
                            "Unit {0} crashed due to rough sea. Effective sea state is {1}",
                            ToShortString(),
                            GetEffectiveSeaState() ) );
                    }
                    else
                    {
                        // Only set this when the craft actually launched to avoid sending redundant data on next tick.
                        craft.SetDirty( GameConstants.DirtyStatus.NewlyCreated );

                        OwnerPlayer.Send(
                        new GameStateInfo( GameConstants.GameStateInfoType.AircraftTakeoff, craft.Id, this.Id )
                        {
                            UnitClassId = craft.UnitClass.Id,
                            BearingDeg = ( double ) craft.Position.BearingDeg,
                        } );
                    }
					group.AddUnit(craft);
					if (craft.Id == group.MainUnit.Id) //orders only added to first/main unit of launch
					{
						if (!string.IsNullOrEmpty(tag))
						{
							craft.Tag = tag;
							group.Tag = tag;
						}
						if (orders != null && orders.Count > 0)
						{
							foreach (var order in orders)
							{
								if (order is EngagementOrder)
								{
									MissionType = GameConstants.MissionType.Attack;
									var engOrder = order as EngagementOrder;
									if (engOrder != null && engOrder.TargetDetectedUnit != null)
									{
										MissionTargetType = GetMissionTargetTypeFromDetectedUnit(engOrder.TargetDetectedUnit);
									}
								}
								craft.Orders.Enqueue(order);
							}
						}
					}
					else
					{
						if (group != null)
						{
							group.AutoAssignUnitsToFormation();
						}
						craft.SetUnitFormationOrder();
					}
					AircraftHangar.ReadyInSec = GameConstants.DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC; 
					takeOffCount++;
					if (takeOffCount >= maxSimultanousTakeoffs && listUnits.Count > takeOffCount)
					{
						GameManager.Instance.Log.LogDebug("LaunchAircraft: Reached maxsimultanoustakeoffs. Recreates order.");
						break;
					}
				}
				else
				{
					GameManager.Instance.Log.LogWarning("BaseUnit.LaunchAircraft: could not launch aircraft " 
													   + craft.ToString() + " from " + this.Name);

				}
			}
		    var remainingList = listUnits.Where(aircraft => aircraft.Group != group).ToList();
		    if (remainingList.Count > 0)
			{
				BaseUnit carriedByUnit = this;
				LaunchAircraftOrder launchOrder = new LaunchAircraftOrder(OwnerPlayer);
				foreach(var u in remainingList)
				{
					if (u.CarriedByUnit != null)
					{
						carriedByUnit = u.CarriedByUnit;
						launchOrder.UnitList.Add(u);
					}
				}
				if (launchOrder.UnitList.Count > 0)
				{
					launchOrder.GroupId = group.Id;
					GameManager.Instance.Log.LogDebug("LaunchAircraft: Enqueues new launch order with "
						+ launchOrder.UnitList.Count + " units.");
					carriedByUnit.Orders.Enqueue(launchOrder);
				}
			}
			group.Name = groupName;
			if(string.IsNullOrEmpty(group.Name))
			{
				group.Name = this.Name + " aircraft group";
			}
			if (group.Units.Count == 0)
			{
				GameManager.Instance.Log.LogWarning("BaseUnit.LaunchAircraft: could not launch any aircraft from " + this.Name);
			}
			else
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				if (group.Units.Count > 1)
				{
					group.AutoAssignUnitsToFormation();
				}
			}
			bool result = (group.Units.Count > 0);
			group.RemoveIfSingleUnit();
			return result;
		}
		/// <summary>
		/// Launches one or more carried aircraft from a unit.
		/// </summary>
		/// <param name="listUnits">A list of string specifying Ids of aircraft to be launched</param>
		/// <param name="groupId">If non-empty, the id of the group the launched units will join</param>
		/// <param name="groupName">If non-empty, specifies name of group</param>
		/// <param name="orders">A List of orders to be gives to first unit in group</param>
		/// <param name="tag">An optional tag for the units</param>
		/// <returns></returns>
        //public virtual bool LaunchAircraft(List<string> listUnits, string groupId, string groupName, List<BaseOrder> orders, string tag)
        //{
        //    var hangar = AircraftHangar;
        //    var aircraftList = listUnits.Select(unitCode => hangar.Aircraft.Find(a => a.Id == unitCode)).Where(unit => unit != null).ToList();
        //    return LaunchAircraft(aircraftList, groupId, groupName, orders, tag);
        //}

		/// <summary>
		/// Specified a new formation for a group, and executes the changes
		/// </summary>
		/// <param name="formation"></param>
		public virtual void SetNewGroupFormation(Formation formation)
		{
			if (Group == null)
			{ 
				GameManager.Instance.Log.LogError(
					string.Format("{0} is not part of a group and thus has no formation.", ToShortString()));
				return;
			}
			if (formation == null)
			{
				Group.Formation = null;
			}
			else
			{
				Group.Formation = formation;
				GameManager.Instance.Log.LogDebug(
					string.Format("SetNewGroupFormation: Unit {0} has received new Formation:\n{1}",
					ToShortString(), Group.Formation.ToString()));
				Group.AutoAssignUnitsToFormation();
				foreach (var unit in Group.Units)
				{
					if (unit.Id != Group.MainUnit.Id)
					{
						unit.SetUnitFormationOrder();
					}
					unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
				}
			}
			Group.DirtySetting = GameConstants.DirtyStatus.UnitChanged;
		}

		/// <summary>
		/// For units in a group, creates a new UnitFormationOrder and executes it. Will load default formations
		/// if necessary and assign units to positions.
		/// </summary>
		public virtual void SetUnitFormationOrder()
		{
			if(!IsInGroupWithOthers() || IsGroupMainUnit())
			{
				return;
			}
			var formPos = GetFormationPosition();
			if (formPos == null)
			{
				//TODO: Attempt to auto-assign a position
				return;
			}
			ActualSpeedKph = GetGroupSpeedFromSpeedType(UserDefinedSpeed); //hmm.
			var formationOrder = new MovementFormationOrder(Group.MainUnit, formPos.PositionOffset);
			this.MovementOrder = formationOrder;
			if (Group != null)
			{
			    Group.CheckIfGroupIsStaging();
			}
		}

		/// <summary>
		/// Returns the formation position if unit is in a group. Not valid for main unit; will return null.
		/// </summary>
		/// <returns></returns>
		public FormationPosition GetFormationPosition()
		{
			if (!IsInGroupWithOthers() || IsGroupMainUnit())
			{
				return null;
			}
			if (Group == null)
			{
				GameManager.Instance.Log.LogError(
					string.Format("Unit {0} fails to find group Id {1}", ToShortString(), this.GroupId));
				return null;
			}
			if (Group.Formation != null)
			{
				return Group.Formation.GetFormationPositionById(this.FormationPositionId);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the current PositionInfo of the unit, giving its position and other related data.
		/// The PositionInfo object is transmitted to the game client.
		/// </summary>
		/// <returns></returns>
		public virtual PositionInfo GetPositionInfo()
		{
			if (Position == null)
			{
				return null;
			}
			PositionInfo pos = this.Position.GetPositionInfo();
			pos.UnitId = this.Id;
			pos.BingoFuelPercent = (int)BingoFuelPercent;
            if ( MaxRangeCruiseM > 0 )
            {
                pos.FuelDistanceRemainingM = (float)FuelDistanceRemainingM;
            }
			pos.IsDetection = false;
            pos.ActualSpeedKph = (float)this.ActualSpeedKph;
            //pos.ActualSpeed = GetSpeedTypeFromKph(this.ActualSpeedKph);
            //pos.MostRecentDistanceToTargetM = (float)this.MostRecentDistanceToTargetM;
            //if (this.DesiredBearingDeg != null)
            //{
            //    pos.DesiredBearingDeg = (float)this.DesiredBearingDeg.Value;
            //}
            //if (this.Position.HasHeightOverSeaLevel)
            //{
            //    pos.HeightOverSeaLevelM = (float)this.Position.HeightOverSeaLevelM.Value;
            ////    //pos.HeightDepth = pos.HeightOverSeaLevelM.ToHeightDepthMark();
            //}
            //if (this.MovementOrder is MovementFormationOrder)
            //{
            //    MovementFormationOrder formationOrder = this.MovementOrder as MovementFormationOrder;
            //    if (formationOrder != null)
            //    {
            //        try
            //        {
            //            if (formationOrder.UnitToFollow != null && formationOrder.UnitToFollow.Position != null)
            //            {
            //                //pos.BearingToTargetPosDeg = (float)MapHelper.CalculateBearingDegrees(
            //                //    this.Position.Coordinate, formationOrder.UnitToFollow.Position.Coordinate);
            //                //pos.DistanceToTargetPosM = (float)MapHelper.CalculateDistanceM(
            //                //    this.Position.Coordinate, formationOrder.UnitToFollow.Position.Coordinate);
            //                pos.IsFormationMovementOrder = true;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            GameManager.Instance.Log.LogError(
            //                "GetPositionInfo failed to calculate offset positions for FormationOrder. " + ex.ToString()); 
            //        }
            //    }
            //}
            //else if (this.ActiveWaypoint != null && this.Position != null && this.ActiveWaypoint.Position != null)
            //{
            //    //pos.DestinationLatitude = ActiveWaypoint.Position.Coordinate.LatitudeDeg;
            //    //pos.DestinationLongitude = ActiveWaypoint.Position.Coordinate.LongitudeDeg;
            //    pos.DistanceToTargetPosM = (float)MapHelper.CalculateDistanceM(
            //        this.Position.Coordinate, ActiveWaypoint.Position.Coordinate);
            //    pos.BearingToTargetPosDeg = (float)MapHelper.CalculateBearingDegrees(
            //        this.Position.Coordinate, ActiveWaypoint.Position.Coordinate);
            //}

            pos.EtaCurrentWaypointSec = (float)this.EtaCurrentWaypoint.TotalSeconds;
            pos.EtaAllWaypointsSec = (float)this.EtaAllWaypoints.TotalSeconds;

            pos.IsAtFormationPosition = this.IsAtFormationPositionFlag;
			return pos;
		}

		/// <summary>
		/// Returns a CarriedUnitInfo for this unit, assuming it is currently on an airport/carrier.
		/// </summary>
		/// <returns></returns>
		public virtual CarriedUnitInfo GetCarriedUnitInfo()
		{
			var info = new CarriedUnitInfo();
			if (CarriedByUnit != null)
			{
				info.CarriedByUnitId = CarriedByUnit.Id;
			}
			info.Id = Id;
            //info.HitPoints = HitPoints;
            info.DamagePercent = ( int )this.DamagePercent();
			info.ReadyInSec = ReadyInSec;
			info.UnitClassId = UnitClass.Id;
			info.UnitName = Name;
			info.CurrentWeaponLoadName = CurrentWeaponLoadName;
            //info.UnitClassName = UnitClass.UnitClassShortName;
			if (this.MaxRangeCruiseM > 0)
			{
				info.MaxOperatingDistanceM = this.MaxRangeCruiseM;
				info.MaxOperatingRangeM = info.MaxOperatingDistanceM / 2;
			}

			foreach (var role in this.RoleList)
			{
				info.RoleList.Add(role);
			}
			foreach (var wpnLoads in this.UnitClass.WeaponLoads)
			{
				if(this.CanChangeToWeaponLoad(wpnLoads, false))
				{
					info.AvailableWeaponLoads.Add(wpnLoads.Name);
				}
			}
			info.UnitSubType = GetUnitSubType();
			return info;
		}

		/// <summary>
		/// Returns a BaseUnitInfo for this unit, detailing all unit data. A BaseUnitInfo is
		/// transmitted to the client.
		/// </summary>
		/// <returns></returns>
		public virtual BaseUnitInfo GetBaseUnitInfo()
		{
			var info = new BaseUnitInfo();
            //info.HitPoints = this.HitPoints;
			info.DamagePercent = (int)this.DamagePercent();
            info.IsCivilianUnit = this.IsCivilianUnit;
			info.Id = this.Id;
            //info.Tag = this.Tag;
			info.UnitName = this.Name;
			info.UnitClassId = this.UnitClass.Id;
			info.Position = GetPositionInfo();
			info.GroupId = GroupId;
			info.ReadyInSec = (float)ReadyInSec;
            info.TimeToLiveRemainingSec = (float)TimeToLiveSec;
            info.CalculatedMaxRangeCruiseM = (float)MaxRangeCruiseM;
			info.IsMarkedForDeletion = IsMarkedForDeletion;
			info.CurrentWeaponLoadName = CurrentWeaponLoadName;
			info.FormationPositionId = this.FormationPositionId;
			info.MissionType = this.MissionType;
			info.MissionTargetType = this.MissionTargetType;
            info.UserDefinedSpeed = this.UserDefinedSpeed;
            if (this.UserDefinedElevation != null)
            {
                info.UserDefinedElevation = this.UserDefinedElevation.Value;
                info.IsUserDefinedElevationSet = true;
            }
            else
            {
                info.IsUserDefinedElevationSet = false;
            }

            if (TargetDetectedUnit != null)
			{
				info.TargetDetectedUnitId = TargetDetectedUnit.Id;
			}
			// info.DomainType = this.DomainType;
			if (!this.UnitClass.IsFixed && !this.UnitClass.IsMissileOrTorpedo 
				&& this.UnitClass.UnitType != GameConstants.UnitType.Sonobuoy 
				&& this.UnitClass.UnitType != GameConstants.UnitType.Mine)
			{
				info.SupportsOrderType.Add(GameConstants.OrderType.MovementOrder);
                if (this.UnitClass.GetPossibleSpeedTypes().Count > 1)
				{
					info.SupportsOrderType.Add(GameConstants.OrderType.SetSpeed);
				}
                if (this.UnitClass.GetPossibleHeightDepth().Count > 1)
				{
					info.SupportsOrderType.Add(GameConstants.OrderType.SetElevation);
				}
				if (this.LaunchedFromUnit != null)
				{
					info.SupportsOrderType.Add(GameConstants.OrderType.ReturnToBase);
				}
				info.SupportsOrderType.Add(GameConstants.OrderType.JoinGroups);
				if (!string.IsNullOrEmpty(this.GroupId))
				{
					info.SupportsOrderType.Add(GameConstants.OrderType.SplitGroup);
				}
				
			}
			if(!string.IsNullOrEmpty(this.GroupId))
			{
				//Group gr = OwnerPlayer.Groups.GetById(this.GroupId);
				info.IsGroupMainUnit = IsGroupMainUnit();
				if (MovementOrder is MovementFormationOrder)
				{ 
					var formOrder = MovementOrder as MovementFormationOrder;
					info.PositionOffset = formOrder.PositionOffset;
				}
				else if (info.IsGroupMainUnit)
				{
					info.PositionOffset = new PositionOffset(0, 0, 0);
				}
				if (!info.IsGroupMainUnit)
				{
					info.SupportsOrderType.Add(GameConstants.OrderType.MovementFormationOrder);
				}
			}
			
            //info.HasLightingOn = HasLightingOn;
			info.FireLevel = FireLevel;
            info.CalculatedMaxSpeedKph = (float)GetMaxSpeedKph();
			WeatherSystem wsystem = GetWeatherSystem();
			if(wsystem != null)
			{
				info.WeatherSystem = wsystem.GetWeatherSystemInfo();
			}

			if (MovementOrder != null && MovementOrder.CountWaypoints > 0)
			{
                var waypoints = MovementOrder.GetWaypoints().Where(wp => wp != null && wp.Position != null);
                foreach (var wp in waypoints)
			    {
			        info.Waypoints.Add(wp.GetWaypointInfo());
			    }
			}
		    if (MovementOrder != null && MovementOrder is MovementFormationOrder)
			{
				info.HasFormationOrder = true;
			}
			if(CarriedByUnit != null)
			{
				info.CarriedByUnitId = CarriedByUnit.Id;
			}
			if (UnitClass.CanCarryUnits && AircraftHangar != null)
			{
				info.SupportsOrderType.Add(GameConstants.OrderType.LaunchOrder);
				foreach (var aircraft in AircraftHangar.Aircraft)
				{
					info.CarriedUnits.Add(aircraft.GetCarriedUnitInfo());
				}
			}
			info.WeaponOrders = this.WeaponOrders;

			if (this.RoleList.Count > 0)
			{ 
				foreach(var role in RoleList)
				{
					info.RoleList.Add(role);
				}
			}
			foreach (var order in Orders)
			{
				info.OrderQueue.Add(order.GetBaseOrderInfo());
			}
			foreach (var wpnStore in CarriedWeaponStores)
			{
				info.CarriedWeaponStores.Add(wpnStore);
			}
			if (Weapons.Any(w => !w.WeaponClass.IsNotWeapon))
			{
				info.SupportsOrderType.Add(GameConstants.OrderType.EngagementOrder);
			}
			foreach (var wp in Weapons)
			{
				if (wp.WeaponClass.SpecialOrders != GameConstants.SpecialOrders.None)
				{
					info.SupportsSpecialOrders.Add(wp.WeaponClass.SpecialOrders);
					if (!info.SupportsOrderType.Contains(GameConstants.OrderType.SpecialOrders))
					{
						info.SupportsOrderType.Add(GameConstants.OrderType.SpecialOrders);
					}
				}
				info.Weapons.Add(wp.GetWeaponInfo());
			}
            if (!UnitClass.IsMissileOrTorpedo && Sensors.Any())
			{
				info.SupportsOrderType.Add(GameConstants.OrderType.SensorActivationOrder);
				info.SupportsOrderType.Add(GameConstants.OrderType.SensorDeploymentOrder);
			}
			info.IsUsingActiveRadar = false;
			info.IsUsingActiveSonar = false;
			foreach (var sens in Sensors)
			{
				if (sens.SensorClass.SensorType == GameConstants.SensorType.Radar)
				{
					if (sens.SensorClass.IsPassiveActiveSensor && sens.IsActive && sens.IsOperational)
					{
						info.IsUsingActiveRadar = true;
					}
				}
				if (sens.SensorClass.SensorType == GameConstants.SensorType.Sonar)
				{
					if (sens.SensorClass.IsPassiveActiveSensor && sens.IsActive && sens.IsOperational)
					{
						info.IsUsingActiveSonar = true;
					}

				}
				info.Sensors.Add(sens.GetSensorInfo());
			}
            info.ActualSpeedKph = (float)ActualSpeedKph;
            //info.ActualSpeed = this.GetSpeedTypeFromKph(ActualSpeedKph);

			info.UnitSubType = GetUnitSubType();
			if (this is MissileUnit)
			{
				var thisMissile = (MissileUnit)this;
				info.LaunchPlatformId = thisMissile.LaunchPlatform.Id;
				info.LaunchWeaponClassId = thisMissile.WeaponClassId;
			}
            else
            {
                if ( LaunchedFromUnit != null )
                {
                    info.LaunchPlatformId = LaunchedFromUnit.Id;
                }
            }
            //info.IsAtFormationPosition = this.IsAtFormationPositionFlag;
			return info;

		}

		/// <summary>
		/// Returns the Damage as a percent for the unit. 0 is undamaged, 100 is destroyed.
		/// </summary>
		/// <returns></returns>
		public int DamagePercent()
		{
			Debug.Assert(UnitClass != null, "UnitClass should never be null.");
			Debug.Assert(UnitClass.MaxHitpoints > 0, "UnitClass.MaxHitpoints must be > 0");
			int damageHP = UnitClass.MaxHitpoints - HitPoints;
			if(damageHP <= 0)
			{
				return 0;
			}
			else 
			{
				double damagePercent = ((double)damageHP / (double)UnitClass.MaxHitpoints) * 100.0;
				return (int)Math.Round(damagePercent);
			}
		}

		/// <summary>
		/// Returns true if this unit is currently in another player's communication jamming area.
		/// In that case, orders will not go through to unit.
		/// </summary>
		/// <returns></returns>
		public bool IsCommunicationJammingCurrentlyInEffect()
		{
			if (Position == null)
			{
				return false;
			}
			var finder = new BlackboardFinder<AreaEffect>();
			var allAreaEffects = finder.GetAllSortedByCoordinateAndType(Position.Coordinate, GameConstants.MAX_DETECTION_DISTANCE_M);
			var relevantEffects = allAreaEffects.Where(
				j => (j is AreaEffect) && (j as AreaEffect).TimeToLiveSec >= 0
				&& ((j as AreaEffect).AreaEffectType == GameConstants.AreaEffectType.JammerCommunicationDegradation)
				&& j.OwnerId != OwnerPlayer.Id
				&& j.DistanceToM(Position.Coordinate) <= (j as AreaEffect).RadiusM);
			return relevantEffects.Any();
		}

		/// <summary>
		/// Executes a order to deploy a special weapon (ie mine or sonobuy drop, jamming, etc).
		/// </summary>
		/// <param name="order"></param>
		public virtual void ExecuteSpecialOrder(BaseOrder order)
		{
			if(this.Position == null)
			{
				return;
			}
			if (order.Position != null)
			{
				double distanceM = MapHelper.CalculateDistanceM(order.Position.Coordinate, this.Position.Coordinate);
				double maxDistance = GameConstants.DISTANCE_TO_TARGET_IS_HIT_M * 2;
				if (order.SpecialOrders == GameConstants.SpecialOrders.JammerCommunicationDegradation ||
					order.SpecialOrders == GameConstants.SpecialOrders.JammerRadarDegradation || 
					order.SpecialOrders == GameConstants.SpecialOrders.DropMine ||
					order.SpecialOrders == GameConstants.SpecialOrders.JammerRadarDistortion)
				{
					var wpn = GetSpecialWeapon(order.SpecialOrders);
					if (wpn != null)
					{
						maxDistance = wpn.WeaponClass.MaxWeaponRangeM;
					}
				}
				if (distanceM > maxDistance)
				{
					Waypoint wp = new Waypoint(order.Position);
					wp.Orders.Add(order);
					if (MovementOrder is MovementFormationOrder)
					{
						MovementOrder = new MovementOrder(wp);
					}
					else
					{
						MovementOrder.AddWaypointToTop(wp);
					}
					return;
				}
			}

			switch (order.SpecialOrders)
			{
				case GameConstants.SpecialOrders.None:
					//welll....
					break;
				case GameConstants.SpecialOrders.DropSonobuoy:
					ExecuteDropSonobuoy(order);
					break;
				case GameConstants.SpecialOrders.DropMine:
					ExecuteDropMine(order);
					break;
				case GameConstants.SpecialOrders.JammerRadarDegradation:
				case GameConstants.SpecialOrders.JammerCommunicationDegradation:
				case GameConstants.SpecialOrders.JammerRadarDistortion:
					ExecuteJammer(order);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Executes an mine drop order
		/// </summary>
		/// <param name="order">Order specifying the details of the mine drop</param>
		public void ExecuteDropMine(BaseOrder order)
		{
			BaseWeapon weapon = GetSpecialWeapon(order.SpecialOrders);
			if (weapon == null)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
                        string.Format("ExecuteDropMine: {0} has no capability to drop mines.", ToShortString()));
				}
				return;
			}
			if (weapon.AmmunitionRemaining < 1)
			{
				GameManager.Instance.Log.LogError(
                    string.Format("ExecuteDropMine: {0} is out of mines.", ToShortString()));
                return;
			}
            if (TerrainReader.GetHeightM(order.Position.Coordinate) > 0)
            {
					GameManager.Instance.Log.LogError(
                        string.Format("ExecuteDropMine: {0} ordered to drop mines on land. Ignored.", ToShortString()));
                    return;
            }
			double distanceToTargetM = MapHelper.CalculateDistance3DM(order.Position, this.Position);
			if (distanceToTargetM > weapon.WeaponClass.MaxWeaponRangeM)
			{
				double distanceToClose = distanceToTargetM - weapon.WeaponClass.EffectiveWeaponRangeM;
				double bearingToTargetDeg = MapHelper.CalculateBearingDegrees(
					this.Position.Coordinate, order.Position.Coordinate);
				var newPos = MapHelper.CalculateNewPosition2(this.Position.Coordinate, bearingToTargetDeg, distanceToClose);
				Waypoint wp = new Waypoint(newPos);
				wp.Orders.Add(order);
				if (MovementOrder == null || MovementOrder is MovementFormationOrder)
				{
					MovementOrder = new MovementOrder();
				}
				MovementOrder.AddWaypointToTop(wp);
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogDebug(
                        string.Format("ExecuteDropMine: {0} is closing {1}km to deploy mine(s).", ToShortString(), Math.Floor(distanceToClose / 1000.0)));
				}
				return;
			}
			//ok, we are there, and we have mines.
			int noOfMines = 1;
			if (order.IsParameter) //means that unit is ordered to deploy all mines it has
			{
				noOfMines = weapon.AmmunitionRemaining;
			}
			List<Coordinate> coordList = new List<Coordinate>();
			coordList.Add(order.Position.Coordinate.Clone());

			if (noOfMines > 1)
			{
				coordList = MapHelper.CreateCoordinateGrid(
					order.Position.Coordinate.Clone(), 
					noOfMines, 
					GameConstants.MINE_FIELD_DISTANCE_BETWEEN_MINES_M);
			}
			int minesPut = 0;
			Group group = new Group();
			foreach (var c in coordList)
			{
				if (minesPut > noOfMines)
				{
					break;
				}
				Position minePos = new Position(c);
                if (TerrainReader.GetHeightM(minePos.Coordinate) > 0) //on land, ignore and move on to next
                {
                    continue;
                }
				minePos.HeightOverSeaLevelM = -50;
				minePos.BearingDeg = 90;
				var mineUnit = GameManager.Instance.GameData.CreateUnit(
					this.OwnerPlayer, 
					group, 
					weapon.WeaponClass.SpawnUnitClassId, 
					string.Empty, 
					minePos, 
					true, true);
				var mine = mineUnit as Mine;
				if (mine == null)
				{
					GameManager.Instance.Log.LogError("ExecuteDropMine: Mine is null!");
                    continue;
				}
				mine.HitPercent = 100;
				mine.DamageHitpoints = weapon.WeaponClass.DamageHitPoints;
				mine.ReadyInSec = 5*60; //TODO: Should have a system for mine readying time
				minesPut++;
				mine.LaunchWeapon = weapon;
				mine.LaunchPlatform = this;
			}
			weapon.AmmunitionRemaining -= noOfMines;
			if(weapon.AmmunitionRemaining < 0)
			{
				weapon.AmmunitionRemaining = 0;
			}
			group.RemoveIfSingleUnit();
			GameManager.Instance.Log.LogDebug(
				string.Format("ExecuteDropMine: {0} has deployed {1} mine(s).", ToShortString(), noOfMines));
			
		}

		/// <summary>
		/// Executes a jamming order for this unit.
		/// </summary>
		/// <param name="order">Order specifying jamming type and position</param>
		public void ExecuteJammer(BaseOrder order)
		{
			BaseWeapon weapon = GetSpecialWeapon(order.SpecialOrders);
			string jammingTypeName = GameManager.Instance.GetJammingNameFromSpecialOrder(order.SpecialOrders);
			if (weapon == null)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
                        string.Format("ExecuteJammer: {0} has no capability for {1}.", ToShortString(), jammingTypeName));
				}
                return;
			}

			if (weapon.AmmunitionRemaining < 1)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
                        string.Format("ExecuteJammer: {0} has no more 'ammo' for {1}.", ToShortString(), jammingTypeName));
				}
                return;
			}

			double distanceToTargetM = MapHelper.CalculateDistance3DM(order.Position, this.Position);
			if (distanceToTargetM > weapon.WeaponClass.MaxWeaponRangeM)
			{
				double distanceToClose = distanceToTargetM - weapon.WeaponClass.EffectiveWeaponRangeM;
				double bearingToTargetDeg = MapHelper.CalculateBearingDegrees(
					this.Position.Coordinate, order.Position.Coordinate);
				var newPos = MapHelper.CalculateNewPosition2(this.Position.Coordinate, bearingToTargetDeg, distanceToClose);
				Waypoint wp = new Waypoint(newPos);
				wp.Orders.Add(order);
				if (MovementOrder == null || MovementOrder is MovementFormationOrder)
				{
					MovementOrder = new MovementOrder();
				}
				MovementOrder.AddWaypointToTop(wp);
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
				return;
			}
			weapon.AmmunitionRemaining--;
			GameConstants.AreaEffectType areaEffectType = GameConstants.AreaEffectType.JammerRadarDegradation;
			if(order.SpecialOrders == GameConstants.SpecialOrders.JammerRadarDistortion)
			{
				areaEffectType = GameConstants.AreaEffectType.JammerRadarDistortion;
			}
			else if (order.SpecialOrders == GameConstants.SpecialOrders.JammerCommunicationDegradation)
			{
				areaEffectType = GameConstants.AreaEffectType.JammerCommunicationDegradation;
			}
			AreaEffect ae = new AreaEffect(areaEffectType);
			ae.Coordinate = order.Position.Coordinate;
			ae.Strength = weapon.WeaponClass.StrengthPercent;
			ae.TimeToLiveSec = weapon.WeaponClass.MaxEffectDurationSec;
			ae.OwnerId = OwnerPlayer.Id;
			ae.RadiusM = weapon.WeaponClass.EffectRangeM;
			ae.Weapon = weapon;
			BlackboardController.Objects.Add(ae);
            var jamMsg = new GameStateInfo(GameConstants.GameStateInfoType.JammingOrderExecuted, this.Id);
            jamMsg.WeaponClassId = weapon.WeaponClass.Id;
            jamMsg.RadiusM = ae.RadiusM;
            if (order.Position != null)
            {
                jamMsg.Position = order.Position.Clone().GetPositionInfo();    
            }
            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            OwnerPlayer.Send(jamMsg);
		}

        /// <summary>
        /// When detected, a unit (missile or other) 
        /// </summary>
        /// <param name="detectedUnit"></param>
        public void AddTargetingDetectedUnit(DetectedUnit detectedUnit)
        {
            if (!_DetectedUnitsTargettingThis.Contains(detectedUnit))
            {
                _DetectedUnitsTargettingThis.Add(detectedUnit);
            }
        }

        public List<DetectedUnit> DetectedUnitsTargettingThis
        {
            get
            {
                ForgetOldTargettingThis();
                return _DetectedUnitsTargettingThis;
            }
        }

        public List<DetectedUnit> DetecedMissilesTargettingThis
        {
            get
            {
                
                return _DetectedUnitsTargettingThis.Where(d 
                    => d.DetectionClassification == GameConstants.DetectionClassification.BallisticMissile 
                    || d.DetectionClassification == GameConstants.DetectionClassification.Missile
                    || d.DetectionClassification ==  GameConstants.DetectionClassification.Torpedo).ToList<DetectedUnit>();
            }
        }

        private void ForgetOldTargettingThis()
        {
            _DetectedUnitsTargettingThis.RemoveAll(d => d.IsMarkedForDeletion);
            _DetectedUnitsTargettingThis.RemoveAll(d => !d.IsKnownToTargetUnit(this, true));
        }

		/// <summary>
		/// Creates and queues an EngagementOrder to engage an entire DetectedGroup
		/// </summary>
		/// <param name="detectedGroup"></param>
		/// <param name="engagementOrderType"></param>
		/// <param name="isGroupAttack"></param>
		/// <returns></returns>
		public bool EngageDetectedGroup(DetectedGroup detectedGroup, GameConstants.EngagementOrderType engagementOrderType,
					GameConstants.EngagementStrength engagementStrength, bool isGroupAttack)
		{

			return OwnerPlayer.AIHandler.EngageDetectedGroup(this, detectedGroup, engagementOrderType, engagementStrength, string.Empty, isGroupAttack);
		}

		public bool EngageDetectedGroup(EngagementOrder order)
		{
			return OwnerPlayer.AIHandler.EngageDetectedGroup(this, order);
		}


        //public void EngageDetectedUnit(EngagementOrder engageOrder, bool clearExistingCloseEngageOrders)
        //{
        //    if (engageOrder.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage && clearExistingCloseEngageOrders)
        //    {
        //        //if (TargetDetectedUnit != null && CurrentEngagementOrder != null)
        //        //{
        //        //    Orders.Enqueue(CurrentEngagementOrder); //next target
        //        //}
        //        this.TargetDetectedUnit = null;
        //        this.CurrentEngagementOrder = null;
        //        //TargetDetectedUnit = engageOrder.TargetDetectedUnit;
        //        Orders.Insert(0, engageOrder);
        //        ExecuteOrders();
        //        return;

        //        //var target = engageOrder.TargetDetectedUnit;
        //        //if (target != null)
        //        //{
        //        //    if (ActiveWaypoint != null && ActiveWaypoint.TargetDetectedUnit != null)
        //        //    {
        //        //        MovementOrder.ActivateNextWaypoint();
        //        //    }
        //        //}
        //    }
        //    Orders.Enqueue(engageOrder);
        //}

		/// <summary>
		/// Creates and queues an EngagementOrder for a target. If this unit already
		/// has an order to engage the same unit, this order is ignored.
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <param name="engagementOrderType"></param>
		/// <param name="isGroupAttack"></param>
		/// <returns></returns>
		public bool EngageDetectedUnit(DetectedUnit detectedUnit,
					GameConstants.EngagementOrderType engagementOrderType,
					bool isGroupAttack)
		{
			return EngageDetectedUnit(detectedUnit, engagementOrderType, string.Empty, isGroupAttack, false);
		}

		/// <summary>
		/// Creates and queues an EngagementOrder for a target. If this unit already
		/// has an order to engage the same unit, this order is ignored.
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <param name="engagementOrderType"></param>
		/// <param name="isGroupAttack"></param>
		/// <param name="immediateExecution"></param>
		/// <returns></returns>
		public bool EngageDetectedUnit(DetectedUnit detectedUnit,
			GameConstants.EngagementOrderType engagementOrderType, 
            string weaponClassId,
			bool isGroupAttack, 
			bool immediateExecution)
		{
			if (!HasAnyEngagementOrders(detectedUnit) || immediateExecution)
			{
				EngagementOrder order = new EngagementOrder(detectedUnit, engagementOrderType);
				order.IsGroupAttack = isGroupAttack;
                order.WeaponClassId = weaponClassId;
				if(this.DomainType == GameConstants.DomainType.Air && 
					(detectedUnit.DomainType == GameConstants.DomainType.Surface || detectedUnit.DomainType == GameConstants.DomainType.Land))
				{
					order.EngagementStrength = GameConstants.EngagementStrength.OverkillAttack;
				}
				order.Priority = GameConstants.Priority.Elevated;
				if (immediateExecution)
				{
					ExecuteEngagementOrder(order);
				}
				else
				{
					Orders.Enqueue(order);
				}
				GameManager.Instance.Log.LogDebug(
					string.Format("BaseUnit->EngageDetectedUnit creates new order for {0}. Type {1}  Target {2}",
					ToShortString(), order.EngagementOrderType, detectedUnit.ToString()));

				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the weapon in a unit's current arsenal that can execute a special weapon. If
		/// several weapons satisfy criteria, the one with the most ammunition remaining will be returned.
		/// If similar, the first will be returned.
		/// </summary>
		/// <param name="specialOrders"></param>
		/// <returns></returns>
		public virtual BaseWeapon GetSpecialWeapon(GameConstants.SpecialOrders specialOrders)
		{
			try
			{
				var weapons = Weapons.Where(w => w.WeaponClass.IsNotWeapon && w.WeaponClass.SpecialOrders == specialOrders).ToList();
				if (weapons.Count() == 1)
				{
					return weapons.First();
				}
				else if (!weapons.Any())
				{
					return null;
				}
				else 
				{
					var maxAmmo = weapons.Max(w => w.AmmunitionRemaining);
					return weapons.First(w => w.AmmunitionRemaining == maxAmmo);
				}
			}
			catch (Exception)
			{
				return null;
				
			}
		}

		public virtual void ExecuteDropSonobuoy(BaseOrder order)
		{
			BaseWeapon weapon = GetSpecialWeapon(GameConstants.SpecialOrders.DropSonobuoy);
			if (weapon == null)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
						string.Format("Unit {0} has no sonobuoy deployment capability.", ToShortString()));
				}
				return;
			}
			if (weapon.AmmunitionRemaining < 1)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
						string.Format("Unit {0} has no more sonobuoys.", ToShortString())); 
				}
				return;
			}
			if (weapon.WeaponClass.MaxSeaState < GetEffectiveSeaState())
			{
                if (!order.IsComputerGenerated)
                {
                    //GameManager.Instance.Log.LogDebug(
                    //    string.Format("ExecuteDropSonobuoy: Unit {0} cannot deploy sonobuoy. Max sea state is {1}, current sea state is {2}",
                    //    ToShortString(), weapon.WeaponClass.MaxSeaState, GetEffectiveSeaState()));
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.DropSonobuoyFailedRoughWeather, this.Id));
                }
                
				return;
			}
            if(order.RadiusM > 0)
            {
                ExecuteDropSonobuoyAll(order);
                return;
            
            }
            if (TerrainReader.GetHeightM(this.Position.Coordinate) > 0)
            {
                GameManager.Instance.Log.LogWarning(
                    string.Format("ExecuteDropSonobouy: {0} ordered to drop sonobuoys on land. Ignored.", ToShortString()));
                return;

            }
            weapon.AmmunitionRemaining--;
			string unitClassId = weapon.WeaponClass.SpawnUnitClassId;
			Debug.Assert(!string.IsNullOrEmpty(unitClassId), "SpawnUnitClassId should not be null");
			Sonobuoy sonobuoy = (Sonobuoy) GameManager.Instance.GameData.CreateUnit(
				OwnerPlayer, null, unitClassId, string.Empty, this.Position, true, true);
			if (sonobuoy == null)
			{
				if (!order.IsComputerGenerated)
				{
					GameManager.Instance.Log.LogError(
						string.Format("ExecuteDropSonobouy: Unit {0} failed to deploy sonobuoy for unknown reasons.", ToShortString()));
				}
				return;
			}

			OwnerPlayer.Send(
                new GameStateInfo(GameConstants.GameStateInfoType.SonobuoyDropped, sonobuoy.Id, this.Id)
				{
					UnitClassId = sonobuoy.UnitClass.Id,
				});
			sonobuoy.Position.HeightOverSeaLevelM = -50;
			sonobuoy.DesiredHeightOverSeaLevelM = -50;
            sonobuoy.UserDefinedElevation = GameConstants.HeightDepthPoints.ShallowDepth;
			sonobuoy.TimeToLiveSec = sonobuoy.UnitClass.TimeToLiveSec;
			sonobuoy.ReadyInSec =  GameConstants.TIME_DEPLOY_SONOBUOY_SEC; //takes some time to deploy 
			Sonar sonar = (Sonar)sonobuoy.Sensors.First<BaseSensor>();
			if (sonar != null)
			{
				sonar.IsOperational = true;
				if (order.IsParameter)
				{
					if (sonar.SensorClass.IsPassiveActiveSensor)
					{
						sonar.IsActive = true;
					}
				}
				if (order.ValueParameter != 0 && sonar.SensorClass.IsVariableDepthSensor)
				{
					sonar.IsDeployedIntermediateDepth = true;
				}
				else
				{
					sonar.IsDeployedIntermediateDepth = false;
				}
			}
		}

        private void ExecuteDropSonobuoyAll(BaseOrder order)
        {
            //if (MovementOrder != null)
            //{
            //    MovementOrder.ClearAllWaypoints();
            //}
            var noOfBuoys = 100;
            var wpn = GetSpecialWeapon(GameConstants.SpecialOrders.DropSonobuoy);
            if (wpn != null)
            {
                noOfBuoys = wpn.AmmunitionRemaining;
            }
            var moveOrder = OwnerPlayer.AIHandler.CreateSearchOrdersAircraft(order.Position.Coordinate, order.RadiusM, noOfBuoys, true, order.IsParameter);
            Orders.Enqueue(moveOrder);
        }

		public virtual bool ExecuteEngagementOrder(EngagementOrder order)
		{
			if (this.Position == null || this.CarriedByUnit != null)
			{
				return false;
			}
            if (this.TargetDetectedUnit != null && this.TargetDetectedUnit.IsMarkedForDeletion)
            {
                this.TargetDetectedUnit = null;
            }
            if (!OwnerPlayer.IsPlayerPermittedToOpenFire)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.NotPermittedToEngageUnderOoE, this.Id));
                return true; //must report true to avoid msg being repeated
            }
            //if (order.TargetDetectedUnit != null && this.TargetDetectedUnit != null)
            //{
            //    if (this.TargetDetectedUnit.Id != order.TargetDetectedUnit.Id)
            //    {
            //        if (this.TargetDetectedUnit.IsMarkedForDeletion)
            //        {
            //            this.TargetDetectedUnit = null;
            //        }
            //        else if(order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage || 
            //            this.TargetDetectedUnit.DomainType != order.TargetDetectedUnit.DomainType )
            //        {
            //            this.Orders.Enqueue(order);
            //            return false; //not yet - wait in line
            //        }
            //    }
            //}
            if (this.DomainType != GameConstants.DomainType.Subsea && (MovementOrder == null || MovementOrder.CountWaypoints == 0)
                && UserDefinedSpeed < GameConstants.UnitSpeedType.Cruise
                && order.UnitSpeedType == GameConstants.UnitSpeedType.UnchangedDefault)
            {
                UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
            }

			if (order.TargetDetectedGroup != null)
			{
				return EngageDetectedGroup(order);
			}
			if (order.TargetDetectedUnit != null)
			{
				if (order.TargetDetectedUnit.Position == null || order.TargetDetectedUnit.RefersToUnit == null || 
					(order.TargetDetectedUnit.RefersToUnit != null &&
					order.TargetDetectedUnit.RefersToUnit.IsMarkedForDeletion))
				{
					//GameManager.Instance.Log.LogDebug(
					//    string.Format("ExecuteEngagementOrder: Unit {0} ordered to engage {1} which does not exist. Ignored.",
					//    this.ToShortString(), order.TargetDetectedUnit.ToString()));
					return true; //true since mission apparently accomplished by earlier fire
				}
                if (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage 
                    && order.TargetDetectedUnit != null && order.TargetDetectedUnit.DomainType == GameConstants.DomainType.Air 
                    && this.DomainType != GameConstants.DomainType.Air) //no non-air units are allowed to close and engage with air units
                {
                    GameManager.Instance.Log.LogDebug(
                        string.Format("ExecuteEngagementOrder: Unit {0} ordered to Close And Engage air unit {1}. Changed to EngageNotClose.",
                        this.ToShortString(), order.TargetDetectedUnit.ToString()));

                    order.EngagementOrderType = GameConstants.EngagementOrderType.EngageNotClose;
                }

                if (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage)
                {
                    //On close and engage, remove orders from currently active waypoint
                    var activeWaypoint = GetActiveWaypoint();
                    if (activeWaypoint != null && activeWaypoint.HasEngagementOrder())
                    {
                        activeWaypoint.Orders.Clear();
                    }
                }
				if (order.TargetDetectedUnit.DomainType != GameConstants.DomainType.Air 
					|| this.DomainType == GameConstants.DomainType.Air)
				{
					if (TargetDetectedUnit == null)
					{
						TargetDetectedUnit = order.TargetDetectedUnit;
					}
					else if (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage)
					{
						TargetDetectedUnit = order.TargetDetectedUnit;
					}
				}
				List<EngagementStatus> engagementStatuses = new List<EngagementStatus>();
                bool primaryWeaponOnly = false;
                if (string.IsNullOrEmpty(order.WeaponClassId))
                {
                    if (this.DomainType == GameConstants.DomainType.Air)
                    {
                        primaryWeaponOnly = (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage);
                    }
                }

				if (order.IsGroupAttack && !IsInGroupWithOthers())
				{
					order.IsGroupAttack = false;
				}
				if (order.IsGroupAttack)
				{
					engagementStatuses = Group.GetAllWeaponEngagementStatuses(
						order.WeaponClassId, order.TargetDetectedUnit,
						primaryWeaponOnly);
				}
				else
				{
					var wpnStatus = GetBestAvailableWeapon(order.WeaponClassId, order.TargetDetectedUnit, primaryWeaponOnly);
					if (wpnStatus != null)
					{
						engagementStatuses.Add(wpnStatus);
					}
				}

				var bestWeaponStatus = GetBestUnitEngagementStatusResult(engagementStatuses, primaryWeaponOnly);
				if (bestWeaponStatus == null || !bestWeaponStatus.WeaponCanBeUsedAgainstTarget)
				{
					if (!order.IsComputerGenerated)
					{
                        var gMsg = new GameStateInfo(GameConstants.GameStateInfoType.UnitHasNoWeaponToEngageTarget, this.Id, order.TargetDetectedUnit.Id);
                        OwnerPlayer.Send(gMsg);

#if LOG_DEBUG
                        string attackerDesc = ToShortString();
                        if (order.IsGroupAttack && IsInGroupWithOthers())
                        {
                            attackerDesc = Group.ToString();
                        }
                        var debugMsg = string.Format("ExecuteEngagementOrder: {0} does not have a weapon with which to engage target {1}. ",
							attackerDesc, order.TargetDetectedUnit);
						if (bestWeaponStatus != null)
						{
							debugMsg += "  " + bestWeaponStatus.ReportReason();
						}
                        GameManager.Instance.Log.LogDebug(debugMsg);
#endif
                    }
					return false;
				}
				//GameManager.Instance.Log.LogDebug(
				//    string.Format("ExecuteEngagementOrder: Unit {0} attemting to engage target {1}",
				//    this.ToShortString(), order.TargetDetectedUnit.ToString()));
				//double distanceToTargetM = MapHelper.CalculateDistance3DM(this.Position,
				//    order.TargetDetectedUnit.Position);

				int noOfRounds = 1;
				if (order.RoundCount == 0 && bestWeaponStatus.Weapon != null)
				{
					noOfRounds = GetPreferredNoOfRounds(order.TargetDetectedUnit, 
						bestWeaponStatus.Weapon, 
						order.EngagementStrength);
				}
				else
				{
					noOfRounds = order.RoundCount;
				}
				int firedRounds = 0;
				int remainingRounds = noOfRounds - firedRounds;

				if (bestWeaponStatus.EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage)
				{
					firedRounds += bestWeaponStatus.Weapon.Fire(
						order.TargetDetectedUnit, noOfRounds, bestWeaponStatus.DistanceToTargetM);
					if (firedRounds >= noOfRounds)
					{
						if (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage
							&& this.UnitClass.IsAircraft && !(MovementOrder is MovementFormationOrder) 
                            && order.TargetDetectedUnit.DomainType != GameConstants.DomainType.Subsea)
						{
							//if an aircraft set to close and engage, get away from target, and reattack in case 
							//weapon misses.
                            if (order.TargetDetectedUnit.DomainType != GameConstants.DomainType.Air 
                                || order.TargetDetectedUnit.IsKnownToSupportRole(GameConstants.Role.AttackAir))
                            {
                                EvadeIncomingThreat(order.TargetDetectedUnit, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M * 0.3, false, order);    
                            }
                            
                            //var bearingAwayFromTarget = MapHelper.CalculateBearingDegrees(
                            //    order.TargetDetectedUnit.Position.Coordinate, this.Position.Coordinate);
                            //var posAway = MapHelper.CalculateNewPosition2(this.Position.Coordinate,
                            //    bearingAwayFromTarget, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M * 0.3);
                            //var wp = new Waypoint(posAway);
                            //wp.Orders.Add(order);
                            //MovementOrder.AddWaypointToTop(wp);
						}
						return true;
					}
					else
					{
						
						order.RoundCount = noOfRounds - firedRounds; 
						if (order.RoundCount > 0)
						{
                            order.WeaponClassId = bestWeaponStatus.Weapon.WeaponClass.Id; //make sure it doesn't close to engage with other weapon later
							Orders.Enqueue(order); //Fire remaining rounds when ready
						}
						return true;
					}
				}
				else if (bestWeaponStatus.EngagementStatusResult == GameConstants.EngagementStatusResultType.OutOfSectorRange)
				{
					var bearingToTargetDeg = MapHelper.CalculateBearingDegrees(
						this.Position.Coordinate, order.TargetDetectedUnit.Position.Coordinate);

					var posCloserToTarget = MapHelper.CalculateNewPosition2(this.Position.Coordinate,
						bearingToTargetDeg, 1000);
					var wp = new Waypoint(posCloserToTarget);
                    wp.IsNotRecurring = true;
					wp.Orders.Add(order);
					if (this.MovementOrder is MovementFormationOrder)
					{
						this.MovementOrder = new MovementOrder();
					}
					this.MovementOrder.AddWaypointToTop(wp);
					GameManager.Instance.Log.LogDebug(
						string.Format("ExecuteEngagementOrder: {0} weapon out of sector range. Pointing nose...", this));
					return true;
				}
				else
				{
					if (bestWeaponStatus.EngagementStatusResult == GameConstants.EngagementStatusResultType.MustCloseToEngage)
					{
						if (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage)
						{
							if (HasAnyEngagementOrders(order.TargetDetectedUnit)) //already in place
							{
								return true;
							}
							Orders<BaseOrder> orderList = new Orders<BaseOrder>();
							orderList.Add(order);
							if (MovementOrder is MovementFormationOrder)
							{
								MovementOrder = new MovementOrder();
							}
							if (order.TargetDetectedUnit.RefersToUnit != null)
							{
								var targetUnit = order.TargetDetectedUnit.RefersToUnit;
								if (targetUnit != null)
								{
									var relativePos = MapHelper.CalculatePositionRelationship(
										this.Position, this.ActualSpeedKph, targetUnit.Position, targetUnit.ActualSpeedKph);

									if (relativePos.RelativeBearing == GameConstants.RelativeBearing.MovingAway &&
										this.ActualSpeedKph <= targetUnit.ActualSpeedKph * 1.2)
									{
										if (this.GetSpeedTypeFromKph(this.ActualSpeedKph) 
											< GameConstants.UnitSpeedType.Military &&
											HasEnoughFuelToReachTarget(targetUnit.Position.Coordinate, 
											GameConstants.UnitSpeedType.Military, true))
										{
											this.UserDefinedSpeed = GameConstants.UnitSpeedType.Military;
										}
										else if (this.UnitClass.MaxSpeedKph > this.UnitClass.MilitaryMaxSpeedKph &&
											HasEnoughFuelToReachTarget(targetUnit.Position.Coordinate, 
											GameConstants.UnitSpeedType.Afterburner, true))
										{
											this.UserDefinedSpeed = GameConstants.UnitSpeedType.Afterburner;
										}
									}
								}
							}
                            
                            var wpClose = new Waypoint(order.TargetDetectedUnit);
                            wpClose.DesiredDistanceToTargetM = CalculateDesiredWeaponDistanceToTargetM(bestWeaponStatus);;
                            wpClose.Orders = orderList;
                            wpClose.IsNotRecurring = true;
							MovementOrder.AddWaypointToTop(wpClose);
                            this.TargetDetectedUnit = order.TargetDetectedUnit;
							if (!order.IsComputerGenerated)
							{ 
								GameManager.Instance.Log.LogDebug(
									string.Format("ExecuteEngagementOrder: Unit {0} is closing {1:F}km to engage {2}.",
									ToShortString(), Math.Round(bestWeaponStatus.DistanceToCloseM / 1000.0, 2), order.TargetDetectedUnit));
							}
							if (order.TargetDetectedUnit != null)
							{
								order.TargetDetectedUnit.TargettingList.Add(new DetectedUnitTargetted(null, this));
							}
							return true;
						}
						else //not a close and engage order
						{
							if (!order.IsComputerGenerated)
							{
                                if (order.TargetDetectedUnit.FriendOrFoeClassification != GameConstants.FriendOrFoe.Foe)
                                {
                                    GameManager.Instance.Log.LogError(string.Format("ExecuteEngagementOrder: {0} is not permitted to engage {1} as it is designated {2}.",
                                        ToString(), order.TargetDetectedUnit.ToString(),
                                        order.TargetDetectedUnit.FriendOrFoeClassification));
                                    return false; 
                                }
                                var gMsg = new GameStateInfo(GameConstants.GameStateInfoType.UnitMustCloseToEngage, this.Id);
                                gMsg.RadiusM = bestWeaponStatus.DistanceToCloseM;
                                gMsg.SecondaryId = order.TargetDetectedUnit.Id;
                                OwnerPlayer.Send(gMsg);
							}
							return false;
						}
					} //has closed and engaged, or not
					foreach (var wpnStatus in engagementStatuses)
					{
						if (wpnStatus.EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage
							&& wpnStatus.Weapon.Id != bestWeaponStatus.Weapon.Id)
						{
							int rounds = wpnStatus.Weapon.Fire(
								order.TargetDetectedUnit, remainingRounds, wpnStatus.DistanceToTargetM);
							firedRounds += rounds;
							remainingRounds = noOfRounds - firedRounds;
							if (remainingRounds < 1)
							{
								return true;
							}
						}
					}
					if (remainingRounds > 0)
					{
						order.RoundCount = remainingRounds;

						Orders.Enqueue(order);
						return true;
					}
					return (firedRounds > 0);
				} 
				//return false; //no meaningful engagememt order
			}
			else if (order.Position != null) //fire on position, ie bearing only launch or ASROC
			{
				var weapons = from w in Weapons
							 where w.WeaponClass.Id == order.WeaponClassId && w.AmmunitionRemaining > 0
							 select w;
				var weapon = weapons.FirstOrDefault<BaseWeapon>();
				if (weapon == null)
				{
					GameManager.Instance.Log.LogError(
						string.Format("ExecuteEngagementOrder: {0}  Could not fire on that position. No weapon or ammunition available.", this));
					return false;
				}
				if (order.EngagementOrderType == GameConstants.EngagementOrderType.BearingOnlyAttack && !weapon.WeaponClass.CanFireBearingOnly)
				{
					GameManager.Instance.Log.LogError(string.Format("ExecuteEngagementOrder: weapon {0} can not fire bearing only.", weapon.ToString()));
					return false;
				}
				int noOfRounds = 1;
				switch (order.EngagementStrength)
				{
					case GameConstants.EngagementStrength.MinimalAttack:
						break;
					case GameConstants.EngagementStrength.DefaultAttack:
						noOfRounds = 2;
						break;
					case GameConstants.EngagementStrength.OverkillAttack:
						noOfRounds = 10; 
						break;
					default:
						break;
				}
				noOfRounds = noOfRounds.Clamp(1, weapon.AmmunitionRemaining);
				if (!weapon.WeaponClass.SpawnsUnitOnFire)
				{
					noOfRounds = noOfRounds.Clamp(1, weapon.WeaponClass.MaxSimultanousShots);
				}

                // We only check if weapon is ready, not that the position is within range, as that's handled
                // by the weapon itself when firing bearing only
                if (weapon.IsReady)
                {
                    if (weapon.IsCoordinateInSectorRange(order.Position.Coordinate))
                    {
                        return (weapon.Fire(order.Position, noOfRounds, true) > 0);
                    }
                    else //point nose
                    {
                        var bearingDeg = MapHelper.CalculateBearingDegrees(Position.Coordinate, order.Position.Coordinate);
                        var newPos = MapHelper.CalculateNewPosition2(Position.Coordinate, bearingDeg, 100);
                        if (MovementOrder == null || MovementOrder is MovementFormationOrder)
                        {
                            MovementOrder = new MovementOrder();
                        }
                        var wp = new Waypoint(newPos);
                        wp.Orders.Add(order);
                        MovementOrder.AddWaypointToTop(wp);
                        return true;
                    }
                }
                else //not read, retry
                {
                    Orders.Enqueue(order);
                    return true;
                }

			}
			return false;
		}

        /// <summary>
        /// Based on weapon and target, calculates the optimal distance from target where the unit should open fire.
        /// </summary>
        /// <param name="bestWeaponStatus"></param>
        /// <returns></returns>
        public double CalculateDesiredWeaponDistanceToTargetM(EngagementStatus bestWeaponStatus)
        {
            if (bestWeaponStatus == null || bestWeaponStatus.Weapon == null || bestWeaponStatus.TargetDetectedUnit == null)
            {
                return 0;
            }
            return bestWeaponStatus.Weapon.CalculateDesiredWeaponDistanceToTargetM(bestWeaponStatus.TargetDetectedUnit);

            //var desiredDistanceToTargetM = bestWeaponStatus.Weapon.WeaponClass.EffectiveWeaponRangeM;
            //if (bestWeaponStatus.TargetDetectedUnit != null || !bestWeaponStatus.Weapon.WeaponClass.SpawnsUnitOnFire)
            //{
            //    return desiredDistanceToTargetM;
            //}
            //var wpn = bestWeaponStatus.Weapon;
            //if (this.DomainType == GameConstants.DomainType.Air && bestWeaponStatus.TargetDetectedUnit.DomainType == GameConstants.DomainType.Subsea)
            //{
            //    //Aircraft should go as close as absolutely possibly before launching torps, to avoid subs evading.
            //    desiredDistanceToTargetM = wpn.WeaponClass.MinWeaponRangeM * 2.0;
            //    if (desiredDistanceToTargetM < 100)
            //    {
            //        desiredDistanceToTargetM = 100;
            //    }
            //}
            //if (this.DomainType == GameConstants.DomainType.Air && bestWeaponStatus.TargetDetectedUnit.DomainType ==  GameConstants.DomainType.Air)
            //{
            //    var speedTargetKph = 1000.0;
            //    if (bestWeaponStatus.TargetDetectedUnit.RefersToUnit != null)
            //    {
            //        speedTargetKph = bestWeaponStatus.TargetDetectedUnit.RefersToUnit.UnitClass.MaxSpeedKph;
            //    }
            //    var missileSpeedKph = wpn.WeaponClass.MaxSpeedKph;
            //    if (missileSpeedKph <= speedTargetKph)
            //    {
            //        desiredDistanceToTargetM *= 0.5; //if the target can outrun missile anyway, hope for the best
            //    }
            //    else
            //    {
            //        //we have some math to do
            //        var maxRangeSec = wpn.WeaponClass.MaxWeaponRangeM / missileSpeedKph.ToMperSecFromKph();
            //        var targetMaxMovementM = speedTargetKph.ToMperSecFromKph() * maxRangeSec;
            //        desiredDistanceToTargetM = wpn.WeaponClass.MaxWeaponRangeM - targetMaxMovementM;
            //    }
            //}
            //return desiredDistanceToTargetM;

        }

		/// <summary>
		/// Returns UnitEngagementStatus for this unit targetting a specific platform.
		/// </summary>
		/// <param name="weaponClassId">If not an emtoy string, only return weapons for this id</param>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public EngagementStatus GetUnitEngagementStatus(string weaponClassId, DetectedUnit detectedUnit, bool primaryWeaponsOnly)
		{
			return GetBestUnitEngagementStatusResult(GetAllWeaponEngagementStatuses(weaponClassId, detectedUnit), primaryWeaponsOnly);
		}

		/// <summary>
		/// Returns a list of EngagementStatus representing each weapon on the unit and its ability to
		/// engage the specified target.
		/// </summary>
		/// <param name="weaponClassId">If not null or empty, limit to this weapon class</param>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public List<EngagementStatus> GetAllWeaponEngagementStatuses(string weaponClassId, DetectedUnit detectedUnit)
		{
		    var targetUnit = detectedUnit.RefersToUnit;
			if (targetUnit == null || targetUnit.Position == null)
			{
				var status = new EngagementStatus(this, GameConstants.EngagementStatusResultType.Undetermined);
				return new List<EngagementStatus> { status };
			}

			var distanceToTarget = MapHelper.CalculateDistance3DM(Position, targetUnit.Position);
		    return (from wpn in Weapons where string.IsNullOrEmpty(weaponClassId) || wpn.WeaponClass.Id == weaponClassId 
                    select wpn.GetEngagementStatus(detectedUnit, distanceToTarget)).ToList();
		}

		/// <summary>
		/// Takes a list of EngagementStatus and returns the best, preferrably one that points to a weapon
		/// that can fire immediately. Otherwise reports what is the cause.
		/// </summary>
		/// <param name="primaryWeaponOnly">If true, all non-primary weapons removed from list first</param>
		/// <returns></returns>
		public EngagementStatus GetBestUnitEngagementStatusResult(List<EngagementStatus> weaponStatuses, bool primaryWeaponOnly)
		{
			//var readyWpnStatuses = weaponStatuses.FindAll(
			//    s => s.EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage);
			//if (readyWpnStatuses.Count > 0)
			//{
			//    return readyWpnStatuses[0]; //Just return the first wpn in list. prioritize here?
			//}
			if (weaponStatuses == null || weaponStatuses.Count == 0)
			{
				return null;
			}

			if (primaryWeaponOnly)
			{
				weaponStatuses.RemoveAll(w => w.Weapon != null && w.Weapon.IsPrimaryWeapon == false);
			}
			if (weaponStatuses.Count == 0)
			{
				return null;
			}
			var bestScore = weaponStatuses.Max<EngagementStatus>(s=>s.Score);
			var bestStatus = weaponStatuses.First<EngagementStatus>(s => s.Score == bestScore);
			GameManager.Instance.Log.LogDebug(
				string.Format("GetBestUnitEngagementStatusResult: {0} Best status: {1}", this, bestStatus));
			//foreach (var st in weaponStatuses)
			//{ 
			//    GameManager.Instance.Log.LogDebug("--- " + st.ToString());
			//}
			return bestStatus;

		}

		//public bool ExecuteEngagementOrderGroup(EngagementOrder order)
		//{
		//    Group group = GetGroup();
		//    if (group == null || !order.IsGroupAttack)
		//    {
		//        order.IsGroupAttack = false;
		//        Orders.Enqueue(order);
		//        return true;
		//    }
		//    bool primaryWeaponOnly = (order.EngagementOrderType == GameConstants.EngagementOrderType.CloseAndEngage);
		//    List<EngagementStatus> unitsEngagementStatuses = new List<EngagementStatus>();
		//    foreach (var unit in group.Units)
		//    {
				
		//        var status = unit.GetUnitEngagementStatus(order.WeaponClassId, order.TargetDetectedUnit, primaryWeaponOnly);
		//        if (status != null)
		//        {
		//            unitsEngagementStatuses.Add(status);
		//        }
		//    }
		//    var bestStatus = GetBestUnitEngagementStatusResult(unitsEngagementStatuses, primaryWeaponOnly);
		//    if (bestStatus == null)
		//    {
		//        return false;
		//    }
		//    if (bestStatus.EngagementStatusResult != GameConstants.EngagementStatusResultType.ReadyToEngage)
		//    {
		//        if (bestStatus.EngagementStatusResult 
		//            == GameConstants.EngagementStatusResultType.MustCloseToEngage 
		//            && order.EngagementOrderType 
		//            == GameConstants.EngagementOrderType.CloseAndEngage)
		//        {
		//            if (!this.HasAnyEngagementOrders(order.TargetDetectedUnit))
		//            {
		//                Orders<BaseOrder> orderList = new Orders<BaseOrder>();
		//                //Position pos = new Position(MapHelper.CalculatePositionInRange(
		//                //    Position.Coordinate, order.TargetDetectedUnit.Position.Coordinate, DistanceToClose));
		//                orderList.Add(order);
		//                if (MovementOrder is MovementFormationOrder)
		//                {
		//                    MovementOrder = new MovementOrder();
		//                }
		//                MovementOrder.AddWaypointToTop(order.TargetDetectedUnit,
		//                    bestStatus.DistanceToCloseM, orderList);
		//                if (order.TargetDetectedUnit != null)
		//                {
		//                    order.TargetDetectedUnit.TargettingList.Add(new DetectedUnitTargetted(null, this));
		//                }
		//                if (!order.IsComputerGenerated)
		//                {
		//                    var msg = OwnerPlayer.CreateNewMessage(
		//                        string.Format("Group {0} is closing {1:F}m to engage target {1}",
		//                        group.ToString(), bestStatus.DistanceToCloseM, order.TargetDetectedUnit.ToString()));
		//                    if (this.Position != null)
		//                    {
		//                        msg.Position = this.Position.Clone();
		//                    }
		//                }
		//            }
		//            return true;
		//        }
		//        if (!order.IsComputerGenerated)
		//        {
		//            var msg2 = OwnerPlayer.CreateNewMessage(
		//                string.Format("No units in group {0} can engage target {1}: {2}.",
		//                group.ToString(), order.TargetDetectedUnit.ToString(), bestStatus.ReportReason()));
		//            if (this.Position != null)
		//            {
		//                msg2.Position = this.Position.Clone();
		//            }
		//        }
		//        return false;
		//    }
		//    int totalRounds = 0;
		//    int roundsAllocated = order.RoundCount;
		//    foreach (var unitWpnStatus in unitsEngagementStatuses)
		//    {
		//        if (unitWpnStatus.Weapon == null 
		//            || unitWpnStatus.EngagementStatusResult 
		//               != GameConstants.EngagementStatusResultType.ReadyToEngage)
		//        {
		//            continue;
		//        }
		//        BaseWeapon weapon = unitWpnStatus.Weapon; //.Unit.GetBestAvailableWeapon(order.WeaponClassId, order.TargetDetectedUnit);
		//        if (totalRounds == 0)
		//        {
		//            totalRounds = weapon.GetRoundToFireCount(order.TargetDetectedUnit, order.EngagementStrength);
		//        }
		//        int roundsLeft = weapon.AmmunitionRemaining;
		//        int roundsToFire = totalRounds;
		//        if (roundsToFire > roundsLeft)
		//        {
		//            roundsToFire = roundsLeft;
		//        }
		//        if (roundsLeft > weapon.WeaponClass.MaxSimultanousShots)
		//        {
		//            roundsToFire = weapon.WeaponClass.MaxSimultanousShots;
		//        }
		//        if(roundsToFire > totalRounds - roundsAllocated)
		//        {
		//            roundsToFire = totalRounds - roundsAllocated;
		//        }
		//        if (roundsToFire > 0 && roundsAllocated < totalRounds)
		//        {
		//            EngagementOrder newOrder = new EngagementOrder(order.TargetDetectedUnit, 
		//                GameConstants.EngagementOrderType.EngageNotClose);
		//            newOrder.RoundCount = roundsToFire;
		//            newOrder.IsComputerGenerated = order.IsComputerGenerated;
		//            newOrder.WeaponClassId = string.Empty; //to make sure it attempts something if weapon unavailable
		//            unitWpnStatus.Unit.Orders.Enqueue(newOrder);
		//            roundsAllocated += roundsToFire;
		//            GameManager.Instance.Log.LogDebug(
		//                string.Format("ExecuteEngagementOrderGroup: Unit {0} set to engage {1} with {2} rounds.",
		//                unitWpnStatus.Unit.ToShortString(), order.TargetDetectedUnit, roundsToFire));
		//        }
		//    }
		//    if (roundsAllocated < totalRounds && totalRounds > 0)
		//    {
		//        EngagementOrder newOrder = new EngagementOrder(order.TargetDetectedUnit,
		//            GameConstants.EngagementOrderType.EngageNotClose);
		//        newOrder.IsGroupAttack = true;
		//        newOrder.RoundCount = totalRounds - roundsAllocated;
		//        this.Orders.Enqueue(newOrder);
		//    }
		//    SetMissionStatusFromOrders();
		//    CheckForOutOfAmmo();
		//    return true; 
		//}

        public bool IsUsingActiveRadar()
        {
            try
            {
                var activeRadars = from s in Sensors
                                   where s.SensorClass.SensorType == GameConstants.SensorType.Radar
                                   && s.SensorClass.IsPassiveActiveSensor && s.IsActive
                                   select s;
                return (activeRadars.Any());
            }
            catch (Exception)
            {
                //ignore
                return false;
            }
        }

        public bool IsUsingActiveSonar()
        {
            try
            {
                var activeSonars = from s in Sensors
                                   where s.SensorClass.SensorType == GameConstants.SensorType.Sonar
                                   && s.SensorClass.IsPassiveActiveSensor && s.IsActive
                                   select s;
                return (activeSonars.Any());
            }
            catch (Exception)
            {
                //ignore
                return false;
            }
        }

		public bool IsPrimaryWeaponsOutOfAmmo()
		{
            bool hasAnyPrimaryWeaponsAmmo = false;
            bool hasPrimaryWeapons = false;
			foreach (var wpn in Weapons)
			{
				if (!wpn.WeaponClass.IsNotWeapon && wpn.IsPrimaryWeapon)
				{
                    hasPrimaryWeapons = true;
                    if (wpn.AmmunitionRemaining > 0)
	                {
                        hasAnyPrimaryWeaponsAmmo = true;
	                }
				}
			}
			return hasPrimaryWeapons && !hasAnyPrimaryWeaponsAmmo;
		}

		public void IsOutOfAmmo()
		{
			if (DomainType == GameConstants.DomainType.Air)
			{
				var isOutOfAmmo = IsPrimaryWeaponsOutOfAmmo();
				if (isOutOfAmmo)
				{
					if (!IsInGroupWithOthers())
					{
						ReturnToBase();
					}
					else if (IsGroupMainUnit())
					{
						bool isAllOutOfAmmo = true;
						foreach (var unit in Group.Units)
						{
							if (!unit.IsPrimaryWeaponsOutOfAmmo())
							{
								isAllOutOfAmmo = false;
							}
						}
						if (isAllOutOfAmmo)
						{
							ReturnToBase();
						}
					}
					else
					{
						ReturnToBase();
					}
				}
			}
		}

		public void SetMissionStatusFromOrders()
		{
            var oldMissionType = _missionType;
            if (IsInGroupWithOthers() && !IsGroupMainUnit())
            {
                Group.MainUnit.SetMissionStatusFromOrders();
                MissionType = Group.MainUnit.MissionType;
            }
            else
            {
                if (HasAnyEngagementOrders())
                {
                    MissionType = GameConstants.MissionType.Attack;
                    DetectedUnit detUnit = GetTargetUnitFromEngagementOrders();
                    if (detUnit != null)
                    {
                        TargetDetectedUnit = detUnit; //also sets MissionTargetType
                    }
                }
                else
                {
                    if (IsOrderedToReturnToBase)
                    {
                        MissionType = GameConstants.MissionType.Ferry;
                    }
                    else if (MissionType != GameConstants.MissionType.Other)
                    {
                        MissionType = GameConstants.MissionType.Patrol;
                    }
                }
            }
            if (oldMissionType != _missionType)
            {
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
		}

        /// <summary>
        /// Attempts to respond to imminent threat from supplied DetectedUnit by either evasion or an aggressive response. 
        /// </summary>
        /// <param name="threateningDetectedUnit"></param>
        public void RespondToImminentThreat(DetectedUnit threateningDetectedUnit)
        {
            bool handled = false;
            if (UnitClass.IsMissileOrTorpedo)
            {
                return;
            }
            GameManager.Instance.Log.LogDebug(
                string.Format("RespondToImminentThreat: {0} requested to respond to threat {1}", this, threateningDetectedUnit));
            //Can we engage and eliminate the threat?
            var isThreatMissile = threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.Torpedo
                    || threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.Missile;
            
            var noOfTargettingMissiles = threateningDetectedUnit.CountTargettingMissiles();
            int maxTargettingMissiles = 2;
            if (OwnerPlayer.AIHandler != null)
            {
                maxTargettingMissiles = OwnerPlayer.AIHandler.GetMaxTargettingMissiles(threateningDetectedUnit);
            }    
            if (threateningDetectedUnit.CanBeTargeted && this.DomainType != GameConstants.DomainType.Air)
            {
                if (HasAnyEngagementOrders(threateningDetectedUnit))
                {
                    handled = true;  
                } 
                else if (this.WeaponOrders == GameConstants.WeaponOrders.FireOnAllClearedTargets)
                {
                    if (this.CanImmediatelyFireOnTargetType(threateningDetectedUnit) && 
                        noOfTargettingMissiles < maxTargettingMissiles && (isThreatMissile || OwnerPlayer.IsComputerPlayer))
                    {
                        EngageDetectedUnit(threateningDetectedUnit, GameConstants.EngagementOrderType.EngageNotClose, string.Empty, false, true);
                        handled = true;
                    }
                    else if (OwnerPlayer.IsComputerPlayer && MissionType == GameConstants.MissionType.Patrol 
                        && this.CanTargetDetectedUnit(threateningDetectedUnit, false))
                    {
                        var engOrderType = GameConstants.EngagementOrderType.EngageNotClose;
                        if (this.DomainType == GameConstants.DomainType.Air)
                        {
                            engOrderType = GameConstants.EngagementOrderType.CloseAndEngage;
                        }
                        EngageDetectedUnit(threateningDetectedUnit, engOrderType, string.Empty, IsInGroupWithOthers(), false);
                        handled = true;
                    }
                }
                else if (this.WeaponOrders == GameConstants.WeaponOrders.FireInSelfDefenceOnly) //only if it's missile/aircraft
                {
                    if (threateningDetectedUnit.RefersToUnit != null && isThreatMissile)
                    {
                        if (this.CanImmediatelyFireOnTargetType(threateningDetectedUnit) && noOfTargettingMissiles < maxTargettingMissiles)
                        {
                            EngageDetectedUnit(threateningDetectedUnit, GameConstants.EngagementOrderType.EngageNotClose, string.Empty, false, true);
                            handled = true;
                        }
                    }
                }
            }
            if (handled)
            {
                return; //if it is set to engage, it makes no sense to evade at the same time...
            }
            if (IsMarkedForDeletion || threateningDetectedUnit.IsMarkedForDeletion || 
                this.Position == null || threateningDetectedUnit.Position == null) //in case unit has alrerady died
            {
                return;
            }

            //Can we evade? Aircraft can run away from "all" threats. Surface ships and subs can run away from torpedoes
            var distanceM = MapHelper.CalculateDistanceRoughM(this.Position.Coordinate, threateningDetectedUnit.Position.Coordinate);
            if (OwnerPlayer.IsAutomaticallyEvadingAttacks)
            {
                switch (this.DomainType)
                {
                    case GameConstants.DomainType.Subsea:
                    case GameConstants.DomainType.Surface:
                        //evade if torpedo
                        if (threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.Torpedo)
                        {
                            EvadeIncomingThreat(threateningDetectedUnit, GameConstants.DEFAULT_TORPEDO_STRIKE_RANGE_M * 0.5, true, null);
                            handled = true;
                        }
                        break;
                    case GameConstants.DomainType.Air:
                        if (!this.IsOrderedToReturnToBase)
                        {
                            if (isThreatMissile || distanceM < GameConstants.DEFAULT_AA_ATTACK_RANGE_M * 1.5) //avoid planes being too chicken
                            {
                                EvadeIncomingThreat(threateningDetectedUnit, GameConstants.DEFAULT_AIR_AA_STRIKE_RANGE_M * 0.5, true, null);
                                handled = true;
                            }
                        }
                        break;
                }
            }
            
        }

        public bool IsKnownThreatToThisUnit(DetectedUnit detectedUnit)
        {
            if (!this.UnitClass.CanBeTargeted)
            {
                return false;
            }

            if (!detectedUnit.IsIdentified)
            {
                if (detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Missile
                    && this.DomainType != GameConstants.DomainType.Subsea)
                {
                    return true;
                }
                if (detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Torpedo &&
                    (this.DomainType == GameConstants.DomainType.Surface || this.DomainType == GameConstants.DomainType.Subsea))
                {
                    return true;
                }
            }
            else //is identified
            {
                if (detectedUnit.RefersToUnit != null && detectedUnit.RefersToUnit.UnitClass.IsMissileOrTorpedo)
                {
                    var missile = detectedUnit.RefersToUnit as MissileUnit;
                    if (missile != null)
                    {
                        var wmp = missile.LaunchWeapon;
                        if (wmp != null)
                        {
                            switch (this.DomainType)
                            {
                                case GameConstants.DomainType.Surface:
                                    return (wmp.WeaponClass.CanTargetSurface);
                                case GameConstants.DomainType.Air:
                                    return (wmp.WeaponClass.CanTargetAir);
                                case GameConstants.DomainType.Subsea:
                                    return (wmp.WeaponClass.CanTargetSubmarine);
                                case GameConstants.DomainType.Land:
                                    return (wmp.WeaponClass.CanTargetLand);
                                case GameConstants.DomainType.Unknown:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            switch (this.DomainType)
            {
                case GameConstants.DomainType.Surface:
                    return(detectedUnit.IsKnownToSupportRole(GameConstants.Role.AttackSurface));
                case GameConstants.DomainType.Air:
                    return(detectedUnit.IsKnownToSupportRole(GameConstants.Role.AttackAir));
                case GameConstants.DomainType.Subsea:
                    return(detectedUnit.IsKnownToSupportRole(GameConstants.Role.AttackSubmarine));
                case GameConstants.DomainType.Land:
                    return (detectedUnit.IsKnownToSupportRole(GameConstants.Role.AttackLand));
                case GameConstants.DomainType.Unknown:
                    break;
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// Instructs a unit to move away rapidly from an incoming threat (missile or attacking unit)
        /// </summary>
        /// <param name="threateningDetectedUnit"></param>
        /// <param name="distanceToRunM"></param>
        public void EvadeIncomingThreat(DetectedUnit threateningDetectedUnit, double distanceToRunM, bool increaseSpeed, BaseOrder reAttackOrder)
        {
            double targetSpeedKph = 0;
            if (threateningDetectedUnit.RefersToUnit != null)
            {
                targetSpeedKph = threateningDetectedUnit.RefersToUnit.ActualSpeedKph; //hmm. cheating a bit
            }
            if (!IsKnownThreatToThisUnit(threateningDetectedUnit))
            {
                return; 
            }
            if ((threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.FixedWingAircraft
                || threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.FixedWingAircraftLarge
                || threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.Helicopter) 
                && this.SupportsRole(GameConstants.Role.InterceptAircraft) 
                && this.CanTargetDetectedUnit(threateningDetectedUnit, false)) //fighters don't run away
            {
                return;
            }
            //is it already near defensive units? Then only run away from missiles and torpedoes
            if (threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.FixedWingAircraft 
                || threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.FixedWingAircraftLarge 
                || threateningDetectedUnit.DetectionClassification == GameConstants.DetectionClassification.Helicopter)
            {
                var units = from u in OwnerPlayer.Units
                            where !u.IsMarkedForDeletion
                            && u.Position != null
                            && u.GetMaxWeaponRangeM(GameConstants.DomainType.Air) >= GameConstants.DEFAULT_AIR_AA_STRIKE_RANGE_M
                            && MapHelper.CalculateDistanceApproxM(this.Position.Coordinate, u.Position.Coordinate) <= u.GetMaxWeaponRangeM(GameConstants.DomainType.Air) * 1.5
                            select u;
                if (units.Any())
                {
                    return;
                }
            }
            var rel = MapHelper.CalculatePositionRelationship(this.Position, this.ActualSpeedKph, threateningDetectedUnit.Position, targetSpeedKph);
            if (rel.RelativeBearing != GameConstants.RelativeBearing.MovingAway)
            {
                var bearingDeg = MapHelper.CalculateBearingDegrees(threateningDetectedUnit.Position.Coordinate, this.Position.Coordinate);
                var newPos = MapHelper.CalculateNewPositionApprox(this.Position.Coordinate, bearingDeg, distanceToRunM);
                var wp = new Waypoint(newPos);
                wp.IsNotRecurring = true;
                wp.IsAutomaticEvasionPoint = true;
                if (MovementOrder == null || MovementOrder is MovementFormationOrder)
                {
                    MovementOrder = new MovementOrder(wp);
                }
                else
                {
                    // Remove any previous evasion points before adding new to top
                    MovementOrder.RemoveEvasionWaypoints();
                    MovementOrder.AddWaypointToTop(wp);
                }
                var gsInfo = new GameStateInfo(GameConstants.GameStateInfoType.UnitIsEvadingEnemyFire, this.Id);
                OwnerPlayer.HasPlayerBeenFiredUpon = true; //even though it is not technically weapon fire, a hostile course and targetting radar is interpreted as firing
                gsInfo.SecondaryId = threateningDetectedUnit.Id;
                OwnerPlayer.Send(gsInfo);
                if (increaseSpeed)
                {
                    if (rel.RelativeSpeed < 0 && UserDefinedSpeed != GetSpeedTypeFromKph(this.GetMaxSpeedKph())) //if not already set to do so, move away at max speed
                    {
                        if (targetSpeedKph > this.ActualSpeedKph)
                        {
                            UserDefinedSpeed = GameConstants.UnitSpeedType.Military;
                            if (this.UnitClass.MaxSpeedKph > this.UnitClass.MilitaryMaxSpeedKph && !threateningDetectedUnit.CanBeTargeted) //only if aa missile
                            {
                                UserDefinedSpeed = GameConstants.UnitSpeedType.Afterburner;
                            }
                        }
                        var slowDownOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetSpeed);
                        slowDownOrder.UnitSpeedType = GameConstants.UnitSpeedType.Cruise;
                        wp.Orders.Add(slowDownOrder);
                    }
                }
                if (reAttackOrder != null)
                {
                    wp.Orders.Add(reAttackOrder);
                }
            }
        }

        /// <summary>
        /// If this unit (or other units in its group) is currently subject to an imminent threat, the primary threat is returned
        /// as a DetectedUnit. 
        /// </summary>
        /// <returns></returns>
        public DetectedUnit GetThreateningDetectedUnit()
        {
            if (this.Position == null)
	        {
                return null;
	        }
            if (this.UnitClass.IsMissileOrTorpedo)
            {
                return null;
            }
            var enemies = from d in OwnerPlayer.DetectedUnits  // GetEnemyUnitsInArea(this.Position.Coordinate, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
                          where !d.IsMarkedForDeletion && 
                          MapHelper.CalculateDistanceApproxM(d.Position.Coordinate, this.Position.Coordinate) < GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M
                          select d;
            var enemiesSorted = from e in enemies
                            orderby e.ValueScore descending
                            select e;
            foreach (var enemy in enemiesSorted)
            {
                if (enemy.IsKnownToTargetUnit(this, true))
                {
                    return enemy;
                }
                var distanceM = MapHelper.CalculateDistanceApproxM(this.Position.Coordinate, enemy.Position.Coordinate);
                switch (this.DomainType)
                {
                    case GameConstants.DomainType.Surface:
                        if (enemy.DomainType == GameConstants.DomainType.Air)
                        {
                            if (enemy.IsKnownToSupportRole(GameConstants.Role.AttackSurface))
                            {
                                return enemy;
                            }
                        }
                        else if (enemy.DomainType == GameConstants.DomainType.Subsea)
                        {
                            if (enemy.IsKnownToSupportRole(GameConstants.Role.AttackSurface))
                            {
                                return enemy;
                            }
                        }
                        break;
                    case GameConstants.DomainType.Air:
                        break;
                    case GameConstants.DomainType.Subsea:
                        break;
                    case GameConstants.DomainType.Land:
                        break;
                    case GameConstants.DomainType.Unknown:
                        break;
                    default:
                        break;
                }
                
            }
            return null;
        }

		public DetectedUnit GetTargetUnitFromEngagementOrders()
		{
			try
			{
				var engOrders = Orders.Where(o => o is EngagementOrder).ToList();
				if (engOrders.Any())
				{
					var order = engOrders.First() as EngagementOrder;
					if (order != null)
					{
						return order.TargetDetectedUnit;
					}
				}
				if (MovementOrder != null)
				{
				    return (from wp in MovementOrder.GetWaypoints()
				            where wp.Orders != null
				            select (from o in wp.Orders
				                    where o is EngagementOrder
				                    select o)
				            into engWpOrders where engWpOrders.Any() 
                            select engOrders.First()).OfType<EngagementOrder>().Select(order => order.TargetDetectedUnit).FirstOrDefault();
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}

		}

		/// <summary>
		/// If area is currently being jammed, returning radar degradation as a percentage. If
		/// not jammed, returns 0.
		/// </summary>
		/// <param name="sensorType"></param>
		/// <returns></returns>
		public double GetDegradationFromJammingPercent(GameConstants.SensorType sensorType)
		{
			if (this.Position == null)
			{
				return 0.0;
			}
			if (sensorType == GameConstants.SensorType.Radar)
			{
				double jammingDegradationPercent = 0.0;
				jammingDegradationPercent = GameManager.Instance.Game.GetJammingDegradationPercent(
					this.Position.Coordinate, this.OwnerPlayer);
				return jammingDegradationPercent;
			}
			else
			{
				return 0.0;
			}
		}

		/// <summary>
		/// Returns current noise level as a percentage of quiet, related to how much it 
		/// modifies detection range 
		/// </summary>
		/// <returns></returns>
		public double GetCurrentNoiseLevelPercentage()
		{
			double noisePercent = 100.0;
			var noiseLevel = GetCurrentNoiseLevel();
			switch (noiseLevel)
			{
				case GameConstants.SonarNoiseLevel.ExtremelyQuiet:
					noisePercent = 25.0;
					break;
				case GameConstants.SonarNoiseLevel.VeryQuiet:
					noisePercent = 50.0;
					break;
				case GameConstants.SonarNoiseLevel.Quiet:
					noisePercent = 100.0;
					break;
				case GameConstants.SonarNoiseLevel.Noisy:
					noisePercent = 200.0;
					break;
				case GameConstants.SonarNoiseLevel.Loud:
					noisePercent = 400.0;
					break;
				case GameConstants.SonarNoiseLevel.VeryLoud:
					noisePercent = 800.0;
					break;
				default:
					break;
			}
			noisePercent *= (ExtraordinaryNoiseModifyerPercent / 100.0);
			return noisePercent;
		}

		/// <summary>
		/// Returns current noise level as an enum. Takes into account unit speed (faster is louder)
		/// but not extraordinary noise.
		/// </summary>
		/// <returns></returns>
		public GameConstants.SonarNoiseLevel GetCurrentNoiseLevel()
		{
			GameConstants.SonarNoiseLevel noise = UnitClass.NoiseLevel;
			GameConstants.UnitSpeedType speedType = this.GetSpeedTypeFromKph(ActualSpeedKph);
			if (speedType < GameConstants.UnitSpeedType.Cruise)
			{
				noise--;
			}
			if (speedType < GameConstants.UnitSpeedType.Half)
			{
				noise--;
			}
			if (speedType > GameConstants.UnitSpeedType.Cruise)
			{
				noise++;
			}
			if (noise < GameConstants.SonarNoiseLevel.ExtremelyQuiet)
			{
				noise = GameConstants.SonarNoiseLevel.ExtremelyQuiet;
			}
			if (noise > GameConstants.SonarNoiseLevel.VeryLoud)
			{
				noise = GameConstants.SonarNoiseLevel.VeryLoud;
			}
			return noise;
		}

		/// <summary>
		/// Returns the maximum detection distance for this unit in meters, taking into account speed
		/// and ir shielding (but not weather).
		/// </summary>
		/// <returns></returns>
		public double GetMaxIrDetectionDistanceM()
		{
			double maxDetectionDistance = 0.0;
			if (this.Position == null || this.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M)
			{
				return maxDetectionDistance;
			}
			maxDetectionDistance = 15000.0;
			if (this.UnitClass.IsAircraft || this.UnitClass.IsMissileOrTorpedo)
			{
				
				if (this.ActualSpeedKph > 1000) //supersonic
				{
					maxDetectionDistance *= 1.2;
				}
			}
			else if (this.UnitClass.IsSurfaceShip 
				&& this.UnitClass.DetectionClassification == GameConstants.DetectionClassification.LargeSurface)
			{
				maxDetectionDistance *= 1.2;
			}
			if (this.UnitClass.IsIrShielded)
			{
				maxDetectionDistance *= 0.5;
			}
			if (this.FireLevel == GameConstants.FireLevel.MinorFire)
			{
				maxDetectionDistance *= 2.0;
			}
			else if (this.FireLevel > GameConstants.FireLevel.MinorFire)
			{
				maxDetectionDistance *= 3.0;
			}
			return maxDetectionDistance;
		}

		/// <summary>
		/// Returns current ESM radiation level as an enum. Takes into account active radar and unit size.
		/// </summary>
		/// <returns></returns>
		public GameConstants.EsmRadiationLevel GetCurrentEsmRadiation()
		{
			GameConstants.EsmRadiationLevel radiationLevel = GameConstants.EsmRadiationLevel.EsmNone;
			if (!UnitClass.IsEsmShielded)
			{
				radiationLevel = GameConstants.EsmRadiationLevel.EsmLow;
				if (UnitClass.DetectionClassification == GameConstants.DetectionClassification.LargeSurface)
				{
					radiationLevel = GameConstants.EsmRadiationLevel.EsmMedium;
				}
			}
			foreach (var sensor in Sensors)
			{
				if (sensor.SensorClass.SensorType == GameConstants.SensorType.Radar && sensor.SensorClass.IsPassiveActiveSensor && sensor.IsActive)
				{
                    if (sensor.SensorClass.AESAfactorPercent < 100 && radiationLevel < GameConstants.EsmRadiationLevel.EsmMedium)
                    {
                        if (sensor.SensorClass.AESAfactorPercent < 20)
                        {
                            radiationLevel = GameConstants.EsmRadiationLevel.EsmLow;
                        }
                        else
                        {
                            radiationLevel = GameConstants.EsmRadiationLevel.EsmMedium;
                        }
                    }
                    else
                    { 
					    radiationLevel = GameConstants.EsmRadiationLevel.EsmHigh;
                    }
				}
			}
			return radiationLevel;
		}

		/// <summary>
		/// Returns current sea state for unit based on weather, taking into account 
		/// stability bonus for the unitclass.
		/// </summary>
		/// <returns></returns>
		public int GetEffectiveSeaState()
		{
			WeatherSystem system = GetWeatherSystem();
			if (system == null)
			{
				GameManager.Instance.Log.LogError("GetEffectiveSeaState: WeatherSystem is null.");
				return 0;
			}
			int seaState = system.SeaState - UnitClass.StabilityBonus;
			if (UnitClass.IsLandbased)
			{
				seaState -= 2;
			}
            seaState = seaState.Clamp(0, 10);
			
			return seaState;
		}

		/// <summary>
		/// Returns current max height over sea level for unit, in meters.
		/// </summary>
		/// <returns></returns>
		public double GetMaxHeightOverSeaLevelM()
		{
            return UnitClass.HighestOperatingHeightM;
		}
		/// <summary>
		/// Returns current max depth (expressed in negative meters) for unit.
		/// </summary>
		/// <returns></returns>
		public double GetMinHeightOverSeaLevelM()
		{
			double minHeight = UnitClass.LowestOperatingHeightM;
			if (UnitClass.UnitType == GameConstants.UnitType.Submarine 
				|| minHeight < 0)
			{
				if (FireLevel == GameConstants.FireLevel.MinorFire) //in minor fires, subs must go to perscope depth
				{
					minHeight = GameConstants.DEPTH_PERISCOPE_MIN_M;
				}
				else if (FireLevel > GameConstants.FireLevel.NoFire) //more major fires, subs must surface
				{
					minHeight = GameConstants.DEPTH_SURFACE_MIN_M;
				}
				if (DamagePercent() > 50)
				{
					minHeight = GameConstants.DEPTH_SURFACE_MIN_M;
				}
			}
			return minHeight;
		}

		/// <summary>
		/// Returns current max speed in kph for this unit. Takes into account firelevel and 
		/// damage levels.
		/// </summary>
		/// <returns></returns>
		public double GetMaxSpeedKph()
		{
			//TODO: Adjust for weather
			Debug.Assert(UnitClass != null, "UnitClass should not be null.");
			if (UnitClass.UnitType == GameConstants.UnitType.LandInstallation)
			{
				return 0;
			}
			double MaxSpeed = UnitClass.MaxSpeedKph;
            int damagePercent = DamagePercent();
			if (UnitClass.UnitType == GameConstants.UnitType.Submarine 
				|| UnitClass.UnitType == GameConstants.UnitType.SurfaceShip)
			{
				switch (FireLevel)
				{
					case GameConstants.FireLevel.NoFire:
						break;
					case GameConstants.FireLevel.MinorFire:
						break;
					case GameConstants.FireLevel.MajorFire:
						MaxSpeed = MaxSpeed.Clamp(0, 50);
						break;
					case GameConstants.FireLevel.SevereFire:
						MaxSpeed = MaxSpeed.Clamp(0, 25);
						break;
					default:
						break;
				}
                if ( damagePercent > 75 )
				{
					MaxSpeed = MaxSpeed.Clamp(0, 5);
				}

                else if ( damagePercent > 50 )
				{
					MaxSpeed = MaxSpeed.Clamp(0, 25);
				}
			}
			else //aircraft, missiles or other units
			{
                if ( damagePercent > 50 || FireLevel > GameConstants.FireLevel.NoFire )
				{
					MaxSpeed = UnitClass.CruiseSpeedKph;
				}
			}
			return MaxSpeed;
		}

		/// <summary>
		/// Returns this unit's speed in kph for the provided speedtype (ie cruise, slow).
		/// </summary>
		/// <param name="speedType"></param>
		/// <returns></returns>
		public double GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType speedType)
		{
			double maxSpeed = GetMaxSpeedKph();
			double speed = 0;
			switch (speedType)
			{
				case GameConstants.UnitSpeedType.Afterburner:
					speed = maxSpeed;
					break;
				case GameConstants.UnitSpeedType.Military:
					speed = UnitClass.MilitaryMaxSpeedKph;
					break;
				case GameConstants.UnitSpeedType.Cruise:
					speed = UnitClass.CruiseSpeedKph;
					break;
				case GameConstants.UnitSpeedType.Half:
					speed = UnitClass.CruiseSpeedKph / 2.0;
					break;
				case GameConstants.UnitSpeedType.Slow:
					speed = GameConstants.DEFAULT_SLOW_SPEED;
					break;
				case GameConstants.UnitSpeedType.Crawl:
					speed = GameConstants.DEFAULT_CRAWL_SPEED;
					break;
				case GameConstants.UnitSpeedType.Stop:
					speed = 0; 
					break;
				default:
					speed = UnitClass.CruiseSpeedKph;
					break;
			}
			speed = speed.Clamp(UnitClass.MinSpeedKph, maxSpeed);
			return speed;
		}

		/// <summary>
		/// Returns mission target type (enum) from a DetectedUnit
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public GameConstants.MissionTargetType GetMissionTargetTypeFromDetectedUnit(DetectedUnit detectedUnit)
		{
			if (detectedUnit == null)
			{
				return GameConstants.MissionTargetType.Undefined;
			}
			switch (detectedUnit.DomainType)
			{
				case GameConstants.DomainType.Surface:
					return GameConstants.MissionTargetType.Surface;
				case GameConstants.DomainType.Air:
					return GameConstants.MissionTargetType.Air;
				case GameConstants.DomainType.Subsea:
					return GameConstants.MissionTargetType.Sub;
				case GameConstants.DomainType.Land:
					return GameConstants.MissionTargetType.Land;
				case GameConstants.DomainType.Unknown:
					return GameConstants.MissionTargetType.Undefined;
				default:
					return GameConstants.MissionTargetType.Undefined;
			}
		}

		/// <summary>
		/// Returns the speed type for this unit given an actual speed in kph.
		/// </summary>
		/// <param name="speedKph"></param>
		/// <returns></returns>
		public GameConstants.UnitSpeedType GetSpeedTypeFromKph(double speedKph)
		{
            return this.UnitClass.GetSpeedTypeFromKph(speedKph);
		}

		/// <summary>
		/// Returns the preferred number of round to fire and a specific unit.
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <param name="weapon"></param>
		/// <param name="engagementStrength"></param>
		/// <returns></returns>
		public int GetPreferredNoOfRounds(DetectedUnit detectedUnit, BaseWeapon weapon, 
			GameConstants.EngagementStrength engagementStrength)
		{
            if (weapon != null && detectedUnit != null)
            {
                return weapon.GetRoundToFireCount(detectedUnit, engagementStrength);    
            }
            return 0;
		}

		/// <summary>
		/// Reads the next order from the unit's Orders queue and executes it if appropriate.
		/// </summary>
		public void ExecuteOrders()
		{
            if (GameManager.Instance.Game.RealTimeCompressionFactor == 0.0)
            {
                return;
            }
			if (Orders.Count > 0) //change: one order per tick only
			{
				if (MissionType == GameConstants.MissionType.Attack 
					&& this.TargetDetectedUnit != null 
					&& !this.TargetDetectedUnit.IsMarkedForDeletion)
				{
                    var order = Orders.Peek();
					if (order.OrderType == GameConstants.OrderType.MovementOrder //ignore movementorders when attacking
						|| order.OrderType == GameConstants.OrderType.MovementFormationOrder)
					{
						return;
					}
				}
				ExecuteOrders(Orders.Dequeue());
			}
		}

		/// <summary>
		/// Executes a specific order on this unit
		/// </summary>
		/// <param name="order"></param>
		public void ExecuteOrders(BaseOrder order)
		{
            if (order.IsMarkedForDeletion)
            {
                return; 
            }
			
            GameManager.Instance.Log.LogDebug(
				string.Format("ExecuteOrders: Unit {0} order: {1}", ToString(), order.OrderType));
            
            if (order.RemoveAllExistingWaypoints)
            {
                ClearAllWaypoints();

                // Remove orders as well for computer generated orders
                if (order.IsComputerGenerated)
                {
                    Orders.Clear();
                }
            }
			
            if (order is ScheduledOrder)
			{
                var scheduledOrder = order as ScheduledOrder;
                if (scheduledOrder.TriggerNextTime < GameManager.Instance.Game.GameCurrentTime)
                {
                    foreach (var suborder in scheduledOrder.Orders)
                    {
                        Orders.AddFirst(suborder);
                    }
                    scheduledOrder.RecurringCount--;
                    if (scheduledOrder.RecurringCount > 0)
                    {
                        scheduledOrder.TriggerNextTime = GameManager.Instance.Game.GameCurrentTime.AddSeconds(
                            scheduledOrder.TriggerInSec);
                        Orders.Enqueue(scheduledOrder);
                    }
                }
                else
                {
                    Orders.AddFirst(scheduledOrder);
                }
			}
			else if (order is EngagementOrder)
			{
				ExecuteEngagementOrder((EngagementOrder)order);

			}
			else if (order is MovementFormationOrder)
			{
				MovementOrder = order as MovementFormationOrder;
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
			}
			else if (order is MovementOrder)
			{
				var moveOrder = order as MovementOrder;

                // Get valid waypoints
                var wps = moveOrder.GetWaypoints();
			    var waypoints = wps.Where(wp => IsValidWaypointForUnit(wp.Position)).ToList();
                if (waypoints.Any())
                {
                    if (MovementOrder is MovementFormationOrder)
                    {
                        MovementOrder = new MovementOrder();
                    }

                    foreach (var wp in waypoints)
                    {
                        MovementOrder.AddWaypoint(wp);
                    }

                    if (order.UnitSpeedType != GameConstants.UnitSpeedType.UnchangedDefault)
                    {
                        this.UserDefinedSpeed = order.UnitSpeedType;
                    }
                    else if (ActualSpeedKph < 1)
                    {
                        ActualSpeedKph = UnitClass.CruiseSpeedKph;
                    }

                    MovementOrder.IsRecurring = moveOrder.IsRecurring;

                    ReCalculateEta();
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }
			}
			else if (order is LaunchAircraftOrder)
			{
                var launchOrder = order as LaunchAircraftOrder;
                if (AircraftHangar != null && AircraftHangar.IsReady)
                {
                    LaunchAircraft(launchOrder.UnitList, order.GroupId, string.Empty, order.Orders, order.Tag);
                }
                else
                {
                    Orders.Enqueue(order);
                }
			}
			else
			{
				switch (order.OrderType)
				{
					case GameConstants.OrderType.UnknownOrder:
						break;
					case GameConstants.OrderType.MovementOrder:
						break;
					case GameConstants.OrderType.MovementFormationOrder:
						break;
					case GameConstants.OrderType.EngagementOrder:
						break;
					case GameConstants.OrderType.ReturnToBase:
						GameManager.Instance.Log.LogDebug(
							string.Format("ExecuteOrders: Unit {0} Return to Base or has assigned new carrier.", ToString()));
						if (this is AircraftUnit)
						{
                            if (!string.IsNullOrEmpty(order.SecondId))
                            {
                                SetHomeToNewCarrier(order.SecondId);
                            }
                            else
                            {
                                this.IsOrderedToReturnToBase = false; //overrides flag
                                this.ReturnToBase();
                            }
						}
						break;
					case GameConstants.OrderType.SensorActivationOrder:
						{
							if (string.IsNullOrEmpty(order.SecondId))
							{
								SetSensorsActivePassive(order.SensorType, order.IsParameter);
							}
							else
							{
								BaseSensor sensor = GetComponent(order.SecondId) as BaseSensor;
								if (sensor != null)
								{
									if (sensor.SensorClass.IsPassiveActiveSensor)
									{
										sensor.IsActive = order.IsParameter;
									}
									SetDirty(GameConstants.DirtyStatus.UnitChanged);
								}
							}
						}
						break;
					case GameConstants.OrderType.SensorDeploymentOrder:
						{
							BaseSensor sensor = GetComponent(order.SecondId) as BaseSensor;
							if (sensor != null && sensor.SensorClass.IsDeployableSensor)
							{
								sensor.IsOperational = order.IsParameter;
								if (sensor.IsOperational)
								{
									if (sensor.SensorClass.TimeToDeploySec > 0)
									{
										sensor.ReadyInSec = sensor.SensorClass.TimeToDeploySec;
									}
									if (sensor.SensorClass.MaxSpeedDeployedKph > 0
										&& ActualSpeedKph > sensor.SensorClass.MaxSpeedDeployedKph)
									{
										ActualSpeedKph = sensor.SensorClass.MaxSpeedDeployedKph;
										UserDefinedSpeed = GetSpeedTypeFromKph(sensor.SensorClass.MaxSpeedDeployedKph);
									}
									if (sensor.SensorClass.MaxHeightDeployedM > 0 && this.Position != null
										&& this.Position.HasHeightOverSeaLevel
										&& this.Position.HeightOverSeaLevelM > sensor.SensorClass.MaxHeightDeployedM)
									{
										DesiredHeightOverSeaLevelM = sensor.SensorClass.MaxHeightDeployedM;
									}
									if (sensor is Sonar && sensor.SensorClass.IsVariableDepthSensor)
									{
										if (order.ValueParameter > 0)
										{
											Sonar sonar = sensor as Sonar;
											if (sonar != null)
											{
												sonar.IsDeployedIntermediateDepth = true;
											}
										}
									}
								}
								else
								{
									sensor.ReadyInSec = 0;
								}
								SetDirty(GameConstants.DirtyStatus.UnitChanged);
							}
							else //sensor not specified, use SensorType and apply to all relevant
							{
                                try
                                {
                                    foreach (var sen in Sensors)
                                    {
                                        if (sensor.SensorClass.IsDeployableSensor
                                            && sen.SensorClass.SensorType == order.SensorType)
                                        {
                                            BaseOrder newOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorDeploymentOrder);
                                            newOrder.SecondId = sensor.Id;
                                            newOrder.IsParameter = order.IsParameter;
                                            newOrder.ValueParameter = order.ValueParameter;
                                            Orders.Enqueue(newOrder);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    //ignore. NullReferenceException may happen if triggered when unit is destroyed
                                }
							}
						}
						break;
					case GameConstants.OrderType.SetSpeed:
                        SetDirty(GameConstants.DirtyStatus.UnitChanged);
                        if (order.IsParameter && IsInGroupWithOthers()) //set for entire group
                        {
                            foreach (var u in Group.Units)
                            {
                                if (u.Id != this.Id)
                                {
                                    BaseOrder newOrder = order.Clone();
                                    newOrder.IsParameter = false;
                                    u.Orders.Enqueue(newOrder);
                                }
                            }
                        }
						if (order.ValueParameter > 0)
						{
							ActualSpeedKph = order.ValueParameter;
							UserDefinedSpeed = GetSpeedTypeFromKph(order.ValueParameter);
						}
						else
						{
							ActualSpeedKph = GetSpeedInKphFromSpeedType(order.UnitSpeedType);
							UserDefinedSpeed = order.UnitSpeedType;
						}
						break;
					case GameConstants.OrderType.SetElevation:
						if (order.ValueParameter != 0)
						{
							ActualHeightOverSeaLevelM = order.ValueParameter;
                            DesiredHeightOverSeaLevelM = order.ValueParameter;
                            UserDefinedElevation = ActualHeightOverSeaLevelM.Value.ToHeightDepthMark();
						}
						else
						{
							double desiredHeightM =
								GetElevationMFromHeightDepthMark(order.HeightDepthPoints);
							//ActualHeightOverSeaLevelM = desiredHeightM;
                            if (DesiredHeightOverSeaLevelM != desiredHeightM)
                            {
                                SetDirty(GameConstants.DirtyStatus.UnitChanged);
                            }
                            UserDefinedElevation = order.HeightDepthPoints;
							DesiredHeightOverSeaLevelM = desiredHeightM;
						}
						break;
					case GameConstants.OrderType.LaunchOrder:
						break;
					case GameConstants.OrderType.ChangeAircraftLoadout:
						BaseUnit plane = OwnerPlayer.GetUnitById(order.SecondId);
						if (plane != null)
						{
                            if (string.IsNullOrEmpty(order.StringParameter))
                            {
                                plane.SetWeaponLoad(order.WeaponLoadType, order.WeaponLoadModifier);
                            }
                            else
                            { 
							    plane.SetWeaponLoad(order.StringParameter);
                            }
						}
						break;
					case GameConstants.OrderType.SetNewGroupFormation:
						SetNewGroupFormation(order.Formation);
						break;
					case GameConstants.OrderType.SetUnitFormationOrder:
						FormationPositionId = order.SecondId;
						SetUnitFormationOrder();
						break;
					case GameConstants.OrderType.SplitGroup:
                        if (Group != null)
						{
							Group.SplitGroup(order.ParameterList);
						}
						else
						{
							GameManager.Instance.Log.LogError(
								"Split group order sent to unit " + ToShortString()
								+ " which is not in a group.");
						}
						break;
					case GameConstants.OrderType.JoinGroups:
						GameManager.Instance.Log.LogDebug(
							string.Format("ExecuteOrders: JoinGroups. Unit {0} to join group {1}.",
							ToShortString(), order.SecondId));
						JoinNewGroup(order);
						break;
					case GameConstants.OrderType.MissileSearchForTarget:
						GameManager.Instance.Log.LogDebug(
							string.Format("ExecuteOrders: MissileSearchForTarget. Unit {0} to search for target {1}.",
							ToShortString(), order.SecondId));
						MissileUnit missile = this as MissileUnit;
						if (missile != null)
						{
							missile.SearchForTarget(order);
						}
						else
						{
							GameManager.Instance.Log.LogError(
								string.Format("ExecuteOrders: MissileSearchForTarget. Unit {0} is not a missile!",
								ToShortString()));
						}
						break;
					case GameConstants.OrderType.SpecialOrders:
						GameManager.Instance.Log.LogDebug(
							string.Format("ExecuteOrders: SpecialOrders. Unit {0} executes {1}.",
							ToShortString(), order.SpecialOrders));
						ExecuteSpecialOrder(order);
						break;
                    case GameConstants.OrderType.RefuelInAir:
				        var airUnit = this as AircraftUnit;
                        if ( airUnit != null )
                        {
                            var refuelAircraft = OwnerPlayer.GetUnitById(order.SecondId) as AircraftUnit;
                            if (refuelAircraft != null)
                            {
                                // Inform AIHandler if refueling fails for a computer generated order
                                if (!airUnit.RefuelInAir(refuelAircraft))
                                {
                                    if (order.IsComputerGenerated)
                                    {
                                        Debug.Assert(OwnerPlayer != null && OwnerPlayer.AIHandler != null, "Neither OwnerPlayer or AIHandler should be null!");
                                        OwnerPlayer.AIHandler.RefuelInAirFailed(airUnit, refuelAircraft);
                                    }
                                    
                                    // Message player of failing to refuel
                                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftRefuelInAirFailed, this.Id, refuelAircraft.Id));
                                }
                            }
                        }
				        break;
					default:
						break;
				}
			}
		}

        public void RemoveExistingEngagementOrders()
        {
            this.MissionTargetType = GameConstants.MissionTargetType.Undefined;
            this.MissionType = GameConstants.MissionType.Patrol;
            TargetDetectedUnit = null;
            foreach (var order in this.Orders)
            {
                if (order is EngagementOrder)
                {
                    order.IsMarkedForDeletion = true;
                }
            }
        }

		/// <summary>
		/// Force all units in unit's current group to join a new group, discarding
		/// the old group.
		/// </summary>
		/// <param name="newGroupId"></param>
		public void JoinNewGroup(BaseOrder order)
		{
			string newGroupId = order.SecondId;
			
			Group newGroup = OwnerPlayer.GetGroupById(newGroupId);
			if (newGroup == null || newGroup.MainUnit == null)
			{
				newGroup = new Group();
				//GameManager.Instance.Log.LogError("JoinNewGroupAllUnits: No or empty group " + newGroupId);
				//return;
			}
			if(this.Position == null || this.Position.Coordinate == null)
			{
				return;
			}
			BaseUnit unitToOrder = this;
			if (Group != null)
			{
				GameManager.Instance.Log.LogDebug(
					string.Format("JoinNewGroup join unit {0} current group {1} to group {2}",
					ToShortString(), Group.ToString(), newGroup.ToString()));
                //if (currentGroup.Id == newGroup.Id) //funny
                //{
                //    return;
                //}
				unitToOrder = Group.MainUnit;
				if (newGroup.MainUnit != null && newGroup.MainUnit.DomainType == GameConstants.DomainType.Air
					&& unitToOrder.DomainType != GameConstants.DomainType.Air)
				{
                    var msgErr = new GameStateInfo(GameConstants.GameStateInfoType.UnitJoinGroupInvalid, this.Id, newGroup.Id);
                    OwnerPlayer.Send(msgErr);
                    //var msg = OwnerPlayer.CreateNewMessage(
                    //    string.Format("{0} cannot conceivably be joined with air group {1}.",
                    //    unitToOrder.ToShortString(), newGroup.ToString()));
                    //msg.Position = this.Position.Clone();
					return;
				}
			}
            if (newGroup.MainUnit != null)
            {
                double distanceM = MapHelper.CalculateDistance3DM(this.Position, newGroup.MainUnit.Position);
                if (distanceM > GameConstants.MAX_DISTANCE_JOIN_M)
                {
                    Waypoint wp = new Waypoint(newGroup.MainUnit);
                    wp.DesiredDistanceToTargetM = GameConstants.MAX_DISTANCE_JOIN_M;
                    wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.JoinGroups)
                    {
                        SecondId = newGroupId,
                    });
                    unitToOrder.MovementOrder.AddWaypointToTop(wp);
                    OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitMovingToJoinGroup, this.Id, newGroup.Id));
                    GameManager.Instance.Log.LogDebug(
                        string.Format("JoinNewGroup: Unit {0} is moving closer to group {1} to attempt to join it. Distance: {2:F}m.",
                        ToShortString(), newGroup.ToString(), distanceM));
                    
                    return;
                }
            }
			newGroup.AddUnit(this);
            //var msg2 = OwnerPlayer.CreateNewMessage(
            //    string.Format("{0} has now joined group {1}.",
            //    ToShortString(), newGroup.ToString()));
            //msg2.Position = this.Position.Clone();

			if (order.ParameterList != null && order.ParameterList.Count > 0)
			{
				foreach (var uId in order.ParameterList)
				{
					if (uId != this.Id)
					{
						var unit = GetUnitById(OwnerPlayer, uId);
						if(unit != null)
						{
							newGroup.AddUnit(unit);
						}
					}
				}
				//msg2.MessageBody = string.Format("New group {0} formed.", newGroup.ToString());
			}
			//else //whole group joins new group
			//{
			//    int count = currentGroup.Units.Count;
			//    foreach (var u in currentGroup.Units)
			//    {
			//        u.FormationPositionId = string.Empty;
			//        newGroup.AddUnit(u);
			//    }
			//    currentGroup.Units.Clear();
			//    currentGroup.MainUnit = null;
			//    var msg = OwnerPlayer.CreateNewMessage(
			//        string.Format("All {0} unit(s) from group {1} has now joined group {2}.",
			//        count, currentGroup.ToString(), newGroup.ToString()));
			//    msg.Position = this.Position.Clone();
			//    currentGroup.IsMarkedForDeletion = true;
			//}
			newGroup.AutoAssignUnitsToFormation();
		}

		/// <summary>
		/// Returns true if unit is member of a group that has more than itself as a member, 
		/// otherwise false.
		/// </summary>
		/// <returns></returns>
		public bool IsInGroupWithOthers()
		{
		    return (Group != null && Group.Units.Count > 1);
		}

		/// <summary>
		/// Returns true if unit is in a group and it is the group MainUnit
		/// </summary>
		/// <returns></returns>
		public bool IsGroupMainUnit()
		{
		    return (Group != null && Group.MainUnit != null && Group.MainUnit.Id == Id);
		}

		/// <summary>
		/// This is the method called by external weapons and missiles to inflict damage on this unit.
		/// This method determines if the weapon hit, how much damage it did, and creates a
		/// BattleDamageReport for both users.
		/// </summary>
		/// <param name="weapon"></param>
		public void InflictDamageFromProjectileHit(BaseWeapon weapon, DetectedUnit detectedUnit)
		{
            OwnerPlayer.HasPlayerBeenFiredUpon = true;
			
			double distanceToTargetM = MapHelper.CalculateDistance3DM(
				weapon.OwnerUnit.Position, Position);
            int probHit = (int)weapon.GetHitPercent(detectedUnit, distanceToTargetM);  //.WeaponClass.HitPercent;

			if (distanceToTargetM > weapon.WeaponClass.MaxWeaponRangeM 
				|| distanceToTargetM < weapon.WeaponClass.MinWeaponRangeM)
			{
				probHit = 0;
			}
			else if (distanceToTargetM > weapon.WeaponClass.EffectiveWeaponRangeM)
			{
				probHit /= 2;
			}
			bool IsMissing = !GameManager.Instance.ThrowDice(probHit);

			int HitPointsDamage = 0;
			if (!IsMissing)
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				HitPointsDamage = GameManager.Instance.GetDamageHitpoints(weapon.WeaponClass.DamageHitPoints);
			}
			HitPoints -= HitPointsDamage;

			if (HitPoints < 0)
			{
				HitPoints = 0;
			}
			List<CriticalDamage> criticalDamageList = new List<CriticalDamage>();
			if (HitPointsDamage > 0 && HitPoints > 0) //no need to inflict crit damage on destroyed unit
			{
				criticalDamageList = InflictCriticalDamage(HitPointsDamage, weapon.WeaponClass.IsAntiRadiationWeapon);
			}
			BattleDamageReport report = new BattleDamageReport();
			report.CriticalDamageList = criticalDamageList;
            report.IsTargetHit = !IsMissing;
            //report.DamageHitpoints = HitPointsDamage;
			report.DamagePercent = (int)(((double)HitPointsDamage / (double)UnitClass.MaxHitpoints) * 100.0);
			if (report.DamagePercent > 100)
			{
				report.DamagePercent = 100;
			}
			report.GameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
			report.IsTargetPlatformDestroyed = (HitPoints == 0);
			report.Position = this.GetPositionInfo();
            //if (weapon.OwnerUnit != null && weapon.OwnerUnit.Position != null)
            //{
            //    report.AbsoluteBearingAttackFromDeg = 
            //        MapHelper.CalculateBearingDegrees(Position.Coordinate, weapon.OwnerUnit.Position.Coordinate);
            //}
			if (weapon.OwnerUnit != null && weapon.OwnerUnit.UnitClass != null)
			{
				report.PlatformInflictingDamageClassId = weapon.OwnerUnit.UnitClass.Id;
                //report.PlatformInflictingDamageClassName = weapon.OwnerUnit.UnitClass.UnitClassShortName;
				report.PlatformInflictingDamageId = weapon.OwnerUnit.Id;
				report.PlatformInflictingDamageUnitName = weapon.OwnerUnit.Name;
                //report.PlayerInflictingDamageName = weapon.OwnerPlayer.Name;
			}
			report.WeaponClassId = weapon.WeaponClass.Id;
            //WeaponClass weaponClass = GameManager.Instance.GetWeaponClassById(report.WeaponClassId);
            //if (weaponClass != null)
            //{
            //    report.WeaponClassName = weaponClass.WeaponClassName;
            //}
			report.PlayerInflictingDamageId = weapon.OwnerPlayer.Id;
            //report.PlayerInflictingDamageName = weapon.OwnerPlayer.Name;
			report.PlayerSustainingDamageId = OwnerPlayer.Id;
            //report.PlayerSustainingDamageName = OwnerPlayer.Name;
			report.TargetPlatformClassId = UnitClass.Id;
            //report.TargetPlatformClassName = UnitClass.UnitClassShortName;
			report.TargetPlatformUnitName = Name;
			report.TargetPlatformId = Id;
			report.TargetPlatformRoles = new List<GameConstants.Role>();
			foreach (var role in RoleList)
			{
				report.TargetPlatformRoles.Add(role);
			}
            if (this.IsCivilianUnit)
            {
                report.IsTargetCivilianUnit = true;
            }
            if (!weapon.OwnerPlayer.IsEnemy(OwnerPlayer))
            {
                report.IsTargetNeutralUnit = true;
            }
			//report.TimeStamp = DateTime.Now;
			UpdateDamageReportPriorityAndMessage(report);

            // Clone the report and remove attacking platform data if target can't see it
            BattleDamageReport reportToTarget = report.Clone();
            if (!OwnerPlayer.CanCurrentlySeeEnemyUnit(weapon.OwnerUnit))
            {
                reportToTarget.RemoveAttackingPlatformData();
            }

            OwnerPlayer.BattleDamageReports.Add(reportToTarget);
            weapon.OwnerPlayer.BattleDamageReports.Add(report);

            // Change to detected unit id's after we've cloned the report, but before sending it to players
            //var detectedUnit = OwnerPlayer.GetDetectedUnitByUnitId(reportToTarget.PlatformInflictingDamageId);
            if (detectedUnit != null)
            {
                //report.PlatformInflictingDamageDetectedId = detectedUnit.Id;
                reportToTarget.PlatformInflictingDamageId = detectedUnit.Id;
            }
            var detectUnit = weapon.OwnerPlayer.GetDetectedUnitByUnitId(Id);
            if (detectUnit != null)
            {
                //report.TargetPlatformDetectedId = detectUnit.Id;
                report.TargetPlatformId = detectUnit.Id;
            }

            OwnerPlayer.SendNewBattleDamageReport(reportToTarget);
			weapon.OwnerPlayer.SendNewBattleDamageReport(report);

			if (HitPointsDamage > 0)
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				SendDetectedUnitInfoForThisUnit();
			}
			if (report.IsTargetPlatformDestroyed)
			{
				bool defeat = OwnerPlayer.HasBeenDefeated;
				IsMarkedForDeletion = true;
				UnitToBeDestroyed();
			}
		}
		/// This is the method called by external weapons and missiles to inflict damage on this unit.
		/// This method determines if the weapon hit, how much damage it did, and creates a
		/// BattleDamageReport for both users.
		/// <param name="missileUnit"></param>
		public void InflictDamageFromProjectileHit(MissileUnit missileUnit)
		{
			if (missileUnit == null 
				|| missileUnit.Position == null 
				|| this.Position == null 
				|| missileUnit.IsMarkedForDeletion 
				|| this.IsMarkedForDeletion)
			{
				return;
			}
            OwnerPlayer.HasPlayerBeenFiredUpon = true;
			GameManager.Instance.Log.LogDebug(
				string.Format("BaseUnit->InflictDamageFromProjectileHit. Missile {0} hits unit {1}.",
				missileUnit.ToString(), ToString()));
            double distanceMissileTargetM = MapHelper.CalculateDistance3DM(Position, missileUnit.Position);
			bool isMissing = (distanceMissileTargetM > GameConstants.DISTANCE_TO_TARGET_IS_HIT_M * 2.0);
			if (missileUnit.UnitClass.UnitType == GameConstants.UnitType.Mine) //always close enough if this is called
			{
				isMissing = false;
			}
            var isAntiRadiation = false;
            if (missileUnit.LaunchWeapon != null && missileUnit.LaunchWeapon.WeaponClass.IsAntiRadiationWeapon)
            {
                if (this.IsUsingActiveRadar())
                {
                    isAntiRadiation = true;
                    missileUnit.HitPercent = 100;
                }
                else
                {
                    missileUnit.HitPercent =(int)(missileUnit.HitPercent * 0.5);
                }
            }
			if (!isMissing && !GameManager.Instance.ThrowDice(missileUnit.HitPercent))
			{
				isMissing = true;
			}
			int HitPointsDamage = 0;
            var isSoftKilled = InflictMissileSoftkill(missileUnit);
            if (isSoftKilled)
            {
                isMissing = true;
            }
			if (!isMissing)
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				HitPointsDamage = GameManager.Instance.GetDamageHitpoints(missileUnit.DamageHitpoints);
			}
			HitPoints -= HitPointsDamage;
			if(HitPoints < 0)
			{
				HitPoints = 0;
			}
			var criticalDamageList = new List<CriticalDamage>();
			if (HitPointsDamage > 0 && HitPoints > 0) //no need to inflict crit damage on destroyed unit
			{
				 criticalDamageList = InflictCriticalDamage(HitPointsDamage, isAntiRadiation);
			}
			BattleDamageReport report = new BattleDamageReport();
            report.IsProjectileSoftKilled = isSoftKilled;
			report.CriticalDamageList = criticalDamageList;
            report.IsTargetHit = !isMissing;
            //report.DamageHitpoints = HitPointsDamage;
			report.DamagePercent = (int)(((double)HitPointsDamage / (double)UnitClass.MaxHitpoints) * 100.0);
			if (report.DamagePercent > 100)
			{
				report.DamagePercent = 100;
			}
			report.GameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
			report.IsTargetPlatformDestroyed = (HitPoints == 0);
			report.Position = this.GetPositionInfo();
            //if (missileUnit.Position.BearingDeg != null)
            //{
            //    report.AbsoluteBearingAttackFromDeg = missileUnit.Position.BearingDeg.Value;
            //}
			if (missileUnit.LaunchPlatform != null && missileUnit.LaunchPlatform.UnitClass != null)
			{
				report.PlatformInflictingDamageClassId = missileUnit.LaunchPlatform.UnitClass.Id;
                //report.PlatformInflictingDamageClassName = missileUnit.LaunchPlatform.UnitClass.UnitClassShortName;
				report.PlatformInflictingDamageId = missileUnit.LaunchPlatform.Id;
				report.PlatformInflictingDamageUnitName = missileUnit.LaunchPlatform.Name;
			}
			report.MissileId = missileUnit.Id;
			report.WeaponClassId = missileUnit.WeaponClassId;
            //WeaponClass weaponClass = GameManager.Instance.GetWeaponClassById(report.WeaponClassId);
            //if (weaponClass != null)
            //{
            //    report.WeaponClassName = weaponClass.WeaponClassName;
            //}
			
			report.PlayerInflictingDamageId = missileUnit.OwnerPlayer.Id;
            //report.PlayerInflictingDamageName = missileUnit.OwnerPlayer.Name;
			report.PlayerSustainingDamageId = OwnerPlayer.Id;
            //report.PlayerSustainingDamageName = OwnerPlayer.Name;
			report.TargetPlatformClassId = UnitClass.Id;
            //report.TargetPlatformClassName = UnitClass.UnitClassShortName;
			report.TargetPlatformUnitName = Name;
			report.TargetPlatformId = Id;

            if (this.IsCivilianUnit)
            {
                report.IsTargetCivilianUnit = true;
            }
            if (!missileUnit.OwnerPlayer.IsEnemy(OwnerPlayer))
            {
                report.IsTargetNeutralUnit = true;
            }

			report.TargetPlatformRoles = new List<GameConstants.Role>();
			foreach (var role in RoleList)
			{
				report.TargetPlatformRoles.Add(role);
			}
			//report.TimeStamp = DateTime.Now;
			UpdateDamageReportPriorityAndMessage(report); 

            // Clone the report and remove attacking platform data if target can't see it
            BattleDamageReport reportToTarget = report.Clone();
            if (!OwnerPlayer.CanCurrentlySeeEnemyUnit(missileUnit))
            {
                reportToTarget.RemoveAttackingPlatformData();
            }

            OwnerPlayer.BattleDamageReports.Add(reportToTarget);
            missileUnit.OwnerPlayer.BattleDamageReports.Add(report);

            // Change to detected unit id's after we've cloned the report, but before sending it to players
            DetectedUnit detectedUnit = OwnerPlayer.GetDetectedUnitByUnitId(reportToTarget.PlatformInflictingDamageId);
            if (detectedUnit != null)
            {
                //report.PlatformInflictingDamageDetectedId = detectedUnit.Id;
                reportToTarget.PlatformInflictingDamageId = detectedUnit.Id;
            }
            DetectedUnit detMissile = OwnerPlayer.GetDetectedUnitByUnitId(missileUnit.Id);
            if (detMissile != null)
            {
                //report.MissileDetectedUnitId = detMissile.Id;
                reportToTarget.MissileId = detMissile.Id;
            }
            detectedUnit = missileUnit.OwnerPlayer.GetDetectedUnitByUnitId(this.Id);
            if (detectedUnit != null)
            {
                //report.TargetPlatformDetectedId = detectUnit.Id;
                report.TargetPlatformId = detectedUnit.Id;
            }            

            OwnerPlayer.SendNewBattleDamageReport(reportToTarget);
            missileUnit.OwnerPlayer.SendNewBattleDamageReport(report);

			missileUnit.HitPoints = 0;
			missileUnit.IsMarkedForDeletion = true;
			missileUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
			if (HitPointsDamage > 0)
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				SendDetectedUnitInfoForThisUnit();
			}
            missileUnit.OwnerPlayer.RemoveTargettingForDetectedUnitAfterImpact(missileUnit, report.IsTargetHit);

			if (report.IsTargetPlatformDestroyed)
			{
				IsMarkedForDeletion = true;
				UnitToBeDestroyed();
				bool defeat = OwnerPlayer.HasBeenDefeated;
			}
		}

        /// <summary>
        /// If this unit has countermeasures for incoming hostile missileUnit, it attempts to take them out. Does
        /// not remove or destroy missile, but reports true if it has disabled it.
        /// </summary>
        /// <param name="missileUnit"></param>
        /// <returns></returns>
        public bool InflictMissileSoftkill(MissileUnit missileUnit)
        {
            var softKillWeapons = from w in Weapons
                                  where w.WeaponClass.IsNotWeapon && w.IsReady && w.WeaponClass.EwCounterMeasures != GameConstants.EwCounterMeasuresType.None
                                  select w;
            if (!softKillWeapons.Any())
            {
                return false;
            }
            foreach (var wpn in softKillWeapons)
            {
                if (missileUnit.CanBeSoftKilled(wpn))
                {
                    if (wpn.MaxAmmunition > 0) //if maxammo is 0, this means unlimited
                    {
                        wpn.AmmunitionRemaining--;
                    }
                    double hitPercent = wpn.WeaponClass.HitPercent;
                    if (missileUnit.LaunchWeapon != null && missileUnit.LaunchWeapon.WeaponClass.EwCounterMeasureResistancePercent > 0)
                    {
                        hitPercent *=  (1.0 - (missileUnit.LaunchWeapon.WeaponClass.EwCounterMeasureResistancePercent / 100.0));
                    }
                    if (wpn.WeaponClass.EwCounterMeasures == GameConstants.EwCounterMeasuresType.Chaff 
                        || wpn.WeaponClass.EwCounterMeasures == GameConstants.EwCounterMeasuresType.Flare)
                    {
                        var gsinfo = new GameStateInfo(GameConstants.GameStateInfoType.UnitUsesCountermeasures, Id) { WeaponClassId = wpn.WeaponClass.Id };
                        OwnerPlayer.Send(gsinfo);
                        foreach (var pl in GameManager.Instance.Game.Players)
                        {
                            if (pl.Id != this.OwnerPlayer.Id && pl.IsCompetitivePlayer && !pl.IsComputerPlayer)
                            {
                                var detUnit = pl.GetDetectedUnitByUnitId(this.Id);
                                if (detUnit != null && detUnit.IsIdentified)
                                {
                                    pl.Send(
                                    new GameStateInfo(GameConstants.GameStateInfoType.EnemyUnitUsesCountermeasures, detUnit.Id)
                                    {
                                        WeaponClassId = wpn.WeaponClass.Id,
                                    });
                                }
                            }
                        }
                    }
                    if (GameManager.Instance.ThrowDice(hitPercent))
                    {
                        GameManager.Instance.Log.LogDebug(string.Format("InflictMissileSoftkill: Missile {0} was SOFT KILLED by {1}", missileUnit, wpn));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }


		/// <summary>
		/// When unit has changed, ensures that all other players who can see this unit
		/// receives notified updates to that effect.
		/// </summary>
		private void SendDetectedUnitInfoForThisUnit()
		{
			foreach (var p in GameManager.Instance.Game.Players)
			{
				if (p.IsCompetitivePlayer && p.TcpPlayerIndex > 0)
				{
					var det = p.GetDetectedUnitByUnitId(this.Id);
					if (det != null)
					{
						det.SetDirty(GameConstants.DirtyStatus.UnitChanged);
					}
				}
			}
		}

		/// <summary>
		/// Makes changes to a BattleDamageReport by setting priorities and creating
		/// user friendly messages to attacker and attackee.
		/// </summary>
		/// <param name="report"></param>
		private void UpdateDamageReportPriorityAndMessage(BattleDamageReport report)
		{
			report.AttackeePriority = GameConstants.Priority.Normal;
			report.AttackerPriority = GameConstants.Priority.Normal;
			var targetUnit = GameManager.Instance.Game.GetUnitById(report.TargetPlatformId);
			var attackingUnit = GameManager.Instance.Game.GetUnitById(report.PlatformInflictingDamageId);
			if (targetUnit == null || attackingUnit == null)
			{
				return;
			}
			//Debug.Assert(targetUnit != null, "TargetUnit cannot be null.");
			//Debug.Assert(attackingUnit != null, "AttackingUnit cannot be null.");
			if (targetUnit.SupportsRole(GameConstants.Role.IsSurfaceCombattant) ||
				targetUnit.SupportsRole(GameConstants.Role.IsSubmarine))
			{
				report.AttackeePriority = GameConstants.Priority.Critical;
				report.AttackerPriority = GameConstants.Priority.Critical;
			}
			if (!report.IsTargetHit)
			{
				report.AttackerPriority = GameConstants.Priority.Low;
				report.AttackeePriority = GameConstants.Priority.Low;
			}

			if (targetUnit.UnitClass.IsAircraft && !targetUnit.UnitClass.IsMissileOrTorpedo)
			{
				if (report.IsTargetPlatformDestroyed)
				{ 
                    //report.MessageToAttackee = string.Format("A {0} was SHOT DOWN by a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName) ;
                    //report.MessageToAttacker = string.Format("We shot down an enemy {0} using a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName);

                    report.AttackerPriority = GameConstants.Priority.Elevated;
                    report.AttackeePriority = GameConstants.Priority.Elevated;
				}
				else if (report.IsTargetHit)
				{
                    //report.MessageToAttackee = string.Format( "A {0} was damaged by a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName );
                    //report.MessageToAttacker = string.Format( "We damaged an enemy {0} using a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName );
				}
				else
				{
                    //report.MessageToAttackee = string.Format("An enemy {0} missed one of our {1}",
                    //    report.WeaponClassName, report.TargetPlatformClassName);
                    //report.MessageToAttacker = string.Format("Our {0} missed an enemy {1}",
                    //    report.WeaponClassName, report.TargetPlatformClassName);
                    //if (report.IsMissileOutOfFuel)
                    //{
                    //    report.MessageToAttacker += " (out of fuel)";
                    //}
				}
			}
			else if (targetUnit.UnitClass.IsMissileOrTorpedo)
			{
				report.AttackerPriority = GameConstants.Priority.Low;
                report.AttackeePriority = GameConstants.Priority.Low;
				if (report.IsTargetPlatformDestroyed)
				{
                    //report.MessageToAttackee = string.Format("A {0} was SHOT DOWN by a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We shot down an enemy {0} using a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName);
				}
                else if (report.IsTargetHit)
				{
                    //report.MessageToAttackee = string.Format("A {0} was damaged by a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We damaged an enemy {0} using a {1}",
                    //    report.TargetPlatformClassName, report.WeaponClassName);
				}
				else
				{
                    //report.MessageToAttackee = string.Format("An enemy {0} missed one of our {1}",
                    //    report.WeaponClassName, report.TargetPlatformClassName);
                    //report.MessageToAttacker = string.Format("Our {0} missed an enemy {1}",
                    //    report.WeaponClassName, report.TargetPlatformClassName);
                    //if (report.IsMissileOutOfFuel)
                    //{
                    //    report.MessageToAttacker += " (out of fuel)";
                    //}
				}
			}
			else if (targetUnit.UnitClass.IsSubmarine || targetUnit.UnitClass.IsSurfaceShip)
			{
				if (report.IsTargetPlatformDestroyed)
				{
                    //report.MessageToAttackee = string.Format("{0}, a {1} class, was SUNK by an enemy {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We SUNK an enemy {0}, a {1} class, using a {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
				}
                else if (report.IsTargetHit)
				{
                    //report.MessageToAttackee = string.Format("{0}, a {1} class, was DAMAGED by an enemy {2} ({3}% dmg total)",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName, this.DamagePercent());
                    //report.MessageToAttacker = string.Format("We DAMAGED an enemy {0}, a {1} class, using a {2} ({3}% dmg total)",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName, this.DamagePercent());
				}
				else
				{
                    //report.MessageToAttackee = string.Format("An enemy {0} missed  {1} (a {2} class)",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //report.MessageToAttacker = string.Format("Our {0} missed {1} (a {2} class)",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //if (report.IsMissileOutOfFuel)
                    //{
                    //    report.MessageToAttacker += " (out of fuel)";
                    //}
				}

			}
			else if (targetUnit.UnitClass.IsLandbased)
			{
				if (report.IsTargetPlatformDestroyed)
				{
                    //report.MessageToAttackee = string.Format("{0} ({1}), was DESTROYED by an enemy {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We DESTROYED the enemy {0} ({1}) using a {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
					report.AttackeePriority = GameConstants.Priority.Critical;
					report.AttackerPriority = GameConstants.Priority.Critical;

				}
                else if (report.IsTargetHit)
				{
                    //report.MessageToAttackee = string.Format("{0} ({1}) was DAMAGED by an enemy {2} ({3}% dmg total)",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName, this.DamagePercent());
                    //report.MessageToAttacker = string.Format("We DAMAGED an enemy {0} ({1}), using a {2} ({3}% dmg total)",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName, this.DamagePercent());
					report.AttackeePriority = GameConstants.Priority.Critical;
				}
				else
				{
                    //report.MessageToAttackee = string.Format("An enemy {0} missed  {1} ({2})",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //report.MessageToAttacker = string.Format("Our {0} missed {1} ({2})",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //report.MessageToAttacker += " (out of fuel)";
				}

			}
			else //not a plane, missile, ship or landbased thingy
			{
				if (report.IsTargetPlatformDestroyed)
				{
                    //report.MessageToAttackee = string.Format("{0}, a {1} class, was SUNK by an enemy {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We DESTROYED an enemy {0}, a {1} class, using a {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
				}
                else if (report.IsTargetHit)
				{
                    //report.MessageToAttackee = string.Format("{0}, a {1} class, was DAMAGED by an enemy {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
                    //report.MessageToAttacker = string.Format("We DAMAGED an enemy {0}, a {1} class, using a {2}",
                    //    report.TargetPlatformUnitName, report.TargetPlatformClassName, report.WeaponClassName);
				}
				else
				{
                    //report.MessageToAttackee = string.Format("An enemy {0} missed  {1} (a {2} class)",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //report.MessageToAttacker = string.Format("Our {0} missed {1} (a {2} class)",
                    //    report.WeaponClassName, report.TargetPlatformUnitName, report.TargetPlatformClassName);
                    //report.MessageToAttacker += " (out of fuel)";
				}

			}
            //if (string.IsNullOrEmpty(report.MessageToAttacker) || string.IsNullOrEmpty(report.MessageToAttackee))
            //{
            //    report.MessageToAttackee = report.ToString();
            //    report.MessageToAttacker = report.ToString();
            //}
            //foreach (var critDmg in report.CriticalDamageList)
            //{
            //    //report.MessageToAttacker += "\n" + critDmg;
            //    report.MessageToAttackee += "\n" + critDmg;
            //}
		}

		public override string ToString()
		{
			return string.Format("[{0}] {1} ({2})", Id, Name, UnitClass.UnitClassShortName);
		}

		public virtual string ToShortString()
		{
			return ToString();
		}

		public virtual string ToLongString()
		{
			string posString = "(no position)";
			if (Position != null)
			{
				posString = Position.ToString();
			}
			else if (CarriedByUnit != null)
			{
				posString = "(carried by " + CarriedByUnit.Name + ")";
			}

			string temp = ToShortString() + string.Format(" at {0}  Owner:{1}", posString, OwnerPlayer.Id);
			double damagepercent = DamagePercent();
			if (TimeToLiveSec > 0)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(TimeToLiveSec);
				temp += string.Format("\nLifetime remaining: {0}", timeSpan); 
			}
			if (damagepercent > 0)
			{
				temp += string.Format("\nDamage: {0} %", Math.Round(damagepercent)); 
			}
			if (MaxRangeCruiseM > 0)
			{
				temp += string.Format("\nBingo fuel {0:F} %", BingoFuelPercent);
			}
			return temp;
		}

		/// <summary>
		/// In case of fire, randomly increase or decrease damage, and cause damage to unit.
		/// Also, with high damage, increase it to destroy unit.
		/// </summary>
		/// <param name="gameTimeMs">Number of ms since last call</param>
		public void InflictDamageFromFires(double deltaGameTimeSec)
		{
			if (CarriedByUnit != null || Position == null || HitPoints < 1 || IsMarkedForDeletion)
			{
				return;
			}
			if (FireLevel == GameConstants.FireLevel.NoFire)
			{
				return;
			}
			//GameManager.Instance.Log.LogDebug(
			//    string.Format("InflictDamageFromFires: SecondsGameTimeCovered: {0}", SecondsGameTimeCovered));
			double ProbPerMinutePercent = 10;
			double DamageHitpoints = 0;
			switch (FireLevel)
			{
				case GameConstants.FireLevel.NoFire:
					ProbPerMinutePercent = 0;
					break;
				case GameConstants.FireLevel.MinorFire:
					ProbPerMinutePercent = 5;
					break;
				case GameConstants.FireLevel.MajorFire:
					ProbPerMinutePercent = 20;
					break;
				case GameConstants.FireLevel.SevereFire:
					ProbPerMinutePercent = 35;
					break;
			}
			//Prob = ProbPerMinutePercent * (SecondsGameTimeCovered / 60);
            if (GameManager.Instance.ThrowDiceTime(deltaGameTimeSec, ProbPerMinutePercent))
			{
				DamageHitpoints = GameManager.Instance.GetDamageHitpoints((int)(UnitClass.MaxHitpoints * 0.05));
			}
			if (DamageHitpoints > 0)
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				GameManager.Instance.Log.LogDebug(string.Format(
					"Unit {0} takes damage {1} HP from {2}", 
					ToString(), DamageHitpoints, FireLevel.ToString()));
				HitPoints -= (int)DamageHitpoints;
				SendDetectedUnitInfoForThisUnit();
				if (HitPoints < 0)
				{
					HitPoints = 0;
				}
			}
			if (DamagePercent() > 60 && HitPoints > 0)
			{
				ProbPerMinutePercent = 25;

                if (GameManager.Instance.ThrowDiceTime(deltaGameTimeSec, ProbPerMinutePercent))
				{
					GameManager.Instance.Log.LogDebug(string.Format(
						"Unit {0} takes further damage {1} HP.",
						ToString(), DamageHitpoints));
					DamageHitpoints = GameManager.Instance.GetDamageHitpoints(25);
					HitPoints -= (int)DamageHitpoints;
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
					SendDetectedUnitInfoForThisUnit();
					if (HitPoints < 0)
					{
						HitPoints = 0;
					}

				}
			}
			ProbPerMinutePercent = 10;
			//fires worsen:
			if (GameManager.Instance.ThrowDiceTime(deltaGameTimeSec, ProbPerMinutePercent)) 
			{
				int NewFire = (int)FireLevel + 1;
				if (NewFire > (int)GameConstants.FireLevel.SevereFire)
				{
					NewFire = (int)GameConstants.FireLevel.SevereFire;
				}

				if (NewFire != (int)FireLevel)
				{
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
					FireLevel = (GameConstants.FireLevel)NewFire;
					GameManager.Instance.Log.LogDebug(string.Format(
						"Unit {0} firelevel is now {1}.",
						ToString(), FireLevel.ToString()));
				}
			} //fires ease:
			else if (GameManager.Instance.ThrowDiceTime(deltaGameTimeSec, ProbPerMinutePercent * 2)) 
			{
				int NewFire = (int)FireLevel - 1;
				if (NewFire < (int)GameConstants.FireLevel.NoFire)
				{
					NewFire = (int)GameConstants.FireLevel.NoFire;
				}

				if (NewFire != (int)FireLevel)
				{
					FireLevel = (GameConstants.FireLevel)NewFire;
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
					GameManager.Instance.Log.LogDebug(string.Format(
						"Unit {0} firelevel is now {1}.",
						ToString(), FireLevel.ToString()));
				}

			}
			if (HitPoints <= 0)
			{
				IsMarkedForDeletion = true;
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.UnitHasBeenDestroyedByFire, this.Id));
				GameManager.Instance.Log.LogDebug(string.Format(
					"InflictDamageFromFires: Unit {0} is destroyed.",
					ToString()));
                var btdmgreport = from b in OwnerPlayer.BattleDamageReports
                                  where b.TargetPlatformId == this.Id && b.IsTargetHit
                                  select b;
                if (btdmgreport.Any())
                {
                    GameManager.Instance.Log.LogDebug("InflictDamagesFromFire generates new BattleDamageReport.");
                    var lastBtlDmgRep = btdmgreport.Last<BattleDamageReport>();
                    var fireBtlDmgReport = lastBtlDmgRep.Clone();
                    fireBtlDmgReport.CriticalDamageList = new List<CriticalDamage>();
                    //fireBtlDmgReport.DamageHitpoints = (int)DamageHitpoints;
                    fireBtlDmgReport.DamagePercent = ( int )( ( ( double )DamageHitpoints / ( double )UnitClass.MaxHitpoints ) * 100.0 );
                    if ( fireBtlDmgReport.DamagePercent > 100 )
                    {
                        fireBtlDmgReport.DamagePercent = 100;
                    }
                    fireBtlDmgReport.GameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
                    fireBtlDmgReport.IsTargetPlatformDestroyed = true;
                    //fireBtlDmgReport.MessageToAttackee = "(Destroyed by fire)";
                    //fireBtlDmgReport.MessageToAttacker = "(Destroyed by fire)";

                    var playerShooting = GameManager.Instance.Game.GetPlayerById(fireBtlDmgReport.PlayerInflictingDamageId);
                    if (playerShooting != null)
                    {
                        playerShooting.BattleDamageReports.Add(fireBtlDmgReport);
                    }
                    if (fireBtlDmgReport.TargetPlatformRoles == null || fireBtlDmgReport.TargetPlatformRoles.Count == 0)
                    {
                        GameManager.Instance.Log.LogWarning("InflictDamagesFromFire: New BattleDamageReport has null or empty roles list for unit " + ToShortString());
                    }
                    OwnerPlayer.BattleDamageReports.Add(fireBtlDmgReport);
                }
                else
                {
                    GameManager.Instance.Log.LogError("InflictDamagesFromFire: Could not find old BattleDamageReports on unit " + this.ToString());
                }
                foreach (var player in GameManager.Instance.Game.Players)
                {
                    if (player.Id != OwnerPlayer.Id)
                    {
                        var detUnit = player.GetDetectedUnitByUnitId(this.Id);
                        if (detUnit != null)
                        {
                            var gsInfo = new GameStateInfo(GameConstants.GameStateInfoType.EnemyUnitHasBeenDestroyedByFire, detUnit.Id);
                            player.Send(gsInfo);
                        }
                        else
                        {
                            var gsInfo = new GameStateInfo();
                            gsInfo.InfoType = GameConstants.GameStateInfoType.EnemyUnitHasBeenDestroyedByFire;
                            gsInfo.UnitClassId = this.UnitClass.Id;
                            player.Send(gsInfo);
                        }
                    }
                }
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				UnitToBeDestroyed();
			}
		}

		/// <summary>
		/// Called when unit has been destroyed to release dependencies, update groups, reassign flying units, etc.
		/// </summary>
		public void UnitToBeDestroyed()
		{
            OwnerPlayer.ExecuteEventTriggers();
            if (OwnerPlayer.Enemies.Count > 0)
            {
                foreach (var pl in OwnerPlayer.Enemies)
                {
                    pl.ExecuteEventTriggers();
                }
                //OwnerPlayer.Enemies[0].ExecuteEventTriggers();
            }
            if (AircraftHangar != null)
            {
                foreach (var air in OwnerPlayer.Units.Where(u=>u.Position != null && u.LaunchedFromUnit != null && u.LaunchedFromUnit.Id == this.Id))
                {
                    air._hasCheckedForNearestCarrier = false;
                    air.SetHomeToNewCarrier();
                }
            }
			if (IsMarkedForDeletion)
			{
				if (Group != null)
				{
				    Group.RemoveUnit(this);
				}
			}
		}

		/// <summary>
		/// Returns max weapon range for the specified domain (air, surface, land, sub) for this unit,
		/// only taking into account weapons that are not damaged and has ammo.
		/// </summary>
		/// <param name="domainType"></param>
		/// <returns></returns>
		public double GetMaxWeaponRangeM(GameConstants.DomainType domainType)
		{
			var relevantWeapons = Weapons.Where(w => w.CanTargetDomain(domainType) && w.AmmunitionRemaining > 0 && !w.WeaponClass.IsNotWeapon && w.ReadyInSec < 60);
			if (relevantWeapons.Any())
			{
				var maxRange = relevantWeapons.Max(w=>w.WeaponClass.MaxWeaponRangeM);
				return maxRange;
			}
			else
			{
				return 0.0;
			}
		}

		public int TotalHitpointsAttackTarget(DetectedUnit detectedUnit)
		{
		    return Weapons.Where(weapon => weapon.ReadyInSec < 60 && weapon.IsOperational && 
                weapon.CanTargetDetectedUnit(detectedUnit.RefersToUnit, false)).Sum(weapon => weapon.WeaponClass.DamageHitPoints*weapon.AmmunitionRemaining);
		}

	    public bool CanTargetDetectedUnit(DetectedUnit detectedUnit, bool ignoreNoAmmo)
		{
			if (detectedUnit.RefersToUnit == null)
			{
				return false;
			}
	        return Weapons.Any(weapon => weapon.ReadyInSec < 60 && weapon.IsOperational && weapon.CanTargetDetectedUnit(detectedUnit.RefersToUnit, ignoreNoAmmo));
		}

		/// <summary>
		/// Returns true if any weapon is ready to fire on DetectedUnit, has ammo and range (max and min), 
		/// and is in sector range.
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public bool CanImmediatelyFireOnTargetType(DetectedUnit detectedUnit)
		{
            var weapons = (from weapon in Weapons
		            where weapon.IsReady && weapon.IsOperational && weapon.CanTargetDetectedUnit(detectedUnit.RefersToUnit, false)
		            let distanceM = MapHelper.CalculateDistance3DM(Position, detectedUnit.Position)
		            where distanceM <= weapon.WeaponClass.MaxWeaponRangeM && distanceM >= weapon.WeaponClass.MinWeaponRangeM
		            select weapon);

            // Aircraft can fire if any weapon was found.
            if (this.UnitClass.IsAircraft)
            {
                return weapons.Any();
            }

            // Other units can only fire if target is within sector range.
            return weapons.Any(weapon => weapon.IsCoordinateInSectorRange(detectedUnit.Position.Coordinate));
		}

        /// <summary>
        /// Returns true if, and only if, any of the unit's weapons are ready to fire on the target immediately, and it is within
        /// optimal range to fire.
        /// </summary>
        /// <param name="detectedUnit"></param>
        /// <returns></returns>
        public bool ShouldImmediatelyFireOnTargetType(DetectedUnit detectedUnit)
        {
            var weapons = (from weapon in Weapons
                    where weapon.IsReady && weapon.IsOperational && weapon.CanTargetDetectedUnit(detectedUnit.RefersToUnit, false)
                    let distanceM = MapHelper.CalculateDistance3DM(Position, detectedUnit.Position)
                    where distanceM <= weapon.CalculateDesiredWeaponDistanceToTargetM(detectedUnit) && distanceM >= weapon.WeaponClass.MinWeaponRangeM
                    select weapon);

            // Aircraft should fire if any weapon was found.
            if (this.UnitClass.IsAircraft)
            {
                return weapons.Any();
            }

            // Other units should only fire if target is within sector range.
            return weapons.Any(weapon => weapon.IsCoordinateInSectorRange(detectedUnit.Position.Coordinate));
        }


	    #endregion

		#region "Private and protected methods"


        /// <summary>
        /// Called by the properties that read terrain height, checks at regular intervals if enough time
        /// has lapsed to reread the terrain data into the property for future reference. Ensures that not all
        /// methods call the terrain data constantly.
        /// </summary>
        protected void CheckForTerrainUpdates()
        {
            if (this.Position == null)
            {
                return;
            }
            var gameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
            if (gameWorldTimeSec > _GameWorldTimeNextTerrainCheck)
            {
                _GameWorldTimeLastTerrainCheck = gameWorldTimeSec;
                _GameWorldTimeNextTerrainCheck = gameWorldTimeSec + GetTerrainRefreshIntervalSec();
                _TerrainHeightAtPosM = TerrainReader.GetHeightM(Position.Coordinate);
                if (ActualSpeedKph < 1 || !this.Position.HasBearing)
	            {
                    _TerrainHeight10SecForwardM = _TerrainHeightAtPosM;
                    _TerrainHeight30SecForwardM = _TerrainHeightAtPosM;
	            }
                else
                {
                    var distanceM = ActualSpeedKph.ToMperSecFromKph() * 10.0;
                    var coordForward = MapHelper.CalculateNewPosition2(Position.Coordinate, (double)this.Position.BearingDeg, distanceM);
                    _TerrainHeight10SecForwardM = TerrainReader.GetHeightM(coordForward);
                    distanceM = ActualSpeedKph.ToMperSecFromKph() * 30.0;
                    coordForward = MapHelper.CalculateNewPosition2(Position.Coordinate, (double)this.Position.BearingDeg, distanceM);
                    _TerrainHeight30SecForwardM = TerrainReader.GetHeightM(coordForward);
                }
            }
            
        }

        protected int GetTerrainRefreshIntervalSec()
        {
            switch (DomainType)
            {
                case GameConstants.DomainType.Surface:
                    return 67;
                case GameConstants.DomainType.Air:
                    return 7; //11;
                case GameConstants.DomainType.Subsea:
                    return 67;
                case GameConstants.DomainType.Land:
                    return 999999;
                default:
                    return 89;
            }
        }

		/// <summary>
		/// Inflict damage randomly to critical components and carried units, and start fires. Contains hardcoded
		/// probabilities.
		/// </summary>
		/// <param name="HitPoints">Number of hitpoints damage inflicted</param>
		private List<CriticalDamage> InflictCriticalDamage(int hitPoints, bool isAntiRadiationWeapon)
		{
			var CriticalDamageReportList = new List<CriticalDamage>();
			if (hitPoints < 1)
			{
				return CriticalDamageReportList;
			}
			double PercentDamageThisHit = (hitPoints / (double)UnitClass.MaxHitpoints) * 100.0;
			if (PercentDamageThisHit > 10) //more than 10% damage
			{
				if (GameManager.Instance.ThrowDice(25)) //1 in 4 chance
				{
					int fireLevel = (int)this.FireLevel + 1;
					if (GameManager.Instance.ThrowDice(25))
					{
						fireLevel += 1;
					}
					if (fireLevel > (int)GameConstants.FireLevel.SevereFire)
					{
						fireLevel = (int)GameConstants.FireLevel.SevereFire;
					}
					FireLevel = (GameConstants.FireLevel)fireLevel;
					CriticalDamage fireDamage = new CriticalDamage();
                    fireDamage.CriticalDamageComponentType = GameConstants.CriticalDamageType.Fire;
					fireDamage.FireLevel = FireLevel;
					CriticalDamageReportList.Add(fireDamage);
				}
				var dmg = new CriticalDamage();
				int DamageTimeMin = GameManager.Instance.GetRandomNumber(240) + 1;
				TimeSpan timeready = TimeSpan.FromMinutes(DamageTimeMin);
                if (isAntiRadiationWeapon) //if anti-radiation, missile, 100% chance of damage to some radar
                {
                    var radars = from r in Sensors
                                 where r.SensorClass.SensorType == GameConstants.SensorType.Radar && r.ReadyInSec < 60
                                 select r;
                    var aRadar = radars.FirstOrDefault<BaseSensor>();
                    if (aRadar != null)
                    {
                        dmg.ComponentId = aRadar.Id;
                        dmg.CriticalDamageComponentType = GameConstants.CriticalDamageType.SensorDamaged;
                        SetDirty(GameConstants.DirtyStatus.UnitChanged);
                        aRadar.ReadyInSec = DamageTimeMin * 60;
                        dmg.ReadyInSec = timeready.TotalSeconds;
                        CriticalDamageReportList.Add(dmg);
                    }
                }
				if (GameManager.Instance.ThrowDice(50) && _components.Count > 0) //1 in 2 chance
				{
					int CompNo = GameManager.Instance.GetRandomNumber(_components.Count);

					BaseComponent comp = _components.ElementAt(CompNo).Value;

					if (comp != null && comp.ReadyInSec < 60)
					{
						comp.ReadyInSec = DamageTimeMin * 60;
						if (comp is BaseWeapon)
						{
                            dmg.ComponentId = comp.Id;
							SetDirty(GameConstants.DirtyStatus.UnitChanged);
							dmg.CriticalDamageComponentType = GameConstants.CriticalDamageType.WeaponDamaged;
							dmg.ReadyInSec = timeready.TotalSeconds;
							
						}
						else if (comp is BaseSensor)
						{
							BaseSensor sens = comp as BaseSensor;
							
							if (sens != null)
							{
								sens.IsDamaged = true;
                                dmg.ComponentId = comp.Id;
								SetDirty(GameConstants.DirtyStatus.UnitChanged);
								if (sens.SensorClass.SensorType == GameConstants.SensorType.Visual) //no damage to visual
								{
									comp.ReadyInSec = 0;
									dmg.ComponentId = string.Empty;
								}
								else
								{
									dmg.CriticalDamageComponentType = GameConstants.CriticalDamageType.SensorDamaged;
									dmg.ReadyInSec = comp.ReadyInSec;
								}
							}
						}
						if (!string.IsNullOrEmpty(dmg.ComponentId))
						{
							CriticalDamageReportList.Add(dmg);
						}
					}
				}
				else if (AircraftHangar != null && GameManager.Instance.ThrowDice(50) &&
					PercentDamageThisHit > 10 && this.DamagePercent() > 30)
				{
					dmg.ReadyInSec = timeready.TotalSeconds;
					dmg.CriticalDamageComponentType = GameConstants.CriticalDamageType.AircraftHangarDamaged;
					CriticalDamageReportList.Add(dmg);
				}
			}

			return CriticalDamageReportList;
		}


		/// <summary>
		/// Based on how much game time has elapsed, deducts fuel from the unit's tanks,
		/// (really changing FuelDistanceCoveredSinceRefuelM property). Takes into account
		/// unit speed.
		/// </summary>
		/// <param name="distanceMeters"></param>
		/// <param name="elapsedTimeSec"></param>
		public void UpdateFuelDistanceCovered(double distanceMeters, double elapsedTimeSec)
		{
            FuelDistanceCoveredSinceRefuelM = GetFuelDistanceCovered(distanceMeters, elapsedTimeSec);
            CheckForBingoFuel();
		}

        private double GetFuelDistanceCovered(double distanceMeters, double elapsedTimeSec)
        {
            if (distanceMeters <= 0 || elapsedTimeSec <= 0)
            {
                return FuelDistanceCoveredSinceRefuelM;
            }
            double normalizedDistanceMeters = distanceMeters;
            if (this.UnitClass.IsAircraft && ActualSpeedKph < 10) //Hovering helicopters/VTOL use same fuel as if cruising
            {
                double normalizedSpeedMs = UnitClass.CruiseSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
                normalizedDistanceMeters = normalizedSpeedMs * elapsedTimeSec;
            }
            else if (ActualSpeedKph > UnitClass.CruiseSpeedKph && !UnitClass.IsMissileOrTorpedo)
            {
                normalizedDistanceMeters = normalizedDistanceMeters * GetFuelEnduranceModifier();
            }
            return (FuelDistanceCoveredSinceRefuelM + normalizedDistanceMeters);
            //if (this.UnitClass.UnitType == GameConstants.UnitType.Helicopter)
            //{
            //    double distanceHomeM = 0;
            //    if (LaunchedFromUnit != null && LaunchedFromUnit.Position != null)
            //    {
            //        distanceHomeM = MapHelper.CalculateDistanceMeters(Position.Coordinate, LaunchedFromUnit.Position.Coordinate);
            //    }
            //    GameManager.Instance.Log.LogDebug(
            //        string.Format("++ UpdateFuelDistanceCovered(): Unit {0} used {1:F}m fuel for {2:F}m movement.\n"+
            //        "Total fuel distance covered: {3:F}m. Distance home is {4:F}m.",
            //        ToShortString(), normalizedDistanceMeters, DistanceMeters, FuelDistanceCoveredSinceRefuelM, distanceHomeM));
            //}
            //if (normalizedDistanceMeters > DistanceMeters)
            //{ 
            //    GameManager.Instance.Log.LogDebug(
            //        string.Format("++ UpdateFuelDistanceCovered(): Unit {0} used {1:F}m fuel for {2:F}m movement.",
            //        ToShortString(),normalizedDistanceMeters,DistanceMeters));
            //}
        }

		/// <summary>
		/// Updates the elevation (height/depth) of the unit, based on DesireHeightOverSeaLevelM,
		/// current speed and how many seconds have elapsed. 
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns>Change in elevation in M</returns>
        public double UpdateElevation(double deltaGameTimeSec, double actualSpeedKph)
		{
			Debug.Assert(UnitClass != null, "BaseUnit.UpdateElevation: UnitClass should never be null.");
			
            double maxSpeedMs = actualSpeedKph * GameConstants.KPH_TO_MS_CONVERSION_FACTOR;
            double maxElevationChange = maxSpeedMs * deltaGameTimeSec;

            // Special handling of missiles. Should go to height of waypoint if set.
            if (UnitClass.IsMissileOrTorpedo)
            {
                var activeWaypoint = GetActiveWaypoint();
                if (activeWaypoint != null && activeWaypoint.Position != null && activeWaypoint.Position.HasHeightOverSeaLevel)
                {
                    DesiredHeightOverSeaLevelM = activeWaypoint.Position.HeightOverSeaLevelM.Value;
                }
                else if (UserDefinedElevation != null)
                {
                    DesiredHeightOverSeaLevelM = GetElevationMFromHeightDepthMark(UserDefinedElevation.Value);
                }
            }
            else if (UserDefinedElevation != null)
            {
                DesiredHeightOverSeaLevelM = GetElevationMFromHeightDepthMark(UserDefinedElevation.Value);
            }
            else
            {
                var activeWaypoint = GetActiveWaypoint();
                if (activeWaypoint != null && activeWaypoint.Position != null && activeWaypoint.Position.HasHeightOverSeaLevel)
                {
                    DesiredHeightOverSeaLevelM = activeWaypoint.Position.HeightOverSeaLevelM.Value;
                }
            }

			if (ActualHeightOverSeaLevelM == null)
			{
				return 0;
			}
			double oldActualHeightOverSeaLevelM = (double)ActualHeightOverSeaLevelM;
			if (DesiredHeightOverSeaLevelM != null)
			{
				DesiredHeightOverSeaLevelM = DesiredHeightOverSeaLevelM.Value.Clamp(
					GetMinHeightOverSeaLevelM(), 
					GetMaxHeightOverSeaLevelM()); //if unit is outside current bounds, justify
			}
			if (DesiredHeightOverSeaLevelM != null && Position.HasHeightOverSeaLevel
				&& DesiredHeightOverSeaLevelM != ActualHeightOverSeaLevelM)
			{
				double MaxElevationIncrease = (UnitClass.MaxClimbrateMSec * deltaGameTimeSec).Clamp(0,maxElevationChange);

				double MaxElevationDecrease = (UnitClass.MaxFallMSec * deltaGameTimeSec).Clamp(
					Math.Abs(maxElevationChange) * -1, maxElevationChange);
				if (MaxElevationDecrease < 0)
				{
					MaxElevationDecrease *= -1; //make it a positive number anyway
				}
				if (Position.HeightOverSeaLevelM < DesiredHeightOverSeaLevelM)
				{
					Position.HeightOverSeaLevelM += MaxElevationIncrease;
					if (Position.HeightOverSeaLevelM > DesiredHeightOverSeaLevelM)
					{
						Position.HeightOverSeaLevelM = DesiredHeightOverSeaLevelM;
					}
				}
				else if (ActualHeightOverSeaLevelM > DesiredHeightOverSeaLevelM)
				{
					Position.HeightOverSeaLevelM -= MaxElevationDecrease;
					if (Position.HeightOverSeaLevelM < DesiredHeightOverSeaLevelM)
					{
						Position.HeightOverSeaLevelM = DesiredHeightOverSeaLevelM;
					}
				}
				Position.HeightOverSeaLevelM = ((double)ActualHeightOverSeaLevelM).Clamp(
					UnitClass.LowestOperatingHeightM, UnitClass.HighestOperatingHeightM);
				CurrentRangeReductionPercent = CalculateCurrentRangeReductionPercent();
			}
            //Forcibly clamp at terrain
            var highestTerrainInPathM = MapHelper.GetHighestValue(TerrainHeightAtPosM, TerrainHeight10SecForwardM);
            if (ActualHeightOverSeaLevelM > 0)
            {
                if (ActualHeightOverSeaLevelM < highestTerrainInPathM)
                {
                    ActualHeightOverSeaLevelM = highestTerrainInPathM;
                }
            }
            if (ActualHeightOverSeaLevelM < 0)
            {
                if (ActualHeightOverSeaLevelM < highestTerrainInPathM)
                {
                    ActualHeightOverSeaLevelM = highestTerrainInPathM;
                }
            }
			return (double)ActualHeightOverSeaLevelM - oldActualHeightOverSeaLevelM; 
		}

        public double GetElevationMFromHeightDepthMark(GameConstants.HeightDepthPoints heightDepthPoints)
        {
            var elevationM = 0.0;
            if (heightDepthPoints == GameConstants.HeightDepthPoints.MaxDepth)
            {
                elevationM = this.GetMinHeightOverSeaLevelM();
            }
            else if (heightDepthPoints == GameConstants.HeightDepthPoints.MaxHeight)
            {
                elevationM = this.GetMaxHeightOverSeaLevelM();
            }
            else
            {
                elevationM = MapHelper.GetElevationMFromHeightDepthMark(heightDepthPoints);
            }
            //Justify for terrain height
            var highestTerrainHeightInPathM = MapHelper.GetHighestValue(TerrainHeightAtPosM, TerrainHeight10SecForwardM, TerrainHeight30SecForwardM);
            if (elevationM < 0)
            {
                if (elevationM < TerrainHeight30SecForwardM)
                {
                    elevationM = TerrainHeight30SecForwardM;
                }
            }
            if (elevationM > 0)
            {
                if (elevationM <= highestTerrainHeightInPathM + 10)
                {
                    elevationM = highestTerrainHeightInPathM + 10;
                }
            }
            return elevationM;
        }

		/// <summary>
		/// For units with movement range restrictions, calculates reduction in (default)
		/// range in percent based on load level and current speed and PropulsionSystem. Ensures that
		/// afterburner consumes much more fuel than cruise speed.
		/// </summary>
		/// <returns></returns>
		protected double CalculateCurrentRangeReductionPercent()
		{
			if (UnitClass.MaxRangeCruiseM < 1 || !UnitClass.IsAircraft) //Only for aircraft and unit that has no range restriction
			{
				return 0;
			}
			double rangeReductionPercent = 0;
			double HeightOslM = 0;
			GameConstants.HeightDepthPoints HeightMark = GameConstants.HeightDepthPoints.Surface;
			if (Position != null && Position.HasHeightOverSeaLevel)
			{
				HeightOslM = Position.HeightOverSeaLevelM.Value;
			}
			else if (CarriedByUnit != null && CarriedByUnit.Position != null)
			{
				HeightOslM = CarriedByUnit.Position.HeightOverSeaLevelM.Value;
			}
			HeightMark = HeightOslM.ToHeightDepthMark();
			GameConstants.UnitSpeedType SpeedType = GetSpeedTypeFromKph(ActualSpeedKph);


			switch (UnitClass.PropulsionSystem)
			{
				case GameConstants.PropulsionSystem.TurboJet:
				case GameConstants.PropulsionSystem.TurboFan:
					switch (this.LoadLevel)
					{
						case 0: //no load
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 50;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 40;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 0;
									break;
								default:
									break;
							}
							break;
						case 1: //medium loaded
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 60;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 50;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 20;
									break;
								default:
									break;
							}
							break;
						case 2: //heavy loaed
						case 3:
						case 4:
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 70;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 60;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 40;
									break;
								default:
									break;
							}
							break;
						default:
							break;
					}
					break;
				case GameConstants.PropulsionSystem.TurboProp:
				case GameConstants.PropulsionSystem.Piston:
					switch (this.LoadLevel)
					{
						case 0: //no load
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 20;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 10;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 0;
									break;
								default:
									break;
							}
							break;
						case 1: //medium loaded
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 30;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 20;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 10;
									break;
								default:
									break;
							}
							break;
						case 2: //heavy loaed
						case 3:
						case 4:
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 40;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 30;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 20;
									break;
								default:
									break;
							}
							break;
						default:
							break;
					}

					break;
				case GameConstants.PropulsionSystem.TurboShaft:
					switch (this.LoadLevel)
					{
						case 0: //no load
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 0;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 10;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 0;
									break;
								default:
									break;
							}
							break;
						case 1: //medium loaded
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 10;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 20;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 0;
									break;
								default:
									break;
							}
							break;

						case 2: //heavy loaed
						case 3:
						case 4:
							switch (HeightMark)
							{
								case GameConstants.HeightDepthPoints.Surface:
								case GameConstants.HeightDepthPoints.VeryLow:
								case GameConstants.HeightDepthPoints.Low:
									rangeReductionPercent = 20;
									break;
								case GameConstants.HeightDepthPoints.MediumHeight:
									rangeReductionPercent = 30;
									break;
								case GameConstants.HeightDepthPoints.High:
								case GameConstants.HeightDepthPoints.VeryHigh:
									rangeReductionPercent = 0;
									break;
								default:
									break;
							}
							break;
						default:
							break;
					}
					break;
			}

			return rangeReductionPercent;
		}

		public GameConstants.UnitSubType GetUnitSubType()
		{
			GameConstants.UnitSubType subType = GameConstants.UnitSubType.Other;
			switch (UnitClass.UnitType)
			{
				case GameConstants.UnitType.SurfaceShip:
					subType = GameConstants.UnitSubType.SurfaceShipSupport;
					if (this.SupportsRole(GameConstants.Role.LaunchFixedWingAircraft))
					{
						subType = GameConstants.UnitSubType.AircraftCarrier;
					}
					else if (this.SupportsRole(GameConstants.Role.IsLittoralPatrol))
					{
						subType = GameConstants.UnitSubType.LittoralPatrol;
					}
					else if (this.SupportsRole(GameConstants.Role.IsSurfaceCombattant))
					{
						subType = GameConstants.UnitSubType.SurfaceShipCombattant;
					}
					break;
				case GameConstants.UnitType.FixedwingAircraft:
					subType = GameConstants.UnitSubType.FixedWingSupport;
					if (this.SupportsRole(GameConstants.Role.AttackSurface)
						|| this.SupportsRole(GameConstants.Role.AttackSurfaceStandoff)
						|| this.SupportsRole(GameConstants.Role.AttackLand))
					{
						subType = GameConstants.UnitSubType.FixedWingStrike;
					}
					else if (this.SupportsRole(GameConstants.Role.InterceptAircraft))
					{
						subType = GameConstants.UnitSubType.FixedWingFighter;
					}
					break;
				case GameConstants.UnitType.Helicopter:
					subType = GameConstants.UnitSubType.HelicopterOther;
					if (this.SupportsRole(GameConstants.Role.ASW))
					{
						subType = GameConstants.UnitSubType.HelicopterAsw;
					}
					else if (this.SupportsRole(GameConstants.Role.AttackSurface)
						|| this.SupportsRole(GameConstants.Role.AttackSurfaceStandoff)
						|| this.SupportsRole(GameConstants.Role.AttackLand))
					{
						subType = GameConstants.UnitSubType.HelicopterStrike;
					}

					break;
				case GameConstants.UnitType.Submarine:
					subType = GameConstants.UnitSubType.Submarine;
					break;
				case GameConstants.UnitType.Missile:
					subType = GameConstants.UnitSubType.Missile;
					break;
				case GameConstants.UnitType.Torpedo:
					subType = GameConstants.UnitSubType.Torpedo;
					break;
				case GameConstants.UnitType.Mine:
					break;
				case GameConstants.UnitType.Decoy:
					break;
				case GameConstants.UnitType.Sonobuoy:
					break;
				case GameConstants.UnitType.BallisticProjectile:
					break;
				case GameConstants.UnitType.Bomb:
					break;
				case GameConstants.UnitType.LandInstallation:
					subType = GameConstants.UnitSubType.LandOther;
					if (this.SupportsRole(GameConstants.Role.LaunchFixedWingAircraft))
					{
						subType = GameConstants.UnitSubType.LandAirport;
					}
					break;
				default:
					break;
			}
			return subType;
		}

		/// <summary>
		/// Return weapon most ready and able to target detectedUnit. If weaponClassId is supplied, 
		/// search limited to weapons of that class.
		/// </summary>
		/// <param name="weaponClassId"></param>
		/// <param name="detectedUnit"></param>
		/// <returns></returns>
		public EngagementStatus GetBestAvailableWeapon(string weaponClassId, DetectedUnit detectedUnit, bool primaryWeaponOnly)
		{
			var wpnStatus = GetUnitEngagementStatus(weaponClassId, detectedUnit, primaryWeaponOnly);
			return wpnStatus;
		}

		#endregion

		#region "Private methods"

		private void CreateBattleDamageReportOutOfFuel()
		{
			if (this.TargetDetectedUnit == null || !(this is MissileUnit))
			{
				return;
			}
			
			BattleDamageReport report = new BattleDamageReport();
            //report.DamageHitpoints = 0;
            report.IsTargetHit = false;
			report.DamagePercent = 0;
			report.GameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
			report.IsTargetPlatformDestroyed = false;
			report.MissileId = this.Id;
			report.IsMissileOutOfFuel = true;

			BaseUnit targetUnit = TargetDetectedUnit.RefersToUnit;
			var missileUnit = this as MissileUnit;
			report.WeaponClassId = missileUnit.WeaponClassId;
            //WeaponClass weaponClass = GameManager.Instance.GetWeaponClassById(report.WeaponClassId);
            //if (weaponClass != null)
            //{
            //    report.WeaponClassName = weaponClass.WeaponClassName;
            //}
			if (missileUnit.LaunchPlatform != null && missileUnit.LaunchPlatform.UnitClass != null)
			{
				report.PlatformInflictingDamageClassId = missileUnit.LaunchPlatform.UnitClass.Id;
                //report.PlatformInflictingDamageClassName = missileUnit.LaunchPlatform.UnitClass.UnitClassShortName;
				report.PlatformInflictingDamageId = missileUnit.LaunchPlatform.Id;
				report.PlatformInflictingDamageUnitName = missileUnit.LaunchPlatform.Name;
			}
			report.MissileId = missileUnit.Id;
			report.WeaponClassId = missileUnit.WeaponClassId;

			report.PlayerInflictingDamageId = missileUnit.OwnerPlayer.Id;
            //report.PlayerInflictingDamageName = missileUnit.OwnerPlayer.Name;
            //report.TargetPlatformDetectedId = this.TargetDetectedUnit.Id;
            report.TargetPlatformId = this.TargetDetectedUnit.Id;
			if (targetUnit != null)
			{
				report.PlayerSustainingDamageId = targetUnit.OwnerPlayer.Id;
                //report.PlayerSustainingDamageName = targetUnit.OwnerPlayer.Name; report.TargetPlatformClassId = targetUnit.UnitClass.Id;
                //report.TargetPlatformClassName = targetUnit.UnitClass.UnitClassShortName;
				report.TargetPlatformUnitName = targetUnit.Name;
                //report.TargetPlatformId = targetUnit.Id;
			}
			if (this.Position != null)
			{
				report.Position = this.GetPositionInfo();
			}

			UpdateDamageReportPriorityAndMessage(report);

            // Clone report and add to players before we change the ID's to detected unit id
            BattleDamageReport reportToTarget = report.Clone();
            if (this.TargetDetectedUnit.OwnerPlayer.CanCurrentlySeeEnemyUnit(missileUnit.LaunchedFromUnit))
            {
                reportToTarget.RemoveAttackingPlatformData();
            }

			OwnerPlayer.BattleDamageReports.Add(report);
            this.TargetDetectedUnit.OwnerPlayer.BattleDamageReports.Add(reportToTarget);

            // Get detected unit ID's before sending to player
            var detectedUnit = TargetDetectedUnit.OwnerPlayer.GetDetectedUnitByUnitId(reportToTarget.PlatformInflictingDamageId);
            if (detectedUnit != null)
            {
                //report.PlatformInflictingDamageDetectedId = detectedUnit.Id;
                reportToTarget.PlatformInflictingDamageId = detectedUnit.Id;
            }

            DetectedUnit detMissile = TargetDetectedUnit.OwnerPlayer.GetDetectedUnitByUnitId( this.Id );
            if ( detMissile != null )
            {
                //report.MissileDetectedUnitId = detMissile.Id;
                reportToTarget.MissileId = detMissile.Id;
            }

			OwnerPlayer.SendNewBattleDamageReport(report);			
			this.TargetDetectedUnit.OwnerPlayer.SendNewBattleDamageReport(reportToTarget);
		}

		#endregion
    }
}
