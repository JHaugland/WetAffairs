using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class BattleManager : MonoBehaviour {

	public void Attack(string lat, string lng, BaseUnitInfo info)
	{
		
            //double Lat = double.Parse(lat);
            //double Lon = double.Parse(lng);
            //string Id;
            //if (info != null)
            //{
            //    Id = info.Id;
            //}
            //else
            //{
            //    //~ ShowInfo("Select a unit in ListBox first.");
            //    return;
            //}
            //UnitMovementOrder order = new UnitMovementOrder();
            //order.Id = Id;
            //PositionInfo pos = new PositionInfo();
            //pos.Latitude = Lat;
            //pos.Longitude = Lon;
            //order.RemoveAllExistingWaypoints = true;
            //order.Positions.Add(pos);
	}
}
