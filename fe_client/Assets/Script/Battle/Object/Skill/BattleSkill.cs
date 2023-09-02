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
        // 
        // ��ų�� Ư�� �ý��ۿ� ���ؼ��� �ߵ� �����ϴ� ��� �ϴ� ���ص���.
        EnumSystem m_system_type;
        int        m_system_skill_timing;

        public bool UseSkill(ISystem _system, IOwner _owner)
        {
            // if (_system.SystemType != m_system_type || _system.SkillTiming != m_system_skill_timing)
                // return false;
            
            return true;
        }
    }
    
}

