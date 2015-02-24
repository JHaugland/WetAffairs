using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWData.Ai;


namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class BaseSensor : BaseComponent
    {

        public BaseSensor() : base()
        {
            
        }

        public BaseSensor(Player playerOwner, BaseUnit unitOwner) : this()
        {
            OwnerUnit = unitOwner;
            OwnerPlayer = playerOwner;
        }
        #region "Public properties"

        /// <summary>
        /// If SensorClass.IsPassiveActiveSensor is true, this determines whether sensor is emitting (true) or
        /// silent (false).
        /// </summary>
        public virtual bool IsActive { get; set; }

        public virtual bool IsDamaged { get; set; }
        
        public virtual string Description
        {
            get
            {
                string temp = string.Empty;
                if (SensorClass.IsPassiveActiveSensor)
                {
                    if (!IsActive)
                    {
                        temp = "Passive ";
                        if (SensorClass.SensorType == GameConstants.SensorType.Radar)
                        {
                            temp = "ESM on ";
                        }
                    }
                    else
                    {
                        temp = "Active ";
                    }
                }
                temp += Name;
                return temp;
            }
        }

        public virtual SensorClass SensorClass { get; set; }
        
        /// <summary>
        /// Direction sensor points, in degrees (0-360). 0 is forward on unit.
        /// </summary>
        public virtual double SensorBearingDeg { get; set; }

        #endregion

        #region "Public methods"

        public override void Tick(double timer)
        {
            base.Tick(timer);
            if (IsDamaged && ReadyInSec == 0)
            {
                IsDamaged = false;
            }
        }

        public double GetCurrentSensorBearingDeg()
        {
            return MapHelper.CalculateCombinedBearingDeg((double)OwnerUnit.Position.BearingDeg, this.SensorBearingDeg);
        }

        public virtual SensorInfo GetSensorInfo()
        {
            SensorInfo info = new SensorInfo();
            info.Id = Id;
            //info.Name = Name;
            info.IsOperational = IsOperational;
            
            //info.OwnerPlayerId = OwnerPlayer.Id;
            info.OwnerUnitId = OwnerUnit.Id;
            info.SensorClassId = SensorClass.Id;
            info.ReadyInSec = ReadyInSec;
            info.SensorType = SensorClass.SensorType;
            info.IsActive = IsActive;
            info.IsDamaged = this.IsDamaged;

            return info;
        }

        //public virtual void Sweep()
        //{
        //    if (!IsReady)
        //    {
        //        return;
        //    }
        //    if (OwnerUnit.CarriedByUnit != null)
        //    {
        //        return;
        //    }
        //    System.Diagnostics.Debug.Assert(OwnerPlayer != null, "BaseSensor.Sweep: Sensor OwnerPlayer should never be null.");
        //    System.Diagnostics.Debug.Assert(OwnerUnit != null, "BaseSensor.Sweep: Sensor OwnerUnit should never be null.");
        //    System.Diagnostics.Debug.Assert(SensorClass != null, "BaseSensor.Sweep: Sensor SensorClass should never be null.");
            

        //}

        public virtual bool AttemptDetectUnit(BaseUnit unit, double distanceM)
        {
            //TODO: Hit test on detection
            if (!IsAbleToDetectUnit(unit, distanceM))
            {
                return false;
            }
            if (SensorClass.MaxHeightDeployedM > 0 && OwnerUnit.ActualHeightOverSeaLevelM > SensorClass.MaxHeightDeployedM)
            {
                ReadyInSec = 120;
                return false;
            }
            if (SensorClass.MaxSpeedDeployedKph > 0 && OwnerUnit.ActualSpeedKph > SensorClass.MaxSpeedDeployedKph)
            {
                ReadyInSec = 120;
                return false;
            }
            //if (!OwnerPlayer.IsComputerPlayer)
            //{ 
            //    GameManager.Instance.Log.LogDebug(string.Format(
            //        "PLAYER {0}, UNIT {1}, SENSOR {2} attemtping to detect unit {3}.",
            //        OwnerPlayer.ToString(), OwnerUnit.ToString(), this.ToString(), unit.ToString()));
            //}
            GameConstants.DirectionCardinalPoints DirectionToTarget =
                MapHelper.CalculateBearingDegrees(OwnerUnit.Position.Coordinate,
                    unit.Position.Coordinate).ToCardinalMark();

            TerrainLineSummary terrainLineSummary = GetTerrainHeightSummaryToTarget(unit);

            //No sensor can see through terrain
            if (terrainLineSummary.MaxHeightM > OwnerUnit.Position.HeightOverSeaLevelM && terrainLineSummary.MaxHeightM > unit.Position.HeightOverSeaLevelM)
            {
                return false;
            }
            double targetApparentSizeArcSec = 
                unit.CalculateRadarCorrectedSizeArcSec(DirectionToTarget, distanceM);

            if (SensorClass.SensorType == GameConstants.SensorType.Visual)
            {
                if ((double)OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M 
                    || (double)unit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_SHALLOW_MIN_M) //under sea level
                {
                    return false;
                }
                double lineOfSightM = MapHelper.CalculateMaxLineOfSightM(
                    (double)OwnerUnit.Position.HeightOverSeaLevelM 
                    + OwnerUnit.UnitClass.HeightM, 
                    (double)unit.Position.HeightOverSeaLevelM 
                    + unit.UnitClass.HeightM);
                
                double minimumTargetSizeArcSec = SensorClass.MinimumTargetSurfaceSizeArcSec;
                if (unit.Position.HeightOverSeaLevelM > 10)
                {
                    minimumTargetSizeArcSec = SensorClass.MinimumTargetAirSizeArcSec;
                }
                if (distanceM <= lineOfSightM && targetApparentSizeArcSec >= minimumTargetSizeArcSec)
                {
                    double degradationPercent = 0;
                    WeatherSystem wsystem = OwnerUnit.GetWeatherSystem();
                    if (wsystem != null && SensorClass.SensorType == GameConstants.SensorType.Visual)
                    {
                        degradationPercent = 100 - wsystem.TotalLightPercent;
                    }

                    if (OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M
                        || unit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M) //only if any below cloud cover
                    {
                        degradationPercent += GameManager.Instance.GetRadarDegradationFromWeatherPercent(wsystem);
                    }
                    //double MinDetectableSizeArcSec = SensorClass.MinimumTargetSurfaceSizeArcSec;
                    minimumTargetSizeArcSec = minimumTargetSizeArcSec * (1.0 + (degradationPercent / 100));
                    double DetectionStrength = targetApparentSizeArcSec
                        / minimumTargetSizeArcSec; //(LineOfSightM - DistanceM) / LineOfSightM;
                    DetectedUnit detect = CreateOrUpdateDetectionReport(unit, DetectionStrength, distanceM,
                        targetApparentSizeArcSec, minimumTargetSizeArcSec);
                    return (detect != null);
                }
                else
                {
                    return false; //does not detect it 
                }
            }
            else if (SensorClass.SensorType == GameConstants.SensorType.Infrared)
            {
                if ((double)OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M
                    || (double)unit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_SHALLOW_MIN_M) //under sea level
                {
                    return false;
                }

                double irDegradationPercent = 0;
                WeatherSystem weatherSystem = OwnerUnit.GetWeatherSystem();
                if (OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M
                    || unit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M) //only if any below cloud cover
                {
                    irDegradationPercent += GameManager.Instance.GetIRDegradationFromWeatherPercent(weatherSystem);
                }

                double maxTargetDetectionDistanceM = unit.GetMaxIrDetectionDistanceM();
                double LineOfSightM = MapHelper.CalculateMaxLineOfSightM(
                    (double)OwnerUnit.Position.HeightOverSeaLevelM
                    + OwnerUnit.UnitClass.HeightM,
                    (double)unit.Position.HeightOverSeaLevelM)
                    + unit.UnitClass.HeightM;
                if (maxTargetDetectionDistanceM > LineOfSightM)
                {
                    maxTargetDetectionDistanceM = LineOfSightM;
                }
                double detectionStrength = distanceM / maxTargetDetectionDistanceM;
                if (detectionStrength > 1.0)
                {
                    DetectedUnit detect = CreateOrUpdateDetectionReport(unit, detectionStrength, distanceM,
                        0, 0);
                    return (detect != null);
                }
                else
                {
                    return false;
                }
            }
            else if (SensorClass.SensorType == GameConstants.SensorType.MAD)
            {
                if(unit.Position == null 
                    || !unit.Position.HasHeightOverSeaLevel 
                    || unit.Position.HeightOverSeaLevelM >= GameConstants.DEPTH_SHALLOW_MIN_M)
                {
                    return false;
                }
                var unitDepthM = (double)unit.Position.HeightOverSeaLevelM;
                if (OwnerUnit.Position == null || !OwnerUnit.Position.HasHeightOverSeaLevel
                    || OwnerUnit.Position.HeightOverSeaLevelM >= GameConstants.HEIGHT_MEDIUM_MIN_M)
                {
                    return false;
                }
                if (unitDepthM <= GameConstants.DEPTH_DEEP_MIN_M)
                {
                    return false;
                }
                double maxDetectionDistanceM = 1000.0;
                if (unitDepthM <= GameConstants.DEPTH_MEDIUM_MIN_M)
                {
                    maxDetectionDistanceM = 500;
                }
                double detectionStrength = distanceM / maxDetectionDistanceM;
                if (detectionStrength > 1.0)
                {
                    DetectedUnit detect = CreateOrUpdateDetectionReport(unit, detectionStrength, distanceM,
                        0, 0);
                    return (detect != null);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public virtual bool IsAbleToDetectUnit(BaseUnit unit)
        {
            if (unit == null || unit.IsMarkedForDeletion)
            {
                return false;
            }
            if (!IsReady)
            {
                return false;
            }
            if ( !IsOperational )
            {
                return false;
            }
            if (unit.DomainType == GameConstants.DomainType.Air && !SensorClass.CanTargetAir)
            {
                return false;
            }
            if (unit.DomainType == GameConstants.DomainType.Land && !SensorClass.CanTargetSurface)
            {
                return false;
            }
            if (unit.DomainType == GameConstants.DomainType.Surface && !SensorClass.CanTargetSurface)
            {
                return false;
            }
            if ((unit.Position.HasHeightOverSeaLevel && unit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M) && !SensorClass.CanTargetSubmarine)
            {
                return false;
            }
            return true;
        }

        public virtual bool IsAbleToDetectUnit(BaseUnit unit, double distanceM)
        {
            if ( distanceM > SensorClass.MaxRangeM )
                return false;
            return IsAbleToDetectUnit(unit);
        }

        public TerrainLineSummary GetTerrainHeightSummaryToTarget(BaseUnit targetUnit)
        {
            if ( OwnerUnit.Position.HeightOverSeaLevelM > GameConstants.MAX_HEIGHT_TERRAIN_M
                && targetUnit.Position.HeightOverSeaLevelM > GameConstants.MAX_HEIGHT_TERRAIN_M )
            {
                return new TerrainLineSummary( 100, 100, 0 ); //if both units are over terrain, no need to check
            }
            else
            {
                return TerrainReader.GetTerrainHeightSummary( OwnerUnit.Position.Coordinate,
                    targetUnit.Position.Coordinate, GameConstants.DEFAULT_NO_OF_POINTS_TERRAIN_LINE );
            }
        }

        public virtual DetectedUnit CreateOrUpdateDetectionReport(BaseUnit unit,
            double detectionStrength, double distanceM, double targetApparentSizeArcSec, double minDetectableSizeArcSec)
        {
            DetectedUnit detect = OwnerPlayer.DetectedUnits.Find(d => d.RefersToUnit.Id == unit.Id);
            DetectedUnitSensor sensor;

            if (detect != null) //detected previously
            {
                detect.IsMarkedForDeletion = false;
                sensor = detect.AddOrUpdateSensor(unit, this, detectionStrength, distanceM, 
                    targetApparentSizeArcSec, minDetectableSizeArcSec);
                detect.DetectedGameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
                if (!unit.Position.Equals(detect.Position))
                {
                    detect.SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
                    if (detect.IsFixed)
                    {
                        detect.Position = unit.Position.Clone(); //TODO: Add uncertainty if applicable    
                    }
                    
                }
                if (unit.DirtySetting == GameConstants.DirtyStatus.UnitChanged)
                {
                    detect.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }

                if (detectionStrength >= SensorClass.IdentifyDetectionStrength 
                    && !detect.IsFixed && !detect.IsIdentified)
                {
                    detect.IsFixed = true;
                    detect.IsIdentified = true;
                    detect.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                    if (this.OwnerPlayer.IsEnemy(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                    }
                    else if (OwnerPlayer.IsAlly(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Friend;
                    }
                    GameManager.Instance.Log.LogDebug("CreateOrUpdateDetectionReport: UPDATE: " + detect.ToLongString());
                }
                if (detect.IsIdentified)
                {
                    if (this.OwnerPlayer.IsEnemy(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                    }
                    else if (OwnerPlayer.IsAlly(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Friend;
                    }
                    //detect.SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }
                if (OwnerPlayer.AIHandler != null && detect.DirtySetting == GameConstants.DirtyStatus.UnitChanged)
                {
                    OwnerPlayer.AIHandler.DetectionUpdated(detect);
                }
                return detect; //otherwise unchanged
            }
            //NEW DETECTION:
            detect = new DetectedUnit();
            detect.OwnerPlayer = this.OwnerPlayer;
            
            detect.ThreatClassification = GameConstants.ThreatClassification.U_Undecided;
            sensor = detect.AddOrUpdateSensor(unit, this, detectionStrength, distanceM, 
                targetApparentSizeArcSec, minDetectableSizeArcSec);
            if (!unit.UnitClass.CanBeTargeted)
            {
                detect.CanBeTargeted = false;
            }
            if (detect.IsIdentified)
            {
                if (this.OwnerPlayer.IsEnemy(unit.OwnerPlayer))
                {
                    detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                }
                else
                { 
                    if (this.OwnerPlayer.IsAlly(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Friend;
                    }

                }
            }
            if (detect.FriendOrFoeClassification == GameConstants.FriendOrFoe.Undetermined)
            {
                if (detect.RefersToUnit != null && detect.RefersToUnit.UnitClass.IsMissileOrTorpedo) //missiles assumed hostile
                {
                    if (!this.OwnerPlayer.IsAlly(unit.OwnerPlayer))
                    {
                        detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                    }
                }
                if (detect.FriendOrFoeClassification == GameConstants.FriendOrFoe.Undetermined 
                    && !detect.IsIdentified
                    && OwnerPlayer.IsAllUnknownContactsHostile)
                {
                    detect.FriendOrFoeClassification = GameConstants.FriendOrFoe.Foe;
                }
            }
            GameManager.Instance.Log.LogDebug(string.Format(
                "CreateOrUpdateDetectionReport NEW Player: {0}: Detection {1} ", 
                OwnerPlayer.ToString(), detect.ToLongString()));

            if (!this.SensorClass.IsTargetingSensorOnly)
            {
                OwnerPlayer.DetectedUnits.Add(detect);
            }
            detect.SetDetectedGroup();
            if (OwnerPlayer.AIHandler != null)
            {
                OwnerPlayer.AIHandler.NewDetection(detect);
            }
            return detect;
        }

        public override string ToString()
        {
            return string.Format("Sensor [{0}] {1} Class: {2}  Type: {3}", 
                Id, Name, SensorClass.SensorClassName, SensorClass.SensorType.ToString());
        }
        #endregion


        #region "Private methods"


        #endregion

    }
}
