using UnityEngine;
using System.Collections;

public class OnStartWizard : MonoBehaviour {

    private Camera _SateliteCamera;
    private Camera _SurfaceCamera;

    public Camera SateliteCamera
    {
        get
        {
            return _SateliteCamera;
        }
        set
        {
            _SateliteCamera = value;
        }
    }

    public Camera SurfaceCamera
    {
        get
        {
            return _SurfaceCamera;
        }
        private set
        {
            _SurfaceCamera = value;
        }
    }

	// Use this for initialization
	void Start () {
        SateliteCamera = GameManager.Instance.CameraManager.SateliteCamera;
        SurfaceCamera = GameManager.Instance.CameraManager.MainCamera;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
