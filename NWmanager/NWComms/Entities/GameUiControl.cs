using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameUiControl : IMarshallable
    {
        #region "Constructors"

        public GameUiControl()
        {

        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public string Tag { get; set; }

        public GameConstants.GameUiControlType GameUiControlType { get; set; }

        public PositionInfo Position { get; set; }

        public float RadiusM { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return "GameUiControl " + GameUiControlType;
        }

        public GameUiControl Clone()
        {
            return (GameUiControl)this.MemberwiseClone();
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameUiControl; }
        }

        #endregion
    }
}
