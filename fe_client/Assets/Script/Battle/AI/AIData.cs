using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    // public abstract class AITarget : ITarget
    // {
    //     public Int64 OwnerEntityID { get; private set; } = 0;  


    //     public Int64       MainTargetID    { get; }        
    //     public List<Int64> AllTargetIDList { get; }
        
    // }

    // public abstract class AITargetPosition : ITargetPosition
    // {
    //     public Int64 OwnerEntityID { get; private set; } = 0;       


    //     public List<(int x, int y)> Positions {get;} 
    // }

    // public class AIData : IAIData
    // {
    //     public ITarget         Targets        { get; private set; } = new AITarget();

    //     public ITargetPosition TargetPosition { get; private set; } = new AITargetPosition();

    //     public (int x, int y)  BasePosition   { get; private set; } = (0, 0);
    // }


    public class AIDataManager : IAIDataManager
    {
        public Int64             ID     { get; private set; } = 0;
        public EnumAIType        AIType { get; private set; } = EnumAIType.None;
        // public IAIData           AIData { get; private set; } = new AIData();

        public void Initialize(Entity _owner)
        {
            ID     = _owner.ID;
            AIType = EnumAIType.None;            
        }


        public void SetAIType(EnumAIType _ai_type)
        {
            AIType = _ai_type;
        }

        public AIDataManager_IO Save()
        {
            return new AIDataManager_IO()
            {
                ID     = ID,
                AIType = AIType,
            };
        }
        
        public void Load(AIDataManager_IO _snapshot)
        {
            ID     = _snapshot.ID;
            AIType = _snapshot.AIType;
        }
    }

    public class AIDataManager_IO
    {
        public Int64             ID     { get; set; } = 0;
        public EnumAIType        AIType { get; set; } = EnumAIType.None;


    }

}