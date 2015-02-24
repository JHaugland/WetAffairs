using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using System;
using System.Text;

public class GUIManager : MonoBehaviour
{

    #region Private variables

    private List<GUIGroup> _PlayerGroups;
    private List<GUIGroup> _EnemyGroups;

    private float _FormationFactorX;
    private float _FormationFactorY;


    #endregion

    #region GUIStyles

    public GUIStyle BattleMessageStyle;
    public GUIStyle BattleMessageStyleRead;
    public GUIStyle SystemMessageStyle;
    public GUIStyle SystemMessageStyleRead;
    public GUIStyle DetectionMessageStyle;
    public GUIStyle DetectionMessageStyleRead;
    public GUIStyle GameMessageStyle;
    public GUIStyle GameMessageStyleRead;
    public GUIStyle ChatMessageStyle;
    public GUIStyle ChatMessageStyleRead;


    #endregion

    #region Public variables

    public int HorizontalDocks = 2;
    public int VerticalDocks = 2;

    private float HorizontalDockWidth; //set by screensize
    public float HorizontalDockHeight = 100;

    public float VerticalDockWidth = 100;
    private float VerticalDockHeight;//set by screensize

    public float PopupWindowTimer = 5.0f;
    public float PopupFadeTimer = 1.0f;
    public Vector2 FormationRectSize = new Vector2(200, 200);
    public float FormationZoomLevelInM = 10;


    public List<Dock> DockingStations;
    public bool ShowDocks = false;
    public List<Rect> UsedGUIRects;

    public string LastTooltip;

    //~ public Rect IntersectingRect = new Rect(0,0,0,0);

    public GUIStyle DockStyle;
    public GUIStyle DockClosestStyle;

    public GUIStyle _DockStyleToggleOn;
    public GUIStyle _DockStyleToggleOff;

    public GUIStyle _MinimizeStyle;

    public GUIStyle _ResizeControl;

    public GUIStyle _PopupWindowStyle;
    public GUIStyle GroupedGUIUnitStyle;
    public GUIStyle GroupedGUIMainUnitStyle;

    public GUIStyle TooltipStyle;

    public GUIStyle ArchiveButtonStyle1;
    public GUIStyle ArchiveButtonStyle2;
    public GUIStyle DeleteButtonStyle1;
    public GUIStyle DeleteButtonStyle2;



    public InteractionWindow InteractionWindow;

    //TEXTURES
    public Texture2D LogoTexture;
    public Texture2D _MinimizeButtonTexture;
    public Texture2D _DestinationTexture;
    public Texture2D _WaypointTexture;
    public Texture2D _DotTexture;

    public Texture2D BattleCruiserTex;
    public Texture2D CarrierTex;
    public Texture2D PatrolBoatTex;
    public Texture2D CivilianVesselTex;
    public Texture2D SubmarineTex;
    public Texture2D HelicopterTex;
    public Texture2D FighterTex;
    public Texture2D FrigateTex;
    public Texture2D BomberTex;
    public Texture2D TransportECWTex;
    public Texture2D GroupTex;
    public Texture2D GroupTexMouseOver;
    public Texture2D GroupTexMouseDown;
    public Texture2D GroupTexSelectedUnit;
    public Texture2D EnemyDetectionTex;
    public Texture2D EnemyDetectionTexMouseOver;
    public Texture2D EnemyDetectionTexMouseDown;

    public Texture2D UndeterminedTex;
    public Texture2D UndeterminedTexOver;
    public Texture2D UndeterminedTexDown;

    public Texture2D FriendTex;
    public Texture2D FriendTexOver;
    public Texture2D FriendTexDown;

    public Texture2D MissileTex;
    public Texture2D AirportTexture;

    public Texture2D SelectedUnitTex;

    public Texture2D CompassTexture;

    public Texture2D DetectionTexture;
    public Texture2D RadarTexture;
    public Texture2D RadarLine;
    public Texture2D AltimeterIcon;
    public Texture2D PilotIcon;
    public Texture2D ArchiveIcon;
    public Texture2D DeleteIcon;


    public Texture2D RainDrizzle;
    public Texture2D RainIntermediate;
    public Texture2D RainHeavy;
    public Texture2D SnowDrizzle;
    public Texture2D SnowIntermediate;
    public Texture2D SnowHeavy;
    public Texture2D Hail;
    public Texture2D WindFlag;
    public Texture2D Wave;
    public Texture2D Clouds;
    public Texture2D Thermometer;
    public Texture2D MessageBackground1;
    public Texture2D MessageBackground2;
    public Texture2D ArrowDown;
    public GUIStyle ComboBoxStyle;


    public bool IsWindowDragging = false;

    public Rect _MinimizeButtonRect = new Rect(0, 20, 30, 30);
    public Rect _DockToggleRect = new Rect(30, 20, 30, 30);
    public Rect TooltipRect = new Rect(0, 0, 0, 0);


    public float _LineGap = 5;
    public float _LineLength = 5;




    public bool IsPopupPositionRandom = false;

    public Rect RandomPopupRect;

    public Rect _PopupWindowPosition = new Rect(300, 300, 150, 40);
    #endregion

    #region Script Variables


    #endregion

    #region Public Properties

    public Rect MinimizeButtonRect
    {
        get
        {
            return _MinimizeButtonRect;
        }
    }

    public Rect DockToggleRect
    {
        get
        {
            return _DockToggleRect;
        }
    }

    public Rect PopupWindowPosition
    {
        get
        {
            if (IsPopupPositionRandom)
            {
                return new Rect(UnityEngine.Random.Range(RandomPopupRect.xMin, RandomPopupRect.xMax), UnityEngine.Random.Range(RandomPopupRect.yMin, RandomPopupRect.yMax), _PopupWindowPosition.width, _PopupWindowPosition.height);
            }
            else
            {
                return _PopupWindowPosition;
            }
        }
    }

    public GUIStyle DockStyleToggleOn
    {
        get
        {
            return _DockStyleToggleOn;
        }
    }

    public GUIStyle DockStyleToggleOff
    {
        get
        {
            return _DockStyleToggleOff;
        }
    }

    public GUIStyle ResizeControl
    {
        get
        {
            return _ResizeControl;
        }
    }

    public GUIStyle MinimizeStyle
    {
        get
        {
            return _MinimizeStyle;
        }
    }

    public GUIStyle PopupWindowStyle
    {
        get
        {
            return _PopupWindowStyle;
        }
    }

    public Texture2D MinimizeButtonTexture
    {
        get
        {
            return _MinimizeButtonTexture;
        }
    }

    public Texture2D DestinationTexture
    {
        get
        {
            return _DestinationTexture;
        }
    }

    public Texture2D WaypointTexture
    {
        get
        {
            return _WaypointTexture;
        }
    }

    public Texture2D DotTexture
    {
        get
        {
            return _DotTexture;
        }
    }

    public float LineGap
    {
        get
        {
            return _LineGap;
        }
    }

    public float LineLength
    {
        get
        {
            return _LineLength;
        }
    }

    public float FormationFactorX
    {
        get
        {
            return _FormationFactorX;
        }
    }
    public float FormationFactorY
    {
        get
        {
            return _FormationFactorY;
        }
    }


    #endregion


    #region Unity methods

    void Start()
    {
        if (UsedGUIRects == null)
        {
            UsedGUIRects = new List<Rect>();
        }

        _PlayerGroups = new List<GUIGroup>();
        _EnemyGroups = new List<GUIGroup>();

        //HorizontalDockWidth = Screen.width / HorizontalDocks;
        //VerticalDockHeight = (Screen.height - (HorizontalDockHeight * 2) )/ VerticalDocks;

        //float x = 0;
        //float y = 0;

        //DockingStations = new List<Dock>();

        ////top horizontal
        //for(int i = 0; i < HorizontalDocks ; ++i)
        //{
        //    DockingStations.Add(new Dock(new Rect(x, y, HorizontalDockWidth, HorizontalDockHeight)));
        //    x += HorizontalDockWidth;
        //}
        //x = 0;
        ////bottom horizontal
        //y = Screen.height - HorizontalDockHeight;
        //for(int i = 0; i < HorizontalDocks ; ++i)
        //{
        //    DockingStations.Add(new Dock(new Rect(x, y, HorizontalDockWidth, HorizontalDockHeight)));
        //    x += HorizontalDockWidth;
        //}

        //x = 0;
        //y = HorizontalDockHeight;
        ////top horizontal
        //for(int i = 0; i < VerticalDocks ; ++i)
        //{
        //    DockingStations.Add(new Dock(new Rect(x, y, VerticalDockWidth, VerticalDockHeight)));
        //    y += VerticalDockHeight;
        //}
        //x = Screen.width - VerticalDockWidth;
        //y = HorizontalDockHeight;
        ////bottom horizontal

        //for(int i = 0; i < VerticalDocks ; ++i)
        //{
        //    DockingStations.Add(new Dock(new Rect(x, y, VerticalDockWidth, VerticalDockHeight)));
        //    y += VerticalDockHeight;
        //}



    }


    public Rect CreateButton(Rect rect)
    {


        foreach (Rect r in UsedGUIRects)
        {
            if (r == rect)
            {
                continue;
            }
            Rect intersect = FindIntersectingRect(r, rect);
            if (intersect != null)
            {
                //Rect r = UsedGUIRects[i];
                //rect = new Rect(r.x, r.y + r.height + 2, r.width, r.height);
                rect.x += rect.xMax < r.xMax ? -intersect.width : intersect.width;
                rect.y += rect.yMax < r.yMax ? -intersect.height : intersect.height;


            }
            //if (r == rect)
            //{
            //    continue;
            //}
            //Rect intersectingRect = FindIntersectingRect(r, rect);


        }

        UsedGUIRects.Add(rect);

        return rect;
    }

    void Update()
    {
        #region Formation Stuff
        PlayerUnit selectedUnit = GameManager.Instance.UnitManager.SelectedUnit;

        if (selectedUnit != null)
        {


            //float longestX = 0;
            //float longestY = 0;

            //foreach (PlayerUnit p in selectedUnit.UnitsInGroup)
            //{
            //    if (p.FormationPosition == null)
            //    {
            //        continue;
            //    }
            //    if (p.FormationPosition.PositionOffset.RightM > longestX)
            //    {
            //        longestX = (float)p.FormationPosition.PositionOffset.RightM;
            //    }
            //    if (p.FormationPosition.PositionOffset.ForwardM > longestY)
            //    {
            //        longestY = (float)p.FormationPosition.PositionOffset.ForwardM;
            //    }
            //}

            //Then calculate the factor which all will be multiplied with
            //TODO: Fix hack on static width and height
            //Change 0 with margin from edges
            _FormationFactorX = FormationZoomLevelInM / FormationRectSize.x;
            _FormationFactorY = FormationZoomLevelInM / FormationRectSize.y;
        }

        #endregion

        #region Units Obsulete
        //#region PlayerGroups

        //try
        //{
        //    _PlayerGroups.RemoveAll(delegate(GUIGroup g) { return g.Units.Count < 1; });
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("Error in OnGUI Removeall _GUIGroups. Message: " + ex.Message);
        //}

        //List<PlayerUnit> AllUnits = GameManager.Instance.UnitManager.GameUnits.FindAll(delegate(PlayerUnit p) { return p.Info.IsGroupMainUnit || string.IsNullOrEmpty(p.Info.GroupId); }); ;
        //if (AllUnits != null)
        //{
        //    //Debug.Log(_GUIGroups.Count);
        //    for (int i = 0; i < AllUnits.Count; i++)
        //    {
        //        for (int j = 0; j < AllUnits.Count; j++)
        //        {
        //            PlayerUnit p1 = AllUnits[i];
        //            PlayerUnit p2 = AllUnits[j];


        //            if (p1 == p2)
        //            {
        //                continue;
        //            }

        //            //First check if PlayerUnit is part of GUIGroup. If not add new GUIGroup
        //            GUIGroup g1 = UnitExistsInPlayerGroup(p1);
        //            GUIGroup g2 = UnitExistsInPlayerGroup(p2);

        //            //GUIGroup g1 = p1.MyGUIGroup;
        //            //GUIGroup g2 = p2.MyGUIGroup;

        //            if (g1 != null && g2 != null)
        //            {
        //                if (string.Equals(g1.Name, g2.Name))
        //                {
        //                    if (IntersectRect(p1.MyRect, p2.MyRect))
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //            //if (g1 == null && p1.MyGUIGroup != null)
        //            //{
        //            //    g1 = p1.MyGUIGroup;
        //            //}

        //            if (g1 == null)
        //            {
        //                g1 = new GUIGroup();
        //                g1.AddUnit(p1);
        //                _PlayerGroups.Add(g1);

        //            }

        //            //then check if they intersect
        //            if (g2 != null)
        //            {
        //                if (IntersectRect(p1.MyRect, p2.MyRect) && !string.Equals(g1.Name, g2.Name))
        //                {
        //                    //Remove both from list
        //                    _PlayerGroups.Remove(g1);
        //                    _PlayerGroups.Remove(g2);

        //                    //Fuse them together
        //                    g1 += g2;
        //                    p1.MyGUIGroup = g1;
        //                    p2.MyGUIGroup = g1;
        //                    _PlayerGroups.Add(g1);
        //                    //g1.GUIRect = FindIntersectingRect(g1.GUIRect, g2.GUIRect);

        //                }
        //                else if (!IntersectRect(p1.MyRect, p2.MyRect) && string.Equals(g1.Name, g2.Name))
        //                {
        //                    //Dissect
        //                    _PlayerGroups.Remove(g1);
        //                    _PlayerGroups.Remove(g2);
        //                    g1 = new GUIGroup();
        //                    g1 = g2.RemoveUnit(p1);
        //                    _PlayerGroups.Add(g1);
        //                    _PlayerGroups.Add(g2);
        //                }
        //            }
        //        }
        //    }
        //}
        //foreach (GUIGroup g in _PlayerGroups)
        //{
        //    g.Update();
        //}

        //#endregion

        //#region Enemy Groups
        //try
        //{
        //    _EnemyGroups.RemoveAll(delegate(GUIGroup g) { return g.EnemyUnits.Count == 0; });
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("Error in OnGUI Removeall _GUIGroups. Message: " + ex.Message);
        //}

        //List<Enemy> EnemyUnits = GameManager.Instance.UnitManager.EnemyDetections;
        //if (EnemyUnits != null)
        //{
        //    //Debug.Log(_GUIGroups.Count);
        //    for (int i = 0; i < EnemyUnits.Count; i++)
        //    {
        //        for (int j = 0; j < EnemyUnits.Count; j++)
        //        {
        //            Enemy e1 = EnemyUnits[i];
        //            Enemy e2 = EnemyUnits[j];

        //            GUIGroup g1 = UnitExistsInPlayerGroup(e1);

        //            if (e1 == e2)
        //            {
        //                if (g1 == null)
        //                {
        //                    g1 = new GUIGroup(true);
        //                    g1.AddUnit(e1);
        //                    _EnemyGroups.Add(g1);
        //                }
        //                continue;
        //            }


        //            //First check if PlayerUnit is part of GUIGroup. If not add new GUIGroup

        //            GUIGroup g2 = UnitExistsInPlayerGroup(e2);

        //            if (g1 != null && g2 != null)
        //            {
        //                if (string.Equals(g1.Name, g2.Name))
        //                {
        //                    if (IntersectRect(e1.GUIPos, e2.GUIPos))
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }

        //            if (g1 == null)
        //            {
        //                g1 = new GUIGroup(true);
        //                g1.AddUnit(e1);
        //                _EnemyGroups.Add(g1);

        //            }

        //            //then check if they intersect
        //            if (g2 != null)
        //            {
        //                if (IntersectRect(e1.GUIPos, e2.GUIPos) && !string.Equals(g1.Name, g2.Name))
        //                {
        //                    if (e1.Info.DomainType == GameConstants.DomainType.Land || e2.Info.DomainType == GameConstants.DomainType.Land)
        //                    {
        //                        continue;
        //                    }
        //                    //Remove both from list
        //                    _EnemyGroups.Remove(g1);
        //                    _EnemyGroups.Remove(g2);

        //                    //Fuse them together
        //                    g1 += g2;

        //                    _EnemyGroups.Add(g1);

        //                    //g1.GUIRect = FindIntersectingRect(g1.GUIRect, g2.GUIRect);

        //                }
        //                else if (!IntersectRect(e1.GUIPos, e2.GUIPos) && string.Equals(g1.Name, g2.Name))
        //                {
        //                    //Dissect
        //                    _EnemyGroups.Remove(g1);
        //                    _EnemyGroups.Remove(g2);
        //                    g1 = new GUIGroup(true);
        //                    g1 = g2.RemoveUnit(e1);
        //                    _EnemyGroups.Add(g1);
        //                    _EnemyGroups.Add(g2);
        //                }
        //            }
        //        }
        //    }
        //}
        //foreach (GUIGroup g in _EnemyGroups)
        //{

        //    g.IsEnemy = true;
        //}
        //#endregion
        #endregion
    }

    void OnGUI()
    {
        //Debug.Log("Elements :" + UsedGUIRects.Count);

        Event currentEvent = Event.current;

        #region Logo

        //GUI.DrawTexture(InteractionWindow.WindowRect, LogoTexture);

        #endregion

        #region Compass





        //GUIUtility.RotateAroundPivot(GameManager.Instance.CameraManager.MainCamera.transform.eulerAngles.y, new Vector2(100, Screen.height - 100));
        //GUI.DrawTexture(new Rect(0, Screen.height - 200, 200, 200), GameManager.Instance.GUIManager.CompassTexture);


        #endregion


        #region Units

        //#region Enemy Groups



        //foreach (GUIGroup g in _EnemyGroups)
        //{
        //    continue;
        //    //if (GUI.Button(g.GUIRect, GetTextureByUnitClass(GameManager.UnitClasses.Group)))
        //    //{
        //    Rect enemyRect = g.GUIRect;


        //    if (g.Visible)
        //    {
        //        GUI.depth = 500;
        //        //if (GUI.Button(enemyRect, GetTextureByUnitClass(GameManager.UnitClasses.EnemyDetection)))
        //        //{
        //        //    g.Expanded = !g.Expanded;
        //        //}
        //        GUI.Label(enemyRect, GetTextureByUnitClass(GameManager.UnitClasses.EnemyDetection));


        //        if (g.Expanded)
        //        {
        //            Rect group = new Rect(enemyRect.xMax, enemyRect.y, 200, g.EnemyUnits.Count * 40);
        //            GUI.contentColor = Color.white;
        //            GUI.Box(group, "");

        //            GameManager.Instance.InputManager.IsMouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, group);

        //            GUILayout.BeginArea(group);
        //            g.ScrollPosition = GUILayout.BeginScrollView(g.ScrollPosition);
        //            GUILayout.BeginVertical();

        //            foreach (Enemy e in g.EnemyUnits)
        //            {
        //                GUILayout.BeginHorizontal();
        //                if (GUILayout.Button(GetTextureByUnitClass(GameManager.UnitClasses.EnemyDetection), GUILayout.MaxWidth(e.Size.x), GUILayout.MaxHeight(e.Size.y)))
        //                {
        //                    GameManager.Instance.UnitManager.SelectedEnemyUnit = e;
        //                    g.OptionsMenuExpanded = !g.OptionsMenuExpanded;
        //                }
        //                GUILayout.Label(e.Info.RefersToUnitName != "" ? e.Info.RefersToUnitName : "Unknown unit");
        //                GUILayout.EndHorizontal();
        //            }

        //            GUILayout.EndVertical();
        //            GUILayout.EndScrollView();
        //            GUILayout.EndArea();

        //            if (g.OptionsMenuExpanded)
        //            {
        //                Rect options = group;
        //                options.x = group.xMax;

        //                GUI.Box(options, "Options");
        //                options.y += 20;
        //                options.height -= 20;
        //                GUILayout.BeginArea(options);

        //                //GUILayout.Label("Put whatever is needed in here");
        //                if (GUILayout.Button("Attack!"))
        //                {
        //                    GameManager.Instance.OrderManager.Attack(GameManager.Instance.UnitManager.SelectedUnit.Info, GameManager.Instance.UnitManager.SelectedEnemyUnit.Info, GameConstants.EngagementOrderType.CloseAndEngage, "", 1);
        //                }

        //                GUILayout.EndArea();
        //            }
        //        }
        //    }
        //}




        //#endregion
        ////First find all empty groups

        //#region PlayerGroups

        //foreach (GUIGroup g in _PlayerGroups)
        //{
        //    continue;
        //    if (g.Units.Count == 0)
        //    {
        //        continue;
        //    }
        //    //if (GUI.Button(g.GUIRect, GetTextureByUnitClass(GameManager.UnitClasses.Group)))
        //    //{

        //    if (g.Visible)
        //    {
        //        GameManager.Instance.InputManager.IsMouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, g.GUIRect);
        //        //GUI.depth = -1000;

        //        if (GUI.Button(g.GUIRect, new GUIContent(GetTextureByUnitClass(GameManager.UnitClasses.Group), g.ToString())))
        //        {
        //            if (currentEvent.button == 0)
        //            {
        //                if (g.GroupMainUnits.Count > 1)
        //                {
        //                    g.Expanded = !g.Expanded;

        //                }
        //                else
        //                {
        //                    GameManager.Instance.UnitManager.SelectedUnit = g.GroupMainUnits[0];
        //                }
        //            }
        //            else
        //            {
        //                if (g.GroupMainUnits.Count > 1)
        //                {
        //                    if (!g.Expanded)
        //                    {
        //                        g.Expanded = !g.Expanded;
        //                    }
        //                    else
        //                    {
        //                        g.ContextMenuExpanded = !g.ContextMenuExpanded;
        //                    }
        //                }
        //                else
        //                {
        //                    if (GameManager.Instance.UnitManager.SelectedGroupMainUnit != g.GroupMainUnits[0])
        //                    {
        //                        g.ContextMenuExpanded = !g.ContextMenuExpanded;
        //                        Debug.Log("Show Context Menu");

        //                    }
        //                }

        //            }
        //        }


        //        //The below needs polish if it is to work
        //        //if (GUI.tooltip == g.ToString() && Event.current.type == EventType.Repaint)
        //        //{
        //        //    g.Expanded = true;
        //        //}
        //        //else
        //        //{
        //        //    g.Expanded = false;
        //        //}
        //        if (g.ContextMenuExpanded)
        //        {
        //            Rect contectMenuRect = new Rect(g.GUIRect.x, g.GUIRect.yMax, 200, 100);

        //            GUI.Box(contectMenuRect, "Context Menu");
        //            contectMenuRect.y += 10;
        //            contectMenuRect.height -= 10;

        //            GUILayout.BeginArea(contectMenuRect);
        //            if (GUILayout.Button("Join group"))
        //            {
        //                GameManager.Instance.OrderManager.JoinGroups(GameManager.Instance.UnitManager.SelectedGroupMainUnit, g.GroupMainUnits[0]);
        //            }
        //            if (GUILayout.Button("Move here"))
        //            {
        //                Debug.Log("Move here");
        //            }
        //            GUILayout.EndArea();
        //        }


        //        if (g.Expanded)
        //        {
        //            Rect group = new Rect(g.GUIRect.xMax, g.GUIRect.y, 200, g.Units.Count * 40);
        //            //GUI.contentColor = Color.white;
        //            GUI.Box(group, "");

        //            GameManager.Instance.InputManager.IsMouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, group);

        //            GUILayout.BeginArea(group);
        //            g.ScrollPosition = GUILayout.BeginScrollView(g.ScrollPosition);
        //            GUILayout.BeginVertical();

        //            foreach (PlayerUnit p in g.Units)
        //            {
        //                if (p.Info.IsGroupMainUnit)
        //                {
        //                    GUILayout.BeginHorizontal();
        //                    if (GUILayout.Button(new GUIContent(GetTextureByUnitClass(GameManager.UnitClasses.Group), p.Info.UnitName), GUILayout.MaxWidth(p.Size.x), GUILayout.MaxHeight(p.Size.y)))
        //                    {
        //                        GameManager.Instance.UnitManager.SelectedUnit = p;
        //                    }
        //                    GUILayout.Label(p.Info.UnitName, p.Info.IsGroupMainUnit == true ? GroupedGUIMainUnitStyle : GroupedGUIUnitStyle);
        //                    GUILayout.EndHorizontal();
        //                }
        //            }

        //            GUILayout.EndVertical();
        //            GUILayout.EndScrollView();
        //            GUILayout.EndArea();
        //        }
        //    }
        //}

        //#endregion




        #endregion
        ShowToolTip(GUI.tooltip);

        //GUI.tooltip = "Test";
        #region Docks
        //if (DockingStations != null)
        //{
        //    if (DockingStations.Count > 0)
        //    {
        //        Event e = Event.current;
        //        int id = GUIUtility.hotControl;


        //        foreach (Dock dock in DockingStations)
        //        {
        //            dock.Update();
        //            if ((dock.IsIntersecting && id > 0 && IsWindowDragging) || ShowDocks)
        //            {
        //                //~ DockClosestStyle.Draw(IntersectingRect, GUIContent.none, id);
        //                if (dock.IsClosest)
        //                {
        //                    //~ Debug.Log("one");
        //                    DockClosestStyle.Draw(dock.Rect, GUIContent.none, id);
        //                }
        //                else
        //                {
        //                    DockStyle.Draw(dock.Rect, GUIContent.none, id);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

    }


    #endregion

    public void RemoveFromGroup(Enemy enemy)
    {
        GUIGroup g = UnitExistsInPlayerGroup(enemy);

        if (g != null)
        {
            g.RemoveUnit(enemy);
        }
    }

    public void RemoveGroup(GUIGroup guiGroup)
    {
        _EnemyGroups.Remove(guiGroup);
    }

    public void ShowToolTip(string tooltip)
    {
        //GUILayout.BeginArea(TooltipRect);
        LastTooltip = !string.IsNullOrEmpty(tooltip) ? tooltip : LastTooltip;

        GUI.Label(TooltipRect, LastTooltip, TooltipStyle);
        //GUILayout.EndArea();
    }

    public T ComboBox<T>(T selectedItem, ref bool isExpanded, ref Vector2 scrollPosition,  List<T> items, Rect rect, bool nullAllowed, string text)
    {
        string current = string.Empty;

        if (selectedItem != null)
        {
          current = selectedItem.ToString();
        }
        
        
        Rect buttonRect = new Rect(rect.x, rect.y, rect.width, 20);
        
        if (GUILayout.Button(new GUIContent(string.IsNullOrEmpty(current) ? text : current, ArrowDown, string.Format("Currently selected: {0} - Choices: {1}", current, items.Count)), GUILayout.Height(25)))
        {
            if (items.Count > 1)
            {
                isExpanded = !isExpanded;
            }
        }



        if (isExpanded)
        {
            //Rect scrollerRect = new Rect(0, 0, rect.width, items.Count * 30);

            //GUI.Box(scrollerRect, "", ComboBoxStyle);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));

            foreach (T item in items)
            {
                if (GUILayout.Button(item.ToString()))
                {
                    isExpanded = false;
                    return item;
                }
            }
            if (nullAllowed)
            {
                if (GUILayout.Button(text))
                {
                    isExpanded = false;
                    return default(T);
                }
            }

            GUILayout.EndScrollView();
        }
        return selectedItem;
    }



    private GUIGroup UnitExistsInPlayerGroup(PlayerUnit unit)
    {
        GUIGroup group = _PlayerGroups.Find(delegate(GUIGroup g) { return g.Units.Contains(unit); });

        //Debug.Log(string.Format("Finding group containing unit:{0}. Found:{1}", unit.Info.Id, group != null ? group.ToString() : "no group!"));

        return group;
    }

    private GUIGroup UnitExistsInPlayerGroup(Enemy unit)
    {
        GUIGroup group = _EnemyGroups.Find(delegate(GUIGroup g) { return g.EnemyUnits.Contains(unit); });

        //Debug.Log(string.Format("Finding group containing unit:{0}. Found:{1}", unit.Info.Id, group != null ? group.ToString() : "no group!"));

        return group;
    }

    public DockableWindow IsMouseOverGUI(Vector2 mousePos)
    {
        GameObject gui = GameObject.Find("GUI");

        Component[] windows = gui.GetComponents(typeof(DockableWindow));

        if (windows == null)
        {
            //~ Debug.Log(gui);
            return null;
        }

        //~ Debug.Log(windows.Length);
        if (windows.Length > 0)
        {
            Vector2 guiMousePos = new Vector2(mousePos.x, Screen.height - mousePos.y);
            foreach (DockableWindow guiWindow in windows)
            {
                //if(guiWindow is CameraWindow || guiWindow is GameMessageWindow)
                //{
                if (guiWindow.WindowRect.Contains(guiMousePos) && guiWindow.enabled)
                {
                    return guiWindow;
                }

                //}

            }
        }

        return null;
    }



    public bool IntersectRect(Rect r1, Rect r2)
    {
        //return !(r2.xMin > r1.xMin + r1.width
        //    || r2.xMin + r2.width < r1.xMin
        //    || r2.yMin > r1.yMin + r1.height
        //    || r2.yMin + r2.height < r1.yMin
        //    );
        return !(r2.xMin > r1.xMax
            || r2.xMax < r1.xMin
            || r2.yMin > r1.yMax
            || r2.yMax < r1.yMin
            );
    }

    public Rect FindIntersectingRect(Rect r1, Rect r2)
    {
        if (IntersectRect(r1, r2))
        {
            //return new Rect(Mathf.Max(r1.xMin, r2.xMin),
            //             Mathf.Max(r1.yMin, r2.yMin),
            //             Mathf.Min(r1.xMin + r1.width, r2.xMin + r2.width) - Mathf.Max(r1.xMin, r2.xMin),
            //             Mathf.Min(r1.yMin + r1.height, r2.yMin + r2.height) - Mathf.Max(r1.yMin, r2.yMin));
            return new Rect(Mathf.Max(r1.xMin, r2.xMin),
                         Mathf.Max(r1.yMin, r2.yMin),
                         Mathf.Min(r1.xMax, r2.xMax) - Mathf.Max(r1.xMin, r2.xMin),
                         Mathf.Min(r1.yMax, r2.yMax) - Mathf.Max(r1.yMin, r2.yMin));
        }
        else
        {
            return new Rect(0, 0, 0, 0);
        }
    }

    public GUIStyle GetMessageStyleByType(GameManager.MessageTypes messageType, bool isRead, int mode)
    {
        GUIStyle ret;
        switch (messageType)
        {
            case GameManager.MessageTypes.Game:
                ret = isRead == true ? GameMessageStyleRead : GameMessageStyle;
                break;
            case GameManager.MessageTypes.Chat:
                ret = isRead == true ? ChatMessageStyleRead : ChatMessageStyle;
                break;
            case GameManager.MessageTypes.Battle:
                ret = isRead == true ? BattleMessageStyleRead : BattleMessageStyle;
                break;
            case GameManager.MessageTypes.Detection:
                ret = isRead == true ? DetectionMessageStyleRead : DetectionMessageStyle;
                break;
            default:
                ret = SystemMessageStyleRead;
                break;
        }

        if (mode != -1)
        {
            GUIStyleState gss = ret.normal;

            gss.background = mode == 0 ? MessageBackground1 : MessageBackground2;
            ret.normal = gss;
            ret.imagePosition = ImagePosition.TextOnly;
            ret.alignment = TextAnchor.MiddleLeft;
        }

        return ret;

    }

    public GUIStyle GetArchiveStyleByType(bool deleteButton, int mode)
    {
        if (deleteButton)
        {
            return mode == 0 ? DeleteButtonStyle1 : DeleteButtonStyle2;
        }
        return mode == 0 ? ArchiveButtonStyle1 : ArchiveButtonStyle2;
    }



    public Texture2D GetTextureByUnitClassId(string UnitClassId)
    {
        switch (UnitClassId)
        {
            case "arleighburke":
                return GetTextureByUnitClass(GameManager.UnitClasses.BattleCruiser);

            case "sh60b":
                return GetTextureByUnitClass(GameManager.UnitClasses.Heilcopter);

            case "asumissile":
                return GetTextureByUnitClass(GameManager.UnitClasses.TransportECM);

            case "f22":
                return GetTextureByUnitClass(GameManager.UnitClasses.Fighter);
            case "f35c":
                return GetTextureByUnitClass(GameManager.UnitClasses.Fighter);
            case "group":
                return GetTextureByUnitClass(GameManager.UnitClasses.Group);
            case "nimitz":
                return GetTextureByUnitClass(GameManager.UnitClasses.Carrier);
            case "fridtjofnansen":
                return GetTextureByUnitClass(GameManager.UnitClasses.Frigate);
            case "enemyDetection":
                return GetTextureByUnitClass(GameManager.UnitClasses.EnemyDetection);
            case "detection":
                return DetectionTexture;
            case "ukairportlarge":
                return GetTextureByUnitClass(GameManager.UnitClasses.Airport);
            default:
                return GetTextureByUnitClass(GameManager.UnitClasses.None);
        }
    }


    public Texture2D GetTextureByUnitClass(GameManager.UnitClasses unitClass)
    {
        switch (unitClass)
        {
            case GameManager.UnitClasses.BattleCruiser:
                {
                    return BattleCruiserTex;

                }

            case GameManager.UnitClasses.Carrier:
                {
                    return CarrierTex;

                }

            case GameManager.UnitClasses.PatrolBoat:
                {
                    return PatrolBoatTex;

                }

            case GameManager.UnitClasses.Submarine:
                {
                    return SubmarineTex;

                }

            case GameManager.UnitClasses.CivillianVessel:
                {
                    return CivilianVesselTex;

                }

            case GameManager.UnitClasses.Heilcopter:
                {
                    return HelicopterTex;

                }

            case GameManager.UnitClasses.Fighter:
                {
                    return FighterTex;

                }
            case GameManager.UnitClasses.Frigate:
                {
                    return FrigateTex;
                }

            case GameManager.UnitClasses.Bomber:
                {
                    return BomberTex;
                }

            case GameManager.UnitClasses.TransportECM:
                {
                    return TransportECWTex;

                }

            case GameManager.UnitClasses.None:
                {
                    return null;

                }

            case GameManager.UnitClasses.Group:
                {
                    return GroupTex;
                }

            case GameManager.UnitClasses.EnemyDetection:
                {
                    return EnemyDetectionTex;
                }
            case GameManager.UnitClasses.Airport:
                return AirportTexture;
        }
        return null;
    }

    //public Texture2D GetPrecipitationSymbol(WeatherSystemInfo weatherInfo)
    //{
    //}




}

public class GUIGroup
{
    public List<PlayerUnit> Units;
    public List<Enemy> EnemyUnits;
    public bool Expanded = false;
    public bool OptionsMenuExpanded = false;
    public bool ContextMenuExpanded = false;
    public Vector2 ScrollPosition = Vector2.zero;
    public bool IsEnemy = false;


    private int _UpdateInterval = 100;
    private int _CurrentInt = 100;

    //public Rect GUIRect;

    public GUIGroup()
    {
        Units = new List<PlayerUnit>();
        EnemyUnits = new List<Enemy>();
    }

    public GUIGroup(bool isEnemy)
    {
        Units = new List<PlayerUnit>();
        EnemyUnits = new List<Enemy>();
        IsEnemy = isEnemy;
    }

    public bool Visible
    {
        get
        {
            if (!IsEnemy)
            {
                return Units[0].GUIVisible;
            }
            return EnemyUnits[0].GUIVisible;
        }
    }

    public List<PlayerUnit> GroupMainUnits
    {
        get
        {
            return Units.FindAll(delegate(PlayerUnit p) { return p.Info.IsGroupMainUnit; });
        }
    }


    public Rect GUIRect
    {
        get
        {
            if (!IsEnemy)
            {
                if (Units.Count > 0)
                {
                    return Units[0].MyRect;
                }
            }
            else
            {
                if (EnemyUnits.Count > 0)
                {
                    return EnemyUnits[0].GUIPos;
                }
            }
            return new Rect(0, 0, 100, 100);
        }
    }

    public string Name
    {
        get
        {
            if (!IsEnemy)
            {
                if (Units.Count > 0)
                {
                    return Units[0].Info.Id;
                }
            }
            else
            {
                if (EnemyUnits.Count > 0)
                {
                    if (!string.IsNullOrEmpty(EnemyUnits[0].Info.Id))
                    {
                        return EnemyUnits[0].Info.Id;
                    }
                    else
                    {
                        return "Unidentified";
                    }
                }
            }

            return "noname";
        }
    }

    public void AddUnit(PlayerUnit p)
    {
        //Add MainUnit
        Units.Add(p);
        p.MyGUIGroup = this;
        //Add units in same group
        List<PlayerUnit> GroupUnits = GameManager.Instance.UnitManager.GameUnits.FindAll(delegate(PlayerUnit p1) { return p1.Info.GroupId == p.Info.GroupId && p1 != p; });
        if (GroupUnits != null && GroupUnits.Count > 0)
        {
            Units.AddRange(GroupUnits);
        }
    }

    public void AddUnit(Enemy e)
    {
        //Add MainUnit
        EnemyUnits.Add(e);
    }

    public GUIGroup RemoveUnit(PlayerUnit p)
    {
        GUIGroup ret = new GUIGroup();
        ret.AddUnit(p);
        this.Units.RemoveAll(delegate(PlayerUnit p1) { return p1.Info.GroupId == p.Info.GroupId; });
        return ret;
    }

    public GUIGroup RemoveUnit(Enemy e)
    {
        GUIGroup ret = new GUIGroup();
        ret.AddUnit(e);
        this.EnemyUnits.Remove(e);
        if (this.EnemyUnits.Count == 0)
        {
            GameManager.Instance.GUIManager.RemoveGroup(this);
        }
        return ret;
    }

    public void Update()
    {
        if (_CurrentInt >= _UpdateInterval)
        {
            //Units.Clear();
            List<PlayerUnit> newList = new List<PlayerUnit>();
            List<PlayerUnit> mainUnits = Units.FindAll(delegate(PlayerUnit p1) { return p1.Info.IsGroupMainUnit; });
            foreach (PlayerUnit unit in mainUnits)
            {
                List<PlayerUnit> allInGroup = GameManager.Instance.UnitManager.FindUnitsByGroupId(unit.Info.GroupId);
                newList.AddRange(allInGroup);
            }



            if (newList.Count > Units.Count)
            {
                Units = newList;
            }
        }
        _CurrentInt++;
        if (_CurrentInt > _UpdateInterval)
        {
            _CurrentInt = 0;
        }
    }


    public static GUIGroup operator +(GUIGroup a, GUIGroup b)
    {
        GUIGroup ret = new GUIGroup();
        ret.IsEnemy = a.IsEnemy || b.IsEnemy;

        if (!ret.IsEnemy)
        {
            ret.Units.AddRange(a.Units);
            ret.Units.AddRange(b.Units);
        }
        else
        {
            ret.EnemyUnits.AddRange(a.EnemyUnits);
            ret.EnemyUnits.AddRange(b.EnemyUnits);
        }
        //Debug.Log(string.Format("New group created. a:{0} with {1} units and b:{2} with {3} units became {4} with {5} units", a.Name, a.Units.Count,
        //b.Name, b.Units.Count,
        //ret.Name, ret.Units.Count));
        ret.Expanded = a.Expanded || b.Expanded;

        return ret;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Format("Group: {0}", Name));
        sb.AppendLine("Units:");
        if (IsEnemy)
        {
            foreach (Enemy e in EnemyUnits)
            {
                sb.AppendLine(string.Format("{0} - {1}", string.IsNullOrEmpty(e.Info.RefersToUnitName) ? "Unidentified" : e.Info.RefersToUnitName, e.Info.RefersToUnitClassId));
            }
        }
        else
        {
            foreach (PlayerUnit p in Units)
            {
                sb.AppendLine(string.Format("{0} - {1}", string.IsNullOrEmpty(p.Info.UnitName) ? "noname" : p.Info.UnitName, p.Info.UnitType.ToString()));
            }
        }


        return sb.ToString();
    }
}
