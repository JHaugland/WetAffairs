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

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class PositionRelationship
    {
        #region "Constructors"

        public PositionRelationship()
        {

        }

        #endregion


        #region "Public properties"

        public double DistanceM { get; set; }

        public GameConstants.RelativeBearing RelativeBearing { get; set; }


        /// <summary>
        /// Positive means first is faster, 0 means similar speed, negative means second is faster.
        /// </summary>
        public int RelativeSpeed { get; set; }

        #endregion



        #region "Public methods"

        #endregion


    }
}
