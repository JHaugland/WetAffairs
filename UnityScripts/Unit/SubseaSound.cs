using UnityEngine;
using System.Collections;

public class SubseaSound : MonoBehaviour {

	
	// Update is called once per frame
	void Update () 
    {
        audio.volume = GameManager.Instance.CameraManager.MainCamera.transform.position.y < 0 ? 1 : 0;
	
	}
}
