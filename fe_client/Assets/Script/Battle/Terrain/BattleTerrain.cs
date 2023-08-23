using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct Collision
    {
        int     m_width;
        int     m_height;
        bool[,] m_collision;

        public Collision(int _width, int _height)
        {
            m_width     = _width;
            m_height    = _height;

            m_collision = new bool[_width, _height];
        }
    }

    public struct Tile
    {
        BaseContainer m_attribute;

    }


    public class BattleTerrain
    {
        public int Width  { get; private set; }
        public int Height { get; private set; }

        Collision m_collision;
    }
}
