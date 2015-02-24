using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class WeaponWindow : DockableWindow {

	
	private WeaponInfo _SelectedWeapon;
	
	public GUIStyle Separator;

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(_SelectedUnit != null)
		{
			foreach(WeaponInfo wi in _SelectedUnit.Info.Weapons)
			{
				WeaponClass wc = GameManager.Instance.GetWeaponClass(wi.WeaponClassId);
				if(wc != null)
				{
					if(GUILayout.Button(wc.WeaponClassName))
					{
						_SelectedWeapon = wi;
					}
				}
					//~ else if(_SelectedWeapon != null)
					//~ {
						//~ if(_SelectedWeapon.Id == wi.Id)
						//~ {
							//~ if(_SelectedWeapon.IsActive != wi.IsActive)
							//~ {
								//~ _SelectedWeapon = wi;
							//~ }
						//~ }
					//~ }
				//~ }
			}
			
			if(_SelectedWeapon != null)
			{
				Separator.fixedWidth = WindowRect.width - this.PaddingLeft * 2;
				
				GUILayout.Label("", Separator);
				
				GUILayout.BeginVertical();
				
				GUILayout.Label(_SelectedWeapon.ToString());
				
				GUILayout.Label(string.Format("Ready in : {0} seconds", _SelectedWeapon.ReadyInSec));
				
				GUILayout.EndVertical();
			}
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
	}
}
