using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public static class AIHelper
    {
        public static bool IsEnemy(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // TODO: 일단 FixedObject 제외.
            if (target.IsFixedObject)
                return false;

                      
            // 공격자와 타겟이 같은 진영인지 체크.
            return BattleSystemManager.Instance.IsEnemy(attacker.GetFaction(), target.GetFaction());
        }


        public static bool IsAlly(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // TODO: 일단 FixedObject 제외.
            if (target.IsFixedObject)
                return false;
            

            // 공격자와 타겟이 같은 진영인지 체크.
            var    is_ally  = BattleSystemManager.Instance.IsAlly(attacker.GetFaction(), target.GetFaction());
            return is_ally == true;
        }


        public static bool Verify_AI_Enemy(Int64 _attacker_id, Int64 _target_id)
        {
            // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.
            {
                // is_ally = rand() %2 ?;;;
                // is_ally = !is_ally;;;;
            }
            
            // 적이 맞는지 체크.
            if (AIHelper.IsEnemy(_attacker_id, _target_id) == false)
                return false;

            // 공격자 엔티티 체크.
            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            if (attacker == null)
                return false;

            // TODO: 시나리오 태그 체크.
            {
                var tag_owner_info   = TAG_INFO.Create(EnumTagType.Entity, _attacker_id);
                var tag_target_info  = TAG_INFO.Create(EnumTagType.Entity, _target_id);

                var is_focus_target  = TagManager.Instance.IsExistTagRelation(tag_owner_info, tag_target_info, EnumTagAttributeType.TARGET_FOCUS);
                var is_ignore_target = TagManager.Instance.IsExistTagRelation(tag_owner_info, tag_target_info, EnumTagAttributeType.TARGET_IGNORE);

                // 무시 태그만 셋팅 되어있다면 타겟팅에서 제외.
                if (is_ignore_target && !is_focus_target)
                    return false;
            }


            

            return true;
        }
    }
}