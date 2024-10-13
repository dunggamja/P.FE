using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 이동 명령.
    /// </summary>
    public abstract class Command
    {
        public Int64     UnitID { get; private set; }
        public EnumState State  { get; private set; }

        protected Command(Int64 _unit_id)
        {
            UnitID = _unit_id;
        }


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

