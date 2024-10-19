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
        return m_attribute[_x, _y];
    }

    protected bool HasAttribute(int _x, int _y, int _attribute_type)
    {
        return (m_attribute[_x, _y] & (1 << _attribute_type)) != 0;
    }

    protected void SetAttribute(int _x, int _y, int _attribute_type)
    {
        m_attribute[_x, _y] |= (1 << _attribute_type);
    }

    protected void RemoveAttribute(int _x, int _y, int _attribute_type)
    {
        m_attribute[_x, _y] &= ~(1 << _attribute_type);
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

    public Int64 FindEntityID(int _x, int _y)
    {
        (var block_x, var block_y) = FindBlockIndex(_x, _y);
        if (block_x < 0 || block_y < 0)
            return 0;

        return m_blocks[block_x, block_y].FindEntity(_x, _y);
    }

    public void RefreshEntity(Int64 _entity_id, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        (var block_from_x, var block_from_y) = FindBlockIndex(_from_x, _from_y);
        (var block_to_x,   var block_to_y)   = FindBlockIndex(_to_x, _to_y);

        // 블록이 변경되었으면 이전 블록에서 제거.
        if (block_from_x != block_to_x && block_from_y != block_to_y)
        {
            if (0 <= block_from_x && 0 <= block_from_y)
            {
                m_blocks[block_from_x, block_from_y].RemoveEntity(_entity_id);
            }
        }

        
        // 블록 위치 갱신.
        if (0 <= block_to_x && 0 <= block_to_y)
        {
            m_blocks[block_to_x, block_to_y].SetEntity(_entity_id, _to_x, _to_y);
        }
    }

}

namespace Battle
{
    public class TerrainMap
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        // public TerrainCollision    Collision    { get; private set; }
        public Terrain_ZOC         ZOC          { get; private set; }
        public Terrain_Attribute   Attribute    { get; private set; }
        public TerrainBlockManager BlockManager { get; private set; }

        public void Initialize(int _width, int _height)
        {
            Width        = _width;
            Height       = _height;

            // Collision    = new TerrainCollision(_width, _height);
            ZOC          = new Terrain_ZOC(_width, _height);
            Attribute    = new Terrain_Attribute(_width, _height);
            BlockManager = new TerrainBlockManager(_width, _height, 16);
        }

        // TODO: SAVE/LOAD



    }

    
}
