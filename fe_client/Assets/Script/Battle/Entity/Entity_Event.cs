using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [EventReceiverAttribute(typeof(SituationUpdatedEvent))]
    public partial class Entity
    {

        public void OnReceiveEvent(IEventParam _param)
        {
            // situation update event
            if (_param is SituationUpdatedEvent situation_updated)
            {
                // 진영 갱신 
                OnSituationEvent_Turn(situation_updated);

                // AI 갱신
                OnSituationEvent_AI(situation_updated);

                // 스킬 사용.
                Skill.UseSkill(situation_updated.Situation, this);
            }
        }


        void OnSituationEvent_Turn(SituationUpdatedEvent _param)
        {
            if (_param == null)
                return;

            if (_param.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // 턴이 변경되었을 때.
                // 행동 완료 처리 reset 
                ResetCommandProgressState();
            }
        }   

        void OnSituationEvent_AI(SituationUpdatedEvent _param)
        {
            if (_param == null)
                return;

            switch(_param.Situation)
            {
                case EnumSituationType.BattleSystem_Command_Dispatch_AI_Update:
                OnReceiveEvent_AI_CommandDispatch(_param);
                break;
            }

        }

        void OnReceiveEvent_AI_CommandDispatch(SituationUpdatedEvent _param)
        {
            // 다른 진영의 턴이면 아무 것도 하지 않는다.
            var faction = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);
            if (faction != GetFaction())
                return;

            // 플레이어 턴이면 AI Update 할 일이 없겠지?
            // TODO: 혼란 상태 등은 나중에 따로 처리.
            var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);
            if (commander_type == EnumCommanderType.Player)
                return;

            // 현재 명령을 진행중인 유닛이 있다면 해당 유닛의 처리가 끝날때까지 기다리자.
            var command_progress_id = BattleSystemManager.Instance.BlackBoard.PeekCommandProgressEntityID();
            if (command_progress_id > 0 && command_progress_id != ID)
                return;
            
            // TODO: 나중에 필요한 Sensor만 업데이트 할 수 있게 정리 필요.
            AIManager.Update(this);

            var my_score  = GetAIScoreMax().score;
            var top_score = BattleSystemManager.Instance.BlackBoard.GetBPValueAsFloat(EnumBattleBlackBoard.AIScore);
        
            if (my_score > top_score)
            {
                BattleSystemManager.Instance.BlackBoard.SetBPValue(EnumBattleBlackBoard.AIScore, my_score);
                BattleSystemManager.Instance.BlackBoard.aiscore_top_entity_id = ID;
            }
        }
    }
}