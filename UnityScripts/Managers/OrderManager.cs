using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System.Collections.Generic;
using System.Text;
using System;

public class OrderManager : MonoBehaviour
{

    public void Move(double lat, double lng, BaseUnitInfo info, bool removeWaypoints)
    {

        string Id;
        if (info != null)
        {
            Id = info.Id;
        }
        else
        {

            return;
        }
        UnitMovementOrder order = OrderFactory.CreateUnitMovementOrder(Id, new PositionInfo(lat, lng), removeWaypoints);
        //order.Id = Id;
        //PositionInfo pos = new PositionInfo();
        //pos.Latitude = lat;
        //pos.Longitude = lng;
        //order.RemoveAllExistingWaypoints = removeWaypoints;
        //order.Waypoints.Add(new WaypointInfo(pos));

        GameManager.Instance.NetworkManager.Send(order);

        //GameManager.Instance.MessageManager.AddMessage(string.Format("Move order sendt. Heading for {0}, {1}", lat, lng), GameManager.MessageTypes.Game);
    }

    public void Attack(BaseUnitInfo attacker, DetectedUnitInfo attackee, GameConstants.EngagementOrderType engagementOrderType, string weaponClassID, int roundCount)
    {
        if (attacker != null && attackee != null)
        {
            //~ ShowInfo(string.Format("*** Unit {0} to engage target {1}", unit.UnitName, det.DetectedUnitDescription));
            UnitEngagementOrder order = OrderFactory.CreateEngagementOrder(attacker.Id, attackee.Id, weaponClassID, engagementOrderType,
                                                                            GameConstants.EngagementStrength.DefaultAttack, false);
            //order.DetectedUnitId = attackee.Id;
            //order.Id = attacker.Id;
            //if (weaponClassID != null)
            //{
            //    order.WeaponClassID = weaponClassID;
            //}
            //if (roundCount != null && roundCount > 0)
            //{
            //    order.RoundCount = roundCount;
            //}

            //order.EngagementType = engagementOrderType;
            GameManager.Instance.NetworkManager.Send(order);

            GameManager.Instance.MessageManager.AddMessage(string.Format("Attacking {0} with unit {1}", attackee.Id, attacker.Id), GameManager.MessageTypes.Battle, attackee.Position);
        }
        else
        {
            //~ ShowInfo("Select own unit and detected unit to engage.");
        }
    }

    public void Attack(string attackerId, string detectedInfoId, GameConstants.EngagementOrderType engagementOrderType, WeaponInfo weaponInfo, GameConstants.EngagementStrength strength, bool isGroupAttack)
    {
        GameManager.Instance.NetworkManager.Send(OrderFactory.CreateEngagementOrder(attackerId, detectedInfoId, weaponInfo == null ? string.Empty : weaponInfo.Id, engagementOrderType, strength, isGroupAttack));

        GameManager.Instance.MessageManager.AddMessage(string.Format("Attacking {0} with unit {1}", attackerId, detectedInfoId), GameManager.MessageTypes.Battle, null);
    }

    public void Launch(BaseUnitInfo unit, GameConstants.UnitOrderType unitOrderType)
    {
        if (unit != null)
        {
            UnitOrder order = new UnitOrder(unitOrderType, unit.Id);
            int unitcount = 0;
            string aircraftlist = string.Empty;
            foreach (CarriedUnitInfo c in unit.CarriedUnits)
            {
                if (c.ReadyInSec < 1 && unitcount < 1)
                {
                    order.ParameterList.Add(c.Id);
                    aircraftlist += c.ToString() + " ";
                    unitcount++;
                }
            }
            GameManager.Instance.MessageManager.AddMessage(string.Format(
                "UnitOrder to unit {0} : Launch {1} aircraft: {2} ",
                order.Id, order.ParameterList.Count, aircraftlist), GameManager.MessageTypes.Game, unit.Position);
            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void Launch(BaseUnitInfo unit, CarriedUnitInfo unitToLaunch, GameConstants.UnitOrderType unitOrderType)
    {
        if (unit != null)
        {
            UnitOrder order = new UnitOrder(unitOrderType, unit.Id);
            order.ParameterList.Add(unitToLaunch.Id);
            GameManager.Instance.MessageManager.AddMessage(string.Format(
                "Launching aircraft: {0}", unit.UnitName), GameManager.MessageTypes.Game, unit.Position);

            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void LaunchAttack(BaseUnitInfo unit, DetectedUnitInfo det, List<CarriedUnit> unitsToLaunch, Coordinate coord)
    {
        List<string> units = new List<string>();
        foreach (CarriedUnit cui in unitsToLaunch)
        {
            units.Add(cui.UnitInfo.Id);
        }
        UnitMovementOrder moveOrder = new UnitMovementOrder();
        if (coord != null)
        {
            moveOrder.Waypoints.Add(new WaypointInfo(new PositionInfo(coord.Latitude, coord.Longitude)));
        }
        GameManager.Instance.NetworkManager.Send(OrderFactory.CreateAircraftLaunchOrder(unit.Id, units, moveOrder));

        GameManager.Instance.NetworkManager.Send(OrderFactory.CreateEngagementOrder(unit.Id, det.Id, string.Empty, GameConstants.EngagementOrderType.CloseAndEngage,
                                        GameConstants.EngagementStrength.DefaultAttack, true));
    }

    public void Launch(BaseUnitInfo unit, List<CarriedUnit> unitsToLaunch, GameConstants.UnitOrderType unitOrderType)
    {
        if (unit != null)
        {
            UnitOrder order = new UnitOrder(unitOrderType, unit.Id);

            foreach (CarriedUnit cui in unitsToLaunch)
            {
                order.ParameterList.Add(cui.UnitInfo.Id);
            }

            GameManager.Instance.MessageManager.AddMessage(string.Format(
                "Launching aircrafts: {0}", unit.UnitName), GameManager.MessageTypes.Game, unit.Position);

            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void Launch(BaseUnitInfo unit, List<CarriedUnit> unitsToLaunch, GameConstants.UnitOrderType unitOrderType, Coordinate coord)
    {
        if (unit != null)
        {
            UnitOrder order = new UnitOrder(unitOrderType, unit.Id);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Launcing Aircrafts:");

            foreach (CarriedUnit cui in unitsToLaunch)
            {
                order.ParameterList.Add(cui.UnitInfo.Id);
                sb.AppendLine(cui.UnitInfo.Id);
            }

            UnitMovementOrder moveOrder = new UnitMovementOrder();
            moveOrder.Waypoints.Add(new WaypointInfo(new PositionInfo(coord.Latitude, coord.Longitude)));
            order.UnitOrders.Add(moveOrder);




            GameManager.Instance.MessageManager.AddMessage(sb.ToString(), GameManager.MessageTypes.Game, unit.Position);

            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void ChangeLoadOut(BaseUnitInfo carrier, CarriedUnitInfo carriedUnit, string loadOutName)
    {
        UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.ChangeAircraftLoadout, carrier.Id);
        order.SecondId = carriedUnit.Id;
        order.StringParamater = loadOutName;

        GameManager.Instance.MessageManager.AddMessage(string.Format(
            "Changing loadout for {0} to {1}", carriedUnit.UnitName, loadOutName), GameManager.MessageTypes.Game, carrier.Position);

        GameManager.Instance.NetworkManager.Send(order);

    }

    public void SetSensorPassiveActive(BaseUnitInfo unit, SensorInfo sensor, bool active)
    {
        if (unit != null)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SensorActivationOrder, unit.Id);
            order.IsParameter = active;
            order.SecondId = sensor.Id;

            GameManager.Instance.MessageManager.AddMessage(string.Format(
                "UnitOrder to unit {0} : Change {1} to: {2} sensoring",
                order.Id, sensor.Name, active == true ? "active" : "passive"), GameManager.MessageTypes.Detection, unit.Position);

            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void SetTimeCompression(int compression)
    {
        GameControlRequest request = new GameControlRequest();

        request.ControlRequestType = CommsMarshaller.GameControlRequestType.SetTimeCompressionRatio;
        request.ControlParameterValue = compression;

        GameManager.Instance.NetworkManager.Send(request);
    }

    public void SetNewGroupFormation(PlayerUnit mainUnit, Formation formation)
    {
        if (mainUnit != null)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetNewGroupFormation, mainUnit.Info.Id);
            order.Formation = formation;


            GameManager.Instance.MessageManager.AddMessage(string.Format(
                "Changing formation on groupMainUnit: {0}",
                mainUnit.Info.UnitName), GameManager.MessageTypes.Detection, mainUnit.Info.Position);

            try
            {
                GameManager.Instance.NetworkManager.Send(order);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("ERROR: {0}", ex.Message));
            }
        }
    }

    public void SplitGroup(PlayerUnit mainUnit, List<PlayerUnit> unitsInNewGroup)
    {
        if (mainUnit != null && unitsInNewGroup.Count > 0)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SplitGroup, mainUnit.Info.Id);
            List<string> units = new List<string>();
            foreach (PlayerUnit p in unitsInNewGroup)
            {
                units.Add(p.Info.Id);
            }

            order.ParameterList = units;

            GameManager.Instance.MessageManager.AddMessage(string.Format(
               "Splitting group on main unit: {0}",
               mainUnit.Info.UnitName), GameManager.MessageTypes.Detection, mainUnit.Info.Position);

            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void JoinGroups(PlayerUnit mainUnit, PlayerUnit submitor)
    {
        if (mainUnit != null)
        {
            UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.JoinGroups, mainUnit.Info.Id);
            order.SecondId = submitor.Info.GroupId;

            GameManager.Instance.MessageManager.AddMessage(string.Format(
               "Joining group on main unit: {0}",
               mainUnit.Info.UnitName), GameManager.MessageTypes.Detection, mainUnit.Info.Position);

            Debug.Log("Joining groups");
            GameManager.Instance.NetworkManager.Send(order);
        }
    }

    public void ChangeHeightDepth(PlayerUnit unit, GameConstants.HeightDepthPoints heightDepthPoint)
    {
        if (unit != null)
        {
            GameManager.Instance.MessageManager.AddMessage(string.Format(
               "Unit: {0} changing elevation from: {1} to : {2}",
               unit.Info.UnitName, unit.Info.Position.HeightDepth.ToString(), heightDepthPoint.ToString()), GameManager.MessageTypes.Detection, unit.Info.Position);

            GameManager.Instance.NetworkManager.Send(OrderFactory.CreateSetElevationOrder(unit.Info.Id, heightDepthPoint));
        }
    }

    public void ChangeSpeed(PlayerUnit mainUnit, GameConstants.UnitSpeedType speedType)
    {
        if (mainUnit != null)
        {
            //UnitOrder order = new UnitOrder(GameConstants.UnitOrderType.SetElevation, mainUnit.Info.Id);
            //order.UnitSpeedType = speedType;

            GameManager.Instance.MessageManager.AddMessage(string.Format(
               "Unit: {0} changing speed from: {1} to : {2}",
               mainUnit.Info.UnitName, mainUnit.Info.ActualSpeed.ToString(), speedType.ToString()), GameManager.MessageTypes.Detection, mainUnit.Info.Position);

            GameManager.Instance.NetworkManager.Send(OrderFactory.CreateSetSpeedOrder(mainUnit.Info.Id, speedType));
            
        }
    }

    public void ChangeFriendOrFoeDesignation(DetectedUnitInfo detectedUnit, GameConstants.FriendOrFoe friendOfFoe)
    {
        GameManager.Instance.NetworkManager.Send(OrderFactory.CreateFriendOrFoeDesignationOrder(detectedUnit.Id, friendOfFoe));

        GameManager.Instance.MessageManager.AddMessage(string.Format(
              "Changing FriendOrFoe Designation on : {0} to: {1}",
              detectedUnit.Id, friendOfFoe.ToString()), GameManager.MessageTypes.Detection, detectedUnit.Position);
    }

    public void ReturnToBase(PlayerUnit unit)
    {
        GameManager.Instance.NetworkManager.Send(OrderFactory.CreateReturnToBaseOrder(unit.Info.Id));

        GameManager.Instance.MessageManager.AddMessage(string.Format(
              "Roger! Returning to base. "), GameManager.MessageTypes.Detection, unit.Position);
    }


}
