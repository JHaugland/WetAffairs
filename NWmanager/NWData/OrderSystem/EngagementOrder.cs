using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms.NonCommEntities;

namespace TTG.NavalWar.NWData.OrderSystem
{
	[Serializable]
	public class EngagementOrder : MovementOrder
	{
		#region "Private fields and public enums"

		

		#endregion
		
		#region "Constructors"

		public EngagementOrder()
		{
			OrderType = GameConstants.OrderType.EngagementOrder;
		}

		public EngagementOrder(DetectedUnit targetDetectedUnit, 
			GameConstants.EngagementOrderType engagementOrder ) : this()
		{
			TargetDetectedUnit = targetDetectedUnit;
			EngagementOrderType = engagementOrder;
		}

		public EngagementOrder(DetectedUnit targetDetectedUnit,
			GameConstants.EngagementOrderType engagementOrder, 
			GameConstants.EngagementStrength engagementStrength)
			: this()
		{
			TargetDetectedUnit = targetDetectedUnit;
			EngagementOrderType = engagementOrder;
			EngagementStrength = engagementStrength;
		}


		public EngagementOrder(DetectedUnit targetDetectedUnit, 
			GameConstants.EngagementOrderType engagementOrder, 
			string weaponClassId) : this()
		{
			TargetDetectedUnit = targetDetectedUnit;
			EngagementOrderType = engagementOrder;
			WeaponClassId = weaponClassId;
		}


		#endregion


		#region "Public properties"

		public DetectedUnit TargetDetectedUnit { get; set; }

		public DetectedGroup TargetDetectedGroup { get; set; }

		public GameConstants.EngagementOrderType EngagementOrderType { get; set; }

		public GameConstants.EngagementStrength EngagementStrength { get; set; }

		public bool IsGroupAttack { get; set; }

		/// <summary>
		/// If null/empty, automatic selection
		/// </summary>
		public string WeaponClassId { get; set; }

		/// <summary>
		/// Preferred number of rounds to fire at target. If 0, automatic selection.
		/// </summary>
		public int RoundCount { get; set; }

		#endregion



		#region "Public methods"

		public override BaseOrderInfo GetBaseOrderInfo()
		{
			var info = base.GetBaseOrderInfo();
			if (TargetDetectedUnit != null)
			{
				info.TargetId = this.TargetDetectedUnit.Id;
				info.TargetDescription = this.TargetDetectedUnit.ToString();
			}
			if (TargetDetectedGroup != null)
			{
				info.TargetId = this.TargetDetectedGroup.Id;
				info.TargetDescription = this.TargetDetectedGroup.ToString();
			}
			return info;
		}

		public override Waypoint GetActiveWaypoint()
		{
			if (TargetDetectedUnit == null)
			{
				return null;
			}
			//if(TargetDetectedUnit.IsFixed)

			//TODO: Replace with intercept

			//TODO: Check for reached objective
			return new Waypoint(TargetDetectedUnit.Position);
			
			
			//return base.CalculateWaypoint();
		}

		public override string ToString()
		{
			string method = "Engage ";
			if (this.EngagementOrderType == GameConstants.EngagementOrderType.EngageNotClose)
			{
				method = "Close and Engage ";
			}
			string target = "Lost target";
			if(TargetDetectedUnit != null)
			{
				target = TargetDetectedUnit.ToString();
			}
			if (TargetDetectedGroup != null)
			{
				target = TargetDetectedGroup.ToString();
		 
			}
			return string.Format("{0} target {1}", method, target);
		}
		#endregion


	}
}
