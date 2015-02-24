using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitClassVesselName
    {
        #region "Constructors"

        public UnitClassVesselName()
        {

        }

        #endregion


        #region "Public properties"

        public virtual string UnitName { get; set; }

        public virtual string UnitDesignation { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return UnitName + " [" + UnitDesignation + "]";
        }
        #endregion


    }
}
