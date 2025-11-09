using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
      public void Equip_Weapon_Auto()
      {
         // 이미 무기 장착중이면 종료.
         if (StatusManager.Weapon.ItemID != 0)
            return;

         using var list_weapon = ListPool<Item>.AcquireWrapper();

         Inventory.CollectItemByType(list_weapon.Value, EnumItemType.Weapon);

         foreach(var e in list_weapon.Value)
         {
            if (IsEnableAction(e, EnumItemActionType.Equip))
            {
                ProcessAction(e, EnumItemActionType.Equip);
                break;
            }
         }
      }


      public bool IsEnableAction(Item _item, EnumItemActionType _action)
      {
         //var is_system_action = (owner == null);

         switch(_action)
         {
               case EnumItemActionType.Equip:   return IsEnableAction_Weapon_Equip(_item);
               case EnumItemActionType.Unequip: return IsEnableAction_Weapon_Unequip(_item);      
               case EnumItemActionType.Consume: return IsEnableAction_Weapon_Consume(_item);
               case EnumItemActionType.Acquire: return IsEnableAction_Weapon_Acquire(_item);
               case EnumItemActionType.Dispose: return IsEnableAction_Weapon_Dispose(_item);
         }

         return false;
      }

      public bool ProcessAction(Item _item, EnumItemActionType _action)
      {
         if (_item == null)
            return false;

         if (!IsEnableAction(_item, _action))
            return false;

         var is_success = false;

         switch (_action)
         {
               case EnumItemActionType.Equip:   is_success = ProcessAction_Weapon_Equip(_item);   break;
               case EnumItemActionType.Unequip: is_success = ProcessAction_Weapon_Unequip(_item); break;
               case EnumItemActionType.Consume: is_success = ProcessAction_Weapon_Consume(_item); break;
               case EnumItemActionType.Acquire: is_success = ProcessAction_Weapon_Acquire(_item); break;
               case EnumItemActionType.Dispose: is_success = ProcessAction_Weapon_Dispose(_item); break;
         }

         if (is_success)
         {
            OnRefreshBuff_Item(_item.Kind, _action);
            return true;
         }
         else
         {
            return false;
         }
      }


      bool IsEnableAction_Weapon_Equip(Item _item)
      {
         if (_item == null)
             return false;

         // 아이템 타입 체크.
         if (_item.ItemType != EnumItemType.Weapon)                
             return false;         
         
         // 소유자 무기 체크.
         var owner_weapon = StatusManager.Weapon; //as Weapon;
         if (owner_weapon == null || owner_weapon.ItemID == ID)
             return false;


         // 장착이 가능한 무기인지 체크.
         var weapon_category = (EnumWeaponCategory)_item.ItemCategory;
         if (StatusManager.Status.HasClassAttribute_Weapon(weapon_category) == false)
             return false;
         
         return true;
      }

      bool IsEnableAction_Weapon_Unequip(Item _item)
      {
         if (_item == null)
            return false;

         // 소유자 무기 체크.
         var owner_weapon = StatusManager.Weapon; //as Weapon;
         if (owner_weapon == null || owner_weapon.ItemID != _item.ID)
            return false;

                        
         return true;
      }

      bool IsEnableAction_Weapon_Consume(Item _item)
      {
         if (_item == null)
            return false;

         // 소유중인 아이템 체크.
         var item = Inventory.GetItem(_item.ID);
         if (item == null || item.CurCount <= 0)
            return false;

         if (item.ItemType != EnumItemType.Consumable)
            return false;

         var consume_category = (EnumItemConsumeCategory) item.ItemCategory;
         switch(consume_category)
         {
            // TODO: 소모품 카테고리별 체크.

         }



         return true;
      }

      bool IsEnableAction_Weapon_Acquire(Item _item)
      {
         if (_item == null)
            return false;

         // 이미 소유중.
         if (Inventory.GetItem(_item.ID) != null)
            return false;

         // 소지 갯수 체크.
         return (Inventory.Count < Inventory.MaxCount);
      }

      bool IsEnableAction_Weapon_Dispose(Item _item)
      {
         return true;
      }

      bool ProcessAction_Weapon_Equip(Item _item)
      {
         if (_item == null)
               return false;

         
         var owner_weapon  = StatusManager.Weapon; //as Weapon;
         if (owner_weapon != null)                                    
             owner_weapon.Equip(_item.ID);

         return true;
      }

      bool ProcessAction_Weapon_Unequip(Item _item)
      {
         var owner_weapon  = StatusManager.Weapon; //as Weapon;
         if (owner_weapon != null)                                    
             owner_weapon.Unequip();
               
         return true;
      }

      bool ProcessAction_Weapon_Consume(Item _item)
      {



         return true;
      }

      bool ProcessAction_Weapon_Acquire(Item _item)
      {


         return true;
      }

      bool ProcessAction_Weapon_Dispose(Item _item)
      {

         return true;
      }
    }


}