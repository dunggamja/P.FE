using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{

    // class Entity_Score_Calculator
    // {
    //     // public int      Faction      { get; private set; } = 0;
    //     // public int      Priority     { get; private set; } = 0;

    //     public float               BestScore    { get; private set; } = 0;
    //     public Int64               BestEntityID { get; private set; } = 0;

    //     // public EnumCommandPriority Priority     { get; private set; } = EnumCommandPriority.None;
    //     public void Reset()
    //     {
    //         // Faction      = 0;
    //         // Priority     = 0;

    //         BestScore    = 0;
    //         BestEntityID = 0;
    //         // Priority     = EnumCommandPriority.None;
    //     }

    //     // public void SetFaction(int _faction)
    //     // {
    //     //     Faction = _faction;
    //     // }

    //     // public void SetPriority(int _priority)
    //     // {
    //     //     Priority = _priority;
    //     // }


    //     public void OnUpdate_Entity_Command_Score(Entity _entity)
    //     {
    //         // 죽었다.
    //         if (_entity.IsDead)
    //             return;

    //         // // 진영이 다르다.
    //         // if (Faction != _entity.GetFaction())
    //         //     return;

    //         // 행동이 진행 중이 아니다.
    //         if (_entity.IsEnableCommandProgress() == false)
    //             return;

    //         // 행동 우선순위가 다르다.
    //         // if (Priority != _entity.GetCommandPriority())
    //         //     return;
            


    //         _entity.AIManager.Update(Priority, _entity.AIDataManager);

    //         var entity_score = _entity.GetAIScoreMax().score;
    //         if (entity_score > BestScore)
    //         {
    //             BestScore      = entity_score;
    //             BestEntityID   = _entity.ID;
    //         }
    //     }
    // }


    public partial class BattleSystem_Decision_Making : BattleSystem//, IEventReceiver
    {
        // 행동 우선순위.
        // static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));


        // 행동 우선순위 계산기.
        // Entity_Score_Calculator  m_entity_score_calculator = new Entity_Score_Calculator();

        private CommandQueueHandler m_command_queue_handler = null;

        public BattleSystem_Decision_Making(CommandQueueHandler _command_queue_handler) : base(EnumSystem.BattleSystem_Decision_Making)
        {
            m_command_queue_handler = _command_queue_handler;
        }

        protected override void OnInit()
        {
            
        }

        protected override void OnRelease()
        {
            // m_list_command.Clear();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // 현재 진영.
            var faction         = (int)BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);

            // 진영 명령 유형.
            var commander_type  = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            switch(commander_type)
            {
                // AI 명령 유형.
                case EnumCommanderType.None:
                case EnumCommanderType.AI:
                {   
                    OnUpdate_DecisionMaking_AI(faction);
                }
                break;

                case EnumCommanderType.Player:           
                {
                    // TODO: 플레이어 명령 유형.
                }     
                break;
            }

           
                
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }


        void OnUpdate_DecisionMaking_AI(int _faction)//, EnumCommandPriority _priority)
        {
            // 진영 설정.


            // 행동 우선순위에 대한 값.
            var priority = int.MinValue;

            // 현재 행동가능한 유닛중 가장 행동우선순위가 높은 유닛을 찾습니다.
            EntityManager.Instance.Loop(e =>
            {
                // 기본 체크 1. (진영, 생존 체크)
                if (e == null || e.IsDead || e.GetFaction() != _faction)
                    return;

                // 기본 체크 2. (행동 가능 체크)
                if (e.IsEnableCommandProgress() == false)
                    return;

                priority = Mathf.Max(priority, e.GetCommandPriority());
            });

            // 행동 우선순위가 일치하는 유닛 목록을 찾습니다.
            using var list_entity = ListPool<Entity>.AcquireWrapper();
            EntityManager.Instance.Loop(e =>
            {
                // 진영, 생존 체크.
                if (e == null || e.IsDead || e.GetFaction() != _faction)
                    return;

                // 행동 가능 체크
                if (e.IsEnableCommandProgress() == false)
                    return;

                // 행동 우선순위 체크.
                if (e.GetCommandPriority() != priority)
                    return;

                list_entity.Value.Add(e);
            });

            // 추출된 유닛 목록중 가장 최적의 행동을 할수있는 유닛을 고른다.
            Int64 best_score_id = 0;
            float best_score    = 0f;
            foreach(var entity in list_entity.Value)
            {
                entity.AIManager.Update(entity.AIDataManager);
                
                var ai_score = entity.AIManager.AIBlackBoard.GetBestScore().score;
                if (ai_score > best_score)
                {
                    best_score_id = entity.ID;
                    best_score    = ai_score;
                }
            }

            //  AI 연산 결과후 최적의 행동을 할수있는 유닛이 없으면... 그냥 첫번째 유닛으로...
            if (best_score_id == 0 && list_entity.Value.Count > 0)
            {
                best_score_id = list_entity.Value[0].ID;
            }

            // 최고점 유닛은 행동 명령을 실행.
            if (best_score_id != 0)
            {
                PushCommand(best_score_id);
                // Debug.Log($"PushCommand: {priority}, {best_score_id}");
            }
            else
            {
                // 이런 경우는 없을 것이다.
            }
        }

        bool PushCommand(Int64 _entity_id)
        {
            var entity_object  = EntityManager.Instance.GetEntity(_entity_id);
            if (entity_object == null)
                return false;

            switch(entity_object.AIManager.AIBlackBoard.GetBestScore().type)
            {
                case EnumAIBlackBoard.Score_Attack:
                {
                    // 공격 명령
                    PushCommand_Attack(entity_object.ID, entity_object.AIManager.AIBlackBoard.Score_Attack);
                }
                break;

                case EnumAIBlackBoard.Score_Move:
                {
                    // 이동 명령
                    PushCommand_Move(entity_object.ID, entity_object.AIManager.AIBlackBoard.Score_Move);
                }
                break;

                default:
                {
                    // 대기 명령
                    PushCommand_Done(entity_object.ID);
                }
                break;
            }

            return true;
        }

        void PushCommand_Attack(Int64 _entity_id, AI_Score_Attack.Result _damage_score)
        {           
            // 이동 우선순위 계산 결과 처리.               
            m_command_queue_handler.PushCommand(
                
                    // 이동 우선순위 계산 결과 처리.
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        // _cell_event: EnumCellPositionEvent.Move,
                        _execute_command: true
                        //, _is_immediate: true // 즉시 이동.    
                    ));
                    
            m_command_queue_handler.PushCommand(
                    // 공격 명령
                    new Command_Attack
                    (
                        _entity_id,
                        _damage_score.TargetID,
                        _damage_score.WeaponID,
                        _damage_score.Position
                    ));

            // 행동 종료 명령.
            m_command_queue_handler.PushCommand(new Command_Done(_entity_id));
        }

        void PushCommand_Done(Int64 _entity_id)
        {
            m_command_queue_handler.PushCommand(new Command_Done(_entity_id));
        }

        void PushCommand_Move(Int64 _entity_id, AI_Score_Move.Result _move_score)
        {
            // 이동 명령
            m_command_queue_handler.PushCommand(new Command_Move(
                _entity_id, 
                _move_score.Position,
                _execute_command: true,
                _visual_immediate: false,
                _is_plan: false
            ));

            // 행동 종료 명령.
            m_command_queue_handler.PushCommand(new Command_Done(_entity_id));
        }

    }
}