using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms
{

    public static class MathsHelper
    {
        #region "Public static methods"



        public static string ToDegMinSecString(this double degrees)
        {
            var d = Math.Abs(degrees);  // (unsigned result ready for appending compass dir'n)
            d += 1 / 7200;  // add ½ second for rounding
            double deg = Math.Floor(d);
            double min = Math.Floor((d - deg) * 60);
            double sec = Math.Floor((d - deg - min / 60) * 3600);
            string sdeg = deg.ToString();
            string smin = min.ToString();
            string ssec = sec.ToString();

            // add leading zeros if required
            if (deg < 100)
            {
                sdeg = '0' + sdeg;
            }
            if (deg < 10)
            {
                sdeg = '0' + sdeg;
            }
            if (min < 10)
            {
                smin = '0' + smin;
            }
            if (sec < 10)
            {
                ssec = '0' + ssec;
            }
            return string.Format("{0}\u00B0 {1}' {2}\"", sdeg, smin, ssec);

        }

        #endregion


    }
}
