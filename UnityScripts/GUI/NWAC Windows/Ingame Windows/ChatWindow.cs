using UnityEngine;
using System.Collections;

public class ChatWindow : DockableWindow {

	private Vector2 _ScrollPosition;
	private string _Message = "";
	
	public override void DockableWindowFunc(int id)
	{
		DoResizing();
		GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		if(GameManager.Instance.UnitManager.SelectedUnit != null)
		{
			GUILayout.BeginArea(new Rect(0, 0, WindowRect.width, WindowRect.height - 20));
			_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition);
			GUILayout.BeginVertical();
			foreach(Message m in GameManager.Instance.MessageManager.GetMessagesByType(GameManager.MessageTypes.Chat))
			{
				GUILayout.Label(m.Body, GameManager.Instance.GUIManager.GetMessageStyleByType(GameManager.MessageTypes.Chat, m.IsRead, -1));
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			
			GUILayout.BeginHorizontal();
			_Message = GUILayout.TextField(_Message);
			if(GUILayout.Button("Send"))
			{
				//Send message
				_Message = "";
			}
			GUILayout.EndHorizontal();
			
			//~ GUILayout
		}
		else
		{
			GUILayout.Label("Not Connected");
		}

	}
	
	private void Send()
	{
		//Send message
		
		//Clear textfield
		_Message = "";
	}
}
