using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{
    public partial class BattleSystem_Command_Dispatch : BattleSystem//, IEventReceiver
    {
        public BattleSystem_Command_Dispatch() : base(EnumSystem.BattleSystem_Command_Dispatch)
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
            // ���� �ൿ ����.
            var faction         = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);
            var commander_type  = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            //  AI ���� �̺�Ʈ�� �������ô�.
            var ai_update_event = ObjectPool<AIUpdateEvent>.Acquire();

            switch(commander_type)
            {
                case EnumCommanderType.None:
                case EnumCommanderType.AI:
                // AI Update Event Dispatch
                EventDispatchManager.Instance.UpdateEvent(ai_update_event);
                break;

                case EnumCommanderType.Player:                
                // TODO: ������ ����� ������ �ϴ� ��쿡 ���� ó�� �ʿ�.
                break;
            }

            // ai ���� �� ����� ó���ؾ� �ϴ� ���� �ִ��� ������.
            var entity_object  = EntityManager.Instance.GetEntity(ai_update_event.TopScore_EntityID);
            if (entity_object != null)
            {
                switch(entity_object.GetAIScoreMax()._type)
                {
                    case EnumEntityBlackBoard.AIScore_Attack:
                    {
                        // ���� ����� ������.
                        Command_By_DamageScore(entity_object.ID, entity_object.BlackBoard.Score_Attack);
                    }
                    break;

                    case EnumEntityBlackBoard.AIScore_Done:
                    {
                        // �ൿ �Ϸ� ����� ������.
                        CommandManager.Instance.PushCommand(new Command_Done(entity_object.ID));
                    }
                    break;
                }
            }
           
                
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

        void Command_By_DamageScore(Int64 _entity_id, AI_Attack.ScoreResult _damage_score)
        {
            // ���� ��� ����.                
            CommandManager.Instance.PushCommand(
                new Command[]
                {
                    // �̵� ���
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position
                        //, _is_immediate: true // �ϴ��� ��� �̵�.    
                    ),

                    // ���� ���
                    new Command_Attack
                    (
                        _entity_id,
                        _damage_score.TargetID,
                        _damage_score.WeaponID,
                        _damage_score.Position
                    )

                }
            );
        }

    }
}