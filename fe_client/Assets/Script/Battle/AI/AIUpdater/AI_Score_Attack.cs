using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    // 사정거리 내의 공격 타겟을 찾는 로직.
    public class AI_Score_Attack : IAIUpdater
    {
        public class Result : IPoolObject
        {
            public enum EnumScoreType
            {                
                Dead,             // 죽일 수 있는지 체크.
                DamageRate_Dealt, // 입힐 수 있는 데미지 양
                // HitRate,          // 명중률
                MoveCost,         // 이동 거리 (가까울수록 높은 점수)

                // DamageRate_Taken, // 내가 받는 데미지 양
                // DodgeRate,        // 회피율                

                // TOOD: 적에게 위험을 받는 위치인지 체크.
                // ThreatenRate,

                // TODO: 타겟의 우선순위 체크. (힐러 등 고 가치유닛?)
                // TargetPriority,
                MAX
            }

            // 각 항목별 점수. (기준도 뭐도 없이 그냥 정한 수치)
            static float Default_Score_Multiplier(EnumScoreType _type, float _score)
            {   
                switch(_type)         
                {
                    case EnumScoreType.Dead:             return 2f;
                    case EnumScoreType.DamageRate_Dealt: return 1f;
                    case EnumScoreType.MoveCost:         return 0.2f;
                    // case EnumScoreType.HitRate:          return 0.5f;
                    // case EnumScoreType.DamageRate_Taken: return -0.7f;
                    // case EnumScoreType.DodgeRate:        return 0.4f;
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


        public void Update(IAIDataManager _param)
        {
            if (_param == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_param.ID);
            if (owner_entity == null)
                return;

            // 행동들이 가능한 상태인지 체크.            
            var is_attackable = owner_entity.HasCommandEnable(EnumCommandFlag.Action);


            // 공격 행동이 불가능한 상태면 종료
            if (is_attackable == false)
                return;            

            // 사정거리 내에 적이 없으면 종료.
            var weapon_range = owner_entity.GetWeaponRange(0).max;
            var move_range   = owner_entity.PathMoveRange;
            if (Update_QueryEnemyInRange(
                    _param.ID, 
                    owner_entity.Cell, 
                    weapon_range + move_range) == false)
                return;


            // 공격 점수 갱신.
            Update_AttackScore(_param.ID);
        }

        private bool Update_QueryEnemyInRange(Int64 _entity_id, (int x, int y) _position, int _range)
        {
            var owner_entity = EntityManager.Instance.GetEntity(_entity_id);
            if (owner_entity == null)
                return false;


            using var list_target = ListPool<Int64>.AcquireWrapper();

            SpacePartitionManager.Instance.Query_Position_Range(
                list_target.Value,
                _position, 
                _range);

            foreach(var e in list_target.Value)
            {
                // 공격 가능한지 체크.
                if (AIHelper.Verify_AI_Enemy(owner_entity.ID, e) == false)
                    continue;


                return true;                
            }


            return false;

        }

        private void Update_AttackScore(Int64 _entity_id)
        {
            var owner_entity = EntityManager.Instance.GetEntity(_entity_id);
            if (owner_entity == null)
                return;

            // 코드가 너무 길어져서 변수들 캐싱.
            var owner_status     = owner_entity.StatusManager;
            var owner_blackboard = owner_entity.BlackBoard;
            var owner_inventory  = owner_entity.Inventory;
            var is_moveable      = owner_entity.HasCommandEnable(EnumCommandFlag.Move);

            // 착용중인 무기 / 아이템 ID
            var owner_weapon      = owner_status.Weapon;
            var equiped_weapon_id = owner_weapon.ItemID;


            // 점수 계산 결과값.
            using var current_score = ObjectPool<Result>.AcquireWrapper();

            // 소유 중인 무기들로 테스트 전투를 돌립니다.
            using var list_weapon = ListPool<Item>.AcquireWrapper();
            owner_inventory.CollectItem_Weapon_Available(list_weapon.Value, owner_entity);

            // 공격 가능한 타겟 목록.
            using var list_collect_target = ListPool<(Int64 target_id, int attack_pos_x, int attack_pos_y)>.AcquireWrapper();

            try
            {                            
                foreach(var e in list_weapon.Value)
                {
                    // 무기 장착.
                    if (owner_entity.ProcessAction(e, EnumItemActionType.Equip) == false)
                        continue;
                    
                    // 최대 이동 거리. 
                    var move_distance = (is_moveable) ? owner_entity.PathMoveRange : 0;

                    // 무기 사정거리. (최소/최대)
                    var range_min = owner_status.GetBuffedWeaponStatus(owner_weapon.ItemObject, EnumWeaponStatus.Range_Min);
                    var range_max = owner_status.GetBuffedWeaponStatus(owner_weapon.ItemObject, EnumWeaponStatus.Range);


                    // 공격 가능한 타겟 목록 초기화.
                    list_collect_target.Value.Clear();
                    
                    // Target Collect
                    CollectTarget(
                        list_collect_target.Value,
                        TerrainMapManager.Instance.TerrainMap,
                        owner_entity,
                        owner_entity.Cell.x,
                        owner_entity.Cell.y,
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
                        if (AIHelper.Verify_AI_Enemy(owner_entity.ID, target_id) == false)
                            continue;

                        // 점수 계산 결과값 초기화.
                        current_score.Value.Reset();
            
                        Score_Calculate(
                            current_score.Value,
                            owner_entity,
                            target_entity,
                            (attack_pos_x, attack_pos_y));

                        // 점수 계산.
                        var calculate_score = current_score.Value.CalculateScore();

                        // 점수 비교.
                        if (owner_blackboard.GetBPValueAsFloat(EnumEntityBlackBoard.AIScore_Attack) <= calculate_score)
                        {
                            // 높은 점수 셋팅.
                            owner_blackboard.Score_Attack.CopyFrom(current_score.Value);                            
                            owner_blackboard.SetBPValue(EnumEntityBlackBoard.AIScore_Attack, calculate_score); 
                        }
                    }
                }
            }  
            finally
            {
                // 무기 원상 복구.
                owner_entity.ProcessAction(owner_inventory.GetItem(equiped_weapon_id), EnumItemActionType.Equip);
            }    
        }




        static void Score_Calculate(Result _score, Entity _owner, Entity _target, (int x, int y) _attack_position)
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

            var damage_taken       = result.Attacker.HP_Before - result.Attacker.HP_After;
            var damage_dealt       = result.Defender.HP_Before - result.Defender.HP_After;

            var hit_rate           = Util.PERCENT(result.Attacker.HitRate, true);
            var dodge_rate         = Util.PERCENT(100 - result.Defender.HitRate, true);

            var target_is_dead     = result.Defender.HP_After <= 0 && 0f < hit_rate;

            


            // 공격자/타겟 HP
            var owner_hp  = Math.Max(1, _owner.StatusManager.Status.GetPoint(EnumUnitPoint.HP));
            var target_hp = Math.Max(1, _target.StatusManager.Status.GetPoint(EnumUnitPoint.HP));

            
            // 데미지 점수 셋팅. 
            _score.SetScore(Result.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / target_hp);
            // _score.SetScore(Result.EnumScoreType.DamageRate_Taken, (float)damage_taken / owner_hp);

            // 이동거리 점수 셋팅.
            var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            var distance_max     = Math.Max(1, _owner.PathMoveRange);
            _score.SetScore(Result.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // 명중률 셋팅.
            // _score.SetScore(Result.EnumScoreType.HitRate,   hit_rate);

            // 죽일 수 있는지 셋팅.
            _score.SetScore(Result.EnumScoreType.Dead, target_is_dead ? 1f : 0f);
            

            // 공격 위치 셋팅.
            _score.SetAttackPosition(_attack_position.x, _attack_position.y);
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

                        // TODO: 똑같은 위치를 매번 Collecting 할 것인지 말지는 추후 고려.
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

            // FloodFill을 통해서 공격 가능한 타겟을 찾아봅시다. 
            PathAlgorithm.FloodFill(visitor.Value);

            // 타겟 이관.
            _collect_targets.AddRange(visitor.Value.GetCollectTargets());

            // ObjectPool<CollectTargetVisitor>.Return(visitor.Value);
        }

    }    
}