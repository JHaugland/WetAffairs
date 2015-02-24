using UnityEngine;
using System.Collections;

public class Buoyancy : MonoBehaviour {

    public float _XFactor = 0;

    public float MaxX = 2;
    public float MinX = -2;

    public float ActualMax = 2;
    public float ActualMin = -2;

    private float StartY;

    public bool Reverse = false;

	// Use this for initialization
	void Start () {
        StartY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        float change = Random.Range(0, Time.deltaTime);
        _XFactor += Reverse == true ? -change : change;

        if (_XFactor <= MinX || _XFactor >= MaxX)
        {
            Reverse = !Reverse;
            //ActualMax = Random.Range(MaxX - 1, MaxX);
            //ActualMin = Random.Range(MinX, MinX + 1);
        }

        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //rot.x =
        pos.y = StartY + _XFactor;

        transform.position = pos;
        transform.eulerAngles = rot;
	}
}
