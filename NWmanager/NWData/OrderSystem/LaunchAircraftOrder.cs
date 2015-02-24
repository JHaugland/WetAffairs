using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class LaunchAircraftOrder : BaseOrder
    {
        #region "Constructors"

        public LaunchAircraftOrder() : base()
        {
            OrderType = GameConstants.OrderType.LaunchOrder;
            UnitList = new List<AircraftUnit>();
        }

        public LaunchAircraftOrder(Player ownerPlayer)
            : this()
        {
            this.OwnerPlayer = ownerPlayer;
        }
        #endregion


        #region "Public properties"

        public List<AircraftUnit> UnitList { get; set; }

        #endregion



        #region "Public methods"

        #endregion


    }
}
