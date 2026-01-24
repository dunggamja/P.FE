using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

[Serializable]
public class sheet_class_status_max
{
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
	public Int32       MOVEMENT;	

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
         case EnumUnitStatus.Movement:    return MOVEMENT;
         case EnumUnitStatus.Proficiency: return PROFICIENCY;
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
   public string      PATH;
   public string      WEAPON;
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
	public Int32       MOVEMENT;	

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
         case EnumUnitStatus.Movement:    return MOVEMENT;
         case EnumUnitStatus.Proficiency: return PROFICIENCY;
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
   public List<sheet_class_asset>         class_asset;
   public List<sheet_class_localization>  class_localization;
   public List<sheet_class_change>        class_change;
   public List<sheet_unit_status_levelup> unit_status_levelup;

   private Dictionary<Int32, sheet_class_status_max>    m_cache_class_status_max = new ();
   private Dictionary<Int32, List<EnumUnitAttribute>>   m_cache_class_attribute_unit = new ();
   private Dictionary<Int32, Int32>                     m_cache_class_attribute_path = new ();
   private Dictionary<Int32, List<EnumWeaponCategory>>  m_cache_class_attribute_weapon = new ();
   private Dictionary<Int32, List<sheet_class_asset>>   m_cache_class_asset = new ();
   private Dictionary<Int32, (string table, string key)>  m_cache_class_localization = new ();
   private Dictionary<Int32, sheet_class_change>        m_cache_class_change = new ();
   private Dictionary<Int32, sheet_unit_status_levelup> m_cache_unit_status_levelup = new ();


   public void Initialize()
   {
      foreach (var item in class_status_max)
      {
         m_cache_class_status_max.Add(item.KIND, item);
      }
      
      foreach (var item in class_attribute)
      {
         var unit_split   = Util.SplitEnumText<EnumUnitAttribute>(item.UNIT, Data_Const.SHEET_SEPERATOR);
         var path_split   = Util.SplitEnumText<EnumPathOwnerAttribute>(item.PATH, Data_Const.SHEET_SEPERATOR);
         var weapon_split = Util.SplitEnumText<EnumWeaponCategory>(item.WEAPON, Data_Const.SHEET_SEPERATOR);

         Int32 path_attribute = 0;
         foreach (var e in path_split)
               path_attribute |= 1 << (int)e;

         m_cache_class_attribute_unit.Add(item.KIND, unit_split);
         m_cache_class_attribute_path.Add(item.KIND, path_attribute);
         m_cache_class_attribute_weapon.Add(item.KIND, weapon_split);
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

   public int GetClassAttribute_Path(Int32 _class_kind)
   {
      if (m_cache_class_attribute_path.TryGetValue(_class_kind, out var result)) 
         return result;

      return 0;
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