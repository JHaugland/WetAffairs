using UnityEngine;
using System.Collections;

public class SateliteCam : MonoBehaviour
{

    public float MoveSpeed = 100.0f;
    public float ZoomSpeed = 100.0f;
    public int ClickZoomSpeed = 3;

    public float m_stiffness = 1000f;
    public float m_damping = 1000f;
    public float m_mass = 10f;
    public Vector3 m_velocity = Vector3.zero;


    public float MaxHeight = 40000;
    public float MinHeight = 35000;


    public float[] ZoomLevels;

    private int _CurrentZoomLevel;

    public int NumFrameJitterStop = 50;

    private int _CurrentFrameJitterStop = 0;
    private bool _JitterStop = false;


    public Vector3 _tempDestination = Vector3.zero;

    //TESTCODE
    private bool _ZoomingOut = true;


    public void StopZoom()
    {
        _ZoomingOut = true;
    }

    public Vector3 TempDestination
    {
        get
        {
            return _tempDestination;
        }
        set
        {
            _tempDestination = value;
        }
    }

    public int CurrentZoomLevel
    {
        get
        {
            return _CurrentZoomLevel;
        }
        set
        {
            _CurrentZoomLevel = value;
            if (value < 0)
            {
                _CurrentZoomLevel = 0;
            }
            if (_CurrentZoomLevel >= ZoomLevels.Length)
            {
                _CurrentZoomLevel = ZoomLevels.Length - 1;
            }
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SateliteMove(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        //if(Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        //{
        //    _tempDestination = Vector3.zero;
        //}
        //else if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
        //{
        //    _ZoomingOut = true;
        //}


        //if(_tempDestination != Vector3.zero)
        //{
        //    if(Vector3.Distance(transform.position, _tempDestination) < 100)
        //    {
        //        _tempDestination = Vector3.zero;
        //    }
        //    else
        //    {
        //        transform.position = Vector3.Lerp(transform.position, _tempDestination, Time.deltaTime * ClickZoomSpeed);
        //    }
        //}
        Zoom(Input.GetAxis("Mouse ScrollWheel"), false);
    }

    void SateliteMove(Vector3 moveDirection)
    {
        transform.Translate(moveDirection, Space.World);
    }

    void SateliteZoom(Vector3 delta)
    {

    }

    public void Zoom(float delta, bool zoomStraight)
    {


        if (GameManager.Instance.CameraManager.CamMode == CameraManager.CameraMode.Satelite && this.camera.rect != new Rect(0, 0, 0, 1))
        {
            cubePos = Vector3.zero;
            RaycastHit hit;

            Ray screenToRay = this.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToRay, out hit))
            {
                float x = hit.point.x;
                float y = hit.point.z;
                cubePos = hit.point;
                cubeSize = new Vector3(10, 10, 10);

            }

            //if(delta > 0)
            //{
            //    CurrentZoomLevel++;
            //}
            //else if(delta < 0)
            //{
            //    CurrentZoomLevel--;
            //}

            float m = (Vector3.Distance(transform.position, hit.point) / 1700);
            DoZoom(delta, m);
            //DoTestZoom();


            Vector3 after = Vector3.zero;
            screenToRay = this.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToRay, out hit) && cubePos != Vector3.zero)
            {
                after = hit.point;
                Vector3 moveDirection = cubePos - after;

                //moveDirection *= delta > 0 ? -1 : 1;
                //~ if(delta == 0.0fm_
                //~ {
                //~ _CurrentFrameJitterStop++;

                //~ if(_CurrentFrameJitterStop >= NumFrameJitterStop)
                //~ {
                //~ _JitterStop = true;
                //~ _CurrentFrameJitterStop = 0;
                //~ }
                //~ }
                //~ else
                //~ {
                //~ _JitterStop = false;
                //~ }

                moveDirection.y = 0;
                //if(Vector3.Distance(transform.position, new Vector3(transform.position.x, ZoomLevels[CurrentZoomLevel], transform.position.z)) > 10)
                //{
                transform.Translate(moveDirection, Space.World);
                //TempDestination += moveDirection;
                //Debug.Log(TempDestination);
                //}

            }
        }

    }

    private void DoTestZoom()
    {
        //~ if(!_JitterStop)
        //~ {
        transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, ZoomLevels[CurrentZoomLevel], transform.position.z), Time.deltaTime);
        //~ }
    }


    void OnGUI()
    {
        //~ GUI.Box(ZoomRect,"Zoom");
        //~ GUI.Box(_AfterZoom,"After");
        //GUI.Button(new Rect((, 0.0f, 50, 50), "test");

    }

    //~ void TestZoom(float delta)
    //~ {
    //~ Vector3 zoomAmount = delta > 0 ? new Vector3(transform.position.x, ZoomLevels[CurrentZoomLevel--], transform.position.z) : new Vector3(transform.position.x, ZoomLevels[CurrentZoomLevel++], transform.position.z);
    //~ transform.Translate(zoomAmount - transform.position, Space.World);
    //~ }

    //void DoZoom(float delta, float modifier)
    //{
    //    //delta *= transform.position.y / MaxHeight;
    //    //Debug.Log(string.Format("delta : {0}   {1}", delta, _tempDestination));

    //    Vector3 stretch = new Vector3(0, delta, 0) * modifier;
    //    Vector3 force = -m_stiffness * stretch - m_damping * m_velocity * ZoomSpeed;

    //    // Apply acceleration
    //    Vector3 acceleration = force / m_mass;
    //    m_velocity += (acceleration * Time.deltaTime);

    //    // Apply velocity
    //    Vector3 newPosition = transform.position + m_velocity;// * ZoomSpeed);
    //    //Debug.Log(m_velocity);
    //    newPosition.y = Mathf.Clamp(newPosition.y, 30001, 32000);

    //    //~ Debug.Log("delta=" + delta);
    //    transform.position = newPosition;// * ZoomSpeed;


    //}

    void DoZoom(float delta, float modifier)
    {
        float change = Mathf.Clamp((delta * ZoomSpeed) * modifier, -10, 10);
        Vector3 stretch = new Vector3(0, change, 0);
        
        TempDestination += stretch;
        _tempDestination.x = transform.position.x;
        _tempDestination.z = transform.position.z;
        _tempDestination.y = Mathf.Clamp(_tempDestination.y, 30001, 31700);
        transform.position = Vector3.Lerp(transform.position, TempDestination, Time.deltaTime);

    }

    private Vector3 cubePos = Vector3.zero;
    private Vector3 cubeSize = Vector3.zero;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(cubePos, cubeSize);
        //~ Gizmos.DrawGUITexture(ZoomRect, ZoomTexture);
    }



}

