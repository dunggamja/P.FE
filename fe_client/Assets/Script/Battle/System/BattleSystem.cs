using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IBattleSystemParam { }

    public class BattleSystem 
    {
        public enum EnumState
        {
            None,
            Progress,
            Finished,
        }

        public EnumState State { get; private set; } = EnumState.None;

        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        protected virtual  void OnEnter(IBattleSystemParam _param)  { }
        protected virtual  bool OnUpdate(IBattleSystemParam _param) { return true; }
        protected virtual  void OnExit(IBattleSystemParam _param)   { }

        public void Update(IBattleSystemParam _param)
        {
            if (State != EnumState.Progress)
            {
                State = EnumState.None;
                OnEnter(_param);
                State = EnumState.Progress;
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



    public class BattleSystemManager : Singleton<BattleSystemManager>
    {
        public BattleSystem_BattleTurn BattleTurnSystem { get; }
        
    }
}



