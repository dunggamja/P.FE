using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class Sensor_Target_Score : ISensor
    {
        public class ScoreResult
        {
            public enum EnumScoreType
            {
                // Tactical,     // 전술적 타겟 TODO: 요것은 어떻게 제어할지 고민좀 해보자...
                
                DamageRate_Dealt, // 입힐 수 있는 데미지 양
                DamageRate_Taken, // 내가 받는 데미지 양

                HitRate,
                DodgeRate,
                
                MoveCost,     // 이동 거리

                MAX
            }

            // 각 항목별 점수., 하드코딩해두고...
            // 시나리오에 따라서 제어가 필요할 경우에는 어떻게 제어할 지 고민해보자 . 
            static Dictionary<EnumScoreType, float> s_score_multiplier = new ()
            {            
                { EnumScoreType.DamageRate_Dealt, 5f  },
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
                
                // 각 스코어는 0.0 ~ 1.0 까지만 유효.
                score[index] = Mathf.Clamp01(_score_value);                
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

            // 착용중인 무기 / 아이템 ID
            var weapon            = owner_entity.StatusManager.Weapon;
            var equiped_weapon_id = weapon.ItemID;

            // 결과값은 가장 높은 것 1개만 저장해봅세...
            BestScore.Reset();


            // 소유 중인 무기들로 테스트 전투를 돌립니다.
            foreach(var e in owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon))
            {
                // 테스트 전투를 연산하기 위해 착용중인 무기를 바꿔줍니다.
                weapon.Equip(e.ID);

                // 최대 이동 거리.
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
                    var target_entity = EntityManager.Instance.GetEntity(target_id);
                    if (target_entity == null)
                        continue;          

                    // 점수 계산.
                    var current_score = Score_Calculate(owner_entity, target_entity);

                    // 결과 값 변경.
                    if (BestScore.Calculate_Total_Score() <= current_score.Calculate_Total_Score())
                    {
                        BestScore = current_score;
                    }
                }
            }

            // 무기 원상 복구.
            weapon.Equip(equiped_weapon_id);
        }


        static ScoreResult Score_Calculate(Entity _owner, Entity _target)
        {
            if (_owner == null || _target == null)
                return new ScoreResult();


            // 전투 결과 스코어
            var current_score = new ScoreResult().Setup(_target.ID, _owner.StatusManager.Weapon.ItemID);

            // 테스트 전투를 돌려서 계산을 해본다.                   
            CombatSystemManager.Instance.Setup(TestCombat_Param.Cache.Set(_owner, _target));


            // 공격자/타겟이 입힌 데미지.
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

                // 연산 결과.
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

            // 공격자/타겟 HP
            var owner_hp  = Math.Max(1, _owner.StatusManager.Status.GetPoint(EnumUnitPoint.HP));
            var target_hp = Math.Max(1, _target.StatusManager.Status.GetPoint(EnumUnitPoint.HP));
            
            // 데미지 점수 셋팅. 
            current_score.SetScore(ScoreResult.EnumScoreType.DamageRate_Dealt, (float)damage_dealt / target_hp);
            current_score.SetScore(ScoreResult.EnumScoreType.DamageRate_Taken, (float)damage_taken / owner_hp);

            // 이동거리 점수 셋팅.
            var distance_current = PathAlgorithm.Distance(_owner.Cell.x, _owner.Cell.y, _target.Cell.x, _target.Cell.y);
            var distance_max     = _owner.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);
            current_score.SetScore(ScoreResult.EnumScoreType.MoveCost, 1f - (float)distance_current / distance_max);

            // 명중 / 회피 점수 셋팅.
            current_score.SetScore(ScoreResult.EnumScoreType.HitRate,   hit_rate   / Math.Max(1, damage_dealt_count));
            current_score.SetScore(ScoreResult.EnumScoreType.DodgeRate, dodge_rate / Math.Max(1, damage_taken_count));

            return current_score;
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
                        if (distance <= 0)
                        {
                            // 위치가 겹치는 경우는 없다.
                            continue;
                        }

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
                        var entity_id = _terrain_map.BlockManager.FindEntityID(x, y);
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

                        // 이미 체크하였음.
                        if (close_list_move.Contains((x, y)))
                        {
                            continue;
                        }

                        // 가로, 세로 1칸씩만 이동가능. (대각선 이동 없음)
                        if (1 < PathAlgorithm.Distance(item.x, item.y, x, y))
                        {
                            continue;
                        }
                        
                        // 이동 불가능 지역은 거른다.
                        var move_cost  = Terrain_Attribute.Calculate_MoveCost(_path_owner_attribute, _terrain_map.Attribute.GetAttribute(x, y));
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

                        // open_list 에 추가.
                        open_list_move.Add((x, y, total_cost));
                    }
                }
            }
            
            

            return list_target;
        }

    }    
}