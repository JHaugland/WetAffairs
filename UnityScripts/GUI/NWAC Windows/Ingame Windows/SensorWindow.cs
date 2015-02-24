using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;


public class SensorWindow: DockableWindow {
	
	private Vector2 _SensorsScrollPosition;
	private Vector2 _SensorInformationPosition;
	
	private SensorInfo _SelectedSensor;
	
	public GUIStyle Separator;

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		
		if(_SelectedUnit != null)
		{
			
			foreach(SensorInfo si in _SelectedUnit.Info.Sensors)
			{
				SensorClass sc = GameManager.Instance.GetSensorClass(si.SensorClassId);
				if(sc != null && sc.IsPassiveActiveSensor)
				{
					if(GUILayout.Button(si.Name))
					{
						_SelectedSensor = si;
					}
					else if(_SelectedSensor != null)
					{
						if(_SelectedSensor.Id == si.Id)
						{
							if(_SelectedSensor.IsActive != si.IsActive)
							{
								_SelectedSensor = si;
							}
						}
						//~ _SelectedSensor = si;
					}
				}
			}
			
			if(_SelectedSensor != null)
			{
				Separator.fixedWidth = WindowRect.width - this.PaddingLeft * 2;
				
				GUILayout.Label("", Separator);
				
				GUILayout.BeginVertical();
				
				GUILayout.Label(string.Format("Status: {0}", _SelectedSensor.IsOperational == true ? "Operational" : string.Format("Ready in : {0} seconds", _SelectedSensor.ReadyInSec)));
				
				if(GUILayout.Button(_SelectedSensor.IsActive == true ? "Set passive" : "Set active"))
				{
					GameManager.Instance.OrderManager.SetSensorPassiveActive(_SelectedUnit.Info, _SelectedSensor, !_SelectedSensor.IsActive);
				}
				
				GUILayout.Label(_SelectedSensor.ToString());
				
				GUILayout.EndVertical();
			}
		}
		else
		{
			GUILayout.Label("Not Connected");
		}

		
	}
}
