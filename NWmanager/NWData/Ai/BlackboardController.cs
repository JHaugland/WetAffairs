using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Ai
{
    public static class BlackboardController
    {
        static List<IBlackboardObject> _BlackboardObjects = new List<IBlackboardObject>();

        public static List<IBlackboardObject> Objects 
        {
            get
            {
                return _BlackboardObjects;
            }
            set
            {
                _BlackboardObjects = value;
            }
        }
    }
}