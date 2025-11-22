using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;


[Serializable]
public class sheet_map_setting_entity
{
	public Int64       ID;
	public string      MEMO;
	public Int32       FACTION;
	public Int32       STATUS_KIND;
	public Int32       ITEM_KIND;
	public Int32       LOCALIZATION_KIND;
}


[Serializable]
public class sheet_map_setting_status
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       LEVEL;
	public Int32       HP;
	public Int32       STRENGTH;
	public Int32       MAGIC;
	public Int32       SKILL;
	public Int32       SPEED;
	public Int32       LUCK;
	public Int32       PROFICIENCY;
	public Int32       DEFENSE;
	public Int32       RESISTANCE;
	public Int32       MOVEMENT;	
	public Int32       CLASS;

   // private HashSet<EnumUnitAttribute>      m_cache_attributes_unit = null;
   // private HashSet<EnumPathOwnerAttribute> m_cache_attributes_path = null;

   // private void CacheUnitAttributes()
   // {
   //    m_cache_attributes_unit = new HashSet<EnumUnitAttribute>(); 
   //    foreach (var e in Util.SplitText<EnumUnitAttribute>(UNIT_ATTRIBUTE, Data_Const.SHEET_SEPERATOR))
   //       m_cache_attributes_unit.Add(e);
   // }

   // private void CachePathAttributes()
   // {
   //    m_cache_attributes_path = new HashSet<EnumPathOwnerAttribute>(); 
   //    foreach (var e in Util.SplitText<EnumPathOwnerAttribute>(PATH_ATTRIBUTE, Data_Const.SHEET_SEPERATOR))
   //       m_cache_attributes_path.Add(e);
   // }

   // public  bool HasUnitAttribute(EnumUnitAttribute _attribute)
   // {
   //    if (m_cache_attributes_unit == null)
   //       CacheUnitAttributes();

   //    return (m_cache_attributes_unit != null && m_cache_attributes_unit.Contains(_attribute));
   // }

	// public  HashSet<EnumUnitAttribute> GetUnitAttributes()
	// {
	// 	if (m_cache_attributes_unit == null)
	// 		CacheUnitAttributes();

	// 	return m_cache_attributes_unit;
	// }

	// public  HashSet<EnumPathOwnerAttribute> GetPathAttributes()
	// {
	// 	if (m_cache_attributes_path == null)
	// 		CachePathAttributes();

	// 	return m_cache_attributes_path;
	// }



   // public  bool HasPathAttribute(EnumPathOwnerAttribute _attribute)
   // {
   //    if (m_cache_attributes_path == null)
   //       CachePathAttributes();

   //    return (m_cache_attributes_path != null && m_cache_attributes_path.Contains(_attribute));
   // }
}

[Serializable]
public class sheet_map_setting_item
{
	public Int32       KIND;
	public string      MEMO;
	public int         ITEM_KIND;
   public int         VALUE;
	public int         DROP;
}


[Serializable]
public class sheet_map_setting_localize
{
	public Int32       KIND;
	public string      MEMO;
	public string      NAME;
	public string      DESC;
}





[ExcelAsset]
public class sheet_map_setting : ScriptableObject
{
	public List<sheet_map_setting_entity>    entity;
	public List<sheet_map_setting_status>    status;
	public List<sheet_map_setting_item>      item;
	public List<sheet_map_setting_localize>  localization	;


	private Dictionary<Int32, (string table, string key)>   m_cache_localize_name  = new ();
	private Dictionary<Int32, (string table, string key)>   m_cache_localize_desc  = new ();


	public void Initialize()
	{
		foreach (var item in localization)
		{
			var name_split = Util.SplitText(item.NAME, Data_Const.SHEET_LOCALIZATION_SEPERATOR);
			var desc_split = Util.SplitText(item.DESC, Data_Const.SHEET_LOCALIZATION_SEPERATOR);

			if (name_split.Count >= 2)
			 	m_cache_localize_name.Add(item.KIND, (name_split[0], name_split[1]));

			if (desc_split.Count >= 2)
			 	m_cache_localize_desc.Add(item.KIND, (desc_split[0], desc_split[1]));
		}
	}


	public sheet_map_setting_entity GetEntity(Int64 _id)
	{
		if (entity == null)
			return null;

		foreach(var e in entity)
		{
			if (e.ID == _id)
				return e;
		}
		return null;
	}



	public sheet_map_setting_status GetStatus(Int32 _KIND)
	{
		if (status == null)
			return null;

		foreach(var e in status)
		{
			if (e.KIND == _KIND)
				return e;
		}
		return null;
	}

	public List<sheet_map_setting_item> GetItem(Int32 _KIND)
	{
		var list_result = new List<sheet_map_setting_item>();

		foreach(var e in item)
		{
			if (e.KIND == _KIND)
				list_result.Add(e);
		}

		return list_result;	
	}

	// public sheet_map_setting_asset GetAsset(Int32 _KIND)
	// {
	// 	if (asset == null)
	// 		return null;

	// 	foreach(var e in asset)
	// 	{
	// 		if (e.KIND == _KIND)
	// 			return e;
	// 	}
	// 	return null;
	// }


	public sheet_map_setting_status GetStatus_EntityID(Int64 _entity_id)
	{
		var sheet = GetEntity(_entity_id);
		if (sheet == null)
			return null;

		return GetStatus(sheet.STATUS_KIND);
	}

	public List<sheet_map_setting_item> GetItem_EntityID(Int64 _entity_id)
	{
		var sheet = GetEntity(_entity_id);
		if (sheet == null)
			return null;

		return GetItem(sheet.ITEM_KIND);
	}

	// public sheet_map_setting_asset GetAsset_EntityID(Int64 _entity_id)
	// {
	// 	var sheet = GetEntity(_entity_id);
	// 	if (sheet == null)
	// 		return null;

	// 	return GetAsset(sheet.ASSET_KIND);
	// }

}
