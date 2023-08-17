using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Action : BattleSystem
    {
       

        public enum EnumActionState
        {
            None,

            Before,  // 행동 전
            OnHit,   // 행동 명중했을 경우
            OnMiss,  // 행동 빗나갔을 경우
            OnBlock, // 행동 방해받음. (데미지 0도 여기로)
            After,   // 행동 후
        }

        
        public EnumActionState ActionState { get; private set; }


        public BattleSystem_Action() : base(EnumSystem.BattleSystem_Action)
        { }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            var attacker = _param.Attacker;
            var defender = _param.Defender;


            ActionState = EnumActionState.Before;

            // 공격 전 스킬 사용.
            attacker.Skill.UseSkill(this, attacker);
            defender.Skill.UseSkill(this, defender);
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var attacker = _param.Attacker;
            var defender = _param.Defender;

            //attacker


            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

            ActionState = EnumActionState.After;
        }

        public override void Reset()
        {
            
        }
    }

}
