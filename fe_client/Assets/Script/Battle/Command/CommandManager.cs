using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CommandManager
    {
        // SRPG는 턴제라서... 명령이 들어오면 모두 처리한다고 보면 될 거 같음.
        Int64          m_owner_id      = 0;
        Queue<Command> m_command_queue = new (10);

        public bool Initialize(IOwner _owner)
        {
            if (_owner == null)
                return false;

            m_owner_id = _owner.ID;
            return true;
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
    }
}