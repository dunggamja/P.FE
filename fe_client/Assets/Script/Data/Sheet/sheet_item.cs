using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

[Serializable]
public class sheet_item_status
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       TYPE;
	public Int32       CATEGORY;
	public Int32       PHYSICS;
	public Int32       MAGIC;
	public Int32       HIT;
	public Int32       CRITICAL;
	public Int32       WEIGHT;
	public Int32       PROFICIENCY;
	public Int32       RANGE;
	public Int32       RANGE_MIN;
	public Int32       MAX_COUNT;
	public Int32       DODGE;
	public Int32       DODGE_CRITICAL;
	public Int32       PRICE;
}

[Serializable]
public class sheet_item_attribute
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       TYPE;
	public Int32       TARGET;
	public Int32       VALUE;
}

[Serializable]
public class sheet_item_skill
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       SKILL;
}

[Serializable]
public class sheet_item_localize
{
	public Int32       KIND;
	public string      MEMO;
	public string      NAME;
	public string      DESC;
}



[ExcelAsset]
public class sheet_item : ScriptableObject
{
	public List<sheet_item_status>    status;
	public List<sheet_item_attribute> attribute;
	public List<sheet_item_skill>     skill;
	public List<sheet_item_localize>  localize;


	private Dictionary<Int32, sheet_item_status>          m_cache_status    = new ();
	private Dictionary<Int32, List<sheet_item_attribute>> m_cache_attribute = new ();
	private Dictionary<Int32, List<sheet_item_skill>>     m_cache_skill     = new ();
	private Dictionary<Int32, (string table, string key)>   m_cache_localize_name  = new ();
	private Dictionary<Int32, (string table, string key)>   m_cache_localize_desc  = new ();


	public void Initialize()
	{
		foreach (var item in status)
		{
			m_cache_status.Add(item.KIND, item);
		}

		foreach (var item in attribute)
		{
			if (m_cache_attribute.TryGetValue(item.KIND, out var list_attribute) == false)
			{
				list_attribute = new List<sheet_item_attribute>();
				m_cache_attribute.Add(item.KIND, list_attribute);
			}

			list_attribute.Add(item);
		}

		foreach (var item in skill)
		{
			if (m_cache_skill.TryGetValue(item.KIND, out var list_skill) == false)
			{
				list_skill = new List<sheet_item_skill>();
				m_cache_skill.Add(item.KIND, list_skill);
			}

			list_skill.Add(item);
		}

		foreach (var item in localize)
		{
			var name_split = Util.SplitText(item.NAME, Data_Const.SHEET_LOCALIZATION_SEPERATOR);
			var desc_split = Util.SplitText(item.DESC, Data_Const.SHEET_LOCALIZATION_SEPERATOR);


			if (name_split.Count >= 2)
			 	m_cache_localize_name.Add(item.KIND, (name_split[0], name_split[1]));

			if (desc_split.Count >= 2)
			 	m_cache_localize_desc.Add(item.KIND, (desc_split[0], desc_split[1]));
		}		
	}


	public sheet_item_status GetStatus(Int32 kind)
	{
		if (m_cache_status.TryGetValue(kind, out var result))
		{
			return result;
		}

		return null;
	}

	public List<sheet_item_attribute> GetAttribute(Int32 kind)
	{
		if (m_cache_attribute.TryGetValue(kind, out var result))
		{
			return result;
		}

		return null;
	}

	public bool HasAttribute(Int32 kind, EnumItemAttribute _attribute)
	{
		var attributes = GetAttribute(kind);
		if (attributes == null)
			return false;

		return attributes.FindIndex(x => x.TYPE == (int)_attribute) != -1;
	}

	public List<sheet_item_skill> GetSkill(Int32 kind)
	{
		if (m_cache_skill.TryGetValue(kind, out var result))
		{
			return result;
		}

		return null;
	}

	


	public (string table, string key) GetLocalizeName(Int32 kind)
	{
		if (m_cache_localize_name.TryGetValue(kind, out var result))
		{
			return result;
		}

		return (string.Empty, string.Empty);
	}


	public (string table, string key) GetLocalizeDesc(Int32 kind)
	{
		if (m_cache_localize_desc.TryGetValue(kind, out var result))
		{
			return result;
		}

		return (string.Empty, string.Empty);
	}

	public void ClearCache()
	{
		m_cache_status.Clear();
		m_cache_attribute.Clear();
		m_cache_skill.Clear();
		m_cache_localize_name.Clear();
		m_cache_localize_desc.Clear();
	}

}
