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


        // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
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


            // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
            var snapshot = GameSnapshot.Save();





            // Debug.Log("GameSnapshot.Save() Start");
            // Debug.Log("GameSnapshot.Save() End");

            // 공격자 무기 장착.
            attacker.StatusManager.Weapon.Equip(_weapon_id);

            // 공격자 위치 업데이트.
            attacker.UpdateCellPosition(
                _attack_position,
                (_apply: false, _immediatly: false),
                _is_plan: false);


            var result = new Result();

            // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
            {
                var combat_param = ObjectPool<CombatParam_Plan>
                    .Acquire()
                    .Set(attacker, target);

                // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                result.Attacker.EntityID  = attacker.ID;
                result.Attacker.WeaponID  = _weapon_id;
                result.Attacker.HP_Before = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                result.Defender.EntityID  = target.ID;
                result.Defender.WeaponID  = target.StatusManager.Weapon.ItemID;
                result.Defender.HP_Before = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);


                // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                {
                    CombatSystemManager.Instance.Setup(combat_param);
                    while (CombatSystemManager.Instance.IsFinished == false)
                        CombatSystemManager.Instance.Update();
                }

                // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                var damage_result = CombatSystemManager.Instance.GetCombatDamageResult();
                foreach (var damage in damage_result)
                {
                    // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                    var is_attacker  = (damage.AttackerID == attacker.ID);

                    // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                    var unit_info          = (is_attacker) ? result.Attacker: result.Defender;
                    unit_info.Damage       = Math.Max(damage.Result_Damage, unit_info.Damage);
                    unit_info.HitRate      = Math.Max(damage.Result_HitRate_Percent, unit_info.HitRate);
                    unit_info.CriticalRate = Math.Max(damage.Result_CriticalRate_Percent, unit_info.CriticalRate);


                    // 공격자와 타겟 데미지 저장.
                    var damage_value = damage.Result_Damage;

                    result.Actions.Add(Result_Action.Create(is_attacker, damage_value));
                }

                // 공격자와 타겟 HP 상태 저장.
                result.Attacker.HP_After     = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);
                result.Defender.HP_After     = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                ObjectPool<CombatParam_Plan>.Return(combat_param);
            }


            // 공격 후 상태 복구.
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

            // TODO: 일단 FixedObject 제외.
            if (target.IsFixedObject)
                return false;

            // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.

          
            // 공격자와 타겟이 같은 진영인 경우 제외.
            var is_alliance = BattleSystemManager.Instance.IsFactionAlliance(attacker.GetFaction(), target.GetFaction());
            if (is_alliance)
                return false;

            return true;
        }
    }
}