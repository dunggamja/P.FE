using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Battle
{
    public partial class Weapon
    {
        // 기본 무기 특효표
        static HashSet<(EnumWeaponAttribute, EnumUnitAttribute)> s_default_effectiveness = new HashSet<(EnumWeaponAttribute, EnumUnitAttribute)>()
        {
            // 활 => 비행
            (EnumWeaponAttribute.Bow, EnumUnitAttribute.Flyer),

            // 그 외 특효
            (EnumWeaponAttribute.KillInfantry  , EnumUnitAttribute.Infantry),
            (EnumWeaponAttribute.KillCavalry   , EnumUnitAttribute.Cavalry),
            (EnumWeaponAttribute.KillFlyer     , EnumUnitAttribute.Flyer),
            (EnumWeaponAttribute.KillUndead    , EnumUnitAttribute.Undead),
            (EnumWeaponAttribute.KillBeast     , EnumUnitAttribute.Beast),
            (EnumWeaponAttribute.KillLarge     , EnumUnitAttribute.Large),
            (EnumWeaponAttribute.KillHeavyArmor, EnumUnitAttribute.HeavyArmor), 
        };

        // <summary> 무기 특효관계 체크 </summary>
        public static bool Calculate_Effectiveness(Weapon _source, IStatus _target)
        {
            foreach ((var weapon_attribute, var target_attribute) in s_default_effectiveness)
            {
                // TODO: 무효 상성 체크?


                // 특효 상성 체크
                if (!_source.HasAttribute(weapon_attribute) || !_target.HasAttribute(target_attribute))
                    return false;

                return true;
            }

            return false;
        }

    }
}