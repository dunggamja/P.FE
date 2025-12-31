using System;
using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEngine;

namespace Battle
{
   public struct TAG_INFO
   {
      public  EnumTagType  TagType;
      public  Int64        TagValue;

      public static TAG_INFO Create(EnumTagType _tag_type, Int64 _tag_value)
      {
         return new TAG_INFO { TagType = _tag_type, TagValue = _tag_value };
      }

      public static bool operator <(TAG_INFO _left, TAG_INFO _right)
      {
         if (_left.TagType != _right.TagType)
            return _left.TagType < _right.TagType;

         return _left.TagValue < _right.TagValue;
      }

      public static bool operator >(TAG_INFO _left, TAG_INFO _right)
      {
         if (_left.TagType != _right.TagType)
            return _left.TagType > _right.TagType;

         return _left.TagValue > _right.TagValue;
      }

      public static bool operator ==(TAG_INFO _left, TAG_INFO _right)
      {
         return (_left.TagType, _left.TagValue) == (_right.TagType, _right.TagValue);
      }

      public static bool operator !=(TAG_INFO _left, TAG_INFO _right)
      {
         return (_left.TagType, _left.TagValue) != (_right.TagType, _right.TagValue);
      }

      public override bool Equals(object obj)
      {
         if (obj is TAG_INFO tag_info)
            return this == tag_info;

         return false;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine((int)TagType, TagValue);
      }



      public static TAG_INFO Create(Entity _entity)
      {
         if (_entity == null)
            return TAG_INFO.Create(EnumTagType.None, 0);

         return TAG_INFO.Create(EnumTagType.Entity, _entity.ID);
      }



   }



   public static class TagHelper
   {
      const Int64 POSITION_VALUE_MAX = Constants.MAX_MAP_SIZE * Constants.MAX_MAP_SIZE;

      static (int x, int y) ToPosition(Int64 _value)
      {
         _value %= POSITION_VALUE_MAX;
         return ((int)(_value / Constants.MAX_MAP_SIZE), (int)(_value % Constants.MAX_MAP_SIZE));
      }

      public static Int64 PositionToValue(int _x, int _y)
      {
         _x %= Constants.MAX_MAP_SIZE;
         _y %= Constants.MAX_MAP_SIZE;

         return _x * Constants.MAX_MAP_SIZE + _y;      
      }

      public static (int x, int y) ToPosition(this TAG_INFO _tag_info)
      {
         if (_tag_info.TagType == EnumTagType.Position)
            return ToPosition(_tag_info.TagValue);

         return (0, 0);
      }

      public static (int min_x, int min_y, int max_x, int max_y) ToPositionRect(this TAG_INFO _tag_info)
      {
         if (_tag_info.TagType != EnumTagType.Position_Rect)
            return (0, 0, 0, 0);


         var value     = _tag_info.TagValue;         
         var max_value = ToPosition(value);
         var min_value = ToPosition(value / POSITION_VALUE_MAX);

         return (min_value.x, min_value.y, max_value.x, max_value.y);
      }


      public static Int64 PositionRectToValue(int _min_x, int _min_y, int _max_x, int _max_y)
      {
         var value = PositionToValue(_min_x, _min_y);
         value    *= POSITION_VALUE_MAX;
         value    += PositionToValue(_max_x, _max_y);

         return value;
      }



      public static void SetupTag_Entity_Hierarchy(Entity _entity, bool _is_alive)
      {
         if (_entity == null)
            return;

         //  ENTITY_FACTION, ENTITY_ALL 태그 관련 처리를 진행합니다.
         var tag_entity_all = TAG_INFO.Create(EnumTagType.Entity_All, 0);
         var tag_faction    = TAG_INFO.Create(EnumTagType.Entity_Faction, _entity.GetFaction());
         var tag_entity     = TAG_INFO.Create(_entity);



         //  ENTITY_FACTION, ENTITY_ALL 태그 일단 삭제 후 살아있으면 다시 셋팅합니다.
         {
            using var list_remove = ListPool<TAG_DATA>.AcquireWrapper();
            TagManager.Instance.CollectTagTarget(tag_entity, EnumTagAttributeType.TAG_HIERARCHY, list_remove.Value);
            foreach(var tag in list_remove.Value)
            {
               switch(tag.TagInfo.TagType)
               {
                  case EnumTagType.Entity_Faction:
                  case EnumTagType.Entity_All:
                     TagManager.Instance.RemoveTag(tag);
                     break;
               }
            }
         }

         
         // 유닛이 살아있다면. 태그 셋팅.
         if (_is_alive)
         {
            TagManager.Instance.SetTag(TAG_DATA.Create(tag_entity_all, EnumTagAttributeType.TAG_HIERARCHY, tag_entity));
            TagManager.Instance.SetTag(TAG_DATA.Create(tag_faction,    EnumTagAttributeType.TAG_HIERARCHY, tag_entity));
         }
      }

   }



}
