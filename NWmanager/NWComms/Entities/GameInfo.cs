using System;
using System.Collections.Generic;

using System.Text;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameInfo: IMarshallable
    {
        
        #region "Constructors"

        public GameInfo()
        {
            Players = new List<PlayerInfo>();
        }

        #endregion

        #region "Public properties"
        public string Id { get; set; }

        //public string GameName { get; set; }

        //public string MapName { get; set; }

        public string CampaignId { get; set; }

        public string ScenarioId { get; set; }

        public NWDateTime GameStartTime { get; set; }

        public NWDateTime GameCurrentTime { get; set; }

        public double GameWorldTimeSec { get; set; }

        public double GameEngineTimeMs { get; set; }

        public double RealTimeCompressionFactor { get; set; }

        public PositionInfo UpperLeftCorner { get; set; }

        public PositionInfo LowerRightCorner { get; set; }

        public bool IsGamePlayStarted { get; set; }

        public int SkillLevel { get; set; }

        public string GameOwnerPlayerCode { get; set; }

        public int MaxNonComputerPlayers { get; set; }

        public int CountNonComputerPlayers { get; set; }

        public int MinNonComputerPlayersToStartGame { get; set; }

        public List<PlayerInfo> Players { get; set; }

        public GameConstants.GameEconoomicModel EconomicModel { get; set; }

        public int CreditsPerMinute { get; set; }

        #endregion

        #region "Public methods"

        public override string ToString()
        {
            return string.Format(
                "Game [{0}] {1}. Player count: {2}  Human player count: {3}  Start time: {4}  Current Time: {5}",
                Id, this.ScenarioId, Players.Count, CountNonComputerPlayers, 
                GameStartTime.ToString(), GameCurrentTime.ToString());
        }
        #endregion

        #region "Private methods"
        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameInfo; }
        }

        #endregion


    }
}
