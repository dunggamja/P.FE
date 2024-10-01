using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class Sensor_Target_Score : ISensor
    {
        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null || owner_entity.StatusManager == null)
                return;

            // 착용중인 무기 아이템 ID
            var weapon            = owner_entity.StatusManager.Weapon;
            var equiped_weapon_id = weapon.ItemID;

            // 착용 가능한 무기들로 타겟팅 가능한 적들 탐색.
            foreach(var e in owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon))
            {
                // 타겟 계산을 위해서 무기를 바꿔준다.
                weapon.Equip(e.ID);

                // 이동 거리.
                var move_distance = owner_entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);

                // 무기 사정거리. (최소/최대)
                var range_min = owner_entity.StatusManager.GetBuffedWeaponStatus(weapon, EnumWeaponStatus.Range_Min);
                var range_max = owner_entity.StatusManager.GetBuffedWeaponStatus(weapon, EnumWeaponStatus.Range);

                // Target Collect
                var list_target_id = CollectTarget(
                    TerrainMapManager.Instance.TerrainMap,
                    owner_entity.PathAttribute,
                    owner_entity.Cell.x,
                    owner_entity.Cell.y,
                    move_distance,
                    range_max,
                    range_min);

                foreach(var target_id in list_target_id)
                {
                    // Test Combat 을 돌려서 데미지 계산을 해봅시다.                   
                    CombatSystemManager.Instance.Setup(TestCombat_Param.Cache.Set(owner_entity, EntityManager.Instance.GetEntity(target_id)));

                    // 
                    while (!CombatSystemManager.Instance.IsFinished)
                    {
                        CombatSystemManager.Instance.Update();

                        var sytem_turn   = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
                        var sytem_damage = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Damage;

                        // 연산 결과.
                        var turn_side = sytem_turn.CombatTurn;
                        var damage    = sytem_damage.Result_Damage;

                    }

                }

            }

            // 무기 원상 복구.
            weapon.Equip(equiped_weapon_id);

        }


        static List<Int64> CollectTarget(
            TerrainMap _terrain_map, 
            int        _path_owner_attribute, 
            int        _x, 
            int        _y, 
            int        _move_distance, 
            int        _weapon_range_min, 
            int        _weapon_range_max)
        {

                if (_terrain_map == null)
                    return new();

                var list_target       = new List<Int64>();

                var open_list_move    = new List<(int x, int y, int move_cost)>(10);
                var close_list_move   = new List<(int x, int y)>(10);
                var close_list_weapon = new List<(int x, int y)>(10);
                

                // 시작 지점.
                open_list_move.Add((_x, _y, 0));

                while(open_list_move.Count > 0)
                {
                    // cost가 가장 적은 아이템을 가져옵니다.            
                    var item = open_list_move.Aggregate(open_list_move.First(), (a, b) => a.move_cost < b.move_cost ? a : b);

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

                            // 가로, 세로 1칸씩만 이동가능. (대각선 이동 없음)
                            if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                            {
                                continue;
                            }
                            
                            // 이동 불가능 지역은 거른다.
                            var move_cost  = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));
                            if (move_cost <= 0)
                            {
                                continue;
                            }

                            // 이동 범위 초과.
                            var total_cost = item.move_cost + move_cost;
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
                
                // TODO: 최소 범위 따로 체크해서 불가능한 지역에 위치

                return list_target;
        }

    }    
}