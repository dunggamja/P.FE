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

        public override void Init()
        {
            
        }

        public override void Release()
        {
            
        }


        protected override void OnEnter(IBattleSystemParam _param)
        {
           
        
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // 쌓여있는 명령을 각 유닛들에게 전달합니다.
            while (BattleSystemManager.Instance.PeekCommand() != null)
            {
                var command = BattleSystemManager.Instance.PopCommand();
                if (command == null)
                    continue;

                var entity = EntityManager.Instance.GetEntity(command.OwnerID);
                if (entity == null)
                    continue;

                entity.CommandManager.PushCommand(command);           

                // 명령 처리 중인 유닛 목록에 추가.
                m_entity_progress.Add(command.OwnerID);
            }

            // 명령 처리가 완료된 유닛 목록. 
            m_entity_completed.Clear();

            // 모든 엔티티들의 명령을 처리.
            foreach (var entity_id in m_entity_progress)
            {
                var entity = EntityManager.Instance.GetEntity(entity_id);
                if (entity == null)
                    continue;

                // 명령 처리.
                entity.CommandManager.Update();     

                // 명령이 완료되었는지 체크.
                if (entity.CommandManager.PeekCommand() == null)
                    m_entity_completed.Add(entity_id);
            }

            // 명령 처리가 완료된 유닛은 목록에서 제거합니다.
            foreach (var entity_id in m_entity_completed)
                m_entity_progress.Remove(entity_id);


            // 모든 유닛들의 행동이 완료되었다면 종료.
            return m_entity_progress.Count == 0;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            m_entity_progress.Clear();
            m_entity_completed.Clear();
        }        
    }
}
