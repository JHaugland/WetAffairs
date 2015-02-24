using System;
using System.Collections.Generic;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameStateInfo: IMarshallable
    {
        #region "Constructors"

        public GameStateInfo()
        {
            IdList = new List<string>();
        }

        public GameStateInfo(GameConstants.GameStateInfoType infoType, string id) : this()
        {
            InfoType = infoType;
            Id = id;
        }

        public GameStateInfo(GameConstants.GameStateInfoType infoType, string id, string secondId)
            : this()
        {
            InfoType = infoType;
            Id = id;
            SecondaryId = secondId;
        }


        #endregion


        #region "Public properties"

        public GameConstants.GameStateInfoType InfoType { get; set; }

        public string Id { get; set; }

        public string SecondaryId { get; set; }

        public string UnitClassId { get; set; }

        public string WeaponClassId { get; set; }

        public string WeaponId { get; set; }

        public double BearingDeg { get; set; }

        public double RadiusM { get; set; }

        public int Count { get; set; }

        public PositionInfo Position { get; set; }

        public List<string> IdList { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return string.Format("GameStateInfo {0} for Id {1}", InfoType, Id);
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameStateInfo; }
        }

        #endregion
    }
}
