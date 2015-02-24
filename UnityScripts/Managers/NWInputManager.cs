using UnityEngine;
using System.Collections;



public class NWInputManager : MonoBehaviour
{


    public float DoubleClickTime = 0.5f;
    public float MouseSensitivity = 0.1f;
    public float SateliteMoveSpeed = 100.0f;
    public float SateliteDragSpeed = 200.0f;
    public float SateliteZoomSpeed = 300.0f;
    public GameObject GUIRoot;




    private bool _leftClicked = false;
    private bool _rightClicked = false;
    private float _leftLastClickedTime = 0.0f;
    private float _rightLastClickedTime = 0.0f;
    public float _sensitivity;
    //START SATELITE SCRIPTS
    public SateliteCam _SateliteCam;
    public OrbitCam _OrbitCam;
    public GameObject Map;
    //END SATELITESCRIPTS
    private Vector3 LastMousePoint = Vector3.zero;
    private bool IsLeftMouseDown = false;
    private DockableWindow _WindowWithFocus;
    private bool _IsMouseOverGUI;


    private CameraManager.CameraMode CamMode;

    public bool IsMouseOverGUI
    {
        get
        {
            return _IsMouseOverGUI;
        }
        set
        {
            _IsMouseOverGUI = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        //~ sateliteController = GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteController)) as SateliteController;
        //~ sateliteCam = GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam;
        _sensitivity = MouseSensitivity;
        if (_SateliteCam != null)
        {
            _SateliteCam = GameManager.Instance.CameraManager.SateliteCamera.GetComponent(typeof(SateliteCam)) as SateliteCam;
        }
        if (_OrbitCam != null)
        {
            _OrbitCam = GameManager.Instance.CameraManager.MainCamera.GetComponent(typeof(OrbitCam)) as OrbitCam;
        }

        CamMode = GameManager.Instance.CameraManager.CamMode;
    }


    void OnSelectedCameraChanged()
    {
        //Debug.Log("CamMode changed");
        CamMode = GameManager.Instance.CameraManager.CamMode;
        if (CamMode == CameraManager.CameraMode.Satelite)
        {
            //Disable SurfaceControl script and enable satelite control
            _OrbitCam.Enabled = false;
            //_SateliteCam.enabled = true;
            //_MapMove.enabled = true;
        }
        else
        {
            _OrbitCam.Enabled = true;
            //_SateliteCam.enabled = false;
            //_MapMove.enabled = false;
            //Disable satelite, enable surface
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Rect temp = GameManager.Instance.CameraManager.SateliteCamera.rect;
            GameManager.Instance.CameraManager.SateliteCamera.rect = GameManager.Instance.CameraManager.MainCamera.rect;
            GameManager.Instance.CameraManager.MainCamera.rect = temp;
            SendMessage("OnSelectedCameraChanged", SendMessageOptions.DontRequireReceiver);
        }

        SateliteClick();
    }

    void KeyBoardClick()
    {

    }

    void SateliteMove(Vector3 moveDirection)
    {
        Vector3 newPos = GameManager.Instance.CameraManager.SateliteCamera.transform.localPosition + moveDirection;
        //~ Debug.Log(newPos);

        //~ Debug.Log("stop zoom");
        if (Input.GetMouseButtonDown(2))
        {
            _SateliteCam.StopZoom();
        }


        if (newPos.x > 0 && newPos.x < 10000 && newPos.z > 0 && newPos.z <= 10000)
        {
            GameManager.Instance.CameraManager.SateliteCamera.transform.Translate(moveDirection * (GameManager.Instance.CameraManager.SateliteCamera.transform.localPosition.y / 40000), Space.World);
        }
    }

    void SateliteZoom()
    {
        Camera sat = GameManager.Instance.CameraManager.SateliteCamera;
        if (sat != null)
        {
            //_SateliteCam.Zoom(Input.GetAxis("Mouse ScrollWheel") * SateliteZoomSpeed, GameManager.Instance.GameState != GameManager.GameStates.Satelite);
            //~ Debug.Log(GameManager.Instance.CameraManager.SateliteCamera.ViewportToWorldPoint(Input.mousePosition));
        }
    }

    void SateliteDrag(Vector3 mousePos)
    {

        Vector3 moveDirection = LastMousePoint != Vector3.zero ? mousePos - LastMousePoint : LastMousePoint;
        float temp = moveDirection.y;
        moveDirection = new Vector3(moveDirection.x, moveDirection.z, temp);
        //~ Debug.Log(moveDirection);
        SateliteMove(-moveDirection * SateliteDragSpeed);
        if (!IsLeftMouseDown)
        {
            (_SateliteCam.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = Vector3.zero;
        }
        LastMousePoint = mousePos;
        //~ Debug.Log(LastMousePoint);
        IsLeftMouseDown = true;

    }

    void SateliteClick()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //~ Debug.Log(GUIUtility.keyboardControl);
            if (GUIUtility.hotControl == 0)
            {
                //~ if(_leftClicked)
                //~ {         

                //~ if (Time.time - _leftLastClickedTime > DoubleClickTime)
                //~ {
                //~ //too long, so set this as a first click
                //~ _leftClicked = true;
                //~ _leftLastClickedTime = Time.time;
                //~ }
                //~ else
                //~ {

                //~ //it was a double left click!
                //~ _leftClicked = false;
                //~ _leftLastClickedTime = 0.0f;

                //~ //TOADD: DO DOUBLE CLICK LOGIC
                //~ RaycastHit hit;

                //~ if(Physics.Raycast(GameManager.Instance.CameraManager.SateliteCamera.ScreenPointToRay (Input.mousePosition), out hit))
                //~ {
                //~ (GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = new Vector3(hit.point.x, 36100, hit.point.z);

                //~ }
                //~ }
                //~ }
                //~ else
                //~ {
                //~ _leftClicked = true;
                //~ _leftLastClickedTime = Time.time;
                //~ } 
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (GameManager.Instance.UnitManager.SelectedUnit != null && !IsMouseOverGUI)
            {
                MoveToMouse();
            }



            //~ if(_rightClicked)
            //~ {         

            //~ if (Time.time - _rightLastClickedTime > DoubleClickTime)
            //~ {
            //~ //too long, so set this as a first click
            //~ _rightClicked = true;
            //~ _rightLastClickedTime = Time.time;
            //~ }
            //~ else
            //~ {

            //~ //it was a double left click!
            //~ _rightClicked = false;
            //~ _rightLastClickedTime = 0.0f;
            //~ RaycastHit hit;

            //~ if(Physics.Raycast(GameManager.Instance.CameraManager.SateliteCamera.ScreenPointToRay (Input.mousePosition), out hit))
            //~ {
            //~ (GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = new Vector3(hit.point.x, 39900, hit.point.z);
            //~ }
            //~ }
            //~ }
            //~ else
            //~ {
            //~ _rightClicked = true;
            //~ _rightLastClickedTime = Time.time;
            //~ } 
        }

    }

    public void MoveToMouse()
    {
        RaycastHit hit;

        if (Physics.Raycast(GameManager.Instance.CameraManager.SateliteCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.gameObject == Map)
            {
                if (Map)
                {
                    Vector3 worldPoint = Map.transform.InverseTransformPoint(hit.point);
                    //~ Debug.Log(map.transform.InverseTransformPoint(hit.point)));
                    Vector3 point = new Vector3(worldPoint.x * Map.transform.localScale.x, worldPoint.y * Map.transform.localScale.y, worldPoint.z * Map.transform.localScale.z);
                    //~ Debug.Log(GameManager.Instance.xToLong(point.x));

                    //Vector3 pos = new Vector3(((worldPoint.x - 5.0f) * map.transform.localScale.x) * -1, worldPoint.y, ((worldPoint.z + 5.0f) * map.transform.localScale.z));

                    float a = GameManager.Instance.XMapModifier;
                    float b = GameManager.Instance.YMapModifier;

                    Vector3 pos2 = new Vector3();

                    //y = -225a-1 + a-1x

                    //pos2.x = -1347.305389f + 5.988023952f * hit.point.x;
                    pos2.x = -GameManager.Instance.XMapAddition * Mathf.Pow(a, -1) + Mathf.Pow(a, -1) * hit.point.x;
                    pos2.z = -GameManager.Instance.YMapAddtion * Mathf.Pow(b, -1) + Mathf.Pow(b, -1) * hit.point.z;
                    pos2.y = Map.transform.position.y;

                    Coordinate coordPoint = CoordinateHelper.FromOrthoProjectedCoordinate(pos2.z, pos2.x);

                    GameObject go = new GameObject("PositionMarker");
                    DetectionMarker marker = go.AddComponent<DetectionMarker>();
                    //marker.Position = new Vector3(pos2.z, 0, pos2.x);
                    marker.Position = hit.point;
                    marker.MinSize = new Vector2(8, 8);
                    marker.MaxSize = new Vector2(128, 128);
                    marker.KillTime = 1;
                    marker.FadeTime = 0.5f;
                    marker.MessageToDisplay = new Message("Movement", GameManager.MessageTypes.Game);
                    //Debug.Log(pos2);


                    //Vector3 pos = new Vector3();
                    //pos.x = ((float)_Info.Position.LongitudeOrthoProjected * 0.1671117f) + 225.0f;
                    //pos.z = ((float)_Info.Position.LatitudeOrthoProjected * -0.1675854f) + 360.0f;
                    //pos.y = 30002;



                    //Debug.Log(hit.point);
                    //Gizmos.DrawCube(hit.point, new Vector3(10, 10, 10));

                    //double lat = CoordinateHelper.YToLat(point.z);
                    //double lng = CoordinateHelper.XToLong(point.x);
                    //Debug.Log(string.Format("Lat:{0} , Long: {1}", lat, lng));

                    bool removeWaypoints = true;
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        removeWaypoints = false;
                    }

                    GameObject gui = GameObject.Find("GUI");
                    InteractionWindow iw = gui.GetComponent<InteractionWindow>();

                    if (iw.AwaitingOrder)
                    {
                        iw.Move(coordPoint);
                    }
                    else
                    {
                        GameManager.Instance.OrderManager.Move(coordPoint.Latitude, coordPoint.Longitude, GameManager.Instance.UnitManager.SelectedGroupMainUnit.Info, removeWaypoints);
                    }
                }
            }
        }
    }

    public Coordinate GetCoordinateFromScreenPoint(Vector3 screenPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(GameManager.Instance.CameraManager.SateliteCamera.ScreenPointToRay(screenPoint), out hit))
        {


            Vector3 worldPoint = Map.transform.InverseTransformPoint(hit.point);
            //~ Debug.Log(map.transform.InverseTransformPoint(hit.point)));
            Vector3 point = new Vector3(worldPoint.x * Map.transform.localScale.x, worldPoint.y * Map.transform.localScale.y, worldPoint.z * Map.transform.localScale.z);
            //~ Debug.Log(GameManager.Instance.xToLong(point.x));

            //Vector3 pos = new Vector3(((worldPoint.x - 5.0f) * map.transform.localScale.x) * -1, worldPoint.y, ((worldPoint.z + 5.0f) * map.transform.localScale.z));

            float a = GameManager.Instance.XMapModifier;
            float b = GameManager.Instance.YMapModifier;

            Vector3 pos2 = new Vector3();

            //y = -225a-1 + a-1x

            //pos2.x = -1347.305389f + 5.988023952f * hit.point.x;
            pos2.x = -GameManager.Instance.XMapAddition * Mathf.Pow(a, -1) + Mathf.Pow(a, -1) * hit.point.x;
            pos2.z = -GameManager.Instance.YMapAddtion * Mathf.Pow(b, -1) + Mathf.Pow(b, -1) * hit.point.z;
            pos2.y = Map.transform.position.y;

            Coordinate coordPoint = CoordinateHelper.FromOrthoProjectedCoordinate(pos2.z, pos2.x);

            return coordPoint;

        }
        return null;
    }

    public void MoveToPoint(Vector3 screenPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(GameManager.Instance.CameraManager.SateliteCamera.ScreenPointToRay(screenPoint), out hit))
        {

            if (Map)
            {
                Vector3 worldPoint = Map.transform.InverseTransformPoint(hit.point);
                //~ Debug.Log(map.transform.InverseTransformPoint(hit.point)));
                Vector3 point = new Vector3(worldPoint.x * Map.transform.localScale.x, worldPoint.y * Map.transform.localScale.y, worldPoint.z * Map.transform.localScale.z);
                //~ Debug.Log(GameManager.Instance.xToLong(point.x));

                //Vector3 pos = new Vector3(((worldPoint.x - 5.0f) * map.transform.localScale.x) * -1, worldPoint.y, ((worldPoint.z + 5.0f) * map.transform.localScale.z));

                float a = GameManager.Instance.XMapModifier;
                float b = GameManager.Instance.YMapModifier;

                Vector3 pos2 = new Vector3();

                //y = -225a-1 + a-1x

                //pos2.x = -1347.305389f + 5.988023952f * hit.point.x;
                pos2.x = -GameManager.Instance.XMapAddition * Mathf.Pow(a, -1) + Mathf.Pow(a, -1) * hit.point.x;
                pos2.z = -GameManager.Instance.YMapAddtion * Mathf.Pow(b, -1) + Mathf.Pow(b, -1) * hit.point.z;
                pos2.y = Map.transform.position.y;

                Coordinate coordPoint = CoordinateHelper.FromOrthoProjectedCoordinate(pos2.z, pos2.x);

                GameObject go = new GameObject("PositionMarker");
                DetectionMarker marker = go.AddComponent<DetectionMarker>();
                //marker.Position = new Vector3(pos2.z, 0, pos2.x);
                marker.Position = hit.point;
                marker.MinSize = new Vector2(8, 8);
                marker.MaxSize = new Vector2(128, 128);
                marker.KillTime = 1;
                marker.FadeTime = 0.5f;
                marker.MessageToDisplay = new Message("Movement", GameManager.MessageTypes.Game);
                //Debug.Log(pos2);


                //Vector3 pos = new Vector3();
                //pos.x = ((float)_Info.Position.LongitudeOrthoProjected * 0.1671117f) + 225.0f;
                //pos.z = ((float)_Info.Position.LatitudeOrthoProjected * -0.1675854f) + 360.0f;
                //pos.y = 30002;



                //Debug.Log(hit.point);
                //Gizmos.DrawCube(hit.point, new Vector3(10, 10, 10));

                //double lat = CoordinateHelper.YToLat(point.z);
                //double lng = CoordinateHelper.XToLong(point.x);
                //Debug.Log(string.Format("Lat:{0} , Long: {1}", lat, lng));

                bool removeWaypoints = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    removeWaypoints = false;
                }

                GameObject gui = GameObject.Find("GUI");
                InteractionWindow iw = gui.GetComponent<InteractionWindow>();

                if (iw.AwaitingOrder)
                {
                    iw.Move(coordPoint);
                }
                else
                {
                    GameManager.Instance.OrderManager.Move(coordPoint.Latitude, coordPoint.Longitude, GameManager.Instance.UnitManager.SelectedGroupMainUnit.Info, removeWaypoints);
                }
            }
        }
    }

    private void GameCameraMove(Vector2 moveDirection)
    {
        (GameManager.Instance.CameraManager.MainCamera.gameObject.GetComponent(typeof(OrbitCamNWAC)) as OrbitCamNWAC).Move(moveDirection);
    }

    private void GameCameraZoom(float deltaValue)
    {
        (GameManager.Instance.CameraManager.MainCamera.gameObject.GetComponent(typeof(OrbitCamNWAC)) as OrbitCamNWAC).Zoom(deltaValue);
    }
}
