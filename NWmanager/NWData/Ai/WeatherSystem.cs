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
using System.Globalization;

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class WeatherSystem : IBlackboardObject
    {
        private GameConstants.PrecipitationType _PrecipitationType = GameConstants.PrecipitationType.None;

        #region "Constructors"

        public WeatherSystem() 
        {
            Id = GameManager.GetUniqueCode();
        }

        #endregion


        #region "Public properties"

        public DateTime GameTime { get; set; }

        public DateTime SunriseTime { get; set; }

        public DateTime SunsetTime { get; set; }

        public bool IsSunRising { get; set; }

        public bool IsSunSetting { get; set; }

        public double CurrentSunheightDeg { get; set; }

        public double CurrentSunDeclinationDeg { get; set; }

        public double SunshineWm2 { get; set; }

        public double SunshinePercent { get; set; }

        public double TotalLightPercent { get; set; }

        public double TemperatureC { get; set; }

        public int CloudCover8ths { get; set; }

        public GameConstants.PrecipitationLevel Precipitation { get; set; }

        public GameConstants.PrecipitationType PrecipitationType
        {
            get
            {
                if (Precipitation == GameConstants.PrecipitationLevel.None)
                {
                    _PrecipitationType = GameConstants.PrecipitationType.None;
                }
                if (_PrecipitationType == GameConstants.PrecipitationType.Snow && TemperatureC > 4)
                {
                    _PrecipitationType = GameConstants.PrecipitationType.Rain;
                }
                if (_PrecipitationType == GameConstants.PrecipitationType.Hail && TemperatureC > 7)
                {
                    _PrecipitationType = GameConstants.PrecipitationType.Rain;
                }
                if (_PrecipitationType == GameConstants.PrecipitationType.Rain && TemperatureC < -1)
                {
                    _PrecipitationType = GameConstants.PrecipitationType.Snow;
                }
                return _PrecipitationType;

            }
            set
            {
                _PrecipitationType = value;
            }
        }

        public double WindSpeedMSec { get; set; }

        public double WindDirectionFromDeg { get; set; }

        public GameConstants.DirectionCardinalPoints WindDirectionFrom
        {
            get
            {
                return WindDirectionFromDeg.ToCardinalMark();
            }
        }

        public GameConstants.BeaufortScale WindForceBeaufort
        {
            get
            {
                if (WindSpeedMSec < 0.3) return GameConstants.BeaufortScale.Calm;
                else if (WindSpeedMSec < 1.5) return GameConstants.BeaufortScale.LightAir;
                else if (WindSpeedMSec < 3.3) return GameConstants.BeaufortScale.LightBreeze;
                else if (WindSpeedMSec < 5.5) return GameConstants.BeaufortScale.GentleBreeze;
                else if (WindSpeedMSec < 8.0) return GameConstants.BeaufortScale.ModerateBreeze;
                else if (WindSpeedMSec < 11.0) return GameConstants.BeaufortScale.FreshBreeze;
                else if (WindSpeedMSec < 14.0) return GameConstants.BeaufortScale.StrongBreeze;
                else if (WindSpeedMSec < 17.0) return GameConstants.BeaufortScale.NearGale;
                else if (WindSpeedMSec < 20.0) return GameConstants.BeaufortScale.Gale;
                else if (WindSpeedMSec < 25.0) return GameConstants.BeaufortScale.StrongGale;
                else if (WindSpeedMSec < 29.0) return GameConstants.BeaufortScale.Storm;
                else if (WindSpeedMSec < 33.0) return GameConstants.BeaufortScale.ViolentStorm;
                else return GameConstants.BeaufortScale.Hurricane;
            }
        }

        public int SeaState
        {
            get
            {
                int Beaufort = (int)WindForceBeaufort;
                if (Beaufort == 0)
                {
                    return 0;
                }
                if (Beaufort <= 9)
                {
                    return Beaufort - 1;
                }
                if (Beaufort == 10)
                {
                    return 8;
                }
                return 9; //highest sea state
            }
        }

        public bool IsMoonUp { get; set; }

        public DateTime TimeNextTideEvent { get; set; }

        public GameConstants.TideEvent NextTideEvent { get; set; }

        public GameConstants.TideEventType CurrentMoonPhase { get; set; }

        public double JulianDay { get; set; }

        #endregion



        #region "Public methods"

        public void RecalculateMoonAndSunInformation()
        {
            if (GameManager.Instance.Game == null || Coordinate == null)
            {
                return;
            }
            DateTime date = GameManager.Instance.Game.GameCurrentTime;

            double jd = date.DtToJulianDay();
            JulianDay = jd;
            //bool is_rise = true;
            GameConstants.RiseSetType riseSetType = GameConstants.RiseSetType.Lunar;
            GameConstants.TideEvent tideEvent = GameConstants.TideEvent.MoonSet; //init
            double jdout;
            try
            {
                AstronomyHelper.findNextRiseOrSet(date, Coordinate, ref riseSetType, ref tideEvent, out jdout);
                TimeNextTideEvent = jdout.JdToDateTime();
                NextTideEvent = tideEvent;
                IsMoonUp = (NextTideEvent == GameConstants.TideEvent.MoonSet);
            }
            catch (Exception)
            {
                //TODO: Fix
                //GameManager.Instance.Log.LogError(
                //    string.Format("RecalculateMoonPhase failed for Time {0}, Coordinate {1}", date, Coordinate) 
                //    + ". " + ex.Message);

            }
            //And now for the sun
            CurrentSunheightDeg = WeatherSystem.CalculateSunHeightDeg(date, Coordinate);
            CurrentSunDeclinationDeg = WeatherSystem.CalculateSunDeclination(date, Coordinate);
            DateTimeFromTo SunRiseSet = WeatherSystem.CalculateSunRiseSunSet(date, Coordinate);
            if (SunRiseSet.FromTime != null)
            {
                SunriseTime = (DateTime)SunRiseSet.FromTime;
                IsSunSetting = true;
            }
            else
            {
                IsSunSetting = false;
            }
            if (SunRiseSet.ToTime != null)
            {
                SunsetTime = (DateTime)SunRiseSet.ToTime;
                IsSunRising = true;
            }
            else
            {
                IsSunSetting = false;
            }
        }

        /// <summary>
        /// Sets SunshineWm2 based on GameCurrentTime and Coordinate for this weathersystem, 
        /// calculated from the formula from hell.
        /// </summary>
        public void RecalculateSunshine()
        {
            if (GameManager.Instance.Game == null || Coordinate == null)
            {
                return;
            }
            GameTime = GameManager.Instance.Game.GameCurrentTime;
            SunshineWm2 = CalculateSunShineWm2(GameTime, 0, Coordinate, CloudCover8ths);
            if (SunshineWm2 < 0)
            {
                SunshineWm2 = 0;
            }
            RecalculateEffectiveLight();
        }

        /// <summary>
        /// This method calculates the "effective light" as perceived by a normal human, based on
        /// pre-calculated SunshineWm2. SunshinePercent is set based on the sun, while TotalLightPercent
        /// has added some light based on IsMoonUp and CurrentMoonPhase. Contains hardcoded numbers.
        /// </summary>
        public void RecalculateEffectiveLight()
        {
            double sunLightPercent = 0;
            if (SunshineWm2 <= 0)
            {
                sunLightPercent = 0;
            }
            else if (SunshineWm2 < 5)
            {
                sunLightPercent = 5;
            }
            else if (SunshineWm2 < 20)
            {
                sunLightPercent = 10;
            }
            else if (SunshineWm2 < 40)
            {
                sunLightPercent = 15;
            }
            else if (SunshineWm2 < 85)
            {
                sunLightPercent = 35;
            }
            else if (SunshineWm2 < 165)
            {
                sunLightPercent = 70;
            }
            else if (SunshineWm2 < 300)
            {
                sunLightPercent = 80;
            }
            else if (SunshineWm2 < 600)
            {
                sunLightPercent = 90;
            }
            else 
            {
                sunLightPercent = 100;
            }
            SunshinePercent = sunLightPercent;
            TotalLightPercent = sunLightPercent;
            if(IsMoonUp && sunLightPercent < 2)
            {
                if (CurrentMoonPhase == GameConstants.TideEventType.NewMoon)
                {
                    TotalLightPercent = 4;
                }
                else if (CurrentMoonPhase == GameConstants.TideEventType.FirstQuarter
                    || CurrentMoonPhase == GameConstants.TideEventType.LastQuarter)
                {
                    TotalLightPercent = 6;
                }
                else if (CurrentMoonPhase == GameConstants.TideEventType.FullMoon)
                {
                    TotalLightPercent = 10;
                }
            }
        }

        
        #endregion

        #region "Static methods"

        public static double InterpolateDouble(double v1, double v2)
        {
            return ((Math.Max(v1, v2) - Math.Min(v1, v2)) / 2) + Math.Min(v1, v2);
        }

        public static double InterpolateWeighedDouble(double v1, double v2, double weightV1Percent)
        {
            double x = (100 - weightV1Percent) / 100.0;
            double fac1 = 3 * System.Math.Pow(1 - x, 2) - 2 * System.Math.Pow(1 - x, 3);
            double fac2 = 3 * System.Math.Pow(x, 2) - 2 * System.Math.Pow(x, 3);

            double result = (v1 * fac1) + (v2 * fac2); //add the weighted factors
            return result;

        }

        public static int InterpolateInt(int v1, int v2)
        {
            return ((Math.Max(v1, v2) - Math.Min(v1, v2)) / 2) + Math.Min(v1, v2);
        }

        public static WeatherSystem CreateInterpolatedWeatherSystem(Coordinate coord)
        {
            WeatherSystem system = new WeatherSystem();
            system.Coordinate = new Coordinate(coord);//coord;
            BlackboardFinder<WeatherSystem> finder = new BlackboardFinder<WeatherSystem>();
            WeatherSystem mainSystem = (WeatherSystem)finder.GetClosestObject(coord);
            if (mainSystem == null)
            {
                return null;
            }
            WeatherSystem otherSystem = null;
            List<IBlackboardObject> wsystems = finder.GetAllSortedByCoordinateAndType(coord, 10000000).ToList<IBlackboardObject>();
            //{
            //    WeatherSystem wsystem = wsys as WeatherSystem;
            //    if (wsystem !=null && wsys.Id != mainSystem.Id)
            //    {
            //        otherSystem = wsystem;
            //    }
            //}
            if (wsystems.Count > 1)
            {
                otherSystem = (WeatherSystem)wsystems[1];
            }
            system = (WeatherSystem)mainSystem.MemberwiseClone();
            system.Coordinate = new Coordinate(coord);//coord;
            system.GameTime = GameManager.Instance.Game.GameCurrentTime;
            double BearingDegToMain = MapHelper.CalculateBearingDegrees(coord, mainSystem.Coordinate);
            
            if (otherSystem != null)
            {
                double BearingDegToOther = MapHelper.CalculateBearingDegrees(coord, otherSystem.Coordinate);
                double DistanceToMain = MapHelper.CalculateDistanceM(mainSystem.Coordinate, coord);
                double DistanceToOther = MapHelper.CalculateDistanceM(otherSystem.Coordinate, coord);
                double WeightMainPercent = (DistanceToMain / (DistanceToMain + DistanceToOther)) * 100;
                //if (Math.Abs(BearingDegToMain - BearingDegToOther) > 100)
                //{
                system.CloudCover8ths = InterpolateInt(mainSystem.CloudCover8ths, 
                    otherSystem.CloudCover8ths);
                system.PrecipitationType = (GameConstants.PrecipitationType)InterpolateInt(
                    (int)mainSystem.PrecipitationType, (int)otherSystem.PrecipitationType);
                system.TemperatureC = Math.Round(InterpolateWeighedDouble(mainSystem.TemperatureC,
                    otherSystem.TemperatureC, WeightMainPercent));
                system.WindDirectionFromDeg = Math.Round(InterpolateWeighedDouble(mainSystem.WindDirectionFromDeg,
                    otherSystem.WindDirectionFromDeg, WeightMainPercent));
                system.WindSpeedMSec = Math.Round(InterpolateWeighedDouble(mainSystem.WindSpeedMSec,
                    otherSystem.WindSpeedMSec, WeightMainPercent));
//                }

            }
            //Add some random variation
            system.CloudCover8ths += GameManager.Instance.GetRandomNumber(2) - 1;
            if (system.CloudCover8ths > 8)
            {
                system.CloudCover8ths = 8;
            }
            else if (system.CloudCover8ths < 0)
            {
                system.CloudCover8ths = 0;
            }
            system.TemperatureC += GameManager.Instance.GetRandomNumber(2) - 1;
            system.WindSpeedMSec += GameManager.Instance.GetRandomNumber(2) - 1;
            if (system.WindSpeedMSec < 0)
            {
                system.WindSpeedMSec = 0;
            }
            if (system.Precipitation > GameConstants.PrecipitationLevel.None
                && system.Precipitation < GameConstants.PrecipitationLevel.Heavy)
            {
                system.Precipitation = (GameConstants.PrecipitationLevel)((int)system.Precipitation) 
                    + (GameManager.Instance.GetRandomNumber(2) - 1); 
            }
            if (system.Precipitation > GameConstants.PrecipitationLevel.None && system.CloudCover8ths < 7)
            {
                system.CloudCover8ths = 7;
            }
            if (system.Precipitation > GameConstants.PrecipitationLevel.Light && system.CloudCover8ths < 8)
            {
                system.CloudCover8ths = 8;
            }
            if (system.Precipitation != GameConstants.PrecipitationLevel.None)
            {
                if (system.TemperatureC <= 0)
                {
                    system.PrecipitationType = GameConstants.PrecipitationType.Snow;
                    if (system.TemperatureC > -5 && system.WindSpeedMSec < 15 
                        && GameManager.Instance.ThrowDice(5))
                    {
                        system.PrecipitationType = GameConstants.PrecipitationType.Hail;
                    }
                }
                else
                {
                    system.PrecipitationType = GameConstants.PrecipitationType.Rain;
                }
                
            }
            system.RecalculateMoonAndSunInformation();
            system.RecalculateSunshine();
            return system;
        }

        private static void GetMinMaxTempFromLatitude(GameConstants.WeatherSystemSeasonTypes season, double latitude, out double minTemp, out double maxTemp)
        {
            double lat = Math.Abs(latitude);
            lat = lat.Clamp(0, 90);
            minTemp = 5;
            maxTemp = 15; 
            switch (season)
            {
                case GameConstants.WeatherSystemSeasonTypes.Summer:
                    if (lat > 70)
                    {
                        minTemp = -1;
                        maxTemp = 10;

                    }
                    else if (lat > 60)
                    {
                        minTemp = 5;
                        maxTemp = 20;
                    }
                    else if (lat > 50)
                    {
                        minTemp = 10;
                        maxTemp = 25;
                    }
                    else if (lat > 40)
                    {
                        minTemp = 10;
                        maxTemp = 30;
                    }
                    else
                    {
                        minTemp = 15;
                        maxTemp = 40;
                    }
                    break;
                case GameConstants.WeatherSystemSeasonTypes.Spring:
                    if (lat > 70)
                    {
                        minTemp = -30;
                        maxTemp = -5;

                    }
                    else if (lat > 60)
                    {
                        minTemp = -10;
                        maxTemp = 10;
                    }
                    else if (lat > 50)
                    {
                        minTemp = 0;
                        maxTemp = 20;
                    }
                    else if (lat > 40)
                    {
                        minTemp = 15;
                        maxTemp = 30;
                    }
                    else
                    {
                        minTemp = 20;
                        maxTemp = 40;
                    }

                    break;
                case GameConstants.WeatherSystemSeasonTypes.Autumn:
                    if (lat > 70)
                    {
                        minTemp = -20;
                        maxTemp = -2;

                    }
                    else if (lat > 60)
                    {
                        minTemp = -5;
                        maxTemp = 12;
                    }
                    else if (lat > 50)
                    {
                        minTemp = 2;
                        maxTemp = 10;
                    }
                    else if (lat > 40)
                    {
                        minTemp = 5;
                        maxTemp = 20;
                    }
                    else
                    {
                        minTemp = 10;
                        maxTemp = 35;
                    }

                    break;
                case GameConstants.WeatherSystemSeasonTypes.Winter:
                case GameConstants.WeatherSystemSeasonTypes.ArcticWinter:
                    if (lat > 70)
                    {
                        minTemp = -50;
                        maxTemp = -20;

                    }
                    else if (lat > 60)
                    {
                        minTemp = -30;
                        maxTemp = -5;
                    }
                    else if (lat > 50)
                    {
                        minTemp = -10;
                        maxTemp = 10;
                    }
                    else if (lat > 40)
                    {
                        minTemp = -1;
                        maxTemp = 15;
                    }
                    else
                    {
                        minTemp = 10;
                        maxTemp = 30;
                    }
                    break;
            }

        }

        private static double GetRange(double minTemp, double maxTemp)
        {
            return maxTemp - minTemp;
        }

        public static WeatherSystem CreateRandomWeatherSystem(
            GameConstants.WeatherSystemTypes weatherType, 
            GameConstants.WeatherSystemSeasonTypes season,
            double latitude)
        {
            WeatherSystem system = new WeatherSystem();
            system.WindDirectionFromDeg = GameManager.Instance.GetRandomNumber(360);
            if (GameManager.Instance.ThrowDice(34))
            {
                system.WindDirectionFromDeg = GameManager.Instance.GetRandomNumber(30) + 240; //wind tends to be from the west
            }
            double MinTemp = 0;
            double MaxTemp = 0;
            GetMinMaxTempFromLatitude(season, latitude, out MinTemp, out MaxTemp);
            int TempRange = (int)GetRange(MinTemp, MaxTemp);
            system.TemperatureC = GameManager.Instance.GetRandomNumber(TempRange) + MinTemp;

            switch (weatherType)
            {
                case GameConstants.WeatherSystemTypes.Random:
                    system.CloudCover8ths = GameManager.Instance.GetRandomNumber(8);
                    if (season == GameConstants.WeatherSystemSeasonTypes.Summer 
                        && GameManager.Instance.ThrowDice(50))
                    {
                        system.CloudCover8ths = GameManager.Instance.GetRandomNumber(3);
                    }
                    system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(30);
                    if (system.WindSpeedMSec > 10 && GameManager.Instance.ThrowDice(25))
                    {
                        system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(5);
                    }
                    system.Precipitation = (GameConstants.PrecipitationLevel)
                        GameManager.Instance.GetRandomNumber((int)GameConstants.PrecipitationLevel.Heavy);
                    if (GameManager.Instance.ThrowDice(25))
                    {
                        system.Precipitation = GameConstants.PrecipitationLevel.None;
                    }
                    if (system.Precipitation > GameConstants.PrecipitationLevel.None)
                    {
                        if (system.TemperatureC > 0)
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Rain;
                        }
                        else
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Snow;
                        }
                    }

                    break;
                case GameConstants.WeatherSystemTypes.Fine:
                    system.CloudCover8ths = GameManager.Instance.GetRandomNumber(2);
                    system.Precipitation = GameConstants.PrecipitationLevel.None;
                    system.TemperatureC = system.TemperatureC.Clamp(-5, 25);
                    system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(5);

                    break;
                case GameConstants.WeatherSystemTypes.Fair:
                    system.CloudCover8ths = GameManager.Instance.GetRandomNumber(2);
                    system.Precipitation = GameConstants.PrecipitationLevel.None;
                    if (GameManager.Instance.ThrowDice(20))
                    {
                        system.Precipitation = GameConstants.PrecipitationLevel.Light;
                    }
                    system.TemperatureC = system.TemperatureC.Clamp(-5, 25);
                    system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(12);

                    
                    break;
                case GameConstants.WeatherSystemTypes.Rough:
                    system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(15) + 10;
                    system.Precipitation = (GameConstants.PrecipitationLevel)
                        GameManager.Instance.GetRandomNumber((int)GameConstants.PrecipitationLevel.Heavy);
                    if (system.Precipitation > GameConstants.PrecipitationLevel.None)
                    {
                        if (system.TemperatureC > 0)
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Rain;
                        }
                        else
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Snow;
                        }
                    }

                    break;
                case GameConstants.WeatherSystemTypes.Severe:
                    system.WindSpeedMSec = GameManager.Instance.GetRandomNumber(20) + 20;
                    system.Precipitation = (GameConstants.PrecipitationLevel)
                        GameManager.Instance.GetRandomNumber((int)GameConstants.PrecipitationLevel.Heavy);
                    if (system.Precipitation > GameConstants.PrecipitationLevel.None)
                    {
                        if (system.TemperatureC > 0)
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Rain;
                        }
                        else
                        {
                            system.PrecipitationType = GameConstants.PrecipitationType.Snow;
                        }
                    }

                    break;
            }
            if (system.Precipitation >= GameConstants.PrecipitationLevel.Light && system.CloudCover8ths < 7)
            {
                system.CloudCover8ths = 7;
            }
            if (system.Precipitation > GameConstants.PrecipitationLevel.Intermediate && system.CloudCover8ths < 8)
            {
                system.CloudCover8ths = 8;
            }

            return system;
        }

        public static double CalculateSunHeightDeg(DateTime time, Coordinate coordinate)
        {
            double DayNumber = time.DayOfYear;
            double HourDay = time.Hour;
            double SunDeclination = 23.45*(Math.Cos(Radians(360*(DayNumber - 173)/365.25)));
            double latitudeDeg = coordinate.LatitudeDeg;
            double longitudeDeg = coordinate.LongitudeDeg;

            //=(ASIN((SIN(RADIANS($C$4))*SIN(RADIANS(H8))-(COS(RADIANS($C$4))*COS(RADIANS(H8))*COS(RADIANS((180*(C8-1)/12)-$C$5))))))*180/3.1415926
            return (Math.Asin((Math.Sin(Radians(latitudeDeg)) * Math.Sin(Radians(SunDeclination))
                - (Math.Cos(Radians(latitudeDeg)) * Math.Cos(Radians(SunDeclination))
                * Math.Cos(Radians((180 * (HourDay - 1) / 12) - longitudeDeg)))))) * 180 / Math.PI;
                    
        }

        public static DateTimeFromTo CalculateSunRiseSunSet(DateTime time, Coordinate coordinate)
        {
            DateTime theDate = time.Date;
            DaylightTime daylightChanges = TimeZone.CurrentTimeZone.GetDaylightChanges(time.Year);
            SunTime sunTime = new SunTime(coordinate.LatitudeDeg, coordinate.LongitudeDeg,
                1, daylightChanges, time);
            DateTimeFromTo result = new DateTimeFromTo(sunTime.RiseTime, sunTime.SetTime);
            return result;
        }




        public static double CalculateLocalSiderealTime(double lon, double jd, double z )
        {
            var s = 24110.5 + 8640184.812999999*jd/36525 + 86636.6*z + 86400*lon;
            s = s/86400;
            s = s - Math.Floor(s);
            return s*360*(Math.PI / 180);
        }

        // calculate moonrise and moonset times

        public static double[] CalculateMoon(double  jd )
        {
            double d, f, g, h, m, n, s, u, v, w;
            double[] Sky = new double[3];
            h = 0.606434 + 0.03660110129*jd;
            m = 0.374897 + 0.03629164709*jd;
            f = 0.259091 + 0.0367481952 *jd;
            d = 0.827362 + 0.03386319198*jd;
            n = 0.347343 - 0.00014709391*jd;
            g = 0.993126 + 0.0027377785 *jd;
         
            h = h - Math.Floor(h);
            m = m - Math.Floor(m);
            f = f - Math.Floor(f);
            d = d - Math.Floor(d);
            n = n - Math.Floor(n);
            g = g - Math.Floor(g);

            h = h * 2 * Math.PI;
            m = m * 2 * Math.PI;
            f = f * 2 * Math.PI;
            d = d * 2 * Math.PI;
            n = n * 2 * Math.PI;
            g = g * 2 * Math.PI;
         
            v = 0.39558*Math.Sin(f + n);
            v = v + 0.082  *Math.Sin(f);
            v = v + 0.03257*Math.Sin(m - f - n);
            v = v + 0.01092*Math.Sin(m + f + n);
            v = v + 0.00666*Math.Sin(m - f);
            v = v - 0.00644*Math.Sin(m + f - 2*d + n);
            v = v - 0.00331*Math.Sin(f - 2*d + n);
            v = v - 0.00304*Math.Sin(f - 2*d);
            v = v - 0.0024 *Math.Sin(m - f - 2*d - n);
            v = v + 0.00226*Math.Sin(m + f);
            v = v - 0.00108*Math.Sin(m + f - 2*d);
            v = v - 0.00079*Math.Sin(f - n);
            v = v + 0.00078*Math.Sin(f + 2*d + n);
            
            u = 1 - 0.10828*Math.Cos(m);
            u = u - 0.0188 *Math.Cos(m - 2*d);
            u = u - 0.01479*Math.Cos(2*d);
            u = u + 0.00181*Math.Cos(2*m - 2*d);
            u = u - 0.00147*Math.Cos(2*m);
            u = u - 0.00105*Math.Cos(2*d - g);
            u = u - 0.00075*Math.Cos(m - 2*d + g);
            
            w = 0.10478*Math.Sin(m);
            w = w - 0.04105*Math.Sin(2*f + 2*n);
            w = w - 0.0213 *Math.Sin(m - 2*d);
            w = w - 0.01779*Math.Sin(2*f + n);
            w = w + 0.01774*Math.Sin(n);
            w = w + 0.00987*Math.Sin(2*d);
            w = w - 0.00338*Math.Sin(m - 2*f - 2*n);
            w = w - 0.00309*Math.Sin(g);
            w = w - 0.0019 *Math.Sin(2*f);
            w = w - 0.00144*Math.Sin(m + n);
            w = w - 0.00144*Math.Sin(m - 2*f - n);
            w = w - 0.00113*Math.Sin(m + 2*f + 2*n);
            w = w - 0.00094*Math.Sin(m - 2*d + g);
            w = w - 0.00092*Math.Sin(2*m - 2*d);
         
            s = w/Math.Sqrt(u - v*v);                  // compute moon's right ascension ...  
            Sky[0] = h + Math.Atan(s/Math.Sqrt(1 - s*s));
         
            s = v/Math.Sqrt(u);                        // declination ...
            Sky[1] = Math.Atan(s/Math.Sqrt(1 - s*s));
         
            Sky[2] = 60.40974*Math.Sqrt ( u );          // and parallax
            return Sky;
        }

        // 3-point interpolation
        public static double Interpolate3values(double f0, double f1, double f2, double p )
        {
            var a = f1 - f0;
            var b = f2 - f1 - a;
            var f = f0 + p*(2*a + b*(2*p - 1));
         
            return f;
        }

        public static MoonRiseResult test_moon(DateTime date, 
            double k, double zone, double t0, double lat, double plx )
        {
            MoonRiseResult result = new MoonRiseResult();

            double[] ha = new double[3] {0.0, 0.0, 0.0};
            double a, b, c, d, e, s, z;
            double hr, min, time;
            double az, hz, nz, dz;

            double[] RAn = new double[3];
            double[] Dec = new double[3];
            double[] VHz = new double[3];

            double DR = Math.PI/180;
            double K1 = 15 * DR * 1.0027379;

            if (RAn[2] < RAn[0])
                RAn[2] = RAn[2] + 2*Math.PI;
            
            ha[0] = t0 - RAn[0] + k*K1;
            ha[2] = t0 - RAn[2] + k*K1 + K1;
            
            ha[1]  = (ha[2] + ha[0])/2;                // hour angle at half hour
            Dec[1] = (Dec[2] + Dec[0])/2;              // declination at half hour
         
            s = Math.Sin(DR*lat);
            c = Math.Cos(DR*lat);
         
            // refraction + sun semidiameter at horizon + parallax correction
            z = Math.Cos(DR*(90.567 - 41.685/plx));
         
            if (k <= 0)                                // first call of function
                VHz[0] = s*Math.Sin(Dec[0]) + c*Math.Cos(Dec[0])*Math.Cos(ha[0]) - z;
         
            VHz[2] = s*Math.Sin(Dec[2]) + c*Math.Cos(Dec[2])*Math.Cos(ha[2]) - z;

            if (Math.Sign(VHz[0]) == Math.Sign(VHz[2]))
            {
                result.Vhz = VHz[2];
                return result; // VHz[2];                         // no event this hour
            }
            
            VHz[1] = s*Math.Sin(Dec[1]) + c*Math.Cos(Dec[1])*Math.Cos(ha[1]) - z;
         
            a = 2*VHz[2] - 4*VHz[1] + 2*VHz[0];
            b = 4*VHz[1] - 3*VHz[0] - VHz[2];
            d = b*b - 4*a*VHz[0];

            if (d < 0)
            {
                result.Vhz = VHz[2];
                return result; // VHz[2];                         // no event this hour
            }
            
            d = Math.Sqrt(d);
            e = (-b + d)/(2*a);
         
            if (( e > 1 )||( e < 0 ))
                e = (-b - d)/(2*a);
         
            time = k + e + 1/120;                      // time of an event + round up
            hr   = Math.Floor(time);
            min  = Math.Floor((time - hr)*60);
         
            hz = ha[0] + e*(ha[2] - ha[0]);            // azimuth of the moon at the event
            nz = -Math.Cos(Dec[1])*Math.Sin(hz);
            dz = c*Math.Sin(Dec[1]) - s*Math.Cos(Dec[1])*Math.Cos(hz);
            az = Math.Atan2(nz, dz)/DR;
            if (az < 0) az = az + 360;

            if ((VHz[0] < 0) && (VHz[2] > 0))
            {
                result.RiseTime = date.Date.AddHours(hr).AddMinutes(min);
                result.RiseAz = az;
                result.IsMoonRise = true;
                result.Vhz = VHz[2];
                //Rise_time[0] = hr;
                //Rise_time[1] = min;
                //Rise_az = az;
                //Moonrise = true;
            }

            if ((VHz[0] > 0) && (VHz[2] < 0))
            {
                result.SetTime = date.Date.AddHours(hr).AddMinutes(min);
                result.SetAZ = az;
                result.IsMoonSet = true;
                result.Vhz = VHz[2];
                //Set_time[0] = hr;
                //Set_time[1] = min;
                //Set_az = az;
                //Moonset = true;
            }
         
            return result;
        }


        public static MoonRiseResult CalculateMoonRiseSet(Coordinate coordinate, DateTime time)
        {
            int zone = TimeZone.CurrentTimeZone.ToUniversalTime(time).Hour; //1; // Math.Round(Now.getTimezoneOffset()/60);

            double jd = time.DtToJulianDay(); // -2451545;           // Julian day relative to Jan 1.5, 2000


            
            //if ((sgn(zone) == sgn(lon))&&(zone != 0))
            //    window.alert("WARNING: time zone and longitude are incompatible!");
         
            double[,] mp = new double[3,3];                     // create a 3x3 array
            for (int i = 0; i < 3; i++)
            {
                //mp[i] = new Array(3);
                for (int j = 0; j < 3; j++)
                {
                    mp[i, j] = 0.0;
                }
            }
         
            double lon = coordinate.LongitudeDeg/360;
            var tz = zone/24;
            var t0 = CalculateLocalSiderealTime(lon, jd, tz);                 // local sidereal time
         
            jd = jd + tz;                              // get moon position at start of day
         
            for (int k = 0; k < 3; k++)
            {
                double[] Sky =  CalculateMoon(jd);
                mp[k,0] = Sky[0];
                mp[k,1] = Sky[1];
                mp[k,2] = Sky[2];
                jd = jd + 0.5;      
            }   
         
            if (mp[1,0] <= mp[0,0])
                mp[1,0] = mp[1,0] + 2*Math.PI;
         
            if (mp[2,0] <= mp[1,0])
                mp[2,0] = mp[2,0] + 2*Math.PI;
            
            double[] RAn = new double[3];
            double[] Dec = new double[3];
            double[] VHz = new double[3];

            RAn[0] = mp[0,0];
            Dec[0] = mp[0,1];
         
            //bool Moonrise = false;                          // initialize
            //bool Moonset  = false;
            List<MoonRiseResult> resultArray = new List<MoonRiseResult>();
            for (int k = 0; k < 24; k++)                   // check each hour of this day
            {
                double ph = (k + 1)/24;
                
                RAn[2] = Interpolate3values(mp[0,0], mp[1,0], mp[2,0], ph);
                Dec[2] = Interpolate3values(mp[0,1], mp[1,1], mp[2,1], ph);
                
                MoonRiseResult TestMoonHour = test_moon(time, k, zone, t0, coordinate.LatitudeDeg, mp[1,2]);
                resultArray.Add(TestMoonHour);
                VHz[2] = TestMoonHour.Vhz;
                RAn[0] = RAn[2];                       // advance to next hour
                Dec[0] = Dec[2];
                VHz[0] = VHz[2];
            }
            MoonRiseResult result = new MoonRiseResult();
            foreach (var moonres in resultArray)
            {
                if (moonres.IsMoonRise)
                {
                    result.IsMoonRise = true;
                    result.RiseTime = moonres.RiseTime;
                    result.RiseAz = moonres.RiseAz;
                }
                if (moonres.IsMoonSet)
                {
                    result.IsMoonSet = true;
                    result.SetTime = moonres.SetTime;
                    result.SetAZ = moonres.SetAZ;
                }
            }
            // display results
            //calc.moonrise.value = zintstr(Rise_time[0], 2) + ":" + zintstr(Rise_time[1], 2) 
            //                   + ", az = " + frealstr(Rise_az, 5, 1) + "°";
            //calc.moonset.value  = zintstr( Set_time[0], 2) + ":" + zintstr( Set_time[1], 2)
            //                   + ", az = " + frealstr(Set_az, 5, 1) + "°";
            return result;
        }


        public static double CalculateSunDeclination(DateTime time, Coordinate coordinate)
        {
            double DayNumber = time.DayOfYear;
            return 23.45 * (Math.Cos(Radians(360 * (DayNumber - 173) / 365.25)));
        }


        public static double CalculateSunShineWm2(DateTime time, double dowPoint, Coordinate coordinate, int cloudCover8s )
        { 
        //=(1-(0.8*(G8/8)*(G8/8)*(G8/8)))*((COS((90-((ASIN(((SIN(RADIANS(C4))*SIN(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))-(COS(RADIANS(C4))*COS(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))*COS(RADIANS((180*(C8-1.5)/12)-C5)))))))*180/3.1415926))*3.14/180))/(((COS((90-((ASIN(((SIN(RADIANS(C4))*SIN(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))-(COS(RADIANS(C4))*COS(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))*COS(RADIANS((180*(C8-1.5)/12)-C5)))))))*180/3.1415926))*3.14/180))+(1+(COS((90-((ASIN(((SIN(RADIANS(C4))*SIN(RADIANS(H8))-(COS(RADIANS(C4))*COS(RADIANS(H8))*COS(RADIANS((180*(C8-1.5)/12)-C5))))))))*180/3.1415926)*3.14/180)))*(2530000000*(POWER(2.72,(-5420/(F8+273.15)))))*0.001)+0.046))*1370*COS((90-((ASIN(((SIN(RADIANS(C4))*SIN(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))-(COS(RADIANS(C4))*COS(RADIANS((23.45*(COS(RADIANS(360*(D8-173)/365.25))))))*COS(RADIANS((180*(C8-1.5)/12)-C5)))))))*180/3.1415926))*3.14/180)*0.83

            double DayNumber = time.DayOfYear;
            double HourDay = time.Hour;
            double SunDeclination = 23.45*(Math.Cos(Radians(360*(DayNumber - 173)/365.25)));
            double latitudeDeg = coordinate.LatitudeDeg;
            double longitudeDeg = coordinate.LongitudeDeg;

            return (1 - (0.8 * (cloudCover8s / 8) * (cloudCover8s / 8) * (cloudCover8s / 8))) 
                * ((Math.Cos((90 - ((Math.Asin(((Math.Sin(Radians(latitudeDeg)) 
                * Math.Sin(Radians((23.45 * (Math.Cos(Radians(360 * (DayNumber - 173) / 365.25)))))) 
                - (Math.Cos(Radians(latitudeDeg)) * Math.Cos(Radians((23.45 * (Math.Cos(Radians(360
                * (DayNumber - 173) / 365.25)))))) * Math.Cos(Radians((180 * (HourDay - 1.5) / 12) 
                - longitudeDeg))))))) * 180 / Math.PI)) * Math.PI / 180)) / (((Math.Cos((90 
                - ((Math.Asin(((Math.Sin(Radians(latitudeDeg)) * Math.Sin(Radians((23.45 
                * (Math.Cos(Radians(360 * (DayNumber - 173) / 365.25)))))) 
                - (Math.Cos(Radians(latitudeDeg)) * Math.Cos(Radians((23.45 
                * (Math.Cos(Radians(360 * (DayNumber - 173) / 365.25)))))) 
                * Math.Cos(Radians((180 * (HourDay - 1.5) / 12) - longitudeDeg))))))) 
                * 180 / Math.PI)) * Math.PI / 180)) + (1 + (Math.Cos((90 
                - ((Math.Asin(((Math.Sin(Radians(latitudeDeg)) * Math.Sin(Radians(SunDeclination)) 
                - (Math.Cos(Radians(latitudeDeg)) * Math.Cos(Radians(SunDeclination)) 
                * Math.Cos(Radians((180 * (HourDay - 1.5) / 12) - longitudeDeg)))))))) 
                * 180 / Math.PI) * Math.PI / 180))) * (2530000000
                * (Math.Pow(2.72, (-5420 / (dowPoint + 273.15))))) * 0.001) + 0.046)) * 1370 
                * Math.Cos((90 - ((Math.Asin(((Math.Sin(Radians(latitudeDeg)) 
                * Math.Sin(Radians((23.45 * (Math.Cos(Radians(360 
                * (DayNumber - 173) / 365.25)))))) - (Math.Cos(Radians(latitudeDeg)) 
                * Math.Cos(Radians((23.45 * (Math.Cos(Radians(360 * (DayNumber - 173) / 365.25))))))
                * Math.Cos(Radians((180 * (HourDay - 1.5) / 12) - longitudeDeg))))))) * 180 / Math.PI)) 
                * Math.PI / 180) * 0.83;
        }

        private static double Radians(double degree)
        {
            return degree.ToRadian();
        }

        #endregion

        #region "Public methods"

        public WeatherSystemInfo GetWeatherSystemInfo()
        {
            WeatherSystemInfo info = new WeatherSystemInfo();
            info.CloudCover8ths = CloudCover8ths;
            info.Precipitation = Precipitation;
            info.PrecipitationType = PrecipitationType;
            info.SeaState = SeaState;
            info.TemperatureC = TemperatureC;
            info.WindDirectionFromDeg = WindDirectionFromDeg;
            info.WindDirectionFrom = WindDirectionFrom;
            info.WindForceBeaufort = WindForceBeaufort;
            info.WindSpeedMSec = WindSpeedMSec;
            info.SunshineWm2 = Math.Round(SunshineWm2);
            info.NextTideEvent = NextTideEvent;
            info.IsMoonUp = IsMoonUp;
            info.CurrentMoonPhase = CurrentMoonPhase;
            info.NextTideEvent = NextTideEvent;
            info.TimeNextTideEvent = TimeNextTideEvent;
            info.GameTime = new NWDateTime(GameTime);
            info.CurrentSunDeclinationDeg = CurrentSunDeclinationDeg;
            info.CurrentSunheightDeg = CurrentSunheightDeg;
            info.IsSunRising = IsSunRising;
            info.IsSunSetting = IsSunSetting;
            info.JulianDay = JulianDay;
            if (IsSunSetting)
            {
                info.SunsetTime = new NWDateTime(SunsetTime);
            }
            if (IsSunRising)
            {
                info.SunriseTime = new NWDateTime(SunriseTime);
            }
            info.TotalLightPercent = TotalLightPercent;
            info.SunshinePercent = SunshinePercent;

            return info;
        }

        #endregion

        #region "Public Overrides"

        public override string ToString()
        {
            return GetWeatherSystemInfo().ToString(); //avoid redundancy
        }

        public override bool Equals(object obj)
        {
            WeatherSystem other = obj as WeatherSystem;
            if (other == null)
            {
                return false;
            }
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region IBlackboardObject Members

        public string Id { get; set; }

        public string OwnerId { get; set; }

        public Coordinate Coordinate { get; set; }


        public double DistanceToM(Coordinate coordinate)
        {
            if (Coordinate == null || coordinate == null)
            {
                return 0; //hmm
            }
            return MapHelper.CalculateDistanceM(Coordinate, coordinate);
        }

        #endregion
    }
}
