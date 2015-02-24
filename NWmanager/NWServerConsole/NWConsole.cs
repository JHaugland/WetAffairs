using System;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWComms;

namespace NWServerConsole
{
    public class NWConsole
    {
        static Game _game;

        static void Main(string[] args)
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                GameManager.Instance.ResetAll();
                GameManager.Instance.Log.LogDebug("***** NWServerConsole started *****");

                GameManager.Instance.GameData.InitAllData();
                _game = GameManager.Instance.CreateGame("test game");
                _game.ClientAdded += new ConnectionDelegate(game_ClientAdded);
                _game.ConnectionStatusChanged += new ConnectionStatusChangedDelegate(game_ConnectionStatusChanged);
                _game.ServerDataReceived += new ServerDataReceivedDelegate(_game_ServerDataReceived);
                _game.IsNetworkEnabled = true;

                ReportGameInfo(_game);
                ReportConsoleAndLog("Game server started on port " + GameServer.DEFAULT_PORT
                    + ". Send terminate request object from client to end.");
                ReportConsoleAndLog("");

                GameManager.Instance.Game.StartGameLoop();

                ReportConsoleAndLog("");
                ReportConsoleAndLog("*** Game Over! ***");
                ReportGameInfo(_game);
                ReportConsoleAndLog(">>> Press [r] to restart, any other key to quit.");

                var key = Console.ReadKey();
                if (key.KeyChar.ToString().ToUpper() != "R")
                {
                    keepRunning = false;
                }
            }
            
            GameManager.Instance.Log.LogDebug("***** NWServerConsole ended *****");
        }

        static void _game_ServerDataReceived(int clientID, IMarshallable dataReceived)
        {
            ReportConsoleAndLog("");
            ReportConsoleAndLog("ServerDataReceived. ClientId=" + clientID + "  DataReceived=" + dataReceived.ObjectTypeToken);
            ReportConsoleAndLog("");
            
        }

        static void game_ConnectionStatusChanged( GameConstants.ConnectionStatusEnum status )
        {
            ReportConsoleAndLog("");
            ReportConsoleAndLog("Status changed. Status=" + status);
            ReportConsoleAndLog("");
        }

        static void game_ClientAdded(int clientId)
        {
            ReportConsoleAndLog("");
            ReportConsoleAndLog("NWConsole: New connection. Id=" + clientId);
            //Player player = _game.GetPlayerByClientIndex(clientId);
            //if (player != null)
            //{
            //    Position pos = new Position(60, 3.02, 0, 120);
            //    BaseUnit unit = GameManager.Instance.GameData.CreateUnit(player, null, "arleighburke", "", pos, true);
            //    unit.ActualSpeedKph = 5;
            //    unit.MovementOrder.AddWaypoint(new Waypoint(new Position(61.0, 4.0)));
            //    ReportConsoleAndLog("Player " + player.Id + " has receieved a brand new Arleigh Burke : " + unit.ToLongString());
                
            //}
            //ReportGameInfo(_game);
            ReportConsoleAndLog("");
        }

        static void ReportGameInfo(Game game)
        {
            ReportConsoleAndLog("");
            ReportConsoleAndLog(string.Format(" *** Game: [{0}] {1}  Player Count: {2}   Tick: {3}",
                game.Id, game.GameName, game.Players.Count, game.GameEngineTimeMs));
            foreach (var p in game.Players)
            {
                ReportPlayerInfo(p);
            }
            ReportConsoleAndLog("");
        }
        static void ReportPlayerInfo(Player player)
        {
            ReportConsoleAndLog("Player: " + player.ToLongString());
            foreach (var u in player.Units)
            {
                ReportConsoleAndLog(u.ToLongString());
            }
        }

        static void ReportConsoleAndLog(string info)
        {
            Console.WriteLine(info);
            GameManager.Instance.Log.LogDebug(info);
        }
    }
}
