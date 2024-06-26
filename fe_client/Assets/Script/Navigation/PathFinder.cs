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



    public static List<PathNode> Find(TerrainCollision _collision, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        

        var result     = new List<PathNode>(20);
        

        // A* 길찾�?
        var    node  = AStar(_collision, _from_x, _from_y, _to_x, _to_y);
        while (node != null)
        {
            result.Add(new PathNode(node.x, node.y));
            node = node.parent;
        }

        // ?��?�� �?�?.
        result.Reverse();

        return result;
    }


    static Node AStar(TerrainCollision _collision, int _from_x, int _from_y, int _to_x, int _to_y)
    {
        if (_collision == null)
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
                    var y        = item.y + i;
                    var x        = item.x + k;
                    var new_item = new Node(x, y, _to_x, _to_y, item);

                    // �浹 ������ �Ÿ���.
                    if (_collision.IsCollision(x, y))
                        continue;

                    // �밢�� ������ ��� ���� ���������� �����ִ��� üũ�ؾ� ��.
                    var is_diagonal = (i != 0) && (k != 0);
                    if (is_diagonal)
                    {
                        if (_collision.IsCollision(item.x + k, item.y) || _collision.IsCollision(item.x, item.y + i))
                            continue;
                    }

                    // �̹� �˻��� ������ �Ÿ���.
                    if (close_list.Contains(new_item))
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