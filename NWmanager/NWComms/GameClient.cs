using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Net;
using System.Net.Sockets;

namespace TTG.NavalWar.NWComms
{
    public class GameClient : IGameClient
    {
        #region "Consts, delegates, events and private fields"

        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 2055;
        public const double DEFAULT_TIMER_MS = 1000;

        public event ClientDataReceivedDelegate DataReceived;
        public event ConnectionStatusChangedDelegate ConnectionStatusChanged;

        private TcpClient _TcpClient = null;
        //private Timer _Timer = null;
        private GameConstants.ConnectionStatusEnum _ConnectionStatus =
            GameConstants.ConnectionStatusEnum.Disconnected;
        private CommsMarshaller _CommsMarshaller = new CommsMarshaller();

        protected Logger _Logger = new Logger();

        private Byte [] _ReadBuffer = new Byte [ CommsMarshaller.DEFAULT_BUFFER_SIZE ];

        #endregion

        #region "Constructors"
        public GameClient()
        {
            Host = DEFAULT_HOST;
            Port = DEFAULT_PORT;
        }

        public GameClient(string hostName, int port)
        {
            Host = hostName;
            Port = port;
        }
        #endregion

        #region "Public properties"

        public string Host { get; set; }

        public int Port { get; set; }

        public GameConstants.ConnectionStatusEnum ConnectionStatus
        {
            get
            {
                return _ConnectionStatus;
            }
            set
            {
                //_Logger.LogDebug("Attempting to change connection status...");
                //_Logger.LogDebug("Current status: " + _ConnectionStatus.ToString() + " status to change to: " + value.ToString());
                try
                {
                    if (value != _ConnectionStatus)
                    {
                        _ConnectionStatus = value;
                        ConnectionStatusChanged(_ConnectionStatus);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        _Logger.LogError("Error in GameClient.ConnectionStatus: " + ex.ToString());
                    }
                    else
                    {
                        _Logger.LogError("Error in GameClient.ConnectionStatus: " + ex.ToString() 
                            + "\nInnerexception: " +  ex.InnerException.ToString());
                    }
                }
            }
        }
        #endregion


        #region "Public methods"

        public void Init()
        {
           
            _Logger.LogDebug("////////////////////////////////////////////////////////");
            _Logger.LogDebug("////////////////////////////////////////////////////////");
            _Logger.LogDebug("////////////////////NEW SESSION/////////////////////////");
            _Logger.LogDebug("////////////////////////////////////////////////////////");
            _Logger.LogDebug("////////////////////////////////////////////////////////");

            try
            {
                //_Logger.LogDebug("Game client connecting...");
                _TcpClient = new TcpClient(Host, Port);
                //_Logger.LogDebug("Game client connected");

                //_Timer = new Timer(DEFAULT_TIMER_MS);
                //_Timer.Elapsed += new ElapsedEventHandler(_Timer_Elapsed);
                //_Timer.Start();
                //_Logger.LogDebug("Connection status is : " + ConnectionStatus.ToString());
                ConnectionStatus = GameConstants.ConnectionStatusEnum.Connected;
                //_Logger.LogDebug("Game client status is now " + ConnectionStatus.ToString());

            }
            catch (Exception ex)
            {
                _Logger.LogError("Error in GameClient.Init(): " + ex.ToString());
                throw new Exception("GameClient->Init failed to reach host " 
                    + Host + " on port " + Port + ". " + ex.Message, ex);
            }
        }

        public void Send(IMarshallable obj)
        {
            byte[] bytes = _CommsMarshaller.SerializeObjectForSending(obj);
            //_CommsMarshaller.AddToBufferEnd(bytes, bytes.Length);
            Send(bytes);
        }

        public void Send(string message)
        {
            MessageString str = new MessageString {Message = message};
            Send(str); 
        }

        private void Send(byte[] message)
        {
            try
            {
                if ( ConnectionStatus != GameConstants.ConnectionStatusEnum.Connected
                    || _TcpClient == null || !_TcpClient.Connected)
                {
                    throw new InvalidOperationException("Client cannot send. Connection is not open.");
                }
                NetworkStream stream = _TcpClient.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(message, 0, message.Length);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("GameClient send error. " + ex.Message);
            }

        }

        public void Disconnect()
        {
            //if (_Timer != null)
            //{
            //    _Timer.Stop();
            //    _Timer = null;
            //}
            //_Logger.LogDebug("Game client disconnecting...");
            if (_TcpClient != null)
            {
                _TcpClient.Close();
                _TcpClient = null;
            }
            ConnectionStatus = GameConstants.ConnectionStatusEnum.Disconnected;
            //_Logger.LogDebug("Game client disconnected.");

        }

        /// <summary>
        /// This method must be called repeatedly by the client code by a timer or otherwise.
        /// </summary>
        public void PollNetwork()
        {
            //_Logger.LogDebug("Gameclient attempting to poll network...");

            if ( ConnectionStatus != GameConstants.ConnectionStatusEnum.Connected ||
                _TcpClient == null || !_TcpClient.Connected)
            {
                //try
                //{
                //    _Logger.LogWarning("Cannot poll network. Connection status: " + ConnectionStatus.ToString()
                //        + " TcpClient: " + _TcpClient == null ? "null" : _TcpClient.ToString()
                //        + " TcpClient connected?: " + _TcpClient == null ? "null" : _TcpClient.Connected.ToString());
                //}
                //catch (Exception ex)
                //{
                //    _Logger.LogError("PollNetwork failed to log warning. " + ex.ToString());
                //}
                return;
            }            

            NetworkStream stream = _TcpClient.GetStream();
            try
            {
                if ( stream.DataAvailable )
                {
                    do
                    {
                        int i = stream.Read(_ReadBuffer, 0, _ReadBuffer.Length);
                        _CommsMarshaller.AddToReceiveBufferEnd(_ReadBuffer, i);

                        while ( _CommsMarshaller.IsObjectInReceiveBuffer() )
                        {
                            IMarshallable obj = _CommsMarshaller.DeSerializeNextObjectInReceiveBuffer();
                            DataReceived(obj);
                        }
                    } while ( stream.DataAvailable );
                }             
            }
            catch (ArgumentException ex)
            {
                _Logger.LogError("Error in GameClient.PollNetwork: " + ex.ToString());
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    _Logger.LogError("Error in GameClient.PollNetwork: " + ex.ToString());
                }
                else
                {
                    _Logger.LogError("Error in GameClient.PollNetwork: " + ex.ToString() +
                        "\nInnerException: " + ex.InnerException.ToString());
                }
            }
        }

        #endregion

        #region "Private methods"

        #endregion


    }
}
