using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class BattleSystem_BattleTurn : BattleSystem
    {
     

        // 공방 순서 관련된 스킬과 관련된 조건/효과 들도 여기에 작성해보자.

        public class Condition_BattleTurnSystem : ICondition
        {
            public enum EnumConditionType
            {
                None,
                IsAttacker,
                IsDefender,
            }


            public bool IsValid(BattleObject _owner)
            {
                var battle_turn_system = BattleSystemManager.Instance.BattleTurnSystem;
                if (battle_turn_system != null)
                {
                    var data = battle_turn_system.GetDataByID(_owner.ID);
                    if (data != null)
                    {
                        data.
                    }
                }

                return false;
            }
        }


        public class Effect_BattleTurnSystem : IEffect
        {
            public enum EnumEffectType
            {
                None,
                TurnSequence,       // 행동 순서 변경
                ExtraAttackCount,   // 현재 행동에 추가 공격 발동 
            }

            public Int64          TargetID    { get; private set; }
            public EnumEffectType EffectType  { get; private set; }
            public int            EffectValue { get; private set; }


            public void Apply(BattleObject _owner)
            {
                var battle_turn_system = BattleSystemManager.Instance.BattleTurnSystem;
                if (battle_turn_system != null)
                {
                    battle_turn_system.Process_Effect(this);
                }
            }
        }


        public void Process_Effect(Effect_BattleTurnSystem _effect)
        {
            if (_effect == null)
                return;

            var data = GetDataByID(_effect.TargetID);
            if (data == null)
                return;

            switch((int)_effect.EffectType)
            {
                case (int)Effect_BattleTurnSystem.EnumEffectType.TurnSequence:
                    {
                        // 행동 순서 변경
                        data.AddTurnSequence(_effect.EffectValue);
                    }
                    break;

                case (int)Effect_BattleTurnSystem.EnumEffectType.ExtraAttackCount:
                    {
                        // 추가 공격 적용.
                        data.AddExtraAttackCount(_effect.EffectValue);
                    }
                    break;
            }
            
        }

    }
}
