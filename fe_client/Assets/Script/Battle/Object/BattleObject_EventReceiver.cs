using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleObject 
    {
        public void OnReceiveEvent(IEventParam _param)
        {
            if (_param is SkillUseEvent skill_use_param)
            {
                // 스킬 사용 이벤트.
                Skill.UseSkill(skill_use_param.System, this);
            }
        }
    }
}
