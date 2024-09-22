using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static partial class PathAlgorithm
{
    static float Distance(int _from_x, int _from_y, int _to_x, int _to_y)
    {
        // 1. 4방향 이동만 존재할 경우.
        return Math.Abs(_from_x - _to_x) + Math.Abs(_from_y - _to_y);

        // 2. 대각선 이동이 존재할 경우
        // return Mathf.Sqrt((_from_x - _to_x) * (_from_x - _to_x) + (_from_y - _to_y) * (_from_y - _to_y));
    }



    class Node 
    {     
        public int   x         { get; private set; } = 0;
        public int   y         { get; private set; } = 0;
        public float cost      { get; private set; } = 0f;
        public float heuristic { get; private set; } = 0f;
        public Node  parent    { get; private set; } = null;       
        
        public float TotalCost => cost + heuristic;

        public Node(int _x, int _y, int _to_x, int _to_y, Node _parent)
        {
            x         = _x;
            y         = _y;
            cost      = (_parent != null) ? _parent.cost + Distance(_parent.x, _parent.y, _x, _y) : 0;
            heuristic = Distance(_x, _y, _to_x, _to_y);
            parent    = _parent;            
        }

    }



}