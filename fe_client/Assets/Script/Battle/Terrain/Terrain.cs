using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public abstract class Terrain
{
    int    m_width;
    int    m_height;
    int[,] m_attribute;

    protected Terrain(int _width, int _height)
    {
        m_width     = _width;
        m_height    = _height;

        m_attribute = new int[_width, _height];       
    }

    public int  GetAttribute(int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return 0;

        return m_attribute[_x, _y];
    }

    protected bool HasAttribute(int _x, int _y, int _attribute_type)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return false;

        return (m_attribute[_x, _y] & (1 << _attribute_type)) != 0;
    }

    protected void SetAttribute(int _x, int _y, int _attribute_type)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        m_attribute[_x, _y] |= (1 << _attribute_type);
    }

    protected void RemoveAttribute(int _x, int _y, int _attribute_type)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        m_attribute[_x, _y] &= ~(1 << _attribute_type);
    }

    public void OverwriteAttribute(int _x, int _y, int _attribute)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        m_attribute[_x, _y] = _attribute;
    }

    public void ClearAttribute(int _x, int _y)
    {
        if (_x < 0 || _y < 0 || m_width <= _x || m_height <= _y)
            return;

        m_attribute[_x, _y] = 0;
    }

    public void ClearAll()
    {
        Array.Clear(m_attribute, 0, m_attribute.Length);
    }
}


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
    }

    public void RemoveCellData(int _x, int _y)
    {
        m_cell_data[_x % m_block_size, _y % m_block_size] = 0;
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

    (int block_x, int block_y) FindBlockIndex(int _x, int _y)
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
}



namespace Battle
{
    public class TerrainMap
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        // public TerrainCollision    Collision    { get; private set; }
        const int BLOCK_SIZE = 8;

        // zoc, attribute 도 블록을 이용하도록 해볼까??;;
        public Terrain_ZOC         ZOC          { get; private set; }
        public Terrain_Attribute   Attribute    { get; private set; }
        public TerrainBlockManager EntityManager { get; private set; }

        public void Initialize(int _width, int _height)
        {
            Width        = _width;
            Height       = _height;

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

        // TODO: SAVE/LOAD



    }

    
}
