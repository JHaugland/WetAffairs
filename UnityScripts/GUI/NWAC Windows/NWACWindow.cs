using UnityEngine;
using System.Collections;


public class NWACWindow : MonoBehaviour {

    private Rect _WindowRect;
    private Rect _LEFT_RECT;
    private Rect _RIGHT_RECT;

    public float WindowWidth;
    public int WindowId;
    //public Rect WindowDragArea;
    public Vector2 ToggleButtonSize;
    public GUIContent WindowContent;
    public GUISkin Skin;
    public GUIStyle Style;

    public InteractionWindow InteractionWindow;

    public Rect ToggleRect
    {
        get
        {
            return _WindowRect == _LEFT_RECT ? _RIGHT_RECT : _LEFT_RECT;
        }
    }

    public Rect WindowRect
    {
        get
        {
            return _WindowRect;
        }
    }

    void Start()
    {
        
        _LEFT_RECT = new Rect(0, 0, WindowWidth, Screen.height);
        _RIGHT_RECT = new Rect(Screen.width - WindowWidth, 0, WindowWidth, Screen.height);
        _WindowRect = _RIGHT_RECT;
    }

    void OnGUI()
    {
        if (Skin != null)
        {
            GUI.skin = Skin;
        }
        GUI.Window(WindowId, _WindowRect, DoWindow, WindowContent);
    }

    void DoWindow(int id)
    {
        if (GUILayout.Button("I", GUILayout.Height(100), GUILayout.Width(32)))
        {
            if (InteractionWindow.MySizeState == InteractionWindow.SizeState.Full)
            {
                InteractionWindow.MySizeState = InteractionWindow.LastSize;
            }

            //GUI FOR info
            InteractionWindow.MyGUIState = InteractionWindow.GUIState.Info;
        }

        if (GUILayout.Button("M", GUILayout.Height(100), GUILayout.Width(32)))
        {
            if (InteractionWindow.MySizeState == InteractionWindow.SizeState.Full)
            {
                InteractionWindow.MySizeState = InteractionWindow.LastSize;
            }

            //GUI for movement
            InteractionWindow.MyGUIState = InteractionWindow.GUIState.Movement;

        }
        if (GUILayout.Button("S", GUILayout.Height(100), GUILayout.Width(32)))
        {
            if (InteractionWindow.MySizeState == InteractionWindow.SizeState.Full)
            {
                InteractionWindow.MySizeState = InteractionWindow.LastSize;
            }
            InteractionWindow.MyGUIState = InteractionWindow.GUIState.Sensors;
           
        }
        if (GUILayout.Button("W", GUILayout.Height(100), GUILayout.Width(32)))
        {
            if (InteractionWindow.MySizeState == InteractionWindow.SizeState.Full)
            {
                InteractionWindow.MySizeState = InteractionWindow.LastSize;
            }
            InteractionWindow.MyGUIState = InteractionWindow.GUIState.Weapons;

        }
        if (GUILayout.Button("L_A", GUILayout.Height(100), GUILayout.Width(32)))
        {
            if (InteractionWindow.MySizeState == InteractionWindow.SizeState.Full)
            {
                InteractionWindow.MySizeState = InteractionWindow.LastSize;
            }

        }

        if (GUILayout.Button("Air", GUILayout.Height(100), GUILayout.Width(32)))
        {
            InteractionWindow.LastSize = InteractionWindow.MySizeState;
            InteractionWindow.MySizeState = InteractionWindow.SizeState.Full;
            InteractionWindow.MyGUIState = InteractionWindow.GUIState.AircraftHangar;
        }

        Rect buttonRect = new Rect(0, _WindowRect.yMax - ToggleButtonSize.y,
            ToggleButtonSize.x, ToggleButtonSize.y);
        if (GUI.Button(buttonRect, "Change Side"))
        {
            _WindowRect = ToggleRect;
            
        }
    }
    private Rect ClampRectToX(Rect rect)
    {
        return new Rect(Mathf.Clamp(rect.xMin, 0, Screen.width - rect.width), Mathf.Clamp(rect.yMin, 0, 0), Mathf.Clamp(rect.width, 0, Screen.width), Mathf.Clamp(rect.height, 0, Screen.height));
    }
}
