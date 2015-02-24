using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class MessageInfo : IMarshallable
    {
        #region "Constructors"

        public MessageInfo()
        {

        }
        
        #endregion


        #region "Public properties"

        public string Id { get; set; }
        public string MessageBody { get; set; }
        public PositionInfo Position { get; set; }
        public GameConstants.Priority Priority { get; set; }
        public string FromPlayerId { get; set; }
        public string FromPlayerName { get; set; }
        public string ToPlayerId { get; set; }
        public bool IsRead { get; set; }
        //public bool HasBeenSentToClient { get; set; }
        public double GameTimeGenerated { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            string temp = string.Empty;
            if (!string.IsNullOrEmpty(FromPlayerId))
            {
                temp = "From: " + FromPlayerName + "\n";
            }
            if (!string.IsNullOrEmpty(ToPlayerId))
            {
                temp += "To: " + ToPlayerId + "\n";
            }
            else
            {
                temp += "To: (none)\n";
            }
            temp += MessageBody + "\n";
            return temp;
        }
  
        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.MessageInfo; }
        }

        #endregion
    }
}
