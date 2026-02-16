using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Battle;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Battle
{

    public class Terrain_ZOC 
    {
        int                        m_width;
        int                        m_height;
        int                        m_block_size;
        Dictionary<int, TerrainBlockManager> m_faction_zoc = new();


        public Terrain_ZOC(int _width, int _height, int _block_size)    
        {
            m_width      = _width;
            m_height     = _height;
            m_block_size = _block_size;        
        }

        public void IncreaseZOC(int _faction, int _x, int _y)
        {
            if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
                return;
            
            if (_faction == 0)
                return;

            if (!m_faction_zoc.ContainsKey(_faction))
                m_faction_zoc.Add(_faction, new TerrainBlockManager(m_width, m_height, m_block_size));


            var new_value = m_faction_zoc[_faction].GetCellData(_x, _y) + 1;
            m_faction_zoc[_faction].SetCellData(_x, _y, new_value);

            // Debug.Log($"IncreaseZOC: f:{_faction}, pos:{_x}, {_y}, count:{new_value}");
        }

        public void DecreaseZOC(int _faction, int _x, int _y)
        {
            if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
                return;

            if (_faction == 0)
                return;

            if (!m_faction_zoc.ContainsKey(_faction))
                return;

            var new_value = m_faction_zoc[_faction].GetCellData(_x, _y) - 1;
            m_faction_zoc[_faction].SetCellData(_x, _y, new_value);

            // Debug.Log($"DecreaseZOC: f:{_faction}, pos:{_x}, {_y}, count:{new_value}");
        }

        public bool IsBlockedZOC(int _x, int _y, Func<int, bool> _func_ignore_zoc)
        {
            if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
                return false;


            foreach((var faction, var zoc_array) in m_faction_zoc)
            {
                // 통과 조건 체크.
                if (_func_ignore_zoc != null && _func_ignore_zoc(faction))
                    continue;

                // 셀 점유 확인.
                if (0 < zoc_array.GetCellData(_x, _y))
                    return true;
            }

            return false;
        }

        void CreateZOCArray(int _faction)
        {
            if (m_faction_zoc.ContainsKey(_faction))
                return;

            m_faction_zoc.Add(_faction, new TerrainBlockManager(m_width, m_height, m_block_size));
        }

        // public static bool IsBlocked_ZOC(int _owner_zoc_faction, int _x, int _y)
        // {
        //     // 0은 통과시킨다는 뜻.
        //     if (_owner_zoc_faction == 0 || _terrain_zoc_faction == 0)
        //         return false;

        //     // 다른 진영이면 통과불가능.
        //     if (_owner_zoc_faction != _terrain_zoc_faction)
        //         return true;
            
        //     return false;
        // }

        public Terrain_ZOC_IO Save()
        {
            var zoc = new Terrain_ZOC_IO 
            { 
                Width = m_width, 
                Height = m_height, 
                BlockSize = m_block_size, 
                ZOC = new Dictionary<int, TerrainBlockManager_IO>()
            };

            foreach(var (faction, zoc_array) in m_faction_zoc)
            {
                zoc.ZOC.Add(faction, zoc_array.Save());
            }
            return zoc;
        }

        public void Load(Terrain_ZOC_IO _io)
        {
            m_width      = _io.Width;
            m_height     = _io.Height;
            m_block_size = _io.BlockSize;

            foreach(var (faction, zoc_array) in _io.ZOC)
            {
                if (m_faction_zoc.TryGetValue(faction, out var terrain) == false)
                {
                    terrain = new TerrainBlockManager(m_width, m_height, m_block_size);
                    m_faction_zoc.Add(faction, terrain);
                }
                terrain.Load(zoc_array);
            }

            using var list_delete_faction = ListPool<int>.AcquireWrapper();

            foreach(var (faction, _) in m_faction_zoc)
            {
                if (!_io.ZOC.ContainsKey(faction))
                    list_delete_faction.Value.Add(faction);
            }

            list_delete_faction.Value.ForEach(faction => m_faction_zoc.Remove(faction));            
        }
    }

    public class Terrain_ZOC_IO
    {
        public int Width { get; set; }  
        public int Height { get; set; }
        public int BlockSize { get; set; }
        public Dictionary<int, TerrainBlockManager_IO> ZOC { get; set; }

    }
}
