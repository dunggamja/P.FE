using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


public class CutsceneCondition_Range : CutsceneCondition
{
   public TAG_INFO Tag1 { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);
   public TAG_INFO Tag2 { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

   public int      MinRange { get; private set; } = 0;
   public int      MaxRange { get; private set; } = 0;

   public CutsceneCondition_Range(TAG_INFO _tag_1, TAG_INFO _tag_2, int _min_range, int _max_range)
   {
      Tag1     = _tag_1;
      Tag2     = _tag_2;
      MinRange = _min_range;
      MaxRange = _max_range;
   }


   public override bool Verify(CutsceneSequence _sequence)
   {
      using var list_position_1 = ListPool<(int x, int y)>.AcquireWrapper();        
      using var list_position_2 = ListPool<(int x, int y)>.AcquireWrapper();        
      TagHelper.Collect_Position(Tag1, list_position_1.Value);
      TagHelper.Collect_Position(Tag2, list_position_2.Value);

      foreach(var (x1, y1) in list_position_1.Value)
      {
         foreach(var (x2, y2) in list_position_2.Value)
         {
            var distance = PathAlgorithm.Distance((x1, y1), (x2, y2));
            if (MinRange <= distance && distance <= MaxRange)
            {
                return true;
            }
         }
      }

      return false;
   }
}
