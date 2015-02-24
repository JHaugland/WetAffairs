using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class BaseUnitInfo : IMarshallable
    {
        #region "Constructors"

        public BaseUnitInfo()
        {
            // DomainType = GameConstants.DomainType.Surface;
            Weapons = new List<WeaponInfo>();
            Sensors = new List<SensorInfo>();
            CarriedUnits = new List<CarriedUnitInfo>();
            Waypoints = new List<WaypointInfo>();
            SupportsOrderType = new List<GameConstants.OrderType>();
            RoleList = new List<GameConstants.Role>();
            SupportsSpecialOrders = new List<GameConstants.SpecialOrders>();
            CarriedWeaponStores = new List<WeaponStoreEntry>();
            OrderQueue = new List<BaseOrderInfo>();
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string UnitName { get; set; }

        public string UnitClassId { get; set; }

        //public GameConstants.DomainType DomainType { get; set; }

        //public string Tag { get; set; }

        //public int HitPoints { get; set; }

        public bool IsCivilianUnit { get; set; }

        public int DamagePercent { get; set; }

        public string CarriedByUnitId { get; set; }

        public GameConstants.UnitSpeedType UserDefinedSpeed { get; set; }

        public bool IsUserDefinedElevationSet { get; set; }

        public GameConstants.HeightDepthPoints UserDefinedElevation { get; set; }

        /// <summary>
        /// For aircrafts, missiles and torpedoes, Id of launching platform
        /// </summary>
        public string LaunchPlatformId { get; set; }

        /// <summary>
        /// For missiles and torpedoes, launching weapon class id
        /// </summary>
        public string LaunchWeaponClassId { get; set; }
        
        public GameConstants.MissionType MissionType { get; set; }

        public GameConstants.MissionTargetType MissionTargetType { get; set; }

        public string TargetDetectedUnitId { get; set; }

        public string GroupId { get; set; }

        public bool IsGroupMainUnit { get; set; }

        public bool IsMarkedForDeletion { get; set; }

        public float ReadyInSec { get; set; }

        public float TimeToLiveRemainingSec { get; set; }

        public PositionInfo Position { get; set; }

        /// <summary>
        /// If part of a formation
        /// </summary>
        public PositionOffset PositionOffset { get; set; }

        //public PositionInfo Destination { get; set; }

        public List<WaypointInfo> Waypoints { get; set; }

        public List<GameConstants.SpecialOrders> SupportsSpecialOrders { get; set; }

        public WeatherSystemInfo WeatherSystem { get; set; }

        public float ActualSpeedKph 
        { 
            get
            {
                if (Position != null)
                {
                    return Position.ActualSpeedKph;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (Position != null)
                {
                    Position.ActualSpeedKph = value;
                }
            }

        }

        //public GameConstants.UnitSpeedType ActualSpeed 
        //{ 
        //    get
        //    {
        //        if (Position != null)
        //        {
        //            return Position.ActualSpeed;
        //        }
        //        else
        //        {
        //            return GameConstants.UnitSpeedType.UnchangedDefault;
        //        }
        //    }
        //    set
        //    {
        //        if (Position != null)
        //        {
        //            Position.ActualSpeed = value;
        //        }
        //    }
        //}

        /// <summary>
        /// Calculated max speed, calculated based on damage and weather.
        /// </summary>
        public float CalculatedMaxSpeedKph { get; set; }

        public List<BaseOrderInfo> OrderQueue { get; set; }

        public float CalculatedMaxRangeCruiseM { get; set; }

        //public bool HasLightingOn { get; set; }

        public string CurrentWeaponLoadName { get; set; }

        public string FormationPositionId { get; set; }

        public GameConstants.FireLevel FireLevel { get; set; }

        public bool IsUsingActiveRadar { get; set; }

        public bool IsUsingActiveSonar { get; set; }

        public bool HasFormationOrder { get; set; }

        //public bool IsAtFormationPosition { get; set; }

        //public double MaxWeaponRangeAirM { get; set; }

        //public double MaxWeaponRangeSurfaceM { get; set; }

        //public double MaxWeaponRangeLandM { get; set; }

        //public double MaxWeaponRangeSubM { get; set; }

        public List<CarriedUnitInfo> CarriedUnits { get; set; }

        public List<WeaponInfo> Weapons { get; set; }

        public List<SensorInfo> Sensors { get; set; }

        public List<GameConstants.OrderType> SupportsOrderType { get; set; }

        public List<WeaponStoreEntry> CarriedWeaponStores { get; set; }

        public List<GameConstants.Role> RoleList { get; set; }

        public GameConstants.WeaponOrders WeaponOrders { get; set; }

        public GameConstants.UnitSubType UnitSubType { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = string.Format("[{0}] {1} ({2})", Id, this.UnitName, this.UnitClassId);
            if (this.IsGroupMainUnit)
            {
                temp += " *";
            }
            return temp;
        }

        public virtual string ToShortString()
        {
            return ToString();
        }

        public virtual string ToLongString()
        {
            string posString = "(no position)";
            if (Position != null)
            {
                posString = Position.ToLongString();
            }
            else if (!string.IsNullOrEmpty(CarriedByUnitId))
            {
                posString = "(carried by " + CarriedByUnitId + ")";
            }
            string temp = ToShortString() + string.Format(" at {0}.", posString);
            if (!string.IsNullOrEmpty(this.GroupId))
            {
                temp += "\nGroup " + GroupId;
            }
            if (IsGroupMainUnit)
            {
                temp += "  Group Leader";
            }
            if (Position != null && Position.BingoFuelPercent > 0)
            {
                temp += string.Format("\nBingo fuel: {0}%", Position.BingoFuelPercent);
            }
            if (DamagePercent > 0)
            {
                temp += string.Format("\nDamage: {0} %", DamagePercent);
            }
            if (Position != null && Position.EtaAllWaypointsSec > 0)
            {
                temp += string.Format("\nETA: {0}", Position.EtaAllWaypoints);
            }
            return temp;
        }


        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.BaseUnitInfo; }
        }

        #endregion
    }
}
