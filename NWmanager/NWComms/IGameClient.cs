
namespace TTG.NavalWar.NWComms
{
    public delegate void ClientDataReceivedDelegate( IMarshallable dataReceived );

    public interface IGameClient
    {
        event ClientDataReceivedDelegate DataReceived;
        event ConnectionStatusChangedDelegate ConnectionStatusChanged;

        string Host { get; set; }

        int Port { get; set; }

        GameConstants.ConnectionStatusEnum ConnectionStatus { get; set; }

        void Init();
        void Disconnect();
        void PollNetwork();
        void Send( IMarshallable obj );
        void Send( string message );
    }
}
