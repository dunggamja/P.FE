using System;
using System.Collections;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using Battle;

// namespace Battle
// {
   public struct TAG_INFO : IEquatable<TAG_INFO>
   {
      public  EnumTagType  TagType;
      public  Int64        TagValue;

      public static TAG_INFO Create(EnumTagType _tag_type, Int64 _tag_value)
      {
         return new TAG_INFO { TagType = _tag_type, TagValue = _tag_value };
      }

      public static TAG_INFO Create(Entity _entity)
      {
         if (_entity == null)
            return TAG_INFO.Create(EnumTagType.None, 0);

         return TAG_INFO.Create(EnumTagType.Entity, _entity.ID);
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

      public override int GetHashCode()
      {
         return HashCode.Combine((int)TagType, TagValue);
      }

      public bool Equals(TAG_INFO other)
      {
         return this == other;
      }
      public override bool Equals(object obj)
      {
         if (obj is TAG_INFO tag_info)
            return Equals(tag_info);

         return false;
      }
    }

   public static partial class TagHelper
   {
   }
   
// }
