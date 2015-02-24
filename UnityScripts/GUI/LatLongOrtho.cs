using UnityEngine;
using System.Collections;

public class LatLongOrtho : MonoBehaviour {


    public float xMulti = 0.1671117f;
    public float yMulit = -0.1675854f;

    public float latOrtho = 0;
    public float longOrtho = 0;

    public Camera Camera;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 pos = new Vector3();
        pos.x = (longOrtho * xMulti) + 2250.0f;
        pos.z = (latOrtho * yMulit) + 3600.0f;
        pos.y = 30002;

        this.transform.position = pos;
	}

    void OnGUI()
    {


        Vector3 guiPos = this.Camera.WorldToScreenPoint(this.transform.position);

        Rect rect = new Rect(guiPos.x - 25, this.Camera.pixelHeight - guiPos.y - 25, 50, 50);
        //Debug.Log(string.Format("{0} - {1}", guiPos, rect));
        GUI.Button(rect, "Pole");
    }
}
