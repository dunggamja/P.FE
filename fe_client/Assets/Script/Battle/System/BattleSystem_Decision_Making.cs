using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{

    class Entity_Score_Calculator
    {
        public int                 Faction      { get; private set; } = 0;
        public EnumAIPriority      Priority     { get; private set; } = EnumAIPriority.Primary;



        public float               BestScore    { get; private set; } = 0;
        public Int64               BestEntityID { get; private set; } = 0;

        // public EnumCommandPriority Priority     { get; private set; } = EnumCommandPriority.None;
        public void Reset()
        {
            Faction      = 0;
            Priority     = EnumAIPriority.Primary;

            BestScore    = 0;
            BestEntityID = 0;
            // Priority     = EnumCommandPriority.None;
        }

        public void SetFaction(int _faction)
        {
            Faction = _faction;
        }

        public void SetPriority(EnumAIPriority _priority)
        {
            Priority = _priority;
        }


        public void OnUpdate_Entity_Command_Score(Entity _entity)
        {
            // �׾����� �ϰ͵� ����.
            if (_entity.IsDead)
                return;

            // ������ �ٸ�.
            if (Faction != _entity.GetFaction())
                return;

            // �ൿ�� �Ұ����� ����.
            if (_entity.IsEnableCommandProgress() == false)
                return;

            // // �ൿ �켱������ �ٸ�.
            // if (Priority != _entity.GetCommandPriority())
            //     return;
            


            _entity.AIManager.Update(Priority, _entity.AIDataManager);

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
        // static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));


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
                    OnUpdate_DecisionMaking_AI(faction);
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


        bool OnUpdate_DecisionMaking_AI(int _faction)//, EnumCommandPriority _priority)
        {
            m_entity_score_calculator.Reset();

            // ���� ����.
            m_entity_score_calculator.SetFaction(_faction);


            // ��� ��ƼƼ�� AIScore�� ����.
            EntityManager.Instance.Loop(e => e.ResetAIScore());


            // �켱������ ���� AIŸ�Ժ��� ó���մϴ�.
            for(int i = (int)EnumAIPriority.Begin; i < (int)EnumAIPriority.Max; ++i)
            {
                var priority = (EnumAIPriority)i;

                // �˻��� AI �켱���� Ÿ��.
                m_entity_score_calculator.SetPriority(priority);

                // ��� ��ƼƼ�� ���ؼ� ��� ������ ����Ѵ�.
                EntityManager.Instance.Loop(m_entity_score_calculator.OnUpdate_Entity_Command_Score);


                // ���� ������ ���� ���� ���ֿ��� ����� ������.
                if (0 < m_entity_score_calculator.BestEntityID)
                {
                    if (PushCommand(m_entity_score_calculator.BestEntityID))
                        return true;                    
                }
            }           
            

            return false;
        }

        bool PushCommand(Int64 _entity_id)
        {
            var entity_object  = EntityManager.Instance.GetEntity(_entity_id);
            if (entity_object == null)
                return false;

            switch(entity_object.GetAIScoreMax()._type)
            {
                case EnumEntityBlackBoard.AIScore_Attack:
                {
                    // ���� ����� ������.
                    PushCommand_Attack(entity_object.ID, entity_object.BlackBoard.Score_Attack);
                }
                break;

                case EnumEntityBlackBoard.AIScore_Done:
                {
                    // �ൿ �Ϸ� ����� ������.
                    PushCommand_Done(entity_object.ID);
                }
                break;
            }

            return true;
        }

        void PushCommand_Attack(Int64 _entity_id, AI_Score_Attack.Result _damage_score)
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