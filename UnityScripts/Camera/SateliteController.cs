using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;

public class SateliteController : MonoBehaviour
{

    public float XMoveSpeed = 10;
    public float YMoveSpeed = 10;
    public float ZoomMaxSpeed = 10;
    public float DragSpeed = 10;

    public Vector3 MaxMove;
    public Vector3 MinMove;


    private Vector3 _WantedPosition = Vector3.zero;
    private Vector3 _LastMousePoint;
    private bool _IsLeftMouseDown;
    private float _MouseXDelta;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (MathHelper.ViewportRectToScreenRect(this.camera.rect).Contains(Input.mousePosition) && GameManager.Instance.CameraManager.CurrentCamera == this.camera)
        {
            Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

            if (!GameManager.Instance.InputManager.IsMouseOverGUI)
            {
                RaycastHit hit;
                Ray screenToRay = this.camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(screenToRay, out hit))
                {
                    float mod = (Vector3.Distance(transform.position, hit.point) / 1000);
                    //Debug.Log(mod);
                    Zoom(-Input.GetAxis("Mouse ScrollWheel") * mod);

                    if (transform.position.y > MinMove.y && transform.position.y < MaxMove.y)
                    {
                        Vector3 before = hit.point;

                        if (Physics.Raycast(this.camera.ScreenPointToRay(Input.mousePosition), out hit))
                        {
                            transform.position += before - hit.point;
                        }

                        transform.position = MathHelper.ClampVector(transform.position, MinMove, MaxMove);
                    }
                    Drag();
                    
                }
                
            }

        }
        //else if (_IsLeftMouseDown)
        //{
        //    Drag();

        //}


        Pan();

    }

    public void CenterOnPosition(PositionInfo position)
    {
        Vector3 newPos = new Vector3();
        newPos.x = ((float)position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier) + GameManager.Instance.XMapAddition;
        newPos.z = ((float)position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier) + GameManager.Instance.YMapAddtion;


        newPos.y = transform.position.y;
        
        transform.position = newPos;
        
        
    }

    void Pan()
    {
        float amount = 0;
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            amount = 1;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            amount = -1;
        }
        if (amount != 0)
        {
            Vector3 euler = transform.eulerAngles;

            euler.x += amount;
            euler.x = Mathf.Clamp(euler.x, 45, 80);
            transform.eulerAngles = euler;
        }
    }


    
    void Drag()
    {
        if (_IsLeftMouseDown)
        {
            RaycastHit hit;
            float mod = 1;
            if (Physics.Raycast(this.transform.position, -Vector3.up, out hit))
            {
                mod = (Vector3.Distance(transform.position, hit.point) / 1000);

            }

            Vector3 moveDirection = _LastMousePoint != Vector3.zero ? Input.mousePosition - _LastMousePoint : _LastMousePoint;
            moveDirection.z = moveDirection.y;
            moveDirection.y = 0;
            //~ Debug.Log(moveDirection);
            transform.Translate(-moveDirection * DragSpeed * mod, Space.World);

            _LastMousePoint = Input.mousePosition;
            //~ Debug.Log(LastMousePoint);
        }
        else
        {
            _LastMousePoint = Vector3.zero;
        }
        _IsLeftMouseDown = Input.GetMouseButton(0);
    }

    void Move(Vector3 moveDirection)
    {
        RaycastHit hit;
        float mod = 1;
        if (Physics.Raycast(this.transform.position, -Vector3.up, out hit))
        {
            mod = (Vector3.Distance(transform.position, hit.point) / 1000);

        }
        transform.Translate(new Vector3(moveDirection.x * XMoveSpeed, 0, moveDirection.z * YMoveSpeed) * mod, Space.World);

        transform.position = MathHelper.ClampVector(transform.position, MinMove, MaxMove);
    }

    void Zoom(float delta)
    {
        if (delta < 0 || transform.position.y < MaxMove.y)
        {
            transform.Translate(new Vector3(0, delta * ZoomMaxSpeed, 0), Space.World);
        }
    }
}
