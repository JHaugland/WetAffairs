using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameScenarioCarriedUnit :IMarshallable
    {
        #region "Constructors"

        public GameScenarioCarriedUnit()
        {
            UnitCount = 1;
            SkillLevel = GameConstants.SkillLevelInclusion.IncludeAlways;
        }

        #endregion


        #region "Public properties"

        public string UnitClassId { get; set; }

        public int UnitCount { get; set; }

        public string Tag { get; set; }

        public GameConstants.SkillLevelInclusion SkillLevel { get; set; }

        public double ReadyInSec { get; set; }

        public string WeaponLoad { get; set; }

        public GameConstants.WeaponLoadType WeaponLoadType { get; set; }

        public GameConstants.WeaponLoadModifier WeaponLoadModifier { get; set; }

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenarioCarriedUnit; }
        }

        #endregion
    }
}
