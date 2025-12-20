using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
   public abstract class Scenario_Action
   {
      // public TAG_TARGET_INFO                Owner  { get; private set; } = TAG_TARGET_INFO.Create_None();
      // public TAG_TARGET_INFO                Target { get; private set; } = TAG_TARGET_INFO.Create_None();
      public abstract EnumScenarioAction    Action { get; }
      public abstract void Execute();

      public virtual void OnEnter() {}
      public virtual void OnExit() {}
      public virtual void OnAbort() {}
   }

   public abstract class Scenario_Action_Victory : Scenario_Action
   {
      public override EnumScenarioAction Action => EnumScenarioAction.Victory;
      public override void Execute()
      {
         // TODO:/// 즉시 승리 처리.
      }
   }

   public abstract class Scenario_Action_Defeat : Scenario_Action
   {
      public override EnumScenarioAction Action => EnumScenarioAction.Defeat;
      public override void Execute()
      {
         // TODO:/// 즉시 패배 처리.
      }
   }

   public abstract class Scenario_Action_Event : Scenario_Action
   {
      public override EnumScenarioAction Action => EnumScenarioAction.Event;
      public override void Execute()
      {
         // TODO:/// 이벤트 처리...;;; 이건 어떻게 하면 될까?
      }
   }



}