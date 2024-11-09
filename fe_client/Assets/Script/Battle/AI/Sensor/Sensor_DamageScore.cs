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
                
                // 각 스코어는 0.0 ~ 1.0 까지만 유효.
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


                    // TODO: 적/아군 체크에 대한 함수는 따로 빼야한다.// 혼돈같은 상태이상도 있을수 있고... NPC인데 아군도 있을 수 있고...
                    if (target_entity.GetFaction() == owner_entity.GetFaction())
                        continue;

                    // 점수 계산.
                    var current_score = Score_Calculate(owner_entity, target_entity);

                    // 위치 셋팅.
                    current_score.SetAttackPosition(attack_position_x, attack_position_y);

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
            CombatSystemManager.Instance.Setup(CombatParam_Plan.Cache.Set(_owner, _target));


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
                                
                // 무기 사거리 범위 안에 들어온 타겟들 콜렉팅
                for(int i = -_weapon_range_max; i <= _weapon_range_max; ++i)
                {
                    for(int k = -_weapon_range_max; k <= _weapon_range_max; ++k)
                    {
                        var x = _cell.x + i;
                        var y = _cell.y + k;

                        // 무기 사거리 체크
                        var distance = PathAlgorithm.Distance(_cell.x, _cell.y, x, y);
                        if (distance < _weapon_range_min || _weapon_range_max < distance)
                        {
                            continue;
                        }

                        // TODO: 똑같은 위치를 매번 Collecting 할 것인지 말지는 추후 고려.
                        if (close_list_attack.Contains((x, y)))
                        {
                            continue;
                        }

                        // 검사 기록에 추가.
                        close_list_attack.Add((x, y));
                        
                        // 타겟 추가. (대상 id, 공격 위치 x, 공격 위치 y)
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