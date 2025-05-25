using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CommandManager //: Singleton<CommandManager>
    {

        enum EnumCommandAbort
        {
            None,
            PendingOnly,    // 대기 중인 명령만 중단.
            IncludeRunning, // 실행중인 명령 포함.
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
                // 중단 명령이 왔을 경우에 대한 처리.
                case Command_Abort command_abort:
                {
                    var abort_type = (command_abort.IsPendingOnly) 
                                    ?  (int)EnumCommandAbort.PendingOnly 
                                    :  (int)EnumCommandAbort.IncludeRunning;

                    // 큰 값을 유지함.
                    m_command_abort = (EnumCommandAbort)Math.Max((int)m_command_abort, abort_type);                                
                    break;
                }
            }

            return true;
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
            // 명령 중단 처리.
            if (m_command_abort != EnumCommandAbort.None)   
            {
                // 명령 중단 처리에 성공하면 중단 처리 상태 초기화.
                if (AbortCommand(m_command_abort))
                    m_command_abort = EnumCommandAbort.None;                
            }

            // 현재 실행할 명령.
            var command  = PeekCommand();
            if (command == null)
                return;


            // 명령 실행.
            if (command.Update() == EnumState.Finished)
            {
                // command가 완료되면 pop 처리.
                PopCommand();
            }
        }


        bool AbortCommand(EnumCommandAbort _command_abort)
        {
            switch (_command_abort)
            {
                case EnumCommandAbort.IncludeRunning:
                {
                    // 현재 실행중인 명령 중단.
                    var running  = PopCommand();
                    if (running != null && running.State == EnumState.Progress)
                    {
                        running.Abort();
                    }

                    RemoveAbortCommands();

                    return true;
                }
                case EnumCommandAbort.PendingOnly:
                {
                    var running       = PeekCommand();
                    var is_not_running = (running == null) || (running.State != EnumState.Progress);

                    // 실행중인 명령이 없으면 중단.
                    if (is_not_running)
                    {              
                        RemoveAbortCommands();
                        return true;
                    }


                    return false;
                }
            }

            return false;   
        }

        void RemoveAbortCommands()
        {
            // 중단 요청 전 까지의 명령을 삭제합니다. 
            Command last_abort = null;

            var command_list = ListPool<Command>.Acquire();

            // 기존 명령 백업
            while (m_command_queue.Count > 0)
            {
                var command = PopCommand();

                command_list.Add(command);

                if (command is Command_Abort)
                    last_abort = command;
            }

            // 명령 중단 요청이 오기 전까지 항목을 제외하고 명령 복원.
            foreach (var command in command_list)
            {
                if (last_abort != null)
                {
                    if (command != last_abort)
                        continue;

                    last_abort = null;
                }
                else
                {
                    PushCommand(command);
                }
            }

            ListPool<Command>.Return(command_list);
        }
    }
}

