using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

public static partial class BuffHelper
{
    private static readonly List<EnumBuffStatus> s_empty = new();

    static Dictionary<EnumUnitStatus, List<EnumBuffStatus>> s_buff_list_unit_status = new Dictionary<EnumUnitStatus, List<EnumBuffStatus>>
        {  
            { EnumUnitStatus.Strength,   new List<EnumBuffStatus> { EnumBuffStatus.Unit_Strength   } },
            { EnumUnitStatus.Magic,      new List<EnumBuffStatus> { EnumBuffStatus.Unit_Magic      } },
            { EnumUnitStatus.Skill,      new List<EnumBuffStatus> { EnumBuffStatus.Unit_Skill      } },
            { EnumUnitStatus.Speed,      new List<EnumBuffStatus> { EnumBuffStatus.Unit_Speed      } },
            { EnumUnitStatus.Luck,       new List<EnumBuffStatus> { EnumBuffStatus.Unit_Luck       } },
            { EnumUnitStatus.Defense,    new List<EnumBuffStatus> { EnumBuffStatus.Unit_Defense    } },
            { EnumUnitStatus.Resistance, new List<EnumBuffStatus> { EnumBuffStatus.Unit_Resistance } },
            { EnumUnitStatus.Movement,   new List<EnumBuffStatus> { EnumBuffStatus.Unit_Movement   } },
            { EnumUnitStatus.Weight,     new List<EnumBuffStatus> { EnumBuffStatus.Unit_Weight     } },
        };


    public static List<EnumBuffStatus> CollectBuff_UnitStatus(EnumUnitStatus _unit_status)
    {
        if (s_buff_list_unit_status.TryGetValue(_unit_status, out var buff_list))
        {
            return buff_list;
        }

        // empty list
        return s_empty;
    }


     static Dictionary<EnumWeaponStatus, List<EnumBuffStatus>> s_buff_list_weapon_status = new Dictionary<EnumWeaponStatus, List<EnumBuffStatus>> 
        {  
            { EnumWeaponStatus.Might_Physics,  new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Might          } },
            { EnumWeaponStatus.Might_Magic,    new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Might          } },
            { EnumWeaponStatus.Hit,            new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Hit            } },
            { EnumWeaponStatus.Critical,       new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Critical       } },
            { EnumWeaponStatus.Weight,         new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Weight         } },
            { EnumWeaponStatus.Dodge,          new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Dodge          } },
            { EnumWeaponStatus.Dodge_Critical, new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Dodge_Critical } },
            { EnumWeaponStatus.Range,          new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Range          } },
            { EnumWeaponStatus.Range_Min,      new List<EnumBuffStatus> { EnumBuffStatus.Weapon_Range_Min      } },

        };

    public static List<EnumBuffStatus> CollectBuff_WeaponStatus(EnumWeaponStatus _weapon_status)
    {
        if (s_buff_list_weapon_status.TryGetValue(_weapon_status, out var buff_list))
        {
            return buff_list;
        }

        // empty list
        return s_empty;
    }
}