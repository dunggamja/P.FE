using UnityEngine;
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Battle;
using System.Collections.Generic;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class FixedObjects : MonoBehaviour
{
    [Serializable]
    public struct TileAttribute
    {
        public EnumTerrainAttribute Attribute;
        public BoxCollider          Collider;
    }


    // 
    [SerializeField]
    [Header("Entity ID, 17자리 이하까지만 사용해주세요.")]    
    private Int64 m_entity_id = 0;


    [SerializeField]
    [Header("Tile Attribute, 점유 타일의 속성과 범위를 정의합니다.")]
    private List<TileAttribute> m_tile_attributes = null;


    const float RAY_ORIGIN_Y = 100f;
    const float RAY_DISTANCE = 200f;

    static Ray PositionToRay(int _x, int _y)
    {
        return new Ray(new Vector3(_x + 0.5f, RAY_ORIGIN_Y, _y + 0.5f), Vector3.down);
    }




    public List<(EnumTerrainAttribute attribute, int x, int y)> GetTileAttributes()
    {
        var tile_attributes = new List<(EnumTerrainAttribute attribute, int world_x, int world_y)>();

        if (m_tile_attributes != null)
        {
            foreach (var e in m_tile_attributes)
            {
                if (e.Collider == null)
                    continue;

                var bounds = e.Collider.bounds;

                int min_x = (int)bounds.min.x;
                int min_z = (int)bounds.min.z;
                int max_x = (int)bounds.max.x;
                int max_z = (int)bounds.max.z;

                for (int y = min_z; y <= max_z; y++)
                {
                    for (int x = min_x; x <= max_x; x++)
                    {
                        if (e.Collider.Raycast(PositionToRay(x, y), out var _, RAY_DISTANCE))
                        {
                            tile_attributes.Add((e.Attribute, x, y));
                        }
                    }
                }
            }
        }

        return tile_attributes;
    }


    public float GetMaxHeight_FromColliders(int _x, int _y)
    {
        var max_height = 0f;

        if (m_tile_attributes != null)
        {
            foreach (var e in m_tile_attributes)
            {
                if (e.Collider == null)
                    continue;

                // 바운드 범위로 1차 체크.                
                var bounds         = e.Collider.bounds;
                if (bounds.Contains(new Vector3(_x + 0.5f, bounds.center.y, _y + 0.5f)) == false)
                    continue;

                // 레이캐스트로 2차 체크.
                if (e.Collider.Raycast(PositionToRay(_x, _y), out var _, RAY_DISTANCE))
                    max_height = Mathf.Max(max_height, bounds.max.y);                
            }
        }

        return max_height;
    }
    // bool IsInCollider(BoxCollider _collider, Vector3 _world_position)
    // {
    //     if (_collider == null)
    //         return false;

    //     var local_position = _collider.transform.InverseTransformPoint(_world_position);


    //     var max_position =_collider.center + _collider.size * 0.5f;
    //     var min_position =_collider.center - _collider.size * 0.5f;



    
    //     // Collider의 center와 size 고려 (로컬 공간에서)
    //     var center   = _collider.center;
    //     var halfSize = _collider.size * 0.5f;
        
    //     // 로컬 공간에서 AABB 체크 (회전 고려됨!)
    //     return Mathf.Abs(local_position.x - center.x) <= halfSize.x &&
    //         //    Mathf.Abs(local_position.y - center.y) <= halfSize.y &&
    //            Mathf.Abs(local_position.z - center.z) <= halfSize.z;
    // }





    


    


}


