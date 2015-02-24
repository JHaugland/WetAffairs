using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Ai
{
    public class BlackboardFinder<T> where T : IBlackboardObject
    {
        public BlackboardFinder()
        {

        }

        public IBlackboardObject GetClosestObject(Coordinate coordinate)
        {
            double DistanceClosestM = double.MaxValue;
            IBlackboardObject closestObj = null;
            foreach(var obj in BlackboardController.Objects)
            {
                if (obj is T)
                {
                    double ThisDistanceM = obj.DistanceToM(coordinate);
                    if (closestObj == null)
                    {
                        DistanceClosestM = ThisDistanceM;
                        closestObj = obj;
                    }
                    else
                    {
                        if (obj.DistanceToM(closestObj.Coordinate) < DistanceClosestM)
                        {
                            DistanceClosestM = ThisDistanceM;
                            closestObj = obj;
                        }
                    }
                }
            }
            return closestObj;
        }
        public IBlackboardObject GetById(string id)
        {
            return BlackboardController.Objects.Find(o => o.Id == id);
        }

        public IEnumerable<IBlackboardObject> GetAllByType()
        {
            var result = BlackboardController.Objects.FindAll(o => o is T);
            return (IEnumerable<IBlackboardObject>)result;
        }

        public IEnumerable<IBlackboardObject> GetAllByOwner(string ownerId)
        {
            var result = BlackboardController.Objects.FindAll(o => o is T && o.OwnerId == ownerId);
            return (IEnumerable<IBlackboardObject>)result;
        }


        public IEnumerable<IBlackboardObject> GetAllByCoordinate(Coordinate coordinate, double maxDistance)
        {
            IEnumerable<IBlackboardObject> list = from o in BlackboardController.Objects
                                                  where MapHelper.CalculateDistanceM(coordinate, o.Coordinate) <= maxDistance
                                                  select o;
            return (IEnumerable<IBlackboardObject>)list;
        }

        public IEnumerable<IBlackboardObject> GetAllByCoordinateAndOwner(string ownerId, Coordinate coordinate, double maxDistance)
        {
            IEnumerable<IBlackboardObject> list = from o in BlackboardController.Objects
                                                  where MapHelper.CalculateDistanceM(coordinate, o.Coordinate) 
                                                    <= maxDistance && o.OwnerId == ownerId
                                                  select o;
            return (IEnumerable<IBlackboardObject>)list;
        }


        public IEnumerable<IBlackboardObject> GetAllByCoordinateAndType(Coordinate coordinate, double maxDistance)
        {
            var list = from o in BlackboardController.Objects
                       where MapHelper.CalculateDistanceM(coordinate, o.Coordinate) <= maxDistance
                       && o is T
                       select o;
            return list;
        }

        public IEnumerable<IBlackboardObject> GetAllSortedByCoordinateAndType(Coordinate coordinate, double maxDistance)
        {
            var list = from o in BlackboardController.Objects
                       where o.Coordinate != null && MapHelper.CalculateDistanceM(coordinate, o.Coordinate) <= maxDistance
                       && o is T
                       orderby MapHelper.CalculateDistanceM(coordinate, o.Coordinate) ascending
                       select o;
            return list;
        }


        public IEnumerable<IBlackboardObject> GetAllByCoordinateOwnerAndType(string ownerId, Coordinate coordinate, double maxDistance)
        {
            var list = from o in BlackboardController.Objects
                       where MapHelper.CalculateDistanceM(coordinate, o.Coordinate) <= maxDistance
                       && o is T && o.OwnerId == ownerId
                       select o;
            return list;
        }

        /// <summary>
        /// Return all matching objects sorted by distance to coordinate, closest first.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="coordinate"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public IEnumerable<IBlackboardObject> GetAllSortedByCoordinateOwnerAndType(string ownerId, Coordinate coordinate, double maxDistance)
        {
            var list = from o in BlackboardController.Objects
                       where MapHelper.CalculateDistanceM(coordinate, o.Coordinate) <= maxDistance
                       && o is T && o.OwnerId == ownerId
                       orderby MapHelper.CalculateDistanceM(coordinate, o.Coordinate) ascending
                       select o;
            return list;

        }

    }
}
