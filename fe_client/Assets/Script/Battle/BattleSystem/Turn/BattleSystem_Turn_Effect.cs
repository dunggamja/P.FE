using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {
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
            public int Value { get; private set; }

            public void Apply(ISystem _system, IOwner _owner)
            {
                var turn_system = _system.SystemManager.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
                if (turn_system == null || !turn_system.IsProgress)
                    return;

                var owner_is_attacker = BattleSystemManager.Instance.IsAttacker(_owner.ID);
                var battle_side = (owner_is_attacker) ? EnumTurnPhase.Attacker : EnumTurnPhase.Defender;

                switch (Effect)
                {
                    case EnumEffectType.TurnSequence: turn_system.AddTurnSequence(battle_side, Value); break;
                    case EnumEffectType.ExtraAttackCount: turn_system.AddExtraAttackCount(battle_side, Value); break;
                }
            }
        }
    }
}