using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{


    public static class CombatHelper
    {
       
        public struct Result_Action
        {
            public bool isAttacker;
            public int  Damage;

            public static Result_Action Create(bool _isAttacker, int _damage)
            {
                return new Result_Action()
                {
                    isAttacker = _isAttacker,
                    Damage     = _damage
                };
            }

        }
        public class Result_Unit
        {
            public Int64           EntityID     { get; set; }
            public Int64           WeaponID     { get; set; }
            public int             Damage       { get; set; }
            public int             HitRate      { get; set; }
            public int             CriticalRate { get; set; }
            public int             HP_Before    { get; set; }
            public int             HP_After     { get; set; }
        }

        public class Result
        {
            public Result_Unit         Attacker { get; set; } = new();
            public Result_Unit         Defender { get; set; } = new();
            public List<Result_Action> Actions  { get; set; } = new();
        }


        // 전투 예측 실행.
        public static Result Run_Plan(
            Int64          _attacker_id,
            Int64          _target_id,
            Int64          _weapon_id,
            (int x, int y) _attack_position)
        {
            
            var attacker     = EntityManager.Instance.GetEntity(_attacker_id);
            var target       = EntityManager.Instance.GetEntity(_target_id);  

            if (attacker == null || target == null)
                return null;


            // 전투 예측 전 스냅샷 저장.
            var snapshot = GameSnapshot.Save();





            // Debug.Log("GameSnapshot.Save() Start");
            // Debug.Log("GameSnapshot.Save() End");

            // 무기 장착.
            attacker.StatusManager.Weapon.Equip(_weapon_id);

            // 공격 위치 셋팅.
            attacker.UpdateCellPosition(
                _attack_position,
                (_apply: false, _immediatly: false),
                _is_plan: false);


            var result = new Result();

            // 전투 예측 실행.
            {
                var combat_param = ObjectPool<CombatParam_Plan>
                    .Acquire()
                    .Set(attacker, target);

                // 전투 전 데이터 셋팅.
                result.Attacker.EntityID  = attacker.ID;
                result.Attacker.WeaponID  = _weapon_id;
                result.Attacker.HP_Before = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                result.Defender.EntityID  = target.ID;
                result.Defender.WeaponID  = target.StatusManager.Weapon.ItemID;
                result.Defender.HP_Before = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);


                // 전투 예측 실행.
                {
                    CombatSystemManager.Instance.Setup(combat_param);
                    while (CombatSystemManager.Instance.IsFinished == false)
                        CombatSystemManager.Instance.Update();
                }

                // 전투 예측 결과 정리.
                var damage_result = CombatSystemManager.Instance.GetCombatDamageResult();
                foreach (var damage in damage_result)
                {
                    // 공격자 / 방어자 구분.
                    var is_attacker  = (damage.AttackerID == attacker.ID);

                    // 유닛 정보 셋팅. (데미지, 명중률, 치명타율)
                    var unit_info          = (is_attacker) ? result.Attacker: result.Defender;
                    unit_info.Damage       = Math.Max(damage.Result_Damage, unit_info.Damage);
                    unit_info.HitRate      = Math.Max(damage.Result_HitRate_Percent, unit_info.HitRate);
                    unit_info.CriticalRate = Math.Max(damage.Result_CriticalRate_Percent, unit_info.CriticalRate);


                    // 데미지 셋팅.
                    var damage_value = damage.Result_Damage;

                    result.Actions.Add(Result_Action.Create(is_attacker, damage_value));
                }

                // 전투 후 HP 셋팅
                result.Attacker.HP_After     = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);
                result.Defender.HP_After     = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                ObjectPool<CombatParam_Plan>.Return(ref combat_param);
            }


            // 전투 예측 후 스냅샷 복원
            GameSnapshot.Load(snapshot, _is_plan: true);

            return result;
        }
    
    
    
        public static bool IsAttackable(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // TODO: 혼란 체크.

            // 아군이면 공격 불가.
            var is_alliance = BattleSystemManager.Instance.IsFactionAlliance(attacker.GetFaction(), target.GetFaction());
            if (is_alliance)
                return false;

            return true;
        }
    }
}