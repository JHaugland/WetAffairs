using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class DetectedGroupInfo:IMarshallable
    {
        #region "Constructors"

        public DetectedGroupInfo()
        {
            DetectedUnitIds = new List<string>();
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> DetectedUnitIds { get; set; }

        public PositionInfo Position { get; set; }

        public string Description { get; set; }

        #endregion



        #region "Public methods"
        public override string ToString()
        {
            return Description;
        }

        #endregion


    
        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens  ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.DetectedGroupInfo; }
        }

#endregion
}
}
