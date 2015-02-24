using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWData.Ai;

namespace TTG.NavalWar.NWData.Units
{
	[Serializable]
	public class Group : BaseMovableObject
	{
		private readonly List<BaseUnit> _units = new List<BaseUnit>();
		private BaseUnit _mainUnit;
		private Formation _formation;

		#region "Constructors"

		public Group()
		{

		}

		public Group(BaseUnit mainUnit)
			: this()
		{
			MainUnit = mainUnit;
		}

		#endregion
        
		#region "Public properties"

		public IList<BaseUnit> Units
		{
			get 
			{
				return _units.AsReadOnly();
			}
		}

		public GameConstants.DirtyStatus DirtySetting { get; set; }

		public BaseUnit MainUnit
		{
			get
			{
				if (_mainUnit == null)
				{
				    foreach (var unit in _units)
				    {
				        if (!unit.IsMarkedForDeletion)
				        {
				            _mainUnit = unit;
                            SetDirty(GameConstants.DirtyStatus.UnitChanged);
				            break;
				        }
				    }
				}
				return _mainUnit;
			}
			set
			{
				_mainUnit = value;
			}
		}

		public GameConstants.MissionType MissionType
		{
			get
			{
				if (MainUnit != null)
				{
					return MainUnit.MissionType;
				}
				else
				{
					return GameConstants.MissionType.Other;
				}
			}
		}

		public GameConstants.MissionTargetType MissionTargetType
		{
			get
			{
				if (MainUnit != null)
				{
					return MainUnit.MissionTargetType;
				}
				else
				{
					return GameConstants.MissionTargetType.Undefined;
				}
			}
		}

	    /// <summary>
	    /// A flag indicating whether the group is reorganising/staging/waiting for units to
	    /// catch up to their proper position. This to allow group leader(s) and other units
	    /// to slow down properly.
	    /// </summary>
	    public bool IsStaging { get; private set; }

	    /// <summary>
		/// The speed set for the group by order (user or AI).
		/// </summary>
		public double SpeedKph { get; set; }

		public override Position Position //note override is read-only. surprisingly, the compiler allows it
		{
			get
			{
				if (MainUnit != null)
				{
					return MainUnit.Position;
				}
                return null;
			}
		}

		public Player OwnerPlayer
		{
			get
			{
				if (MainUnit != null)
				{
					return MainUnit.OwnerPlayer;
				}
                return null;
			}
		}


		public Formation Formation
		{
			get
			{
				return _formation;
			}
			set
			{
                if (_formation != value)
                {
                    _formation = value;
                    SetDirty(GameConstants.DirtyStatus.UnitChanged);
                }
			}
		}

		#endregion

		#region "Public methods"

		public bool RemoveIfSingleUnit()
		{
			if (_units.Count < 2)
			{
				Player player = OwnerPlayer;
			    foreach (var unit in _units)
			    {
			        unit.Group = null;
                    unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
			    }
				_units.Clear();
			    _mainUnit = null;
				IsMarkedForDeletion = true;
				if (player != null)
				{
					player.RemoveGroup(this);
				}
				return true;
			}
			return false;
		}

		public Dictionary<string, int> GetUnitTypeBreakdownList()
		{
		    var unitTypes = GetUnitTypeBreakdown();
		    return unitTypes.ToDictionary(ut => GameManager.Instance.GetUnitSubTypeName(ut.Key), ut => ut.Value);
		}

		public Dictionary<GameConstants.UnitSubType, int> GetUnitTypeBreakdown()
		{
			var list = new Dictionary<GameConstants.UnitSubType, int>();

			foreach (var unit in _units)
			{ 
				GameConstants.UnitSubType subType = unit.GetUnitSubType();
				if (!list.ContainsKey(subType))
				{
					list.Add(subType, 1);
				}
				else
				{
					var entry = list.First(u => u.Key == subType);
					int value = entry.Value;
					value++;
					list.Remove(subType);
					list.Add(subType, value);
				}
				
			}
			return list;
		}

		public void AutoAssignUnitsToFormation()
		{
			var newMainUnitAssigned = false;
			BaseUnit oldMainUnit = null;
			if (MainUnit != null && (MainUnit.IsMarkedForDeletion || MainUnit.HitPoints < 1))
			{
				oldMainUnit = MainUnit;
				newMainUnitAssigned = true;
				GameManager.Instance.Log.LogDebug(
					string.Format("AutoAssignUnitsToFormation: Group {0} mainunit {1} has been destroyed and is removed.", 
					ToString(), this.MainUnit.ToShortString()));
				RemoveUnit(MainUnit);
				MainUnit = null; //Remember: getter automatically assigns new MainUnit!
			}

			if (_units.Count == 0 || MainUnit == null)
			{
				GameManager.Instance.Log.LogDebug(
					string.Format("AutoAssignUnitsToFormation: Group {0} has no units and is being deleted.", ToString()));
				IsMarkedForDeletion = true;
				return;
			}

			if(newMainUnitAssigned)
			{
				try 
				{
					MainUnit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
					SetDirty(GameConstants.DirtyStatus.UnitChanged);
					GameManager.Instance.Log.LogDebug(
						string.Format("AutoAssignUnitsToFormation: Group {0} has been assigned a new mainunit {1}.",
						ToString(), this.MainUnit.ToShortString()));
					if (MainUnit.MovementOrder is MovementFormationOrder)
					{
						MainUnit.MovementOrder = new MovementOrder();
						if (oldMainUnit.MovementOrder != null) //take over movementorders of old main unit
						{
							MainUnit.MovementOrder = oldMainUnit.MovementOrder;
						}
					}
				}
				catch (Exception ex)
				{
					GameManager.Instance.Log.LogError(
						string.Format("Group {0} has no main unit and automatic assignment failed: {1}", 
						ToString(), ex.Message));
					return;
				}
			}

			if (Formation == null 
				|| Formation.FormationPositions == null 
				|| Formation.FormationPositions.Count == 0) //load default formation
			{
				try
				{
					if (MainUnit is AircraftUnit)
					{
						Formation = GameManager.Instance.GameData.GetFormationById(GameConstants.FORMATION_AIRCRAFT_DEFAULT).Clone();
					}
					else
					{
						Formation = GameManager.Instance.GameData.GetFormationById(GameConstants.FORMATION_SURFACE_DEFAULT).Clone();
					} 
                    //TODO: MORE, like for subs or mixed groups?
				}
				catch (Exception ex)
				{
					GameManager.Instance.Log.LogError(
						string.Format("AutoAssignUnitsToFormation(): Failed to load default formation for Group {0}." + ex.Message));
					return;
				}
			}

			//Mainunit should always have the first FormationPosition!
			MainUnit.FormationPositionId = Formation.FormationPositions[0].Id;
			Formation.FormationPositions[0].AssignedUnitId = MainUnit.Id;
			
            //Ok, we now have a formation. Assign units that do not have a formation position to a sensible pos.
			//First, ensure all unit positions are valid
			foreach (var u in _units)
			{
				if (!string.IsNullOrEmpty(u.FormationPositionId))
				{
					var fpos = Formation.GetFormationPositionById(u.FormationPositionId);
					if (fpos == null)
					{
						u.FormationPositionId = string.Empty; //if it doesn't exist, clear it
					}
					else
					{
						fpos.AssignedUnitId = u.Id;
					}
				}
			}

            var unassignedUnits = from u in _units
								  where string.IsNullOrEmpty(u.FormationPositionId)
								  select u;

			foreach (var unit in unassignedUnits)
			{
				var fPos = Formation.GetPositionForUnit(unit.UnitType, unit.RoleList);
				if (fPos != null)
				{
                    unit.FormationPositionId = fPos.Id;
					fPos.AssignedUnitId = unit.Id;
				}
				else
				{
					var newFormPos = Formation.CreateNewPositionForUnit(unit.UnitType);
					newFormPos.AssignedUnitId = unit.Id;
					unit.FormationPositionId = newFormPos.Id;
				}
				unit.SetUnitFormationOrder();
			}

            CheckIfGroupIsStaging();
		}

		public void SetDirty(GameConstants.DirtyStatus newDirtySetting)
		{
			if (newDirtySetting == GameConstants.DirtyStatus.Clean) //when set clean we mean it
			{
				DirtySetting = GameConstants.DirtyStatus.Clean;
			}
			else if ((int)newDirtySetting > (int)DirtySetting) //only change dirty setting if it increases dirtyness
			{
				DirtySetting = newDirtySetting;
			}
		}

		public void AddUnit(BaseUnit unit)
		{
            if (unit.Group != null && unit.Group != this)
            {
                unit.Group.RemoveUnit(unit);
            }

			if(!_units.Exists(u=>u.Id == unit.Id))
			{
				SetDirty(GameConstants.DirtyStatus.UnitChanged);
				_units.Add(unit);
				unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
				GameManager.Instance.Log.LogDebug(
					string.Format("Group {0} adds unit {1}. Count={2}", 
					Id, unit.ToString(), _units.Count));
				if (_mainUnit == null)
				{
					_mainUnit = unit;
				}
				if (OwnerPlayer != null)
				{
					OwnerPlayer.RemoveGroup(this);
					OwnerPlayer.AddGroup(this);
				}
			}
			unit.Group = this;
		}

		public List<EngagementStatus> GetAllWeaponEngagementStatuses(string weaponClassId, 
			DetectedUnit detectedUnit, bool primaryWeaponOnly)
		{
            var statuses = _units.Select(u => u.GetBestAvailableWeapon(weaponClassId, detectedUnit, primaryWeaponOnly)).Where(status => status != null).ToList();
		    var statusesSorted = from s in statuses
								 orderby s.Score descending
								 select s;

			return statusesSorted.ToList();
		}

		public bool RemoveUnit(BaseUnit unit)
		{
			if (unit.GroupId == Id)
			{
                unit.Group = null;
			}
			
			if (_units.Remove(unit))
			{
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
                AutoAssignUnitsToFormation();
			    RemoveIfSingleUnit();
			    return true;
			}
		    return false;
		}

		public void SplitGroup(List<string> unitIdList)
		{
			if (unitIdList == null || unitIdList.Count == 0)
			{
				return;
			}
			var wp = new Waypoint();
			var newGroup = new Group();
		    var activeWaypoint = MainUnit.GetActiveWaypoint();
            if (activeWaypoint != null)
			{
                wp = activeWaypoint.Clone();
			}
			else
			{ 
				wp = new Waypoint(MainUnit.Position.Offset(new PositionOffset(0,1000,0)));
			}
            // Cache the owner player as the group might be marked for deletion if only one unit is remaining after splitting from group.
		    Player ownerPlayer = OwnerPlayer;
			foreach (var id in unitIdList)
			{
                var unit = ownerPlayer.GetUnitById(id);
				if (unit != null)
				{
					unit.SetDirty(GameConstants.DirtyStatus.UnitChanged);
					unit.FormationPositionId = string.Empty;
					RemoveUnit(unit);
					if (wp != null)
					{
						unit.MovementOrder = new MovementOrder(wp);
					}
					newGroup.AddUnit(unit);
				}
			}
            ownerPlayer.AddGroup(newGroup);
			SetDirty(GameConstants.DirtyStatus.UnitChanged);
			newGroup.SetDirty(GameConstants.DirtyStatus.NewlyCreated);
            if (Formation != null)
            {
                newGroup.Formation = Formation.Clone();    
            }
			newGroup.AutoAssignUnitsToFormation();
			newGroup.Name = Name + " (2)";
			GameManager.Instance.Log.LogDebug(
				string.Format("Group.SplitGroup: Group {0} split. New group formed: {1}.", 
				ToString(), newGroup.ToString()));
            var splitMsg = new GameStateInfo(GameConstants.GameStateInfoType.GroupHasBeenSplit, this.Id, newGroup.Id);
            ownerPlayer.Send(splitMsg);
		    newGroup.RemoveIfSingleUnit();
		}
		
		/// <summary>
		/// Finds out if all units in the group with a FormationOrder is in its correct position. Sets IsStaging
		/// property, and also sets IsAtFormationPositionFlag for all relevant units in group.
		/// </summary>
		public void CheckIfGroupIsStaging()
		{
            if (IsMarkedForDeletion)
            {
                return;
            }

			if (MainUnit != null
				&& MainUnit.UnitClass.IsAircraft
				&& MainUnit.TargetDetectedUnit != null)
			{
				//ignore staging if aircraft is attacking
				IsStaging = false;
				return;
			}

            foreach (var unit in _units)
			{
				if (unit.Position == null || unit.CarriedByUnit != null)
				{
					IsStaging = true;
					return;
				}
				if (!unit.IsGroupMainUnit() && unit.MovementOrder is MovementFormationOrder && !unit.IsAtFormationPositionFlag)
				{
					IsStaging = true;
					return;
				}
			}

			IsStaging = false;
		}

        public override bool Equals(object obj)
        {
            if (obj is Group)
            {
                return (obj as Group).Id == this.Id;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            try
            {
                return (int)GameManager.GetNumericId(this.Id);
            }
            catch (Exception)
            {
                return base.GetHashCode();
            }
        }

		public GroupInfo GetGroupInfo()
		{
			var info = new GroupInfo();
			info.Id = Id;
			if(Formation != null)
			{
				info.FormationId = Formation.Id;
			}
			if(MainUnit != null)
			{
				info.MainUnitId = MainUnit.Id;
			}
            info.UnitCount = _units.Count;
			info.Name = Name;
            if (_units.Count > 0)
			{
                info.MaxWeaponRangeAirM = _units.Max(u => u.GetMaxWeaponRangeM(GameConstants.DomainType.Air));
                info.MaxWeaponRangeLandM = _units.Max(u => u.GetMaxWeaponRangeM(GameConstants.DomainType.Land));
                info.MaxWeaponRangeSubM = _units.Max(u => u.GetMaxWeaponRangeM(GameConstants.DomainType.Subsea));
                info.MaxWeaponRangeSurfaceM = _units.Max(u => u.GetMaxWeaponRangeM(GameConstants.DomainType.Surface));
			}
			return info;
		}

		public string ToLongString()
		{
			if (MainUnit != null)
			{
				return string.Format("Group [{0}] {1} lead by {2} ({3} units)",
					Id, Name, MainUnit.ToString(), _units.Count);
			}
			else
			{
				return "Group " + Id + " " + Name;
			}
		}

		public override string ToString()
		{
			return Id + " " + Name;
		}

		#endregion
	}
}
