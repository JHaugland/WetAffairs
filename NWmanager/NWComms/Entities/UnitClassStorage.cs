using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitClassStorage
    {
        #region "Constructors"

        public UnitClassStorage()
        {

        }

        #endregion


        #region "Public properties"

        public int NumberOfUnits { get; set; }

        public string UnitClassId { get; set; }

        public string WeaponLoadName { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifier { get; set; }

        #endregion



        #region "Public methods"
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(UnitClassId))
            {
                return UnitClassId;
            }
            else
            {
                return "Unnamed UnitClassStorage";
            }

        }
        #endregion


    }
}
