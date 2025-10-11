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
    public enum EnumCheckZOC
    {
        None,        // ZOC 검사 없음.

        Occupy,      // 점유를 가능한지 체크.

        PassThrough, // 통과가 가능한지 체크.
    }

    // FIND OPTION
    public struct PathFindOption
    {
        // 출발지점으로 부터 이동 가능한 범위를 체크.
        public (bool check, int range, (int x, int y) base_pos)     
                                MoveLimitRange { get; private set; } 


        // 도착지점까지의 범위 체크. 
        public (bool check, int range) GoalRange { get; private set; }


        // TRUE: 목표지점이 점유되어있는지 체크하지 않음, FALSE: 목표지점은 점유가능해야함.
        public bool  MoveApproximately { get; private set; }

        

        public PathFindOption SetMoveLimitRange(int _range, (int x, int y) _base_pos)
        {
            MoveLimitRange = (true, _range, _base_pos);
            return this;
        }

        public PathFindOption SetGoalRange(int _range)
        {
            GoalRange = (true, _range);
            return this;
        }

        public PathFindOption SetAllowApproximate()
        {
            MoveApproximately = true;
            return this;
        }


        public static PathFindOption Create() => new PathFindOption()
        {
            MoveLimitRange    = (false, 0, (0, 0)),
            GoalRange         = (false, 0),
            MoveApproximately = false,
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
    



    public static (bool result, int move_cost) PathFind(
        TerrainMap         _terrain_map, 
        IPathOwner         _path_owner, 
        (int x, int y)     _from_cell,
        (int x, int y)     _to_cell,
        PathFindOption     _option     = default,
        List<PathNode>     _path_nodes = null)
    {
        var list_node = ListPool<Node>.Acquire();

        try
        {
            // A* 검색. 검색 결과가 null 이면 검색 실패.
            if (AStar(list_node, _terrain_map, _path_owner, _from_cell, _to_cell, _option))
            {
                if (_path_nodes != null)
                {
                    _path_nodes.Clear();
                    foreach(var e in list_node)
                        _path_nodes.Add(new PathNode(e.x, e.y, e.cost));                                        
                } 


                var move_cost = (0 < list_node.Count) ? list_node[list_node.Count - 1].cost : 0;

                return (true, move_cost);
            }
        } 
        finally
        {

            ListPool<Node>.Return( list_node);
        }

        return (false, 0);
    }


    static bool Verify_GoalRange((int x, int y) _cell, (int x, int y) _goal, int _goal_range)
         => PathAlgorithm.Distance(_cell.x, _cell.y, _goal.x, _goal.y) <= _goal_range;   

    // 길찾기 결과 반환.
    static void CredatePathList(
        Node                             _node,
        Dictionary<(int x, int y), Node> _close_list,
        List<Node>                       _path_find_list)
    {
        _path_find_list.Clear();    

        while(true)
        {
            _path_find_list.Add(_node);

            // 부모 노드가 없다면 끝.
            if (_node.HasParent == false)
                break;
                            
            _close_list.TryGetValue((_node.parent_node.x, _node.parent_node.y), out _node);
        } 

        // 역순으로 정렬.
        _path_find_list.Reverse();
    }


    static bool AStar(
        List<Node>      _path_find_list,
        TerrainMap      _terrain_map, 
        IPathOwner      _path_owner, 
        (int x, int y)  _start_cell,
        (int x, int y)  _goal_cell,
        PathFindOption  _option)
    {

        if (_terrain_map == null)
            return false;


        // 목표지점 체크 로직.
        var check_goal_zoc = _option.MoveApproximately ? EnumCheckZOC.None : EnumCheckZOC.Occupy;

        // 통과지점 체크 로직.
        var check_pass_zoc = _option.MoveApproximately ? EnumCheckZOC.None    : EnumCheckZOC.PassThrough;

        // 목표지점 범위 체크.
        var goal_range     = _option.GoalRange.check ? _option.GoalRange.range: 0;

        // 목적지 점유 가능한지 체크.   
        var verify_goal_movecost = false;

        // 목적지를 범위로 체크해봅시다.
        for(int y = -goal_range; y <= goal_range && verify_goal_movecost == false; ++y)
        {
            for(int x = -goal_range; x <= goal_range && verify_goal_movecost == false; ++x)
            {                   
                if (PathAlgorithm.Distance(0, 0, x, y) <= goal_range)
                {
                    verify_goal_movecost |=  Verify_Movecost(
                                            _terrain_map,
                                            _path_owner,
                                            (_goal_cell.x + x, _goal_cell.y + y),
                                            check_goal_zoc).result;
                }                
            }
        }

        // 목적지를 범위로 체크해봤는데 이동 가능한 좌표가 없다면 실패.
        if (verify_goal_movecost == false)
            return false;


        // // 출발지가 이동가능한지 체크.
        // if (!Verify_Movecost(
        //     _terrain_map, 
        //     _path_owner, 
        //     _start_cell, 
        //     _check_ignore_zoc:true).result)
        //     return false;


        // 풀 사용.
        var open_list        = DictionaryPool<(int x, int y),Node>.Acquire();
        var close_list       = DictionaryPool<(int x, int y), Node>.Acquire();
        

        
        // 이동 범위를 벗어나지 않아야 하는 경우 
        var move_range_check = (_option.MoveLimitRange.check) ? ObjectPool<MoveRangeCheck>.Acquire() : null;
        if (move_range_check != null)
        {
            move_range_check.TerrainMap   = _terrain_map;
            move_range_check.Visitor      = _path_owner;
            move_range_check.Position     = _option.MoveLimitRange.base_pos;
            move_range_check.MoveDistance = _option.MoveLimitRange.range;
            PathAlgorithm.FloodFill(move_range_check);
        }



        try
        {   
            // 출발 노드            
            var node_start       = Node.Create(_start_cell.x, _start_cell.y, _goal_cell.x, _goal_cell.y);

            // 목표에 가장 근접한 노드. (길찾기 실패시 사용?)
            // var node_approximate = node_start;

            // 출발 노드를 open_list에 추가.
            open_list.Add((node_start.x, node_start.y), node_start);        


            // 최소 비용을 찾는 함수.
            Node func_find_minimum_heuristic(Dictionary<(int x, int y), Node> _list_node)
            {
                Node minimum = _list_node.First().Value;
                foreach (var e in _list_node.Values)
                {
                    if (e.heuristic < minimum.heuristic)
                        minimum = e;
                }
                return minimum;
            }

            while(0 < open_list.Count)
            {
                // 최소 비용을 찾는 함수.
                var item = func_find_minimum_heuristic(open_list);

                open_list.Remove((item.x, item.y));
                close_list.Add((item.x, item.y), item);

                
                // 목표지에 도달했는지 체크.
                 if (PathAlgorithm.Distance(_goal_cell.x, _goal_cell.y, item.x, item.y) <= goal_range)
                {                    
                    // 길찾기 결과 반환.
                    if (_path_find_list != null)
                        CredatePathList(item, close_list, _path_find_list);

                    // 길찾기 성공.
                    return true;
                }

                // 이동 가능한 좌표를 찾는다.
                for(int i = -1; i <= 1; ++i)
                {
                    for(int k = -1; k <= 1; ++k)
                    {
                        var x = item.x + i;
                        var y = item.y + k;

                        // 1칸 이상 이동하는지 체크.
                        if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                            continue;

                        // 이미 방문한 좌표인지 체크.
                        if (close_list.ContainsKey((x, y)))
                            continue;

                        // 이동 범위 체크.
                        if (move_range_check != null && !move_range_check.IsInMoveRange(x, y))
                            continue;


                        // 이동 가능한지 체크.  
                        (var moveable, var move_cost) = Verify_Movecost(
                            _terrain_map, 
                            _path_owner, 
                            (x, y), 
                            check_pass_zoc);

                        if (!moveable)
                            continue;

                        // 새로운 노드 생성.
                        var new_item  = new Node(
                            // 현재 셀
                            x, y,

                            // 목표 지점
                            _goal_cell.x, _goal_cell.y, 

                            // 이동 비용= 이전 까지의 비용 + 현재 셀의 이동 비용.
                            item.cost + move_cost, 

                            // 부모 노드 셋팅
                            (item.x, item.y));

                        
                        if (open_list.TryGetValue((new_item.x, new_item.y), out var old_item))
                        {
                            // 기존 노드와 위치가 겹칠경우 cost 비교.                            
                            if (new_item.cost < old_item.cost)                            
                                open_list[(new_item.x, new_item.y)] = new_item;
                        }                    
                        else 
                        {
                            // open list에 추가.
                            open_list.Add((new_item.x, new_item.y), new_item);
                        }
                    }
                }
            }

            // // 근처에 도달한 값이 있으면 그걸 성공처리?
            // if (node_approximate.cost > node_start.cost)
            // {
            //     CredatePathList(node_approximate, close_list, _path_find_list);
            //     return true;
            // }
        }
        finally
        {
            DictionaryPool<(int x, int y), Node>.Return( open_list);
            DictionaryPool<(int x, int y), Node>.Return( close_list);
            ObjectPool<MoveRangeCheck>.Return( move_range_check);            
        } 
       
        // 길찾기 실패.
        return false;
    }



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
            var item = open_list_move.First();            
            foreach(var e in open_list_move)
            {
                if (e.move_cost < item.move_cost) 
                    item = e;
            }            

            // #1. 시작 위치 체크. 
            var is_start_position = (item.x == _visitor.Position.x && item.y == _visitor.Position.y);

            // 점유 가능한지 체크.
            var check_zoc = (_visitor.VisitOnlyEmptyCell) ? EnumCheckZOC.Occupy : EnumCheckZOC.PassThrough;

            // #2. 방문 가능한지 체크.
            var enable_visit  = Verify_Movecost(
                _visitor.TerrainMap,
                _visitor.Visitor, 
                (item.x, item.y), 
                check_zoc)
                .result;

            // 시작 위치 or 방문 가능한지 체크.
            var call_visit = (is_start_position) || enable_visit;                                    
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
                        _check_zoc: EnumCheckZOC.PassThrough);


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

        HashSetPool<(int x, int y, int move_cost)>.Return( open_list_move);
        HashSetPool<(int x, int y, int move_cost)>.Return( close_list_move);
        // Debug.Log($"FloodFill, Complete, x:{_visitor.Position.x}, y:{_visitor.Position.y}");            
    }

    static (bool result, int move_cost) Verify_Movecost(
        TerrainMap     _terrain_map, 
        IPathOwner     _path_owner, 
        (int x, int y) _cell,
        EnumCheckZOC   _check_zoc)
    {
        if (_terrain_map == null || _path_owner == null)
            return (false, 0);
        
        if (_terrain_map.IsInBound(_cell.x, _cell.y) == false)
            return (false, 0);  
  

        // ZOC 체크 여부.
        if (_check_zoc != EnumCheckZOC.None)
        {
            // 통과가 목적일 경우, Entity의 통과 로직 체크.
            Func<int, bool> func_ignore_zoc = (_check_zoc == EnumCheckZOC.PassThrough) ?
                                               _path_owner.IsIgnoreZOC : null;    

            if (_terrain_map.ZOC.IsBlockedZOC(_cell.x, _cell.y, func_ignore_zoc))        
                return (false, 0);
        }

        
        // 이동 Cost 계산
        var move_cost = Terrain_Attribute.Calculate_MoveCost(
            _path_owner.PathAttribute, 
            _terrain_map.Attribute.GetAttribute(_cell.x, _cell.y));

        if (move_cost.cost <= 0)
            return (false, 0);

        return (true, move_cost.cost);      
    }


}