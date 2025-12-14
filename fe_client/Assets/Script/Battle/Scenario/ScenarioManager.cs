using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
   public abstract class Scenario_Condition
   {
      public TAG_TARGET_INFO                Owner     { get; private set; } = TAG_TARGET_INFO.Create_None();
      public TAG_TARGET_INFO                Target    { get; private set; } = TAG_TARGET_INFO.Create_None();
      public abstract EnumScenarioCondition Condition { get; }
      public abstract bool Verify();
   }

   public abstract class Scenario_Action
   {
      public TAG_TARGET_INFO                Owner  { get; private set; } = TAG_TARGET_INFO.Create_None();
      public TAG_TARGET_INFO                Target { get; private set; } = TAG_TARGET_INFO.Create_None();
      public abstract EnumScenarioAction    Action { get; }
      public abstract void Execute();
   }


   public class Scenario
   {
      public Int64                    ID         { get; private set; } = 0;
      public EnumScenarioType         Type       { get; private set; } = EnumScenarioType.None;
      public EnumScenarioTrigger      Trigger    { get; private set; } = EnumScenarioTrigger.None;
      public List<Scenario_Condition> Conditions { get; private set; } = new();
      public List<Scenario_Action>    Actions    { get; private set; } = new();
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
            RemoveScenario(_scenario);


         if (m_repository_by_trigger.TryGetValue(_scenario.Trigger, out var list_scenario_id) == false)         
         {
            list_scenario_id = new();
            m_repository_by_trigger.Add(_scenario.Trigger, list_scenario_id);
         }
         list_scenario_id.Add(_scenario.ID);

         m_repository.Add(_scenario.ID, _scenario);
      }


      public void RemoveScenario(Scenario _scenario)
      {
         if (_scenario == null)
            return;


         if (m_repository_by_trigger.TryGetValue(_scenario.Trigger, out var list_scenario_id))
         {
            list_scenario_id.Remove(_scenario.ID);
         }

         m_repository.Remove(_scenario.ID);
      }


      public void CollectScenario(EnumScenarioTrigger _trigger, List<Scenario> _result)
      {
         if (m_repository_by_trigger.TryGetValue(_trigger, out var list_scenario_id))
         {
            foreach(var scenario_id in list_scenario_id)
            {
               _result.Add(GetScenario(scenario_id));
            }
         }
      }



      
   }

}

