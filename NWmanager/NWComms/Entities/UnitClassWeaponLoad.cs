using System;
using System.Collections.Generic;

using System.Text;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitClassWeaponLoad
    {
        
        public UnitClassWeaponLoad()
        {
        }

        #region "Public Methods"

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(WeaponClassId))
            {
                return WeaponClassId;
            }
            else
            {
                return "Unnamed WeaponLoad";
            }

        }
        #endregion

        #region "Public properties"

        /// <summary>
        /// WeaponClassId of this weapon.
        /// </summary>
        public virtual string WeaponClassId { get; set; }

        /// <summary>
        /// Position of weapon relative to center of unit, in meters. X is across the width of unit.
        /// </summary>
        public virtual double XPosition { get; set; }

        /// <summary>
        /// Position of weapon relative to center of unit, in meters. Y is along the length of the unit.
        /// </summary>
        public virtual double YPosition { get; set; }

        /// <summary>
        /// Height of weapon on unit, above sea level on ships, or bottom of unit body on aircraft.
        /// </summary>
        public virtual double HeightPosition { get; set; }

        /// <summary>
        /// Direction weapon mount points, in degrees (0-360). 0 is forward on unit.
        /// </summary>
        public virtual double WeaponBearingDeg { get; set; }

        /// <summary>
        /// Pitch in degrees, giving the initial pitch and direction of the fired projectile.
        /// 0 is the forward direction of the baseunit.
        /// </summary>
        public virtual double WeaponPitchDeg { get; set; }

        /// <summary>
        /// Max ammunition. If not 0, overrides WeaponClass.MaxAmmunition if lower.
        /// </summary>
        public int MaxAmmunition { get; set; }

        /// <summary>
        /// Whether this weapon is a primary weapon used for offensive operations. A unit 
        /// or group will only close range to engage with primary weapons.
        /// </summary>
        public bool IsPrimaryWeapon { get; set; }


        #endregion
    }
}
