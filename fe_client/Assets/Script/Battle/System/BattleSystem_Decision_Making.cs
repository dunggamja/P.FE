using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{
    public partial class BattleSystem_Decision_Making : BattleSystem//, IEventReceiver
    {
        // �ǻ���� �켱����.
        static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));

        public BattleSystem_Decision_Making() : base(EnumSystem.BattleSystem_Decision_Making)
        {
        }

        public override void Init()
        {
            
        }

        public override void Release()
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

            // AI ���� �̺�Ʈ
            var ai_update_event = ObjectPool<Battle_AI_Command_DecisionEvent>.Acquire();

            // ����, �켱���� ����.
            ai_update_event.Set(_faction, _priority);

            // AI Update Event Dispatch
            EventDispatchManager.Instance.UpdateEvent(ai_update_event);

            if (0 < ai_update_event.Out_EntityID)
            { 
                // 
                var entity_id      = ai_update_event.Out_EntityID;
                var entity_object  = EntityManager.Instance.GetEntity(entity_id);
                if (entity_object != null)
                {
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
                        _damage_score.Position
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