using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct CombatLog
    {
        public enum EnumLogType
        {
            None,
            Damage,
            Heal,
        }

        public EnumLogType LogType  { get; private set; }
        public Int64       ActorID  { get; private set; }
        public Int64       TargetID { get; private set; }
        public int         Value    { get; private set; }

        public CombatLog(EnumLogType _log_type, Int64 _actor_id, Int64 _target_id, int _value)
        {
            LogType  = _log_type;
            ActorID  = _actor_id;
            TargetID = _target_id;
            Value    = _value;
        }        
    }


    public interface ICombatSystemParam : ISystemParam
    {
        // BHManager BHManager { get; }
        Entity    Attacker  { get; }
        Entity    Defender  { get; }
        bool      IsPlan    { get; }

        void PushLog(CombatLog _log);

        // List<IBHEffect> Results { get; }
    }

    public class CombatParam_Plan : ICombatSystemParam, IPoolObject
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity    Attacker  { get; private set; }
        public Entity    Defender  { get; private set; }
        public bool      IsPlan => true;

        private List<CombatLog> m_logs = new();

        public void PushLog(CombatLog _log)
        {
            m_logs.Add(_log);
        }


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
            m_logs.Clear();
        }

    }

    public class CombatParam : ICombatSystemParam
    {
        // public BHManager BHManager { get; private set; } = new();
        public Entity          Attacker  { get; private set; }
        public Entity          Defender  { get; private set; }
        public bool            IsPlan    => false;

        public void PushLog(CombatLog _log)
        {
            // Results.Add(_log);
        }

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



