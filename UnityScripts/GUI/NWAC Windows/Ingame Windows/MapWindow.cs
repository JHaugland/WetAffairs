using UnityEngine;
using System.Collections;

public class MapWindow : DockableWindow {

	public Texture2D MapTexture;
	
	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			GUILayout.Label(MapTexture, GUILayout.Width(WindowRect.width) , GUILayout.Height(WindowRect.height));
		}
		else
		{
			GUILayout.Label("Not Connected");
		}

	}
}
