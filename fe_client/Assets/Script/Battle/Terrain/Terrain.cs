using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


// public class TerrainCollision
// {
//     int     m_width;
//     int     m_height;
//     bool[,] m_collision;

//     public TerrainCollision(int _width, int _height)
//     {
//         m_width     = _width;
//         m_height    = _height;

//         m_collision = new bool[_width, _height];
//     }

//     public bool IsCollision(int _x, int _y)
//     {
//         if (_x < 0 || m_width <= _x || _y < 0 || m_height <= _y)
//             return true;

//         return m_collision[_x, _y];
//     }
// }

public partial class TerrainAttribute
{
    int    m_width;
    int    m_height;
    int[,] m_attribute;

    public TerrainAttribute(int _width, int _height)
    {
        m_width     = _width;
        m_height    = _height;

        m_attribute = new int[_width, _height];
    }

    public int  GetAttribute(int _x, int _y)
    {
        return m_attribute[_x, _y];
    }

    public bool HasAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        return (m_attribute[_x, _y] & (1 << (int)_attribute_type)) != 0;
    }

    public void SetAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        m_attribute[_x, _y] |= (1 << (int)_attribute_type);
    }

    public void RemoveAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        m_attribute[_x, _y] &= ~(1 << (int)_attribute_type);
    }

    public void OverwriteAttribute(int _x, int _y, int _attribute)
    {
        m_attribute[_x, _y] = _attribute;
    }

    public void ClearAttribute(int _x, int _y)
    {
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
    Dictionary<Int64, (int x, int y)> m_entities;

    public TerrainBlock(int _x, int _y, int _size)
    {
        m_block_x    = _x;
        m_block_y    = _y;
        m_block_size = _size;
        m_entities   = new(10);
    }

    public void SetEntity(Int64 _id, int _x, int _y)
    {
        m_entities[_id] = (_x, _y);
    }

    public void RemoveEntity(Int64 _id)
    {
        m_entities.Remove(_id);
    }

    public Int64 FindEntity(int _x, int _y)
    {
        foreach((var id, var position) in m_entities)
        {
            if (position.x == _x && position.y == _y)
                return id;
        }

        return 0;
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
    }

    (int block_x, int block_y) FindBlockIndex(int _x, int _y)
    {
        if (_x < 0 || m_width <= _x || _y < 0 || m_height <= _y)
            return (-1, -1);

        var block_x = _x / m_block_size;
        var block_y = _y / m_block_size;

        return (block_x, block_y);
    }

    public Int64 FindEntity(int _x, int _y)
    {
        (var block_x, var block_y) = FindBlockIndex(_x, _y);
        if (block_x < 0 || block_y < 0)
            return 0;

        return m_blocks[block_x, block_y].FindEntity(_x, _y);
    }

}

namespace Battle
{
    public class TerrainMap
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        // public TerrainCollision    Collision    { get; private set; }
        public TerrainAttribute    Attribute    { get; private set; }
        public TerrainBlockManager BlockManager { get; private set; }

        public void Initialize(int _width, int _height)
        {
            Width        = _width;
            Height       = _height;

            // Collision    = new TerrainCollision(_width, _height);
            Attribute    = new TerrainAttribute(_width, _height);
            BlockManager = new TerrainBlockManager(_width, _height, 16);
        }

        // TODO: SAVE/LOAD

    }

    
}
