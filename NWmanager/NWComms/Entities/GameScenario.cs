using System;
using System.Collections.Generic;
using System.Text;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GameScenario : IMarshallable, IGameDataObject
    {
        #region "Constructors"

        public GameScenario() 
        {
            Alliences = new List<GameScenarioAlliance>();
            IsMultiplayer = false;
            IsSkirmishVsAi = true;
            IsTutorial = false;
        }

        public GameScenario(string gameName, int maxNonComputerPlayers)
            : this()
        {
            GameName = gameName;
            MaxNonComputerPlayers = maxNonComputerPlayers;
        }

        #endregion


        #region "Public properties"

        public List<GameScenarioAlliance> Alliences { get; set; }

        //public GameConstants.GameScenarioType GameScenarioType { get; set; }

        public string Id { get; set; }

        public string GameName { get; set; }

        public string GameDescription { get; set; }

        //public int SkillLevel { get; set; }

        //public string GameOwnerPlayerCode { get; set; }

        public GameConstants.GameEconoomicModel EconomicModel { get; set; }

        public int CreditsPerMinute { get; set; }

        public int InitialCreditsPerPlayer { get; set; }

        public int MaxNonComputerPlayers { get; set; }
        
        //public bool IsNetworkEnabled { get; set; }

        public bool IsTutorial { get; set; } //default false

        public bool IsSkirmishVsAi { get; set; } //default true

        public bool IsMultiplayer { get; set; } //default false

        public double InitialRealTimeCompressionFactor { get; set; }

        public PositionInfo UpperLeftCorner { get; set; }

        public PositionInfo LowerRightCorner { get; set; }

        public GameConstants.WeatherSystemSeasonTypes Season { get; set; }

        public GameConstants.WeatherSystemTypes WeatherType { get; set; }

        public NWDateTime StartDateTime { get; set; }

        public string PreviewImage { get; set; }

        #endregion



        #region "Public methods"
        public List<GameScenarioPlayer> GetPlayers()
        {
            List<GameScenarioPlayer> players = new List<GameScenarioPlayer>();
            foreach ( GameScenarioAlliance al in this.Alliences )
            {
                players.AddRange( al.ScenarioPlayers );
            }
            return players;
        }

        public List<GameScenarioPlayer> GetPlayablePlayers()
        {
            List<GameScenarioPlayer> players = new List<GameScenarioPlayer>();
            foreach ( GameScenarioAlliance al in this.Alliences )
            {
                foreach ( GameScenarioPlayer pl in al.ScenarioPlayers )
                {
                    if ( pl.IsCompetitivePlayer )
                    {
                        players.Add( pl );
                    }
                }
            }
            return players;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(GameName))
            {
                return Id + " " + GameName;
            }
            else
            {
                return Id + " Unnamed";
            }

        }
        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GameScenario; }
        }

        #endregion



        
    }
}
