using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class WeatherSystemInfo : IMarshallable
    {
        #region "Constructors"

        public WeatherSystemInfo()
        {

        }

        #endregion


        #region "Public properties"

        public TTG.NavalWar.NWComms.Entities.NWDateTime GameTime { get; set; }

        public double TemperatureC { get; set; }

        public int CloudCover8ths { get; set; }

        public GameConstants.PrecipitationLevel Precipitation { get; set; }

        public GameConstants.PrecipitationType PrecipitationType { get; set; }

        public double WindSpeedMSec { get; set; }

        public double WindDirectionFromDeg { get; set; }

        public GameConstants.DirectionCardinalPoints WindDirectionFrom { get; set; }

        public GameConstants.BeaufortScale WindForceBeaufort { get; set; }

        public DateTime TimeNextTideEvent { get; set; }
        
        public GameConstants.TideEvent NextTideEvent { get; set; }

        public int SeaState { get; set; }

        public GameConstants.TideEventType CurrentMoonPhase { get; set; }

        public bool IsMoonUp { get; set; }

        public NWDateTime SunriseTime { get; set; }

        public NWDateTime SunsetTime { get; set; }

        public bool IsSunRising { get; set; }

        public bool IsSunSetting { get; set; }

        public double CurrentSunheightDeg { get; set; }

        public double CurrentSunDeclinationDeg { get; set; }

        public double JulianDay { get; set; }

        public double SunshineWm2 { get; set; }

        public double SunshinePercent { get; set; }

        public double TotalLightPercent { get; set; }


        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string PrecipDesc = Precipitation.ToString() + " " + PrecipitationType.ToString();
            if (Precipitation == GameConstants.PrecipitationLevel.None)
            {
                PrecipDesc = "None";
            }
            string temp = string.Format(
                "Temp: {0}C, Clouds: {1}/8ths, {2} m/s wind from {3}, Beaufort: {4},"
                + " Precipitation: {5}, Sea state: {6} \nMoon phase: {7}, Is Moon Up? {8}, Sunshine: {9:F} Wm2 {10}% \n",
                TemperatureC, CloudCover8ths, WindSpeedMSec, WindDirectionFrom,
                WindForceBeaufort, PrecipDesc, SeaState, CurrentMoonPhase, IsMoonUp, SunshineWm2, SunshinePercent);
            string sunRiseSetDesc = string.Empty;
            if (IsSunRising)
            {
                sunRiseSetDesc = "Sun rising: " + SunriseTime.ToString() + " \n";
            }
            if (IsSunSetting)
            {
                sunRiseSetDesc += "Sun setting: " + SunsetTime.ToString() + " \n ";
            }
            temp += string.Format("Sun height: {0:F} deg, Sun declination: {1:F} deg \n{2}",
                CurrentSunheightDeg, CurrentSunDeclinationDeg, sunRiseSetDesc);
            return temp;
        }
        
        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.WeatherSystemInfo; }
        }

        #endregion

        
    }
}
