using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static partial class PathAlgorithm
{
    static public int Distance(int _from_x, int _from_y, int _to_x, int _to_y)
    {
        // 1. 4방향 거리 체크.
        return Math.Abs(_from_x - _to_x) + Math.Abs(_from_y - _to_y);
    }

    static public int Distance((int x, int y) _from, (int x, int y) _to)
    {
        return Distance(_from.x, _from.y, _to.x, _to.y);
    }



    struct Node 
    {     
        public int   x                    { get; private set; }
        public int   y                    { get; private set; }
        public int   cost                 { get; private set; }
        public float heuristic            { get; private set; }
        public (int x, int y) parent_node { get; private set; }   
        
        public float TotalCost => cost + heuristic;
        public bool  HasParent => parent_node.x != -1 && parent_node.y != -1;

        // public Node  parent    { get; private set; }// = null;       
        public Node(int _x, int _y, int _to_x, int _to_y, int _cost, (int x, int y) _parent_node)
        {
            x           = _x;
            y           = _y;
            cost        = _cost;
            heuristic   = Distance(_x, _y, _to_x, _to_y);
            parent_node = _parent_node;
            // parent    = _parent;            
        }


        public static Node Create(int _x, int _y, int _to_x, int _to_y) 
        => new Node(_x, _y, _to_x, _to_y, 0, (-1, -1));

        // public void Reset()
        // {
        //     x         = 0;
        //     y         = 0;
        //     cost      = 0;
        //     heuristic = 0f;
        //     parent    = null;
        // }
    }



}