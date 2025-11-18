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
        bool      UseWand   { get; }
        bool      IsPlan    { get; }



        // List<IBHEffect> Results { get; }
    }

    public class CombatParam_Plan : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity    Attacker       { get; private set; } = null;
        public Entity    Defender       { get; private set; } = null;
        public bool      UseWand        { get; private set; } = false;
        public bool      IsPlan => true;


        public CombatParam_Plan Set(Entity _attacker, Entity _defender, bool _use_wand)
        {
            Attacker = _attacker;
            Defender = _defender;
            UseWand  = _use_wand;
            return this;
        }

        public void Reset()
        {
            // BHManager.Clear();
            // Results.Clear();
            Attacker  = null;
            Defender  = null;
            UseWand   = false;
            // IsWand   = false;
        }

    }

    public class CombatParam : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity          Attacker  { get; private set; }
        public Entity          Defender  { get; private set; }
        public bool            UseWand   { get; private set; }
        public bool            IsPlan    => false;


        public CombatParam Set(Entity _attacker, Entity _defender, bool _use_wand)
        {
            Attacker = _attacker;
            Defender = _defender;
            UseWand  = _use_wand;
            return this;
        }

        public void Reset()
        {
            Attacker = null;
            Defender = null;
            UseWand  = false;
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



