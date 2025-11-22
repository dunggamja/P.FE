using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CombatSystem_Wand : CombatSystem
    {
        public CombatSystem_Wand() : base(EnumSystem.CombatSystem_Wand)
        {
        }

        protected override void OnEnter(ICombatSystemParam _param)
        {
            // 지팡이 시전 시작 이벤트 발생.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Situation_UpdateEvent>
                .Acquire()
                .Set(EnumSituationType.CombatSystem_Wand_Action_Start, _param.IsPlan)
                );
        }

        protected override bool OnUpdate(ICombatSystemParam _param)
        {
            var dealer = _param.Attacker;
            var target = _param.Defender;
            if (dealer == null || target == null)
                return true;


            bool  Result_Hit          = false;
            bool  Result_Guard        = false;
            int   Result_Heal         = 0;

            // 명중 / 필살 / 데미지 계산.
            float Result_HitRate       = Calculate_HitRate(_param);

            // TODO: 힐이 아닌 다른 부가효과들 적용 필요. (혼란, 이동불능, 등...)

            // EnumBattleBlackBoard


            // bool  Result_Critical     = false;
            // float Result_CriticalRate  = 0f;
            // float Result_GuardRate     = 0f;

            if (_param.IsPlan)
            {
                Result_Hit       = true;                
                Result_Heal      = Calculate_Heal(_param);
                Result_Guard     = false;
               //  Result_Critical  = false;
            }
            else
            {
                Result_Hit       = Util.Random01_Result(Result_HitRate);                
                Result_Guard     = false;
                Result_Heal      = (Result_Hit && !Result_Guard) ? Calculate_Heal(_param) : 0;
               //  Result_Critical  = false;
            }

            if (Result_Hit && Result_Heal > 0)
            {
               target.ApplyHeal(Result_Heal);
            }


            CombatSystemManager.Instance.AddCombatDamageResult(
                Combat_DamageResult.Create
                (
                    dealer.ID,
                    target.ID,
                    dealer.StatusManager.Weapon.ItemID,
                    dealer.StatusManager.Status.GetPoint(EnumUnitPoint.HP),
                    target.StatusManager.Status.GetPoint(EnumUnitPoint.HP),
                    false,
                    Result_Hit,
                    false,
                    Result_Guard,
                    Result_HitRate,
                    0f,
                    0f,
                    Result_Heal
                )
            );

            // 바로 완료처리.              


            return true;
        }

        protected override void OnExit(ICombatSystemParam _param)
        {
            // 지팡이 시전 후처리.
            PostProcess_Wand_Action(_param);


             // 지팡이 시전 완료 이벤트 발생.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Situation_UpdateEvent>
                .Acquire()
                .Set(EnumSituationType.CombatSystem_Wand_Action_Finish, _param.IsPlan)
                );

        }

        protected override void OnInit()
        {
            // throw new NotImplementedException();
        }

        protected override void OnRelease()
        {
            // throw new NotImplementedException();
        }


        float Calculate_HitRate(ICombatSystemParam _param)
        {
            return 1f;
        }

        int Calculate_Heal(ICombatSystemParam _param)
        {
            var dealer = _param.Attacker;
            if (dealer == null)
                return 0;

            var wand = dealer.StatusManager.Wand;
            if (wand == null)
                return 0;

            // 기본 회복량.
            var heal_value = wand.ItemObject.GetAttribute(EnumItemAttribute.Heal);
            
            // 회복 보너스 추가. (유닛 스탯 기반)
            using var list_heal_bonus = ListPool<(int target, int value)>.AcquireWrapper();
            Item.CollectAttribute(wand.ItemObject.Kind, EnumItemAttribute.HealBonus_UnitStatus, list_heal_bonus.Value);
            foreach (var (target, value) in list_heal_bonus.Value)
            {
               var status  = dealer.StatusManager.GetBuffedUnitStatus((EnumUnitStatus)target);
               heal_value += (int)(status * Util.PERCENT(value));
            }

            return heal_value;
        }

        void PostProcess_Wand_Action(ICombatSystemParam _param)
        {
            var dealer = _param.Attacker;
            if (dealer == null)
                return;

            // 무기 사용 후 수량 감소.
            var dealer_weapon = dealer.StatusManager.Wand.ItemObject;
            if (dealer_weapon != null)
            {
                dealer_weapon.DecreaseCount();

                // 무기 수량이 0이 되면 버리기.
                if (dealer_weapon.CurCount <= 0)
                    dealer.ProcessAction(dealer_weapon, EnumItemActionType.Dispose);
            }
        }

    }

}


