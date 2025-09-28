using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{

    public class AIManager
    {
        private EnumAIType       m_ai_type    = EnumAIType.None;
        
        private Dictionary<EnumAIPriority, List<IAIUpdater>> m_repository = new();

        public bool Initialize(IAIDataManager _owner)
        {
            SetAIType(_owner.AIType);
            return true;
        }


        public void Update(EnumAIPriority _priority, IAIDataManager _param)
        {
            if (_param == null)
                return;

            // AIType 셋팅. 
            if (m_ai_type != _param.AIType)
                SetAIType(_param.AIType);
 

            if (m_repository.TryGetValue(_priority, out var list_updater))
            {
                foreach(var ai in list_updater)
                {
                    ai.Update(_param);
                }
            }
        }


        private void SetAIType(EnumAIType _ai_type)
        {
            m_ai_type = _ai_type;
            m_repository.Clear();

            // 대기 : 다른 행동들 모두 할거 없을때 처리.
            AddAIUpdater(EnumAIPriority.Others, new AI_Score_Done());


            switch(_ai_type)
            {
                // 공격:
                case EnumAIType.Attack:
                    // 1. 공격 가능한 적이 있으면 공격.
                    AddAIUpdater(EnumAIPriority.Primary,   new AI_Score_Attack());
                    // 2. 가까운 적을 향해 이동.
                    AddAIUpdater(EnumAIPriority.Secondary, new AI_Score_Move());
                    break;

                // 요격:
                case EnumAIType.Intercept:
                    // 1. 공격 가능한 적이 있으면 공격.
                    AddAIUpdater(EnumAIPriority.Primary,   new AI_Score_Attack());
                    break;

                // 경계
                case EnumAIType.Alert:
                    // 1. 공격 가능한 적이 있으면 공격.
                    AddAIUpdater(EnumAIPriority.Primary,   new AI_Score_Attack());
                    // 2. 적 사정거리 내에 있을경우 도망친다.
                    //AddAIUpdater(EnumAIPriority.Secondary, new AI_Score_Move());
                    break;
            }
        }

        private void AddAIUpdater(EnumAIPriority _priority, IAIUpdater _ai_updater)
        {
            if (m_repository.TryGetValue(_priority, out var list_updater) == false)
            {
                list_updater = new List<IAIUpdater>();
                m_repository.Add(_priority, list_updater);
            }
            
            list_updater.Add(_ai_updater);
        }
    }
}