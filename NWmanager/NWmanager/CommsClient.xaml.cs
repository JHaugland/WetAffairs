using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using NWServerConsole;
using System.Timers;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for CommsClient.xaml
    /// </summary>
    public partial class CommsClient : Window
    {
        private GameClient _gameClient;
        private GameInfo _gameInfo;
        private PlayerInfo _playerInfo;
        
        private DispatcherTimer _timer;
        private List<BaseUnitInfo> _Units = new List<BaseUnitInfo>();
        private List<GroupInfo> _Groups = new List<GroupInfo>();

        private ClientUnitInfo _unitInfoWindows = new ClientUnitInfo();
        private BaseUnitInfo _selectedUnitInfo;
        private DetectedUnitInfo _selectedDetectedUnitInfo;

        private List<DetectedUnitInfo> _Detections = new List<DetectedUnitInfo>();
        private List<DetectedGroupInfo> _DetectedGroups = new List<DetectedGroupInfo>();

        private Logger _log = new Logger();
        private DetectedUnitInfo _lastSelectedDetectedUnit = null;

        public CommsClient()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(CommsClient_Closing);
            this.Loaded += new RoutedEventHandler(CommsClient_Loaded);
        }

        void CommsClient_Loaded(object sender, RoutedEventArgs e)
        {
            lstUnits.ItemsSource = _Units;
            //lstUnits.DisplayMemberPath = "Description"; 
            lstDetections.ItemsSource = _Detections;
            //lstDetections.DisplayMemberPath = "DetectedUnitDescription";
            this.Left = 0;
            this.Top = 0;
            txtLat.Text = (60.5).ToString();
            txtLon.Text = (3.0).ToString();
            _unitInfoWindows.Show();
            _unitInfoWindows.Left = this.Left + this.Width;
            _unitInfoWindows.Top = this.Top;
            _unitInfoWindows.Height = this.Height;

            
        }

        void CommsClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_unitInfoWindows != null)
            {
                _unitInfoWindows.Close();
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            if (_gameClient != null)
            {
                _gameClient.Disconnect();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblInfoPanel.Content = "Connecting...";
                string Ip = txtIpAddress.Text.Trim();
                int Port = int.Parse(txtPort.Text);

                _gameClient = new GameClient(Ip, Port);
                _gameClient.ConnectionStatusChanged += _gameClient_ConnectionStatusChanged;
                _gameClient.DataReceived += _gameClient_DataReceived;
                _gameClient.Init();
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(500);
                _timer.Tick += new EventHandler(_timer_Tick);
                _timer.Start();
                btnConnect.IsEnabled = false;
                btnStartGame.IsEnabled = true;
                btnScenario.IsEnabled = true;
                lblInfoPanel.Content = "Connected. Awaiting data...";
                
            }
            catch (Exception ex)
            {
                ShowInfo("Error on connect! " + ex.Message);
                
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                _gameClient.PollNetwork();
            }
            catch (Exception ex)
            {
                ShowInfo("FATAL: Network fail. " + ex.Message);
                if (_timer != null)
                {
                    _timer.Stop();
                }
            }
            
        }


        void _gameClient_DataReceived(IMarshallable dataReceived)
        {
            HandleReceivedData(dataReceived);
            
        }



        void _gameClient_ConnectionStatusChanged( GameConstants.ConnectionStatusEnum status )
        {
            ShowInfo("Connection status changed: " + status);
            if ( _gameClient != null || _gameClient.ConnectionStatus != GameConstants.ConnectionStatusEnum.Connected )
            {
                btnConnect.IsEnabled = true;
            }
        }

        private void ShowInfo(string msg)
        {
            _log.LogDebug("ShowInfo: " + msg);
            if (txtUnitInfo.LineCount > 9000)
            {
                txtUnitInfo.Text = ""; //prevent out of memory exception
            }
            txtInfo.Text += msg + "\n";
        }
        
        private void HandleReceivedData(IMarshallable dataReceived)
        {
            switch (dataReceived.ObjectTypeToken)
            {
                case CommsMarshaller.ObjectTokens.NoObject:
                    break;
                case CommsMarshaller.ObjectTokens.Enq:
                    break;
                case CommsMarshaller.ObjectTokens.Ack:
                    break;
                case CommsMarshaller.ObjectTokens.ClientInfoRequest:
                    break;
                case CommsMarshaller.ObjectTokens.GameControlRequest:
                    break;

                case CommsMarshaller.ObjectTokens.GameStateInfo:
                    HandleGameStateInfo(dataReceived);
                    break;
                case CommsMarshaller.ObjectTokens.MessageString:
                    MessageString str = dataReceived as MessageString;
                    if (str != null)
                    {
                        if (str.Message.StartsWith("CHEAT"))
                        {
                            _selectedUnitInfo = null;
                            _unitInfoWindows.ShowInfo(str.Message);
                        }
                        else
                        {
                            ShowInfo("MessageString: " + str.Message);
                        }
                    }

                    break;
                case CommsMarshaller.ObjectTokens.BattleDamageReport:
                    BattleDamageReport report = dataReceived as BattleDamageReport;
                    if (report != null)
                    {
                        if (_playerInfo != null)
                        {
                            //if (string.IsNullOrEmpty(report.MessageToAttacker) || string.IsNullOrEmpty(report.MessageToAttacker))
                            //{ 

                            //}
                            //if (report.PlayerInflictingDamageId == _playerInfo.Id)
                            //{
                            //    ShowInfo("+++" + report.MessageToAttacker);
                            //}
                            //else if (report.PlayerSustainingDamageId == _playerInfo.Id)
                            //{
                            //    ShowInfo("---" + report.MessageToAttackee);
                            //}
                        }
                        //ShowInfo("***BattleDamageReport: " + report.ToString());
                    }
                    
                    break;
                case CommsMarshaller.ObjectTokens.GameInfo:
                    GameInfo gameInfo = dataReceived as GameInfo;
                    if (gameInfo != null)
                    {
                        _gameInfo = gameInfo;
                        ShowInfo("*** GameInfo received.\n" + gameInfo.ToString());
                        //ShowInfo("GameWorldTimeSec=" + gameInfo.GameWorldTimeSec);
                        //ShowInfo("GameEngineTimeMs=" + gameInfo.GameEngineTimeMs);

                    }
                    break;
                case CommsMarshaller.ObjectTokens.PlayerInfo:
                    PlayerInfo playerInfo = dataReceived as PlayerInfo;
                    HandlePlayerInfo(playerInfo);
                    break;
                case CommsMarshaller.ObjectTokens.PositionInfo:
                    HandlePositionInfo(dataReceived);

                    break;
                case CommsMarshaller.ObjectTokens.BaseUnitInfo:
                    HandleBaseUnitInfo(dataReceived);

                    break;
                case CommsMarshaller.ObjectTokens.GroupInfo:
                    HandleGroupInfo(dataReceived);
                    break;
                case CommsMarshaller.ObjectTokens.DetectedUnitInfo:
                    HandleDetectedUnitInfo(dataReceived);
                    break;
                case CommsMarshaller.ObjectTokens.DetectedGroupInfo:
                    HandleDetectedGroupInfo(dataReceived);
                    break;
                case CommsMarshaller.ObjectTokens.MessageInfo:
                    HandleMessageInfo(dataReceived);
                    break;
                case CommsMarshaller.ObjectTokens.OrderInfo:
                    break;
                case CommsMarshaller.ObjectTokens.UnitOrder:
                    break;
                case CommsMarshaller.ObjectTokens.UnitMovementOrder:
                    break;
                case CommsMarshaller.ObjectTokens.UnitEngagementOrder:
                    break;
                case CommsMarshaller.ObjectTokens.UnitClass:
                    UnitClass unitClass = dataReceived as UnitClass;
                    if (unitClass != null)
                    { 
                        ShowInfo("Received new UnitClass " + unitClass.Id + " " + unitClass.UnitClassShortName);
                    }
                    break;
                case CommsMarshaller.ObjectTokens.WeaponClass:
                    break;
                case CommsMarshaller.ObjectTokens.SensorClass:
                    break;
                case CommsMarshaller.ObjectTokens.GameScenario:
                    var scenario = dataReceived as GameScenario;
                    if(scenario != null)
                    {
                        cmbPlayers.Items.Clear();
                        foreach (var al in scenario.Alliences)
                        {
                            foreach (var pl in al.ScenarioPlayers)
                            {
                                if (pl.IsCompetitivePlayer)
                                {
                                    var item = new ComboBoxItem();
                                    item.Content = pl.ToString();
                                    item.Tag = pl.Id;
                                    cmbPlayers.Items.Add(item);
                                }
                            }
                        }
                        if (cmbPlayers.Items.Count > 0)
                        {
                            cmbPlayers.SelectedIndex = 0;
                        }
                    }
                    break;
                case CommsMarshaller.ObjectTokens.GameScenarioAlliance:
                    break;
                case CommsMarshaller.ObjectTokens.GameScenarioPlayer:
                    break;
                case CommsMarshaller.ObjectTokens.GameScenarioGroup:
                    break;
                case CommsMarshaller.ObjectTokens.GameScenarioUnit:
                    break;
                case CommsMarshaller.ObjectTokens.GameUiControl:
                    GameUiControl control = dataReceived as GameUiControl;
                    if (control != null)
                    {
                        ShowInfo("### GameUiControl received: " + control.ToString());
                    }
                    break;
                
                default:
                    break;
            }
        }

        private void HandlePlayerInfo(PlayerInfo playerInfo)
        {
            if (playerInfo != null)
            {
                _playerInfo = playerInfo;
                ShowInfo("*** You are player: " + playerInfo.ToString());
                //if (playerInfo.CurrentUser != null)
                //{
                //    ShowInfo("   UserId =" + playerInfo.CurrentUser.UserId);
                //}
                //if (playerInfo.CurrentCampaign != null)
                //if (!string.IsNullOrEmpty(playerInfo.CurrentCampaignId))
                //{
                //    //ShowInfo("   Campaign Name =" + playerInfo.CurrentCampaign.Name);
                //    ShowInfo( "   Campaign Name =" + playerInfo.CurrentCampaignId );
                //}
                ShowInfo("*** Credits: " + playerInfo.Credits + "\n");
                foreach (var tr in playerInfo.PlayerObjectives)
                {
                    ShowInfo("Trigger: " + tr.ToString());
                }

            }
        }

        private void HandleDetectedGroupInfo(IMarshallable dataReceived)
        {
            DetectedGroupInfo det = dataReceived as DetectedGroupInfo;
            if (det != null)
            {
                ShowInfo("### DetectedGroupInfo received: " + det.ToString());
                if (_DetectedGroups.Contains(det))
                {
                    _DetectedGroups.Remove(det);
                }
                _DetectedGroups.Add(det);
            }
        }

        private void HandleGroupInfo(IMarshallable dataReceived)
        {
            GroupInfo groupInfo = dataReceived as GroupInfo;
            if (groupInfo != null)
            {
                ShowInfo("***GroupInfo received.\n" + groupInfo.ToLongString());
                if (_Groups.Find(u => groupInfo.Id == u.Id) != null)
                {
                    _Detections.RemoveAll(u => u.Id == groupInfo.Id);
                }
                _Groups.Add(groupInfo);

            }

        }

        private void HandleMessageInfo(IMarshallable dataReceived)
        {
            MessageInfo info = dataReceived as MessageInfo;
            if (info != null)
            {
                string temp = "";
                if (!string.IsNullOrEmpty(info.FromPlayerId))
                {
                    temp = "From: " + info.FromPlayerId + "\n";
                }
                temp += info.MessageBody;
                ShowInfo(temp);
            }
        }

        private void HandlePositionInfo(IMarshallable dataReceived)
        {
            PositionInfo posInfo = dataReceived as PositionInfo;
            if(posInfo == null)
            {
                return;
            }
            if (posInfo.IsDetection)
            {
                DetectedUnitInfo det = _Detections.Find(d => d.Id == posInfo.UnitId);
                if (det != null)
                {
                    det.Position = posInfo;
                    lstDetections.Items.Refresh();
                    if (_selectedDetectedUnitInfo != null && _selectedDetectedUnitInfo.Id == det.Id)
                    {
                        UpdateUnitInfoWindow();
                    }

                }
                else
                {
                    ShowInfo("Received PositionInfo on Non-Existing Detection " + det.Id);
                }
            }
            else //refers to unit
            {
                BaseUnitInfo unit = _Units.Find(u => u.Id == posInfo.UnitId);
                if (unit != null)
                {
                    //double oldBearingDeg = unit.Position.BearingDeg;
                    //double newBearingDeg = posInfo.BearingDeg;
                    //double diffBearing = Math.Abs(oldBearingDeg - newBearingDeg);
                    if (_selectedUnitInfo != null && _selectedUnitInfo.Id == unit.Id)
                    {
                        UpdateUnitInfoWindow();
                    }
                    unit.Position = posInfo;
                    //if (unit.Id == "BRZ") //that darned helo
                    //{
                        
                    //    txtUnitInfo.Text = string.Format("BRZ bearing: {0:F}", newBearingDeg);
                    //    if(diffBearing > 1.0) 
                    //    {
                    //        ShowInfo(
                    //            string.Format("### BRZ has changed bearing: Old: {0:F}  New: {1:F}  Des: {2:F}\nPositionInfo: {3}", 
                    //            oldBearingDeg, newBearingDeg, unit.Position.DesiredBearingDeg, posInfo));
                    //    }
                    //}

                    //if (unit.Tag == "main")
                    //{
                    //    _log.LogDebug(string.Format("Unit {0} PositionInfo: {1}", unit.Id, posInfo.ToString()));
                    //}
                    //if (posInfo.IsFormationMovementOrder)
                    //{
                    //    _log.LogDebug(string.Format("Unit {0} PositionInfo: {1}", unit.Id, posInfo.ToString()));
                    //}
                    unit.ActualSpeedKph = posInfo.ActualSpeedKph;
                }
                ShowGameInfo();
                lstUnits.Items.Refresh();
            }
        }

        private void HandleGameStateInfo(IMarshallable dataReceived)
        {
            GameStateInfo gameInfo = dataReceived as GameStateInfo;
            if (gameInfo != null)
            {
                if (gameInfo.InfoType == GameConstants.GameStateInfoType.UnitIsDestroyed)
                {
                    if (_selectedUnitInfo != null && _selectedUnitInfo.Id == gameInfo.Id)
                    {
                        _selectedUnitInfo = null;
                    }
                    _Units.RemoveAll(u => u.Id == gameInfo.Id);
                    lstUnits.Items.Refresh();
                }
                else if (gameInfo.InfoType == GameConstants.GameStateInfoType.DetectedContactIsLost || gameInfo.InfoType == GameConstants.GameStateInfoType.DetectedContactIsDestroyed)
                {
                    _Detections.RemoveAll(d => d.Id == gameInfo.Id);
                    lstDetections.Items.Refresh();
                }
                else if (gameInfo.InfoType == GameConstants.GameStateInfoType.DetectedContactGroupIsLost)
                {
                    _DetectedGroups.RemoveAll(d => d.Id == gameInfo.Id);
                }
                else if (gameInfo.InfoType == GameConstants.GameStateInfoType.AircraftIsLanded)
                {
                    _Units.RemoveAll(u => u.Id == gameInfo.Id);
                    lstUnits.Items.Refresh();
                    ShowInfo("Aircraft has landed : " + gameInfo.Id);
                }
                else if (gameInfo.InfoType == GameConstants.GameStateInfoType.MissileLaunch)
                {

                }
                ShowInfo("***GameStateInfo: " + gameInfo.ToString());
            }
        }

        private void HandleDetectedUnitInfo(IMarshallable dataReceived)
        {
            string selectedId = string.Empty;
            
            DetectedUnitInfo det = dataReceived as DetectedUnitInfo;
            
            if (lstDetections.SelectedItem != null)
            {
                DetectedUnitInfo detUnit = lstDetections.SelectedItem as DetectedUnitInfo;
                if (detUnit != null)
                {
                    selectedId = detUnit.Id;
                }
            }
            if (_Detections.Find(u => det.Id == u.Id) != null)
            {
                _Detections.RemoveAll(u => u.Id == det.Id);
                ShowInfo("DetectedUnitInfo received: Id=" + det.Id + "  " + det.ToString());
            }
            _Detections.Add(det);
            //if (det.PositionRegion != null)
            //{
            //    ShowInfo("### PositionRegion: " + det.PositionRegion.ToString());
            //}
            ShowGameInfo();
            UpdateUnitInfoWindow();
            lstDetections.Items.Refresh();
            if (!string.IsNullOrEmpty(selectedId))
            {
                foreach (var it in lstDetections.Items)
                {
                    DetectedUnitInfo detLst = it as DetectedUnitInfo;
                    if (detLst != null)
                    {
                        if (detLst.Id == selectedId)
                        {
                            lstDetections.SelectedItem = it;
                        }
                    }
                }
            }
        }

        private void HandleBaseUnitInfo(IMarshallable dataReceived)
        {
            BaseUnitInfo info = dataReceived as BaseUnitInfo;
            if (info != null)
            {

                if (_Units.Find(u => info.Id == u.Id) != null)
                {
                    _Units.RemoveAll(u => u.Id == info.Id);
                }
                else
                {
                    ShowInfo("Received BaseUnitInfo: " + info.ToString());


                }
                if (info.IsMarkedForDeletion)
                {
                    //ShowInfo(string.Format("Unit [{0}] {1} has been DESTROYED.", info.Id, info.UnitName));
                }
                else
                {
                    _Units.Add(info);
                }
                ShowGameInfo();
                lstUnits.Items.Refresh();
            }
        }

        private void ShowGameInfo()
        {
            string temp = string.Format("Units: {0}  Detected: {1}", _Units.Count, _Detections.Count);
            lblInfoPanel.Content = temp;
        }

        private void btnReqGameInfo_Click(object sender, RoutedEventArgs e)
        {
            ClientInfoRequest req = ClientInfoRequest.CreateRequestObject(CommsMarshaller.ObjectTokens.GameInfo, "");
            _gameClient.Send(req);
            ShowInfo("ClientInfoRequest object sent");

        }

        private void btnEndGame_Click(object sender, RoutedEventArgs e)
        {
            GameControlRequest req = new GameControlRequest();
            req.ControlRequestType = CommsMarshaller.GameControlRequestType.TerminateGame;
            _gameClient.Send(req);
            ShowInfo("Terminate object sent");
            btnConnect.IsEnabled = true;
            _DetectedGroups.Clear();
            _Detections.Clear();
            _Units.Clear();
            _Groups.Clear();
            lstDetections.Items.Refresh();
            lstUnits.Items.Refresh();

        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double Lat = double.Parse(txtLat.Text);
                double Lon = double.Parse(txtLon.Text);
                BaseUnitInfo info = lstUnits.SelectedItem as BaseUnitInfo;
                string Id;
                if (info != null)
                {
                    Id = info.Id;
                }
                else
                {
                    ShowInfo("Select a unit in ListBox first.");
                    return;
                }
                UnitMovementOrder order = new UnitMovementOrder();
                order.Id = Id;
                PositionInfo pos = new PositionInfo();
                pos.Latitude = Lat;
                pos.Longitude = Lon;
                order.RemoveAllExistingWaypoints = (bool)chkClearWaypoints.IsChecked;
                order.Waypoints.Add(new WaypointInfo(pos));
                order.UnitSpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
                _gameClient.Send(order);
                ShowInfo("*** Movement order sent to unit " + info.UnitName);
                lstUnits.SelectedIndex = -1;
                lstDetections.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowInfo("Select a unit. Use decimal points for Lat and Long. " + ex.Message);                

            }
            

        }

        private void lstUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                _selectedUnitInfo = unit;
                UpdateUnitInfoWindow();
            }
            else
            {
                _selectedUnitInfo = null;
            }
        }

        private void UpdateUnitInfoWindow()
        {
            if (_unitInfoWindows == null)
            {
                return;
            }
            if (_selectedUnitInfo != null)
            {
                string info = "";
                info = _selectedUnitInfo.ToLongString();// +"\n" 
                    //+ "Distance to target: " + _selectedUnitInfo.Position.MostRecentDistanceToTargetM + "\n";
                info += _selectedUnitInfo.UnitSubType.ToString() + "\n";
                info += "Mission: " + _selectedUnitInfo.MissionType + " " + _selectedUnitInfo.MissionTargetType + "\n";
                if (_selectedUnitInfo.HasFormationOrder)
                {
                    info += "Has formation order\n";
                }
                if (_selectedUnitInfo.SupportsOrderType.Contains(GameConstants.OrderType.SpecialOrders))
                {
                    info += "Special orders:\n";
                    foreach (var specOrder in _selectedUnitInfo.SupportsSpecialOrders)
                    {
                        info += specOrder + "\n";
                    }
                    
                }
                foreach (var order in _selectedUnitInfo.OrderQueue)
                {
                    info += order + "\n";
                }
                if (_selectedUnitInfo.IsUsingActiveRadar)
                {
                    info += "Active Radar\n";
                }
                if (_selectedUnitInfo.IsUsingActiveSonar)
                {
                    info += "Active Sonar\n";
                }
                foreach (var sens in _selectedUnitInfo.Sensors)
                {
                    info += string.Format("\n* {0} Operational: {1}",
                        sens.ToString(), sens.IsOperational);
                }
                foreach (var wpn in _selectedUnitInfo.Weapons)
                {
                    info += string.Format("\n* {0}  Ammo: {1} of {2}",
                        wpn.ToString(), wpn.AmmunitionRemaining, wpn.MaxAmmunition);
                }
                _unitInfoWindows.ShowInfo(info);
            }
            if (_selectedDetectedUnitInfo != null)
            {
                string info = _selectedDetectedUnitInfo.ToLongString();

                _unitInfoWindows.ShowDetectedUnitInfo(info);

            }

        }
        private void btnResetLists_Click(object sender, RoutedEventArgs e)
        {
            lstDetections.SelectedIndex = -1;
            _lastSelectedDetectedUnit = null;
            lstUnits.SelectedIndex = -1;
        }

        private void btnEngage_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            DetectedUnitInfo det = lstDetections.SelectedItem as DetectedUnitInfo;
            if (unit != null && det != null)
            {
                ShowInfo(string.Format("*** Unit {0} to engage target {1}", unit.UnitName, det.ToString()));
                UnitEngagementOrder order = new UnitEngagementOrder();
                order.TargetId = det.Id;
                order.IsGroupAttack = true; //test that
                order.Id = unit.Id;
                //order.WeaponClassID = "mk45mod4";
                order.EngagementType = GameConstants.EngagementOrderType.EngageNotClose;
                _gameClient.Send(order);
            }
            else
            {
                ShowInfo("Select own unit and detected unit to engage.");
            }
        }

        private void lstDetections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DetectedUnitInfo unit = lstDetections.SelectedItem as DetectedUnitInfo;
            if (unit != null)
            {
                _selectedDetectedUnitInfo = unit;
                UpdateUnitInfoWindow();
                if (_lastSelectedDetectedUnit != null && _lastSelectedDetectedUnit.Id != unit.Id)
                {
                    ShowInfo(string.Format("*** Unit {0} is at {1}", unit.ToString(), unit.Position.ToString()));
                }
            }

        }

        private void btnRtb_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                var unitOrder = OrderFactory.CreateReturnToBaseOrder(unit.Id);
                _gameClient.Send(unitOrder);
            }
            else
            {
                ShowInfo("Select own unit");
            }

        }

        private void btnLaunchAircraft_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.LaunchAircraft, unit.Id);
                LaunchAircraft launchDialog = new LaunchAircraft();
                launchDialog.AircraftList = unit.CarriedUnits;
                launchDialog.Init();
                bool? result = launchDialog.ShowDialog();
                order.ParameterList = launchDialog.GetSelectedIds();

                //int unitcount = 0;
                //int maxUnits = 1;
                //string aircraftlist = string.Empty;
                //if(unit.UnitClassId.Contains("airport"))
                //{
                //    maxUnits = 3;
                //}
                //foreach (var c in unit.CarriedUnits)
                //{
                //    if (c.ReadyInSec < 1 && unitcount < maxUnits)
                //    {
                //        order.ParameterList.Add(c.Id);
                //        aircraftlist += c.ToString() + " ";
                //        unitcount++;
                //    }
                //}
                ShowInfo(string.Format(
                    "UnitOrder to unit {0} : Launch {1} aircraft.", 
                    order.Id, order.ParameterList.Count));
                UnitMovementOrder moveOrder = new UnitMovementOrder();
                moveOrder.Waypoints.Add(new WaypointInfo(new PositionInfo(55,3)));
                order.UnitOrders.Add(moveOrder);
                _gameClient.Send(order);
            }
            else
            {
                ShowInfo("No unit selected!");
            }
        }

        private void lstDetections_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DetectedUnitInfo unit = lstDetections.SelectedItem as DetectedUnitInfo;
            if (unit != null)
            {
                ShowInfo(unit.ToString());
            }
        }

        private void btnScenario_Click(object sender, RoutedEventArgs e)
        {
            if (_gameClient == null)
            {
                ShowInfo("** NOT CONNECTED **");
                return;
            }
            GameControlRequest reqCampaign = GameControlRequest.CreateControlRequestObject(
                CommsMarshaller.GameControlRequestType.PlayerSelectCampaign, "test", string.Empty, 0);
            _gameClient.Send(reqCampaign);

            ShowInfo("*** Sending select scenario ***");
            if (cmbPlayers.SelectedItem != null)
            {
                var item = cmbPlayers.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    string id = (string)item.Tag;
                    if (!string.IsNullOrEmpty(id))
                    {
                        var selectReq = GameControlRequest.CreateControlRequestObject(
                            CommsMarshaller.GameControlRequestType.PlayerSelectScenarioPlayer, 
                            id, "", 0);
                        _gameClient.Send(selectReq);
                    }
                }
            }
            GameControlRequest req = GameControlRequest.CreateControlRequestObject(
                CommsMarshaller.GameControlRequestType.PlayerSelectScenario, 
                txtScenario.Text, "", 0);
            _gameClient.Send(req);
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            ShowInfo("*** Sending start game ***");
            if (cmbPlayers.SelectedItem != null)
            {
                var item = cmbPlayers.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    string id = (string)item.Tag;
                    if (!string.IsNullOrEmpty(id))
                    {
                        //var userReq = OrderFactory.CreateSetPlayerSelectUserOrder("Jan");
                        //_gameClient.Send(userReq);
                        //ShowInfo("*** Player selects user " + userReq.Id);
                        var selectReq = GameControlRequest.CreateControlRequestObject(
                            CommsMarshaller.GameControlRequestType.PlayerSelectScenarioPlayer,
                            id, "", 0);
                        _gameClient.Send(selectReq);
                        ShowInfo("*** Player selects scenario player " + id);
                    }
                }
            }
            GameControlRequest req = GameControlRequest.CreateControlRequestObject(
                CommsMarshaller.GameControlRequestType.GameSceneLoaded, "", "", 0 );
            _gameClient.Send( req );
            req = GameControlRequest.CreateControlRequestObject(
                CommsMarshaller.GameControlRequestType.StartGame, "", "", 0);
            _gameClient.Send(req);
            var timeComprReq = GameControlRequest.CreateControlRequestObject(CommsMarshaller.GameControlRequestType.SetTimeCompressionRatio, "", "", 10);
            _gameClient.Send(timeComprReq);

        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                string info = "";
                info = unit.ToLongString(); // +"\n" + "Distance to target: " + unit.Position.MostRecentDistanceToTargetM + "\n";
                foreach (var sens in unit.Sensors)
                {
                    info+=string.Format("\n* {0} Operational: {1}",
                        sens.ToString(), sens.IsOperational);
                }
                foreach (var wpn in unit.Weapons)
                {
                    info+=string.Format("\n* {0}  Ammo: {1} of {2}",
                        wpn.ToString(), wpn.AmmunitionRemaining, wpn.MaxAmmunition);
                }
                _unitInfoWindows.ShowInfo(info);
            }

            
        }

        //private void SetFormation()
        //{
        //    BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
        //    if (unit != null)
        //    {
        //        GroupInfo group = _Groups.Find(g => g.Id == unit.GroupId);
        //        if (group != null)
        //        {
        //            if (group.Formation != null)
        //            {
        //                UnitOrder order =
        //                    new UnitOrder(GameConstants.UnitOrderType.SetNewGroupFormation, unit.Id);
        //                Formation newForm = group.Formation.Clone();
        //                if (newForm.FormationPositions.Count > 2)
        //                {
        //                    newForm.FormationPositions[1] = new FormationPosition()
        //                    {
        //                        PositionOffset = new PositionOffset(1000, 1000)
        //                    };
        //                }
        //                order.Formation = newForm;
        //                _gameClient.Send(order);
        //                ShowInfo("*** New Formation order sent to unit " + unit.ToShortString());
        //            }
        //        }
        //        else
        //        {
        //            ShowInfo(string.Format("Unit {0} has no group", unit.ToShortString()));
        //        }
        //    }

        //}

        private void btnDesignateHostile_Click(object sender, RoutedEventArgs e)
        {
            DetectedUnitInfo unit = lstDetections.SelectedItem as DetectedUnitInfo;
            if (unit != null)
            {
                ShowInfo("Designate " + unit.ToString() + " as hostile.");
                var req = OrderFactory.CreateFriendOrFoeDesignationOrder(unit.Id, GameConstants.FriendOrFoe.Foe);
                _gameClient.Send(req);
            }
        }

        private void btnRadar_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                var order = 
                    OrderFactory.CreateSensorActivationOrder(unit.Id, 
                    GameConstants.SensorType.Radar, true);
                _gameClient.Send(order);
            }

        }

        private void btnCloseAndEngage_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            DetectedUnitInfo det = lstDetections.SelectedItem as DetectedUnitInfo;
            if (unit != null && det != null)
            {
                ShowInfo(string.Format("*** Unit {0} to close and engage target {1}", unit.UnitName, det.ToString()));
                UnitEngagementOrder order = new UnitEngagementOrder();
                order.TargetId = det.Id;
                order.IsGroupAttack = true; //test that
                order.Id = unit.Id;
                //order.WeaponClassID = "mk45mod4";
                order.EngagementType = GameConstants.EngagementOrderType.CloseAndEngage;
                _gameClient.Send(order);
            }
            else
            {
                ShowInfo("Select own unit and detected unit to engage.");
            }

        }

        private void btnCheat_Click(object sender, RoutedEventArgs e)
        {
            var req = OrderFactory.CreateCheatOrder(GameConstants.CHEAT_CODE_REVEAL_ORDERS);
            _gameClient.Send(req);
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            string temp = SerializationHelper.GetServerFolder() + "NWServerConsole.exe";
            //ProcessStartInfo psi = new ProcessStartInfo(temp);
            
            Process.Start(temp);
        }

        private void btnMaxSpeed_Click(object sender, RoutedEventArgs e)
        {
            BaseUnitInfo unit = lstUnits.SelectedItem as BaseUnitInfo;
            if (unit != null)
            {
                var order =
                    OrderFactory.CreateSetSpeedOrder(unit.Id, GameConstants.UnitSpeedType.Afterburner, true);
                _gameClient.Send(order);
            }
        }

    }
}
