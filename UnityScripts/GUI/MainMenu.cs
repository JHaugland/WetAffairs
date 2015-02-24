using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class MainMenu : MonoBehaviour
{

    #region Enums and Privates
    public enum MenuScreen
    {
        Main,
        Options,
        GraphicOptions,
        AudioOptions,
        ControlOptions,
        SinglePlayer,
        MultiPlayer,
        SkirmishSinglePlayer,
        SkirmishMultiPlayer
    }

    private GameScenario _CurrentScenario;

    #endregion

    public MenuScreen CurrentScreen = MenuScreen.Main;

    public List<GameScenario> Scenarios;


    #region Skirmish

    private Vector2 ScenarioListScrollPosition = Vector2.zero;

    #endregion

    void Update()
    {
        if (Scenarios == null)
        {
            Scenarios = GameManager.Instance.Scenarios;
            _CurrentScenario = Scenarios[0];
        }
    }

    void OnGUI()
    {
        switch (CurrentScreen)
        {
             
            case MenuScreen.Main:
                GUILayout.BeginArea(new Rect(100, 100, Screen.width / 3, Screen.height));
                GUILayout.Box("Menu");
                if(GUILayout.Button("Single Player!"))
                {
                    CurrentScreen = MenuScreen.SinglePlayer;
                }
                if (GUILayout.Button("MultiPlayer!"))
                {
                    CurrentScreen = MenuScreen.MultiPlayer;
                }
                if(GUILayout.Button("Options!"))
                {
                    CurrentScreen = MenuScreen.Options;
                }
                if (GUILayout.Button("Quit"))
                {
                    Application.Quit();
                }

                GUILayout.EndArea();

                break;
            case MenuScreen.Options:
                break;
            case MenuScreen.GraphicOptions:
                break;
            case MenuScreen.AudioOptions:
                break;
            case MenuScreen.ControlOptions:
                break;
            case MenuScreen.SinglePlayer:
                GUILayout.BeginArea(new Rect(100, 100, Screen.width / 3, Screen.height));
                GUILayout.Box("Menu");
                if (GUILayout.Button("Skirmish"))
                {
                    CurrentScreen = MenuScreen.SkirmishSinglePlayer;
                }
                if (GUILayout.Button("Campaign"))
                {
                }
                if (GUILayout.Button("Back"))
                {
                    CurrentScreen = MenuScreen.Main;
                }

                GUILayout.EndArea();
                break;
            case MenuScreen.MultiPlayer:
                break;
            case MenuScreen.SkirmishSinglePlayer:

                GUILayout.BeginArea(new Rect(10, 10, Screen.width / 3, Screen.height - 10));
                GUILayout.Box("Scenarios");
                ScenarioListScrollPosition = GUILayout.BeginScrollView(ScenarioListScrollPosition, GUIStyle.none, GUIStyle.none, GUILayout.Height(300));
                
                if(Scenarios != null)
                {
                    foreach (GameScenario gs in Scenarios)
                    {
                        if (GUILayout.Button(gs.GameName))
                        {
                            //Show this as current Scenario
                            _CurrentScenario = gs;
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(Screen.width / 3 + 10, 10, Screen.width * 0.60f, Screen.height / 2 - 10));
                GUILayout.Box("Options");

                foreach (GameScenarioAlliance allience in _CurrentScenario.Alliences)
                {
                    if (!allience.IsCompetitivePlayer) { continue; }
                    
                    GUILayout.Label(allience.Description);
                    GUILayout.BeginHorizontal();

                    foreach (GameScenarioPlayer player in allience.ScenarioPlayers)
                    {
                        GUILayout.Label(string.Format("Player : {0}  {1}", player.ToString(), player.IsComputerPlayer == true ? "Computer" : "Human"));
                        
                    }

                    GUILayout.EndHorizontal();
                }


                GUILayout.EndArea();                

                GUILayout.BeginArea(new Rect(Screen.width / 3 + 10, Screen.height / 2 - 10, Screen.width * 0.60f, Screen.height / 2 - 10));
                if(_CurrentScenario != null)
                {
                    GUILayout.Box(_CurrentScenario.GameName);
                    
                    GUILayout.Label(string.Format("Scenario Type: {0}", _CurrentScenario.GameScenarioType.ToString()));
                    GUILayout.Label(string.Format("Season: {0}",_CurrentScenario.Season.ToString()));
                    GUILayout.Label(string.Format("Weather Type: {0}", _CurrentScenario.WeatherType.ToString()));
                    //Show scenario info
                }


                if (GUILayout.Button("Play Scenario"))
                {
                    GameManager.Instance.LoadScenario(_CurrentScenario);
                }
                GUILayout.EndArea();

                break;
            case MenuScreen.SkirmishMultiPlayer:
                break;
            default:
                break;
        }
    }
}
