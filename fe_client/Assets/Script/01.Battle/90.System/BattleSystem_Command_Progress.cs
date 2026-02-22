using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;


namespace Battle
{
    public interface ICommandQueueHandler
    {
        void    PushCommand(Command _command);
        Command PopCommand();
        Command PeekCommand();
    }

    public class CommandQueueHandler : ICommandQueueHandler
    {
        private Queue<Command>  m_command_queue = new (10);

        public int Count => m_command_queue.Count;

        public void Clear()
        {
            m_command_queue.Clear();
        }

        public void PushCommand(Command _command)
        {
            m_command_queue.Enqueue(_command);
        }

        public Command PopCommand()
        {
            if (Count > 0)
                return m_command_queue.Dequeue();

            return null;
        }

        public Command PeekCommand()
        {
            if (Count > 0)
                return m_command_queue.Peek();

            return null;
        }        
    }


    public class BattleSystem_Command_Progress : BattleSystem//, IEventReceiver
    {
        private CommandQueueHandler m_command_queue_handler = null;

        private HashSet<Int64>      m_entity_progress  = new (10);
     
        private List<Int64>         m_entity_completed = new (10);

        // Command m_command_progress = null;

        public BattleSystem_Command_Progress(CommandQueueHandler _command_queue_handler) : base(EnumSystem.BattleSystem_Command_Progress)
        {
            m_command_queue_handler = _command_queue_handler;
        }

        protected override void OnInit()
        {
            
        }

        protected override void OnRelease()
        {
            
        }


        protected override void OnEnter(IBattleSystemParam _param)
        {
           
        
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // // 대기 중인 명령어를 모든 엔티티에게 분배합니다.
            while (m_command_queue_handler.PeekCommand() != null)
            {
                var command = m_command_queue_handler.PopCommand();
                if (command == null)
                    continue;

                var entity = EntityManager.Instance.GetEntity(command.OwnerID);
                if (entity == null)
                    continue;

                entity.CommandManager.PushCommand(command);           

                // 명령 진행 중인 엔티티 추가.
                m_entity_progress.Add(command.OwnerID);
            }

            // 명령 진행 완료 엔티티 초기화. 
            m_entity_completed.Clear();

            // 명령 진햌 중인 엔티티 처리.
            foreach (var entity_id in m_entity_progress)
            {
                // 명령이 진행중인지 체크하기 위한 변수.
                var is_progress = false;


                var entity  = EntityManager.Instance.GetEntity(entity_id);
                if (entity != null)
                {
                    // 명령 진행.
                    entity.CommandManager.Update();     

                    // 명령 진행중인게 있는지 체크.
                    is_progress = (entity.CommandManager.PeekCommand() != null);
                }


                // 명령 진행중인게 없으면 완료상태로.
                if (is_progress == false)
                    m_entity_completed.Add(entity_id);
            }

            // 명령 진행 완료 엔티티 처리.
            foreach (var entity_id in m_entity_completed)
                m_entity_progress.Remove(entity_id);


            // 명령 진행 중인 엔티티가 없으면 완료.
            return m_entity_progress.Count == 0;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            m_entity_progress.Clear();
            m_entity_completed.Clear();
        }        
    }
}
