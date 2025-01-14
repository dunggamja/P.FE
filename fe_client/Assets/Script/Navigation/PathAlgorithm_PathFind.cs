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

         // ��ǥ ������ �̵� ������ �� üũ.
        if (!Verify_Movecost(_terrain_map, _path_owner, _to_x, _to_y, _is_occupancy:true).result)
            return null;                    

        var open_list  = new List<Node>(20);
        var close_list = new List<Node>(20);

        // ���� ������ �ֽ��ϴ�.
        open_list.Add(new Node(_from_x, _from_y, _to_x, _to_y, 0, null));

        // ���� �Ÿ��� ���� ���� ��带 ã�ƺ��ô�.
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
            // ���� �Ÿ��� ���� ����� ��带 �����´�.
            var item = func_find_minimum_heuristic(open_list);

            // ����� ���� open_list���� ���� �� close_list�� �߰�.
            open_list.Remove(item);
            close_list.Add(item);

            // ��ǥ ������ ����.
            if (item.x == _to_x && item.y == _to_y)
            {
                return item;
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

                    // �̵� ������ �� üũ.
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, x, y, _is_occupancy:false);
                    if (!moveable)
                        continue;

                    var new_item  = new Node(x, y, _to_x, _to_y, move_cost, item);
                    


                    // ���� ��ǥ�� ���ϴ� ��尡 ���� ��� �� �� ����.
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



    class floodfill_node : IPoolObject
    {        
        List<(int x, int y, int move_cost)> Nodes = new(10);

        public int Count => Nodes.Count;


        public void Reset()
        {
            Nodes.Clear();
        }

        public void Add((int x, int y, int move_cost) _item)
        {
            Nodes.Add(_item);
        }

        public void Remove((int x, int y, int move_cost) _item)
        {
            Nodes.Remove(_item);
        }

        public bool Contains((int x, int y, int move_cost) _item)
        {
            return Nodes.Contains(_item);
        }

        public (int x, int y, int move_cost) FindMinimumCostNode()
        {
            if (Nodes.Count == 0)
                return (0, 0, int.MaxValue);

            var min_cost_node = Nodes[0];
            foreach (var node in Nodes)
            {
                if (node.move_cost < min_cost_node.move_cost)
                {
                    min_cost_node = node;
                }
            }

            return min_cost_node;
        }
    }


    public static void FloodFill(
        TerrainMap              _terrain_map,
        IPathOwner              _path_owner,
        (int x, int y)          _position,
        int                     _move_distance,        
        Action<(int x, int y)>  _func_on_cell = null,
        bool                    _is_call_func_any_cell = false)
    {
        // callback�� ������ �ƹ��͵� ���� �ʽ��ϴ�.    
        if (_func_on_cell == null)
            return;


        var open_list_move    = ObjectPool<floodfill_node>.Acquire();
        var close_list_move   = ObjectPool<floodfill_node>.Acquire();

        open_list_move.Add((_position.x, _position.y, 0));
        // Debug.Log($"FloodFill, Start, x:{_position.x}, y:{_position.y}");

        while(open_list_move.Count > 0)
        {
            // movecost�� ���� ���� �������� �����ɴϴ�.            
            var item = open_list_move.FindMinimumCostNode();

            // callback ȣ�� ���� üũ. 
            var call_func_on_cell = _is_call_func_any_cell ||
                                    (item.x == _position.x && item.y == _position.y) ||
                                    Verify_Movecost(_terrain_map, _path_owner, item.x, item.y, _is_occupancy:true).result;
                                    
            if (call_func_on_cell)
                _func_on_cell((item.x, item.y));

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
                    (var moveable, var move_cost) = Verify_Movecost(_terrain_map, _path_owner, x, y, _is_occupancy:false);
                    if (!moveable)
                        continue;

                    // �̵� ���� �ʰ�.
                    var total_cost = item.move_cost + move_cost;
                    if (total_cost > _move_distance)
                    {
                        continue;
                    }

                    // open_list �� �߰�.
                    open_list_move.Add((x, y, total_cost));
                }
            }   
        }

        ObjectPool<floodfill_node>.Release(open_list_move);
        ObjectPool<floodfill_node>.Release(close_list_move);
        // Debug.Log($"FloodFill, Complete, x:{_position.x}, y:{_position.y}");            
    }

    static (bool result, int move_cost) Verify_Movecost(TerrainMap _terrain_map, IPathOwner _path_owner, int _x, int _y, bool _is_occupancy)
    {
        if (_terrain_map == null || _path_owner == null)
            return (false, 0);
        
        if (_x < 0 || _y < 0 || _terrain_map.Width <= _x || _terrain_map.Height <= _y)
            return (false, 0);  
  
        // ZOC�� �������� üũ�մϴ�. (��ǥ������ ������ ����־�� ��.)
        if (_terrain_map.ZOC.IsBlockedZOC(_x, _y, (_is_occupancy) ? 0 : _path_owner.PathZOCFaction))        
            return (false, 0);

        
        // �̵� Cost ���.
        var move_cost = Terrain_Attribute.Calculate_MoveCost(_path_owner.PathAttribute, _terrain_map.Attribute.GetAttribute(_x, _y));
        if (move_cost <= 0)
            return (false, 0);

        return (true, move_cost);      
    }


}