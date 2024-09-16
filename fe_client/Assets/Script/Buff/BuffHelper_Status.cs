using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

public static partial class BuffHelper
{
    static Dictionary<EnumUnitStatus, IEnumerable<EnumBuffStatus>> s_buff_list_unit_status = new Dictionary<EnumUnitStatus, IEnumerable<EnumBuffStatus>>
        {  
            { EnumUnitStatus.Strength,   new[] { EnumBuffStatus.Unit_Strength   } },
            { EnumUnitStatus.Magic,      new[] { EnumBuffStatus.Unit_Magic      } },
            { EnumUnitStatus.Skill,      new[] { EnumBuffStatus.Unit_Skill      } },
            { EnumUnitStatus.Speed,      new[] { EnumBuffStatus.Unit_Speed      } },
            { EnumUnitStatus.Luck,       new[] { EnumBuffStatus.Unit_Luck       } },
            { EnumUnitStatus.Defense,    new[] { EnumBuffStatus.Unit_Defense    } },
            { EnumUnitStatus.Resistance, new[] { EnumBuffStatus.Unit_Resistance } },
            { EnumUnitStatus.Movement,   new[] { EnumBuffStatus.Unit_Movement   } },
            { EnumUnitStatus.Weight,     new[] { EnumBuffStatus.Unit_Weight     } },
        };

    public static IEnumerable<EnumBuffStatus> CollectBuff_UnitStatus(EnumUnitStatus _unit_status)
    {
        if (s_buff_list_unit_status.TryGetValue(_unit_status, out var buff_list))
        {
            return buff_list;
        }

        // empty list
        return new EnumBuffStatus[] {};
    }


     static Dictionary<EnumWeaponStatus, IEnumerable<EnumBuffStatus>> s_buff_list_weapon_status = new Dictionary<EnumWeaponStatus, IEnumerable<EnumBuffStatus>> 
        {  
            { EnumWeaponStatus.Might_Physics,  new[] { EnumBuffStatus.Weapon_Might          } },
            { EnumWeaponStatus.Might_Magic,    new[] { EnumBuffStatus.Weapon_Might          } },
            { EnumWeaponStatus.Hit,            new[] { EnumBuffStatus.Weapon_Hit            } },
            { EnumWeaponStatus.Critical,       new[] { EnumBuffStatus.Weapon_Critical       } },
            { EnumWeaponStatus.Weight,         new[] { EnumBuffStatus.Weapon_Weight         } },
            { EnumWeaponStatus.Dodge,          new[] { EnumBuffStatus.Weapon_Dodge          } },
            { EnumWeaponStatus.Dodge_Critical, new[] { EnumBuffStatus.Weapon_Dodge_Critical } },
            { EnumWeaponStatus.Range,          new[] { EnumBuffStatus.Weapon_Range          } },
            { EnumWeaponStatus.Range_Min,      new[] { EnumBuffStatus.Weapon_Range_Min      } },

        };

    public static IEnumerable<EnumBuffStatus> CollectBuff_WeaponStatus(EnumWeaponStatus _weapon_status)
    {
        if (s_buff_list_weapon_status.TryGetValue(_weapon_status, out var buff_list))
        {
            return buff_list;
        }

        // empty list
        return new EnumBuffStatus[] {};
    }
}