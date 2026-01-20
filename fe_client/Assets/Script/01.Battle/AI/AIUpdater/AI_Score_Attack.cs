using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    // 사정거리 내의 공격 타겟을 찾는 로직.
    public class AI_Score_Attack : AI_Score_Base
    {
        public class Result : IPoolObject
        {
            public enum EnumScoreType
            {                
                TargetPriority,   // 타겟 우선순위.
                Kill,             // 죽일 수 있는지 체크.
                DamageRate_Dealt, // 입힐 수 있는 데미지 양
                DamageRate_Taken, // 내가 받는 데미지 양

                // Position,      // 공격 위치 선호도. (지형 등 고려)
                // TargetPriority,
                MAX
            }

            // 각 항목별 점수. (기준도 뭐도 없이 그냥 정한 수치)
            static float Default_Score_Multiplier(EnumScoreType _type, float _score)
            {   
                switch(_type)         
                {
                    // 타겟 우선순위.
                    case EnumScoreType.TargetPriority:   return 2f;
                    // 죽일 수 있는지 체크.
                    case EnumScoreType.Kill:             return 1f;
                    // 입힐 수 있는 데미지 양
                    case EnumScoreType.DamageRate_Dealt: return 1f;
                    // 내가 받는 데미지 양
                    case EnumScoreType.DamageRate_Taken: return -0.5f;                    
                }

                return 0f;
            }

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
                
                // 각 스코어는 0.0 ~ 1.0 까지만 유효.
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


            public float CalculateScore(Func<EnumScoreType, float, float> _func_score_multiplier = null)
            {
                if (_func_score_multiplier == null)
                    _func_score_multiplier = Default_Score_Multiplier;

                var score_total = 0f;
                var score_max = 0f;

                // 점수 합산.
                for (int i = 0; i < m_score.Length; ++i)
                {
                    score_total += Mathf.Max(_func_score_multiplier((EnumScoreType)i, m_score[i]), 0f);
                    score_max   += Mathf.Max(_func_score_multiplier((EnumScoreType)i, 1f), 0f);
                }

                // 예외처리.
                if (score_max <= float.Epsilon)
                    return 0f;

                // 점수는 0.0 ~ 1.0으로 제한.
                return Mathf.Clamp01(score_total / score_max);
            }
        }


        public enum EnumBehavior
        {
            Normal, // 아무나 공격가능한 타겟 체크.            
            Target, // 타겟을 공격가능한지 체크, 
            Fixed,  // 고정된 위치에서 공격.
        }

        public EnumBehavior     BehaviorType     { get; private set; } = EnumBehavior.Normal;

        
        public AI_Score_Attack(EnumBehavior _behavior_type)
        {
            BehaviorType = _behavior_type;
        }


        private int GetMoveRange(Entity _entity)
        {
            // 행동 타입 체크.
            if (BehaviorType == EnumBehavior.Fixed)
                return 0;

            // 이동 가능한 상태인지 체크.
            if (_entity.HasCommandEnable(EnumCommandFlag.Move) == false)
                return 0;

            return _entity.PathMoveRange;
        } 


        private bool Verify_Enemy(Entity _entity, Entity _target) 
        {
            if (_entity == null || _target == null)
                return false;

            // 적인지 체크.
            if (AIHelper.Verify_IsEnemy(_entity.ID, _target.ID) == false)
                return false;

            // 무시 대상인지 체크
            if (AIHelper.Verify_Target_Ignore(_entity, _target))
                return false;

            
            if (BehaviorType == EnumBehavior.Target)
            {
                // 포커싱 대상인지 체크.
                if (AIHelper.Verify_Target_Focus(_entity, _target) == false)
                    return false;
            }

            return true;
        }
       

        private bool Verify_EnemyInRange(Entity _entity)
        {
            if (_entity == null)
                return false;


            var entity_id       = _entity.ID;
            var entity_position = _entity.Cell;

            // 소유 중인 무기중 최대 사정거리.
            var weapon_range    = _entity.GetWeaponRange(0).max;

            // 이동가능한 거리.
            var move_range      = GetMoveRange(_entity);

            using var list_target = ListPool<Int64>.AcquireWrapper();

            // 정해진 범위내에 위치한 entity 목록을 조회합니다.
            SpacePartitionManager.Instance.Query_Position_Range(
                list_target.Value,
                entity_position, 
                weapon_range + move_range);

            // 
            foreach(var e in list_target.Value)
            {
                // 공격 가능한 적인지 체크합니다.
                if (Verify_Enemy(_entity, EntityManager.Instance.GetEntity(e)))
                    return true;            
            }


            return false;

        }


        


        protected override bool OnUpdate(IAIDataManager _param)
        {
            if (_param == null)
                return false;

            var owner_entity = EntityManager.Instance.GetEntity(_param.ID);
            if (owner_entity == null)
                return false;

            // 공격이 가능한 상태인지 체크.            
            var is_attackable = owner_entity.HasCommandEnable(EnumCommandFlag.Action);
            if (is_attackable == false)
                return false; 

            // 사정거리 내에 적이 없으면 종료.            
            if (Verify_EnemyInRange(owner_entity) == false)
                return false;

            // 사정거리 내 공격 가능한 적들의 점수 계산.
            return Calculate_AttackScore(owner_entity);
        }

        private void Process_Attack_Fixed(Int64 _entity_id)
        {
            var owner_entity = EntityManager.Instance.GetEntity(_entity_id);
            if (owner_entity == null)
                return;
        }

        private bool Calculate_AttackScore(Entity _entity)
        {
            if (_entity == null)
                return false;

            // 코드가 너무 길어져서 변수들 캐싱.
            var owner_status     = _entity.StatusManager;
            var owner_blackboard = _entity.BlackBoard;
            var owner_inventory  = _entity.Inventory;

            // 착용중인 무기 / 아이템 ID
            var owner_weapon      = owner_status.Weapon;
            var equiped_weapon_id = owner_weapon.ItemID;


            // 점수 계산 결과값.
            using var current_score = ObjectPool<Result>.AcquireWrapper();

            // 소유 중인 무기들로 테스트 전투를 돌려봅시다.
            using var list_weapon = ListPool<Item>.AcquireWrapper();
            owner_inventory.CollectItem_Weapon_Available(list_weapon.Value, _entity);

            // 공격 가능한 타겟 목록.
            using var list_collect_target = ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.AcquireWrapper();

            // 공격 대상이 될 수 있는 타겟 갯수.
            int target_count = 0;

            try
            {                            
                foreach(var e in list_weapon.Value)
                {
                    // 무기 장착.
                    if (_entity.ProcessAction(e, EnumItemActionType.Equip) == false)
                        continue;
                    
                    // 최대 이동 거리. 
                    var move_distance = GetMoveRange(_entity);

                    // 무기 사정거리. (최소/최대)
                    var range_min = owner_status.GetBuffedWeaponStatus(e, EnumWeaponStatus.Range_Min);
                    var range_max = owner_status.GetBuffedWeaponStatus(e, EnumWeaponStatus.Range);


                    // 공격 가능한 타겟 목록 초기화.
                    list_collect_target.Value.Clear();
                    
                    // Target Collect
                    CollectTarget(
                        list_collect_target.Value,
                        TerrainMapManager.Instance.TerrainMap,
                        _entity,
                        _entity.Cell.x,
                        _entity.Cell.y,
                        move_distance,
                        range_min,
                        range_max);

                    // 타겟 순회.
                    foreach((var target_id, var attack_pos_x, var attack_pos_y) in list_collect_target.Value)
                    {
                        var target_entity = EntityManager.Instance.GetEntity(target_id);
                        if (target_entity == null)
                            continue; 

                        // 죽었으면 타겟팅에서 제외
                        if (target_entity.IsDead)
                            continue;     

                        // 공격 가능한지 체크.
                        if (Verify_Enemy(_entity, target_entity) == false)
                            continue;

                        // 점수 계산 결과값 초기화.
                        current_score.Value.Reset();
            
                        Score_Calculate(
                            current_score.Value,
                            _entity,
                            target_entity,
                            (attack_pos_x, attack_pos_y));

                        // 점수 계산.
                        var calculate_score = current_score.Value.CalculateScore();

                        // 점수 비교.
                        if (_entity.AIManager.AIBlackBoard.GetBPValueAsFloat(EnumAIBlackBoard.Score_Attack) <= calculate_score)
                        {
                            // 높은 점수 셋팅.
                            _entity.AIManager.AIBlackBoard.Score_Attack.CopyFrom(current_score.Value);                            
                            _entity.AIManager.AIBlackBoard.SetBPValue(EnumAIBlackBoard.Score_Attack, calculate_score); 
                        }

                        // 타겟 갯수 증가.
                        ++target_count;
                    }
                }
            }  
            finally
            {
                // 무기 원상 복구.
                _entity.ProcessAction(owner_inventory.GetItem(equiped_weapon_id), EnumItemActionType.Equip);
            }    


            // 공격 가능한 타겟이 있었는지 체크.
            return target_count > 0;
        }


        private void Score_Calculate(Result _score, Entity _owner, Entity _target, (int x, int y) _attack_position)
        {
            if (_owner == null || _target == null)
                return;


            // 전투 결과 스코어
            _score.Reset();
            _score.Setup(_target.ID, _owner.StatusManager.Weapon.ItemID);           
            

            var result = CombatHelper.Run_Plan(
                _attacker_id: _owner.ID, 
                _target_id:   _target.ID, 
                _weapon_id:   _owner.StatusManager.Weapon.ItemID,
                _command_type: EnumUnitCommandType.Attack,
                _attack_position: _attack_position);

            if (result == null)
                return;

            var damage_dealt_count = result.Actions.Count(e => e.isAttacker);   
            var damage_taken_count = result.Actions.Count(e => e.isAttacker == false);   


            var hit_rate           = Util.PERCENT(result.Attacker.HitRate, true);
            var dodge_rate         = Util.PERCENT(100 - result.Defender.HitRate, true);

            // 데미지 계산 (명중률이 0인 경우는 아예 0으로 처리.)
            var damage_taken       = (0 < hit_rate)   ? result.Attacker.HP_Before - result.Attacker.HP_After : 0;
            var damage_dealt       = (0 < dodge_rate) ? result.Defender.HP_Before - result.Defender.HP_After : 0;

            // 타겟을 죽일수 있는지 체크.
            var target_kill        = result.Defender.HP_After <= 0 && 0 < damage_dealt;


            // 포커싱 대상이 있다면 점수 셋팅.
            var focus_score        = 0f;
            if (TagManager.Instance.IsExistTagOwner(_owner, EnumTagAttributeType.TARGET_FOCUS))
            {
                if (AIHelper.Verify_Target_Focus(_owner, _target))
                {
                    // 포커싱 대상일 경우.
                    focus_score = 1f;                    
                }
                else
                {
                    // 포커싱 대상이 아니라면 특정 거리내에 있는 포커싱 타겟과의 거리를 기반으로 점수 체크.
                    const int QUERY_RANGE = 10;
                    using var list_target = ListPool<Int64>.AcquireWrapper();
                    SpacePartitionManager.Instance.Query_Position_Range(list_target.Value, _target.Cell, QUERY_RANGE);

                    // 그 중 가장 짧은 거리에 있는 포커싱 타겟을 찾습니다.
                    int min_distance = int.MaxValue;
                    foreach(var e in list_target.Value)
                    {
                        var check_target = EntityManager.Instance.GetEntity(e);                        
                        if (check_target == null)
                            continue;

                        // 포커싱 대상과의 거리를 기반으로 점수 체크.
                        var distance = PathAlgorithm.Distance(_target.Cell, check_target.Cell);
                        if (distance < min_distance)
                            min_distance = distance;
                    }

                    // 거리를 기반으로 점수 셋팅. 진짜 포커싱을 한 경우와 차이를 두기위해 0.5 곱 처리.
                    focus_score  = Mathf.Clamp01((float)(QUERY_RANGE - min_distance) / QUERY_RANGE);
                    focus_score *= 0.5f;                    
                }
            }



            // 포커싱 점수.
            _score.SetScore(Result.EnumScoreType.TargetPriority, focus_score);

                      
            // 데미지 점수 셋팅. (30점 만점) 
            // TODO: 만점점수는 레벨 밸런스에 따라서 변동이 되어야 할듯.
            _score.SetScore(Result.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / 30f);
            _score.SetScore(Result.EnumScoreType.DamageRate_Taken, (float)damage_taken / 30f);

            // 죽일 수 있는지 셋팅.
            _score.SetScore(Result.EnumScoreType.Kill, target_kill ? 1f : 0f);

            // 공격 위치 셋팅.
            _score.SetAttackPosition(_attack_position.x, _attack_position.y);


            // // 이동거리 점수 셋팅.
            // var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            // var distance_max     = Math.Max(1, _owner.PathMoveRange);
            // _score.SetScore(Result.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // 명중률 셋팅.
            // _score.SetScore(Result.EnumScoreType.HitRate,   hit_rate);
        }







        class CollectTargetVisitor : PathAlgorithm.IFloodFillVisitor, IPoolObject
        {
            public TerrainMap     TerrainMap     { get; set; }  
            public IPathOwner     Visitor        { get; set; }  
            public (int x, int y) Position       { get; set; }  
            public int            MoveDistance   { get; set; }   
            public bool           VisitOnlyEmptyCell  => true;
            public bool           IsStop()   => false;
            
            
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
                Visitor        = null;
                Position       = (0, 0);
                MoveDistance   = 0;
                WeaponRangeMin = 0;
                WeaponRangeMax = 0;
                CollectTargets.Clear();
                VisitList.Clear();
            }



            public void Visit(int _visit_x, int _visit_y)
            {
                // bool result = false;

                for(int i = -WeaponRangeMax; i <= WeaponRangeMax; ++i)
                {
                    for(int k = -WeaponRangeMax; k <= WeaponRangeMax; ++k)
                    {
                        var x = _visit_x + i;
                        var y = _visit_y + k;

                        // 무기 사거리 체크
                        var distance = PathAlgorithm.Distance(_visit_x, _visit_y, x, y);
                        if (distance < WeaponRangeMin || WeaponRangeMax < distance)
                        {
                            continue;
                        }

                        // 똑같은 위치를 매번 방문하지 않도록 예외처리.
                        if (VisitList.Contains((x, y)))
                        {
                            continue;
                        }

                        // 검사 기록에 추가.
                        VisitList.Add((x, y));
                        
                        // 타겟 추가. (대상 id, 공격 위치 x, 공격 위치 y)
                        var entity_id = TerrainMap.EntityManager.GetCellData(x, y);
                        if (entity_id > 0)
                        {
                            CollectTargets.Add((entity_id, _visit_x, _visit_y));
                            // result = true;
                        }
                    }
                }

                // return result;
            }
        }


        static void
            CollectTarget(
            List<(Int64 target_id, int attack_pos_x, int atatck_pos_y)> _collect_targets,
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

            // 콜백
            using var visitor            = ObjectPool<CollectTargetVisitor>.AcquireWrapper();
            visitor.Value.TerrainMap     = _terrain_map;
            visitor.Value.Visitor        = _path_owner;
            visitor.Value.Position       = (_x, _y);
            visitor.Value.MoveDistance   = _move_distance;
            visitor.Value.WeaponRangeMax = _weapon_range_max;
            visitor.Value.WeaponRangeMin = _weapon_range_min;

            // FloodFill을 통해서 공격 가능한 타겟을 수집합니다.
            PathAlgorithm.FloodFill(visitor.Value);

            _collect_targets.AddRange(visitor.Value.GetCollectTargets());

            // ObjectPool<CollectTargetVisitor>.Return(visitor.Value);
        }

   
   
   
   
    }    
}