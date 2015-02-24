using UnityEngine;
using System.Collections;

public class ScreenSplitController : MonoBehaviour
{

    public Camera SurfaceCamera;
    public Camera SateliteCamera;

    public bool ClampLeft = false;

    public GUIStyle SeparatorStyle;
    public GUIStyle SnapStyle;
    public GUIStyle FocusStyle;

    public int Id;


    private Rect _SeperatorRect;
    private bool _IsMouseDown = false;
    private float _Delta = 0.0f;
    private bool _ShowSnapLeft = false;
    private bool _ShowSnapRight = false;
    private bool _Snap = false;
    private bool _Snapped = false;
    private Rect _SnapRect;
    private Rect _FULL_VIEW_RECT = new Rect(0, 0, 1, 1);
    private Rect _EMPTY_RECT = new Rect(0, 0, 0, 1);
    private float _LastKnownMouseX;

    public Rect SeparatorRect
    {
        get
        {
            return _SeperatorRect;
        }
    }

    ////// Use this for initialization
    void Start()
    {
        _SeperatorRect = new Rect(Screen.width / 2, 0, 10, Screen.height);
    }

    ////// Update is called once per frame
    ////void Update () {

    ////}

    void OnGUI()
    {
        if (_IsMouseDown)
        {

            float mouseX = Input.mousePosition.x;
            float mouseViewportX = mouseX / Screen.width;
            float clampViewportX = 400.0f / (float)Screen.width;

            

            //Divide screen into 4 parts
            float quartScreen = Screen.width / 4;
            _ShowSnapLeft = false;
            _ShowSnapRight = false;
            if (mouseX > Screen.width - quartScreen)
            //More than 75% to the right
            {
                _ShowSnapRight = true;
            }
            else if (mouseX < quartScreen)
            //Less than a quarter of screen
            {
                _ShowSnapLeft = true;
            }
            bool freeze = _ShowSnapLeft == true || _ShowSnapRight == true;

            Rect sat = SateliteCamera.rect;
            Rect surface = SurfaceCamera.rect;

            if (mouseViewportX <= clampViewportX)
            {
                _Delta = 0;
                _ShowSnapLeft = false;
            }

            if (_Delta > 0)
            {
                surface.width = mouseViewportX;
                sat.x = mouseViewportX;
            }
            else if (_Delta < 0)
            {
                //if (ClampLeft)
                //{
                //    Debug.Log(string.Format("Clampleft {0}   mousex {1}", clampViewportX, mouseViewportX));
                //    sat.x = Mathf.Clamp(mouseViewportX, clampViewportX, 1);
                //    sat.width = 1 - Mathf.Clamp(mouseViewportX, clampViewportX, 1);
                //    surface.width = Mathf.Clamp(mouseViewportX, clampViewportX, 1);

                //    if (mouseViewportX < clampViewportX)
                //    {
                //        _IsMouseDown = false;
                //    }
                //}
                //else
                //{
                    sat.x = mouseViewportX;
                    sat.width = 1 - mouseViewportX;
                    surface.width = mouseViewportX;
                //}
            }

            //_SeperatorRect.x = freeze == false ? mouseX : _SeperatorRect.x;
            _SeperatorRect.x = SateliteCamera.rect.x * Screen.width;

            SurfaceCamera.rect = surface;
            SateliteCamera.rect = sat;

            //Calculate snap rects
            if (_ShowSnapLeft)
            {
                _SnapRect = new Rect(0, 0, mouseX, Screen.height);
                SnapStyle.Draw(_SnapRect, GUIContent.none, Id);
            }
            if (_ShowSnapRight)
            {
                _SnapRect = new Rect(mouseX, 0, Screen.width - mouseX, Screen.height);
                SnapStyle.Draw(_SnapRect, GUIContent.none, Id);
            }
        }

        if (_Snap)
        {
            if (_ShowSnapLeft)
            {
                SateliteCamera.rect = _FULL_VIEW_RECT;
                SurfaceCamera.rect = _EMPTY_RECT;
                //~ SurfaceCamera.enabled = false;
                ShowSeperator(0);
            }
            else if (_ShowSnapRight)
            {
                SurfaceCamera.rect = _FULL_VIEW_RECT;
                SateliteCamera.rect = _EMPTY_RECT;
                //~ SateliteCamera.enabled = false;
                ShowSeperator(Screen.width - _SeperatorRect.width);
            }

            _Snap = false;
            _Snapped = true;
            _ShowSnapLeft = false;
            _ShowSnapRight = false;
        }

        //if (_Snapped)
        //{
        //    bool left = SateliteCamera.rect == _FULL_VIEW_RECT;

        //    Rect buttonRect = left == true ? new Rect(0, Screen.height - 100, 100, 100) : new Rect(Screen.width - 100, Screen.height - 100, 100, 100);
        //    if (GUI.Button(buttonRect, "S"))
        //    {
        //        float sateliteX = _LastKnownMouseX / Screen.width;
        //        float sateliteWidth = 1 - sateliteX;
        //        float surfaceWidth = 1 - sateliteWidth;

        //        SateliteCamera.rect = new Rect(sateliteX, 0, sateliteWidth, 1);
        //        SurfaceCamera.rect = new Rect(0.0f, 0, surfaceWidth, 1);
        //        ShowSeperator(_LastKnownMouseX);
        //        _Snapped = false;
        //        SurfaceCamera.enabled = true;
        //        SateliteCamera.enabled = true;
        //    }
        //}
        GUI.Label(MathHelper.ViewportRectToScreenRect(GameManager.Instance.CameraManager.CamMode == CameraManager.CameraMode.Satelite ? SateliteCamera.rect : SurfaceCamera.rect), "", FocusStyle);

        if (!_Snap && !_Snapped && !_IsMouseDown)
        {
            _LastKnownMouseX = SateliteCamera.rect.x * Screen.width;
        }

        GUI.Window(Id, _SeperatorRect, SeperatorLogic, "", SeparatorStyle);
    }

    private void ShowSeperator(float x)
    {
        _SeperatorRect = new Rect(x, 0, 10, Screen.height);
    }

    void SeperatorLogic(int id)
    {
        Event e = Event.current;
        _Delta = e.delta.x;

        switch (e.GetTypeForControl(id))
        {
            case EventType.ContextClick:
                break;
            case EventType.DragExited:
                break;
            case EventType.DragPerform:
                break;
            case EventType.DragUpdated:
                break;
            case EventType.ExecuteCommand:
                break;
            case EventType.Ignore:
                break;
            case EventType.KeyDown:
                break;
            case EventType.KeyUp:
                break;
            case EventType.Layout:
                break;
            case EventType.MouseDown:
                _IsMouseDown = true;
                _Delta = 0;
                break;
            case EventType.MouseDrag:
                break;
            case EventType.MouseMove:
                break;
            case EventType.MouseUp:
                _IsMouseDown = false;
                break;
            case EventType.Repaint:
                break;
            case EventType.ScrollWheel:
                break;
            case EventType.Used:
                break;
            case EventType.ValidateCommand:
                break;

            default:
                break;
        }

        if (!Input.GetMouseButton(0))
        {
            _IsMouseDown = false;
            _Delta = 0.0f;
            if (_ShowSnapLeft || _ShowSnapRight)
            {
                _Snap = true;
            }
        }
    }
}
