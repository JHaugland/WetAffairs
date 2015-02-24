using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class Enemy : MonoBehaviour
{

    private DetectedUnitInfo _Info;
    public string Name;

    private Rect _Pos;
    public Rect GUIPos;
    public Vector3 GUIScreenPos = Vector3.zero;
    private Vector2 _Size;
    private Vector3 _WorldPos;
    private Camera _SateliteCamera;
    private bool _ShowAttackMenu = false;
    private EnemyMapUnit MapUnit;
    public Coordinate LastCoordinate;

    public bool GUIVisible = false;
    public bool InRepository;
    public float Bearing;
    public float RealBearing;




    public Texture2D _VesselTexture;

    private Transform _Map;

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

    public Vector3 MapPosition
    {
        get
        {
            return _WorldPos;
        }
    }

    public DetectedUnitInfo Info
    {
        get
        {
            return _Info;
        }
        set
        {
            _Info = value;

            if (_Info != null && MapUnit == null)
            {
                GameObject mapUnit = Instantiate(GameManager.Instance.CommunicationManager.EnemyMapUnit, new Vector3(0, 1000, 0), Quaternion.identity) as GameObject;
                MapUnit = mapUnit.AddComponent<EnemyMapUnit>();
                mapUnit.transform.parent = GameObject.Find("EnemyMapUnits").transform;
                mapUnit.layer = 8;
                mapUnit.name = _Info.RefersToUnitClassId;
                MapUnit.EnemyUnit = this;
            }

            if (MapUnit != null)
            {
                MapUnit.UpdateTextures();
            }


            Coordinate currentCoordinate = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
            if (LastCoordinate == null)
            {
                LastCoordinate = currentCoordinate;
                transform.eulerAngles = new Vector3(0, (float)Info.Position.BearingDeg, 0);
            }
            else
            {
                if (LastCoordinate == currentCoordinate)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, (float)Info.Position.BearingDeg, transform.eulerAngles.y);
                }
                else
                {
                    float bearing = CoordinateHelper.CalculateBearing(LastCoordinate, currentCoordinate);
                    bearing = MathHelper.Clamp360(bearing + 270);
                    Vector3 euler = transform.eulerAngles;

                    transform.eulerAngles = new Vector3(euler.x, bearing, euler.z);
                    LastCoordinate = currentCoordinate;
                    Bearing = bearing;
                    RealBearing = (float)Info.Position.BearingDeg;
                }

            }




            //Interpolate position

            if (GameManager.Instance.Origin != null)
            {
                //First Extrapolate from previous position
                float distanceLat = Mathf.Abs((float)Info.Position.Latitude - (float)GameManager.Instance.Origin.Latitude);
                float distanceLng = Mathf.Abs((float)Info.Position.Longitude - (float)GameManager.Instance.Origin.Longitude);

                if (distanceLat < 1 && distanceLng < 1)
                {


                    //calculate distance and bearing
                    //Coordinate origin = new Coordinate((float)selectedGroupMainUnit.Info.Position.Latitude, (float)selectedGroupMainUnit.Info.Position.Longitude);
                    Coordinate position = new Coordinate((float)Info.Position.Latitude, (float)Info.Position.Longitude);

                    Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                    Vector3 worldPos = new Vector3(coord.Longitude, (float)Info.Position.HeightOverSeaLevelM, coord.Latitude);

                    //Debug.Log(worldPos);
                    //worldPos += selectedGroupMainUnit.transform.position;

                    if (Info.DomainType == GameConstants.DomainType.Air)
                    {
                        float heightOveSeaLevelM = (float)Info.Position.HeightOverSeaLevelM;
                        if (heightOveSeaLevelM <= 101)
                        {
                            worldPos.y = (float)Info.Position.HeightOverSeaLevelM;
                        }
                        else
                        {
                            worldPos.y = ((heightOveSeaLevelM / GameManager.Instance.MaxHeightInM) * 1400) + 100;
                        }

                    }
                    else
                    {
                        worldPos.y = transform.position.y > -10 ? transform.position.y : 0;
                    }

                    if (!InRepository)
                    {
                        transform.position = Vector3.Lerp(transform.position, worldPos, 0.9f);
                    }
                    else
                    {
                        transform.position = worldPos;
                    }

                   
                    //}
                    //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                    InRepository = false;
                    //renderer.enabled = true;

                }
                else
                {
                    if (GameManager.Instance.UnitManager.SelectedEnemyUnit == this)
                    {
                        GameManager.Instance.Origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                    }
                    else
                    {
                        InRepository = true;
                        transform.position = GameManager.Instance.UnitRepository;
                    }
                    //renderer.enabled = false;

                }
            }
            else
            {
                InRepository = true;
                //renderer.enabled = false;
                transform.position = GameManager.Instance.UnitRepository;
            }

        }
    }

    public PositionInfo Position
    {
        get
        {
            return Info.Position;
        }
        set
        {
            //_PositionInfo = value;
            Info.Position = value;

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
                    
                    if ( !InRepository )
                    {
                        transform.position = Vector3.Slerp(transform.position, worldPos, Time.deltaTime);
                    }
                    else
                    {
                        transform.position = worldPos;
                    }

                    InRepository = false;
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
                    }

                }
            }
            else
            {
                InRepository = true;
                transform.position = GameManager.Instance.UnitRepository;
            }
        }
    }

    public void SpawnExplosion()
    {
        GameObject go = Instantiate(GameManager.Instance.SurfaceExplosion, transform.position, Quaternion.identity) as GameObject;
        //go.transform.parent = this.transform;
    }

    void Start()
    {
        _SateliteCamera = GameManager.Instance.CameraManager.SateliteCamera;
        //~ _Map = GameObject.Find("Map").transform;
        _VesselTexture = GameManager.Instance.GUIManager.GetTextureByUnitClassId("enemyDetection");
        
    }

    void OnGUI()
    {

        
        if (GUIVisible)
        {
            //if (GUI.Button(_Pos, _VesselTexture, "Label"))
            //{
            //    if (Event.current.button == 1)
            //    {
            //        _ShowAttackMenu = !_ShowAttackMenu;
            //        //Attack it
            //    }
            //}
            //        InteractionWindow iw = GameObject.Find("GUI").GetComponent<InteractionWindow>();
            //        if (iw.AwaitingOrder)
            //        {
            //            //Send back order to attack with unlaunch aircrafts
            //            iw.Attack();
                      
            //        }
            //        Event.current.Use();

            //    }
            //    else
            //    {
            //        //~ GameManager.Instance.UnitManager.SelectedUnit = _PUnit;
            //        //(_SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = new Vector3(_WorldPos.x, _SateliteCamera.transform.position.y, _WorldPos.z);
            //        GameManager.Instance.UnitManager.SelectedEnemyUnit = this;
            //    }

            //}
        }

        //if (_ShowAttackMenu)
        //{
        //    //TODO: Clamp box to screen
        //    Rect boxRect = new Rect(_Pos.left, _Pos.yMax, 100, 100);
        //    GUI.Box(boxRect, "Attack Menu");
        //    GUILayout.BeginArea(boxRect);
        //    GUILayout.BeginVertical();

        //    if (GUILayout.Button("Default Attack"))
        //    {
        //        GameManager.Instance.OrderManager.Attack(GameManager.Instance.UnitManager.SelectedUnit.Info, Info, GameConstants.EngagementOrderType.EngageNotClose, null, 0);
        //    }
        //    if (GUILayout.Button("Weak Attack"))
        //    {
        //        Debug.Log("Weak");
        //    }
        //    if (GUILayout.Button("Strong Attack"))
        //    {
        //        Debug.Log("Strong");
        //    }
        //    if (GUILayout.Button("Custom Attack"))
        //    {
        //        //~ Debug.Log("Custom");
        //        GameObject gui = GameObject.Find("GUI");

        //        CustomAttackWindow caw = gui.GetComponent(typeof(CustomAttackWindow)) as CustomAttackWindow;
        //        if (caw != null)
        //        {
        //            caw.Target = this.Info;
        //            caw.SelectedWeapon = null;
        //            caw.enabled = true;
        //            Vector2 mouse = Input.mousePosition;
        //            caw.WindowRect.x = mouse.x;
        //            caw.WindowRect.y = mouse.y;
        //        }
        //    }

        //    GUILayout.EndVertical();
        //    GUILayout.EndArea();
        //}
    }

    void Update()
    {

        if (_Info != null)
        {

            _WorldPos.x = ((float)_Info.Position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier) + GameManager.Instance.XMapAddition;
            _WorldPos.z = ((float)_Info.Position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier) + GameManager.Instance.YMapAddtion;
            _WorldPos.y = 30000;


            GUIScreenPos = _SateliteCamera.WorldToScreenPoint(_WorldPos);

            _Pos = new Rect(GUIScreenPos.x - Size.x / 2, _SateliteCamera.pixelHeight - GUIScreenPos.y - Size.y / 2, Size.x, Size.x);


            GUIVisible = false;
            if (GameManager.Instance.CameraManager.SateliteCamera.rect.height != 1)
            {
                _Pos.y += Screen.height / 2;
                if (_Pos.yMin < Screen.height / 2 && _Pos.xMax < Screen.height / 2)
                {
                    GUIVisible = true;
                }
            }
            else
            {
                if (MathHelper.ViewportRectToScreenRect(GameManager.Instance.CameraManager.SateliteCamera.rect).Contains(new Vector2(_Pos.x, _Pos.y)))
                {
                    GUIVisible = true;
                }
                else
                {
                    GUIVisible = false;
                }
            }
            GUIPos = _Pos;
            //Move unit




            transform.eulerAngles = new Vector3(0, Bearing, 0);


            if ( Info.DomainType == GameConstants.DomainType.Air )
            {
                Vector3 worldPos = transform.position;
                float heightOveSeaLevelM = ( float ) Info.Position.HeightOverSeaLevelM;
                if ( heightOveSeaLevelM <= 101 )
                {
                    worldPos.y = ( float ) Info.Position.HeightOverSeaLevelM;
                }
                else
                {
                    worldPos.y = ( ( heightOveSeaLevelM / GameManager.Instance.MaxHeightInM ) * 5000 ) + 100;
                }

                float angle = 0;
                float yDiff = worldPos.y - transform.position.y;

                angle = worldPos.y > transform.position.y ? 1 : -1;

                //euler.x = Mathf.Clamp(changeValue, -70, 70);

                transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, euler, Time.deltaTime); ;


                //float tiltAroundX = angle;
                //XAngle = angle;
                //transform.Rotate(new Vector3(angle, 0, 0), Space.Self);


            }
            else
            {

            }

            if ( !InRepository )
            {
                if ( Info.DomainType != GameConstants.DomainType.Land )
                {
                    float delta = Time.deltaTime;
                    float speedInKph = ( float ) Info.Position.ActualSpeedKph;
                    float kmPerSec = speedInKph / 3600;
                    float mPerSec = kmPerSec * 1000;
                    float mPerFrame = mPerSec * delta;

                    //if (Info.IsGroupMainUnit && Info.UnitName == "Nimitz")
                    //{
                    //rigidbody.AddForce(new Vector3(0, 0, mPerFrame), ForceMode.VelocityChange);
                    //rigidbody.velocity = new Vector3(0, 0, mPerFrame);
                    transform.Translate(new Vector3(0, 0, mPerFrame * GameManager.Instance.GlobalScale.z * ( float ) GameManager.Instance.GameInfo.RealTimeCompressionFactor), Space.Self);
                    //Debug.Log(string.Format("delta:{0} - speedinKph:{1} - kilometers per Sec: {2} - meters per sec:{3} - meters per frame:{4} - kmTraveled:{5}", delta, speedInKph, kmPerSec, mPerSec, mPerFrame, kmTraveledInFrame));
                    //}


                    //WantedPos += new Vector3(0, 0, mPerFrame * GameManager.Instance.GlobalScale.z * ( float ) GameManager.Instance.GameInfo.RealTimeCompressionFactor);
                    //Vector3 diff = transform.position - WantedPos;
                    //transform.Translate(diff * Time.deltaTime, Space.World);
                    //if ( Info.UnitName.ToLower() == "nimitz" )
                    //{
                        //Debug.Log(string.Format("Difference = {0}", diff));
                    //}
                }

            }
            else
            {
                transform.position = GameManager.Instance.UnitRepository;
            }

            if ( GameManager.Instance.Origin != null )
            {
                //First Extrapolate from previous position
                float distanceLat = Mathf.Abs(( float ) Info.Position.Latitude - ( float ) GameManager.Instance.Origin.Latitude);
                float distanceLng = Mathf.Abs(( float ) Info.Position.Longitude - ( float ) GameManager.Instance.Origin.Longitude);

                if ( distanceLat < 1 && distanceLng < 1 )
                {
                    //calculate distance and bearing
                    //Coordinate origin = new Coordinate((float)selectedGroupMainUnit.Info.Position.Latitude, (float)selectedGroupMainUnit.Info.Position.Longitude);
                    Coordinate position = new Coordinate(( float ) Info.Position.Latitude, ( float ) Info.Position.Longitude);

                    Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                    Vector3 worldPos = new Vector3(coord.Longitude, ( float ) Info.Position.HeightOverSeaLevelM, coord.Latitude);

                    //Debug.Log(worldPos);
                    //worldPos += selectedGroupMainUnit.transform.position;
                    if ( Info.DomainType == GameConstants.DomainType.Air )
                    {
                        worldPos.y = ( float ) Info.Position.HeightOverSeaLevelM;
                    }
                    else
                    {
                        worldPos.y = transform.position.y != GameManager.Instance.UnitRepository.y ? transform.position.y : 0;
                    }
                    if ( !InRepository )
                    {
                        transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                    }
                    else
                    {
                        transform.position = worldPos;
                    }

                    //}
                    //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                    InRepository = false;
                }
                else
                {
                    InRepository = true;
                    transform.position = GameManager.Instance.UnitRepository;
                }
            }
            else
            {
                InRepository = true;
                transform.position = GameManager.Instance.UnitRepository;

            }
            

            
            //gameObject.layer = InRepository ? 9 : 0;
        }




    }

    public void Kill()
    {
        GameManager.Instance.GUIManager.RemoveFromGroup(this);
        GameManager.Instance.UnitManager.RemoveEnemy(this);
        Destroy(MapUnit.gameObject);
        Destroy(this.gameObject);
    }
}
