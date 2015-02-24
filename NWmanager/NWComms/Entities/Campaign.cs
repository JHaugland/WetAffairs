using System;
using System.Collections.Generic;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class Campaign :IMarshallable, IGameDataObject
    {
        #region "Constructors"

        public Campaign()
        {
            CampaignScenarios = new List<CampaignScenario>();
        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public string PreviewImage { get; set; }

        public List<CampaignScenario> CampaignScenarios { get; set; }

        public NewsReport EndNews { get; set; }

        #endregion

        #region "Public methods"
        
        public override string ToString()
        {
            return string.Format("{0}  {1}", Id, Name);
        }

        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.Campaign; }
        }

        #endregion
    }
}
