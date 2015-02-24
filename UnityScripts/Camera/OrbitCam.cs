using UnityEngine;
using System.Collections;

public class OrbitCam : MonoBehaviour
{

    public Transform target;
    public float distance = 10.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float zoomSpeed = 10.0f;
    public float FollowSlerpSpeed = 10.0f;

    public float m_stiffness = 100f;
    public float m_damping = 10f;
    public float m_mass = 1f;
    public float m_velocity = 0.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float defaultDistance = 300;

    private float x = 0.0f;
    private float y = 0.0f;

    public float m_MinDistance = 100;
    public float m_MaxDistance = 500;

    public GameObject CameraController;

    public bool IsStandardMode = true;
    private bool _Enabled = false;

    private Transform _TempTarget;
    private ParticleEmitter _UnderwaterEmitter;

    public bool Enabled
    {
        get
        {
            return _Enabled;
        }
        set
        {
            _Enabled = value;
        }
    }



    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            if (target != value)
            {
                //_TempTarget = value;
                ////distance = defaultDistance;
                //DoMove(Vector3.zero);

                ////Wait for a few seconds
                //StartCoroutine(FadeOut(2));

                target = value;
                GetComponent<CameraFade>().FadeIn();
                GameManager.Instance.EnviromentManager.Skydome.Follower.target = value;
            }
        }
    }

    private IEnumerator FadeOut(int secondsToBeforeFadeIn)
    {
        GetComponent<CameraFade>().FadeOut();

        yield return new WaitForSeconds(secondsToBeforeFadeIn);
        target = _TempTarget;
        GetComponent<CameraFade>().FadeIn();

        //yield return new WaitForSeconds(secondsToBeforeFadeIn);
    }

    private void FadeIn()
    {

    }

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    // Use this for initialization
    void Start()
    {
        Rect rect = MathHelper.ScreenRectToViewportRect(new Rect(0, Screen.height / 2, Screen.height / 2, Screen.height / 2));
        this.camera.rect = rect;

        Rect satelite = new Rect(rect.width, 0, 1 - rect.width, 1);
        GameManager.Instance.CameraManager.SateliteCamera.rect = satelite;

        if (target)
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            // Make the rigid body not change rotation
            //if (rigidbody)
            //    rigidbody.freezeRotation = true;

            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = Vector3.Slerp(transform.position, position, Time.deltaTime * FollowSlerpSpeed);
        }

        _UnderwaterEmitter = GetComponentInChildren<ParticleEmitter>();
    }

    void Update()
    {
        if (GameManager.Instance.UnitManager.SelectedUnit != null)
        {
            if (GameManager.Instance.UnitManager.SelectedUnit.Info.DomainType == TTG.NavalWar.NWComms.GameConstants.DomainType.Air)
            {
                if (GameManager.Instance.UnitManager.SelectedUnit.transform.position.y > 601)
                {
                    yMaxLimit = 89;
                    yMinLimit = 45;
                }
                else
                {
                    yMinLimit = 20;
                }
            }
        }
        if ( _UnderwaterEmitter )
        {
            _UnderwaterEmitter.emit = transform.position.y < 0;
        }
       
    }

    void LateUpdate()
    {
        if (Enabled && !GameManager.Instance.InputManager.IsMouseOverGUI)
        {
            if (Input.GetMouseButton(0))
            {
                Move(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            }
            else
            {
                Move(Vector2.zero);
            }
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    void OnGUI()
    {

    }


    public void Move(Vector2 moveDirection)
    {
        //if (Input.anyKey)
        //{
        DoMove(moveDirection);
        //}
    }

    public void Zoom(float deltaValue)
    {
        DoZoom(deltaValue);
    }

    private void DoMove(Vector2 moveDirection)
    {

        if (target)
        {//&& (gameObject.GetComponent(typeof(Camera)) as Camera).enabled) {

            x += moveDirection.x * xSpeed;
            y -= moveDirection.y * ySpeed;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
            //transform.position = Vector3.Slerp(transform.position, position, Time.deltaTime * FollowSlerpSpeed);
        }

    }

    private void DoZoom(float deltaValue)
    {
        if (target != null)
        {
            float force = -m_stiffness * deltaValue - m_damping * m_velocity;

            // Apply acceleration
            float acceleration = force / m_mass;
            m_velocity += acceleration * Time.deltaTime;

            distance += m_velocity * zoomSpeed;
            distance = Mathf.Clamp(distance, m_MinDistance, m_MaxDistance);

            if (deltaValue != 0 || m_velocity != 0.0f)
            {
                Vector3 position = transform.rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5);
            }
        }
        else
        {
            //~ GameManager.Instance.CameraManager.SwitchCamera(GameManager.Instance.CameraManager.SateliteCamera);
        }
    }


    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }


}


