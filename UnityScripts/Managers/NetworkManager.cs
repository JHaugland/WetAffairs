using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class NetworkManager : MonoBehaviour
{




    #region Privates, consts and enums
    private bool _IsPollingData = false;
    private GameClient _client;
    private string _LastMessage = "LastMessage";
    private bool _Quitting = false;
    private bool _GameInfoRequested = false;

    private int _NumberOfPolls = 0;
    private bool _ServerStarted = false;
    #endregion

    #region Private Methods
    private void DoConnect()
    {
        try
        {
            //_client = null;
            _client = new GameClient(HostName, Port);
            _client.ConnectionStatusChanged += new GameClient.ConnectionStatusChangedDelegate(Client_ConnectionStatusChanged);
            _client.DataReceived += new GameClient.DataReceivedDelegate(Client_DataReceived);
            _client.Init();
            TogglePolling();
        }
        catch (Exception ex)
        {
            GameManager.Instance.MessageManager.AddMessage(ex.Message, GameManager.MessageTypes.Game, null);
        }
    }

    private void DoDisconnect(bool reconnect)
    {
        if (_client != null)
        {
            try
            {
                CancelInvoke();

                if (_Quitting)
                {
                    //~ TerminateGame();
                    //_client = null;

                }
                TerminateGame();
                Debug.Log("Game terminated. Attempting to set client to null");
                _client = null;
                Debug.Log("Client is null");

                
            }
            catch (Exception)
            {

                //ignore
            }
        }
        if (reconnect)
        {
            DoConnect();
        }
    }

    private void DoPollNetwork()
    {
        if (_client != null)
        {
            if (_client.ConnectionStatus == GameServer.ConnectionStatusEnum.Connected)
            {
                if (!_GameInfoRequested)
                {
                    ClientInfoRequest req = ClientInfoRequest.CreateRequestObject(CommsMarshaller.ObjectTokens.GameInfo, "");
                    _client.Send(req);
                    _GameInfoRequested = true;
                }
                //~ ShowMessage("Polling network");
                _client.PollNetwork();
                _NumberOfPolls++;
            }
        }
    }

    private void TerminateGame()
    {
        ServerProsess.Kill();

        //GameControlRequest req = new GameControlRequest();
        //req.ControlRequestType = CommsMarshaller.GameControlRequestType.TerminateGame;
        //_client.Send(req);
        
        _ServerStarted = false;


        //ServerProsess.Dispose();
        //ServerProsess.Close();
        //GameManager.Instance.GameInfo = null;

        //TODO: Send deletion to unitmanager

        //GameManager.Instance.UnitManager.SelectedUnit = null;
       

    }
    /// <summary>
    /// Handler for dataReceived. Ships it to communicationManager for further use
    /// </summary>
    /// <param name="dataReceived">Received from delegate.</param>
    private void Client_DataReceived(IMarshallable dataReceived)
    {
        //~ Debug.Log(dataReceived.ObjectTypeToken.ToString());
        //~ GameManager.Instance.MessageManager.AddMessage(dataReceived.ToString());
        GameManager.Instance.CommunicationManager.HandleReceivedData(dataReceived);
    }
    #endregion

    #region Script variabels
    public string HostName = "127.0.0.1";
    public int Port = 2055;
    public float CoordinateModifier = 1000.0f;
    public string ServerApplicationName = @"C:\Users\Espen\Documents\Visual Studio 2008\Projects\SvnNW\NWmanager\NWServerConsole\bin\Debug";
    private System.Diagnostics.Process ServerProsess;
    #endregion
    #region Public Properties
    public int NumberOfPolls
    {
        get
        {
            return _NumberOfPolls;
        }
    }


    public string LastMessage
    {
        get
        {
            return _LastMessage;
        }
    }

    public bool Connected
    {
        get
        {
            if (_client == null)
            {
                return false;
            }
            return _client.ConnectionStatus == GameServer.ConnectionStatusEnum.Connected;
        }
    }

    public bool ServerStarted
    {
        get
        {
            return _ServerStarted;
        }
    }
    #endregion

    #region Public Methods

    public void Connect()
    {
        ShowMessage("Connecting to default ip and port");
        DoConnect();
    }

    public void Connect(string host, int port)
    {
        HostName = host;
        Port = port;
        ShowMessage("Attempting to connect");
        DoConnect();
    }

    public void LoadScenario(string scenarioName)
    {
        GameControlRequest req = GameControlRequest.CreateControlRequestObject(CommsMarshaller.GameControlRequestType.PlayerSelectScenario, scenarioName, "", 0);
        _client.Send(req);
    }

    public void StartGame()
    {
        GameControlRequest req = GameControlRequest.CreateControlRequestObject(
                CommsMarshaller.GameControlRequestType.StartGame, "", "", 0);
        _client.Send(req);
    }



    public void Disconnect(bool reconnect)
    {
        DoDisconnect(reconnect);
    }

    public void Send(IMarshallable obj)
    {
        if (obj != null)
        {
            _client.Send(obj);
        }

    }

    public void StartServer()
    {
        if (!_ServerStarted)
        {
            ServerProsess = new System.Diagnostics.Process();
            //string dataPath = Path.GetDirectoryName(Application.dataPath) + "/ServerApplication/";
            //ServerProsess.StartInfo.FileName = dataPath + ServerApplicationName;
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            
            startInfo.FileName = ServerApplicationName;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            startInfo.CreateNoWindow = true;
            ServerProsess.StartInfo = startInfo;
            ServerProsess.Start();
            _ServerStarted = true;
            
        }
        else
        {

        }
    }



    public void PollNetwork()
    {
        DoPollNetwork();
    }

    public void TogglePolling()
    {
        if (!_IsPollingData)
        {
            InvokeRepeating("DoPollNetwork", 1, 0.1f);

        }
        else
        {
            CancelInvoke();
        }
        _IsPollingData = !_IsPollingData;
    }
    /// <summary>
    /// Creates and sends a ClientInfoRequest to server
    /// </summary>
    /// <param name="objectToken">Objectoken from CommsMarshaller.ObjectTokens enum.</param>
    public void RequestObject(CommsMarshaller.ObjectTokens objectToken)
    {
        if (_client != null)
        {
            ClientInfoRequest req = ClientInfoRequest.CreateRequestObject(objectToken, "");
            _client.Send(req);
        }
    }
    #endregion

    #region Unity specific methods
    void Start()
    {
        //~ _client = new GameClient(HostName, Port) ;

        //ShowMessage(@"GameClient Initialized");
    }

    void OnApplicationQuit()
    {
        _Quitting = true;
        DoDisconnect(false);

    }
    #endregion




    void ShowMessage(string message)
    {
        GameManager.Instance.MessageManager.AddMessage(message, GameManager.MessageTypes.Game, null);
    }

    void Client_ConnectionStatusChanged(GameServer.ConnectionStatusEnum status)
    {
        ShowMessage("Client status changed to " + status);
    }

    void OnGUI()
    {
        GUILayout.Label(NumberOfPolls.ToString());
    }
}