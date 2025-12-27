using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public static class AIHelper
    {
        public static bool Verify_IsEnemy(Int64 _attacker_id, Int64 _target_id)
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


        public static bool Verify_IsAlly(Int64 _attacker_id, Int64 _target_id)
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


        // public static bool Verify_AI_Enemy(Int64 _attacker_id, Int64 _target_id)
        // {
        //     // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.
        //     {
        //         // is_ally = rand() %2 ?;;;
        //         // is_ally = !is_ally;;;;
        //     }
            
        //     // 적이 맞는지 체크.
        //     if (AIHelper.IsEnemy(_attacker_id, _target_id) == false)
        //         return false;

        //     // // 공격자 엔티티 체크.
        //     // var attacker = EntityManager.Instance.GetEntity(_attacker_id);
        //     // if (attacker == null)
        //     //     return false;

        //     // var target = EntityManager.Instance.GetEntity(_target_id);
        //     // if (target == null)
        //     //     return false;

        //     // // 시나리오 태그 체크. (포커싱 또는 포커싱 무시)
        //     // {
        //     //     var target_contain = TagManager.Instance.IsExistTagRelation(attacker, target,  EnumTagAttributeType.TARGET_FOCUS) 
        //     //                       || TagManager.Instance.IsExistTagRelation(attacker, target,  EnumTagAttributeType.TARGET_CONTAIN);
        //     //     var target_ignore  = TagManager.Instance.IsExistTagRelation(attacker, target,  EnumTagAttributeType.TARGET_IGNORE);

        //     //     // 무시 태그만 셋팅 되어있다면 타겟팅에서 제외.
        //     //     if (target_contain == false)
        //     //     {
        //     //         if (target_ignore)
        //     //             return false;
        //     //     }
        //     // }
            

        //     return true;
        // }

        public static bool Verify_Target_Ignore(Entity _attacker, Entity _target)
        {
            if (_attacker == null || _target == null)
                return true;

            if (_attacker.IsDead || _target.IsDead)
                return true;

            // 태그 체크. (포커싱 또는 포커싱 무시)

            // 타겟 포커싱/포함.
            var target_contain = TagManager.Instance.IsExistTagRelation(_attacker, _target,  EnumTagAttributeType.TARGET_FOCUS) 
                              || TagManager.Instance.IsExistTagRelation(_attacker, _target,  EnumTagAttributeType.TARGET_CONTAIN);

            // 타겟 무시.
            var target_ignore  = TagManager.Instance.IsExistTagRelation(_attacker, _target,  EnumTagAttributeType.TARGET_IGNORE);

            // 무시 태그만 셋팅 되어있다면 타겟팅에서 제외.
            return target_ignore && !target_contain;
        }


        public static bool Verify_Target_Focus(Entity _attacker, Entity _target)
        {
            if (_attacker == null || _target == null)
                return false;

            if (_attacker.IsDead || _target.IsDead)
                return false;

            return TagManager.Instance.IsExistTagRelation(_attacker, _target,  EnumTagAttributeType.TARGET_FOCUS);
        }

        
    }
}