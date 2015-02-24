using UnityEngine;
using System.Collections;

public class PopupWindow : MonoBehaviour {

	private Message _MessageToDisplay;
	private int _WindowId = Random.Range(1000, 9999);
	
	private float _LifeTime = 0.0f;
	private float _FadeTime = 0.0f;
	private float _KillTime = 0.0f;
	
	private Color _Color;
	private Color _TransparentColor = new Color(1, 1, 1, 0);
	
	private GUIStyle _Style;
	private Vector2 _ScrollPosition = new Vector2(0,0);
	
	private Rect Position;
	
	
	public Message MessageToDisplay
	{
		get
		{
			return _MessageToDisplay;
		}
		set
		{
			_MessageToDisplay = value;
		}
	}
	
	void OnGUI()
	{
		if(MessageToDisplay != null)
		{
            GUI.depth = -1000;
			//TODO: Maybe add link functionality
			
			_Color = _KillTime - _LifeTime < _FadeTime ?  Color.Lerp(_Color, _TransparentColor, Time.deltaTime) : _Color;
			GUI.color = _Color;
			//~ Debug.Log(guiColor.a);
			//~ Debug.Log(_Color);
			Position = GUILayout.Window(_WindowId, Position, WindowFunc, "");
			GUI.color = Color.white;
		}
	}
	
	void WindowFunc(int windowId)
	{
		GUILayout.BeginVertical();
		_ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, GUILayout.Width(Position.width), GUILayout.Height(Position.height - 20));
		GUILayout.Label(MessageToDisplay.ToString());
		GUILayout.EndScrollView();
		if(GUILayout.Button("Close"))
		{
			Kill();
		}
		
		GUI.DragWindow();
	}
	
	void Start()
	{
		Position = GameManager.Instance.GUIManager.PopupWindowPosition;
		_KillTime = GameManager.Instance.GUIManager.PopupWindowTimer;
		_FadeTime = GameManager.Instance.GUIManager.PopupFadeTimer;
		_Style = GameManager.Instance.GUIManager.PopupWindowStyle;
		
		//~ _Color = Color.white;
		
		if(MessageToDisplay != null)
		{
			_Color = MessageToDisplay.Style.normal.textColor;
			_TransparentColor = MessageToDisplay.Style.normal.textColor;
			_TransparentColor.a = 0;
		}
		
		//~ Debug.Log(_Color);
		//~ Debug.Log(_TransparentColor);
	}
	
	// Update is called once per frame
	void Update () {
		
		_LifeTime += Time.deltaTime;
		
		if(_LifeTime >= _KillTime)
		{
			Kill();
		}
	}
	
	void Kill()
	{
		Destroy(gameObject);
		
	}
}
