using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    public partial class CombatSystem_Turn : CombatSystem
    {

        public class Condition_IsOwnerTurn : ICondition
        {
            public bool IsValid(ISystem _system, IOwner _owner)
            {
                //var turn_system = _system.SystemManager.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
                var turn_system = CombatSystemManager.Instance.GetSystem(EnumSystem.CombatSystem_Turn) as CombatSystem_Turn;
                if (turn_system == null)
                    return false;

                var turn_data = turn_system.GetTurnDataByID(_owner.ID);
                if (turn_data == null)
                    return false;

                switch (turn_system.CombatTurn)
                {
                    case EnumCombatTurn.Attacker: return turn_data.IsAttacker;
                    case EnumCombatTurn.Defender: return turn_data.IsDefender;
                }

                return false;
            }
        }


        
    }
}
