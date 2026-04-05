using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


public enum EnumEntityCondition
{
   Alive,  // 생존 여부 체크.
   Dead ,  // 사망 여부 체크.
   Exit ,  // 이탈 여부 체크.

   Active, // 맵에 존재하는지 체크. (생존 O, 이탈 X, 사망 X)


   // Active, // 맵에 존재하는지 체크 (생존 O, 이탈 X, 사망 X)

}

public class CutsceneCondition_Entity_Condition : CutsceneCondition
{
   public TAG_INFO            Tag       { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);
   public EnumEntityCondition Condition { get; private set; } = EnumEntityCondition.Alive;

   public CutsceneCondition_Entity_Condition(TAG_INFO _tag, EnumEntityCondition _condition)
   {
      Tag       = _tag;
      Condition = _condition;
   }

   public override bool Verify(CutsceneSequence _sequence)
   {
       // 태그 대상.
      using var list_entity = ListPool<Entity>.AcquireWrapper();
      TagHelper.Collect_Entity(Tag, list_entity.Value);

      foreach(var e in list_entity.Value)
      {
         switch(Condition)
         {
            case EnumEntityCondition.Alive:
            {
               if (e.IsDead)
                  return false;

            }
            break;

            case EnumEntityCondition.Dead:
            {
               if (e.IsDead == false)
                  return false;

            }
            break;
            
            case EnumEntityCondition.Exit:
            {
               // 사망한 유닛은 제외합니다.
               if (e.IsDead)
                  continue;

               if (e.IsExit == false)
                  return false;
            }
            break;

            case EnumEntityCondition.Active:
            {
               if (e.IsActive == false)
                  return false;
            }
            break;
         }

      }

      // 성공.
      return true;
   }
}



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
      var command_entity_id_input    = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CommandEntityID_Input);
      var command_entity_id_progress = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CommandEntityID_Progress);

      if (command_entity_id_input == 0 && command_entity_id_progress == 0)
         return false;

      // 태그 대상.
      using var list_entity = ListPool<Entity>.AcquireWrapper();
      TagHelper.Collect_Entity(Tag, list_entity.Value);

      foreach(var e in list_entity.Value)
      {
         if (e.ID == command_entity_id_input || e.ID == command_entity_id_progress)
         {
            return true;
         }
      }

      return false;
   }
}

