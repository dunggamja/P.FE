using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Damage : BattleSystem
    {
        public bool IsHit         { get; private set; }
        public bool IsHitCritical { get; private set; }
        public int  HitDamage     { get; private set; }


        public BattleSystem_Damage() : base(EnumSystem.BattleSystem_Damage)
        { }


        public override void Reset()
        {
            IsHit         = false;
            IsHitCritical = false;
            HitDamage     = 0;
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            Reset();

            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 공격 전 스킬 사용.
            dealer.Skill.UseSkill(this, dealer);
            target.Skill.UseSkill(this, target);
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 명중 / 필살 / 데미지 계산.
            IsHit         = Calculate_Hit(_param);
            IsHitCritical = (IsHit) ? Calculate_HitCritical(_param) : false;
            HitDamage     = (IsHit) ? Calculate_Damage(_param) : 0;
            HitDamage    *= (IsHitCritical) ? 3 : 1;

            // 데미지 적용 여기서 한다.



            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 공격 후 스킬 사용.
            dealer.Skill.UseSkill(this, dealer);
            target.Skill.UseSkill(this, target);
        }



        bool Calculate_Hit(IBattleSystemParam _param)
        {
            // 명중률 = 명중 - 회피
            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 스탯 계산.
            var hit        = dealer.Status.Calc_Hit();
            var dodge      = target.Status.Calc_Dodge();
            var buff_value = dealer.Status.Buff.Collect(this, dealer, EnumBuffStatus.System_Hit) + target.Status.Buff.Collect(this, target, EnumBuffStatus.System_Hit);

            // 100분율 
            var calc_rate   = Math.Max(0, buff_value.Calculate(hit - dodge));
            var random_rate = UnityEngine.Random.Range(0, 100);

            return random_rate < calc_rate;
        }

        bool Calculate_HitCritical(IBattleSystemParam _param)
        {
            // 필살 발생 확률 = 필살 - 필살 회피
            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 스탯 계산.
            var hit        = dealer.Status.Calc_Critical();
            var dodge      = target.Status.Calc_DodgeCritical();
            var buff_value = dealer.Status.Buff.Collect(this, dealer, EnumBuffStatus.System_Critical) + target.Status.Buff.Collect(this, target, EnumBuffStatus.System_Critical);

            // 100분율 
            var calc_rate   = Math.Max(0, buff_value.Calculate(hit - dodge));
            var random_rate = UnityEngine.Random.Range(0, 100);

            return random_rate < calc_rate;
        }

        int Calculate_Damage(IBattleSystemParam _param)
        {
            // 피해량    = (공격[마공] - 수비[마방])
            var dealer = GetDamageDealer(_param);
            var target = GetDamageTarget(_param);

            // 스탯 계산.
            var might_physic   = dealer.Status.Calc_Might_Physic();
            var might_magic    = dealer.Status.Calc_Might_Magic();
            var defense_physic = target.Status.Calc_Defense();
            var defense_magic  = target.Status.Calc_Resistance();

            var damage_physic  = might_physic - defense_physic;
            var damage_magic   = might_magic  - defense_magic;

            var buff_value     = dealer.Status.Buff.Collect(this, dealer, EnumBuffStatus.System_Damage) + target.Status.Buff.Collect(this, target, EnumBuffStatus.System_Damage);
            var damage         = Math.Max(0, buff_value.Calculate(damage_physic + damage_magic));

            return damage;
        }

        BattleObject GetDamageDealer(IBattleSystemParam _param)
        {
            var turn_system = SystemManager.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return null;

            switch(turn_system.TurnPhase)
            {
                case BattleSystem_Turn.EnumTurnPhase.Attacker: return _param.Attacker;
                case BattleSystem_Turn.EnumTurnPhase.Defender: return _param.Defender;
            }

            return null;
        }

        BattleObject GetDamageTarget(IBattleSystemParam _param)
        {
            var turn_system = SystemManager.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return null;

            switch (turn_system.TurnPhase)
            {
                case BattleSystem_Turn.EnumTurnPhase.Attacker: return _param.Defender;
                case BattleSystem_Turn.EnumTurnPhase.Defender: return _param.Attacker;
            }

            return null;
        }
    }

}
