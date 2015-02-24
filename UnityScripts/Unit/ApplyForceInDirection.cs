using UnityEngine;
using System.Collections;

public class ApplyForceInDirection : MonoBehaviour {


    public Vector3 Direction;
    public float Force;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        rigidbody.velocity = Direction * Force;
    }

}
