using UnityEngine;
using System.Collections;

public class UnitInfoWindow : DockableWindow {

	private string _Lat = "";
	private string _Lng = "";
	
	private Vector2 _ScrollPosition = new Vector2(0, 0);
	public float ItemSize = 100;
	public GUIStyle ButtonAsLabel;
	



	public override void DockableWindowFunc(int id)
	{
		
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			PlayerUnit selected = GameManager.Instance.UnitManager.SelectedUnit;
			 
			string[] unitInfo = {"Unit Class Name: " + selected.Info.UnitClassId, "Hit Points: " + selected.Info.HitPoints.ToString(),
											"Ready In Sec: " + selected.Info.ReadyInSec.ToString(), string.Format("BearingDeg: {0}", selected.Info.Position.BearingDeg),
											string.Format("Speed: {0}", selected.Info.ActualSpeedKph), string.Format("Position: {0}", selected.Info.Position.ToString()),
											string.Format("Destination: {0}", selected.Info.Destination.ToString())};
			GUILayout.BeginArea(new Rect(PaddingLeft, PaddingTop, WindowRect.width - PaddingLeft * 2, WindowRect.height - PaddingTop * 2));
			
											//~ GUILayout.Button("Test1");
											//~ GUILayout.Button("Test2");
											//~ GUILayout.Button("Test3");
											//~ GUILayout.Button("Test4");
											//~ GUILayout.Button("Test5");
											//~ GUILayout.Button("Test6");
		
			
			_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, GUILayout.Width(WindowRect.width - PaddingLeft * 2), GUILayout.Height(WindowRect.height - PaddingTop * 2 ));

			GUILayout.Label("Name: " + selected.Info.UnitName, ButtonAsLabel);
			
			int xCount = (int)(WindowRect.width / ItemSize);
			
			
			GUILayout.SelectionGrid(0, unitInfo, Mathf.Clamp(xCount, 1, unitInfo.Length),  ButtonAsLabel);
			
			GUILayout.EndScrollView();
			
			GUILayout.EndArea();
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
		
		
		
	
	}
	
	
}
