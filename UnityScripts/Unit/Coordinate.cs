using UnityEngine;
using System.Collections;

[System.Serializable] 
public class Coordinate {

    [SerializeField]
    private float _Latitude;
    [SerializeField]
    private float _Longitude;

    public float Latitude
    {
        get
        {
            return _Latitude;
        }
        set
        {
            _Latitude = value;
        }
    }

    public float Longitude
    {
        get
        {
            return _Longitude;
        }
        set
        {
            _Longitude = value;
        }
    }

    public Coordinate()
    {
    }
    public Coordinate(float lat, float lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public bool IsInDistanceOf(Coordinate compareTo, float distanceInLatOrLng)
    {
        float distanceLat = Mathf.Abs(compareTo.Latitude - this.Latitude);
        float distanceLng = Mathf.Abs(compareTo.Longitude - this.Longitude);

        return distanceLat < 1 && distanceLng < 1;
    }
}
