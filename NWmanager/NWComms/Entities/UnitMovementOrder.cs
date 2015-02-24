using System;
using System.Collections.Generic;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitMovementOrder: UnitOrder, IMarshallable
    {
        #region "Constructors"

        public UnitMovementOrder() : base()
        {
            Waypoints = new List<WaypointInfo>();
            UnitOrderType = GameConstants.UnitOrderType.MovementOrder;
        }

        #endregion

        #region "Public properties"

        public List<WaypointInfo> Waypoints { get; set; }

        #endregion

        #region IMarshallable Members

        public override CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.UnitMovementOrder; }
        }

        #endregion
    }
}
