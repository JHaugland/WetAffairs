using System;
using System.Collections.Generic;

using System.Text;
//using TTG.NavalWar.NWData;
//using TTG.NavalWar.NWData.GamePlay;
//using TTG.NavalWar.NWData.Util;
//using TTG.NavalWar.NWData.OrderSystem;
//using TTG.NavalWar.NWData.Units;
//using TTG.NavalWar.NWComms;
//using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameControlRequest : IMarshallable
    {
        #region "Constructors"

        public GameControlRequest()
        {

        }

        #endregion


        #region "Public properties"

        public CommsMarshaller.GameControlRequestType ControlRequestType { get; set; }

        public string Id { get; set; }

        public string ControlParameterString { get; set; }

        public double ControlParameterValue { get; set; }

        public PositionInfo Position { get; set; }

        public bool IsParameter { get; set; }

        public GameConstants.FriendOrFoe FriendOrFoeDesignation { get; set; }

        public GameConstants.WeaponOrders WeaponOrders { get; set; }

        public GameConstants.SendMessageTo SendMessageTo { get; set; }

        #endregion



        #region "Public static methods"

        public static GameControlRequest CreateControlRequestObject(CommsMarshaller.GameControlRequestType controlRequestType, 
            string id, string controlParameterString, double controlParameterValue)
        {
            GameControlRequest req = new GameControlRequest
                                         {
                                             ControlRequestType = controlRequestType,
                                             Id = id,
                                             ControlParameterValue = controlParameterValue,
                                             ControlParameterString = controlParameterString
                                         };
            return req;
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameControlRequest; }
        }

        #endregion
    }
}
