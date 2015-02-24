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

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class DateTimeFromTo
    {
        #region "Constructors"

        public DateTimeFromTo()
        {

        }

        public DateTimeFromTo(DateTime? fromTime, DateTime? toTime)
        {
            FromTime = fromTime;
            ToTime = toTime;
        }

        #endregion


        #region "Public properties"

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        #endregion



        #region "Public methods"

        #endregion


    }

    public class Vector2
    {
        public Vector2()
        {

        }
        public Vector2(double value1, double value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public double Value1 { get; set; }
        public double Value2 { get; set; }
    }

    public class MoonRiseResult
    {
        public MoonRiseResult()
        {

        }

        public DateTime RiseTime { get; set; }

        public DateTime SetTime { get; set; }

        public bool IsMoonRise { get; set; }

        public bool IsMoonSet { get; set; }

        public double RiseAz { get; set; }

        public double SetAZ { get; set; }

        public double Vhz { get; set; }

    }
}
