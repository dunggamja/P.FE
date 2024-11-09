using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace Battle
{
    public class Sensor_DamageScore : ISensor
    {
        public class ScoreResult
        {
            public enum EnumScoreType
            {
                // Tactical,     // ������ Ÿ�� TODO: ����� ��� �������� ����� �غ���...
                
                DamageRate_Dealt, // ���� �� �ִ� ������ ��
                DamageRate_Taken, // ���� �޴� ������ ��

                HitRate,
                DodgeRate,
                
                MoveCost,     // �̵� �Ÿ�

                MAX
            }

            // �� �׸� ����., �ϵ��ڵ��صΰ�...
            // �ó������� ���� ��� �ʿ��� ��쿡�� ��� ������ �� ����غ��� . 
            static Dictionary<EnumScoreType, float> s_score_multiplier = new ()
            {            
                { EnumScoreType.DamageRate_Dealt, 5f   },
                { EnumScoreType.DamageRate_Taken, 0.5f },
                { EnumScoreType.HitRate,          0.2f },
                { EnumScoreType.DodgeRate,        0.2f },
                { EnumScoreType.MoveCost,         0.1f }

            };

            public  Int64          TargetID { get; private set; } = 0;
            public  Int64          WeaponID { get; private set; } = 0;
            public  (int x, int y) Position { get; private set; } = (0, 0);

            private float[]        score = new float[(int)EnumScoreType.MAX] ;

            public ScoreResult Setup(Int64 _entity_id, Int64 _weapon_id)
            {
                TargetID = _entity_id;
                WeaponID = _weapon_id;
                return this;
            }

            public ScoreResult Reset()
            {
                TargetID = 0;
                WeaponID = 0;
                Array.Clear(score, 0, score.Length);

                return this;
            }

            public void SetScore(EnumScoreType _score_type, float _score_value)
            {
                var index = (int)_score_type;
                if (index < 0 || score.Length <= index)
                    return;
                
                // �� ���ھ�� 0.0 ~ 1.0 ������ ��ȿ.
                score[index] = Mathf.Clamp01(_score_value);                
            }

            public void SetAttackPosition(int _x, int _y)
            {
                Position = (_x, _y);
            }


            // public float GetScore(EnumScoreType _score_type)
            // {
            //     var index = (int)_score_type;
            //     if (index < 0 || score.Length <= index)
            //         return 0f;
            //     return score[index];
            // }

            public float Calculate_Total_Score()
            {
                var result = 0f;

                for (int i = 0; i < score.Length; ++i)
                {
                    if (s_score_multiplier.TryGetValue((EnumScoreType)i, out var multiplier))
                    {
                        result += score[i] * multiplier;
                    }
                }

                return result;
            }
        }

        public ScoreResult BestScore { get; private set; } = new();


        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null || owner_entity.StatusManager == null)
                return;

            // �������� ���� / ������ ID
            var weapon            = owner_entity.StatusManager.Weapon;
            var equiped_weapon_id = weapon.ItemID;

            // ������� ���� ���� �� 1���� �����غ���...
            BestScore.Reset();


            // ���� ���� ������ �׽�Ʈ ������ �����ϴ�.
            foreach(var e in owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon))
            {
                // �׽�Ʈ ������ �����ϱ� ���� �������� ���⸦ �ٲ��ݴϴ�.
                weapon.Equip(e.ID);

                // �ִ� �̵� �Ÿ�.
                var move_distance = owner_entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);

                // ���� �����Ÿ�. (�ּ�/�ִ�)
                var range_min = owner_entity.StatusManager.GetBuffedWeaponStatus(weapon, EnumWeaponStatus.Range_Min);
                var range_max = owner_entity.StatusManager.GetBuffedWeaponStatus(weapon, EnumWeaponStatus.Range);

                // Target Collect
                var list_target_id = CollectTarget(
                    TerrainMapManager.Instance.TerrainMap,
                    owner_entity,
                    owner_entity.Cell.x,
                    owner_entity.Cell.y,
                    move_distance,
                    range_min,
                    range_max);


                foreach((var target_id, var attack_position_x, var attack_position_y) in list_target_id)
                {
                    var target_entity = EntityManager.Instance.GetEntity(target_id);
                    if (target_entity == null)
                        continue;          


                    // TODO: ��/�Ʊ� üũ�� ���� �Լ��� ���� �����Ѵ�.// ȥ������ �����̻� ������ �ְ�... NPC�ε� �Ʊ��� ���� �� �ְ�...
                    if (target_entity.GetFaction() == owner_entity.GetFaction())
                        continue;

                    // ���� ���.
                    var current_score = Score_Calculate(owner_entity, target_entity);

                    // ��ġ ����.
                    current_score.SetAttackPosition(attack_position_x, attack_position_y);

                    // ��� �� ����.
                    if (BestScore.Calculate_Total_Score() <= current_score.Calculate_Total_Score())
                    {
                        BestScore = current_score;
                    }
                }
            }

            // ���� ���� ����.
            weapon.Equip(equiped_weapon_id);
        }


        static ScoreResult Score_Calculate(Entity _owner, Entity _target)
        {
            if (_owner == null || _target == null)
                return new ScoreResult();


            // ���� ��� ���ھ�
            var current_score = new ScoreResult().Setup(_target.ID, _owner.StatusManager.Weapon.ItemID);

            // �׽�Ʈ ������ ������ ����� �غ���.                   
            CombatSystemManager.Instance.Setup(CombatParam_Plan.Cache.Set(_owner, _target));


            // ������/Ÿ���� ���� ������.
            var damage_dealt_count = 0;
            var damage_taken_count = 0;                    
            var damage_dealt       = 0;
            var damage_taken       = 0;
            var hit_rate           = 0f;
            var dodge_rate         = 0f;                    
            

            // 
            while (!CombatSystemManager.Instance.IsFinished)
            {
                CombatSystemManager.Instance.Update();

                var system_turn   = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn)   as CombatSystem_Turn;
                var system_damage = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Damage) as CombatSystem_Damage;

                // ���� ���.
                var turn_side   = system_turn.CombatTurn;
                var turn_damage = system_damage.Result_Damage;
                var turn_hit    = system_damage.Result_HitRate;

                switch (turn_side)
                {
                    case CombatSystem_Turn.EnumCombatTurn.Attacker: 
                    {
                        damage_dealt += turn_damage; 
                        hit_rate     += turn_hit;

                        ++damage_dealt_count;

                    }
                    break;
                    case CombatSystem_Turn.EnumCombatTurn.Defender: 
                    {
                        damage_taken += turn_damage; 
                        dodge_rate   += 1f - turn_hit;

                        ++damage_taken_count;
                    }
                    break;
                }
            }

            // ������/Ÿ�� HP
            var owner_hp  = Math.Max(1, _owner.StatusManager.Status.GetPoint(EnumUnitPoint.HP));
            var target_hp = Math.Max(1, _target.StatusManager.Status.GetPoint(EnumUnitPoint.HP));

            
            // ������ ���� ����. 
            current_score.SetScore(ScoreResult.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / target_hp);
            current_score.SetScore(ScoreResult.EnumScoreType.DamageRate_Taken, (float)damage_taken / owner_hp);

            // �̵��Ÿ� ���� ����.
            var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            var distance_max     = _owner.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);
            current_score.SetScore(ScoreResult.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // ���� / ȸ�� ���� ����.
            current_score.SetScore(ScoreResult.EnumScoreType.HitRate,   hit_rate   / Math.Max(1, damage_dealt_count));
            current_score.SetScore(ScoreResult.EnumScoreType.DodgeRate, dodge_rate / Math.Max(1, damage_taken_count));

            return current_score;
        }


        static List<(Int64 target_id, int attack_position_x, int atatck_position_y)> 
            CollectTarget(
            TerrainMap _terrain_map, 
            IPathOwner _path_owner, 
            int        _x, 
            int        _y, 
            int        _move_distance, 
            int        _weapon_range_min, 
            int        _weapon_range_max)
        {

            if (_terrain_map == null || _path_owner == null)
                return new();

            var list_attack       = new List<(Int64 target_id, int attack_position_x, int atatck_position_y)>();
            var close_list_attack = new List<(int x, int y)>(10);


            PathAlgorithm.FloodFill(_terrain_map, _path_owner, (_x, _y), _move_distance,
            ((int x, int y) _cell) =>
            {
                                
                // ���� ��Ÿ� ���� �ȿ� ���� Ÿ�ٵ� �ݷ���
                for(int i = -_weapon_range_max; i <= _weapon_range_max; ++i)
                {
                    for(int k = -_weapon_range_max; k <= _weapon_range_max; ++k)
                    {
                        var x = _cell.x + i;
                        var y = _cell.y + k;

                        // ���� ��Ÿ� üũ
                        var distance = PathAlgorithm.Distance(_cell.x, _cell.y, x, y);
                        if (distance < _weapon_range_min || _weapon_range_max < distance)
                        {
                            continue;
                        }

                        // TODO: �Ȱ��� ��ġ�� �Ź� Collecting �� ������ ������ ���� ���.
                        if (close_list_attack.Contains((x, y)))
                        {
                            continue;
                        }

                        // �˻� ��Ͽ� �߰�.
                        close_list_attack.Add((x, y));
                        
                        // Ÿ�� �߰�. (��� id, ���� ��ġ x, ���� ��ġ y)
                        var entity_id = _terrain_map.BlockManager.FindEntityID(x, y);
                        if (entity_id > 0)
                        {
                            list_attack.Add((entity_id, _cell.x, _cell.y));
                        }
                    }
                }
            });

            return list_attack;
        }

    }    
}