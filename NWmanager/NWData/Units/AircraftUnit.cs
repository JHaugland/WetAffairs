using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;

namespace TTG.NavalWar.NWData
{
    [Serializable]
    public class AircraftUnit : BaseUnit
    {
        #region "Constants, Private and internal fields"

        private bool _hasSentCarrierLostMessage = false;

        // List of aircraft units that this unit is going to refuel. List consists of unit and amount to refuel.
        private List<KeyValuePair<AircraftUnit, double>> _aircraftsToRefuel;

        // The current aircraft we're refueling from (if any)
        private AircraftUnit _refuelingFromAircraft;

        #endregion

        #region "Constructors"

        public AircraftUnit()
            : base()
        {

        }

        #endregion

        #region "Public properties"

        //public virtual GameConstants.AircraftDockingStatus AircraftDockingStatus { get; set; }

        public override bool IsMarkedForDeletion
        {
            get
            {
                return base.IsMarkedForDeletion;
            }
            set
            {
                base.IsMarkedForDeletion = value;

                if (IsMarkedForDeletion)
                {
                    StopRefueling();
                }
            }
        }

        #endregion

        #region "Public methods"

        public bool LandOnBase()
        {
            GameManager.Instance.Log.LogDebug(
                string.Format("AircraftUnit->LandOnBase: Unit {0}.", ToString()));

            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            if (LaunchedFromUnit == null || LaunchedFromUnit.Position == null ||
                LaunchedFromUnit.IsMarkedForDeletion || LaunchedFromUnit.HitPoints <= 0)
            {
                if (!_hasSentCarrierLostMessage)
                {
                    _hasSentCarrierLostMessage = true;
                    SetHomeToNewCarrier();
                }
                return false;
            }
            double distanceM = MapHelper.CalculateDistanceM(this.Position.Coordinate, LaunchedFromUnit.Position.Coordinate);
            if (distanceM > GameConstants.DISTANCE_TO_TARGET_IS_HIT_M * 3.0)
            {
                GameManager.Instance.Log.LogDebug(
                    string.Format("LandOnBase: {0} is {1:F}m away from {2}, reattempting ReturnToBase.",
                    Id, distanceM, LaunchedFromUnit));
                this.IsOrderedToReturnToBase = false;
                return ReturnToBase();
            }
            if (LaunchedFromUnit.AircraftHangar == null)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftLandingFailed, this.Id, LaunchedFromUnit.Id));
                GameManager.Instance.Log.LogError(
                    string.Format("Unit{0} cannot land on unit {1} since it has no AircraftHangar",
                    ToShortString(), LaunchedFromUnit.ToShortString()));
                SetHomeToNewCarrier();
                return false;
            }
            if (LaunchedFromUnit.AircraftHangar.ReadyInSec > GameConstants.DEFAULT_TIME_BETWEEN_TAKEOFFS_SEC * 2) //it is damaged
            {
                TimeSpan time = TimeSpan.FromSeconds(LaunchedFromUnit.AircraftHangar.ReadyInSec);
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftLandingFailed, this.Id, LaunchedFromUnit.Id));
                SetHomeToNewCarrier();
                return false;
            }
            double CrashChancePercent = 0;
            int EffectiveLandingSeaState = LaunchedFromUnit.GetEffectiveSeaState();
            if (EffectiveLandingSeaState > GameConstants.AIRCRAFT_LANDING_MAX_SEA_STATE)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftLandingFailed, this.Id, LaunchedFromUnit.Id));

                GameManager.Instance.Log.LogDebug(string.Format(
                    "LandOnBase: Unit {0} could not land due to rough weather. Max sea state is {1},"
                    + "effective sea state is {2}",
                    ToShortString(), GameConstants.AIRCRAFT_LANDING_MAX_SEA_STATE,
                    EffectiveLandingSeaState));
                return false;
            }
            else if (GetEffectiveSeaState() == GameConstants.AIRCRAFT_LANDING_MAX_SEA_STATE)
            {
                CrashChancePercent = 25.0;
            }
            if (CrashChancePercent > 0 && GameManager.Instance.ThrowDice(CrashChancePercent))
            {
                IsMarkedForDeletion = true;
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftCrashedOnLandingRoughWeather, this.Id, LaunchedFromUnit.Id));
                GameManager.Instance.Log.LogDebug(string.Format(
                    "LandOnBase: {0} CRASHED due to rough weather. Effective sea state is {1}", ToShortString(),
                    EffectiveLandingSeaState));
                return false;
            }

            if (LaunchedFromUnit.UnitClass.CarriedRunwayStyle < this.UnitClass.RequiredRunwayStyle)
            {
                OwnerPlayer.Send(new GameStateInfo(GameConstants.GameStateInfoType.AircraftLandingFailed, this.Id, LaunchedFromUnit.Id));
                GameManager.Instance.Log.LogError(string.Format(
                    "LandOnBase: {0} cannot land on {1}: Requires {2}.", ToShortString(),
                    LaunchedFromUnit.ToShortString(), this.UnitClass.RequiredRunwayStyle));
                SetHomeToNewCarrier();
                return false;
            }
            if (OwnerPlayer != null && OwnerPlayer.TcpPlayerIndex > 0)
            {
                //GameManager.Instance.Log.LogDebug("LandOnBase: Sending GameStateInfo object");
                OwnerPlayer.Send(new GameStateInfo()
                {
                    Id = this.Id,
                    InfoType = GameConstants.GameStateInfoType.AircraftIsLanded,
                    SecondaryId = LaunchedFromUnit.Id,
                    UnitClassId = this.UnitClass.Id,
                    BearingDeg = (double)this.Position.BearingDeg,
                });
            }
            _hasSentCarrierLostMessage = false;
            CarriedByUnit = LaunchedFromUnit;
            _hasCheckedForNearestCarrier = false;
            _hasSentActiveDetectionMessage = false;
            MovementOrder = null;
            Orders.Clear();
            IsOrderedToReturnToBase = false;
            TargetDetectedUnit = null;
            Position = null;

            LaunchedFromUnit.AircraftHangar.Aircraft.Add(this);
            try
            {
                UnitClassWeaponLoads load = UnitClass.WeaponLoads.Single<UnitClassWeaponLoads>(l => l.Name == this.CurrentWeaponLoadName);
                if (load != null)
                {
                    ReadyInSec = load.TimeToReloadHour * 60 * 60;
                }
                else
                {
                    ReadyInSec = LaunchedFromUnit.AircraftHangar.GetStatusChangeTimeSec(
                        GameConstants.AircraftDockingStatus.TankingAndRefitting,
                        GameConstants.AircraftDockingStatus.ReadyForTakeoff);

                }
            }
            catch (Exception)
            {   //default reload time
                ReadyInSec = LaunchedFromUnit.AircraftHangar.GetStatusChangeTimeSec(
                    GameConstants.AircraftDockingStatus.TankingAndRefitting,
                    GameConstants.AircraftDockingStatus.ReadyForTakeoff);
            }
            MissionType = GameConstants.MissionType.Patrol;
            MissionTargetType = GameConstants.MissionTargetType.Undefined;
            AssignedHighLevelOrder = null;
            LaunchedFromUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
            var wpnLoad = GameManager.Instance.GameData.GetWeaponLoadByName(UnitClass.Id, CurrentWeaponLoadName);
            if (wpnLoad != null)
            {
                bool isEnoughAmmoToReload = CanChangeToWeaponLoad(wpnLoad, false);
                if (!isEnoughAmmoToReload)
                {
                    SetWeaponLoad(string.Empty); //default

                }
                foreach (var weapon in Weapons)
                {
                    weapon.ReadyInSec = 0;
                    weapon.IsOperational = true;
                    weapon.AmmunitionRemaining = weapon.MaxAmmunition;
                }
            }
            foreach (var sensor in Sensors)
            {
                sensor.ReadyInSec = 0;
                sensor.IsOperational = true;
                sensor.IsDamaged = false;
                if (sensor.SensorClass.MaxSpeedDeployedKph > 0 || sensor.SensorClass.MaxHeightDeployedM > 0)
                {
                    sensor.IsOperational = false; //if sensor has height/speed restrictions, set non-operational
                }
                //sensor.IsActive = false;

                if (sensor.SensorClass.IsPassiveActiveSensor)
                {
                    sensor.IsActive = false;
                }
            }
            FuelDistanceCoveredSinceRefuelM = 0;
            if (_refuelingFromAircraft != null)
            {
                _refuelingFromAircraft.RemoveFromFuelQueue(this);
            }
            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            return true;
        }

        public override bool ReturnToBase()
        {
            if (base.ReturnToBase())
            {
                // When returning to base, stop any refueling
                StopRefueling();
                return true;
            }
            return false;
        }

        public override void ClearAllWaypoints()
        {
            base.ClearAllWaypoints();

            // Remove from any refueling air as we're no longer moving to it
            if (_refuelingFromAircraft != null)
                _refuelingFromAircraft.RemoveFromFuelQueue(this);
        }

        /// <summary>
        /// Add an aircraft unit to the fuel queue. Also make sure to remove any old aircrafts that's either deleted or has landed.
        /// Aircraft is only added to the queue if we currently have enough fuel to give.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="fuelDistanceM"></param>
        /// <returns></returns>
        public bool AddToFuelQueue(AircraftUnit unit, double fuelDistanceM)
        {
            // Remove units that's marked for deletion
            if (_aircraftsToRefuel != null)
            {
                _aircraftsToRefuel.RemoveAll(pair => pair.Key == null || pair.Key.IsMarkedForDeletion);
            }

            if (GetMaxFuelDistanceToGiveM() >= fuelDistanceM)
            {
                if (_aircraftsToRefuel == null)
                {
                    _aircraftsToRefuel = new List<KeyValuePair<AircraftUnit, double>>();
                }
                _aircraftsToRefuel.Add(new KeyValuePair<AircraftUnit, double>(unit, fuelDistanceM));
                unit._refuelingFromAircraft = this;
                return true;
            }
            return false;
        }

        public void RemoveFromFuelQueue(AircraftUnit unit)
        {
            if (_aircraftsToRefuel != null && unit._refuelingFromAircraft == this)
            {
                _aircraftsToRefuel.RemoveAll(pair => pair.Key.Id == unit.Id);
                unit._refuelingFromAircraft = null;
            }
        }

        /// <summary>
        /// Transfer fuel to another aircraft unit.
        /// </summary>
        /// <param name="unit">The aircraft unit to transfer fuel to.</param>
        /// <param name="fuelDistanceM">The amount of fuel to transfer.</param>
        /// <returns>True if any fuel was transferred.</returns>
        private bool GiveFuel(AircraftUnit unit)
        {
            // Make sure unit is in queue
            if (!IsInFuelQueue(unit))
                return false;

            // Get amount to give
            var fuelDistanceM = _aircraftsToRefuel.Where(pair => pair.Key != null && pair.Key.Id == unit.Id).Sum(pair => pair.Value);

            // Make sure we don't give too much fuel
            fuelDistanceM = fuelDistanceM.Clamp(0.0, GetMaxFuelDistanceToGiveM());
            if (fuelDistanceM > unit.FuelDistanceCoveredSinceRefuelM)
                fuelDistanceM = unit.FuelDistanceCoveredSinceRefuelM;

            // Give fuel to unit
            unit.IsOrderedToReturnToBase = false;
            unit.FuelDistanceCoveredSinceRefuelM -= fuelDistanceM;

            // Remove fuel from ourself
            FuelDistanceCoveredSinceRefuelM += fuelDistanceM;

            // Remove unit from fueling list
            RemoveFromFuelQueue(unit);

            // Also remove units that's marked for deletion
            _aircraftsToRefuel.RemoveAll(
                pair => pair.Key == null || pair.Key.IsMarkedForDeletion);

            // Go back to cruise speed if no more units to refuel
            // TODO: go back to the user defined speed instead
            if (!_aircraftsToRefuel.Any())
            {
                ActualSpeedKph = GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Cruise);
                SetActualSpeed(ActualSpeedKph);
                UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
            }

            SetDirty(GameConstants.DirtyStatus.UnitChanged);
            unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);

            // The unit should check for bingo fuel after refueling as it might be too far away from base.
            //CheckForBingoFuel(); //removed to avoid stack exhausted
            //unit.CheckForBingoFuel();

            return (fuelDistanceM > 0.0);
        }

        public bool IsInFuelQueue(AircraftUnit unit)
        {
            return (_aircraftsToRefuel != null && _aircraftsToRefuel.Any(pair => pair.Key.Id == unit.Id));
        }

        /// <summary>
        /// Refuel the aircraft unit in air.
        /// </summary>
        /// <param name="refuelingAircraft">The aircraft unit to refuel from.</param>
        /// <returns></returns>
        public bool RefuelInAir(AircraftUnit refuelingAircraft)
        {
            // Check if refueling aircraft is within distance
            var distanceToTargetM = MapHelper.CalculateDistanceM(Position.Coordinate, refuelingAircraft.Position.Coordinate);
            if (distanceToTargetM < GameConstants.DISTANCE_TO_TARGET_IS_HIT_M)
            {
                // Make sure we're in the queue
                if (!refuelingAircraft.IsInFuelQueue(this) && !refuelingAircraft.AddToFuelQueue(this, FuelDistanceCoveredSinceRefuelM))
                {
                    return false;
                }
                return refuelingAircraft.GiveFuel(this);
            }

            // Take bingo fuel factor into account
            distanceToTargetM *= GameConstants.BINGO_FUEL_FACTOR;
            if (distanceToTargetM < FuelDistanceRemainingM)
            {
                // Check if refueling aircraft has enough fuel to give us. Make sure to remove from queue before adding
                refuelingAircraft.RemoveFromFuelQueue(this);
                if (refuelingAircraft.AddToFuelQueue(this, FuelDistanceCoveredSinceRefuelM + distanceToTargetM))
                {
                    MissionType = GameConstants.MissionType.Ferry;
                    MissionTargetType = GameConstants.MissionTargetType.Undefined;
                    TargetDetectedUnit = null;
                    IsOrderedToReturnToBase = true;
                    ActualSpeedKph = GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Cruise);
                    SetActualSpeed(ActualSpeedKph); //force afterburners off immediately
                    UserDefinedSpeed = GameConstants.UnitSpeedType.Cruise;
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);

                    // Change speed on refueling aircraft as well
                    refuelingAircraft.ActualSpeedKph = refuelingAircraft.GetSpeedInKphFromSpeedType(GameConstants.UnitSpeedType.Slow);
                    refuelingAircraft.SetActualSpeed(refuelingAircraft.ActualSpeedKph);
                    refuelingAircraft.UserDefinedSpeed = GameConstants.UnitSpeedType.Slow;
                    refuelingAircraft.SetDirty(GameConstants.DirtyStatus.UnitChanged);

                    // Create waypoint to the refueling aircraft and add refuel order
                    if (MovementOrder is MovementFormationOrder)
                    {
                        MovementOrder = new MovementOrder();
                    }
                    var wp = new Waypoint(refuelingAircraft);
                    wp.DesiredDistanceToTargetM = GameConstants.DISTANCE_TO_TARGET_IS_HIT_M; // TODO: set proper distance
                    wp.IsNotRecurring = true;
                    wp.Orders.Add(new BaseOrder() { OrderType = GameConstants.OrderType.RefuelInAir, SecondId = refuelingAircraft.Id });

                    MovementOrder.AddWaypointToTop(wp);
                    ReCalculateEta();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region "Private and protected methods"

        private void StopRefueling()
        {
            // Remove from any refueling air
            if (_refuelingFromAircraft != null)
                _refuelingFromAircraft.RemoveFromFuelQueue(this);

            // Inform any aircraft in refueling queue that we're stopping
            if (_aircraftsToRefuel != null && _aircraftsToRefuel.Any())
            {
                foreach (var unit in _aircraftsToRefuel.Select(pair => pair.Key))
                {
                    unit._refuelingFromAircraft = null;
                    unit.CheckForBingoFuel();
                }
                _aircraftsToRefuel = null;
            }
        }

        private double GetMaxFuelDistanceToGiveM()
        {
            // Make sure we're not returning home
            if (IsOrderedToReturnToBase || IsMarkedForDeletion)
                return 0.0;

            var minFuelDistanceRemainingM = GetDistanceToHomeM() * GetFuelEnduranceModifier() * GameConstants.BINGO_FUEL_FACTOR;
            var maxFuelDistanceToGiveM = FuelDistanceRemainingM - minFuelDistanceRemainingM;

            // Subtract any previously promised fuel
            if (_aircraftsToRefuel != null)
            {
                maxFuelDistanceToGiveM -= _aircraftsToRefuel.Sum(pair => pair.Value);
            }

            if (maxFuelDistanceToGiveM < 0.0)
                maxFuelDistanceToGiveM = 0.0;

            return maxFuelDistanceToGiveM;
        }

        #endregion
    }
}
