using UnityEngine;
using System.Collections;

public class FollowCameraXZ : MonoBehaviour {
	
	public Transform target;
	public bool FollowX = true;
	public bool FollowY = false;
	public bool FollowZ = true;
	
	public float YOffset = 5.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if ( target )
        {
            transform.position = new Vector3(FollowX == true ? target.position.x : transform.position.x, FollowY == true ? target.position.y + YOffset : transform.position.y, FollowZ == true ? target.position.z : transform.position.z);
        }
	}
}
