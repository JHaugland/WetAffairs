using UnityEngine;
using System.Collections;

public class KillParticles : MonoBehaviour {

    public float KillOnY = 0;
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y < KillOnY)
        {
            for (int i = 0; i < particleEmitter.particles.Length; i++)
            {
                if (particleEmitter.particles[i].position.y >= KillOnY)
                {
                    particleEmitter.particles[i] = new Particle();
                }
            }

        }


	}
}
