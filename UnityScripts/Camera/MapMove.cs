using UnityEngine;
using System.Collections;

public class MapMove : MonoBehaviour {


    public float MinHeight = 30000;
    public float MaxHeight = 30500;

    public float XSpeed;
    public float YSpeed;
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * XSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * YSpeed * Time.deltaTime));
        
        if (transform.position.x > 1500)
        {
            Vector3 test = transform.position;
            test.x = -1500;
            transform.position = test;
        }
        if (transform.position.x < -1500)
        {
            Vector3 test = transform.position;
            test.x = 1500;
            transform.position = test;
        }

        transform.Translate(new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * 100, 0));
        Vector3 ypos = transform.position;
        ypos.y = Mathf.Clamp(ypos.y, MinHeight, MaxHeight);
        transform.position = ypos;

	}

    void OnDrawGizmos()
    {
        //RaycastHit hit;
        //Camera sat = GameManager.Instance.CameraManager.SateliteCamera;
        //if(Physics.Raycast(sat.ScreenPointToRay(Input.mousePosition), out hit))
        //{
        //    Vector3 inversed = this.transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z);
        //    Vector3 pos = new Vector3((inversed.x - 5.0f) * -1, inversed.y, (inversed.z + 5.0f));
        //    //Debug.Log(hit.point);
        //    Gizmos.DrawCube(hit.point, new Vector3(10, 10, 10));
        //}
        
    }
}
