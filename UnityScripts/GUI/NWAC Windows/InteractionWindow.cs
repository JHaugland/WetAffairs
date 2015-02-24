using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using System;
using System.Text;

public class InteractionWindow : MonoBehaviour
{

    #region Privates and enums

    private PlayerUnit _SelectedUnit;
    private List<PlayerUnit> _SelectedFormationUnits;

    private UnitClass _SelectedCarriedUnitClass;
    private List<UnitClass> _CarriedUnitClasses;

    private bool _InfoDetailsShowing = true;
    private bool _SensorsShowing = true;
    private bool _LoadOutsShowing = false;
    private Vector2 _SpecificationScrollPosition = Vector2.zero;
    private Vector2 _LaunchAircraftScrollPosition = Vector2.zero;
    private Vector2 _AircraftInHangarScrollPosition = Vector2.zero;
    private Vector2 _AircraftScrollPosition = Vector2.zero;
    private Vector2 _LoadOutsScrollPosition = Vector2.zero;
    private Vector2 _LoadOutsOptionsScrollPosition = Vector2.zero;
    private Vector2 _MessageReadScrollPosition = Vector2.zero;
    private bool _ShowAircraftHangar = false;

    private SensorInfo _SelectedSensor;
    private WeaponInfo _SelectedWeapon;
    private CarriedUnit _SelectedCarriedUnit;
    private UnitClassWeaponLoads _CurrentLoadOut;
    private UnitClassWeaponLoads _SelectedLoadOut;
    private int _SelectedLoadOutIndex = 0;

    private GameConstants.UnitSpeedType _CurrentUnitSpeedType;
    private int _SelectedUnitSpeedType;
    private List<string> _SpeedTypeStrings;

    public float MinFormationZoomInM = 1000;
    public float MaxFormationZoomInM = 100000;

    private float _FormationZoomLevelInM = 10000;
    private GameConstants.HeightDepthPoints _CurrentHeightDepthPoints;
    private List<string> _HeightDepthPointStrings;
    private int _CurrentHeightDepthIndex;

    private List<string> _WeaponLoadsNames;
    private List<GUIContent> _WeaponLoadsDescriptions;


    private string _LatitudeMouse = "0";
    private string _LongitudeMouse = "0";
    //private bool _AwaitingMovementOrder = false;
    //private bool _OrderGiven = false;

    private Vector3 _DestinationVector = Vector3.zero;
    private Vector3 _HitPoint = Vector3.zero;

    private bool _AwaitingOrder = false;
    private bool _UnitSpeedTypeComboboxOpen = false;
    private Vector2 _UnitSpeedTypeComboboxScroller = Vector2.zero;

    private bool _HeightDepthComboboxOpen = false;
    private Vector2 _HeightDepthComboboxScroller = Vector2.zero;

    private List<CarriedUnit> _UnitsCarried;



    public enum SizeState
    {
        Collapsed,
        Mini,
        Half,
        Full
    }

    public enum GUIState
    {
        Info,
        Movement,
        Sensors,
        Weapons,
        Formation,
        AircraftHangar

    }

    #endregion

    #region Public variables and Properties

    public SizeState _SizeState = SizeState.Collapsed;
    public GUIState _GUIState = GUIState.Info;
    public SizeState LastSize = SizeState.Collapsed;

    public SizeState MySizeState
    {
        get
        {
            return _SizeState;
        }
        set
        {
            _SizeState = value;
        }

    }

    public GUIState MyGUIState
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

    #region CommsPanel

    private Vector2 _ScrollPosition = new Vector2(0, 0);
    private Vector2 _MessageScrollPosition = new Vector2(0, 0);
    private bool _Expanded = false;
    private string _MessageToDisplay = "";
    private bool _ShowWeatherString;

    public float ItemSize = 600;
    public float MessageSize = 20;
    public float ChannelButtonSize = 150;
    public GUIStyle ButtonAsLabel;

    public bool SetBottomCenter = true;

    public GUIStyle BattleChannelStyle;
    public GUIStyle DetectionChannelStyle;
    public GUIStyle GameChannelStyle;
    public GUIStyle AllChannelStyle;
    public GUIStyle ActiveChannelStyle;
    public GUIStyle ArchiveChannelStyle;


    public Channels CurrentChannel = Channels.All;

    public enum Channels
    {
        Battle,
        Detection,
        Game,
        All,
        Archive

    }

    #endregion



    public Camera SurfaceCamera;
    public Camera SateliteCamera;

    public int WindowId;
    public Rect WindowRect;
    public GUISkin Skin;
    public GUISkin TestSkin;

    public ScreenSplitController SplitController;
    public NWACWindow ControlWindow;
    public float HalfSizeWidth;
    public float HalfSizeHeight;
    public GUIStyle Separator;
    public GUIStyle ButtonGroup1Style;
    public float ButtonGroupSize = 32;
    public float HangarWidth = 200;
    public float HangarHeightMod = 200;

    public GUIStyle InfoButtonStyle;
    public GUIStyle MovementButtonStyle;
    public GUIStyle SensorsButtonStyle;
    public GUIStyle WeaponsButtonStyle;
    public GUIStyle AircraftButtonStyle;
    public GUIStyle FormationButtonStyle;
    public GUIStyle TimeCompressionButtonStyle;
    public GUIStyle WeatherPanelStyle;
    public GUIStyle WeatherInfoStyle;
    public GUIStyle MessageBoxStyle;
    public GUIStyle MessageListScrollStyle;

    public GUIStyle ReadMessageStyle;
    public GUIStyle ArchiveButtonStyle;
    public GUIStyle HangarStyle;

    public GUIStyle FormationObjectNotSelected;
    public GUIStyle FormationObjectSelected;
    public GUIStyle WeatherBackgroundOne;
    public GUIStyle WeatherBackgroundTwo;

    public GUIContent InfoContent;
    public GUIContent MovementContent;
    public GUIContent SensorContent;
    public GUIContent WeaponContent;
    public GUIContent AircraftContent;


    public GUIStyle WindowStyle;
    public Texture2D PlaceHolderUnitDBTex;
    public List<StackMessage> StackMessages;

    //Reference will not change and thus it is safe to only assign once.
    public AudioManager TheAudioManager;

    #endregion

    public PlayerUnit SelectedUnit
    {
        get
        {

            return _SelectedUnit;
        }
    }

    public void OnSelectedUnitChanged(PlayerUnit unit)
    {
        if ( unit.Info == null )
        {
            unit = null;
        }
        _SelectedUnit = unit;
        if ( _SelectedUnit != null )
        {
            _UnitsCarried = _SelectedUnit.CarriedUnits;

            _CarriedUnitClasses = new List<UnitClass>();
            foreach ( CarriedUnit carriedUnit in _UnitsCarried )
            {
                _CarriedUnitClasses.Add(GameManager.Instance.GetUnitClass(carriedUnit.UnitInfo.UnitClassId));
            }
        }
        else
        {
            _UnitsCarried = new List<CarriedUnit>();
        }
        _CurrentHeightDepthPoints = unit.Info.Position.HeightDepth;

        _HeightDepthPointStrings = new List<string>();
        foreach ( GameConstants.HeightDepthPoints hdp in unit.Info.SupportedDepthHeight )
        {
            _HeightDepthPointStrings.Add(hdp.ToString());
        }
        _CurrentHeightDepthIndex = _HeightDepthPointStrings.FindIndex(delegate(string s) { return s == _CurrentHeightDepthPoints.ToString(); });
        _SelectedSensor = null;
        _SelectedWeapon = null;
        _CurrentUnitSpeedType = unit.Info.ActualSpeed;
        _SpeedTypeStrings = new List<string>();
        foreach ( GameConstants.UnitSpeedType ust in unit.Info.SupportedSpeedTypes )
        {
            _SpeedTypeStrings.Add(ust.ToString());
        }
        _SelectedUnitSpeedType = _SpeedTypeStrings.FindIndex(delegate(string s) { return s == _CurrentUnitSpeedType.ToString(); });

    }

    public bool AwaitingOrder
    {
        get
        {
            return _AwaitingOrder;
        }
    }


    // Use this for initialization
    void Start()
    {
        _SelectedFormationUnits = new List<PlayerUnit>();
        StackMessages = new List<StackMessage>();
        _WeaponLoadsNames = new List<string>();
        _WeaponLoadsDescriptions = new List<GUIContent>();

        TheAudioManager = GameManager.Instance.AudioManager;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.L) )
        {
            _GUIState = GUIState.Formation;
        }
        foreach ( StackMessage sm in StackMessages )
        {
            sm.Update();
        }
        if ( StackMessages.Count > 0 )
        {
            StackMessages.RemoveAll(delegate(StackMessage sm) { return sm.Dead; });
        }

    }


    void OnGUI()
    {
        GUI.skin = Skin;

        switch ( _SizeState )
        {
            case SizeState.Collapsed:
                WindowRect = new Rect(0, 0, 200, 50);
                break;
            case SizeState.Mini:
                WindowRect = new Rect(0, 0, HalfSizeWidth, HalfSizeHeight);
                break;
            case SizeState.Half:
                WindowRect = new Rect(0, Screen.height / 2, Screen.height / 2, Screen.height / 2);
                break;
            case SizeState.Full:
                float cWidth = ControlWindow.WindowRect.width;
                WindowRect = new Rect(cWidth, 0, Screen.width, Screen.height);
                WindowRect.x = cWidth;
                WindowRect.width -= cWidth;

                break;
            default:
                break;
        }
        GUI.DrawTexture(WindowRect, GameManager.Instance.GUIManager.MessageBackground2);



        #region MissileButtons

        //Test

        if ( GameManager.Instance.UnitManager.Missiles.Count > 0 )
        {
            GUILayout.BeginArea(new Rect(0, 0, 132, Screen.height / 2));

            GUILayout.BeginVertical();

            foreach ( PlayerUnit pUnit in GameManager.Instance.UnitManager.Missiles )
            {
                if ( GUILayout.Button(pUnit.Info.Id) )
                {
                    GameManager.Instance.CameraManager.Target = pUnit.transform;
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }


        //End test

        #endregion

        //WindowRect = GUI.Window(WindowId, WindowRect, WindowFunc, "");
        GUILayout.BeginArea(WindowRect);

        #region Window




        #region Buttons
        GUILayout.BeginHorizontal();


        if ( GUILayout.Button(InfoContent, InfoButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.MaxHeight(ButtonGroupSize)) )
        {
            _GUIState = GUIState.Info;
            TheAudioManager.PlayClickButtonSound();
        }
        //if (GUILayout.Button(MovementContent, MovementButtonStyle, GUILayout.MaxWidth(32), GUILayout.MaxHeight(32)))
        //{
        //    _GUIState = GUIState.Movement;
        //}
        if ( GUILayout.Button(SensorContent, SensorsButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.MaxHeight(ButtonGroupSize)) )
        {
            _GUIState = GUIState.Sensors;
        }
        //if (GUILayout.Button(WeaponContent, WeaponsButtonStyle, GUILayout.MaxWidth(32), GUILayout.MaxHeight(32)))
        //{
        //    _GUIState = GUIState.Weapons;
        //}
        if ( GUILayout.Button(AircraftContent, AircraftButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.MaxHeight(ButtonGroupSize)) )
        {
            _GUIState = GUIState.AircraftHangar;
        }
        //if (GUILayout.Button("", FormationButtonStyle, GUILayout.MaxWidth(32), GUILayout.MaxHeight(32)))
        //{
        //    _GUIState = GUIState.Formation;
        //}
        if ( _SelectedUnit != null && GameManager.Instance.GameInfo != null )
        {
            if ( GUILayout.Button(new GUIContent("1x", "Set time compression to 1(real time)"), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
            {
                GameManager.Instance.OrderManager.SetTimeCompression(1);
            }
            if ( GUILayout.Button(new GUIContent("2x", "Set time compression to twice as fast as real time"), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
            {
                GameManager.Instance.OrderManager.SetTimeCompression(2);
            }
            if ( GUILayout.Button(new GUIContent("4x", "Set Timecompression to 4"), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
            {
                GameManager.Instance.OrderManager.SetTimeCompression(4);
            }
            if ( GUILayout.Button(new GUIContent("10x", "Set Timecompression to 10(fast)"), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
            {
                GameManager.Instance.OrderManager.SetTimeCompression(10);
            }
            if ( GUILayout.Button(new GUIContent("20x", "Set Timecompression to 20(really fast)"), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
            {
                GameManager.Instance.OrderManager.SetTimeCompression(20);
            }

            //int timeCompression = Mathf.FloorToInt(GUILayout.HorizontalSlider((float)GameManager.Instance.GameInfo.RealTimeCompressionFactor, 1, 20));
            //if (GUI.changed)
            //{
            //    Debug.Log("new TimeCompression = " + timeCompression);
            //}

        }
        if ( _SelectedUnit != null )
        {
            if ( _SelectedUnit.UnitClass.IsAircraft )
            {
                if ( GUILayout.Button(new GUIContent("RTB", "Return to base."), TimeCompressionButtonStyle, GUILayout.MaxWidth(ButtonGroupSize), GUILayout.Height(ButtonGroupSize)) )
                {
                    GameManager.Instance.OrderManager.ReturnToBase(_SelectedUnit);
                }
            }
        }

        GUILayout.EndHorizontal();

        #endregion
        GUILayout.Space(5);
        GUISkin old = GUI.skin;

        switch ( _GUIState )
        {
            #region Info
            case GUIState.Info:

                GUI.skin = TestSkin;
                if ( _SizeState == SizeState.Half || _SizeState == SizeState.Mini )
                {

                    GUILayout.BeginHorizontal();

                    if ( GUILayout.Button(new GUIContent("Info", "Info concerning the specific unit chosen")) )
                    {
                        _InfoDetailsShowing = true;
                    }
                    if ( GUILayout.Button(new GUIContent("Specification", "Specification for the Unit class chosen")) )
                    {
                        _InfoDetailsShowing = false;
                    }
                    GUILayout.EndHorizontal();



                    if ( _InfoDetailsShowing )
                    {
                        switch ( GameManager.Instance.UnitManager.CurrentWatchMode )
                        {
                            case UnitManager.UnitToWatch.Player:
                                if ( _SelectedUnit != null )
                                {
                                    GUILayout.BeginVertical();
                                    GUILayout.Label(_SelectedUnit.Info.UnitName);
                                    GUILayout.Label(string.Format("Class Name:\t\t {0}", _SelectedUnit.Info.UnitClassId));
                                    GUILayout.Label(string.Format("Latitude:\t\t\t {0}", _SelectedUnit.Latitude));
                                    GUILayout.Label(string.Format("Longitude:\t\t\t {0}", _SelectedUnit.Longitude));
                                    GUILayout.Label(string.Format("Course:\t\t\t\t {0}", _SelectedUnit.Info.Position.BearingDeg.ToString("F")));
                                    GUILayout.Label(string.Format("Speed:\t\t\t\t {0}", _SelectedUnit.Info.ActualSpeedKph));
                                    string unitLength = "";
                                    float distanceToNextWaypoint = MathHelper.MetersToUnitOfLength(( float ) _SelectedUnit.Info.Position.MostRecentDistanceToTargetM, GameManager.Instance.UnitLength, out unitLength);
                                    GUILayout.Label(string.Format("Distance to next waypoint:\t\t {0}{1}", distanceToNextWaypoint, unitLength));
                                    TimeSpan etaCurrent = _SelectedUnit.Info.Position.EtaCurrentWaypoint;
                                    GUILayout.Label(string.Format("ETA:\t\t\t\t {0:00}:{1:00}:{2:00}:{3:00}", etaCurrent.Days, etaCurrent.Hours, etaCurrent.Minutes, etaCurrent.Seconds));
                                    GUILayout.Label(string.Format("Height over sea:\t\t\t {0}", _SelectedUnit.Info.Position.HeightOverSeaLevelM.ToString("F")));
                                    GUILayout.Label(string.Format("Hitpoints: \t\t\t {0}", _SelectedUnit.Info.HitPoints));
                                    GUILayout.Label(string.Format("Fuel:\t\t\t\t\t {0}", _SelectedUnit.Info.CalculatedMaxRangeCruiseM));
                                    GUILayout.Label(string.Format("Bingo Fuel:\t\t\t\t\t {0}%", _SelectedUnit.Info.Position.BingoFuelPercent));
                                    GUILayout.Label(string.Format("Seastate:\t\t\t {0}", _SelectedUnit.Info.WeatherSystem.SeaState));
                                    GUILayout.Label(string.Format("Wind speed:\t\t {0} m/sec", _SelectedUnit.Info.WeatherSystem.WindSpeedMSec));
                                    GUILayout.Label(string.Format("Cloud cover:\t\t {0}", _SelectedUnit.Info.WeatherSystem.CloudCover8ths));
                                    GUILayout.Label(string.Format("Precipition:\t\t\t {0} {1}", _SelectedUnit.Info.WeatherSystem.PrecipitationType.ToString(),
                                                                                           _SelectedUnit.Info.WeatherSystem.Precipitation.ToString()));
                                    GUILayout.EndVertical();
                                }
                                break;
                            case UnitManager.UnitToWatch.Ally:
                                break;
                            case UnitManager.UnitToWatch.Enemy:
                                
                                Enemy enemy = GameManager.Instance.UnitManager.SelectedEnemyUnit;
                                if ( enemy != null )
                                {
                                    Color contentColor = GUI.contentColor;
                                    GUI.contentColor = Color.red;

                                    GUILayout.BeginVertical();
                                    GUILayout.Label(string.Format("Detection Classification:\t\t {0}", enemy.Info.DetectionClassification));
                                    GUILayout.Label(string.Format("Domain Type:\t\t\t {0}", enemy.Info.DomainType));
                                    GUILayout.Label(string.Format("Friend or foe Classification:\t\t\t {0}", enemy.Info.FriendOrFoeClassification));
                                    GUILayout.Label(string.Format("Known Damage Percent:\t\t\t\t {0}", enemy.Info.KnownDamagePercent));
                                    GUILayout.Label(string.Format("Threat Classification:\t\t\t\t {0}", enemy.Info.ThreatClassification));
                                    GUILayout.Label(string.Format("Uncertainty Range in meters: \t\t\t {0}", enemy.Info.UncertaintyRangeM));
                                    GUILayout.Label(string.Format("Known damage percent: \t\t\t {0}", enemy.Info.KnownDamagePercent));
                                    GUILayout.EndVertical();

                                    GUI.contentColor = contentColor;
                                }
                                break;
                            default:
                                break;
                        }
                        if ( _SelectedUnit != null && GameManager.Instance.UnitManager.SelectedEnemyUnit == null )
                        {
                            
                        }
                        else if ( GameManager.Instance.UnitManager.SelectedEnemyUnit != null )
                        {
                            
                        }
                    }
                    else
                    {
                        if ( SelectedUnit != null )
                        {
                            if ( SelectedUnit.UnitClass != null )
                            {
                                _SpecificationScrollPosition = GUILayout.BeginScrollView(_SpecificationScrollPosition);

                                GUILayout.BeginVertical();
                                GUILayout.Label(string.Format("Unit Class Name:\t\t {0}", SelectedUnit.UnitClass.UnitClassLongName));
                                GUILayout.Label(string.Format("Type:\t\t {0}", SelectedUnit.UnitClass.UnitType.ToString()));
                                GUILayout.Label(string.Format("Crew:\t\t\t {0}", SelectedUnit.UnitClass.CrewTotal));
                                GUILayout.Label(string.Format("Radar cross section:\t\t\t {0}", SelectedUnit.UnitClass.RadarCrossSectionSize));
                                GUILayout.Label(string.Format("Esm shielded:\t\t\t\t {0}", SelectedUnit.UnitClass.IsEsmShielded == true ? "Yes" : "No"));
                                GUILayout.Label(string.Format("Propulsion:\t\t\t\t {0}", SelectedUnit.UnitClass.PropulsionSystem.ToString()));
                                GUILayout.Label(string.Format("Country: \t\t\t {0}", SelectedUnit.UnitClass.CountryId));
                                GUILayout.Label(string.Format("Total mass empty kilograms:\t\t\t\t\t {0}", SelectedUnit.UnitClass.TotalMassEmptyKg));
                                GUILayout.Label(string.Format("Total mass loaded kilograms:\t\t\t\t\t {0}", SelectedUnit.UnitClass.TotalMassLoadedKg));
                                GUILayout.Label(string.Format("Length meters:\t\t\t {0}", SelectedUnit.UnitClass.LengthM));
                                GUILayout.Label(string.Format("Width meters:\t\t {0}", SelectedUnit.UnitClass.HeightM));
                                GUILayout.Label(string.Format("Draft meters:\t\t {0}", SelectedUnit.UnitClass.DraftM));
                                GUILayout.Label(string.Format("Max speed Kph:\t\t\t {0} ", SelectedUnit.UnitClass.MaxSpeedKph));
                                GUILayout.Label(string.Format("Cruise speed Kph:\t\t\t {0} ", SelectedUnit.UnitClass.CruiseSpeedKph));
                                GUILayout.Label(string.Format("Max acceleration speed Kph/sec:\t\t\t {0} ", SelectedUnit.UnitClass.MaxAccelerationKphSec.ToString()));

                                GUILayout.Label(string.Format("Operational units:\t\t\t "));
                                foreach ( UnitClassVesselName ucvn in SelectedUnit.UnitClass.VesselNames )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", ucvn.UnitName));
                                }

                                GUILayout.Label(string.Format("Weapon loads::\t\t "));
                                foreach ( UnitClassWeaponLoads ucwl in SelectedUnit.UnitClass.WeaponLoads )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", ucwl.Name));
                                }

                                GUILayout.Label(string.Format("Sensor platforms::\t\t "));
                                foreach ( string sensor in SelectedUnit.UnitClass.SensorClassIdList )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", sensor));
                                }

                                GUILayout.Label(string.Format("Roles::\t\t "));
                                foreach ( GameConstants.Role roles in SelectedUnit.UnitClass.UnitRoles )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", roles.ToString()));
                                }

                                GUILayout.Label(string.Format("Carried aircraft::\t\t "));
                                foreach ( UnitClassStorage unitclassStorage in SelectedUnit.UnitClass.CarriedUnitClassses )
                                {
                                    UnitClass u = GameManager.Instance.GetUnitClass(unitclassStorage.UnitClassId);

                                    GUILayout.Label(string.Format("\t{0}({1}) ", u.UnitClassLongName, unitclassStorage.NumberOfUnits));
                                }
                                GUILayout.EndVertical();
                                GUILayout.EndScrollView();
                            }
                        }
                        else if ( GameManager.Instance.UnitManager.SelectedEnemyUnit != null )
                        {
                            Enemy enemy = GameManager.Instance.UnitManager.SelectedEnemyUnit;
                            GUILayout.Label("BE AWARE THAT THIS IS ONLY INTELLIGENCE AND MAY CONTAIN FAULTY INFORMATION");
                            GUILayout.Space(10);

                            UnitClass enemyUnitClass = GameManager.Instance.GetUnitClass(enemy.Info.RefersToUnitClassId);
                            if ( enemyUnitClass != null )
                            {
                                _SpecificationScrollPosition = GUILayout.BeginScrollView(_SpecificationScrollPosition);

                                GUILayout.BeginVertical();
                                GUILayout.Label(string.Format("Unit Class Name:\t\t {0}", enemyUnitClass.UnitClassLongName));
                                GUILayout.Label(string.Format("Type:\t\t {0}", enemyUnitClass.UnitType.ToString()));
                                GUILayout.Label(string.Format("Crew:\t\t\t {0}", enemyUnitClass.CrewTotal));
                                GUILayout.Label(string.Format("Radar cross section:\t\t\t {0}", enemyUnitClass.RadarCrossSectionSize));
                                GUILayout.Label(string.Format("Esm shielded:\t\t\t\t {0}", enemyUnitClass.IsEsmShielded == true ? "Yes" : "No"));
                                GUILayout.Label(string.Format("Propulsion:\t\t\t\t {0}", enemyUnitClass.PropulsionSystem.ToString()));
                                GUILayout.Label(string.Format("Country: \t\t\t {0}", enemyUnitClass.CountryId));
                                GUILayout.Label(string.Format("Total mass empty kilograms:\t\t\t\t\t {0}", enemyUnitClass.TotalMassEmptyKg));
                                GUILayout.Label(string.Format("Total mass loaded kilograms:\t\t\t\t\t {0}", enemyUnitClass.TotalMassLoadedKg));
                                GUILayout.Label(string.Format("Length meters:\t\t\t {0}", enemyUnitClass.LengthM));
                                GUILayout.Label(string.Format("Width meters:\t\t {0}", enemyUnitClass.HeightM));
                                GUILayout.Label(string.Format("Draft meters:\t\t {0}", enemyUnitClass.DraftM));
                                GUILayout.Label(string.Format("Max speed Kph:\t\t\t {0} ", enemyUnitClass.MaxSpeedKph));
                                GUILayout.Label(string.Format("Cruise speed Kph:\t\t\t {0} ", enemyUnitClass.CruiseSpeedKph));
                                GUILayout.Label(string.Format("Max acceleration speed Kph/sec:\t\t\t {0} ", enemyUnitClass.MaxAccelerationKphSec.ToString()));

                                GUILayout.Label(string.Format("Operational units:\t\t\t "));
                                foreach ( UnitClassVesselName ucvn in enemyUnitClass.VesselNames )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", ucvn.UnitName));
                                }

                                GUILayout.Label(string.Format("Weapon loads::\t\t "));
                                foreach ( UnitClassWeaponLoads ucwl in enemyUnitClass.WeaponLoads )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", ucwl.Name));
                                }

                                GUILayout.Label(string.Format("Sensor platforms::\t\t "));
                                foreach ( string sensor in enemyUnitClass.SensorClassIdList )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", sensor));
                                }

                                GUILayout.Label(string.Format("Roles::\t\t "));
                                foreach ( GameConstants.Role roles in enemyUnitClass.UnitRoles )
                                {
                                    GUILayout.Label(string.Format("\t{0} ", roles.ToString()));
                                }

                                GUILayout.Label(string.Format("Carried aircraft::\t\t "));
                                foreach ( UnitClassStorage unitclassStorage in enemyUnitClass.CarriedUnitClassses )
                                {
                                    UnitClass u = GameManager.Instance.GetUnitClass(unitclassStorage.UnitClassId);

                                    GUILayout.Label(string.Format("\t{0}({1}) ", u.UnitClassLongName, unitclassStorage.NumberOfUnits));
                                }
                                GUILayout.EndVertical();
                                GUILayout.EndScrollView();
                            }
                        }
                    }
                }
                GUI.skin = old;
                break;

            #endregion

            #region Movement
            case GUIState.Movement:

                GUILayout.BeginVertical();
                GUILayout.Label(string.Format("Mouse Latitude: {0}", _LatitudeMouse));
                GUILayout.Label(string.Format("Mouse Longitude: {0}", _LongitudeMouse));
                GUILayout.BeginHorizontal();
                //GUILayout.Label(_AwaitingMovementOrder == true ? "Awaiting movement order (click point on map)..." : "Click to give movement order");
                //if (GUILayout.Button(_AwaitingMovementOrder == true ? "Cancel" : "Give movement order"))
                //{
                //    _AwaitingMovementOrder = !_AwaitingMovementOrder;
                //}
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();



                break;
            #endregion

            #region Sensors

            case GUIState.Sensors:
                if ( _SizeState == SizeState.Mini || _SizeState == SizeState.Half )
                {

                    GUILayout.BeginHorizontal();

                    if ( GUILayout.Button(new GUIContent("Sensors", "Modify sensor options for unit")) )
                    {
                        _SensorsShowing = true;
                    }
                    if ( GUILayout.Button(new GUIContent("Weapons", "Modify weapon options for unit")) )
                    {
                        _SensorsShowing = false;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.Label(_SensorsShowing == true ? "Sensors:" : "Weapons:");

                    GUILayout.Space(10);

                    if ( _SensorsShowing )
                    {
                        if ( _SelectedUnit != null )
                        {

                            foreach ( SensorInfo si in _SelectedUnit.Info.Sensors )
                            {
                                SensorClass sc = GameManager.Instance.GetSensorClass(si.SensorClassId);
                                if ( sc != null )
                                {
                                    if ( GUILayout.Button(si.Name) )
                                    {
                                        _SelectedSensor = si;
                                    }
                                    else if ( _SelectedSensor != null )
                                    {
                                        if ( _SelectedSensor.Id == si.Id )
                                        {
                                            if ( _SelectedSensor.IsActive != si.IsActive )
                                            {
                                                _SelectedSensor = si;
                                            }
                                        }
                                        //~ _SelectedSensor = si;
                                    }
                                }
                            }
                            if ( _SelectedSensor != null )
                            {
                                SensorClass sc = GameManager.Instance.GetSensorClass(_SelectedSensor.SensorClassId);


                                Separator.fixedWidth = WindowRect.width;

                                GUILayout.Label("", Separator);

                                GUILayout.BeginVertical();

                                GUILayout.Label(string.Format("Status: {0}", _SelectedSensor.IsOperational == true ? "Operational" : string.Format("Ready in : {0} seconds", _SelectedSensor.ReadyInSec)));

                                if ( sc.IsPassiveActiveSensor )
                                {
                                    if ( GUILayout.Button(_SelectedSensor.IsActive == true ? "Set passive" : "Set active") )
                                    {
                                        GameManager.Instance.OrderManager.SetSensorPassiveActive(_SelectedUnit.Info, _SelectedSensor, !_SelectedSensor.IsActive);
                                    }
                                    if ( sc.IsDeployableSensor )
                                    {
                                        if ( GUILayout.Button(_SelectedSensor.IsOperational ? "Undeploy" : "Deploy") )
                                        {
                                            //GameManager.Instance.
                                        }
                                    }
                                }

                                GUILayout.Label(_SelectedSensor.ToString());

                                GUILayout.EndVertical();
                            }
                        }
                        else
                        {
                            GUILayout.Label("Not Connected");
                        }
                    }
                    else
                    {
                        if ( _SelectedUnit != null )
                        {
                            foreach ( WeaponInfo wi in _SelectedUnit.Info.Weapons )
                            {
                                WeaponClass wc = GameManager.Instance.GetWeaponClass(wi.WeaponClassId);
                                if ( wc != null )
                                {
                                    if ( GUILayout.Button(wc.WeaponClassName) )
                                    {
                                        _SelectedWeapon = wi;
                                    }
                                }

                            }

                            if ( _SelectedWeapon != null )
                            {
                                Separator.fixedWidth = WindowRect.width;

                                GUILayout.Label("", Separator);

                                GUILayout.BeginVertical();

                                GUILayout.Label(_SelectedWeapon.ToString());
                                GUILayout.Label(string.Format("Ammo left: {0}", _SelectedWeapon.AmmunitionRemaining));
                                GUILayout.Label(string.Format("Ready in : {0} seconds", _SelectedWeapon.ReadyInSec));

                                GUILayout.EndVertical();
                            }
                        }
                        else
                        {
                            GUILayout.Label("Not Connected");
                        }
                    }

                }
                break;

            #endregion

            #region Weapons

            case GUIState.Weapons:

                if ( _SizeState == SizeState.Half || _SizeState == SizeState.Mini )
                {
                    if ( _SelectedUnit != null )
                    {
                        foreach ( WeaponInfo wi in _SelectedUnit.Info.Weapons )
                        {
                            WeaponClass wc = GameManager.Instance.GetWeaponClass(wi.WeaponClassId);
                            if ( wc != null )
                            {
                                if ( GUILayout.Button(wc.WeaponClassName) )
                                {
                                    _SelectedWeapon = wi;
                                }
                            }

                        }

                        if ( _SelectedWeapon != null )
                        {
                            Separator.fixedWidth = WindowRect.width;

                            GUILayout.Label("", Separator);

                            GUILayout.BeginVertical();

                            GUILayout.Label(_SelectedWeapon.ToString());

                            GUILayout.Label(string.Format("Ready in : {0} seconds", _SelectedWeapon.ReadyInSec));

                            GUILayout.EndVertical();
                        }
                    }
                    else
                    {
                        GUILayout.Label("Not Connected");
                    }
                }
                break;

            #endregion

            #region Formation

            case GUIState.Formation:
                if ( _SizeState == SizeState.Half || _SizeState == SizeState.Mini )
                {
                    if ( _SelectedUnit != null )
                    {


                    }
                    else
                    {
                        GUILayout.Label("Not Connected");
                    }
                }
                break;

            #endregion

            #region AircraftHangar

            case GUIState.AircraftHangar:

                if ( _SelectedUnit != null )
                {
                    if ( _SelectedUnit.UnitClass.CanCarryUnits )
                    {
                        _AircraftScrollPosition = GUILayout.BeginScrollView(_AircraftScrollPosition, GUILayout.Height(WindowRect.height / 2 - 32));
                        GUILayout.BeginVertical();
                        foreach ( CarriedUnit cui in _SelectedUnit.CarriedUnits )
                        {
                            GUILayout.BeginHorizontal();

                            if ( GUILayout.Button(string.Format("{0} - {2} - Ready:{1}", cui.UnitInfo.UnitClassName, cui.ReadyInSec, cui.LoadOut)) )
                            {
                                _SelectedCarriedUnit = cui;
                                SetSelectedCarriedUnitClass(GameManager.Instance.GetUnitClass(cui.UnitInfo.UnitClassId));
                                //find current load out
                                _CurrentLoadOut = _SelectedCarriedUnitClass.WeaponLoads.Find(delegate(UnitClassWeaponLoads loadOut) { return loadOut.Name == _SelectedCarriedUnit.UnitInfo.CurrentWeaponLoadName; });
                                _SelectedLoadOut = _SelectedCarriedUnitClass.WeaponLoads.Find(delegate(UnitClassWeaponLoads loadOut) { return loadOut != _CurrentLoadOut; });
                                //cui.Selected = true;
                            }
                            if ( cui.Ready )
                            {
                                cui.Selected = GUILayout.Toggle(cui.Selected, "");
                                if ( GUI.changed )
                                {
                                    _SelectedCarriedUnit = cui;
                                    SetSelectedCarriedUnitClass(GameManager.Instance.GetUnitClass(cui.UnitInfo.UnitClassId));




                                    //find current load out
                                    _CurrentLoadOut = _SelectedCarriedUnitClass.WeaponLoads.Find(delegate(UnitClassWeaponLoads loadOut) { return loadOut.Name == _SelectedCarriedUnit.UnitInfo.CurrentWeaponLoadName; });
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndScrollView();

                        if ( _SelectedCarriedUnitClass != null )
                        {

                            _AircraftInHangarScrollPosition = GUILayout.BeginScrollView(_AircraftInHangarScrollPosition, GUILayout.Height(WindowRect.height / 2 - 32));

                            GUILayout.BeginVertical();

                            GUILayout.Label(string.Format("Unit Class Name: {0}", _SelectedCarriedUnit.UnitInfo.UnitClassName));
                            //if (_SelectedCarriedUnit.UnitInfo.ReadyInSec == 0)
                            //{

                            //}
                            //else
                            //{
                            //    GUILayout.Label("Aircraft is refitting loadout");
                            //}

                            GUILayout.Label(string.Format("Unit Name:\t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnit.UnitInfo.UnitName));
                            GUILayout.Label(string.Format("Max Operating Distance meters:\t\t\t\t {0}", _SelectedCarriedUnit.UnitInfo.MaxOperatingDistanceM.ToString("F")));
                            GUILayout.Label(string.Format("Max Operating Range meters:\t\t\t\t\t {0}", _SelectedCarriedUnit.UnitInfo.MaxOperatingRangeM.ToString("F")));
                            GUILayout.Label(string.Format("Aircraft Docking Status:\t\t\t\t\t\t\t {0}", _SelectedCarriedUnit.UnitInfo.AircraftDockingStatus.ToString()));
                            GUILayout.Label(string.Format("Hitpoints:\t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnit.UnitInfo.HitPoints));
                            GUILayout.Space(10);
                            GUILayout.Label(string.Format("Ready in sec:\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnit.ReadyInSec));

                            GUILayout.Label(string.Format("Type:\t\t\t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.UnitType.ToString()));
                            GUILayout.Label(string.Format("Crew:\t\t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.CrewTotal));
                            GUILayout.Label(string.Format("Radar cross section:\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.RadarCrossSectionSize));
                            GUILayout.Label(string.Format("Esm shielded:\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.IsEsmShielded == true ? "Yes" : "No"));
                            GUILayout.Label(string.Format("Propulsion:\t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.PropulsionSystem.ToString()));
                            GUILayout.Label(string.Format("Country: \t\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.CountryId));
                            GUILayout.Label(string.Format("Total mass empty kilograms:\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.TotalMassEmptyKg));
                            GUILayout.Label(string.Format("Total mass loaded kilograms:\t\t\t\t\t {0}", _SelectedCarriedUnitClass.TotalMassLoadedKg));
                            GUILayout.Label(string.Format("Length meters:\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.LengthM));
                            GUILayout.Label(string.Format("Width meters:\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.HeightM));
                            GUILayout.Label(string.Format("Draft meters:\t\t\t\t\t\t\t\t\t\t\t {0}", _SelectedCarriedUnitClass.DraftM));
                            GUILayout.Label(string.Format("Max speed Kph:\t\t\t\t\t\t\t\t\t\t {0} ", _SelectedCarriedUnitClass.MaxSpeedKph));
                            GUILayout.Label(string.Format("Cruise speed Kph:\t\t\t\t\t\t\t\t\t {0} ", _SelectedCarriedUnitClass.CruiseSpeedKph));
                            GUILayout.Label(string.Format("Max acceleration speed Kph/sec:\t\t\t\t {0} ", _SelectedCarriedUnitClass.MaxAccelerationKphSec.ToString()));

                            GUILayout.Label(string.Format("Operational units: "));
                            foreach ( UnitClassVesselName ucvn in _SelectedCarriedUnitClass.VesselNames )
                            {
                                GUILayout.Label(string.Format("\t{0} ", ucvn.UnitName));
                            }

                            GUILayout.Label(string.Format("Possible Weapon loads: "));
                            foreach ( UnitClassWeaponLoads ucwl in _SelectedCarriedUnitClass.WeaponLoads )
                            {
                                GUILayout.Label(string.Format("\t{0} ", ucwl.Name));
                            }
                            GUILayout.Label(string.Format("Current Load out: {0}", _SelectedCarriedUnit.UnitInfo.CurrentWeaponLoadName));

                            GUILayout.Label(string.Format("Sensor platforms: "));
                            foreach ( string sensor in _SelectedCarriedUnitClass.SensorClassIdList )
                            {
                                GUILayout.Label(string.Format("\t{0} ", sensor));
                            }

                            GUILayout.Label(string.Format("Possible Roles: "));
                            foreach ( GameConstants.Role roles in _SelectedCarriedUnitClass.UnitRoles )
                            {
                                GUILayout.Label(string.Format("\t{0} ", roles.ToString()));
                            }



                            GUILayout.EndVertical();


                            GUILayout.EndScrollView();

                        }
                    }
                    else
                    {
                        _GUIState = GUIState.Info;
                    }
                }
                else
                {
                    GUILayout.Label("Not Connected");
                }

                break;

            #endregion

            default:
                break;
        }


        #endregion


        GUILayout.EndArea();

        bool mouseOverGUI = false;

        mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, WindowRect);

        #region StackMessages

        Rect stackMessageRect = new Rect(WindowRect.width, 0, Screen.width - WindowRect.width, 100);
        GUILayout.BeginArea(stackMessageRect);
        GUILayout.BeginVertical();


        foreach ( StackMessage sm in StackMessages )
        {
            if ( GUILayout.Button(sm.MessageToDisplay.ToString()) )
            {
                if ( sm.MessageToDisplay.Position != null )
                {
                    SateliteCamera.GetComponent<SateliteController>().CenterOnPosition(sm.MessageToDisplay.Position);
                }
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        #endregion



        #region Buttons

        Rect sateliteCameraRect = MathHelper.ViewportRectToScreenRect(SateliteCamera.rect);
        Rect surfaceCameraRect = MathHelper.ViewportRectToScreenRect(SurfaceCamera.rect);


        /********************** MERGE CODE 24/02-2010 *************************************/
        /************************* REMAINS UNMERGED ***************************************/
        bool isSateliteCamMain = SateliteCamera.rect.height == 1;
        float infoBoxWidth = HangarWidth;
        float infoBoxHeight = Screen.height - 200 - HangarHeightMod;

        /**************************** END MERGE CODE **************************************/



        #endregion



        #region Messaging

        float messageWindowWidth = MathHelper.ViewportRectToScreenRect(SateliteCamera.rect).width - 500;
        if ( SateliteCamera.rect.height != 1 )
        {
            messageWindowWidth = MathHelper.ViewportRectToScreenRect(SurfaceCamera.rect).width - 500;
        }

        Rect messageListRect = new Rect(WindowRect.width, Screen.height - 200, messageWindowWidth, 200);

        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, messageListRect);
        }


        #region Message List
        GUI.color = Color.white;
        GUI.Box(messageListRect, "", MessageBoxStyle);
        GUILayout.BeginArea(messageListRect);

        int battleMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Battle, true);
        int detectionMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Detection, true);
        int gameMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Game, true);

        //~ Debug.Log(gameMessageCount);
        float btnWidth = messageListRect.width / 5;
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button(new GUIContent(battleMessageCount > 0 ? string.Format("Battle ({0})", battleMessageCount) : "Battle", "Messages concerning battle"), CurrentChannel == Channels.Battle ? ActiveChannelStyle : BattleChannelStyle, GUILayout.Width(btnWidth)) ) { CurrentChannel = Channels.Battle; }
        if ( GUILayout.Button(new GUIContent(detectionMessageCount > 0 ? string.Format("Detection ({0})", detectionMessageCount) : "Detection", "Messages concerning detection"), CurrentChannel == Channels.Detection ? ActiveChannelStyle : DetectionChannelStyle, GUILayout.Width(btnWidth)) ) { CurrentChannel = Channels.Detection; }
        if ( GUILayout.Button(new GUIContent(gameMessageCount > 0 ? string.Format("Game ({0})", gameMessageCount) : "Game", "Messages concerning game stuff"), CurrentChannel == Channels.Game ? ActiveChannelStyle : GameChannelStyle, GUILayout.Width(btnWidth)) ) { CurrentChannel = Channels.Game; }

        if ( GUILayout.Button(new GUIContent("All", "All unarchived messages"), CurrentChannel == Channels.All ? ActiveChannelStyle : AllChannelStyle, GUILayout.Width(btnWidth)) ) { CurrentChannel = Channels.All; }
        if ( GUILayout.Button(new GUIContent("Archive", "Archived messages"), CurrentChannel == Channels.Archive ? ActiveChannelStyle : ArchiveChannelStyle, GUILayout.Width(btnWidth)) ) { CurrentChannel = Channels.Archive; }
        GUILayout.EndHorizontal();


        old = GUI.skin;
        GUI.skin = TestSkin;
        if ( GameManager.Instance.NetworkManager.Connected )
        {

            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, false, true);
            List<Message> Messages = new List<Message>();
            switch ( CurrentChannel )
            {
                case Channels.Battle:
                    Messages = GameManager.Instance.MessageManager.BattleMessages;
                    break;
                case Channels.Detection:
                    Messages = GameManager.Instance.MessageManager.DetectionMessages;
                    break;
                case Channels.Game:
                    Messages = GameManager.Instance.MessageManager.GameMessages;
                    break;
                case Channels.All:
                    Messages = GameManager.Instance.MessageManager.AllMessages;
                    break;
                case Channels.Archive:
                    Messages = GameManager.Instance.MessageManager.ArchiveMessages;
                    break;
                default:
                    break;
            }
            List<Message> ForDeletion = new List<Message>();
            int i = 0;
            GUILayout.BeginVertical();
            foreach ( Message m in Messages )
            {

                GUILayout.BeginHorizontal();
                if ( GUILayout.Button(m.ToString(), m.GetStyle(i), GUILayout.Width(messageListRect.width - 62), GUILayout.Height(50)) )
                {
                    _Expanded = m.HasBody;
                    _MessageToDisplay = m.Body;
                    m.IsRead = true;
                }

                //GUILayout.Space(10);
                if ( GUILayout.Button(m.MessageType != GameManager.MessageTypes.Archive ?
                    new GUIContent("", "Archive message") :
                    new GUIContent("", "Delete message (you will not be able to get it back)"),
                    m.GetArchiveDeleteButtonStyle(m.MessageType == GameManager.MessageTypes.Archive, i), GUILayout.Width(50), GUILayout.Height(50)) )
                {
                    ForDeletion.Add(m);
                }


                GUILayout.EndHorizontal();
                //GUILayout.Label("", Separator);
                i = i == 0 ? 1 : 0;
            }
            GUILayout.EndVertical();

            foreach ( Message m in ForDeletion )
            {
                if ( m.MessageType != GameManager.MessageTypes.Archive )
                {
                    GameManager.Instance.MessageManager.ArchiveMessage(m);
                }
                else
                {
                    GameManager.Instance.MessageManager.RemoveMessage(m);
                }
            }

            GUILayout.EndScrollView();






        }
        else
        {
            GUILayout.Label("Not Connected");
        }
        GUILayout.EndArea();
        GUI.skin = old;
        #endregion

        #region Read message
        //Rect messageDisplay = new Rect(WindowRect.xMax, altButtonRect.y - 200, altButtonRect.width, 200);
        //if (_MessageToDisplay != "")
        //{
        //    GUI.Box(messageDisplay, "");
        //    GUILayout.BeginArea(messageDisplay);

        //    GUILayout.Space(10);
        //    _MessageReadScrollPosition = GUILayout.BeginScrollView(_MessageReadScrollPosition);
        //    GUILayout.Label(string.Format("{0}", _MessageToDisplay), ReadMessageStyle);
        //    GUILayout.EndScrollView();
        //    if (GUILayout.Button("Close"))
        //    {
        //        _MessageToDisplay = "";
        //    }
        //    GUILayout.EndArea();
        //}

        #endregion






        #endregion

        #region Weather

        ///////////////////////////////Weather ////////////////////*****************************//////////////////////
        Rect weatherRect = new Rect(WindowRect.width, messageListRect.y - 32, 256, 32);
        if ( _SelectedUnit != null )
        {

            GUI.Box(weatherRect, "");
            GUILayout.BeginArea(weatherRect);
            GUILayout.BeginHorizontal();
            WeatherSystemInfo weather = _SelectedUnit.Info.WeatherSystem;
            GUILayout.Label(new GUIContent(string.Format("{0}°C", weather.TemperatureC), GameManager.Instance.GUIManager.Thermometer, string.Format("Temperature: {0}°C", weather.TemperatureC)), WeatherBackgroundOne, GUILayout.MaxWidth(64), GUILayout.MaxHeight(32));
            //GUILayout.Label(string.Format("{0}°C", weather.TemperatureC), WeatherPanelStyle, GUILayout.MaxHeight(32));
            GUILayout.Label(new GUIContent(string.Format("{0}m/s", weather.WindSpeedMSec), GameManager.Instance.GUIManager.WindFlag, string.Format("Windspeed: {0}m/s", weather.WindSpeedMSec)), WeatherBackgroundTwo, GUILayout.MaxWidth(64), GUILayout.MaxHeight(32));
            //GUILayout.Label(string.Format("{0}m/s", weather.WindSpeedMSec), WeatherPanelStyle, GUILayout.MaxHeight(32));
            GUILayout.Label(new GUIContent(string.Format("{0}", weather.SeaState), GameManager.Instance.GUIManager.Wave, string.Format("Seastate: {0}", weather.SeaState)), WeatherBackgroundOne, GUILayout.MaxWidth(64), GUILayout.MaxHeight(32));
            //GUILayout.Label(string.Format("{0}", weather.SeaState), WeatherPanelStyle, GUILayout.MaxWidth(32), GUILayout.MaxHeight(32));
            GUILayout.Label(new GUIContent(string.Format("{0}/8ths", weather.CloudCover8ths), GameManager.Instance.GUIManager.Clouds, string.Format("Cloud cover: {0}/8ths", weather.CloudCover8ths)), WeatherBackgroundTwo, GUILayout.MaxWidth(64), GUILayout.MaxHeight(32));
            //GUILayout.Label(string.Format("{0}/8ths", weather.CloudCover8ths), WeatherPanelStyle, GUILayout.MaxHeight(32));
            //if (GUILayout.Button("More.."))
            //{
            //    _MessageToDisplay = weather.ToString();
            //}
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if ( Event.current.type == EventType.MouseUp )
            {
                if ( weatherRect.Contains(Event.current.mousePosition) )
                {
                    //_ShowWeatherString = !_ShowWeatherString;
                    _MessageToDisplay = weather.ToString();
                }
            }
        }

        //////////////////////////////END TEST //////////////////////////////////





        #endregion

        #region Aircraft Hangar

        _ShowAircraftHangar = _GUIState == GUIState.AircraftHangar;
        //GameManager.Instance.InputManager.IsMouseOverGUI = GameManager.Instance.InputManager.IsMouseOverGUI == true && _GUIState == GUIState.AircraftHangar ? true : false;

        if ( _ShowAircraftHangar )
        {

            if ( SelectedUnit != null )
            {



                Rect aircraftInfoBox = new Rect(WindowRect.width, HangarHeightMod, infoBoxWidth, infoBoxHeight); // Remember this line

                Rect aircraftInfoContent = new Rect(aircraftInfoBox.x, aircraftInfoBox.y, aircraftInfoBox.width, aircraftInfoBox.height / 2);
                float loadOutInfoHeight = WindowRect.height - messageListRect.height;
                Rect aircarftLoadOut = new Rect(messageListRect.x, weatherRect.y - loadOutInfoHeight, 256, loadOutInfoHeight);
                Rect loadOutRect = new Rect(aircarftLoadOut.xMax, aircarftLoadOut.y, 128, _WeaponLoadsDescriptions.Count * 40);
                //aircraftInfoBox.y -= 30;
                //aircraftInfoBox.height -= 30;

                #region Launch
                Rect launchButton = new Rect(weatherRect.xMax, weatherRect.y - 32, 600, 32);
                if ( _SelectedCarriedUnit != null )
                {


                    //if (!mouseOverGUI)
                    //{
                    //    mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, aircraftFittingBox);
                    //}
                    if ( !mouseOverGUI )
                    {

                    }

                    List<CarriedUnit> unitsToLaunch = SelectedUnit.CarriedUnits.FindAll(delegate(CarriedUnit cu) { return cu.Selected && cu.UnitInfo.ReadyInSec == 0 && !cu.LaunchOrderAwaiting; });

                    GUILayout.BeginArea(launchButton);

                    GUILayout.BeginHorizontal();
                    if ( unitsToLaunch.Count > 0 )
                    {
                        if ( !_AwaitingOrder )
                        {
                            if ( GUILayout.Button(new GUIContent(string.Format("Launch {0} units!", unitsToLaunch.Count), GameManager.Instance.GUIManager.PilotIcon, "Enables you to launch selected units. Right click map to give order"), GUILayout.Height(32), GUILayout.MaxWidth(128)) )
                            {
                                _AwaitingOrder = true;
                            }
                        }
                        else
                        {
                            if ( GUILayout.Button("Cancel") )
                            {
                                _AwaitingOrder = false;
                            }
                            GUILayout.Label("Click on a unit to attack it or just click somewhere on the map to move the launched there");
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.EndArea();
                }

                if ( !mouseOverGUI )
                {
                    mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, launchButton);
                }




                #endregion

                #region Aircraft Loadout


                StringBuilder sb = new StringBuilder();
                sb.Append("Possible loadouts: ");
                sb.Append(string.Join(", ", _WeaponLoadsNames.ToArray()));

                if ( _CurrentLoadOut != null && _SelectedCarriedUnitClass != null )
                {
                    GUI.Box(aircarftLoadOut, "", HangarStyle);
                    GUILayout.BeginArea(aircarftLoadOut);

                    //GUILayout.Label(string.Format("Current Load Out: {0}", _CurrentLoadOut.Name));
                    //GUILayout.Label(string.Format("Increase Cruise Range in meters: {0}", _CurrentLoadOut.IncreasesCruiseRangeInM));
                    //GUILayout.Label(string.Format("Increase Load Range by levels: {0}", _CurrentLoadOut.IncreasesLoadRangeByLevels));
                    //GUILayout.Label(string.Format("Increase Radar Cross Section: {0}", _CurrentLoadOut.IncreasesRadarCrossSection));

                    //GUILayout.Space(5);

                    //GUILayout.Label(string.Format("Here you can change the load out for the different unit classes. Be aware of the time it takes").ToUpper());
                    _LoadOutsScrollPosition = GUILayout.BeginScrollView(_LoadOutsScrollPosition);
                    if ( _SelectedCarriedUnitClass != null )
                    {
                        if ( _WeaponLoadsNames.Count > 1 )
                        {
                            GUILayout.BeginHorizontal();
                            if ( GUILayout.Button(new GUIContent("Load outs..", sb.ToString())) )
                            {
                                _LoadOutsShowing = !_LoadOutsShowing;
                            }
                            if ( GUILayout.Button(new GUIContent("X", "Close"), GUILayout.Width(32), GUILayout.Height(32)) )
                            {
                                _SelectedCarriedUnitClass = null;
                                _LoadOutsShowing = false;
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            int selectedWeaponLoad = _SelectedLoadOutIndex;
                            _SelectedLoadOutIndex = GUILayout.SelectionGrid(_SelectedLoadOutIndex, _WeaponLoadsDescriptions.ToArray(), 1);
                            if ( selectedWeaponLoad != _SelectedLoadOutIndex )
                            {
                                _SelectedLoadOut = _SelectedCarriedUnitClass.WeaponLoads.Find(delegate(UnitClassWeaponLoads ucwl) { return ucwl.Name == _WeaponLoadsNames[_SelectedLoadOutIndex]; });
                            }
                        }

                        if ( _SelectedLoadOut != null )
                        {
                            GUILayout.Label(string.Format("Name: {0}", _SelectedLoadOut.Name));
                            GUILayout.Label(string.Format("Increase Cruise Range in meters: {0}", _SelectedLoadOut.IncreasesCruiseRangeInM));
                            GUILayout.Label(string.Format("Increase Load Range by levels: {0}", _SelectedLoadOut.IncreasesLoadRangeByLevels));
                            GUILayout.Label(string.Format("Increase Radar Cross Section: {0}", _SelectedLoadOut.IncreasesRadarCrossSection));

                            GUILayout.Label(string.Format("Time To Change (!!!!): {0} minutes", _SelectedLoadOut.TimeToChangeLoadoutHour * 60));

                            GUILayout.Label("Weapons:");
                            foreach ( UnitClassWeaponLoad load in _SelectedLoadOut.WeaponLoads )
                            {
                                GUILayout.Label(string.Format("\tClass: {0}({1})", load.WeaponClassId, load.MaxAmmunition));
                            }

                            if ( _SelectedLoadOut != _CurrentLoadOut )
                            {
                                if ( GUILayout.Button(string.Format("Change to {0}", _SelectedLoadOut.Name), GUILayout.MaxWidth(200)) )
                                {
                                    GameManager.Instance.OrderManager.ChangeLoadOut(_SelectedUnit.Info, _SelectedCarriedUnit.UnitInfo, _SelectedLoadOut.Name);
                                }
                            }
                            else
                            {
                                if ( _SelectedCarriedUnit.Ready )
                                {
                                    GUILayout.Label("This is current loadout");
                                }
                                else
                                {
                                    GUILayout.Label("Load out being fitted to aircraft");
                                }
                            }
                        }
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndArea();

                    if ( _LoadOutsShowing )
                    {
                        GUI.Box(loadOutRect, "");
                        GUILayout.BeginArea(loadOutRect);
                        _LoadOutsOptionsScrollPosition = GUILayout.BeginScrollView(_LoadOutsOptionsScrollPosition);
                        int selectedWeaponLoad = _SelectedLoadOutIndex;
                        _SelectedLoadOutIndex = GUILayout.SelectionGrid(_SelectedLoadOutIndex, _WeaponLoadsDescriptions.ToArray(), 1);
                        if ( selectedWeaponLoad != _SelectedLoadOutIndex )
                        {
                            _SelectedLoadOut = _SelectedCarriedUnitClass.WeaponLoads.Find(delegate(UnitClassWeaponLoads ucwl) { return ucwl.Name == _WeaponLoadsNames[_SelectedLoadOutIndex]; });
                            _LoadOutsShowing = false;
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndArea();


                    }
                    if ( !mouseOverGUI )
                    {
                        mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, loadOutRect);
                    }
                    if ( !mouseOverGUI )
                    {
                        mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, aircarftLoadOut);
                    }
                #endregion

                }

            }
        }

        #endregion


        #region Formation


        Rect formationRect = new Rect(Screen.width - 200, Screen.height - 200, 200, 200);
        GUI.Box(formationRect, "");

        Rect formationButtonRect = new Rect(formationRect.x, formationRect.y - 32, formationRect.width, 32);
        GUI.Box(formationButtonRect, "");

        GUILayout.BeginArea(formationButtonRect);
        GUILayout.BeginHorizontal();
        if ( _SelectedUnit != null )
        {
            if ( _SelectedUnit.IsMissile || _SelectedUnit.UnitClass.UnitType == GameConstants.UnitType.Sonobuoy )
            {

            }
            else if ( _SelectedUnit.UnitsInGroup.Find(delegate(PlayerUnit p) { return p.FormationChanged; }) != null )
            {
                if ( GUILayout.Button("V", GUILayout.Width(32), GUILayout.Height(32)) )
                {
                    Formation formation = new Formation();
                    PlayerUnit main = _SelectedUnit.Info.IsGroupMainUnit ? _SelectedUnit : GameManager.Instance.UnitManager.SelectedGroupMainUnit;

                    if ( main.GroupInfo != null )
                    {
                        formation = main.GroupInfo.Formation;
                    }

                    for ( int i = 0; i < formation.FormationPositions.Count; i++ )
                    {
                        FormationPosition fp = formation.FormationPositions[i];
                        if ( string.IsNullOrEmpty(fp.AssignedUnitId) )
                        {
                            continue;
                        }

                        for ( int j = 0; j < main.UnitsInGroup.Count; j++ )
                        {
                            PlayerUnit p = main.UnitsInGroup[j];
                            if ( fp.AssignedUnitId == p.Info.Id )
                            {
                                fp.PositionOffset = p.NewFormationPosition != null ? p.NewFormationPosition : fp.PositionOffset;
                                break;
                            }
                        }
                    }



                    GameManager.Instance.OrderManager.SetNewGroupFormation(main, formation);

                    _SelectedFormationUnits.Clear();
                    foreach ( PlayerUnit p in GameManager.Instance.UnitManager.SelectedGroupMainUnit.UnitsInGroup )
                    {
                        p.NewFormationPosition = null;
                    }


                }
                if ( GUILayout.Button("X", GUILayout.Width(32), GUILayout.Height(32)) )
                {
                    _SelectedFormationUnits.Clear();
                    foreach ( PlayerUnit p in GameManager.Instance.UnitManager.SelectedGroupMainUnit.UnitsInGroup )
                    {
                        p.NewFormationPosition = null;
                    }
                    Debug.Log("Cancel formation change");
                }
            }
            else
            {
                if ( _SelectedFormationUnits.Count > 1 )
                {
                    if ( GUILayout.Button(new GUIContent("Split", "Make new group from selected units"), GUILayout.Width(64), GUILayout.Height(32)) )
                    {
                        GameManager.Instance.OrderManager.SplitGroup(GameManager.Instance.UnitManager.SelectedGroupMainUnit, _SelectedFormationUnits);
                    }
                }
                else
                {
                    GUILayout.Space(64);
                }
            }
            if ( GUILayout.RepeatButton(new GUIContent("-", "Zoom in")) )
            {
                if ( _FormationZoomLevelInM > 1000 )
                {
                    _FormationZoomLevelInM -= 100;
                }
                else
                {
                    _FormationZoomLevelInM -= 10;
                }
            }
            GUILayout.Label(string.Format("{0} km", _FormationZoomLevelInM / 1000));
            if ( GUILayout.RepeatButton(new GUIContent("+", "Zoom out")) )
            {
                if ( _FormationZoomLevelInM >= 1000 )
                {
                    _FormationZoomLevelInM += 100;
                }
                else
                {
                    _FormationZoomLevelInM += 10;
                }
            }
            if ( GUILayout.Button(new GUIContent("++", "Fit all units in view")) )
            {
                FitAllUnitsInFormationScreen();
            }

            GameManager.Instance.GUIManager.FormationZoomLevelInM = _FormationZoomLevelInM;

            //_FormationZoomLevelInM = Mathf.FloorToInt(GUILayout.HorizontalSlider(_FormationZoomLevelInM, MinFormationZoomInM, MaxFormationZoomInM));
            //GUILayout.Label(_FormationZoomLevelInM.ToString());
            //if (GUI.changed)
            //{
            //    GameManager.Instance.GUIManager.FormationZoomLevelInM = _FormationZoomLevelInM;
            //    //GUI.tooltip = string.Format("Formation radius: {0}", _FormationZoomLevelInKm);
            //    //Debug.Log("FormationZoomLevel Changed to " + _FormationZoomLevelInKm);
            //}
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();




        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, formationRect);
        }
        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, formationButtonRect);
        }





        GUI.DrawTexture(formationRect, GameManager.Instance.GUIManager.RadarTexture);
        Rect radarLine = new Rect(formationRect.x + formationRect.width / 2, formationRect.y + formationRect.height / 2, 1, formationRect.width / 2);

        Matrix4x4 matrixBackup = GUI.matrix;
        {
            GUIUtility.RotateAroundPivot(MathHelper.Clamp360(GameManager.Instance.CameraManager.MainCamera.transform.eulerAngles.y + 180), new Vector2(radarLine.x, radarLine.y));
            GUI.DrawTexture(radarLine, GameManager.Instance.GUIManager.RadarLine);
        }
        GUI.matrix = matrixBackup;
        GUI.BeginGroup(formationRect);
        if ( SelectedUnit != null )
        {
            PlayerUnit pu = null;

            if ( !SelectedUnit.UnitClass.IsMissileOrTorpedo && SelectedUnit.UnitClass.UnitType != GameConstants.UnitType.Sonobuoy )
            {

                foreach ( PlayerUnit p in _SelectedUnit.UnitsInGroup )
                {
                    if ( p.FormationPosition != null )
                    {
                        //float xFac = (float)p.FormationPosition.PositionOffset.RightM * FormationFactorX;
                        //float yFac = (float)p.FormationPosition.PositionOffset.ForwardM * FormationFactorY;
                        ////Debug.Log(string.Format("x : {0} - - yFac: {1}", xFac, yFac));
                        //xFac += formationRect.width / 2;
                        //yFac += formationRect.height / 2;
                        //xFac += formationRect.x;
                        //yFac += formationRect.y;


                        Rect pos = new Rect(p.FormationGhostPosition2D.x + formationRect.width / 2, p.FormationGhostPosition2D.y + formationRect.height / 2, p.Size.x, p.Size.y);

                        //p.FormationPosition2D = new Vector2(pos.x, pos.y);
                        //GUI.SetNextControlName(p.Info.Id);



                        Event e = Event.current;


                        switch ( e.GetTypeForControl(p.UniqueId) )
                        {
                            case EventType.ContextClick:
                                break;
                            case EventType.DragExited:
                                break;
                            case EventType.DragPerform:
                                break;
                            case EventType.DragUpdated:
                                break;
                            case EventType.ExecuteCommand:
                                break;
                            case EventType.Ignore:
                                break;
                            case EventType.KeyDown:
                                break;
                            case EventType.KeyUp:
                                break;
                            case EventType.Layout:
                                break;
                            case EventType.MouseDown:
                                if ( pos.Contains(e.mousePosition) )
                                {
                                    if ( Event.current.clickCount >= 2 )
                                    {
                                        GameManager.Instance.UnitManager.SelectedUnit = p;

                                    }

                                }
                                break;
                            case EventType.MouseDrag:
                                break;
                            case EventType.MouseMove:
                                break;
                            case EventType.MouseUp:
                                if ( pos.Contains(e.mousePosition) && e.button == 0 && !p.Info.IsGroupMainUnit )
                                {
                                    if ( _SelectedFormationUnits.Count > 0 )
                                    {
                                        if ( e.control )
                                        {
                                            if ( _SelectedFormationUnits.Contains(p) )
                                            {
                                                _SelectedFormationUnits.Remove(p);
                                            }
                                            else
                                            {
                                                _SelectedFormationUnits.Add(p);
                                            }
                                        }
                                        else
                                        {
                                            _SelectedFormationUnits.Clear();

                                            if ( _SelectedFormationUnits.Contains(p) )
                                            {
                                                _SelectedFormationUnits.Remove(p);
                                            }
                                            else
                                            {
                                                _SelectedFormationUnits.Add(p);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ( _SelectedFormationUnits.Contains(p) )
                                        {
                                            _SelectedFormationUnits.Remove(p);
                                        }
                                        else
                                        {
                                            _SelectedFormationUnits.Add(p);
                                        }
                                    }
                                }
                                else if ( e.button == 1 && _SelectedFormationUnits.Contains(p) && _SelectedFormationUnits.Count == 1 )
                                {
                                    PositionOffset po = new PositionOffset();



                                    Vector2 guiPos = MathHelper.MousepositionToGUICoord(Input.mousePosition) - new Vector2(formationRect.x, formationRect.y);

                                    float x = guiPos.x - GameManager.Instance.GUIManager.FormationRectSize.x / 2;
                                    float y = GameManager.Instance.GUIManager.FormationRectSize.y / 2 - guiPos.y;

                                    Debug.Log("Mouse pos before calculation" + new Vector2(x, y));

                                    //guiPos = MathHelper.GUIPositionToActualFormationPosition(new Vector2(x, y));
                                    guiPos = new Vector2(x * GameManager.Instance.GUIManager.FormationFactorX, y * GameManager.Instance.GUIManager.FormationFactorY);

                                    Debug.Log("Mouse pos after " + guiPos);
                                    Debug.Log("Actual formation pos: " + p.FormationPosition.PositionOffset.ToString());

                                    po.ForwardM = guiPos.y;
                                    po.RightM = guiPos.x;

                                    p.NewFormationPosition = po;
                                }
                                break;
                            case EventType.Repaint:
                                FormationObjectNotSelected.Draw(pos, new GUIContent("", GameManager.Instance.GUIManager.GetTextureByUnitClassId(p.Info.UnitClassId), p.Info.UnitName), p.UniqueId);
                                if ( _SelectedFormationUnits.Contains(p) )
                                {
                                    //Draw selected style on top
                                    FormationObjectSelected.Draw(pos, new GUIContent("", GameManager.Instance.GUIManager.GetTextureByUnitClassId(p.Info.UnitClassId), p.Info.UnitName), p.UniqueId);
                                    if ( p.FormationChanged && _SelectedFormationUnits.Count == 1 )
                                    {


                                        Rect newPos = new Rect(p.NewFormationPosition2D.x + formationRect.width / 2, p.NewFormationPosition2D.y + formationRect.height / 2, p.Size.x, p.Size.y);
                                        FormationObjectSelected.Draw(newPos, new GUIContent("", GameManager.Instance.GUIManager.EnemyDetectionTex, p.Info.UnitName), p.UniqueId);

                                    }

                                }
                                break;
                            case EventType.ScrollWheel:
                                break;
                            case EventType.Used:
                                break;
                            case EventType.ValidateCommand:
                                break;
                            default:
                                break;
                        }



                        //if (GUI.Button(pos, GameManager.Instance.GUIManager.GetTextureByUnitClassId(p.Info.UnitClassId), "Label"))
                        //{
                        //    if (Event.current.clickCount >= 2)
                        //    {
                        //        GameManager.Instance.UnitManager.SelectedUnit = p;
                        //    }
                        //}
                    }
                    else
                    //Place icon in the middle
                    {
                        Rect pos = new Rect(formationRect.x + formationRect.width / 2, formationRect.y + formationRect.height / 2, p.Size.x, p.Size.y);
                        FormationObjectNotSelected.Draw(pos, new GUIContent("", GameManager.Instance.GUIManager.GetTextureByUnitClassId(p.Info.UnitClassId), p.Info.UnitName), p.UniqueId);
                    }
                }
               
                if ( pu != null )
                {

                }
            }
            //}
        }
        else
        {
            //GUILayout.Label("No ally group selected");
        }
        GUI.EndGroup();

        #endregion

        #region Misc window

        Rect interactionWindow = new Rect(messageListRect.xMax, messageListRect.y, 150, 200);
        //old = GUI.skin;
        //GUI.skin = TestSkin;
        GUI.Box(interactionWindow, "");

        GUILayout.BeginArea(interactionWindow);
        if ( _SelectedUnit != null )
        {

            GUILayout.Label("Speed Types:");
            GameConstants.UnitSpeedType currentSpeedType = _CurrentUnitSpeedType;

            _CurrentUnitSpeedType = GameManager.Instance.GUIManager.ComboBox<GameConstants.UnitSpeedType>(_CurrentUnitSpeedType,
                ref _UnitSpeedTypeComboboxOpen, ref _UnitSpeedTypeComboboxScroller,
                _SelectedUnit.Info.SupportedSpeedTypes, new Rect(0, 0, interactionWindow.width, interactionWindow.height), false, "Test");

            if ( _CurrentUnitSpeedType != currentSpeedType )
            {
                GameManager.Instance.OrderManager.ChangeSpeed(SelectedUnit, _CurrentUnitSpeedType);
            }

            if ( !_UnitSpeedTypeComboboxOpen && SelectedUnit.Info.SupportedDepthHeight.Count > 1 )
            {
                GUILayout.Label("Height Types:");

                GameConstants.HeightDepthPoints currentHeightDepth = _CurrentHeightDepthPoints;

                _CurrentHeightDepthPoints = GameManager.Instance.GUIManager.ComboBox<GameConstants.HeightDepthPoints>(_CurrentHeightDepthPoints,
                    ref _HeightDepthComboboxOpen, ref _HeightDepthComboboxScroller, _SelectedUnit.Info.SupportedDepthHeight,
                    new Rect(0, 0, interactionWindow.width, interactionWindow.height), false, "");

                if ( _CurrentHeightDepthPoints != currentHeightDepth )
                {
                    GameManager.Instance.OrderManager.ChangeHeightDepth(SelectedUnit, _CurrentHeightDepthPoints);
                }
            }

        }
        GUILayout.EndArea();


        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, interactionWindow);
        }

        #endregion

        #region TextWindow

        Rect textWindowRect = new Rect(interactionWindow.xMax, interactionWindow.y, 150, 200);
        GUI.Box(textWindowRect, "");

        if ( _MessageToDisplay != "" )
        {
            GUILayout.BeginArea(textWindowRect);

            GUILayout.Space(1);
            _MessageReadScrollPosition = GUILayout.BeginScrollView(_MessageReadScrollPosition);
            GUILayout.Label(string.Format("{0}", _MessageToDisplay), ReadMessageStyle);
            GUILayout.EndScrollView();
            //if (GUILayout.Button("Close"))
            //{
            //    _MessageToDisplay = "";
            //}
            GUILayout.EndArea();
        }


        #endregion
        //GUI.skin = old;


        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, textWindowRect);
        }

        if ( !mouseOverGUI )
        {
            mouseOverGUI = MathHelper.MouseOverGUI(Input.mousePosition, formationRect);
        }

        GameManager.Instance.InputManager.IsMouseOverGUI = mouseOverGUI;

        //if (GameManager.Instance.GameInfo != null)
        //{
        //    Rect gameTime = new Rect(weatherRect.xMax, weatherRect.y - 32, 64, weatherRect.height);
        //    GUI.Box(gameTime, "");
        //    GUILayout.BeginArea(gameTime);

        //    GUILayout.Label(new GUIContent(GameManager.Instance.GameInfo.GameCurrentTime.ToShortTimeString(), GameManager.Instance.GameInfo.ToString()));

        //    GUILayout.EndArea();
        //}

        Rect toolTipRect = new Rect(weatherRect.xMax, weatherRect.y, Screen.width - WindowRect.width - weatherRect.width - formationRect.width, weatherRect.height);
        GameManager.Instance.GUIManager.TooltipRect = toolTipRect;
        GUI.Box(toolTipRect, "");
        GameManager.Instance.GUIManager.ShowToolTip(GUI.tooltip);


    }



    public void FitAllUnitsInFormationScreen()
    {
        if ( _SelectedUnit != null )
        {
            float maxForward = 0;
            float maxRight = 0;

            foreach ( PlayerUnit p in _SelectedUnit.UnitsInGroup )
            {
                if ( p.FormationPosition != null )
                {
                    if ( Mathf.Abs(( float ) p.FormationPosition.PositionOffset.ForwardM) > maxForward )
                    {
                        maxForward = Mathf.Abs(( float ) p.FormationPosition.PositionOffset.ForwardM);
                    }

                    if ( Mathf.Abs(( float ) p.FormationPosition.PositionOffset.RightM) > maxRight )
                    {
                        maxRight = Mathf.Abs(( float ) p.FormationPosition.PositionOffset.RightM);
                    }
                }
            }
            _FormationZoomLevelInM = maxForward > maxRight ? ( int ) maxForward : ( int ) maxRight;
            _FormationZoomLevelInM *= 3;
        }
    }

    public void SetSelectedCarriedUnitClass(UnitClass unitclass)
    {
        _SelectedCarriedUnitClass = unitclass;

        _WeaponLoadsNames = new List<string>();
        _WeaponLoadsDescriptions = new List<GUIContent>();
        foreach ( UnitClassWeaponLoads weaponLoads in _SelectedCarriedUnitClass.WeaponLoads )
        {
            //if (weaponLoads == _CurrentLoadOut)
            //{
            //    continue;
            //}
            _WeaponLoadsNames.Add(weaponLoads.Name);
            _WeaponLoadsDescriptions.Add(new GUIContent(weaponLoads.Name, weaponLoads.ToString()));
        }
    }

    public void Attack()
    {
        if ( AwaitingOrder )
        {
            List<CarriedUnit> unitsToLaunch = SelectedUnit.CarriedUnits.FindAll(delegate(CarriedUnit cu) { return cu.Selected == true && cu.UnitInfo.ReadyInSec == 0; });

            GameManager.Instance.OrderManager.Launch(_SelectedUnit.Info, unitsToLaunch, GameConstants.UnitOrderType.LaunchAircraft);
            _SelectedCarriedUnit = null;
            _SelectedCarriedUnitClass = null;

            foreach ( CarriedUnit cu in unitsToLaunch )
            {
                cu.Selected = false;
            }
        }
    }

    public void LaunchAttack(Enemy enemy)
    {
        if ( AwaitingOrder )
        {
            List<CarriedUnit> unitsToLaunch = SelectedUnit.CarriedUnits.FindAll(delegate(CarriedUnit cu) { return cu.Selected == true && cu.UnitInfo.ReadyInSec == 0; });

            GameManager.Instance.OrderManager.LaunchAttack(SelectedUnit.Info, enemy.Info, unitsToLaunch, GameManager.Instance.InputManager.GetCoordinateFromScreenPoint(enemy.GUIScreenPos));
        }
    }

    public void Move(Coordinate coord)
    {
        if ( AwaitingOrder )
        {
            List<CarriedUnit> unitsToLaunch = SelectedUnit.CarriedUnits.FindAll(delegate(CarriedUnit cu) { return cu.Selected == true && cu.UnitInfo.ReadyInSec == 0; });

            GameManager.Instance.OrderManager.Launch(_SelectedUnit.Info, unitsToLaunch, GameConstants.UnitOrderType.LaunchAircraft, coord);
            _SelectedCarriedUnit = null;
            _SelectedCarriedUnitClass = null;
            _AwaitingOrder = false;

            foreach ( CarriedUnit cu in unitsToLaunch )
            {
                cu.Selected = false;
                cu.LaunchOrderAwaiting = true;
            }
        }
    }
}
