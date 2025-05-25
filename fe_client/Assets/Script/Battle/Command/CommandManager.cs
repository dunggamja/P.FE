using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CommandManager //: Singleton<CommandManager>
    {
        
        Int64          m_owner_id      = 0;
        Queue<Command> m_command_queue = new (10);

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
            // IsAbort    = false;
        }

        public void Clear()
        {
            m_command_queue.Clear();
            // IsAbort = false;
        }

        public bool PushCommand(Command _command)
        {
            m_command_queue.Enqueue(_command);
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
    }
}

