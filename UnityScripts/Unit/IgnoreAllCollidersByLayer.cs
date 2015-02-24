using UnityEngine;
using System.Collections;

public class IgnoreAllCollidersByLayer : MonoBehaviour {

    public int Layer = -1;

	// Use this for initialization
	void Start () 
    {
        if ( collider != null )
        {
            if ( Layer == -1 )
            {
                Collider[] colliders = GameObject.FindObjectsOfType(typeof(Collider)) as Collider[];

                for ( int i = 0; i < colliders.Length; i++ )
                {
                    if ( colliders[i] == this.collider )
                    {
                        continue;
                    }
                    Physics.IgnoreCollision(this.collider, colliders[i]);
                }
            }

        }
        //TODO:Find objects in layer
	}
}
