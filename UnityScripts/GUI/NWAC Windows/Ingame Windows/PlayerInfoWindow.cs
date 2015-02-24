using UnityEngine;
using System.Collections;

public class PlayerInfoWindow : DockableWindow{

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			if(GameManager.Instance.GameInfo != null)
			{
				GUILayout.Label(GameManager.Instance.GameInfo.GameCurrentTime.GetDateTime().ToString());
			}
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
	}
}
