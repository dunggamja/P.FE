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
            // { EnumUnitStatus.Weight,     new List<EnumBuffStatus> { EnumBuffStatus.Unit_Weight     } },
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



    static public bool IsBuffUpdateSituation(EnumItemType _item_type, EnumItemActionType _action_type)
    {
        switch (_action_type)
            {
                case EnumItemActionType.Equip:   // 장비 장착 시 버프 적용.
                case EnumItemActionType.Unequip: // 장비 해제 시 버프 제거.
                case EnumItemActionType.Consume: // 소모품 사용 시 버프 적용.
                    return true;


                case EnumItemActionType.Acquire: // 액세서리, 잡화 획득 시 버프 적용.
                case EnumItemActionType.Dispose: // 액세서리, 잡화 버리기/매각 시 버프 제거.
                {
                    return _item_type switch {                        
                        EnumItemType.Accessory => true,
                        EnumItemType.Misc      => true,
                        _ => false
                    };
                }
            }

        return false;        
    }

    static public bool IsAddBuff(EnumItemActionType _action_type)
    {
        switch (_action_type)
        {
            case EnumItemActionType.Equip:   return true;
            case EnumItemActionType.Consume: return true;
            case EnumItemActionType.Acquire: return true;
            case EnumItemActionType.Unequip: return false;
            case EnumItemActionType.Dispose: return false;
        }
        return false;
    }



    static public EnumBuffContentsType GetContentsTypeByItemType(EnumItemType _item_type)
    {
        return _item_type switch {
            EnumItemType.Weapon     => EnumBuffContentsType.Item_Equipment,
            EnumItemType.Consumable => EnumBuffContentsType.Item_Consumable,
            EnumItemType.Accessory  => EnumBuffContentsType.Item_Accessory,
            EnumItemType.Misc       => EnumBuffContentsType.Item_Accessory,
            _ => EnumBuffContentsType.None
        };
    }
}