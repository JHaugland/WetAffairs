using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;
namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class CriticalDamage 
    {
        #region "Constructors"

        public CriticalDamage()
        {

        }

        #endregion



        #region "Public properties"

        public GameConstants.CriticalDamageType CriticalDamageComponentType { get; set; }

        public string ComponentId { get; set; }

        public double ReadyInSec { get; set; }

        public GameConstants.FireLevel FireLevel { get; set; }

        /// <summary>
        /// A non-serialized field used on the client side to hold name of component used in GUI.
        /// The client resolves this property upon receiving battle damage report.
        /// </summary>
        [NonSerialized]
        public string ComponentGUIName;

        /// <summary>
        /// Another non-serialized field. This one is used for tooltips in GUI.
        /// </summary>
        [NonSerialized]
        public string ComponentGUITooltip;

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = "Critical Damage: ";
            switch (CriticalDamageComponentType)
            {
                case GameConstants.CriticalDamageType.WeaponDamaged:
                    temp += "Weapon " + ComponentId ;
                    break;
                case GameConstants.CriticalDamageType.SensorDamaged:
                    temp += "Sensor " + ComponentId;
                    break;
                case GameConstants.CriticalDamageType.AircraftHangarDamaged:
                    temp += "Aircraft facilities";
                    break;
                case GameConstants.CriticalDamageType.Fire:
                    temp += FireLevel.ToString();
                    break;
                default:
                    break;
            }
            return temp;
        }

        #endregion


    }
}
