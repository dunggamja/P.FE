using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SkillUseEvent : IEventParam
    {
        public ISystem System { get; private set; }

        public SkillUseEvent(ISystem _system)
        {
            System = _system;
        }
    }

    public class BattleSkill : ISkill
    {
        // 스킬은 특정 시스템에 의해서만 발동 가능하다 라고 일단 정해두자.
        EnumSystem m_system_type;
        int        m_system_skill_timing;

        public bool UseSkill(ISystem _system, IOwner _owner)
        {
            if (_system.SystemType != m_system_type || _system.SkillTiming != m_system_skill_timing)
                return false;

            return true;
        }
    }
    
}

