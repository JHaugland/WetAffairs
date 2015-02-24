using UnityEngine;
using System.Collections;

public class TestHeightAtLocation : MonoBehaviour {


    //Only used for reading from inspector
    public float Height;
    public Ocean ocean;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Height = ocean.GetWaterHeightAtLocation(transform.position.x, transform.position.z);


	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, Height, transform.position.z));

    }
}
