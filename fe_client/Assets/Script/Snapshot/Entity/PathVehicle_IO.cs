using System;
using System.Collections.Generic;
using UnityEngine;

public class PathVehicle_IO
{
    public Vector3 PositionPrev { get; set; } = Vector3.zero;
    public Vector3 Position     { get; set; } = Vector3.zero;
    public Vector3 Velocity     { get; set; } = Vector3.zero;
    public float   MaxSpeed     { get; set; } = 0f;

}
