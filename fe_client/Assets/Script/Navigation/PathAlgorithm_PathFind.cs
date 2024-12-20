using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static partial class PathAlgorithm
{
    
    public static List<PathNode> PathFind(TerrainMap _terrain_map, IPathOwner _path_owner, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        var result     = new List<PathNode>(20);

        // A* 
        var    node  = AStar(_terrain_map, _path_owner, _from_x, _from_y, _to_x, _to_y);
        while (node != null)
        {
            result.Add(new PathNode(node.x, node.y));
            node = node.parent;
        }

        // 
        result.Reverse();

        return result;
    }


    static Node AStar(TerrainMap _terrain_map, IPathOwner _path_owner, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        if (_terrain_map == null)
            return null;

         // 목표 지점에 이동 가능한 지 체크.
        if (!Verify_Movecost(_terrain_map, _path_owner, _to_x, _to_y, _is_occupancy:true).result)
            return null;                    

        var open_list  = new List<Node>(20);
        var close_list = new List<Node>(20);

        // 시작 지점을 넣습니다.
        open_list.Add(new Node(_from_x, _from_y, _to_x, _to_y, 0, null));

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
            {
                return item;
            }

            // 주변 노드 탐색.
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x         = item.x + i;
                    var y         = item.y + k;

                    // 가로, 세로 1칸씩만 이동가능. (대각선 이동 없음)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // 이미 검사한 지점인지 체크.
                    if (0 <= close_list.FindIndex(e => e.x == x && e.y == y))
                        continue;

                    // 이동 가능한 지 체크.
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, x, y, _is_occupancy:false);
                    if (!moveable)
                        continue;

                    var new_item  = new Node(x, y, _to_x, _to_y, move_cost, item);
                    


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



    public static void FloodFill(
        TerrainMap              _terrain_map,
        IPathOwner              _path_owner,
        (int x, int y)          _position,
        int                     _move_distance,        
        Action<(int x, int y)>  _func_on_cell = null,
        bool                    _is_call_func_any_cell = false)
    {
        // callback이 없으면 아무것도 하지 않습니다.    
        if (_func_on_cell == null)
            return;


        var open_list_move    = new List<(int x, int y, int move_cost)>(10);
        var close_list_move   = new List<(int x, int y)>(10);

        open_list_move.Add((_position.x, _position.y, 0));
        // Debug.Log($"FloodFill, Start, x:{_position.x}, y:{_position.y}");

        while(open_list_move.Count > 0)
        {
            // movecost가 가장 적은 아이템을 가져옵니다.            
            var item = open_list_move.Aggregate(open_list_move.First(), (a, b) => a.move_cost < b.move_cost ? a : b);

            // callback 호출 여부 체크. 
            var call_func_on_cell = _is_call_func_any_cell ||
                                    (item.x == _position.x && item.y == _position.y) ||
                                    Verify_Movecost(_terrain_map, _path_owner, item.x, item.y, _is_occupancy:true).result;
                                    
            if (call_func_on_cell)
                _func_on_cell((item.x, item.y));

            // open/close list 셋팅
            open_list_move.Remove(item);
            close_list_move.Add((item.x, item.y));
            // Debug.Log($"FloodFill, CloseList Add, x:{item.x}, y:{item.y}");

            // 이동 가능 지역 탐색. (FloodFill)
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x = item.x + i;
                    var y = item.y + k;                    

                    // 이미 체크하였음.
                    if (close_list_move.Contains((x, y)))
                        continue;

                    // 가로, 세로 1칸씩만 이동가능. (대각선 이동 없음)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // Debug.Log($"FloodFill, x:{x}, y:{y}");

                    // 통과 가능한 지역인지 체크합니다.
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, x, y, _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // 이동 범위 초과.
                    var total_cost = item.move_cost + move_cost;
                    if (total_cost > _move_distance)
                    {
                        continue;
                    }

                    // open_list 에 추가.
                    open_list_move.Add((x, y, total_cost));
                }
            }   
        }

        // Debug.Log($"FloodFill, Complete, x:{_position.x}, y:{_position.y}");
            
    }

    static (bool result, int move_cost) Verify_Movecost(TerrainMap _terrain_map, IPathOwner _path_owner, int _x, int _y, bool _is_occupancy)
    {
        if (_terrain_map == null || _path_owner == null)
            return (false, 0);
        
        if (_x < 0 || _y < 0 || _terrain_map.Width <= _x || _terrain_map.Height <= _y)
            return (false, 0);  
  
        // ZOC에 막히는지 체크합니다. (목표지점은 완전히 비어있어야 함.)
        if (_terrain_map.ZOC.IsBlockedZOC(_x, _y, (_is_occupancy) ? 0 : _path_owner.PathZOCFaction))
            return (false, 0);

        // 이동 Cost 계산.
        var move_cost = Terrain_Attribute.Calculate_MoveCost(_path_owner.PathAttribute, _terrain_map.Attribute.GetAttribute(_x, _y));
        if (move_cost <= 0)
            return (false, 0);

        return (true, move_cost);      
    }


}