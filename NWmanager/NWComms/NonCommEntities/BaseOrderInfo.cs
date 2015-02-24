using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class BaseOrderInfo
    {
        #region "Constructors"

        public BaseOrderInfo()
        {
            Waypoints = new List<WaypointInfo>();
        }

        public GameConstants.OrderType OrderType { get; set; }

        public string Id { get; set; }
        
        //public string UnitId { get; set; }

        public string TargetId { get; set; }

        public string TargetDescription { get; set; }

        //public PositionInfo Position { get; set; }

        public List<WaypointInfo> Waypoints { get; set; }

        #endregion


        #region "Public properties"
        public override string ToString()
        {
            return "Order " + OrderType;
        }
        #endregion



        #region "Public methods"

        #endregion

    }
}
