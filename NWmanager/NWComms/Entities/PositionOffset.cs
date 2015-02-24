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
    public class PositionOffset
    {
        #region "Constructors"

        public PositionOffset()
        {

        }

        public PositionOffset(double rightM, double forwardM): this()
        {
            RightM = rightM;
            ForwardM = forwardM;
        }

        public PositionOffset(double rightM, double forwardM, double upM): this()
        {
            RightM = rightM;
            ForwardM = forwardM;
            UpM = upM;

        }

        #endregion


        #region "Public properties"
        
        public double RightM { get; set; }

        public double ForwardM { get; set; }

        public double UpM { get; set; }

        #endregion



        #region "Public methods"

        public PositionOffset Clone()
        {
            return (PositionOffset)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("{0:F}M to the right, {1:F}M forward, {2:F}M up", RightM, ForwardM, UpM);
        }
        #endregion


    }
}
