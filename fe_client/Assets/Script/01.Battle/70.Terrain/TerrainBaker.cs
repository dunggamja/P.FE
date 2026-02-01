using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using System.Linq;
using System.IO;
using UnityEngine.Localization.SmartFormat.PersistentVariables;






#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Battle
{
    [Serializable]
    public class MapBakeData_Attribute_Dynamic
    {
        [Serializable]
        public struct AttributeCount
        {
            public int Attribute;
            public int Count;

            public AttributeCount(int _attribute, int _count)
            {
                Attribute = _attribute;
                Count     = _count;
            }

            public void Deconstruct(out int _attribute, out int _count)
            {
                _attribute = Attribute;
                _count     = Count;
            }
        }
        public int m_x;
        public int m_y;
        public List<AttributeCount> m_attribute_count = new();

        public MapBakeData_Attribute_Dynamic(int _x, int _y)
        {
            m_x               = _x;
            m_y               = _y;
            m_attribute_count = new();
        }

        public void IncreaseAttributeCount(int _attribute)
        {
            var index  = m_attribute_count.FindIndex(e => e.Attribute == _attribute);
            if (index != -1)
            {
                var count                = m_attribute_count[index].Count;
                m_attribute_count[index] = new AttributeCount(_attribute, count + 1);
            }
            else
            {
                m_attribute_count.Add(new AttributeCount(_attribute, 1));
            }
        }
    }



    [Serializable]
    public class MapBakeData_Terrain
    {
        public int     m_width     = 0;
        public int     m_height    = 0;

        // 정적 속성.
        public Int64[]                             m_attribute_mask_static = null;

        // 동적 속성.
        public List<MapBakeData_Attribute_Dynamic> m_attribute_dynamic     = null;

        public Int64[]                             m_attribute_mask_result = null;

        public MapBakeData_Terrain()
        {
            m_attribute_mask_static = null;
            m_attribute_dynamic     = null;
            m_attribute_mask_result = null;
        }

        public MapBakeData_Terrain(int _width, int _height)
        {
            Initialize(_width, _height);
        }

        public void Initialize(int _width, int _height)
        {
            
            m_width                 = _width;            
            m_height                = _height;
            m_attribute_mask_static = new Int64[m_width * m_height];
            m_attribute_dynamic     = new List<MapBakeData_Attribute_Dynamic>();
            m_attribute_mask_result = new Int64[m_width * m_height];
        }

        public void Bake(Int64[,] _attribute_static,  List<(int _x, int _y, int _attribute)> _attribute_dynamic)
        {
            m_attribute_dynamic.Clear();
            Array.Clear(m_attribute_mask_result, 0, m_attribute_mask_result.Length);
            Array.Clear(m_attribute_mask_static, 0, m_attribute_mask_static.Length);

            // 지형속성 - 정적
            if (_attribute_static != null)
            {
                if (m_attribute_mask_static.Length != _attribute_static.Length)
                {
                    Debug.LogError($"Attribute static length mismatch: {m_attribute_mask_static.Length} != {_attribute_static.Length}");
                    return;
                }

                for(int y = 0; y < m_height; y++)
                {
                    for(int x = 0; x < m_width; x++)
                    {
                        m_attribute_mask_static[y * m_width + x] = _attribute_static[x, y];
                        m_attribute_mask_result[y * m_width + x] = _attribute_static[x, y];
                    }
                }
            }

            // 지형속성 - 동적
            if (_attribute_dynamic != null)
            {
                foreach(var (x, y, attribute) in _attribute_dynamic)
                {
                    IncreaseAttribute_Dynamic(x, y, attribute);

                    m_attribute_mask_result[y * m_width + x] |= (1L << attribute);
                }
            }

        }

        public Int64 GetAttributeMask_Static(int _x, int _y)
        {
            return m_attribute_mask_static[_y * m_width + _x];
        }

        public Int64 GetAttributeMask_Result(int _x, int _y)
        {
            return m_attribute_mask_result[_y * m_width + _x];
        }

        // public Int64 GetAttributeMask_Dynamic(int _x, int _y)
        // {
        //     Int64 mask = 0;

        //     var index = m_attribute_dynamic.FindIndex(e => e.m_x == _x && e.m_y == _y);
        //     if (index == -1)            
        //     {
        //         return mask;
        //     }

        //     foreach(var (attribute, count) in m_attribute_dynamic[index].m_attribute_count)
        //     {
        //         mask |= (1L << attribute);
        //     }
            
        //     return mask;
        // }

        public void IncreaseAttribute_Dynamic(int _x, int _y, int _attribute)
        {
            var index = m_attribute_dynamic.FindIndex(e => e.m_x == _x && e.m_y == _y);
            if (index == -1)            
            {
                index = m_attribute_dynamic.Count;
                m_attribute_dynamic.Add(new MapBakeData_Attribute_Dynamic(_x, _y));
            }
            

            m_attribute_dynamic[index].IncreaseAttributeCount(_attribute);
        }

       
    }

    [Serializable]
    public class MapData_Entities
    {
        [Serializable]
        public struct Data
        {
            public Int64 m_entity_id;
            public Int32 m_cell_x;
            public Int32 m_cell_y;
            // public bool  m_is_fixed_object;

            public Data(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y)
            {
                m_entity_id       = _entity_id;
                m_cell_x          = _cell_x;
                m_cell_y          = _cell_y;
                // m_is_fixed_object = _is_fixed_object;
            }
        }

        public List<Data> m_entities = new();

        public bool AddEntity(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y)
        {
            if (m_entities.Any(e => e.m_entity_id == _entity_id))
            {
                Debug.LogError($"Entity {_entity_id} already exists");
                return false;
            }


            m_entities.Add(new Data(_entity_id, _cell_x, _cell_y));
            return true;
        }
    }


    [Serializable]
    public class MapData_Interactions
    {
        [Serializable]
        public struct Data
        {
            public Int64 m_entity_id;
            public Int32 m_cell_x;
            public Int32 m_cell_y;
            // public bool  m_is_fixed_object;

            public Data(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y)
            {
                m_entity_id       = _entity_id;
                m_cell_x          = _cell_x;
                m_cell_y          = _cell_y;
                // m_is_fixed_object = _is_fixed_object;
            }
        }

        public List<Data> m_repository = new();

        public bool Add(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y)
        {
            if (m_repository.Any(e => e.m_entity_id == _entity_id))
            {
                Debug.LogError($"Entity {_entity_id} already exists");
                return false;
            }


            m_repository.Add(new Data(_entity_id, _cell_x, _cell_y));
            return true;
        }
    }


    [Serializable]
    public class MapBakeData
    {
        public MapBakeData_Terrain   Terrain  = null;
        public MapData_Entities      Entities = null; 
        public MapData_Interactions  Interactions = null;        
    }


    public class TerrainBaker : MonoBehaviour
    {
       #if UNITY_EDITOR
        [SerializeField]
        private TerrainBinder      m_terrain_binder = null;
            
        [SerializeField]      
        private TerrainTileOverlay  m_tile_overlay   = null;

        [SerializeField]
        private DefaultAsset        m_file_folder    = null;

        [SerializeField]
        private string              m_map_file_name  = "map.json";


        string GetFilePath()
        {
            if (m_file_folder == null)
                return string.Empty;

            var path_folder = AssetDatabase.GetAssetPath(m_file_folder);
            if (AssetDatabase.IsValidFolder(path_folder) == false)
                return string.Empty;

            if (string.IsNullOrEmpty(m_map_file_name))
                return string.Empty;

            return Path.Combine(path_folder, m_map_file_name);
        }


        public void Bake()
        {
            if (m_tile_overlay == null)
                return;

            var file_path = GetFilePath();
            if (string.IsNullOrEmpty(file_path))
                return;

            // 지형 데이터. - 정적 데이터.
            var (tile_data, width, length) = m_tile_overlay.CollectTileData();

            // 지형 데이터 - 동적 데이터.
            using var list_tile_dynamic = ListPool<(int _x, int _y, int _attribute)>.AcquireWrapper();
            m_tile_overlay.CollectTileData_Dynamic(list_tile_dynamic.Value, width, length);

            // 오브젝트 데이터.
            var list_tiled_object          = m_tile_overlay.Collect_TiledOjects();
            var list_fixed_objects         = m_tile_overlay.Collect_FixedObjects();
            
            var bake_data          = new MapBakeData();
            bake_data.Terrain      = new MapBakeData_Terrain(width, length);
            bake_data.Entities     = new MapData_Entities();
            bake_data.Interactions = new MapData_Interactions();

            // 지형 데이터 베이크.
            bake_data.Terrain.Bake(tile_data, list_tile_dynamic.Value);


            // 오브젝트 데이터 적재.
            foreach (var e in list_tiled_object)
                if (e != null) bake_data.Entities.AddEntity(e.EntityID, e.Cell.x, e.Cell.y);

            // fixed object 데이터 적재.
            foreach (var e in list_fixed_objects)
                if (e != null) bake_data.Interactions.Add(e.EntityID, e.Cell.x, e.Cell.y);


            // foreach (var e in list_fixed_objects)
            //     if (e != null) bake_data.Entities.AddEntity(e.EntityID, e.Cell.x, e.Cell.y, true);
            
            
            File.WriteAllText(file_path, JsonUtility.ToJson(bake_data, true));

            AssetDatabase.ImportAsset(file_path);
        }


       #endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(TerrainBaker))]
    public class TerrainBakerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();



            if (GUILayout.Button("Bake"))
            {
                ((TerrainBaker)target).Bake();
            }
        }
    }
    #endif
}