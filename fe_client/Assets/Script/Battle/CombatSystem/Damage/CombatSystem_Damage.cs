using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

        public struct Combat_DamageResult
        {
            public Int64  AttackerID         { get; private set; }
            public Int64  TargetID           { get; private set; }
            public Int64  WeaponID           { get; private set; }

            public int    Result_HP_Attacker { get; private set; }
            public int    Result_HP_Target   { get; private set; }
            


            public bool   WeaponEffectiveness { get; private set; }
            public bool   Result_Hit          { get; private set; }
            public bool   Result_Critical     { get; private set; }
            public bool   Result_Guard        { get; private set; }

            public float  Result_HitRate      { get; private set; }
            public float  Result_CriticalRate { get; private set; }
            public float  Result_GuardRate    { get; private set; }
            public int    Result_Damage       { get; private set; }   

            public int    Result_HitRate_Percent      => Math.Clamp((int)(Result_HitRate * 100), 0, 100);
            public int    Result_CriticalRate_Percent => Math.Clamp((int)(Result_CriticalRate * 100), 0, 100);
            public int    Result_GuardRate_Percent    => Math.Clamp((int)(Result_GuardRate * 100), 0, 100);

            public static Combat_DamageResult Create(
                Int64 _attacker_id,
                Int64 _target_id,
                Int64 _weapon_id,
                int   _result_hp_attacker,
                int   _result_hp_target,
                bool  _weapon_effectiveness, 
                bool  _result_hit,
                bool  _result_critical, 
                bool  _result_guard, 
                float _result_hit_rate, 
                float _result_critical_rate,
                float _result_guard_rate, 
                int   _result_damage)
            {
                return new Combat_DamageResult
                {
                    AttackerID          = _attacker_id,
                    TargetID            = _target_id,
                    WeaponID            = _weapon_id,
                    Result_HP_Attacker  = _result_hp_attacker,
                    Result_HP_Target    = _result_hp_target,
                    WeaponEffectiveness = _weapon_effectiveness,
                    Result_Hit          = _result_hit,
                    Result_Critical     = _result_critical,
                    Result_Guard        = _result_guard,
                    Result_HitRate      = _result_hit_rate,
                    Result_CriticalRate = _result_critical_rate,
                    Result_GuardRate    = _result_guard_rate,
                    Result_Damage       = _result_damage,
                };
            }
        }





    public partial class CombatSystem_Damage : CombatSystem
    {
        // const int   ADVANTAGE_HIT               = 20;
        const float CRITICAL_DAMAGE_MULTIPLIER  = 2f;
        const float EFFECTIVE_DAMAGE_MULTIPLIER = 1.5f;
       
        public CombatSystem_Damage() : base(EnumSystem.CombatSystem_Damage)
        { }

        protected override void OnInit()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEnter(ICombatSystemParam _param)
        {            
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
            if (dealer == null || target == null)
                return true;

            
            // 무기 상성에 따른 명중률 보정은 일단 제거.
            // WeaponAdvantage     = Calculate_WeaponAdvantage(_param);
            // 무기 특효에 대한 값 셋팅.
            bool WeaponEffectiveness = Calculate_WeaponEffectiveness(_param);

            bool  Result_Hit          = false;
            bool  Result_Critical     = false;
            bool  Result_Guard        = false;
            int   Result_Damage       = 0;

            // 명중 / 필살 / 데미지 계산.
            float Result_HitRate       = Calculate_HitRate(_param);
            float Result_CriticalRate  = Calculate_CriticalRate(_param);
            float Result_GuardRate     = Calculate_GuardRate(_param);
          

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
            // //, _param.IsPlan);

            if (Result_Hit)
            {
                // 데미지 적용.
                target.ApplyDamage(Result_Damage);
            }


            CombatSystemManager.Instance.AddCombatDamageResult(
                Combat_DamageResult.Create
                (
                    dealer.ID,
                    target.ID,
                    dealer.StatusManager.Weapon.ItemID,
                    dealer.StatusManager.Status.GetPoint(EnumUnitPoint.HP),
                    target.StatusManager.Status.GetPoint(EnumUnitPoint.HP),
                    WeaponEffectiveness,
                    Result_Hit,
                    Result_Critical,
                    Result_Guard,
                    Result_HitRate,
                    Result_CriticalRate,
                    Result_GuardRate,
                    Result_Damage
                )
            );
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
            var might_physic           = dealer.StatusManager.Calc_Might_Physic();
            var might_magic            = dealer.StatusManager.Calc_Might_Magic();
            var penetration_defense    = dealer.StatusManager.Calc_Penetration_Defense();
            var penetration_resistance = target.StatusManager.Calc_Penetration_Resistance();
            
            var defense_physic         = target.StatusManager.Calc_Defense();
            var defense_magic          = target.StatusManager.Calc_Resistance();

            // 방어/저항력 관통력 적용.
            if (0f < penetration_defense)
                defense_physic = (int)(defense_physic * Mathf.Clamp01(1f - penetration_defense));
            if (0f < penetration_resistance)
                defense_magic  = (int)(defense_magic * Mathf.Clamp01(1f - penetration_resistance));


            var damage_physic = (0 < might_physic) ? Math.Max(0, might_physic - defense_physic) : 0;
            var damage_magic  = (0 < might_magic)  ? Math.Max(0, might_magic  - defense_magic) : 0;
            var damage_total  = damage_physic + damage_magic;

            Debug.Log($"atid:{dealer.ID}, deid:{target.ID}, atk:{might_physic}, def:{defense_physic}");

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
