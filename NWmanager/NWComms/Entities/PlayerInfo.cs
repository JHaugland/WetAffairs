using System;
using System.Collections.Generic;

using System.Text;
using TTG.NavalWar.NWComms;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class PlayerInfo: IMarshallable
    {

        #region "Constructors"
        public PlayerInfo()
        {
            //AvailableGameScenarios = new List<GameScenario>();
            HighLevelOrders = new List<HighLevelOrder>();
            PlayerObjectives = new List<PlayerObjective>();
        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsComputerPlayer { get; set; }

        public bool IsCompetitivePlayer { get; set; }

        public bool HasPlayerLostGame { get; set; }

        public bool HasPlayerWonGame { get; set; }

        public string ScenarioPlayerId { get; set; }

        public float RequestedTimeCompression { get; set; }

        public bool IsAllUnknownContactsHostile { get; set; }

        public bool IsAutomaticallyEvadingAttacks { get; set; }

        public bool IsAutomaticallyRespondingToActiveSensor { get; set; }

        public bool IsAutomaticallyChangingTimeOnDetection { get; set; }

        public bool IsAutomaticallyChangingTimeOnBattleReport { get; set; }

        public bool IsAutomaticallyChangingTimeOnNoOrder { get; set; }

        public float TimeCompressionOnDetection { get; set; }

        public float TimeCompressionOnBattleReport { get; set; }

        public float TimeCompressionOnNoOrder { get; set; }

        public GameConstants.WeaponOrders DefaultWeaponOrders { get; set; }

        public GameConstants.RulesOfEngagement CurrentRulesOfEngagement { get; set; }

        public bool IsPlayerPermittedToOpenFire { get; set; }

        public User CurrentUser { get; set; }

        //public Campaign CurrentCampaign { get; set; }

        //public string CurrentCampaignId { get; set; }

        public int Credits { get; set; }

        public List<PlayerObjective> PlayerObjectives { get; set; }

        //public List<GameScenario> AvailableGameScenarios { get; set; }

        public List<HighLevelOrder> HighLevelOrders { get; set; }

        public bool IsAdministrator { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return string.Format(
                "Player [{0}] {1}  IsComputer:{2}, IsCompetetive:{3}", 
                Id, Name, IsComputerPlayer, IsCompetitivePlayer);
        }

        #endregion

        #region "Private methods"
        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.PlayerInfo; }
        }

        #endregion


    }
}
