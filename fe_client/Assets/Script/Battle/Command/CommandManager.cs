using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class CommandManager : Singleton<CommandManager>
    {
        // SRPG는 턴제라서... 명령이 들어오면 모두 처리한다고 보면 될 거 같음.
        // Int64          m_owner_id      = 0;
        Queue<Command> m_command_queue = new (10);

        protected override void Init()
        {
            base.Init();
        }

        public bool PushCommand(Command _command)
        {
            m_command_queue.Enqueue(_command);
            return true;
        }

        public bool PushCommand(IEnumerable<Command> _commands)
        {
            foreach (var command in _commands)
                PushCommand(command);

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
    }
}

public static class QueuePool<T>
{
    private static readonly Stack<Queue<T>> m_pool = new();

    public static Queue<T> Acquire()
    {
        var queue = m_pool.Count > 0 ? m_pool.Pop() : new Queue<T>();
        return queue;
    }

    public static void Release(Queue<T> queue)
    {
        if (queue == null)
            return;

        queue.Clear();
        m_pool.Push(queue);
    }
}
