using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

// public class CutsceneCondition_Entity_Posi : CutsceneCondition
// {
//     public TAG_INFO Tag { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);
    
    

// }


public class CutsceneCondition_CommandEntity : CutsceneCondition
{
   public TAG_INFO Tag { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

   public CutsceneCondition_CommandEntity(TAG_INFO _tag)
   {
      Tag = _tag;
   }

   public override bool Verify(CutsceneSequence _sequence)
   {
      // 현재 명령중인 유닛ID
      var command_entity_id = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CommandEntityID);
      if (command_entity_id == 0)
         return false;

      // 태그 대상.
      using var list_entity = ListPool<Entity>.AcquireWrapper();
      TagHelper.Collect_Entity(Tag, list_entity.Value);

      foreach(var e in list_entity.Value)
      {
         if (e.ID == command_entity_id)
         {
            return true;
         }
      }

      return false;
   }
}

