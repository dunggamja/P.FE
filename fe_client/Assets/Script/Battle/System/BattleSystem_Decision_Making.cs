using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{

    /*
    TODO: �ൿ Ÿ���� ���� ���� ������ �Ѵ�.
          ���� �ൿ������ ������ũ �簡 �ൿ ���� (�����)
    
    ����	    
        ��ó�� �ִ� ���� ���� �̵���, �����Ÿ��� ������ ������ �´�.
    
    ����(������)	
        Ư�� ��ǥ�� ���� �̵���, �����Ÿ��� ������ ������ �´�.
        ��ǥ �̿ܿ��� �������� �ʴ´�. ��ǥ���� �̵� ��Ʈ�� ������ �����ص� �ٸ� �����Դ� �������� �ʴ´�.
        ��ǥ�� �ʻ󿡼� ������� ������� ���� Ÿ���� �ȴ�.
    
    ����(�̵���)
    	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ Ư�� ��ǥ�� ���� �̵�.	��
    
    ���
    	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ �� �ڸ����� ���.	��
    
    ���(������)
    	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ �������� �̵�.	��
    
    ���
    	�����Ÿ� ���� ���� ������ ���� ������ ����,
        ���� ��쿡�� ���� �����Ÿ��� ������ �����Ÿ� ������ ����ģ��, �ȿ� ������ �� �ڸ����� ���.	��
    
    ���(������)
    	�����Ÿ� ���� ���� ������ ���� ������ ����,
        ���� ���� ���� �����Ÿ��� ������ �����Ÿ� ������ ����ġ��, �� ������ ������ ���� ���� ���� �ƽ��ƽ��ϰ� �ٱ����� �ٰ�����.	��
    
    ���(�����)
    	���� �����Ÿ��� ���� �����Ÿ� ������ ����ġ��, ���� ������ �� �ڸ����� ���. ������ ���� �ʴ´�.	��
    
    ��ȸ
        �������� �̵�.
    
    ��ȸ(ȣ����)
        �����Ÿ� ���� ���� ������ ���� ������ ����, ������ �������� �̵�.
    
    ��ȸ
        �����Ÿ� ���� ���� ������ ���� ������ ����, ������ ������ ��Ʈ�� �̵�.
    
    ����
        �� �ڸ��� �������� �ʴ´�.
        �� �ڸ����� ���� �����ϸ� �����Ѵ�.
        ���� �� ���� �������� ������ ������ �� �� ����.
    
    �̵�
        Ư�� ��ǥ�� ���� �̵�. ������ ���� �ʴ´�.
    
    ��Ż	
        ��Ż ����Ʈ�� ���Ѵ�. ������ ���� �ʴ´�.
    
    ��Ż(ȣ����)	
        ��Ż ����Ʈ�� ���Ѵ�.
        �̵�ó���� ���� ������ ��밡 ������ ������ �´�.
        �� ã�� �� ����� ���̴� ������ ���� �����̿��� �νĵȴ�.
        �̵�ó ������ ���� ���� ������ �� �� �̵�ó���� ������ �������� ������ ������ �� ����.
        ���� �Ҵ��̳� ����� �� �ִ� ���Ⱑ ������ ���, �Ϻθ� �����ϰ� '��Ż'�� �ȴ�. �� ���, �̵�ó�� ��ǥ �� ������ �Ϻθ� �����ϰ� �����ȴ�.( �̰���)

    ��Ż Ÿ��: 
        ��Ż ����Ʈ�� ���� �� �ִ� ��쿡�� �ִ� �Ÿ��� ��ҷ� ���Ѵ�.
    
    ��� Ÿ��: 
        ���� �����Ÿ� ������ �� ���� ù ��° �տ� �̷������ ������ �� �������� ���� ���⸦ �������� �ʰų� �޴� ���濡 �ְ� �ִ� ��쿡�� �����Ÿ��� �νĵ��� �ʴ´�.
    */

    class Entity_Score_Calculator
    {
        public int                 Faction      { get; private set; } = 0;
        public EnumCommandPriority Priority     { get; private set; } = EnumCommandPriority.None;

        public float               BestScore    { get; private set; } = 0;
        public Int64               BestEntityID { get; private set; } = 0;

        public void Reset()
        {
            Faction      = 0;
            Priority     = EnumCommandPriority.None;
            BestScore    = 0;
            BestEntityID = 0;
        }

        public void SetData(int _faction, EnumCommandPriority _priority)
        {
            Faction      = _faction;
            Priority     = _priority;
        }


        public void OnUpdate_Entity_Command_Score(Entity _entity)
        {
            // �׾����� �ϰ͵� ����.
            if (_entity.IsDead)
                return;

            // ������ �ٸ�.
            if (Faction != _entity.GetFaction())
                return;

            // �ൿ �켱������ �ٸ�.
            if (Priority != _entity.GetCommandPriority())
                return;
            
            // TODO: ���߿� �ʿ��� Sensor�� ������Ʈ �� �� �ְ� ���� �ʿ�.
            _entity.AIManager.Update(_entity);

            var entity_score = _entity.GetAIScoreMax().score;
            if (entity_score > BestScore)
            {
                BestScore      = entity_score;
                BestEntityID   = _entity.ID;
            }
        }
    }


    public partial class BattleSystem_Decision_Making : BattleSystem//, IEventReceiver
    {
        // �ǻ���� �켱����.
        static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));


        // �ൿ ���� ����.
        Entity_Score_Calculator  m_entity_score_calculator = new Entity_Score_Calculator();

        public BattleSystem_Decision_Making() : base(EnumSystem.BattleSystem_Decision_Making)
        {
        }

        protected override void OnInit()
        {
            
        }

        protected override void OnRelease()
        {
            // m_list_command.Clear();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // ���� �Ͽ� �ൿ�ϴ� ����.
            var faction         = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);

            // ���� �Ͽ� �ൿ�ϴ� ������ ��� ó�� Ÿ��.
            var commander_type  = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            switch(commander_type)
            {
                // AI�� �ǻ���� ���μ���.
                case EnumCommanderType.None:
                case EnumCommanderType.AI:
                {
                    // �ǻ���� �켱���� ������ ó��.
                    for (int i = s_list_priority.Length - 1; i >= 0; --i)
                    {
                        // �ǻ������ ���������� �� �̻� ó������ �ʴ´�.
                        if (OnUpdate_DecisionMaking_AI(faction, s_list_priority[i]) == true)
                            break;
                    }
                }
                break;

                case EnumCommanderType.Player:           
                {
                    // TODO: ������ �Է� ��ɵ� ���� �ʿ�?
                }     
                break;
            }

           
                
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }


        bool OnUpdate_DecisionMaking_AI(int _faction, EnumCommandPriority _priority)
        {

            // �켱������ ������ ����.
            if (_priority == EnumCommandPriority.None)
                return false;


            m_entity_score_calculator.Reset();
            m_entity_score_calculator.SetData(_faction, _priority);

            // ��� ��ƼƼ�� ���ؼ� ��� ������ ����Ѵ�.
            EntityManager.Instance.Loop(m_entity_score_calculator.OnUpdate_Entity_Command_Score);


            if (0 < m_entity_score_calculator.BestEntityID)
            { 
                // 
                var entity_id      = m_entity_score_calculator.BestEntityID;
                var entity_object  = EntityManager.Instance.GetEntity(entity_id);
                if (entity_object != null)
                {
                    switch(entity_object.GetAIScoreMax()._type)
                    {
                        case EnumEntityBlackBoard.AIScore_Attack:
                        {
                            // ���� ����� ������.
                            PushCommand_Attack(entity_object.ID, entity_object.BlackBoard.Score_Attack);

                            // Debug.LogWarning($"PushCommand_Attack: {entity_object.ID}");
                        }
                        break;

                        case EnumEntityBlackBoard.AIScore_Done:
                        {
                            // �ൿ �Ϸ� ����� ������.
                            PushCommand_Done(entity_object.ID);

                            // Debug.LogWarning($"PushCommand_Done: {entity_object.ID}");
                        }
                        break;
                    }

                    return true;
                }
            }

            return false;
        }

        void PushCommand_Attack(Int64 _entity_id, AI_Attack.ScoreResult _damage_score)
        {
           
            // ���� ��� ����.               
            BattleSystemManager.Instance.PushCommand(
                
                    // �̵� ���
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        // _cell_event: EnumCellPositionEvent.Move,
                        _execute_command: true
                        //, _is_immediate: true // �ϴ��� ��� �̵�.    
                    ));
                    
            BattleSystemManager.Instance.PushCommand(
                    // ���� ���
                    new Command_Attack
                    (
                        _entity_id,
                        _damage_score.TargetID,
                        _damage_score.WeaponID,
                        _damage_score.Position
                    ));
        }

        void PushCommand_Done(Int64 _entity_id)
        {
            BattleSystemManager.Instance.PushCommand(new Command_Done(_entity_id));
        }

    }
}