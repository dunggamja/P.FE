using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static partial class Util
{
    public const float TERRAIN_RAY_ORIGIN_Y = 100f;
    public const float TERRAIN_RAY_DISTANCE = 200f;

    public static Ray  PositionToTerrainRay(int _x, int _y)
    {
        return PositionToTerrainRay(new Vector3(_x + 0.5f, TERRAIN_RAY_ORIGIN_Y, _y + 0.5f));
    }

    public static Ray  PositionToTerrainRay(Vector3 _world_position)
    {
        return new Ray(new Vector3(_world_position.x, TERRAIN_RAY_ORIGIN_Y, _world_position.z), Vector3.down);
    }

    public static bool RaycastToTerrain(int _x, int _y, out RaycastHit _hit, int _layer_mask)
    {
        return RaycastToTerrain(PositionToTerrainRay(_x, _y), out _hit, _layer_mask);
    }

    public static bool RaycastToTerrain(Ray _ray, out RaycastHit _hit, int _layer_mask)
    {
        return Physics.Raycast(_ray, out _hit, TERRAIN_RAY_DISTANCE, _layer_mask);
    }
}
