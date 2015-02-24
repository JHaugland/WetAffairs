using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    /// <summary>
    /// Mono binary deserialization butchers the DateTime object, thus we had to roll our own.
    /// </summary>
    [Serializable]
    public class NWDateTime :IMarshallable
    {
        #region "Constructors"

        public NWDateTime()
        {

        }

        public NWDateTime(DateTime time)
        {
            Year = time.Year;
            Month = time.Month;
            Day = time.Day;
            Hour = time.Hour;
            Minute = time.Minute;
            Second = time.Second;
            Millisecond = time.Millisecond;
        }
        #endregion


        #region "Public properties"

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }

        #endregion



        #region "Public methods"

        public DateTime GetDateTime()
        {
            DateTime time = new DateTime(Year, Month, Day, Hour, Minute, Second);
            time =time.AddMilliseconds(Millisecond);
            return time;
        }

        public string ToShortTimeString()
        {
            try
            {
                return GetDateTime().ToShortTimeString();
            }
            catch (Exception ex)
            {
                return "Invalid NWDateTime." + ex.ToString();
            }
        }

        public override string ToString()
        {
            try
            {
                return GetDateTime().ToString();
            }
            catch (Exception ex)
            {
                return "Invalid NWDateTime." + ex.ToString();
            }
        }    

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.NWDateTime; }
        }

        #endregion


    }
}
