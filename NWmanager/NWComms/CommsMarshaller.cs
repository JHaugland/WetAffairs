using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms
{
    public class CommsMarshaller
    {
        #region "Public constants and enums, private fields"
        
        // PROTOCOL:
        // Bytes 0 - 1 : Token identifying type of object
        // Bytes 2 - 5 : 32 bit unsigned giving the size of the object excluding 6 byte header
        // Bytes 4 - n : Binary serialized object

        public enum ObjectTokens
        {
            NoObject = 0,
            Enq,
            Ack,
            ClientInfoRequest,
            GameControlRequest,
            GameStateInfo,
            MessageInfo,
            MessageString,
            BattleDamageReport,
            Formation,
            FormationPosition,
            GameInfo,
            PlayerInfo,
            PositionInfo,
            BaseUnitInfo,
            GroupInfo,
            DetectedGroupInfo,
            DetectedUnitInfo,
            DetecedUnitSensorInfo,
            OrderInfo,
            UnitOrder,
            UnitMovementOrder,
            UnitEngagementOrder,
            UnitClass,
            WeaponClass,
            SensorClass,
            GameScenario,
            GameScenarioAlliance,
            GameScenarioPlayer,
            GameScenarioGroup,
            GameScenarioUnit,
            GameScenarioCarriedUnit,
            WeatherSystemInfo,
            NWDateTime,
            WaypointInfo,
            HighLevelOrder,
            GameUiControl,
            EventTrigger,
            RegionInfo,
            User,
            Campaign,
            AiHintInfo,
            PlayerObjective,
            SteamClientInitiateConnection,
            SteamGameServerInfo,
            SteamClientBeginAuthentication,
            SteamServerPassAuthentication,
            ClientLeavingServer,
            ServerExiting,
        }

        public enum GameControlRequestType
        { 
            StartGame,
            TerminateGame,
            LoadGameScene,       // Send to inform other players that it's time to load the game scene (send before starting game)
            GameSceneLoaded,     // Send to inform server that we've loaded the game scene
            SetTimeCompressionRatio,
            PlayerSelectScenario,
            PlayerSelectScenarioPlayer,
            PlayerSelectCampaign,
            PlayerSelectUser,
            PlayerSetName,
            DesignateContactFriendOrFoe,
            SetUnknownContactFoFDesignation,
            SetAutomaticUnitEvasion,
            SetAutomaticActiveSensorResponse,
            SetWeaponOrdersUnit,
            SetWeaponOrdersGlobal,
            SetAutomaticTimeCompressionOnDetection,
            SetAutomaticTimeCompressionOnBattleReport,
            SetAutomaticTimeCompressionOnNoOrder,
            RemoveHighLevelOrder,
            RenameUnit,
            RenameGroup,
            SendMessageToPlayers,
            GetAllUnitsInArea,
            TriggerSignal,
            CheatCode,
        }



        public const int DEFAULT_BUFFER_SIZE = 1024 * 1024; // 1MB. Should NOT need this big buffer, but we're sending some HUGE data sets sometimes.
        public const int TOKEN_BYTE_COUNT = sizeof(UInt16);
        public const int LENGTH_BYTE_COUNT = sizeof(UInt32);

        private byte[] _ReceiveBuffer;
        private int _BufferIndex = 0;

        #endregion

        #region "Constructors"

        public CommsMarshaller()
        {
            _ReceiveBuffer = new byte[DEFAULT_BUFFER_SIZE];
        }

        #endregion


        #region "Public properties"

        public byte[] Buffer
        {
            get
            {
                return _ReceiveBuffer;
            }
        }

        public int BufferIndex
        {
            get
            {
                return _BufferIndex;
            }
        }

        #endregion

        #region "Public methods"

        public void AddToReceiveBufferEnd(byte[] bytes, int length)
        {
            try
            {
                if (_BufferIndex + length > _ReceiveBuffer.Length)
                {
                    var log = new Logger();
                    log.LogError("CommsMarshaller: AddToReceiveBufferEnd -> Receive buffer is not big enough. Resizing.");

                    var newBuffer = new byte[ _ReceiveBuffer.Length + Math.Max(length, DEFAULT_BUFFER_SIZE) ];
                    Array.Copy(_ReceiveBuffer, newBuffer, _ReceiveBuffer.Length);
                    _ReceiveBuffer = newBuffer;
                }
                Array.Copy(bytes, 0, _ReceiveBuffer, _BufferIndex, length);
                //bytes.CopyTo(_Buffer, _BufferIndex);
                _BufferIndex += length;

            }
            catch (Exception ex)
            {
                var log = new Logger();
                log.LogError(
                    string.Format("CommsMarshaller: AddToReceiveBufferEnd failed. length={0}  _BufferIndex={1}  _ReceiveBuffer.Length={2}\nError:{3}",
                    length, _BufferIndex, _ReceiveBuffer.Length, ex.ToString()));
                throw;
            }
        }

        public bool IsObjectInReceiveBuffer()
        {
            if (_BufferIndex < (TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT))
            {
                return false;
            }
            //int Token = GetTokenFromBuffer();
            int Length = GetLengthFromReceiveBuffer();
            if (Length == 0)
            {
                _BufferIndex = 0;
                return false;
            }
            if(_BufferIndex < (Length + TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT))
            {
                return false;
            }
            return true;
        }

        public int GetLengthFromReceiveBuffer()
        {
            byte[] LengthHeader = new byte[LENGTH_BYTE_COUNT];
            Array.Copy(_ReceiveBuffer, TOKEN_BYTE_COUNT, LengthHeader, 0, LENGTH_BYTE_COUNT);
            int length = (int)BitConverter.ToUInt32(LengthHeader, 0);
            return length;
            //return _Buffer[2] + (_Buffer[3] * 256);
        }

        public ObjectTokens GetTokenFromReceiveBuffer()
        {
            //int TokenValue = _Buffer[0] + (_Buffer[1] * 256);
            byte[] TokenHeader = new byte[TOKEN_BYTE_COUNT];
            Array.Copy(_ReceiveBuffer, 0, TokenHeader, 0, TOKEN_BYTE_COUNT);
            return (ObjectTokens)BitConverter.ToUInt16(TokenHeader, 0);
            
        }

        public byte[] PopObjectBytesFromReceiveBuffer(int length)
        {
            byte[] SerializedObject = ReadObjectBytesFromReceiveBuffer(length);
            RemoveBytesFromReceiveBuffer(length + TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT);

            return SerializedObject;
        }

        public byte[] ReadObjectBytesFromReceiveBuffer(int length)
        {
            if (length < 0)
            {
                throw new IndexOutOfRangeException("ReadObjectBytesFromReceiveBuffer: Length is negative! Buffer is " +
                    _ReceiveBuffer.Length + ", tried to read header and " + length + " bytes.");
            }
            byte[] SerializedObject = new byte[length];
            if (_ReceiveBuffer.Length < length + TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT - 1)
            {
                throw new IndexOutOfRangeException("ReadObjectBytesFromReceiveBuffer: Buffer is " +
                    _ReceiveBuffer.Length + ", tried to read header and " + length + " bytes.");
            }
            Array.Copy(_ReceiveBuffer, TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT, SerializedObject, 0, length);
            return SerializedObject;
        }

        public void RemoveBytesFromReceiveBuffer(int untilIndex)
        {
            int BufferLength = _BufferIndex;
            int RemainingLength = BufferLength - untilIndex;
            byte[] TempBuffer = new byte[RemainingLength];
            Array.Copy(_ReceiveBuffer, untilIndex, TempBuffer, 0, RemainingLength);
            Array.Copy(TempBuffer, _ReceiveBuffer, RemainingLength);
            _BufferIndex = RemainingLength;
        }

        public IMarshallable DeSerializeNextObjectInReceiveBuffer()
        {
            if (!IsObjectInReceiveBuffer())
            {
                return null;
            }
            ObjectTokens token = GetTokenFromReceiveBuffer();
            int Length = GetLengthFromReceiveBuffer();
            byte[] bytes = PopObjectBytesFromReceiveBuffer(Length);
            switch (token) //TODO: IMPLEMENT ALL!
            {
                case ObjectTokens.NoObject:
                    break;
                case ObjectTokens.Enq:
                    return new CommsSerializationHelper<EnqObject>().DeserializeFromBytes(bytes);
                case ObjectTokens.Ack:
                    return new CommsSerializationHelper<AckObject>().DeserializeFromBytes(bytes);
                case ObjectTokens.MessageString:
                    return new CommsSerializationHelper<MessageString>().DeserializeFromBytes(bytes);                    
                case ObjectTokens.PositionInfo:
                    return new CommsSerializationHelper<PositionInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.BaseUnitInfo:
                    return new CommsSerializationHelper<BaseUnitInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.GroupInfo:
                    return new CommsSerializationHelper<GroupInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.DetectedGroupInfo:
                    return new CommsSerializationHelper<DetectedGroupInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.DetectedUnitInfo:
                    return new CommsSerializationHelper<DetectedUnitInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.DetecedUnitSensorInfo:
                    return new CommsSerializationHelper<DetecedUnitSensorInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.MessageInfo:
                    return new CommsSerializationHelper<MessageInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameInfo:
                    return new CommsSerializationHelper<GameInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameStateInfo:
                    return new CommsSerializationHelper<GameStateInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.BattleDamageReport:
                    return new CommsSerializationHelper<BattleDamageReport>().DeserializeFromBytes(bytes);
                case ObjectTokens.FormationPosition:
                    return new CommsSerializationHelper<FormationPosition>().DeserializeFromBytes(bytes);
                case ObjectTokens.Formation:
                    return new CommsSerializationHelper<Formation>().DeserializeFromBytes(bytes);
                case ObjectTokens.ClientInfoRequest:
                    return new CommsSerializationHelper<ClientInfoRequest>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameControlRequest:
                    return new CommsSerializationHelper<GameControlRequest>().DeserializeFromBytes(bytes);
                case ObjectTokens.UnitOrder:
                    return new CommsSerializationHelper<UnitOrder>().DeserializeFromBytes(bytes);
                case ObjectTokens.UnitMovementOrder:
                    return new CommsSerializationHelper<UnitMovementOrder>().DeserializeFromBytes(bytes);
                case ObjectTokens.UnitEngagementOrder:
                    return new CommsSerializationHelper<UnitEngagementOrder>().DeserializeFromBytes(bytes);
                case ObjectTokens.OrderInfo:
                    //TODO: Add an orderinfo class to begin with
                    return null;
                case ObjectTokens.PlayerInfo:
                    return new CommsSerializationHelper<PlayerInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.SensorClass:
                    return new CommsSerializationHelper<SensorClass>().DeserializeFromBytes(bytes);
                case ObjectTokens.UnitClass:
                    return new CommsSerializationHelper<UnitClass>().DeserializeFromBytes(bytes);
                case ObjectTokens.WeaponClass:
                    return new CommsSerializationHelper<WeaponClass>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenario:
                    return new CommsSerializationHelper<GameScenario>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenarioAlliance:
                    return new CommsSerializationHelper<GameScenarioAlliance>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenarioPlayer:
                    return new CommsSerializationHelper<GameScenarioPlayer>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenarioGroup:
                    return new CommsSerializationHelper<GameScenarioGroup>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenarioUnit:
                    return new CommsSerializationHelper<GameScenarioUnit>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameScenarioCarriedUnit:
                    return new CommsSerializationHelper<GameScenarioCarriedUnit>().DeserializeFromBytes(bytes);
                case ObjectTokens.WeatherSystemInfo:
                    return new CommsSerializationHelper<WeatherSystemInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.NWDateTime:
                    return new CommsSerializationHelper<NWDateTime>().DeserializeFromBytes(bytes);
                case ObjectTokens.WaypointInfo:
                    return new CommsSerializationHelper<WaypointInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.HighLevelOrder:
                    return new CommsSerializationHelper<HighLevelOrder>().DeserializeFromBytes(bytes);
                case ObjectTokens.GameUiControl:
                    return new CommsSerializationHelper<GameUiControl>().DeserializeFromBytes(bytes);
                case ObjectTokens.EventTrigger:
                    return new CommsSerializationHelper<EventTrigger>().DeserializeFromBytes(bytes);
                case ObjectTokens.RegionInfo:
                    return new CommsSerializationHelper<RegionInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.User:
                    return new CommsSerializationHelper<User>().DeserializeFromBytes(bytes);
                case ObjectTokens.Campaign:
                    return new CommsSerializationHelper<Campaign>().DeserializeFromBytes(bytes);
                case ObjectTokens.AiHintInfo:
                    return new CommsSerializationHelper<AIHintInfo>().DeserializeFromBytes(bytes);
                case ObjectTokens.PlayerObjective:
                    return new CommsSerializationHelper<PlayerObjective>().DeserializeFromBytes(bytes);
                case ObjectTokens.SteamClientInitiateConnection:
                    return new CommsSerializationHelper<SteamClientInitiateConnection>().DeserializeFromBytes( bytes );
                case ObjectTokens.SteamGameServerInfo:
                    return new CommsSerializationHelper<SteamGameServerInfo>().DeserializeFromBytes( bytes );
                case ObjectTokens.SteamClientBeginAuthentication:
                    return new CommsSerializationHelper<SteamClientBeginAuthentication>().DeserializeFromBytes( bytes );
                case ObjectTokens.SteamServerPassAuthentication:
                    return new CommsSerializationHelper<SteamServerPassAuthentication>().DeserializeFromBytes( bytes );
                case ObjectTokens.ClientLeavingServer:
                    return new CommsSerializationHelper<ClientLeavingServer>().DeserializeFromBytes( bytes );
                case ObjectTokens.ServerExiting:
                    return new CommsSerializationHelper<ServerExiting>().DeserializeFromBytes( bytes );
                default:
                    throw new InvalidOperationException("DeSerializeNextObjectInBuffer called with unknown object. " 
                        + token.ToString());
            }
            return null;
        }

        public byte[] SerializeObjectToBytes(IMarshallable obj)
        {
            ObjectTokens Token = GetTypeTokenFromObject(obj);
            switch (Token)
            {
                case ObjectTokens.NoObject:
                    throw new InvalidOperationException("SerializeObject called with NoObject.");
                case ObjectTokens.Enq:
                    return new CommsSerializationHelper<EnqObject>().SerializeToBytes((EnqObject)obj);
                case ObjectTokens.Ack:
                    return new CommsSerializationHelper<AckObject>().SerializeToBytes((AckObject)obj);
                case ObjectTokens.MessageString:
                    return new CommsSerializationHelper<MessageString>().SerializeToBytes((MessageString)obj);
                case ObjectTokens.PositionInfo:
                    return new CommsSerializationHelper<PositionInfo>().SerializeToBytes((PositionInfo)obj);
                case ObjectTokens.BaseUnitInfo:
                    return new CommsSerializationHelper<BaseUnitInfo>().SerializeToBytes((BaseUnitInfo)obj);
                case ObjectTokens.GroupInfo:
                    return new CommsSerializationHelper<GroupInfo>().SerializeToBytes((GroupInfo)obj);
                case ObjectTokens.DetectedGroupInfo:
                    return new CommsSerializationHelper<DetectedGroupInfo>().SerializeToBytes((DetectedGroupInfo)obj);
                case ObjectTokens.DetectedUnitInfo:
                    return new CommsSerializationHelper<DetectedUnitInfo>().SerializeToBytes((DetectedUnitInfo)obj);
                case ObjectTokens.DetecedUnitSensorInfo:
                    return new CommsSerializationHelper<DetecedUnitSensorInfo>().SerializeToBytes((DetecedUnitSensorInfo)obj);
                case ObjectTokens.GameInfo:
                    return new CommsSerializationHelper<GameInfo>().SerializeToBytes((GameInfo)obj);
                case ObjectTokens.GameStateInfo:
                    return new CommsSerializationHelper<GameStateInfo>().SerializeToBytes((GameStateInfo)obj);
                case ObjectTokens.BattleDamageReport:
                    return new CommsSerializationHelper<BattleDamageReport>().SerializeToBytes((BattleDamageReport)obj);
                case ObjectTokens.Formation:
                    return new CommsSerializationHelper<Formation>().SerializeToBytes((Formation)obj);
                case ObjectTokens.FormationPosition:
                    return new CommsSerializationHelper<FormationPosition>().SerializeToBytes((FormationPosition)obj);
                case ObjectTokens.MessageInfo:
                    return new CommsSerializationHelper<MessageInfo>().SerializeToBytes((MessageInfo)obj);
                //case ObjectTokens.OrderInfo:
                //    break;
                case ObjectTokens.UnitOrder:
                    return new CommsSerializationHelper<UnitOrder>().SerializeToBytes((UnitOrder)obj);
                case ObjectTokens.UnitEngagementOrder:
                    return new CommsSerializationHelper<UnitEngagementOrder>().SerializeToBytes((UnitEngagementOrder)obj);
                case ObjectTokens.UnitMovementOrder:
                    return new CommsSerializationHelper<UnitMovementOrder>().SerializeToBytes((UnitMovementOrder)obj);
                case ObjectTokens.PlayerInfo:
                    return new CommsSerializationHelper<PlayerInfo>().SerializeToBytes((PlayerInfo)obj);
                case ObjectTokens.SensorClass:
                    return new CommsSerializationHelper<SensorClass>().SerializeToBytes((SensorClass)obj);
                case ObjectTokens.UnitClass:
                    return new CommsSerializationHelper<UnitClass>().SerializeToBytes((UnitClass)obj);
                case ObjectTokens.WeaponClass:
                    return new CommsSerializationHelper<WeaponClass>().SerializeToBytes((WeaponClass)obj);

                case ObjectTokens.ClientInfoRequest:
                    return new CommsSerializationHelper<ClientInfoRequest>().SerializeToBytes((ClientInfoRequest)obj);

                case ObjectTokens.GameControlRequest:
                    return new CommsSerializationHelper<GameControlRequest>().SerializeToBytes((GameControlRequest)obj);
                case ObjectTokens.GameScenario:
                    return new CommsSerializationHelper<GameScenario>().SerializeToBytes((GameScenario)obj);
                case ObjectTokens.GameScenarioCarriedUnit:
                    return new CommsSerializationHelper<GameScenarioCarriedUnit>().SerializeToBytes((GameScenarioCarriedUnit)obj);
                case ObjectTokens.WeatherSystemInfo:
                    return new CommsSerializationHelper<WeatherSystemInfo>().SerializeToBytes((WeatherSystemInfo)obj);
                case ObjectTokens.NWDateTime:
                    return new CommsSerializationHelper<NWDateTime>().SerializeToBytes((NWDateTime)obj);
                case ObjectTokens.WaypointInfo:
                    return new CommsSerializationHelper<WaypointInfo>().SerializeToBytes((WaypointInfo)obj);
                case ObjectTokens.HighLevelOrder:
                    return new CommsSerializationHelper<HighLevelOrder>().SerializeToBytes((HighLevelOrder)obj);
                case ObjectTokens.GameUiControl:
                    return new CommsSerializationHelper<GameUiControl>().SerializeToBytes((GameUiControl)obj);
                case ObjectTokens.EventTrigger:
                    return new CommsSerializationHelper<EventTrigger>().SerializeToBytes((EventTrigger)obj);
                case ObjectTokens.RegionInfo:
                    return new CommsSerializationHelper<RegionInfo>().SerializeToBytes((RegionInfo)obj);
                case ObjectTokens.Campaign:
                    return new CommsSerializationHelper<Campaign>().SerializeToBytes((Campaign)obj);
                case ObjectTokens.User:
                    return new CommsSerializationHelper<User>().SerializeToBytes((User)obj);
                case ObjectTokens.AiHintInfo:
                    return new CommsSerializationHelper<AIHintInfo>().SerializeToBytes((AIHintInfo)obj);
                case ObjectTokens.PlayerObjective:
                    return new CommsSerializationHelper<PlayerObjective>().SerializeToBytes((PlayerObjective)obj);
                case ObjectTokens.SteamClientInitiateConnection:
                    return new CommsSerializationHelper<SteamClientInitiateConnection>().SerializeToBytes( ( SteamClientInitiateConnection )obj );
                case ObjectTokens.SteamGameServerInfo:
                    return new CommsSerializationHelper<SteamGameServerInfo>().SerializeToBytes( ( SteamGameServerInfo )obj );
                case ObjectTokens.SteamClientBeginAuthentication:
                    return new CommsSerializationHelper<SteamClientBeginAuthentication>().SerializeToBytes( ( SteamClientBeginAuthentication )obj );
                case ObjectTokens.SteamServerPassAuthentication:
                    return new CommsSerializationHelper<SteamServerPassAuthentication>().SerializeToBytes( ( SteamServerPassAuthentication )obj );
                case ObjectTokens.ClientLeavingServer:
                    return new CommsSerializationHelper<ClientLeavingServer>().SerializeToBytes( ( ClientLeavingServer )obj );
                case ObjectTokens.ServerExiting:
                    return new CommsSerializationHelper<ServerExiting>().SerializeToBytes( ( ServerExiting )obj );
                default:
                    throw new InvalidOperationException("SerializeObject called with unknown object. " + obj.ObjectTypeToken);
            }
        }

        public ObjectTokens GetTypeTokenFromObject(IMarshallable obj)
        {
            return obj.ObjectTypeToken;
        }

        public byte[] SerializeObjectForSending(IMarshallable obj)
        {
            int Token = (int)GetTypeTokenFromObject(obj);
            byte[] RawBytes = SerializeObjectToBytes(obj);
            int Length = RawBytes.Length;
            byte[] AllBytes = new byte[Length + TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT];
            byte[] TokenBytes = BitConverter.GetBytes((UInt16)Token);
            byte[] LengthBytes = BitConverter.GetBytes((UInt32)Length);
            Array.Copy(TokenBytes, 0, AllBytes, 0, TOKEN_BYTE_COUNT);
            Array.Copy(LengthBytes, 0, AllBytes, TOKEN_BYTE_COUNT, LENGTH_BYTE_COUNT);
            Array.Copy(RawBytes, 0, AllBytes, TOKEN_BYTE_COUNT + LENGTH_BYTE_COUNT, RawBytes.Length);
            return AllBytes;
        }

        #endregion

    }

    [Serializable]
    public class EnqObject : IMarshallable
    {
        public override string ToString()
        {
            return "Enq";
        }

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.Enq; }
        }

        #endregion
    }

    [Serializable]
    public class AckObject : IMarshallable
    {
        public override string ToString()
        {
            return "Ack";
        }

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.Ack; }
        }

        #endregion
    }

    [Serializable]
    public class MessageString :IMarshallable
    {
        public MessageString()
        {

        }

        public MessageString(string message):this()
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.MessageString; }
        }

        #endregion
    }

    [Serializable]
    public class SteamClientInitiateConnection : IMarshallable
    {
        public override string ToString()
        {
            return "SteamClientInitiateConnection";
        }

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.SteamClientInitiateConnection; }
        }

        #endregion
    }

    [Serializable]
    public class SteamGameServerInfo : IMarshallable
    {
        public ulong SteamIDServer { get; set; }

        public bool IsSecure { get; set; }

        public string ServerName { get; set; }

        public override string ToString()
        {
            return "SteamGameServerInfo";
        }

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.SteamGameServerInfo; }
        }

        #endregion
    }

    [Serializable]
    public class SteamClientBeginAuthentication : IMarshallable
    {
        public List<byte> Token { get; set; }

        public override string ToString()
        {
            return "SteamClientBeginAuthentication";
        }

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.SteamClientBeginAuthentication; }
        }
    }

    [Serializable]
    public class SteamServerPassAuthentication : IMarshallable
    {
        public override string ToString()
        {
            return "SteamServerPassAuthentication";
        }

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.SteamServerPassAuthentication; }
        }
    }

    [Serializable]
    public class ClientLeavingServer : IMarshallable
    {
        public string ClientID { get; set; }

        public override string ToString()
        {
            return "ClientLeavingServer";
        }

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.ClientLeavingServer; }
        }
    }

    [Serializable]
    public class ServerExiting : IMarshallable
    {
        public override string ToString()
        {
            return "ServerExiting";
        }

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.ServerExiting; }
        }
    }
}
