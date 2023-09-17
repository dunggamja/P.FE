using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 명령 처리에 대한 로직을 작성해봅시다.
    /// </summary>
    public abstract class Command
    {
        public Int64     UnitID { get; private set; }
        public EnumState State  { get; private set; }


        protected abstract void OnEnter();
        protected abstract bool OnUpdate();
        protected abstract void OnExit();

        public EnumState Update()
        {
            if (State != EnumState.Progress)
            {
                State = EnumState.Progress;
                OnEnter();
            }

            if (OnUpdate())
            {
                State = EnumState.Finished;
            }

            if (State != EnumState.Progress)
            {
                OnExit();
            }

            return State;   
        }
    }

    


}

