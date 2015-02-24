using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class Mine : MissileUnit
    {
        #region "Constructors"

        private DateTime _timeLastTick;

        public Mine() : base()
        {
            _timeLastTick = GameManager.Instance.Game.GameCurrentTime;
        }

        #endregion

        #region "Public properties"


        #endregion

        #region "Public methods"

        public override void MoveToNewCoordinate(double gameTime)
        {
            return;
        }
        
        public override void MoveToNewPosition3D(double elapsedGameTimeSec)
        {
            return;
        }

        public override Waypoint GetActiveWaypoint()
        {
            return null;
        }

        public override void Tick(double timer)
        {
            base.Tick(timer);
            if (this.IsMarkedForDeletion || this.Position == null || ReadyInSec > 0 || GameManager.Instance.Game == null)
            {
                return;
            }
            var timePassedSinceLastTickSec = GameManager.Instance.Game.GameCurrentTime.Subtract(_timeLastTick).TotalSeconds;
            if (timePassedSinceLastTickSec < 10)
            {
                return;
            }
            _timeLastTick = DateTime.FromBinary(GameManager.Instance.Game.GameCurrentTime.ToBinary());
            var enemyUnits = OwnerPlayer.GetEnemyUnitsInAreaByUnitType(GameConstants.UnitType.SurfaceShip, 
                this.Position.Coordinate, 
                GameConstants.MAX_MINE_DETECTION_RANGE_M);
            foreach (var unit in enemyUnits)
            { 
                if(unit.SupportsRole( GameConstants.Role.MineCountermeasures))
                {
                    var msgToMineOwner = new GameStateInfo(GameConstants.GameStateInfoType.MineDestroyedByEnemy, this.Id);
                    OwnerPlayer.Send(msgToMineOwner);

                    var msgToMineDestroyer = new GameStateInfo(GameConstants.GameStateInfoType.MineDestroyedByUs, unit.Id);
                    unit.OwnerPlayer.Send(msgToMineDestroyer);

                    //var msg = OwnerPlayer.CreateNewMessage(
                    //    string.Format("A {0} has been destroyed by enemy mine countermeasures.", ToShortString()));
                    //msg.Position = this.Position.Clone();

                    //var msg2 = unit.OwnerPlayer.CreateNewMessage(
                    //    string.Format("An enemy {0} has been destroyed by mine countermeasures from {1}.", 
                    //    UnitClass.UnitClassShortName, unit.ToShortString()));
                    //msg2.Position = unit.Position.Clone();

                    this.IsMarkedForDeletion = true;
                    return;
                }
                double distanceM = MapHelper.CalculateDistanceM(this.Position.Coordinate, unit.Position.Coordinate);
                if (distanceM <= GameConstants.MAX_MINE_IMPACT_RANGE_M)
                {
                    GameManager.Instance.Log.LogDebug(
                        string.Format("Mine->Tick() reports IMPACT between mine {0} and unit {1}", ToShortString(), unit.ToShortString()));
                    unit.InflictDamageFromProjectileHit(this);
                    this.IsMarkedForDeletion = true;
                    return;
                }
            }

            
        }

        #endregion

    }
    
}
