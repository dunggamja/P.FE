using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumActionTurn
    {
        None = 0,

        Attacker,
        Defender,
    }

    public class BattleSystem_ActionTurn_Param : IBattleSystemParam
    {
        public BattleObject   input_attacker     { get; set; }
        public BattleObject   input_defender     { get; set; }

        public EnumActionTurn output_action_turn { get; set; }
    }

    public class BattleSystem_ActionTurn : BattleSystem
    {
        public const int FOLLOW_ACTION_TURN_SPEED = 5; // 스피드 차이가 이 이상나면, 추가 행동이 발생합니다.

        protected override void OnEnter(IBattleSystemParam _param)
        {
            var param = _param as BattleSystem_ActionTurn_Param;
            if (param == null)
                return;

            var attacker_status     = param.input_attacker.StatusManager;
            var defender_status     = param.input_defender.StatusManager;

            var attacker_blackboard = param.input_attacker.BlackBoard;
            var defender_blackboard = param.input_defender.BlackBoard;            


            var attacker_action_turn = attacker_status.Calc_ActionTurn(attacker_status);
            var defender_action_turn = defender_status.Calc_ActionTurn(defender_status);


        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {

            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            var param = _param as BattleSystem_ActionTurn_Param;
            if (param == null)
                return;

            var attacker_blackboard = param.input_attacker.BlackBoard;
            var defender_blackboard = param.input_defender.BlackBoard;

            attacker_blackboard.ActionCounter.Reset();
            defender_blackboard.ActionCounter.Reset();
        }
    }
}

