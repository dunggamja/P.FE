using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public interface ICombatSystemParam : ISystemParam
    {
        // BHManager BHManager { get; }
        Entity    Attacker  { get; }
        Entity    Defender  { get; }
        bool      IsPlan    { get; }



        // List<IBHEffect> Results { get; }
    }

    public class CombatParam_Plan : ICombatSystemParam, IPoolObject
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity    Attacker  { get; private set; }
        public Entity    Defender  { get; private set; }
        public bool      IsPlan => true;


        public CombatParam_Plan Set(Entity _attacker, Entity _defender)
        {
            Attacker = _attacker;
            Defender = _defender;
            return this;
        }

        public void Reset()
        {
            // BHManager.Clear();
            // Results.Clear();
            Attacker = null;
            Defender = null;
        }

    }

    public class CombatParam : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity          Attacker  { get; private set; }
        public Entity          Defender  { get; private set; }
        public bool            IsPlan    => false;



        public static CombatParam Cache = new ();


        public CombatParam Set(Entity _attacker, Entity _defender)
        {
            Attacker = _attacker;
            Defender = _defender;
            return this;
        }

        public CombatParam Reset()
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



