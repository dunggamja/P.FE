using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public partial class Entity
  {
     public (int min, int max) GetWeaponRange(Int64 _weapon_id = 0)
     {
        
        (int num, bool set)  range_max = (0, false); 
        (int num, bool set)  range_min = (0, false); 

        // 착용중인 무기 id.
        var equip_weapon_id = StatusManager.Weapon.ItemID;

        // 소유 중인 무기들의 목록.
        var list_weapon = ListPool<Item>.Acquire();
        
        // 소유 중인 무기들의 사거리 범위 체크.
        Inventory.CollectItemByType(ref list_weapon, EnumItemType.Weapon);

        foreach(var e in list_weapon)
        {
          // 특정 무기가 아니면 제외.
          if (e.ID != _weapon_id && _weapon_id != 0)
            continue;

          if (e.ID == 0)
            continue;


          // 착용중인 무기를 계산을 위해 바꿔줍세.
          StatusManager.Weapon.Equip(e.ID);

          var weapon_range_max = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range);
          var weapon_range_min = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range_Min);

          // 최대 사거리 체크.
          if (weapon_range_max > range_max.num || range_max.set == false)
          {
            range_max = (weapon_range_max, true);
          }

          // 최소 사거리 체크.
          if (weapon_range_min < range_min.num || range_min.set == false)
          {
            range_min = (weapon_range_min, true);
          }
        }

        // 무기 목록 반환.
        ListPool<Item>.Return(list_weapon);

        // 착용중인 무기를 원래대로 바꿔줍시다.
        StatusManager.Weapon.Equip(equip_weapon_id);

        return (range_min.num, range_max.num);
     }
    
  }
}