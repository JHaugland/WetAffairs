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
using System.Diagnostics;
using TTG.NavalWar.NWData.Ai;

namespace TTG.NavalWar.NWData.Units
{
	[Serializable]
	public class DetectedUnit : GameObject
	{
		private List<DetectedUnitSensor> _DetectionSensors = new List<DetectedUnitSensor>();
		private bool _IsFixed = false;
		//private PositionOffset _PositionOffset = null; //used for uncertainty in detection position
		private double _ErrorBearingDeg = 0;
		private double _ErrorDistanceM = 0;
		private bool _IsIdentified;
        private List<MissileUnit> _targettingMissiles;

		#region "Constructors"

		public DetectedUnit() : base()
		{
			CanBeTargeted = true;
			TargettingList = new List<DetectedUnitTargetted>();
            _targettingMissiles = new List<MissileUnit>();
		}

		#endregion


		#region "Public properties"

		public GameConstants.DetectionClassification DetectionClassification { get; set; }

		public GameConstants.ThreatClassification ThreatClassification { get; set; }

        internal GameConstants.FriendOrFoe _FriendOrFoeClassification; 
		public GameConstants.FriendOrFoe FriendOrFoeClassification 
        {
            get
            {
                return _FriendOrFoeClassification;
            }
            set
            {
                if (_FriendOrFoeClassification != value)
                {
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    var detGroup = GetDetectedGroup();
                    if (detGroup != null)
                    {
                        detGroup.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                        foreach (var det in detGroup.DetectedUnits)
                        {
                            if (det.Id != this.Id) //do not recurse
                            {
                                det._FriendOrFoeClassification = value;
                                det.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                            }
                        }
                    }
                }
                _FriendOrFoeClassification = value;
            }
        }

		public override string Name
		{
			get
			{
				if (_IsIdentified && RefersToUnit != null)
				{
					base.Name = RefersToUnit.Name;
				}
				else
				{
					base.Name = DetectionClassification.ToString();
				}
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public string DetectedGroupId { get; set; }

		public List<DetectedUnitTargetted> TargettingList { get; set; }


		/// <summary>
		/// If true, all players in the game will always know position of this unit without requiring detection. 
		/// Used mainly for land installations like airports and seaports.
		/// </summary>
		public bool IsAlwaysVisibleForEnemy { get; set; }

		public BaseUnit RefersToUnit { get; set; }

		public List<DetectedUnitSensor> DetectionSensors
		{
			get
			{
				return _DetectionSensors;
			}
		}

		public bool CanBeTargeted { get; set; }

        public string Tag
        {
            get
            {
                if (RefersToUnit != null)
                {
                    return RefersToUnit.Tag;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

		public double DetectedGameWorldTimeSec { get; set; }

        /// <summary>
        /// Used by AI to determine which of multiple units to prioritize
        /// </summary>
	    public int ValueScore
	    {
            get { return ThreatValueScore + DistanceValueScore; }
	    }

        /// <summary>
        /// Used by AI to determine which of multiple units to prioritize
        /// </summary>
        public int ThreatValueScore { get; set; }

        /// <summary>
        /// Used by AI to determine which of multiple units to prioritize
        /// </summary>
        public int DistanceValueScore { get; set; }

		/// <summary>
		/// Used by AI to determine distance to nearest own valuable target, based on unit type and roles
		/// </summary>
		public double DistanceToValuableTargetM { get; set; }

		private Position _Position;
		public Position Position
		{
			get
			{
				if (this.IsFixed && this.RefersToUnit != null && RefersToUnit.Position != null)
				{
                    //_Position = RefersToUnit.Position.Clone();
				    return RefersToUnit.Position;
				}
				return _Position;
			}
            set
            {
                _Position = value;
            }
		}

		public Region PositionRegion { get; set; }

		public GameConstants.DirtyStatus DirtySetting { get; set; }

        public bool IsKnownToUseActiveRadar { get; set; }

        public bool IsKnownToUseActiveSonar { get; set; }

		public bool IsFixed
		{
			get
			{
				return _IsFixed;
			}
			set
			{
				if (_IsFixed != value)
				{
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
				}

				_IsFixed = value;
			}
		}

		public bool IsTargetted
		{
			get
			{
				ForgetOldTargetting();
				return !(TargettingList.Count == 0);
			}
		}

        public void AddTargettingMissile(MissileUnit missile)
        {
            if (missile != null && !_targettingMissiles.Contains(missile) && !missile.IsMarkedForDeletion)
            {
                _targettingMissiles.Add(missile);
            }
        }

        public int CountTargettingMissiles()
        {
            ForgetOldTargetting();
            return _targettingMissiles.Count;
        }

		public bool IsFiredUpon
		{
			get
			{
				try 
				{
					ForgetOldTargetting();
					var targettedWeapons = from t in TargettingList
										   where t.WeaponFired != null
										   select t;
					if (targettedWeapons.Count() > 0)
					{
						return true;
					}
					return false;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public bool IsIdentified
		{
			get
			{
				return _IsIdentified;
			}
			set
			{
				if (_IsIdentified != value)
				{
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    var detGroup = GetDetectedGroup();
                    if (detGroup != null)
                    {
                        detGroup.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                        if (value)
                        {
                            detGroup.Name = string.Empty;
                        }
                    }
				}
				_IsIdentified = value;
			}
		}

		public int KnownDamagePercent
		{
			get
			{
				if (IsIdentified && RefersToUnit != null)
				{
					int damage = RefersToUnit.DamagePercent();
					return damage;
				}
				else
				{
					return 0;
				}
			}
		}

		public Player OwnerPlayer { get; set; }

		public GameConstants.FireLevel KnownFireLevel
		{
			get
			{
				if (RefersToUnit != null)
				{
					if (RefersToUnit.FireLevel == GameConstants.FireLevel.MinorFire && !IsIdentified)
					{
						return GameConstants.FireLevel.NoFire; //Will not know about minor fires
					}
					return RefersToUnit.FireLevel;
				}
				else
				{
					return GameConstants.FireLevel.NoFire;
				}
			}
		}

		public bool HasLightingOn
		{
			get
			{
				if (RefersToUnit != null)
				{
					return RefersToUnit.HasLightingOn;
				}
				else
				{
					return false;
				}
			}
		}

		public GameConstants.DomainType DomainType
		{
			get
			{
				switch (DetectionClassification)
				{
					case GameConstants.DetectionClassification.Unknown:
						return GameConstants.DomainType.Unknown;

					//case GameConstants.DetectionClassification.Surface:
					case GameConstants.DetectionClassification.SmallSurface:
					case GameConstants.DetectionClassification.MediumSurface:
					case GameConstants.DetectionClassification.LargeSurface:
						return GameConstants.DomainType.Surface;

					//case GameConstants.DetectionClassification.Aircraft:
					case GameConstants.DetectionClassification.FixedWingAircraft:
					case GameConstants.DetectionClassification.FixedWingAircraftLarge:
					case GameConstants.DetectionClassification.Helicopter:
					case GameConstants.DetectionClassification.Missile:
					case GameConstants.DetectionClassification.BallisticMissile: 
						return GameConstants.DomainType.Air;

					case GameConstants.DetectionClassification.Subsurface:
					case GameConstants.DetectionClassification.Submarine:
					case GameConstants.DetectionClassification.Torpedo:
					case GameConstants.DetectionClassification.Mine:
					case GameConstants.DetectionClassification.Sonobuoy:
						return GameConstants.DomainType.Subsea;

					case GameConstants.DetectionClassification.LandInstallation:
						return GameConstants.DomainType.Land;
					default:
						return GameConstants.DomainType.Unknown;
				}
			}
		}
		#endregion



		#region "Public methods"

        public bool IsKnownToTargetUnit(BaseUnit unit, bool includeGroupMembers)
        {
            if (RefersToUnit == null)
            {
                return false;
            }
            if (RefersToUnit.TargetDetectedUnit != null &&
                RefersToUnit.TargetDetectedUnit.RefersToUnit != null) //kinda cheat
            {
                if (RefersToUnit.TargetDetectedUnit.RefersToUnit.Id == unit.Id)
                {
                    return true;
                }
                if (includeGroupMembers && !string.IsNullOrEmpty(RefersToUnit.TargetDetectedUnit.RefersToUnit.GroupId) && !string.IsNullOrEmpty(unit.GroupId) &&
                   RefersToUnit.TargetDetectedUnit.RefersToUnit.GroupId == unit.GroupId) //is attacking my group
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsKnownToBeCivilianUnit
        {
            get
            {
                if (IsIdentified && RefersToUnit != null)
                {
                    return RefersToUnit.IsCivilianUnit;
                }
                return false;
            }
        }

		public bool IsKnownToSupportRole(GameConstants.Role role)
		{ 
			//hmm should really only know unit class info, not actual unit
			if (this.IsIdentified && RefersToUnit != null)
			{
				return this.RefersToUnit.SupportsRole(role);
			}
			else if (RefersToUnit != null)
			{
				switch (this.DetectionClassification)
				{
					case GameConstants.DetectionClassification.Unknown:
						return false;
					//case GameConstants.DetectionClassification.Surface:
					case GameConstants.DetectionClassification.SmallSurface:
					case GameConstants.DetectionClassification.MediumSurface:
					case GameConstants.DetectionClassification.LargeSurface:
						return false; //nothing to know
                    //case GameConstants.DetectionClassification.Aircraft:
                    //    return (role == GameConstants.Role.IsAircraft);
					case GameConstants.DetectionClassification.FixedWingAircraft:
					case GameConstants.DetectionClassification.FixedWingAircraftLarge:
						return (role == GameConstants.Role.IsAircraft || role == GameConstants.Role.IsFixedWingAircraft);

					case GameConstants.DetectionClassification.Helicopter:
						return (role == GameConstants.Role.IsAircraft || role == GameConstants.Role.IsRotaryWingAircraft);

					case GameConstants.DetectionClassification.Missile:
						return (role == GameConstants.Role.IsAircraft);
					case GameConstants.DetectionClassification.Subsurface:
                        return false;
					case GameConstants.DetectionClassification.Submarine:
						return (role == GameConstants.Role.IsSubmarine 
                            || role == GameConstants.Role.AttackSubmarine 
                            || role == GameConstants.Role.AttackSurface); //for AI, assume all subs can attack surface and subs
					case GameConstants.DetectionClassification.Torpedo:
						return false;
					case GameConstants.DetectionClassification.Mine:
						return false;
					case GameConstants.DetectionClassification.Sonobuoy:
						return (role == GameConstants.Role.ASW);
					case GameConstants.DetectionClassification.BallisticMissile:
						return false;
					case GameConstants.DetectionClassification.LandInstallation:
						return false;
					default:
						return false;
				}
			}
			else //nothing to know
			{
				return false;
			}
		}

		public void ForgetOldTargetting(double olderThanSec)
		{ 
			//DateTime timeTooOld = GameManager.Instance.Game.GameCurrentTime.Subtract(TimeSpan.FromSeconds(olderThanSec));
			var worldTimeTooOld = GameManager.Instance.Game.GameWorldTimeSec - olderThanSec;

            _targettingMissiles.RemoveAll(m => m.IsMarkedForDeletion || m.TargetDetectedUnit == null || m.TargetDetectedUnit.Id != this.Id);
			
            //TargettingList.RemoveAll(p => p.UnitTargetting != null && p.UnitTargetting.IsMarkedForDeletion);
            //TargettingList.RemoveAll(p => p.UnitTargetting != null && p.UnitTargetting.TargetDetectedUnit != null && p.UnitTargetting.TargetDetectedUnit.Id != this.Id);
            //TargettingList.RemoveAll(p => p.TargettedGameWorldTimeSec < worldTimeTooOld);

            TargettingList.RemoveAll(p => p.UnitTargetting == null || p.UnitTargetting.IsMarkedForDeletion ||
                p.UnitTargetting.TargetDetectedUnit == null || p.UnitTargetting.TargetDetectedUnit.Id != this.Id ||
                p.TargettedGameWorldTimeSec < worldTimeTooOld);
		}

		public void ForgetOldTargetting()
		{
			ForgetOldTargetting(GameConstants.TIME_DETECION_TARGETTED_TOO_OLD_SEC);
		}

		public DetectedUnitSensor AddOrUpdateSensor(BaseUnit unit, BaseSensor detectionSensor, 
			double detectionStrength, double distanceM, double targetApparentSizeArcSec, double minDetectableSizeArcSec)
		{
			DetectedUnitSensor sensor = _DetectionSensors.Find(d => d.Id == detectionSensor.Id);
			if (sensor == null)
			{
				sensor = new DetectedUnitSensor();
				sensor.Id = detectionSensor.Id;
				sensor.DetectionSensor = detectionSensor;
				sensor.DetectionPlatform = detectionSensor.OwnerUnit;
				sensor.DetectedUnit = this;
				DetectionClassification = unit.UnitClass.DetectionClassification;
				SetDirty(GameConstants.DirtyStatus.UnitChanged);

				_DetectionSensors.Add(sensor);
			}
			if ((detectionSensor.SensorClass.SensorType == GameConstants.SensorType.Radar
				|| detectionSensor.SensorClass.SensorType == GameConstants.SensorType.Sonar)
				&& detectionSensor.IsActive && detectionSensor.SensorClass.IsPassiveActiveSensor)
			{
				unit.DetectionWithActiveSensor(detectionSensor, detectionStrength);
			}
			sensor.DetectedUnit = this;
			sensor.BearingDetectorToTargetDeg = MapHelper.CalculateBearingDegrees(
				detectionSensor.OwnerUnit.Position.Coordinate, unit.Position.Coordinate);
			sensor.DistanceDetectorToTargetM = distanceM;
			sensor.DetectionStrength = detectionStrength;
			sensor.ApparentSizeArcSec = targetApparentSizeArcSec;
			DetectedGameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
			sensor.DetectedWorldGameTimeSec = GameManager.Instance.Game.GameWorldTimeSec;

            // Check if more than one unit has contact with the detected unit, and if so set fixed
            if ( _DetectionSensors.Count > 1 )
            {
                var firstPlatformId =
                    _DetectionSensors.First(s => !string.IsNullOrEmpty(s.DetectionPlatform.Id)).DetectionPlatform.Id;

                if (_DetectionSensors.Any(s => s.DetectionPlatform.Id != firstPlatformId))
                {
                    IsFixed = true;
                }
            }

			sensor.IsBearingOnly = false;
			switch (detectionSensor.SensorClass.SensorType)
			{
				case GameConstants.SensorType.Radar:

					if (detectionSensor.SensorClass.IsPassiveActiveSensor && detectionSensor.IsActive) //Active radar
					{
						IsFixed = true;
						if (detectionStrength > detectionSensor.SensorClass.IdentifyDetectionStrength)
						{
							IsIdentified = true;
						}
					}
					else //passive radar, ie ESM
					{
						IsIdentified = true;
						sensor.IsBearingOnly = true;
                        IsKnownToUseActiveRadar = unit.IsUsingActiveRadar();
					}
					break;
				case GameConstants.SensorType.Sonar:
					if (detectionSensor.SensorClass.IsPassiveActiveSensor && !detectionSensor.IsActive 
						&& detectionStrength >= detectionSensor.SensorClass.IdentifyDetectionStrength)
					{
						IsIdentified = true;

					}
                    if ((detectionSensor.SensorClass.IsPassiveActiveSensor && !detectionSensor.IsActive) || (!detectionSensor.SensorClass.IsPassiveActiveSensor))
                    {
                        IsKnownToUseActiveSonar = unit.IsUsingActiveSonar();
                    }
					break;
				case GameConstants.SensorType.MAD:
					//IsFixed = false;
					//IsIdentified = false;
					break;
				case GameConstants.SensorType.Infrared:
				case GameConstants.SensorType.Visual:
					IsFixed = true;
					if (detectionStrength >= detectionSensor.SensorClass.IdentifyDetectionStrength)
					{
						IsIdentified = true;
					}
					break;
				default:
					break;
			}
			if (unit != null)
			{
				RefersToUnit = unit;
				if (!IsFixed)
				{
					sensor.UncertaintyRangeDeg = 10.0 / detectionStrength;
					if (sensor.UncertaintyRangeDeg < 0.5)
					{
						sensor.UncertaintyRangeDeg = 0;
					}
					sensor.UncertaintyRangeM = sensor.DistanceDetectorToTargetM / 10.0;
					if (detectionStrength > sensor.DetectionSensor.SensorClass.IdentifyDetectionStrength)
					{
						sensor.UncertaintyRangeM = sensor.UncertaintyRangeM / 2.0;
					}
					Coordinate newCoordinate = new Coordinate(unit.Position.Coordinate);
                    //TODO: UncertaintyRangeDeg betyr to forskjellige ting her... 
					if ((sensor.UncertaintyRangeM > 0 || sensor.UncertaintyRangeDeg > 0) 
						&& _ErrorDistanceM == 0 && _ErrorBearingDeg == 0)
					{
						double errorBearingDeg = 0;
						double errorDistanceM = 0;
						CalculatePositionFromUncertainty(
							distanceM, ref errorBearingDeg, ref errorDistanceM, ref newCoordinate);
						_ErrorBearingDeg = errorBearingDeg;
						_ErrorDistanceM = errorDistanceM;
					}
					var coord = MapHelper.CalculateNewPosition2(unit.Position.Coordinate, _ErrorBearingDeg, _ErrorDistanceM);
					Position = new Position(coord);
					Position.HeightOverSeaLevelM = unit.Position.HeightOverSeaLevelM;
					Position.BearingDeg = unit.Position.BearingDeg;
					sensor.DistanceDetectorToTargetM = MapHelper.CalculateDistanceRoughM(
						sensor.DetectionPlatform.Position.Coordinate, coord);
					sensor.BearingDetectorToTargetDeg = MapHelper.CalculateBearingDegrees(
						sensor.DetectionPlatform.Position.Coordinate, coord);
					PositionRegion = GetPositionRegion(sensor);
					
				}
				else
				{
					PositionRegion = null;
					Position = unit.Position.Clone();
				}
			}
			//sensor.UncertaintyRangeM = 0; 
			return sensor;
		}

		/// <summary>
		/// Based on uncertainty and Position, create a region wherein the detectedunit is found
		/// </summary>
		/// <returns></returns>
		public Region GetPositionRegion(DetectedUnitSensor sensor)
		{
			Region region = new Region();
			Coordinate coord = sensor.DetectionPlatform.Position.Coordinate;
			region.Coordinates.Add(coord);
			Coordinate coord1 = MapHelper.CalculateNewPosition2(
				coord, sensor.BearingDetectorToTargetDeg - (_ErrorBearingDeg * 0.5), 
				sensor.DistanceDetectorToTargetM);
			region.Coordinates.Add(coord1);
			Coordinate coord2 = MapHelper.CalculateNewPosition2(
				coord, sensor.BearingDetectorToTargetDeg, 
				sensor.DistanceDetectorToTargetM + _ErrorDistanceM);
			region.Coordinates.Add(coord2);
			Coordinate coord3 = MapHelper.CalculateNewPosition2(
				coord, sensor.BearingDetectorToTargetDeg + (_ErrorBearingDeg * 0.5), 
				sensor.DistanceDetectorToTargetM);
			region.Coordinates.Add(coord3);
			region.Coordinates.Add(coord);
			return region;
		}

		public void CalculatePositionFromUncertainty(double distanceM, 
			ref double bearingRealToReportedPosDeg, ref double distanceRealToReportedM, ref Coordinate newCoordinate)
		{
			var smallestUncertaintyRangeM = this.DetectionSensors.Min(s => s.UncertaintyRangeM);
			var smallestUncertaintyDeg = this.DetectionSensors.Min(s => s.UncertaintyRangeDeg);
			var firstSensor = this.DetectionSensors[0];
			bearingRealToReportedPosDeg = 0;
			distanceRealToReportedM = 0;
			if(firstSensor.DetectionPlatform == null 
				|| firstSensor.DetectionPlatform.Position == null 
				|| RefersToUnit == null 
				|| RefersToUnit.Position == null)
			{
				return;
			}
			if (smallestUncertaintyDeg < 0.1 && smallestUncertaintyRangeM < 100)
			{
				return;
			}
			var bearingFromTargetDeg = MapHelper.CalculateBearingDegrees(
				firstSensor.DetectionPlatform.Position.Coordinate, RefersToUnit.Position.Coordinate);
			double distanceSensorToReportedM = 0;
			double bearingSensorToReportedDeg = 0;
			double differenceRealToReportedBeaqringDeg = 0;
            double differenceRealToReportedM = 0;
            int maxIterations = 100;
            int noOfIterations = 0;
			do
			{

				bearingRealToReportedPosDeg = GameManager.Instance.GetRandomNumber(360);
				distanceRealToReportedM = GameManager.Instance.GetRandomNumber((int)smallestUncertaintyRangeM);
				newCoordinate = MapHelper.CalculateNewPosition2(
					RefersToUnit.Position.Coordinate, bearingRealToReportedPosDeg, distanceRealToReportedM);
				bearingSensorToReportedDeg = MapHelper.CalculateBearingDegrees(
					firstSensor.DetectionPlatform.Position.Coordinate, newCoordinate);
				distanceSensorToReportedM = MapHelper.CalculateDistanceApproxM(
					firstSensor.DetectionPlatform.Position.Coordinate, newCoordinate);
				differenceRealToReportedBeaqringDeg = Math.Abs(bearingSensorToReportedDeg - bearingRealToReportedPosDeg);
                differenceRealToReportedM = Math.Abs(distanceM - distanceSensorToReportedM);
                noOfIterations++;
			}
			while (noOfIterations < maxIterations 
                && (differenceRealToReportedBeaqringDeg > smallestUncertaintyDeg || differenceRealToReportedM > smallestUncertaintyRangeM));
			//if (GameManager.Instance.GetRandomNumber(1) < 1)
			//{
			//    errorBearingDeg = bearingFromTargetDeg + (errorBearingDeg / 2.0);
			//}
			//else
			//{
			//    errorBearingDeg = bearingFromTargetDeg - (errorBearingDeg / 2.0);
			//}

            //GameManager.Instance.Log.LogDebug(
            //    string.Format("CalculatePositionFromUncertainty: Real dist: {0:F}m  Error Deg:{1:F} Dist:{2:F}m",
            //    distanceM, differenceRealToReportedBeaqringDeg, distanceRealToReportedM));
		}

		public bool RemoveSensor(DetectedUnitSensor detectionSensor)
		{
			return _DetectionSensors.Remove(detectionSensor);
		}

		public virtual void SetDirty(GameConstants.DirtyStatus newDirtySetting)
		{
			if (newDirtySetting == GameConstants.DirtyStatus.Clean) //when set clean we mean it
			{
				DirtySetting = GameConstants.DirtyStatus.Clean;
			}
			else if ((int)newDirtySetting > (int)DirtySetting) //only change dirty setting if it increases dirtyness
			{
				//GameManager.Instance.Log.LogDebug("SetDirty: Unit" + ToShortString() + " set " + newDirtySetting);
				DirtySetting = newDirtySetting;
			}
		}

		public DetectedGroup GetDetectedGroup()
		{
			if (!string.IsNullOrEmpty(DetectedGroupId))
			{
				try
				{
					DetectedGroup det = this.OwnerPlayer.DetectedGroups.Find(d => !d.IsMarkedForDeletion && d.Id == this.DetectedGroupId);
					return det;
				}
				catch (Exception)
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		public PositionInfo GetPositionInfo()
		{ 
			PositionInfo pos = this.Position.GetPositionInfo();
			pos.UnitId = this.Id;
			if (RefersToUnit != null)
			{
                if (IsFixed)
                {
                    pos.ActualSpeedKph = (float)this.RefersToUnit.ActualSpeedKph;
                }

			    pos.IsAtFormationPosition = RefersToUnit.IsAtFormationPositionFlag || RefersToUnit.IsGroupMainUnit();
			}
			pos.IsDetection = true;
			return pos;
		}

		public DetectedUnitInfo GetDetectedUnitInfo()
		{
			DetectedUnitInfo info = new DetectedUnitInfo();
			DetectedUnitSensor sensor = null;
			//Debug.Assert(_DetectionSensors != null && _DetectionSensors.Count > 0, 
			//    "DetectionSensors should contain at least one entry.");
			if (this._DetectionSensors != null && this._DetectionSensors.Count > 0)
			{
				sensor = this._DetectionSensors[0]; //provide information from first sensor
				foreach (var DetSensor in _DetectionSensors)
				{
					info.DetectionSensors.Add(DetSensor.GetDetectedUnitSensorInfo());
				}
			}
			info.Id = Id;
			//info.DetectedUnitDescription = ToLongString();
			info.DetectionClassification = DetectionClassification;
			info.DomainType = DomainType;
			info.FriendOrFoeClassification = FriendOrFoeClassification;
			info.ThreatClassification = ThreatClassification;
			if (sensor != null)
			{
				info.IsBearingOnly = sensor.IsBearingOnly;
			}
			info.HasLightingOn = HasLightingOn;
			info.KnownDamagePercent = KnownDamagePercent;
			info.KnownFireLevel = KnownFireLevel;
			
			info.IsFixed = IsFixed;
			info.IsIdentified = IsIdentified;
            info.IsKnownToUseActiveRadar = IsKnownToUseActiveRadar;
            info.IsKnownToUseActiveSonar = IsKnownToUseActiveSonar;
            info.IsKnownToBeCivilianUnit = IsKnownToBeCivilianUnit;
		    info.Position = GetPositionInfo();
			info.DetectedGroupId = DetectedGroupId;
			info.IsFiredUpon = this.IsFiredUpon;
			info.IsTargetted = this.IsTargetted;
			if (this.PositionRegion != null)
			{
				info.PositionRegion = this.PositionRegion.GetRegionInfo();
			}
			if (RefersToUnit != null)
			{
				WeatherSystem weather = RefersToUnit.GetWeatherSystem();
				if(weather != null)
				{
					info.WeatherSystemInfo = weather.GetWeatherSystemInfo();
				}
			}
			if(IsIdentified && RefersToUnit != null)
			{
				info.RefersToUnitName = RefersToUnit.Name;
				info.RefersToUnitClassId = RefersToUnit.UnitClass.Id;
				if (OwnerPlayer != null)
				{
					info.OwnerPlayerId = OwnerPlayer.Id;
					info.OwnerPlayerName = OwnerPlayer.Name;
				}
			}
			//info.UncertaintyRangeM = UncertaintyRangeM;
			return info;
		}

		public void SetDetectedGroup()
		{
			if (RefersToUnit == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(RefersToUnit.GroupId))
			{
				return;
			}
			foreach (var det in OwnerPlayer.DetectedUnits)
			{
				if (!det.IsMarkedForDeletion && det.Id != this.Id && det.RefersToUnit != null)
				{
					if ((!string.IsNullOrEmpty(det.RefersToUnit.GroupId)) 
						&& det.RefersToUnit.GroupId == this.RefersToUnit.GroupId)
					{
						var detGroup = det.GetDetectedGroup();
						if (detGroup != null)
						{
							detGroup.AddUnit(this);
                            detGroup.IsMarkedForDeletion = false;
						}
						else
						{
							detGroup = new DetectedGroup();
							detGroup.AddUnit(det);
							detGroup.AddUnit(this);
							OwnerPlayer.DetectedGroups.Add(detGroup);
						}
					}
				}
			}
		}

		public override string ToString()
		{
			if(Position == null || Position.Coordinate == null || RefersToUnit == null)
			{
				return "Invalid contact " + Id;
			}
			string Info = string.Empty;

			if (IsIdentified && RefersToUnit != null)
			{
				Info += " " + RefersToUnit.Name + " (" + RefersToUnit.UnitClass.UnitClassShortName + ")";
			}
			else
			{
				Info += DetectionClassification.ToString() + " " + Id;
			}
			Info += "  [" + this.FriendOrFoeClassification.ToString() + "]";
			return Info;
		}

		public string ToLongString()
		{
			if (Position == null || Position.Coordinate == null)
			{
				return "Invalid contact " + Id;
			}
			string Info = ToString();
			if (KnownFireLevel != GameConstants.FireLevel.NoFire)
			{
				Info += "Fire: " + KnownFireLevel + "  ";
			}
			if (KnownDamagePercent > 0)
			{
				Info += string.Format("Damage: {0:F}%", KnownDamagePercent);
			}

			Info += "\nPosition: " + Position.Coordinate.ToString();
			
			if (IsFixed)
			{
				Info += " Fixed.";
			}
			if (IsIdentified)
			{
				Info += " Identified.";
			}
			Info += "Sensors: (" + this.DetectionSensors.Count +  ")\n";
			if (this.DetectionSensors.Count == 0)
			{
				Info += "Intelligence\n";
			}
			else
			{
				int maxCount = 3;
				int currentCount = 0;
				foreach (var sensor in this.DetectionSensors)
				{
					Info += sensor.ToString() + "\n";
					currentCount++;
					if (currentCount >= maxCount)
					{
						break;
					}
				}
			}
			return Info;
		}
		#endregion





	}
}
