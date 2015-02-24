using UnityEngine;
using System.Collections;

public class LerpBetweenDestinations : MonoBehaviour {

    
    public Transform[] Destinations;
    public int CurrentIndex = 0;
    public float Speed = 10;
    public float RotationSpeed = 10;
    public float MaxChangeDistance = 10;
    public float Distance = 0;
    private float _TotalDistance;

    void Start()
    {
        _TotalDistance = Vector3.Distance(transform.position, Destinations[CurrentIndex].position);
    }

	void Update () 
    {

        transform.LookAt(Destinations[CurrentIndex]);

        Distance = Vector3.Distance(transform.position, Destinations[CurrentIndex].position);

        if (Distance < MaxChangeDistance)
        {
            //transform.position = Vector3.Lerp(transform.position, Destinations[CurrentIndex].position, 0.9f);
            CurrentIndex++;
            if (CurrentIndex >= Destinations.Length)
            {
                CurrentIndex = 0;
            }
        }
       
	}

    void FixedUpdate()
    {
        if (rigidbody != null)
        {
            Vector3 movedirection = Vector3.forward * Speed;
            movedirection = transform.TransformDirection(movedirection);

            rigidbody.velocity = movedirection;
        }
    }
}
