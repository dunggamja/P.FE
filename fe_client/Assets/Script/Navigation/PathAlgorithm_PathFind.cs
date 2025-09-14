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
    // FIND OPTION
    public struct PathFindOption
    {
        // 출발지점으로 부터 이동 가능한 범위를 체크.
        public (bool check, int range, (int x, int y) base_pos)     
                                MoveRange { get; private set; } 

        public PathFindOption SetMoveRange(bool _check, int _range, (int x, int y) _base_pos)
        {
            MoveRange = (_check, _range, _base_pos);
            return this;
        }


        public static PathFindOption EMPTY => new PathFindOption()
        {
            MoveRange = (false, 0, (0, 0)),
        };
    }

    class MoveRangeCheck : IFloodFillVisitor, IPoolObject
    {
        public TerrainMap              TerrainMap   { get; set; }  
        public IPathOwner              Visitor      { get; set; }  
        public (int x, int y)          Position     { get; set; }  
        public int                     MoveDistance { get; set; }
        public bool                    VisitOnlyEmptyCell => false;
        public bool                    IsStop() => false;

        public HashSet<(int x, int y)> VisitList { get; set; } = new();

        public void Reset()
        {
            VisitList.Clear();
        }


        public void Visit(int x, int y)
        {
            VisitList.Add((x, y));
            // return true;
        }

        public bool IsInMoveRange(int _x, int _y)
        {
            return VisitList.Contains((_x, _y));
        }
    }
    

    public static (bool result, int move_distance) PathFind(
        TerrainMap         _terrain_map, 
        IPathOwner         _path_owner, 
        (int x, int y)     _from_cell,
        (int x, int y)     _to_cell,
        PathFindOption     _option,
        List<PathNode>     _path_nodes = null)
    {
        var list_node = ListPool<Node>.Acquire();

        try
        {
            // A* 검색. 검색 결과가 null 이면 검색 실패.
            if (AStar(ref list_node, _terrain_map, _path_owner, _from_cell, _to_cell, _option))
            {
                if (_path_nodes != null)
                {
                    _path_nodes.Clear();
                    foreach(var e in list_node)
                        _path_nodes.Add(new PathNode(e.x, e.y, -1, e.cost));
                }        

                return (true, list_node.Count);
            }
        }
        finally
        {

            ListPool<Node>.Return(ref list_node);
        }

        return (false, 0);
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

        
        // 목적지 점유 가능한지 체크.
        if (!Verify_Movecost(
            _terrain_map,
            _path_owner,
            _to_cell,
            _check_ignore_zoc:false).result)
            return false;                    

        // 출발지가 이동가능한지 체크.
        if (!Verify_Movecost(
            _terrain_map, 
            _path_owner, 
            _from_cell, 
            _check_ignore_zoc:true).result)
            return false;


        // 풀 사용.
        var open_list        = ListPool<Node>.Acquire();
        var close_list       = ListPool<Node>.Acquire();
        
        // 이동 범위를 벗어나지 않아야 하는 경우 
        var move_range_check = (_option.MoveRange.check) ? ObjectPool<MoveRangeCheck>.Acquire() : null;
        if (move_range_check != null)
        {
            move_range_check.TerrainMap   = _terrain_map;
            move_range_check.Visitor      = _path_owner;
            move_range_check.Position     = _option.MoveRange.base_pos;
            move_range_check.MoveDistance = _option.MoveRange.range;
            PathAlgorithm.FloodFill(move_range_check);
        }

        // 목표지.
        Node goal_node = null;


        try
        {       
            // 출발지를 추가.
            open_list.Add(new Node(_from_cell.x, _from_cell.y, _to_cell.x, _to_cell.y, 0, null));

            // Debug.Log($"start:{_from_cell.x}, {_from_cell.y}");

            // 최소 비용을 찾는 함수.
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

            

            while(0 < open_list.Count)
            {
                // 최소 비용을 찾는 함수.
                var item = func_find_minimum_heuristic(open_list);

                // 최소 비용을 찾은 노드를 제거하고 추가.
                open_list.Remove(item);
                close_list.Add(item);
                // Debug.Log($"close:{item.x}, {item.y}");

                // 목표지에 도달했는지 체크.
                if ((item.x, item.y) == _to_cell)
                {
                    goal_node = item;
                    // Debug.Log($"goal:{item.x}, {item.y}");
                    break;
                }

                // 이동 가능한 좌표를 찾는다.
                for(int i = -1; i <= 1; ++i)
                {
                    for(int k = -1; k <= 1; ++k)
                    {
                        var x         = item.x + i;
                        var y         = item.y + k;

                        // 1칸 이상 이동하는지 체크.
                        if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                            continue;

                        // 이미 방문한 좌표인지 체크.
                        if (0 <= close_list.FindIndex(e => e.x == x && e.y == y))
                            continue;

                        // 이동 범위 체크.
                        if (move_range_check != null && !move_range_check.IsInMoveRange(x, y))
                            continue;

                        // 이동 가능한지 체크.
                        (var moveable, var move_cost) = Verify_Movecost(
                            _terrain_map, 
                            _path_owner, 
                            (x, y), 
                            _check_ignore_zoc:true);

                        if (!moveable)
                            continue;

                        // 새로운 노드 생성.
                        var new_item  = new Node(x, y, _to_cell.x, _to_cell.y, move_cost, item);

                        // 이미 방문한 노드인지 체크.
                        var old_item = open_list.Find((e) => e.x == new_item.x && e.y == new_item.y);
                        if (old_item != null && old_item.cost < new_item.cost)
                            continue;                    

                        // 이미 방문한 노드인지 체크.
                        if (old_item != null)
                            open_list.Remove(old_item);

                        open_list.Add(new_item);
                        // Debug.Log($"open:{new_item.x}, {new_item.y}");

                    }
                }
            }
        }
        finally
        {
            ListPool<Node>.Return(ref open_list);
            ListPool<Node>.Return(ref close_list);
            ObjectPool<MoveRangeCheck>.Return(ref move_range_check);            
        }        

        // 결과 반환.
        

        if (goal_node != null)
        {
            // 결과 반환.
            if (_path_find_list != null)
            {
                _path_find_list.Clear();    

                while(goal_node != null)
                {
                    _path_find_list.Add(goal_node);
                    goal_node = goal_node.parent;
                }

                // 역순으로 정렬.
                _path_find_list.Reverse();
            }

            return true;
        }
        else
        {
            // 결과 반환.
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
        TerrainMap     TerrainMap         { get; }
        IPathOwner     Visitor            { get; }
        (int x, int y) Position           { get; }
        int            MoveDistance       { get; }
        bool           VisitOnlyEmptyCell { get; }


        bool  IsStop();
        void  Visit(int x, int y);
    }

    static public void FloodFill(IFloodFillVisitor _visitor)
    {
        // _visitor ������ �ƹ��͵� ���� �ʽ��ϴ�.    
        if (_visitor == null)
            return;

        var open_list_move    = HashSetPool<(int x, int y, int move_cost)>.Acquire();
        var close_list_move   = HashSetPool<(int x, int y, int move_cost)>.Acquire();

        open_list_move.Add((_visitor.Position.x, _visitor.Position.y, 0));
        // Debug.Log($"FloodFill, Start, x:{_position.x}, y:{_position.y}");

        while(open_list_move.Count > 0)
        {
            // 중단 조건 체크.
            if (_visitor.IsStop())
                break;

            // movecost 최소값 찾기.            
            var item = (x:0, y:0, move_cost:int.MaxValue);
            
            foreach(var e in open_list_move)
            {
                if (e.move_cost < item.move_cost) 
                    item = e;
            }            

            // #1. 시작 위치 체크. 
            var is_start_position = (item.x == _visitor.Position.x && item.y == _visitor.Position.y);

            // #2. 이동 가능한지 체크.
            var verify_move_cost  = Verify_Movecost(
                _visitor.TerrainMap,
                _visitor.Visitor, 
                (item.x, item.y), 
                _check_ignore_zoc: _visitor.VisitOnlyEmptyCell == false)
                .result;

            // 시작 위치 or 이동 가능한지 체크.
            var call_visit = (is_start_position) || verify_move_cost;                                    
            if (call_visit)
            {
                _visitor.Visit(item.x, item.y);                
            }

            // open/close list 추가.
            open_list_move.Remove(item);
            close_list_move.Add((item.x, item.y, 0));
            // Debug.Log($"FloodFill, CloseList Add, x:{item.x}, y:{item.y}");

            // 이동 가능한 좌표 찾기. (FloodFill)
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x = item.x + i;
                    var y = item.y + k;                    

                    // 이미 방문한 좌표인지 체크.
                    if (close_list_move.Contains((x, y, 0)))
                        continue;

                    // 1칸 이상 이동하는지 체크. 
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // Debug.Log($"FloodFill, x:{x}, y:{y}");

                    // 이동 가능한지 체크.
                    (var moveable, var move_cost) = Verify_Movecost(
                        _visitor.TerrainMap,
                        _visitor.Visitor, 
                        (x, y), 
                        _check_ignore_zoc:true);


                    if (!moveable)
                        continue;

                    // 이동 가능한지 체크.
                    var total_cost = item.move_cost + move_cost;
                    if (total_cost > _visitor.MoveDistance)
                    {
                        continue;
                    }

                    // open_list 추가.
                    open_list_move.Add((x, y, total_cost));
                }
            }   
        }

        HashSetPool<(int x, int y, int move_cost)>.Return(ref open_list_move);
        HashSetPool<(int x, int y, int move_cost)>.Return(ref close_list_move);
        // Debug.Log($"FloodFill, Complete, x:{_visitor.Position.x}, y:{_visitor.Position.y}");            
    }

    static (bool result, int move_cost) Verify_Movecost(
        TerrainMap     _terrain_map, 
        IPathOwner     _path_owner, 
        (int x, int y) _cell,
         bool          _check_ignore_zoc)
    {
        if (_terrain_map == null || _path_owner == null)
            return (false, 0);
        
        if (_terrain_map.IsInBound(_cell.x, _cell.y) == false)
            return (false, 0);  
  
        // ZOC 체크 여부.
        Func<int, bool> func_ignore_zoc = null;        
        if (_check_ignore_zoc)
            func_ignore_zoc = _path_owner.IsIgnoreZOC;

        if (_terrain_map.ZOC.IsBlockedZOC(_cell.x, _cell.y, func_ignore_zoc))        
            return (false, 0);

        
        // 이동 Cost 계산
        var move_cost = Terrain_Attribute.Calculate_MoveCost(
            _path_owner.PathAttribute, 
            _terrain_map.Attribute.GetAttribute(_cell.x, _cell.y));

        if (move_cost <= 0)
            return (false, 0);

        return (true, move_cost);      
    }


}