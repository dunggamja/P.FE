using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

[Serializable]
public class sheet_class_status_max
{
   // TODO: 레벨별 스탯 min/max로 바꿀 예정.
	public Int32       KIND;
	public string      MEMO;
	public Int32       HP;
	public Int32       STRENGTH;
	public Int32       MAGIC;
	public Int32       SKILL;
	public Int32       SPEED;
	public Int32       LUCK;
	public Int32       PROFICIENCY;
	public Int32       DEFENSE;
	public Int32       RESISTANCE;
	// public Int32       MOVEMENT;	

   public int GetStatus(EnumUnitStatus _status)
   {
      switch (_status)
      {
         case EnumUnitStatus.Strength:    return STRENGTH;
         case EnumUnitStatus.Magic:       return MAGIC;
         case EnumUnitStatus.Skill:       return SKILL;
         case EnumUnitStatus.Speed:       return SPEED;
         case EnumUnitStatus.Luck:        return LUCK;
         case EnumUnitStatus.Defense:     return DEFENSE;
         case EnumUnitStatus.Resistance:  return RESISTANCE;
         case EnumUnitStatus.Proficiency: return PROFICIENCY;
         // case EnumUnitStatus.Movement:    return MOVEMENT;
      }

      throw new Exception($"Invalid unit status: {_status}");
   }

   public int GetPoint(EnumUnitPoint _point)
   {
      switch (_point)
      {
         case EnumUnitPoint.HP:     return HP;
         case EnumUnitPoint.HP_Max: return HP;
      }

      throw new Exception($"Invalid unit point: {_point}");
   }
}
[Serializable]
public class sheet_class_attribute
{
	public Int32       KIND;
	public string      MEMO;
   public string      UNIT;
   public string      MOUNT;
   public Int32       TERRAIN;
   public Int32       TERRAIN_MOUNTED; 
   public string      WEAPON;
}

[Serializable]
public class sheet_class_terrain
{
	public Int32       KIND;
	public string      MEMO;
   public Int32       MOVEMENT;
   public Int32       GROUND;
   public Int32       FOREST;
   public Int32       DIRT;
   public Int32       CLIMB;
   public Int32       INDOOR;
   public Int32       WATER;
   public Int32       WATER_SHALLOW;
   public Int32       FLYER_ONLY;
}

[Serializable]
public class sheet_class_asset
{
	public Int32       KIND;
	public string      MEMO;
   public Int64       UNIT_ID;
	public string      ASSET_KEY;
}

[Serializable]
public class sheet_class_localization
{
	public Int32       KIND;
	public string      MEMO;
   public string      NAME;
}

[Serializable]
public class sheet_class_change
{
	public Int32       KIND;
	public string      MEMO;
   public Int32       NEXT_CLASS;
}

[Serializable]
public class sheet_unit_status_levelup
{
	public Int32       ID;
	public string      MEMO;
	public Int32       HP;
	public Int32       STRENGTH;
	public Int32       MAGIC;
	public Int32       SKILL;
	public Int32       SPEED;
	public Int32       LUCK;
	public Int32       PROFICIENCY;
	public Int32       DEFENSE;
	public Int32       RESISTANCE;
	// public Int32       MOVEMENT;	

      public int GetStatus(EnumUnitStatus _status)
   {
      switch (_status)
      {
         case EnumUnitStatus.Strength:    return STRENGTH;
         case EnumUnitStatus.Magic:       return MAGIC;
         case EnumUnitStatus.Skill:       return SKILL;
         case EnumUnitStatus.Speed:       return SPEED;
         case EnumUnitStatus.Luck:        return LUCK;
         case EnumUnitStatus.Defense:     return DEFENSE;
         case EnumUnitStatus.Resistance:  return RESISTANCE;
         case EnumUnitStatus.Proficiency: return PROFICIENCY;
         // case EnumUnitStatus.Movement:    return MOVEMENT;
      }

      throw new Exception($"Invalid unit status: {_status}");
   }

   public int GetPoint(EnumUnitPoint _point)
   {
      switch (_point)
      {
         case EnumUnitPoint.HP:     return HP;
         case EnumUnitPoint.HP_Max: return HP;
      }

      throw new Exception($"Invalid unit point: {_point}");
   }
}


[ExcelAsset]
public class sheet_unit : ScriptableObject
{
	public List<sheet_class_status_max>    class_status_max;
   public List<sheet_class_attribute>     class_attribute;
   public List<sheet_class_terrain>       class_terrain;
   public List<sheet_class_asset>         class_asset;
   public List<sheet_class_localization>  class_localization;
   public List<sheet_class_change>        class_change;
   public List<sheet_unit_status_levelup> unit_status_levelup;

   private Dictionary<Int32, sheet_class_status_max>    m_cache_class_status_max        = new ();
   private Dictionary<Int32, List<EnumUnitAttribute>>   m_cache_class_attribute_unit    = new ();
   private Dictionary<Int32, EnumUnitMountedType>       m_cache_class_mounted_type      = new ();
   private Dictionary<Int32, List<EnumWeaponCategory>>  m_cache_class_attribute_weapon  = new ();
   private Dictionary<Int32, List<sheet_class_asset>>   m_cache_class_asset             = new ();
   private Dictionary<Int32, (string table, string key)>  m_cache_class_localization    = new ();
   private Dictionary<Int32, sheet_class_change>        m_cache_class_change            = new ();
   private Dictionary<Int32, sheet_unit_status_levelup> m_cache_unit_status_levelup     = new ();
   private Dictionary<Int32, sheet_class_terrain>       m_cache_terrain           = new ();
   private Dictionary<Int32, sheet_class_attribute>     m_cache_class_attribute         = new ();

   public void Initialize()
   {
      foreach (var item in class_status_max)
      {
         m_cache_class_status_max.Add(item.KIND, item);
      }
      
      foreach (var item in class_attribute)
      {
         var unit_split    = Util.SplitEnumText<EnumUnitAttribute>(item.UNIT, Data_Const.SHEET_SEPERATOR);
         var mounted_split = Util.EnumParse<EnumUnitMountedType>(item.MOUNT);
         var weapon_split  = Util.SplitEnumText<EnumWeaponCategory>(item.WEAPON, Data_Const.SHEET_SEPERATOR);
        

         m_cache_class_attribute_unit.Add(item.KIND, unit_split);
         m_cache_class_mounted_type.Add(item.KIND, mounted_split);
         m_cache_class_attribute_weapon.Add(item.KIND, weapon_split);
         m_cache_class_attribute.Add(item.KIND, item);
      }

      foreach (var item in class_asset)
      {
         if (m_cache_class_asset.TryGetValue(item.KIND, out var list_asset) == false)
         {
            list_asset = new List<sheet_class_asset>();
            m_cache_class_asset.Add(item.KIND, list_asset);
         }

         list_asset.Add(item);
      }
      
      foreach (var item in class_localization)
      {
         var name_split = Util.SplitText(item.NAME, Data_Const.SHEET_LOCALIZATION_SEPERATOR);
         if (name_split.Count >= 2)
         {
            m_cache_class_localization.Add(item.KIND, (name_split[0], name_split[1]));
         }
      }

      foreach (var item in class_change)
      {
         m_cache_class_change.Add(item.KIND, item);
      }

      foreach (var item in unit_status_levelup)
      {
         m_cache_unit_status_levelup.Add(item.ID, item);
      }

      foreach (var item in class_terrain)
      {
         m_cache_terrain.Add(item.KIND, item);
      }
   }


   public int GetClassStatusMax(Int32 _kind, EnumUnitStatus _status)
   {
      if (m_cache_class_status_max.TryGetValue(_kind, out var result))
      {
         return result.GetStatus(_status);
      }

      return 0;      
   }

   public bool HasClassAttribute_Unit(Int32 _class_kind, EnumUnitAttribute _attribute)
   {
      if (m_cache_class_attribute_unit.TryGetValue(_class_kind, out var result)) 
         return result.Contains(_attribute);

      return false;
   }

   public EnumUnitMountedType GetClassMountedType(Int32 _class_kind)
   {
      if (m_cache_class_mounted_type.TryGetValue(_class_kind, out var mounted_type)) 
         return mounted_type;

      return EnumUnitMountedType.None;
   }

   // public int GetClassAttribute_Path(Int32 _class_kind)
   // {
   //    if (m_cache_class_attribute_path.TryGetValue(_class_kind, out var result)) 
   //       return result;

   //    return 0;
   // }

   public sheet_class_attribute GetClassAttribute(Int32 _class_kind)
   {
      if (m_cache_class_attribute.TryGetValue(_class_kind, out var result))
         return result;

      return null;
   }

   public int GetTerrainCost(Int32 _terrain_kind, EnumTerrainAttribute _terrain_attribute)
   {
      if (m_cache_terrain.TryGetValue(_terrain_kind, out var result) == false)
         return 0;

      switch(_terrain_attribute)
      {
         case EnumTerrainAttribute.Ground:        return result.GROUND;
         case EnumTerrainAttribute.Ground_Forest: return result.FOREST;
         case EnumTerrainAttribute.Ground_Dirt:   return result.DIRT;
         case EnumTerrainAttribute.Ground_Climb:  return result.CLIMB;
         case EnumTerrainAttribute.Ground_Indoor: return result.INDOOR;
         case EnumTerrainAttribute.Water:         return result.WATER;
         case EnumTerrainAttribute.Water_Shallow: return result.WATER_SHALLOW;
         case EnumTerrainAttribute.FlyerOnly:     return result.FLYER_ONLY;
      }

      return 0;
   }

   public int GetTerrainKind(Int32 _class_kind, bool _mounted)
   {
      var class_attribute = GetClassAttribute(_class_kind);
      if (class_attribute == null)
         return 0;

      return (_mounted) ? class_attribute.TERRAIN_MOUNTED : class_attribute.TERRAIN;
   }

   public int GetTerrainMovement(Int32 _class_kind, bool _mounted)
   {
      var terrain_kind = GetTerrainKind(_class_kind, _mounted);
      if (m_cache_terrain.TryGetValue(terrain_kind, out var result) == false)
         return 0;

      return result.MOVEMENT;
   }

   public int  GetTerrainCost(Int32 _class_kind, bool _mounted, EnumTerrainAttribute _terrain_attribute)
   {
      var terrain_kind = GetTerrainKind(_class_kind, _mounted);

      return GetTerrainCost(terrain_kind, _terrain_attribute);
   }
   
   public bool HasClassAttribute_Weapon(Int32 _class_kind, EnumWeaponCategory _attribute)
   {
      if (m_cache_class_attribute_weapon.TryGetValue(_class_kind, out var result)) 
         return result.Contains(_attribute);

      return false;
   }

   public (string table, string key) GetClassLocalization(Int32 _class_kind)
   {
      if (m_cache_class_localization.TryGetValue(_class_kind, out var result)) 
         return result;

      return (string.Empty, string.Empty);
   }

   public int GetNextClass(Int32 _class_kind)
   {
      if (m_cache_class_change.TryGetValue(_class_kind, out var result)) 
         return result.NEXT_CLASS;

      return 0;
   }



   public int GetUnitStatusLevelUp(Int32 _kind, EnumUnitStatus _status)
   {
      if (m_cache_unit_status_levelup.TryGetValue(_kind, out var result))
      {
         return result.GetStatus(_status);
      }

      return 0;
   }

   public string GetUnitAssetName(Int32 _class_kind, Int64 _unit_id)
   {
      var default_asset_name = string.Empty;
      var unique_asset_name  = string.Empty;


      if (m_cache_class_asset.TryGetValue(_class_kind, out var list_asset))
      {
         foreach (var e in list_asset)
         {
            if (e.UNIT_ID == _unit_id)
               unique_asset_name = e.ASSET_KEY;
            if (e.UNIT_ID == 0)
               default_asset_name = e.ASSET_KEY;
         }
      }

      // unique 에셋이 있으면 그것을 사용.
      if (string.IsNullOrEmpty(unique_asset_name) == false)
         return unique_asset_name;

      return default_asset_name;
   }
}