using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static partial class PathAlgorithm
{
    
    public static List<PathNode> PathFind(TerrainMap _terrain_map, int _path_owner_attribute, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        var result     = new List<PathNode>(20);

        // A* 
        var    node  = AStar(_terrain_map, _path_owner_attribute, _from_x, _from_y, _to_x, _to_y);
        while (node != null)
        {
            result.Add(new PathNode(node.x, node.y));
            node = node.parent;
        }

        // 
        result.Reverse();

        return result;
    }


    static Node AStar(TerrainMap _terrain_map, int _path_owner_attribute, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        if (_terrain_map == null)
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
                return item;

            // �ֺ� ��� Ž��.
            for(int i = -1; i <= 1; ++i)
            {
                for(int k = -1; k <= 1; ++k)
                {
                    var y         = item.y + i;
                    var x         = item.x + k;
                    var new_item  = new Node(x, y, _to_x, _to_y, item);

                    // var is_diagonal = (i != 0) && (k != 0);
                    // if (is_diagonal)
                    // {
                    //     // �밢�� �̵��� ����.
                    //     continue;

                    //     // �밢�� ������ ��� ���� ���������� ��� �����ִ��� üũ�ؾ� ��.?
                    //     // var move_cost_diagonal_1 = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, item.y));
                    //     // var move_cost_diagonal_2 = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(item.x, y));
                    //     // if (_terrain_map.IsCollision(item.x + k, item.y) || _terrain_map.IsCollision(item.x, item.y + i))
                    //     //     continue;
                    // }

                    // ����, ���� 1ĭ���� �̵�����.
                    if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                    {
                        continue;
                    }

                    // �̹� �˻��� ������ �Ÿ���.
                    if (close_list.Contains(new_item))
                        continue;

                    // �̵� Cost ���.
                    var move_cost = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));

                    // �浹 ������ �Ÿ���.
                    if (move_cost <= 0)
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



}