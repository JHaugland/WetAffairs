using System;
using System.Collections.Generic;

using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TTG.NavalWar.NWComms;


[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitClassWeaponLoads //: IEnumerable<UnitClassWeaponLoad>//, ISerializable
    {
        private List<UnitClassWeaponLoad> _UnitClassWeaponLoad = new List<UnitClassWeaponLoad>();
        private List<GameConstants.Role> _AdditionalRoles = new List<GameConstants.Role>();

        #region "Constructors"

        public UnitClassWeaponLoads()
        {
        }

        public UnitClassWeaponLoads(string name) 
            : this()
        {
            Name = name;
        }

        public UnitClassWeaponLoads(string name, double timeToReloadHour)
            : this()
        {
            Name = name;
            TimeToReloadHour = timeToReloadHour; // * 3600;
        }


        #endregion


        #region "Public properties"

        /// <summary>
        /// Descriptive name for this loadout, for example ferry, air superiority, long range AA, 
        /// strike, airfield bombing, etc.
        /// </summary>
        public string Name { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifyer { get; set; }
        /// <summary>
        /// The number of levels by which this load increases or decreases (negative value) the maxiumum range
        /// of the unit. 
        /// </summary>
        public int IncreasesLoadRangeByLevels { get; set; }

        /// <summary>
        /// In case of carried droptanks, the percentage this load INCREASES cruise range. Use negative numbers for decrease (heavy load). 
        /// NOTE: Changed from IncreasesCruiseRangeM to make creating changes easier.
        /// </summary>
        public int IncreasesCruiseRangePercent { get; set; }

        /// <summary>
        /// The rate with which this load increases the radar cross section of the unit. 0 is none,
        /// 1 goes from stealth to small, from small to medium, etc. 
        /// </summary>
        public int IncreasesRadarCrossSection { get; set; }

        /// <summary>
        /// Time in hours it takes to reload unit (normally a plane) before it is again ready
        /// to be deployed.
        /// </summary>
        public double TimeToReloadHour { get; set; }

        /// <summary>
        /// Time in hours it takes to *change* loadout on a unit (normally a plane on the ground)
        /// before it is ready to be deployed with the new WeaponLoad configuration.
        /// </summary>
        public double TimeToChangeLoadoutHour { get; set; }

        public UnitClassWeaponLoad this[int index] //Default index properties... Hmmm...
        {
            get
            {
                return _UnitClassWeaponLoad[index];
            }
        }

        public virtual List<UnitClassWeaponLoad> WeaponLoads
        {
            set
            {
                _UnitClassWeaponLoad = value;
            }
            get
            {
                return _UnitClassWeaponLoad;
            }
        }

        //public virtual List<GameConstants.Role> AdditionalRoles
        //{
        //    get
        //    {
        //        return _AdditionalRoles;
        //    }
        //    set
        //    {
        //        _AdditionalRoles = value;
        //    }
        //}

        #endregion



        #region "Public methods"



        public virtual void Add(UnitClassWeaponLoad unitClassWeaponLoad)
        {
            _UnitClassWeaponLoad.Add(unitClassWeaponLoad);
        }

        public virtual void Remove(UnitClassWeaponLoad unitClassWeaponLoad)
        {
            _UnitClassWeaponLoad.Remove(unitClassWeaponLoad);
        }

        public List<UnitClassWeaponLoad>.Enumerator GetEnumerator()
        {
            return _UnitClassWeaponLoad.GetEnumerator();

        }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }
            else
            {
                return "Unnamed WeaponLoads";
            }

        }


        #endregion



    }
}
