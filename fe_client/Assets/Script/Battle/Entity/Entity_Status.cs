using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public partial class Entity
  {
     public (int min, int max) GetWeaponRange(bool _equipped_only = false)
     {
        
        (int num, bool set)  range_max = (0, false); 
        (int num, bool set)  range_min = (0, false); 

        // �������� ���� id.
        var equip_weapon_id = StatusManager.Weapon.ItemID;

        // ���� ���� ������� ���.
        var list_weapon = ListPool<Item>.Acquire();
        
        // ���� ���� ������� ��Ÿ� ���� üũ.
        Inventory.CollectItemByType(ref list_weapon, EnumItemType.Weapon);

        foreach(var e in list_weapon)
        {
          // �������� ���Ⱑ �ƴϸ� �����ϴ� ���.
          if (e.ID != equip_weapon_id && _equipped_only)
            continue;

          if (e.ID == 0)
            continue;


          // �������� ���⸦ ����� ���� �ٲ��ݼ�.
          StatusManager.Weapon.Equip(e.ID);

          var weapon_range_max = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range);
          var weapon_range_min = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range_Min);

          // �ִ� ��Ÿ� üũ.
          if (weapon_range_max > range_max.num || range_max.set == false)
          {
            range_max = (weapon_range_max, true);
          }

          // �ּ� ��Ÿ� üũ.
          if (weapon_range_min < range_min.num || range_min.set == false)
          {
            range_min = (weapon_range_min, true);
          }
        }

        // ���� ��� ��ȯ.
        ListPool<Item>.Return(list_weapon);

        // �������� ���⸦ ������� �ٲ��ݽô�.
        StatusManager.Weapon.Equip(equip_weapon_id);

        return (range_min.num, range_max.num);
     }
    
  }
}