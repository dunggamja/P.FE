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
    public struct PathFindOption
    {
        public (bool check, int range, (int x, int y) base_pos)     MoveRange { get; private set; }

        public PathFindOption SetMoveRange(bool _check, int _range, (int x, int y) _base_pos)
        {
            MoveRange = (_check, _range, _base_pos);
            return this;
        }

        public static PathFindOption EMPTY => new PathFindOption()
        {
            MoveRange = (false, 0, (0, 0))
        };
    }

    class MoveRangeCheck : IFloodFillVisitor, IPoolObject
    {
        public TerrainMap              TerrainMap   { get; set; }  
        public IPathOwner              Visitor    { get; set; }  
        public (int x, int y)          Position     { get; set; }  
        public int                     MoveDistance { get; set; }

        public HashSet<(int x, int y)> VisitList { get; set; } = new();

        public void Reset()
        {
            VisitList.Clear();
        }

        public void Visit(int x, int y)
        {
            VisitList.Add((x, y));
        }

        public bool IsInMoveRange(int _x, int _y)
        {
            return VisitList.Contains((_x, _y));
        }
    }
    

    public static void PathFind(
        ref List<PathNode> _path_nodes, 
        TerrainMap         _terrain_map, 
        IPathOwner         _path_owner, 
        (int x, int y)     _from_cell,
        (int x, int y)     _to_cell,
        PathFindOption     _option)
    {
        if (_path_nodes == null)
            return;

        _path_nodes.Clear();

        var list_node = ListPool<Node>.Acquire();

        // A* 로 길찾기. 못찾으면 null 반환.
        if (AStar(ref list_node, _terrain_map, _path_owner, _from_cell, _to_cell, _option))
        {
            foreach(var e in list_node)
            {
                _path_nodes.Add(new PathNode(e.x, e.y));
            }
        }

        ListPool<Node>.Return(list_node);
    }


    static bool AStar(
        ref List<Node>          _path_find_list,
        TerrainMap              _terrain_map, 
        IPathOwner              _path_owner, 
        (int x, int y)          _from_cell,
        (int x, int y)          _to_cell,
        PathFindOption          _option)
    {
        if (_terrain_map == null)
            return false;

        
        // 시작 지점. 목표지점에 이동이 가능한지 체크합니다.
        if (!Verify_Movecost(_terrain_map, _path_owner, _to_cell, _is_occupancy:true).result)
                return false;                    

        if (!Verify_Movecost(_terrain_map, _path_owner, _from_cell, _is_occupancy:false).result)
            return false;

        
        var open_list        = ListPool<Node>.Acquire();
        var close_list       = ListPool<Node>.Acquire();
        var move_range_check = (_option.MoveRange.check) ? ObjectPool<MoveRangeCheck>.Acquire() : null;

        // 이동 범위 체크가 필요할 경우, FloodFill을 실행하여 이동범위를 미리 계산해둡시다.
        if (move_range_check != null)
        {
            move_range_check = ObjectPool<MoveRangeCheck>.Acquire();
            move_range_check.TerrainMap   = _terrain_map;
            move_range_check.Visitor    = _path_owner;
            move_range_check.Position     = _option.MoveRange.base_pos;
            move_range_check.MoveDistance = _option.MoveRange.range;
            PathAlgorithm.FloodFill(move_range_check);
        }


        // 시작 지점을 넣습니다.
        open_list.Add(new Node(_from_cell.x, _from_cell.y, _to_cell.x, _to_cell.y, 0, null));

        // 남은 거리가 가장 적은 노드를 찾아봅시다.
        Node func_find_minimum_heuristic(List<Node> _list_node)
        {
            if (_list_node == null)
                return null;

            Node minimum = null;         
            foreach (var e in _list_node)
            {
                if (e == null || (minimum != null && minimum.heuristic <= e.heuristic))
                    continue;

                minimum = e;
            }

            return minimum;
        }

        // 도착 노드.
        Node goal_node = null;

        while(0 < open_list.Count)
        {
            // 남은 거리가 가장 가까운 노드를 가져온다.
            var item = func_find_minimum_heuristic(open_list);

            // 사용한 노드는 open_list에서 제거 후 close_list에 추가.
            open_list.Remove(item);
            close_list.Add(item);

            // 목표 지점에 도달.
            if ((item.x, item.y) == _to_cell)
            {
                goal_node = item;
                break;
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

                    // 이동 범위 체크.
                    if (move_range_check != null && !move_range_check.IsInMoveRange(x, y))
                        continue;

                    // 이동 가능한 지 체크.
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, (x, y), _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // 새로운 노드 데이터 
                    var new_item  = new Node(x, y, _to_cell.x, _to_cell.y, move_cost, item);

                    // 같은 좌표에 속하는 노드가 있을 경우 비교 후 변경.
                    var old_item = open_list.Find((e) => e.x == new_item.x && e.y == new_item.y);
                    if (old_item != null && old_item.cost < new_item.cost)
                        continue;                    

                    // 겹치는 노드 제거.
                    if (old_item != null)
                        open_list.Remove(old_item);

                    open_list.Add(new_item);

                }
            }
        }        

        // 오브젝트 반환
        ListPool<Node>.Return(open_list);
        ListPool<Node>.Return(close_list);
        ObjectPool<MoveRangeCheck>.Return(move_range_check);

        if (goal_node != null)
        {
            // 길찾기 성공.
            if (_path_find_list != null)
            {
                _path_find_list.Clear();    

                while(goal_node != null)
                {
                    _path_find_list.Add(goal_node);
                    goal_node = goal_node.parent;
                }

                // 뒤집어 줘야함.
                _path_find_list.Reverse();
            }

            return true;
        }
        else
        {
            // 길찾기 실패.
            return false;
        }
    }



    // class floodfill_node : IPoolObject
    // {        
    //     // List<(int x, int y, int move_cost)> Nodes = new(10);
    //     HashSet<(int x, int y, int move_cost)> Nodes = new();
        

    //     public int Count => Nodes.Count;


    //     public void Reset()
    //     {
    //         Nodes.Clear();
    //     }

    //     public void Add((int x, int y, int move_cost) _item)
    //     {
    //         Nodes.Add(_item);
    //     }

    //     public void Remove((int x, int y, int move_cost) _item)
    //     {
    //         Nodes.Remove(_item);
    //     }

    //     public bool Contains((int x, int y, int move_cost) _item)
    //     {
    //         return Nodes.Contains(_item);
    //     }

    //     public (int x, int y, int move_cost) FindMinimumCostNode()
    //     {
    //         var min_cost_node = (x:0, y:0, move_cost:int.MaxValue);
    //         foreach (var node in Nodes)
    //         {
    //             if (node.move_cost < min_cost_node.move_cost)
    //             {
    //                 min_cost_node = node;
    //             }
    //         }

    //         return min_cost_node;
    //     }
    // }

    public interface IFloodFillVisitor
    {
        TerrainMap     TerrainMap   { get; }
        IPathOwner     Visitor      { get; }
        (int x, int y) Position     { get; }
        int            MoveDistance { get; }


        void Visit(int x, int y);
    }

    static public void FloodFill(IFloodFillVisitor _visitor)
    {
        // _visitor 없으면 아무것도 하지 않습니다.    
        if (_visitor == null)
            return;

        var open_list_move    = HashSetPool<(int x, int y, int move_cost)>.Acquire();
        var close_list_move   = HashSetPool<(int x, int y, int move_cost)>.Acquire();

        open_list_move.Add((_visitor.Position.x, _visitor.Position.y, 0));
        // Debug.Log($"FloodFill, Start, x:{_position.x}, y:{_position.y}");

        while(open_list_move.Count > 0)
        {
            // movecost가 가장 적은 아이템을 가져옵니다.            
            var item = (x:0, y:0, move_cost:int.MaxValue);
            
            foreach(var e in open_list_move)
            {
                if (e.move_cost < item.move_cost) 
                    item = e;
            }            

            // 내가 점유가능한 위치에서만 Visit을 실행합니다. 
            var call_visit = (item.x == _visitor.Position.x && item.y == _visitor.Position.y)
                           || Verify_Movecost(_visitor.TerrainMap, _visitor.Visitor, (item.x, item.y), _is_occupancy:true).result;
                                    
            if (call_visit)
                _visitor.Visit(item.x, item.y);

            // open/close list 셋팅
            open_list_move.Remove(item);
            close_list_move.Add((item.x, item.y, 0));
            // Debug.Log($"FloodFill, CloseList Add, x:{item.x}, y:{item.y}");

            // 이동 가능 지역 탐색. (FloodFill)
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x = item.x + i;
                    var y = item.y + k;                    

                    // 이미 체크하였음.
                    if (close_list_move.Contains((x, y, 0)))
                        continue;

                    // 가로, 세로 1칸씩만 이동가능. (대각선 이동 없음)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // Debug.Log($"FloodFill, x:{x}, y:{y}");

                    // 통과 가능한 지역인지 체크합니다.
                    (var moveable, var move_cost) = Verify_Movecost(_visitor.TerrainMap, _visitor.Visitor, (x, y), _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // 이동 범위 초과.
                    var total_cost = item.move_cost + move_cost;
                    if (total_cost > _visitor.MoveDistance)
                    {
                        continue;
                    }

                    // open_list 에 추가.
                    open_list_move.Add((x, y, total_cost));
                }
            }   
        }

        HashSetPool<(int x, int y, int move_cost)>.Return(open_list_move);
        HashSetPool<(int x, int y, int move_cost)>.Return(close_list_move);
        // Debug.Log($"FloodFill, Complete, x:{_position.x}, y:{_position.y}");            
    }

    static (bool result, int move_cost) Verify_Movecost(
        TerrainMap     _terrain_map, 
        IPathOwner     _path_owner, 
        (int x, int y) _cell,
         bool          _is_occupancy)
    {
        if (_terrain_map == null || _path_owner == null)
            return (false, 0);
        
        if (_cell.x < 0 || _cell.y < 0 ||
            _terrain_map.Width <= _cell.x || _terrain_map.Height <= _cell.y)
            return (false, 0);  
  
        // ZOC에 막히는지 체크합니다. (목표지점은 완전히 비어있어야 함.)
        if (_terrain_map.ZOC.IsBlockedZOC(_cell.x, _cell.y, 
            (_is_occupancy) ? 0 : _path_owner.PathZOCFaction))        
            return (false, 0);

        
        // 이동 Cost 계산.
        var move_cost = Terrain_Attribute.Calculate_MoveCost(
            _path_owner.PathAttribute, 
            _terrain_map.Attribute.GetAttribute(_cell.x, _cell.y));

        if (move_cost <= 0)
            return (false, 0);

        return (true, move_cost);      
    }


}