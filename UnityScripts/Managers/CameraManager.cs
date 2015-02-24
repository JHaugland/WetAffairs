using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	#region Privates, consts and enums
	private bool _InMotion = false;
	public float  _CameraSwitchSpeed= 1;
	private bool _MotionBlurEnabled = true;
	public Camera CurrentCamera;

    public enum CameraMode
    {
        Satelite,
        Surface
    }

	#endregion
	

	
	#region Script variables
	public Camera m_MainCamera;
	public Camera m_SateliteCamera;
	
    //public RenderTexture MainCameraRender;
    //public RenderTexture SateliteCameraRender;
	
	
	public float MotionBlurModifier = 100.0f;
	public MotionBlur MotionBlur;
    public OrbitCam OrbitCameraScript;
    public float FadeDuration = 1;
	
	#endregion
	
	#region Public Properties

    public Transform Target
    {
        get
        {
            return OrbitCameraScript.Target;
        }
        set
        {
            OrbitCameraScript.Target = value;
        }
    }
	public Camera MainCamera
	{
		get
		{
            return m_MainCamera;
		}
		set
		{
            m_MainCamera = value;
		}
	}
	
	
	
	public Camera SateliteCamera
	{
		get
		{
			return m_SateliteCamera;
		}
		set
		{
			m_SateliteCamera = value;
		}
	}
	
	public float CameraSwitchSpeed
	{
		get
		{
			return _CameraSwitchSpeed;
		}
		set
		{
			_CameraSwitchSpeed = value;
		}
	}

    public CameraMode CamMode
    {
        get
        {
            return CurrentCamera == SateliteCamera ? CameraMode.Satelite : CameraMode.Surface;
        }
    }

	#endregion


    public void SetCameraFocus(Vector3 position, Vector3 rotation, PlayerUnit unit)
    {
        if (unit != null)
        {
            GameManager.Instance.UnitManager.SelectedUnit = unit;
        }
        else
        {
            this.MainCamera.GetComponent<OrbitCam>().Target = null;
        }

        this.MainCamera.transform.position = position;
        this.MainCamera.transform.eulerAngles = rotation;
    }

	
	
	#region Unity specific methods
	    void Start()
	    {
		    CurrentCamera = m_SateliteCamera;
            SendMessage("OnSelectedCameraChanged", SendMessageOptions.DontRequireReceiver);
	    }
    	
    	
	    void Update()
	    {
            if (CurrentCamera != null)
            {
                if (GameManager.Instance.GUIManager.IsMouseOverGUI(Input.mousePosition) == null)
                {
                    //check which view mouse is over. switch CurrentCam thereafter.
                    Rect sateliteScreenPort = MathHelper.ViewportRectToScreenRect(m_SateliteCamera.rect);
                    Rect surfaceScreenPort = MathHelper.ViewportRectToScreenRect(m_MainCamera.rect);

                    Camera current = CurrentCamera;

                    float x = Input.mousePosition.x;

                    //TODO: Fix this.
                    if (sateliteScreenPort.Contains(Input.mousePosition))
                    {
                        CurrentCamera = SateliteCamera;
                    }
                    if (surfaceScreenPort.Contains(Input.mousePosition))
                    {
                        CurrentCamera = MainCamera;
                    }

                    
                    if (current != CurrentCamera)
                    {
                        SendMessage("OnSelectedCameraChanged", SendMessageOptions.DontRequireReceiver);
                    }

                }
            }
	    }
	#endregion
	
	
}
