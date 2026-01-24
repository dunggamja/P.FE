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

    [SerializeField]
    [Header("상호작용이 가능한 지점의 오프셋 값입니다.")]
    private Vector3[] m_interaction_offset = null;


    public  Int64  EntityID => m_entity_id;
    public  (int x, int y) Cell 
    {
        get 
        {        
            // Fixed Object의 포지션이... 중요한경우가 있    
            var center       = Vector3.zero;
            var center_count = 0;

            if (m_tile_attributes != null)
            {
                foreach (var e in m_tile_attributes)
                {
                    if (e.Collider != null)
                    {
                        center +=  e.Collider.bounds.center; 
                        center_count++;
                    }
                }                
            }

            if (0 < center_count)
            {
                center /= center_count;
                return center.PositionToCell();
            }


            return transform.position.PositionToCell();
        }
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

                int min_x  = (int)bounds.min.x;
                int min_z  = (int)bounds.min.z;
                int max_x  = (int)bounds.max.x;
                int max_z  = (int)bounds.max.z;

                for (int y = min_z; y <= max_z; y++)
                {
                    for (int x = min_x; x <= max_x; x++)
                    {
                        if (e.Collider.Raycast(Util.PositionToTerrainRay(x, y), out var _, Util.TERRAIN_RAY_DISTANCE))
                        {
                            tile_attributes.Add((e.Attribute, x, y));
                        }
                    }
                }
            }
        }

        return tile_attributes;
    }

}


