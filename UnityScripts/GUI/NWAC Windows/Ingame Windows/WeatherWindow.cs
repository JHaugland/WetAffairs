using UnityEngine;
using System.Collections;

public class WeatherWindow : DockableWindow {

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			GUILayout.Label("Weather reports from selected vessel");
			GUILayout.Space(5);
			GUILayout.Label(GameManager.Instance.UnitManager.SelectedUnit.Info.WeatherSystem.ToString());
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
	}
}
