using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameMessageWindow: DockableWindow {

	private Vector2 _ScrollPosition = new Vector2(0, 0);
	private Vector2 _MessageScrollPosition = new Vector2(0,0);
	private bool _Expanded = false;
	private string _MessageToDisplay = "";
	public float ItemSize = 600;
	public float MessageSize = 20;
	public float ChannelButtonSize = 150;
	public GUIStyle ButtonAsLabel;

    public bool SetBottomCenter = true;
	
	public GUIStyle BattleChannelStyle;
	public GUIStyle DetectionChannelStyle;
	public GUIStyle GameChannelStyle;
	public GUIStyle AllChannelStyle;
	public GUIStyle ActiveChannelStyle;
	
	public GUIStyle Separator;
	
	public Channels CurrentChannel = Channels.All;
    public Camera SurfaceCamera;
	
	public enum Channels
	{
		Battle,
		Detection,
		Game,
		All
		
	}

    void Start()
    {
        //if (SetBottomCenter)
        //{
        //    WindowRect = new Rect(Screen.width / 2 - WindowRect.width / 2, Screen.height - WindowRect.height, WindowRect.width, WindowRect.height);
        //}
        WindowRect = MathHelper.ViewportRectToScreenRect(new Rect(SurfaceCamera.rect.width, 0.8f, 0.5f, 0.2f)); 
        Separator.fixedHeight = 2;
    }
	
	
	public override void DockableWindowFunc(int id)
	{
        //DoResizing();
        //GUILayout.Space(GameManager.Instance.GUIManager.MinimizeButtonRect.height);
		//TODO: FIX Scrollview problem.
		Separator.fixedWidth = WindowRect.width - this.PaddingLeft * 2;
		this.Content = new GUIContent(string.Format("Channel - {0}", CurrentChannel.ToString()));
		
		
		int battleMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Battle, true);
		int detectionMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Detection, true);
		int gameMessageCount = GameManager.Instance.MessageManager.GetMessageTypeCount(GameManager.MessageTypes.Game, true);
		
		//~ Debug.Log(gameMessageCount);
        GUILayout.BeginHorizontal();
		if(GUILayout.Button(battleMessageCount > 0 ? string.Format("Battle ({0})", battleMessageCount) : "Battle", CurrentChannel == Channels.Battle ? ActiveChannelStyle : BattleChannelStyle)) { CurrentChannel = Channels.Battle; }
		if(GUILayout.Button(detectionMessageCount > 0 ? string.Format("Detection ({0})", detectionMessageCount) : "Detection", CurrentChannel == Channels.Detection ? ActiveChannelStyle : DetectionChannelStyle)) { CurrentChannel = Channels.Detection; }
		if(GUILayout.Button(gameMessageCount > 0 ? string.Format("Game ({0})", gameMessageCount) : "Game", CurrentChannel == Channels.Game ? ActiveChannelStyle : GameChannelStyle)) { CurrentChannel = Channels.Game; }
		
		if(GUILayout.Button("All", CurrentChannel == Channels.All ? ActiveChannelStyle : AllChannelStyle)) { CurrentChannel = Channels.All; }
		
		GUILayout.EndHorizontal();
		
		
		
		if(GameManager.Instance.NetworkManager.Connected)
		{

			float windowHeight = _Expanded == true ? WindowRect.height - this.PaddingTop * 2 : WindowRect.height - this.PaddingTop * 4 ;
			_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, GUILayout.Width(WindowRect.width - this.PaddingLeft * 2), _Expanded == true ? GUILayout.Height(windowHeight / 2) : GUILayout.Height(WindowRect.height / 2) );
			List<Message> Messages = new List<Message>();
			switch(CurrentChannel)
			{
				case Channels.Battle:
					Messages = GameManager.Instance.MessageManager.GetMessagesByType(GameManager.MessageTypes.Battle);
                    break;
                case Channels.Detection:
					Messages = GameManager.Instance.MessageManager.GetMessagesByType(GameManager.MessageTypes.Detection);
                    break;
				case Channels.Game:
					Messages = GameManager.Instance.MessageManager.GetMessagesByType(GameManager.MessageTypes.Game);
                    break;
                case Channels.All:
					Messages = GameManager.Instance.MessageManager.AllMessages;
                    break;
                default:
                    break;
			}
            List<Message> ForDeletion = new List<Message>();
			foreach(Message m in Messages)
			{
                GUILayout.BeginHorizontal();
				if(GUILayout.Button(m.ToString(), m.Style, GUILayout.Width(WindowRect.width - 100)))
				{
					_Expanded = m.HasBody;
					_MessageToDisplay = m.Body;				
					m.IsRead = true;
				}

                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    ForDeletion.Add(m);
                }
                GUILayout.EndHorizontal();
			}

            foreach (Message m in ForDeletion)
            {
                GameManager.Instance.MessageManager.RemoveMessage(m);
            }
			
			GUILayout.EndScrollView();
			
			if(_Expanded)
			{
                //GUILayout.Space(10);
				GUILayout.Label("", Separator);
				_MessageScrollPosition = GUILayout.BeginScrollView(_MessageScrollPosition, GUILayout.Width(WindowRect.width - this.PaddingLeft * 2), _Expanded == true ? GUILayout.Height(windowHeight / 2) : GUILayout.Height(windowHeight) );
				GUILayout.Label(_MessageToDisplay);
				GUILayout.EndScrollView();
			}
		}
		else
		{
			GUILayout.Label("Not Connected");
		}
	
		DoResizing();
	}
}
