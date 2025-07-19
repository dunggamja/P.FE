using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [EventReceiver(
        typeof(Battle_Situation_UpdateEvent)
        //,
        // typeof(Battle_AI_Command_DecisionEvent)
        // typeof(Battle_Entity_MoveEvent)
        )]
    public partial class Entity
    {

        public void OnReceiveEvent(IEventParam _event)
        {            
            switch (_event)
            {
                case Battle_Situation_UpdateEvent situation_updated:
                    // 진영 갱신 
                    OnReceiveEvent_Battle_Situation_UpdateEvent(situation_updated);
                    
                    
                    break;
                // case Battle_AI_Command_DecisionEvent ai_update_event:
                //     // AI 갱신
                //     OnReceiveEvent_Battle_AI_Command_DecisionEvent(ai_update_event);
                //     break;

                // case Battle_Entity_MoveEvent entity_move_event:
                //     // 이동 이벤트
                //     OnReceiveEvent_Battle_Entity_MoveEvent(entity_move_event);
                //     break;
            }
        }


        void OnReceiveEvent_Battle_Situation_UpdateEvent(Battle_Situation_UpdateEvent _event)
        {
            if (_event == null)
                return;

            if (_event.IsPlan)
            {
                // TODO: 현재로서는 플랜 모드일 때는 아무것도 안함.
                // 각 스킬별로 Plan 모드 처리가 필요할까?
                return;
            }

            if (_event.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // 턴이 변경되었을 때.
                // 행동 가능하게끔...
                SetAllCommandEnable();
            }


            // 스킬 사용.
            Skill.UseSkill(_event.Situation, this);
        }   


        // void OnReceiveEvent_Battle_AI_Command_DecisionEvent(Battle_AI_Command_DecisionEvent _event)
        // {
        //     // 죽었으면 암것도 안함.
        //     if (IsDead)
        //         return;

        //     // 진영이 다름.
        //     if (_event.Faction != GetFaction())
        //         return;

        //     // 행동 우선순위가 다름.
        //     if (_event.Priority != GetCommandPriority())
        //         return;
            
        //     // TODO: 나중에 필요한 Sensor만 업데이트 할 수 있게 정리 필요.
        //     AIManager.Update(this);

        //     // event param에 등록.
        //     _event.TrySetScore(ID, GetAIScoreMax().score);
        // }

        // void OnReceiveEvent_Battle_Entity_MoveEvent(Battle_Entity_MoveEvent _event)
        // {
        //     if (_event == null)
        //         return;
        // }
    }
}