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

        // 소유 중인 무기 id.
        var equip_weapon_id = StatusManager.Weapon.ItemID;

        // 소유 중인 무기 목록.
        using var list_weapon = ListPool<Item>.AcquireWrapper();

        // 소유 중인 무기 목록 추출.
        Inventory.CollectItemByType(list_weapon.Value, EnumItemType.Weapon);

        foreach(var e in list_weapon.Value)
        {
          // 소유 중인 무기가 아니면 제외.
          if (e.ID != _weapon_id && _weapon_id != 0)
            continue;

          if (e.ID == 0)
            continue;


          // 소유 중인 무기 착용.
          StatusManager.Weapon.Equip(e.ID);

          var weapon_range_max = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range);
          var weapon_range_min = StatusManager.GetBuffedWeaponStatus(StatusManager.Weapon, EnumWeaponStatus.Range_Min);

          // 최대 사정거리 체크.
          if (weapon_range_max > range_max.num || range_max.set == false)
          {
            range_max = (weapon_range_max, true);
          }

          // 최소 사정거리 체크.
          if (weapon_range_min < range_min.num || range_min.set == false)
          {
            range_min = (weapon_range_min, true);
          }
        }

        // 소유 중인 무기 목록 반환.
        // ListPool<Item>.Return(list_weapon.Value);

        // 소유 중인 무기 착용.
        StatusManager.Weapon.Equip(equip_weapon_id);

        return (range_min.num, range_max.num);
     }


    public void ApplyDamage(int _damage)
    {
        // 데미지 체크.
        _damage = Math.Max(0, _damage);

        
        var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
        var new_hp = cur_hp - _damage;

        // 체력 설정.
        SetPoint(EnumUnitPoint.HP, new_hp);

        // 데미지 로그 추가.
        // BattleLogManager.Instance.AddLog(EnumBattleLogType.Damage, ID, _damage);
    }

    public void ApplyHeal(int _heal)
    {
        if (_heal <= 0)
            return;

        var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
        var new_hp = cur_hp + _heal;

        SetPoint(EnumUnitPoint.HP, new_hp);
    }

    void SetPoint(EnumUnitPoint _point_type, int _value)
    {
        // 데미지 체크.
        var new_value = CorrectPoint(_point_type, _value);

        // 데미지 설정.
        StatusManager.Status.SetPoint(_point_type, new_value);
    }

    int CorrectPoint(EnumUnitPoint _point_type, int _value)
    {
        var min_value = 0;
        var max_value = _value;

        switch (_point_type)
        {
            case EnumUnitPoint.HP:
                min_value = 0;
                max_value = StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max);//, _is_plan);
                break;
        }

        return Math.Clamp(_value, min_value, max_value);
    }
    
  }
}