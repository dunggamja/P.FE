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

         Inventory.CollectItem_Weapon_Available(list_weapon.Value, this);

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
               case EnumItemActionType.Equip:   return IsEnableAction_Item_Equip(_item);
               case EnumItemActionType.Unequip: return IsEnableAction_Item_Unequip(_item);      
               case EnumItemActionType.Consume: return IsEnableAction_Item_Consume(_item);
               case EnumItemActionType.Acquire: return IsEnableAction_Item_Acquire(_item);
               case EnumItemActionType.Dispose: return IsEnableAction_Item_Dispose(_item);
         }

         return false;
      }

      public bool ProcessAction(Item _item, EnumItemActionType _action)
      {
         if (_item == null)
            return false;

         if (!IsEnableAction(_item, _action))
            return false;

         switch (_action)
         {
               case EnumItemActionType.Equip:   
               if (ProcessAction_Item_Equip(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Unequip: 
               if (ProcessAction_Item_Unequip(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Consume:
               if (ProcessAction_Item_Consume(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Acquire:
               if (ProcessAction_Item_Acquire(_item))
               {
                  return true;
               }
               break;

               case EnumItemActionType.Dispose:
               if (ProcessAction_Item_Dispose(_item))
               {
                  return true;
               }
               break;
         }

         return false;
      }


      bool IsEnableAction_Item_Equip(Item _item)
      {
         if (_item == null)
             return false;

         // 아이템 타입 체크.
         if (_item.ItemType != EnumItemType.Weapon)                
             return false;         

         // 아이템 사용 체크.
         return ItemHelper.Verify_Item_Use(_item.Kind, this);
      }

      bool IsEnableAction_Item_Unequip(Item _item)
      {
         if (_item == null)
            return false;

         if (_item.WeaponCategory == EnumWeaponCategory.Wand)
         {
            // 소유자 지팡이 체크.
            var owner_wand = StatusManager.Wand; //as Weapon;
            if (owner_wand == null || owner_wand.ItemID != _item.ID)
               return false;
         }
         else
         {            
            // 소유자 무기 체크.
            var owner_weapon = StatusManager.Weapon; //as Weapon;
            if (owner_weapon == null || owner_weapon.ItemID != _item.ID)
               return false;
         }


         
                        
         return true;
      }

      bool IsEnableAction_Item_Consume(Item _item)
      {
         if (_item == null)
            return false;

         // 소유중인 아이템 수량 체크.
         var item = Inventory.GetItem(_item.ID);
         if (item == null || item.CurCount <= 0)
            return false;

         // 소모 가능한 아이템인지 체크.
         if (item.ItemType != EnumItemType.Consumable)
            return false;

   
         if (ItemHelper.Verify_Item_Use(_item.Kind, this) == false)
            return false;

         var consume_category = (EnumItemConsumeCategory) item.ItemCategory;
         if (ItemHelper.VerifyItem_Consume(consume_category, this) == false)
            return false;

         return true;
      }

      bool IsEnableAction_Item_Acquire(Item _item)
      {
         if (_item == null)
            return false;

         // 이미 소유중.
         if (Inventory.GetItem(_item.ID) != null)
            return false;

         // 소지 갯수 체크.
         return (Inventory.Count < Inventory.MaxCount);
      }

      bool IsEnableAction_Item_Dispose(Item _item)
      {
         if (_item == null)
            return false;

         // 소유하지 않은 아이템.
         if (Inventory.GetItem(_item.ID) == null)
            return false;

         return true;
      }

      bool ProcessAction_Item_Equip(Item _item)
      {
         if (_item == null)
               return false;

         StatusManager.Equip_Weapon(_item);

         ApplyBuff_Inventory();

         // 장착한 아이템이 첫번째 순서로 오도록 변경.
         if (Inventory.GetItemOrder(_item.ID) > 0)
             Inventory.SetItemOrder(_item.ID, 0);


         return true;
      }

      bool ProcessAction_Item_Unequip(Item _item)
      {
         if (_item == null)
            return false;

         StatusManager.Unequip_Weapon(_item);

         ApplyBuff_Inventory();
               
         return true;
      }

      bool ProcessAction_Item_Consume(Item _item)
      {
         if (_item == null)
            return false;

         var item = Inventory.GetItem(_item.ID);
         if (item == null)
            return false;

         if (item.DecreaseCount() == false)
            return false;

         // 버프 적용
         ApplyBuff_Item(_item.Kind, EnumItemActionType.Consume);

         // 회복 적용
         ApplyConsume_Item(_item.Kind);

         return true;
      }

      bool ProcessAction_Item_Acquire(Item _item)
      {
         if (_item == null)
            return false;

         if (Inventory.AddItem(_item) == false)
            return false;

         ApplyBuff_Inventory();

         return true;
      }

      bool ProcessAction_Item_Dispose(Item _item)
      {
         if (_item == null)
            return false;

         
         if (Inventory.RemoveItem(_item.ID) == false)
            return false;

         StatusManager.Unequip_Weapon(_item);

         ApplyBuff_Inventory();

         return true;
      }
    }


}