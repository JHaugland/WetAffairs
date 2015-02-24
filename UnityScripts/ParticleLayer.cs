using UnityEngine;
using System.Collections;

public class ParticleLayer : MonoBehaviour
{

    public float Layerheight = 0.00f;

    private Vector3 _Position;



    // Use this for initialization
    void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
        _Position = transform.localPosition;
        Camera cam = GameManager.Instance.CameraManager.MainCamera;

        Vector3 vector = cam.transform.position - transform.position;

        if (transform.parent && (vector.magnitude > Layerheight || Layerheight < 0.00))
        {
            transform.position = transform.parent.TransformPoint(_Position) + vector.normalized * Layerheight;
        }
    }
}
