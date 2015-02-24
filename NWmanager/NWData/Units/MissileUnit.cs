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
    public class MissileUnit : BaseUnit
    {
        #region "Constructors"

        public MissileUnit() : base()
        {
            //TakesElevationFromTargetWaypoint = true;
            MaxSectorRangeSearchDeg = 360;
        }

        #endregion

        #region "Public properties"

        public string TargetUnitClassId { get; set; }

        public Position TargetPosition { get; set; }

        public int DamageHitpoints { get; set; }

        public int HitPercent { get; set; }

        public BaseUnit LaunchPlatform { get; set; }

        public BaseWeapon LaunchWeapon { get; set; }

        public string WeaponClassId { get; set; }

        /// <summary>
        /// The maximum sector range the missile will search for targets within. Default is 360
        /// </summary>
        public double MaxSectorRangeSearchDeg { get; set; }

        public override Waypoint GetActiveWaypoint()
        {
            if (this.UnitClass.UnitType == GameConstants.UnitType.Mine)
            {
                return null;
            }
            if (MovementOrder != null && MovementOrder.CountWaypoints > 0)
            {
                return MovementOrder.GetActiveWaypoint();
            }
            else if (TargetDetectedUnit != null)
            {
                if (TargetDetectedUnit.IsMarkedForDeletion)
                {
                    SearchForTarget();
                }
                if (TargetDetectedUnit != null && !TargetDetectedUnit.IsMarkedForDeletion)
                {
                    Waypoint wp = new Waypoint(TargetDetectedUnit);
                    return wp;
                }
                else
                {
                    return null;
                }
            }
            else if (TargetPosition != null)
            {
                Waypoint wp = new Waypoint(TargetPosition);
                return wp;
            }
            else
            {
                SearchForTarget();
                if (TargetDetectedUnit == null)
                {
                    GameManager.Instance.Log.LogDebug(string.Format(
                        "MissileUnit->ActiveWaypoint Unit {0} has no destination or target and is being deleted.",
                        ToString()));
                    IsMarkedForDeletion = true; //delete missile with no target
                    HitPoints = 0;
                    DirtySetting = TTG.NavalWar.NWComms.GameConstants.DirtyStatus.UnitChanged;
                    return null;
                }
                else
                {
                    Waypoint wp = new Waypoint(TargetDetectedUnit);
                    return wp;
                }
            }
        }

        public override bool SensorSweep()
        {
            // If we detected a unit and doesn't have any target set, search for new target.
            if (base.SensorSweep())
            {
                if (TargetDetectedUnit == null || TargetDetectedUnit.IsMarkedForDeletion)
                {
                    // No need for sensor sweep when searching for target here as it's already done.
                    SearchForTarget(false);
                }
                return true;
            }
            return false;
        }

        protected override bool AttemptDetectUnit(BaseUnit unit)
        {
            if (LaunchWeapon != null && !LaunchWeapon.CanTargetDomain(unit.DomainType))
            {
                return false;
            }
            return base.AttemptDetectUnit(unit);
        }

        #endregion

        #region "Public methods"

        /// <summary>
        /// Given a weapon (countermeasures type), determines if this weapon can work on this missile. Returns bool.
        /// Takes into account remaining ammunition on soft kill weapon, but does not deduct from it. If MaxAmmunition==0,
        /// this is interpreted as unlimited ammo.
        /// </summary>
        /// <param name="wpn"></param>
        /// <returns></returns>
        public bool CanBeSoftKilled(BaseWeapon wpn)
        {
            if (wpn.MaxAmmunition > 0 && wpn.AmmunitionRemaining < 1)
            {
                return false; //soft kill is out of ammo
            }
            try
            {
                switch (wpn.WeaponClass.EwCounterMeasures)
                {
                    case GameConstants.EwCounterMeasuresType.None:
                        return false;
                    case GameConstants.EwCounterMeasuresType.Flare:
                        return Sensors.Any(s => s.SensorClass.SensorType == GameConstants.SensorType.Infrared);
                    case GameConstants.EwCounterMeasuresType.Chaff:
                        //Missiles without any sensors is considered semiactive radar seeking (uses radar transmitter from launch platform)
                        if (UnitClass.UnitType == GameConstants.UnitType.Missile)
                        {
                            return Sensors.Any(s => s.SensorClass.SensorType == GameConstants.SensorType.Radar)
                                || !Sensors.Any(s => s.SensorClass.SensorType != GameConstants.SensorType.Radar);
                        }
                        return false;
                    case GameConstants.EwCounterMeasuresType.TorpedoDecoy:
                        return UnitClass.UnitType == GameConstants.UnitType.Torpedo;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log.LogError("MissileUnit->CanBeSoftKilled error " + ex.Message);
                return false;

            }
        }


        public void Impact()
        {
            if (Position == null)
            {
                IsMarkedForDeletion = true;
                return;
            }
            if (IsMarkedForDeletion)
            {
                return;
            }
            GameManager.Instance.Log.LogDebug("MissileUnit->Impact called for unit " + Id);
            if (TargetDetectedUnit != null && TargetDetectedUnit.RefersToUnit != null && TargetDetectedUnit.Position != null)
            {
                if (TargetDetectedUnit.RefersToUnit.IsMarkedForDeletion)
                {
                    this.IsMarkedForDeletion = true;
                    return;
                }
                //double distanceM = MapHelper.CalculateDistance3DM(this.Position, TargetDetectedUnit.Position);
                double distanceM = MapHelper.CalculateDistanceM(this.Position.Coordinate, TargetDetectedUnit.Position.Coordinate);
                if (distanceM > GameConstants.DISTANCE_TO_TARGET_IS_HIT_M)
                {
                    if (!InterceptTarget(TargetDetectedUnit)) //try again
                        IsMarkedForDeletion = true;
                }
                else
                {
                    BaseUnit unit = TargetDetectedUnit.RefersToUnit;
                    unit.InflictDamageFromProjectileHit(this);
                    IsMarkedForDeletion = true;
                }
            }
            else
            {
                if (TargetPosition != null)
                {
                    var distanceM = MapHelper.CalculateDistanceM(this.Position.Coordinate, TargetPosition.Coordinate);
                    if (distanceM <= GameConstants.DISTANCE_TO_TARGET_IS_HIT_M)
                    {
                        IsMarkedForDeletion = true;
                        return;
                    }
                }

                if (Sensors.Any())
                {
                    TargetDetectedUnit = null;
                    SearchForTarget();
                    if (TargetDetectedUnit == null && TargetPosition == null 
                        && (MovementOrder == null || MovementOrder.GetActiveWaypoint() == null))
                    {
                        IsMarkedForDeletion=true;
                    }
                }
                else
                {
                    IsMarkedForDeletion = true;
                }
            }
        }

        public void SearchForTarget()
        {
            if (IsMarkedForDeletion)
            {
                return;
            }

            SearchForTarget(true);
        }

        public void SearchForTarget(bool doSensorSweep)
        {
            if (IsMarkedForDeletion)
            {
                return;
            }
            if (this.LaunchWeapon != null && !this.LaunchWeapon.WeaponClass.CanRetargetAfterLaunch)
            {
                if (TargetDetectedUnit == null || TargetDetectedUnit.IsMarkedForDeletion)
                {
                    IsMarkedForDeletion = true;
                }
                return;
            }
            var targetId = (TargetDetectedUnit != null) ? TargetDetectedUnit.Id : null;
            var targetClassId = (TargetDetectedUnit != null && TargetDetectedUnit.RefersToUnit != null)
                                    ? TargetDetectedUnit.RefersToUnit.UnitClass.Id
                                    : TargetUnitClassId;

            SearchForTarget(targetId, targetClassId, doSensorSweep);
        }

        public void SearchForTarget(BaseOrder order)
        {
            if (IsMarkedForDeletion)
            {
                return;
            }

            SearchForTarget(order.SecondId, order.StringParameter, true);
        }

        public void SearchForTarget(string targetId, string classId, bool doSensorSweep)
        {
            if (IsMarkedForDeletion)
            {
                return;
            }
            //first, see if targetdetectedunit is still known
            var targetDet = OwnerPlayer.GetDetectedUnitById(targetId);
            if (targetDet != null)
            {
                InterceptTarget(targetDet);
                return;
            }
            GameManager.Instance.Log.LogDebug(
                string.Format("MissileUnit->SearchForTarget Missile {0} looks for detid={1} or classid={2}",
                this.ToShortString(), targetId, classId));
            if (doSensorSweep && Sensors.Any())
            {
                if (UnitClass.UnitType == GameConstants.UnitType.Missile)
                {
                    SetSensorsActivePassive(GameConstants.SensorType.Radar, true);
                }
                else if (UnitClass.UnitType == GameConstants.UnitType.Torpedo)
                {
                    SetSensorsActivePassive(GameConstants.SensorType.Sonar, true);
                }
                this.SensorSweep();
            }

            //otherwise, see if target with same unitclass can be found. UnitClass in 
            var unitClassTarget = GameManager.Instance.GetUnitClassById(classId);

            // Get ordered list by distance of detected units within our fuel distance
            IEnumerable<DetectedUnit> detectedUnitsOrdered;

            // Filter out targets outside the max search sector range
            if (MaxSectorRangeSearchDeg < 360)
            {
                var bearing = ActualBearingDeg.HasValue ? ActualBearingDeg.Value : 0.0;
                var maxDistanceM = FuelDistanceRemainingM;

                if (TargetPosition != null)
                {
                    bearing = MapHelper.CalculateBearingDegrees(Position.Coordinate, TargetPosition.Coordinate);
                    maxDistanceM = MapHelper.CalculateDistanceRoughM(Position.Coordinate, TargetPosition.Coordinate);
                }

                detectedUnitsOrdered = from d in OwnerPlayer.DetectedUnits
                                       where !d.IsMarkedForDeletion && d.Position != null && d.CanBeTargeted
                                       && MapHelper.IsWithinSector(Position.Coordinate, bearing,
                                                      MaxSectorRangeSearchDeg, d.Position.Coordinate)
                                       let distanceToTargetM = MapHelper.CalculateDistance3DM(d.Position, this.Position)
                                       where distanceToTargetM <= maxDistanceM
                                       orderby distanceToTargetM ascending
                                       select d;
            }
            else
            {
                detectedUnitsOrdered = from d in OwnerPlayer.DetectedUnits
                                       where !d.IsMarkedForDeletion && d.Position != null && d.CanBeTargeted
                                       let distanceToTargetM = MapHelper.CalculateDistance3DM(d.Position, this.Position)
                                       where distanceToTargetM <= FuelDistanceRemainingM
                                       orderby distanceToTargetM ascending
                                       select d;
            }

            //var weaponClass = GameManager.Instance.GetWeaponClassById(this.WeaponClassId);
            foreach (var det in detectedUnitsOrdered)
            {
                if (unitClassTarget != null)
                {
                    if (det.IsIdentified && det.RefersToUnit != null)
                    {
                        if (det.RefersToUnit.UnitClass.Id == unitClassTarget.Id)
                        {
                            InterceptTarget(det);
                            return;
                        }
                    }
                    else
                    {
                        if (LaunchWeapon != null
                            && det.RefersToUnit != null
                            && LaunchWeapon.CanTargetDetectedUnit(det.RefersToUnit, true))
                        {
                            InterceptTarget(det);
                            return;
                        }
                    }
                }
                else if (this.LaunchWeapon != null && this.LaunchWeapon.CanTargetDetectedUnit(det, true))
                {
                    InterceptTarget(det);
                    return;
                }
            }
            if (TargetDetectedUnit == null || TargetDetectedUnit.IsMarkedForDeletion)
            {
                if (MovementOrder != null && MovementOrder.CountWaypoints == 1)
                {
                    var wp = MovementOrder.PeekNextWaypoint();
                    if (MapHelper.CalculateDistance3DM(Position, wp.Position) < GameConstants.DISTANCE_TO_TARGET_IS_HIT_M)
                    {
                        GameManager.Instance.Log.LogDebug(string.Format("SearchForTarget: Missile {0} waypoint was close: deleted.", this));
                        MovementOrder = null;
                        //this.ActiveWaypoint = null;
                    }
                }
            }
            if ((TargetDetectedUnit == null || TargetDetectedUnit.IsMarkedForDeletion)
                && (MovementOrder == null || MovementOrder.CountWaypoints == 0 || GetActiveWaypoint() == null)) //can't find anything. self-destruct
            {
                GameManager.Instance.Log.LogDebug(
                    string.Format("MissileUnit->SearchForTarget Missile {0} found no target and is being deleted.",
                    this.ToShortString()));

                this.IsMarkedForDeletion = true;
            }
        }

        public bool InterceptTarget(DetectedUnit targetDet)
        {
            if (targetDet != null && !targetDet.IsMarkedForDeletion)
            {
                TargetDetectedUnit = targetDet;
                if(targetDet.RefersToUnit != null)
                {
                    TargetUnitClassId = targetDet.RefersToUnit.UnitClass.Id;
                }
                MovementOrder = null;
                //var wp = new Waypoint(targetDet);
                //if (MovementOrder == null)
                //{
                //    MovementOrder = new MovementOrder(wp);
                //}
                //else
                //{
                //    MovementOrder.ClearAllWaypoints();
                //    //MovementOrder.AddWaypoint(wp);
                //}
                ReCalculateEta();
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
                GameManager.Instance.Log.LogDebug(
                    string.Format("MissileUnit->InterceptTarget. Missile {0} set to intercept target {1}", ToShortString(), targetDet));
                return true;
            }
            return false;
        }

        #endregion
    }
}
