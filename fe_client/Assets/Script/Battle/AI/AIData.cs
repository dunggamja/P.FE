using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public class AITarget : ITarget
    {
        private List<Int64> m_list_target_id = new();


        public Int64 MainTargetID 
        {
          get
          {
            if (m_list_target_id.Count == 0)
              return 0;

            return m_list_target_id[0];
          }
        }

        public List<Int64> AllTargetIDList => m_list_target_id;
    }

    public class AITargetPosition : ITargetPosition
    {
        public List<(int x, int y)> Positions {get; private set;} = new();
    }
    
    


    public class AIData : IAIData
    {
        public ITarget         Targets        { get; private set; } = new AITarget();

        public ITargetPosition TargetPosition { get; private set; } = new AITargetPosition();

        public (int x, int y)  BasePosition   { get; private set; } = (0, 0);
    }


    public class AIDataManager : IAIDataManager
    {
        public Int64             ID     { get; private set; } = 0;
        public EnumAIType        AIType { get; private set; } = EnumAIType.None;
        public IAIData           AIData { get; private set; } = new AIData();

        public void Initialize(Entity _owner)
        {
            ID     = _owner.ID;
            AIType = EnumAIType.None;            
        }


        public void SetAIType(EnumAIType _ai_type)
        {
            AIType = _ai_type;
        }
    }

}