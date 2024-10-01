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

            // �������� ���� ������ ID
            var weapon            = owner_entity.StatusManager.Weapon;
            var equiped_weapon_id = weapon.ItemID;

            // ���� ������ ������ Ÿ���� ������ ���� Ž��.
            foreach(var e in owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon))
            {
                // Ÿ�� ����� ���ؼ� ���⸦ �ٲ��ش�.
                weapon.Equip(e.ID);

                // �̵� �Ÿ�.
                var move_distance = owner_entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);

                // ���� �����Ÿ�. (�ּ�/�ִ�)
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
                    // Test Combat �� ������ ������ ����� �غ��ô�.                   
                    CombatSystemManager.Instance.Setup(TestCombat_Param.Cache.Set(owner_entity, EntityManager.Instance.GetEntity(target_id)));

                    // 
                    while (!CombatSystemManager.Instance.IsFinished)
                    {
                        CombatSystemManager.Instance.Update();

                        var sytem_turn   = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
                        var sytem_damage = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Damage;

                        // ���� ���.
                        var turn_side = sytem_turn.CombatTurn;
                        var damage    = sytem_damage.Result_Damage;

                    }

                }

            }

            // ���� ���� ����.
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
                

                // ���� ����.
                open_list_move.Add((_x, _y, 0));

                while(open_list_move.Count > 0)
                {
                    // cost�� ���� ���� �������� �����ɴϴ�.            
                    var item = open_list_move.Aggregate(open_list_move.First(), (a, b) => a.move_cost < b.move_cost ? a : b);

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

                            // ����, ���� 1ĭ���� �̵�����. (�밢�� �̵� ����)
                            if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                            {
                                continue;
                            }
                            
                            // �̵� �Ұ��� ������ �Ÿ���.
                            var move_cost  = TerrainAttribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));
                            if (move_cost <= 0)
                            {
                                continue;
                            }

                            // �̵� ���� �ʰ�.
                            var total_cost = item.move_cost + move_cost;
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
                
                // TODO: �ּ� ���� ���� üũ�ؼ� �Ұ����� ������ ��ġ

                return list_target;
        }

    }    
}