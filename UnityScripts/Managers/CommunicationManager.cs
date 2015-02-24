using UnityEngine;
using System;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class CommunicationManager : MonoBehaviour
{

    #region Privates


    #endregion

    #region Public Properties



    #endregion

    #region Scriptvariables

    //////////////////// TO BE DELETED WHEN BETTER SOLUTION IS MADE////////////////////
    #region American Alliance

    public GameObject ABurke;
    public GameObject Heli;
    public GameObject Seahawk;
    public GameObject Airplane;
    public GameObject Nimitz;
    public GameObject FridtjofNansen;
    public GameObject F22;

    #endregion

    #region Russion Federation

    public GameObject Kirov;
    public GameObject Kamov27;
    public GameObject Gorshkov;
    public GameObject Kuznetsov;


    #endregion

    #region Shared

    public GameObject Sunobuoy;
    public GameObject EnemyMapUnit;
    public GameObject MapUnit;
    public GameObject MissileMap;
    public GameObject HealthBar;
    public GameObject AsuMissile;
    public GameObject Default;
    public GameObject Airport;


    #endregion

    


    /////////////////////////// END TO BE DELETED /////////////////////////////////////////////
    public Vector2 MapUnitSize = new Vector2(50, 50);


    #endregion

    #region Public Methods

    public void HandleReceivedData(IMarshallable dataReceived)
    {
        //Debug.Log(dataReceived);
        switch (dataReceived.ObjectTypeToken)
        {
            case CommsMarshaller.ObjectTokens.NoObject:
                break;
            case CommsMarshaller.ObjectTokens.Enq:
                break;
            case CommsMarshaller.ObjectTokens.Ack:
                break;
            case CommsMarshaller.ObjectTokens.ClientInfoRequest:
                break;
            case CommsMarshaller.ObjectTokens.GameControlRequest:
                break;

            case CommsMarshaller.ObjectTokens.GameStateInfo:
                HandleGameStateInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.GameUiControl:
                HandleGameUIControlInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.MessageInfo:
                HandleMessageInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.DefeatConditionSetInfo:
                break;
            case CommsMarshaller.ObjectTokens.DefeatConditionInfo:
                break;
            case CommsMarshaller.ObjectTokens.MessageString:
                MessageString str = dataReceived as MessageString;
                if (str != null)
                {
                    //~ ShowMessage("MessageString: " + str.Message);
                }

                GameManager.Instance.MessageManager.AddMessage(str.Message, GameManager.MessageTypes.Game, null);

                break;
            case CommsMarshaller.ObjectTokens.BattleDamageReport:
                BattleDamageReport report = dataReceived as BattleDamageReport;
                PlayerInfo playerInfo = GameManager.Instance.PlayerInfo;
                
                if (report != null)
                {
                    if (playerInfo != null)
                    {
                        //GameManager.Instance.MessageManager.AddMessage(string.Format("MessageToAttacker: {0} - MessageToAttackee: {1}",
                        //    string.IsNullOrEmpty(report.MessageToAttacker) ? report.ToString() : report.MessageToAttacker, 
                        //    string.IsNullOrEmpty(report.MessageToAttackee) ? report.ToString() : report.MessageToAttackee), 
                        //    GameManager.MessageTypes.Battle, report.Position);
                        GameManager.Instance.MessageManager.AddMessage(report.PlayerInflictingDamageId == playerInfo.Id ? report.MessageToAttacker : report.MessageToAttackee, GameManager.MessageTypes.Battle, report.Position);


                        if ( report.DamagePercent > 0 )
                        {
                            ////Check if it is we who have been hit
                            if ( report.PlayerSustainingDamageId == playerInfo.Id )
                            {
                                //Find unit which has been hit
                                PlayerUnit unit = GameManager.Instance.UnitManager.FindUnitById(report.TargetPlatformId);
                                if ( unit != null )
                                {
                                    //Spawn explosion
                                    unit.SpawnExplosion();
                                }
                            }
                            else
                            {

                                //TODO:Jan du må huske TargetPlatformId
                                Enemy enemyUnit = GameManager.Instance.UnitManager.FindEnemyById(report.TargetPlatformId);
                                if ( enemyUnit != null )
                                {
                                    enemyUnit.SpawnExplosion();
                                }
                                else
                                {
                                    Debug.Log("enemy is null");
                                }

                            }
                        }


                        float distanceLat = Mathf.Abs((float)report.Position.Latitude - GameManager.Instance.Origin.Latitude);
                        //float distanceLng = Mathf.Abs((float)report.Position.Longitude - GameManager.Instance.Origin.Longitude);

                        //if (distanceLat < 1 && distanceLng < 1)
                        //{
                        //    //calculate distance and bearing
                        //    //Coordinate origin = new Coordinate((float)this.Info.Position.Latitude, (float)this.Info.Position.Longitude);
                        //    Coordinate position = new Coordinate((float)report.Position.Latitude, (float)report.Position.Longitude);

                        //    Coordinate coord = CoordinateHelper.CalculateCoordinateFromBearingAndDistance(GameManager.Instance.Origin, position);

                        //    Vector3 worldPos = new Vector3(coord.Longitude, (float)report.Position.HeightOverSeaLevelM, coord.Latitude);

                        //    //Debug.Log(worldPos);
                        //    worldPos.y = transform.position.y;
                        //    //transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime);
                         
                        //}

                    }
                }

                break;
            case CommsMarshaller.ObjectTokens.GameInfo:
                HandleGameInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.PlayerInfo:
                HandlePlayerInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.PositionInfo:
                HandlePositionInfo(dataReceived);

                break;
            case CommsMarshaller.ObjectTokens.BaseUnitInfo:
                HandleBaseUnitInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.GroupInfo:
                HandleGroupInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.DetectedUnitInfo:
                HandleDetectedUnitInfo(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.OrderInfo:
                break;
            case CommsMarshaller.ObjectTokens.UnitMovementOrder:
                break;
            case CommsMarshaller.ObjectTokens.UnitEngagementOrder:
                break;
            case CommsMarshaller.ObjectTokens.UnitClass:
                Debug.Log("Received new unitclass");
                HandleUnitClass(dataReceived);
                break;
            case CommsMarshaller.ObjectTokens.WeaponClass:
                break;
            case CommsMarshaller.ObjectTokens.SensorClass:
                break;
            case CommsMarshaller.ObjectTokens.GameScenario:
                break;
            case CommsMarshaller.ObjectTokens.GameScenarioAlliance:
                break;
            case CommsMarshaller.ObjectTokens.GameScenarioPlayer:
                break;
            case CommsMarshaller.ObjectTokens.GameScenarioGroup:
                break;
            case CommsMarshaller.ObjectTokens.GameScenarioUnit:
                break;
            default:
                //~ ShowMessage("WTF?");
                break;
        }
    }

    

    





    #endregion

    #region Private Methods

    private void HandleGameUIControlInfo(IMarshallable dataReceived)
    {
        Debug.Log("GameUIControl received");
        GameUiControl guc = dataReceived as GameUiControl;

        Vector3 position = new Vector3((float)guc.PositionX, (float)guc.PositionY, (float)guc.PositionZ);
        Vector3 rotation = new Vector3((float)guc.RotationX, (float)guc.RotationY, (float)guc.RotationZ);

        PlayerUnit unit = GameManager.Instance.UnitManager.FindUnitById(guc.Id);

        GameManager.Instance.CameraManager.SetCameraFocus(position, rotation, unit != null ? unit : null);

    }

    private void HandleGameInfo(IMarshallable dataReceived)
    {


        Debug.Log(dataReceived.ToString());
        GameManager.Instance.GameInfo = dataReceived as GameInfo;
    }

    private void HandleGroupInfo(IMarshallable dataReceived)
    {
        GroupInfo gi = dataReceived as GroupInfo;
        GameManager.Instance.UnitManager.AddGroupInfo(gi);
    }

    private void HandlePlayerInfo(IMarshallable dataReceived)
    {
        GameManager.Instance.PlayerInfo = dataReceived as PlayerInfo;
    }

    private void HandleMessageInfo(IMarshallable dataReceived)
    {
        MessageInfo info = dataReceived as MessageInfo;

        if (info != null)
        {
            GameManager.Instance.MessageManager.AddMessage(info.ToString(), GameManager.MessageTypes.Game, null);
        }
    }

    private void HandlePositionInfo(IMarshallable dataReceived)
    {

        PositionInfo posInfo = dataReceived as PositionInfo;
        if (posInfo != null)
        {
            if (posInfo.IsDetection)
            {
                Enemy eUnit = GameManager.Instance.UnitManager.FindEnemyById(posInfo.UnitId);
                //~ Debug.Log("Is Detection");
                if (eUnit != null)
                {
                    eUnit.Position = posInfo;
                }
                else
                {
                    Debug.Log("Enemy Unit does not exist. The Flying Dutchman???");
                }
            }
            else
            {
                PlayerUnit pUnit = GameManager.Instance.UnitManager.FindUnitById(posInfo.UnitId);

                if (pUnit == null)
                //DO Something
                {
                    //~ Debug.Log(string.Format("Unit is null {0}", posInfo.UnitId));
                }
                else
                {
                    pUnit.Position = posInfo;
                    if (pUnit.Info.Tag == "main")
                    {
                        //Debug.Log(string.Format("Main unit PositionInfo.BearingDeg : {0} - for Unit:  {1}", posInfo.BearingDeg, posInfo.UnitId));
                    }


                }
            }
        }
    }


    private void HandleGameStateInfo(IMarshallable dataReceived)
    {

        //GameManager.Instance.MessageManager.AddMessage(dataReceived.ToString(), GameManager.MessageTypes.Game, null);


        GameStateInfo gameInfo = dataReceived as GameStateInfo;
        if (gameInfo != null)
        {
            switch (gameInfo.InfoType)
            {
                case CommsMarshaller.GameStateInfoType.UnitIsDestroyed:
                    PlayerUnit unit = GameManager.Instance.UnitManager.FindUnitById(gameInfo.Id);
                    if (unit != null)
                    {
                        unit.Kill(true);

                    }
                    //~ ShowMessage("GameStateInfo object, UnitIsDestroyed");
                    break;
                case CommsMarshaller.GameStateInfoType.DetectedContactIsLost:
                    //~ ShowMessage("GameStateInfo object, DetectedContactIsLost");
                    Debug.Log(string.Format("Lost detection Id: {0} - Time:{1}", gameInfo.Id, Time.time));

                    Enemy e = GameManager.Instance.UnitManager.FindEnemyById(gameInfo.Id);
                    if (e != null)
                    {
                        e.Kill();
                    }
                    else
                    {
                        Debug.Log("Lost contact with unit not in enemy list. Error?");
                    }

                    break;
                case CommsMarshaller.GameStateInfoType.AircraftIsLanded:
                    {
                        
                        PlayerUnit launchPlatform = GameManager.Instance.UnitManager.FindUnitById(gameInfo.SecondaryId);
                        PlayerUnit aircraft = GameManager.Instance.UnitManager.FindUnitById(gameInfo.Id);

                        Debug.Log(string.Format("Aircraft has landed: {0} on {1}", aircraft.Info.UnitName, launchPlatform.Info.UnitName));
                        if ( aircraft != null )
                        {
                            Debug.Log(string.Format("Killing off unit: {0}. ", aircraft.Info.UnitName));
                            aircraft.Kill(false);
                            Debug.Log(string.Format("{0} killed ", aircraft.Info.UnitName));
                        }
                        else
                        {
                            Debug.Log("Aircraft is null");
                            break;
                        }

                        //gameInfo.
                        if ( launchPlatform != null )
                        {
                            AnimationLauncher al = launchPlatform.GetComponent<AnimationLauncher>();
                            Debug.Log(string.Format("Animation launcher is on: {0}", al.gameObject.name));
                            if ( al != null )
                            {
                                
                                UnitClass uc = GameManager.Instance.GetUnitClass(aircraft.Info.UnitClassId);
                                Debug.Log(string.Format("Unitclass found: {0}. ", uc.UnitClassShortName));
                                al.TakeOffMode = uc.UnitType == GameConstants.UnitType.Helicopter ? AnimationLauncher.AnimMode.HelicopterLanding : AnimationLauncher.AnimMode.FixedWingLanding;
                                Debug.Log(string.Format("Changed takeoffmode to: {0}. Attempting launch...", al.TakeOffMode.ToString()));
                                al.LaunchAnimation();
                                Debug.Log(string.Format("Animation launched..."));
                            }
                            else
                            {
                                Debug.Log("AnimationLauncher is null");
                            }
                        }
                        else
                        {
                            Debug.Log("Launchplatform is null");
                        }

                        

                       
                        
                        //if (carrier != null)
                        //{
                        //    GameManager.Instance.UnitManager.SelectedUnit = carrier;
                        //}
                        //~ ShowMessage("GameStateInfo object, AircraftIsLanded");
                        break;
                    }
                case CommsMarshaller.GameStateInfoType.AircraftTakeoff:
                    {
                        PlayerUnit launchPlatform = GameManager.Instance.UnitManager.FindUnitById(gameInfo.SecondaryId);
                        //gameInfo.
                        if ( launchPlatform != null )
                        {
                            AnimationLauncher al = launchPlatform.GetComponent<AnimationLauncher>();
                            if ( al != null )
                            {
                                UnitClass uc = GameManager.Instance.GetUnitClass(gameInfo.UnitClassId);

                                al.TakeOffMode = uc.UnitType == GameConstants.UnitType.Helicopter ? AnimationLauncher.AnimMode.HelicopterTakeOff : AnimationLauncher.AnimMode.FixedWingTakeOff;
                                al.LaunchAnimation();
                            }
                        }
                        break;
                    }
                case CommsMarshaller.GameStateInfoType.MissileLaunch:
                    PlayerUnit shooter = GameManager.Instance.UnitManager.FindUnitById(gameInfo.SecondaryId);


                    if ( shooter != null )
                    {
                        shooter.FireMissile();
                    }
                    else
                    {
                        Debug.Log(gameInfo.SecondaryId + " is not a unit here");
                    }

                    break;
            }

        }
    }

    private void HandleDetectedUnitInfo(IMarshallable dataReceived)
    {
        //~ string selectedId = string.Empty;
        DetectedUnitInfo det = dataReceived as DetectedUnitInfo;
        GameManager.Instance.MessageManager.AddMessage(dataReceived.ToString(), GameManager.MessageTypes.Detection, det.Position);

        //Debug.Log(dataReceived.ToString());
        if (det != null)
        {
            Enemy unit = null;
            GameObject go = null;
            try
            {
                unit = GameManager.Instance.UnitManager.FindEnemyById(det.Id);

                go = unit != null ? unit.gameObject : null;
            }
            catch (NullReferenceException ex)
            {
                Debug.Log(ex.Message);
            }
            if (go == null)
            {
                
                string UnitModelFileName = "unknownSurface";

                switch ( det.DetectionClassification )
                {
                    case GameConstants.DetectionClassification.FixedWingAircraft:
                        UnitModelFileName = "unknownFixedWing";
                        break;
                    case GameConstants.DetectionClassification.Helicopter:
                        UnitModelFileName = "unknownHeli";
                        break;
                    case GameConstants.DetectionClassification.Mine:
                        break;
                    case GameConstants.DetectionClassification.Missile:
                        break;
                    case GameConstants.DetectionClassification.Submarine:
                        UnitModelFileName = "unknownSubmarine";
                        break;
                    case GameConstants.DetectionClassification.Surface:
                        UnitModelFileName = "unknownSurface";
                        break;
                    case GameConstants.DetectionClassification.Torpedo:
                        break;
                    case GameConstants.DetectionClassification.Unknown:
                        break;
                    default:
                        break;
                }

                if ( !string.IsNullOrEmpty(det.RefersToUnitClassId) )
                {
                    UnitModelFileName = GameManager.Instance.GetUnitClass(det.RefersToUnitClassId).UnitModelFileName;
                }

                go = GetVesselByUnitClassId(UnitModelFileName, det.Position);
                go.name = det.RefersToUnitName;
                unit = go.AddComponent<Enemy>();
                unit.Size = new Vector2(32, 32);
                

                

                Vector3 pos = new Vector3();
                pos.x = ((float)det.Position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier) + GameManager.Instance.XMapAddition;
                pos.z = ((float)det.Position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier) + GameManager.Instance.YMapAddtion;
                pos.y = 30000;

                

                go.transform.parent = GameObject.Find("EnemyUnits").transform;
                

                if ( det.DomainType != GameConstants.DomainType.Air )
                {
                    go.transform.localScale = GameManager.Instance.GlobalScale;
                    GameObject healthBar = Instantiate(HealthBar, Vector3.zero, Quaternion.identity) as GameObject;
                    healthBar.transform.parent = go.transform;
                    healthBar.transform.localPosition = Vector3.up * 10;
                }

                GameObject detGo = new GameObject("Detection");
                DetectionMarker marker = detGo.AddComponent<DetectionMarker>();
                marker.Position = pos;
                marker.MinSize = new Vector2(8, 8);
                marker.MaxSize = new Vector2(128, 128);
                marker.KillTime = 3;
                marker.FadeTime = 1.5f;
                marker.MessageToDisplay = new Message("Detection", GameManager.MessageTypes.Detection);

            }
            //~ MapEnemy mu = AddMapEnemy(unit, GameManager.Instance.GetTextureByUnitClassId("enemyDetection"));
            unit.Info = det;
            GameManager.Instance.UnitManager.AddEnemy(unit);
        }
    }

    private void HandleUnitClass(IMarshallable dataReceived)
    {
        UnitClass unitClass = dataReceived as UnitClass;
        if ( GameManager.Instance.AddUnitClass(unitClass) )
        {
            Debug.Log("UnitClass Added");
        }
        else
        {
            Debug.Log("UnitClass not added. Already exists");
        }
    }

    private void HandleBaseUnitInfo(IMarshallable dataReceived)
    {
        //~ GameManager.Instance.MessageManager.AddMessage(dataReceived.ToString());

        BaseUnitInfo info = dataReceived as BaseUnitInfo;

        if (info != null)
        {
            PlayerUnit unit = GameManager.Instance.UnitManager.FindUnitById(info.Id);
            if (info.IsMarkedForDeletion)
            {
                //~ ShowMessage(string.Format("Unit [{0}] {1} has been DESTROYED.", info.Id, info.UnitName));
                ////////////REMOVE OBJECT///////////////////////
                Debug.Log(info + " is marked for deletion");
                if (unit.Info.UnitType == GameConstants.UnitType.Missile)
                {
                    //Instantiate explosives
                    unit.MyMapUnit.Explode();

                }
                Destroy(unit.gameObject);
                return;
            }
            //~ ShowMessage(info.UnitName + " " + info.IsMarkedForDeletion.ToString());

            GameObject go = null;

            if (unit != null)
            {
                unit.Info = info;
                return;
            }

            if (go == null)
            //Make new one and add to _GameUnits list
            {
                //Debug.Log(info.UnitClassId);
                UnitClass vesselUnitClass = GameManager.Instance.GetUnitClass(info.UnitClassId);
                if ( vesselUnitClass == null )
                {
                    Debug.Log("Unitclass is NuLLL!!!!");
                }
                go = GetVesselByUnitClassId(vesselUnitClass.UnitModelFileName, info.Position);
                
                unit = go.AddComponent<PlayerUnit>();
                unit.Size = new Vector2(32, 32);

                //unit.OptionalAngleTest = optionalAngle;

                unit.PrefabMapUnit = MapUnit;

                go.transform.parent = GameObject.Find("SurfaceUnits").transform;
                if ( info.DomainType != GameConstants.DomainType.Land && info.DomainType != GameConstants.DomainType.Air )
                {
                    go.transform.localScale = GameManager.Instance.GlobalScale;
                }
                go.name = info.UnitName;

                if ( info.DomainType != GameConstants.DomainType.Air )
                {
                    GameObject healthBar = Instantiate(HealthBar, Vector3.zero, Quaternion.identity) as GameObject;
                    healthBar.transform.parent = go.transform;
                    healthBar.transform.localPosition = Vector3.up * 10;
                }
                //~ if(map != null)
                //~ {
                //~ map.UnitInfo = unit;
                //~ }
                //~ GameObject clone = Instantiate(ABurke, GameManager.Instance.GetVectorByLatLng((float)info.Position.Latitude, (float)info.Position.Longitude), Quaternion.identity) as GameObject;
                //~ Debug.Log(string.Format("Adding unit {0} to list", unit.Info.Id)); 

            }

            //Debug.Log(info.Id + " created");
            unit.Info = info;
            //~ MapUnit mu = AddMapUnit(unit, GameManager.Instance.GetTextureByUnitClassId(unit.Info.UnitClassId));
            //~ unit.MapUnit = mu;
            GameManager.Instance.UnitManager.AddUnit(unit);
            if ( go.audio != null )
            {
                GameManager.Instance.AudioManager.AddSound(go.audio);
            }

            if (GameManager.Instance.UnitManager.SelectedUnit == null)
            {
                GameManager.Instance.UnitManager.SelectedUnit = unit;
            }
        }
    }


    private GameObject GetVesselByUnitClassId(string unitModelFilename, PositionInfo position)
    {
        GameObject vessel = Resources.Load(string.Format("Prefabs/{0}", unitModelFilename)) as GameObject;

        return Instantiate(vessel, CoordinateHelper.GetPositionByPositionInfo(position), Quaternion.identity) as GameObject;;
    }


    #endregion



}
