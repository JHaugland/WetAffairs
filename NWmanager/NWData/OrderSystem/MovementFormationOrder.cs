using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using System.Diagnostics;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class MovementFormationOrder : MovementOrder
    {
        private Waypoint _waypoint;

        #region "Constructors"

        public MovementFormationOrder() : base()
        {
            PositionOffset = new PositionOffset();
            OrderType = TTG.NavalWar.NWComms.GameConstants.OrderType.MovementFormationOrder;

        }

        public MovementFormationOrder(BaseUnit unitToFollow, double rightM, double forwardM, double upM)
            : this()
        {
            UnitToFollow = unitToFollow;
            PositionOffset = new PositionOffset(rightM, forwardM, upM);

            GetActiveWaypoint();
        }

        public MovementFormationOrder(BaseUnit unitToFollow, PositionOffset offset)
            : this()
        {
            UnitToFollow = unitToFollow;
            PositionOffset = offset;
            GetActiveWaypoint();
        }

        public MovementFormationOrder(BaseUnit mainUnit, 
                                            string formationPositionId,
                                            string formationId,
                                            PositionOffset positionOffset) : this()
        {
            UnitToFollow = mainUnit;
            if (!string.IsNullOrEmpty(formationId) && !string.IsNullOrEmpty(formationPositionId))
            {
                try
                {
                    GameData gameData = GameManager.Instance.GameData;
                    Formation formation = gameData.GetFormationById(formationId);
                    if (formation != null)
                    {
                        FormationPosition offset = formation.FormationPositions.Find(p => p.Id == formationPositionId);
                        PositionOffset = offset.PositionOffset;
                    }
                    else
                    {
                        PositionOffset = positionOffset;
                    }


                }
                catch (Exception ex)
                {
                    GameManager.Instance.Log.LogError(
                        "MovementFormationOrder instantiated with invalid data. " + ex.ToString());
                    if (positionOffset != null)
                    {
                        PositionOffset = positionOffset;
                    }
                    else
                    {
                        PositionOffset = new PositionOffset();
                    }
                }
            }
            else
            {
                if (positionOffset != null)
                {
                    PositionOffset = positionOffset;
                }
            }
            GetActiveWaypoint();
        }

        #endregion


        #region "Public properties"

        public virtual BaseUnit UnitToFollow { get; set; }

        public virtual PositionOffset PositionOffset { get; set; }

        #endregion



        #region "Public methods"

        public override Waypoint GetActiveWaypoint()
        {
            if (UnitToFollow != null && UnitToFollow.Position != null)
            {
                _waypoint = new Waypoint();
                _waypoint.Position = MapHelper.CalculatePositionFromOffset2(UnitToFollow.Position, PositionOffset);
                return _waypoint;
            }
            return null;
        }

        public override string ToString()
        {
            Waypoint wp = GetActiveWaypoint();
            string temp = "MovementFormationOrder ";
            if(UnitToFollow != null && UnitToFollow.Position != null && PositionOffset != null)
            {
                temp += string.Format("following unit {0} at offset {1}", UnitToFollow.ToShortString(), PositionOffset.ToString());
            }
            if (wp != null)
            { 
                temp += "\nCurrent waypoint: " + wp.ToString();
            }
            return temp;
        }
        #endregion


    }
}
