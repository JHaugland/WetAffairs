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

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class AircraftHangar : BaseComponent
    {
        #region "Private variables"
        
        //   Storage,
        //   TankingAndRefitting,
        //   ReadyForTakeoff,
        //   Flying

        private List<AircraftUnit> _Aircraft = new List<AircraftUnit>();
        private double[,] _TimeStorageStatusTimeSec = new double[4, 4];

        #endregion

        #region "Constructors"


        public AircraftHangar() : base()
        {
            SetDefaultDockingStatusChangeTimes();
            RunwayStyle = GameConstants.RunwayStyle.Helicopter; //default
        }

        #endregion


        #region "Public properties"

        public GameConstants.RunwayStyle RunwayStyle { get; set; } 


        public virtual int MaxAircraft { get; set; }

        public virtual List<AircraftUnit> Aircraft
        {
            get
            {
                return _Aircraft;
            }
        }

        #endregion



        #region "Public methods"

        public int MaxUnitsDockingStatus(GameConstants.AircraftDockingStatus status)
        {
            //switch (status)
            //{
            //    case GameConstants.AircraftDockingStatus.Storage:
            //        return MaxAircraftStorage;
            //    case GameConstants.AircraftDockingStatus.TankingAndRefitting:
            //        return MaxAircraftTanking;
            //    case GameConstants.AircraftDockingStatus.ReadyForTakeoff:
            //        return MaxAircraftReadyForTakeoff;
            //    case GameConstants.AircraftDockingStatus.Flying:
            //        return int.MaxValue;
            //    default:
            //        return 0;
            //}
            return MaxAircraft;
        }

        public void SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus fromStatus, 
            GameConstants.AircraftDockingStatus toStatus, double seconds)
        {
            _TimeStorageStatusTimeSec[(int)fromStatus, (int)toStatus] = seconds;
        }

        public double GetStatusChangeTimeSec(GameConstants.AircraftDockingStatus fromStatus, 
            GameConstants.AircraftDockingStatus toStatus)
        {
            return _TimeStorageStatusTimeSec[(int)fromStatus, (int)toStatus];
        }

        //public bool ChangeAircraftDockingStatus(string unitCode, 
        //    GameConstants.AircraftDockingStatus dockingStatus)
        //{
        //    AircraftUnit unit = Aircraft.Find(a => a.Id == unitCode);
        //    if (unit == null || unit.AircraftDockingStatus == GameConstants.AircraftDockingStatus.Flying)
        //    {
        //        return false;
        //    }
        //    if (unit.AircraftDockingStatus == dockingStatus)
        //    { 
        //        //Already done, ignore
        //        return true;
        //    }
        //    int CountPlanesInState = Aircraft.Count<AircraftUnit>(a => a.AircraftDockingStatus == dockingStatus);
        //    int MaxPlanesState = MaxUnitsDockingStatus(dockingStatus);
        //    if (CountPlanesInState >= MaxPlanesState)
        //    {
        //        return false;
        //    }
        //    GameConstants.AircraftDockingStatus OldStatus = unit.AircraftDockingStatus;
        //    double TimeSec = GetStatusChangeTimeSec(OldStatus, dockingStatus);
        //    unit.ReadyInSec += TimeSec; 
        //    unit.AircraftDockingStatus = dockingStatus;
        //    return true;
        //}

        public void SetDefaultDockingStatusChangeTimes()
        {
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.Storage, 
                GameConstants.AircraftDockingStatus.TankingAndRefitting, 15 * 60);
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.TankingAndRefitting, 
                GameConstants.AircraftDockingStatus.Storage, 5 * 60);
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.TankingAndRefitting, 
                GameConstants.AircraftDockingStatus.ReadyForTakeoff, 5 * 60);
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.Storage, 
                GameConstants.AircraftDockingStatus.ReadyForTakeoff, 20 * 60); //adds both stages
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.ReadyForTakeoff, 
                GameConstants.AircraftDockingStatus.TankingAndRefitting, 10 * 60);
            SetStatusChangeTimeSec(GameConstants.AircraftDockingStatus.ReadyForTakeoff, 
                GameConstants.AircraftDockingStatus.Storage, 15 * 60);
        }

        #endregion


    }
}
