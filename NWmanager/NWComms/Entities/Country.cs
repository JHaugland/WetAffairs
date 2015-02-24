using System;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class Country : IGameDataObject
    {
        #region "Public properties"

        public string Id { get; set; }

        public string CountryNameShort { get; set; }

        public string CountryNameLong { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(CountryNameShort))
            {
                return CountryNameShort;
            }
            else
            {
                return "Country " + Id;
            }

        }

        #endregion
    }
}
