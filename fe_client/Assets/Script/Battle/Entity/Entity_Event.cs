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
                    // ���� ���� 
                    OnReceiveEvent_Battle_Situation_UpdateEvent(situation_updated);
                    
                    
                    break;
                // case Battle_AI_Command_DecisionEvent ai_update_event:
                //     // AI ����
                //     OnReceiveEvent_Battle_AI_Command_DecisionEvent(ai_update_event);
                //     break;

                // case Battle_Entity_MoveEvent entity_move_event:
                //     // �̵� �̺�Ʈ
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
                // TODO: ����μ��� �÷� ����� ���� �ƹ��͵� ����.
                // �� ��ų���� Plan ��� ó���� �ʿ��ұ�?
                return;
            }

            if (_event.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // ���� ����Ǿ��� ��.
                // �ൿ �����ϰԲ�...
                SetAllCommandEnable();
            }


            // ��ų ���.
            Skill.UseSkill(_event.Situation, this);
        }   


        // void OnReceiveEvent_Battle_AI_Command_DecisionEvent(Battle_AI_Command_DecisionEvent _event)
        // {
        //     // �׾����� �ϰ͵� ����.
        //     if (IsDead)
        //         return;

        //     // ������ �ٸ�.
        //     if (_event.Faction != GetFaction())
        //         return;

        //     // �ൿ �켱������ �ٸ�.
        //     if (_event.Priority != GetCommandPriority())
        //         return;
            
        //     // TODO: ���߿� �ʿ��� Sensor�� ������Ʈ �� �� �ְ� ���� �ʿ�.
        //     AIManager.Update(this);

        //     // event param�� ���.
        //     _event.TrySetScore(ID, GetAIScoreMax().score);
        // }

        // void OnReceiveEvent_Battle_Entity_MoveEvent(Battle_Entity_MoveEvent _event)
        // {
        //     if (_event == null)
        //         return;
        // }
    }
}