using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumCommandType
    {
        None,
        Attack,   // 공격
        Move,     // 이동
        Wand,     // 지팡이
        Exchange, // 아이템 교환
        Item,     // 아이템 사용
        Done,     // 대기
        Abort,    // 취소 (시스템적인 명령어임...)
    }


    public class CommandManager //: Singleton<CommandManager>
    {

        enum EnumCommandAbort
        {
            None,
            PendingOnly,    // 대기 중인 명령 취소.
            RunningInclude, // 진행 중인 명령까지 취소.
            Force,          // 강제로 모두 취소.
        }
        
        Int64            m_owner_id      = 0;
        Queue<Command>   m_command_queue = new (10);
        EnumCommandAbort m_command_abort = EnumCommandAbort.None;

        Entity Owner => EntityManager.Instance.GetEntity(m_owner_id);



        // public bool    IsAbort { get; set; }


        // protected override void Init()
        // {
        //     base.Init();
        // }

        public void Initialize(IOwner _owner)
        {
            m_owner_id = _owner.ID;
            m_command_queue.Clear();

            m_command_abort = EnumCommandAbort.None;
            // IsAbort    = false;
        }

        public void Clear()
        {
            m_command_queue.Clear();

            m_command_abort = EnumCommandAbort.None;
            // IsAbort = false;
        }

        

        public bool PushCommand(Command _command)
        {
            m_command_queue.Enqueue(_command);

            switch (_command)
            {
                // 명령 취소 명령어 처리.
                case Command_Abort command_abort: 
                Update_CommandAbortState(command_abort); 
                break;
            }

            return true;
        }

        void Update_CommandAbortState(Command_Abort _command)
        {
            if (_command == null)
                return;

            var abort_type = (_command.IsPendingOnly) 
                                ?  (int)EnumCommandAbort.PendingOnly 
                                :  (int)EnumCommandAbort.RunningInclude;

            // 명령 취소 명령어 처리.
            m_command_abort = (EnumCommandAbort)Math.Max((int)m_command_abort, abort_type);                                

        }

 
        public Command PopCommand()
        {
            if (m_command_queue.Count == 0)
                return null;
                
            return m_command_queue.Dequeue();
        }

        public Command PeekCommand()
        {
            if (m_command_queue.Count == 0)
                return null;

            return m_command_queue.Peek();
        }


        public void Update()
        {
            // 명령 취소 처리.
            if (Verify_Abort_Command(m_command_abort))
            {
                Process_Abort();
                m_command_abort = EnumCommandAbort.None;                
            }



            // 명령 진행 중인 명령어 처리.
            var command  = PeekCommand();
            if (command == null)
                return;


            // 명령 진행.
            if (command.Update() == EnumState.Finished)
            {
                // command 완료되면 pop 처리.
                PopCommand();
            }

            // 이벤트 발생.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Command_Event>.Acquire().Set(
                    m_owner_id,
                    command.CommandType,
                    command.State
                ));
        }


        bool Verify_Abort_Command(EnumCommandAbort _abort_state)
        {
            var running_command       = PeekCommand();
            var is_running_command    = running_command != null && running_command.State == EnumState.Progress;
            var is_not_abortable      = running_command             != null 
                                     && running_command.IsAbortable == false
                                     && running_command.State       == EnumState.Progress;


            switch (_abort_state)
            {
                // 명령 취소 상태가 아님.
                case EnumCommandAbort.None:
                    return false;

                // 실행중인 명령이 있으면 취소 불가.
                case EnumCommandAbort.PendingOnly:
                {
                    return (is_running_command == false);
                }

                // 실행중인 명령도 취소. (취소 불가능한 명령은 제외)
                case EnumCommandAbort.RunningInclude:
                {
                    return (is_not_abortable == false);
                }

                // 강제로 모두 취소.
                case EnumCommandAbort.Force:
                {
                    return true;
                }
            }

            return false;   
        }

        void Process_Abort()
        {
            // 명령 취소 명령어 처리. 

            using var command_list = ListPool<Command>.AcquireWrapper();

            // 일단 모든 명령을 리스트에 담는다.
            while (m_command_queue.Count > 0)
                command_list.Value.Add(PopCommand());

            // 명령 취소 이벤트 전까지의 명령들을 취소처리합시다.
            var last_abort_index = command_list.Value.FindLastIndex(command => command is Command_Abort);
            for (int i = 0; i <= last_abort_index; ++i)
                command_list.Value[i].Abort();


            // 명령 취소 이벤트 이후의 명령들을 다시 큐에 추가합시다.
            for (int i = Math.Max(0, last_abort_index + 1); i < command_list.Value.Count; ++i)
                PushCommand(command_list.Value[i]);
        }



    }
}

