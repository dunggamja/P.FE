using System;
using System.Collections.Generic;
using UnityEngine;

public class PathVehicleSnapshot
{
    public Vector3 PositionPrev { get; private set; } = Vector3.zero;
    public Vector3 Position     { get; private set; } = Vector3.zero;
    public Vector3 Velocity     { get; private set; } = Vector3.zero;
    public float   MaxSpeed     { get; private set; } = 0f;

    public static PathVehicleSnapshot Create(
        Vector3 _position_prev,
        Vector3 _position,
        Vector3 _velocity,
        float   _max_speed)
    {
        return new PathVehicleSnapshot 
        { 
          PositionPrev = _position_prev, 
          Position     = _position, 
          Velocity     = _velocity, 
          MaxSpeed     = _max_speed 
        };
    }
}
