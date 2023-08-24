using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface ICombatSystemParam
    {
        BattleObject Attacker { get; }
        BattleObject Defender { get; }
        bool         IsPlan   { get; }
    }

    public abstract class CombatSystem : System<ICombatSystemParam>
    {
        public override ISystemManager SystemManager => CombatSystemManager.Instance;

        //public bool IsInit    => State == EnumState.Init;

        protected CombatSystem(EnumSystem _system_type) : base(_system_type)
        { }

        
    }


}



