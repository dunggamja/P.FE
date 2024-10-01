using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface ICombatSystemParam : ISystemParam
    {
        Entity Attacker { get; }
        Entity Defender { get; }
        bool   IsPlan   { get; }
    }

    public class TestCombat_Param : ICombatSystemParam
    {
        public Entity Attacker { get; private set; }
        public Entity Defender { get; private set; }
        public bool   IsPlan   => true;

        

        public static TestCombat_Param Cache = new TestCombat_Param();

        public TestCombat_Param Set(Entity _attacker, Entity _defender)
        {
            Attacker = _attacker;
            Defender = _defender;
            return this;
        }

        public TestCombat_Param Reset()
        {
            Attacker = null;
            Defender = null;
            return this;
        }
    }


    public abstract class CombatSystem : System<ICombatSystemParam>
    {
        //public override ISystemManager SystemManager => CombatSystemManager.Instance;

        //public bool IsInit    => State == EnumState.Init;

        protected CombatSystem(EnumSystem _system_type) : base(_system_type)
        { }

        
    }


}



