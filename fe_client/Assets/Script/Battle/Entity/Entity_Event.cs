using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [EventReceiver(
        typeof(Battle_Situation_UpdateEvent),
        typeof(Battle_AI_Command_DecisionEvent)
        )]
    public partial class Entity
    {

        public void OnReceiveEvent(IEventParam _event)
        {            
            switch (_event)
            {
                case Battle_Situation_UpdateEvent situation_updated:
                    // 진영 갱신 
                    OnSituationEvent_Turn(situation_updated);
                    // 스킬 사용.
                    Skill.UseSkill(situation_updated.Situation, this);
                    
                    break;
                case Battle_AI_Command_DecisionEvent ai_update_event:
                    // AI 갱신
                    OnAIUpdateEvent(ai_update_event);
                    break;
            }
        }


        void OnSituationEvent_Turn(Battle_Situation_UpdateEvent _event)
        {
            if (_event == null)
                return;

            if (_event.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // 턴이 변경되었을 때.
                // 행동 가능하게끔...
                SetAllCommandEnable();
            }
        }   


        void OnAIUpdateEvent(Battle_AI_Command_DecisionEvent _event)
        {
            // 죽었으면 암것도 안함.
            if (IsDead)
                return;

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
            var command_progress_id = BattleSystemManager.Instance.BlackBoard.PeekCommandProgressEntity();
            if (command_progress_id > 0 && command_progress_id != ID)
                return;
            
            // TODO: 나중에 필요한 Sensor만 업데이트 할 수 있게 정리 필요.
            AIManager.Update(this);

            // event param에 등록.
            _event.TryTopScore(ID, GetAIScoreMax().score);
        }
    }
}