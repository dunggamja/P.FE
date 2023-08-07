using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumBattleTurnSide
    {
        None = 0,

        Attacker,
        Defender,
    }

    public class BattleSystemParam_BattleTurn : IBattleSystemParam
    {
        public BattleObject attacker { get; set; }
        public BattleObject defender { get; set; }

        //public EnumActionTurn output_action_turn { get; set; }
    }

    /// <summary>
    /// 전투 공/방 순서
    /// </summary>
    
    public class BattleSystem_BattleTurn : BattleSystem
    {
        struct BattleTurn
        {
            public int  GetTurnSpeed()              // 행동 순서
            {
                return 0;
            }
            public void AddTurnSpeed(int _value) { }    // 행동 순서 증/감 
            public void AddTurnCount(int _value) { }    // 행동 횟수 증/감 
            public void AddAttackCount(int _value) { }  // 행동당 공격 횟수 증/감


            public void ResetTurn() { }
            public bool IsRemainTurn() { return true; }
            public void ProcessTurn() { }
        }



        public const int      ADD_EXTRA_TURN_SPEED = 5; // 스피드 차이가 이 이상나면, 추가 행동이 발생합니다.

        public  EnumBattleTurnSide CurrentTurn  { get; private set; } = EnumBattleTurnSide.None;
        private BattleTurn         TurnAttacker { get; set; }
        private BattleTurn         TurnDefender { get; set; }


        protected override void OnEnter(IBattleSystemParam _param)
        {
            var param = _param as BattleSystemParam_BattleTurn;
            if (param == null)
                return;

            var attacker_status     = param.attacker.StatusManager;
            var defender_status     = param.defender.StatusManager;

            var attacker_memory     = param.attacker.Memory;
            var defender_memory     = param.defender.Memory;

            // 행동 순서를 계산한다.
            var attacker_action_turn   = attacker_status.Buff.Calculate(attacker_status, EnumBuffStatus.System_ActionTurn).Calculate(0);
            var defender_action_turn   = defender_status.Buff.Calculate(defender_status, EnumBuffStatus.System_ActionTurn).Calculate(0);

            // 행동 횟수를 계산한다. 
            var attacker_speed        = attacker_status.Calc_Speed();
            var defender_speed        = defender_status.Calc_Speed();

            // 속도가 특정 값 이상으로 차이가 나면 추가행동이 발생합니다. 
            var attacker_action_count = (attacker_speed - defender_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;
            var defender_action_count = (defender_speed - attacker_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;
            attacker_action_count     = attacker_status.Buff.Calculate(attacker_status, EnumBuffStatus.System_ActionCount).Calculate(attacker_action_count);
            defender_action_count     = defender_status.Buff.Calculate(defender_status, EnumBuffStatus.System_ActionCount).Calculate(defender_action_count);

            // 행동당 공격 횟수를 계산합니다. 
            var attacker_attack_count = attacker_status.Buff.Calculate(attacker_status, EnumBuffStatus.System_AttackCount).Calculate(1);
            var defender_attack_count = defender_status.Buff.Calculate(defender_status, EnumBuffStatus.System_AttackCount).Calculate(1);

            // 공격자/방어자 행동횟수/공격횟수를 기록해둡니다.
            TurnAttacker.ResetTurn();
            TurnDefender.ResetTurn();

            TurnAttacker.AddTurnSpeed(attacker_action_turn);
            TurnAttacker.AddTurnCount(attacker_action_count);
            TurnAttacker.AddAttackCount(attacker_attack_count);

            TurnDefender.AddTurnSpeed(defender_action_turn);
            TurnDefender.AddTurnCount(defender_action_count);
            TurnDefender.AddAttackCount(defender_attack_count);
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var param = _param as BattleSystemParam_BattleTurn;
            if (param == null)
                return true;



            var attacker_status = param.attacker.StatusManager;
            var defender_status = param.defender.StatusManager;

            // 공격자 / 방어자의 행동 순위를 비교합니다.


            var attacker_has_remain_turn = TurnAttacker.IsRemainTurn();
            var defender_has_remain_turn = TurnDefender.IsRemainTurn();

            if (attacker_has_remain_turn)
            {
                var is_attacker_turn = TurnAttacker.GetTurnSpeed() <= TurnDefender.GetTurnSpeed();

                if (is_attacker_turn || !defender_has_remain_turn)
                {
                    TurnAttacker.ProcessTurn();
                    CurrentTurn = EnumBattleTurnSide.Attacker;
                    return false;
                }
            }
            
            if (TurnDefender.IsRemainTurn())
            {
                TurnDefender.ProcessTurn();
                CurrentTurn = EnumBattleTurnSide.Defender;
                return false;
            }


            // 아무도 행동 할 수 없는 상태.
            CurrentTurn = EnumBattleTurnSide.None;
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            var param = _param as BattleSystemParam_BattleTurn;
            if (param == null)
                return;

            TurnAttacker.ResetTurn();
            TurnDefender.ResetTurn();
        }

    }
}

