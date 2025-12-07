using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;
using Shapes;
using R3;

public static class ItemHelper
{

    public static bool VerifyItem_ExclusiveAttribute(int _item_kind, int _class_kind)
    {
		var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
		if (attributes == null)
			return true;

		bool has_exclusive_attribute = false;

		foreach (var attribute in attributes)
		{
			if (attribute.TYPE != (int)Battle.EnumItemAttribute.ExclusiveAttribute)
				continue;	

            has_exclusive_attribute = true;

			// 1개라도 일치하면 성공.
			if (DataManager.Instance.UnitSheet.HasClassAttribute_Unit(_class_kind, (Battle.EnumUnitAttribute)attribute.VALUE))
				return true;

		}

        // 전용 속성 제한 조건이 있고, 유닛 속성이 일치하지 않으면 실패 처리.
        return has_exclusive_attribute == false;
    }

    public static bool VerifyItem_ExclusiveClass(int _item_kind, int _class_kind)
    {
         // 클래스 제한 조건 체크.
		var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
		if (attributes == null)
			return true;

		bool has_exclusive_class = false;
		foreach (var attribute in attributes)
		{
			if (attribute.TYPE != (int)Battle.EnumItemAttribute.ExclusiveClass)
				continue;

			has_exclusive_class = true;

			// 클래스 중 1개라도 일치하면 성공.
			if (attribute.VALUE == _class_kind)
				return true;			
		}

		// 클래스 제한 조건이 있고, CLASS KIND 가 일치하지 않으면 실패 처리.
		return has_exclusive_class == false;
    }

    public static bool VerifyItem_ExclusiveCharacter(int _item_kind, Int64 _character_id)
    {
		var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
		if (attributes == null)
			return true;

		bool has_exclusive_character = false;

		foreach (var attribute in attributes)
		{
			if (attribute.TYPE != (int)Battle.EnumItemAttribute.ExclusiveCharacter)
				continue;

			has_exclusive_character = true;

			// 캐릭터 중 1개라도 일치하면 성공.
			if (attribute.VALUE == _character_id)
				return true;
		}

		// 전용 캐릭터 제한 조건이 있고, 캐릭터 ID 가 일치하지 않으면 실패 처리.
		return has_exclusive_character == false;
    }



	 // 아이템 사용 대상 체크.
	 public static EnumTargetType GetItemTargetType(int _item_kind)
	 {
		// 아이템 속성에 따로 정의되어 있는지 체크.
		var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
		if (attributes != null)
		{
			foreach (var attribute in attributes)
			{
				if (attribute.TYPE != (int)Battle.EnumItemAttribute.TargetType)
					continue;

				return (EnumTargetType)attribute.VALUE;
			}
		}

		var status = DataManager.Instance.ItemSheet.GetStatus(_item_kind);
		if (status == null)
			return EnumTargetType.None;

		// 무기의 경우, 지팡이는 아군대상, 그 외는 적군대상이 기본.
		if ((EnumItemType)status.TYPE == EnumItemType.Weapon)
		{
        var weapon_category  = (EnumWeaponCategory)status.CATEGORY;
        var is_wand          = weapon_category == EnumWeaponCategory.Wand;

        // 지팡이는  아군 대상, 그 외는 적군 대상이 기본.	
        return (is_wand) ? EnumTargetType.Ally : EnumTargetType.Enemy;			
		}

		// 그 외는 소유자 대상이 기본.
		return EnumTargetType.Owner;
	 }

    public static bool IsCommandAction(EnumItemActionType _action_type)
    {
        switch(_action_type)
        {
            // 아이템 사용인 경우 커맨드 처리.
            case EnumItemActionType.Consume:
               return true;
        }
        return false;
    }


    public static bool Verify_Item_Use(int _item_kind, Entity _entity)
    {
         if(_entity == null)
            return false;

         // 아이템 데이터 체크.
         var item_data = DataManager.Instance.ItemSheet.GetStatus(_item_kind);
         if (item_data == null)
            return false;

         // 숙련도 체크.
         var proficiency = _entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Proficiency);
         if (proficiency < item_data.PROFICIENCY)
            return false;


         if (item_data.TYPE == (int)EnumItemType.Weapon)
         {
            // 무기 클래스 속성 체크.
            if (_entity.StatusManager.Status.HasClassAttribute_Weapon((EnumWeaponCategory)item_data.CATEGORY) == false)
               return false;
         }

         // 전용 캐릭터 체크.
         if (VerifyItem_ExclusiveCharacter(_item_kind, _entity.ID) == false)
            return false;

         // 전용 속성 체크.
         if (VerifyItem_ExclusiveAttribute(_item_kind, _entity.StatusManager.Status.ClassKIND) == false)
            return false;

         // 전용 클래스 체크.
         if (VerifyItem_ExclusiveClass(_item_kind, _entity.StatusManager.Status.ClassKIND) == false)
            return false;
         
         return true;
    }

    public static void Verify_Item_Action(Int64 _item_id, Entity _entity, List<EnumItemActionType> _list_verify_action_type)
    {
      if (_entity == null)
        return;

      if (_list_verify_action_type == null)
        return;

      var item = _entity.Inventory.GetItem(_item_id);
      if (item == null)
        return;


      _list_verify_action_type.Clear();
      
      if (_entity.IsEnableAction(item, EnumItemActionType.Equip))   _list_verify_action_type.Add(EnumItemActionType.Equip);
      if (_entity.IsEnableAction(item, EnumItemActionType.Unequip)) _list_verify_action_type.Add(EnumItemActionType.Unequip);
      if (_entity.IsEnableAction(item, EnumItemActionType.Consume)) _list_verify_action_type.Add(EnumItemActionType.Consume);
      if (_entity.IsEnableAction(item, EnumItemActionType.Acquire)) _list_verify_action_type.Add(EnumItemActionType.Acquire);
      if (_entity.IsEnableAction(item, EnumItemActionType.Dispose)) _list_verify_action_type.Add(EnumItemActionType.Dispose);     
      
    }


    public static LocalizeKey GetLocalizeName(this EnumItemActionType _action_type)
    {
        var table = string.Empty;
        var key   = string.Empty;

        switch (_action_type)
        {
            case EnumItemActionType.Equip:
                table = "localization_base";
                key   = "ui_menu_item_equip";
                break;
            case EnumItemActionType.Unequip:
                table = "localization_base";
                key   = "ui_menu_item_unequip";
                break;
            case EnumItemActionType.Consume:
                table = "localization_base";
                key   = "ui_menu_item_consume";
                break;
            case EnumItemActionType.Acquire:
                table = "localization_base";
                key   = "ui_menu_item_acquire";
                break;
            case EnumItemActionType.Dispose:
                table = "localization_base";
                key   = "ui_menu_item_dispose";
                break;
        }
        return LocalizeKey.Create(table, key);
    }


    public static bool VerifyItem_Consume(EnumItemConsumeCategory _consume_category, Entity _entity)
    {
       if (_entity == null)
          return false;

       switch (_consume_category)
       {
         case EnumItemConsumeCategory.HPPotion:
            {
              // 체력 최대치 검사.
              var max_hp = _entity.StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max);
              var cur_hp = _entity.StatusManager.Status.GetPoint(EnumUnitPoint.HP);    
              if (max_hp <= cur_hp)
                  return false;
            }
            break;
       }

       return true;
    }


    public static int Calculate_Item_Heal(int _item_kind, Entity _entity)
    {
        var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
        if (attributes == null)
          return 0;

        using var list_heal       = ListPool<(int target, int value)>.AcquireWrapper();
        using var list_heal_bonus = ListPool<(int target, int value)>.AcquireWrapper();

        Item.CollectAttribute(_item_kind, EnumItemAttribute.Heal, list_heal.Value);
        Item.CollectAttribute(_item_kind, EnumItemAttribute.HealBonus_UnitStatus, list_heal_bonus.Value);

        int heal_value = 0;

        // 기본 회복량 추가.
        foreach (var (_, value) in list_heal.Value)
        {
            heal_value += value;
        }

        // 회복 보너스 추가. (유닛 스탯 기반)
        foreach (var (target, value) in list_heal_bonus.Value)
        {
            var status = _entity.StatusManager.GetBuffedUnitStatus((EnumUnitStatus)target);
            heal_value += (int)(status * Util.PERCENT(value));
        }


        return heal_value;
    }


    








}