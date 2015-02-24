using System;
using System.Collections.Generic;

using System.Text;
//using TTG.NavalWar.NWData;
//using TTG.NavalWar.NWData.GamePlay;
//using TTG.NavalWar.NWData.Util;
//using TTG.NavalWar.NWData.OrderSystem;
//using TTG.NavalWar.NWData.Units;
//using TTG.NavalWar.NWComms;
//using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameScenarioGroup : IMarshallable
    {
        #region "Constructors"

        public GameScenarioGroup()
        {
            Units = new List<GameScenarioUnit>();
            InitialWaypointList = new List<WaypointInfo>();
            AlternatePositions = new List<Entities.PositionInfo>();
            SpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
        }

        #endregion


        #region "Public properties"

        public string Name { get; set; }

        public string Tag { get; set; }

        public List<GameScenarioUnit> Units { get; set; }

        public PositionInfo PositionInfo { get; set; }

        public List<PositionInfo> AlternatePositions { get; set; }

        public double PositionVariationRadiusM { get; set; }

        public GameConstants.UnitSpeedType SpeedType { get; set; }

        public string FormationId { get; set; }

        public List<WaypointInfo> InitialWaypointList { get; set; }

        /// <summary>
        /// If non-zero, the unit will patrol from its point of origin, in a sqaure area, with each side having this length in M.
        /// </summary>
        public int PatrolPatternLengthM { get; set; }

        public string ReturnToUnitTag { get; set; }

        #endregion



        #region "Public methods"

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenarioGroup; }
        }

        #endregion
    }
}
