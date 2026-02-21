using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    
    public class AIDataManager : IAIDataManager
    {
        public  Int64      ID     { get; private set; } = 0;

        public  EnumAIType AIType
        {
            get 
            {
                // // TAG에 셋팅된 AIType 조회. 그 중 첫번째 것을 반환
                // {

                //     var tag_info       = TAG_INFO.Create(EntityManager.Instance.GetEntity(ID));
                //     using var list_tag = ListPool<TAG_DATA>.AcquireWrapper();

                //     //
                //     TagManager.Instance.CollectTagOwner(tag_info, EnumTagAttributeType.AI_TYPE, list_tag.Value);
                //     foreach(var tag in list_tag.Value)
                //     {         
                //         return (EnumAIType)tag.TargetInfo.TagValue;
                //     }
                // }

                // 없으면 기본 AIType 반환.
                return m_ai_type_base;
            }
        }

        private EnumAIType m_ai_type_base    = EnumAIType.None;

        public void Initialize(Entity _owner)
        {
            ID                = _owner.ID;
            m_ai_type_base    = EnumAIType.None;
            // m_ai_type_current = EnumAIType.None;            
        }


        public void SetAITypeBase(EnumAIType _ai_type)
        {
            m_ai_type_base = _ai_type;
        }

        public AIDataManager_IO Save()
        {
            return new AIDataManager_IO()
            {
                ID         = ID,
                AITypeBase = m_ai_type_base,
            };
        }
        
        public void Load(AIDataManager_IO _snapshot)
        {
            ID             = _snapshot.ID;
            m_ai_type_base = _snapshot.AITypeBase;
        }
    }

    public class AIDataManager_IO
    {
        public Int64             ID         { get; set; } = 0;
        public EnumAIType        AITypeBase { get; set; } = EnumAIType.None;


    }

}