using System;
using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
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
    }


   public static class TagHelper
   {
      public static (int x, int y) ToPosition(this TAG_INFO _tag_info)
      {
         if (_tag_info.TagType == EnumTagType.Position)
            return ((int)(_tag_info.TagValue / Constants.MAX_MAP_SIZE), (int)(_tag_info.TagValue % Constants.MAX_MAP_SIZE));

         return (0, 0);
      }

      public static Int64 PositionToValue(int _x, int _y)
      {
         return _x * Constants.MAX_MAP_SIZE + _y;
      }
   }



}
