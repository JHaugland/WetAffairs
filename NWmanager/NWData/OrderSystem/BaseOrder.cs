using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;
using System.Runtime.Serialization;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class BaseOrder : GameObject
    {
        private BaseMovableObject _AssignedEntity = null;

        #region "Constructors"

        public BaseOrder() : base() 
        {
            Priority = GameConstants.Priority.Normal;
            //UnitList = new List<string>();
            Orders = new List<BaseOrder>();
            UnitSpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
            ParameterList = new List<string>();
            IsComputerGenerated = true; //default on
        }

        public BaseOrder(Player playerOwner) : this()
        {
            OwnerPlayer = playerOwner;
        }

        
        public BaseOrder(Player playerOwner, GameConstants.OrderType orderType)
            : this()
        {
            OwnerPlayer = playerOwner;
            OrderType = orderType;
        }

        public BaseOrder(Player playerOwner, GameConstants.OrderType orderType, 
            BaseMovableObject assignedEntity)
            : this()
        {
            OwnerPlayer = playerOwner;
            OrderType = orderType;
            AssignedEntity = assignedEntity;
        }

        #endregion


        #region "Public properties"

        public GameConstants.Priority Priority { get; set; }
        
        public Player OwnerPlayer { get; set; }

        public GameConstants.OrderType OrderType { get; set; }

        public GameConstants.SpecialOrders SpecialOrders { get; set; }

        public bool IsComputerGenerated { get; set; }

        //public virtual List<string> UnitList { get; set; }

        public bool RemoveAllExistingWaypoints { get; set; }

        public List<BaseOrder> Orders { get; set; }

        public string Tag { get; set; }

        public string GroupId { get; set; }

        public string StringParameter { get; set; }

        public double ValueParameter { get; set; }

        public bool IsParameter { get; set; }

        public string SecondId { get; set; }

        public GameConstants.UnitSpeedType UnitSpeedType { get; set; }

        public GameConstants.HeightDepthPoints HeightDepthPoints { get; set; }

        public Formation Formation { get; set; }

        public List<string> ParameterList { get; set; }

        public GameConstants.SensorType SensorType { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifier { get; set; }

        public Position Position { get; set; }

        public double RadiusM { get; set; }

        public bool IsAssigned
        {
            get
            {
                return (AssignedEntity != null);
            }
        }

        public BaseMovableObject AssignedEntity
        {
            get
            {
                return _AssignedEntity;
            }
            set
            {
                _AssignedEntity = value;
            }
        }

        #endregion



        #region "Public methods"


        public virtual BaseOrderInfo GetBaseOrderInfo()
        {
            var info = new BaseOrderInfo();
            info.Id = this.Id;
            info.OrderType = this.OrderType;

            return info;
        }

        public virtual Waypoint GetActiveWaypoint()
        {
            return null;
        }

        public BaseOrder Clone()
        {
            BaseOrder newOrder = (BaseOrder) this.MemberwiseClone();
            return newOrder;
        }

        public override string ToString()
        {
            return string.Format("Order {0} Type: {1}", Id, OrderType);
        }
        #endregion




    }
}
