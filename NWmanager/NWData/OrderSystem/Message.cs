using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Message : GameObject
    {
        #region "Constructors"

        public Message(): base()
        {
            
        }
        public Message(Player toPlayer, Player fromPlayer) : this()
        {
            ToPlayer = toPlayer;
            FromPlayer = fromPlayer;
            
        }

        #endregion


        #region "Public properties"
        
        //[Obsolete]
        //public string Subject { get; set; }
        
        public string MessageBody { get; set; }
        public Position Position { get; set; }
        public GameConstants.Priority Priority { get; set; }
        public Player FromPlayer { get; set; }
        public Player ToPlayer { get; set; }
        public bool IsRead { get; set; }
        public bool HasBeenSentToClient { get; set; }
        public double GameTimeGeneratedSec { get; set; }

        #endregion


        #region "Public methods"

        public MessageInfo GetMessageInfo()
        {
            MessageInfo info = new MessageInfo();
            info.Id = Id;
            info.MessageBody = MessageBody;
            info.IsRead = IsRead;
            if(Position != null)
            {
                info.Position = Position.GetPositionInfo();
            }
            info.Priority = Priority;
            if(FromPlayer != null)
            {
                info.FromPlayerId = FromPlayer.Id;
                info.FromPlayerName = FromPlayer.Name;

            }
            if (ToPlayer != null)
            {
                info.ToPlayerId = ToPlayer.Id;
            }
            info.GameTimeGenerated = GameTimeGeneratedSec;
            return info;
        }

        public override string ToString()
        {
            string temp = string.Empty;
            if (FromPlayer != null)
            {
                temp = "From: " + FromPlayer.ToString() + "\n";
            }
            if (ToPlayer != null)
            {
                temp += "To: " + ToPlayer.ToString() + "\n";
            }
            else
            {
                temp += "To: (none)\n";
            }
            temp += MessageBody + "\n";
            return temp;
        }
        #endregion


    }
}
