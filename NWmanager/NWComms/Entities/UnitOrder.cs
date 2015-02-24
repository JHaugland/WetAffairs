using System;
using System.Collections.Generic;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitOrder : IMarshallable
    {
        #region "Constructors"

        public UnitOrder()
        {
            ParameterList = new List<string>();
            UnitOrders = new List<UnitOrder>();
            UnitSpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
        }

        public UnitOrder(GameConstants.UnitOrderType unitOrderType, string id) : this()
        {
            UnitOrderType = unitOrderType;
            Id = id;
        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public GameConstants.UnitOrderType UnitOrderType { get; set; }

        public string SecondId { get; set; }

        public List<string> ParameterList { get; set; }

        public List<UnitOrder> UnitOrders { get; set; }

        public bool RemoveAllExistingWaypoints { get; set; }

        public double ValueParameter { get; set; }

        public string StringParameter { get; set; }

        public bool IsParameter { get; set; }

        public Formation Formation { get; set; }

        public double RadiusM { get; set; }

        public GameConstants.SpecialOrders SpecialOrder { get; set; }

        public GameConstants.UnitSpeedType UnitSpeedType { get; set; }

        public GameConstants.HeightDepthPoints HeightDepthPoints { get; set; }

        public GameConstants.FriendOrFoe FriendOrFoeDesignation { get; set; }

        public GameConstants.SensorType SensorType { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifier { get; set; }

        public PositionInfo Position { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return string.Format("UnitOrder: {0} for unit {1}", UnitOrderType, Id);
        }

        #endregion

        #region IMarshallable Members

        public virtual CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.UnitOrder; }
        }

        #endregion
    }
}
