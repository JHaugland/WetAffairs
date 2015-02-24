using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DockableWindow : MonoBehaviour {

	
	public int WindowId = Random.Range(1, 1000);
	
	public Rect WindowRect = new Rect(0, 0, 100, 100);
	
	public Rect[] ResizeAreas;
	
	public Rect DraggableArea = new Rect(0, 0, 1000, 20);
	public Rect MinWindowRect = new Rect(0, 0, 100, 100);
	
	public bool UseScreenWidth = false;
	public bool UseScreenHeight = false;
	public bool Draggable = true;
	public bool Dockable = false;
	public bool IsQuadrant = false;
	public bool IgnoreScreenEdge = false;
	
	public float PaddingLeft = 5;
	public float PaddingTop = 5;
	public float MaxWidth = 512;
	
	public Vector2 ResizerSize = new Vector2(20, 20);
	
	public GUIContent Content ;
	public GUIContent DockToggle = new GUIContent("Dockable");

	public GUISkin skin;
	
	protected PlayerUnit _SelectedUnit;
    private bool _SelectedUnitChanged = false;
	
	public enum ResizeArea
	{
		Top,
		Left,
		Right,
		Bottom
	}

    public bool SelectedUnitChanged
    {
        get
        {
            return _SelectedUnitChanged;
        }
        set
        {
            _SelectedUnitChanged = value;
        }
    }

    public PlayerUnit SelectedUnit
    {
        get
        {
            _SelectedUnitChanged = false;
            return _SelectedUnit;
        }
    }
	

	
	private Vector2 _ChangeSizeValue;
	private bool _CanResize;
	private bool _MenuShowing;
	private bool _ResizeChange;
	private bool _SetToDefault;
	private bool _Dock;
	private Rect _OriginalPos;
	private Rect _ResizePos;
	private Rect _DockableTogglePos;
	private Rect _DockPos;
	
	 
	void Start()
	{
		if(UseScreenHeight)
		{
			WindowRect.height = Screen.height;
		}
		_OriginalPos = WindowRect;
		
		
		
	}
	
	void OnGUI()
	{
		if(skin != null)
		{
			GUI.skin = skin;
		}
		if(_ResizeChange)
		{
			WindowRect.width += _ChangeSizeValue.x;
			WindowRect.height += _ChangeSizeValue.y;
			_ResizeChange = false;
		}
		if(_SetToDefault)
		{
			WindowRect = _OriginalPos;
			_SetToDefault = false;
		}
		else if(_Dock)
		{
			WindowRect = _DockPos;
			_Dock = false;
		}
		if(Draggable)
		{
			DraggableArea.width = WindowRect.width;
		}
		
		if(_MenuShowing)
		{
			
		}
		
		_ResizePos = new Rect(WindowRect.width - ResizerSize.x, WindowRect.height - ResizerSize.y,
													ResizerSize.x, ResizerSize.y);
		_DockableTogglePos = new Rect(40, 20, 40, 40);
		
		
		
		if(IgnoreScreenEdge)
		{
			WindowRect = GUI.Window(WindowId, WindowRect, DockableWindowFunc, Content);
			
		}
		else
		{
			WindowRect = ClampRectToScreen(GUI.Window(WindowId, WindowRect, DockableWindowFunc, Content));
		}
		
	}
	
	public virtual void DockableWindowFunc(int id)
	{
		DoResizing();
	}
	
	public void DoResizing()
	{
		if(IsQuadrant)
		{
			float high = WindowRect.width >= WindowRect.height ? WindowRect.width : WindowRect.height;
			WindowRect.width = WindowRect.height = Mathf.Clamp(high, high, MaxWidth);
		}
		
		if(GUI.Button(GameManager.Instance.GUIManager.MinimizeButtonRect, GameManager.Instance.GUIManager.MinimizeButtonTexture, GameManager.Instance.GUIManager.MinimizeStyle))
		{
			this.enabled = false;
		}
		
        //Dockable = GUI.Toggle(GameManager.Instance.GUIManager.DockToggleRect, Dockable, DockToggle, Dockable == true ? GameManager.Instance.GUIManager.DockStyleToggleOn : GameManager.Instance.GUIManager.DockStyleToggleOff);
		
		Event e = Event.current;
		bool canDrag = Draggable;
		Vector2 resizeAmount = e.delta;

		
		switch(e.GetTypeForControl(WindowId))
		{
			 case EventType.MouseDrag:
				{
					if(Dockable)
					{
						GameManager.Instance.GUIManager.IsWindowDragging = true;
						foreach(Dock dock in GameManager.Instance.GUIManager.DockingStations)
						{
							dock.IsIntersecting = GameManager.Instance.GUIManager.IntersectRect(WindowRect, dock.Rect);
						}
						List<Dock> intersectedRects = GameManager.Instance.GUIManager.DockingStations.FindAll(delegate(Dock c){ return c.IsIntersecting == true ;});
						if(intersectedRects.Count > 0)
						{
							float leastAreal = 0;
							Dock closest = new Dock();
							bool changed = false;
							foreach(Dock dock in intersectedRects)
							{
								dock.IsClosest = false;
								if(dock.WindowInDock != null)
								{
									continue;
								}
								Rect intersect = GameManager.Instance.GUIManager.FindIntersectingRect(WindowRect, dock.Rect);
								//~ Debug.Log(intersect);
								float areal = intersect.width * intersect.height;
								//~ Debug.Log(areal);
								if(areal > leastAreal)
								{
									leastAreal = areal;
									closest = dock;
									//~ GameManager.Instance.GUIManager.IntersectingRect = intersect;
								}
							}

							closest.IsClosest = true;
							
						}
					}
					
					break;
				}
				case EventType.MouseDown:
					if(_ResizePos.Contains(e.mousePosition))
					{
						_CanResize = true;
						resizeAmount = Vector2.zero;
						Dockable = false;
					}
				
					break;
					
				case EventType.ContextClick:
				{
					_MenuShowing = !_MenuShowing;
					break;
				}
				case EventType.MouseUp:
					GameManager.Instance.GUIManager.IsWindowDragging = false;
					//If window is inside a dock
					if(Dockable && e.button == 0)
					{
						List<Dock> intersectedRects = GameManager.Instance.GUIManager.DockingStations.FindAll(delegate(Dock c){ return c.IsIntersecting == true; });
						
						if(intersectedRects.Count > 0)
						{
							float distance = 10000;
							Dock closest = new Dock(new Rect(0, 0, 1000, 1000));
							foreach(Dock dock in intersectedRects)
							{
								if(dock.IsClosest)
								{
									closest = dock;
								}
								
									
							}
							if(closest.Rect.width != 1000 && closest.Rect.height != 1000)
							{
								closest.WindowInDock = this;
								
								if(MinWindowRect.width > closest.Rect.width)
								{
									MinWindowRect.width = closest.Rect.width;
								}
								
								if(MinWindowRect.height > closest.Rect.height)
								{
									MinWindowRect.height = closest.Rect.height;
								}
								
								_DockPos = closest.Rect;
								
								//~ MinWindowRect = closest.Rect;
								_Dock = true;
							}
						}
					}
				
					break;
				
				case EventType.KeyUp:
					
					if(e.keyCode == KeyCode.Q)
					{
						_SetToDefault = true;
					}
					break;
				
				case EventType.Repaint:
				{
					GameManager.Instance.GUIManager.ResizeControl.Draw(_ResizePos, GUIContent.none, WindowId);
					//~ if(Dockable)
					//~ {
						//~ DockStyleToggleOn.Draw(_DockableTogglePos, GUIContent.none, WindowId);
					//~ }
					//~ else
					//~ {
						//~ DockStyleToggleOff.Draw(_DockableTogglePos, GUIContent.none, WindowId);
					//~ }
					break;
				}
		}
		
		
		
		if(!Input.GetMouseButton(0))
		{
			_CanResize = false;
		}
		
		if(_CanResize)
		{
			_ChangeSizeValue = resizeAmount;
			_ResizeChange = true;
			canDrag = false;
		}
		if(canDrag)
		{
			GUI.DragWindow(DraggableArea);
		}
	
	}

    public void OnSelectedUnitChanged(PlayerUnit unit)
	{
		_SelectedUnit = unit;
        _SelectedUnitChanged = true;
	}
	
	private Rect ClampRectToScreen(Rect rect)
	{
		return new Rect(Mathf.Clamp(rect.left, 0, Screen.width - rect.width), Mathf.Clamp(rect.top, 0, Screen.height - rect.height), Mathf.Clamp(rect.width, MinWindowRect.width, Screen.width), Mathf.Clamp(rect.height, MinWindowRect.height, Screen.height));
	}
	
	

	
}
