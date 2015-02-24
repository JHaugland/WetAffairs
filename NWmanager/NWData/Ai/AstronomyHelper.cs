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
using System.Diagnostics;

namespace TTG.NavalWar.NWData.Ai
{
    public static class AstronomyHelper
    {
        public const double DEG_IN_RADIAN =    57.2957795130823;
        public const double HRS_IN_RADIAN =     3.819718634;
        public const double SEC_IN_DAY =        86400;
        public const double  FLATTEN =          0.003352813;   /* flattening of earth, 1/298.257 */
        public const double EQUAT_RAD =        6378137;    /* equatorial radius of earth, meters */
        public const double J2000 = 2451545;        /* Julian date at standard epoch */
        public const int eventPrecision = 15;
        public const double rise_altitude = -0.83;

        public const double eventPrecisionJD = ((double)eventPrecision / SEC_IN_DAY);


        // http://www.google.com/codesearch/p?hl=en&sa=N&cd=5&ct=rc#QbzkpHdjmYI/xtide-2.9/Skycal.cc&q=moonrise%20moonset
        #region "Public properties"

        #endregion



        #region "Public methods"


        public static DateTime JdToDateTime(this double julianDay)
        {
            int year, month, day, hour, minute;
            double second;
            GetGregorianDateTimeFromJulianDay(julianDay, out year, out month, out day,
                out hour, out minute, out second);
            DateTime dateTime = new DateTime(year, month, day, hour, minute,(int)second);
            return dateTime;
        }

        public static double DtToJulianDay(this DateTime date)
        {
            int Y = date.Year;
            int M = date.Month;
            int D = date.Day;
            long JDN = (1461 * (Y + 4800 + (M - 14) / 12)) / 4 +
                (367 * (M - 2 - 12 * ((M - 14) / 12))) / 12 - (3 *
                ((Y + 4900 + (M - 14) / 12) / 100)) / 4 + D - 32075;
            return JDN;


        }



        /// <summary>
        /// Gets gregorian date from integer julian day.</summary>
        public static void GetGregorianDateFromJulianDay(
                int julianDay, out int year, out int month, out int day)
        {
            // From http://en.wikipedia.org/wiki/Julian_day
            int J = julianDay;
            int j = J + 32044;
            int g = j / 146097;
            int dg = j % 146097;
            int c = (dg / 36524 + 1) * 3 / 4;
            int dc = dg - c * 36524;
            int b = dc / 1461;
            int db = dc % 1461;
            int a = (db / 365 + 1) * 3 / 4;
            int da = db - a * 365;
            int y = g * 400 + c * 100 + b * 4 + a;
            int m = (da * 5 + 308) / 153 - 2;
            int d = da - (m + 4) * 153 / 5 + 122;
            year = y - 4800 + (m + 2) / 12;
            month = (m + 2) % 12 + 1;
            day = d + 1;
        }

        /// <summary>
        /// Gets gregorian date time from doubleing point julian day.</summary>
        public static void GetGregorianDateTimeFromJulianDay(
                double julianDay, out int year, out int month, out int day,
                out int hour, out int minute, out double second)
        {
            // Integer julian days are at noon.
            int ijd = System.Convert.ToInt32(Math.Floor(julianDay + 0.5d));
            GetGregorianDateFromJulianDay(ijd, out year, out month, out day);

            double s = (julianDay + 0.5d - ijd) * 86400.0d;
            hour = System.Convert.ToInt32((Math.Floor(s / 3600d)));
            s -= hour * 3600d;
            minute = System.Convert.ToInt32((Math.Floor(s / 60d)));
            s -= minute * 60d;
            second = s;
        }

        /// <summary>
        /// Gets gregorian date from doubleing point julian day.</summary>
        public static void GetGregorianDateFromJulianDay(
                double julianDay, out int year, out int month, out int day)
        {
            int hour;
            int minute;
            double second;
            GetGregorianDateTimeFromJulianDay(julianDay, out year, out month, out day, out hour, out minute, out second);
        }



        // This function combines a few steps that are always used together to
        // find the altitude of the sun or moon.
        public static double altitude(double jd, double lat, double longit, bool lunar)
        {
            if (lunar)
            {
                double ra, dec, dist, sid;
                sid = lst(jd, longit);
                accumoon(jd, lat, sid, 0, out ra, out dec, out dist);
                return altit(dec, sid - ra, lat);
            }
            else
            {
                double ra, dec;
                lpsun(jd, out ra, out dec);
                return altit(dec, lst(jd, longit) - ra, lat);
            }
        }

        static void lpsun(double jd, out double ra, out double dec)

        /* Low precision formulae for the sun, from Almanac p. C24 (1990) */
        /* ra and dec are returned as decimal hours and decimal degrees. */
        {
                double n, L, g, lambda,epsilon,x,y,z;

                n = jd - J2000;
                L = 280.460 + 0.9856474 * n;
                g = (357.528 + 0.9856003 * n)/DEG_IN_RADIAN;
                lambda = (L + 1.915 * Math.Sin(g) + 0.020 * Math.Sin(2 * g))/DEG_IN_RADIAN;
                epsilon = (23.439 - 0.0000004 * n)/DEG_IN_RADIAN;

                x = Math.Cos(lambda);
                y = Math.Cos(epsilon) * Math.Sin(lambda);
                z = Math.Sin(epsilon)*Math.Sin(lambda);

                ra = (atan_circ(x,y))*HRS_IN_RADIAN;
                dec = (Math.Asin(z))*DEG_IN_RADIAN;
        }

        static double atan_circ(double x, double y)
        {
                /* returns radian angle 0 to 2pi for coords x, y --
                   get that quadrant right !! */

                double theta;

                if((x == 0) && (y == 0)) return(0);  /* guard ... */

                theta = Math.Atan2(y,x);  /* turns out there is such a thing in math.h */
                while(theta < 0) theta += 2.0 * Math.PI;
                return(theta);
        }

        static double lst(double jd, double longit)
        {
                /* returns the local MEAN sidereal time (dec hrs) at julian date jd
                   at west longitude long (decimal hours).  Follows
                   definitions in 1992 Astronomical Almanac, pp. B7 and L2.
                   Expression for GMST at 0h ut referenced to Aoki et al, A&A 105,
                   p.359, 1982. */

                double t, ut, jdmid, jdint, jdfrac, sid_g;
                long jdin, sid_int;

                jdin = (long)jd;         /* fossil code from earlier package which
                                split jd into integer and fractional parts ... */
                jdint = jdin;
                jdfrac = jd - jdint;
                if(jdfrac < 0.5) {
                        jdmid = jdint - 0.5;
                        ut = jdfrac + 0.5;
                }
                else {
                        jdmid = jdint + 0.5;
                        ut = jdfrac - 0.5;
                }
                t = (jdmid - J2000)/36525;
                sid_g = (24110.54841+8640184.812866*t+0.093104*t*t-6.2e-6*t*t*t)/86400;
                sid_int = (long)sid_g;
                sid_g = sid_g - (double) sid_int;
                sid_g = sid_g + 1.0027379093 * ut - longit/24;
                sid_int = (long)sid_g;
                sid_g = (sid_g - (double) sid_int) * 24;
                if(sid_g < 0) sid_g = sid_g + 24;
                return(sid_g);
        }


        static double circulo (double x) 
        {
            /* assuming x is an angle in degrees, returns
               modulo 360 degrees. */

            int n;

            n = (int)(x / 360);
            return(x - 360 * n);
        }


        public static double altit(double dec, double ha, double lat)
        /* dec deg, dec hrs, dec deg */
        {
            double x;
            dec = dec / DEG_IN_RADIAN;
            ha = ha / HRS_IN_RADIAN;
            lat = lat / DEG_IN_RADIAN;  /* thank heavens for pass-by-value */
            x = DEG_IN_RADIAN * Math.Asin(Math.Cos(dec) * Math.Cos(ha) * Math.Cos(lat) 
                + Math.Sin(dec) * Math.Sin(lat));
            return (x);
        }

        static void eclrot (double jd, ref double x, ref double y, ref double z)
//        static void eclrot (double jd, out double x unusedParameter, double *y, double *z)

            /* rotates ecliptic rectangular coords x, y, z to
        equatorial (all assumed of date.) */
        {
                double incl;
                double ypr,zpr;
                double T;

                T = (jd - J2000) / 36525;  /* centuries since J2000 */

                incl = (23.439291 + T * (-0.0130042 - 0.00000016 * T))/DEG_IN_RADIAN;
                        /* 1992 Astron Almanac, p. B18, dropping the
                           cubic term, which is 2 milli-arcsec! */
                ypr = Math.Cos(incl) * y - Math.Sin(incl) * z;
                zpr = Math.Sin(incl) * y + Math.Cos(incl) * z;
                y = ypr;
                z = zpr;
                /* x remains the same. */
        }


        // Added 2003-02-04 from Skycal 5 for moonrise/moonset
        // DWF: this function is no good for years < 1900 or >= 2100.  I have
        // added assertions to make it abort if that range is exceeded.  (So
        // we can't have moonrise and moonset outside that range.)
        static double etcorr (double jd) {

                /* Given a julian date in 1900-2100, returns the jd corrected
                   for delta t; delta t  is
                        TDT - UT (after 1983 and before 1994)
                        ET - UT (before 1983)
                        an extrapolated guess  (after 1994).

                For dates in the past (<= 1994 and after 1900) the value is linearly
                interpolated on 5-year intervals; for dates after the present,
                an extrapolation is used, because the true value of delta t
                cannot be predicted precisely.  Note that TDT is essentially the
                modern version of ephemeris time with a slightly cleaner
                definition.

                Where the algorithm shifts there is an approximately 0.1 second
                discontinuity.  Also, the 5-year linear interpolation scheme can
                lead to errors as large as 0.5 seconds in some cases, though
                usually rather smaller. */

                double[] dates = new double[20];
                double[] delts = new double[20];  /* can't initialize this look-up table
                    with stupid old sun compiler .... */
                double year, delt=0;
                int i;

                /* this stupid patch for primitive sun C compilers ....
                        do not allow automatic initialization of arrays! */

                for(i = 0; i <= 18; ++i) dates[i] = 1900 + (double) i * 5;
                dates[19] = 1994;

                delts[0] = -2.72;  delts[1] = 3.86; delts[2] = 10.46;
                delts[3] = 17.20;  delts[4] = 21.16; delts[5] = 23.62;
                delts[6] = 24.02;  delts[7] = 23.93; delts[8] = 24.33;
                delts[9] = 26.77;  delts[10] = 29.15; delts[11] = 31.07;
                delts[12] = 33.15;  delts[13] = 35.73; delts[14] = 40.18;
                delts[15] = 45.48;  delts[16] = 50.54; delts[17] = 54.34;
                delts[18] = 56.86;  delts[19] = 59.98;

                year = 1900 + (jd - 2415019.5) / 365.25;

                if(year < 1994.0 && year >= 1900) {
                        i = ((int)year - 1900) / 5;
                        Debug.Assert (i >= 0 && i < 20,"Too many iterations.");
                        delt = delts[i] +
                         ((delts[i+1] - delts[i])/(dates[i+1] - dates[i])) * (year - dates[i]);
                }

                else if (year > 1994 && year < 2100)
                        delt = 33.15 + (2.164e-3) * (jd - 2436935.4);  /* rough extrapolation */

                else if (year < 1900) {
                  // printf("etcorr ... no ephemeris time data for < 1900.\n");
                  // delt = 0.;
                  Debug.Assert (false, "Year is less than 1900.");
                }

                else if (year >= 2100) {
                  // printf("etcorr .. very long extrapolation in delta T - inaccurate.\n");
                  // delt = 180.; /* who knows? */
                  Debug.Assert (false,"Not valid beyond 2099.");
                }

                return(jd + delt/SEC_IN_DAY);
        }



        static void accumoon(double jd, double geolat, double lst, double elevsea,
            out double topora, out double topodec, out double topodist)

            // double jd,geolat,lst,elevsea;  /* jd, dec. degr., dec. hrs., meters */

            /* More accurate (but more elaborate and slower) lunar
               ephemeris, from Jean Meeus' *Astronomical Formulae For Calculators*,
               pub. Willman-Bell.  Includes all the terms given there. */

            {
            /*      double *eclatit,*eclongit, *pie,*ra,*dec,*dist; geocent quantities,
                            formerly handed out but not in this version */
                    double pie, dist;  /* horiz parallax */
                    double Lpr,M,Mpr,D,F,Om,T,Tsq,Tcb;
                    double e,lambda,B,beta,om1,om2;
                    double sinx, x, y, z, l, m, n;
                    double x_geo, y_geo, z_geo;  /* geocentric position of *observer* */

                    jd = etcorr(jd);   /* approximate correction to ephemeris time */
                    T = (jd - 2415020) / 36525;   /* this based around 1900 ... */
                    Tsq = T * T;
                    Tcb = Tsq * T;

                    Lpr = 270.434164 + 481267.8831 * T - 0.001133 * Tsq
                                    + 0.0000019 * Tcb;
                    M = 358.475833 + 35999.0498*T - 0.000150*Tsq
                                    - 0.0000033*Tcb;
                    Mpr = 296.104608 + 477198.8491*T + 0.009192*Tsq
                                    + 0.0000144*Tcb;
                    D = 350.737486 + 445267.1142*T - 0.001436 * Tsq
                                    + 0.0000019*Tcb;
                    F = 11.250889 + 483202.0251*T -0.003211 * Tsq
                                    - 0.0000003*Tcb;
                    Om = 259.183275 - 1934.1420*T + 0.002078*Tsq
                                    + 0.0000022*Tcb;

                    Lpr = circulo(Lpr);
                    Mpr = circulo(Mpr);
                    M = circulo(M);
                    D = circulo(D);
                    F = circulo(F);
                    Om = circulo(Om);


                    sinx =  Math.Sin((51.2 + 20.2 * T)/DEG_IN_RADIAN);
                    Lpr = Lpr + 0.000233 * sinx;
                    M = M - 0.001778 * sinx;
                    Mpr = Mpr + 0.000817 * sinx;
                    D = D + 0.002011 * sinx;

                    sinx = 0.003964 * Math.Sin((346.560+132.870*T -0.0091731*Tsq)/DEG_IN_RADIAN);

                    Lpr = Lpr + sinx;
                    Mpr = Mpr + sinx;
                    D = D + sinx;
                    F = F + sinx;

                    sinx = Math.Sin(Om/DEG_IN_RADIAN);
                    Lpr = Lpr + 0.001964 * sinx;
                    Mpr = Mpr + 0.002541 * sinx;
                    D = D + 0.001964 * sinx;
                    F = F - 0.024691 * sinx;
                    F = F - 0.004328 * Math.Sin((Om + 275.05 -2.30*T)/DEG_IN_RADIAN);

                    e = 1 - 0.002495 * T - 0.00000752 * Tsq;

                    M = M / DEG_IN_RADIAN;   /* these will all be arguments ... */
                    Mpr = Mpr / DEG_IN_RADIAN;
                    D = D / DEG_IN_RADIAN;
                    F = F / DEG_IN_RADIAN;

                    lambda = Lpr + 6.288750 * Math.Sin(Mpr)
                            + 1.274018 * Math.Sin(2*D - Mpr)
                            + 0.658309 * Math.Sin(2*D)
                            + 0.213616 * Math.Sin(2*Mpr)
                            - e * 0.185596 * Math.Sin(M)
                            - 0.114336 * Math.Sin(2*F)
                            + 0.058793 * Math.Sin(2*D - 2*Mpr)
                            + e * 0.057212 * Math.Sin(2*D - M - Mpr)
                            + 0.053320 * Math.Sin(2*D + Mpr)
                            + e * 0.045874 * Math.Sin(2*D - M)
                            + e * 0.041024 * Math.Sin(Mpr - M)
                            - 0.034718 * Math.Sin(D)
                            - e * 0.030465 * Math.Sin(M+Mpr)
                            + 0.015326 * Math.Sin(2*D - 2*F)
                            - 0.012528 * Math.Sin(2*F + Mpr)
                            - 0.010980 * Math.Sin(2*F - Mpr)
                            + 0.010674 * Math.Sin(4*D - Mpr)
                            + 0.010034 * Math.Sin(3*Mpr)
                            + 0.008548 * Math.Sin(4*D - 2*Mpr)
                            - e * 0.007910 * Math.Sin(M - Mpr + 2*D)
                            - e * 0.006783 * Math.Sin(2*D + M)
                            + 0.005162 * Math.Sin(Mpr - D);

                            /* And furthermore.....*/

                    lambda = lambda + e * 0.005000 * Math.Sin(M + D)
                            + e * 0.004049 * Math.Sin(Mpr - M + 2*D)
                            + 0.003996 * Math.Sin(2*Mpr + 2*D)
                            + 0.003862 * Math.Sin(4*D)
                            + 0.003665 * Math.Sin(2*D - 3*Mpr)
                            + e * 0.002695 * Math.Sin(2*Mpr - M)
                            + 0.002602 * Math.Sin(Mpr - 2*F - 2*D)
                            + e * 0.002396 * Math.Sin(2*D - M - 2*Mpr)
                            - 0.002349 * Math.Sin(Mpr + D)
                            + e * e * 0.002249 * Math.Sin(2*D - 2*M)
                            - e * 0.002125 * Math.Sin(2*Mpr + M)
                            - e * e * 0.002079 * Math.Sin(2*M)
                            + e * e * 0.002059 * Math.Sin(2*D - Mpr - 2*M)
                            - 0.001773 * Math.Sin(Mpr + 2*D - 2*F)
                            - 0.001595 * Math.Sin(2*F + 2*D)
                            + e * 0.001220 * Math.Sin(4*D - M - Mpr)
                            - 0.001110 * Math.Sin(2*Mpr + 2*F)
                            + 0.000892 * Math.Sin(Mpr - 3*D)
                            - e * 0.000811 * Math.Sin(M + Mpr + 2*D)
                            + e * 0.000761 * Math.Sin(4*D - M - 2*Mpr)
                            + e * e * 0.000717 * Math.Sin(Mpr - 2*M)
                            + e * e * 0.000704 * Math.Sin(Mpr - 2 * M - 2*D)
                            + e * 0.000693 * Math.Sin(M - 2*Mpr + 2*D)
                            + e * 0.000598 * Math.Sin(2*D - M - 2*F)
                            + 0.000550 * Math.Sin(Mpr + 4*D)
                            + 0.000538 * Math.Sin(4*Mpr)
                            + e * 0.000521 * Math.Sin(4*D - M)
                            + 0.000486 * Math.Sin(2*Mpr - D);

            /*              *eclongit = lambda;  */

                    B = 5.128189 * Math.Sin(F)
                            + 0.280606 * Math.Sin(Mpr + F)
                            + 0.277693 * Math.Sin(Mpr - F)
                            + 0.173238 * Math.Sin(2*D - F)
                            + 0.055413 * Math.Sin(2*D + F - Mpr)
                            + 0.046272 * Math.Sin(2*D - F - Mpr)
                            + 0.032573 * Math.Sin(2*D + F)
                            + 0.017198 * Math.Sin(2*Mpr + F)
                            + 0.009267 * Math.Sin(2*D + Mpr - F)
                            + 0.008823 * Math.Sin(2*Mpr - F)
                            + e * 0.008247 * Math.Sin(2*D - M - F)
                            + 0.004323 * Math.Sin(2*D - F - 2*Mpr)
                            + 0.004200 * Math.Sin(2*D + F + Mpr)
                            + e * 0.003372 * Math.Sin(F - M - 2*D)
                            + 0.002472 * Math.Sin(2*D + F - M - Mpr)
                            + e * 0.002222 * Math.Sin(2*D + F - M)
                            + e * 0.002072 * Math.Sin(2*D - F - M - Mpr)
                            + e * 0.001877 * Math.Sin(F - M + Mpr)
                            + 0.001828 * Math.Sin(4*D - F - Mpr)
                            - e * 0.001803 * Math.Sin(F + M)
                            - 0.001750 * Math.Sin(3*F)
                            + e * 0.001570 * Math.Sin(Mpr - M - F)
                            - 0.001487 * Math.Sin(F + D)
                            - e * 0.001481 * Math.Sin(F + M + Mpr)
                            + e * 0.001417 * Math.Sin(F - M - Mpr)
                            + e * 0.001350 * Math.Sin(F - M)
                            + 0.001330 * Math.Sin(F - D)
                            + 0.001106 * Math.Sin(F + 3*Mpr)
                            + 0.001020 * Math.Sin(4*D - F)
                            + 0.000833 * Math.Sin(F + 4*D - Mpr);
                 /* not only that, but */
                    B = B + 0.000781 * Math.Sin(Mpr - 3*F)
                            + 0.000670 * Math.Sin(F + 4*D - 2*Mpr)
                            + 0.000606 * Math.Sin(2*D - 3*F)
                            + 0.000597 * Math.Sin(2*D + 2*Mpr - F)
                            + e * 0.000492 * Math.Sin(2*D + Mpr - M - F)
                            + 0.000450 * Math.Sin(2*Mpr - F - 2*D)
                            + 0.000439 * Math.Sin(3*Mpr - F)
                            + 0.000423 * Math.Sin(F + 2*D + 2*Mpr)
                            + 0.000422 * Math.Sin(2*D - F - 3*Mpr)
                            - e * 0.000367 * Math.Sin(M + F + 2*D - Mpr)
                            - e * 0.000353 * Math.Sin(M + F + 2*D)
                            + 0.000331 * Math.Sin(F + 4*D)
                            + e * 0.000317 * Math.Sin(2*D + F - M + Mpr)
                            + e * e * 0.000306 * Math.Sin(2*D - 2*M - F)
                            - 0.000283 * Math.Sin(Mpr + 3*F);

                    om1 = 0.0004664 * Math.Cos(Om/DEG_IN_RADIAN);
                    om2 = 0.0000754 * Math.Cos((Om + 275.05 - 2.30*T)/DEG_IN_RADIAN);

                    beta = B * (1 - om1 - om2);
            /*      *eclatit = beta; */

                    pie = 0.950724
                            + 0.051818 * Math.Cos(Mpr)
                            + 0.009531 * Math.Cos(2*D - Mpr)
                            + 0.007843 * Math.Cos(2*D)
                            + 0.002824 * Math.Cos(2*Mpr)
                            + 0.000857 * Math.Cos(2*D + Mpr)
                            + e * 0.000533 * Math.Cos(2*D - M)
                            + e * 0.000401 * Math.Cos(2*D - M - Mpr)
                            + e * 0.000320 * Math.Cos(Mpr - M)
                            - 0.000271 * Math.Cos(D)
                            - e * 0.000264 * Math.Cos(M + Mpr)
                            - 0.000198 * Math.Cos(2*F - Mpr)
                            + 0.000173 * Math.Cos(3*Mpr)
                            + 0.000167 * Math.Cos(4*D - Mpr)
                            - e * 0.000111 * Math.Cos(M)
                            + 0.000103 * Math.Cos(4*D - 2*Mpr)
                            - 0.000084 * Math.Cos(2*Mpr - 2*D)
                            - e * 0.000083 * Math.Cos(2*D + M)
                            + 0.000079 * Math.Cos(2*D + 2*Mpr)
                            + 0.000072 * Math.Cos(4*D)
                            + e * 0.000064 * Math.Cos(2*D - M + Mpr)
                            - e * 0.000063 * Math.Cos(2*D + M - Mpr)
                            + e * 0.000041 * Math.Cos(M + D)
                            + e * 0.000035 * Math.Cos(2*Mpr - M)
                            - 0.000033 * Math.Cos(3*Mpr - 2*D)
                            - 0.000030 * Math.Cos(Mpr + D)
                            - 0.000029 * Math.Cos(2*F - 2*D)
                            - e * 0.000029 * Math.Cos(2*Mpr + M)
                            + e * e * 0.000026 * Math.Cos(2*D - 2*M)
                            - 0.000023 * Math.Cos(2*F - 2*D + Mpr)
                            + e * 0.000019 * Math.Cos(4*D - M - Mpr);

                    beta = beta/DEG_IN_RADIAN;
                    lambda = lambda/DEG_IN_RADIAN;
                    l = Math.Cos(lambda) * Math.Cos(beta);
                    m = Math.Sin(lambda) * Math.Cos(beta);
                    n = Math.Sin(beta);
                    eclrot(jd,ref l,ref m,ref n);

                    dist = 1/Math.Sin((pie)/DEG_IN_RADIAN);
                    x = l * dist;
                    y = m * dist;
                    z = n * dist;

            /*      *ra = atan_circ(l,m) * DEG_IN_RADIAN;
                    *dec = aMath.Sin(n) * DEG_IN_RADIAN;        */

                    geocent(lst,geolat,elevsea,out x_geo, out y_geo,out z_geo);

                    x = x - x_geo;  /* topocentric correction using elliptical earth fig. */
                    y = y - y_geo;
                    z = z - z_geo;

                    topodist = Math.Sqrt(x*x + y*y + z*z);

                    l = x / (topodist);
                    m = y / (topodist);
                    n = z / (topodist);

                    topora = atan_circ(l,m) * HRS_IN_RADIAN;
                    topodec = Math.Asin(n) * DEG_IN_RADIAN;
            }


        static void geocent (double geolong, double geolat, double height,
                     out double x_geo, out double y_geo, out double z_geo)

        /* computes the geocentric coordinates from the geodetic
        (standard map-type) longitude, latitude, and height.
        These are assumed to be in decimal hours, decimal degrees, and
        meters respectively.  Notation generally follows 1992 Astr Almanac,
        p. K11 */

        {

                double denom, C_geo, S_geo;

                geolat = geolat / DEG_IN_RADIAN;
                geolong = geolong / HRS_IN_RADIAN;
                denom = (1 - FLATTEN) * Math.Sin(geolat);
                denom = Math.Cos(geolat) * Math.Cos(geolat) + denom*denom;
                C_geo = 1 / Math.Sqrt(denom);
                S_geo = (1 - FLATTEN) * (1 - FLATTEN) * C_geo;
                C_geo = C_geo + height / EQUAT_RAD;  /* deviation from almanac
                               notation -- include height here. */
                S_geo = S_geo + height / EQUAT_RAD;
                x_geo = C_geo * Math.Cos(geolat) * Math.Cos(geolong);
                y_geo = C_geo * Math.Cos(geolat) * Math.Sin(geolong);
                z_geo = S_geo * Math.Sin(geolat);
        }


        public static double? jd_alt(double alt, double jdorig, double lat, double longit,
            bool lunar, out bool is_rise)
        {
            /* returns jd at which sun/moon is at a given
                    altitude, given jdguess as a starting point. */

            double jdguess = jdorig;
            double jdout, adj = 1.0;
            double deriv, err, del = 0.002;
            double alt2, alt3;
            short i = 0;

            /* first guess */

            alt2 = altitude(jdguess, lat, longit, lunar);
            jdguess = jdguess + del;
            alt3 = altitude(jdguess, lat, longit, lunar);
            err = alt3 - alt;
            deriv = (alt3 - alt2) / del;
            if (deriv == 0.0)
            {
                is_rise = false;
                return (null); // Found dead end.
            }
            adj = -err / deriv;
            while (Math.Abs(adj) >= eventPrecisionJD)
            {
                if (i++ == 12)
                {
                    is_rise = false;
                    return (null); // Exceeded max iterations.
                }
                jdguess += adj;
                if (Math.Abs(jdguess - jdorig) > 0.5)
                {
                    is_rise = false;
                    return (null); // Ran out of bounds.
                }
                alt2 = alt3;
                alt3 = altitude(jdguess, lat, longit, lunar);
                err = alt3 - alt;
                deriv = (alt3 - alt2) / adj;
                if (deriv == 0.0)
                {
                    is_rise = false;
                    return (null); // Found dead end.
                }
                adj = -err / deriv;
            }
            jdout = jdguess;

            // Figure out whether this is a rise or a set by shifting
            // by 1 second.
            {
                jdguess -= 1.0 / SEC_IN_DAY;
                alt2 = altitude(jdguess, lat, longit, lunar);
                is_rise = (alt2 < alt3);
            }

            return (jdout);
        }


        static void flmoon(int n, int nph, out double jdout)
        /* Gives jd (+- 2 min) of phase nph on lunation n; replaces
        less accurate Numerical Recipes routine.  This routine
        implements formulae found in Jean Meeus' *Astronomical Formulae
        for Calculators*, 2nd edition, Willman-Bell.  A very useful
        book!! */
        {
            double jd, cor;
            double M, Mpr, F;
            double T;
            double lun;

            lun = (double) n + (double) nph / 4;
            T = lun / 1236.85;
            jd = 2415020.75933 + 29.53058868 * lun
                  + 0.0001178 * T * T
                  - 0.000000155 * T * T * T
                  + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T * T)/DEG_IN_RADIAN);
            M = 359.2242 + 29.10535608 * lun - 0.0000333 * T * T - 0.00000347 * T * T * T;
            M = M / DEG_IN_RADIAN;
            Mpr = 306.0253 + 385.81691806 * lun + 0.0107306 * T * T + 0.00001236 * T * T * T;
            Mpr = Mpr / DEG_IN_RADIAN;
            F = 21.2964 + 390.67050646 * lun - 0.0016528 * T * T - 0.00000239 * T * T * T;
            F = F / DEG_IN_RADIAN;
            if((nph == 0) || (nph == 2)) 
            {/* new or full */
                  cor =   (0.1734 - 0.000393*T) * Math.Sin(M)
                          + 0.0021 * Math.Sin(2*M)
                          - 0.4068 * Math.Sin(Mpr)
                          + 0.0161 * Math.Sin(2*Mpr)
                          - 0.0004 * Math.Sin(3*Mpr)
                          + 0.0104 * Math.Sin(2*F)
                          - 0.0051 * Math.Sin(M + Mpr)
                          - 0.0074 * Math.Sin(M - Mpr)
                          + 0.0004 * Math.Sin(2*F+M)
                          - 0.0004 * Math.Sin(2*F-M)
                          - 0.0006 * Math.Sin(2*F+Mpr)
                          + 0.0010 * Math.Sin(2*F-Mpr)
                          + 0.0005 * Math.Sin(M+2*Mpr);
                  jd = jd + cor;
        }
            else 
            {
                  cor = (0.1721 - 0.0004*T) * Math.Sin(M)
                          + 0.0021 * Math.Sin(2 * M)
                          - 0.6280 * Math.Sin(Mpr)
                          + 0.0089 * Math.Sin(2 * Mpr)
                          - 0.0004 * Math.Sin(3 * Mpr)
                          + 0.0079 * Math.Sin(2*F)
                          - 0.0119 * Math.Sin(M + Mpr)
                          - 0.0047 * Math.Sin(M - Mpr)
                          + 0.0003 * Math.Sin(2 * F + M)
                          - 0.0004 * Math.Sin(2 * F - M)
                          - 0.0006 * Math.Sin(2 * F + Mpr)
                          + 0.0021 * Math.Sin(2 * F - Mpr)
                          + 0.0003 * Math.Sin(M + 2 * Mpr)
                          + 0.0004 * Math.Sin(M - 2 * Mpr)
                          - 0.0003 * Math.Sin(2*M + Mpr);
                  if(nph == 1) cor = cor + 0.0028 -
                                  0.0004 * Math.Cos(M) + 0.0003 * Math.Cos(Mpr);
                  if(nph == 3) cor = cor - 0.0028 +
                                  0.0004 * Math.Cos(M) - 0.0003 * Math.Cos(Mpr);
                  jd = jd + cor;

            }
             jdout = jd;
        }



        public static void find_next_moon_phase (ref double jd, out TTG.NavalWar.NWComms.GameConstants.TideEventType phase) 
        {
            double newjd, lastnewjd, nextjd;
            short kount=0;

            // Originally, there was no problem with getting snagged, but since
            // I introduced the roundoff error going back and forth with Timestamp,
            // now it's a problem.
            // Move ahead by 1 second to avoid snagging.
            jd += 1.0 / SEC_IN_DAY;

            // Find current lunation.  I doubled the safety margin since it
            // seemed biased for forwards search.  Backwards search has since
            // been deleted, but little reason to mess with it.
            int nlast = (int)((jd - 2415020.5) / 29.5307 - 2);

            flmoon(nlast,0,out lastnewjd);
            flmoon(++nlast,0,out newjd);
            while (newjd <= jd) 
            {
                lastnewjd = newjd;
                flmoon(++nlast,0, out newjd);
                if (kount++ > 5)
                {
                    break;
                }; // Original limit was 35 (!) //hmmm
            }

            // We might save some work here by estimating, i.e.:
            //   x = jd - lastnewjd;
            //   noctiles = (int)(x / 3.69134);  /* 3.69134 = 1/8 month; truncate. */
            // However....

            Debug.Assert (lastnewjd <= jd && newjd > jd);
            phase = (TTG.NavalWar.NWComms.GameConstants.TideEventType)1;
            // Lunation is lastnewjd's lunation
            flmoon (--nlast, (int)phase, out nextjd);       // Phase = 1
            if (nextjd <= jd) 
            {
                flmoon (nlast, (int)++phase, out nextjd);   // Phase = 2
                if (nextjd <= jd) 
                {
                    flmoon (nlast, (int)++phase, out nextjd); // Phase = 3
                    if (nextjd <= jd) 
                    {
                      
                        phase = TTG.NavalWar.NWComms.GameConstants.TideEventType.NewMoon;
                        nextjd = newjd;
                    }
                }
            }
            jd = nextjd;
        }


        public static void findNextMoonPhase (DateTime t,
                                        out TTG.NavalWar.NWComms.GameConstants.TideEventType tideEvent_out) 
        {
            TTG.NavalWar.NWComms.GameConstants.TideEventType phase;
            double jd = t.DtToJulianDay(); //
            find_next_moon_phase(ref jd, out phase);
            //tideEvent_out.eventTime = jd;
            tideEvent_out = phase;
            //switch (phase) 
            //{
            //case 0:
            //    tideEvent_out = TideEventType.newmoon;
            //    break;
            //case 1:
            //    tideEvent_out = TideEventType.firstquarter;
            //    break;
            //case 2:
            //    tideEvent_out = TideEventType.fullmoon;
            //    break;
            //case 3:
            //    tideEvent_out = TideEventType.lastquarter;
            //    break;
            //default:
            //    tideEvent_out = TideEventType.newmoon; //to avoid error
            //    Debug.Assert (false);
            //    break;
            //}
        }


        public static void find_next_rise_or_set (ref double jd, double lat, double longit, bool lunar,
            ref bool is_rise) 
        {
            // Move ahead by precision interval to avoid snagging.
            jd += eventPrecisionJD;

            double jdorig = jd;
            double inc = 1.0 / 6.0; // 4 hours

            // First we want to know what we are looking for.
            bool looking_for = (altitude (jdorig, lat, longit, lunar) < rise_altitude);

            // Now give it a decent try.  Because jd_alt is so unpredictable,
            // we can even find things out of order (which is one reason we need
            // to know what we're looking for).
            double jdlooper = jdorig;
            do 
            {
                //TODO: Fix null problem
                jd = (double)jd_alt (rise_altitude, jdlooper, lat, longit, lunar, out is_rise);
                jdlooper += inc;
                // Loop either on error return (which is a negative number), or if we
                // found an event in the wrong direction, or the wrong kind of event.
            } while ((jd < 0.0) ||
               (jd <= jdorig) ||
               (is_rise != looking_for));
        }


        public static void findNextRiseOrSet (DateTime t,
                                        Coordinate c,
                                        ref TTG.NavalWar.NWComms.GameConstants.RiseSetType riseSetType,
                                        ref TTG.NavalWar.NWComms.GameConstants.TideEvent tideEvent_out,
                                        out double jdout) 
        {
          
          bool isRise = false;
          double jd =  t.DtToJulianDay(); //t.jd();
          // skycal "longit" is measured in HOURS WEST, not degrees east.
          // (lat is unchanged)
          find_next_rise_or_set (ref jd,
                                 c.LatitudeDeg,
                                 -(c.LongitudeDeg)/15.0,
                                 (riseSetType==TTG.NavalWar.NWComms.GameConstants.RiseSetType.Lunar),
                                 ref isRise);
          jdout = jd;
          if (isRise)
              tideEvent_out = ((riseSetType == TTG.NavalWar.NWComms.GameConstants.RiseSetType.Lunar) ? TTG.NavalWar.NWComms.GameConstants.TideEvent.MoonRise
                                                              : TTG.NavalWar.NWComms.GameConstants.TideEvent.SunRise);
          else
            tideEvent_out = ((riseSetType == TTG.NavalWar.NWComms.GameConstants.RiseSetType.Lunar) ? TTG.NavalWar.NWComms.GameConstants.TideEvent.MoonSet
                                                              : TTG.NavalWar.NWComms.GameConstants.TideEvent.SunSet);
        }


        // Simple question deserving a simple answer...
        public static bool sunIsUp (DateTime t, Coordinate c) 
        {
            //  assert (!(c.isNull()));
            return (altitude (t.DtToJulianDay(), c.LatitudeDeg, -(c.LongitudeDeg)/15.0, false) >= rise_altitude);
        }



        #endregion


    }

}
