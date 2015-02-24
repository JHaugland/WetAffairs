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
    public class AreaEffect : IBlackboardObject
    {
        #region "Constructors"

        public AreaEffect()
        {

        }

        public AreaEffect(GameConstants.AreaEffectType areaEffectType)
            : this()
        {
            AreaEffectType = areaEffectType;
        }

        #endregion


        #region "Public properties"

        public GameConstants.AreaEffectType AreaEffectType { get; set; }

        /// <summary>
        /// The 'weapon' creating the area effect.
        /// </summary>
        public BaseWeapon Weapon { get; set; }

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

        public double TimeToLiveSec { get; set; }

        public double Strength { get; set; }

        public double RadiusM { get; set; }

        #endregion



        #region "Public methods"

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
