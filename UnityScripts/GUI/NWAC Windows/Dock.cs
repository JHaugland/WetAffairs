using UnityEngine;
using System.Collections;

public class Dock{

	private Rect _Rect;
	private bool _IsIntersecting;
	private bool _IsClosest;
	private DockableWindow _WindowInDock;
	
	public Rect Rect
	{
		get
		{
			return _Rect;
		}
		set
		{
			_Rect = value;
		}
	}
	
	public bool IsIntersecting
	{
		get
		{
			return _IsIntersecting;
		}
		set
		{
			_IsIntersecting = value;
		}
	}
	
	public bool IsClosest
	{
		get
		{
			return _IsClosest;
		}
		set
		{
			_IsClosest = value;
		}
	}
	
	public DockableWindow WindowInDock
	{
		get
		{
			return _WindowInDock;
		}
		set
		{
			_WindowInDock = value;
		}
	}
	
	public Dock()
	{
		Rect = new Rect(0, 0, 100, 100);
		_IsIntersecting = false;
	}
	
	public Dock(Rect dockPos)
	{
		Rect = dockPos;
		_IsIntersecting = false;
	}
	
	public void Update()
	{
		if(WindowInDock != null)
		{
			//~ Debug.Log(WindowInDock);
			if(Rect != WindowInDock.WindowRect)
			{
				WindowInDock = null;
				//~ Debug.Log("resetting dock");
			}
		}
	}
	
	
}
