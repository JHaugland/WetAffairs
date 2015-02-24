using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class WaypointInfo : IMarshallable
    {
        #region "Constructors"

        public WaypointInfo()
        {
            UnitOrders = new List<UnitOrder>();
        }
        
        public WaypointInfo(PositionInfo position) : this()
        {
            Position = position;
        }

        public WaypointInfo(PositionInfo position, List<UnitOrder> unitOrders)
            : this()
        {
            Position = position;
            UnitOrders = unitOrders;
        }

        #endregion


        #region "Public properties"

        public PositionInfo Position { get; set; }

        public List<UnitOrder> UnitOrders { get; set; }

        public string TargetDetectedUnitId { get; set; }

        public string ReturningToUnitId { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            if (Position != null)
            {
                return string.Format(
                    "Waypoint destination: {0}. {1} Order(s)", Position.ToString(), UnitOrders.Count);
            }
            else
            {
                return "Waypoint with no position";
            }
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.WaypointInfo; }
        }

        #endregion
    }
}
