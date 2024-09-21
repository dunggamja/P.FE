using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class Sensor_Target_Score : ISensor
    {
        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null || owner_entity.StatusManager == null)
                return;

            // �̵� �Ÿ�.
            var move_distance = owner_entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);

            // ���� �����Ÿ�.
            var weapon_range_max = owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon).Max(e =>            
                owner_entity.StatusManager.GetBuffedWeaponStatus(new Weapon(_owner.ID, e.ID), EnumWeaponStatus.Range));
            // var weapon_range_min = owner_entity.Inventory.CollectItemByType(EnumItemType.Weapon).Max(e =>            
            //     owner_entity.StatusManager.GetBuffedWeaponStatus(new Weapon(_owner.ID, e.ID), EnumWeaponStatus.Range_Min));
            // var range_min = weapon_range_min;

            // ���� �����Ÿ�.
            var range_max = move_distance + weapon_range_max;


            // �����Ÿ� ����.
            var position_center    = owner_entity.Cell;
            var position_range_min = (position_center.x - range_max, position_center.y - range_max);
            var position_range_max = (position_center.x + range_max, position_center.y + range_max);


            

        }
    }
}