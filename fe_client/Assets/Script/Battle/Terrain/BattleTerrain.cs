﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class TerrainCollision
{
    int     m_width;
    int     m_height;
    bool[,] m_collision;

    public TerrainCollision(int _width, int _height)
    {
        m_width     = _width;
        m_height    = _height;

        m_collision = new bool[_width, _height];
    }

    public bool IsCollision(int _x, int _y)
    {
        if (_x < 0 || m_width <= _x || _y < 0 || m_height <= _y)
            return true;

        return m_collision[_x, _y];
    }
}

public class TerrainAttribute
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

namespace Battle
{
    public class BattleTerrain
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        public TerrainCollision Collision { get; private set; }
        public TerrainAttribute Attribute { get; private set; }

        public void Initialize(int _width, int _height)
        {
            Width     = _width;
            Height    = _height;

            Collision = new TerrainCollision(_width, _height);
            Attribute = new TerrainAttribute(_width, _height);
        }
        
        // TODO: SAVE/LOAD

    }

    
}
