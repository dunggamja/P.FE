using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class CombatSystem_Damage : CombatSystem
    {
        const int ADVANTAGE_HIT               = 20;
        const int CRITICAL_DAMAGE_MULTIPLIER  = 3;
        const int EFFECTIVE_DAMAGE_MULTIPLIER = 2;

        public enum EnumSkillTiming
        {
            None,

            Attack_Start,
            Attack_Complete,

            Modify_Advantage,
            Modify_Hit,
            Modify_Critical,
            Modify_Damage,
            Modify_ApplyDamage,
        }


        public EnumAdvantageState WeaponAdvantage     { get; private set; }
        public bool               WeaponEffectiveness { get; private set; }

        public bool Result_Hit          { get; private set; }
        public bool Result_Critical     { get; private set; }

        public int  Result_HitRate      { get; private set; }
        public int  Result_CriticalRate { get; private set; }
        public int  Result_Damage       { get; private set; }


        public CombatSystem_Damage() : base(EnumSystem.CombatSystem_Damage)
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

        protected override void OnEnter(ICombatSystemParam _param)
        {
            Reset();

            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 공격 전 스킬 사용.
            //SkillTiming = 
            EventManager.Instance.DispatchEvent(new SkillUseEvent(this));

            // TODO: Attack_Start
        }

        protected override bool OnUpdate(ICombatSystemParam _param)
        {
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 무기 상성/특효에 대한 값 셋팅.
            WeaponAdvantage     = Calculate_WeaponAdvantage(_param);
            WeaponEffectiveness = Calculate_WeaponEffectiveness(_param);

            // 명중 / 필살 / 데미지 계산.
            Result_HitRate       = Calculate_HitRate(_param);
            Result_Hit           = Util.Random100_Result(Result_HitRate);

            Result_CriticalRate  = Calculate_CriticalRate(_param);
            Result_Critical      = Util.Random100_Result(Result_CriticalRate);

            Result_Damage        = (Result_Hit) ? Calculate_Damage(_param) : 0;

            // 크리티컬 & 특효 적용
            Result_Damage       *= (WeaponEffectiveness) ? EFFECTIVE_DAMAGE_MULTIPLIER : 1;
            Result_Damage       *= (Result_Critical)     ? CRITICAL_DAMAGE_MULTIPLIER  : 1;

            // 데미지 적용 여기서 한다.
            target.ApplyDamage(Result_Damage);

            return true;
        }

        protected override void OnExit(ICombatSystemParam _param)
        {
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 공격 후 스킬 사용.
            EventManager.Instance.DispatchEvent(new SkillUseEvent(this));
        }



        int Calculate_HitRate(ICombatSystemParam _param)
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

        int Calculate_CriticalRate(ICombatSystemParam _param)
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

        int Calculate_Damage(ICombatSystemParam _param)
        {
            // 피해량 = (공격[마공] - 수비[마방])
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 스탯 계산.
            var might_physic   = dealer.StatusManager.Calc_Might_Physic();
            var might_magic    = dealer.StatusManager.Calc_Might_Magic();
            var defense_physic = target.StatusManager.Calc_Defense();
            var defense_magic  = target.StatusManager.Calc_Resistance();

            var damage_physic = (0 < might_physic) ? Math.Max(0, might_physic - defense_physic) : 0;
            var damage_magic  = (0 < might_magic)  ? Math.Max(0, might_magic  - defense_magic) : 0;
            var damage_total  = damage_physic + damage_magic;

            // 버프 계산.
            var buff_value     = dealer.StatusManager.Buff.Collect(this, dealer, EnumBuffStatus.System_Damage) + target.StatusManager.Buff.Collect(this, target, EnumBuffStatus.System_Damage);
            damage_total       = Math.Max(0, buff_value.Calculate(damage_total));

            return damage_total;
        }

        BattleObject GetDealer(ICombatSystemParam _param)
        {
            //var turn_system = SystemManager.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
            var turn_system = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
            if (turn_system == null)
                return null;

            switch(turn_system.CombatTurn)
            {
                case CombatSystem_Turn.EnumCombatTurn.Attacker: return _param.Attacker;
                case CombatSystem_Turn.EnumCombatTurn.Defender: return _param.Defender;
            }

            return null;
        }

        BattleObject GetTarget(ICombatSystemParam _param)
        {
            //var turn_system = SystemManager.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
            var turn_system = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
            if (turn_system == null)
                return null;

            switch (turn_system.CombatTurn)
            {
                case CombatSystem_Turn.EnumCombatTurn.Attacker: return _param.Defender;
                case CombatSystem_Turn.EnumCombatTurn.Defender: return _param.Attacker;
            }

            return null;
        }

        EnumAdvantageState Calculate_WeaponAdvantage(ICombatSystemParam _param)
        {
            //
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            // 무기 상성 체크.
            var weapon_advantage = Weapon.Calculate_Advantage(dealer.StatusManager.Weapon, target.StatusManager.Weapon);

            // TODO: 버프 체크

            return weapon_advantage;
        }

        bool Calculate_WeaponEffectiveness(ICombatSystemParam _param)
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
