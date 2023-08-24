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

    public abstract class CombatSystem : ISystem
    {
        public EnumSystem     SystemType    { get; private set; } = EnumSystem.None;
        public EnumState      State         { get; private set; } = EnumState.None;
        public ISystemManager SystemManager { get; private set; } = CombatSystemManager.Instance;

        //public bool IsInit    => State == EnumState.Init;
        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        protected CombatSystem(EnumSystem _system_type)
        {
            SystemType = _system_type;
        }

        public    abstract void Reset();

        protected abstract void OnEnter(ICombatSystemParam _param);
        protected abstract bool OnUpdate(ICombatSystemParam _param);
        protected abstract void OnExit(ICombatSystemParam _param);

        public EnumState Update(ICombatSystemParam _param)
        {
            if (State != EnumState.Progress)
            {
                //State = EnumState.Init;
                State = EnumState.Progress;

                OnEnter(_param);

            }

            if (OnUpdate(_param))
            {
                State = EnumState.Finished;
            }

            if (State != EnumState.Progress)
            {
                OnExit(_param);
            }

            return State;
        }
    }


}



