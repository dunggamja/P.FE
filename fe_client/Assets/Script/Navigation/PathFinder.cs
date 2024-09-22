using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static class PathFinder
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



    public static List<PathNode> Find(TerrainMap _terrain_map, int _move_attribute, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        

        var result     = new List<PathNode>(20);
        

        // A* 
        var    node  = AStar(_terrain_map, _move_attribute, _from_x, _from_y, _to_x, _to_y);
        while (node != null)
        {
            result.Add(new PathNode(node.x, node.y));
            node = node.parent;
        }

        // 
        result.Reverse();

        return result;
    }


    static Node AStar(TerrainMap _terrain_map, int _move_attribute, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        if (_terrain_map == null)
            return null;

        var open_list  = new List<Node>(20);
        var close_list = new List<Node>(20);

        // 시작 지점을 넣습니다.
        open_list.Add(new Node(_from_x, _from_y, _to_x, _to_y, null));

        // 남은 거리가 가장 적은 노드를 찾아봅시다.
        Node func_find_minimum_heuristic(List<Node> _list_node)
        {
            Node minimum = null;
            if (_list_node != null)
            {
                foreach (var e in _list_node)
                {
                    if ((minimum == null) || (e != null && e.heuristic < minimum.heuristic))
                    {
                        minimum = e;
                    }
                }
            }

            return minimum;
        }
        

        while(0 < open_list.Count)
        {
            // 남은 거리가 가장 가까운 노드를 가져온다.
            var item = func_find_minimum_heuristic(open_list);

            // 사용한 노드는 open_list에서 제거 후 close_list에 추가.
            open_list.Remove(item);
            close_list.Add(item);

            // 목표 지점에 도달.
            if (item.x == _to_x && item.y == _to_y)
                return item;

            // 주변 노드 탐색.
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var y         = item.y + i;
                    var x         = item.x + k;
                    var new_item  = new Node(x, y, _to_x, _to_y, item);

                    var is_diagonal = (i != 0) && (k != 0);
                    if (is_diagonal)
                    {
                        // 대각선 이동은 없음.
                        continue;

                        // 대각선 방향일 경우 양쪽 직선방향이 모두 열려있는지 체크해야 함.?
                        // var move_cost_diagonal_1 = TerrainAttribute.Calculate_MoveCost(_move_attribute, _terrain_map.Attribute.GetAttribute(x, item.y));
                        // var move_cost_diagonal_2 = TerrainAttribute.Calculate_MoveCost(_move_attribute, _terrain_map.Attribute.GetAttribute(item.x, y));
                        // if (_terrain_map.IsCollision(item.x + k, item.y) || _terrain_map.IsCollision(item.x, item.y + i))
                        //     continue;
                    }

                    // 이미 검사한 지점은 거른다.
                    if (close_list.Contains(new_item))
                        continue;

                    // 이동 Cost 계산.
                    var move_cost = TerrainAttribute.Calculate_MoveCost(_move_attribute, _terrain_map.Attribute.GetAttribute(x, y));

                    // 충돌 지점은 거른다.
                    if (move_cost <= 0)
                        continue;
                    

                    // 같은 좌표에 속하는 노드가 있을 경우 비교 후 변경.
                    var old_item = open_list.Find((e) => e.x == new_item.x && e.y == new_item.y);
                    if (old_item != null)
                    {
                        if (old_item.cost < new_item.cost)
                            continue;

                        open_list.Remove(old_item);
                    }

                    // 
                    open_list.Add(new_item);
                }
            }
        }        

        return null;
    }



}