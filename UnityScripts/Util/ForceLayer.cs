using UnityEngine;
using System.Collections;

public class ForceLayer : MonoBehaviour {

    public int Layer;
    public bool ForceOnChildren;

	
	// Update is called once per frame
	void Update () {

        //Do this in update because objects can be created during runtime

        if (gameObject.layer != Layer)
        {
            gameObject.layer = Layer;
        }

        if (ForceOnChildren)
        {
            SetLayerOnChildren(transform, Layer);
        }
	}

    private void SetLayerOnChildren(Transform t, int layer)
    {
        foreach (Transform tra in t)
        {
            if (tra.gameObject.layer == layer)
            {
                continue;
            }
            tra.gameObject.layer = layer;
            SetLayerOnChildren(t, layer);
        }
    }
}
