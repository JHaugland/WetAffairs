using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTG.NavalWar.NWComms
{
    public delegate void ServerDataReceivedDelegate( int clientID, IMarshallable dataReceived );
    public delegate void ConnectionStatusChangedDelegate( GameConstants.ConnectionStatusEnum status );
    public delegate void ConnectionDelegate( int clientId );

    public interface IGameServer
    {
        event ServerDataReceivedDelegate DataReceived;
        event ConnectionStatusChangedDelegate ConnectionStatusChanged;
        event ConnectionDelegate ClientAdded;
        event ConnectionDelegate ClientRemoved;

        int Port { get; set; }
        bool IsOpenForNewConnecions { get; set; }
        GameConstants.ConnectionStatusEnum ConnectionStatus { get; set; }

        void Init();
        void Disconnect();
        void Send( int clientId, IMarshallable message );
        void PollNetwork();
    }
}
