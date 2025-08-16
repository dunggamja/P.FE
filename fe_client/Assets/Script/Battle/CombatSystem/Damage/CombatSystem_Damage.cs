using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class CombatSystem_Damage : CombatSystem
    {
        const int   ADVANTAGE_HIT               = 20;
        const float CRITICAL_DAMAGE_MULTIPLIER  = 2f;
        const float EFFECTIVE_DAMAGE_MULTIPLIER = 1.5f;

        public enum EnumDamageType
        {

        }


        // // 요건 사용 안 할 거 같은...?
        // public enum EnumSkillTiming
        // {
        //     None,

        //     Attack_Start,
        //     Attack_Complete,

        //     Modify_Advantage,
        //     Modify_Hit,
        //     Modify_Critical,
        //     Modify_Damage,
        //     Modify_ApplyDamage,
        // }


        // public EnumAdvantageState WeaponAdvantage     { get; private set; }
        
        public bool   WeaponEffectiveness { get; private set; }

        public bool   Result_Hit          { get; private set; }
        public bool   Result_Critical     { get; private set; }
        public bool   Result_Guard        { get; private set; }

        public float  Result_HitRate      { get; private set; }
        public float  Result_CriticalRate { get; private set; }
        public float  Result_GuardRate    { get; private set; }
        public int    Result_Damage       { get; private set; }        


        public CombatSystem_Damage() : base(EnumSystem.CombatSystem_Damage)
        { }

        public override void Init()
        {
            
        }


        public override void Release()
        {
            // WeaponAdvantage     = EnumAdvantageState.None;
            WeaponEffectiveness = false;


            Result_Hit          = false;
            Result_Critical     = false;
            Result_Guard        = false;
            
            Result_HitRate      = 0f;
            Result_CriticalRate = 0f;
            Result_GuardRate    = 0f;
            Result_Damage       = 0;
        }

        protected override void OnEnter(ICombatSystemParam _param)
        {
            Release();

            //var dealer = GetDealer(_param);
            //var target = GetTarget(_param);

            // 공격 전 스킬 사용.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Situation_UpdateEvent>
                .Acquire()
                .Set(EnumSituationType.CombatSystem_Damage_Start, _param.IsPlan)
                );
        }

        protected override bool OnUpdate(ICombatSystemParam _param)
        {
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);

            
            // 무기 상성에 따른 명중률 보정은 일단 제거.
            // WeaponAdvantage     = Calculate_WeaponAdvantage(_param);
            // 무기 특효에 대한 값 셋팅.
            WeaponEffectiveness  = Calculate_WeaponEffectiveness(_param);

            // 명중 / 필살 / 데미지 계산.
            Result_HitRate       = Calculate_HitRate(_param);
            Result_CriticalRate  = Calculate_CriticalRate(_param);
            Result_GuardRate     = Calculate_GuardRate(_param);

            if (_param.IsPlan)
            {
                // 계획시는 랜덤 관련 처리를 하지 않는다.
                Result_Hit           = true;                
                Result_Critical      = false;
                Result_Damage        = Calculate_Damage(_param);
                Result_Guard         = false;
            }
            else
            {
                
                Result_Hit           = Util.Random01_Result(Result_HitRate);                
                Result_Critical      = Util.Random01_Result(Result_CriticalRate);
                Result_Guard         = (Result_Hit) ? Util.Random01_Result(Result_GuardRate)    : false;
                Result_Damage        = (Result_Hit && !Result_Guard) ? Calculate_Damage(_param) : 0;
            }

            // 크리티컬 & 특효 적용
            if (WeaponEffectiveness)
                Result_Damage = (int)(Result_Damage * EFFECTIVE_DAMAGE_MULTIPLIER);

            if (Result_Critical)
                Result_Damage = (int)(Result_Damage * CRITICAL_DAMAGE_MULTIPLIER);

            // TODO: HEAL.


            //, _param.IsPlan);

            if (Result_Hit)
            {
                // 데미지 적용.
                target.ApplyDamage(Result_Damage);
            }

            // // 로그 적용.
            // _param.PushLog(new CombatLog(
            //     CombatLog.EnumLogType.Damage,
            //         dealer.ID,
            //         target.ID,
            //         Result_Damage));

            // 1 Tick에 완료 처리.
            return true;
        }

        protected override void OnExit(ICombatSystemParam _param)
        {
            // var dealer = GetDealer(_param);
            // var target = GetTarget(_param);

            // 공격 후 스킬 사용.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Situation_UpdateEvent>
                .Acquire()
                .Set(EnumSituationType.CombatSystem_Damage_Finish, _param.IsPlan)
                );
        }



        float Calculate_HitRate(ICombatSystemParam _param)
        {
            // 명중률 = 명중 - 회피
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);
 
            // 스탯  & 버프 계산.
            var hit        = dealer.StatusManager.Calc_Hit();
            var dodge      = target.StatusManager.Calc_Dodge();

            var buff_hit   = dealer.StatusManager
                .Buff
                .Collect_Combat(
                EnumSituationType.CombatSystem_Damage_Start,
                dealer, target,
                EnumBuffStatus.System_Hit);

            // // 무기 상성 적용.
            // switch(WeaponAdvantage)
            // {
            //     case EnumAdvantageState.Advantage:    hit += ADVANTAGE_HIT; break;
            //     case EnumAdvantageState.Disadvantage: hit -= ADVANTAGE_HIT; break;
            // }

            // 100분율 
            return Math.Max(0, buff_hit.Calculate(hit - dodge)) * 0.01f;
        }

        float Calculate_CriticalRate(ICombatSystemParam _param)
        {
            // 필살 발생 확률 = 필살 - 필살 회피
            var dealer = GetDealer(_param);
            var target = GetTarget(_param);


            // 스탯  & 버프 계산.
            var hit      = dealer.StatusManager.Calc_Critical();
            var dodge    = target.StatusManager.Calc_DodgeCritical();
            var hit_buff = dealer.StatusManager
            .Buff
            .Collect_Combat(
                EnumSituationType.CombatSystem_Damage_Start,
                dealer, target,
                EnumBuffStatus.System_Critical);


            // TODO: 피격자의 버프 계산도 필요.




            // 100분율 
            return Math.Max(0, hit_buff.Calculate(hit - dodge)) * 0.01f;
        }

        float Calculate_GuardRate(ICombatSystemParam _param)
        {
            // // 방어 확률 = 방어 - 방어 회피
            // var dealer = GetDealer(_param);
            // var target = GetTarget(_param);


            // 방어 확률은 아직 어떻게 처리할지 미정.
            return 0f;            
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
            var buff_value     = dealer.StatusManager
            .Buff
            .Collect_Combat(
                EnumSituationType.CombatSystem_Damage_Start,
                dealer, target,
                EnumBuffStatus.System_Damage);
               
            damage_total       = Math.Max(0, buff_value.Calculate(damage_total));

            return damage_total;
        }

        Entity GetDealer(ICombatSystemParam _param)
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

        Entity GetTarget(ICombatSystemParam _param)
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

        // EnumAdvantageState Calculate_WeaponAdvantage(ICombatSystemParam _param)
        // {
        //     //
        //     var dealer = GetDealer(_param);
        //     var target = GetTarget(_param);

        //     // 무기 상성 체크.
        //     var weapon_advantage = Weapon.Calculate_Advantage(dealer.StatusManager.Weapon, target.StatusManager.Weapon);

        //     // TODO: 버프 체크

        //     return weapon_advantage;
        // }

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
