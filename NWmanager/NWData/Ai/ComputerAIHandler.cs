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

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class ComputerAIHandler: BaseAIHandler
    {

        #region "Private fields"

        #endregion

        #region "Constructors"

        public ComputerAIHandler() : base()
        {
        }

        #endregion


        #region "Public properties"


        #endregion



        #region "Public methods"

        public override void NewDetection(DetectedUnit detectedUnit)
        {
            CheckFriendOrFoeStatus(detectedUnit);
            SetThreatClassification(detectedUnit);
            CheckEventTriggersForDetection(detectedUnit);
            EngageAirThreat(detectedUnit, true, true);
            //base.NewDetection(detectedUnit);
        }

        public override void DetectionUpdated(DetectedUnit detectedUnit)
        {
            CheckFriendOrFoeStatus(detectedUnit);
            SetThreatClassification(detectedUnit);
            CheckEventTriggersForDetection(detectedUnit);
            //EngageAirThreat(detectedUnit, true, true);
        }

        public override void BattleDamageReportReceived(BattleDamageReport battleDamageReport)
        {
            HandleIncomingBattleDamageReport(battleDamageReport, true, true, true, true, true);
            base.BattleDamageReportReceived(battleDamageReport);
        }
        public override void GamePlayHasStarted()
        {
            //OwnerPlayer.IsAllUnknownContactsHostile = true; //now set in GameScenarioPlayer
            OwnerPlayer.IsAutomaticallyRespondingToActiveSensor = true;
            OwnerPlayer.IsAutomaticallyEvadingAttacks = true;
            OwnerPlayer.DefaultWeaponOrders = GameConstants.WeaponOrders.FireOnAllClearedTargets;
            if (string.IsNullOrEmpty(OwnerPlayer.Name) || OwnerPlayer.Name == OwnerPlayer.Id)
            {
                OwnerPlayer.Name = "Computer AI";
            }
            //foreach (var player in OwnerPlayer.Enemies)
            //{
            //    if (!player.IsComputerPlayer)
            //    {
            //        OwnerPlayer.CreateNewMessage(player, "gl hf");
            //    }
            //}
            SetPrioritizedTargets();
            HighLevelOrder hlo = new HighLevelOrder(
                GameConstants.HighLevelOrderType.TurnOffUnnecessaryActiveSensors, 600, 600, 9999);
            OwnerPlayer.HighLevelOrders.Enqueue(hlo);
            if (OwnerPlayer.IsAutomaticallySettingHighValueDefence)
            {
                DefendHighValueUnits();    
            }
            
            SetWeaponAndSensorOrders();
            
            base.GamePlayHasStarted();
        }



        public override void GameLossOrVictoryAchieved()
        {
            //if (OwnerPlayer.HasBeenDefeated)
            //{
            //    foreach (var player in OwnerPlayer.Enemies)
            //    {
            //        if (!player.IsComputerPlayer)
            //        {
            //            OwnerPlayer.CreateNewMessage(player, "gg wp");
            //        }
            //    }

            //}
            //else if (OwnerPlayer.HasWonGame)
            //{
            //    foreach (var player in OwnerPlayer.Enemies)
            //    {
            //        if (!player.IsComputerPlayer)
            //        {
            //            OwnerPlayer.CreateNewMessage(player, "gg");
            //        }
            //    }
            //}
            base.GameLossOrVictoryAchieved();
        }

        public override void UnitHasNoMovementOrders(BaseUnit unit)
        {
            if (unit is AircraftUnit && unit.Position != null && !unit.IsMarkedForDeletion)
            {
                Coordinate coord = unit.Position.Coordinate;
                List<GameConstants.Role> roleList = new List<GameConstants.Role>();
                roleList.Add(GameConstants.Role.IsSurfaceCombattant);
                var nearestUnit = OwnerPlayer.FindNearestAvailableUnitRole( coord, roleList, string.Empty, false );
                
                if (nearestUnit != null)
                {
                    GameManager.Instance.Log.LogDebug(
                        string.Format("UnitHasNoMovementOrders: {0} has no movement orders: directed to airspace around unit {1}",
                        unit, nearestUnit));
                    //Find nearest surface combattant and patrol area
                    var moveOrder = this.CreateSearchOrdersAircraft(
                        Region.FromCircle(coord, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M),
                        unit.SupportsRole(GameConstants.Role.ASW), false);
                    unit.Orders.Enqueue(moveOrder);
                    unit.MissionType = GameConstants.MissionType.Patrol;
                }
                else
                {
                    //if no surface combattant, patrol near airport
                    roleList.Clear();
                    roleList.Add(GameConstants.Role.LaunchFixedWingAircraft);
                    nearestUnit = OwnerPlayer.FindNearestAvailableUnitRole( coord, roleList, string.Empty, false );
                    if (nearestUnit != null)
                    {
                        GameManager.Instance.Log.LogDebug(
                            string.Format("UnitHasNoMovementOrders: {0} has no movement orders: directed to airspace around airfield {1}",
                            unit, nearestUnit));

                        var moveOrder = this.CreateSearchOrdersAircraft(
                            Region.FromCircle(coord, GameConstants.DEFAULT_AIR_ASu_STRIKE_RANGE_M),
                            false, false);
                        unit.Orders.Enqueue(moveOrder);
                        unit.MissionType = GameConstants.MissionType.Patrol;
                    }
                }//TODO: Look at AI hints when ready!
                unit.SetMissionStatusFromOrders();
                unit.IsOutOfAmmo();
            }
            //TODO: Also give orders to ships and subs!
            base.UnitHasNoMovementOrders(unit);
        }

        public override void Tick(DateTime gameTime)
        {
            base.Tick(gameTime);
        }

        public override void TickEveryMinute(DateTime gameTime)
        {
            base.TickEveryMinute(gameTime);
            if (_minuteCounter % 10 == 0)
            {
                if (OwnerPlayer.IsAutomaticallyEngagingHighValueTargets)
                {
                    EngageHighValueTargets();
                }

                if (OwnerPlayer.IsAutomaticallySettingHighValueDefence)
                {
                    DefendHighValueUnits();    
                }
                
            }
            EngageDetectedTargets();
        }

        protected void CheckFriendOrFoeStatus(DetectedUnit detectedUnit)
        {
            if (detectedUnit.IsIdentified && detectedUnit.RefersToUnit != null)
            {
                if (detectedUnit.FriendOrFoeClassification == GameConstants.FriendOrFoe.Foe && !OwnerPlayer.IsEnemy(detectedUnit.RefersToUnit.OwnerPlayer))
                {
                    detectedUnit.FriendOrFoeClassification = GameConstants.FriendOrFoe.Undetermined;
                }
            }
        }



        #endregion


    }
}
