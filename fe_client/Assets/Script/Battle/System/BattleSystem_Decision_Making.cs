using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{

    /*
    TODO: 행동 타입을 여러 개로 나눠야 한다.
          밑의 행동패턴은 베르위크 사가 행동 패턴 (참고용)
    
    진격	    
        근처에 있는 적을 향해 이동해, 사정거리에 잡으면 공격해 온다.
    
    진격(한정형)	
        특정 목표를 향해 이동해, 사정거리에 잡으면 공격해 온다.
        목표 이외에는 공격하지 않는다. 목표로의 이동 루트를 완전히 봉쇄해도 다른 적에게는 공격하지 않는다.
        목표가 맵상에서 사라지면 통상적인 진격 타입이 된다.
    
    진격(이동형)
    	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 특정 목표를 향해 이동.	○
    
    요격
    	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 그 자리에서 대기.	○
    
    요격(거점형)
    	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 거점으로 이동.	○
    
    경계
    	사정거리 내에 공격 가능한 적이 있으면 공격,
        없는 경우에는 적의 사정거리에 있으면 사정거리 밖으로 도망친다, 안에 없으면 그 자리에서 대기.	○
    
    경계(접근형)
    	사정거리 내에 공격 가능한 적이 있으면 공격,
        없는 경우는 적의 사정거리에 있으면 사정거리 밖으로 도망치고, 안 들어오지 않으면 적의 공격 범위 아슬아슬하게 바깥까지 다가간다.	○
    
    경계(대기형)
    	적의 사정거리에 들어가면 사정거리 밖으로 도망치고, 들어가지 않으면 그 자리에서 대기. 공격해 오지 않는다.	×
    
    배회
        랜덤으로 이동.
    
    배회(호전형)
        사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 랜덤으로 이동.
    
    순회
        사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 일정한 루트를 이동.
    
    고정
        그 자리를 움직이지 않는다.
        그 자리에서 공격 가능하면 공격한다.
        인접 등 공격 가능하지 않으면 도발은 할 수 없다.
    
    이동
        특정 목표를 향해 이동. 공격해 오지 않는다.
    
    이탈	
        이탈 포인트로 향한다. 공격해 오지 않는다.
    
    이탈(호전형)	
        이탈 포인트로 향한다.
        이동처에서 공격 가능한 상대가 있으면 공격해 온다.
        적 찾기 맵 등에서는 보이는 유닛이 가장 가까이에서 인식된다.
        이동처 한정의 적은 숲에 인접한 등 그 이동처에서 공격이 가능하지 않으면 도발할 수 없다.
        전투 불능이나 사용할 수 있는 무기가 없어진 경우, 일부를 제외하고 '이탈'이 된다. 그 경우, 이동처나 목표 등 한정은 일부를 제외하고 해제된다.( 미검증)

    이탈 타입: 
        이탈 포인트가 여러 개 있는 경우에는 최단 거리의 장소로 향한다.
    
    경계 타입: 
        적의 사정거리 판정은 그 군의 첫 번째 손에 이루어지기 때문에 그 시점에서 적이 무기를 소지하지 않거나 휴대 가방에 넣고 있는 경우에는 사정거리는 인식되지 않는다.
    */

    class Entity_Score_Calculator
    {
        public int                 Faction      { get; private set; } = 0;
        public EnumCommandPriority Priority     { get; private set; } = EnumCommandPriority.None;

        public float               BestScore    { get; private set; } = 0;
        public Int64               BestEntityID { get; private set; } = 0;

        public void Reset()
        {
            Faction      = 0;
            Priority     = EnumCommandPriority.None;
            BestScore    = 0;
            BestEntityID = 0;
        }

        public void SetData(int _faction, EnumCommandPriority _priority)
        {
            Faction      = _faction;
            Priority     = _priority;
        }


        public void OnUpdate_Entity_Command_Score(Entity _entity)
        {
            // 죽었으면 암것도 안함.
            if (_entity.IsDead)
                return;

            // 진영이 다름.
            if (Faction != _entity.GetFaction())
                return;

            // 행동 우선순위가 다름.
            if (Priority != _entity.GetCommandPriority())
                return;
            
            // TODO: 나중에 필요한 Sensor만 업데이트 할 수 있게 정리 필요.
            _entity.AIManager.Update(_entity);

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
        static readonly EnumCommandPriority[] s_list_priority = (EnumCommandPriority[])Enum.GetValues(typeof(EnumCommandPriority));


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
                    // 의사결정 우선순위 순으로 처리.
                    for (int i = s_list_priority.Length - 1; i >= 0; --i)
                    {
                        // 의사결정에 성공했으면 더 이상 처리하지 않는다.
                        if (OnUpdate_DecisionMaking_AI(faction, s_list_priority[i]) == true)
                            break;
                    }
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


        bool OnUpdate_DecisionMaking_AI(int _faction, EnumCommandPriority _priority)
        {

            // 우선순위가 없으면 무시.
            if (_priority == EnumCommandPriority.None)
                return false;


            m_entity_score_calculator.Reset();
            m_entity_score_calculator.SetData(_faction, _priority);

            // 모든 엔티티에 대해서 명령 점수를 계산한다.
            EntityManager.Instance.Loop(m_entity_score_calculator.OnUpdate_Entity_Command_Score);


            if (0 < m_entity_score_calculator.BestEntityID)
            { 
                // 
                var entity_id      = m_entity_score_calculator.BestEntityID;
                var entity_object  = EntityManager.Instance.GetEntity(entity_id);
                if (entity_object != null)
                {
                    switch(entity_object.GetAIScoreMax()._type)
                    {
                        case EnumEntityBlackBoard.AIScore_Attack:
                        {
                            // 공격 명령을 내린다.
                            PushCommand_Attack(entity_object.ID, entity_object.BlackBoard.Score_Attack);

                            // Debug.LogWarning($"PushCommand_Attack: {entity_object.ID}");
                        }
                        break;

                        case EnumEntityBlackBoard.AIScore_Done:
                        {
                            // 행동 완료 명령을 내린다.
                            PushCommand_Done(entity_object.ID);

                            // Debug.LogWarning($"PushCommand_Done: {entity_object.ID}");
                        }
                        break;
                    }

                    return true;
                }
            }

            return false;
        }

        void PushCommand_Attack(Int64 _entity_id, AI_Attack.ScoreResult _damage_score)
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