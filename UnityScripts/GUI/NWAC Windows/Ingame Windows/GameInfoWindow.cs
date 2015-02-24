using UnityEngine;
using System.Collections;

public class GameInfoWindow : DockableWindow {

	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		if(GameManager.Instance.GameInfo != null)
		{
			
			GUILayout.Label(GameManager.Instance.GameInfo.ToString());
		}
		else
		{
			GUILayout.Label("Not Connected");
		}


	}
}
