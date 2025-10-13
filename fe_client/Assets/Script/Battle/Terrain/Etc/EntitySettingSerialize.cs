using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Battle
{
   [CreateAssetMenu(fileName = "EntitySettingSerialize", menuName = "ScriptableObjects/Battle/EntitySettingSerialize")]
   public class EntitySettingSerialize : ScriptableObject
   {
      [Serializable]
      public struct EntitySetting
      {
         public string Memo;
         public Int64  ID;
         public int    Faction;         

         public int    StatusSettingKIND;        
         public int    ItemSettingKIND; 
         public int    AssetsSettingKIND;
      }

      [Serializable]
      public struct EntitySetting_Status
      {
         public string Memo;
         public int    KIND;         


         public int HP;
         public int HP_Max;

      
         public int Level;
         public int Strength;
         public int Magic;
         public int Skill;
         public int Speed;
         public int Luck;
         public int Defense;
         public int Resistance;
         public int Movement;
         public int Weight;

         public List<EnumUnitAttribute> Attributes;
      }

      [Serializable]
      public struct EntitySetting_Item
      {
         public string     Memo;
         public int        KIND;

         public List<Item> Items;
      }

      [Serializable]
      public struct EntitySetting_Assets
      {
         public string Memo;
         public int    KIND;

         public AssetReferenceGameObject AssetReference;

         public string GetAddressablePath()
         {
            return AssetReference?.AssetGUID ?? string.Empty;
         }
      }

      public List<EntitySetting>        Entities;
      public List<EntitySetting_Status> Statuses;
      public List<EntitySetting_Item>   Items;
      public List<EntitySetting_Assets> Assets;
   }

   
}