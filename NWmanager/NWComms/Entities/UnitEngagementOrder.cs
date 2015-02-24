using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class UnitEngagementOrder :UnitOrder, IMarshallable
    {
        #region "Constructors"

        public UnitEngagementOrder()
        {
            EngagementStrength = GameConstants.EngagementStrength.DefaultAttack;
            EngagementType = GameConstants.EngagementOrderType.EngageNotClose;
            UnitOrderType = GameConstants.UnitOrderType.EngagementOrder;
        }

        #endregion


        #region "Public properties"

        public GameConstants.EngagementOrderType EngagementType { get; set; }

        public GameConstants.EngagementStrength EngagementStrength { get; set; }

        //public string Id { get; set; }

        public string TargetId { get; set; }

        //public PositionInfo Position { get; set; }

        public string WeaponClassID { get; set; }

        public int RoundCount { get; set; }

        /// <summary>
        /// If true, will select any and all units in group to engage target, otherwise
        /// only the receipient unit.
        /// </summary>
        public bool IsGroupAttack { get; set; }

        /// <summary>
        /// If true, the TargetId is interpreted as a DetectedGroup, otherwise a DetectedUnit
        /// </summary>
        public bool IsTargetAGroup { get; set; } 

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public override CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.UnitEngagementOrder; }
        }

        #endregion
    }
}
