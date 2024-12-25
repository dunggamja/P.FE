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
                // ���� ���� 
                OnSituationEvent_Turn(situation_updated);

                // AI ����
                OnSituationEvent_AI(situation_updated);

                // ��ų ���.
                Skill.UseSkill(situation_updated.Situation, this);
            }
        }


        void OnSituationEvent_Turn(SituationUpdatedEvent _param)
        {
            if (_param == null)
                return;

            if (_param.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // ���� ����Ǿ��� ��.
                // �ൿ �Ϸ� ó�� reset 
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
            var command_progress_id = BattleSystemManager.Instance.BlackBoard.PeekCommandProgressEntityID();
            if (command_progress_id > 0 && command_progress_id != ID)
                return;
            
            // TODO: ���߿� �ʿ��� Sensor�� ������Ʈ �� �� �ְ� ���� �ʿ�.
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