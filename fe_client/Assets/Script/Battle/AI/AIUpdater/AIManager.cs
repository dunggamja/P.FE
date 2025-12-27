using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public class AIBlackBoard : BlackBoard<EnumAIBlackBoard>
    {
        public AI_Score_Attack.Result Score_Attack { get; private set; } = new();
        public AI_Score_Move.Result   Score_Move   { get; private set; } = new();
        public AI_Score_Wand.Result   Score_Wand   { get; private set; } = new();


        public override void Reset()
        {
            base.Reset();
            Score_Attack.Reset();
            Score_Move.Reset();
            Score_Wand.Reset();
        }

        public EnumAIBlackBoard GetBestScoreType()
        {
            var top_score_type = EnumAIBlackBoard.None;
            var top_score      = 0f;



            for(int i = (int)EnumAIBlackBoard.Begin; i < (int)EnumAIBlackBoard.Max; ++i)
            {
                var score = GetBPValueAsFloat((EnumAIBlackBoard)i);
                if (score > top_score)
                {
                    top_score_type = (EnumAIBlackBoard)i;
                    top_score      = score;
                }
            }

            return top_score_type;
        }
    }



    public class AIManager
    {
        private EnumAIType                             m_ai_type       = EnumAIType.None;        
        private Dictionary<int, List<IAIUpdater>>      m_repository    = new();
        private List<int>                              m_priority_list = new();
        public  AIBlackBoard  AIBlackBoard { get; private set; } = new();

        

        public bool Initialize(IAIDataManager _owner)
        {
            SetAIType(_owner.AIType);
            return true;
        }


        public void Update(IAIDataManager _param)
        {
            if (_param == null)
                return;

            // AIType 셋팅. 
            if (m_ai_type != _param.AIType)
                SetAIType(_param.AIType);


            // 우선순위에 따라서 AI 업데이트.
            foreach(var priority in m_priority_list)
            {
                var list_updater = GetAIUpdaterList(priority);
                if (list_updater == null)
                    continue;

                // AI 블랙보드 초기화.
                AIBlackBoard.Reset();

                // AI 업데이트.
                list_updater.ForEach(e => e.Update(_param));

                // 최고 점수 셋팅 성공.
                if (AIBlackBoard.GetBestScoreType() != EnumAIBlackBoard.None)
                    break;                
            }
        }


        private void SetAIType(EnumAIType _ai_type)
        {
            m_ai_type = _ai_type;

            // 기존 AIUpdater 정리.
            m_repository.Clear();
            m_priority_list.Clear();

            // 대기 : 다른 행동들 모두 할거 없을때 처리.
            AddAIUpdater(999, new AI_Score_Done());

            // TODO: 도발 등 상태이상에 걸린 경우에 대한 행동로직 추가해야할듯 
            // AddAIUpdater(-30, // 수면 )
            // AddAIUpdate(-20,  // 도발 )
            // AddAIUpdaer(-10,  // 혼란 )


            switch(_ai_type)
            {
                // 공격:
                case EnumAIType.Attack:
                    // 공격 가능한 적이 있으면 공격.
                    AddAIUpdater(1, new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Normal));
                    // 가까운 적을 향해 이동. 
                    AddAIUpdater(2, new AI_Score_Move(AI_Score_Move.EnumBehavior.Closest_Enemy));

                    break;
                case EnumAIType.Attack_Target:
                    // 타겟 공격. (타겟이 있을 경우만 동작)
                    AddAIUpdater(1, new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Target));
                    // // 타겟으로 가는 길이 막혀있을경우. 타겟으로 가는 길을 막고 있는 것이 적이면 공격.
                    // AddAIUpdater(2, new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Target_Guard));
                    // 타겟과의 가까워지는 것을 최우선. (타겟이 있을 경우만 동작)
                    AddAIUpdater(3, new AI_Score_Move(AI_Score_Move.EnumBehavior.Closest_Target));

                    // 공격 가능한 적이 있으면 공격. (타겟이 없을 경우 동작)
                    AddAIUpdater(4, new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Normal));
                    // 가까운 적을 향해 이동. (타겟이 없을 경우 동작)
                    AddAIUpdater(5, new AI_Score_Move(AI_Score_Move.EnumBehavior.Closest_Enemy));
                    break;


                // 요격:
                case EnumAIType.Intercept:
                    // 1. 공격 가능한 적이 사거리 내에 있으면 공격.
                    AddAIUpdater(1,   new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Normal));
                    break;

                // 경계
                case EnumAIType.Alert:
                    // 1. 공격 가능한 적이 있으면 공격.
                    AddAIUpdater(2,   new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Normal));
                    // 2. 적 사정거리 내에 있을경우 도망친다.
                    //AddAIUpdater(EnumAIPriority.Secondary, new AI_Score_Move());
                    break;

                case EnumAIType.Fixed:
                    // 1. 이동하지 않은 상태에서 공격 가능하면 공격.
                    AddAIUpdater(1, new AI_Score_Attack(AI_Score_Attack.EnumBehavior.Fixed));
                    break;

            }
        }







        private void AddAIUpdater(int _priority, IAIUpdater _ai_updater)
        {
            if (m_repository.TryGetValue(_priority, out var list_updater) == false)
            {
                list_updater = new List<IAIUpdater>();
                m_repository.Add(_priority, list_updater);
            }
            
            list_updater.Add(_ai_updater);

            if (m_priority_list.Contains(_priority) == false)
            { 
                m_priority_list.Add(_priority);
                m_priority_list.Sort();
            }
        }

        private List<IAIUpdater> GetAIUpdaterList(int _priority)
        {
            if (m_repository.TryGetValue(_priority, out var list_updater))
            {
                return list_updater;
            }

            return null;
        }


        
    }
}
