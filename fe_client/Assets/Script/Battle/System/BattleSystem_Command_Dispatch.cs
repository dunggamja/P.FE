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
            // 현재 행동 진영.
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
                // TODO: 유저가 명령을 내려야 하는 경우에 대한 처리 필요.
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
                        // 공격 명령을 내린다.
                        Command_By_DamageScore(entity_id, entity_object.BlackBoard.aiscore_attack);
                    }
                    break;

                    case EnumEntityBlackBoard.AIScore_Done:
                    {
                        // 행동 완료 명령을 내린다.
                        CommandManager.Instance.PushCommand(new Command_Done(entity_id));
                    }
                    break;
                }

            }
            



            // // TODO: 행동중인 유닛이 있다면 그것을 우선 처리 해야함.
            // var list_progress = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.Progress);
            // if (list_progress.Count > 0)
            // {
            //     // TODO: 일단은 완료 명령을 내리는 것으로 해둠. 
            //     //       행동이 가능한게 있는지 확인하고 종료하도록 바꿔야 한다.
            //     foreach (var e in list_progress)
            //     {
            //         CommandManager.Instance.PushCommand(new Command_Done(e.ID));
            //     }

            //     return true;
            // }
            

            //var list_none = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.None);
           
            
            //EventDispatchManager.Instance.DispatchEvent(new SituationUpdatedEvent(EnumSituationType.BattleSystem_Command_Dispatch_test, _param));
            
            // // TODO: 지금은 공격 스코어가 가장 높은 타겟을 고르고 있는데...
            // //       추후에 다른 스코어를 추가할 수 있도록 변경 필요.   (전술적 목표에 따라서.)
            // var best_score = (EntityID:(Int64)0, Score:0f);

            // // 가장 점수가 높은 행동을 할 수 있는 유닛을 1명 고릅시다.
            // foreach(var e in list_none)
            // {
            //     // e.BlackBoard.aiscore_attack
            //     var score = e.BlackBoard.GetBPValueAsFloat(EnumEntityBlackBoard.AIScore_Attack);
            //     if (best_score.Score < score)
            //     {
            //         best_score = (e.ID, score);
            //     }
            // }

            // // 가장 점수가 높은 유닛에게 공격 명령   
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
            // 공격 명령 셋팅.                
            CommandManager.Instance.PushCommand(
                new Command[]
                {
                    // 이동 명령
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        _is_immediate: true // 일단은 즉시 이동.    
                    ),

                    // 공격 명령
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