using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SituationUpdatedEvent : IEventParam
    {
        public EnumSituationType  Situation   { get; private set; } 
        // public ISystemParam       SystemParam { get; private set; }   

        public SituationUpdatedEvent(EnumSituationType _situation_type)
        {
            Situation   = _situation_type;
            // SystemParam = _system_param;
        }
    }

    public class AIUpdateEvent : IEventParam
    {
        public float TopScore          { get; private set; } = 0;
        public Int64 TopScore_EntityID { get; private set; } = 0;

        public void TryTopScore(Int64 _entity_id, float _score)
        {
            if (TopScore < _score)
            {
                TopScore          = _score;
                TopScore_EntityID = _entity_id;
            }
        }
    }
}

