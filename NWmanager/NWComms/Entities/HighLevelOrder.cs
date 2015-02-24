using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class HighLevelOrder : IMarshallable
    {
        #region "Constructors"

        public HighLevelOrder()
        {
            CountDesired = 1;
            CountMinimum = 1;
            RecurringCount = 1;
            RolesList = new List<GameConstants.Role>();
        }
        public HighLevelOrder(GameConstants.HighLevelOrderType highLeverOrderType) : this()
        {
            HighLeverOrderType = highLeverOrderType;
        }

        public HighLevelOrder(GameConstants.HighLevelOrderType highLeverOrderType, 
            double firstTriggerInSec, double triggerEverySec, int recurringCount)
            : this()
        {
            GameUiControls = new List<GameUiControl>();
            HighLeverOrderType = highLeverOrderType;
            TriggerEverySec = triggerEverySec;
            FirstTriggerInSec = firstTriggerInSec;
            RecurringCount = recurringCount;
        }


        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string Tag { get; set; }

        public double TriggerEverySec { get; set; }

        public double FirstTriggerInSec { get; set; }

        public NWDateTime TriggerNextTime { get; set; }

        public bool IsForComputerPlayerOnly { get; set; }

        public GameConstants.SkillLevelInclusion SkillLevel { get; set; }

        public int RecurringCount { get; set; }

        public GameConstants.RulesOfEngagement NewRulesOfEngagement { get; set; }

        public GameConstants.HighLevelOrderType HighLeverOrderType { get; set; }

        /// <summary>
        /// When order is executed, this GameUiControl is sent to the (human) player.
        /// </summary>
        public List<GameUiControl> GameUiControls { get; set; }

        public RegionInfo Region { get; set; }

        /// <summary>
        /// For support/protection/escort, own unit
        /// </summary>
        public string OwnUnitId { get; set; }

        /// <summary>
        /// For attack (etc), enemy target unit
        /// </summary>
        public string TargetUnitId { get; set; }

        /// <summary>
        /// For unit escort, etc, the direction
        /// </summary>
        public GameConstants.DirectionCardinalPoints Direction { get; set; }

        public PositionInfo PositionCenter { get; set; }

        /// <summary>
        /// Radius of area or distance to target
        /// </summary>
        public double DistanceM { get; set; }

        public int CountDesired { get; set; }

        public int CountMinimum { get; set; }

        public List<GameConstants.Role> RolesList { get; set; }

        public string UnitClassId { get; set; }

        public bool SetActiveRadar { get; set; }

        public bool SetActiveSonar { get; set; }

        #endregion



        #region "Public methods"

        public HighLevelOrder Clone()
        {
            var serhelper = new TTG.NavalWar.NWComms.CommsSerializationHelper<HighLevelOrder>();
            byte[] bytes = serhelper.SerializeToBytes(this);

            HighLevelOrder hlo = serhelper.DeserializeFromBytes(bytes);
            return hlo;
        }

        public override string ToString()
        {
            string temp = "HighLevelOrder: " + HighLeverOrderType + " " + Tag;
            if (PositionCenter != null)
            {
                temp += "  Pos: " + PositionCenter.ToString();
            }
            if (Region != null)
            { 
                temp += "  Region: " + Region.Coordinates.Count + " vertices";
            }
            return temp;
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.HighLevelOrder; }
        }

        #endregion
    }
}
