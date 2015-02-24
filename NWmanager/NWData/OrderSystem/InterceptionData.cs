using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class InterceptionData
    {
        #region "Constructors"

        public InterceptionData()
        {

        }

        public InterceptionData(Position position, double timeToInterceptSec) : this()
        {
            Position = position;
            TimeToInterceptSec = timeToInterceptSec;
        }
        

        #endregion


        #region "Public properties"

        public Position Position { get; set; }

        public double TimeToInterceptSec { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            string temp = string.Empty;
            if (Position != null)
            {
                temp = Position.ToString() + " in " ;
            }

            temp += TimeToInterceptSec + " sec.";
            return temp;
        }
        #endregion


    }
}
