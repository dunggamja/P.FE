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

        public bool Initialize(IAIUpdaterOwner _owner)
        {
            SetAIType(_owner.AIType);
            return true;
        }


        public void Update(EnumAIPriority _priority, IAIUpdaterOwner _param)
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

            // 대기는 우선순위 매우 낮음.
            AddAIUpdater(EnumAIPriority.Others, new AI_Score_Done());


            switch(_ai_type)
            {
                case EnumAIType.Attack:
                    AddAIUpdater(EnumAIPriority.Primary, new AI_Score_Attack());
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