using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Weapon : IWeapon, IPoolObject
    {
        public Int64 OwnerID { get; private set; } = 0;
        public Int64 ItemID  { get; private set; } = 0;

        public Item  ItemObject
        {
            get
            {
                var owner  = EntityManager.Instance.GetEntity(OwnerID);
                if (owner == null)
                    return null;

                return owner.Inventory.GetItem(ItemID);
            }
        }

        public Weapon(Int64 _owner_id, Int64 _item_id)
        {
            OwnerID = _owner_id;
            ItemID  = _item_id;
        }

        public void Equip(Int64 _item_id)
        {
            ItemID = _item_id;
        }

        public void Unequip()
        {
            ItemID = 0;
        }


        public int  GetStatus(EnumWeaponStatus _status_type)
        {
            var item_object  = ItemObject;
            if (item_object == null)
                return 0;

            return item_object.GetWeaponStatus(_status_type);
        }

        public bool HasAttribute(EnumWeaponAttribute _attribute_type)
        {
            var item_object  = ItemObject;
            if (item_object == null)
                return false;

            return item_object.HasWeaponAttribute(_attribute_type);
        }

        public void Reset()
        {
            OwnerID = 0;
            ItemID  = 0;
        }
    }
}

