using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using System.Linq;
using System.IO;





#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Battle
{

    [Serializable]
    public class MapBakeData_Terrain
    {
        public Int64[] m_tile_data = null;
        public int     m_width     = 0;
        public int     m_height    = 0;

        public MapBakeData_Terrain()
        {
            m_tile_data = null;
        }

        public MapBakeData_Terrain(Int64[,] _tile_data)
        {
            Initialize(_tile_data);
        }

        public void Initialize(Int64[,] _tile_data)
        {
            if (_tile_data == null)
                return;

            m_width     = _tile_data.GetLength(0);            
            m_height    = _tile_data.GetLength(1);
            m_tile_data = new Int64[m_width * m_height];

            for(int y = 0; y < m_height; y++)
            {
                for(int x = 0; x < m_width; x++)
                {
                    m_tile_data[y * m_width + x] = _tile_data[x, y];
                }
            }
        }

        public Int64 GetTileData(int _x, int _y)
        {
            return m_tile_data[_y * m_width + _x];
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
            public bool  m_is_fixed_object;

            public Data(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y, bool _is_fixed_object)
            {
                m_entity_id       = _entity_id;
                m_cell_x          = _cell_x;
                m_cell_y          = _cell_y;
                m_is_fixed_object = _is_fixed_object;
            }
        }

        public List<Data> m_entities = new();

        public bool AddEntity(Int64 _entity_id, Int32 _cell_x, Int32 _cell_y, bool _is_fixed_object)
        {
            if (m_entities.Any(e => e.m_entity_id == _entity_id))
            {
                Debug.LogError($"Entity {_entity_id} already exists");
                return false;
            }


            m_entities.Add(new Data(_entity_id, _cell_x, _cell_y, _is_fixed_object));
            return true;
        }
    }


    [Serializable]
    public class MapBakeData
    {
        public MapBakeData_Terrain Terrain  = null;
        public MapData_Entities    Entities = null;        
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

            // 지형 데이터.
            var (tile_data, width, length) = m_tile_overlay.CollectTileData();

            // 오브젝트 데이터.
            var list_tiled_object          = m_tile_overlay.Collect_TiledOjects();
            var list_fixed_objects         = m_tile_overlay.Collect_FixedObjects();
            
            var bake_data      = new MapBakeData();
            bake_data.Terrain  = new MapBakeData_Terrain(tile_data);
            bake_data.Entities = new MapData_Entities();

            foreach (var e in list_tiled_object)
                if (e != null) bake_data.Entities.AddEntity(e.EntityID, e.Cell.x, e.Cell.y, false);

            foreach (var e in list_fixed_objects)
                if (e != null) bake_data.Entities.AddEntity(e.EntityID, e.Cell.x, e.Cell.y, true);
            
            
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