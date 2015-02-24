using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class BattleDamageReport : IMarshallable
    {
        #region "Constructors"

        public BattleDamageReport() : base()
        {
            CriticalDamageList = new List<CriticalDamage>();
            TargetPlatformRoles = new List<GameConstants.Role>();
        }

        #endregion


        #region "Public properties"

        public string PlayerInflictingDamageId { get; set; }

        public string PlayerSustainingDamageId { get; set; }

        //public string PlayerInflictingDamageName { get; set; }

        //public string PlayerSustainingDamageName { get; set; }

        public string PlatformInflictingDamageId { get; set; }

        //public string PlatformInflictingDamageDetectedId { get; set; }

        //public string PlatformInflictingDamageClassName { get; set; }

        public string PlatformInflictingDamageUnitName { get; set; }

        public string PlatformInflictingDamageClassId { get; set; }

        public string TargetPlatformId { get; set; }

        //public string TargetPlatformDetectedId { get; set; }

        //public string TargetPlatformClassName { get; set; }

        public string TargetPlatformUnitName { get; set; }

        public string TargetPlatformClassId { get; set; }

        public string MissileId { get; set; }

        public bool IsTargetCivilianUnit { get; set; }

        public bool IsTargetNeutralUnit { get; set; }

        //public string MissileDetectedUnitId { get; set; }

        public GameConstants.Priority AttackerPriority { get; set; }

        public GameConstants.Priority AttackeePriority { get; set; }

        //public string MessageToAttacker { get; set; }

        //public string MessageToAttackee { get; set; }

        public PositionInfo Position { get; set; }

        //public double AbsoluteBearingAttackFromDeg { get; set; }

        /// <summary>
        /// This list is only used in the backend and not sent over network.
        /// </summary>
        [NonSerialized]
        public List<GameConstants.Role> TargetPlatformRoles;

        public List<CriticalDamage> CriticalDamageList { get; set; }

        public string WeaponClassId { get; set; }

        //public string WeaponClassName { get; set; }

        //public int DamageHitpoints { get; set; }

        public int DamagePercent { get; set; }

        public bool IsTargetHit { get; set; }

        public bool IsTargetPlatformDestroyed { get; set; }

        public bool IsMissileOutOfFuel { get; set; }

        public bool IsProjectileSoftKilled { get; set; }

        //public DateTime TimeStamp { get; set; }

        public double GameTimeSec { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = string.Format(
                "Player {0} unit {1} fires weapon {2} at player {3} unit {4}: Damage {5} %.",
                PlayerInflictingDamageId, PlatformInflictingDamageUnitName, WeaponClassId,
                PlayerSustainingDamageId, TargetPlatformUnitName, DamagePercent);
            if (CriticalDamageList != null && CriticalDamageList.Count > 0)
            {
                foreach (var cd in CriticalDamageList)
                {
                    temp += "\n" + cd;
                }
            }
            if (!IsTargetHit)
            {
                temp += "\nMissed!";
            }
            if (IsTargetPlatformDestroyed)
            {
                temp += "\nTarget destroyed!";
            }
            return temp;
        }

        public void RemoveAttackingPlatformData()
        {
            this.PlatformInflictingDamageClassId = string.Empty;
            //this.PlatformInflictingDamageClassName = string.Empty;
            this.PlatformInflictingDamageId = string.Empty;
            this.PlatformInflictingDamageUnitName = string.Empty;
            //this.PlatformInflictingDamageDetectedId = string.Empty;
        }

        public BattleDamageReport Clone()
        {
            var serhelper = new TTG.NavalWar.NWComms.CommsSerializationHelper<BattleDamageReport>();
            byte[] bytes = serhelper.SerializeToBytes(this);

            BattleDamageReport bdr = serhelper.DeserializeFromBytes(bytes);
            bdr.TargetPlatformRoles = new List<GameConstants.Role>(this.TargetPlatformRoles);
            return bdr;
        }
        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.BattleDamageReport; }
        }

        #endregion
    }
}
