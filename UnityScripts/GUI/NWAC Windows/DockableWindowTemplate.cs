using UnityEngine;
using System.Collections;

public class DockableWindowTemplate : DockableWindow {

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			
		}
		else
		{
			GUILayout.Label("Not Connected");
		}

		
	}
}
