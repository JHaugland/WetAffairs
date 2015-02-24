using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;

public class DetectionMarker : MonoBehaviour {

	private Message _MessageToDisplay;
    private Camera _SateliteCamera;
	private int _WindowId = Random.Range(1000, 9999);
	
	private float _LifeTime = 0.0f;
	public float FadeTime = 0.0f;
	public float KillTime = 0.0f;
	
	private Color _Color;
	private Color _TransparentColor = new Color(1, 1, 1, 0);
	
	private GUIStyle _Style;
	private Vector2 _ScrollPosition = new Vector2(0,0);
	
    public Vector3 Position;
    public Vector2 MaxSize;
    public Vector2 MinSize;
    private Vector2 Size;

    private bool _Shrink;
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
            GUI.depth = 1000;
			//TODO: Maybe add link functionality
			bool doFade = KillTime - _LifeTime < FadeTime ;
			_Color = doFade == true ?  Color.Lerp(_Color, _TransparentColor, Time.deltaTime) : _Color;


            Size = Vector3.Lerp(Size, doFade == true ? MinSize : MaxSize, Time.deltaTime);
            

			GUI.color = _Color;
			//~ Debug.Log(guiColor.a);
			//~ Debug.Log(_Color);


            //Vector3 pos = new Vector3();
            //pos.x = ((float)Position.z * GameManager.Instance.XMapModifier) + GameManager.Instance.XMapAddition;
            //pos.z = ((float)Position.x * GameManager.Instance.YMapModifier) + GameManager.Instance.YMapAddtion;
            //pos.y = 30000;
            Vector3 _GUIScreenPos = _SateliteCamera.WorldToScreenPoint(Position);

            bool show = false;

            Rect shipPos = new Rect(_GUIScreenPos.x - Size.x / 2, _SateliteCamera.pixelHeight - _GUIScreenPos.y - Size.y / 2, Size.x, Size.x);

            if (GameManager.Instance.CameraManager.SateliteCamera.rect.height != 1)
            {
                shipPos.y += Screen.height / 2;
                
                if (shipPos.yMin > 0 && shipPos.xMax < Screen.height / 2)
                {
                    show = true;
                }
            }
            else
            {
                if (MathHelper.ViewportRectToScreenRect(_SateliteCamera.rect).Contains(new Vector2(shipPos.x, _SateliteCamera.pixelHeight - shipPos.y)))
                {
                    show = true;  
                }
            }
            //if (MessageToDisplay.MessageType == GameManager.MessageTypes.Game)
            //{
            //    Debug.Log(string.Format("shippos:{0} - shippos.yMin:{1} > {2} && {3} < {4}", shipPos, shipPos.yMin, Screen.height / 2, shipPos.yMax, Screen.height / 2));
            //}
            if (show)
            {
                if (GUI.Button(shipPos, GameManager.Instance.GUIManager.GetTextureByUnitClassId("detection"), "Label"))
                {
                    //(GameManager.Instance.CameraManager.SateliteCamera.gameObject.GetComponent(typeof(SateliteCam)) as SateliteCam).TempDestination = new Vector3(_WorldPos.x, 35100, _WorldPos.z);
                }
            }
            //GUI.Label(Position, MessageToDisplay.ToString());
            //Position = GUILayout.Window(_WindowId, Position, WindowFunc, "");
			GUI.color = Color.white;
		}
	}
	
	
	void Start()
	{
        //KillTime = GameManager.Instance.GUIManager.PopupWindowTimer;
        //FadeTime = GameManager.Instance.GUIManager.PopupFadeTimer;
		_Style = GameManager.Instance.GUIManager.PopupWindowStyle;
        _SateliteCamera = GameManager.Instance.CameraManager.SateliteCamera;
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
		
		if(_LifeTime >= KillTime)
		{
			Kill();
		}
	}
	
	void Kill()
	{
		Destroy(gameObject);
		
	}
}
