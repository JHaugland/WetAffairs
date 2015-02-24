using UnityEngine;
using System.Collections;

public class UnitOverlay : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        this.renderer.enabled = Vector3.Distance(transform.position, GameManager.Instance.CameraManager.MainCamera.transform.position)
                                > GameManager.Instance.ShowOverlayOnUnitAtDistanceM;
	}
}
