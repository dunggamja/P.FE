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
                    // ���� ���� 
                    OnSituationEvent_Turn(situation_updated);
                    // ��ų ���.
                    Skill.UseSkill(situation_updated.Situation, this);
                    
                    break;
                case Battle_AI_Command_DecisionEvent ai_update_event:
                    // AI ����
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
                // ���� ����Ǿ��� ��.
                // �ൿ �����ϰԲ�...
                SetAllCommandEnable();
            }
        }   


        void OnAIUpdateEvent(Battle_AI_Command_DecisionEvent _event)
        {
            // �׾����� �ϰ͵� ����.
            if (IsDead)
                return;

            // �ٸ� ������ ���̸� �ƹ� �͵� ���� �ʴ´�.
            var faction = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);
            if (faction != GetFaction())
                return;

            // �÷��̾� ���̸� AI Update �� ���� ������?
            // TODO: ȥ�� ���� ���� ���߿� ���� ó��.
            var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);
            if (commander_type == EnumCommanderType.Player)
                return;

            // ���� ����� �������� ������ �ִٸ� �ش� ������ ó���� ���������� ��ٸ���.
            var command_progress_id = BattleSystemManager.Instance.BlackBoard.PeekCommandProgressEntity();
            if (command_progress_id > 0 && command_progress_id != ID)
                return;
            
            // TODO: ���߿� �ʿ��� Sensor�� ������Ʈ �� �� �ְ� ���� �ʿ�.
            AIManager.Update(this);

            // event param�� ���.
            _event.TryTopScore(ID, GetAIScoreMax().score);
        }
    }
}