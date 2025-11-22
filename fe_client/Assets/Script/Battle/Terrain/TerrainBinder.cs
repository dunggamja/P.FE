using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Battle
{
    public class TerrainBinder : MonoBehaviour
    {
        [SerializeField]
        private Terrain                    m_terrain            = null;
         
        // [SerializeField]         
        // private sheet_map_setting          m_map_setting        = null;
        // [SerializeField]
        // private sheet_map_faction_setting  m_faction_setting    = null;

        [SerializeField]
        private Transform                  m_root_fixed_objects = null;


        public float GetHeight(Vector3 _world_position, bool _exclude_fixedobjects = false)
        {   
            float height = 0f;

            if (_exclude_fixedobjects == false)
            {
                if (Util.RaycastToTerrain(Util.PositionToTerrainRay(_world_position), out var hit, 1 << LayerMask.NameToLayer(Constants.LAYER_FIXED_OBJECT)))
                {
                    height = Mathf.Max(height, hit.point.y);
                }
            }


            if (m_terrain != null)
            {
                height = Mathf.Max(height, m_terrain.SampleHeight(_world_position));
            }

            return height;
        }



        public float GetHeight(int _x, int _y, bool _exclude_fixedobjects = false)
        {
            var    world_position = new Vector3(_x + 0.5f, 0f, _y + 0.5f);
            return GetHeight(world_position, _exclude_fixedobjects);
        }
    
    }
}


