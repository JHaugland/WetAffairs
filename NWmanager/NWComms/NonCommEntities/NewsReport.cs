using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class NewsReport
    {
        #region "Constructors"

        public NewsReport()
        {
        }

        #endregion

        #region "Public properties"

        public NWDateTime Date { get; set; }

        public string Header { get; set; }

        public string SubHeader { get; set; }

        public string Body { get; set; }

        public string Image { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return string.Format( "NewsReport Header={0}, SubHeader={1}, Body={2}", Header, SubHeader, Body );
        }

        #endregion
    }
}
