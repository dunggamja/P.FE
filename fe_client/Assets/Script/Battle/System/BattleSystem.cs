using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumState
    {
        None,
        //Init,
        Progress,
        Finished,
    }

    public interface IBattleSystemParam
    {
        BattleObject Attacker { get; set; }
        BattleObject Defender { get; set; }
    }

    public abstract class BattleSystem : ISystem
    {
        public EnumSystem     SystemType    { get; private set; } = EnumSystem.None;
        public EnumState      State         { get; private set; } = EnumState.None;
        public ISystemManager SystemManager { get; private set; } = BattleSystemManager.Instance;

        //public bool IsInit    => State == EnumState.Init;
        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        protected BattleSystem(EnumSystem _system_type)
        {
            SystemType = _system_type;
        }

        public abstract void Reset();

        protected abstract void OnEnter(IBattleSystemParam _param);
        protected abstract bool OnUpdate(IBattleSystemParam _param);
        protected abstract void OnExit(IBattleSystemParam _param);

        public void Update(IBattleSystemParam _param)
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
        }
    }


}



