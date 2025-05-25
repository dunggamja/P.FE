using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{
    public partial class BattleSystem_Decision_Making : BattleSystem//, IEventReceiver
    {
        // 의사결정 우선순위.
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
            // 현재 턴에 행동하는 진영.
            var faction         = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);

            // 현재 턴에 행동하는 진영의 명령 처리 타입.
            var commander_type  = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            switch(commander_type)
            {
                // AI의 의사결정 프로세스.
                case EnumCommanderType.None:
                case EnumCommanderType.AI:
                {
                    // 의사결정 우선순위 순으로 처리.
                    for (int i = s_list_priority.Length - 1; i >= 0; --i)
                    {
                        // 의사결정에 성공했으면 더 이상 처리하지 않는다.
                        if (OnUpdate_DecisionMaking_AI(faction, s_list_priority[i]) == true)
                            break;
                    }
                }
                break;

                case EnumCommanderType.Player:           
                {
                    // TODO: 유저의 입력 명령들 검증 필요?
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

            // 우선순위가 없으면 무시.
            if (_priority == EnumCommandPriority.None)
                return false;

            // AI 갱신 이벤트
            var ai_update_event = ObjectPool<Battle_AI_Command_DecisionEvent>.Acquire();

            // 진영, 우선순위 셋팅.
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
                            // 공격 명령을 내린다.
                            PushCommand_Attack(entity_object.ID, entity_object.BlackBoard.Score_Attack);
                        }
                        break;

                        case EnumEntityBlackBoard.AIScore_Done:
                        {
                            // 행동 완료 명령을 내린다.
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
           
            // 공격 명령 셋팅.               
            BattleSystemManager.Instance.PushCommand(
                
                    // 이동 명령
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position
                        //, _is_immediate: true // 일단은 즉시 이동.    
                    ));
                    
            BattleSystemManager.Instance.PushCommand(
                    // 공격 명령
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