using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CommandManager
    {
        // SRPG�� ������... ����� ������ ��� ó���Ѵٰ� ���� �� �� ����.
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