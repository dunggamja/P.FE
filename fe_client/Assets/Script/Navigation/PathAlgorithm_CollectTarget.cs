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
            

            // 시작 지점.
            open_list_move.Add((_x, _y, 0));

            while(open_list_move.Count > 0)
            {
                // cost가 가장 적은 아이템을 가져옵니다.
                var item = FindMinimumCostItem(open_list_move);

                // open/close list 셋팅
                open_list_move.Remove(item);
                close_list_move.Add((item.x, item.y));


                // 무기 사거리 범위 안에 들어온 타겟들 콜렉팅
                for(int i = -_weapon_range_max; i <= _weapon_range_max; ++i)
                {
                    for(int k = -_weapon_range_max; k <= _weapon_range_max; ++k)
                    {
                        var y = item.y + i;
                        var x = item.x + k;

                        // 무기 사거리 체크
                        var distance = PathAlgorithm.Distance(item.x, item.y, x, y);
                        if (distance < _weapon_range_min || _weapon_range_max < distance)
                        {
                            continue;
                        }

                        // 이미 검사한 위치.
                        if (close_list_weapon.Contains((x, y)))
                        {
                            continue;
                        }

                        // 검사 기록에 추가.
                        close_list_weapon.Add((x, y));

                        // 타겟 목록에 추가.
                        var entity_id = _terrain_map.BlockManager.FindEntity(x, y);
                        if (entity_id > 0)
                        {
                            list_target.Add(entity_id);
                        }
                    }
                }



                // 이동 가능 지역 탐색. (FloodFill)
                for(int i = -1; i <= 1; ++i)
                {
                    for(int k = -1; k <= 1; ++k)
                    {
                        var y = item.y + i;
                        var x = item.x + k;

                        // 가로, 세로 1칸씩만 이동가능.
                        if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        {
                            continue;
                        }

                        // 대각선 로직은 여기선 필요가 없넹.
                        // var is_diagonal = (i != 0) && (k != 0);
                        // if (is_diagonal)
                        // {
                        //     continue;
                        // }

                        
                        // 이동 불가능 지역은 거른다.
                        var move_cost  = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));
                        if (move_cost <= 0)
                        {
                            continue;
                        }

                        // 이동 범위 초과.
                        var total_cost = item.cost + move_cost;
                        if (total_cost > _move_distance)
                        {
                            continue;
                        }

                        // 이미 체크하였음.
                        if (close_list_move.Contains((x, y)))
                        {
                            continue;
                        }

                        // open_list 에 추가.
                        open_list_move.Add((x, y, total_cost));
                    }
                }
            }
        }

        // TODO: 최소 범위 따로 체크해서 불가능한 지역에 위치

        return list_target;

   }



}