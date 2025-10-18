using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;


[Serializable]
public class sheet_map_setting_entity
{
	public Int32       ID;
	public string      MEMO;
	public Int32       FACTION;
	public Int32       STATUS_KIND;
	public Int32       ITEM_KIND;
	public Int32       ASSET_KIND;
}


[Serializable]
public class sheet_map_setting_status
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       HP;
	public Int32       LEVEL;
	public Int32       STRENGTH;
	public Int32       MAGIC;
	public Int32       SKILL;
	public Int32       SPEED;
	public Int32       LUCK;
	public Int32       DEFENSE;
	public Int32       RESISTANCE;
	public Int32       MOVEMENT;
	public Int32       WEIGHT;
   public string      UNIT_ATTRIBUTE;
   public string      PATH_ATTRIBUTE;

   private HashSet<EnumUnitAttribute>      m_cache_attributes_unit = null;
   private HashSet<EnumPathOwnerAttribute> m_cache_attributes_path = null;

   private void CacheUnitAttributes()
   {
      m_cache_attributes_unit = new HashSet<EnumUnitAttribute>(); 
      foreach (var e in Data_Const.Split<EnumUnitAttribute>(UNIT_ATTRIBUTE))
         m_cache_attributes_unit.Add(e);
   }

   private void CachePathAttributes()
   {
      m_cache_attributes_path = new HashSet<EnumPathOwnerAttribute>(); 
      foreach (var e in Data_Const.Split<EnumPathOwnerAttribute>(PATH_ATTRIBUTE))
         m_cache_attributes_path.Add(e);
   }

   public  bool HasUnitAttribute(EnumUnitAttribute _attribute)
   {
      if (m_cache_attributes_unit == null)
         CacheUnitAttributes();

      return (m_cache_attributes_unit != null && m_cache_attributes_unit.Contains(_attribute));
   }

   public  bool HasPathAttribute(EnumPathOwnerAttribute _attribute)
   {
      if (m_cache_attributes_path == null)
         CachePathAttributes();

      return (m_cache_attributes_path != null && m_cache_attributes_path.Contains(_attribute));
   }
}

[Serializable]
public class sheet_map_setting_item
{
	public Int32       KIND;
	public string      MEMO;
	public int         ITEM_KIND;
   public int         COUNT;
}


[Serializable]
public class sheet_map_setting_asset
{
	public Int32       KIND;
	public string      MEMO;
	public string      ASSET_KEY;
}



[ExcelAsset]
public class sheet_map_setting : ScriptableObject
{
   // [SerializeField]
	public List<sheet_map_setting_entity>    entity;
	// [SerializeField]
	public List<sheet_map_setting_status>    status;
	// [SerializeField]
	public List<sheet_map_setting_item>      item;
	// [SerializeField]
	public List<sheet_map_setting_asset>     asset;
    
}

