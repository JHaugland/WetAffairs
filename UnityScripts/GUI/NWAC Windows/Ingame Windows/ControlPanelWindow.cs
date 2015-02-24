using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPanelWindow : DockableWindow {

	public Component[] Windows;
	private List<Component> MySelection;
	public Rect IdleRect;
	public Rect ActiveRect;
	public float Height;
	public float Width;
	public bool Expanded = false;
	private bool Hidden = false;
	public ScreenPosition Position ;//= ScreenPosition.Bottom;
	public GUIStyle ScreenPositionStyle;
	public float PopupSpeed = 1.0f;
	public float PopupStayTimer = 3.0f;
	private float TimeSinceMouseExit = 0.0f;
	
	public enum ScreenPosition
	{
		Top = 0,
		Left = 1,
		Right = 2,
		Bottom = 3
	}
	
	
	void Start()
	{
		//~ SetPositionByValue(0);
		UpdatePosition();
		
		WindowRect = IdleRect;
		Windows = gameObject.GetComponents(typeof(DockableWindow));
		
		MySelection = new List<Component>();
		
		foreach(DockableWindow w in Windows)
		{
			if(w is ControlPanelWindow)
			{
				continue;
			}
			
			MySelection.Add(w);
		}
	}
	
	void Update()
	{
		if(Expanded)
		{
			TimeSinceMouseExit += Time.deltaTime;
			
			if(TimeSinceMouseExit >= PopupStayTimer)
			{
				Expanded = false;
				TimeSinceMouseExit = 0;
			}
		}
	}
	
	private void UpdatePosition()
	{
		switch(Position)
		{
			case ScreenPosition.Top:
				ActiveRect = new Rect(0, 0, Screen.width, Height);
				IdleRect = new Rect(0, -Height, Screen.width, Height);
				break;
			case ScreenPosition.Left:
				ActiveRect = new Rect(0, 0, Width, Screen.height);
				IdleRect = new Rect(-Width, 0, Width, Screen.height);
				break;
			case ScreenPosition.Right:
				ActiveRect = new Rect(Screen.width - Width, 0, Width, Screen.height);
				IdleRect = new Rect(Screen.width, 0, Width, Screen.height);
				break;
			case ScreenPosition.Bottom:
				ActiveRect = new Rect(0, Screen.height - Height, Screen.width, Height);
				IdleRect = new Rect(0, Screen.height, Screen.width, Height);
				break;
		}
		//~ WindowRect = IdleRect;
	}
	
	private void SetPositionByValue(int value)
	{
		Position = (ScreenPosition)ScreenPosition.ToObject(typeof(ScreenPosition), value);
		
		//~ Debug.Log(string.Format("value: {0} , Enum : {1}", value, Position));
		UpdatePosition();
	}
	
	public override void DockableWindowFunc(int id)
	{
		//~ Debug.Log("Test");
		if(Windows != null)
		{
			
			GUILayout.BeginArea(new Rect(0, 0, WindowRect.width, WindowRect.height));
			
			switch(Position)
			{
				case ScreenPosition.Top:
					GUILayout.BeginHorizontal();
					break;
				case ScreenPosition.Left:
					GUILayout.BeginVertical();
					break;
				case ScreenPosition.Right:
					GUILayout.BeginVertical();
					break;
				case ScreenPosition.Bottom:
					GUILayout.BeginHorizontal();
					break;
			}
			
			
			foreach(DockableWindow dWindow in Windows)
			{
				if(dWindow is ControlPanelWindow)
				{
					continue;
				}
				
				if(GUILayout.Button(dWindow.Content.text))
				{
					if(Hidden)
					{
						Hidden = false;
					}
					dWindow.enabled = !dWindow.enabled;
				}
				
			}
			
			switch(Position)
			{
				case ScreenPosition.Top:
					GUILayout.EndHorizontal();
					break;
				case ScreenPosition.Left:
					GUILayout.EndVertical();
					break;
				case ScreenPosition.Right:
					GUILayout.EndVertical();
					break;
				case ScreenPosition.Bottom:
					GUILayout.EndHorizontal();
					break;
			}
			
			GUILayout.Space(10);
		
			
			GUILayout.BeginVertical();
		
			if(GUILayout.Button(Hidden == true ? "Show Selection" : "Hide Selection"))
			{
				if(!Hidden)
				{
					MySelection.Clear();
					foreach(DockableWindow dWindow in Windows)
					{
						if(dWindow is ControlPanelWindow)
						{
							continue;
						}
						
						if(dWindow.enabled)
						{
							MySelection.Add(dWindow);
							dWindow.enabled = false;
						}
					}
					
					
				}
				else
				{
					foreach(DockableWindow dWindow in MySelection)
					{
						if(dWindow is ControlPanelWindow)
						{
							continue;
						}
						
						dWindow.enabled = true;
						
					}
				}
				
				Hidden = !Hidden;
			}
			
			int selected = GUILayout.SelectionGrid(System.Convert.ToInt32(ScreenPosition.Format(typeof(ScreenPosition), Position, "d")), ScreenPosition.GetNames(typeof(ScreenPosition)), 2, ScreenPositionStyle);
			 
			if(GUI.changed)
			{
				SetPositionByValue(selected);
				Expanded = false;
			}
			
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
		}
		
		Vector2 fromPos = new Vector2(WindowRect.x, WindowRect.y);
		Vector2 fromSize = new Vector2(WindowRect.width, WindowRect.height); 
		
		Vector2 toPos = Vector2.zero;
		Vector2 toSize = Vector2.zero;
		//~ Debug.Log(e.mousePosition);
		
		Vector2 mousePosition = Input.mousePosition;
		mousePosition.y = Screen.height - mousePosition.y;
		
		//~ Debug.Log(ActiveRect.Contains(mousePosition));
		
		if(ActiveRect.Contains(mousePosition))
		{
			
			toPos = new Vector2(ActiveRect.x, ActiveRect.y);
			toSize = new Vector2(ActiveRect.width, ActiveRect.height);
			
			
		}
		else
		{
			
			
			toPos = new Vector2(IdleRect.x, IdleRect.y);
			toSize = new Vector2(IdleRect.width, IdleRect.height);
		}
		
		
		
		fromPos =  MathHelper.Vector2Lerp(fromPos, toPos, Time.deltaTime * PopupSpeed);
		fromSize = MathHelper.Vector2Lerp(fromSize, toSize, Time.deltaTime * PopupSpeed);
			
		if(Vector2.Distance(toPos, fromPos) < 10 && ActiveRect.Contains(mousePosition))
		{
			Expanded = true;
		}
		
		
		if(!Expanded)
		{
			WindowRect = new Rect(fromPos.x, fromPos.y, fromSize.x, fromSize.y);
		}
		
		
		

		//~ DoResizing();
	}
}
