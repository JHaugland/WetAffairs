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
    public class DetectedUnitTargetted
    {
        #region "Constructors"

        public DetectedUnitTargetted()
        {
            if (GameManager.Instance.Game != null)
            {
                TargettedGameWorldTimeSec = GameManager.Instance.Game.GameWorldTimeSec;
            }
        }

        public DetectedUnitTargetted(BaseWeapon weaponFired, BaseUnit unitTargetting) : this()
        {
            WeaponFired = weaponFired;
            UnitTargetting = unitTargetting;
        }

        #endregion


        #region "Public properties"

        public BaseWeapon WeaponFired { get; set; }

        public BaseUnit UnitTargetting { get; set; }

        public double TargettedGameWorldTimeSec { get; set; }

        public DateTime TargettedGameTime
        {
            get
            {
                return GameManager.Instance.Game.GameStartTime.AddSeconds(
                    GameManager.Instance.Game.GameWorldTimeSec);
            }
        }


        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = "DetectedUnitTargetted ";
            if (UnitTargetting != null)
            {
                temp += " Unit " + UnitTargetting.ToShortString();
            }
            if (WeaponFired != null)
            {
                temp += " Weapon " + WeaponFired.ToString();
            }
            temp += " at " + TargettedGameTime.ToLongTimeString();
            //targetted by wpn umit at time 
            return temp;
        }
        #endregion


    }
}
