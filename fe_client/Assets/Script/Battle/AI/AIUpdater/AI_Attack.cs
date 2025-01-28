using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    // ���� Ÿ���� ã�ƺ��ô�.
    public class AI_Attack : IAIUpdater
    {
        public class ScoreResult : IPoolObject
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

            private float[]        m_score = new float[(int)EnumScoreType.MAX] ;

            public ScoreResult Setup(Int64 _entity_id, Int64 _weapon_id)
            {
                TargetID = _entity_id;
                WeaponID = _weapon_id;
                return this;
            }

            public void Reset()
            {
                TargetID = 0;
                WeaponID = 0;
                Position = (0, 0);
                Array.Clear(m_score, 0, m_score.Length);
            }

            public void SetScore(EnumScoreType _score_type, float _score_value)
            {
                var index = (int)_score_type;
                if (index < 0 || m_score.Length <= index)
                    return;
                
                // �� ���ھ�� 0.0 ~ 1.0 ������ ��ȿ.
                m_score[index] = Mathf.Clamp01(_score_value);                
            }

            public void SetAttackPosition(int _x, int _y)
            {
                Position = (_x, _y);
            }

            public void SetData(ScoreResult _o)
            {
                TargetID = _o.TargetID;
                WeaponID = _o.WeaponID;
                Position = _o.Position;
                Array.Copy(_o.m_score, m_score, m_score.Length);
            }


            // public float GetScore(EnumScoreType _score_type)
            // {
            //     var index = (int)_score_type;
            //     if (index < 0 || score.Length <= index)
            //         return 0f;
            //     return score[index];
            // }

            public float CalculateScore()
            {
                var result    = 0f;
                var max_score = 0f;

                // �ְ� ����.
                foreach(var e in s_score_multiplier.Values)
                {
                    max_score += e;
                }

                
                if (max_score <= float.Epsilon)
                    return 0f;

                // ���� �ջ�.
                for (int i = 0; i < m_score.Length; ++i)
                {
                    if (s_score_multiplier.TryGetValue((EnumScoreType)i, out var multiplier))
                    {
                        result += m_score[i] * multiplier;
                    }
                }

                // ������ 0.0 ~ 1.0���� ����.
                return Mathf.Clamp01(result / max_score);
            }
        }

        // public ScoreResult BestScore { get; private set; } = new();


        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            // �ڵ尡 �ʹ� ������� ������ ĳ��.
            var owner_status     = owner_entity.StatusManager;
            var owner_blackboard = owner_entity.BlackBoard;
            var owner_inventory  = owner_entity.Inventory;

            // �������� ���� / ������ ID
            var owner_weapon      = owner_status.Weapon;
            var equiped_weapon_id = owner_weapon.ItemID;


            // ������� ���� ���� �� 1���� �����غ���...
            owner_blackboard.Score_Attack.Reset();
            owner_blackboard.SetBPValue(EnumEntityBlackBoard.AIScore_Attack, 0f);            


            // �ൿ���� ������ �������� üũ.
            var is_moveable   = owner_entity.HasCommandEnable(EnumCommandFlag.Move);
            var is_attackable = owner_entity.HasCommandEnable(EnumCommandFlag.Action);

            // ������ �������� üũ.
            if (is_attackable)
            {
                // ���� ���� ������ �׽�Ʈ ������ �����ϴ�.
                foreach(var e in owner_inventory.CollectItemByType(EnumItemType.Weapon))
                {
                    // �׽�Ʈ ������ �����ϱ� ���� �������� ���⸦ �ٲ��ݴϴ�.
                    owner_weapon.Equip(e.ID);
                    
                    // �ִ� �̵� �Ÿ�. 
                    var move_distance = (is_moveable) ? owner_status.GetBuffedUnitStatus(EnumUnitStatus.Movement) : 0;

                    // ���� �����Ÿ�. (�ּ�/�ִ�)
                    var range_min = owner_status.GetBuffedWeaponStatus(owner_weapon, EnumWeaponStatus.Range_Min);
                    var range_max = owner_status.GetBuffedWeaponStatus(owner_weapon, EnumWeaponStatus.Range);

                    var list_collect_target = ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.Acquire();
                    
                    // Target Collect
                    CollectTarget(
                        ref list_collect_target,
                        TerrainMapManager.Instance.TerrainMap,
                        owner_entity,
                        owner_entity.Cell.x,
                        owner_entity.Cell.y,
                        move_distance,
                        range_min,
                        range_max);


                    foreach((var target_id, var attack_pos_x, var attack_pos_y) in list_collect_target)
                    {
                        var target_entity = EntityManager.Instance.GetEntity(target_id);
                        if (target_entity == null)
                            continue; 

                        // �׾����� Ÿ���ÿ��� ����
                        if (target_entity.IsDead)
                            continue;     

                        // TODO: ��/�Ʊ� üũ�� ���� �Լ��� ���� �����Ѵ�.// ȥ������ �����̻� ������ �ְ�... NPC�ε� �Ʊ��� ���� �� �ְ�...
                        if (target_entity.GetFaction() == owner_entity.GetFaction())
                            continue;

                        try
                        {
                            // ���� ���.
                            var current_score = ObjectPool<ScoreResult>.Acquire();
                            if (current_score == null)
                                continue;

                            Score_Calculate(ref current_score, owner_entity, target_entity);

                            // ��ġ ����.
                            current_score.SetAttackPosition(attack_pos_x, attack_pos_y);

                            // ���� ���.
                            var calculate_score = current_score.CalculateScore();

                            // ���� ��.
                            if (owner_blackboard.GetBPValueAsFloat(EnumEntityBlackBoard.AIScore_Attack) <= calculate_score)
                            {
                                // ���� ���� ����.
                                owner_blackboard.Score_Attack.SetData(current_score);                            
                                owner_blackboard.SetBPValue(EnumEntityBlackBoard.AIScore_Attack, calculate_score); 
                            }
                        }
                        finally
                        {

                        }
                    }

                    ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.Release(list_collect_target);
                }
            }

            // ���� ���� ����.
            owner_weapon.Equip(equiped_weapon_id);                       
        }


        static void Score_Calculate(ref ScoreResult _score, Entity _owner, Entity _target)
        {
            if (_owner == null || _target == null)
                return;


            // ���� ��� ���ھ�
            _score.Reset();
            _score.Setup(_target.ID, _owner.StatusManager.Weapon.ItemID);

            // ������/Ÿ���� ���� ������.
            var damage_dealt_count = 0;
            var damage_taken_count = 0;                    
            var damage_dealt       = 0;
            var damage_taken       = 0;
            var hit_rate           = 0f;
            var dodge_rate         = 0f;                    
            
            // �׽�Ʈ ������ ������ ����� �غ���.                   
            CombatSystemManager.Instance.Setup(CombatParam_Plan.Cached.Set(_owner, _target));

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
            _score.SetScore(ScoreResult.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / target_hp);
            _score.SetScore(ScoreResult.EnumScoreType.DamageRate_Taken, (float)damage_taken / owner_hp);

            // �̵��Ÿ� ���� ����.
            var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            var distance_max     = _owner.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);
            _score.SetScore(ScoreResult.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // ���� / ȸ�� ���� ����.
            _score.SetScore(ScoreResult.EnumScoreType.HitRate,   hit_rate   / Math.Max(1, damage_dealt_count));
            _score.SetScore(ScoreResult.EnumScoreType.DodgeRate, dodge_rate / Math.Max(1, damage_taken_count));

        }


        static void
            CollectTarget(
            ref List<(Int64 target_id, int attack_pos_x, int atatck_pos_y)> _collect_targets,
            TerrainMap _terrain_map, 
            IPathOwner _path_owner, 
            int        _x, 
            int        _y, 
            int        _move_distance, 
            int        _weapon_range_min, 
            int        _weapon_range_max)
        {

            if (_terrain_map == null || _path_owner == null)
                return;


            var list_attack       = ListPool<(Int64 target_id, int attack_pos_x, int atatck_pos_y)>.Acquire();
            var close_list_attack = HashSetPool<(int x, int y)>.Acquire();


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

            
            _collect_targets.AddRange(list_attack);

            ListPool<(Int64, int, int)>.Release(list_attack);
            HashSetPool<(int x, int y)>.Release(close_list_attack);
        }

    }    
}