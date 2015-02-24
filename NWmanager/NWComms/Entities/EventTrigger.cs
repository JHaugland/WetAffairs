using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(PlayerObjective))]
    public class EventTrigger: IMarshallable
    {
        #region "Constructors"

        public EventTrigger()
        {
            HighLevelOrders = new List<HighLevelOrder>();
            HighLevelOrdersEnemy = new List<HighLevelOrder>();
            GameUiControls = new List<GameUiControl>();
            InnerTriggers = new List<EventTrigger>();
        }

        public EventTrigger(GameConstants.EventTriggerType eventTriggerType) : this()
        {
            EventTriggerType = eventTriggerType;
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string DescriptionPlayer { get; set; }

        /// <summary>
        /// For triggers that are victoryconditions, a description reported to the enemy about how to meet this criteria
        /// </summary>
        public string DescriptionEnemy { get; set; }
        
        public GameConstants.EventTriggerType EventTriggerType { get; set; }

        public bool IsForComputerPlayerOnly { get; set; }

        public string Tag { get; set; }

        public string UnitClassId { get; set; }

        public RegionInfo Region { get; set; }

        public PositionInfo Position { get; set; }

        public double AreaRadiusM { get; set; }

        public int CountCurrent { get; set; }

        public int CountDesired { get; set; }

        public GameConstants.RulesOfEngagement RulesOfEngagement { get; set; }

        public GameConstants.Role Role { get; set; }

        public GameConstants.UnitOrderType UnitOrderType { get; set; }

        public long TimeElapsedSec { get; set; }

        public List<GameUiControl> GameUiControls { get; set; }

        public List<EventTrigger> InnerTriggers { get; set; }

        public List<HighLevelOrder> HighLevelOrders { get; set; }

        public List<HighLevelOrder> HighLevelOrdersEnemy { get; set; }

        /// <summary>
        /// If true, this event should be listed as a player objective until met.
        /// </summary>
        public virtual bool IsPlayerObjective
        {
            get { return false; }
        }

        //public virtual bool IsVictoryCondition { get { return false; } }

        public bool HasBeenTriggered { get; set; }

        #endregion



        #region "Public methods"

        public virtual EventTrigger Clone()
        {
            return (EventTrigger)MemberwiseClone();

        }
        public override string ToString()
        {
            string temp = "EventTrigger " + this.EventTriggerType + " " + this.DescriptionPlayer;
            if (!string.IsNullOrEmpty(Tag))
            { 
                temp += "  Tag:" + Tag;
            }
            if (!string.IsNullOrEmpty(UnitClassId))
            {
                temp += "  UnitClassId:" + UnitClassId;
            }
            if (Role != GameConstants.Role.NoOrAnyRole)
            {
                temp += "  Role:" + Role;
            }
            if (this.EventTriggerType == GameConstants.EventTriggerType.OrderReceivedFromPlayer)
            {
                temp += "  OrderType: " + this.UnitOrderType;
            }
            if (this.TimeElapsedSec > 0)
            {
                temp += "  Time " + this.TimeElapsedSec + "sec ";
            }
            if (this.InnerTriggers != null && this.InnerTriggers.Count > 0)
            {
                temp += " " + InnerTriggers.Count + "Inner Triggers";
            }
            return temp;
        }
        #endregion



        #region IMarshallable Members

        public virtual CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.EventTrigger; }
        }

        #endregion


    }
}
