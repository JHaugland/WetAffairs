using UnityEngine;
using System.Collections;

public class UpAndDown : MonoBehaviour {

    private int xTime = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  xTime++;
      Debug.Log(xTime);
      float ypos = ((Mathf.Pow(xTime, 2) * Mathf.Sin(xTime/50))+Mathf.Pow(xTime, 2))/5000; 
      Vector3 myPos = transform.position;
      myPos.y = ypos;
      this.transform.position = myPos; 
	}

         
} 
