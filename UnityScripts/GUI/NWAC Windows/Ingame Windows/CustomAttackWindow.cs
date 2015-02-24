using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class CustomAttackWindow : DockableWindow {

	
	private WeaponInfo _SelectedWeapon;
	
	private DetectedUnitInfo _Target;
	
	private float _RoundCount = 10;
	
	private int _EngagementOrderType;
	
	public GUIStyle EngagementOrderStyle;
	
	public WeaponInfo SelectedWeapon
	{
		set
		{
			_SelectedWeapon = value;
		}
	}
	
	public DetectedUnitInfo Target
	{
		get
		{
			return _Target;
		}
		set
		{
			_Target = value;
		}
	}
	
	public GUIStyle Separator;
	
	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(_SelectedUnit != null)
		{
			Separator.fixedWidth = WindowRect.width - this.PaddingLeft * 2;
			if(Target != null)
			{
				GUILayout.Label(string.Format("Target : {0} at Position : {1}", Target.RefersToUnitName, Target.Position.ToString()));
				GUILayout.Label("", Separator);
			}
			GUILayout.Label("Choose weapon:");
			
			foreach(WeaponInfo wi in _SelectedUnit.Info.Weapons)
			{
				WeaponClass wc = GameManager.Instance.GetWeaponClass(wi.WeaponClassId);
				if(wc != null)
				{
					if(GUILayout.Button(wc.WeaponClassName))
					{
						_SelectedWeapon = wi;
						_RoundCount = 10;
					}
				}
			}
			
			if(_SelectedWeapon != null)
			{
				GUILayout.Label("", Separator);
				
				WeaponClass wc = GameManager.Instance.GetWeaponClass(_SelectedWeapon.WeaponClassId);
				GUILayout.Label(string.Format("Selected weapon : {0}. Weapon ready in {1} sec", wc.WeaponClassName, _SelectedWeapon.ReadyInSec));
				GUILayout.Label(string.Format("Round count : {0}.", Mathf.Floor(_RoundCount)));
				
				
				GUILayout.BeginVertical();
				
				_RoundCount = GUILayout.HorizontalSlider(_RoundCount, 1, _SelectedWeapon.AmmunitionRemaining);
				_EngagementOrderType = GUILayout.SelectionGrid(_EngagementOrderType, GameConstants.EngagementOrderType.GetNames(typeof(GameConstants.EngagementOrderType)), 3, EngagementOrderStyle);
				
				string eName = GameConstants.EngagementOrderType.GetName( typeof(GameConstants.EngagementOrderType), _EngagementOrderType).ToString();
				GUILayout.Label(eName);
				
				if(GUILayout.Button("Give order"))
				{
					//ATTACK
					
					GameConstants.EngagementOrderType type = (GameConstants.EngagementOrderType)GameConstants.EngagementOrderType.Parse(typeof(GameConstants.EngagementOrderType), eName);
					GameManager.Instance.OrderManager.Attack(_SelectedUnit.Info, Target, type, 
																					_SelectedWeapon.WeaponClassId, (int)_RoundCount);
					this.enabled = false;
				}
				
				
				
				GUILayout.EndVertical();
			}
		}
		else
		{
			GUILayout.Label("Not Connected");
		}

		
	}
}