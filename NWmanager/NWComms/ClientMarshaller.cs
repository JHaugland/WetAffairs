using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TTG.NavalWar.NWComms
{
    public class ClientMarshaller
    {
        #region "Constructors"
        
        public ClientMarshaller()
        {

        }

        public ClientMarshaller(int clientId, TcpClient client, CommsMarshaller marshaller)
        {
            ClientId = clientId;
            Client = client;
            Marshaller = marshaller;
        }
        
        #endregion

        #region "Public properties"

        public int ClientId { get; set; }

        public TcpClient Client { get; set; }

        public CommsMarshaller Marshaller { get; set; }

        #endregion
    }
}
