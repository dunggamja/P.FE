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
        // ��ų�� Ư�� �ý��ۿ� ���ؼ��� �ߵ� �����ϴ� ��� �ϴ� ���ص���.
        EnumSystem m_condition_system;
        int        m_condition_system_ski;

        public bool UseSkill(ISystem _system, IOwner _owner)
        {
            if (_system.SystemType != m_condition_system || _system.State != m_activate_system_state)
                return false;

            return true;
        }
    }
    
}

