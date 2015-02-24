using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class WeaponStoreEntry 
    {
        #region "Constructors"

        public WeaponStoreEntry()
        {
            CanChangeToThisLoad = true;
        }

        public WeaponStoreEntry(string weaponClassId, int count)
            : this()
        {
            WeaponClassId = weaponClassId;
            Count = count;
        }

        #endregion


        #region "Public properties"

        public string WeaponClassId { get; set; }

        public int Count { get; set; }

        public bool CanChangeToThisLoad { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return WeaponClassId + " " + Count;
        }
        #endregion


    }
}
