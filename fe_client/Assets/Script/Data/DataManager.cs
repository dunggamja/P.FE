using Battle;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
// using Sirenix.OdinInspector;

public class DataManager : Singleton<DataManager>
{
    

    public sheet_item ItemSheet { get; private set; }
    public sheet_unit UnitSheet { get; private set; }
    public sheet_buff BuffSheet { get; private set; }

	//  public sheet_map_setting MapSettingSheet { get; private set; }


	 // 맵 설정과 관련된 데이터. (맵파일과 연관되어있다.)
	 private sheet_map_setting         m_map_setting     = null;
	 private sheet_map_faction_setting m_faction_setting = null;

    protected override void Init()
    {
        base.Init();
    }


    public async UniTask LoadSheetData()
    {
        ItemSheet = await AssetManager.Instance.LoadAssetAsync<sheet_item>(AssetName.SHEET_ITEM);
        UnitSheet = await AssetManager.Instance.LoadAssetAsync<sheet_unit>(AssetName.SHEET_UNIT);
        BuffSheet = await AssetManager.Instance.LoadAssetAsync<sheet_buff>(AssetName.SHEET_BUFF);


        ItemSheet.Initialize();     
        UnitSheet.Initialize();
        BuffSheet.Initialize();
    }

    public bool VerifyItem_ExclusiveAttribute(int _item_kind, int _class_kind)
    {
		var attributes = ItemSheet.GetAttribute(_item_kind);
		if (attributes == null)
			return false;

		bool has_exclusive_attribute = false;

		foreach (var attribute in attributes)
		{
			if (attribute.TYPE != (int)Battle.EnumItemAttribute.ExclusiveAttribute)
				continue;	

            has_exclusive_attribute = true;

			// 1개라도 일치하면 성공.
			if (UnitSheet.HasClassAttribute_Unit(_class_kind, (Battle.EnumUnitAttribute)attribute.VALUE))
				return true;

		}

        // 전용 속성 제한 조건이 있고, 유닛 속성이 일치하지 않으면 실패 처리.
        return has_exclusive_attribute == false;
    }

    public bool VerifyItem_ExclusiveClass(int _item_kind, int _class_kind)
    {
         // 클래스 제한 조건 체크.
		var attributes = ItemSheet.GetAttribute(_item_kind);
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

    public bool VerifyItem_ExclusiveCharacter(int _item_kind, int _character_id)
    {
		var attributes = ItemSheet.GetAttribute(_item_kind);
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
	 public EnumTargetType GetItemTargetType(int _item_kind)
	 {
		// 아이템 속성에 따로 정의되어 있는지 체크.
		var attributes = ItemSheet.GetAttribute(_item_kind);
		if (attributes != null)
		{
			foreach (var attribute in attributes)
			{
				if (attribute.TYPE != (int)Battle.EnumItemAttribute.TargetType)
					continue;

				return (EnumTargetType)attribute.VALUE;
			}
		}

		var status = ItemSheet.GetStatus(_item_kind);
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

}



