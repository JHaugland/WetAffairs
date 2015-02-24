using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class MovementOrder : BaseOrder
    {
        #region "Constructors"

        private readonly FlexQueue<Waypoint> _waypoints = new FlexQueue<Waypoint>();

        public MovementOrder() : base()
        {
            OrderType = TTG.NavalWar.NWComms.GameConstants.OrderType.MovementOrder;
        }

        public MovementOrder(Waypoint waypoint)
            : this()
        {
            this.AddWaypoint(waypoint);
        }
        #endregion


        #region "Public properties"

        public virtual Waypoint this[int index] //Default index properties... Hmmm...
        {
            get
            {
                return _waypoints.ElementAt(index);
            }
        }

        public virtual bool HasMoreWaypoints
        {
            get
            {
                return (_waypoints.Count > 0);
            }
        }

        public virtual bool IsRecurring { get; set; }

        public virtual int CountWaypoints
        {
            get
            {
                return _waypoints.Count;
            }
        }

        #endregion

        #region "Public methods"

        public override BaseOrderInfo GetBaseOrderInfo()
        {
            var info = base.GetBaseOrderInfo();
            foreach (var wp in _waypoints.Where(wp => wp != null && wp.Position != null))
            {
                info.Waypoints.Add(wp.GetWaypointInfo());
            }
            return info;
        }

        public void ClearAllWaypoints()
        {
            IsRecurring = false;
            _waypoints.Clear();
        }

        public void RemoveEvasionWaypoints()
        {
            for (var i = 0; i < _waypoints.Count; )
            {
                var wp = _waypoints.ElementAt(i);
                if (wp.IsAutomaticEvasionPoint)
                {
                    _waypoints.Remove(wp);
                }
                else
                {
                    i++;
                }
            }
        }

        public override Waypoint GetActiveWaypoint()
        {
            var wp = PeekNextWaypoint();

            // If Position is null in waypoint, remove it
            while (wp != null && wp.Position == null)
            {
                RemoveFirstWaypoint();
                wp = PeekNextWaypoint();
            }

            return wp;
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            if (waypoint != null)
            {
                _waypoints.Enqueue(waypoint);
            }
        }

        public void AddWaypoint(Position position)
        {
            Waypoint wp = new Waypoint(position);
            AddWaypoint(wp);
        }

        public void AddWaypoint(PositionInfo positionInfo)
        {
            Waypoint wp = new Waypoint(positionInfo);
            AddWaypoint(wp);
        }

        public void AddWaypoint(WaypointInfo waypointInfo)
        {
            Waypoint wp = new Waypoint(waypointInfo);
            foreach (var order in waypointInfo.UnitOrders)
            { 
                BaseOrder baseOrder = GameManager.Instance.Game.GetBaseOrderFromUnitOrder(order);
                if (baseOrder != null)
                {
                    wp.Orders.Add(baseOrder);
                }
            }
            AddWaypoint(wp);
        }

        public void AddWaypoint(Position position, Orders<BaseOrder> orders)
        {
            Waypoint wp = new Waypoint(position);
            wp.Orders = orders;
            AddWaypoint(wp);
        }

        public void AddWaypoint(DetectedUnit targetDetectedUnit, double effectiveRangeM, Orders<BaseOrder> orders)
        {
            Waypoint wp = new Waypoint(targetDetectedUnit);
            wp.Orders = orders;
            wp.DesiredDistanceToTargetM = effectiveRangeM;
            AddWaypoint(wp);
        }

        public void AddWaypointToTop(Waypoint waypoint)
        {
            _waypoints.AddFirst(waypoint);
        }

        public void AddWaypointToTop(Position position)
        {
            Waypoint wp = new Waypoint(position);
            _waypoints.AddFirst(wp);
        }

        public void AddWaypointToTop(Position position, Orders<BaseOrder> orders)
        {
            Waypoint wp = new Waypoint(position);
            wp.Orders = orders;
            _waypoints.AddFirst(wp);
        }

        public void AddWaypointToTop(DetectedUnit targetDetectedUnit, double effectiveRangeM, Orders<BaseOrder> orders)
        {
            Waypoint wp = new Waypoint(targetDetectedUnit);
            wp.Orders = orders;
            wp.DesiredDistanceToTargetM = effectiveRangeM;
            _waypoints.AddFirst(wp);
        }

        public virtual Waypoint PeekNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0)
            {
                return null;
            }
            return _waypoints.Peek();
        }

        public virtual Waypoint ActivateNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0)
            {
                return null;
            }
            if (!IsRecurring)
            {
                _waypoints.RemoveFirst();
                //GameManager.Instance.Log.LogDebug(
                //    string.Format("Waypoint Id={0},  removed from MovementOrder {1}. Remaining wp count={2}", 
                //    Id, this.ToString(), _waypoints.Count));
            }
            else
            {
                var wp2 = _waypoints.Dequeue();
                if (_waypoints.Count > 0 && !wp2.IsNotRecurring) //do not enqueue same waypoint if no other remaining
                {
                    if (wp2.TargetDetectedUnit == null) //do not re-enqueue targetting waypoints
                    {
                        _waypoints.Enqueue(wp2);
                    }
                }
            }
            return GetActiveWaypoint();
        }

        public bool RemoveWaypoint(Waypoint wp)
        {
            return _waypoints.Remove(wp);
        }

        public void RemoveFirstWaypoint()
        {
            if (_waypoints.Count > 0)
            {
                _waypoints.RemoveFirst();
            }
        }

        public virtual List<Waypoint> GetWaypoints()
        {
            var wayPointList = _waypoints.ToList();
            return wayPointList;
        }

        public override string ToString()
        {
            string temp = string.Format(
                "MovementOrder {0}, {1} waypoint(s).", Id, _waypoints.Count);
            if (IsRecurring)
            {
                temp += " Recurring.";
            }
            Waypoint wp = this.GetActiveWaypoint();
            if(wp != null)
            {
                temp+= "\nCurrent waypoint: " + wp.ToString();
            }
            return temp;
        }

        #endregion
    }
}
