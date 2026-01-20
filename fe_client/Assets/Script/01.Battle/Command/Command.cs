using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// �̵� ����.
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

        public abstract EnumCommandType CommandType { get; }
        public virtual  bool            IsAbortable => false;



        protected abstract void OnEnter();
        protected abstract bool OnUpdate();
        protected abstract void OnExit(bool _is_abort);

        public void Abort() { if (State == EnumState.Progress) OnExit(true); }

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

            // 카메라 이동 처리.
            // Update_CameraPositionEvent();

            return State;   
        }


        // protected void Update_CameraPositionEvent()
        // {
        //     var entity = EntityManager.Instance.GetEntity(OwnerID);
        //     if (entity == null || entity.IsFixedObject)
        //         return;

        //     var is_player_faction =BattleSystemManager.Instance.GetFactionCommanderType(entity.GetFaction()) == EnumCommanderType.Player;
        //     if (is_player_faction)
        //         return;            

        //     // AI 턴일 경우 해당 유닛의 위치로 카메라를 이동.
        //     EventDispatchManager.Instance.UpdateEvent(
        //         ObjectPool<Battle_Camera_PositionEvent>.Acquire()
        //         .SetCell(entity.Cell)
        //     ); 
        // }
    }

    


}

