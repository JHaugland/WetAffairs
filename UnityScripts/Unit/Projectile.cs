using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{


    // === variables you need ===
    //how fast our shots move
    public float shotSpeed = 10.0f;
    public float m_distanceInMetersOverGround = 20.0f;
    public float m_lifeTimeInSeconds = 100.0f;
    public float m_thrustForce = 100.0f;
    public float m_initialAngle = 45.0f;
    public float m_turnspeed = 10.0f;
    public float m_maxVelocityChange = 10.0f;
    public float m_flyHeight = 10.0f;
    public float m_destroyTimeInSeconds = 2.0f;
    public float m_explosionForce = 1000.0f;
    public float m_explosionRange = 1000.0f;
    //objects
    public GameObject shooter;
    public GameObject target;
    public GameObject explosion;

    protected bool isFired = false;
    protected bool hasSufficientHeight = false;
    protected bool killCam = false;

    protected float m_elapsed = 0.0f;
    // === derived variables ===
    //positions
    protected Vector3 shooterPosition = Vector3.zero;
    protected Vector3 targetPosition = Vector3.zero;
    //velocities
    protected Vector3 shooterVelocity = Vector3.zero;
    protected Vector3 targetVelocity = Vector3.zero;

    protected Vector3 stopVector = Vector3.zero;

    //calculate intercept
    public Vector3 interceptPoint = Vector3.zero;
    //now use whatever method to launch the projectile at the intercept point 
    // Use this for initialization
    void Start()
    {
        transform.LookAt(new Vector3(transform.position.x, 1000, transform.position.z));
        Fire();

    }

    public void Fire()
    {
        if ( !isFired )
        {
            //positions
            //if ( target  && shooter)
            //{
                shooterPosition = shooter.transform.position;

                targetPosition = target.transform.position;
                //velocities
                shooterVelocity = shooter.rigidbody ? shooter.rigidbody.velocity : Vector3.zero;
                targetVelocity = target.rigidbody ? target.rigidbody.velocity : Vector3.zero;

                interceptPoint = FirstOrderIntercept(shooterPosition,
                                                        shooterVelocity,
                                                        shotSpeed,
                                                        targetPosition,
                                                        targetVelocity);
                //~ Debug.Log(targetPosition);
            //}
        }
    }

    void DetachEmitters()
    {
        int i = 0;
        while ( i < transform.childCount )
        {
            //~ child = transform.GetChild(i);
            foreach ( Transform child in transform )
            {
                if ( child.particleEmitter )
                {
                    child.parent = null;
                    child.particleEmitter.emit = false;
                    ParticleAnimator p = child.GetComponent(typeof(ParticleAnimator)) as ParticleAnimator;
                    p.autodestruct = true;
                    Destroy(child);
                }
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_elapsed += Time.deltaTime;
        if ( m_elapsed >= m_lifeTimeInSeconds )
        {
            DestroyProjectile();
        }
        RaycastHit hit;
        if ( !hasSufficientHeight )
        {
            if ( !Physics.Raycast(transform.position, -Vector3.up, out hit, m_flyHeight) )
            {
                hasSufficientHeight = true;
            }
        }

        if ( target && transform.position.y >= m_distanceInMetersOverGround )
        {
            Vector3 lineOfSight;

            lineOfSight = target.transform.position - transform.position;
            lineOfSight.Normalize();
            lineOfSight = Vector3.Slerp(transform.forward, lineOfSight, Time.deltaTime * m_turnspeed);
            lineOfSight = transform.position + lineOfSight;
            // lineOfSight.y = target.transform.position.y;

            transform.LookAt(lineOfSight);
        }



    }

    public void FixedUpdate()
    {

        Vector3 velocityChange = ( transform.forward * m_thrustForce - rigidbody.velocity );

        velocityChange.x = Mathf.Clamp(velocityChange.x, -m_maxVelocityChange, m_maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -m_maxVelocityChange, m_maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -m_maxVelocityChange, m_maxVelocityChange);

        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if ( !hasSufficientHeight )
        {

            rigidbody.AddForce(0, m_thrustForce, 0, ForceMode.Force);
        }
    }

    void OnCollisionEnter(Collision col1)
    {
        Explode(col1);

    }

    private void Explode(Collision col1)
    {
        Debug.Log(col1.gameObject.name);
        if ( explosion )
            Instantiate(explosion, transform.position, Quaternion.LookRotation(transform.position));
        Vector3 explosionPos = transform.position;
        Collider[] colliders  = Physics.OverlapSphere(explosionPos, 100.0f);

        foreach ( Collider col in colliders )
        {
            if ( !col )
                continue;

            if ( col.rigidbody )
            {
                col.rigidbody.AddExplosionForce(m_explosionForce, explosionPos, m_explosionRange);
            }
        }
        DetachEmitters();

        Destroy(gameObject);

    }

    private void DestroyProjectile()
    {
        DetachEmitters();
        Destroy(gameObject);
        Destroy(target);
    }



    //first-order intercept using absolute target position

    public static Vector3 FirstOrderIntercept(Vector3 shooterPosition,
                                                Vector3 shooterVelocity,
                                                float shotSpeed,
                                                Vector3 targetPosition,
                                                Vector3 targetVelocity)
    {
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime(shotSpeed,
                                            targetPosition - shooterPosition,
                                            targetRelativeVelocity);
        return targetPosition + t * ( targetRelativeVelocity );
    }
    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime(float shotSpeed,
                                                Vector3 targetRelativePosition,
                                                Vector3 targetRelativeVelocity)
    {
        float a = targetRelativeVelocity.sqrMagnitude - shotSpeed * shotSpeed;
        //handle similar velocities
        if ( Mathf.Abs(a) < 0.001f )
        {
            float t = -targetRelativePosition.sqrMagnitude
                        / ( 2f * Vector3.Dot(targetRelativeVelocity,
                                            targetRelativePosition) );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float   b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition),
				c = targetRelativePosition.sqrMagnitude,
				determinant = b * b - 4f * a * c;

        if ( determinant > 0f )
        { //determinant > 0; two intercept paths (most common)
            float   t1 = ( -b + Mathf.Sqrt(determinant) ) / ( 2f * a ),
					t2 = ( -b - Mathf.Sqrt(determinant) ) / ( 2f * a );
            if ( t1 > 0f )
            {
                if ( t2 > 0f )
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if ( determinant < 0f ) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / ( 2f * a ), 0f); //don't shoot back in time
    }
}
