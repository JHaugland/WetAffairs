using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class AIHint : IBlackboardObject
    {
        #region "Constructors"

        public AIHint()
        {
            if (GameManager.Instance.Game != null && GameManager.Instance.Game.GameCurrentTime != null)
            {
                GameTime = GameManager.Instance.Game.GameCurrentTime;
            }
        }

        public AIHint(AIHintInfo info):this()
        {
            this.Id = info.Id;
            this.AIHintType = info.AIHintType;
            this.DirectionDeg = info.DirectionDeg;
            if (info.GameTime != null)
            { 
                this.GameTime = info.GameTime.GetDateTime();
            }
            this.OwnerId = info.OwnerId;
            if (info.Position != null)
            { 
                this.Coordinate = new Coordinate(info.Position.Latitude, info.Position.Longitude);
            }
            this.RadiusM = info.RadiusM;
            this.Strength = info.Strength;
        }
        #endregion


        #region "Public properties"

        public GameConstants.AIHintType AIHintType { get; set; }

        public Player OwnerPlayer
        {
            get
            {
                if (GameManager.Instance.Game != null)
                {
                    return GameManager.Instance.Game.GetPlayerById(OwnerId);
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                OwnerId = value != null ? value.Id : string.Empty;
            }
        }

        public DateTime GameTime { get; set; }

        public double Strength { get; set; }

        public double DirectionDeg { get; set; }

        public GameConstants.DirectionCardinalPoints Direction
        {
            get
            {
                return DirectionDeg.ToCardinalMark();
            }
        }

        public double RadiusM { get; set; }

        #endregion



        #region "Public methods"

        public AIHintInfo GetAiHintInfo()
        {
            AIHintInfo info = new AIHintInfo();
            info.Id = this.Id;
            info.Position = new PositionInfo(this.Coordinate.LatitudeDeg, this.Coordinate.LongitudeDeg);
            info.AIHintType = this.AIHintType;
            info.DirectionDeg = this.DirectionDeg;
            info.Direction = this.Direction;
            info.GameTime = new NWDateTime(this.GameTime);
            info.RadiusM = this.RadiusM;
            info.Strength = this.Strength;
            info.OwnerId = this.OwnerId;
            return info;
        }
        public override string ToString()
        {
            string temp = AIHintType.ToString();
            if (Coordinate != null)
            {
                temp += " " + Coordinate.ToString();
            }
            temp += " >>" + Direction;
            return temp;
        }

        #endregion



        #region IBlackboardObject Members

        public string Id { get; set; }

        public string OwnerId { get; set; }

        public Coordinate Coordinate { get; set; }


        public double DistanceToM(Coordinate coordinate)
        {
            if (Coordinate == null || coordinate == null)
            {
                return 0;
            }
            return MapHelper.CalculateDistanceM(Coordinate, coordinate);
        }

        #endregion
    }
}
