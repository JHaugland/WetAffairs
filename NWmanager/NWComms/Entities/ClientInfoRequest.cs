using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms;

namespace TTG.NavalWar.NWComms.Entities
{
    /// <summary>
    /// This is the object passed between server and client to request specific information,
    /// like GameInfo, PlayerInfo, etc
    /// </summary>
    /// 
    [Serializable]
    public class ClientInfoRequest : IMarshallable
    {
        #region "Constructors"
        public ClientInfoRequest()
        {

        }


        #endregion

        #region "Public properties"

        public CommsMarshaller.ObjectTokens RequestObjectType { get; set; }

        public string Id { get; set; }


        #endregion

        #region "Public static methods"
        
        public static ClientInfoRequest CreateRequestObject(CommsMarshaller.ObjectTokens objectType, string Id)
        {
            ClientInfoRequest req = new ClientInfoRequest {RequestObjectType = objectType, Id = Id};
            return req;
        }
        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return RequestObjectType.ToString() + "Id=" + Id;
        }
        
        #endregion

        #region "Private methods"
        #endregion


        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.ClientInfoRequest; }
        }

        #endregion
    }
}
