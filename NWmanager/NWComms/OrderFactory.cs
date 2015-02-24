using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms
{
    [Serializable]
    public static class OrderFactory
    {


        #region "Public properties"

        #endregion



        #region "Public methods"

        /// <summary>
        /// Creates a UnitEngagementOrder
        /// </summary>
        /// <param name="unitId">Id of unit. If isGroupAttack is true, any unitid in group will work</param>
        /// <param name="targetId">Id of TargetDetectedUnit</param>
        /// <param name="weaponClassId">Which weapon to use. If empty or null, automatic selection</param>
        /// <param name="engagementOrderType">CloseAndEngage or EngageNotClose</param>
        /// <param name="engagementStrength">MinimalAttack, DefaultAttack or OverkillAttack</param>
        /// <param name="isGroupAttack">If true, all units in group may participate in attack. 
        /// <param name="isTargetAGroup">If true, targetId is interpreted as the Id of a DetectedGroup, and all units in group will be engaged. 
        /// If false, only unit receiving order.</param>
        /// <returns>Well-formed order object ready for sending</returns>
        public static UnitEngagementOrder CreateEngagementOrder(string unitId,
            string targetId,
            string weaponClassId,
            GameConstants.EngagementOrderType engagementOrderType,
            GameConstants.EngagementStrength engagementStrength,
            bool isGroupAttack, bool isTargetAGroup)
        {
            UnitEngagementOrder order = new UnitEngagementOrder();
            order.TargetId = targetId;
            order.IsGroupAttack = isGroupAttack; 
            order.Id = unitId;
            order.WeaponClassID = weaponClassId; //"mk45mod4";
            order.EngagementType = engagementOrderType;
            order.EngagementStrength = engagementStrength;
            order.IsTargetAGroup = isTargetAGroup;
            return order;
        }

        public static UnitEngagementOrder CreateEngagePositionOrder(string unitId, PositionInfo position, 
            string weaponClassId,
            GameConstants.EngagementOrderType engagementOrderType,
            GameConstants.EngagementStrength engagementStrength,
            int noOfRounds)
        {
            UnitEngagementOrder order = new UnitEngagementOrder();
            order.Id = unitId;
            order.Position = position;
            order.WeaponClassID = weaponClassId;
            order.RoundCount = noOfRounds;
            order.EngagementStrength = engagementStrength;
            order.EngagementType = engagementOrderType;
            return order;
        }

        public static UnitMovementOrder CreateUnitMovementOrder(string unitId, 
            WaypointInfo wayPoint, 
            bool clearExisting)
        {
            UnitMovementOrder order = new UnitMovementOrder();
            order.Id = unitId;
            order.RemoveAllExistingWaypoints = clearExisting;
            order.Waypoints.Add(wayPoint);
            order.UnitSpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
            return order;
        }

        public static UnitMovementOrder CreateUnitMovementOrder(string unitId,
            PositionInfo position,
            bool clearExisting)
        {
            UnitMovementOrder order = new UnitMovementOrder();
            order.Id = unitId;
            order.RemoveAllExistingWaypoints = clearExisting;
            order.Waypoints.Add(new WaypointInfo(position));
            order.UnitSpeedType = GameConstants.UnitSpeedType.UnchangedDefault;
            return order;
        }

        public static UnitMovementOrder CreateUnitMovementOrder(string unitId,
            WaypointInfo wayPoint, GameConstants.UnitSpeedType unitSpeedType,
            bool clearExisting)
        {
            UnitMovementOrder order = new UnitMovementOrder();
            order.Id = unitId;
            order.RemoveAllExistingWaypoints = clearExisting;
            order.Waypoints.Add(wayPoint);
            order.UnitSpeedType = unitSpeedType;
            return order;
        }

        /// <summary>
        /// Creates and returns new UnitMovementOrder
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="wayPoints"></param>
        /// <param name="unitSpeedType"></param>
        /// <param name="clearExisting"></param>
        /// <param name="isRecurring"></param>
        /// <returns></returns>
        public static UnitMovementOrder CreateUnitMovementOrder(string unitId,
            List<WaypointInfo> wayPoints, GameConstants.UnitSpeedType unitSpeedType,
            bool clearExisting, bool isRecurring)
        {
            UnitMovementOrder order = new UnitMovementOrder();
            order.Id = unitId;
            order.RemoveAllExistingWaypoints = clearExisting;
            foreach (var wp in wayPoints)
            {
                order.Waypoints.Add(wp);
            }
            order.IsParameter = isRecurring;
            order.UnitSpeedType = unitSpeedType;
            return order;
        }

        /// <summary>
        /// Creates and returns new UnitMovementOrder
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="wayPoints"></param>
        /// <param name="unitSpeedType"></param>
        /// <param name="clearExisting"></param>
        /// <returns></returns>
        public static UnitMovementOrder CreateUnitMovementOrder(string unitId,
            List<WaypointInfo> wayPoints, GameConstants.UnitSpeedType unitSpeedType,
            bool clearExisting)
        {
            return CreateUnitMovementOrder(unitId, wayPoints, unitSpeedType, clearExisting, false);
        }

        public static UnitOrder CreateAircraftLaunchOrder(string unitId, 
            List<string> aircraftList, UnitMovementOrder moveOrder)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.LaunchAircraft, unitId);
            foreach (var id in aircraftList)
            {
                order.ParameterList.Add(id);
            }
            order.UnitOrders.Add(moveOrder);
            return order;
        }

        public static UnitOrder CreateAircraftLaunchOrder(string unitId, 
            List<string> aircraftList, DetectedUnitInfo attackTarget)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.LaunchAircraft, unitId);
            foreach (var id in aircraftList)
            {
                order.ParameterList.Add(id);
            }
            UnitMovementOrder moveOrder = new UnitMovementOrder();
            moveOrder.Position = null;
            moveOrder.SecondId = attackTarget.Id;
            order.UnitOrders.Add(moveOrder);
            return order;
        }

        public static UnitOrder CreateSetElevationOrder(string unitId, GameConstants.HeightDepthPoints heightDepth)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetElevation, unitId);
            order.HeightDepthPoints = heightDepth;
            return order;
        }

        public static UnitOrder CreateSetSpeedOrder(string unitId, GameConstants.UnitSpeedType unitSpeedType, bool isGroupOrder)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetSpeed, unitId);
            order.UnitSpeedType = unitSpeedType;
            order.IsParameter = isGroupOrder;
            return order;
        }

        public static UnitOrder CreateSetSpeedOrder(string unitId, GameConstants.UnitSpeedType unitSpeedType)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetSpeed, unitId);
            order.UnitSpeedType = unitSpeedType;
            return order;
        }


        public static UnitOrder CreateSetUnitFormationOrder(string unitId, string formationPositionId)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetUnitFormationOrder, unitId);
            order.SecondId = formationPositionId;
            return order;
        }

        public static UnitOrder CreateNewGroupFormationOrder(string unitId, Formation formation)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetNewGroupFormation, unitId);
            order.Formation = formation;
            return order;
        }

        /// <summary>
        /// Creates an order to split one or more units from an existing group, forming a new group. Order can be
        /// sent to any member of the existing group. This unit will only be split from the group if it is
        /// explicitly listed in the unitIds parameter list.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="groupId"></param>
        /// <param name="unitIds"></param>
        /// <returns></returns>
        public static UnitOrder CreateSplitGroupOrder(string unitId, List<string> unitIds)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SplitGroup, unitId);
            if (unitIds != null && unitIds.Count > 0)
            {
                foreach (var uId in unitIds)
                {
                    order.ParameterList.Add(uId);
                }
            }
            return order;
        }

        /// <summary>
        /// Orders one or more units to form a new or existing group (leaving its existing group if nexessary). If
        /// groupId is blank or invalid, a new group will be formed. The unit specified with newMainUnitId will always
        /// join the new group, and be its main unit. In addition, all units specified in the parameter list 
        /// unitIds will join new group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="newMainUnitId"></param>
        /// <param name="unitIds"></param>
        /// <returns></returns>
        public static UnitOrder CreateJoinGroupOrder(string groupId, string newMainUnitId, List<string> unitIds)
        {

            UnitOrder order = new UnitOrder(TTG.NavalWar.NWComms.GameConstants.UnitOrderType.JoinGroups, newMainUnitId);
            order.SecondId = groupId;
            if (unitIds != null && unitIds.Count > 0)
            {
                foreach (var uId in unitIds)
                {
                    order.ParameterList.Add(uId);
                }
            }
            return order;
        }

        public static UnitOrder CreateChangeAircraftLoadoutOrder(string carrierUnitId, 
            string aircraftUnitId, 
            string weaponLoadoutName)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.ChangeAircraftLoadout, carrierUnitId);
            order.SecondId = aircraftUnitId;
            order.StringParameter = weaponLoadoutName;
            return order;
        }

        public static UnitOrder CreateChangeAircraftLoadoutOrder(string carrierUnitId, 
            string aircraftUnitId, GameConstants.WeaponLoadType weaponLoadType, GameConstants.WeaponLoadModifier weaponLoadModifier)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.ChangeAircraftLoadout, carrierUnitId);
            order.SecondId = aircraftUnitId;
            order.StringParameter = string.Empty;
            order.WeaponLoadType = weaponLoadType;
            order.WeaponLoadModifier = weaponLoadModifier;
            return order;

        }

        public static UnitOrder CreateSensorActivationOrder(string unitId, GameConstants.SensorType sensorType, bool isActive)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SensorActivationOrder, unitId);
            order.SecondId = string.Empty;
            order.SensorType = sensorType;
            order.IsParameter = isActive;

            return order;

        }

        public static UnitOrder CreateSensorActivationOrder(string unitId, string sensorId, bool isActive)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SensorActivationOrder, unitId);
            order.SecondId = sensorId;
            order.IsParameter = isActive;

            return order;
        }

        public static UnitOrder CreateSensorDeploymentOrder(string unitId, string sensorId, 
            bool isDeployed, bool isDeep)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SensorDeploymentOrder, unitId);
            order.SecondId = sensorId;
            order.IsParameter = isDeployed;
            if (isDeep)
            {
                order.ValueParameter = 1;
            }

            return order;
        }

        public static UnitOrder CreateSensorDeploymentOrder(string unitId, GameConstants.SensorType sensorType,
            bool isDeployed, bool isDeep)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SensorDeploymentOrder, unitId);
            order.SecondId = string.Empty;
            order.SensorType = sensorType;
            order.IsParameter = isDeployed;
            if (isDeep)
            {
                order.ValueParameter = 1;
            }

            return order;
        }

        public static UnitOrder CreateRadarDegradationJammingOrder(string unitId, PositionInfo position)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.JammerRadarDegradation;
            if (position != null)
            {
                order.Position = position;
            }
            return order;
        }

        public static UnitOrder CreateCommunicationDegradationJammingOrder(string unitId, PositionInfo position)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.JammerCommunicationDegradation;
            if (position != null)
            {
                order.Position = position;
            }
            return order;
        }

        public static UnitOrder CreateRadarDistortionOrder(string unitId, PositionInfo position)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.JammerRadarDistortion;
            if (position != null)
            {
                order.Position = position;
            }
            return order;
        }

        public static UnitOrder CreateSonobuoyDeploymentOrder(string unitId, PositionInfo position, bool isActive, bool isIntermediateDepth)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.DropSonobuoy;
            if (position != null)
            {
                order.Position = position;
            }
            order.IsParameter = isActive;
            order.ValueParameter = isIntermediateDepth ? 1.0 : 0.0;

            return order;
        }

        public static UnitOrder CreateSonobuoyDeploymentOrder(string unitId, PositionInfo position, bool isActive, double radiusM)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.DropSonobuoy;
            if (position != null)
            {
                order.Position = position;
            }
            order.IsParameter = isActive;
            order.ValueParameter = 1.0;
            order.RadiusM = radiusM; //note: if RadiusM is > 0, order is interpreted as a "drop all sonobouys you have in this area"
            return order;
        }

        public static UnitOrder CreateMineDeploymentOrder(string unitId, PositionInfo position, bool deployAllMines)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SpecialOrders, unitId);
            order.SpecialOrder = GameConstants.SpecialOrders.DropMine;
            if (position != null)
            {
                order.Position = position;
            }
            order.IsParameter = deployAllMines;
            return order;
        }



        public static UnitOrder CreateReturnToBaseOrder(string unitId)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.ReturnToBase, unitId);
            return order;
        }

        public static UnitOrder CreateSetBaseOrder(string unitId, string returnToUnitId)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.ReturnToBase, unitId);
            order.SecondId = returnToUnitId;
            return order;
        }

        public static UnitOrder CreateRefuelInAirOrder(string unitId, string refuelingUnitId)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.RefuelInAir, unitId);
            order.SecondId = refuelingUnitId;
            return order;
        }

        public static GameControlRequest CreateFriendOrFoeDesignationOrder(string detectedContactId, 
            GameConstants.FriendOrFoe friendOrFoe)
        {
            GameControlRequest request = new GameControlRequest();
            request.Id = detectedContactId;
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.DesignateContactFriendOrFoe;
            request.FriendOrFoeDesignation = friendOrFoe;
            return request;
        }

        public static UnitOrder CreateAcquireNewUnitOrder(string carrierId, string unitClassId, string weaponLoadName)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.AcquireNewUnit, carrierId);
            order.SecondId = unitClassId;
            order.StringParameter = weaponLoadName;
            return order;
        }

        public static UnitOrder CreateAcquireAmmoOrder(string platformId, string weaponClassId, int count)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.AcquireAmmo, platformId);
            order.SecondId = weaponClassId;
            order.ValueParameter = count;
            return order;
        }


        /// <summary>
        /// Generates a request object to request from server a list of all non-carried units within 
        /// a certain radius from a position. Server will respond by sending a GameStateInfo object
        /// over the network.
        /// </summary>
        /// <param name="position">Center of area</param>
        /// <param name="radiusM">Include units within this radius in meters from center</param>
        /// <returns>GameControlRequest</returns>
        public static GameControlRequest CreateGetAllUnitsInAreaRequest(PositionInfo position, double radiusM)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlParameterValue = radiusM;
            request.Position = position;
            return request;
        }

        public static GameControlRequest CreateDefaultFoFDesignationRequest(bool isAllUndefinedTargetsHostile)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetUnknownContactFoFDesignation;
            request.IsParameter = isAllUndefinedTargetsHostile;
            return request;
        }

        public static GameControlRequest CreateAutomaticEvasionStatusRequest(bool isAutomaticallyEvadingAttacks)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetAutomaticUnitEvasion;
            request.IsParameter = isAutomaticallyEvadingAttacks;
            return request;
        }

        public static GameControlRequest CreateAutomaticActiveSensorResponseRequest(bool isAlwaysRespondingToActiveSensor)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetAutomaticActiveSensorResponse;
            request.IsParameter = isAlwaysRespondingToActiveSensor;
            return request;
        }

        public static GameControlRequest CreateAutomaticTimeCompressionOnDetectionRequest(bool isAutomaticallyChangingTimeCompressionOnDetection, double timeCompressionToChangeTo)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnDetection;
            request.IsParameter = isAutomaticallyChangingTimeCompressionOnDetection;
            request.ControlParameterValue = timeCompressionToChangeTo;
            return request;
        }

        public static GameControlRequest CreateAutomaticTimeCompressionOnBattleReportRequest(bool isAutomaticallyChangingTimeCompressionOnBattleReport, double timeCompressionToChangeTo)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnBattleReport;
            request.IsParameter = isAutomaticallyChangingTimeCompressionOnBattleReport;
            request.ControlParameterValue = timeCompressionToChangeTo;
            return request;
        }

        public static GameControlRequest CreateAutomaticTimeCompressionOnNoOrderRequest(bool isAutomaticallyChangingTimeCompressionOnNoOrder, double timeCompressionToChangeTo)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetAutomaticTimeCompressionOnNoOrder;
            request.IsParameter = isAutomaticallyChangingTimeCompressionOnNoOrder;
            request.ControlParameterValue = timeCompressionToChangeTo;
            return request;
        }

        public static GameControlRequest CreateCheatOrder(string cheatCode, string parameter)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.CheatCode;
            request.Id = cheatCode;
            request.ControlParameterString = parameter;
            return request;
        }

        public static GameControlRequest CreateCheatOrder(string cheatCode)
        {
            return CreateCheatOrder(cheatCode, null);
        }

        public static GameControlRequest CreateRemoveHighLevelOrderRequest(string id)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.RemoveHighLevelOrder;
            request.Id = id;
            return request;
        }

        public static GameControlRequest CreateRenameUnitOrder(string unitId, string name)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.RenameUnit;
            request.ControlParameterString = name;
            request.Id = unitId;
            return request;
        }

        public static GameControlRequest CreateRenameGroupOrder(string unitId, string name)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.RenameGroup;
            request.ControlParameterString = name;
            request.Id = unitId;
            return request;
        }

        public static GameControlRequest CreateMessageToPlayers(string playerId, string message)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SendMessageToPlayers;
            request.ControlParameterString = message;
            request.Id = playerId;
            return request;
        }

        public static GameControlRequest CreateMessageToPlayers(GameConstants.SendMessageTo recipient, string message)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SendMessageToPlayers;
            request.ControlParameterString = message;
            request.SendMessageTo = recipient;
            request.Id = string.Empty;
            return request;
        }

        public static GameControlRequest CreatePlayerSelectScenarioOrder(string scenarioId, int skillLevel)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.PlayerSelectScenario;
            request.Id = scenarioId;
            request.ControlParameterValue = skillLevel;
            return request;
        }


        public static GameControlRequest CreateSetPlayerSelectUserOrder( string userId )
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.PlayerSelectUser;
            request.Id = userId;
            return request;
        }

        public static GameControlRequest CreateSetPlayerSelectCampaignOrder(string campaignId)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.PlayerSelectCampaign;
            request.Id = campaignId;
            return request;
        }

        public static GameControlRequest CreateSetPlayerNameOrder(string playerName)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.PlayerSetName;
            request.Id = playerName;
            return request;
        }

        public static GameControlRequest CreateGlobalWeaponOrdersOrder(
            GameConstants.WeaponOrders weaponOrders, bool changeAllUnits)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetWeaponOrdersGlobal;
            request.WeaponOrders = weaponOrders;
            request.IsParameter = changeAllUnits;
            return request;
        }

        public static GameControlRequest CreateUnitWeaponOrdersOrder(
            GameConstants.WeaponOrders weaponOrders, string unitId)
        {
            GameControlRequest request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetWeaponOrdersUnit;
            request.WeaponOrders = weaponOrders;
            request.Id = unitId;
            return request;
        }

        /// <summary>
        /// Create signal to backend that corresponds to existing EventTrigger. Signal must be equal to Id of EventTrigger.
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public static GameControlRequest CreateTriggerSignal(string signal)
        {
            var request = new GameControlRequest();
            request.ControlRequestType = CommsMarshaller.GameControlRequestType.TriggerSignal;
            request.ControlParameterString = signal;
            return request;
        }

        #endregion


    }
}
