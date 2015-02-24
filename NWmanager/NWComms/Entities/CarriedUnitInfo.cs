using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class CarriedUnitInfo
    {
        #region "Constructors"

        public CarriedUnitInfo()
        {
            RoleList = new List<GameConstants.Role>();
            AvailableWeaponLoads = new List<string>();
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string UnitName { get; set; }

        public string UnitClassId { get; set; }

        //public string UnitClassName { get; set; }

        //public int HitPoints { get; set; }

        public int DamagePercent { get; set; }

        public string CarriedByUnitId { get; set; }

        public string CurrentWeaponLoadName { get; set; }

        public List<string> AvailableWeaponLoads { get; set; }

        public List<GameConstants.Role> RoleList { get; set; }

        public double ReadyInSec { get; set; }

        /// <summary>
        /// The maximum range the unit can fly, calculated at cruise speed
        /// </summary>
        public double MaxOperatingDistanceM { get; set; }

        /// <summary>
        /// The maximum operating range at cruise speed, taking into account 
        /// return flight and safety buffer.
        /// </summary>
        public double MaxOperatingRangeM { get; set; }

        public GameConstants.UnitSubType UnitSubType { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = string.Format("{0} {1} [{2}]", 
                Id, UnitName, UnitClassId);
            if (ReadyInSec > 0)
            {
                temp += " Ready in " + (long)ReadyInSec + " sec.";
            }
            else
            {
                temp += " Ready!";
            }
            if (!string.IsNullOrEmpty(CurrentWeaponLoadName))
            {
                temp += " *" + CurrentWeaponLoadName + "* ";
            }
            if (MaxOperatingRangeM > 0)
            {
                temp += string.Format("  Max range: {0:F} km.",MaxOperatingRangeM/1000);
            }
            return temp;
        }

        #endregion


    }
}
