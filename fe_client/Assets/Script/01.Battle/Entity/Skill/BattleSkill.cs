using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    // public class SkillUseEvent : IEventParam
    // {
    //     public EnumSituationType  Situation { get; private set; }

    //     public SkillUseEvent(EnumSituationType _situation)
    //     {
    //         Situation = _situation;
    //     }
    // }

    public class BattleSkill //: ISkill
    {
        // 
        // Ÿ�̹��� ���� ��  �ߵ�.
        // EnumSystem m_system_type;
        // int        m_system_skill_timing;

        public bool UseSkill(EnumSituationType _system_timing, IOwner _owner)
        {
            // if (_system.SystemType != m_system_type || _system.SkillTiming != m_system_skill_timing)
                // return false;
            
            return true;
        }

        public BattleSkill_IO Save()
        {
            return new BattleSkill_IO();
        }

        public void Load(BattleSkill_IO _snapshot)
        {
        }
    }

    public class BattleSkill_IO
    {
        
    }
    
}

