using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Battle
{
    public partial class Weapon
    {
        // 상성이 유리할 경우 적용할 값 (명중)
        public const int ADVANTAGE_HIT = 20;

        // 기본 무기 상성표 
        static HashSet<(EnumWeaponAttribute, EnumWeaponAttribute)> s_default_advantage = new HashSet<(EnumWeaponAttribute, EnumWeaponAttribute)>()
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

        /// <summary> 무기 상성관계 체크. </summary>
        public static EnumAdvantageState Calculate_Advantage(IWeapon _source, IWeapon _target)
        {
            foreach ((var advantage, var disadvantage) in s_default_advantage)
            {
                // 유리한 상성인지 체크
                if (_source.HasAttribute(advantage) && _target.HasAttribute(disadvantage))
                    return EnumAdvantageState.Advantage;

                // 불리한 상성인지 체크
                else if (_source.HasAttribute(disadvantage) && _target.HasAttribute(advantage))
                    return EnumAdvantageState.Disadvantage;

            }

            return EnumAdvantageState.None;
        }

       

    }
}
