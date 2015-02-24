using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{

    public Vector3 Scale = new Vector3(100, 2, 0.1f);
    public GameObject Health;

    public float Height
    {
        get
        {
            return transform.localPosition.y;
        }
        set
        {
            transform.localPosition = Vector3.up * ( value + 20 );
        }
    }


    // Use this for initialization
    void Start()
    {

        Scale = new Vector3(10, 2, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    public void UpdateHealth(float percentage)
    {
        Vector3 scale = Health.transform.localScale;

        scale.x = ( percentage * Scale.x );
        Debug.Log(string.Format("percentage {0} * scale.x {1} = {2}", percentage, Scale.x, percentage * Scale.x));
        Health.transform.localScale = scale;

        if ( percentage != 1 )
        {
            Vector3 pos = Health.transform.position;
            pos.x = ( scale.x - 10 ) / 2;
            Health.transform.localPosition = pos;
        }

    }

    public void UpdateHealth(float health, float maxHealth)
    {

    }
}
