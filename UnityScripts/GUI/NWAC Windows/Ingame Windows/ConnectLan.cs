using UnityEngine;
using System.Collections;

public class ConnectLan : MonoBehaviour
{

    public int WindowId = 91293;
    public Rect WindowRect = new Rect(300, 20, 300, 100);
    public string ScenarioName;

    void OnGUI()
    {
        GUI.color = Color.yellow;
        WindowRect = GUI.Window(WindowId, WindowRect, DockableWindowFunc, "Connect");
        GUI.color = Color.white;
    }

    public void DockableWindowFunc(int id)
    {
        //DoResizing();
        GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
        ScenarioName = GUILayout.TextField(ScenarioName);
        if (GameManager.Instance.NetworkManager.Connected)
        {
            if (GUILayout.Button("Disconnect"))
            {
                GameManager.Instance.NetworkManager.Disconnect(false);
            }
            if (GameManager.Instance.GameInfo != null)
            {
                if (GUILayout.Button("Start Game"))
                {
                    GameManager.Instance.NetworkManager.StartGame();
                    this.enabled = false;
                }
            }
            else
            {
                GUILayout.Label("Gameinfo is null");
            }
            if (GUILayout.Button("Load scenario"))
            {
                GameManager.Instance.NetworkManager.LoadScenario(ScenarioName);
            }



        }
        else
        {
            if (!GameManager.Instance.NetworkManager.ServerStarted)
            {
                if (GUILayout.Button("Start Server"))
                {
                    GameManager.Instance.NetworkManager.StartServer();
                }
                if (GUILayout.Button("Connect"))
                {
                    GameManager.Instance.NetworkManager.Connect();
                }
            }
            else
            {
                if (GUILayout.Button("Connect"))
                {
                    GameManager.Instance.NetworkManager.Connect();
                }
            }
        }


    }
}
