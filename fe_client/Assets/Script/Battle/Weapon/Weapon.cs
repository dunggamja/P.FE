using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Weapon //: IPoolObject//, IWeapon
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

        // public bool HasAttribute(EnumItemAttribute _attribute_type, int _target = 0)
        // {
        //     var item_object  = ItemObject;
        //     if (item_object == null)
        //         return false;
        //     return item_object.HasAttribute(_attribute_type, _target);
        // }


        public void Reset()
        {
            OwnerID = 0;
            ItemID  = 0;
        }

        public Weapon_IO Save()
        {
            return new Weapon_IO()
            {
                OwnerID = OwnerID,
                ItemID  = ItemID
            };
        }

        public void Load(Weapon_IO _snapshot)
        {
            OwnerID = _snapshot.OwnerID;
            ItemID  = _snapshot.ItemID;
        }
    }

    public class Weapon_IO
    {
        public Int64 OwnerID { get; set; }
        public Int64 ItemID  { get; set; }

    }
}

