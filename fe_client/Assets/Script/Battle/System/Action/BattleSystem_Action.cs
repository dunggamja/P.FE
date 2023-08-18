﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Action : BattleSystem
    {
       

        public bool IsHit         { get; private set; }
        public bool IsHitCritical { get; private set; }


        public BattleSystem_Action() : base(EnumSystem.BattleSystem_Action)
        { }


        public override void Reset()
        {
            IsHit         = false;
            IsHitCritical = false;
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            Reset();

            var attacker = _param.Attacker;
            var defender = _param.Defender;

            // 공격 전 스킬 사용.
            attacker.Skill.UseSkill(this, attacker);
            defender.Skill.UseSkill(this, defender);
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var attacker = _param.Attacker;
            var defender = _param.Defender;

            IsHit         = Calculate_Hit(_param);
            IsHitCritical = (IsHit) ? Calculate_HitCritical(_param) : false;




            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            var attacker = _param.Attacker;
            var defender = _param.Defender;

            // 공격 후 스킬 사용.
            attacker.Skill.UseSkill(this, attacker);
            defender.Skill.UseSkill(this, defender);
        }



        bool Calculate_Hit(IBattleSystemParam _param)
        {
            // 명중률 = 명중 - 회피
            var attacker   = _param.Attacker;
            var defender   = _param.Defender;

            // 스탯 계산.
            var hit        = attacker.Status.Calc_Hit();
            var dodge      = defender.Status.Calc_Dodge();
            var buff_value = attacker.Status.Buff.Collect(this, attacker, EnumBuffStatus.System_Hit) + defender.Status.Buff.Collect(this, defender, EnumBuffStatus.System_Hit);

            // 100분율 
            var calc_rate   = Math.Max(0, hit - dodge);
            calc_rate       = buff_value.Calculate(calc_rate);
            var random_rate = UnityEngine.Random.Range(0, 100);

            return random_rate < calc_rate;
        }

        bool Calculate_HitCritical(IBattleSystemParam _param)
        {
            // 필살 발생 확률 = 필살 - 필살 회피
            var attacker   = _param.Attacker;
            var defender   = _param.Defender;

            // 스탯 계산.
            var hit        = attacker.Status.Calc_Critical();
            var dodge      = defender.Status.Calc_DodgeCritical();
            var buff_value = attacker.Status.Buff.Collect(this, attacker, EnumBuffStatus.System_Critical) + defender.Status.Buff.Collect(this, defender, EnumBuffStatus.System_Critical);

            // 100분율 
            var calc_rate   = Math.Max(0, hit - dodge);
            calc_rate       = buff_value.Calculate(calc_rate);
            var random_rate = UnityEngine.Random.Range(0, 100);

            return random_rate < calc_rate;
        }

        int Calculate_Damage(IBattleSystemParam _param)
        {
            // 공격[마공] = 힘[마력] + 무기[마도서] 위력 * (1[1] or 2[2] or 3[3]) +기타[A](스킬 지형효과 등)
            var attacker = _param.Attacker;
            var defender = _param.Defender;

            // 스탯 계산.
            var might_physic = attacker.Status.Calc_Might_Physic();
            var might_magic = attacker.Status.Calc_Might_Magic();
            var defense_physic = defender.Status.Calc_Defense();
            var defense_magic = defender.Status.Calc_Resistance();

            return 0;

        }
    }

}
