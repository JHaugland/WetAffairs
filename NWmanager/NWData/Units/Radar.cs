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
    public class Radar : BaseSensor
    {
        #region "Constructors"

        public Radar() : base()
        {
            
        }

        #endregion


        #region "Public properties"

        #endregion



        #region "Public methods"

        //ESM detection calculator: http://www.y1pwe.co.uk/AppletCD/Prop2.htm

        public override bool AttemptDetectUnit(BaseUnit unit, double distanceM)
        {
            if (!IsAbleToDetectUnit(unit, distanceM))
            {
                return false;
            }
            if ((double)OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M
                || (double)unit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M) //under sea level
            {
                return false;
            }
            //TODO: Take into account over-the horizon capabilities of some radars
            double sensorHeightOverSeaLevelM = (double)OwnerUnit.Position.HeightOverSeaLevelM
                + OwnerUnit.UnitClass.HeightM;
            double targetMaxHeightOverSeaLevelM = (double)unit.Position.HeightOverSeaLevelM + unit.UnitClass.HeightM;
            double LineOfSightM = MapHelper.CalculateMaxRadarLineOfSightM(sensorHeightOverSeaLevelM, targetMaxHeightOverSeaLevelM);

            TerrainLineSummary terrainLineSummary = null;

            if (IsActive)
            {
                if (distanceM <= LineOfSightM)
                {
                    GameConstants.DirectionCardinalPoints direction =
                        MapHelper.CalculateBearingDegrees(OwnerUnit.Position.Coordinate,
                            unit.Position.Coordinate).ToCardinalMark();
                    double targetApparentSizeArcSec = unit.CalculateRadarCorrectedSizeArcSec(direction, distanceM);
                    double minDetectableSizeArcSec = SensorClass.MinimumTargetSurfaceSizeArcSec;
                    double degradationPercent = 0;
                    if (targetMaxHeightOverSeaLevelM > 90)
                    {
                        minDetectableSizeArcSec = SensorClass.MinimumTargetAirSizeArcSec;
                    }
                    else
                    {
                        degradationPercent = GameManager.Instance.GetRadarDegradationFromSeaStatePercent(
                            OwnerUnit.GetEffectiveSeaState());
                    }

                    terrainLineSummary = GetTerrainHeightSummaryToTarget( unit );
      
                    if (sensorHeightOverSeaLevelM < GameConstants.MAX_HEIGHT_TERRAIN_M)
                    {
                        if (sensorHeightOverSeaLevelM < terrainLineSummary.MaxHeightM && targetMaxHeightOverSeaLevelM < terrainLineSummary.MaxHeightM)
                        {
                            return false; //no radar detection through terrain
                        }
                        if (targetMaxHeightOverSeaLevelM < terrainLineSummary.MaxHeightM * 1.2 && terrainLineSummary.HeightVarianceM > 0)
                        {
                            degradationPercent += 10;
                        }
                    }
                    if (targetMaxHeightOverSeaLevelM < terrainLineSummary.MaxHeightBehindM)
                    {
                        degradationPercent += 75;
                    }
                    if (OwnerUnit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M
                        || unit.Position.HeightOverSeaLevelM < GameConstants.HEIGHT_MEDIUM_MIN_M) //only if any below cloud cover
                    {
                        degradationPercent += GameManager.Instance.GetRadarDegradationFromWeatherPercent(
                            unit.GetWeatherSystem());
                    }
                    degradationPercent += OwnerUnit.GetDegradationFromJammingPercent(this.SensorClass.SensorType);
                    if (unit.UnitClass.UnitType == GameConstants.UnitType.Missile && unit.Position.HeightOverSeaLevelM < 20)
                    {
                        degradationPercent += 40; //make sea skimming missiles much harder to detect
                    }
                    if (degradationPercent > 100)
                    {
                        degradationPercent = 100;
                    }
                    var degradationFactor = (1.0 - (degradationPercent / 100.0));
                    if (degradationFactor < 0.001)
                    {
                        degradationFactor = 0.001;
                    }
                    minDetectableSizeArcSec = minDetectableSizeArcSec * degradationFactor;
                    if (targetApparentSizeArcSec >= minDetectableSizeArcSec)
                    {
                        double detectionStrength = targetApparentSizeArcSec / minDetectableSizeArcSec; //(SensorClass.MaxRangeM - DistanceM) / SensorClass.MaxRangeM;
                        DetectedUnit detect = CreateOrUpdateDetectionReport(unit, detectionStrength,
                            distanceM, targetApparentSizeArcSec, minDetectableSizeArcSec);
                        if (detect != null)
                        {
                            return true;
                        }
                    }
                }
            } //Even if active detection attempt has been made and failed, test for esm (passive)
            if (SensorClass.IsEsmDetector)
            { 
                GameConstants.EsmRadiationLevel esmRad = unit.GetCurrentEsmRadiation();
                if (esmRad == GameConstants.EsmRadiationLevel.EsmNone)
                {
                    return false;
                }
                else
                {
                    LineOfSightM = (LineOfSightM * SensorClass.EsmDetectionOverHorizonPercent) / 100.0;
                    if (esmRad == GameConstants.EsmRadiationLevel.EsmLow)
                    {
                        LineOfSightM *= 0.1;
                    }
                    else if (esmRad == GameConstants.EsmRadiationLevel.EsmMedium)
                    {
                        LineOfSightM *= 0.25;
                    }
                    if (sensorHeightOverSeaLevelM < GameConstants.MAX_HEIGHT_TERRAIN_M)
                    {
                        // Get terrain summary if not already gotten
                        if ( terrainLineSummary == null )
                        {
                            terrainLineSummary = GetTerrainHeightSummaryToTarget( unit );
                        }

                        if (sensorHeightOverSeaLevelM < terrainLineSummary.MaxHeightM && targetMaxHeightOverSeaLevelM < terrainLineSummary.MaxHeightM)
                        {
                            return false; //no radar detection through terrain
                        }
                        if (targetMaxHeightOverSeaLevelM < terrainLineSummary.MaxHeightM * 1.2 && terrainLineSummary.HeightVarianceM > 0)
                        {
                            LineOfSightM *= 0.75;
                        }
                    }
                    if (unit.UnitClass.UnitType == GameConstants.UnitType.Missile && unit.Position.HeightOverSeaLevelM < 20) //sea skimmers
                    {
                        LineOfSightM *= 0.25;
                    }
                    var degrJammingPercent = OwnerUnit.GetDegradationFromJammingPercent(this.SensorClass.SensorType);
                    LineOfSightM *= (100.0 - degrJammingPercent);

                    if (distanceM <= LineOfSightM)
                    {
                        DetectedUnit detect = CreateOrUpdateDetectionReport(unit, (LineOfSightM / distanceM),
                            distanceM, 3600, 60);
                        return (detect != null);
                    }
                }
            }
            return false;
        }

        public override bool IsAbleToDetectUnit( BaseUnit unit )
        {
            if (unit.Position.HasHeightOverSeaLevel && unit.Position.HeightOverSeaLevelM < GameConstants.DEPTH_PERISCOPE_MIN_M)
            {
                return false;
            }
            if ( base.IsAbleToDetectUnit( unit ) )
            {
                return true;
            }            
            if ( IsActive )
            {
                return true;
            }
            if ( SensorClass.IsEsmDetector )
            {
                GameConstants.EsmRadiationLevel esmRad = unit.GetCurrentEsmRadiation();
                if ( esmRad == GameConstants.EsmRadiationLevel.EsmNone )
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion


    }
}
