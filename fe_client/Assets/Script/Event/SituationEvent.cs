using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SituationUpdatedEvent : IEventParam
{
    public EnumSituationType  Situation   { get; private set; } 
    public ISystemParam       SystemParam { get; private set; }   

    public SituationUpdatedEvent(EnumSituationType _situation_type, ISystemParam _system_param)
    {
        Situation   = _situation_type;
        SystemParam = _system_param;
    }

}