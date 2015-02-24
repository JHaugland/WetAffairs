using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Net;
using System.Net.Sockets;

namespace TTG.NavalWar.NWComms
{
    public class GameServer : IGameServer
    {
        #region "Private fields, consts and enums; public events"

        //http://codeidol.com/csharp/csharp-network/Using-Threads/Using-Threads-in-a-Server/
        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 2055;

        private int _clientIndex = 1;
        private readonly Logger _logger;

        public event ServerDataReceivedDelegate DataReceived;
        public event ConnectionStatusChangedDelegate ConnectionStatusChanged;
        public event ConnectionDelegate ClientAdded;
        public event ConnectionDelegate ClientRemoved;

        private TcpListener _listener = null;
        private readonly List<ClientMarshaller> _clients = new List<ClientMarshaller>();
        private GameConstants.ConnectionStatusEnum _connectionStatus;
        private readonly CommsMarshaller _commsMarshaller = new CommsMarshaller();

        private readonly Byte[] _readBuffer = new Byte[CommsMarshaller.DEFAULT_BUFFER_SIZE];

        #endregion

        #region "Constructors"

        public GameServer()
        {
            _logger = new Logger();
            Port = DEFAULT_PORT;
        }

        public GameServer(int port)
        {
            _logger = new Logger();
            Port = port;
        }

        #endregion

        #region "Public properties"

        public int Port { get; set; }

        public IList<ClientMarshaller> Clients
        {
            get
            {
                return _clients.AsReadOnly();
            }
        }

        public GameConstants.ConnectionStatusEnum ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                if (value != _connectionStatus)
                {
                    _connectionStatus = value;
                    ConnectionStatusChanged(_connectionStatus);
                }
            }
        }

        public bool IsOpenForNewConnecions { get; set; }

        #endregion

        #region "Public methods"

        public void Init()
        {
            _logger.LogDebug("************* GameServer Init Start **************");
            Disconnect();
            IPAddress LocalHost = IPAddress.Any; // Parse(DEFAULT_HOST);
            _listener = new TcpListener(LocalHost, Port); //LocalHost, Port
            _listener.Start();
            IsOpenForNewConnecions = true;
            _logger.LogDebug("************* GameServer Init End **************");
        }

        public void Disconnect()
        {
            foreach (var client in _clients)
            {
                if (client.Client != null)
                {
                    client.Client.Close();
                }
            }
            _clients.Clear();
            _clientIndex = 1;
            if (_listener != null)
            {
                _listener.Stop();
                _listener = null;
            }
            ConnectionStatus = GameConstants.ConnectionStatusEnum.Disconnected;
            IsOpenForNewConnecions = false;
        }

        public void Send(int clientId, IMarshallable message)
        {
            TcpClient client = GetClientById(clientId);
            if (client != null)
            {
                Send(client, message);
            }
        }

        public void PollNetwork()
        {
            if (_listener == null)
            {
                return;
            }

            if (IsOpenForNewConnecions && _listener.Pending())
            {
                TcpClient newClient = _listener.AcceptTcpClient();
                var clientMar = new ClientMarshaller(_clientIndex, newClient, new CommsMarshaller());
                _clients.Add(clientMar);
                ConnectionStatus = GameConstants.ConnectionStatusEnum.Connected;
                ClientAdded(_clientIndex);
                var msg = new MessageString { Message = "Server: You are client number " + _clientIndex };
                Send(newClient, msg);
                _clientIndex++;
                _logger.LogDebug("PollNetwork: New client " + _clientIndex + " added.");
            }

            if (ConnectionStatus == GameConstants.ConnectionStatusEnum.Connected)
            {
                try
                {
                    foreach (var clientMarshaller in _clients)
                    {
                        TcpClient client = clientMarshaller.Client;
                        int clientId = clientMarshaller.ClientId;
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            if (stream.DataAvailable)
                            {
                                do
                                {
                                    int i = stream.Read(_readBuffer, 0, _readBuffer.Length);

                                    clientMarshaller.Marshaller.AddToReceiveBufferEnd(_readBuffer, i);

                                    while (clientMarshaller.Marshaller.IsObjectInReceiveBuffer())
                                    {
                                        IMarshallable obj = clientMarshaller.Marshaller.DeSerializeNextObjectInReceiveBuffer();
                                        _logger.LogDebug("PollNetwork: Object read from receive buffer: " + obj.ObjectTypeToken);
                                        DataReceived(clientId, obj);
                                    }
                                } while (stream.DataAvailable);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException == null)
                            {
                                _logger.LogError("Error in GameServer.PollNetwork: " + ex.ToString() +
                                    " \nStackTrace: " + ex.StackTrace);
                            }
                            else
                            {
                                _logger.LogError("Error in GameServer.PollNetwork: " + ex.ToString() +
                                    "\nInnerException: " + ex.InnerException.ToString() +
                                    "\nStackTrace: " + ex.StackTrace);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        _logger.LogError("Error in GameServer.PollNetwork: " + ex.ToString() +
                            " \nStackTrace: " + ex.StackTrace);
                    }
                    else
                    {
                        _logger.LogError("Error in GameServer.PollNetwork: " + ex.ToString() +
                            "\nInnerException: " + ex.InnerException.ToString() +
                            "\nStackTrace: " + ex.StackTrace);
                    }
                }
            }

        }

        #endregion

        #region Private Methods

        private TcpClient GetClientById(int clientId)
        {
            try
            {
                return (from kvp in _clients where kvp.ClientId == clientId select kvp.Client).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Send(TcpClient client, IMarshallable message)
        {
            byte[] msg = _commsMarshaller.SerializeObjectForSending(message);
            Send(client, msg);
        }

        private void Send(TcpClient client, byte[] message)
        {
            if (ConnectionStatus != GameConstants.ConnectionStatusEnum.Connected || _listener == null || client == null)
            {
                throw new InvalidOperationException("GameServer->Send. Server cannot send. Connection is not open.");
            }
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(message, 0, message.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GameServer->Send failed to send to client. " + ex.ToString());
            }
        }

        #endregion Private Methods
    }
}
