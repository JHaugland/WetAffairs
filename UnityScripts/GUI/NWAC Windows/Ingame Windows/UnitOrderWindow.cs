using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

public class UnitOrderWindow : DockableWindow {

	private Vector2 _ScrollPosition;
	
	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, GUILayout.Width(WindowRect.width - (this.PaddingLeft * 2)), GUILayout.Height(1000));
		
			if(GUILayout.Button("Move!"))
			{
				//~ GameManager.Instance.OrderManager.Move(_Lat, _Lng, _Info);
			}
			
			if(GUILayout.Button("Launch Chopper"))
			{
				GameManager.Instance.OrderManager.Launch(GameManager.Instance.UnitManager.SelectedUnit.Info, GameConstants.UnitOrderType.LaunchAircraft);
			}
			
			
            //if(GUILayout.Button(GameManager.Instance.GameState == GameManager.GameStates.Satelite ? "Go to Unit" : "Go To Map"))						
            //{
            //    GameManager.Instance.CameraManager.SwitchCamera(GameManager.Instance.GameState == GameManager.GameStates.Satelite ? GameManager.Instance.CameraManager.MainCamera : GameManager.Instance.CameraManager.SateliteCamera);
            //}
			
			if(GUILayout.Button("Weapons"))						
			{
				GameManager.Instance.UnitManager.SelectedUnit.UnitGUIState = GameManager.GUIState.Weapons;
					
			}
			if(GUILayout.Button("Sensors"))						
			{
				GameManager.Instance.UnitManager.SelectedUnit.UnitGUIState = GameManager.GUIState.Sensors;
					
			}
			if(GUILayout.Button("Carried Units"))						
			{
				GameManager.Instance.UnitManager.SelectedUnit.UnitGUIState = GameManager.GUIState.CarriedUnits;
					
			}
		
		GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label("Not connected");
		}
	}
}
