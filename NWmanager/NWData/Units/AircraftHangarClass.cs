using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class AircraftHangarClass
    {
        #region "Constructors"

        public AircraftHangarClass()
        {

        }

        #endregion


        #region "Public properties"

        public virtual string Id { get; set; }

        public virtual int MaxAircraftReadyForTakeoff { get; set; }

        public virtual int MaxAircraftTanking { get; set; }

        public virtual int MaxAircraftStorage { get; set; }

        public virtual int MaxAircraft
        {
            get
            {
                return MaxAircraftReadyForTakeoff + MaxAircraftStorage + MaxAircraftTanking;
            }
        }

        #endregion



        #region "Public methods"

        #endregion


    }
}
