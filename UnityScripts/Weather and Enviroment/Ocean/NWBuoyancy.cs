using UnityEngine;
using System.Collections;

public class NWBuoyancy : MonoBehaviour
{


    //Only used for reading from inspector
    public Ocean Ocean;
    public float Magnitude;
    public Vector3 CenterOfMass = Vector3.zero;
    public int NumPointsX;
    public int NumPointsZ;
    public float DampCoeff = 0.1f;

    private float _YPos = 0.0f;
    private Vector3[] _Blobs;


    // Use this for initialization
    void Start()
    {
        rigidbody.centerOfMass = CenterOfMass;

        Vector3 bounds = GetComponent<MeshCollider>().mesh.bounds.size;
        float length = bounds.z;
        float width = bounds.x;

        _Blobs = new Vector3[NumPointsX * NumPointsZ];

        float xStep = 1.0f / (NumPointsX - 1);
        float zStep = 1.0f / (NumPointsZ - 1);

        int i = 0;

        for (int x = 0; x < NumPointsX; x++)
        {
            for (int z = 0; z < NumPointsZ; z++)
            {
                _Blobs[i] = new Vector3((-0.5f + x * xStep) * width, 0.0f, (-0.5f + z * zStep) *length) + Vector3.up * _YPos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        for (int i = 0; i < _Blobs.Length; i++)
        {
            Vector3 blob = _Blobs[i];
            if (blob != null)
            {
                Vector3 wPos = transform.TransformPoint(blob);
                float damp = rigidbody.GetPointVelocity(wPos).y;
                if (Ocean != null)
                {
                    rigidbody.AddForceAtPosition(-Vector3.up * (Magnitude * (wPos.y - Ocean.GetWaterHeightAtLocation(wPos.x, wPos.z)) + DampCoeff * damp), wPos);				
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_Blobs != null)
        {
            for (int k = 0; k < _Blobs.Length; k++)
            {
                Vector3 wpos = transform.TransformPoint(_Blobs[k]);
                Gizmos.DrawSphere(new Vector3(wpos.x, Ocean.GetWaterHeightAtLocation(wpos.x, wpos.z), wpos.z), 1);
            }
        }

    }
}
