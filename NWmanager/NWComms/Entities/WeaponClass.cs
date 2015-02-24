using System;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class WeaponClass: IMarshallable, IGameDataObject
    {
        //Weapon type, ie Tomahawk missile or Gatling gun
        public WeaponClass()
        {
            MaxSeaState = 9; //default
            ReferenceAgilityFactor = 1;
            AcquisitionCostAmmoCredits = GameConstants.DEFAULT_AMMO_COST;
            CanRetargetAfterLaunch = true;
        }


        #region "Public properties"

        public string Id { get; set; }

        /// <summary>
        /// In some cases, e.g. for sonobuoy dispensers, a BaseWeapon object is not 
        /// really a weapon that can be fired. Set this property to true for such objects.
        /// </summary>
        public bool IsNotWeapon { get; set; }

        public string WeaponClassName { get; set; }

        public string WeaponClassShortName { get; set; }

        public double MaxWeaponRangeM { get; set; }

        public double MinWeaponRangeM { get; set; }

        /// <summary>
        /// For weapons that spawns a Unit, the max speed for the missile/torpedo
        /// </summary>
        public double MaxSpeedKph { get; set; }

        /// <summary>
        /// Referring to the AgilityFactor on target UnitClass, the highest value that this weapon
        /// can hit at HitPercent probability. If AgilityFactor is equal to or lower than that of
        /// the target UnitClass, HitPercent applies. If unit agility is 1 higher, hit chance is halved.
        /// If unit agility is 2 higher, weapon cannot hit.
        /// </summary>
        public byte ReferenceAgilityFactor { get; set; }

        public double HighestOperatingHeightM { get; set; }

        public double LowestOperatingHeightM { get; set; }

        public GameConstants.WeaponTrajectoryType WeaponTrajectory { get; set; }

        ///// <summary>
        ///// Maximum speed of owner platform where this weapon can be fired. If 0, it is ignored.
        ///// </summary>
        //public double MaxSpeedFiredKph { get; set; }

        public double EffectiveWeaponRangeM { get; set; }

        /// <summary>
        /// Range of weapons mount, relative to WeaponHardpoint.WeaponBearingDeg. 360 means it can fire in any
        /// direction. 30 means 15 degrees to each side of weapon bearing.
        /// </summary>
        public virtual double WeaponBearingRangeDeg { get; set; }

        public virtual bool CanFireOnUnit { get; set; }

        public virtual bool CanFireOnPosition { get; set; }

        public virtual bool CanFireBearingOnly { get; set; }

        /// <summary>
        /// Whether the fired missile can itself be targetted by missiles and/or pointdefense.
        /// </summary>
        public virtual bool CanBeTargetted { get; set; }

        public virtual int HitPercent { get; set; }

        public GameConstants.SpecialOrders SpecialOrders { get; set; }

        public bool SpawnsUnitOnFire { get; set; }

        public string SpawnUnitClassId { get; set; }

        /// <summary>
        /// For weapons that spawn units, generated unitclasses will get this UnitModelFileName when created.
        /// </summary>
        public string UnitModelFileName { get; set; }

        /// <summary>
        /// Only for anti-submarine rockets and other weapons where a missile carries a torpedo or similar.
        /// </summary>
        public string SecondarySpawnUnitClassId { get; set; }

        //internal WeaponClass SpawnUnitClass 
        //{
        //    get
        //    {
        //        //TODO: Get from Unitclasses. 
        //        //Problem: Can't reference Game since that creates dependency to Linq-based assembly
        //        return new WeaponClass();
        //    }
        //} 

        public bool CanTargetAir { get; set; }

        public bool CanTargetSurface { get; set; }

        public bool CanTargetSubmarine { get; set; }

        public bool CanTargetLand { get; set; }

        public bool IsECM { get; set; } //electronic warfare

        public bool IsAntiRadiationWeapon { get; set; }

        public bool CanRetargetAfterLaunch { get; set; }

        public GameConstants.EwCounterMeasuresType EwCounterMeasures { get; set; } //requires IsECM to be true

        /// <summary>
        /// For a normal weapon, determines weapon's ability to avoid being soft killed. If 0, it has no effect. If > 0, the
        /// HitPercent of the EW system is multiplied with this percentage. Ie if 50, EW hit chance is reduced by 50%.
        /// </summary>
        public double EwCounterMeasureResistancePercent { get; set; }

        public virtual bool RequiresPlatformFireControl { get; set; }

        public int MaxSimultanousShots { get; set; }

        /// <summary>
        /// For non-weapons like jammers, how long, in seconds, the effect lasts.
        /// </summary>
        public double MaxEffectDurationSec { get; set; }

        public double StrengthPercent { get; set; }

        public double EffectRangeM { get; set; }

        public int MaxAmmunition { get; set; }

        public double TimeBetweenShotsSec { get; set; }

        public int DamageHitPoints { get; set; }

        public double IncreasesMassByKg { get; set; }

        public double IncreasesRangeByPercent { get; set; }

        /// <summary>
        /// If > 0, a missile will only use its max speed in the set number of meters approaching the target
        /// </summary>
        public int TerminalSpeedRangeM { get; set; }

        public int AcquisitionCostAmmoCredits { get; set; }

        /// <summary>
        /// Maximum sea state for being fired.
        /// </summary>
        public int MaxSeaState { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(WeaponClassName))
            {
                return WeaponClassName;
            }
            else
            {
                return "Unnamed WeaponClass";
            }

        }

        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.WeaponClass; }
        }

        #endregion




    }
}
