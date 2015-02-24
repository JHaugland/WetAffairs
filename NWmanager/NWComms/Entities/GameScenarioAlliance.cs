using System;
using System.Collections.Generic;

using System.Text;
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
    public class GameScenarioAlliance :IMarshallable
    {
        #region "Constructors"

        public GameScenarioAlliance()
        {
            IsCompetitivePlayer = true;
            ScenarioPlayers = new List<GameScenarioPlayer>();
        }

        #endregion


        #region "Public properties"

        /// <summary>
        /// Unique string; must be set!
        /// </summary>
        public string Id { get; set; }

        public string Tag { get; set; }

        public List<GameScenarioPlayer> ScenarioPlayers { get; set; }

        public string Description { get; set; }

        public bool IsCompetitivePlayer { get; set; }

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenarioAlliance; }
        }

        #endregion
    }
}
