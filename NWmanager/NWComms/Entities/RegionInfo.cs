using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class RegionInfo: IMarshallable
    {
        private List<PositionInfo> _CoordinateList = new List<PositionInfo>();

        #region "Constructors"

        public RegionInfo()
        {

        }

        #endregion


        #region "Public properties"

        public List<PositionInfo> Coordinates
        {
            get { return _CoordinateList; }
        }

        public int Count
        {
            get
            {
                return _CoordinateList.Count;
            }
        }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            string temp = "Region: ";
            foreach (var c in Coordinates)
            {
                temp += "\n" + c.ToString();
            }
            return temp;
        }

        #endregion

        #region "Public static methods"
        

        #endregion

    
        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens  ObjectTypeToken
        {
	        get { return CommsMarshaller.ObjectTokens.RegionInfo; }
        }

        #endregion
    }
}
