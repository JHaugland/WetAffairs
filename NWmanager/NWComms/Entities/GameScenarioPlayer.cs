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
    public class GameScenarioPlayer : IMarshallable
    {
        #region "Constructors"

        public GameScenarioPlayer()
        {
            //DefeatConditionSets = new List<DefeatConditionSetInfo>();
            Groups = new List<GameScenarioGroup>();
            InitialGameUiControls = new List<GameUiControl>();
            EventTriggers = new List<EventTrigger>();
            HighLevelOrders = new List<HighLevelOrder>();
            AIHints = new List<AIHintInfo>();
            AcquirableUnitClasses = new List<string>();
            IsPlayableAsHuman = true; //default
            IsAutomaticallyEngagingHighValueTargets = true;
            IsAutomaticallyEngagingOpportunityTargets = true;
            IsAutomaticallySettingHighValueDefence = true;
            IsCompetitivePlayer = true;
            InitialRulesOfEngagement = GameConstants.RulesOfEngagement.FireOnClearedTargets;
        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string CountryId { get; set; }

        public string GameDescription { get; set; }

        public string GameObjectives { get; set; }

        public bool IsPlayableAsHuman { get; set; }

        public bool IsCompetitivePlayer { get; set; }

        public GameConstants.RulesOfEngagement InitialRulesOfEngagement { get; set; }

        /// <summary>
        /// For AI players. If true, tries to automatically find targets based on victory conditions and engage them.
        /// </summary>
        public bool IsAutomaticallyEngagingHighValueTargets { get; set; }

        /// <summary>
        /// For AI players. If true, may engage all targets of opportunity when they are detected. 
        /// </summary>
        public bool IsAutomaticallyEngagingOpportunityTargets { get; set; }

        /// <summary>
        /// For AI players. If true, sets up AEW and ASW for high value units automatically.
        /// </summary>
        public bool IsAutomaticallySettingHighValueDefence { get; set; }

        public bool IsAllUnknownContactsHostile { get; set; }

        public int InitialCredits { get; set; }

        public string Tag { get; set; }

        public string AssignedPlayerId { get; set; }

        //public List<DefeatConditionSetInfo> DefeatConditionSets { get; set; }

        public List<GameScenarioGroup> Groups { get; set; }

        public List<GameUiControl> InitialGameUiControls { get; set; }

        public List<HighLevelOrder> HighLevelOrders { get; set; }

        public List<EventTrigger> EventTriggers { get; set; }

        public List<AIHintInfo> AIHints { get; set; }

        public List<string> AcquirableUnitClasses { get; set; }

        #endregion



        #region "Public methods"
        public override string ToString()
        {
            return Id + " ( " + CountryId + ")";
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenarioPlayer; }
        }

        #endregion

       
    }
}
