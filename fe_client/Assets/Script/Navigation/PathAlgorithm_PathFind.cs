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
        if (!Verify_Moveable(_terrain_map, _path_owner, _to_x, _to_y, _is_goal:true))
            return null;                    

        var open_list  = new List<Node>(20);
        var close_list = new List<Node>(20);

        // ���� ������ �ֽ��ϴ�.
        open_list.Add(new Node(_from_x, _from_y, _to_x, _to_y, null));

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
                    var y         = item.y + i;
                    var x         = item.x + k;
                    var new_item  = new Node(x, y, _to_x, _to_y, item);
                    

                    // ����, ���� 1ĭ���� �̵�����. (�밢�� �̵� ����)
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                    {
                        continue;
                    }

                    // �̹� �˻��� ������ �Ÿ���.
                    if (close_list.Contains(new_item))
                        continue;

                    // �̵� ������ �� üũ.
                    if (!Verify_Moveable(_terrain_map, _path_owner, x, y, _is_goal:false))
                        continue;
                    

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


    static bool Verify_Moveable(TerrainMap _terrain_map, IPathOwner _path_owner, int _x, int _y, bool _is_goal)
    {
        if (_terrain_map == null || _path_owner == null)
            return false;
  
        // ZOC�� �������� üũ�մϴ�.
        if (_terrain_map.ZOC.IsBlockedZOC(_x, _y, (_is_goal) ? 0 : _path_owner.PathZOCFaction))
            return false;

        // �̵� Cost ���.
        var move_cost = Terrain_Attribute.Calculate_MoveCost(_path_owner.PathAttribute, _terrain_map.Attribute.GetAttribute(_x, _y));
        if (move_cost <= 0)
            return false;

        return true;      
    }


}