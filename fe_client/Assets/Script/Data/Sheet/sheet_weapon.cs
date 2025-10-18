using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class sheet_weapon_status
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       PHYSICS;
	public Int32       MAGIC;
	public Int32       HIT;
	public Int32       CRITICAL;
	public Int32       WEIGHT;
	public Int32       DODGE;
	public Int32       DODGE_CRITICAL;
	public Int32       RANGE;
	public Int32       RANGE_MIN;
}

[Serializable]
public class sheet_weapon_attribute
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       ATTRIBUTE;
}

[Serializable]
public class sheet_weapon_localize
{
	public Int32       KIND;
	public string      MEMO;
	public string      NAME;
	public string      DESC;
}



[ExcelAsset]
public class sheet_weapon : ScriptableObject
{
	public List<sheet_weapon_status>    status;
	public List<sheet_weapon_attribute> attribute;
	public List<sheet_weapon_localize>  localize;


	private Dictionary<Int32, sheet_weapon_status>          m_cache_status    = new ();
	private Dictionary<Int32, List<sheet_weapon_attribute>> m_cache_attribute = new ();
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
				list_attribute = new List<sheet_weapon_attribute>();
				m_cache_attribute.Add(item.KIND, list_attribute);
			}

			list_attribute.Add(item);
		}

		foreach (var item in localize)
		{
			var name_split = item.NAME.Split('/');
			var desc_split = item.DESC.Split('/');

			if (name_split.Length >= 2)
			 	m_cache_localize_name.Add(item.KIND, (name_split[0], name_split[1]));

			if (desc_split.Length >= 2)
			 	m_cache_localize_desc.Add(item.KIND, (desc_split[0], desc_split[1]));
		}		
	}


	public sheet_weapon_status GetStatus(Int32 kind)
	{
		if (m_cache_status.TryGetValue(kind, out var result))
		{
			return result;
		}

		return null;
	}

	public List<sheet_weapon_attribute> GetAttribute(Int32 kind)
	{
		if (m_cache_attribute.TryGetValue(kind, out var result))
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
		m_cache_localize_name.Clear();
	}
}
