using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Battle
{
    public partial class Weapon
    {
        // <summary> 무기 특효관계 체크 </summary>
        public static bool Calculate_Effectiveness(Weapon _source, UnitStatus _target)
        {
            if (_source == null || _target == null)
                return false;
            
            if (_source.ItemObject == null)
                return false;


            // 무기 특효 리스트 가져오기.
            using var list_collect = ListPool<(int target, int value)>.AcquireWrapper();
            
            Item.CollectAttribute(_source.ItemObject.Kind, EnumItemAttribute.KillUnitAttribute, list_collect.Value);

            // 타겟이 해당 되는지 체크.
            foreach ((var target, var _) in list_collect.Value)
            {
                if (_target.HasAttribute((EnumUnitAttribute)target))
                    return true;
            }


            return false;
        }

    }
}