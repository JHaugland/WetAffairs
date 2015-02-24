using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

[Serializable, StructLayout(LayoutKind.Sequential)] 
public class ZoomLevel
{
    [SerializeField]
    private float _Y = 30000;
    [SerializeField]
    private float _MoveSpeed = 10;

    public float Y { get { return _Y; } set { _Y = value; } }
    public float MoveSpeed { get { return _MoveSpeed; } set { _MoveSpeed = value; } }


    public ZoomLevel()
    {
    }
    public ZoomLevel(float y, float moveSpeed)
    {
        Y = y;
        MoveSpeed = moveSpeed;
    }
}
