using UnityEngine;
using System.Collections;

public class CameraWindow : DockableWindow {

	public RenderTexture SurfaceTex;
	public RenderTexture SateliteTex;
	

	
	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			Event e  = Event.current;
			
			switch(e.GetTypeForControl(WindowId))
			{
				case EventType.ScrollWheel:
				{
					e.Use();
					break;
				}
			}
			
            //GUI.Label(new Rect(this.PaddingLeft, this.PaddingTop, this.WindowRect.width, this.WindowRect.height - this.PaddingTop * 2), GameManager.Instance.GameState == GameManager.GameStates.Satelite ? SurfaceTex : SateliteTex );
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
		
		
	}
}
