using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;


namespace Battle
{
    public class BattleSystem_Command_Progress : BattleSystem//, IEventReceiver
    {

        // private HashSet<Int64>                     m_command_progress  = new (10);

        private HashSet<Int64> m_entity_progress  = new (10);

        private List<Int64>    m_entity_completed = new (10);

        // Command m_command_progress = null;

        public BattleSystem_Command_Progress() : base(EnumSystem.BattleSystem_Command_Progress)
        {
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
            while (BattleSystemManager.Instance.PeekCommand() != null)
            {
                var command = BattleSystemManager.Instance.PopCommand();
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
                var entity = EntityManager.Instance.GetEntity(entity_id);
                if (entity == null)
                    continue;

                // 명령 진행.
                entity.CommandManager.Update();     

                // 명령 진행 완료 엔티티 추가.
                if (entity.CommandManager.PeekCommand() == null)
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
