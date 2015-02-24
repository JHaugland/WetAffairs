using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class Sonar : BaseSensor
    {
        #region "Constructors"

        public Sonar() : base()
        {

        }

        #endregion


        #region "Public properties"

        /// <summary>
        /// This setting determines whether the sonar is set to shallow (default for surface/air) or
        /// intermediate depth (below the layer). A sub will always listen on its own depth. 
        /// Dependent on SensorClass.IsVariableDepthSensor
        /// </summary>
        public bool IsDeployedIntermediateDepth { get; set; }

        public double CurrentEffectiveActiveDetectionRangeM { get; set; }

        public double CurrentEffectivePassiveDetectionRangeM { get; set; }
        
        #endregion



        #region "Public methods"

        public override SensorInfo GetSensorInfo()
        {
            SensorInfo info = base.GetSensorInfo();
            info.IsDeployedIntermediateDepth = this.IsDeployedIntermediateDepth;
            return info;
        }

        public override bool AttemptDetectUnit(BaseUnit unit, double distanceM)
        {
            // CZ etc: http://www.fas.org/man/dod-101/navy/docs/es310/SNR_PROP/snr_prop.htm
            //calculate blind zone:
            if (!IsAbleToDetectUnit(unit, distanceM))
            {
                return false;
            }
            double bearingDegSensor = GetCurrentSensorBearingDeg();
            double bearingDegToTarget = MapHelper.CalculateCombinedBearingDeg(bearingDegSensor, 
                MapHelper.CalculateBearingDegrees(OwnerUnit.Position.Coordinate, unit.Position.Coordinate));
            //needs to distinguish sensors: dipping, hull mounted, towed, etc:
            switch (this.SensorClass.SonarType) //accounting for baffle arcs ie blind zones
            {
                case GameConstants.SonarType.HullMountedBow:
                    if (MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 150, 210))
                    {
                        return false;
                    }
                    break;
                case GameConstants.SonarType.HullMountedKeel:
                    if (MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 135, 225))
                    {
                        return false;
                    }

                    break;
                case GameConstants.SonarType.HullMountedFlankArray: //assumes both starbord and port
                    if (MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 330, 30))
                    {
                        return false;
                    }
                    if (MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 150, 210))
                    {
                        return false;
                    }

                    break;
                case GameConstants.SonarType.DippingOrSonobuoy: //no bafflezones
                    if (OwnerUnit.ActualSpeedKph > 30) //these sonars do not operate if moving
                    {
                        return false;
                    }
                    break;
                case GameConstants.SonarType.TowedArray:
                    if (MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 315, 45)) //MapHelper.IsAngleWithinRangeDeg(bearingDegToTarget, 135, 225))
                    {
                        return false;
                    }
                    if (OwnerUnit.ActualSpeedKph > GameConstants.DEFAULT_SLOW_SPEED) //do not operate above 15 kph
                    {
                        return false; 
                    }
                    break;
                default:
                    break;
            }
            double noiseLevelTargetPercent = unit.GetCurrentNoiseLevelPercentage();
            double noiseLevelPlatformPercent = OwnerUnit.GetCurrentNoiseLevelPercentage();
            double detectionRangeM = 0;
            int currentSeaState = 0;
            var weather = unit.GetWeatherSystem();
            if (weather != null)
            {
                currentSeaState = weather.SeaState;
            }
            double sensorDepthM = (double)OwnerUnit.Position.HeightOverSeaLevelM - unit.UnitClass.DraftM;
            double targetDepthM = (double)unit.Position.HeightOverSeaLevelM;
            if (this.SensorClass.IsVariableDepthSensor && this.IsDeployedIntermediateDepth && sensorDepthM < GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M)
            {
                sensorDepthM = GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M - 10;
            }
            TerrainLineSummary terrainLineSummary = GetTerrainHeightSummaryToTarget(unit);
            if (sensorDepthM < terrainLineSummary.MaxHeightM && targetDepthM < terrainLineSummary.MaxHeightM)
            {
                return false; //higher terrain between objects - cannot detect
            }
            if (this.IsActive)
            {
                detectionRangeM = this.SensorClass.SonarActiveReferenceRangeM;
                if (OwnerUnit.Position.HeightOverSeaLevelM > GameConstants.DEPTH_SHALLOW_MIN_M ||
                    unit.Position.HeightOverSeaLevelM > GameConstants.DEPTH_SHALLOW_MIN_M)
                {
                    if (currentSeaState > 8)
                    {
                        detectionRangeM *= 0.1;
                    }
                    else if (currentSeaState > 6)
                    {
                        detectionRangeM *= 0.25;
                    }
                    else if (currentSeaState > 4)
                    {
                        detectionRangeM *= 0.5;
                    }
                }
                CurrentEffectiveActiveDetectionRangeM = detectionRangeM;
                if (terrainLineSummary.MaxHeightBehindM > targetDepthM)
                {
                    detectionRangeM *= 0.5;
                }
                if (unit.UnitClass.IsSonarShielded)
                {
                    detectionRangeM *= 0.5;
                }
                
                detectionRangeM = (detectionRangeM / noiseLevelPlatformPercent) * 100.0; //hmm
                double detectionStrength = detectionRangeM / distanceM;
                if (IsOnDifferentSidesOfThermalLayer(unit.Position)) //TODO: Calculate angle to layer and calculate detection based on it
                {
                    detectionStrength *= 0.5;
                }
                if (detectionStrength > 1)
                {
                    DetectedUnit detect = CreateOrUpdateDetectionReport(unit, detectionStrength, distanceM,
                        0, 0); //check!
                    return (detect != null);
                }

                //unit.UnitClass.IsSonarShielded
                //remember, cannot identify
            }
            else //passive detection
            {
                detectionRangeM = this.SensorClass.SonarPassiveReferenceRangeM;
                if (unit.HasActiveSonar)
                {
                    noiseLevelTargetPercent = 800;
                }
                detectionRangeM = (detectionRangeM / noiseLevelPlatformPercent) * 100.0; //hmm

                if (distanceM > detectionRangeM)
                {
                    return false;
                }
                if (sensorDepthM <= GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M 
                    && unit.Position.HeightOverSeaLevelM >= GameConstants.DEPTH_PERISCOPE_MIN_M) //sound channels
                {
                    detectionRangeM *= 3.0;
                }
                if (OwnerUnit.Position.HeightOverSeaLevelM > GameConstants.DEPTH_SHALLOW_MIN_M ||
                    unit.Position.HeightOverSeaLevelM > GameConstants.DEPTH_SHALLOW_MIN_M)
                {
                    if (currentSeaState > 8)
                    {
                        detectionRangeM *= 0.1;
                    }
                    else if (currentSeaState > 6)
                    {
                        detectionRangeM *= 0.25;
                    }
                    else if (currentSeaState > 4)
                    {
                        detectionRangeM *= 0.5;
                    }
                    else if (currentSeaState > 2)
                    {
                        detectionRangeM *= 0.8;
                    }
                }
                if (terrainLineSummary.MaxHeightBehindM > targetDepthM)
                {
                    detectionRangeM *= 0.5;
                }
                CurrentEffectivePassiveDetectionRangeM = detectionRangeM;
                detectionRangeM *= noiseLevelTargetPercent / 100.0;
                double detectionStrength = detectionRangeM / distanceM;
                if (IsOnDifferentSidesOfThermalLayer(unit.Position))
                {
                    detectionStrength *= 0.5;
                }
                if (detectionStrength > 1)
                {
                    DetectedUnit detect = CreateOrUpdateDetectionReport(unit, detectionStrength, distanceM,
                        0, 0); //check!
                    return (detect != null);
                }

                //can identify, but not fix
                
            }
            return false;
            //return base.AttemptDetectUnit(unit, distanceM);
        }

        private bool IsOnDifferentSidesOfThermalLayer(Position unitPosition)
        {
            if (!unitPosition.HasHeightOverSeaLevel)
            {
                return false; //no meaning
            }
            if (unitPosition.HeightOverSeaLevelM < GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M)
            {
                //unit is below thermal layer
                if (OwnerUnit.Position.HeightOverSeaLevelM <= GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M || this.IsDeployedIntermediateDepth)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (OwnerUnit.Position.HeightOverSeaLevelM > GameConstants.DEFAULT_DEPTH_THERMAL_LAYER_M && !this.IsDeployedIntermediateDepth)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion


    }
}
