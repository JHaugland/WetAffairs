using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System.Diagnostics;

namespace TTG.NavalWar.NWData.Ai
{
	[Serializable]
	public class BaseAIHandler
	{
		#region "Constants and private members"
		
		public const double MAX_DISTANCE_PATROL_ASW_M = 20000;
		public const double MAX_DISTANCE_PATROL_AEW_M = 50000;
		
		protected long _minuteCounter = 0;
		protected DateTime _timeLastEngagedTargets;

		#endregion
		#region "Constructors"

		public BaseAIHandler()
		{
			PrioritizedTargetUnitClassIds = new List<string>();
			PrioritizedDefenceOwnUnitClassIds = new List<string>();
		}

		public BaseAIHandler(Player ownerPlayer) : this()
		{
			OwnerPlayer = ownerPlayer;
		}

		#endregion


		#region "Public properties"
		
		public virtual Player OwnerPlayer { get; set; }

		public List<string> PrioritizedTargetUnitClassIds { get; set; }

		public List<string> PrioritizedDefenceOwnUnitClassIds { get; set; }

		#endregion



		#region "Public methods"

		#region "Event handler methods"
		public virtual void NewDetection(DetectedUnit detectedUnit)
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
            //GameManager.Instance.Log.LogDebug("BaseAIHandler.NewDetection triggered for " + detectedUnit.ToString());
			CheckEventTriggersForDetection(detectedUnit);
			EngageAirThreat(detectedUnit, false, false);
            SetPriorityScoresOnDetectedUnit(detectedUnit, true);
            TargetRespondToDetectedUnit(detectedUnit);

            if (!OwnerPlayer.IsComputerPlayer && OwnerPlayer.IsAutomaticallyChangingTimeOnDetection && 
                GameManager.Instance.Game.RealTimeCompressionFactor > OwnerPlayer.TimeCompressionOnDetection)
            {
                GameManager.Instance.Game.RealTimeCompressionFactor = OwnerPlayer.TimeCompressionOnDetection;
            }
		}

		public virtual void DetectionUpdated(DetectedUnit detectedUnit)
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			//GameManager.Instance.Log.LogDebug("BaseAIHandler.DetectionUpdated triggered for " + detectedUnit.ToString());
			CheckEventTriggersForDetection(detectedUnit);
            SetPriorityScoresOnDetectedUnit(detectedUnit, true);
			EngageAirThreat(detectedUnit, false, false);
            TargetRespondToDetectedUnit(detectedUnit);
		}

		public virtual void DetectionLost(DetectedUnit detectedUnit)
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			GameManager.Instance.Log.LogDebug("BaseAIHandler.DetectionLost triggered for " + detectedUnit.ToString());
		    SetDistanceScoresOnDetectedUnits();
		}

		public virtual void BattleDamageReportReceived(BattleDamageReport battleDamageReport)
		{
            Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
            if (battleDamageReport.PlayerInflictingDamageId == OwnerPlayer.Id) //this is a report about this player's attack
            {
                if (battleDamageReport.IsTargetPlatformDestroyed)
                {
                    var targetUnit = OwnerPlayer.GetDetectedUnitById(battleDamageReport.TargetPlatformId);
                    var attackingUnit = OwnerPlayer.GetUnitById(battleDamageReport.PlatformInflictingDamageId);
                    if (targetUnit != null && attackingUnit != null)
                    {
                        if (attackingUnit.TargetDetectedUnit != null && attackingUnit.TargetDetectedUnit.Id == targetUnit.Id)
                        {
                            attackingUnit.TargetDetectedUnit = null;
                        }
                    }
                }
            }

            if (!OwnerPlayer.IsComputerPlayer && OwnerPlayer.IsAutomaticallyChangingTimeOnBattleReport && 
                (battleDamageReport.AttackeePriority != GameConstants.Priority.Low || battleDamageReport.AttackerPriority != GameConstants.Priority.Low) &&
                GameManager.Instance.Game.RealTimeCompressionFactor > OwnerPlayer.TimeCompressionOnBattleReport)
            {
                GameManager.Instance.Game.RealTimeCompressionFactor = OwnerPlayer.TimeCompressionOnBattleReport;
            }
		}

		public virtual void UnitHasNoMovementOrders(BaseUnit baseUnit)
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");

            if (!OwnerPlayer.IsComputerPlayer && OwnerPlayer.IsAutomaticallyChangingTimeOnNoOrder && 
                GameManager.Instance.Game.RealTimeCompressionFactor > OwnerPlayer.TimeCompressionOnNoOrder)
            {
                GameManager.Instance.Game.RealTimeCompressionFactor = OwnerPlayer.TimeCompressionOnNoOrder;
            }
		}

		public virtual void Tick(DateTime gameTime)
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			if (_timeLastEngagedTargets.AddSeconds(60) < gameTime)
			{
				_timeLastEngagedTargets = gameTime;
				TickEveryMinute(gameTime);
			}
		}

		public virtual void TickEveryMinute(DateTime gameTime)
		{
			if (GameManager.Instance.Game.EconomicModel != GameConstants.GameEconoomicModel.NoEconomy)
			{ 
				OwnerPlayer.Credits += GameManager.Instance.Game.CreditsPerMinute;
			}
			_minuteCounter++;
			SetPriorityScoresOnDetectedUnits();
            ActivateIdleUnits();
            var isDefeated = OwnerPlayer.HasBeenDefeated;
		}

		public virtual void GamePlayHasStarted()
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
		}

		public virtual void GameLossOrVictoryAchieved()
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
		}

        /// <summary>
        /// This is called whenever in air refueling fails for a unit when the order to refuel in air was computer generated.
        /// </summary>
        /// <param name="airUnit">The unit that failed to refuel</param>
        /// <param name="refuelAircraft">The refueling aircraft that failed</param>
        public void RefuelInAirFailed(AircraftUnit airUnit, AircraftUnit refuelAircraft)
        {
            Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");

            // For a computer player the refuel aircraft should return to base
            if (OwnerPlayer.IsComputerPlayer)
            {
                refuelAircraft.ReturnToBase();
            }
        }

		#endregion

		#region "Utility methods"

        public void SetLandInstallationsActiveRadar()
        {
            foreach (var unit in OwnerPlayer.Units)
            {
                if (unit.UnitClass.IsAlwaysVisibleForEnemy)
                {
                    unit.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
                }
            };
        }

        public void SetWeaponAndSensorOrders()
        {
            foreach (var unit in OwnerPlayer.Units)
            {
                if (unit.UnitClass.IsLandbased || unit.SupportsRole(GameConstants.Role.IsSurfaceCombattant))
                {
                    unit.WeaponOrders = GameConstants.WeaponOrders.FireOnAllClearedTargets;
                    if (unit.UnitClass.IsAlwaysVisibleForEnemy)
                    {
                        unit.SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
                    }
                }
                if (unit.UnitClass.IsSubmarine)
                {
                    if (unit.Position.HasHeightOverSeaLevel && unit.Position.HeightOverSeaLevelM > -10)
                    {
                        var order = new BaseOrder() { OrderType = GameConstants.OrderType.SetElevation, HeightDepthPoints = GameConstants.HeightDepthPoints.MediumDepth };
                        unit.Orders.Enqueue(order);
                    }
                    foreach (var sonar in unit.Sensors.Where<BaseSensor>(u=>u.SensorClass.SensorType == GameConstants.SensorType.Sonar && u.SensorClass.IsDeployableSensor))
                    {
                        var depOrder = OrderFactory.CreateSensorDeploymentOrder(unit.Id, sonar.Id, true, false);
                        var depBaseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(depOrder);
                        unit.Orders.AddLast(depBaseOrder);
                    }
                }
            }
        }

		public void SetPrioritizedTargets()
		{
			Debug.Assert(OwnerPlayer != null, "OwnerPlayer should never be null.");
			// Go through defeat conditions to find which units to attack, and which to defend
            //foreach (var defCondSets in OwnerPlayer.DefeatConditionSets)
            //{
            //    foreach (var defCond in defCondSets.DefeatConditions)
            //    {
            //        switch (defCond.DefeatConditionType)
            //        {
            //            case GameConstants.DefeatConditionType.CountUnitClassIdLowerThan:
            //            case GameConstants.DefeatConditionType.CountUnitClassLostHigherThan:
            //            case GameConstants.DefeatConditionType.CountUnitClassIdInSectorLowerThan:
            //                PrioritizedDefenceOwnUnitClassIds.Add(defCond.ConditionParameter);
            //                break;
            //            case GameConstants.DefeatConditionType.CountUnitRoleLowerThan:
            //            case GameConstants.DefeatConditionType.CountUnitRoleLostHigherThan:
            //            case GameConstants.DefeatConditionType.CountUnitRoleInSectorLowerThan:
            //                try
            //                {
            //                    var role = (GameConstants.Role)int.Parse(defCond.ConditionParameter);
            //                    var unitClasses = OwnerPlayer.GetActiveUnitClassesByRole(role);
            //                    PrioritizedDefenceOwnUnitClassIds.AddRange(unitClasses);
            //                }
            //                catch (Exception ex)
            //                {
            //                    GameManager.Instance.Log.LogError(
            //                        string.Format("BaseAIHandler->SetPrioritizedTargets own roles error: {0}", ex.Message));
            //                }
            //                break;

            //            case GameConstants.DefeatConditionType.TimeElapsedLongerThanSec:
            //                //ignore
            //                break;
            //            case GameConstants.DefeatConditionType.CountEnemyClassHigherThan:
            //            case GameConstants.DefeatConditionType.CountEnemyClassIdInSectorHigherThan:
            //                PrioritizedTargetUnitClassIds.Add(defCond.ConditionParameter);
            //                break;
            //            case GameConstants.DefeatConditionType.CountEnemyRoleHigherThan:
            //            case GameConstants.DefeatConditionType.CountEnemyRoleInSectorHigherThan:
            //                try
            //                {
            //                    var role = (GameConstants.Role)int.Parse(defCond.ConditionParameter);
            //                    var unitClasses = OwnerPlayer.GetActiveEnemyUnitClassesByRole(role);
            //                    PrioritizedTargetUnitClassIds.AddRange(unitClasses);
            //                }
            //                catch (Exception ex)
            //                {
            //                    GameManager.Instance.Log.LogError(
            //                        string.Format("BaseAIHandler->SetPrioritizedTargets enemy roles error: {0}", ex.Message));
            //                }
						
            //                break;
            //            default:
            //                break;
            //        }
					
            //    }
            //}
            //foreach (var player in OwnerPlayer.Enemies)
            //{
            //    foreach (var defCondSets in player.DefeatConditionSets)
            //    {
            //        foreach (var defCond in defCondSets.DefeatConditions)
            //        {
            //            switch (defCond.DefeatConditionType)
            //            {
            //            case GameConstants.DefeatConditionType.CountUnitClassIdLowerThan:
            //            case GameConstants.DefeatConditionType.CountUnitClassLostHigherThan:
            //            case GameConstants.DefeatConditionType.CountUnitClassIdInSectorLowerThan:
            //                PrioritizedTargetUnitClassIds.Add(defCond.ConditionParameter);
            //                break;
            //            case GameConstants.DefeatConditionType.CountUnitRoleLowerThan:
            //            case GameConstants.DefeatConditionType.CountUnitRoleLostHigherThan:
            //            case GameConstants.DefeatConditionType.CountUnitRoleInSectorLowerThan:
            //                try
            //                {
            //                    var role = (GameConstants.Role)int.Parse(defCond.ConditionParameter);
            //                    var unitClasses = OwnerPlayer.GetActiveUnitClassesByRole(role);
            //                    PrioritizedTargetUnitClassIds.AddRange(unitClasses);
            //                }
            //                catch (Exception ex)
            //                {
            //                    GameManager.Instance.Log.LogError(
            //                        string.Format("BaseAIHandler->SetPrioritizedTargets enemy roles error: {0}", ex.Message));
            //                }
            //                break;

            //            case GameConstants.DefeatConditionType.TimeElapsedLongerThanSec:
            //                //ignore
            //                break;
            //            case GameConstants.DefeatConditionType.CountEnemyClassHigherThan:
            //            case GameConstants.DefeatConditionType.CountEnemyClassIdInSectorHigherThan:
            //                PrioritizedDefenceOwnUnitClassIds.Add(defCond.ConditionParameter);
            //                break;
            //            case GameConstants.DefeatConditionType.CountEnemyRoleHigherThan:
            //            case GameConstants.DefeatConditionType.CountEnemyRoleInSectorHigherThan:
            //                try
            //                {
            //                    var role = (GameConstants.Role)int.Parse(defCond.ConditionParameter);
            //                    var unitClasses = OwnerPlayer.GetActiveEnemyUnitClassesByRole(role);
            //                    PrioritizedDefenceOwnUnitClassIds.AddRange(unitClasses);
            //                }
            //                catch (Exception ex)
            //                {
            //                    GameManager.Instance.Log.LogError(
            //                        string.Format("BaseAIHandler->SetPrioritizedTargets enemy roles error: {0}", ex.Message));
            //                }
						
            //                break;
            //            default:
            //                break;
            //            }
            //        }
            //    }
            //}
			foreach (var unit in OwnerPlayer.Units)
			{
				if (unit.SupportsRole(GameConstants.Role.LaunchFixedWingAircraft)) //always defend airfields and carriers!
				{
					if (!PrioritizedDefenceOwnUnitClassIds.Contains(unit.UnitClass.Id))
					{ 
						PrioritizedDefenceOwnUnitClassIds.Add(unit.UnitClass.Id);
					}
				}
			}
            //GameManager.Instance.Log.LogDebug(
            //    string.Format("SetPrioritizedTargets(): Defence Classes Count: {0}, Target Classes Count: {1}",
            //    PrioritizedDefenceOwnUnitClassIds.Count, PrioritizedTargetUnitClassIds.Count));

		}


		public void CheckEventTriggersForDetection(DetectedUnit detectedUnit)
		{
			var triggerList = from t in OwnerPlayer.EventTriggers
							  where t.EventTriggerType == GameConstants.EventTriggerType.PlayerDetectsEnemy
								&& (t.TimeElapsedSec == 0
								|| t.TimeElapsedSec
								>= GameManager.Instance.Game.GameWorldTimeSec)
								&& !t.HasBeenTriggered
							  select t;
            var tempTriggerList = triggerList.ToList();  //since later events may change original EventTriggers collection
		    foreach (var t in tempTriggerList)
			{
				if (!string.IsNullOrEmpty(t.Tag))
				{
					if (detectedUnit.IsIdentified && detectedUnit.RefersToUnit != null && detectedUnit.RefersToUnit.Tag == t.Tag)
					{
						OwnerPlayer.ExecuteEventTriggers(t);
					}
				} 
				else if (!string.IsNullOrEmpty(t.UnitClassId))
				{
					if (detectedUnit.IsIdentified
						&& detectedUnit.RefersToUnit != null
						&& detectedUnit.RefersToUnit.UnitClass.Id == t.UnitClassId)
					{
						OwnerPlayer.ExecuteEventTriggers(t);
					}
				}
				else if (t.Role == GameConstants.Role.NoOrAnyRole || detectedUnit.IsKnownToSupportRole(t.Role))
				{
					OwnerPlayer.ExecuteEventTriggers(t);
				}

			}
		}

		public bool EngageSurfaceThreat(DetectedUnit detectedUnit, bool closeAndEngage, bool launchAircraftIfNecessary)
		{
			if (detectedUnit.IsTargetted || detectedUnit.IsKnownToBeCivilianUnit || detectedUnit.FriendOrFoeClassification != GameConstants.FriendOrFoe.Foe)
			{
				return true;
			}
			//First, let's try air assets actually within range
			var rolesList = new List<GameConstants.Role> {GameConstants.Role.IsAircraft, GameConstants.Role.AttackSurface};
		    var unitAttackList = OwnerPlayer.FindAllAvailableUnitRole(
				detectedUnit.Position.Coordinate, rolesList, string.Empty, false, false);
		    if (unitAttackList.Count > 0)
			{
				foreach (var u in unitAttackList)
				{
					if (u.IsCommunicationJammingCurrentlyInEffect())
					{
						continue;
					}
					if (u.CanImmediatelyFireOnTargetType(detectedUnit))
					{
						return u.EngageDetectedUnit(detectedUnit, 
							GameConstants.EngagementOrderType.EngageNotClose, 
							u.IsInGroupWithOthers());
					}
				}
			}
			//Next, find out if any surface units within range can fire immediately
			//TODO: This should only be done for HIGH-VALUE targets!

			rolesList.Clear();
			rolesList.Add(GameConstants.Role.IsSurfaceCombattant);
			rolesList.Add(GameConstants.Role.AttackSurface);
            unitAttackList = OwnerPlayer.FindAllAvailableUnitRole(
				detectedUnit.Position.Coordinate, rolesList, string.Empty, false, false);
			
			if (unitAttackList.Count > 0)
			{
				foreach (var u in unitAttackList)
				{
					if (u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
						&& u.TargetDetectedUnit != null
						&& u.TargetDetectedUnit.Id != detectedUnit.Id)
					{
						continue;
					}
					if (u.IsCommunicationJammingCurrentlyInEffect())
					{
						continue;
					}
					if (u.CanImmediatelyFireOnTargetType(detectedUnit))
					{
						return u.EngageDetectedUnit(detectedUnit, 
							GameConstants.EngagementOrderType.EngageNotClose, 
							u.IsInGroupWithOthers());
					}
				}
			}

			//If none of this works, set an air group to close and engage
			if (closeAndEngage)
			{
                const int noOfAircraft = 1;
                rolesList.Clear();
                rolesList.Add(GameConstants.Role.IsFixedWingAircraft);
                rolesList.Add(GameConstants.Role.AttackSurface);
                EngageDetectedTargetWithLaunchedAir(detectedUnit, rolesList, noOfAircraft);
			}
			if (launchAircraftIfNecessary && !IsAnyAircraftTargettingThisTarget(detectedUnit, true))
			{
				rolesList.Clear();
				const int countDesired = 4; //TODO: Evaluate targets
				const int countMin = 2;
				rolesList.Add(GameConstants.Role.IsFixedWingAircraft);
				rolesList.Add(GameConstants.Role.AttackSurface);
				int aircraftCount = LaunchAircraft(detectedUnit.Position.Coordinate, rolesList,
					string.Empty, countDesired, countMin, true,
					new List<BaseOrder>() 
					{ 
						new EngagementOrder(
							detectedUnit,GameConstants.EngagementOrderType.CloseAndEngage, 
							GameConstants.EngagementStrength.DefaultAttack) 
					}, "AI: EngageSurfaceThreat");
				GameManager.Instance.Log.LogDebug(
					string.Format("BaseAIHandler.EngageSurfaceThreat: Launched {0} aircraft to engage target {1}",
					aircraftCount, detectedUnit));
				return (aircraftCount > 0);
			}
			return false;
		}

        public List<BaseUnit> EngageDetectedTargetWithLaunchedAir(DetectedUnit detectedUnit, List<GameConstants.Role> rolesList, int noOfAircraft)
        {
            var unitAttackList = OwnerPlayer.FindAllAvailableUnitRole(
                detectedUnit.Position.Coordinate, rolesList, string.Empty, true, true);
            var attackerList = new List<BaseUnit>();
            var countAttackers = 0;
            if (unitAttackList.Count > 0)
            {
                foreach (var u in unitAttackList)
                {
                    if (u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
                            && u.TargetDetectedUnit != null
                            && u.TargetDetectedUnit.Id != detectedUnit.Id)
                    {
                        continue;
                    }
                    if (u.MissionType != GameConstants.MissionType.Patrol || u.IsOrderedToReturnToBase)
                    {
                        continue;
                    }
                    if (u.IsCommunicationJammingCurrentlyInEffect())
                    {
                        continue;
                    }
                    if (!u.HasAnyEngagementOrders() && u.HasEnoughFuelToReachTarget(detectedUnit.Position.Coordinate, true))
                    {
                        var hasAttacked = u.EngageDetectedUnit(detectedUnit,
                            GameConstants.EngagementOrderType.CloseAndEngage,
                            u.IsInGroupWithOthers());
                        GameManager.Instance.Log.LogDebug(
                            string.Format("EngageDetectedTargetWithLaunchedAir.EngageSurfaceThreat: {0} ordered to CLOSE AND ENGAGE target {1}: {2}",
                            u, detectedUnit, hasAttacked));
                        if (hasAttacked)
                        {
                            var countUnitsThisOrder = 1;
                            if (u.IsInGroupWithOthers())
                            {
                                countUnitsThisOrder = u.Group.Units.Count;
                            }
                            countAttackers += countUnitsThisOrder;
                            attackerList.Add(u);
                        }
                        if (countAttackers >= noOfAircraft)
                        {
                            break;
                        }
                        
                    }
                }
            }
            return attackerList;
        }

		public bool IsAnyAircraftTargettingThisTarget(DetectedUnit detectedUnit, bool includeGroup)
		{
			if (string.IsNullOrEmpty(detectedUnit.DetectedGroupId))
			{
				includeGroup = false;
			}
			if (includeGroup) //return true if any detectedunit in its detectedgroup is targetted
			{
				var detList = from u in OwnerPlayer.Units
							  where u.Position != null
							  && u.TargetDetectedUnit != null 
							  && u.TargetDetectedUnit.DetectedGroupId == detectedUnit.DetectedGroupId
							  && u.UnitClass.IsAircraft
							  select u;
				if (detList.Any())
				{
					return true;
				}
			    return false;
			}
			else //only consider this single unit
			{
				var list = from u in OwnerPlayer.Units
						   where u.Position != null
						   && u.TargetDetectedUnit != null && u.TargetDetectedUnit.Id == detectedUnit.Id
						   && u.UnitClass.IsAircraft
						   select u;
				if (list.Any())
				{
					return true;
				}
			    return false;
			}
		}

		public bool EngageSubseaThreat(DetectedUnit detectedUnit, bool closeAndEngage, bool launchAircraftIfNecessary)
		{
			if (detectedUnit.IsTargetted)
			{
				return true;
			}
			//First, let's try if any units are actually within firing range already
			var rolesList = new List<GameConstants.Role> {GameConstants.Role.AttackSubmarine};
			//rolesList.Add(GameConstants.Role.IsAircraft);
            var unitAttackList = OwnerPlayer.FindAllAvailableUnitRole(
				detectedUnit.Position.Coordinate, rolesList, string.Empty, false, false);
			if (unitAttackList.Count > 0)
			{
				foreach (var u in unitAttackList)
				{
					if (u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
						&& u.TargetDetectedUnit != null
						&& u.TargetDetectedUnit.Id != detectedUnit.Id)
					{
						continue;
					}

					if (u.CanImmediatelyFireOnTargetType(detectedUnit))
					{
						return u.EngageDetectedUnit(detectedUnit, GameConstants.EngagementOrderType.EngageNotClose, true);
					}
				}
			}
			//Then, order some aircraft to close and engage
			if (closeAndEngage)
			{
				rolesList.Clear();
				rolesList.Add(GameConstants.Role.IsAircraft);
				rolesList.Add(GameConstants.Role.AttackSubmarine);
                unitAttackList = OwnerPlayer.FindAllAvailableUnitRole(
					detectedUnit.Position.Coordinate, rolesList, string.Empty, true, true);
				if (unitAttackList.Count > 0)
				{
					foreach (var u in unitAttackList)
					{
						if (u.MissionType != GameConstants.MissionType.Patrol)
						{
							continue;
						}
						if (u.IsCommunicationJammingCurrentlyInEffect())
						{
							continue;
						}
						if (u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
							&& u.TargetDetectedUnit != null
							&& u.TargetDetectedUnit.Id != detectedUnit.Id)
						{
							continue;
						}
						if (u.HasEnoughFuelToReachTarget(detectedUnit.Position.Coordinate, true))
						{
							return u.EngageDetectedUnit(detectedUnit, GameConstants.EngagementOrderType.CloseAndEngage, true);
						}
					}
				}
			}
			
			if (launchAircraftIfNecessary)
			{
				//TODO: Launch air to close and engage
			}
			return false;
		}

		public virtual void SetThreatClassification(DetectedUnit detectedUnit)
		{
			//if (detectedUnit.ThreatClassification != GameConstants.ThreatClassification.U_Undecided)
			//{
			//    return;
			//}
			if (detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Missile
				|| detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Torpedo)
			{
				detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
			}
			else
			{
				if (!detectedUnit.IsIdentified)
				{
					if (detectedUnit.IsKnownToSupportRole(GameConstants.Role.IsSurfaceCombattant))
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.C_PotentOnly;
						double rangeNearestAirM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsAircraft });
						if (rangeNearestAirM <= GameConstants.DEFAULT_ASu_ATTACK_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
					}
					else if (detectedUnit.IsKnownToSupportRole(GameConstants.Role.IsAircraft))
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.C_PotentOnly;
						double rangeNearestAirM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsAircraft });
						if (rangeNearestAirM <= GameConstants.DEFAULT_ASu_ATTACK_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
						double rangeNearestSurfaceM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsSurfaceCombattant });
						double rangeNearestAirportM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.LaunchFixedWingAircraft });
						if (rangeNearestAirportM < GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M
							|| rangeNearestSurfaceM <= GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
					}

				}
				else //identified and referstounit
				{
					if (detectedUnit.FriendOrFoeClassification != GameConstants.FriendOrFoe.Foe)
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.Z_NoThreat;
						return;
					}
					if (detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Torpedo
						|| detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Missile
						|| detectedUnit.DetectionClassification == GameConstants.DetectionClassification.Mine)
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
					} 
					else if(detectedUnit.IsKnownToSupportRole(GameConstants.Role.IsAircraft))
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.C_PotentOnly;
						double rangeNearestAirM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsAircraft });
						if (rangeNearestAirM <= GameConstants.DEFAULT_AA_ATTACK_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
						double rangeNearestSurfaceM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsSurfaceCombattant });
						double rangeNearestAirportM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.LaunchFixedWingAircraft });
						if (rangeNearestAirportM < GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M
							|| rangeNearestSurfaceM <= GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
					}
					else if (detectedUnit.IsKnownToSupportRole(GameConstants.Role.IsSubmarine))
					{
						detectedUnit.ThreatClassification = GameConstants.ThreatClassification.C_PotentOnly;
						double rangeNearestSurfaceM = GetDistanceNearestOwnTarget(detectedUnit.Position.Coordinate,
							new List<GameConstants.Role>() { GameConstants.Role.IsSurfaceCombattant });
						if (rangeNearestSurfaceM < GameConstants.DEFAULT_ASu_ATTACK_RANGE_M)
						{
							detectedUnit.ThreatClassification = GameConstants.ThreatClassification.A_PotentAndImmediate;
						}
					}
				}
			}
		}

		public virtual double GetDistanceNearestOwnTarget(Coordinate coordinate, List<GameConstants.Role> roleList)
		{
            var unit = OwnerPlayer.FindNearestAvailableUnitRole(coordinate, roleList, string.Empty, false);
			if (unit != null)
			{
				double distanceM = MapHelper.CalculateDistanceM(coordinate, unit.Position.Coordinate);
				return distanceM;
			}
			else
			{
				return double.MaxValue;
			}
		}

		public virtual void EngageDetectedAirTargets()
		{ 
			foreach (var detItem in OwnerPlayer.DetectedUnits)
			{
				if (detItem.CanBeTargeted 
					&& detItem.DomainType == GameConstants.DomainType.Air
					&& detItem.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe &&
					!detItem.IsMarkedForDeletion)
				{
					this.EngageAirThreat(detItem, false, false);
				}
			}
		}

		/// <summary>
		/// Will engage a high value target, if any is detected. A high value target is determined based on victory conditions.
		/// </summary>
		public void EngageHighValueTargets()
		{
			if (PrioritizedTargetUnitClassIds.Count == 0)
			{
				return;
			}
			var knownTargetList = OwnerPlayer.DetectedUnits.Where(
                det => det.IsIdentified && !det.IsMarkedForDeletion && det.RefersToUnit != null && 
                    PrioritizedTargetUnitClassIds.Contains(det.RefersToUnit.UnitClass.Id)).ToList();
		    EngageHighValueTargets(knownTargetList);
		}

		public void EngageHighValueTargets(List<DetectedUnit> knownTargetList)
		{
			if (knownTargetList.Count < 1)
			{
				return;
			}

            //multiple high value targets: prioritize or pick at random
		    DetectedUnit det = GetHighestPriorityTarget(knownTargetList, 25);
		    EngageHighValueTargets(det);
		}

		/// <summary>
		/// Returns the highest priority target from a list of DetectedUnit. A certain percentage of
		/// calls (supplied in second parameter), it will simply return a random list member. This to ensure
		/// that attacks are somewhat unpredictable.
		/// </summary>
		/// <param name="knownTargetList">List of DetectedUnit.</param>
		/// <param name="chooseRandomPercent">The percentage chance this method will return random list member. If 0, a prioritized member
		/// will always be returned.</param>
		/// <returns></returns>
		public DetectedUnit GetHighestPriorityTarget(List<DetectedUnit> knownTargetList, int chooseRandomPercent)
		{
			if (knownTargetList == null || knownTargetList.Count == 0)
			{
				return null;
			}
			if (!OwnerPlayer.DetectedUnits.Any(u => !u.IsMarkedForDeletion))
			{
				return null;
			}
			if (knownTargetList.Count == 1)
			{
				return knownTargetList[0];
			}
			if (GameManager.Instance.ThrowDice(chooseRandomPercent))
			{
				Random rnd = new Random();
				int i = rnd.Next(knownTargetList.Count - 1);
				return knownTargetList[i];
			}
			else 
			{
				SetPriorityScoresOnDetectedUnits();
				var maxScore = OwnerPlayer.DetectedUnits.Max<DetectedUnit>(d => d.ValueScore);
				return OwnerPlayer.DetectedUnits.FirstOrDefault<DetectedUnit>(d => d.ValueScore >= maxScore);
			}
		}

		public DetectedUnit GetHighestPriorityTarget(GameConstants.DomainType domainType)
		{
			var allDetectionsDomain = OwnerPlayer.DetectedUnits.Where<DetectedUnit>(d => !d.IsMarkedForDeletion && d.IsIdentified && d.DomainType == domainType);
			if (!allDetectionsDomain.Any())
			{
				return null;
			}
			SetPriorityScoresOnDetectedUnits();
			var maxScore = allDetectionsDomain.Max<DetectedUnit>(d => d.ValueScore);
			return allDetectionsDomain.FirstOrDefault<DetectedUnit>(d => d.ValueScore >= maxScore);
		}

        private void SetPriorityScoresOnDetectedUnit( DetectedUnit det, bool updateDistanceScores )
        {
            // Ignore detections marked for deletion
            if (det.IsMarkedForDeletion)
            {
                det.ThreatValueScore = 0;
                det.DistanceValueScore = 0;
                //SetDistanceScoresOnDetectedUnits();
                return;
            }

            if ( det.IsIdentified && det.RefersToUnit != null && PrioritizedTargetUnitClassIds.Contains( det.RefersToUnit.UnitClass.Id ) )
            {
                det.ThreatValueScore = 100;
            }
            else
            {
                det.ThreatValueScore = 0;
            }

            //Distance to nearest own valuable unit
            //Offensive power
            //var distanceOwnUnitM = GetDistanceNearestOwnTarget(det.Position);
            //For now, use ThreatClassification
            switch ( det.ThreatClassification )
            {
                case GameConstants.ThreatClassification.U_Undecided:
                    break;
                case GameConstants.ThreatClassification.A_PotentAndImmediate:
                    det.ThreatValueScore += 20;
                    break;
                case GameConstants.ThreatClassification.B_ImmediateOnly:
                    det.ThreatValueScore += 5;
                    break;
                case GameConstants.ThreatClassification.C_PotentOnly:
                    det.ThreatValueScore += 10;
                    break;
                case GameConstants.ThreatClassification.D_NeitherPotentNorImmediate:
                    break;
                case GameConstants.ThreatClassification.Z_NoThreat:
                    break;
                default:
                    break;
            }

            //Based on the target unit type, set up rolelist to find nearest own unit fulfilling specified role.
            //Then, find the nearest own unit
            var roleList = new List<GameConstants.Role>();
            var roleList2 = new List<GameConstants.Role>();

            switch ( det.RefersToUnit.UnitClass.UnitType )
            {
                case GameConstants.UnitType.SurfaceShip:
                    roleList.Add( GameConstants.Role.IsSurfaceCombattant );
                    if (det.IsKnownToSupportRole(GameConstants.Role.AttackAir))
                    {
                        roleList2.Add(GameConstants.Role.IsAircraft);

                    }
                    else
                    {
                        roleList2.Add(GameConstants.Role.TransportSupplies);
                    }
                    break;
                case GameConstants.UnitType.FixedwingAircraft:
                case GameConstants.UnitType.Helicopter:
                    if ( det.IsKnownToSupportRole( GameConstants.Role.AttackSurface ) )
                    {
                        roleList.Add( GameConstants.Role.IsSurfaceCombattant );
                    }
                    else if ( det.IsKnownToSupportRole( GameConstants.Role.AttackLand ) )
                    {
                        roleList.Add( GameConstants.Role.LaunchFixedWingAircraft );
                    }
                    else if ( det.IsKnownToSupportRole( GameConstants.Role.ASW ) )
                    {
                        roleList.Add( GameConstants.Role.IsSubmarine );
                    }
                    else if ( det.IsKnownToSupportRole( GameConstants.Role.AttackAir ) )
                    {
                        roleList.Add( GameConstants.Role.IsFixedWingAircraft );
                        roleList2.Add( GameConstants.Role.IsRotaryWingAircraft );
                    }
                    break;
                case GameConstants.UnitType.Submarine:
                    roleList.Add( GameConstants.Role.IsSurfaceCombattant );
                    roleList2.Add( GameConstants.Role.IsSubmarine );
                    break;
                case GameConstants.UnitType.Missile:
                    break;
                case GameConstants.UnitType.Torpedo:
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
                    roleList.Add( GameConstants.Role.IsAircraft );
                    roleList2.Add( GameConstants.Role.IsSurfaceCombattant );
                    break;
                default:
                    roleList.Add( GameConstants.Role.LaunchFixedWingAircraft );
                    break;
            }

            det.DistanceToValuableTargetM = 0;

            if ( roleList.Count > 0 )
            {
                var unit = OwnerPlayer.FindNearestAvailableUnitRole(det.Position.Coordinate, roleList, string.Empty, false);
                if (unit != null && unit.Position != null)
                {
                    det.DistanceToValuableTargetM = MapHelper.CalculateDistanceApproxM(unit.Position.Coordinate, det.Position.Coordinate);
                }
                if (roleList2.Count > 0)
                {
                    unit = OwnerPlayer.FindNearestAvailableUnitRole(det.Position.Coordinate, roleList2, string.Empty, false);
                    if ( unit != null && unit.Position != null )
                    {
                        double distance = MapHelper.CalculateDistanceApproxM(unit.Position.Coordinate, det.Position.Coordinate);
                        if (det.DistanceToValuableTargetM == 0 || distance < det.DistanceToValuableTargetM)
                        {
                            det.DistanceToValuableTargetM = distance;
                        }
                    }
                }
            }

            // Update distance on all units now that this one changed
            if ( updateDistanceScores )
                SetDistanceScoresOnDetectedUnits();
        }

        private void SetDistanceScoresOnDetectedUnits()
        {
            try
            {
                if (OwnerPlayer.DetectedUnits.Count < 1)
                {
                    return;
                }
                double minDistanceToValuableTargetM = 0;
                try
                {
                    minDistanceToValuableTargetM =
                        (from det in OwnerPlayer.DetectedUnits
                         where det.DistanceToValuableTargetM > 0
                         select det.DistanceToValuableTargetM).Min(); //Note: this will throw if no detectedunits have DistanceToValuableTargetM > 0

                }
                catch (Exception)
                {
                    //ignore, not a problem
                }

                // Update distance value score on detections
                var idx = OwnerPlayer.DetectedUnits.Count - 1;
                do
                {
                    try
                    {
                        var det = OwnerPlayer.DetectedUnits[idx];
                        det.DistanceValueScore = 0;
                        if (!det.IsMarkedForDeletion && det.DistanceToValuableTargetM > 0)
                        {
                            if (det.DistanceToValuableTargetM <= minDistanceToValuableTargetM)
                            {
                                det.DistanceValueScore += 2;
                            }
                            if (det.DistanceToValuableTargetM < GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M)
                            {
                                det.DistanceValueScore += 1;
                            }
                            if (det.DistanceToValuableTargetM < GameConstants.DEFAULT_AIR_AA_STRIKE_RANGE_M)
                            {
                                det.DistanceValueScore += 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        idx = -1;
                        GameManager.Instance.Log.LogError("BaseAiHandler->SetDistanceScoresOnDetectedUnits failed in loop. " + ex.Message);
                    }
                    idx--;
                } while (idx >= 0);     

            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("BaseAiHandler->SetDistanceScoresOnDetectedUnits failed. " + ex.Message);
            }
        }

	    public void SetPriorityScoresOnDetectedUnits()
		{
			//actually choose target based on sensible criteria
            if ( OwnerPlayer.DetectedUnits.Count < 1 )
            {
                return;
            }

			foreach (var det in OwnerPlayer.DetectedUnits)
			{
			    SetPriorityScoresOnDetectedUnit(det, false);
			}

            SetDistanceScoresOnDetectedUnits();
		}

		public void EngageHighValueTargets(DetectedUnit target)
		{
			if (target == null || target.IsMarkedForDeletion)
			{
				return;
			}
		    var hlo = new HighLevelOrder {TargetUnitId = target.Id};

		    switch (target.DomainType)
			{
				case GameConstants.DomainType.Surface:
					hlo.HighLeverOrderType = GameConstants.HighLevelOrderType.EngageSurfaceTargets;
					OwnerPlayer.HighLevelOrders.Enqueue(hlo);
					break;
				case GameConstants.DomainType.Air:
					EngageAirThreat(target, true, true);
					break;
				case GameConstants.DomainType.Subsea:
					EngageSubseaThreat(target, true, true);
					break;
				case GameConstants.DomainType.Land:
					hlo.HighLeverOrderType = GameConstants.HighLevelOrderType.EngageLandStructures;
					OwnerPlayer.HighLevelOrders.Enqueue(hlo);
					break;
				case GameConstants.DomainType.Unknown:
					break;
				default:
					break;
			}
		}

		public void EngageLandStructures(DetectedUnit target)
		{
		    HighLevelOrder hlo = new HighLevelOrder
		                             {HighLeverOrderType = GameConstants.HighLevelOrderType.EngageLandStructures};
		    var targetHitpoints = GameManager.GetDefaultHitpoints(GameConstants.UnitType.LandInstallation);
			if (target.IsIdentified && target.RefersToUnit != null)
			{
				targetHitpoints = target.RefersToUnit.HitPoints;
			}

			//hlo.CountDesired = 0;
			hlo.TargetUnitId = target.Id;
			EngageSurfaceOrLandTargets(hlo);
		}

        /// <summary>
        /// Executes a HighLevelOrder to engage a surface or land target with all available assets. 
        /// Is meant for high value targets, as it throws a lot of coordinated resources into the attack.
        /// Will also attempt to defend the involved assets with air cover (CAP).
        /// </summary>
        /// <param name="order"></param>
		public void EngageSurfaceOrLandTargets(HighLevelOrder order)
		{
			DetectedUnit target;
			if (!string.IsNullOrEmpty(order.TargetUnitId))
			{
				target = OwnerPlayer.GetDetectedUnitById(order.TargetUnitId);
			}
			else //no target specified; find one
			{
                if (order.HighLeverOrderType == GameConstants.HighLevelOrderType.EngageSurfaceTargets)
                {
                    target = GetHighestPriorityTarget(GameConstants.DomainType.Surface);
                }
                else
                {
                    target = GetHighestPriorityTarget(GameConstants.DomainType.Land);
                }
			}
			if (target == null)
			{
				return;
			}
            if (target.IsKnownToBeCivilianUnit)
            {
                return;
            }
            if (target.DomainType != GameConstants.DomainType.Surface && target.DomainType != GameConstants.DomainType.Land)
            {
                GameManager.Instance.Log.LogError("EngageSurfaceOrLandTargets called with wrong target type " + target);
                return;
            }
            var targetGroup = target.GetDetectedGroup();
            var noOfUnitsInGroup = 1;
            if (targetGroup != null)
            {
                noOfUnitsInGroup = targetGroup.DetectedUnits.Count;
            }
            var allEnemyUnitsInTargetArea = OwnerPlayer.GetDetectedUnitsInArea(target.Position.Coordinate, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M, true);
            var allSurfaceCombattantsInTargetArea = allEnemyUnitsInTargetArea.Where<DetectedUnit>(u
                => u.IsKnownToSupportRole(GameConstants.Role.IsSurfaceCombattant));
            var aircraftInArea = allEnemyUnitsInTargetArea.Where<DetectedUnit>(u => u.DomainType == GameConstants.DomainType.Air);
            var knownFightersInArea = allEnemyUnitsInTargetArea.Where<DetectedUnit>(u 
                => u.DomainType == GameConstants.DomainType.Air && u.IsKnownToSupportRole(GameConstants.Role.AttackAir));
            var defenceFactor = allSurfaceCombattantsInTargetArea.Count<DetectedUnit>() + knownFightersInArea.Count<DetectedUnit>();

            //First, surface and subs should engage target if in range
            var hasSurfaceAttacked = EngageSurfaceThreat(target, false, false);
            
            //Second, aircraft already in the air should engage it
            var rolesListAir = new List<GameConstants.Role>();
            rolesListAir.Add(GameConstants.Role.IsAircraft); 
            if (order.HighLeverOrderType == GameConstants.HighLevelOrderType.EngageSurfaceTargets)
            {
                rolesListAir.Add(GameConstants.Role.AttackSurface);
            }
            else
            {
                rolesListAir.Add(GameConstants.Role.AttackLand);
            }

            var attackingAir = EngageDetectedTargetWithLaunchedAir(target, 
                rolesListAir, allSurfaceCombattantsInTargetArea.Count() + 1);
            var attackFactor = attackingAir.Count;
            if (hasSurfaceAttacked)
            {
                attackFactor += 2;
            }
            var desiredAirDefenceNumber = 1;

            //Then, launch own fighters to take care of any enemy fighters
            desiredAirDefenceNumber += knownFightersInArea.Count();

            var airDefencePos = target.Position.Clone();
            BaseUnit unit = null; //GetClosestAIHint requires some unit, so here goes...
            if (attackingAir.Count > 0)
            {
                unit = attackingAir.FirstOrDefault<BaseUnit>();
            }
            else
            {
                var myUnits = OwnerPlayer.GetUnitsInAreaByRole(GameConstants.Role.AttackSurface, 
                    target.Position.Coordinate, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M * 10.0);
                if (myUnits.Count > 0)
                {
                    unit = myUnits.FirstOrDefault<BaseUnit>();
                }
            }
            
            if (unit != null)
            {
                var threatAxisAir = GetClosestAIHint(unit, GameConstants.AIHintType.ThreatAxisAir, target.Position.Coordinate, true);
                var bearingDeg = threatAxisAir.DirectionDeg;
                if (bearingDeg <= 180)
                {
                    bearingDeg += 180;
                }
                else
                {
                    bearingDeg -= 180;
                }
                airDefencePos = new Position(MapHelper.CalculateNewPositionApprox(
                    airDefencePos.Coordinate, bearingDeg, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M));
            }
            
            GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAewPatrol, desiredAirDefenceNumber, 
                airDefencePos.Coordinate, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M);
            
            //For land attack: also attack nearby defensive land structures if they exist
            if (target.DomainType == GameConstants.DomainType.Land)
            {
                var enemyDefenceStructures = OwnerPlayer.GetDetectedUnitsInArea(target.Position.Coordinate, 
                    GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M, true);
                enemyDefenceStructures = enemyDefenceStructures.Where<DetectedUnit>(
                    d => d.DomainType == GameConstants.DomainType.Land
                    && d.IsKnownToSupportRole(GameConstants.Role.AttackAir)).ToList<DetectedUnit>();
                if (enemyDefenceStructures.Any())
                {
                    foreach (var struc in enemyDefenceStructures)
                    {
                        var attackLandOrder = new EngagementOrder(struc, 
                            GameConstants.EngagementOrderType.CloseAndEngage, 
                            GameConstants.EngagementStrength.OverkillAttack);

                        var launchedNo = LaunchAircraft(struc.Position.Coordinate, rolesListAir, 
                            string.Empty, 2, 1, true, 
                            new List<BaseOrder> { attackLandOrder }, string.Empty);
                    }
                }
            }

            //Third, launch air to attack original target if necessary
            if (attackFactor < defenceFactor)
            {
                var noOfAircraft = defenceFactor-attackFactor + 1;
                //var launchAttackList = EngageDetectedTargetWithLaunchedAir(target, rolesListAir, noOfAircraft);
                var attackLandOrder = new EngagementOrder(target,
                    GameConstants.EngagementOrderType.CloseAndEngage,
                    GameConstants.EngagementStrength.OverkillAttack);

                var launchedNo = LaunchAircraft(target.Position.Coordinate, rolesListAir,
                    string.Empty, noOfAircraft, 1, true,
                    new List<BaseOrder> { attackLandOrder }, string.Empty);
            }
		}

        /// <summary>
        /// Goes through all deployed units to see if it can be better utilized.
        /// </summary>
        public void ActivateIdleUnits()
        {
            var units = (from u in OwnerPlayer.Units
                        where u.Position != null && (u.IsGroupMainUnit() || !u.IsInGroupWithOthers()) && u.MissionType == GameConstants.MissionType.Patrol
                        select u).ToList<BaseUnit>();
            for (int i = 0; i < units.Count<BaseUnit>(); i++)
            {
                try
                {
                    var unit = units[i];
                    ActivateIdleUnits(unit);
                }
                catch (Exception ex)
                {
                    GameManager.Instance.Log.LogError("ActivateIdleUnits error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Tries to find the optimal activity for the specific idle unit
        /// </summary>
        /// <param name="unit"></param>
        public void ActivateIdleUnits(BaseUnit unit)
        {
            if (unit == null || unit.IsMarkedForDeletion || unit.Position == null || unit.MissionType != GameConstants.MissionType.Patrol)
            {
                return;
            }
            //first, check if it is already doing the right thing.
            DetectedUnit threat = unit.GetThreateningDetectedUnit();
            if (threat != null) //respond to threat
            {
                unit.RespondToImminentThreat(threat);
            }
            else if(OwnerPlayer.IsCompetitivePlayer)
            {
                //here is definitely something sensible to do with targets of opportunity etc...
                //Aircraft: 
                //Out of ammo: just return to base
                //Engage close targets
                //Engage faraway targets
                //Flying away from important areas: move back to protect or patrol areas

                //Land installations:
                //Fire at enemies in range

                //Any movable unit:
                //Consider turning off active sensors (if no threat nearby)

                switch (unit.DomainType)
                {
                    case GameConstants.DomainType.Land:
                    case GameConstants.DomainType.Surface:
                        if (unit.SupportsRole(GameConstants.Role.AttackAir))
                        {
                            var rangeM = unit.GetMaxWeaponRangeM(GameConstants.DomainType.Air);
                            var enemies = OwnerPlayer.GetDetectedUnitsInArea(unit.Position.Coordinate, rangeM, true);
                            if (unit.WeaponOrders == GameConstants.WeaponOrders.FireOnAllClearedTargets)
                            {
                                foreach (var target in enemies)
                                {
                                    if (target.DomainType == GameConstants.DomainType.Air && !target.IsFiredUpon)
                                    {
                                        if (unit.CanImmediatelyFireOnTargetType(target))
                                        {
                                            if (string.IsNullOrEmpty(target.DetectedGroupId)) //if not in a group
                                            {
                                                unit.EngageDetectedUnit(target, GameConstants.EngagementOrderType.EngageNotClose, unit.IsInGroupWithOthers());
                                                return;
                                            }
                                            else //if unit is part of a group, target it
                                            {
                                                var targetGroup = target.GetDetectedGroup();
                                                if (targetGroup != null)
                                                {
                                                    unit.EngageDetectedGroup(targetGroup, GameConstants.EngagementOrderType.EngageNotClose,
                                                        GameConstants.EngagementStrength.DefaultAttack, unit.IsInGroupWithOthers());
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }//foreach
                            }
                            else if (unit.WeaponOrders == GameConstants.WeaponOrders.FireInSelfDefenceOnly)
                            {
                                foreach (var target in enemies)
                                {
                                    if (!target.IsTargetted && target.IsKnownToTargetUnit(unit,true))
                                    {
                                        if (unit.CanImmediatelyFireOnTargetType(target))
                                        {
                                            if (string.IsNullOrEmpty(target.DetectedGroupId)) //if not in a group
                                            {
                                                unit.EngageDetectedUnit(target, GameConstants.EngagementOrderType.EngageNotClose, unit.IsInGroupWithOthers());
                                                return;
                                            }
                                            else //if unit is part of a group, target it
                                            {
                                                var targetGroup = target.GetDetectedGroup();
                                                if (targetGroup != null)
                                                {
                                                    unit.EngageDetectedGroup(targetGroup, GameConstants.EngagementOrderType.EngageNotClose,
                                                        GameConstants.EngagementStrength.DefaultAttack, unit.IsInGroupWithOthers());
                                                    return;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        
                        //attack subs in range
                        if (unit.DomainType == GameConstants.DomainType.Surface && OwnerPlayer.IsComputerPlayer && unit.SupportsRole(GameConstants.Role.AttackSubmarine))
                        {
                            var rangeM = unit.GetMaxWeaponRangeM(GameConstants.DomainType.Subsea);
                            var enemies = OwnerPlayer.GetDetectedUnitsInArea(unit.Position.Coordinate, rangeM, true);
                            var subs = from e in enemies 
                                       where e.DomainType == GameConstants.DomainType.Subsea 
                                       select e;
                            foreach (var sub in subs)
                            {
                                if (unit.CanImmediatelyFireOnTargetType(sub) && !sub.IsFiredUpon)
                                {
                                    unit.EngageDetectedUnit(sub, GameConstants.EngagementOrderType.EngageNotClose, unit.IsInGroupWithOthers());
                                    return;
                                }
                            }

                        }
                        break;
                    case GameConstants.DomainType.Air:
                        if (OwnerPlayer.IsComputerPlayer)
                        {
                            if (unit.IsPrimaryWeaponsOutOfAmmo())
                            {
                                unit.ReturnToBase();
                                return;
                            }
                            DetectedUnit detTarget = GetPrioritizedTargetOfOpportunity(unit);
                            if (detTarget != null)
                            {
                                if (!detTarget.IsTargetted)
                                {
                                    unit.EngageDetectedUnit(detTarget, GameConstants.EngagementOrderType.CloseAndEngage, unit.IsInGroupWithOthers());
                                    return;
                                }
                                //otherwise, only engage if not a sufficient number of other units are busy engaging it
                                var desiredTargetedCount = 1;
                                if (detTarget.ThreatValueScore >= 100) //victory condition
                                {
                                    desiredTargetedCount += 1;
                                }
                                if (detTarget.DomainType == GameConstants.DomainType.Land)
                                {
                                    desiredTargetedCount += 10; //arbitrary
                                }
                                if (detTarget.DomainType == GameConstants.DomainType.Surface)
                                {
                                    desiredTargetedCount += 10;
                                }
                                var currentTargetedCount = detTarget.TargettingList.Count;
                                if (desiredTargetedCount < currentTargetedCount)
                                {
                                    unit.EngageDetectedUnit(detTarget, GameConstants.EngagementOrderType.CloseAndEngage, unit.IsInGroupWithOthers());
                                    return;
                                }
                            }
                            //TODO: make sure air units avoid danger

                        }
                        break;
                    case GameConstants.DomainType.Subsea:
                        if (OwnerPlayer.IsComputerPlayer)
                        {
                            if (SubEngageTargetsInTorpedoRange(unit))
                            {
                                return;
                            }
                            var enemy = GetPrioritizedTargetOfOpportunity(unit);
                            if (enemy != null)
                            {
                                if (unit.CanImmediatelyFireOnTargetType(enemy))
                                {
                                    unit.EngageDetectedUnit(enemy, GameConstants.EngagementOrderType.EngageNotClose, unit.IsInGroupWithOthers());
                                    return;
                                }
                                else if (enemy.DomainType != GameConstants.DomainType.Air && unit.CanTargetDetectedUnit(enemy, false)) //otherwise, if enemy target is reasonably close, engage it
                                {
                                    var distM = MapHelper.CalculateDistanceApproxM(unit.Position.Coordinate, enemy.Position.Coordinate);
                                    if (distM < GameConstants.DEFAULT_ASu_ATTACK_RANGE_M)
                                    {
                                        unit.EngageDetectedUnit(enemy, GameConstants.EngagementOrderType.CloseAndEngage, unit.IsInGroupWithOthers());
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// For a submarine, checks all potential targets within torpedo range and fires at all of them. Returns true if
        /// any targets engaged.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool SubEngageTargetsInTorpedoRange(BaseUnit unit)
        {
            if (unit.DomainType != GameConstants.DomainType.Subsea || unit.Position == null || unit.IsMarkedForDeletion)
            {
                return false;
            }
            var torpWpns = unit.Weapons.Where<BaseWeapon>(w => w.WeaponClass.SpawnsUnitOnFire && w.WeaponClass.MaxSpeedKph <= 500 && w.AmmunitionRemaining > 0);
            if (!torpWpns.Any())
            {
                return false;
            }
            var rangeM = torpWpns.Max<BaseWeapon>(r => r.WeaponClass.EffectiveWeaponRangeM);
            var targets = OwnerPlayer.GetDetectedUnitsInArea(unit.Position.Coordinate, rangeM, true);
            if (!targets.Any())
            {
                return false;
            }
            GameManager.Instance.Log.LogDebug("SubEngageTargetsInTorpedoRange: " + unit + " ordered to immediately fire on " + targets.Count() + " units.");
            var priTargets = from t in targets
                             orderby t.ValueScore descending
                             select t;
            bool hasFired = false;
            foreach (var target in priTargets)
            {
                if (target.CanBeTargeted && target.DomainType == GameConstants.DomainType.Surface && target.DomainType == GameConstants.DomainType.Subsea)
                {
                    if (UnitImmediatelyEngageWithWeapons(unit, torpWpns, target))
                    {
                        hasFired = true;
                    }
                }
            }
            return hasFired;
        }

        /// <summary>
        /// For the specified unit, will use one of the listed weapons and immediately fire on target if possible. Returns true if 
        /// weapons are fired. Will not fire at targets which already has incoming torpedoes in the water
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="selectedWeapons"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool UnitImmediatelyEngageWithWeapons(BaseUnit unit, IEnumerable<BaseWeapon> selectedWeapons, DetectedUnit target)
        {
            if (unit == null || unit.IsMarkedForDeletion || unit.Position == null)
            {
                return false;
            }
            if (target == null || target.IsMarkedForDeletion || target.Position == null || !target.CanBeTargeted)
            {
                return false;
            }

            
            if (CountTorpedoesTargetingDetectedUnit(target) > 0)
            {
                return false;
            }
            var distanceM = MapHelper.CalculateDistance3DM(unit.Position, target.Position);
            foreach (var wpn in selectedWeapons)
            {
                if (wpn.CheckIfWeaponCanFireNow(target, null, distanceM))
                {
                    if (wpn.CanTargetDetectedUnit(target, false))
                    {
                        var roundCount = wpn.GetRoundToFireCount(target, GameConstants.EngagementStrength.DefaultAttack);
                        if (wpn.Fire(target, roundCount, distanceM) > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public int CountTorpedoesTargetingDetectedUnit(DetectedUnit target)
        {
            var torps = from u in OwnerPlayer.Units
                        where u.UnitClass.UnitType == GameConstants.UnitType.Torpedo
                        && u.TargetDetectedUnit != null && u.TargetDetectedUnit.Id == target.Id
                        select u;
            var count = torps.Count();
            GameManager.Instance.Log.LogDebug("CountTorpedoesTargetingDetectedUnit returns " + count + " for target " + target + ".");
            return count;
        }


        /// <summary>
        /// This method returns one (or null) DetectedUnit which presents itself as a target of opportunity (or self defence).
        /// No orders are issued in this method.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public DetectedUnit GetPrioritizedTargetOfOpportunity(BaseUnit unit)
        {
            var rangeM = GameConstants.DEFAULT_ASu_ATTACK_RANGE_M;
            if (unit.DomainType == GameConstants.DomainType.Air)
            {
                rangeM = unit.FuelDistanceRemainingM;
            }
            var enemies = OwnerPlayer.GetDetectedUnitsInArea(unit.Position.Coordinate, rangeM, true);
            foreach (var enemy in enemies)
            {
                if (unit.DomainType == GameConstants.DomainType.Air)
                {
                    if (unit.CanTargetDetectedUnit(enemy, false) && unit.HasEnoughFuelToReachTarget(enemy.Position.Coordinate, true))
                    {
                        return enemy;
                    }
                }
                else
                {
                    if (unit.CanImmediatelyFireOnTargetType(enemy))
                    {
                        return enemy;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Called for new and updated detections, to make sure target (if any) responds by evasion or aggression as appropriate
        /// </summary>
        /// <param name="detectedUnit"></param>
        public void TargetRespondToDetectedUnit(DetectedUnit detectedUnit)
        {
            if (detectedUnit.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe)
            {
                if (detectedUnit.RefersToUnit != null && detectedUnit.RefersToUnit.TargetDetectedUnit != null)
                {
                    var target = detectedUnit.RefersToUnit.TargetDetectedUnit.RefersToUnit;
                    if (target != null && target.OwnerPlayer.Id == OwnerPlayer.Id) //it is targetting one of my units!
                    {
                        target.AddTargetingDetectedUnit(detectedUnit);
                        target.RespondToImminentThreat(detectedUnit);
                    }
                }
            }
        }

		public void DefendHighValueUnits()
		{
			var listUnits = new List<BaseUnit>();
            LaunchTankerAircraft();
			if (PrioritizedDefenceOwnUnitClassIds.Count == 0)
			{
				//add surface groups and airports since info is absent    
				foreach (var unit in OwnerPlayer.Units)
				{
					if (unit.UnitClass.UnitType == GameConstants.UnitType.LandInstallation && unit.SupportsRole(GameConstants.Role.LaunchFixedWingAircraft))
					{
						listUnits.Add(unit);
					}
					else if ((!unit.IsInGroupWithOthers() || unit.IsGroupMainUnit()) && unit.SupportsRole(GameConstants.Role.IsSurfaceCombattant))
                    {
                        listUnits.Add(unit);
                    }
				}
			}
			else
			{
				foreach (var unit in OwnerPlayer.Units)
				{
					if (PrioritizedDefenceOwnUnitClassIds.Contains(unit.UnitClass.Id))
					{
						if (listUnits.FirstOrDefault(u => !string.IsNullOrEmpty(unit.GroupId) && u.GroupId == unit.GroupId) != null)
						{
							//group member already in list, no need to replicate
						}
						else
						{ 
							listUnits.Add(unit);
						}
					}
				}
			}
			foreach (var unit in listUnits)
			{
				DefendHighValueUnits(unit);
			}
		}

        /// <summary>
        /// If the scenario does not specify any tanker aircraft, and none are in the air automatically, launch tanker aircraft in "safe" locations.
        /// </summary>
        public void LaunchTankerAircraft()
        {
            try
            {
                var hlos = from h in OwnerPlayer.HighLevelOrders
                           where h.HighLeverOrderType == GameConstants.HighLevelOrderType.LaunchAircraft
                           && h.RolesList.Contains(GameConstants.Role.RefuelAircaft)
                           select h;
                if (hlos.Any<HighLevelOrder>())
                {
                    return; //HLOs to launch refuelling tankers already exist
                }
                var taskforces = from u in OwnerPlayer.Units
                                 where u.Position != null && u.SupportsRole(GameConstants.Role.IsSurfaceCombattant)
                                    && u.Group != null && u.Group.Units.Count > 2
                                 select u;
                if (taskforces.Any<BaseUnit>())
                {
                    var pos = taskforces.FirstOrDefault<BaseUnit>().Position.GetPositionInfo();
                    CreateLaunchTankerHlo(pos);
                }
                var airfields = from u in OwnerPlayer.Units
                                where u.Position != null
                                && u.SupportsRole(GameConstants.Role.LaunchFixedWingAircraft)
                                && u.AircraftHangar != null && u.AircraftHangar.Aircraft.Count > 1
                                select u;
                if (airfields.Any<BaseUnit>())
                {
                    var pos = airfields.FirstOrDefault<BaseUnit>().Position.GetPositionInfo();
                    CreateLaunchTankerHlo(pos);
                    return;
                }
                var landInstallations = from u in OwnerPlayer.Units
                                        where u.UnitClass.IsLandbased && u.SupportsRole(GameConstants.Role.AttackAir)
                                        select u;
                if (landInstallations.Any<BaseUnit>())
                {
                    var pos = landInstallations.FirstOrDefault<BaseUnit>().Position.GetPositionInfo();
                    CreateLaunchTankerHlo(pos);
                }

            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("BaseAIHandler->LaunchTankerAircraft fails. " + ex.Message);
            }
        }

        private void CreateLaunchTankerHlo(PositionInfo pos)
        {
            var hlo = new HighLevelOrder(GameConstants.HighLevelOrderType.LaunchAircraft);
            hlo.CountDesired = 1;
            hlo.CountMinimum = 1;
            hlo.PositionCenter = pos;
            hlo.DistanceM = 5000;
            hlo.RolesList = new List<GameConstants.Role>() { GameConstants.Role.RefuelAircaft };
            hlo.RecurringCount = 99999;
            hlo.FirstTriggerInSec = 1;
            hlo.TriggerEverySec = 60 * 60;
            OwnerPlayer.HighLevelOrders.AddLast(hlo);
            GameManager.Instance.Log.LogDebug("BaseAIHandler->CreateLaunchTankerHlo creates HLO " + hlo.ToString());
        }

		public void DefendHighValueUnits(BaseUnit unit)
		{
			//TODO: implement defence hv units
			if (unit == null || unit.IsMarkedForDeletion || unit.Position == null)
			{
				return;
			}
			var airThreat = GetClosestAIHint(unit, GameConstants.AIHintType.ThreatAxisAir, unit.Position.Coordinate, true);
			var subThreat = GetClosestAIHint(unit, GameConstants.AIHintType.ThreatAxisSub, unit.Position.Coordinate, true);
			var surfaceThreat = GetClosestAIHint(unit, GameConstants.AIHintType.ThreatAxisSurface, unit.Position.Coordinate, true);
			var coordAirThreatStrike = MapHelper.CalculateNewPosition2(airThreat.Coordinate, airThreat.DirectionDeg, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
			switch (unit.DomainType)
			{
				case GameConstants.DomainType.Surface:
					//one aew towards threat axis, one circling to cover all directions centered on unit
					GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAewPatrol, 1, coordAirThreatStrike, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
					GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAewPatrol, 1, unit.Position.Coordinate, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
					//only asw towards threat axis (which for a moving unit will be forward)
                    var firstWp = unit.GetActiveWaypoint();
                    var aswBearingDeg = subThreat.DirectionDeg;
                    var subThreatCoord = subThreat.Coordinate;
                    if (firstWp != null)
                    {
                        aswBearingDeg = MapHelper.CalculateBearingDegrees(unit.Position.Coordinate, firstWp.Position.Coordinate);
                        subThreatCoord = MapHelper.CalculateNewPosition2(unit.Position.Coordinate, aswBearingDeg, 60000);
                    }
                    var newCoordAsw = MapHelper.CalculateNewPosition2(subThreatCoord, aswBearingDeg, 60000);
                    if (TerrainReader.GetHeightM(newCoordAsw) < 0) //don't do ASW on land!
                    {
                        GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAswPatrol, 1, newCoordAsw, 30000);    
                    }
					break;
				case GameConstants.DomainType.Air:
					break;
				case GameConstants.DomainType.Subsea:
					break;
				case GameConstants.DomainType.Land:
					GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAewPatrol, 1, coordAirThreatStrike, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
					GenerateHloIfNeccessary(GameConstants.HighLevelOrderType.SetAewPatrol, 1, unit.Position.Coordinate, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M);
					break;
				case GameConstants.DomainType.Unknown:
					break;
				default:
					break;
			}
		}

		public HighLevelOrder GenerateHloIfNeccessary(GameConstants.HighLevelOrderType highLevelOrderType, int countDesiredUnits, Coordinate coordinate, double distanceM)
		{
			var oldHlo = FindExistingHighLevelOrder(highLevelOrderType, coordinate, distanceM);
			if (oldHlo != null)
			{
				return oldHlo;
			}
			var hlo = new HighLevelOrder(highLevelOrderType);
			hlo.PositionCenter = new PositionInfo(coordinate.LatitudeDeg, coordinate.LongitudeDeg);
            if (highLevelOrderType == GameConstants.HighLevelOrderType.SetAewPatrol)
            {
                hlo.RolesList = new List<GameConstants.Role>() { GameConstants.Role.InterceptAircraft };
            }
			hlo.DistanceM = distanceM;
			hlo.FirstTriggerInSec = 1;
            hlo.CountDesired = countDesiredUnits;
			hlo.CountMinimum = 1;
			hlo.IsForComputerPlayerOnly = false;
			OwnerPlayer.HighLevelOrders.Enqueue(hlo);
			GameManager.Instance.Log.LogDebug(
				string.Format("GenerateHloifNeccessary adds new hlo {0} for player {1}", hlo, OwnerPlayer));
            return hlo;
		}

		public HighLevelOrder FindExistingHighLevelOrder(GameConstants.HighLevelOrderType highLevelOrderType, Coordinate coordinate, double distanceM)
		{
			var hlo = from h in OwnerPlayer.HighLevelOrders
					  where h.HighLeverOrderType == highLevelOrderType && MapHelper.CalculateDistanceApproxM(coordinate, new Coordinate(h.PositionCenter)) <= distanceM
					  select h;
			if (!hlo.Any())
			{
				return null;
			}
		    return hlo.FirstOrDefault();
		}

		/// <summary>
		/// Attempts to automatically generate an AIhint describing probable threat axis based on unit. Assumes the defence of this
		/// unit is very important.
		/// </summary>
		/// <param name="unit">Unit to be defended</param>
		/// <param name="aiHintType">The hint type to be supplied (air, surface, sub)</param>
		/// <param name="aiHint">If not null, returned as is</param>
		/// <returns>New AI hint. Will be null if unit is null or has no position</returns>
		public AIHint GenerateAiHint(BaseUnit unit,  GameConstants.AIHintType aiHintType, AIHint aiHint)
		{
			if (aiHint != null)
			{
				return aiHint;
			}
			if (unit == null || unit.Position == null || unit.IsMarkedForDeletion)
			{
				return null;
			}
			aiHint = new AIHint();
			aiHint.AIHintType = aiHintType;
			aiHint.Coordinate = unit.Position.Coordinate; 
			aiHint.OwnerId = unit.OwnerPlayer.Id;
			aiHint.RadiusM = GameConstants.DEFAULT_AA_DEFENSE_RANGE_M;
			var enemyBase = OwnerPlayer.DetectedUnits.FirstOrDefault(u => !u.IsMarkedForDeletion && u.IsKnownToSupportRole(GameConstants.Role.LaunchFixedWingAircraft));
			if (enemyBase != null)
			{
				aiHint.DirectionDeg = MapHelper.CalculateBearingDegrees(aiHint.Coordinate, enemyBase.Position.Coordinate);
			}
			else
			{
				if (OwnerPlayer.Country != null && OwnerPlayer.Country.Id == "russia") //TODO: More sensible direction
				{
					aiHint.DirectionDeg = 270;
				}
				else
				{
					aiHint.DirectionDeg = 50;
				}
			}
			
			switch (aiHintType)
			{
					//The threat axis for a unit will be forward for threats moving at roughly the same speed. Ie for
					//moving surface, air threat axis can be any (planes are faster), but sub threat axis is forward. For aircraft,
					//the threat axis will mostly be forward.
				case GameConstants.AIHintType.ThreatAxisAir:
					aiHint.RadiusM = GameConstants.DEFAULT_AA_DEFENSE_RANGE_M;
					if (unit.DomainType == GameConstants.DomainType.Air && unit.DesiredBearingDeg != null) 
					{
						aiHint.DirectionDeg = (double)unit.DesiredBearingDeg;
					}
					break;
				case GameConstants.AIHintType.ThreatAxisSurface:
					aiHint.RadiusM = GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M;
					if (unit.DesiredBearingDeg != null)
					{
						aiHint.DirectionDeg = (double)unit.DesiredBearingDeg;
					}
					break;
				case GameConstants.AIHintType.ThreatAxisSub:
					aiHint.RadiusM = GameConstants.DEFAULT_AIR_ASW_STRIKE_RANGE_M;
					if (unit.DesiredBearingDeg != null)
					{
						aiHint.DirectionDeg = (double)unit.DesiredBearingDeg;
					}
					break;
			}
            //GameManager.Instance.Log.LogDebug(
            //    string.Format("GenerateAiHint returns hint {0} for player {1}", aiHint, OwnerPlayer));
			return aiHint;
		}

		public AIHint GetClosestAIHint(BaseUnit unit, GameConstants.AIHintType aiHintType, Coordinate coordinate, bool generateIfNeccessary)
		{
			var finder = new BlackboardFinder<AIHint>();
			var allAreaEffects = finder.GetAllSortedByCoordinateAndType(coordinate, GameConstants.MAX_DETECTION_DISTANCE_M * 100);
			allAreaEffects = allAreaEffects.Where<IBlackboardObject>(u => u is AIHint && (u as AIHint).AIHintType == aiHintType);
			if (allAreaEffects.Count() == 0)
			{
				if (generateIfNeccessary)
				{
					var newHint = GenerateAiHint(unit, aiHintType, null);
                    //GameManager.Instance.Log.LogDebug(
                    //    string.Format("GetClosestAIHint generates new AIHint {0} for player {1}", newHint, OwnerPlayer));
					return newHint;
				}
				return null;
			}
			
			return (AIHint)allAreaEffects.FirstOrDefault();
		}

		public virtual void EngageDetectedTargets()
		{
			foreach (var detItem in OwnerPlayer.DetectedUnits)
			{
				if (!detItem.IsTargetted && detItem.CanBeTargeted &&
					detItem.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe && 
					!detItem.IsMarkedForDeletion && !detItem.IsKnownToBeCivilianUnit)
				{
                    TargetRespondToDetectedUnit(detItem);
                    EngageDetectedTarget(detItem);
				}
			}
		}

        private void EngageDetectedTarget(DetectedUnit detItem)
        {
            if (detItem.DomainType == GameConstants.DomainType.Air)
            {
                this.EngageAirThreat(detItem, true, true);
            }
            else if (detItem.DomainType == GameConstants.DomainType.Surface)
            {
                if (OwnerPlayer.IsAutomaticallyEngagingOpportunityTargets)
                {
                    this.EngageSurfaceThreat(detItem, true, true);
                }

            }
            else if (detItem.DomainType == GameConstants.DomainType.Subsea)
            {
                this.EngageSubseaThreat(detItem, true, true);
            }
        }

		public virtual bool EngageDetectedGroup(BaseUnit unit, EngagementOrder order)
		{
			return EngageDetectedGroup(unit, order.TargetDetectedGroup, order.EngagementOrderType, 
				order.EngagementStrength, order.WeaponClassId, order.IsGroupAttack);
		}

		/// <summary>
		/// Engages a detectedgroup, potentially with all units in group with the unit.
		/// </summary>
		/// <param name="unit"></param>
		/// <param name="detectedGroup"></param>
		/// <param name="engagementOrderType"></param>
		/// <param name="engagementStrength"></param>
		/// <param name="weaponClassId"></param>
		/// <param name="isGroupAttack"></param>
		/// <returns></returns>
		public virtual bool EngageDetectedGroup(BaseUnit unit, 
			DetectedGroup detectedGroup, 
			GameConstants.EngagementOrderType engagementOrderType,
			GameConstants.EngagementStrength engagementStrength, 
			string weaponClassId, 
			bool isGroupAttack)
		{
			if (detectedGroup == null || detectedGroup.DetectedUnits.Count < 1 || unit == null)
			{
				return false;
			}
			var isAnyTargetEnemy = detectedGroup.DetectedUnits.Any(d => d.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe);
			if (isAnyTargetEnemy)
			{
				foreach (var det in detectedGroup.DetectedUnits)
				{
					det.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe; //if any unit in a group is designated foe, all will be foe
				}
			}
			bool isSuccessfull = false;

			var targetList = from d in detectedGroup.DetectedUnits
                             where !d.IsMarkedForDeletion && d.Position != null
							 orderby 
                                d.ValueScore descending, 
                                MapHelper.CalculateDistanceApproxM(unit.Position.Coordinate, d.Position.Coordinate) ascending
							 select d;
			if (!targetList.Any())
			{
				 return false;
			}
			string attackerName = string.Empty;
			string targetName = detectedGroup.ToString();
			var primaryTarget = targetList.First<DetectedUnit>();
			List<BaseUnit> platformsList = new List<BaseUnit>();
			if (!isGroupAttack || !unit.IsInGroupWithOthers())
			{
				attackerName = unit.ToShortString();
				platformsList.Add(unit);
			}
			else //group attack
			{
			    var group = unit.Group;
				attackerName = group.ToString();
			    platformsList.AddRange(group.Units.Where(u => u.Weapons.Any()).Where(u => u.CanTargetDetectedUnit(primaryTarget, false)));
			    if (platformsList.Count < 1)
				{
					var msg = OwnerPlayer.CreateNewMessage(
						string.Format("{0} cannot engage group {1}", attackerName, targetName));
					msg.Priority = GameConstants.Priority.Urgent;
					msg.Position = unit.Position.Clone();
					return false;
				}
				
			}
			//make a score for each unit in attacking group, to prioritize attackers
			foreach (var u in platformsList)
			{
				u.ValueScore = 0;
			}
			var targetsSortedDistance = from t in targetList 
                                        where !t.IsKnownToBeCivilianUnit
  							            orderby MapHelper.CalculateDistanceApproxM(t.Position.Coordinate, unit.Position.Coordinate) descending
								        select t;
			var closestTarget = targetsSortedDistance.FirstOrDefault<DetectedUnit>();
			foreach (var u in platformsList)
			{
				if (u.CanImmediatelyFireOnTargetType(primaryTarget))
				{
					u.ValueScore += 5;
				}
				if (u.CanImmediatelyFireOnTargetType(closestTarget))
				{
					u.ValueScore += 1;
				}
				var totalHP = u.TotalHitpointsAttackTarget(primaryTarget);
				if (primaryTarget.RefersToUnit != null && totalHP > primaryTarget.RefersToUnit.HitPoints)
				{
					u.ValueScore += 2;
					if (totalHP > primaryTarget.RefersToUnit.HitPoints * 2)
					{
						u.ValueScore += 2;
					}
				}
			}
			
			var platformsSorted = (from u in platformsList
								  orderby u.ValueScore descending
								  select u).ToList<BaseUnit>();

			var idx = 0;
			if (closestTarget != null && closestTarget.IsKnownToSupportRole(GameConstants.Role.AttackAir))
			{
				var success = platformsSorted[0].EngageDetectedUnit(closestTarget, engagementOrderType, weaponClassId, false, true); //First, engage closest target if it can attack air
				if (platformsSorted.Count > 1)
				{ 
					idx = 1;
				}
				if (success)
				{
					isSuccessfull = true;
				}
			}
			
			foreach (var target in targetList)
			{
				var currentPlatform = platformsSorted[idx];
				var success = currentPlatform.EngageDetectedUnit(target, engagementOrderType, weaponClassId, false, true);
				if (success)
				{
					isSuccessfull = true;
				}
				idx++;
				if (idx >= platformsSorted.Count) //roll around to first unit again
				{
					idx = 0;
				}
			}

			return isSuccessfull;
		}

        public void HandleIncomingBattleDamageReport(BattleDamageReport battleDamageReport, 
            bool reAttackAir, bool reAttackSub, bool reAttackSurface, bool reAttackLand, bool launchAirIfNecessary)
        {
            if (battleDamageReport.PlayerInflictingDamageId != OwnerPlayer.Id && OwnerPlayer.IsComputerPlayer)
            {
                //this is an enemy attack on me
                var unitTargeted = OwnerPlayer.GetUnitById(battleDamageReport.TargetPlatformId);
                var detectedEnemy = OwnerPlayer.GetDetectedUnitByUnitId(battleDamageReport.PlatformInflictingDamageId);
                if (detectedEnemy != null)
                {
                    if (unitTargeted != null && !unitTargeted.IsMarkedForDeletion)
                    {
                        if (unitTargeted.CanTargetDetectedUnit(detectedEnemy, false))
                        {
                            unitTargeted.EngageDetectedUnit(detectedEnemy, GameConstants.EngagementOrderType.EngageNotClose, unitTargeted.IsInGroupWithOthers());
                        }
                    }
                    else
                    {
                        this.EngageDetectedTarget(detectedEnemy);
                    }
                }
                return;
            }
            //this is a report about our attack on the enemy
            if (battleDamageReport.IsTargetPlatformDestroyed)
            {
                return;
            }
            var detectedUnit = OwnerPlayer.GetDetectedUnitById(battleDamageReport.TargetPlatformId);
            if (detectedUnit == null || detectedUnit.IsMarkedForDeletion)
            {
                return;
            }
            //at this stage we know the target still exists, and the attack missed the target or didn't get the job done
            if( detectedUnit.CountTargettingMissiles() > 0)
            {
                return; //missiles still in the air, no need to reengage
            }
            var attackingUnit = OwnerPlayer.GetUnitById(battleDamageReport.PlatformInflictingDamageId);
            if (attackingUnit != null && attackingUnit.IsMarkedForDeletion)
            {
                attackingUnit = null;
            }
            switch (detectedUnit.DomainType)
            {
                case GameConstants.DomainType.Surface:
                    if (!reAttackSurface)
                    {
                        return;
                    }
                    break;
                case GameConstants.DomainType.Air:
                    if (!reAttackAir)
                    {
                        return;
                    }
                    break;
                case GameConstants.DomainType.Subsea:
                    if (!reAttackSub)
                    {
                        return;
                    }
                    break;
                case GameConstants.DomainType.Land:
                    if (!reAttackLand)
                    {
                        return;
                    }
                    break;
            }

            if (attackingUnit != null)
            {
                if (attackingUnit.CanTargetDetectedUnit(detectedUnit, false) && !attackingUnit.IsOrderedToReturnToBase) //original attacker to reengage
                {
                    GameConstants.EngagementOrderType engagementType = GameConstants.EngagementOrderType.EngageNotClose;
                    if (attackingUnit.DomainType == GameConstants.DomainType.Air)
	                {
                        engagementType = GameConstants.EngagementOrderType.CloseAndEngage;
	                }
                    attackingUnit.EngageDetectedUnit(detectedUnit, engagementType, attackingUnit.IsInGroupWithOthers());
                    return;
                }
            }
            //at this stage we know that original attacker cannot reengage. What to do. Depends on parameters.
            if (!OwnerPlayer.IsComputerPlayer)
            {
                return; //don't make choices for human player
            }
            switch (detectedUnit.DomainType)
            {
                case GameConstants.DomainType.Surface:
                    if (reAttackSurface)
                    {
                        EngageSurfaceThreat(detectedUnit, true, launchAirIfNecessary);
                    }
                    break;
                case GameConstants.DomainType.Air:
                    if (reAttackAir)
                    {
                        EngageAirThreat(detectedUnit, true, launchAirIfNecessary);
                    }
                    break;
                case GameConstants.DomainType.Subsea:
                    if (reAttackSub)
                    {
                        EngageSubseaThreat(detectedUnit, true, launchAirIfNecessary);
                    }
                    break;
                case GameConstants.DomainType.Land:
                    if (reAttackLand)
                    {
                        EngageLandStructures(detectedUnit);
                    }
                    break;
                case GameConstants.DomainType.Unknown:
                    break;
            }
        }

        //public int CountMissilesTargettingDetectedUnit(DetectedUnit detectedUnit)
        //{
        //    if (detectedUnit == null)
        //    {
        //        return 0; //funny
        //    }
        //    var missiles = from m in OwnerPlayer.Units
        //                   where !m.IsMarkedForDeletion && m.UnitClass.IsMissileOrTorpedo
        //                   && m.TargetDetectedUnit != null && m.TargetDetectedUnit.Id == detectedUnit.Id
        //                   select m;
        //    return missiles.Count<BaseUnit>();
        //}

		public virtual void EngageAirThreat(DetectedUnit detectedUnit, bool closeAndEngage, bool launchAircraftIfNecessary)
		{
			if (detectedUnit.FriendOrFoeClassification != GameConstants.FriendOrFoe.Foe || detectedUnit.IsKnownToBeCivilianUnit ||
				detectedUnit.DomainType != GameConstants.DomainType.Air)
			{
				return;
			}
            if (detectedUnit.CountTargettingMissiles() >= GetMaxTargettingMissiles(detectedUnit))
            {
                return;
            }
            int NoOfUnitsFire = 0; // detectedUnit.CountTargettingMissiles();
            int MaxUnitsFire = 2;
            if (detectedUnit.RefersToUnit != null && detectedUnit.RefersToUnit.UnitClass.IsMissileOrTorpedo)
            {
                MaxUnitsFire = 2;
                //in addition, targetted unit, if any, should always fire
                BaseUnit refersToUnit = detectedUnit.RefersToUnit;
                if (refersToUnit != null && refersToUnit.TargetDetectedUnit != null)
                {
                    DetectedUnit det = refersToUnit.TargetDetectedUnit;
                    if (det != null)
                    {
                        BaseUnit targetUnit = det.RefersToUnit;
                        if (targetUnit != null)
                        { 
                            targetUnit.RespondToImminentThreat(detectedUnit);
                        }
                    }
                }
            }
            if (!detectedUnit.CanBeTargeted)
            {
                return;
            }
			var UnitList = OwnerPlayer.GetSortedUnitsInAreaByRole(GameConstants.Role.AttackAir,
				detectedUnit.Position.Coordinate, GameConstants.DEFAULT_AA_DEFENSE_RANGE_M * 1.5, false);
			GameManager.Instance.Log.LogDebug(
				string.Format("BaseAIHandler->EngageAirThreat to engage {0}: CloseAndEngage: {1}, LaunchAir: {2}",
				detectedUnit, closeAndEngage, launchAircraftIfNecessary));

			if (MaxUnitsFire >= NoOfUnitsFire && detectedUnit.IsFiredUpon)
			{
				return;
			}
			//GameManager.Instance.Log.LogDebug("FireOnAirThreats targets " + detectedUnit.ToString());
			foreach (var u in UnitList)
			{
                if ( u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
                        || ( u.TargetDetectedUnit != null ) )
                {
                    continue;
                }
				if (u.ShouldImmediatelyFireOnTargetType(detectedUnit))
				{
					bool engageResult = u.EngageDetectedUnit(detectedUnit, 
						GameConstants.EngagementOrderType.EngageNotClose, u.IsInGroupWithOthers());
					GameManager.Instance.Log.LogDebug(
						string.Format("EngageAirThreat: {0} ordered to fire on {1}: {2}",
						u.ToShortString(), detectedUnit.ToString(), engageResult));
					//unit.ExecuteOrders();
					NoOfUnitsFire++;
				}
				if (NoOfUnitsFire >= MaxUnitsFire)
				{
					break;
				}
			}
			if (NoOfUnitsFire >= MaxUnitsFire)
			{
				return;
			}
			if (closeAndEngage)
			{
				UnitList = OwnerPlayer.GetSortedUnitsInAreaByRole(GameConstants.Role.InterceptAircraft,
					detectedUnit.Position.Coordinate, GameConstants.DEFAULT_AA_INTERCEPT_RANGE_M, true);
				if (UnitList.Any())
				{
					foreach (var u in UnitList)
					{
						if (u.WeaponOrders != GameConstants.WeaponOrders.FireOnAllClearedTargets
							|| (u.TargetDetectedUnit != null
							&& u.TargetDetectedUnit.Id != detectedUnit.Id))
						{
							continue;
						}

						if (u.HasEnoughFuelToReachTarget(detectedUnit.Position.Coordinate, true))
						{
							GameManager.Instance.Log.LogDebug(
								string.Format("EngageAirThreat: {0} ordered to CloseAndEngage  {1}",
								u.ToShortString(), detectedUnit.ToString()));

							u.EngageDetectedUnit(detectedUnit, GameConstants.EngagementOrderType.CloseAndEngage, true);
							break;
						}
					}
				}
				else if (launchAircraftIfNecessary 
					&& detectedUnit.DetectionClassification != GameConstants.DetectionClassification.Missile 
					&& !IsAnyAircraftTargettingThisTarget(detectedUnit, true))
				{
					int countDesired = 2; //TODO: Change for higher air threat in area, lower for smaller threats
					List<GameConstants.Role> roleList = new List<GameConstants.Role>();
					roleList.Add(GameConstants.Role.InterceptAircraft);
					EngagementOrder order = new EngagementOrder(detectedUnit, GameConstants.EngagementOrderType.CloseAndEngage,
						 GameConstants.EngagementStrength.DefaultAttack);
					int launchedCount = LaunchAircraft(detectedUnit.Position.Coordinate, roleList, string.Empty, countDesired, 1, true,
						new List<BaseOrder>() { order }, "AI: EngageAirThreat");
					GameManager.Instance.Log.LogDebug(
						string.Format("EngageAirThreat: Launched {0} aircraft to intercept target {1}.",
						launchedCount, detectedUnit.ToString()));

				}
			}
		}

        /// <summary>
        /// Return a rough estimate, based on target type and not hit points, about how many
        /// missiles/torpedoes it is desirable to have targetting a single DetectedUnit at one time.
        /// To avoid units wasting ammo.
        /// </summary>
        /// <param name="threateningDetectedUnit"></param>
        /// <returns></returns>
        public int GetMaxTargettingMissiles(DetectedUnit threateningDetectedUnit)
        {
            int defaultNo = 100; //arbitrary max
            switch (threateningDetectedUnit.DomainType)
            {
                case GameConstants.DomainType.Surface:
                    break;
                case GameConstants.DomainType.Air:
                    defaultNo = 2;
                    break;
                case GameConstants.DomainType.Subsea:
                    defaultNo = 2;
                    break;
                case GameConstants.DomainType.Land:
                    break;
                case GameConstants.DomainType.Unknown:
                    break;
            }
            return defaultNo;
        }


		public virtual MovementOrder CreateSearchOrdersAircraft(Region region, bool isAswSearch, bool useActiveSensors)
		{
			MovementOrder moveOrder = new MovementOrder();
			bool isOn = false;
			foreach (var coord in region.Coordinates)
			{
				Waypoint wp = new Waypoint(coord);
				if (isAswSearch)
				{ 
					wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorActivationOrder)
					{
						IsParameter = useActiveSensors,
						SensorType = GameConstants.SensorType.Sonar,
					});
					if (isOn) //every second wp
					{
						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetSpeed)
						{
							UnitSpeedType = GameConstants.UnitSpeedType.Slow,
						});
						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetElevation)
						{
							HeightDepthPoints = GameConstants.HeightDepthPoints.VeryLow,
						});

						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorDeploymentOrder)
						{
							IsParameter = true,
							SensorType = GameConstants.SensorType.Sonar,
						});
					}
					else
					{
						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetSpeed)
						{
							UnitSpeedType = GameConstants.UnitSpeedType.Cruise,
						});
						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetElevation)
						{
							HeightDepthPoints = GameConstants.HeightDepthPoints.MediumHeight,
						});

						//undeploy dipping sonar
						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorDeploymentOrder)
						{
							IsParameter = false,
							SensorType = GameConstants.SensorType.Sonar,
						});

						wp.Orders.Add(new BaseOrder(OwnerPlayer, GameConstants.OrderType.SpecialOrders)
						{
							IsParameter = true,
							SpecialOrders = GameConstants.SpecialOrders.DropSonobuoy,
						});
					}

				}
				isOn = !isOn;
				moveOrder.AddWaypoint(wp);
			}
			moveOrder.IsRecurring = true;
			return moveOrder;
		}

		public MovementOrder CreateSearchOrdersAircraft(Coordinate coordinate, double radiusM, 
			int countWaypoints, bool isAswSearch, bool useActiveSensors)
		{
			var moveOrder = new MovementOrder();
            moveOrder.RemoveAllExistingWaypoints = true;
            var defaultHeightM =  GameConstants.HEIGHT_MEDIUM_MIN_M;
            if (isAswSearch)
	        {
		         defaultHeightM =  GameConstants.HEIGHT_LOW_MIN_M;
	        }
			//var orderList = new List<BaseOrder>();
			double bearingDeg = GameManager.Instance.GetRandomNumber(360);
            int noOfPosPerCircle = countWaypoints;
            if (noOfPosPerCircle > 10)
            {
                noOfPosPerCircle = 10;
            }
			for (int i = 1; i < countWaypoints; i++)
			{
				double distanceM = GameManager.Instance.GetRandomNumber((int)radiusM);
				if(distanceM < radiusM / 2.0)
				{
					distanceM = GameManager.Instance.GetRandomNumber((int)radiusM / 2) + (radiusM / 2.0);
				}
				Coordinate coord = MapHelper.CalculateNewPosition2(coordinate, bearingDeg, distanceM);
				bearingDeg += GameManager.Instance.GetRandomNumber(360/noOfPosPerCircle) * 2.0;
				double heightM = defaultHeightM;
				if (isAswSearch && (i % 2 == 0))
				{
					heightM = GameConstants.HEIGHT_VERY_LOW_MIN_M;
				}
				Position pos = new Position(coord);
				pos.HeightOverSeaLevelM = heightM;
				Waypoint wp = new Waypoint(pos);
				
				if (!isAswSearch && useActiveSensors) //if set to use active sensors, and not asw, it is radar
				{
					if (i == 1 || i == 4) //first and fourth waypoint gets sensor activation order
					{
						BaseOrder orderSensorActive = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorActivationOrder);
						orderSensorActive.IsParameter = true;
						orderSensorActive.SensorType = GameConstants.SensorType.Radar;
						wp.Orders.Add(orderSensorActive);
                        var heightOrder = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetElevation);
                        heightOrder.HeightDepthPoints = GameConstants.HeightDepthPoints.MaxHeight;
                        wp.Orders.Add(heightOrder);
					}
				}
				if (isAswSearch && i % 2 == 0) //every second waypoint, conduct active or passive search
				{
                    if (true)
                    {
                        
                    }
                    if (TerrainReader.GetHeightM(pos.Coordinate) > 0)
                    {
                        GameManager.Instance.Log.LogDebug(
                            string.Format("CreateSearchOrdersAircraft: ASW search on land ignored."));
                        continue;
                    }
					if (useActiveSensors)
					{
						BaseOrder orderSensorActive = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorActivationOrder);
						orderSensorActive.IsParameter = true;
						orderSensorActive.SensorType = GameConstants.SensorType.Sonar;
						wp.Orders.Add(orderSensorActive);
					}
					wp.Orders.Add(
						new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetSpeed)
						{
							UnitSpeedType = GameConstants.UnitSpeedType.Slow
						});
					BaseOrder orderSensorDeployed = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorActivationOrder);
					orderSensorDeployed.IsParameter = true;
					orderSensorDeployed.SensorType = GameConstants.SensorType.Sonar;
					wp.Orders.Add(orderSensorDeployed);
					ScheduledOrder sched = new ScheduledOrder(120, 1);
					sched.Orders = new List<BaseOrder> ()
					{ 
						new BaseOrder()
						{
							OrderType = GameConstants.OrderType.SetElevation,
							HeightDepthPoints = GameConstants.HeightDepthPoints.Low
						},
						new BaseOrder()
						{
							OrderType = GameConstants.OrderType.SetSpeed,
							UnitSpeedType = GameConstants.UnitSpeedType.Cruise,
						}
					};
					wp.Orders.Add(sched); //delay next waypoint, hmm. will this work?
				}
				if (isAswSearch)
				{
					var orderDropSonobuoy = OrderFactory.CreateSonobuoyDeploymentOrder(string.Empty, null, useActiveSensors, true);
					var baseOrderDropSonobuoy = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(orderDropSonobuoy);
					wp.Orders.Add(baseOrderDropSonobuoy);
				}
				moveOrder.AddWaypoint(wp);
			}
			moveOrder.IsRecurring = true; 
			return moveOrder;
		}

		public int LaunchAircraft(HighLevelOrder order)
		{
			MovementOrder moveOrder = new MovementOrder();
            int countTotal = order.CountDesired;
            int countEachGroup = 2;
            if (countEachGroup > countTotal)
            {
                countEachGroup = countTotal;
            }
            int launchCount = 0;
            int launchTotalSoFar = 0;
			if (order.HighLeverOrderType == GameConstants.HighLevelOrderType.SetAswPatrol)
			{
                countEachGroup = 1;
				order.RolesList.Add(GameConstants.Role.ASW);
				order.RolesList.Add(GameConstants.Role.IsAircraft);
			}
			else if(order.HighLeverOrderType == GameConstants.HighLevelOrderType.SetAewPatrol)
			{
				order.RolesList.Add(GameConstants.Role.AEW);
				order.RolesList.Add(GameConstants.Role.IsAircraft);
			}
            bool activeSensor = order.SetActiveRadar;
            if (order.HighLeverOrderType == GameConstants.HighLevelOrderType.SetAswPatrol)
            {
                activeSensor = order.SetActiveSonar;
            }
			if (order.PositionCenter != null)
			{
                do
                {
                    var orderList = new List<BaseOrder>();

                    moveOrder = CreateSearchOrdersAircraft(
                        new Coordinate(order.PositionCenter), order.DistanceM, 20,
                        order.HighLeverOrderType == GameConstants.HighLevelOrderType.SetAswPatrol,
                        activeSensor);
                    if (order.SetActiveRadar)
                    {
                        var activeRadar = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SensorActivationOrder);
                        activeRadar.SensorType = GameConstants.SensorType.Radar;
                        activeRadar.IsParameter = true;
                        orderList.Add(activeRadar);
                    }
                    if (order.RolesList.Contains(GameConstants.Role.AWACS))
                    {
                        var maxElevation = new BaseOrder(OwnerPlayer, GameConstants.OrderType.SetElevation);
                        maxElevation.HeightDepthPoints = GameConstants.HeightDepthPoints.MaxHeight;
                        orderList.Add(maxElevation);
                        
                    }
                    orderList.Add(moveOrder);
                    launchCount = LaunchAircraft(new Coordinate(order.PositionCenter), order.RolesList, order.UnitClassId,
                        countEachGroup, order.CountMinimum, false, orderList, order.Tag);
                    launchTotalSoFar += launchCount;
                    GameManager.Instance.Log.LogDebug("Launchaircraft(order) set to launch " + launchCount + " aircraft.");
                    
                } while (launchCount > 0 && launchTotalSoFar < countTotal);
			}
			else
			{
				GameManager.Instance.Log.LogDebug("Launchaircraft(order) could not launch as Position is null.");
			}
			return launchCount;
		}

		public int LaunchAircraft(Coordinate coordinate,
			List<GameConstants.Role> rolesList, string unitClassId, 
			int countDesired, int countMinimum, bool dontMixClasses, List<BaseOrder> orders, 
			string tag)
		{
			var unitClass = GameManager.Instance.GetUnitClassById(unitClassId);
			var carrierRoleList = new List<GameConstants.Role>();
			if (unitClass == null)
			{
				if (rolesList.Contains(GameConstants.Role.IsFixedWingAircraft))
				{
					carrierRoleList.Add(GameConstants.Role.LaunchFixedWingAircraft);
				}
				else
				{
					carrierRoleList.Add(GameConstants.Role.LaunchRotaryWingAircraft);
				}
			}
			else
			{
				if (unitClass.UnitType == GameConstants.UnitType.FixedwingAircraft)
				{
					carrierRoleList.Add(GameConstants.Role.LaunchFixedWingAircraft);
				}
				else
				{
					carrierRoleList.Add(GameConstants.Role.LaunchRotaryWingAircraft);
				}
			}
            var carrierCandidates = OwnerPlayer.FindAllAvailableUnitRole(
				coordinate, carrierRoleList, string.Empty, false, false);
			if (carrierCandidates == null || carrierCandidates.Count == 0)
			{
				GameManager.Instance.Log.LogDebug("LaunchAircraft could not find any carrier to launch desired aircraft.");
				return 0;
			}
			var missionType = GameConstants.MissionType.Patrol;
			var missionTargetType = GameConstants.MissionTargetType.Undefined;
			if(rolesList.Contains(GameConstants.Role.ASW))
			{
				missionTargetType = GameConstants.MissionTargetType.Sub;
			}
			else if (rolesList.Contains(GameConstants.Role.AttackSurface) 
				|| rolesList.Contains(GameConstants.Role.AttackSurfaceStandoff))
			{
				missionTargetType = GameConstants.MissionTargetType.Surface;
			}
			else if (rolesList.Contains(GameConstants.Role.InterceptAircraft) 
				|| rolesList.Contains(GameConstants.Role.AttackAir))
			{
				missionTargetType = GameConstants.MissionTargetType.Air;
			}
			else if (rolesList.Contains(GameConstants.Role.AttackLand) 
				|| rolesList.Contains(GameConstants.Role.AttackLandStandoff))
			{
				missionTargetType = GameConstants.MissionTargetType.Land;
			}
			var aircraftList = new List<AircraftUnit>();
			foreach (var carrier in carrierCandidates)
			{
				if (carrier.DomainType != GameConstants.DomainType.Land && carrier.IsCommunicationJammingCurrentlyInEffect())
				{
					continue;
				}

				if (carrier.AircraftHangar != null)
				{
					foreach (var aircraft in carrier.AircraftHangar.Aircraft)
					{
						if (aircraft.IsReady && aircraft.SupportsRole(rolesList))
						{
							if (unitClass == null || aircraft.UnitClass.Id == unitClass.Id)
							{
								double distanceM = MapHelper.CalculateDistance3DM(
									new Position(coordinate), carrier.Position);
								if (aircraft.IsReady && !aircraft.IsReserved && aircraft.MaxRangeCruiseM > distanceM * 2.2)
								{
									if (aircraftList.Count > 0)
									{
										if (dontMixClasses)
										{
											if (aircraft.UnitClass.Id != aircraftList[0].UnitClass.Id)
											{
												continue;
											}
										}
										if (aircraft.UnitClass.UnitType != aircraftList[0].UnitClass.UnitType) //assure helos and planes not mixed
										{
											continue;
										}
									}
									aircraftList.Add(aircraft);
                                    aircraft.SetReservation(5);
									if (aircraftList.Count >= countDesired)
									{

										CreateAircraftLaunchOrder(carrier, 
											aircraftList, orders, tag, 
											missionType, missionTargetType);
										return aircraftList.Count;
									}
								}
							}
						} //if isready
					} //foreach aircraft in carrier
					if (aircraftList.Count > 0 && aircraftList.Count >= countMinimum)
					{
						CreateAircraftLaunchOrder(carrier, aircraftList, orders, tag, 
							missionType, missionTargetType);
						return aircraftList.Count;
					}
				} //if aircrafthangar
				aircraftList.Clear();
			} //foreach carrier
			return 0;

		}

		public void CreateAircraftLaunchOrder(BaseUnit carrier, 
			List<AircraftUnit> aircraftList, 
			List<BaseOrder> orders, 
			string tag, 
			GameConstants.MissionType missionType, 
			GameConstants.MissionTargetType missionTargetType)
		{
			GameManager.Instance.Log.LogDebug(
				string.Format("CreateAircraftLaunchOrder orders carrier {0} to launch {1} aircraft with {2} order(s).",
				carrier.ToShortString(), aircraftList.Count, orders.Count));

			LaunchAircraftOrder order = new LaunchAircraftOrder(OwnerPlayer);
			if (string.IsNullOrEmpty(tag))
			{
				order.Tag = tag;
			}
			order.UnitList = aircraftList;
			foreach (var craft in aircraftList)
			{
				craft.MissionType = missionType;
				craft.MissionTargetType = missionTargetType;
			}
			if (orders != null)
			{
				order.Orders = orders;
			}
			carrier.Orders.Enqueue(order);
		}

		public void SetAswAirPatrol(HighLevelOrder order)
		{
			var roleList = new List<GameConstants.Role>();
			roleList.Add(GameConstants.Role.ASW);
			roleList.Add(GameConstants.Role.IsAircraft);
			Coordinate coordinate = new Coordinate(order.PositionCenter);
            var existingPatrol = OwnerPlayer.FindNearestAvailableUnitRole(coordinate, roleList, order.UnitClassId, true);
			double distanceM = 0;
			double maxDistanceM = order.DistanceM;
			if (maxDistanceM < 1)
			{
				maxDistanceM = MAX_DISTANCE_PATROL_ASW_M;
			}
			if (existingPatrol != null)
			{ 
				distanceM = MapHelper.CalculateDistanceM(coordinate, existingPatrol.Position.Coordinate);
				if (distanceM > maxDistanceM)
				{
					existingPatrol = null;
				}
			}
			if (existingPatrol != null)
			{
				GameManager.Instance.Log.LogDebug(
					string.Format("SetAswAirPatrol finds unit {0} to fulfill ASW role near {1}.",
					existingPatrol.ToShortString(), coordinate.ToString()));
				return;
			}
			else
			{
				int countAir = LaunchAircraft(order);
				GameManager.Instance.Log.LogDebug(
					string.Format("SetAswAirPatrol launches {0} aircraft to fulfill ASW role near {1}.",
					countAir, coordinate.ToString()));
			}
		}

		public void SetAewAirPatrol(HighLevelOrder order)
		{
			var roleList = new List<GameConstants.Role>();
			roleList.Add(GameConstants.Role.AEW);
			roleList.Add(GameConstants.Role.IsAircraft);
            if (order.RolesList != null)
            {
                foreach (var role in order.RolesList)
                {
                    roleList.Add(role);
                }
            }
			Coordinate coordinate = new Coordinate(order.PositionCenter);
            var existingPatrol = OwnerPlayer.FindNearestAvailableUnitRole( coordinate, roleList, order.UnitClassId, true );
			double distanceM = 0;
			double maxDistanceM = order.DistanceM;
			if (maxDistanceM < 1)
			{
				maxDistanceM = MAX_DISTANCE_PATROL_AEW_M;
			}
			if (existingPatrol != null)
			{
				distanceM = MapHelper.CalculateDistanceM(coordinate, existingPatrol.Position.Coordinate);
				if (distanceM > maxDistanceM)
				{
					existingPatrol = null;
				}
			}
			if (existingPatrol != null)
			{
				GameManager.Instance.Log.LogDebug(
					string.Format("SetAswAirPatrol finds unit {0} to fulfill AEW role near {1}.",
					existingPatrol.ToShortString(), coordinate.ToString()));
				return;
			}
			else
			{
				int countAir = LaunchAircraft(order);
				GameManager.Instance.Log.LogDebug(
					string.Format("SetAswAirPatrol launches {0} aircraft to fulfill AEW role near {1}.",
					countAir, coordinate.ToString()));
			}
		}

		public virtual void TurnOffActiveRadars()
		{ 
			//First, determine no missiles or torpedoes are currently active,
			//Second, determine that no enemy threat is imminent
			//Third, determine that no BattleDamageReports have appeared in last minute
			//if so, turn off active radar and sonar on units that do not have engagement orders scheduled
			if(IsAnyWarfareOngoingNow(OwnerPlayer, 600)) 
			{
				return;
			}
			GameManager.Instance.Log.LogDebug("BaseAIHandler->TurnOffActiveRadar() triggers for player " + OwnerPlayer);
			foreach (var unit in OwnerPlayer.Units)
			{
				if (!unit.HasAnyEngagementOrders()
					&& !unit.UnitClass.IsMissileOrTorpedo
					&& unit.UnitClass.UnitType != GameConstants.UnitType.LandInstallation
					&& unit.UnitClass.UnitType != GameConstants.UnitType.Decoy
                    && !unit.SupportsRole(GameConstants.Role.AWACS))
				{
					foreach (var sensor in unit.Sensors)
					{
						if (sensor.SensorClass.IsPassiveActiveSensor 
							&& sensor.SensorClass.SensorType == GameConstants.SensorType.Radar)
						{
							sensor.IsActive = false;
						}
					}
				}
			}
		}

		public virtual void TurnOffActiveSonars()
		{
			if (IsAnyWarfareOngoingNow(OwnerPlayer, 600))
			{
				return;
			}
			GameManager.Instance.Log.LogDebug("BaseAIHandler->TurnOffActiveSonar() triggers for player " + OwnerPlayer);
			foreach (var unit in OwnerPlayer.Units)
			{
				if (!unit.HasAnyEngagementOrders()
					&& !unit.UnitClass.IsMissileOrTorpedo
					&& unit.UnitClass.UnitType != GameConstants.UnitType.Mine
					&& unit.UnitClass.UnitType != GameConstants.UnitType.Sonobuoy)
				{
					foreach (var sensor in unit.Sensors)
					{
						if (sensor.SensorClass.IsPassiveActiveSensor
							&& sensor.SensorClass.SensorType == GameConstants.SensorType.Sonar)
						{
							sensor.IsActive = false;
						}
					}
				}
			}
		}

		public virtual bool IsAnyWarfareOngoingNow(Player player, double inLastSeconds)
		{
			var listMissiles = from u in OwnerPlayer.Units
							   where u.UnitClass.IsMissileOrTorpedo
							   select u;
			if (listMissiles.Count<BaseUnit>() > 0)
			{
				return true;  //own missiles in the air
			}
			var listDetectedMissiles = from d in OwnerPlayer.DetectedUnits
									   where !d.IsMarkedForDeletion && d.DetectionClassification == GameConstants.DetectionClassification.Missile
									   select d;
			if (listDetectedMissiles.Count<DetectedUnit>() > 0)
			{
				return true;  //enemy missiles in the air!
			}
			double currentGameTimeSecs = GameManager.Instance.Game.GameWorldTimeSec;
			var listBattleDmg = from b in OwnerPlayer.BattleDamageReports
								where b.GameTimeSec >= (currentGameTimeSecs - inLastSeconds)
								select b;
			if (listBattleDmg.Count<BattleDamageReport>() > 0)
			{
				return true; //battle ongoing inLastSeconds of gametime
			}
			return false;
		}

		#endregion

		#endregion
	}
}
