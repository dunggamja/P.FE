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
        public Int64     ID      { get; private set; }
        public Int64     OwnerID { get; private set; }
        public EnumState State   { get; private set; }

        protected Command(Int64 _owner_id)
        {
            ID      = Util.GenerateID();
            OwnerID = _owner_id;    
        }

        public Entity Owner => EntityManager.Instance.GetEntity(OwnerID);


        protected abstract void OnEnter();
        protected abstract bool OnUpdate();
        protected abstract void OnExit(bool _is_abort);

        public void Abort()
        {
            OnExit(true);
        }

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
                OnExit(false);
                //Debug.Log($"Command, Finished, ID:{OwnerID}, Command:{GetType().Name}");
            }

            return State;   
        }
    }

    


}

