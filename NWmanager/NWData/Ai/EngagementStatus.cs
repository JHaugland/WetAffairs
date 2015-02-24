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

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class EngagementStatus
    {
        #region "Constructors"

        public EngagementStatus()
        {

        }
        public EngagementStatus(BaseUnit unit, GameConstants.EngagementStatusResultType engagementStatusResult)
        {
            Unit = unit;
            EngagementStatusResult = engagementStatusResult;
        }

        #endregion


        #region "Public properties"

        public BaseUnit Unit { get; set; }

        public BaseWeapon Weapon { get; set; }

        public DetectedUnit TargetDetectedUnit { get; set; }

        public double TargetHitpoints { get; set; }

        public double DistanceToCloseM { get; set; }

        public double DistanceToTargetM { get; set; }

        /// <summary>
        /// Depends on EngagementStatusResult, returns true if this weapon can be used against the 
        /// specific target if only range and sector is right. Returns false if weapon is damaged, out 
        /// of ammo, or wrong target type.
        /// </summary>
        public bool WeaponCanBeUsedAgainstTarget
        {
            get
            {
                if (Weapon == null)
                {
                    return true;
                }
                return (EngagementStatusResult == GameConstants.EngagementStatusResultType.ReadyToEngage
                    || EngagementStatusResult == GameConstants.EngagementStatusResultType.TooCloseToEngage
                    || EngagementStatusResult == GameConstants.EngagementStatusResultType.MustCloseToEngage
                    || EngagementStatusResult == GameConstants.EngagementStatusResultType.OutOfSectorRange);
            }
        }

        public GameConstants.EngagementStatusResultType EngagementStatusResult { get; set; }

        public int Score { get; set; }

        public Player OwnerPlayer
        {
            get
            {
                if (Unit != null)
                {
                    return Unit.OwnerPlayer;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion



        #region "Public methods"
        
        public override string ToString()
        {
            string temp = "EngagementStatus: ";
            if (TargetDetectedUnit != null)
            {
                temp += "Trgt:" + TargetDetectedUnit.ToString() + " ";
            }
            if(Weapon != null)
            {
                temp += "Wpn: " + Weapon.Name + " ";
            }
            temp += "(" + Score + ") " + ReportReason();
            return temp;
        }

        public string ReportReason()
        {
            string temp = EngagementStatusResult.ToString();
            switch (EngagementStatusResult)
            {
                case GameConstants.EngagementStatusResultType.Undetermined:
                    break;
                case GameConstants.EngagementStatusResultType.ReadyToEngage:
                    temp = "Ready to engage";
                    break;
                case GameConstants.EngagementStatusResultType.MustCloseToEngage:
                    temp = "Out of range";
                    break;
                case GameConstants.EngagementStatusResultType.TooCloseToEngage:
                    temp = "Too close to engage";
                    break;
                case GameConstants.EngagementStatusResultType.OutOfSectorRange:
                    temp = "Out of sector range";
                    break;
                case GameConstants.EngagementStatusResultType.NoWeapon:
                    temp = "Not a weapon";
                    break;
                case GameConstants.EngagementStatusResultType.CannotFireAtThisTarget:
                    temp = "Wrong target type";
                    break;
                case GameConstants.EngagementStatusResultType.OutOfAmmo:
                    temp = "Out of ammunition";
                    break;
                case GameConstants.EngagementStatusResultType.WeaponDamaged:
                    temp = "Weapon damaged";
                    break;
                default:
                    break;
            }
            return temp;
        }

        #endregion




    }
}
