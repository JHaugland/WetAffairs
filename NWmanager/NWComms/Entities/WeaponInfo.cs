using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class WeaponInfo
    {
        #region "Constructors"

        public WeaponInfo()
        {

        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        //public string Name { get; set; }

        //public string ShortName { get; set; }

        public string OwnerUnitId { get; set; }

        //public string OwnerPlayerId { get; set; }

        public bool IsOperational { get; set; }
        
        public bool IsPrimaryWeapon { get; set; }
        
        public double LastFiredGameWorldTimeSec { get; set; }

        public string WeaponClassId { get; set; }

        public int MaxAmmunition { get; set; }

        public int AmmunitionRemaining { get; set; }

        /// <summary>
        /// Bearing relative to unit, which is considered to have its front at 0 deg
        /// </summary>
        public double WeaponBearingDeg { get; set; }

        //public double WeaponPitchDeg { get; set; }

        public double ReadyInSec { get; set; }

        public bool IsDamaged { get; set; }


        #endregion



        #region "Public methods"

        public override string ToString()
        {
            var temp = string.Format("Weapon [{0}] {1}", Id, WeaponClassId);
            if (IsDamaged)
            {
                temp += " Damaged";
            }
            return temp;
        }

        #endregion


    }
}
