using System;
using System.Linq;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Waypoint //: GameObject
    {
        private Position _Position;
        private Orders<BaseOrder> _Orders = new Orders<BaseOrder>();

        #region "Constructors"

        public Waypoint() : base()
        {
            HasOrdersBeenExecuted = false;
            DesiredDistanceToTargetM = GameConstants.DISTANCE_TO_TARGET_IS_HIT_M;       // default min distance before waypoint is considered to be reached
        }

        public Waypoint(Coordinate newCoord): this() 
        {
            _Position = new Position(newCoord);
        }

        public Waypoint(Position newPos): this()
        {
            _Position = newPos.Clone();
        }

        public Waypoint(WaypointInfo waypointInfo)
            : this()
        {
            Position = new Position(waypointInfo.Position);
            if ( !string.IsNullOrEmpty( waypointInfo.ReturningToUnitId ) )
            {
                BaseUnit returnToUnit = GameManager.Instance.Game.GetUnitById( waypointInfo.ReturningToUnitId );
                if ( returnToUnit != null )
                {
                    this.TargetBaseUnit = returnToUnit;
                }
            }
            else if ( !string.IsNullOrEmpty( waypointInfo.TargetDetectedUnitId ) )
            {
                DetectedUnit det = GameManager.Instance.Game.GetDetectedUnitById( waypointInfo.TargetDetectedUnitId );
                if ( det != null )
                {
                    this.TargetDetectedUnit = det;
                }
            }
            foreach ( var order in waypointInfo.UnitOrders )
            {
                BaseOrder baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder( order );
                this.Orders.Add( baseOrder );
            }
        }

        public Waypoint(DetectedUnit targetDetectedUnit) : this()
        {
            _Position = targetDetectedUnit.Position.Clone();
            TargetDetectedUnit = targetDetectedUnit;
        }

        public Waypoint(BaseUnit targetReturnToBaseUnit)
            : this()
        {
            _Position = targetReturnToBaseUnit.Position.Clone();
            TargetBaseUnit = targetReturnToBaseUnit;
        }

        public Waypoint(PositionInfo positionInfo)
            : this()
        {
            _Position = new Position(positionInfo);
        }

        #endregion

        #region "Public properties"

        public DetectedUnit TargetDetectedUnit { get; set; }

        public double DesiredDistanceToTargetM { get; set; }

        public BaseUnit TargetBaseUnit { get; set; }

        public Position Position
        {
            get
            {
                if (TargetBaseUnit != null)
                {
                    if (TargetBaseUnit.IsMarkedForDeletion || TargetBaseUnit.Position == null)
                    {
                        TargetBaseUnit = null;
                        _Position = null;
                    }
                    else
                    {
                        _Position = TargetBaseUnit.Position.Clone();
                    }
                } 
                else if (TargetDetectedUnit != null)
                {
                    if (TargetDetectedUnit.IsMarkedForDeletion || TargetDetectedUnit.Position == null)
                    {
                        TargetDetectedUnit = null;
                        _Position = null;//hmm.
                    }
                    else if (TargetDetectedUnit.IsFixed 
                        && TargetDetectedUnit.RefersToUnit != null 
                        && TargetDetectedUnit.RefersToUnit.Position != null)
                    {
                        _Position = TargetDetectedUnit.RefersToUnit.Position.Clone();
                    }
                    else
                    {
                        _Position = TargetDetectedUnit.Position.Clone();
                    }
                }
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        public bool IsNotRecurring { get; set; }

        /// <summary>
        /// Set to true for waypoints generated when automatically evading
        /// </summary>
        public bool IsAutomaticEvasionPoint { get; set; }

        public bool HasOrdersBeenExecuted { get; set; }

        /// <summary>
        /// Orders associated with waypoint
        /// </summary>
        public Orders<BaseOrder> Orders 
        {
            get
            {
                return _Orders;
            }
            set
            {
                _Orders = value;
            }
        }

        #endregion

        #region "Public methods"

        public PositionInfo GetPositionInfo()
        {
            if (Position != null)
            {
                return Position.GetPositionInfo();
            }
            return null;
        }

        public bool HasEngagementOrder()
        {
            if (Orders == null || Orders.Count < 1)
            {
                return false;
            }
            return Orders.OfType<EngagementOrder>().Any();
        }

        public WaypointInfo GetWaypointInfo()
        {
            WaypointInfo info = new WaypointInfo();
            info.Position = GetPositionInfo();
            if ( TargetDetectedUnit != null )
            {
                info.TargetDetectedUnitId = TargetDetectedUnit.Id;
            }
            if ( TargetBaseUnit != null )
            {
                info.ReturningToUnitId = TargetBaseUnit.Id;
            }
            return info;
        }
        public Waypoint Clone()
        {
            Waypoint wp = (Waypoint)this.MemberwiseClone();
            wp.Position = this.Position.Clone();
            wp.Orders = new Orders<BaseOrder>();
            foreach (var o in this.Orders)
            {
                wp.Orders.Add(o.Clone());
            }
            return wp;
        }

        public override string ToString()
        {
            string temp = "[WP] ";
            if (Position != null)
            {
                temp += "Pos: " + Position.ToString();
            }
            if (this.TargetDetectedUnit != null)
            {
                temp += " TargetDetectedUnit: " + TargetDetectedUnit.ToString();
            }
            if (this.TargetBaseUnit != null)
            {
                temp += " TargetBaseUnit: " + TargetBaseUnit.ToString();
            }
            if (this.Orders.Count > 0)
            {
                temp += "  (has " + this.Orders.Count + " orders)";
            }
            return temp;
        }

        #endregion
    }
}
