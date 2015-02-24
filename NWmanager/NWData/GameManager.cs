using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData
{
    public class GameManager
    {
        #region "Private fields and constants"

        public const int DEFAULT_TCPIP_PORT = 2055;

        private static GameManager _GameManager = null;
        private GameData _GameData = new GameData();
        private Random _random = new Random();

        private Game _Game = null;

        private Logger _log = null;

        private static long _UniqueValue = 1000;

        #endregion

        #region "Constructors"

        private GameManager()
        {
            _log = new Logger();
            
        }

        #endregion

        #region "Public properties"

        public static GameManager Instance 
        {
            get
            {
                if (_GameManager == null)
                {
                    _GameManager = new GameManager();
                }
                return _GameManager;
            }
        }

        public static long GetUniqueValue()
        {
            _UniqueValue++;
            return _UniqueValue;
        }

        public static string GetUniqueCode()
        {
            long value = GetUniqueValue();
            return GenerateCode(value);
        }

        public static long GetNumericId(string id)
        {
            char[] chars = id.ToCharArray();
            if (chars.Length == 0)
            {
                return 0;
            }
            int counter = 0;

            int sum = 0; ;
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                sum += (Convert.ToInt32(chars[i]) - 64) * (int)Math.Pow(26, counter);
                counter++;
            }
            return sum;
        }

        public static string GenerateCode(long number)
        {
            //generate unique string
            //Upper chars start at 65 ASCII
            return number < 0 ? "" : GenerateCode(number / 26 - 1) + Convert.ToChar(65 + (number % 26));
        }

        public static string GetUniqueCodeString(long number)
        {
            // throws an ArgumentOutOfRangeException if number is 0 or uner
            if (number <= 0)
                throw new ArgumentOutOfRangeException();

            //offset number so that we can start from right position
            number--;

            //generate and return the unique string
            return GenerateCode(number);
        }

        public static double GetEffectiveIdentifyingRange(GameConstants.DomainType activeUnitDomain, 
            GameConstants.DomainType targetUnitDomain)
        {
            if (activeUnitDomain != GameConstants.DomainType.Subsea)
            {
                if (targetUnitDomain != GameConstants.DomainType.Subsea)
                {
                    double RangeOfSightM = MapHelper.CalculateMaxLineOfSightM(100, 0);
                    return RangeOfSightM;
                }
                else
                {
                    return GameConstants.IDENTIFY_TARGET_SURFACE_SUB_M;
                }
            }
            else
            {
                if (targetUnitDomain != GameConstants.DomainType.Subsea)
                {
                    return GameConstants.IDENTIFY_TARGET_SURFACE_SUB_M;
                }
                else
                {
                    return GameConstants.IDENTIFY_TARGET_SUB_SUB_M;
                }
            }
        }
        public Logger Log
        {
            get 
            {
                return _log;
            }
        }

        public Game Game
        {
            get
            {
                return _Game;
            }
        }

        public GameData GameData
        {
            get
            {
                return _GameData;
            }
        }

        #endregion

        #region "Public static methods"

        public static int GetDefaultHitpoints(GameConstants.UnitType unitType)
        {
            switch (unitType)
            {
                case GameConstants.UnitType.SurfaceShip:
                    return 1000;
                case GameConstants.UnitType.FixedwingAircraft:
                    return 50;
                case GameConstants.UnitType.Helicopter:
                    return 50;
                case GameConstants.UnitType.Submarine:
                    return 200;
                case GameConstants.UnitType.Missile:
                    return 10;
                case GameConstants.UnitType.Torpedo:
                    return 50;
                case GameConstants.UnitType.Mine:
                    return 10;
                case GameConstants.UnitType.Decoy:
                    return 10;
                case GameConstants.UnitType.Sonobuoy:
                    return 10;
                case GameConstants.UnitType.BallisticProjectile:
                    return 10;
                case GameConstants.UnitType.Bomb:
                    return 10;
                case GameConstants.UnitType.LandInstallation:
                    return 10000;
                default:
                    return 1000;
            }
        }


        #endregion

        #region "Public methods"

        public Game CreateGame(Player owner, string gameName)
        {
            Log.LogDebug("**************************************************************************");
            Log.LogDebug(string.Format("GameManager->CreateGame. GameName: {0}   Player: {1}", gameName, owner.Name));
            Log.LogDebug("**************************************************************************");
            _Game = new Game(owner, gameName);
            _Game.UpperLeftCorner = new Coordinate(70, -10); //default area for NW:AC
            _Game.LowerRightCorner = new Coordinate(40, 10);

            return _Game;

        }

        public Game CreateGame(string gameName)
        {
            Log.LogDebug("**************************************************************************");
            Log.LogDebug(string.Format("GameManager->CreateGame. GameName: {0}", gameName));
            Log.LogDebug("**************************************************************************");
            _Game = new Game();
            _Game.GameName = gameName;
            _Game.UpperLeftCorner = new Coordinate(70, -10); //default area for NW:AC
            _Game.LowerRightCorner = new Coordinate(40, 10);

            return _Game;

        }

        public void SetGame( Game game )
        {
            if ( _Game != game )
            {
                // Terminate old game first (if any)
                TerminateGame();

                _Game = game;

                if ( _Game != null )
                {
                    _Game.UpperLeftCorner = new Coordinate( 70, -10 ); //default area for NW:AC
                    _Game.LowerRightCorner = new Coordinate( 40, 10 );
                }
            }
        }

        public double GetProbabilityTimePercent(double timeElapsedSec, double probabilityPerMinutePercent)
        {
            double prob = probabilityPerMinutePercent * (timeElapsedSec / 60);
            return prob;
        }

        public bool ThrowDiceTime(double timeElapsedSec, double probabilityPerMinutePercent)
        {
            double prob = GetProbabilityTimePercent(timeElapsedSec, probabilityPerMinutePercent);
            return ThrowDice(prob);
        }

        public bool ThrowDice(double probabilityPercent)
        {
            bool result = false;
            int rnd = _random.Next(100);
            result = (rnd < Convert.ToInt32(probabilityPercent));
            if (probabilityPercent <= 0)
            {
                result = false;
            }
            if (probabilityPercent >= 100)
            {
                result = true;
            }

            //_log.LogDebug(string.Format("GameManager->ThrowDice returns {0} for P={1}%",
            //    result, probabilityPercent));
            return result;
        }

        public int GetDamageHitpoints(int hitpoints)
        {
            double hitfix = ((double)hitpoints) / 2;
            double hitvar = (_random.NextDouble() * hitfix) * 2.0;
            int damagehp = (int)(hitfix + hitvar);
            _log.LogDebug("GameManager->GetDamageHitpoints returns " + damagehp);
            return damagehp;
        }

        /// <summary>
        /// Returns a random number between 0 and MaxValue.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int GetRandomNumber(int maxValue)
        {
            if (maxValue < 0)
            {
                maxValue = 0;
            }
            return _random.Next(maxValue);
        }

        public int GetRandomNumber(int minValue, int maxValue)
        {
            if ( maxValue < minValue )
            {
                maxValue = minValue;
            }
            return _random.Next(minValue, maxValue);
        }

        public double GetRandomNumber(double maxValue)
        {
            if (maxValue < 0)
            {
                maxValue = 0.0;
            }
            return _random.NextDouble() * maxValue;
        }

        public Country GetCountryById(string countryId)
        {
            return GameData.GetCountryById(countryId);
        }

        public UnitClass GetUnitClassById(string unitClassId)
        {
            return GameData.GetUnitClassById(unitClassId);
        }

        public WeaponClass GetWeaponClassById(string weaponClassId)
        {
            return GameData.GetWeaponClassById(weaponClassId);
        }

        public SensorClass GetSensorClassById(string sensorClassId)
        {
            return GameData.GetSensorClassById(sensorClassId);
        }

        public void ResetAll()
        {
            if (_Game != null)
            {
                _UniqueValue = 1000;

                foreach (var p in _Game.Players)
                {
                    p.Units.Clear();
                    p.DetectedUnits.Clear();
                }
                _Game.Players.Clear();
            }
            //_Game = new Game();
            _Game = null;

            _GameData = new GameData();
        }

        public void TerminateGame() //with prejudice
        {
            if (_Game != null)
            { 
                _Game.Terminate();
            }
            _Game = null;
        }

        #region Messages

        public Message CreateNewMessage(Player toPlayer, string message, GameConstants.Priority priority)
        {
            Message msg = new Message();
            msg.Priority = priority;
            msg.ToPlayer = toPlayer;
            msg.MessageBody = message;
            msg.GameTimeGeneratedSec = Game.GameWorldTimeSec;
            //msg.Subject = string.Empty;
            toPlayer.MessageToPlayer.Add(msg);
            Log.LogDebug("CreateNewMessage: " + msg.ToString());
            return msg;
        }

        public Message CreateNewMessage(Player toPlayer, Player fromPlayer, string message, 
             GameConstants.Priority priority)
        {
            Message msg = new Message();
            msg.Priority = priority;
            msg.ToPlayer = toPlayer;
            msg.FromPlayer = fromPlayer;
            msg.MessageBody = message;
            msg.GameTimeGeneratedSec = Game.GameWorldTimeSec;
            //msg.Subject = string.Empty;
            toPlayer.MessageToPlayer.Add(msg);
            if (fromPlayer != null)
            {
                fromPlayer.MessageFromPlayer.Add(msg);
            }
            Log.LogDebug("CreateNewMessage: " + msg.ToString());
            return msg;
        }

        #endregion

        ///// <summary>
        ///// Returns the radar cross section of the enum RadarCrossSectionSize as a percentage of Large. This
        ///// method contains hardcoded values.
        ///// </summary>
        ///// <param name="radarCrossSectionSize"></param>
        ///// <returns></returns>
        //public double CalculateRadarCrossSectionPercentageOfLarge(GameConstants.RadarCrossSectionSize radarCrossSectionSize)
        //{
        //    switch (radarCrossSectionSize)
        //    {
        //        case GameConstants.RadarCrossSectionSize.VeryStealthy:
        //            return 5;
        //        case GameConstants.RadarCrossSectionSize.Stealthy:
        //            return 10;
        //        case GameConstants.RadarCrossSectionSize.Reduced:
        //            return 35;
        //        case GameConstants.RadarCrossSectionSize.Normal:
        //            return 62;
        //        case GameConstants.RadarCrossSectionSize.Large:
        //            return 100;
        //        default:
        //            return 100;
        //    }
        //}

        /// <summary>
        /// Returns the expected radar degradation in percent on a ship-based radar (or other similar sensor) 
        /// based on effective SeaState. This models the effect of the ship rolling and sea scutter. Contains
        /// hardcoded values.
        /// </summary>
        /// <param name="effectiveSeaState"></param>
        /// <returns></returns>
        public double GetRadarDegradationFromSeaStatePercent(int effectiveSeaState)
        {
            if (effectiveSeaState < 5)
            {
                return 0;
            }
            return (effectiveSeaState - 4) * 10;

        }

        /// <summary>
        /// Returns the expected radar degradation in percent of any radar below the cloud cover (if any)
        /// based on the current precipitation type and level.
        /// </summary>
        /// <param name="weatherSystem"></param>
        /// <returns></returns>
        public double GetRadarDegradationFromWeatherPercent(TTG.NavalWar.NWData.Ai.WeatherSystem weatherSystem)
        {
            double degradation = 0;
            if (weatherSystem == null)
            {
                return degradation;
            }
            switch (weatherSystem.Precipitation)
            {
                case GameConstants.PrecipitationLevel.None:
                    break;
                case GameConstants.PrecipitationLevel.Drizzle:
                    degradation = 10;
                    break;
                case GameConstants.PrecipitationLevel.Light:
                    degradation = 25;
                    break;
                case GameConstants.PrecipitationLevel.Intermediate:
                    degradation = 50;
                    break;
                case GameConstants.PrecipitationLevel.Heavy:
                    degradation = 75;
                    break;
                default:
                    return 0;
            }
            if (weatherSystem.PrecipitationType == GameConstants.PrecipitationType.Snow)
            {
                degradation += 10;
            }
            return degradation;
        }

        public double GetIRDegradationFromWeatherPercent(TTG.NavalWar.NWData.Ai.WeatherSystem weatherSystem)
        {
            double degradation = 0;
            if (weatherSystem == null)
            {
                return degradation;
            }
            switch (weatherSystem.Precipitation)
            {
                case GameConstants.PrecipitationLevel.None:
                    break;
                case GameConstants.PrecipitationLevel.Drizzle:
                    degradation = 30;
                    break;
                case GameConstants.PrecipitationLevel.Light:
                    degradation = 50;
                    break;
                case GameConstants.PrecipitationLevel.Intermediate:
                    degradation = 75;
                    break;
                case GameConstants.PrecipitationLevel.Heavy:
                    degradation = 99;
                    break;
                default:
                    return 0;
            }
            return degradation;
        }


        #endregion


        public string GetUnitSubTypeName(GameConstants.UnitSubType unitSubType)
        {
            switch (unitSubType)
            {
                case GameConstants.UnitSubType.AircraftCarrier:
                    return "Aircraft carrier";
                case GameConstants.UnitSubType.SurfaceShipCombattant:
                    return "Surface combattant";
                case GameConstants.UnitSubType.SurfaceShipSupport:
                    return "Surface support";
                case GameConstants.UnitSubType.LittoralPatrol:
                    return "Littoral patrol";
                case GameConstants.UnitSubType.FixedWingFighter:
                    return "Fighter";
                case GameConstants.UnitSubType.FixedWingStrike:
                    return "Strike";
                case GameConstants.UnitSubType.FixedWingSupport:
                    return "Support air";
                case GameConstants.UnitSubType.HelicopterAsw:
                    return "ASW Helo";
                case GameConstants.UnitSubType.HelicopterStrike:
                    return "Strike Helo";
                case GameConstants.UnitSubType.HelicopterOther:
                    return "Support Helo";
                case GameConstants.UnitSubType.Submarine:
                    return "Sub";
                case GameConstants.UnitSubType.Missile:
                    return "Missile";
                case GameConstants.UnitSubType.Torpedo:
                    return "Torpedo";
                case GameConstants.UnitSubType.LandAirport:
                    return "Airport";
                case GameConstants.UnitSubType.LandSeaport:
                    return "Seaport";
                case GameConstants.UnitSubType.LandOther:
                    return "Land installation";
                case GameConstants.UnitSubType.Other:
                    return "Other";
                default:
                    return "Undefined";
            }
        }

        public string GetJammingNameFromSpecialOrder(GameConstants.SpecialOrders specialOrders)
        {
            switch (specialOrders)
            {
                case GameConstants.SpecialOrders.None:
                    return "None";
                case GameConstants.SpecialOrders.DropSonobuoy:
                    return "Drop Sonobuoy";
                case GameConstants.SpecialOrders.DropMine:
                    return "Drop Mine";
                case GameConstants.SpecialOrders.JammerRadarDegradation:
                    return "Radar Degradation Jammer";
                case GameConstants.SpecialOrders.JammerCommunicationDegradation:
                    return "Communication Degradation Jammer";
                case GameConstants.SpecialOrders.JammerRadarDistortion:
                    return "Radar Distortion Jammer";
                default:
                    return "Undefined Special Order";
            }
        }

        public void SendToClient(int tcpPlayerIndex, IMarshallable marshallable)
        {
            if (_Game != null)
            {
                _Game.SendToClient(tcpPlayerIndex, marshallable);
            }
        }
    }
}
