using System;
using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
using UnityEngine;

namespace Battle
{
   public struct TAG_TARGET_INFO
   {
      public  EnumTagTargetType TagType;
      public  Int64             TagValue;

    
      public (int x, int y) Position 
      {
         get
         {
            switch(TagType)
            {
               case EnumTagTargetType.Position:
                  return ValueToPosition(TagValue);
            }

            return (0, 0);
         }
      }


      private static (int x, int y) ValueToPosition(Int64 _id)
      {
         return ((int)(_id / Constants.MAX_MAP_SIZE), (int)(_id % Constants.MAX_MAP_SIZE));
      }

      private static Int64 PositionToValue(int _x, int _y)
      {
         return _x * Constants.MAX_MAP_SIZE + _y;
      }


      public static TAG_TARGET_INFO Create_None()
      {
         return new TAG_TARGET_INFO { TagType = EnumTagTargetType.None, TagValue = 0 };
      }


      public static TAG_TARGET_INFO Create_Entity(Int64 _owner_id)
      {
         return new TAG_TARGET_INFO { TagType = EnumTagTargetType.Entity, TagValue = _owner_id };
      }

      public static TAG_TARGET_INFO Create_All()
      {
         return new TAG_TARGET_INFO { TagType = EnumTagTargetType.All, TagValue = 0 };
      }

      public static TAG_TARGET_INFO Create_Faction(Int64 _faction_id)
      {
         return new TAG_TARGET_INFO { TagType = EnumTagTargetType.Faction, TagValue = _faction_id };
      }

      public static TAG_TARGET_INFO Create_Position(int _x, int _y)
      {
         return new TAG_TARGET_INFO { TagType = EnumTagTargetType.Position, TagValue = PositionToValue(_x, _y) };
      }


      public bool Verify_Entity(Entity _entity)
      {
         if (_entity == null)
            return false;

         if (TagType == EnumTagTargetType.Entity && TagValue == _entity.ID)
            return true;

         if (TagType == EnumTagTargetType.Faction && _entity.GetFaction() == TagValue)
            return true;

         if (TagType == EnumTagTargetType.All)
            return true;

         return false;
      }

      public bool Verify_Faction(int _faction_id)
      {
         if (TagType == EnumTagTargetType.Faction && TagValue == _faction_id)
            return true;

         if (TagType == EnumTagTargetType.All)
            return true;

         return false;
      }

      public bool Verify_Position(int _x, int _y)
      {
         if (TagType == EnumTagTargetType.Position && TagValue == PositionToValue(_x, _y))
            return true;
         
         if (TagType == EnumTagTargetType.All)
            return true;

         return false;
      }
   }


   // public struct TARGET_INFO
   // {
   //    public EnumTagTargetType TargetType;
   //    public Int64             TargetID ;

   //    public static TARGET_INFO Create(EnumTagTargetType _target_type, Int64 _target_id)
   //    {
   //       return new TARGET_INFO { TargetType = _target_type, TargetID = _target_id };
   //    }
   // }



   public static class TagHelper
   {
      public static Int64 Find_EntityID(TAG_TARGET_INFO _tag_info)
      {
         if (_tag_info.TagType == EnumTagTargetType.Entity)
            return _tag_info.TagValue;

         return 0;
      }


      public static int Find_FactionID(TAG_TARGET_INFO _tag_info)
      {
         // Faction 타겟일 경우.
         if (_tag_info.TagType == EnumTagTargetType.Faction)
            return (int)_tag_info.TagValue;

         // Entity 타겟일 경우.
         var entity_id = Find_EntityID(_tag_info);
         if (entity_id > 0)
         {
            var entity  = EntityManager.Instance.GetEntity(entity_id);
            if (entity != null)
               return entity.GetFaction();            
         }

         return 0;
      }

      public static (bool result, int x, int y) Find_Position(TAG_TARGET_INFO _tag_info)
      {
         // Position 타겟일 경우.
         if (_tag_info.TagType == EnumTagTargetType.Position)
            return (true, _tag_info.Position.x, _tag_info.Position.y);

         // Entity 타겟일 경우.
         var entity_id = Find_EntityID(_tag_info);
         if (entity_id > 0)
         {
            var entity  = EntityManager.Instance.GetEntity(entity_id);
            if (entity != null)
               return (true, entity.Cell.x, entity.Cell.y);
         }

         return (false, 0, 0);
      }



   }



}
