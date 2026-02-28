using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public interface ICombatSystemParam : ISystemParam
    {
        // BHManager BHManager { get; }
        Entity                Attacker     { get; }
        Entity                Defender     { get; }        
        EnumUnitCommandType   CommandType  { get; }
        bool                  IsPlan       { get; }
        float                 DeltaTime    { get; }
    }

    public class CombatParam_Plan : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity                Attacker      { get; private set; } = null;
        public Entity                Defender      { get; private set; } = null;
        public EnumUnitCommandType   CommandType   { get; private set; } = EnumUnitCommandType.None;
        public bool                  IsPlan    => true;
        public float                 DeltaTime => CombatSystemManager.Instance.DeltaTime;

        public CombatParam_Plan Set(Entity _attacker, Entity _defender, EnumUnitCommandType _command_type)
        {
            Attacker    = _attacker;
            Defender    = _defender;
            CommandType = _command_type;
            return this;
        }

        public void Reset()
        {
            // BHManager.Clear();
            // Results.Clear();
            Attacker    = null;
            Defender    = null;
            CommandType = EnumUnitCommandType.None;
            // IsWand   = false;
        }

    }

    public class CombatParam : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity                Attacker    { get; private set; }
        public Entity                Defender    { get; private set; }
        public EnumUnitCommandType   CommandType { get; private set; } = EnumUnitCommandType.None;
        public bool                  IsPlan    => false;
        public float                 DeltaTime => CombatSystemManager.Instance.DeltaTime;



        public CombatParam Set(Entity _attacker, Entity _defender, EnumUnitCommandType _command_type)
        {
            Attacker    = _attacker;
            Defender    = _defender;
            CommandType = _command_type;
            return this;
        }

        public void Reset()
        {
            Attacker    = null;
            Defender    = null;
            CommandType = EnumUnitCommandType.None;
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



