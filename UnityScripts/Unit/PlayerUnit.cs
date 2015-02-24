using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerUnit : MonoBehaviour
{

    #region GUI Variables

    private GameManager.GUIState _GUIState = GameManager.GUIState.Default;

    private string _Lat = "";
    private string _Lng = "";

    private bool _ShowAttackGUI;

    private Rect _SateliteRect;
    private Rect _WindowRect = new Rect(100, 50, Screen.width - 200, Screen.height - 150);


    //GUI RELATED VARIABLES
    private Vector3 _GUIScreenPos = Vector3.zero;
    private Vector3 _DestinationScreenPos = Vector3.zero;
    private Vector3 _Destination;
    private Vector3 _WorldPos = Vector3.zero;
    private Vector2 _Size;


    private float _Length;
    private float _Gap;
    private int _Segments;
    private Vector2 _C;
    private Vector2 _Delta;

    private Vector2 _FormationPosition2D;
    private PositionOffset _NewFormationPosition;


    #endregion


    public Texture2D Dot;
    public Texture2D MapTexture;
    public Texture2D PennantTexture;

    public Camera SateliteCamera;

    public float OptionalAngleTest = 0;

    public bool FormationChanged
    {
        get
        {
            return NewFormationPosition2D != Vector2.zero;
        }
    }

    public Vector2 ActualNewFormationPosition
    {
        get
        {
            return MathHelper.GUIPositionToActualFormationPosition(NewFormationPosition2D);
        }
    }

    public Vector2 NewFormationPosition2D
    {
        //TODO: Fix ghost
        get
        {
            if ( NewFormationPosition != null )
            {
                Vector2 guiPos = new Vector2(( float ) NewFormationPosition.RightM / GameManager.Instance.GUIManager.FormationFactorX,
                                                ( ( float ) NewFormationPosition.ForwardM / GameManager.Instance.GUIManager.FormationFactorY ) * -1);

                guiPos.x -= Size.x / 2;
                guiPos.y -= Size.y / 2;


                return guiPos;
            }
            return Vector2.zero;
        }

    }

    public PositionOffset NewFormationPosition
    {
        get
        {
            return _NewFormationPosition;
        }

        set
        {

            _NewFormationPosition = value;
        }
    }


    public Vector2 FormationGhostPosition2D
    {
        get
        {
            if ( FormationPosition != null )
            {
                if ( FormationPosition.PositionOffset != null )
                {
                    //float xFac = (float)FormationPosition.PositionOffset.RightM * GameManager.Instance.GUIManager.FormationFactorX;
                    //float yFac = (float)FormationPosition.PositionOffset.ForwardM * GameManager.Instance.GUIManager.FormationFactorY;
                    ////Debug.Log(string.Format("x : {0} - - yFac: {1}", xFac, yFac));
                    //xFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;
                    //yFac += GameManager.Instance.GUIManager.FormationRectSize.x / 2;


                    //return new Vector2(xFac - Size.x / 2, yFac - Size.y / 2);

                    //Vector2 guiPos = MathHelper.ActualFormationPositionToGUIPosition(new Vector2((float)FormationPosition.PositionOffset.RightM,
                    //                                                                                (float)FormationPosition.PositionOffset.ForwardM));

                    Vector2 guiPos = new Vector2(( float ) FormationPosition.PositionOffset.RightM / GameManager.Instance.GUIManager.FormationFactorX,
                                                    ( ( float ) FormationPosition.PositionOffset.ForwardM / GameManager.Instance.GUIManager.FormationFactorY ) * -1);


                    guiPos.x -= Size.x / 2;
                    guiPos.y -= Size.y / 2;

                    return guiPos;
                }
            }
            return Vector2.zero;
        }
    }

    private FormationPosition _FormationPosition;


    private Ocean _Ocean;
    private Transform _Map;

    private List<Rect> _WaypointRects;
    private List<Vector3> _WaypointPoints;
    private List<Waypoint> _Waypoints;

    private UnitClass _UnitClass;
    private BaseUnitInfo _Info;
    private PositionInfo _PositionInfo;
    private List<CarriedUnit> _CarriedUnits;
    private List<PlayerUnit> _UnitsInGroup;
    private GroupInfo _GroupInfo;
    private Coordinate Origin;
    private Coordinate LastCoordinate;

    public Rect MyRect;
    public GUIGroup MyGUIGroup;
    public bool GUIVisible = false;
    public int UniqueId;
    public MapUnit MyMapUnit;
    public GameObject PrefabMapUnit;
    public bool IsMissile;
    public bool InRepository;
    public float LerpTime;
    public float LastDist;
    public float Drag = 100;

    public float Bearing = 0;
    public float RealBearing;
    public Vector3 WantedPos;
    private bool _DoInterpolate = false;
    public float XAngle = 0;
    public float SpeedMod = 1.1f;
    private Vector3 _LastPos;
    public float ActualSpeedPerFrame;




    public BaseUnitInfo Info
    {
        get
        {
            return _Info;
        }
        set
        {
            _Info = value;
            Position = _Info.Position;
            _Lat = _Info.Position.Latitude.ToString();
            _Lng = _Info.Position.Longitude.ToString();

            UnitClass = GameManager.Instance.GetUnitClass(_Info.UnitClassId);

            if ( PrefabMapUnit != null && MyMapUnit == null )
            {
                if ( _Info != null )
                {
                    if ( _Info.IsGroupMainUnit || string.IsNullOrEmpty(_Info.GroupId) )
                    {
                        //GameObject prefab = !UnitClass.IsMissileOrTorpedo ? GameManager.Instance.CommunicationManager.MapUnit : GameManager.Instance.CommunicationManager.MissileMap;
                        GameObject prefab = GameManager.Instance.CommunicationManager.MapUnit;
                        GameObject mapUnit = Instantiate(prefab, new Vector3(1000, 10000, 1000), Quaternion.identity) as GameObject;
                        mapUnit.name = _Info.UnitName;
                        MapUnit m = mapUnit.AddComponent<MapUnit>();
                        m.Unit = this;
                        mapUnit.transform.parent = GameObject.Find("MapUnits").transform;
                        mapUnit.layer = 8;
                        MyMapUnit = m;
                    }
                }
            }
            if ( MyMapUnit != null && !_Info.IsGroupMainUnit )
            {
                if ( !string.IsNullOrEmpty(_Info.GroupId) )
                {
                    Destroy(MyMapUnit.gameObject);
                }
            }



            UniqueId = MathHelper.GetNumericId(_Info.Id);

            _Waypoints = new List<Waypoint>();


            foreach ( PositionInfo waypoint in _Info.Waypoints )
            {

                //Add waypoints
                _Waypoints.Add(new Waypoint(waypoint, new Vector2(32, 32)));
            }

            IsMissile = UnitClass.IsMissileOrTorpedo;
            List<CarriedUnit> oldUnits = null;
            if ( _CarriedUnits != null )
            {
                oldUnits = _CarriedUnits;
            }
            _CarriedUnits = new List<CarriedUnit>();
            foreach ( CarriedUnitInfo info in _Info.CarriedUnits )
            {
                CarriedUnit unit = new CarriedUnit();
                unit.UnitInfo = info;

                try
                {
                    if ( oldUnits != null )
                    {
                        CarriedUnit old = oldUnits.Find(delegate(CarriedUnit cu) { return cu.UnitInfo.Id == info.Id; });
                        if ( old != null )
                        {
                            unit.Selected = old.Selected;

                        }
                    }
                }
                catch ( Exception ex )
                {
                    Debug.LogError(string.Format("Error in PlayerUnit property Info set. Exception caught: {1}", ex.Message));
                }

                _CarriedUnits.Add(unit);
            }
            _UnitsInGroup = GameManager.Instance.UnitManager.FindUnitsByGroupId(_Info.GroupId);

            if ( _Info.DomainType != GameConstants.DomainType.Air )
            {
                HealthBar hb = GetComponentInChildren<HealthBar>();
                hb.Height = ( float ) UnitClass.HeightM;
                hb.UpdateHealth(( ( float ) 100 - ( float ) _Info.DamagePercent ) / 100.0f);
            }
            //transform.position = GetResetPosition();
        }
    }

    public PositionInfo Position
    {
        get
        {
            //if (_PositionInfo == null)
            //{
            //    return _Info.Position;
            //}
            return Info.Position;
        }
        set
        {
            //_PositionInfo = value;
            Info.Position = value;
            //TODO: Interpolate to real position.
            //if (GameManager.Instance.UnitManager.SelectedGroupMainUnit != this)
            //{
            //    Vector3 mainUnitPos = GameManager.Instance.UnitManager.SelectedGroupMainUnit.transform.position;
            //    float y = transform.position.y;
            //    transform.position = Vector3.Lerp(transform.position, GetResetPosition() + mainUnitPos, 0.9f);

            //}
            Coordinate currentCoordinate = new Coordinate(( float ) this.Info.Position.Latitude, ( float ) this.Info.Position.Longitude);
            if ( LastCoordinate == null )
            {
                LastCoordinate = currentCoordinate;
                transform.eulerAngles = new Vector3(0, ( float ) Info.Position.BearingDeg, 0);
            }
            else
            {
                if ( LastCoordinate == currentCoordinate )
                {
                    transform.eulerAngles = new Vector3(0, ( float ) Info.Position.BearingDeg, 0);
                }
                else
                {
                    float bearing = CoordinateHelper.CalculateBearing(LastCoordinate, currentCoordinate);
                    bearing = MathHelper.Clamp360(bearing + 270);
                    Vector3 euler = transform.eulerAngles;

                    transform.eulerAngles = new Vector3(euler.x, bearing, euler.z);
                    LastCoordinate = currentCoordinate;
                    Bearing = bearing;
                    RealBearing = ( float ) Info.Position.BearingDeg;
                }

            }




            //Interpolate position

            if ( GameManager.Instance.Origin != null )
            {
                //First Extrapolate from previous position
                float distanceLat = Mathf.Abs(( float ) Info.Position.Latitude - ( float ) GameManager.Instance.Origin.Latitude);
                float distanceLng = Mathf.Abs(( float ) Info.Position.Longitude - ( float ) GameManager.Instance.Origin.Longitude);

                if ( distanceLat < 1 && distanceLng < 1 )
                {


                    //calculate distance and bearing
                    //Coordinate origin = new Coordinate((float)selectedGroupMainUnit.Info.Position.Latitude, (float)selectedGroupMainUnit.Info.Position.Longitude);

                    Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, currentCoordinate);

                    Vector3 worldPos = new Vector3(coord.Longitude, ( float ) Info.Position.HeightOverSeaLevelM, coord.Latitude);

                    if ( Info.DomainType == GameConstants.DomainType.Air )
                    {
                        float heightOveSeaLevelM = ( float ) Info.Position.HeightOverSeaLevelM;
                        if ( heightOveSeaLevelM <= 101 )
                        {
                            worldPos.y = heightOveSeaLevelM;
                        }
                        else
                        {
                            worldPos.y = ( ( heightOveSeaLevelM / GameManager.Instance.MaxHeightInM ) * 5000 ) + 100;
                        }

                    }
                    else
                    {
                        worldPos.y = transform.position.y > -10 ? transform.position.y : 0;
                    }

                    if ( UnitClass.IsMissileOrTorpedo )
                    {
                        //Find target
                        Vector3 missilePos = worldPos;

                        if ( _LastPos != Vector3.zero )
                        {
                            missilePos = _LastPos;
                        }
                        transform.position = Vector3.Slerp(transform.position, missilePos - ( transform.forward * 1000 ), 0.9f);
                        _LastPos = worldPos;

                    }
                    else if ( !InRepository )
                    {
                        transform.position = Vector3.Slerp(transform.position, worldPos, Time.deltaTime);
                    }
                    else
                    {
                        transform.position = worldPos;
                    }

                    WantedPos = worldPos;
                    if ( Info.UnitName.ToLower() == "nimitz" )
                    {
                        Vector3 diff = transform.position - WantedPos;
                        //Debug.Log(string.Format("New Wanted pos. Difference: {0}", diff));
                    }
                    LerpTime = 0.1f;

                    //}
                    //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                    InRepository = false;
                    //renderer.enabled = true;
                    //if ( gameObject.layer == 9 )
                    //{
                    //    MathHelper.SetLayerRecursivly(transform, 0);
                    //}

                }
                else
                {
                    if ( GameManager.Instance.UnitManager.SelectedUnit == this )
                    {
                        GameManager.Instance.Origin = new Coordinate(( float ) this.Info.Position.Latitude, ( float ) this.Info.Position.Longitude);
                    }
                    else
                    {
                        InRepository = true;
                        transform.position = GameManager.Instance.UnitRepository;
                        //MathHelper.SetLayerRecursivly(transform, 9);
                    }
                    //renderer.enabled = false;

                }
            }
            else
            {

                InRepository = true;
                //renderer.enabled = false;
                //MathHelper.SetLayerRecursivly(transform, 9);
                transform.position = GameManager.Instance.UnitRepository;
            }
        }
    }

    public void SetGroupInfo(GroupInfo groupInfo)
    {
        if ( groupInfo.Id == Info.GroupId )
        {
            GroupInfo = groupInfo;
        }
    }


    public Vector3 MapPosition
    {
        get
        {
            return _WorldPos;
        }
    }

    public UnitClass UnitClass
    {
        get
        {
            return _UnitClass;
        }
        private set
        {
            _UnitClass = value;
        }
    }

    public List<PlayerUnit> UnitsInGroup
    {
        get
        {
            if ( string.IsNullOrEmpty(Info.GroupId) )
            {
                return null;
            }
            return GameManager.Instance.UnitManager.GameUnits.FindAll(delegate(PlayerUnit p) { return p.Info.GroupId == Info.GroupId; });
        }
    }

    public GroupInfo GroupInfo
    {
        get
        {
            if ( string.IsNullOrEmpty(Info.GroupId) )
            {
                return null;
            }
            GroupInfo groupInfo = null;
            try
            {

                groupInfo = GameManager.Instance.UnitManager.Groups.Find(delegate(GroupInfo gi) { return Info.GroupId == gi.Id; });
            }
            catch ( Exception ex )
            {
                return null;
            }

            return groupInfo;
        }
        set
        {
            _GroupInfo = value;
            if ( _Info != null && _GroupInfo != null )
            {
                if ( _GroupInfo.Formation != null )
                {
                    FormationPosition = _GroupInfo.Formation.FormationPositions.Find(delegate(FormationPosition fp) { return fp.Id == _Info.FormationPositionId; });
                }
            }
        }
    }

    public FormationPosition FormationPosition
    {
        get
        {
            return _FormationPosition;
        }
        set
        {
            _FormationPosition = value;
        }
    }

    public List<CarriedUnit> CarriedUnits
    {
        get
        {
            return _CarriedUnits;
        }
    }

    public string Latitude
    {
        get
        {
            return ToDegMinSecString(( float ) this.Info.Position.Latitude);
        }
    }

    public string Longitude
    {
        get
        {
            return ToDegMinSecString(( float ) this.Info.Position.Longitude);
        }
    }

    public Vector2 Size
    {
        get
        {
            return _Size;
        }
        set
        {
            _Size = value;
        }
    }

    public CoordinateClass MovePos
    {
        get
        {
            return new CoordinateClass(_Lat, _Lng);
        }
    }

    public GameManager.GUIState UnitGUIState
    {
        get
        {
            return _GUIState;
        }
        set
        {
            _GUIState = value;
        }
    }




    void Start()
    {





        _Map = GameObject.Find("Map").transform;
        //this.gameObject.layer = 9;
        //~ _SateliteRect = new Rect(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 50, 50);
        _Ocean = GameObject.Find("Ocean").GetComponent<Ocean>();



        _Gap = GameManager.Instance.GUIManager.LineGap;
        _Length = GameManager.Instance.GUIManager.LineLength;
        Dot = GameManager.Instance.GUIManager.DotTexture;
        MapTexture = GameManager.Instance.GUIManager.GetTextureByUnitClassId(Info.UnitClassId);
        PennantTexture = GameManager.Instance.GUIManager.DestinationTexture;
        SateliteCamera = GameManager.Instance.CameraManager.SateliteCamera;



        transform.position = GetResetPosition();

    }

    private Vector3 GetResetPosition()
    {
        PlayerUnit selectedUnit = GameManager.Instance.UnitManager.SelectedGroupMainUnit;
        Vector3 ret = Vector3.zero;
        //if (selectedUnit.Info.GroupId == Info.GroupId)
        //{
        if ( selectedUnit != null )
        {
            if ( selectedUnit == this )
            {
                //transform.position = new Vector3(0, transform.position.y, 0);
                ret = new Vector3(0, transform.position.y, 0);
            }
            else
            {
                float distanceLat = Mathf.Abs(( float ) Info.Position.Latitude - ( float ) selectedUnit.Info.Position.Latitude);
                float distanceLng = Mathf.Abs(( float ) Info.Position.Longitude - ( float ) selectedUnit.Info.Position.Longitude);

                if ( distanceLat < 1 && distanceLng < 1 )
                {
                    //calculate distance and bearing
                    Coordinate origin = new Coordinate(( float ) selectedUnit.Info.Position.Latitude, ( float ) selectedUnit.Info.Position.Longitude);
                    Coordinate position = new Coordinate(( float ) Info.Position.Latitude, ( float ) Info.Position.Longitude);

                    Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(origin, position);

                    Vector3 worldPos = new Vector3(coord.Longitude, ( float ) Info.Position.HeightOverSeaLevelM, coord.Latitude);

                    //Debug.Log(worldPos);
                    //transform.position = worldPos;
                    ret = worldPos;
                    ret.y = transform.position.y;

                }
                else
                {
                    //set to repository
                    //if (rigidbody != null)
                    //{
                    //    rigidbody.active = false;
                    //}
                    ret = GameManager.Instance.UnitRepository;
                    //transform.position = 
                }

            }
            //Position = transform.position;
            //transform.eulerAngles = new Vector3(OptionalAngleTest, Mathf.Floor((float)_Info.Position.BearingDeg), 0);
        }
        else
        {
            //set to repository
            //if (rigidbody != null)
            //{
            //    rigidbody.active = false;
            //}

            //transform.position = GameManager.Instance.UnitRepository;
            ret = GameManager.Instance.UnitRepository;
        }
        //}
        return ret;
    }

    public void FireMissile()
    {
        MissileLauncher[] gos = GetComponentsInChildren<MissileLauncher>();
        if ( gos.Length > 0 )
        {
            int random = UnityEngine.Random.Range(0, gos.Length);
            MissileLauncher l = gos[random];
            GameObject target = new GameObject("Target for " + Info.UnitName);
            target.transform.position = transform.TransformDirection(Vector3.forward * 1000) + transform.position;
            l.Fire(target);
        }
    }


    //void OnCollisionEnter(Collision collision)
    //{
    //    Collider collider = GetComponentInChildren<Collider>();
    //    Physics.IgnoreCollision(collider, collision.collider);
    //}

    public void SetupSelected()
    {

    }

    // Update is called once per frame

    void FixedUpdate()
    {

    }

    void OnBecameInvisible()
    {
        if ( GameManager.Instance.UnitManager.SelectedUnit != this )
        {
            float distanceLat = Mathf.Abs(( float ) Info.Position.Latitude - GameManager.Instance.Origin.Latitude);
            float distanceLng = Mathf.Abs(( float ) Info.Position.Longitude - GameManager.Instance.Origin.Longitude);

            if ( distanceLat < 1 && distanceLng < 1 )
            {
                //calculate distance and bearing
                //Coordinate origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                Coordinate position = new Coordinate(( float ) Info.Position.Latitude, ( float ) Info.Position.Longitude);

                Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                Vector3 worldPos = new Vector3(coord.Longitude, ( float ) Info.Position.HeightOverSeaLevelM, coord.Latitude);

                //Debug.Log(worldPos);
                worldPos.y = transform.position.y;
                //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                transform.position = worldPos;
                InRepository = false;
                //if (rigidbody)
                //{
                //    rigidbody.drag = Drag;
                //}
            }
            else
            {
                //set to repository
                if ( Info.DomainType == GameConstants.DomainType.Land )
                {
                    Debug.Log("Im not inside origin");
                }
                InRepository = true;
                //transform.position = GameManager.Instance.UnitRepository;
                //MathHelper.SetLayerRecursivly(transform, 9);

                //p.gameObject.active = false;
            }
        }
    }

    void Update()
    {
        if ( Info != null )
        {
            if ( rigidbody )
            {
                if ( rigidbody.drag > 5 )
                {
                    rigidbody.drag *= 0.1f;
                }
                else
                {
                    rigidbody.drag = 5;
                }
            }




            if ( _CarriedUnits != null )
            {
                foreach ( CarriedUnit carriedUnit in _CarriedUnits )
                {
                    carriedUnit.Update();
                }
            }

            if ( rigidbody )
            {
                Vector3 velocity = rigidbody.velocity;
                velocity.x = 0;
                velocity.z = 0;
                rigidbody.velocity = velocity;
            }

            #region GUI related

            _WorldPos = new Vector3();
            _WorldPos.x = ( ( float ) Position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier ) + GameManager.Instance.XMapAddition;
            _WorldPos.z = ( ( float ) Position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier ) + GameManager.Instance.YMapAddtion;
            _WorldPos.y = 30000;
            _GUIScreenPos = SateliteCamera.WorldToScreenPoint(_WorldPos);



            Rect shipPos = new Rect(_GUIScreenPos.x - Size.x / 2, SateliteCamera.pixelHeight - _GUIScreenPos.y - Size.y / 2, Size.x, Size.x);
            GUIVisible = false;

            if ( SateliteCamera.rect.height != 1 )
            {
                shipPos.y += Screen.height / 2;

                if ( shipPos.yMin < Screen.height / 2 && shipPos.xMax < Screen.height / 2 )
                {
                    GUIVisible = true;
                }
            }
            else
            {
                if ( MathHelper.ViewportRectToScreenRect(SateliteCamera.rect).Contains(new Vector2(shipPos.x, shipPos.y)) )
                {
                    GUIVisible = true;
                }
            }

            MyRect = shipPos;


            #endregion
            //if (LastCoordinate == null)
            //{
            //    LastCo

            //}


            Vector3 euler = transform.eulerAngles;
            euler.y = Bearing;
            euler.x = 0;
            //euler.z = Mathf.Clamp(euler.z, -2, 2);
            euler.z = 0;
            transform.eulerAngles = euler;

            if ( Info.DomainType == GameConstants.DomainType.Air )
            {
                Vector3 worldPos = transform.position;
                float heightOveSeaLevelM = ( float ) Info.Position.HeightOverSeaLevelM;
                if ( heightOveSeaLevelM <= 101 )
                {
                    worldPos.y = heightOveSeaLevelM;
                }
                else
                {
                    worldPos.y = ( ( heightOveSeaLevelM / GameManager.Instance.MaxHeightInM ) * 5000 ) + 100;
                }

                float angle = 0;
                float yDiff = worldPos.y - transform.position.y;

                angle = worldPos.y > transform.position.y ? 1 : -1;

                //euler.x = Mathf.Clamp(changeValue, -70, 70);

                transform.position = Vector3.Slerp(transform.position, worldPos, Time.deltaTime);
                //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, euler, Time.deltaTime); ;


                //float tiltAroundX = angle;
                //XAngle = angle;
                //transform.Rotate(new Vector3(angle, 0, 0), Space.Self);


            }
            else
            {

            }




            //Then extrapolate

            if ( !InRepository )
            {
                if ( Info.DomainType != GameConstants.DomainType.Land )
                {
                    float delta = Time.deltaTime;
                    float speedInKph = ( float ) Info.ActualSpeedKph;
                    float kmPerSec = speedInKph / 3600;
                    float mPerSec = kmPerSec * 1000;
                    float mPerFrame = mPerSec * delta;
                    ActualSpeedPerFrame = ( mPerFrame * ( float ) GameManager.Instance.GameInfo.RealTimeCompressionFactor ) * GameManager.Instance.GlobalScale.z;

                    //if (Info.IsGroupMainUnit && Info.UnitName == "Nimitz")
                    //{
                    //rigidbody.AddForce(new Vector3(0, 0, mPerFrame), ForceMode.VelocityChange);
                    //rigidbody.velocity = new Vector3(0, 0, mPerFrame);
                    transform.Translate(new Vector3(0, 0, ActualSpeedPerFrame), Space.Self);
                    //Debug.Log(string.Format("delta:{0} - speedinKph:{1} - kilometers per Sec: {2} - meters per sec:{3} - meters per frame:{4} - kmTraveled:{5}", delta, speedInKph, kmPerSec, mPerSec, mPerFrame, kmTraveledInFrame));
                    //}


                    WantedPos += new Vector3(0, 0, mPerFrame * GameManager.Instance.GlobalScale.z * ( float ) GameManager.Instance.GameInfo.RealTimeCompressionFactor);
                    Vector3 diff = transform.position - WantedPos;
                    //transform.Translate(diff * Time.deltaTime, Space.World);
                    if ( Info.UnitName.ToLower() == "nimitz" )
                    {
                        //Debug.Log(string.Format("Difference = {0}", diff));
                    }
                }

            }
            else
            {
                transform.position = GameManager.Instance.UnitRepository;
            }
        }


    }

    public void SpawnExplosion()
    {
        GameObject go = Instantiate(GameManager.Instance.SurfaceExplosion, transform.position, Quaternion.identity) as GameObject;
        //go.transform.parent = this.transform;
    }

    void OnGUI()
    {
        if ( _Info != null )
        {


            if ( GUIVisible )
            {
                //if (GUI.Button(shipPos, GameManager.Instance.GUIManager.GetTextureByUnitClassId(_Info.UnitClassId), "Label"))
                //{
                //    if (Event.current.button == 1)
                //    {

                //    }
                //    GameManager.Instance.UnitManager.SelectedUnit = this;
                //    //(GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = new Vector3(_WorldPos.x, 35100, _WorldPos.z);
                //}
            }

            if ( GameManager.Instance.UnitManager.SelectedGroupMainUnit == this )
            {
                foreach ( Waypoint waypoint in _Waypoints )
                {
                    Vector3 wp = new Vector3();
                    wp.x = ( ( float ) waypoint.Position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier ) + GameManager.Instance.XMapAddition;
                    wp.z = ( ( float ) waypoint.Position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier ) + GameManager.Instance.YMapAddtion;
                    wp.y = 30000;

                    Vector3 GUIPos = SateliteCamera.WorldToScreenPoint(wp);

                    Rect waypointPos = new Rect(GUIPos.x - waypoint.Size.x / 2, SateliteCamera.pixelHeight - GUIPos.y - waypoint.Size.y / 2, waypoint.Size.x, waypoint.Size.x);
                    bool show = false;

                    if ( SateliteCamera.rect.height != 1 )
                    {
                        waypointPos.y += Screen.height / 2;

                        if ( waypointPos.yMin < Screen.height / 2 && waypointPos.xMax < Screen.height / 2 )
                        {
                            show = true;
                        }
                    }
                    else
                    {
                        //Debug.Log(waypointPos);
                        if ( MathHelper.ViewportRectToScreenRect(SateliteCamera.rect).Contains(new Vector2(waypointPos.x, waypointPos.y)) )
                        {
                            show = true;
                        }
                    }
                    if ( show )
                    {
                        if ( GUI.Button(waypointPos, PennantTexture, "Label") )
                        {


                        }
                    }
                }

                _WaypointPoints = new List<Vector3>();
                foreach ( PositionInfo waypoint in Info.Waypoints )
                {
                    Vector3 wp = new Vector3();
                    wp.x = ( ( float ) waypoint.LongitudeOrthoProjected * GameManager.Instance.XMapModifier ) + GameManager.Instance.XMapAddition;
                    wp.z = ( ( float ) waypoint.LatitudeOrthoProjected * GameManager.Instance.YMapModifier ) + GameManager.Instance.YMapAddtion;
                    wp.y = 30000;

                    _WaypointPoints.Add(SateliteCamera.WorldToScreenPoint(wp));
                }

                _WaypointRects = new List<Rect>();

                Vector2 start = new Vector2(_GUIScreenPos.x, _GUIScreenPos.y);

                //if (_WaypointPoints.Count > 1)
                //{
                if ( _WaypointPoints.Count > 0 )
                {
                    Vector2 wayPointFirst = new Vector2(_WaypointPoints[0].x, _WaypointPoints[0].y);
                    CalculateDestinationSegments(start, wayPointFirst);

                    //    Vector2 middle = new Vector2((start.x + wayPointFirst.x) / 2, SateliteCamera.pixelHeight - ((start.y + wayPointFirst.y) / 2));
                    //    GUI.contentColor = Color.black;
                    //    //Debug.Log(Info.Position.);
                    //    TimeSpan ETACurrentGameTime = TimeSpan.FromSeconds(Position.EtaCurrentWaypointSec);
                    //    TimeSpan ETAAllGameTime = TimeSpan.FromSeconds(Position.EtaAllWaypointsSec);
                    //    TimeSpan ETACurrentRealTime = TimeSpan.FromSeconds(Position.EtaCurrentWaypointSec / GameManager.Instance.GameInfo.RealTimeCompressionFactor);
                    //    TimeSpan ETAAllRealTime = TimeSpan.FromSeconds(Position.EtaAllWaypointsSec / GameManager.Instance.GameInfo.RealTimeCompressionFactor);
                    //    Rect etaInfo = new Rect(middle.x, middle.y, 300, 120);
                    //    GUI.Box(etaInfo, "");
                    //    GUILayout.BeginArea(etaInfo);
                    //    GUILayout.BeginVertical();
                    //    GUILayout.Label(string.Format("ETA current Waypoint in GameTime:{0}days {1:00}:{2:00}:{3:00}", ETACurrentGameTime.Days, ETACurrentGameTime.Hours, ETACurrentGameTime.Minutes, ETACurrentGameTime.Seconds));
                    //    GUILayout.Label(string.Format("ETA current Waypoint in RealTime:{0}days {1:00}:{2:00}:{3:00}", ETACurrentRealTime.Days, ETACurrentRealTime.Hours, ETACurrentRealTime.Minutes, ETACurrentRealTime.Seconds));
                    //    GUILayout.Label(string.Format("ETA All Waypoints in GameTime:{0}days {1:00}:{2:00}:{3:00}", ETAAllGameTime.Days, ETAAllGameTime.Hours, ETAAllGameTime.Minutes, ETAAllGameTime.Seconds));
                    //    GUILayout.Label(string.Format("ETA All Waypoints in RealTime:{0}days {1:00}:{2:00}:{3:00}", ETAAllRealTime.Days, ETAAllRealTime.Hours, ETAAllRealTime.Minutes, ETAAllRealTime.Seconds));
                    //    GUILayout.EndVertical();
                    //    GUILayout.EndArea();

                    for ( int i = 1; i < _WaypointPoints.Count; i++ )
                    {
                        CalculateDestinationSegments(new Vector2(_WaypointPoints[i - 1].x, _WaypointPoints[i - 1].y),
                                                    new Vector2(_WaypointPoints[i].x, _WaypointPoints[i].y));
                    }
                    //}
                }
                //}
            }

        }
    }

    private void CalculateDestinationSeg(Vector2 start, Vector2 end)
    {
        float seglength, deltax, deltay, cx, cy, len, gap = 0;
        len = _Length;
        gap = _Gap;
        // calculate the legnth of a segment
        seglength = len + gap;
        // calculate the length of the dashed line
        deltax = end.x - start.x;
        deltay = end.y - start.y;
        float delta = Mathf.Sqrt(( deltax * deltax ) + ( deltay * deltay ));

        // calculate the number of segments needed
        _Segments = ( int ) Mathf.Floor(Mathf.Abs(delta / seglength));

        // get the angle of the line in radians
        float radians = Mathf.Atan2(deltay, deltax);
        // start the line here
        cx = start.x;
        cy = start.y;
        // add these to cx, cy to get next seg start
        deltax = Mathf.Cos(radians) * seglength;
        deltay = Mathf.Sin(radians) * seglength;
        //~ _LineRenderer.SetVertexCount((int)segs);
        if ( _Segments == 0 )
        {
            _Segments++;
        }
        //~ else if(_Segments > 10)
        //~ {
        //~ _Segments = 10;
        //~ }

        _C = new Vector2(_GUIScreenPos.x, _GUIScreenPos.y);
        _Delta = new Vector2(deltax, deltay);
    }

    private void CalculateDestinationSegments(Vector2 start, Vector2 end)
    {
        float seglength, deltax, deltay, cx, cy, len, gap = 0;
        len = _Length;
        gap = _Gap;
        // calculate the legnth of a segment
        seglength = len + gap;
        // calculate the length of the dashed line
        deltax = end.x - start.x;
        deltay = end.y - start.y;
        float delta = Mathf.Sqrt(( deltax * deltax ) + ( deltay * deltay ));

        // calculate the number of segments needed
        _Segments = ( int ) Mathf.Floor(Mathf.Abs(delta / seglength));

        // get the angle of the line in radians
        float radians = Mathf.Atan2(deltay, deltax);
        // start the line here
        cx = start.x;
        cy = start.y;
        // add these to cx, cy to get next seg start
        deltax = Mathf.Cos(radians) * seglength;
        deltay = Mathf.Sin(radians) * seglength;
        //~ _LineRenderer.SetVertexCount((int)segs);
        if ( _Segments == 0 )
        {
            _Segments++;
        }
        //~ else if(_Segments > 10)
        //~ {
        //~ _Segments = 10;
        //~ }

        _C = start;
        _Delta = new Vector2(deltax, deltay);
        //sqrt((250-100)^2 + (70-50)^2) = 151.3
        //float angle = Vector2.


        float angle = ( Mathf.Atan2(start.y - end.y, start.x - end.x) * 180 / Mathf.PI ) * Mathf.Rad2Deg;
        //Debug.Log(angle);

        for ( int n = 0; n < _Segments; n++ )
        {
            if ( n > 1 )
            {
                Rect position = new Rect(_C.x - 4, SateliteCamera.pixelHeight - _C.y - 4, 8, 8);
                bool show = false;

                _WaypointRects.Add(position);
                if ( SateliteCamera.rect.height != 1 )
                {
                    position.y += Screen.height / 2;
                    if ( position.yMin < Screen.height / 2 && position.xMax < Screen.height / 2 )
                    {
                        show = true;
                    }
                }
                else
                {
                    if ( MathHelper.ViewportRectToScreenRect(SateliteCamera.rect).Contains(new Vector2(position.x, position.y)) )
                    {
                        show = true;
                    }
                }
                if ( show )
                {
                    Matrix4x4 matrixBackup = GUI.matrix;
                    GUIUtility.RotateAroundPivot(angle, new Vector2(position.x + 4, position.y + 4));
                    //GUIUtility.RotateAroundPivot(angle, Vector2.zero);
                    //GUI.Label(position, Dot);
                    GUI.color = Color.green;
                    GUI.DrawTexture(position, Dot);
                    GUI.matrix = matrixBackup;
                }
            }

            _C.x += _Delta.x;
            _C.y += _Delta.y;
        }

    }

    void OnDrawGizmos()
    {
        //if (_WorldPos != Vector3.zero)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(_WorldPos, 100);
        //}
    }

    void WindowFunc(int id)
    {

    }

    void OnMouseOver()
    {
        //Debug.Log("Over me : " + Info.Id);
    }

    void OnMouseDown()
    {
        if ( GameManager.Instance.UnitManager.SelectedUnit != this )
        {
            GameManager.Instance.UnitManager.SelectedUnit = this;
        }
    }

    public void Kill(bool explode)
    {
        if ( explode )
        {
            SpawnExplosion();
            MyMapUnit.Explode();
        }
        GameManager.Instance.UnitManager.RemoveUnit(this);
    }

    public static string ToDegMinSecString(float degrees)
    {
        float d = Mathf.Abs(degrees);  // (unsigned result ready for appending compass dir'n)
        d += 1 / 7200;  // add ½ second for rounding
        float deg = Mathf.Floor(d);
        float min = Mathf.Floor(( d - deg ) * 60);
        float sec = Mathf.Floor(( d - deg - min / 60 ) * 3600);
        string sdeg = deg.ToString();
        string smin = min.ToString();
        string ssec = sec.ToString();

        // add leading zeros if required
        if ( deg < 100 )
        {
            sdeg = '0' + sdeg;
        }
        if ( deg < 10 )
        {
            sdeg = '0' + sdeg;
        }
        if ( min < 10 )
        {
            smin = '0' + smin;
        }
        if ( sec < 10 )
        {
            ssec = '0' + ssec;
        }
        return string.Format("{0}\u00B0 {1}' {2}\"", sdeg, smin, ssec);

    }
}
//////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////
/////////////////////Coordinate class //////////////////////////////
//////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////
public class CoordinateClass
{
    private string _Lat;
    private string _Lng;

    public string Latitude
    {
        get
        {
            return _Lat;
        }
    }

    public string Longitude
    {
        get
        {
            return _Lng;
        }
    }

    public CoordinateClass()
    {
    }

    public CoordinateClass(string lat, string lng)
    {
        _Lat = lat;
        _Lng = lng;
    }
}
