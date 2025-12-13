using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public static class AIHelper
    {
        public static bool IsEnemy_Targetable(Int64 _attacker_id, Int64 _target_id)
        {
            // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.
            {
                // is_ally = rand() %2 ?;;;
                // is_ally = !is_ally;;;;
            }


            // 적 진영인지 체크.
            if (CombatHelper.IsEnemy(_attacker_id, _target_id) == false)
                return false;

            // 공격자 엔티티 체크.
            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            if (attacker == null)
                return false;

            // TODO: 시나리오 태그 체크.
            {                
                using var list_label = ListPool<Label>.AcquireWrapper();
                LabelManager.Instance.Collect_Owner(_target_id, list_label.Value);

                bool is_focus_target  = false;
                bool is_ignore_target = false;

                foreach(var label in list_label.Value)
                {
                    // 공격자에게 적용이 되는 태그인지 체크.
                    if (label.Verify_Target(attacker) == false)
                        continue;

                    is_focus_target  = label.HasAttribute(EnumLabelAttribute.FocusTarget);
                    is_ignore_target = label.HasAttribute(EnumLabelAttribute.IgnoreTarget);
                }

                // 무시 태그만 셋팅되어있다면 타겟팅에서 제외.
                if (is_ignore_target && !is_focus_target)
                    return false;
            }


            

            return true;
        }
    }
}