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



}
