using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    

    public sheet_item ItemSheet { get; private set; }
    public sheet_unit UnitSheet { get; private set; }
    public sheet_buff BuffSheet { get; private set; }
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

}



