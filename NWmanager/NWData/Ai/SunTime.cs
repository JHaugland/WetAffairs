using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace TTG.NavalWar.NWData.Ai

{
    /// <summary>
    /// Calculates sunset / sunrise time.
    /// 
    /// Implementation of algorithm found in Almanac for Computers, 1990
    /// published by Nautical Almanac Office
    /// 
    /// Implemented by Huysentruit Wouter, Fastload-Media.be
    /// See http://69.10.233.10/KB/cs/SunTime.aspx
    /// </summary>
    public class SunTime
    {
        #region Declarations

        public enum ZenithValue : long
        {
            /// <summary>
            /// Official zenith (90.5)
            /// </summary>
            Official = 90500,

            /// <summary>
            /// Civil zenith (96)
            /// </summary>
            Civil = 96000,

            /// <summary>
            /// Nautical zenith (102)
            /// </summary>
            Nautical = 102000,

            /// <summary>
            /// Astronomical zenith (108)
            /// </summary>
            Astronomical = 108000
        }

        internal enum Direction
        {
            Sunrise,
            Sunset
        }
        
        private ZenithValue zenith = ZenithValue.Official;
        private double longitude;
        private double latitude;
        private double utcOffset;
        private DateTime date;
        private DaylightTime daylightChanges;
        private int sunriseTime;
        private int sunsetTime;

        #endregion

        #region Construction

        /// <summary>
        /// Create a new SunTime object with default settings.
        /// </summary>
        public SunTime()
        {
            this.latitude = 0.0;
            this.longitude = 0.0;
            this.utcOffset = 1.0;
            this.date = DateTime.Now;
            this.daylightChanges = TimeZone.CurrentTimeZone.GetDaylightChanges(date.Year);
            Update();
        }

        /// <summary>
        /// Create a new SunTime object for the current date.
        /// </summary>
        /// <param name="latitude">Global position latitude in degrees. Latitude is positive for North and negative for South.</param>
        /// <param name="longitude">Global position longitude in degrees. Longitude is positive for East and negative for West.</param>
        /// <param name="utcOffset">The local UTC offset (f.e. +1 for Brussel, Kopenhagen, Madrid, Paris).</param>
        /// <param name="daylightChanges">The daylight saving settings to use.</param>
        public SunTime(double latitude, double longitude, double utcOffset, DaylightTime daylightChanges)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.utcOffset = utcOffset;
            this.date = DateTime.Now;
            this.daylightChanges = daylightChanges;
            Update();
        }

        /// <summary>
        /// Create a new SunTime object for the given date.
        /// </summary>
        /// <param name="latitude">Global position latitude in degrees. Latitude is positive for North and negative for South.</param>
        /// <param name="longitude">Global position longitude in degrees. Longitude is positive for East and negative for West.</param>
        /// <param name="utcOffset">The local UTC offset (f.e. +1 for Brussel, Kopenhagen, Madrid, Paris).</param>
        /// <param name="daylightChanges">The daylight saving settings to use.</param>
        /// <param name="date">The date to calculate the set- and risetime for.</param>
        public SunTime(double latitude, double longitude, double utcOffset, DaylightTime daylightChanges, DateTime date)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.utcOffset = utcOffset;
            this.daylightChanges = daylightChanges;
            this.date = date;
            Update();
        }

        #endregion

        #region Private methods

        private static double Deg2Rad(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double Rad2Deg(double angle)
        {
            return 180.0 * angle / Math.PI;
        }

        private static double FixValue(double value, double min, double max)
        {
            while (value < min)
                value += (max - min);

            while (value >= max)
                value -= (max - min);

            return value;
        }

        private int Calculate(Direction direction)
        {
            /* doy (N) */
            int N = date.DayOfYear;

            /* appr. time (t) */
            double lngHour = longitude / 15.0;

            double t;

            if (direction == Direction.Sunrise)
                t = N + ((6.0 - lngHour) / 24.0);
            else
                t = N + ((18.0 - lngHour) / 24.0);

            /* mean anomaly (M) */
            double M = (0.9856 * t) - 3.289;

            /* true longitude (L) */
            double L = M + (1.916 * Math.Sin(Deg2Rad(M))) + (0.020 * Math.Sin(Deg2Rad(2 * M))) + 282.634;
            L = FixValue(L, 0, 360);

            /* right asc (RA) */
            double RA = Rad2Deg(Math.Atan(0.91764 * Math.Tan(Deg2Rad(L))));
            RA = FixValue(RA, 0, 360);

            /* adjust quadrant of RA */
            double Lquadrant = (Math.Floor(L / 90.0)) * 90.0;
            double RAquadrant = (Math.Floor(RA / 90.0)) * 90.0;
            RA = RA + (Lquadrant - RAquadrant);

            RA = RA / 15.0;

            /* sin cos DEC (sinDec / cosDec) */
            double sinDec = 0.39782 * Math.Sin(Deg2Rad(L));
            double cosDec = Math.Cos(Math.Asin(sinDec));

            /* local hour angle (cosH) */
            double cosH = (Math.Cos(Deg2Rad((double)zenith / 1000.0f)) - (sinDec * Math.Sin(Deg2Rad(latitude)))) / (cosDec * Math.Cos(Deg2Rad(latitude)));

            /* local hour (H) */
            double H;

            if (direction == Direction.Sunrise)
                H = 360.0 - Rad2Deg(Math.Acos(cosH));
            else
                H = Rad2Deg(Math.Acos(cosH));

            H = H / 15.0;

            /* time (T) */
            double T = H + RA - (0.06571 * t) - 6.622;

            /* universal time (T) */
            double UT = T - lngHour;

            UT += utcOffset;  // local UTC offset

            if (daylightChanges != null)
                if ((date > daylightChanges.Start) && (date < daylightChanges.End))
                    UT += daylightChanges.Delta.TotalHours;

            UT = FixValue(UT, 0, 24);

            return (int)Math.Round(UT * 3600);  // Convert to seconds
        }

        private void Update()
        {
            sunriseTime = Calculate(Direction.Sunrise);
            sunsetTime = Calculate(Direction.Sunset);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Combine a degrees/minutes/seconds value to an angle in degrees.
        /// </summary>
        /// <param name="degrees">The degrees part of the value.</param>
        /// <param name="minutes">The minutes part of the value.</param>
        /// <param name="seconds">The seconds part of the value.</param>
        /// <returns>The combined angle in degrees.</returns>
        public static double DegreesToAngle(double degrees, double minutes, double seconds)
        {
            if (degrees < 0)
                return degrees - (minutes / 60.0) - (seconds / 3600.0);
            else
                return degrees + (minutes / 60.0) + (seconds / 3600.0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global position longitude in degrees.
        /// Longitude is positive for East and negative for West.
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the global position latitude in degrees.
        /// Latitude is positive for North and negative for South.
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the date where the RiseTime and SetTime apply to.
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the local UTC offset in hours.
        /// F.e.: +1 for Brussel, Kopenhagen, Madrid, Paris.
        /// See Windows Time settings for a list of offsets.
        /// </summary>
        public double UtcOffset
        {
            get { return utcOffset; }
            set
            {
                utcOffset = value;
                Update();
            }
        }

        /// <summary>
        /// The time (in seconds starting from midnight) the sun will rise on the given location at the given date.
        /// </summary>
        public int RiseTimeSec
        {
            get { return sunriseTime; }
        }

        /// <summary>
        /// The time (in seconds starting from midnight) the sun will set on the given location at the given date.
        /// </summary>
        public int SetTimeSec
        {
            get { return sunsetTime; }
        }

        /// <summary>
        /// The time the sun will rise on the given location at the given date.
        /// </summary>
        public DateTime RiseTime
        {
            get { return date.Date.AddSeconds(sunriseTime); }
        }

        /// <summary>
        /// The time the sun will set on the given location at the given date.
        /// </summary>
        public DateTime SetTime
        {
            get { return date.Date.AddSeconds(sunsetTime); }
        }

        /// <summary>
        /// Gets or sets the zenith used in the sunrise / sunset time calculation.
        /// </summary>
        public ZenithValue Zenith
        {
            get { return zenith; }
            set
            {
                zenith = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the daylight saving range to use in sunrise / sunset time calculation.
        /// </summary>
        public DaylightTime DaylightChanges
        {
            get { return daylightChanges; }
            set
            {
                daylightChanges = value;
                Update();
            }
        }

        #endregion
    }
}
