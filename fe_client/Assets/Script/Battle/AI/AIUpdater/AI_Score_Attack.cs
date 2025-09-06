using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    // �����Ÿ� ���� ���� Ÿ���� ã�� ����.
    public class AI_Score_Attack : IAIUpdater
    {
        public class Result : IPoolObject
        {
            public enum EnumScoreType
            {                
                DamageRate_Dealt, // ���� �� �ִ� ������ ��
                DamageRate_Taken, // ���� �޴� ������ ��
                HitRate,          // ���߷�
                DodgeRate,        // ȸ����                
                MoveCost,         // �̵� �Ÿ� (�������� ���� ����)
                MAX
            }

            // �� �׸� ����. (���ص� ���� ���� �׳� ���� ��ġ)
            static Dictionary<EnumScoreType, float> s_score_multiplier = new ()
            {            
                { EnumScoreType.DamageRate_Dealt, 1f   },
                { EnumScoreType.DamageRate_Taken, -0.7f }, 
                { EnumScoreType.HitRate,          0.7f },
                { EnumScoreType.DodgeRate,        0.4f },
                { EnumScoreType.MoveCost,         0.1f }

            };

            public  Int64          TargetID { get; private set; } = 0;
            public  Int64          WeaponID { get; private set; } = 0;
            public  (int x, int y) Position { get; private set; } = (0, 0);

            private float[]        m_score = new float[(int)EnumScoreType.MAX] ;

            public Result Setup(Int64 _entity_id, Int64 _weapon_id)
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

            public void CopyFrom(Result _o)
            {
                TargetID = _o.TargetID;
                WeaponID = _o.WeaponID;
                Position = _o.Position;
                Array.Copy(_o.m_score, m_score, m_score.Length);
            }


            public float CalculateScore()
            {
                var result    = 0f;
                var max_score = 0f;

                // �ְ� ����.
                foreach(var e in s_score_multiplier.Values)
                {
                    max_score += Mathf.Max(e, 0f);
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


        public void Update(IAIDataManager _param)
        {
            if (_param == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_param.ID);
            if (owner_entity == null)
                return;

            // �ڵ尡 �ʹ� ������� ������ ĳ��.
            var owner_status     = owner_entity.StatusManager;
            var owner_blackboard = owner_entity.BlackBoard;
            var owner_inventory  = owner_entity.Inventory;

            // �������� ���� / ������ ID
            var owner_weapon      = owner_status.Weapon;
            var equiped_weapon_id = owner_weapon.ItemID;


            // �ൿ���� ������ �������� üũ.
            var is_moveable   = owner_entity.HasCommandEnable(EnumCommandFlag.Move);
            var is_attackable = owner_entity.HasCommandEnable(EnumCommandFlag.Action);


            // ���� �ൿ�� �Ұ����� ���¸� ����
            if (is_attackable == false)
                return;


            // ���� ��� �����.
            var current_score = ObjectPool<Result>.Acquire();

            // ���� ���� ������ �׽�Ʈ ������ �����ϴ�.
            var list_weapon = ListPool<Item>.Acquire();
            owner_inventory.CollectItemByType(ref list_weapon, EnumItemType.Weapon);

            // ���� ������ Ÿ�� ���.
            var list_collect_target = ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.Acquire();


            try
            {                            
                foreach(var e in list_weapon)
                {
                    // �׽�Ʈ ������ �����ϱ� ���� �������� ���⸦ �ٲ��ݴϴ�.
                    owner_weapon.Equip(e.ID);
                    
                    // �ִ� �̵� �Ÿ�. 
                    var move_distance = (is_moveable) ? owner_entity.PathMoveRange : 0;

                    // ���� �����Ÿ�. (�ּ�/�ִ�)
                    var range_min = owner_status.GetBuffedWeaponStatus(owner_weapon, EnumWeaponStatus.Range_Min);
                    var range_max = owner_status.GetBuffedWeaponStatus(owner_weapon, EnumWeaponStatus.Range);


                    // ���� ������ Ÿ�� ��� �ʱ�ȭ.
                    list_collect_target.Clear();
                    
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

                    // Ÿ�� ��ȸ.
                    foreach((var target_id, var attack_pos_x, var attack_pos_y) in list_collect_target)
                    {
                        var target_entity = EntityManager.Instance.GetEntity(target_id);
                        if (target_entity == null)
                            continue; 

                        // �׾����� Ÿ���ÿ��� ����
                        if (target_entity.IsDead)
                            continue;     

                        // ���� �������� üũ.
                        if (CombatHelper.IsAttackable(owner_entity.ID, target_id) == false)
                            continue;

                        // ���� ��� ����� �ʱ�ȭ.
                        current_score.Reset();
            
                        Score_Calculate(ref current_score,
                            owner_entity,
                            target_entity,
                            (attack_pos_x, attack_pos_y));
                        // ���� ���.
                        var calculate_score = current_score.CalculateScore();

                        // ���� ��.
                        if (owner_blackboard.GetBPValueAsFloat(EnumEntityBlackBoard.AIScore_Attack) <= calculate_score)
                        {
                            // ���� ���� ����.
                            owner_blackboard.Score_Attack.CopyFrom(current_score);                            
                            owner_blackboard.SetBPValue(EnumEntityBlackBoard.AIScore_Attack, calculate_score); 
                        }
                    }
                }
            }  
            finally
            {
                // ���� ���� ����.
                owner_weapon.Equip(equiped_weapon_id);     

                // pool ��ȯ.
                ObjectPool<Result>.Return(ref current_score);
                ListPool<Item>.Return(ref list_weapon);
                ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.Return(ref list_collect_target);
            }    
        }


        static void Score_Calculate(ref Result _score, Entity _owner, Entity _target, (int x, int y) _attack_position)
        {
            if (_owner == null || _target == null)
                return;


            // ���� ��� ���ھ�
            _score.Reset();
            _score.Setup(_target.ID, _owner.StatusManager.Weapon.ItemID);
           
            

            var result = CombatHelper.Run_Plan(
                _owner.ID, 
                _target.ID, 
                _owner.StatusManager.Weapon.ItemID,
                _attack_position);

            if (result == null)
                return;

            var damage_dealt_count = result.Actions.Count(e => e.isAttacker);   
            var damage_taken_count = result.Actions.Count(e => !e.isAttacker);   

            var damage_taken       = result.Attacker.HP_Before - result.Attacker.HP_After;
            var damage_dealt       = result.Defender.HP_Before - result.Defender.HP_After;

            var hit_rate           = Mathf.Clamp01(result.Attacker.HitRate / 100f);
            var dodge_rate         = Mathf.Clamp01((100 - result.Defender.HitRate) / 100f);


            // ������/Ÿ�� HP
            var owner_hp  = Math.Max(1, _owner.StatusManager.Status.GetPoint(EnumUnitPoint.HP));
            var target_hp = Math.Max(1, _target.StatusManager.Status.GetPoint(EnumUnitPoint.HP));

            
            // ������ ���� ����. 
            _score.SetScore(Result.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / target_hp);
            _score.SetScore(Result.EnumScoreType.DamageRate_Taken, (float)damage_taken / owner_hp);

            // �̵��Ÿ� ���� ����.
            var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            var distance_max     = Math.Max(1, _owner.PathMoveRange);
            _score.SetScore(Result.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // ���� / ȸ�� ���� ����.
            _score.SetScore(Result.EnumScoreType.HitRate,   hit_rate   / Math.Max(1, damage_dealt_count));
            _score.SetScore(Result.EnumScoreType.DodgeRate, dodge_rate / Math.Max(1, damage_taken_count));

            // ���� ��ġ ����.
            _score.SetAttackPosition(_attack_position.x, _attack_position.y);
        }


        class CollectTargetVisitor : PathAlgorithm.IFloodFillVisitor, IPoolObject
        {
            public TerrainMap     TerrainMap     { get; set; }  
            public IPathOwner     Visitor        { get; set; }  
            public (int x, int y) Position       { get; set; }  
            public int            MoveDistance   { get; set; }   
            public bool           VisitOnlyEmptyCell          => true;
            public bool           StopVisit => false;
            
            public int            WeaponRangeMin { get; set; }
            public int            WeaponRangeMax { get; set; }

            List<(Int64 id, int attack_pos_x, int attack_pos_y)> CollectTargets { get; set;} = new();
            HashSet<(int x, int y)>                              VisitList      { get; set; } = new();

            public List<(Int64 id, int attack_pos_x, int attack_pos_y)> GetCollectTargets()
            {
                return CollectTargets;
            }

            public void Reset()
            {
                TerrainMap     = null;
                Visitor      = null;
                Position       = (0, 0);
                MoveDistance   = 0;
                WeaponRangeMin = 0;
                WeaponRangeMax = 0;
                CollectTargets.Clear();
                VisitList.Clear();
            }

            public bool Visit(int _visit_x, int _visit_y)
            {
                bool result = false;

                for(int i = -WeaponRangeMax; i <= WeaponRangeMax; ++i)
                {
                    for(int k = -WeaponRangeMax; k <= WeaponRangeMax; ++k)
                    {
                        var x = _visit_x + i;
                        var y = _visit_y + k;

                        // ���� ��Ÿ� üũ
                        var distance = PathAlgorithm.Distance(_visit_x, _visit_y, x, y);
                        if (distance < WeaponRangeMin || WeaponRangeMax < distance)
                        {
                            continue;
                        }

                        // TODO: �Ȱ��� ��ġ�� �Ź� Collecting �� ������ ������ ���� ���.
                        if (VisitList.Contains((x, y)))
                        {
                            continue;
                        }

                        // �˻� ��Ͽ� �߰�.
                        VisitList.Add((x, y));
                        
                        // Ÿ�� �߰�. (��� id, ���� ��ġ x, ���� ��ġ y)
                        var entity_id = TerrainMap.EntityManager.GetCellData(x, y);
                        if (entity_id > 0)
                        {
                            CollectTargets.Add((entity_id, _visit_x, _visit_y));

                            result = true;
                        }
                    }
                }

                return result;
            }
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

            // �ݹ�
            var visitor            = ObjectPool<CollectTargetVisitor>.Acquire();
            visitor.TerrainMap     = _terrain_map;
            visitor.Visitor      = _path_owner;
            visitor.Position       = (_x, _y);
            visitor.MoveDistance   = _move_distance;
            visitor.WeaponRangeMax = _weapon_range_max;
            visitor.WeaponRangeMin = _weapon_range_min;

            // FloodFill�� ���ؼ� ���� ������ Ÿ���� ã�ƺ��ô�. 
            PathAlgorithm.FloodFill(visitor);

            // Ÿ�� �̰�.
            _collect_targets.AddRange(visitor.GetCollectTargets());

            ObjectPool<CollectTargetVisitor>.Return(ref visitor);
        }

    }    
}