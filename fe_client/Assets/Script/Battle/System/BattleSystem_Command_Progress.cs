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
            // �׿��ִ� ����� �� ���ֵ鿡�� �����մϴ�.
            while (BattleSystemManager.Instance.PeekCommand() != null)
            {
                var command = BattleSystemManager.Instance.PopCommand();
                if (command == null)
                    continue;

                var entity = EntityManager.Instance.GetEntity(command.OwnerID);
                if (entity == null)
                    continue;

                entity.CommandManager.PushCommand(command);           

                // ��� ó�� ���� ���� ��Ͽ� �߰�.
                m_entity_progress.Add(command.OwnerID);
            }

            // ��� ó���� �Ϸ�� ���� ���. 
            m_entity_completed.Clear();

            // ��� ��ƼƼ���� ����� ó��.
            foreach (var entity_id in m_entity_progress)
            {
                var entity = EntityManager.Instance.GetEntity(entity_id);
                if (entity == null)
                    continue;

                // ��� ó��.
                entity.CommandManager.Update();     

                // ����� �Ϸ�Ǿ����� üũ.
                if (entity.CommandManager.PeekCommand() == null)
                    m_entity_completed.Add(entity_id);
            }

            // ��� ó���� �Ϸ�� ������ ��Ͽ��� �����մϴ�.
            foreach (var entity_id in m_entity_completed)
                m_entity_progress.Remove(entity_id);


            // ��� ���ֵ��� �ൿ�� �Ϸ�Ǿ��ٸ� ����.
            return m_entity_progress.Count == 0;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            m_entity_progress.Clear();
            m_entity_completed.Clear();
        }        
    }
}
