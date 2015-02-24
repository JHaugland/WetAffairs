using UnityEngine;
using System.Collections;

public class FireMissileManually : MonoBehaviour {

	
	// Update is called once per frame
	void Update () 
    {
        if ( Input.GetKeyDown(KeyCode.M) )
        {
            MissileLauncher[] gos = GetComponentsInChildren<MissileLauncher>();
            if ( gos.Length > 0 )
            {
                int random = UnityEngine.Random.Range(0, gos.Length);
                MissileLauncher l = gos[random];
                GameObject target = new GameObject("Target");
                target.transform.position = transform.TransformDirection(Vector3.forward * 1000) + transform.position;
                l.Fire(target);
            }
        }
	}
}
