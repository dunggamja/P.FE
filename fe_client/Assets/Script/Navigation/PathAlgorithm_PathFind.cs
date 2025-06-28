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

        // A* �� ��ã��. ��ã���� null ��ȯ.
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

        
        // ���� ����. ��ǥ������ �̵��� �������� üũ�մϴ�.
        if (!Verify_Movecost(_terrain_map, _path_owner, _to_cell, _is_occupancy:true).result)
                return false;                    

        if (!Verify_Movecost(_terrain_map, _path_owner, _from_cell, _is_occupancy:false).result)
            return false;

        
        var open_list        = ListPool<Node>.Acquire();
        var close_list       = ListPool<Node>.Acquire();
        var move_range_check = (_option.MoveRange.check) ? ObjectPool<MoveRangeCheck>.Acquire() : null;

        // �̵� ���� üũ�� �ʿ��� ���, FloodFill�� �����Ͽ� �̵������� �̸� ����صӽô�.
        if (move_range_check != null)
        {
            move_range_check = ObjectPool<MoveRangeCheck>.Acquire();
            move_range_check.TerrainMap   = _terrain_map;
            move_range_check.Visitor    = _path_owner;
            move_range_check.Position     = _option.MoveRange.base_pos;
            move_range_check.MoveDistance = _option.MoveRange.range;
            PathAlgorithm.FloodFill(move_range_check);
        }


        // ���� ������ �ֽ��ϴ�.
        open_list.Add(new Node(_from_cell.x, _from_cell.y, _to_cell.x, _to_cell.y, 0, null));

        // ���� �Ÿ��� ���� ���� ��带 ã�ƺ��ô�.
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

        // ���� ���.
        Node goal_node = null;

        while(0 < open_list.Count)
        {
            // ���� �Ÿ��� ���� ����� ��带 �����´�.
            var item = func_find_minimum_heuristic(open_list);

            // ����� ���� open_list���� ���� �� close_list�� �߰�.
            open_list.Remove(item);
            close_list.Add(item);

            // ��ǥ ������ ����.
            if ((item.x, item.y) == _to_cell)
            {
                goal_node = item;
                break;
            }

            // �ֺ� ��� Ž��.
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x         = item.x + i;
                    var y         = item.y + k;

                    // ����, ���� 1ĭ���� �̵�����. (�밢�� �̵� ����)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // �̹� �˻��� �������� üũ.
                    if (0 <= close_list.FindIndex(e => e.x == x && e.y == y))
                        continue;

                    // �̵� ���� üũ.
                    if (move_range_check != null && !move_range_check.IsInMoveRange(x, y))
                        continue;

                    // �̵� ������ �� üũ.
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, (x, y), _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // ���ο� ��� ������ 
                    var new_item  = new Node(x, y, _to_cell.x, _to_cell.y, move_cost, item);

                    // ���� ��ǥ�� ���ϴ� ��尡 ���� ��� �� �� ����.
                    var old_item = open_list.Find((e) => e.x == new_item.x && e.y == new_item.y);
                    if (old_item != null && old_item.cost < new_item.cost)
                        continue;                    

                    // ��ġ�� ��� ����.
                    if (old_item != null)
                        open_list.Remove(old_item);

                    open_list.Add(new_item);

                }
            }
        }        

        // ������Ʈ ��ȯ
        ListPool<Node>.Return(open_list);
        ListPool<Node>.Return(close_list);
        ObjectPool<MoveRangeCheck>.Return(move_range_check);

        if (goal_node != null)
        {
            // ��ã�� ����.
            if (_path_find_list != null)
            {
                _path_find_list.Clear();    

                while(goal_node != null)
                {
                    _path_find_list.Add(goal_node);
                    goal_node = goal_node.parent;
                }

                // ������ �����.
                _path_find_list.Reverse();
            }

            return true;
        }
        else
        {
            // ��ã�� ����.
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
        // _visitor ������ �ƹ��͵� ���� �ʽ��ϴ�.    
        if (_visitor == null)
            return;

        var open_list_move    = HashSetPool<(int x, int y, int move_cost)>.Acquire();
        var close_list_move   = HashSetPool<(int x, int y, int move_cost)>.Acquire();

        open_list_move.Add((_visitor.Position.x, _visitor.Position.y, 0));
        // Debug.Log($"FloodFill, Start, x:{_position.x}, y:{_position.y}");

        while(open_list_move.Count > 0)
        {
            // movecost�� ���� ���� �������� �����ɴϴ�.            
            var item = (x:0, y:0, move_cost:int.MaxValue);
            
            foreach(var e in open_list_move)
            {
                if (e.move_cost < item.move_cost) 
                    item = e;
            }            

            // ���� ���������� ��ġ������ Visit�� �����մϴ�. 
            var call_visit = (item.x == _visitor.Position.x && item.y == _visitor.Position.y)
                           || Verify_Movecost(_visitor.TerrainMap, _visitor.Visitor, (item.x, item.y), _is_occupancy:true).result;
                                    
            if (call_visit)
                _visitor.Visit(item.x, item.y);

            // open/close list ����
            open_list_move.Remove(item);
            close_list_move.Add((item.x, item.y, 0));
            // Debug.Log($"FloodFill, CloseList Add, x:{item.x}, y:{item.y}");

            // �̵� ���� ���� Ž��. (FloodFill)
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var x = item.x + i;
                    var y = item.y + k;                    

                    // �̹� üũ�Ͽ���.
                    if (close_list_move.Contains((x, y, 0)))
                        continue;

                    // ����, ���� 1ĭ���� �̵�����. (�밢�� �̵� ����)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        continue;

                    // Debug.Log($"FloodFill, x:{x}, y:{y}");

                    // ��� ������ �������� üũ�մϴ�.
                    (var moveable, var move_cost) = Verify_Movecost(_visitor.TerrainMap, _visitor.Visitor, (x, y), _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // �̵� ���� �ʰ�.
                    var total_cost = item.move_cost + move_cost;
                    if (total_cost > _visitor.MoveDistance)
                    {
                        continue;
                    }

                    // open_list �� �߰�.
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
  
        // ZOC�� �������� üũ�մϴ�. (��ǥ������ ������ ����־�� ��.)
        if (_terrain_map.ZOC.IsBlockedZOC(_cell.x, _cell.y, 
            (_is_occupancy) ? 0 : _path_owner.PathZOCFaction))        
            return (false, 0);

        
        // �̵� Cost ���.
        var move_cost = Terrain_Attribute.Calculate_MoveCost(
            _path_owner.PathAttribute, 
            _terrain_map.Attribute.GetAttribute(_cell.x, _cell.y));

        if (move_cost <= 0)
            return (false, 0);

        return (true, move_cost);      
    }


}