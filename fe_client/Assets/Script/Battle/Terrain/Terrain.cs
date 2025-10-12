using System;
using System.Collections;
using System.Collections.Generic;
// using System.Security.Cryptography.X509Certificates;
// using Sirenix.OdinInspector;
using UnityEngine;


// public abstract class Terrain
// {
//     int    m_width;
//     int    m_height;
//     int[,] m_attribute;

//     protected Terrain(int _width, int _height)
//     {
//         m_width     = _width;
//         m_height    = _height;

//         m_attribute = new int[_width, _height];       
//     }

//     public int  GetAttribute(int _x, int _y)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return 0;

//         return m_attribute[_x, _y];
//     }

//     protected bool HasAttribute(int _x, int _y, int _attribute_type)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return false;

//         return (m_attribute[_x, _y] & (1 << _attribute_type)) != 0;
//     }

//     protected void SetAttribute(int _x, int _y, int _attribute_type)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return;

//         m_attribute[_x, _y] |= (1 << _attribute_type);
//     }

//     protected void RemoveAttribute(int _x, int _y, int _attribute_type)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return;

//         m_attribute[_x, _y] &= ~(1 << _attribute_type);
//     }

//     public void OverwriteAttribute(int _x, int _y, int _attribute)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return;

//         m_attribute[_x, _y] = _attribute;
//     }

//     public void ClearAttribute(int _x, int _y)
//     {
//         if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
//             return;

//         m_attribute[_x, _y] = 0;
//     }

//     public void ClearAll()
//     {
//         Array.Clear(m_attribute, 0, m_attribute.Length);
//     }
// }

namespace Battle
{
    public struct TerrainBlock
    {
        int            m_block_x;
        int            m_block_y;
        int            m_block_size;
        Int64[,]       m_cell_data;

        public TerrainBlock(int _x, int _y, int _size)
        {
            m_block_x    = _x;
            m_block_y    = _y;
            m_block_size = _size;
            m_cell_data  = new Int64[_size, _size];
        }

        public void SetCellData(int _x, int _y, Int64 _data)
        {
            m_cell_data[_x % m_block_size, _y % m_block_size] = _data;
            // Debug.Log($"SetCellData: {_x}, {_y}, {_data}");
        }

        public void RemoveCellData(int _x, int _y)
        {
            m_cell_data[_x % m_block_size, _y % m_block_size] = 0;
            // Debug.Log($"RemoveCellData: {_x}, {_y}");
        }

        public Int64 GetCellData(int _x, int _y)
        {
            return m_cell_data[_x % m_block_size, _y % m_block_size];
        }

        public bool HasBitIndex(int _x, int _y, int _bit_index)
        {
            var     cell_data = GetCellData(_x, _y);        
            return (cell_data & (1L << _bit_index)) != 0;
        }

        public void SetBitIndex(int _x, int _y, int _bit_index)
        {
            var cell_data  = GetCellData(_x, _y);
            cell_data     |= 1L << _bit_index;

            SetCellData(_x, _y, cell_data);
        }

        public void RemoveBitIndex(int _x, int _y, int _bit_index)
        {
            var cell_data = GetCellData(_x, _y);
            cell_data    &= ~(1L << _bit_index);

            SetCellData(_x, _y, cell_data);
        }

        public void ClearBlock()
        {
            Array.Clear(m_cell_data, 0, m_cell_data.Length);
        }

        public TerrainBlock_IO Save()
        {
            return new TerrainBlock_IO 
            { 
                BlockX    = m_block_x, 
                BlockY    = m_block_y, 
                BlockSize = m_block_size, 
                CellData  = (Int64[,])m_cell_data.Clone()
            };

        }

        public void Load(TerrainBlock_IO _io)
        {
            m_block_x    = _io.BlockX;
            m_block_y    = _io.BlockY;
            m_block_size = _io.BlockSize;
            m_cell_data  = (Int64[,])_io.CellData.Clone();
        }
    }

    public class TerrainBlock_IO
    {
        public int      BlockX    { get; set; }
        public int      BlockY    { get; set; }
        public int      BlockSize { get; set; }
        public Int64[,] CellData  { get; set; }
    }



    public class TerrainBlockManager
    {
        int             m_width;
        int             m_height;
        int             m_block_size;
        TerrainBlock[,] m_blocks;

        public TerrainBlockManager(int _width, int _height, int _block_size)
        {
            m_width         = _width;
            m_height        = _height;
            m_block_size    = Math.Max(1, _block_size);

            var tile_size   = Math.Max(_width, _height);
            var block_count = (tile_size / m_block_size) + ((0 < (tile_size % m_block_size)) ? 1: 0);

            m_blocks        = new TerrainBlock[block_count, block_count];
            for(int y = 0; y < block_count; ++y)
            {
                for(int x = 0; x < block_count; ++x)
                {
                    m_blocks[x, y] = new TerrainBlock(x * m_block_size, y * m_block_size, m_block_size);
                }
            }
        }

        public (int block_x, int block_y) FindBlockIndex(int _x, int _y)
        {
            if (_x < 0 || m_width <= _x || _y < 0 || m_height <= _y)
                return (-1, -1);

            var block_x = _x / m_block_size;
            var block_y = _y / m_block_size;

            return (block_x, block_y);
        }

        public void SetCellData(int _x, int _y, Int64 _data)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return;

            m_blocks[block_x, block_y].SetCellData(_x, _y, _data);
        }

        public void RemoveCellData(int _x, int _y)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return;

            m_blocks[block_x, block_y].RemoveCellData(_x, _y);
        }

        public Int64 GetCellData(int _x, int _y)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return 0;

            return m_blocks[block_x, block_y].GetCellData(_x, _y);
        }

        public void ClearBlockData(int _x, int _y)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return;

            m_blocks[block_x, block_y].ClearBlock();
        }


        public bool HasBitIndex(int _x, int _y, int _bit_index)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return false;

            return m_blocks[block_x, block_y].HasBitIndex(_x, _y, _bit_index);
        }

        public void SetBitIndex(int _x, int _y, int _bit_index)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return;

            m_blocks[block_x, block_y].SetBitIndex(_x, _y, _bit_index);
        }

        public void RemoveBitIndex(int _x, int _y, int _bit_index)
        {
            (var block_x, var block_y) = FindBlockIndex(_x, _y);
            if (block_x < 0 || block_y < 0)
                return;

            m_blocks[block_x, block_y].RemoveBitIndex(_x, _y, _bit_index);
        }

        


        public TerrainBlockManager_IO Save()
        {
            var blocks = new TerrainBlock_IO[m_blocks.GetLength(0), m_blocks.GetLength(1)];

            for(int y = 0; y < m_blocks.GetLength(1); ++y)
            {
                for(int x = 0; x < m_blocks.GetLength(0); ++x)
                {
                    blocks[x, y] = m_blocks[x, y].Save();
                }
            }

            return new TerrainBlockManager_IO 
            { 
                BlockSize = m_block_size, 
                Blocks    = blocks
            };
        }

        public void Load(TerrainBlockManager_IO _io)
        {
            m_block_size = _io.BlockSize;
            m_blocks     = new TerrainBlock[m_blocks.GetLength(0), m_blocks.GetLength(1)];

            for(int y = 0; y < m_blocks.GetLength(1); ++y)
            {
                for(int x = 0; x < m_blocks.GetLength(0); ++x)
                {
                    m_blocks[x, y].Load(_io.Blocks[x, y]);
                }
            }
        }
    }

    public class TerrainBlockManager_IO
    {
        public int BlockSize { get; set; }
        public TerrainBlock_IO[,] Blocks { get; set; }
    }

}


namespace Battle
{
    public class TerrainMap
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        // public TerrainCollision    Collision    { get; private set; }
        const int BLOCK_SIZE = 8;


        public Terrain_ZOC         ZOC          { get; private set; }
        public Terrain_Attribute   Attribute    { get; private set; }
        public TerrainBlockManager EntityManager { get; private set; }

        private float[,] m_height_map;

        public void Initialize(int _width, int _height)
        {
            Width        = _width;
            Height       = _height;

            m_height_map = new float[_width + 1, _height + 1];

            // Collision    = new TerrainCollision(_width, _height);
            ZOC          = new Terrain_ZOC(_width, _height, BLOCK_SIZE);
            Attribute    = new Terrain_Attribute(_width, _height, BLOCK_SIZE);

            // 8x8 블록 단위로 관리.
            EntityManager = new TerrainBlockManager(_width, _height, BLOCK_SIZE);
        }


        public bool IsInBound(int _x, int _y)
        {
            return 0 <= _x && _x < Width && 0 <= _y && _y < Height;
        }

        public float GetHeight(int _x, int _y)
        {
            // 범위 체크.
            if (_x < 0 || _y < 0 || _x >= m_height_map.GetLength(0) || _y >= m_height_map.GetLength(1))
                return 0f;

            return m_height_map[_x, _y];
        }

        // TODO: SAVE/LOAD
        //       블록단위로 저장/로드를 만들어야 겠다 생각하다가 작업을 뭔가 해버렸다. 

        public TerrainMap_IO Save()
        {
            return new TerrainMap_IO 
            { 
                Width         = Width, 
                Height        = Height, 
                ZOC           = ZOC.Save(), 
                Attribute     = Attribute.Save(), 
                EntityManager = EntityManager.Save() 
            };
        }

        public void Load(TerrainMap_IO _io)
        {
            Width        = _io.Width;
            Height       = _io.Height;
            ZOC.Load(_io.ZOC);
            Attribute.Load(_io.Attribute);
            EntityManager.Load(_io.EntityManager);
        }
    }

    public class TerrainMap_IO
    {
        public int                    Width         { get; set; }
        public int                    Height        { get; set; }
        public Terrain_ZOC_IO         ZOC           { get; set; }
        public TerrainBlockManager_IO Attribute     { get; set; }
        public TerrainBlockManager_IO EntityManager { get; set; }
    }
    
}

