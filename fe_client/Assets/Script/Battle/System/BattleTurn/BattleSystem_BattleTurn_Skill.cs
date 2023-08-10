using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class BattleSystem_BattleTurn : BattleSystem
    {
        public class Condition_State : ICondition
        {
            BattleSystem.EnumState State { get; set; }

            public bool IsValid(BattleObject _owner)
            {
                var battle_turn_system = BattleSystemManager.Instance.BattleTurnSystem;
                if (battle_turn_system == null)
                    return false;

                if (battle_turn_system.State != State)
                    return false;

                return true;
            }
        }

        public class Condition_Engaged : ICondition
        {
            bool IsOnlyAttacker { get; set; }
            bool IsOnlyDefender { get; set; }

            public bool IsValid(BattleObject _owner)
            {
                var battle_turn_system = BattleSystemManager.Instance.BattleTurnSystem;
                if (battle_turn_system == null || battle_turn_system.IsEngaged(_owner.ID))
                    return false;

                if (IsOnlyAttacker && !battle_turn_system.IsAttacker(_owner.ID))
                    return false;

                if (IsOnlyDefender && !battle_turn_system.IsDefender(_owner.ID))
                    return false;

                return true;
            }
        }



        public class Effect_TurnControl : IEffect
        {
            public enum EnumEffectType
            {
                TurnSequence,     // 행동 순서
                ExtraAttackCount, // 추가 공격
            }

            //public enum EnumTargetType
            //{
            //    Owner,  // 나에게 적용
            //    Target  // 상대방에게 적용.
            //}

            public EnumEffectType Effect { get; private set; }
            public int            Value  { get; private set; }

            public void Apply(BattleObject _owner)
            {
                var battle_turn_system = BattleSystemManager.Instance.BattleTurnSystem;
                if (battle_turn_system == null || !battle_turn_system.IsEngaged(_owner.ID))
                    return;

                var owner_is_attacker = battle_turn_system.IsAttacker(_owner.ID);
                var battle_side       = (owner_is_attacker) ? EnumBattleTurnSide.Attacker : EnumBattleTurnSide.Defender; ;

                switch(Effect)
                {
                    case EnumEffectType.TurnSequence:     battle_turn_system.AddTurnSequence(battle_side, Value);     break;
                    case EnumEffectType.ExtraAttackCount: battle_turn_system.AddExtraAttackCount(battle_side, Value); break;
                }
            }
        }
    }
}
