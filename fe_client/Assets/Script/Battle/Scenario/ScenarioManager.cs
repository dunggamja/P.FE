using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

   public class Scenario
   {
      public Int64                    ID         { get; private set; } = 0;
      public EnumScenarioType         Type       { get; private set; } = EnumScenarioType.None;
      public EnumScenarioTrigger      Trigger    { get; private set; } = EnumScenarioTrigger.None;

      public EnumState                State      { get; private set; } = EnumState.None;
      public List<Scenario_Condition> Conditions { get; private set; } = new();
      public List<Scenario_Action>    Actions    { get; private set; } = new();

      // public void Execute()
      // {
      //    if (State != EnumState.Progress)
      //    {
      //       State = EnumState.Progress;
      //       OnEnter();
      //    }

      //    if (OnUpdate())
      //    {
      //       State = EnumState.Finished;
      //    }

      //    if (State != EnumState.Progress)
      //    {
      //       OnExit();
      //    }
      // }

      protected void OnEnter()
      {
         foreach (var condition in Conditions)
         {
            condition.OnEnter();
         }
      }

      protected bool OnUpdate()
      {
         foreach (var condition in Conditions)
         {
            if (condition.Verify() == false)
               return false;
         }
         return true;
      }

      protected void OnExit()
      {
         foreach (var condition in Conditions)
         {
            condition.OnExit();
         }
      }
   }


   public class ScenarioManager : Singleton<ScenarioManager>
   {
      Dictionary<Int64, Scenario>                     m_repository            = new();
      Dictionary<EnumScenarioTrigger, HashSet<Int64>> m_repository_by_trigger = new();


      public Scenario GetScenario(Int64 _id)
      {
         if (m_repository.TryGetValue(_id, out var scenario))
            return scenario;

         return null;
      }

      public void SetScenario(Scenario _scenario)
      {
         if (_scenario == null)
            return;

         if (GetScenario(_scenario.ID) != null)
             RemoveScenario(_scenario.ID);

         if (m_repository_by_trigger.TryGetValue(_scenario.Trigger, out var list_scenario_id) == false)         
         {
            list_scenario_id = new();
            m_repository_by_trigger.Add(_scenario.Trigger, list_scenario_id);
         }
         list_scenario_id.Add(_scenario.ID);

         m_repository.Add(_scenario.ID, _scenario);
      }


      public void RemoveScenario(Int64 _id)
      {
         var scenario = GetScenario(_id);
         if (scenario == null)
            return;


         if (m_repository_by_trigger.TryGetValue(scenario.Trigger, out var list_scenario_id))
         {
            list_scenario_id.Remove(scenario.ID);
         }

         m_repository.Remove(scenario.ID);
      }


      public void CollectScenario(EnumScenarioTrigger _trigger, List<Scenario> _result)
      {
         if (m_repository_by_trigger.TryGetValue(_trigger, out var list_scenario_id))
         {
            foreach(var scenario_id in list_scenario_id)
            {
               var scenario  = GetScenario(scenario_id);
               if (scenario != null)
                  _result.Add(scenario);
            }
         }
      }
   }

}

