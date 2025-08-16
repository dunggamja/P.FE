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
        public static Result Run_Plan(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id)
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


                CombatSystemManager.Instance.Setup(combat_param);

                while (CombatSystemManager.Instance.IsFinished == false)
                {
                    CombatSystemManager.Instance.Update();

                    var turn_system   = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
                    var damage_system = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Damage) as CombatSystem_Damage;

                    var is_attacker_turn = (turn_system   != null) ? turn_system.CombatTurn == CombatSystem_Turn.EnumCombatTurn.Attacker : false;
                    var damage           = (damage_system != null) ? damage_system.Result_Damage       : 0;
                    var critical_rate    = (damage_system != null) ? (int)(damage_system.Result_CriticalRate * 100) : 0;
                    var hit_rate         = (damage_system != null) ? (int)(damage_system.Result_HitRate * 100) : 0;

                    critical_rate = Math.Clamp(critical_rate, 0, 100);
                    hit_rate      = Math.Clamp(hit_rate, 0, 100);


                    // 전투중 Dmg, Cri, Hit 셋팅. 
                    var unit_info          = (is_attacker_turn) ? result.Attacker : result.Defender;
                    unit_info.Damage       = Math.Max(damage, unit_info.Damage);
                    unit_info.CriticalRate = Math.Max(critical_rate, unit_info.CriticalRate);
                    unit_info.HitRate      = Math.Max(hit_rate, unit_info.HitRate);

                    result.Actions.Add(Result_Action.Create(is_attacker_turn, damage));
                }


                // 전투 후 HP 셋팅
                result.Attacker.HP_After     = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);
                result.Defender.HP_After     = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                ObjectPool<CombatParam_Plan>.Return(combat_param);
            }


            // 전투 예측 후 스냅샷 복원
            GameSnapshot.Load(snapshot);


            // BattleLogManager.Instance.Logs.ForEach(log => 
            // {
            //     Debug.Log($"{log.LogType} {log.EntityID} {log.Value}");
            // });

            // Debug.Log("GameSnapshot.Load() Start");
            // Debug.Log("GameSnapshot.Load() End");

            return result;
        }
    }
}