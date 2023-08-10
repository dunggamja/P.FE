using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {
        public class Condition_State : ICondition
        {
            BattleSystem.EnumState State { get; set; }

            public bool IsValid(BattleObject _owner)
            {
                var turn_system = BattleSystemManager.Instance.TurnSystem;
                if (turn_system == null)
                    return false;

                if (turn_system.State != State)
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
                var turn_system = BattleSystemManager.Instance.TurnSystem;
                if (turn_system == null || turn_system.IsEngaged(_owner.ID))
                    return false;

                if (IsOnlyAttacker && !turn_system.IsAttacker(_owner.ID))
                    return false;

                if (IsOnlyDefender && !turn_system.IsDefender(_owner.ID))
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
                var turn_system = BattleSystemManager.Instance.TurnSystem;
                if (turn_system == null || !turn_system.IsEngaged(_owner.ID))
                    return;

                var owner_is_attacker = turn_system.IsAttacker(_owner.ID);
                var battle_side       = (owner_is_attacker) ? EnumTurnSide.Attacker : EnumTurnSide.Defender; ;

                switch(Effect)
                {
                    case EnumEffectType.TurnSequence:     turn_system.AddTurnSequence(battle_side, Value);     break;
                    case EnumEffectType.ExtraAttackCount: turn_system.AddExtraAttackCount(battle_side, Value); break;
                }
            }
        }
    }
}
