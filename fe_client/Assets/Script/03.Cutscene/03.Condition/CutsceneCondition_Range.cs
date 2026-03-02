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
   public TAG_INFO UnitTag   { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);
   public TAG_INFO TargetTag { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

   public int      MinRange { get; private set; } = 0;
   public int      MaxRange { get; private set; } = 0;

   public CutsceneCondition_Range(TAG_INFO _unit_tag, TAG_INFO _target_tag, int _min_range, int _max_range)
   {
      UnitTag   = _unit_tag;
      TargetTag = _target_tag;
      MinRange  = _min_range;
      MaxRange  = _max_range;
   }


   public override bool Verify(CutsceneSequence _sequence)
   {
      using var list_unit = ListPool<Entity>.AcquireWrapper();        
      using var list_target = ListPool<Entity>.AcquireWrapper();        
      TagHelper.Collect_Entity(UnitTag, list_unit.Value);
      TagHelper.Collect_Entity(TargetTag, list_target.Value);

      foreach(var unit in list_unit.Value)
      {
         foreach(var target in list_target.Value)
         {
            var distance = PathAlgorithm.Distance(unit.Cell, target.Cell);
            if (MinRange <= distance && distance <= MaxRange)
            {
                return true;
            }
         }
      }

      return false;
   }
}
