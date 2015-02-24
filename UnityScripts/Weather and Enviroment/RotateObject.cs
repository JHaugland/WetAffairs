using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    public float XSpeed = 1;
    public float YSpeed = 1;
    public float ZSpeed = 1;

    public bool Manual;

    void Update()
    {

        if (!Manual)
        {

            transform.RotateAround(transform.position, Vector3.right, YSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.up, XSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.forward, ZSpeed * Time.deltaTime);

        }
        else
        {

            //if( Input.GetAxis("Horizontal") != 0 )
            //{

            //    transform.RotateAround( transform.position, Vector3.up, Input.GetAxis("Horizontal")*xSpeed * Time.deltaTime );

            //}

            //if( Input.GetAxis("Vertical") != 0 )
            //{

            //    transform.RotateAround( transform.position, Vector3.right, Input.GetAxis("Vertical")*ySpeed * Time.deltaTime );

            //}

        }
    }

}
