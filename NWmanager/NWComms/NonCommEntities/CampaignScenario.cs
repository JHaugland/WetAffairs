using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class CampaignScenario
    {
        #region "Constructors"

        public CampaignScenario()
        {
            //RequiresCompletedScenarioIds = new List<string>();
        }

        #endregion


        #region "Public properties"

        //public int Ordinal { get; set; }

        public string ScenarioId { get; set; }

        public string GameScenarioPlayerId { get; set; }

        //public List<string> RequiresCompletedScenarioIds { get; set; }

        public NewsReport News { get; set; }

        public List<DialogEntry> DialogTree { get; set; }

        public string DialogBackgroundImage { get; set; }

        public string DialogBackgroundSound { get; set; }

        #endregion



        #region "Public methods"
        public override string ToString()
        {
            //return string.Format("CampaignScenario Id={0} #{1}", ScenarioId, Ordinal);
            return string.Format("CampaignScenario Id={0}", ScenarioId);
        }

        #endregion


    }
}
