using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Battle
{
    public partial class Weapon
    {
        const int COMPATIBILITY_ADVANTAGE_HIT      = 20;
        const int COMPATIBILITY_DISADVANTAGE_HIT   = -20;

        // 기본 무기 상성표 
        static HashSet<(EnumWeaponAttribute, EnumWeaponAttribute)> s_default_attribute_advantage = new HashSet<(EnumWeaponAttribute, EnumWeaponAttribute)>()
        {
            // 검 => 도끼
            (EnumWeaponAttribute.Sword, EnumWeaponAttribute.Axe),
            
            // 도끼 => 창
            (EnumWeaponAttribute.Axe, EnumWeaponAttribute.Lance),
            
            // 창 => 검
            (EnumWeaponAttribute.Lance, EnumWeaponAttribute.Sword),

            // 격투 => 활/지팡이/마법서/단검
            (EnumWeaponAttribute.MartialArts, EnumWeaponAttribute.Bow),
            (EnumWeaponAttribute.MartialArts, EnumWeaponAttribute.Wand),
            (EnumWeaponAttribute.MartialArts, EnumWeaponAttribute.Grimoire),
            (EnumWeaponAttribute.MartialArts, EnumWeaponAttribute.Dagger),
        };

        /// <summary> 유리한 무기인지 상성관계 체크. </summary>
        static bool HasAdvantage(IWeapon _source, IWeapon _target)
        {
            foreach ((var advantage, var disadvantage) in s_default_attribute_advantage)
            {
                if (_source.HasAttribute(advantage) && _target.HasAttribute(disadvantage))
                    return true;
            }

            return false;
        }

        /// <summary> 불리한 무기인지 상성관계 체크. </summary>
        static bool HasDisadvantage(IWeapon _source, IWeapon _target)
        {
            foreach ((var advantage, var disadvantage) in s_default_attribute_advantage)
            {
                if (_source.HasAttribute(disadvantage) && _target.HasAttribute(advantage))
                    return true;
            }

            return false;
        }


        //static Buff DEFAULT_COMPATIBILITY_BUFF = new Buff
        //{
        //    ID = Util.GenerateID(),
        //};

    }
}
