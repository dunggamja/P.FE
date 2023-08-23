using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Damage : BattleSystem
    {
        const int ADVANTAGE_HIT               = 20;
        const int CRITICAL_DAMAGE_MULTIPLIER  = 3;
        const int EFFECTIVE_DAMAGE_MULTIPLIER = 2;

        public EnumAdvantageState WeaponAdvantage     { get; private set; }
        public bool               WeaponEffectiveness { get; private set; }

        public bool Result_Hit          { get; private set; }
        public bool Result_Critical     { get; private set; }

        public int  Result_HitRate      { get; private set; }
        public int  Result_CriticalRate { get; private set; }
        public int  Result_Damage       { get; private set; }


        public BattleSystem_Damage() : base(EnumSystem.BattleSystem_Damage)
        { }


        public override void Reset()
        {
            WeaponAdvantage     = EnumAdvantageState.None;
            WeaponEffectiveness = false;


            Result_Hit          = false;
            Result_Critical     = false;

            Result_HitRate      = 0;
            Result_CriticalRate = 0;
            Result_Damage       = 0;
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            Reset();

            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 무기 상성/특효에 대한 값 셋팅.
            WeaponAdvantage     = Calculate_WeaponAdvantage(_param);
            WeaponEffectiveness = Calculate_WeaponEffectiveness(_param);

            // 공격 전 스킬 사용.
            dealer.Skill.UseSkill(this, dealer);
            target.Skill.UseSkill(this, target);
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 명중 / 필살 / 데미지 계산.
            Result_HitRate       = Calculate_HitRate(_param);
            Result_Hit           = Util.Random100_Result(Result_HitRate);

            Result_CriticalRate  = (Result_Hit) ? Calculate_CriticalRate(_param) : 0;
            Result_Critical      = Util.Random100_Result(Result_CriticalRate);

            Result_Damage        = (Result_Hit) ? Calculate_Damage(_param) : 0;

            // 크리티컬 & 특효 적용
            Result_Damage       *= (WeaponEffectiveness) ? EFFECTIVE_DAMAGE_MULTIPLIER : 1;
            Result_Damage       *= (Result_Critical)     ? CRITICAL_DAMAGE_MULTIPLIER  : 1;

            // 데미지 적용 여기서 한다.
            target.ApplyDamage(Result_Damage);

            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 공격 후 스킬 사용.
            dealer.Skill.UseSkill(this, dealer);
            target.Skill.UseSkill(this, target);
        }



        int Calculate_HitRate(IBattleSystemParam _param)
        {
            // 명중률 = 명중 - 회피
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 스탯  & 버프 계산.
            var hit        = dealer.StatusManager.Calc_Hit();
            var dodge      = target.StatusManager.Calc_Dodge();
            var buff_value = dealer.StatusManager.Buff.Collect(this, dealer, EnumBuffStatus.System_Hit) + target.StatusManager.Buff.Collect(this, target, EnumBuffStatus.System_Hit);


            // 무기 상성 적용.
            switch(WeaponAdvantage)
            {
                case EnumAdvantageState.Advantage:    hit += ADVANTAGE_HIT; break;
                case EnumAdvantageState.Disadvantage: hit -= ADVANTAGE_HIT; break;
            }

            // 100분율 
            return Math.Max(0, buff_value.Calculate(hit - dodge));
        }

        int Calculate_CriticalRate(IBattleSystemParam _param)
        {
            // 필살 발생 확률 = 필살 - 필살 회피
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);



            // 스탯  & 버프 계산.
            var hit        = dealer.StatusManager.Calc_Critical();
            var dodge      = target.StatusManager.Calc_DodgeCritical();
            var buff_value = dealer.StatusManager.Buff.Collect(this, dealer, EnumBuffStatus.System_Critical) + target.StatusManager.Buff.Collect(this, target, EnumBuffStatus.System_Critical);



            // 100분율 
            return Math.Max(0, buff_value.Calculate(hit - dodge));
        }

        int Calculate_Damage(IBattleSystemParam _param)
        {
            // 피해량 = (공격[마공] - 수비[마방])
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 스탯 계산.
            var might_physic   = dealer.StatusManager.Calc_Might_Physic();
            var might_magic    = dealer.StatusManager.Calc_Might_Magic();
            var defense_physic = target.StatusManager.Calc_Defense();
            var defense_magic  = target.StatusManager.Calc_Resistance();

            var damage_physic  = might_physic - defense_physic;
            var damage_magic   = might_magic  - defense_magic;
            var damage_total   = damage_physic + damage_magic;

            // 버프 계산.
            var buff_value     = dealer.StatusManager.Buff.Collect(this, dealer, EnumBuffStatus.System_Damage) + target.StatusManager.Buff.Collect(this, target, EnumBuffStatus.System_Damage);
            damage_total       = Math.Max(0, buff_value.Calculate(damage_total));

            return damage_total;
        }

        BattleObject GetDealer(IBattleSystemParam _param)
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

        BattleObject GetTarget(IBattleSystemParam _param)
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

        EnumAdvantageState Calculate_WeaponAdvantage(IBattleSystemParam _param)
        {
            //
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 무기 상성 체크.
            var weapon_advantage = Weapon.Calculate_Advantage(dealer.StatusManager.Weapon, target.StatusManager.Weapon);

            // TODO: 버프 체크

            return weapon_advantage;
        }

        bool Calculate_WeaponEffectiveness(IBattleSystemParam _param)
        {
            //
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 무기 특효 체크
            var weapon_effectiveness = Weapon.Calculate_Effectiveness(dealer.StatusManager.Weapon, target.StatusManager.Status);

            // TODO: 버프 체크

            return weapon_effectiveness;
        }

       

    }

}
