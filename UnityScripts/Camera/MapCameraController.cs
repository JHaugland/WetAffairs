using UnityEngine;
using System.Collections;

public class MapCameraController : MonoBehaviour
{

    private int _CurrentZoomLevel;

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

    private Vector3 _BeforePos;
    private Vector3 _AfterPos;

    public ZoomLevel[] ZoomLevels;

    public float MoveSpeed = 100.0f;
    public float ZoomSpeed = 100.0f;
    public int ClickZoomSpeed = 3;

    public float m_stiffness = 1000f;
    public float m_damping = 1000f;
    public float m_mass = 10f;
    public Vector3 m_velocity = Vector3.zero;

    public float MaxHeight = 40000;
    public float MinHeight = 30000;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * 100 * Time.deltaTime, 0, Input.GetAxis("Vertical") * 100 * Time.deltaTime), Space.World);

    }

    private void Zoom()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");



        if (GameManager.Instance.CameraManager.CamMode == CameraManager.CameraMode.Satelite && this.camera.rect != new Rect(0, 0, 0, 1))
        {
            Vector3 cubePos = Vector3.zero;
            RaycastHit hit;

            Ray screenToRay = this.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToRay, out hit))
            {
                float x = hit.point.x;
                float y = hit.point.z;
                cubePos = hit.point;
                //cubeSize = new Vector3(10, 10, 10);

            }
            DoZoom(delta, false);


            Vector3 after = Vector3.zero;
            screenToRay = this.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToRay, out hit) && cubePos != Vector3.zero && Mathf.Abs(delta) > 0)
            {
                after = hit.point;
                Vector3 moveDirection = cubePos - after;
                moveDirection.y = 0;
                transform.Translate(moveDirection, Space.World);

            }
        }

    }

    void DoZoom(float delta, bool zoomStraight)
    {
        //delta *= transform.position.y / MaxHeight;
        //Debug.Log(string.Format("delta : {0}   {1}", delta, _tempDestination));

        Vector3 stretch = new Vector3(0, delta, 0);
        Vector3 force = -m_stiffness * stretch - m_damping * m_velocity * ZoomSpeed;

        // Apply acceleration
        Vector3 acceleration = force / m_mass;
        m_velocity += acceleration * Time.deltaTime;

        // Apply velocity
        Vector3 newPosition = transform.position + m_velocity;// * ZoomSpeed);
        //Debug.Log(m_velocity);
        if (newPosition.y > MinHeight && newPosition.y < MaxHeight)
        {
            //~ Debug.Log("delta=" + delta);
            transform.position += m_velocity;// * ZoomSpeed;
        }

    }

    
    

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(_AfterPos, new Vector3(5, 2, 5));
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(_BeforePos, new Vector3(5, 2, 5));
    }
}
