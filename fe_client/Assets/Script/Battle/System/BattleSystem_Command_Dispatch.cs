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
            var faction        = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);
            var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            switch(commander_type)
            {
                case EnumCommanderType.None:
                case EnumCommanderType.AI:
                // AI Update Event Dispatch
                EventDispatchManager.Instance.DispatchEvent(new SituationUpdatedEvent(EnumSituationType.BattleSystem_Command_Dispatch_AI_Update, _param));
                break;

                case EnumCommanderType.Player:                
                // TODO: ������ ����� ������ �ϴ� ��쿡 ���� ó�� �ʿ�.
                break;
            }

            var entity_id      = BattleSystemManager.Instance.BlackBoard.aiscore_top_entity_id;
            var entity_object  = EntityManager.Instance.GetEntity(entity_id);
            if (entity_object != null)
            {
                switch(entity_object.GetAIScoreMax()._type)
                {
                    case EnumEntityBlackBoard.AIScore_Attack:
                    {
                        // ���� ����� ������.
                        Command_By_DamageScore(entity_id, entity_object.BlackBoard.aiscore_attack);
                    }
                    break;

                    case EnumEntityBlackBoard.AIScore_Done:
                    {
                        // �ൿ �Ϸ� ����� ������.
                        CommandManager.Instance.PushCommand(new Command_Done(entity_id));
                    }
                    break;
                }

            }
            



            // // TODO: �ൿ���� ������ �ִٸ� �װ��� �켱 ó�� �ؾ���.
            // var list_progress = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.Progress);
            // if (list_progress.Count > 0)
            // {
            //     // TODO: �ϴ��� �Ϸ� ����� ������ ������ �ص�. 
            //     //       �ൿ�� �����Ѱ� �ִ��� Ȯ���ϰ� �����ϵ��� �ٲ�� �Ѵ�.
            //     foreach (var e in list_progress)
            //     {
            //         CommandManager.Instance.PushCommand(new Command_Done(e.ID));
            //     }

            //     return true;
            // }
            

            //var list_none = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.None);
           
            
            //EventDispatchManager.Instance.DispatchEvent(new SituationUpdatedEvent(EnumSituationType.BattleSystem_Command_Dispatch_test, _param));
            
            // // TODO: ������ ���� ���ھ ���� ���� Ÿ���� ���� �ִµ�...
            // //       ���Ŀ� �ٸ� ���ھ �߰��� �� �ֵ��� ���� �ʿ�.   (������ ��ǥ�� ����.)
            // var best_score = (EntityID:(Int64)0, Score:0f);

            // // ���� ������ ���� �ൿ�� �� �� �ִ� ������ 1�� ���ô�.
            // foreach(var e in list_none)
            // {
            //     // e.BlackBoard.aiscore_attack
            //     var score = e.BlackBoard.GetBPValueAsFloat(EnumEntityBlackBoard.AIScore_Attack);
            //     if (best_score.Score < score)
            //     {
            //         best_score = (e.ID, score);
            //     }
            // }

            // // ���� ������ ���� ���ֿ��� ���� ���   
            // if (best_score.EntityID > 0)
            // { 
            //     var entity_object  = EntityManager.Instance.GetEntity(best_score.EntityID);  
            //     if (entity_object != null)
            //     {
            //         Command_By_DamageScore(entity_object.ID, entity_object.BlackBoard.aiscore_attack);
            //     }
            // }

                
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
                        _damage_score.Position,
                        _is_immediate: true // �ϴ��� ��� �̵�.    
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