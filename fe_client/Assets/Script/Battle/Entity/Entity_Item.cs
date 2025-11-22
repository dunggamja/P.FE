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

      public bool Verify_Weapon_Use(int _item_kind)
      {
         if (_item_kind == 0)
            return false;

         // 아이템 데이터 체크.
         var item_data =DataManager.Instance.ItemSheet.GetStatus(_item_kind);
         if (item_data == null)
            return false;

         // 숙련도 체크.
         var proficiency = StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Proficiency);
         if (proficiency < item_data.PROFICIENCY)
            return false;

         // 클래스 속성 체크.
         if (StatusManager.Status.HasClassAttribute_Weapon((EnumWeaponCategory)item_data.CATEGORY) == false)
            return false;

         
         return true;
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

         switch (_action)
         {
               case EnumItemActionType.Equip:   
               if (ProcessAction_Weapon_Equip(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Unequip: 
               if (ProcessAction_Weapon_Unequip(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Consume:
               if (ProcessAction_Weapon_Consume(_item))
               {
                  return true;
               }
               break;
               case EnumItemActionType.Acquire:
               if (ProcessAction_Weapon_Acquire(_item))
               {
                  return true;
               }
               break;

               case EnumItemActionType.Dispose:
               if (ProcessAction_Weapon_Dispose(_item))
               {
                  return true;
               }
               break;
         }

         return false;
      }


      bool IsEnableAction_Weapon_Equip(Item _item)
      {
         if (_item == null)
             return false;

         // 아이템 타입 체크.
         if (_item.ItemType != EnumItemType.Weapon)                
             return false;         

         // // 지팡이는 장착하는 무기가 아니다...;; <- TODO: 이것때문에 코드가 지저분... 지팡이를 무기에서 빼는게 맞긴 할거 같다.
         // if (_item.WeaponCategory == EnumWeaponCategory.Wand)
         //    return false;
         
         
         return Verify_Weapon_Use(_item.Kind);
      }

      bool IsEnableAction_Weapon_Unequip(Item _item)
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

      bool IsEnableAction_Weapon_Consume(Item _item)
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

         var    consume_category = (EnumItemConsumeCategory) item.ItemCategory;
         switch(consume_category)
         {
            // 회복 아이템 : 체력 최대치 체크.
            case EnumItemConsumeCategory.Potion:
            {
               var hp_cur = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
               var hp_max = StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max);
               if (hp_max <= hp_cur)
                  return false;
            }
            break;

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
         if (_item == null)
            return false;

         // 소유하지 않은 아이템.
         if (Inventory.GetItem(_item.ID) == null)
            return false;

         return true;
      }

      bool ProcessAction_Weapon_Equip(Item _item)
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

      bool ProcessAction_Weapon_Unequip(Item _item)
      {
         if (_item == null)
            return false;

         StatusManager.Unequip_Weapon(_item);

         ApplyBuff_Inventory();
               
         return true;
      }

      bool ProcessAction_Weapon_Consume(Item _item)
      {
         if (_item == null)
            return false;

         var item = Inventory.GetItem(_item.ID);
         if (item == null)
            return false;

         if (item.DecreaseCount() == false)
            return false;

         ApplyBuff_Item(_item.Kind, EnumItemActionType.Consume);

         return true;
      }

      bool ProcessAction_Weapon_Acquire(Item _item)
      {
         if (_item == null)
            return false;

         if (Inventory.AddItem(_item) == false)
            return false;

         ApplyBuff_Inventory();

         return true;
      }

      bool ProcessAction_Weapon_Dispose(Item _item)
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