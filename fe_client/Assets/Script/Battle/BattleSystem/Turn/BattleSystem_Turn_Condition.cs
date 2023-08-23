using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {

        public class Condition_IsOwnerTurn : ICondition
        {
            public bool IsValid(ISystem _system, IOwner _owner)
            {
                var turn_system = _system.SystemManager.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
                if (turn_system == null)
                    return false;

                var turn_data = turn_system.GetTurnDataByID(_owner.ID);
                if (turn_data == null)
                    return false;

                switch (turn_system.TurnPhase)
                {
                    case EnumTurnPhase.Attacker: return turn_data.IsAttacker;
                    case EnumTurnPhase.Defender: return turn_data.IsDefender;
                }

                return false;
            }
        }


        
    }
}
