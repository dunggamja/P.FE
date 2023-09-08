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
            if (_param is SituationUpdatedEvent situation_updated)
            {
                // 상황이 변경되었을 때 사용할 스킬들.
                Skill.UseSkill(situation_updated.Situation, this);
            }
        }
    }
}
