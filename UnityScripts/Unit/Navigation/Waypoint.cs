using UnityEngine;
using System.Collections;
using TTG.NavalWar.NWComms.Entities;

public class Waypoint
{
    private bool _Selected;
    private Vector2 _Size;
    private PositionInfo _Position;

    public PositionInfo Position
    {
        get
        {
            return _Position;
        }
        set
        {
            _Position = value;
        }
    }

    public Vector2 Size
    {
        get
        {
            return _Size;
        }
        set
        {
            _Size = value;
        }
    }

    public bool Selected
    {
        get
        {
            return _Selected;
        }
        set
        {
            _Selected = value;
        }
    }

    public Waypoint(PositionInfo position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public void Draw()
    {
        //Vector3 wp = new Vector3();
        //Debug.Log(CameraPixelHeight);
        //wp.x = ((float)Position.LongitudeOrthoProjected * GameManager.Instance.XMapModifier) + GameManager.Instance.XMapAddition;
        //wp.z = ((float)Position.LatitudeOrthoProjected * GameManager.Instance.YMapModifier) + GameManager.Instance.YMapAddtion;
        //wp.y = 30000;

        //Rect waypointPos = new Rect(wp.x - Size.x / 2, CameraPixelHeight - wp.y - Size.y / 2, Size.x, Size.x);

        //GUI.DrawTexture(waypointPos, GameManager.Instance.GUIManager.WaypointTexture);
    }
}
