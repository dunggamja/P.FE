using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Battle;
using UnityEditor.Build.Utilities;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public static  partial class PathAlgorithm
{
   public static List<Int64> CollectTarget(TerrainMap _terrain_map, int _path_owner_attribute, int _x, int _y, int _move_distance, int _weapon_range_min, int _weapon_range_max)
   {
        var list_target = new List<Int64>();

        if (_terrain_map != null)
        {
            
            (int x, int y, int cost) FindMinimumCostItem(List<(int x, int y, int cost)> _list)
            {
                var result = (is_set:false, item:(x:0, y:0, cost:0));

                foreach((var x, var y, var cost) in _list)
                {
                    if (!result.is_set || cost < result.item.cost)
                    {
                        result.is_set = true;
                        result.item   = (x, y, cost);
                    }
                }

                return result.item;
            } 

            var open_list_move    = new List<(int x, int y, int move_cost)>(10);
            var close_list_move   = new List<(int x, int y)>(10);
            var close_list_weapon = new List<(int x, int y)>(10);
            

            // ���� ����.
            open_list_move.Add((_x, _y, 0));

            while(open_list_move.Count > 0)
            {
                // cost�� ���� ���� �������� �����ɴϴ�.
                var item = FindMinimumCostItem(open_list_move);

                // open/close list ����
                open_list_move.Remove(item);
                close_list_move.Add((item.x, item.y));


                // ���� ��Ÿ� ���� �ȿ� ���� Ÿ�ٵ� �ݷ���
                for(int i = -_weapon_range_max; i <= _weapon_range_max; ++i)
                {
                    for(int k = -_weapon_range_max; k <= _weapon_range_max; ++k)
                    {
                        var y = item.y + i;
                        var x = item.x + k;

                        // ���� ��Ÿ� üũ
                        var distance = PathAlgorithm.Distance(item.x, item.y, x, y);
                        if (distance < _weapon_range_min || _weapon_range_max < distance)
                        {
                            continue;
                        }

                        // �̹� �˻��� ��ġ.
                        if (close_list_weapon.Contains((x, y)))
                        {
                            continue;
                        }

                        // �˻� ��Ͽ� �߰�.
                        close_list_weapon.Add((x, y));

                        // Ÿ�� ��Ͽ� �߰�.
                        var entity_id = _terrain_map.BlockManager.FindEntity(x, y);
                        if (entity_id > 0)
                        {
                            list_target.Add(entity_id);
                        }
                    }
                }



                // �̵� ���� ���� Ž��. (FloodFill)
                for(int i = -1; i <= 1; ++i)
                {
                    for(int k = -1; k <= 1; ++k)
                    {
                        var y = item.y + i;
                        var x = item.x + k;

                        // ����, ���� 1ĭ���� �̵�����.
                        if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        {
                            continue;
                        }

                        // �밢�� ������ ���⼱ �ʿ䰡 ����.
                        // var is_diagonal = (i != 0) && (k != 0);
                        // if (is_diagonal)
                        // {
                        //     continue;
                        // }

                        
                        // �̵� �Ұ��� ������ �Ÿ���.
                        var move_cost  = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));
                        if (move_cost <= 0)
                        {
                            continue;
                        }

                        // �̵� ���� �ʰ�.
                        var total_cost = item.cost + move_cost;
                        if (total_cost > _move_distance)
                        {
                            continue;
                        }

                        // �̹� üũ�Ͽ���.
                        if (close_list_move.Contains((x, y)))
                        {
                            continue;
                        }

                        // open_list �� �߰�.
                        open_list_move.Add((x, y, total_cost));
                    }
                }
            }
        }

        // TODO: �ּ� ���� ���� üũ�ؼ� �Ұ����� ������ ��ġ

        return list_target;

   }



}