using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Util;
using System.Diagnostics;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.Ai;

namespace TTG.NavalWar.NWData.Units
{
	[Serializable]
	public class BaseWeapon : BaseComponent
	{
		public BaseWeapon() : base()
		{

		}

		#region "Public properties"

		public virtual WeaponClass WeaponClass { get; set; }

		public int MaxAmmunition { get; set; }

		public virtual int AmmunitionRemaining { get; set; }

		public virtual double LastFiredGameWorldTimeSec { get; set; }


        /// <summary>
        /// Weapon bearing degree relative to the unit, which has its front at 0 deg
        /// </summary>
		public virtual double WeaponBearingDeg { get; set; }

		/// <summary>
		/// Pitch in degrees, giving the initial pitch and direction of the fired projectile.
		/// 0 is the forward direction of the baseunit.
		/// </summary>
		//public virtual double WeaponPitchDeg { get; set; }

		/// <summary>
		/// Sets or gets whether this is considered a primary weapon. A primary weapon is one 
		/// which the unit or group will actively close range to engage an enemy with. Point
		/// defence weapons should not be set as primary weapons.
		/// </summary>
		public virtual bool IsPrimaryWeapon { get; set; }

		//public virtual GameConstants.WeaponOrders WeaponOrders { get; set; }



		#endregion

		#region "Public methods"

		/// <summary>
		/// Determines whether it is possible for weapon to target a specific unit, regardless
		/// of range. Optionally takes out of ammo considerations.
		/// </summary>
		/// <param name="unit"></param>
		/// <param name="ignoreNoAmmo"></param>
		/// <returns></returns>
		public bool CanTargetDetectedUnit(DetectedUnit detectedUnit, bool ignoreNoAmmo)
		{
			Debug.Assert(WeaponClass != null, "BaseWeapon.WeaponClass should never be null.");
			if (WeaponClass.IsNotWeapon)
			{
				return false;
			}
            if (!detectedUnit.CanBeTargeted)
            {
                return false;
            }
			if (this.AmmunitionRemaining < 1 && !ignoreNoAmmo)
			{
				return false;
			}
			switch (detectedUnit.DomainType)
			{
				case GameConstants.DomainType.Surface:
					return WeaponClass.CanTargetSurface;
				case GameConstants.DomainType.Air:
					return WeaponClass.CanTargetAir;
				case GameConstants.DomainType.Subsea:
                    if (detectedUnit.Position != null && detectedUnit.Position.HasHeightOverSeaLevel && detectedUnit.Position.HeightOverSeaLevelM >= -1)
                    {
                        return (WeaponClass.CanTargetSurface || WeaponClass.CanTargetSubmarine);
                    }
					return WeaponClass.CanTargetSubmarine;
				case GameConstants.DomainType.Land:
					return WeaponClass.CanTargetLand;
				case GameConstants.DomainType.Unknown:
					return false;
				default:
					return false;
			}
		}

		/// <summary>
		/// Returns true if this weapon can target units in the specified domain (air, sea, sub, land),
		/// otherwise false. Ignores current readiness and ammo status of the weapon.
		/// </summary>
		/// <param name="domainType"></param>
		/// <returns></returns>
		public bool CanTargetDomain(GameConstants.DomainType domainType)
		{
			switch (domainType)
			{
				case GameConstants.DomainType.Surface:
					return WeaponClass.CanTargetSurface;
				case GameConstants.DomainType.Air:
					return WeaponClass.CanTargetAir;
				case GameConstants.DomainType.Subsea:
					return WeaponClass.CanTargetSubmarine;
				case GameConstants.DomainType.Land:
					return WeaponClass.CanTargetLand;
				case GameConstants.DomainType.Unknown:
					return false;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determines whether it is possible for weapon to target a specific unit, regardless
		/// of range. Optionally takes out ammo considerations.
		/// </summary>
		/// <param name="unit"></param>
		/// <param name="ignoreNoAmmo"></param>
		/// <returns></returns>
		public bool CanTargetDetectedUnit(BaseUnit unit, bool ignoreNoAmmo)
		{
			Debug.Assert(WeaponClass != null, "BaseWeapon.WeaponClass should never be null.");
			if (WeaponClass.IsNotWeapon || !unit.UnitClass.CanBeTargeted)
			{
				return false;
			}
			if (this.AmmunitionRemaining < 1 && !ignoreNoAmmo)
			{
				return false;
			}
			if (unit == null)
			{
				return false;
			}
			switch (unit.DomainType)
			{
				case GameConstants.DomainType.Surface:
					return WeaponClass.CanTargetSurface;
				case GameConstants.DomainType.Air:
					return WeaponClass.CanTargetAir;
				case GameConstants.DomainType.Subsea:
                    if (unit.Position != null && unit.Position.HasHeightOverSeaLevel && unit.Position.HeightOverSeaLevelM >= -1)
                    {
                        return (WeaponClass.CanTargetSurface || WeaponClass.CanTargetSubmarine);
                    }
					return WeaponClass.CanTargetSubmarine;
				case GameConstants.DomainType.Land:
					return WeaponClass.CanTargetLand;
				case GameConstants.DomainType.Unknown:
					return false;
				default:
					return false;
			}
 
		}

		/// <summary>
		/// Fires on designated DetectedUnit (target), launching the designated number of rounds
		/// if available. 
		/// </summary>
		/// <param name="targetUnit"></param>
		/// <param name="noOfRounds"></param>
		/// <returns>Returns number of projectiles/rounds actually fired.</returns>
		public virtual int Fire(DetectedUnit targetUnit, int noOfRounds, double distanceToTargetM)
		{
			Debug.Assert(WeaponClass != null, "Weaponclass should never be null.");
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			Debug.Assert(OwnerUnit != null, "OwnerUnit should never be null.");
			Debug.Assert(targetUnit != null, "targetUnit should never be null.");

			if (!CheckIfWeaponCanFireNow(targetUnit, targetUnit.Position, distanceToTargetM))
			{
				return 0;
			}
            if (targetUnit.FriendOrFoeClassification != GameConstants.FriendOrFoe.Foe || (OwnerPlayer.IsComputerPlayer && targetUnit.IsKnownToBeCivilianUnit))
            {
                return 0;
            }
            double bearingToTargetDeg = MapHelper.CalculateBearingDegrees(OwnerUnit.Position.Coordinate,
			    	targetUnit.Position.Coordinate);
			if (WeaponClass.SpawnsUnitOnFire)
			{
				if (noOfRounds > AmmunitionRemaining)
				{
					noOfRounds = AmmunitionRemaining;
				}
				if (noOfRounds > WeaponClass.MaxSimultanousShots)
				{
					noOfRounds = WeaponClass.MaxSimultanousShots;
				}
				GameManager.Instance.Log.LogDebug(
					string.Format("Fire: Weapon {0} to engage target {1} with {2} rounds.",
					this.WeaponClass.WeaponClassName, targetUnit.ToString(), noOfRounds));
				if (noOfRounds < 1)
				{
					return 0;
				}
                if (targetUnit.RefersToUnit != null)
                {
                    targetUnit.RefersToUnit.OwnerPlayer.HasPlayerBeenFiredUpon = true;
                }
				AmmunitionRemaining -= noOfRounds;
				ReadyInSec = WeaponClass.TimeBetweenShotsSec;
				
				OwnerUnit.MakeExtraordinaryNoise(200, 11);
				targetUnit.TargettingList.Add(new DetectedUnitTargetted(this, OwnerUnit));
				OwnerUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);

				Group group = null;
				if (noOfRounds > 1)
				{
					group = new Group();
				}
				for (int count = 1; count <= noOfRounds; count++)
				{
					Position pos = OwnerUnit.Position.Clone(); //TODO: Position of weapon, + some variation to not stack missiles
					if (OwnerUnit.Position.HeightOverSeaLevelM == 0)
					{
						pos.HeightOverSeaLevelM += OwnerUnit.UnitClass.HeightM;
					}
                    if (WeaponClass.WeaponTrajectory == GameConstants.WeaponTrajectoryType.AsRoc)
                    {
                        pos = targetUnit.Position.Clone().Offset(GameManager.Instance.GetRandomNumber(360), GameManager.Instance.GetRandomNumber(2000) + 1000);
                        pos.HeightOverSeaLevelM = 0;
                    }
                    MissileUnit missileUnit = SpawnNewMissileUnit(pos);
					missileUnit.MissionType = GameConstants.MissionType.Attack;
					missileUnit.MissionTargetType = OwnerUnit.GetMissionTargetTypeFromDetectedUnit(targetUnit);
					missileUnit.TargetDetectedUnit = targetUnit;
                    targetUnit.AddTargettingMissile(missileUnit);
    				missileUnit.MaxRangeCruiseM = (this.WeaponClass.MaxWeaponRangeM * 1.1) + 1000.0; //hmm
                    if (WeaponClass.WeaponTrajectory == GameConstants.WeaponTrajectoryType.AsRoc)
                    {
                        missileUnit.MaxRangeCruiseM = WeaponClass.EffectRangeM; //For AsRoc only, since MaxCruiseRangeM is range of ballistic shot, this is used for range of torpedo
                    }
					missileUnit.HitPercent = (int)GetHitPercent(targetUnit, distanceToTargetM);
					if (WeaponClass.MaxSeaState <= missileUnit.GetEffectiveSeaState())
					{
						missileUnit.HitPercent /= 2;
					}
                    if (targetUnit.DomainType == GameConstants.DomainType.Land)
                    {
                        missileUnit.HitPercent += 20;
                    }
                    missileUnit.HitPercent = missileUnit.HitPercent.Clamp(1, 100);
					if (targetUnit.RefersToUnit != null)
					{
						missileUnit.TargetUnitClassId = targetUnit.RefersToUnit.UnitClass.Id;
					}
					if (group != null)
					{
						group.AddUnit(missileUnit);
					}

					foreach (var sensor in missileUnit.Sensors)
					{
						sensor.IsOperational = true;
						if (sensor.SensorClass.IsPassiveActiveSensor)
						{
                            if (targetUnit.DomainType != GameConstants.DomainType.Land)
                            {
                                sensor.IsActive = true;    
                            }
						}
					}

					missileUnit.MovementOrder = CreateMissileMovementOrder(bearingToTargetDeg, distanceToTargetM, null, targetUnit);

                    GameManager.Instance.Log.LogDebug(
						string.Format("Fire: Weapon {0} launches MissileUnit {1}. First wp: {2}",
						this.WeaponClass.WeaponClassName, missileUnit.ToShortString(), missileUnit.GetActiveWaypoint()));
                    switch (this.WeaponClass.WeaponTrajectory)
                    {
                        case GameConstants.WeaponTrajectoryType.DirectShot:
                            break;
                        case GameConstants.WeaponTrajectoryType.AntiAirTracking:
                            missileUnit.UserDefinedElevation = null;
                            //if (targetUnit != null && targetUnit.Position != null && targetUnit.Position.HasHeightOverSeaLevel) //hack
                            //{
                            //    missileUnit.DesiredHeightOverSeaLevelM = targetUnit.Position.HeightOverSeaLevelM;    
                            //}
                            break;
                        case GameConstants.WeaponTrajectoryType.CruiseSeaSkimming:
                            missileUnit.UserDefinedElevation = GameConstants.HeightDepthPoints.VeryLow;
                            break;
                        case GameConstants.WeaponTrajectoryType.CruiseHighAltitude:
                            break;
                        case GameConstants.WeaponTrajectoryType.GravityBomb:
                            break;
                        case GameConstants.WeaponTrajectoryType.BallisticMissile:
                            break;
                        case GameConstants.WeaponTrajectoryType.TorpedoHeavyweight:
                            break;
                        case GameConstants.WeaponTrajectoryType.TorpedoLight:
                            break;
                        case GameConstants.WeaponTrajectoryType.AsRoc:
                            break;
                        default:
                            break;
                    }
                    missileUnit.ReCalculateEta();
				}
				if (group != null)
				{
					group.RemoveIfSingleUnit();
				}
                OwnerPlayer.Send(
                new GameStateInfo(GameConstants.GameStateInfoType.MissileLaunch, OwnerUnit.Id)
                {
                    WeaponClassId = this.WeaponClass.Id,
                    WeaponId = this.Id,
                    BearingDeg = bearingToTargetDeg,
                    Count = noOfRounds,
                } );
                foreach (var pl in GameManager.Instance.Game.Players)
                {
                    if (pl.Id != this.OwnerPlayer.Id && pl.IsCompetitivePlayer && !pl.IsComputerPlayer)
                    {
                        var detUnit = pl.GetDetectedUnitByUnitId(OwnerUnit.Id);
                        if (detUnit != null && detUnit.IsIdentified)
                        {
                            pl.Send(
                            new GameStateInfo(GameConstants.GameStateInfoType.EnemyMissileLaunch, detUnit.Id)
                            {
                                WeaponClassId = this.WeaponClass.Id,
                                BearingDeg = bearingToTargetDeg,
                                Count = noOfRounds,
                            } );
                                
                        }
                    }
                }
				return noOfRounds;
			}
			else //instantaneous fire, does not spawn missile/torpedo unit
			{
				if (noOfRounds > AmmunitionRemaining)
				{
					noOfRounds = AmmunitionRemaining;
				}
				if (noOfRounds > WeaponClass.MaxSimultanousShots)
				{
					noOfRounds = WeaponClass.MaxSimultanousShots;
				}
				GameManager.Instance.Log.LogDebug(
					string.Format("Fire: Weapon {0} to engage target {1} with {2} rounds.",
					this.WeaponClass.WeaponClassName, targetUnit.ToString(), noOfRounds));
				if (noOfRounds < 1)
				{
					return 0;
				}
				OwnerUnit.MakeExtraordinaryNoise(200, 11);
				targetUnit.TargettingList.Add(new DetectedUnitTargetted(this, OwnerUnit));
				OwnerUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);

				AmmunitionRemaining -= noOfRounds;
				ReadyInSec = WeaponClass.TimeBetweenShotsSec;
				LastFiredGameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec;

				if (targetUnit.RefersToUnit != null)
				{
					BaseUnit unit = targetUnit.RefersToUnit;
                    if (noOfRounds > 0)
                    {
                        var gsinfo = new GameStateInfo(GameConstants.GameStateInfoType.GunFired, OwnerUnit.Id);
                        gsinfo.WeaponClassId = this.WeaponClass.Id;
                        gsinfo.BearingDeg = bearingToTargetDeg;
                        gsinfo.SecondaryId = targetUnit.Id;
                        gsinfo.Count = noOfRounds;
                        OwnerPlayer.Send(gsinfo);
                        foreach (var pl in GameManager.Instance.Game.Players)
                        {
                            if (pl.Id != this.OwnerPlayer.Id && pl.IsCompetitivePlayer && !pl.IsComputerPlayer)
                            {
                                var detUnit = pl.GetDetectedUnitByUnitId(OwnerUnit.Id);
                                if (detUnit != null && detUnit.IsIdentified)
                                {
                                    pl.Send(
                                    new GameStateInfo(GameConstants.GameStateInfoType.EnemyGunFired, detUnit.Id)
                                    {
                                        WeaponClassId = this.WeaponClass.Id,
                                        BearingDeg = bearingToTargetDeg,
                                        Count = noOfRounds,
                                    });
                                }
                            }
                        }
                    }
					for (int count = 1; count <= noOfRounds; count++)
					{
						unit.InflictDamageFromProjectileHit(this, targetUnit);
					}
					return noOfRounds;
				}
				else //must be firing at decoy/ghost
				{
					return noOfRounds;
				}
			}

		}

		public bool CheckIfWeaponCanFireNow(DetectedUnit targetUnit, Position position, double distanceToTargetM)
		{
			if (ReadyInSec > 0)
			{
				return false;
			}
			if (WeaponClass.IsNotWeapon)
			{
				return false;
			}
			if (distanceToTargetM > WeaponClass.MaxWeaponRangeM)
			{
                if (targetUnit != null)
                {
                    GameManager.Instance.Log.LogDebug(
                        string.Format("Fire: Weapon {0} cannot engage {1}. Out of range at {2}m.",
                                      this.WeaponClass.WeaponClassName, targetUnit.ToString(), distanceToTargetM));
                }
			    return false;
			}
		    if (targetUnit != null)
			{
				position = targetUnit.Position;
			}
            if (position == null)
            {
                return false;
            }
			if (distanceToTargetM < WeaponClass.MinWeaponRangeM)
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.WeaponCantFireTooClose, OwnerUnit.Id);
                errMsg.WeaponClassId = this.WeaponClass.Id;
                if (targetUnit != null)
                    errMsg.SecondaryId = targetUnit.Id;
                OwnerPlayer.Send(errMsg);
				return false;
			}
			if (WeaponClass.MaxSeaState < OwnerUnit.GetEffectiveSeaState())
			{
                var errMsg = new GameStateInfo(GameConstants.GameStateInfoType.WeaponCantFireRoughWeather, OwnerUnit.Id);
                errMsg.WeaponClassId = this.WeaponClass.Id;
                if (targetUnit != null)
                    errMsg.SecondaryId = targetUnit.Id;
                OwnerPlayer.Send(errMsg);
				return false;
			}
			if (!IsCoordinateInSectorRange(position.Coordinate))
			{
				GameManager.Instance.Log.LogDebug(
					string.Format("Fire: Weapon {0} cannot engage target. Target is out of sector range.",
					this.WeaponClass.WeaponClassName));
				return false;
			}
			return true;
		}

		private MissileUnit SpawnNewMissileUnit(Position platformPosition)
		{
			UnitClass unitClass = GameManager.Instance.GameData.GetOrGenerateUnitClassFromWeapon(this.WeaponClass);
			MissileUnit missileUnit = (MissileUnit)GameManager.Instance.GameData.CreateUnit(OwnerPlayer, null,
				WeaponClass.Id, this.WeaponClass.WeaponClassName,
				platformPosition, true, true);
            // Set starting bearing on missile to the same as the weapon bearing
            missileUnit.ActualBearingDeg = GetCurrentWeaponBearingDeg();
			Debug.Assert(missileUnit != null, " BaseWeapon.Fire unable to generate Unit from WeaponClass " + WeaponClass.Id);
			if (WeaponClass.RequiresPlatformFireControl)
			{
				if (missileUnit.UnitClass.UnitType == GameConstants.UnitType.Missile)
				{
					OwnerUnit.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
				}
				else if (missileUnit.UnitClass.UnitType == GameConstants.UnitType.Torpedo)
				{
					OwnerUnit.SetSensorsActivePassive(GameConstants.SensorType.Sonar, true);
				}
			}
			missileUnit.SetActualSpeed(missileUnit.UnitClass.MaxSpeedKph);
            if (this.WeaponClass.TerminalSpeedRangeM > 0)
            {
                missileUnit.SetActualSpeed(GameConstants.DEFAULT_NON_TERMINAL_SPEED_KPH); //default speed?
                missileUnit.UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
            }
            else
            {
                missileUnit.UserDefinedSpeed = GameConstants.UnitSpeedType.Afterburner;
            }
			missileUnit.DamageHitpoints = WeaponClass.DamageHitPoints;
			missileUnit.LaunchPlatform = OwnerUnit;
			missileUnit.LaunchWeapon = this;
			missileUnit.WeaponClassId = WeaponClass.Id;
            //OwnerPlayer.Send(
            //new GameStateInfo(GameConstants.GameStateInfoType.MissileLaunch, missileUnit.Id, OwnerUnit.Id)
            //{
            //    UnitClassId = missileUnit.UnitClass.Id,
            //    WeaponClassId = this.WeaponClass.Id,
            //    WeaponId = this.Id,
            //    BearingDeg = bearingToTargetDeg,
            //});
			return missileUnit;
		}

		public virtual int Fire(Position position, int noOfRounds, bool isBearingOnlyAttack)
		{
			Debug.Assert(WeaponClass != null, "Weaponclass should never be null.");
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			Debug.Assert(OwnerUnit != null, "OwnerUnit should never be null.");
			if (noOfRounds < 1)
			{
				noOfRounds = 1;
			}
            if (noOfRounds > AmmunitionRemaining)
            {
                noOfRounds = AmmunitionRemaining;
            }
            if (noOfRounds < 1)
            {
                return 0;
            }
			if (isBearingOnlyAttack)
			{
                var rangeToShootM = MapHelper.CalculateDistanceRoughM(OwnerUnit.Position.Coordinate, position.Coordinate);
				rangeToShootM = rangeToShootM.Clamp(100, WeaponClass.MaxWeaponRangeM);
                if (!CheckIfWeaponCanFireNow(null, position, rangeToShootM))
                {
                    return 0;
                }

                var bearingToTargetDeg = MapHelper.CalculateBearingDegrees(OwnerUnit.Position.Coordinate, position.Coordinate);

				Group missileGroup = null;
				if (noOfRounds > 1)
				{
					missileGroup = new Group();
				}
				OwnerUnit.MakeExtraordinaryNoise(200, 11);
                OwnerUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
				for (int i = 0; i < noOfRounds; i++)
				{
                    Position pos = OwnerUnit.Position.Clone(); //TODO: Position of weapon, + some variation to not stack missiles
                    if (OwnerUnit.Position.HeightOverSeaLevelM == 0)
                    {
                        pos.HeightOverSeaLevelM += OwnerUnit.UnitClass.HeightM;
                    }

                    var missile = SpawnNewMissileUnit(pos);
                    missile.MissionType = GameConstants.MissionType.Attack;
                    missile.MaxRangeCruiseM = (this.WeaponClass.MaxWeaponRangeM * 1.1) + 1000.0; //hmm
                    missile.HitPercent = this.WeaponClass.HitPercent;
				    missile.MaxSectorRangeSearchDeg = 90;   // when firing bearing only, only allow a 90 degree search sector
                    if (WeaponClass.MaxSeaState <= missile.GetEffectiveSeaState())
                    {
                        missile.HitPercent /= 2;
                    }
                    missile.HitPercent = missile.HitPercent.Clamp(1, 100);

                    this.AmmunitionRemaining--;
					var targetCoord = MapHelper.CalculateNewPosition2(OwnerUnit.Position.Coordinate, bearingToTargetDeg, rangeToShootM);
				    var targetPos = new Position(targetCoord);
                    missile.TargetPosition = targetPos;
					missile.MovementOrder = CreateMissileMovementOrder(bearingToTargetDeg, rangeToShootM, targetPos, null);
					//var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
					//var wp = new Waypoint(targetPos);
					//wp.Orders.Add(searchOrder);
					//missile.MovementOrder = new MovementOrder(wp);
					if (missileGroup != null)
					{
						missileGroup.AddUnit(missile);
					}
					missile.SetActualSpeed(missile.UnitClass.MaxSpeedKph);
                    missile.ReCalculateEta();
				}
                OwnerPlayer.Send(
                new GameStateInfo(GameConstants.GameStateInfoType.MissileLaunch, OwnerUnit.Id)
                {
                    WeaponClassId = this.WeaponClass.Id,
                    WeaponId = this.Id,
                    BearingDeg = bearingToTargetDeg,
                    Count = noOfRounds,
                } );
			}
			else //fire on position
			{
				throw new NotImplementedException("Fire on Position only supports bearing only attack!");
                //if (WeaponClass.SpawnsUnitOnFire)
                //{

                //}
                //else
                //{

                //}
                //OwnerUnit.MakeExtraordinaryNoise(200, 2);

			}
 
			return noOfRounds;
		}

		private MovementOrder CreateMissileMovementOrder(double bearingDeg, double distanceToTargetM, Position targetPos, DetectedUnit targetDetectedUnit)
		{
            try
            {
                if (targetDetectedUnit != null)
                {
                    targetPos = targetDetectedUnit.Position.Clone();
                }
                if (targetPos == null)
                {
                    GameManager.Instance.Log.LogError(
                        string.Format("CreateMissileMovementOrder weapon {0} call with null target.", this.ToString()));
                    return null;
                }
                var movementOrder = new MovementOrder();
                Waypoint wp;
                Position firstPos;
                Waypoint wpTarget;
                Position secondPos;
                var trajRequiresTargetUnit = new List<GameConstants.WeaponTrajectoryType>();
                trajRequiresTargetUnit.Add(GameConstants.WeaponTrajectoryType.AntiAirTracking);
                trajRequiresTargetUnit.Add(GameConstants.WeaponTrajectoryType.DirectShot);
                trajRequiresTargetUnit.Add(GameConstants.WeaponTrajectoryType.GravityBomb);
                if ((targetDetectedUnit == null || targetDetectedUnit.IsMarkedForDeletion || targetDetectedUnit.Position == null)
                    && trajRequiresTargetUnit.Contains(this.WeaponClass.WeaponTrajectory))
                {
                    GameManager.Instance.Log.LogError(
                        string.Format("CreateMissileMovementOrder: Weapon {0} with TrajectorType {1} cannot be called with TargetDetectedUnit null or invalid.",
                        ToString(), this.WeaponClass.WeaponTrajectory));
                    return null;

                }
                if (targetDetectedUnit != null)
                {
                    wpTarget = new Waypoint(targetDetectedUnit);
                }
                switch (this.WeaponClass.WeaponTrajectory)
                {
                    case GameConstants.WeaponTrajectoryType.DirectShot:
                        wp = new Waypoint(targetDetectedUnit);
                        movementOrder.AddWaypoint(wp);
                        break;
                    case GameConstants.WeaponTrajectoryType.AntiAirTracking:
                        if (distanceToTargetM > 5000)
                        {
                            firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 100);
                            if (OwnerUnit.DomainType != GameConstants.DomainType.Air)
                            {
                                firstPos.HeightOverSeaLevelM = GameManager.Instance.GetRandomNumber(50) + 50;
                            }
                            wp = new Waypoint(firstPos);
                            movementOrder.AddWaypoint(wp);
                        }
                        wpTarget = new Waypoint(targetDetectedUnit);
                        movementOrder.AddWaypoint(wpTarget);
                        break;
                    case GameConstants.WeaponTrajectoryType.CruiseSeaSkimming:
                        if (OwnerUnit.DomainType != GameConstants.DomainType.Air) //surface and subs
                        {
                            firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 250 + GameManager.Instance.GetRandomNumber(300));
                            firstPos.HeightOverSeaLevelM = OwnerUnit.Position.HeightOverSeaLevelM + OwnerUnit.UnitClass.HeightM + GameManager.Instance.GetRandomNumber(50) + 50;
                            firstPos.HeightOverSeaLevelM = ((double)firstPos.HeightOverSeaLevelM).Clamp(10.0, (double)OwnerUnit.Position.HeightOverSeaLevelM + 100.0);
                        }
                        else //for firing from flying units
                        {
                            firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 250 + GameManager.Instance.GetRandomNumber(300));
                        }

                        wp = new Waypoint(firstPos);

                        // Turn on sensors when firing without a target
                        if (!WeaponClass.RequiresPlatformFireControl && targetDetectedUnit == null)
                        {
                            var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            wp.Orders.Add(searchOrder);
                        }

                        movementOrder.AddWaypoint(wp);
                        var distanceSecondPos = distanceToTargetM / 10.0;
                        if (distanceSecondPos < 1000)
                        {
                            distanceSecondPos = 1000;
                        }
                        secondPos = OwnerUnit.Position.Offset(bearingDeg, distanceSecondPos);
                        if (targetDetectedUnit != null)
                        {
                            secondPos.HeightOverSeaLevelM = targetDetectedUnit.Position.HeightOverSeaLevelM + 2;
                        }
                        else
                        {
                            secondPos.HeightOverSeaLevelM = GameConstants.HEIGHT_VERY_LOW_MIN_M;
                        }

                        Waypoint wp2 = new Waypoint(secondPos);
                        movementOrder.AddWaypoint(wp2);

                        if (targetDetectedUnit != null)
                        {
                            wpTarget = new Waypoint(targetDetectedUnit);
                        }
                        else
                        {
                            wpTarget = new Waypoint(targetPos);
                            //BaseOrder atkOrder = new BaseOrder(this.OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            //wpTarget.Orders.Add(atkOrder);
                        }
                        movementOrder.AddWaypoint(wpTarget);
#if LOG_DEBUG
                        var wpsstring = string.Empty;
                        foreach (var awp in movementOrder.GetWaypoints())
                        {
                            wpsstring += awp.ToString() + " - ";
                        }
                        GameManager.Instance.Log.LogDebug(string.Format("CreateMissileMovementOrder for Wpn {1}: MovementOrder {1} --Waypoints: {2}", this, movementOrder, wpsstring));
#endif
                        break;

                    case GameConstants.WeaponTrajectoryType.CruiseHighAltitude:
                        firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 500);
                        if (OwnerUnit.DomainType != GameConstants.DomainType.Air)
                        {
                            firstPos.HeightOverSeaLevelM = GameManager.Instance.GetRandomNumber(50) + 50;
                        }
                        wp = new Waypoint(firstPos);
                        movementOrder.AddWaypoint(wp);

                        secondPos = OwnerUnit.Position.Offset(bearingDeg, 1000);
                        secondPos.HeightOverSeaLevelM = GameConstants.HEIGHT_VERY_HIGH_MIN_M;
                        var wp3 = new Waypoint(secondPos);
                        wp3.Orders.Add(new BaseOrder() { OrderType = GameConstants.OrderType.SetElevation, HeightDepthPoints = GameConstants.HeightDepthPoints.VeryHigh });
                        movementOrder.AddWaypoint(wp3);

                        Position lastPos = OwnerUnit.Position.Offset(bearingDeg, distanceToTargetM * .6);
                        lastPos.HeightOverSeaLevelM = GameConstants.HEIGHT_VERY_HIGH_MIN_M;
                        //lastPos.HeightOverSeaLevelM = targetDetectedUnit.Position.HeightOverSeaLevelM + 2;
                        Waypoint wp4 = new Waypoint(lastPos);
                        wp4.Orders.Add(new BaseOrder() { OrderType = GameConstants.OrderType.SetElevation, HeightDepthPoints = GameConstants.HeightDepthPoints.Low });

                        // Turn on sensors when firing without a target
                        if (!WeaponClass.RequiresPlatformFireControl && targetDetectedUnit == null)
                        {
                            var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            wp4.Orders.Add(searchOrder);
                        }
                        // Otherwise make sure target is still present
                        else if (targetDetectedUnit != null && targetDetectedUnit.RefersToUnit != null)
                        {
                            var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            searchOrder.StringParameter = targetDetectedUnit.RefersToUnit.UnitClass.Id;
                            searchOrder.SecondId = targetDetectedUnit.Id;
                            wp4.Orders.Add(searchOrder);
                        }

                        movementOrder.AddWaypoint(wp4);

                        wpTarget = new Waypoint(targetDetectedUnit);
                        movementOrder.AddWaypoint(wpTarget);
                        break;

                    case GameConstants.WeaponTrajectoryType.GravityBomb:
                        firstPos = OwnerUnit.Position.Clone();
                        wp = new Waypoint(firstPos);
                        movementOrder.AddWaypoint(wp);
                        wpTarget = new Waypoint(targetDetectedUnit);
                        movementOrder.AddWaypoint(wpTarget);
                        break;

                    case GameConstants.WeaponTrajectoryType.BallisticMissile:
                        movementOrder = GetBallisticProjectileMovement(targetDetectedUnit, bearingDeg, distanceToTargetM);
                        break;

                    case GameConstants.WeaponTrajectoryType.TorpedoHeavyweight:
                        firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 300 + GameManager.Instance.GetRandomNumber(200));
                        wp = new Waypoint(firstPos);
                        if (!WeaponClass.RequiresPlatformFireControl)
                        {
                            var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            if (targetDetectedUnit != null && targetDetectedUnit.RefersToUnit != null)
                            {
                                searchOrder.StringParameter = targetDetectedUnit.RefersToUnit.UnitClass.Id;
                                searchOrder.SecondId = targetDetectedUnit.Id;
                            }
                            wp.Orders.Add(searchOrder);
                        }
                        movementOrder.AddWaypoint(wp);
                        if (targetDetectedUnit != null)
                        {
                            wpTarget = new Waypoint(targetDetectedUnit);
                        }
                        else
                        {
                            wpTarget = new Waypoint(targetPos);
                            wpTarget.Position.HeightOverSeaLevelM = -10;
                            //wpTarget.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget));
                        }
                        movementOrder.AddWaypoint(wpTarget);
                        break;

                    case GameConstants.WeaponTrajectoryType.TorpedoLight:
                        firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 50 + GameManager.Instance.GetRandomNumber(50));
                        wp = new Waypoint(firstPos);
                        if (!WeaponClass.RequiresPlatformFireControl)
                        {
                            var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                            if (targetDetectedUnit != null && targetDetectedUnit.RefersToUnit != null)
                            {
                                searchOrder.StringParameter = targetDetectedUnit.RefersToUnit.UnitClass.Id;
                                searchOrder.SecondId = targetDetectedUnit.Id;
                            }
                            wp.Orders.Add(searchOrder);
                        }
                        movementOrder.AddWaypoint(wp);
                        //if (targetDetectedUnit == null || !targetDetectedUnit.IsFixed)
                        //{
                        //    var maxRemainingRangeM = WeaponClass.MaxWeaponRangeM - distanceToTargetM;
                        //    var searchRadiusM = maxRemainingRangeM * .1;
                        //    var startDepth = GameConstants.DEPTH_SHALLOW_MIN_M;

                        //    //TODO: make eliptical search pattern
                        //    var regionSearch = Region.FromCircle(targetPos.Coordinate, searchRadiusM);
                        //    var depthIncrement = Math.Abs((GameConstants.DEPTH_VERY_DEEP_MIN_M - GameConstants.DEPTH_SHALLOW_MIN_M) / 16.0);
                        //    foreach (var reg in regionSearch.Coordinates)
                        //    {
                        //        var sPos = new Position(reg);
                        //        sPos.HeightOverSeaLevelM = startDepth;
                        //        startDepth -= depthIncrement;
                        //        var wpSt = new Waypoint(sPos);
                        //        //var searchOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.MissileSearchForTarget);
                        //        //if (targetDetectedUnit != null && targetDetectedUnit.RefersToUnit != null)
                        //        //{
                        //        //    searchOrder.StringParameter = targetDetectedUnit.RefersToUnit.UnitClass.Id;
                        //        //    searchOrder.SecondId = targetDetectedUnit.Id;
                        //        //}
                        //        //wpSt.Orders.Add(searchOrder);
                        //        movementOrder.AddWaypoint(wpSt);
                        //    }
                        //}
                        //else
                        //{
                        //    wpTarget = new Waypoint(targetDetectedUnit);
                        //    movementOrder.AddWaypoint(wpTarget);
                        //}
                        if (targetDetectedUnit != null)
                        {
                            wpTarget = new Waypoint(targetDetectedUnit);
                        }
                        else
                        {
                            wpTarget = new Waypoint(targetPos);
                            wpTarget.Position.HeightOverSeaLevelM = -10;
                        }
                        break;

                    case GameConstants.WeaponTrajectoryType.AsRoc:
                        if (targetDetectedUnit != null)
                        {
                            wpTarget = new Waypoint(targetDetectedUnit);
                        }
                        else
                        {
                            wpTarget = new Waypoint(targetPos);
                        }
                        movementOrder.AddWaypoint(wpTarget);
                        break;
                    default:
                        firstPos = OwnerUnit.Position.Offset(GetCurrentWeaponBearingDeg(), 250 + GameManager.Instance.GetRandomNumber(250));
                        if (OwnerUnit.DomainType != GameConstants.DomainType.Air)
                        {
                            firstPos.HeightOverSeaLevelM = GameManager.Instance.GetRandomNumber(50) + 50;
                        }
                        wp = new Waypoint(firstPos);
                        movementOrder.AddWaypoint(wp);
                        if (targetDetectedUnit != null)
                        {
                            wpTarget = new Waypoint(targetDetectedUnit);
                        }
                        else
                        {
                            wpTarget = new Waypoint(targetPos);
                        }
                        movementOrder.AddWaypoint(wpTarget);
                        break;
                }

                return movementOrder;

            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("CreateMissileMovementOrder failed for Wpn " + this.ToString() + ".  " + ex.ToString());
                return null;
            }
		}

		private MovementOrder GetBallisticProjectileMovement(DetectedUnit targetDetectedUnit, double bearingToTargetDeg, double distanceToTargetM)
		{
			var movementOrder = new MovementOrder();
			if (OwnerUnit.Position == null || OwnerUnit.Position.HeightOverSeaLevelM == null 
				|| targetDetectedUnit == null || targetDetectedUnit.Position == null 
				|| targetDetectedUnit.Position.HeightOverSeaLevelM == null)
			{
				return movementOrder; //error condition
			}
			var pos1 = OwnerUnit.Position.Offset(bearingToTargetDeg, 100);
			pos1.HeightOverSeaLevelM = OwnerUnit.Position.HeightOverSeaLevelM;
			movementOrder.AddWaypoint(pos1);
			double maxHeightM = GameConstants.HEIGHT_VERY_HIGH_MIN_M;
			if (distanceToTargetM < 1000)
			{
			    var ownerHeight = OwnerUnit.Position.HeightOverSeaLevelM != null
			                          ? OwnerUnit.Position.HeightOverSeaLevelM.Value
			                          : 0.0;

                maxHeightM = MapHelper.GetMaxValue(ownerHeight, targetDetectedUnit.Position.HeightOverSeaLevelM.Value);
			    if (maxHeightM < 100)
				{
					maxHeightM = 500.0;
				}
				var midPointCoord = MapHelper.CalculateMidpoint(OwnerUnit.Position.Coordinate, targetDetectedUnit.Position.Coordinate);
				movementOrder.AddWaypoint(new Waypoint(new Position(midPointCoord.LatitudeDeg, midPointCoord.LongitudeDeg, maxHeightM)));
				movementOrder.AddWaypoint(new Waypoint(targetDetectedUnit));
				return movementOrder;
			}
			
			var coord2 = MapHelper.CalculateNewPosition2(OwnerUnit.Position.Coordinate, bearingToTargetDeg, distanceToTargetM * .25);
			var pos2 = new Position(coord2.LatitudeDeg, coord2.LongitudeDeg, maxHeightM * 0.6);
			var coord3 = MapHelper.CalculateMidpoint(OwnerUnit.Position.Coordinate, targetDetectedUnit.Position.Coordinate);
			var pos3 = new Position(coord3.LatitudeDeg, coord3.LongitudeDeg, maxHeightM);
			var coord4 = MapHelper.CalculateNewPosition2(OwnerUnit.Position.Coordinate, bearingToTargetDeg, distanceToTargetM * .75);
			var pos4 = new Position(coord4.LatitudeDeg, coord4.LongitudeDeg, maxHeightM * 0.6);
			
            movementOrder.AddWaypoint(pos2);
			movementOrder.AddWaypoint(pos3);
			movementOrder.AddWaypoint(pos4);
			movementOrder.AddWaypoint(new Waypoint(targetDetectedUnit));

			return movementOrder;
		}

		public double GetCurrentWeaponBearingDeg()
		{
			return MapHelper.CalculateCombinedBearingDeg((double)OwnerUnit.Position.BearingDeg, this.WeaponBearingDeg);
		}

		public bool IsCoordinateInSectorRange(Coordinate coordinate)
		{
			if (this.WeaponClass.WeaponBearingRangeDeg >= 360)
			{
				return true;
			}
			if(OwnerUnit == null || OwnerUnit.Position == null || !OwnerUnit.Position.HasBearing)
			{
				return false;
			}
			return MapHelper.IsWithinSector(OwnerUnit.Position.Coordinate, GetCurrentWeaponBearingDeg(),
				this.WeaponClass.WeaponBearingRangeDeg, coordinate);
		}

		public double GetHitPercent(DetectedUnit detectedUnit, double distanceToTargetM)
		{
            if (distanceToTargetM > WeaponClass.MaxWeaponRangeM)
            {
                //If the system asks before weapon is in range, we return the value it will have when inside effective range.
                distanceToTargetM = WeaponClass.EffectiveWeaponRangeM;
            }
			double hitPercent = WeaponClass.HitPercent;
			double targetAgility = 1.0;
			if (detectedUnit.RefersToUnit != null)
			{
				targetAgility = detectedUnit.RefersToUnit.UnitClass.AgilityFactor;
			}
			if (targetAgility > WeaponClass.ReferenceAgilityFactor)
			{
				hitPercent *= 0.5;
			}
            if (detectedUnit.RefersToUnit != null 
                && detectedUnit.RefersToUnit.UnitClass.UnitType == GameConstants.UnitType.Missile 
                && detectedUnit.Position.HeightOverSeaLevelM < 20)
            {
                hitPercent *= 0.6;
                if (hitPercent < 1)
                {
                    hitPercent = 1;
                }
            }
            //Guns that can fire on air and on land/surface should be less accurate against air, and even less accurate against missiles
            if (detectedUnit.DomainType == GameConstants.DomainType.Air && !WeaponClass.SpawnsUnitOnFire && (WeaponClass.CanTargetSurface || WeaponClass.CanTargetLand))
            {
                hitPercent *= 0.8;
                //if (detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Missile)
                //{
                //    hitPercent -= 30;
                //}
                if (distanceToTargetM > GameConstants.DEFAULT_CIWS_RANGE_M)
                {
                    var factor = (1.0 - ((distanceToTargetM - GameConstants.DEFAULT_CIWS_RANGE_M) / (WeaponClass.MaxWeaponRangeM - GameConstants.DEFAULT_CIWS_RANGE_M)));
                    hitPercent *= factor;
                }
                if (hitPercent < 10)
                {
                    hitPercent = 10;
                }
            }
			if((targetAgility > WeaponClass.ReferenceAgilityFactor + 1))
			{
                if (distanceToTargetM > GameConstants.DEFAULT_CIWS_RANGE_M)
                {
                    hitPercent = 0.0;
                }
                else
                {
                    hitPercent = hitPercent.Clamp(1, 10);
                }
				
			}
            GameManager.Instance.Log.LogDebug(
                string.Format("BaseWeapon->GetHitPercent returns {0}% for weapon {1} against {2}. Range {3}m.", hitPercent, this, detectedUnit, distanceToTargetM));

			return hitPercent;
		}

        /// <summary>
        /// Depending on unit, target, evasion speed and other factors, calculates the optimal speed to open fire on a target.
        /// </summary>
        /// <param name="targetDetectedUnit"></param>
        /// <returns></returns>
        public double CalculateDesiredWeaponDistanceToTargetM(DetectedUnit targetDetectedUnit)
        {
            var desiredDistanceToTargetM = WeaponClass.EffectiveWeaponRangeM;
            if (targetDetectedUnit == null || !WeaponClass.SpawnsUnitOnFire)
            {
                return desiredDistanceToTargetM;
            }
            
            if (OwnerUnit.DomainType == GameConstants.DomainType.Air && targetDetectedUnit.DomainType == GameConstants.DomainType.Subsea)
            {
                //Aircraft should go as close as absolutely possibly before launching torps, to avoid subs evading.
                desiredDistanceToTargetM = WeaponClass.MinWeaponRangeM * 2.0;
                if (desiredDistanceToTargetM < 100)
                {
                    desiredDistanceToTargetM = 100;
                }
            }
            if (OwnerUnit.DomainType == GameConstants.DomainType.Air && targetDetectedUnit.DomainType == GameConstants.DomainType.Air)
            {
                var speedTargetKph = 1000.0;
                if (targetDetectedUnit.RefersToUnit != null)
                {
                    speedTargetKph = targetDetectedUnit.RefersToUnit.UnitClass.MaxSpeedKph;
                }
                var missileSpeedKph = WeaponClass.MaxSpeedKph;
                if (missileSpeedKph <= speedTargetKph)
                {
                    desiredDistanceToTargetM *= 0.5; //if the target can outrun missile anyway, hope for the best
                }
                else
                {
                    //we have some math to do
                    var maxRangeSec = WeaponClass.MaxWeaponRangeM / missileSpeedKph.ToMperSecFromKph();
                    var targetMaxMovementM = speedTargetKph.ToMperSecFromKph() * maxRangeSec;
                    desiredDistanceToTargetM = WeaponClass.MaxWeaponRangeM - targetMaxMovementM;
                }
            }
            return desiredDistanceToTargetM;

        }
		public int GetRoundToFireCount(DetectedUnit detectedUnit, GameConstants.EngagementStrength engagementStrength)
		{
            if (WeaponClass.MaxSimultanousShots == 1)
            {
                return 1;
            }
            if (detectedUnit == null) //bearing only attack
            {
                switch (engagementStrength)
                {
                    case GameConstants.EngagementStrength.MinimalAttack:
                        return 1;
                    case GameConstants.EngagementStrength.DefaultAttack:
                        return 2;
                    case GameConstants.EngagementStrength.OverkillAttack:
                        return 4;
                    default:
                        return 1;
                }
            }
			int hitPointsTarget = 0;
			int assumedDefenseFactor = 1;
			if (detectedUnit.IsIdentified && detectedUnit.RefersToUnit != null)
			{
				hitPointsTarget = detectedUnit.RefersToUnit.UnitClass.MaxHitpoints;
				if (this.WeaponClass.CanBeTargetted) //try to saturate target defenses if e.g. cruise missile
				{
					if (detectedUnit.RefersToUnit.SupportsRole(GameConstants.Role.IsSurfaceCombattant))
					{
						assumedDefenseFactor = 2;
                        if (detectedUnit.RefersToUnit.SupportsRole(GameConstants.Role.AttackAir))
                        {
                            assumedDefenseFactor = 3;    
                        }
					}
					if (detectedUnit.RefersToUnit.UnitClass.UnitType == GameConstants.UnitType.LandInstallation)
					{
						assumedDefenseFactor = 2;
                        if (detectedUnit.IsKnownToSupportRole(GameConstants.Role.LaunchFixedWingAircraft))
                        {
                            assumedDefenseFactor = 5;
                        }
					}
                    if (detectedUnit.GetDetectedGroup() != null && detectedUnit.GetDetectedGroup().DetectedUnits.Count > 2)
                    {
                        assumedDefenseFactor = 4;
                    }
				}
			}
			else
			{
				switch (detectedUnit.DetectionClassification)
				{
					case GameConstants.DetectionClassification.Unknown:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip);
						break;
                    //case GameConstants.DetectionClassification.Surface:
                    //    hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip);
                    //    assumedDefenseFactor = 2;
                    //    break;
					case GameConstants.DetectionClassification.SmallSurface:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) / 2;

						break;
					case GameConstants.DetectionClassification.MediumSurface:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip);
						assumedDefenseFactor = 2;
						break;
					case GameConstants.DetectionClassification.LargeSurface:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip) * 2;
						assumedDefenseFactor = 2;
						break;
                    //case GameConstants.DetectionClassification.Aircraft:
                    //    hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft);

                    //    break;
					case GameConstants.DetectionClassification.FixedWingAircraft:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft);

						break;
					case GameConstants.DetectionClassification.FixedWingAircraftLarge:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.FixedwingAircraft) * 2;

						break;
					case GameConstants.DetectionClassification.Helicopter:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Helicopter);
						break;
					case GameConstants.DetectionClassification.Missile:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Missile);

						break;
					case GameConstants.DetectionClassification.Subsurface:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine);
						break;
					case GameConstants.DetectionClassification.Submarine:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Submarine);
						break;
					case GameConstants.DetectionClassification.Torpedo:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Torpedo);
						break;
					case GameConstants.DetectionClassification.Mine:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Mine);

						break;
					case GameConstants.DetectionClassification.Sonobuoy:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.Sonobuoy);

						break;
					case GameConstants.DetectionClassification.BallisticMissile:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.BallisticProjectile);

						break;
					case GameConstants.DetectionClassification.LandInstallation:
						hitPointsTarget = GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation);

						break;

				} //case

			}
			if (hitPointsTarget <= 0)
			{
				return 0;
			}
			int missFactor = (100 / this.WeaponClass.HitPercent);
			if (engagementStrength == GameConstants.EngagementStrength.MinimalAttack)
			{
				assumedDefenseFactor = 1;
			}
			else if (engagementStrength == GameConstants.EngagementStrength.OverkillAttack)
			{
				assumedDefenseFactor *= 2;
			}

            if (!WeaponClass.CanBeTargetted)
            {
                assumedDefenseFactor = 1;
            }
			int desiredCountRounds = missFactor * (((hitPointsTarget / WeaponClass.DamageHitPoints) + 1)  * assumedDefenseFactor);
			//if (DesiredCountRounds > WeaponClass.MaxSimultanousShots)
			//{
			//    DesiredCountRounds = WeaponClass.MaxSimultanousShots;
			//}
			if (detectedUnit.DomainType != GameConstants.DomainType.Subsea && desiredCountRounds >= 4 
				&& desiredCountRounds < AmmunitionRemaining 
				&& AmmunitionRemaining-desiredCountRounds <= 4) //if only a few rounds left anyway, just launch all missiles
			{
				desiredCountRounds = AmmunitionRemaining;
			}
            if (OwnerUnit.DomainType == GameConstants.DomainType.Subsea 
                 && detectedUnit.DomainType == GameConstants.DomainType.Surface &&
                 assumedDefenseFactor > 1 &&
                 this.WeaponClass.SpawnsUnitOnFire && 
                 this.AmmunitionRemaining <= 14 &&
                 this.WeaponClass.MaxSpeedKph > 400) //subs firing missiles against surface: better go for it!
            {
                desiredCountRounds = AmmunitionRemaining;
            }
			GameManager.Instance.Log.LogDebug("BaseWeapon->GetRoundToFireCount returns Count = " + desiredCountRounds 
                + " for weapon " + this + "  against target:" + detectedUnit);
			return desiredCountRounds;
		}

		public virtual WeaponInfo GetWeaponInfo()
		{
			WeaponInfo info = new WeaponInfo();
			info.Id = Id;
            //info.Name = Name;
            //info.ShortName = this.WeaponClass.WeaponClassShortName;
            //info.OwnerPlayerId = OwnerPlayer.Id;
			info.OwnerUnitId = OwnerUnit.Id;
			info.WeaponClassId = WeaponClass.Id;
			//info.WeaponPitchDeg = WeaponPitchDeg;
			info.WeaponBearingDeg = WeaponBearingDeg;
			info.IsOperational = IsOperational;
			info.IsPrimaryWeapon = IsPrimaryWeapon;
			info.LastFiredGameWorldTimeSec = LastFiredGameWorldTimeSec;
			info.MaxAmmunition = MaxAmmunition;
			info.AmmunitionRemaining = AmmunitionRemaining;
			info.ReadyInSec = ReadyInSec;
			info.IsDamaged = (ReadyInSec > this.WeaponClass.TimeBetweenShotsSec);
			return info;
		}

		public override void Tick(double timer)
		{
			base.Tick(timer);
		}

		public override string ToString()
		{
			string temp = Name;
			if (OwnerUnit != null)
			{ 
				temp += " on " + OwnerUnit.ToShortString();
			}
			return temp;
		}



		/// <summary>
		/// Returns EngagementStatus (readyness to fire) against a specific target
		/// </summary>
		/// <param name="detectedUnit"></param>
		/// <param name="distanceToTargetM"></param>
		/// <returns></returns>
		public EngagementStatus GetEngagementStatus(DetectedUnit detectedUnit, double distanceToTargetM)
		{
			EngagementStatus status = new EngagementStatus();
			status.Unit = OwnerUnit;
			status.Weapon = this;
			status.TargetDetectedUnit = detectedUnit;
			if (detectedUnit.RefersToUnit != null)
			{
				status.TargetHitpoints = detectedUnit.RefersToUnit.HitPoints;
			}
			else
			{
				status.TargetHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.SurfaceShip);
			}
			status.DistanceToTargetM = distanceToTargetM;
			status.EngagementStatusResult = GameConstants.EngagementStatusResultType.Undetermined;
			BaseUnit targetUnit = detectedUnit.RefersToUnit;
			if (targetUnit == null)
			{
				return status;
			}

			bool canTarget = CanTargetDetectedUnit(targetUnit, true);
			if (canTarget)
			{
                if (this.AmmunitionRemaining < 1)
                {
                    status.EngagementStatusResult = GameConstants.EngagementStatusResultType.OutOfAmmo;
                }
                else if (distanceToTargetM <= WeaponClass.MaxWeaponRangeM)
				{
					status.EngagementStatusResult = GameConstants.EngagementStatusResultType.ReadyToEngage;
					if (!IsCoordinateInSectorRange(targetUnit.Position.Coordinate))
					{
						status.EngagementStatusResult = GameConstants.EngagementStatusResultType.OutOfSectorRange;
					}
					if (distanceToTargetM < WeaponClass.MinWeaponRangeM)
					{
						status.EngagementStatusResult = GameConstants.EngagementStatusResultType.TooCloseToEngage;
					}
				}
				else
				{
					status.EngagementStatusResult = GameConstants.EngagementStatusResultType.MustCloseToEngage;
					status.DistanceToCloseM = distanceToTargetM - WeaponClass.EffectiveWeaponRangeM;
				}
			}
			else //cannot target this unit at all
			{
				if (WeaponClass.IsNotWeapon)
				{
					status.EngagementStatusResult = GameConstants.EngagementStatusResultType.NoWeapon;
				}
				else if (this.ReadyInSec > this.WeaponClass.TimeBetweenShotsSec)
				{
					status.EngagementStatusResult = GameConstants.EngagementStatusResultType.WeaponDamaged;
				}
				else
				{
					status.EngagementStatusResult = GameConstants.EngagementStatusResultType.CannotFireAtThisTarget;
				}
			}

			return CalculateEngagementStatusScore(status);
		}
		/// <summary>
		/// Sets Score property on an EngagementStatus based on expected effectiveness in engaging
		/// the set target with this weapon.
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public EngagementStatus CalculateEngagementStatusScore(EngagementStatus status)
		{
			status.Score = 0;
			if (status.Weapon == null)
			{
				return status;
			}
			var weapon = status.Weapon;
			var distanceToTargetM = status.DistanceToTargetM;
			double hitPercent = WeaponClass.HitPercent;
			if (status.TargetDetectedUnit != null && status.WeaponCanBeUsedAgainstTarget)
			{
				hitPercent = GetHitPercent(status.TargetDetectedUnit, distanceToTargetM);
			}
			switch (status.EngagementStatusResult)
			{
				case GameConstants.EngagementStatusResultType.Undetermined:
					break;
				case GameConstants.EngagementStatusResultType.ReadyToEngage:
					status.Score += 10;
					break;
				case GameConstants.EngagementStatusResultType.MustCloseToEngage:
					status.Score += 2;
					break;
				case GameConstants.EngagementStatusResultType.TooCloseToEngage:
					status.Score += 1;
					break;
				case GameConstants.EngagementStatusResultType.OutOfSectorRange:
					status.Score += 1;
					if (OwnerUnit.DomainType == GameConstants.DomainType.Air)
					{
						status.Score += 1; //can easily turn
					}
					break;
				case GameConstants.EngagementStatusResultType.NoWeapon:
					status.Score -= 20;
					break;
				case GameConstants.EngagementStatusResultType.OutOfAmmo:
					status.Score -= 5;
					break;
				case GameConstants.EngagementStatusResultType.WeaponDamaged:
					break;
				case GameConstants.EngagementStatusResultType.CannotFireAtThisTarget:
					status.Score -= 20;
					break;
				default:
					break;
			}

            if (status.EngagementStatusResult == GameConstants.EngagementStatusResultType.MustCloseToEngage ||
                status.EngagementStatusResult == GameConstants.EngagementStatusResultType.OutOfSectorRange ||
                status.EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage ||
                status.EngagementStatusResult == GameConstants.EngagementStatusResultType.TooCloseToEngage)
            {
                if (status.Weapon.IsPrimaryWeapon)
                {
                    status.Score += 3;
                } 
                if (weapon.WeaponClass.MaxSeaState > weapon.OwnerUnit.GetEffectiveSeaState())
                {
                    status.Score += 2;
                }
                else
                {
                    status.Score -= 5;
                }
                if (weapon.IsReady)
                {
                    status.Score += 1;
                }
                if (weapon.ReadyInSec < 10)
                {
                    status.Score += 1;
                }
                if (status.DistanceToCloseM > 100000)
                {
                    status.Score -= 1;
                }
                if (status.DistanceToCloseM > 50000 && OwnerUnit.DomainType != GameConstants.DomainType.Air)
                {
                    status.Score -= 1;
                }
                if (status.DistanceToCloseM > 10000 && OwnerUnit.DomainType != GameConstants.DomainType.Air)
                {
                    status.Score -= 1;
                }

                if (weapon.WeaponClass.EffectiveWeaponRangeM >= distanceToTargetM)
                {
                    status.Score += 2;
                }
                if (weapon.WeaponClass.EffectiveWeaponRangeM >= distanceToTargetM * 3)
                {
                    status.Score -= 2;
                }
                if (weapon.AmmunitionRemaining * weapon.WeaponClass.DamageHitPoints > status.TargetHitpoints)
                {
                    status.Score += 2;
                }
                var hitPointsRatio = (status.TargetHitpoints / weapon.WeaponClass.DamageHitPoints); //how many hits would kill it
                if (hitPointsRatio < 5)
                {
                    status.Score += 1;
                }
                if (hitPointsRatio > 20)
                {
                    status.Score -= 1;
                }
                if (hitPercent < 50)
                {
                    status.Score -= 2;
                }
                if (hitPercent < 30)
                {
                    status.Score -= 2;
                }
                if (hitPercent < 5)
                {
                    status.Score -= 10;
                }

                if (weapon.AmmunitionRemaining > 30)
                {
                    status.Score += 1;
                }
                if (weapon.AmmunitionRemaining > 18) //give more priority to ciws
                {
                    status.Score += 1;
                }
                if (!weapon.WeaponClass.SpawnsUnitOnFire)
                {
                    status.Score += 1;
                }
                if (weapon.WeaponClass.MaxSimultanousShots * weapon.WeaponClass.DamageHitPoints >= status.TargetHitpoints)
                {
                    status.Score += 2;
                }

            }
            else //statuses reported when this weapon can't be used against target
            {
                status.Score -= 10;
            }
			return status;
		}

		#endregion


	}
}
