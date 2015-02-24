using System;
using System.Collections.Generic;

using System.Text;



namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class AIHintInfo: IMarshallable
    {

        public AIHintInfo()
        {

        }


        #region "Public properties"

        public string Id { get; set; }

        public GameConstants.AIHintType AIHintType { get; set; }
        
        public NWDateTime GameTime { get; set; }

        public double Strength { get; set; }

        public double DirectionDeg { get; set; }

        public GameConstants.DirectionCardinalPoints Direction { get; set; }

        public double RadiusM { get; set; }

        public string OwnerId { get; set; }

        public PositionInfo Position{ get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {

            string temp = AIHintType.ToString();
            if (Position != null)
            {
                temp += " " + Position.ToString();
            }
            temp += " [" + Direction + "]";
            return temp;
        }

        #endregion


        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get {return CommsMarshaller.ObjectTokens.AiHintInfo; }
        }
    }
}
