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
    public class User : IMarshallable
    {
        #region "Constructors"

        public User()
        {
            UserPlayedScenarios = new List<UserPlayedScenario>();
        }

        #endregion


        #region "Public properties"

        private string _UserId;
        public string UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                //TODO: Validation?
                _UserId = value;
            }
        }

        public List<UserPlayedScenario> UserPlayedScenarios { get; set; }

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.User; }
        }

        #endregion
    }
}
