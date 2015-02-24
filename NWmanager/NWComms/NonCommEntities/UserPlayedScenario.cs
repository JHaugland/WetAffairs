using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class UserPlayedScenario
    {
        #region "Constructors"

        public UserPlayedScenario()
        {
            TimeStamp = new NWDateTime(DateTime.Now);
        }

        #endregion


        #region "Public properties"

        public string CampaignId { get; set; }

        public string ScenarioId { get; set; }

        public NWDateTime TimeStamp { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            if (TimeStamp == null)
            {
                TimeStamp = new NWDateTime(DateTime.Now);
            }
            return string.Format("UserPlayedScenario Campaign {0}  Scenario {1} at  {2}", CampaignId, ScenarioId, TimeStamp) ;
        }
        #endregion


    }
}
