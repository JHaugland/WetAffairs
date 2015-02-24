using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms.NonCommEntities;
//using TTG.NavalWar.NWData.Entities;
//using TTG.NavalWar.NWData;
//using TTG.NavalWar.NWData.GamePlay;
//using TTG.NavalWar.NWData.Util;
//using TTG.NavalWar.NWData.OrderSystem;
//using TTG.NavalWar.NWData.Units;
//using TTG.NavalWar.NWComms;
//using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameScenarioUnit :IMarshallable
    {
        #region "Constructors"

        public GameScenarioUnit()
        {
            SkillLevel = GameConstants.SkillLevelInclusion.IncludeAlways;
            CarriedUnits = new List<GameScenarioCarriedUnit>();
            CarriedWeaponStore = new List<WeaponStoreEntry>();
            InitialOrders = new List<UnitOrder>();
            IncludeDefaultCarriedUnits = true;
        }

        #endregion


        #region "Public properties"

        public string UnitClassId { get; set; }

        public string UnitName { get; set; }

        public List<GameScenarioCarriedUnit> CarriedUnits { get; set; }

        public List<WeaponStoreEntry> CarriedWeaponStore { get; set; }

        public bool IncludeDefaultCarriedUnits { get; set; }

        public bool IsCivilianUnit { get; set; }

        //public int UnitCount { get; set; }

        public string Tag { get; set; }

        public GameConstants.SkillLevelInclusion SkillLevel { get; set; }

        public double ReadyInSec { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifier { get; set; }

        public string WeaponLoad { get; set; }

        public double ActualSpeedKph { get; set; }

        public List<UnitOrder> InitialOrders { get; set; }

        /// <summary>
        /// If FormationPositionId is included, PositionOffset 
        /// is ignored. If UnitCount > 1, this will be ignored.
        /// </summary>
        public string FormationPositionId { get; set; }

        /// <summary>
        /// Offset relative to group Position.
        /// </summary>
        public PositionOffset PositionOffset { get; set; }

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenarioUnit; }
        }

        #endregion
    }
}
