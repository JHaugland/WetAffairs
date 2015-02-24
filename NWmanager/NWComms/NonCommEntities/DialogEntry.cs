using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class DialogEntry
    {
        public string CharacterId { get; set; }

        public string Dialog { get; set; }
    }
}
