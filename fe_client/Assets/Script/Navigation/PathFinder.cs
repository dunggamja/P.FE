using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static class PathFinder
{
    static float Distance(int _from_x, int _from_y, int _to_x, int _to_y)
    {
        return Mathf.Sqrt((_from_x - _to_x) * (_from_x - _to_x) + (_from_y - _to_y) * (_from_y - _to_y));
    }

    class Node : IComparer<Node>, IEqualityComparer<Node>
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

        public int Compare(Node a, Node b)
        {
            return a.TotalCost.CompareTo(b.TotalCost);
        }

        public bool Equals(Node a, Node b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public int GetHashCode(Node _o)
        {
            return _o.x ^ _o.y;
        }
    }



    public static List<PathNode> Find(ref TerrainCollision _collision, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        var result     = new List<PathNode>();
        var open_list  = new SortedSet<Node>();
        var close_list = new SortedSet<Node>();

        // 시작 지점 셋팅.
        close_list.Add(new Node(_from_x, _from_y, _to_x, _to_y, null));

        // A* 길찾기
        var    node  = AStar(_collision, open_list, close_list, _to_x, _to_y);
        while (node != null)
        {
            result.Add(new PathNode(node.x, node.y));
            node = node.parent;
        }

        // 순서 변경.
        result.Reverse();

        return result;
    }


    static Node AStar(ref TerrainCollision _collision, ref SortedSet<Node> _open_list, ref SortedSet<Node> _close_list, int _to_x, int _to_y)
    {
        while(0 < _close_list.Count)
        {
            var item = _close_list.First();

            // 목적지 도착.
            if (item.x == _to_x && item.y == _to_y)
                return item;

            // 아이템에 인접한 노드들을 openlist에 넣는다.
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x        = item.x + i;
                    var y        = item.y + k;
                    var new_item = new Node(x, y, _to_x, _to_y, item);

                    // 이미 처리한 노드
                    if (_close_list.Contains(new_item))
                        continue;

                    // 갈 수 없는 지형.
                    if (_collision.IsCollision(x, y))
                        continue;

                    // open_list에 이미 있다면 cost 비교 
                    if (_open_list.TryGetValue(new_item, out var old_item))
                    {
                        if (old_item.TotalCost <= new_item.TotalCost)
                        {
                            // 기존 Cost가 더 작다면 추가하지 않는다.
                            continue;
                        }

                        // 기존 아이템 제거.
                        _open_list.Remove(old_item);
                    }

                    // openlist에 추가.
                    _open_list.Add(new_item);
                }
            }

            // openlist 항목중 가장 TotalCost가 작은 것을 closelist에 넣는다.
            if (0 < _open_list.Count)
            {
                var move_item = _open_list.First();
                _close_list.Add(move_item);
                _open_list.Remove(move_item);
            }
        }        

        return null;
    }



}