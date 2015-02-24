using UnityEngine;
using System.Collections;

public class Rotater: MonoBehaviour {

    public float XSpeed = 1;
    public float YSpeed = 1;
    public float ZSpeed = 1;

    public bool Manual = false;

	
	void Update () 
    {
        if (!Manual)
        {
            transform.RotateAround(transform.position, Vector3.right, YSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.up, XSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.forward, ZSpeed * Time.deltaTime);
        }
        else
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Horizontal") * XSpeed * Time.deltaTime);
            }
            if (Input.GetAxis("Vertical") != 0)
            {
                transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Vertical") * YSpeed * Time.deltaTime);
            }
        }
	}
}
