using UnityEngine;
using System.Collections;

public class MapUnit : MonoBehaviour
{

    public SateliteController SateliteController;
    public float ZoomFactor;
    public PlayerUnit Unit;
    private float _InitialScale;
    private int _LastButtonDown = -1;
    private bool _ContextMenuShowing = false;

    private bool _MouseOver = false;
    private bool _MouseDown = false;

    void Start()
    {
        SateliteController = GameManager.Instance.CameraManager.SateliteCamera.GetComponent<SateliteController>();
        _InitialScale = transform.localScale.x;
    }

    void Update()
    {

        //First move unit
        transform.position = Unit.MapPosition;


        float max = SateliteController.MaxMove.y;
        float min = SateliteController.MinMove.y;
        float pos = SateliteController.transform.position.y;

        ZoomFactor = 1 - ((max - pos) / (max - min));
        ZoomFactor = Mathf.Clamp(ZoomFactor, 0.01f, 1);
        transform.localScale = new Vector3(_InitialScale * ZoomFactor, (_InitialScale / 5) * ZoomFactor, _InitialScale * ZoomFactor);

        foreach (Transform t in transform)
        {
            t.localRotation = Quaternion.AngleAxis((float)Unit.Info.Position.BearingDeg + 180, Vector3.up);
        }
        

        if (Unit.Info.UnitType == TTG.NavalWar.NWComms.GameConstants.UnitType.Missile)
        {
            if (renderer.material.mainTexture != GameManager.Instance.GUIManager.MissileTex)
            {
                renderer.material.mainTexture = GameManager.Instance.GUIManager.MissileTex;
            }
        }
        else
        {
            
            if (_MouseOver)
            {
                if (_MouseDown)
                {
                    if (renderer.material.mainTexture != GameManager.Instance.GUIManager.GroupTexMouseDown)
                    {
                        renderer.material.mainTexture = GameManager.Instance.GUIManager.GroupTexMouseDown;
                    }
                }
                else
                {
                    if (renderer.material.mainTexture != GameManager.Instance.GUIManager.GroupTexMouseOver)
                    {
                        renderer.material.mainTexture = GameManager.Instance.GUIManager.GroupTexMouseOver;
                    }
                }
            }
            else
            {
                if (GameManager.Instance.UnitManager.SelectedGroupMainUnit == Unit)
                {
                    if (renderer.material.mainTexture != GameManager.Instance.GUIManager.GroupTexSelectedUnit)
                    {
                        renderer.material.mainTexture = GameManager.Instance.GUIManager.GroupTexSelectedUnit;
                    }
                }
                else
                {

                    if (renderer.material.mainTexture != GameManager.Instance.GUIManager.GroupTex)
                    {
                        renderer.material.mainTexture = GameManager.Instance.GUIManager.GroupTex;
                    }
                }
            }


            if (Input.GetMouseButtonUp(1) && _MouseOver)
            {
                _ContextMenuShowing = !_ContextMenuShowing;
            }
        }
    }

    void OnGUI()
    {
        _LastButtonDown = Event.current.button;
        //Debug.Log(_LastButtonDown);
        if (_ContextMenuShowing && GameManager.Instance.UnitManager.SelectedGroupMainUnit != Unit)
        {
            Vector3 guiScreenPos = SateliteController.camera.WorldToScreenPoint(transform.position);

            Rect contectMenuRect = new Rect(guiScreenPos.x, SateliteController.camera.pixelHeight - guiScreenPos.y, 200, 100);

            GUI.Box(contectMenuRect, "Context Menu");
            contectMenuRect.y += 10;
            contectMenuRect.height -= 10;

            GUILayout.BeginArea(contectMenuRect);
            if (GUILayout.Button("Join group"))
            {
                GameManager.Instance.OrderManager.JoinGroups(GameManager.Instance.UnitManager.SelectedGroupMainUnit, Unit);
                Debug.Log("Join");

            }
            if (GUILayout.Button("Move here"))
            {
                GameManager.Instance.InputManager.MoveToPoint(guiScreenPos);
                _ContextMenuShowing = false;
            }
            GUILayout.EndArea();
        }

        if (_MouseOver)
        {
            GUI.tooltip = Unit.Info.UnitName;
            GameManager.Instance.GUIManager.ShowToolTip(GUI.tooltip);

        }
    }

    void OnMouseOver()
    {
        _MouseOver = true;

    }
    void OnMouseExit()
    {
        _MouseOver = false;
    }

    void OnMouseDrag()
    {
        _MouseDown = true;
    }

    void OnTriggerStay(Collider col)
    {
    }

    void OnMouseUp()
    {
        //Debug.Log(_LastButtonDown);
        _MouseDown = false;
        if (Unit != null && (GameManager.Instance.UnitManager.SelectedUnit != Unit || GameManager.Instance.UnitManager.CurrentWatchMode != UnitManager.UnitToWatch.Player))
        {
            GameManager.Instance.UnitManager.SelectedUnit = Unit;
        }
    }

    public void Explode()
    {
        Vector3 pos = transform.position;
        pos.y += 10;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("MapExplosion");
        bool detonate =  true;
        foreach ( GameObject go in gos )
        {
            if ( Vector3.Distance(go.transform.position, transform.position) < 10 )
            {
                detonate = false;
                break;
            }
        }
        if(detonate)
        {
            GameObject go = Instantiate(GameManager.Instance.MissileMapExplosion, pos, Quaternion.identity) as GameObject;
            go.name = "MapExplosion";
            go.tag = "MapExplosion";
            go.layer = 8;
        }
        //SetChildrenLayerRecursivly(go.transform, 8);
    }

    public override string ToString()
    {
        return string.Format("UnitName: {0}", Unit.Info.UnitName);
    }


}
