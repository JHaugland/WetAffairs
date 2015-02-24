using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System;

/// AManager is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///        AManager.instance.DoSomeThing();
///
public class GameManager : MonoBehaviour
{

    #region Privates



    private GameInfo _GameInfo;
    private PlayerInfo _PlayerInfo;

    #endregion

    #region Enums

    public enum UnitClasses
    {
        BattleCruiser,
        Carrier,
        PatrolBoat,
        Submarine,
        CivillianVessel,
        Heilcopter,
        Fighter,
        Frigate,
        Bomber,
        TransportECM,
        None,
        Group,
        Missile,
        EnemyDetection,
        Airport
    }

    public enum GameStates
    {
        Satelite,
        Surface,
        Fly,
        Menu
    }

    public enum GUIState
    {
        Default,
        CarriedUnits,
        Weapons,
        Sensors

    }

    public enum MessageTypes
    {
        Battle,
        Detection,
        Game,
        Chat,
        Archive
    }

    public enum UnitOfLength
    {
        Kilometer,
        NauticalMiles,
        Feet
    }

    public enum UnitOfVelocity
    {
        KilometersPerHour,
        NauticalMilesPerHour
    }

    #endregion



    // _Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GameManager _Instance = null;
    private AudioManager _AudioManager = null;
    private UnitManager _UnitManager = null;
    private NetworkManager _NetworkManager = null;
    private GUIManager _GuiManager = null;
    private CommunicationManager _CommunicationManager = null;
    private CameraManager _CameraManager = null;
    private BattleManager _BattleManager = null;
    private OrderManager _OrderManager = null;
    private MessageManager _MessageManager = null;
    private NWInputManager _InputManager = null;
    private EnviromentManager _EnviromentManager = null;


    ///////// Lists //////////////
    private List<UnitClass> _UnitClasses = new List<UnitClass>();
    private List<WeaponClass> _WeaponClasses = new List<WeaponClass>();
    private List<SensorClass> _SensorClasses = new List<SensorClass>();
    private List<Country> _Countries = new List<Country>();
    private List<GameScenario> _Scenarios = new List<GameScenario>();
    private List<Formation> _Formations = new List<Formation>();
    ///////////END LISTS ///////////////////////
    /////////STUFF////////////
    private GameScenario _ScenarioLoaded;
    private DateTime _GameCurrentTime = DateTime.Now;
    private UnitOfLength _UnitLength = UnitOfLength.Kilometer;
    private UnitOfVelocity _UnitVelocity = UnitOfVelocity.KilometersPerHour;


    //~ private GUIManager _GuiManager = null;
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (_Instance == null)
                {
                    Debug.Log("Could not locate a GameManager object.");
                }
            }
            return _Instance;
        }
    }

    public AudioManager AudioManager
    {
        get
        {
            if (_AudioManager == null)
            {

                _AudioManager = FindObjectOfType(typeof(AudioManager)) as AudioManager;
                if (_AudioManager == null)
                {
                    Debug.Log("Could not locate an AudioManager object.");
                }
            }

            return _AudioManager;
        }
    }

    public UnitManager UnitManager
    {
        get
        {
            if (_UnitManager == null)
            {

                _UnitManager = FindObjectOfType(typeof(UnitManager)) as UnitManager;
                if (_UnitManager == null)
                {
                    Debug.Log("Could not locate an UnitManager object.");
                }
            }

            return _UnitManager;
        }
    }

    public NetworkManager NetworkManager
    {
        get
        {
            if (_NetworkManager == null)
            {

                _NetworkManager = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
                if (_NetworkManager == null)
                {
                    Debug.Log("Could not locate a NetworkManager object.");
                }
            }

            return _NetworkManager;
        }
    }

    public GUIManager GUIManager
    {
        get
        {
            if (_GuiManager == null)
            {

                _GuiManager = FindObjectOfType(typeof(GUIManager)) as GUIManager;
                if (_GuiManager == null)
                {
                    Debug.Log("Could not locate a GUIManager object.");
                }
            }

            return _GuiManager;
        }
    }

    public CommunicationManager CommunicationManager
    {
        get
        {
            if (_CommunicationManager == null)
            {

                _CommunicationManager = FindObjectOfType(typeof(CommunicationManager)) as CommunicationManager;
                if (_CommunicationManager == null)
                {
                    Debug.Log("Could not locate a CommunicationManager object.");
                }
            }

            return _CommunicationManager;
        }
    }

    public CameraManager CameraManager
    {
        get
        {
            if (_CameraManager == null)
            {

                _CameraManager = FindObjectOfType(typeof(CameraManager)) as CameraManager;
                if (_CameraManager == null)
                {
                    Debug.Log("Could not locate a CameraManager object.");
                }
            }

            return _CameraManager;
        }
    }

    public BattleManager BattleManager
    {
        get
        {
            if (_BattleManager == null)
            {

                _BattleManager = FindObjectOfType(typeof(BattleManager)) as BattleManager;
                if (_BattleManager == null)
                {
                    Debug.Log("Could not locate a BattleManager object.");
                }
            }

            return _BattleManager;
        }
    }

    public OrderManager OrderManager
    {
        get
        {
            if (_OrderManager == null)
            {

                _OrderManager = FindObjectOfType(typeof(OrderManager)) as OrderManager;
                if (_OrderManager == null)
                {
                    Debug.Log("Could not locate a OrderManager object.");
                }
            }

            return _OrderManager;
        }
    }

    public MessageManager MessageManager
    {
        get
        {
            if (_MessageManager == null)
            {

                _MessageManager = FindObjectOfType(typeof(MessageManager)) as MessageManager;
                if (_MessageManager == null)
                {
                    Debug.Log("Could not locate a MessageManager object.");
                }
            }

            return _MessageManager;
        }
    }

    

    public NWInputManager InputManager
    {
        get
        {
            if (_InputManager == null)
            {

                _InputManager = FindObjectOfType(typeof(NWInputManager)) as NWInputManager;
                if (_InputManager == null)
                {
                    Debug.Log("Could not locate a MessageManager object.");
                }
            }

            return _InputManager;
        }
    }

    public EnviromentManager EnviromentManager
    {
        get
        {
            if (_EnviromentManager == null)
            {

                _EnviromentManager = FindObjectOfType(typeof(EnviromentManager)) as EnviromentManager;
                if (_EnviromentManager == null)
                {
                    Debug.Log("Could not locate a MessageManager object.");
                }
            }

            return _EnviromentManager;
        }
    }


    public GameInfo GameInfo
    {
        get
        {
            return _GameInfo;
        }
        set
        {
            _GameInfo = value;
            SetTime();
        }
    }

    public PlayerInfo PlayerInfo
    {
        get
        {
            return _PlayerInfo;
        }
        set
        {
            _PlayerInfo = value;
        }
    }

    public List<GameScenario> Scenarios
    {
        get
        {
            return _Scenarios;
        }
    }

    public GameScenario ScenarioLoaded
    {
        get
        {
            return _ScenarioLoaded;
        }
    }

    public UnitOfLength UnitLength
    {
        get
        {
            return _UnitLength;
        }
        set
        {
            _UnitLength = value;
        }
    }

    public UnitOfVelocity UnitVelocity
    {
        get
        {
            return _UnitVelocity;
        }
        set
        {
            _UnitVelocity = value;
        }
    }

    //public Rect PlayArea
    //{
    //    get
    //    {
    //        return _PlayArea;
    //    }
    //}

    void Awake()
    {
        InitAllClassData();
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        _Instance = null;
    }

    

    public void InitAllClassData()
    {
        _Countries = SerializationHelper.LoadCountriesFromXML();
        _SensorClasses = SerializationHelper.LoadSensorClassFromXML();
        _WeaponClasses = SerializationHelper.LoadWeaponClassFromXML();
        _UnitClasses = SerializationHelper.LoadUnitClassFromXML();
        _Scenarios = SerializationHelper.LoadScenariosFromXML();
        //~ _Formations = SerializationHelper.LoadFormationsFromXML();

    }

    public bool AddUnitClass(UnitClass unitClass)
    {
        if ( _UnitClasses.Find(delegate(UnitClass u) { return u.Id == unitClass.Id; }) == null )
        {
            Debug.Log(string.Format("Adding unitclass: {0}", unitClass.UnitClassShortName));
            _UnitClasses.Add(unitClass);
            return true;
        }

        return false;
    }

    public UnitClass GetUnitClass(string unitClassId)
    {
        try
        {
            return _UnitClasses.Find(delegate(UnitClass uc) { return uc.Id == unitClassId; });
        }
        catch ( Exception ex )
        {
            Debug.Log("Unitclass Not found. Exception thrown " + ex.Message);
        }
        Debug.Log("Unitclass not found");
        return null;
    }


    public SensorClass GetSensorClass(string sensorClassId)
    {
        return _SensorClasses.Find(delegate(SensorClass sc) { return sc.Id == sensorClassId; });
    }

    public WeaponClass GetWeaponClass(string weaponClassId)
    {
        return _WeaponClasses.Find(delegate(WeaponClass wc) { return wc.Id == weaponClassId; });
    }

    public void LoadScenario(GameScenario scenario)
    {
        _ScenarioLoaded = scenario;
        Application.LoadLevel("Scenario");
    }


    private void SetTime()
    {
        GameCurrentTime = GameInfo.GameCurrentTime.GetDateTime();
    }



    #region Unity Methods


    ////TODO: MAKE LOAD SEQUENCE AND INITIALIZING OF GAME TAKE CARE OF THIS
    void Start()
    {
        DontDestroyOnLoad(this);
        InitAllClassData();
    }

    void Update()
    {
        if (GameInfo != null)
        {
            //Debug.Log(GameCurrentTime.ToShortTimeString());
            GameCurrentTime = GameCurrentTime.AddSeconds(Time.deltaTime * GameInfo.RealTimeCompressionFactor);
        }
        if ( Input.GetKeyDown(KeyCode.L) )
        {
            Application.CaptureScreenshot(string.Format("Screenshots/{0}.png", DateTime.Now.Millisecond));
        }
    }

    #endregion

    #region Public Properties

    public DateTime GameCurrentTime
    {
        get
        {
            return _GameCurrentTime;
        }
        set
        {
            _GameCurrentTime = value;
        }
    }



    //public GameStates GameState = GameStates.Satelite;
    public float SurfaceCameraYMinLimitNormal = -20;
    public float SurfaceCameraYMaxLimitNormal = 45;
    public float SurfaceCameraYMinLimitAir = -80;
    public float SurfaceCameraYMaxLimitAir = 10;

    public GameObject MissileMapExplosion;
    public GameObject SurfaceExplosion;
    public float LightModifier = 0;

    public float CircumferenceThroughEquator = 40075.16f;
    public float CircumferenceThroughPoles = 40008.0f;

    public float XMapModifier = 1.660109f;
    public float YMapModifier = -1.684881f;

    public float XMapAddition = 2250;
    public float YMapAddtion = 3600;

    public int _MaxMessageLength = 100;

    public Vector3 UnitRepository = Vector3.zero;
    public Vector3 GlobalScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float MaxHeightInM = 20000;
    public Coordinate Origin;
    public float ShowOverlayOnUnitAtDistanceM = 100;
    
    public int OceanHeight;
    public int OceanWidth;
    public float OceanScale;
    public Vector3 OceanSize;
    public ComplexF[] OceanData;
    

    public void SetOceanSize(Vector3 size) { OceanSize = size; }
    public void SetOceanHeightWidth(Vector2 size) { OceanHeight = (int)size.y; OceanWidth = (int)size.x; }
    public void SetOceanScale(float scale) { OceanScale = scale; }

    public void SetOceanData(ComplexF[] data)
    {
        OceanData = data;
    }

    public float GetWaterHeightAtLocation(Vector3 pos)
    {
        float x = 0;
        float y = 0;
        x = x / OceanSize.x;
        x = (x - Mathf.Floor(x)) * OceanWidth;
        y = y / OceanSize.z;
        y = (y - Mathf.Floor(y)) * OceanHeight;

        return OceanData[OceanWidth * Mathf.FloorToInt(y) + Mathf.FloorToInt(x)].Re * OceanScale / (OceanWidth * OceanHeight);
    }

    


    public int MaxMessageLength
    {
        get
        {
            return _MaxMessageLength;
        }
    }

    

    public float WidthModifier
    {
        get
        {
            Resolution[] resolutions = Screen.resolutions;

            float _HighestWidth = resolutions[resolutions.Length - 1].width;


            float _LowestWidth = resolutions[0].width;


            float currentWidth = Screen.currentResolution.width;


            float widthMod = currentWidth - _LowestWidth;
            return widthMod / (_HighestWidth - _LowestWidth);


        }
        //~ set
        //~ {
        //~ _WidthModifier = value;
        //~ }
    }

    public float HeightModifier
    {
        get
        {
            Resolution[] resolutions = Screen.resolutions;

            float _HighestHeight = resolutions[resolutions.Length - 1].height;
            float _LowestHeight = resolutions[0].height;

            float currentHeight = Screen.currentResolution.height;

            float heightMod = currentHeight - _LowestHeight;
            return heightMod / (_HighestHeight - _LowestHeight);

        }
        //~ set
        //~ {
        //~ _HeightModifier = value;
        //~ }
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(new Vector3(PlayArea.left, 35000, PlayArea.top), new Vector3(PlayArea.width, 10, PlayArea.height));
    }

    #endregion



}