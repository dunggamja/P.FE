using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{

    class Entity_Score_Calculator
    {
        public int                 Faction      { get; private set; } = 0;
        public EnumAIPriority      Priority     { get; private set; } = EnumAIPriority.Primary;



        public float               BestScore    { get; private set; } = 0;
        public Int64               BestEntityID { get; private set; } = 0;

        // public EnumCommandPriority Priority     { get; private set; } = EnumCommandPriority.None;
        public void Reset()
        {
            Faction      = 0;
            Priority     = EnumAIPriority.Primary;

            BestScore    = 0;
            BestEntityID = 0;
            // Priority     = EnumCommandPriority.None;
        }

        public void SetFaction(int _faction)
        {
            Faction = _faction;
        }

        public void SetPriority(EnumAIPriority _priority)
        {
            Priority = _priority;
        }


        public void OnUpdate_Entity_Command_Score(Entity _entity)
        {
            // 죽었으면 암것도 안함.
            if (_entity.IsDead)
                return;

            // 진영이 다름.
            if (Faction != _entity.GetFaction())
                return;

            // 행동이 불가능한 상태.
            if (_entity.IsEnableCommandProgress() == false)
                return;

            // // 행동 우선순위가 다름.
            // if (Priority != _entity.GetCommandPriority())
            //     return;
            


            _entity.AIManager.Update(Priority, _entity.AIDataManager);

            var entity_score = _entity.GetAIScoreMax().score;
            if (entity_score > BestScore)
            {
                BestScore      = entity_score;
                BestEntityID   = _entity.ID;
            }
        }
    }


    public partial class BattleSystem_Decision_Making : BattleSystem//, IEventReceiver
    {
        // 의사결정 우선순위.
        // static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));


        // 행동 점수 계산기.
        Entity_Score_Calculator  m_entity_score_calculator = new Entity_Score_Calculator();

        public BattleSystem_Decision_Making() : base(EnumSystem.BattleSystem_Decision_Making)
        {
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
                    OnUpdate_DecisionMaking_AI(faction);
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


        bool OnUpdate_DecisionMaking_AI(int _faction)//, EnumCommandPriority _priority)
        {
            m_entity_score_calculator.Reset();

            // 진영 셋팅.
            m_entity_score_calculator.SetFaction(_faction);


            // 모든 엔티티의 AIScore를 리셋.
            EntityManager.Instance.Loop(e => e.ResetAIScore());


            // 우선순위가 높은 AI타입부터 처리합니다.
            for(int i = (int)EnumAIPriority.Begin; i < (int)EnumAIPriority.Max; ++i)
            {
                var priority = (EnumAIPriority)i;

                // 검사할 AI 우선순위 타입.
                m_entity_score_calculator.SetPriority(priority);

                // 모든 엔티티에 대해서 명령 점수를 계산한다.
                EntityManager.Instance.Loop(m_entity_score_calculator.OnUpdate_Entity_Command_Score);


                // 가장 점수가 높게 나온 유닛에게 명령을 내린다.
                if (0 < m_entity_score_calculator.BestEntityID)
                {
                    if (PushCommand(m_entity_score_calculator.BestEntityID))
                        return true;                    
                }
            }           
            

            return false;
        }

        bool PushCommand(Int64 _entity_id)
        {
            var entity_object  = EntityManager.Instance.GetEntity(_entity_id);
            if (entity_object == null)
                return false;

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

        void PushCommand_Attack(Int64 _entity_id, AI_Score_Attack.Result _damage_score)
        {
           
            // 공격 명령 셋팅.               
            BattleSystemManager.Instance.PushCommand(
                
                    // 이동 명령
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        // _cell_event: EnumCellPositionEvent.Move,
                        _execute_command: true
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