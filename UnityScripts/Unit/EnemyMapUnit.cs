using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using System.Collections.Generic;

public class EnemyMapUnit : MonoBehaviour
{

    public SateliteController SateliteController;
    public float ZoomFactor;
    public Enemy EnemyUnit;
    public Vector3 TempPosition = Vector3.zero;
    public string FriendOfFoe;

    private float _InitialScale;
    private int _LastButtonDown = -1;
    private bool _ContextMenuShowing = false;
    private bool _AttackMenuShowing = false;
    private bool _EngageNotClose = false;
    private bool _GroupAttack = false;

    private bool _MouseOver = false;
    private bool _MouseDown = false;
    private bool _ComboBoxOpen = false;
    private Vector2 _ComboboxScroll = Vector2.zero;
    private WeaponInfo _SelectedWeapon;

    private Texture2D EnemyTex;
    private Texture2D EnemyTexOver;
    private Texture2D EnemyTexDown;

    private Texture2D UnderterminedTex;
    private Texture2D UnderterminedTexOver;
    private Texture2D UnderterminedTexDown;

    private Texture2D FriendTex;
    private Texture2D FriendTexOver;
    private Texture2D FriendTexDown;

    private Texture2D _Normal;
    private Texture2D _Over;
    private Texture2D _Down;

    void Start()
    {
        SateliteController = GameManager.Instance.CameraManager.SateliteCamera.GetComponent<SateliteController>();
        _InitialScale = transform.localScale.x;

        EnemyTex = GameManager.Instance.GUIManager.EnemyDetectionTex;
        EnemyTexOver = GameManager.Instance.GUIManager.EnemyDetectionTexMouseOver;
        EnemyTexDown = GameManager.Instance.GUIManager.EnemyDetectionTexMouseDown;

        UnderterminedTex = GameManager.Instance.GUIManager.UndeterminedTex;
        UnderterminedTexOver = GameManager.Instance.GUIManager.UndeterminedTexOver;
        UnderterminedTexDown = GameManager.Instance.GUIManager.UndeterminedTexDown;

        FriendTex = GameManager.Instance.GUIManager.FriendTex;
        FriendTexOver = GameManager.Instance.GUIManager.FriendTexOver;
        FriendTexDown = GameManager.Instance.GUIManager.FriendTexDown;

        UpdateTextures();
    }

    public void UpdateTextures()
    {
        switch (EnemyUnit.Info.FriendOrFoeClassification)
        {
            case TTG.NavalWar.NWComms.GameConstants.FriendOrFoe.Foe:
                _Normal = EnemyTex;
                _Over = EnemyTexOver;
                _Down = EnemyTexDown;
                break;
            case TTG.NavalWar.NWComms.GameConstants.FriendOrFoe.Friend:
                _Normal = FriendTex;
                _Over = FriendTexOver;
                _Down = FriendTexDown;
                break;
            case TTG.NavalWar.NWComms.GameConstants.FriendOrFoe.Undetermined:
                _Normal = UnderterminedTex;
                _Over = UnderterminedTexOver;
                _Down = UnderterminedTexDown;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        //Move it move it

        transform.position = EnemyUnit.MapPosition;

        FriendOfFoe = EnemyUnit.Info.FriendOrFoeClassification.ToString();
        //Check for collision
        //CheckForCollisionAndUpdatePos();


        float max = SateliteController.MaxMove.y;
        float min = SateliteController.MinMove.y;
        float pos = SateliteController.transform.position.y;

        ZoomFactor = 1 - ((max - pos) / (max - min));
        ZoomFactor = Mathf.Clamp(ZoomFactor, 0.01f, 1);
        transform.localScale = new Vector3(_InitialScale * ZoomFactor, (_InitialScale / 5) * ZoomFactor, _InitialScale * ZoomFactor);

        if (_MouseOver)
        {
            if (_MouseDown)
            {
                if (renderer.material.mainTexture != _Down)
                {
                    renderer.material.mainTexture = _Down;
                }
            }
            else
            {
                if (renderer.material.mainTexture != _Over)
                {
                    renderer.material.mainTexture = _Over;
                }
            }
        }
        else
        {
            if (renderer.material.mainTexture != _Normal)
            {
                renderer.material.mainTexture = _Normal;
            }
        }


        if (Input.GetMouseButtonUp(1) && _MouseOver)
        {
            if (GameManager.Instance.GUIManager.InteractionWindow.AwaitingOrder)
            {
                GameManager.Instance.GUIManager.InteractionWindow.LaunchAttack(this.EnemyUnit);
            }
            else
            {
                _ContextMenuShowing = !_ContextMenuShowing;
                transform.parent.BroadcastMessage("ResetGUI", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void CheckForCollisionAndUpdatePos()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, this.transform.localScale.x / 2);
        TempPosition = Vector3.zero;

        if (cols.Length > 1)
        {
            foreach (Collider col in cols)
            {
                Debug.Log(col.gameObject.name);
                if (col.gameObject.name == "Map" || col.gameObject == this.gameObject)
                {
                    continue;
                }
                //try to evade on x for now
                float xDiff = transform.position.x - col.transform.position.x;
                TempPosition = transform.position + new Vector3(xDiff, 0, 0);
                break;
            }
        }


    }

    public void ResetGUI(EnemyMapUnit sender)
    {
        if (this != sender)
        {
            _AttackMenuShowing = false;
            _ContextMenuShowing = false;
        }
    }

    void OnGUI()
    {
        _LastButtonDown = Event.current.button;
        //Debug.Log(_LastButtonDown);
        if (_ContextMenuShowing)
        {
            Vector3 guiScreenPos = SateliteController.camera.WorldToScreenPoint(transform.position);

            Rect contectMenuRect = new Rect(guiScreenPos.x, SateliteController.camera.pixelHeight - guiScreenPos.y, 200, 100);

            GUI.Box(contectMenuRect, "Context Menu");
            contectMenuRect.y += 10;
            contectMenuRect.height -= 10;

            GUILayout.BeginArea(contectMenuRect);
            if (EnemyUnit.Info.FriendOrFoeClassification == TTG.NavalWar.NWComms.GameConstants.FriendOrFoe.Foe)
            {
                if (GUILayout.Button("Attack"))
                {
                    _AttackMenuShowing = !_AttackMenuShowing;

                }
            }
            else
            {
                if (GUILayout.Button("Change friendOrFoe designation to Foe"))
                {
                    GameManager.Instance.OrderManager.ChangeFriendOrFoeDesignation(EnemyUnit.Info, GameConstants.FriendOrFoe.Foe);
                }
            }
            if (GUILayout.Button("Move here"))
            {
                GameManager.Instance.InputManager.MoveToPoint(guiScreenPos);
                _ContextMenuShowing = false;
            }
            GUILayout.EndArea();

            if (_AttackMenuShowing)
            {
                Rect attackMenu = new Rect(contectMenuRect.xMax, contectMenuRect.y, contectMenuRect.width, 200);

                GUI.Box(attackMenu, "");
                //Debug.Log(attackMenu);
                GUILayout.BeginArea(attackMenu);
                _SelectedWeapon = GameManager.Instance.GUIManager.ComboBox<WeaponInfo>(_SelectedWeapon, ref _ComboBoxOpen, ref _ComboboxScroll, GameManager.Instance.UnitManager.SelectedUnit.Info.Weapons, attackMenu, true, "Let shipcaptain decide");
                if (!_ComboBoxOpen)
                {
                   


                    GUILayout.Space(25);



                    GUILayout.BeginHorizontal();


                    _EngageNotClose = GUILayout.Toggle(_EngageNotClose, new GUIContent("EngageNotClose", _EngageNotClose ? "Unselect for CloseAndEngage" : "Select for EngageNotClose"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    _GroupAttack = GUILayout.Toggle(_GroupAttack, new GUIContent("Group Attacks", "Choose if single unit should attack or whole group"));
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button(new GUIContent("Default attack", "Attack with default strength")))
                    {
                        GameManager.Instance.OrderManager.Attack(GameManager.Instance.UnitManager.SelectedUnit.Info.Id, EnemyUnit.Info.Id, _EngageNotClose ? GameConstants.EngagementOrderType.EngageNotClose : GameConstants.EngagementOrderType.CloseAndEngage,
                                                                    _SelectedWeapon, GameConstants.EngagementStrength.DefaultAttack, _GroupAttack);
                        transform.parent.BroadcastMessage("ResetGUI", null, SendMessageOptions.DontRequireReceiver);
                        ResetGUI(null);

                    }

                    if (GUILayout.Button(new GUIContent("Minimal attack", "Attack with minimal strength")))
                    {
                        GameManager.Instance.OrderManager.Attack(GameManager.Instance.UnitManager.SelectedUnit.Info.Id, EnemyUnit.Info.Id, _EngageNotClose ? GameConstants.EngagementOrderType.EngageNotClose : GameConstants.EngagementOrderType.CloseAndEngage,
                                                                    _SelectedWeapon, GameConstants.EngagementStrength.MinimalAttack, _GroupAttack);
                        ResetGUI(null);
                        
                        
                    }

                    if (GUILayout.Button(new GUIContent("Strong attack", "Attack with overpowering amount strength(might deplete you munitions)")))
                    {
                        GameManager.Instance.OrderManager.Attack(GameManager.Instance.UnitManager.SelectedUnit.Info.Id, EnemyUnit.Info.Id, _EngageNotClose ? GameConstants.EngagementOrderType.EngageNotClose : GameConstants.EngagementOrderType.CloseAndEngage,
                                                                    _SelectedWeapon, GameConstants.EngagementStrength.OverkillAttack, _GroupAttack);
                        ResetGUI(null);
                    }
                    GameManager.Instance.GUIManager.ShowToolTip(GUI.tooltip);

                   
                }
                GUILayout.EndArea();

                GameManager.Instance.InputManager.IsMouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, attackMenu);
            }
        }
        if (_MouseOver)
        {
            GUI.tooltip = EnemyUnit.Info.IsIdentified == true ? EnemyUnit.Info.RefersToUnitName : string.Format("Unknown {0}", EnemyUnit.Info.DetectionClassification.ToString());
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

    void OnCollisionStay(Collision col)
    {
        Debug.Log("Collision");
    }

    void OnMouseUp()
    {
        //Debug.Log(_LastButtonDown);
        _MouseDown = false;
        if (EnemyUnit != null)
        {
            GameManager.Instance.UnitManager.SelectedEnemyUnit = EnemyUnit;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1 * (this.transform.localScale.x / 2));
    }


}
