using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;

public class UnitManager : MonoBehaviour
{



    #region Privates and Enums

    public enum UnitToWatch
    {
        Player,
        Ally,
        Enemy
    }

    public List<PlayerUnit> _GameUnits;

    private List<Enemy> _EnemyDetections;

    private List<GroupInfo> _Groups;

    public PlayerUnit _SelectedUnit;
    public PlayerUnit _TempSelected;
    public PlayerUnit _SelectedGroupMainUnit;
    private Enemy _SelectedEnemyUnit;
    private UnitClass _SelectedUnitClass;
    private Vector3 _GUIScreenPos = Vector3.zero;
    private Camera _SateliteCamera;
    public Vector2 GUIUnitSize;


    public OrbitCam OrbitCamera;

    public float x3DMulti = 10;
    public float y3DMulti = 10;

    public GameObject WindowsRoot;
    public UnitToWatch CurrentWatchMode = UnitToWatch.Player;
    public List<PlayerUnit> Missiles;

    #endregion

    #region Public Properties

    public List<Enemy> EnemyDetections
    {
        get
        {
            return _EnemyDetections;
        }
    }

    public List<PlayerUnit> GameUnits
    {
        get
        {
            return _GameUnits;
        }
    }

    public List<GroupInfo> Groups
    {
        get
        {
            return _Groups;
        }
        private set
        {
            _Groups = value;
        }
    }

    public PlayerUnit SelectedUnit
    {
        get
        {
            if ( _SelectedUnit == null )
            {
                if ( GameUnits.Count == 0 )
                {
                    return null;
                }
                return null;
                //else
                //{
                //    int random = UnityEngine.Random.Range(0, GameUnits.Count);
                //    SelectedUnit = GameUnits[random];
                //}
            }
            return _SelectedUnit;
        }
        set
        {
            _TempSelected = value;

            StartCoroutine(FadeOut(2));

            _SelectedUnit = value;
            //GameObject.Find("Tactical Map").transform.position = new Vector3(_SelectedUnit.transform.position.x, 30000, _SelectedUnit.transform.position.z);
            if ( OrbitCamera != null && _SelectedUnit != null )
            {
                OrbitCamera.Target = _SelectedUnit.transform;
            }

            //_SelectedEnemyUnit = null;
            if ( _SelectedUnit == null )
            {
                _SelectedGroupMainUnit = null;
            }
            else
            {
                _SelectedGroupMainUnit = _SelectedUnit.Info.IsGroupMainUnit ? _SelectedUnit : FindGroupMainUnitByGroupId(_SelectedUnit.Info.GroupId);
                GameManager.Instance.EnviromentManager.CurrentWeather = _SelectedUnit.Info.WeatherSystem;
                if ( _SelectedGroupMainUnit != null )
                {
                    //_SelectedGroupMainUnit.SetupSelected();
                }
                //SelectedUnit.SetupSelected();
                CurrentWatchMode = UnitToWatch.Player;
                SetupSelectedUnit(_SelectedUnit);
                //_SelectedEnemyUnit = null;

                OrbitCam camscript = GameManager.Instance.CameraManager.MainCamera.GetComponent<OrbitCam>();

                _SelectedUnitClass = GameManager.Instance.GetUnitClass(_SelectedUnit.Info.UnitClassId);

                camscript.m_MaxDistance = ( float ) _SelectedUnitClass.LengthM * GameManager.Instance.GlobalScale.x * 20;
                camscript.m_MinDistance = ( float ) _SelectedUnitClass.LengthM * GameManager.Instance.GlobalScale.x * 1;
                //camscript.distance = camscript.m_MaxDistance;
                GameObject skyDome = GameObject.Find("Skydome");
                if ( skyDome != null )
                {
                    skyDome.GetComponent<FollowCameraXZ>().target = SelectedUnit.transform;
                }

                //if (_SelectedUnit.Info.DomainType == TTG.NavalWar.NWComms.GameConstants.DomainType.Air)
                //{
                //    if (camscript != null)
                //    {
                //        camscript.yMinLimit = GameManager.Instance.SurfaceCameraYMinLimitAir;
                //        camscript.yMaxLimit = GameManager.Instance.SurfaceCameraYMaxLimitAir;
                //    }
                //}
                //else
                //{
                if ( camscript != null )
                {
                    camscript.yMinLimit = GameManager.Instance.SurfaceCameraYMinLimitNormal;
                    camscript.yMaxLimit = GameManager.Instance.SurfaceCameraYMaxLimitNormal;
                }


            }
            WindowsRoot.BroadcastMessage("OnSelectedUnitChanged", _SelectedUnit != null ? _SelectedUnit : new PlayerUnit(), SendMessageOptions.DontRequireReceiver);
            CurrentWatchMode = UnitToWatch.Player;


            //~ Invoke("DoSwitchCamera", 1);

        }
    }

   

    private IEnumerator FadeOut(int secondsToBeforeFadeIn)
    {
        GameManager.Instance.CameraManager.MainCamera.GetComponent<CameraFade>().FadeOut();

        yield return new WaitForSeconds(secondsToBeforeFadeIn);

    }

    public PlayerUnit SelectedGroupMainUnit
    {
        get
        {
            return _SelectedGroupMainUnit;
        }
    }


    public Enemy SelectedEnemyUnit
    {
        get
        {
            return _SelectedEnemyUnit;
        }
        set
        {

            //SelectedUnit = null;
            _SelectedEnemyUnit = value;
            SetupSelectedUnit(_SelectedEnemyUnit);
            if ( OrbitCamera != null )
            {
                OrbitCamera.Target = _SelectedEnemyUnit.transform;
            }
            GameObject skyDome = GameObject.Find("Skydome");
            if ( skyDome != null )
            {
                skyDome.GetComponent<FollowCameraXZ>().target = _SelectedEnemyUnit.transform;
            }
            CurrentWatchMode = UnitToWatch.Enemy;
            //GameManager.Instance.Origin = new Coordinate(( float ) _SelectedEnemyUnit.Info.Position.Latitude, ( float ) _SelectedEnemyUnit.Info.Position.Longitude);
        }
    }


    #endregion


    #region Public Methods

    public PlayerUnit FindUnitById(string id)
    {
        //~ if(_GameUnits == null)
        //~ {
        //~ _GameUnits = new List<PlayerUnit>();
        //~ return null;
        //~ }
        PlayerUnit unit = null;
        try
        {
            unit = GameUnits.Find(delegate(PlayerUnit p) { return p.Info.Id == id; });
        }
        catch ( NullReferenceException ex )
        {
            Debug.Log("Cannot find unit in UnitManager.FindUnitById. Error: " + ex.Message);
            return null;
        }

        return unit;

    }

    public List<PlayerUnit> FindUnitsByGroupId(string groupId)
    {
        List<PlayerUnit> units = new List<PlayerUnit>();
        try
        {
            units = GameUnits.FindAll(delegate(PlayerUnit p) { return p.Info.GroupId == groupId; });
        }
        catch ( NullReferenceException ex )
        {
            Debug.Log("Cannot find unit in UnitManager.FindUnitsByGroupId. Error: " + ex.Message);
            return new List<PlayerUnit>();
        }
        return units;
    }

    public PlayerUnit FindGroupMainUnitByGroupId(string groupId)
    {
        PlayerUnit unit;

        try
        {
            unit = GameUnits.Find(delegate(PlayerUnit p) { return p.Info.GroupId == groupId && p.Info.IsGroupMainUnit; });
        }
        catch ( Exception )
        {
            return null;
        }

        return unit;
    }

    public Enemy FindEnemyById(string id)
    {
        Enemy enemy = null;

        try
        {
            enemy = EnemyDetections.Find(delegate(Enemy e) { return e.Info.Id == id; });
        }
        catch ( NullReferenceException ex )
        {
            Debug.Log("Cannot find unit in UnitManager.FindEnemyById. Error: " + ex.Message);
            return null;
        }
        return enemy;

    }


    public void AddEnemy(Enemy enemy)
    {
        if ( enemy != null )
        {
            if ( _EnemyDetections.Find(delegate(Enemy e) { return e.Info.Id == enemy.Info.Id; }) == null )
            {
                _EnemyDetections.Add(enemy);
            }
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if ( enemy != null )
        {
            Enemy e = _EnemyDetections.Find(delegate(Enemy en) { return enemy.Info.Id == en.Info.Id; });

            if ( e != null )
            {
                _EnemyDetections.Remove(e);
            }
        }
    }

    public void AddUnit(PlayerUnit unit)
    {
        if ( unit != null )
        {
            //~ Debug.Log("Group :" + unit.Info.GroupId);
            if ( _GameUnits.Find(delegate(PlayerUnit pu) { return unit.Info.Id == pu.Info.Id; }) == null )
            {
                _GameUnits.Add(unit);
                if ( GameManager.Instance.GetUnitClass(unit.Info.UnitClassId).IsMissileOrTorpedo )
                {
                    Missiles.Add(unit);
                }
            }
            //~ AddToMapGroup(unit);
        }
    }

    public void RemoveUnit(PlayerUnit unit)
    {
        if ( unit != null )
        {
            Debug.Log(string.Format("Destroying unit: {0}", unit.Info.UnitName));
            _GameUnits.Remove(unit);
            if ( unit.MyMapUnit != null )
            {
                Destroy(unit.MyMapUnit.gameObject);
            }
            Destroy(unit.gameObject);
            if ( Missiles.Contains(unit) )
            {
                Missiles.Remove(unit);
            }
        }
    }

    public void AddGroupInfo(GroupInfo group)
    {
        if ( group != null )
        {
            Debug.Log("Adding group " + group.Id);
            if ( Groups == null )
            {
                Groups = new List<GroupInfo>();
            }
            GroupInfo g = Groups.Find(delegate(GroupInfo gi) { return gi.Id == group.Id; });
            if ( g == null )
            {
                Debug.Log("Group Added");
                Groups.Add(group);
            }
            else
            {
                Debug.Log("Group exists");
            }

            GameObject.Find("SurfaceUnits").BroadcastMessage("SetGroupInfo", group, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void SetupSelectedUnit(PlayerUnit playerUnit)
    {
        if ( playerUnit.Info.DomainType == GameConstants.DomainType.Surface )
        {
            playerUnit.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            playerUnit.transform.position = new Vector3(0, ( float ) playerUnit.Info.Position.HeightOverSeaLevelM, 0);
        }
        Coordinate newPos = new Coordinate(( float ) playerUnit.Info.Position.Latitude, ( float ) playerUnit.Info.Position.Longitude);
        //if ( !GameManager.Instance.Origin.IsInDistanceOf(newPos, 1) )
        //{
        GameManager.Instance.Origin = newPos;

        SetupUnits();


    }
    private void SetupSelectedUnit(Enemy enemyUnit)
    {
        if ( enemyUnit.Info.DetectionClassification == GameConstants.DetectionClassification.Surface )
        {
            enemyUnit.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            enemyUnit.transform.position = new Vector3(0, ( float ) enemyUnit.Info.Position.HeightOverSeaLevelM, 0);
        }
        Coordinate newPos = new Coordinate(( float ) enemyUnit.Info.Position.Latitude, ( float ) enemyUnit.Info.Position.Longitude);
        //if ( !GameManager.Instance.Origin.IsInDistanceOf(newPos, 1) )
        //{
        GameManager.Instance.Origin = newPos;

        SetupUnits();
    }

    private void SetupUnits()
    {
        foreach ( PlayerUnit p in GameUnits )
        {

            if ( SelectedUnit != null )
            {
                if ( p.Info.Id == GameManager.Instance.UnitManager.SelectedUnit.Info.Id )
                {
                    if ( CurrentWatchMode == UnitToWatch.Player )
                    {
                        p.InRepository = false;
                        continue;
                    }
                }
            }
            if ( p == null )
            {
                continue;
            }

            float distanceLat = Mathf.Abs(( float ) p.Info.Position.Latitude - GameManager.Instance.Origin.Latitude);
            float distanceLng = Mathf.Abs(( float ) p.Info.Position.Longitude - GameManager.Instance.Origin.Longitude);
            if ( distanceLat < 1 && distanceLng < 1 )
            {
                //calculate distance and bearing
                //Coordinate origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                Coordinate position = new Coordinate(( float ) p.Info.Position.Latitude, ( float ) p.Info.Position.Longitude);

                Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                Vector3 worldPos = new Vector3(coord.Longitude, ( float ) p.Info.Position.HeightOverSeaLevelM, coord.Latitude);

                //Debug.Log(worldPos);
                //worldPos.y = transform.position.y;
                //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                p.transform.position = worldPos;
                p.InRepository = false;
                //if ( rigidbody )
                //{
                //    rigidbody.drag = Drag;
                //}

            }
            else
            {
                if ( p.Info.DomainType == GameConstants.DomainType.Land )
                {

                }
                //set to repository

                p.InRepository = true;
                p.transform.position = GameManager.Instance.UnitRepository;
                //MathHelper.SetLayerRecursivly(transform, 9);
                //p.gameObject.active = false;
            }
        }
        //enemy units
        foreach ( Enemy e in EnemyDetections )
        {
            if ( SelectedEnemyUnit != null )
            {
                if ( e.Info.Id == SelectedEnemyUnit.Info.Id )
                {
                    if ( CurrentWatchMode == UnitToWatch.Enemy )
                    {
                        e.InRepository = false;
                        continue;
                    }
                }
            }
            if ( e == null )
            {
                continue;
            }

            float distanceLat = Mathf.Abs(( float ) e.Info.Position.Latitude - GameManager.Instance.Origin.Latitude);
            float distanceLng = Mathf.Abs(( float ) e.Info.Position.Longitude - GameManager.Instance.Origin.Longitude);
            if ( distanceLat < 1 && distanceLng < 1 )
            {
                //calculate distance and bearing
                //Coordinate origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                Coordinate position = new Coordinate(( float ) e.Info.Position.Latitude, ( float ) e.Info.Position.Longitude);

                Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                Vector3 worldPos = new Vector3(coord.Longitude, ( float ) e.Info.Position.HeightOverSeaLevelM, coord.Latitude);

                //Debug.Log(worldPos);
                //worldPos.y = transform.position.y;
                //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                e.transform.position = worldPos;
                e.InRepository = false;
                //if ( rigidbody )
                //{
                //    rigidbody.drag = Drag;
                //}

            }
            else
            {
                if ( e.Info.DomainType == GameConstants.DomainType.Land )
                {

                }
                //set to repository

                e.InRepository = true;
                e.transform.position = GameManager.Instance.UnitRepository;
                //MathHelper.SetLayerRecursivly(transform, 9);
                //p.gameObject.active = false;
            }
        }
    }

    #endregion

    #region Private Methods
    private void DoSwitchCamera()
    {
        //GameManager.Instance.CameraManager.SwitchCamera(GameManager.Instance.CameraManager.MainCamera);
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        Missiles = new List<PlayerUnit>();
        _GameUnits = new List<PlayerUnit>();
        _EnemyDetections = new List<Enemy>();
        _SateliteCamera = GameManager.Instance.CameraManager.SateliteCamera;
    }

    public float KmPerPixel = 10;

    void Update()
    {
        //if (SelectedUnit != null)
        //{
        //    GameObject.Find("Ocean").SendMessage("SetPosition", SelectedUnit.transform.position, SendMessageOptions.DontRequireReceiver);
        //}
        #region Commented Out
        ////_Icons.Clear();
        //List<UnitRect> GUIRects = new List<UnitRect>();
        //foreach (PlayerUnit p in _GameUnits)
        //{
        //    _GUIScreenPos = _SateliteCamera.WorldToScreenPoint(Map.position + CoordinateHelper.LatLongToWorldPoint(p.Info.Position));
        //    //Draw GUI or check if interlap
        //    Rect shipPos = new Rect(_GUIScreenPos.x - GUIUnitSize.x / 2, _SateliteCamera.pixelHeight - _GUIScreenPos.y - GUIUnitSize.y / 2, GUIUnitSize.x, GUIUnitSize.x);
        //    GUIRects.Add(new UnitRect(p, shipPos));
        //}
        //for (int i = 0; i < GUIRects.Count; i++)
        //{
        //    for (int j = 0; j < GUIRects.Count; j++)
        //    {
        //        //if (GUIRects[i] == GUIRects[j])
        //        //{
        //        //    continue;
        //        //}

        //        if (GameManager.Instance.GUIManager.IntersectRect(GUIRects[i].Rect, GUIRects[j].Rect))
        //        {
        //            SateliteIcon s = IconExits(GUIRects[i]);
        //            if (s == null)
        //            {
        //                SateliteIcon s2 = IconExits(GUIRects[j]);
        //                if (s2 != null)
        //                {
        //                    s2.Units.Add(GUIRects[i]);
        //                }
        //                else
        //                {
        //                    s = new SateliteIcon();
        //                    s.Units.Add(GUIRects[i]);
        //                    s.Units.Add(GUIRects[j]);
        //                    _Icons.Add(s);
        //                }
        //            }
        //            else
        //            {

        //                SateliteIcon s2 = IconExits(GUIRects[j]);

        //                if (s == s2)
        //                {
        //                    continue;
        //                }

        //                if (s2 == null)
        //                {
        //                    s.Units.Add(GUIRects[j]);
        //                }
        //                else
        //                {
        //                    Debug.Log("Why do you come here " + (s2 == s));
        //                    s.Units.AddRange(s2.Units);
        //                    _Icons.Remove(s2);
        //                }
        //            }
        //        }
        //    }

        //}
        #endregion


    }

    void OnGUI()
    {
    }

    void OnDrawGizmos()
    {

    }



}
