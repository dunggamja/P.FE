// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// namespace Battle
// {
//    public abstract class Scenario_Condition
//    {
//       // public TAG_TARGET_INFO                Owner     { get; private set; } = TAG_TARGET_INFO.Create_None();
//       // public TAG_TARGET_INFO                Target    { get; private set; } = TAG_TARGET_INFO.Create_None();
//       public abstract EnumScenarioCondition Condition { get; }
//       public abstract bool Verify();

//       public virtual void OnEnter() {}
//       public virtual void OnExit() {}
//       public virtual void OnAbort() {}
//    }

//    public class Scenario_Condition_DefeatAllEnemies : Scenario_Condition
//    {
//       public override EnumScenarioCondition Condition => EnumScenarioCondition.DefeatAllEnemies;

//       public override bool Verify()
//       {
//          return false;
//          // 소유자 진영 체크.
//          // var faction_id = TagHelper.Find_FactionID(Owner);

//          // 살아있는 적이 있는지 찾습니다.
//          // bool Verify_Alive_Enemy(Entity _entity)
//          // {
//          //    if (_entity == null)
//          //       return false;

//          //    if (_entity.IsDead)
//          //       return false;

//          //    if (BattleSystemManager.Instance.IsAlly(faction_id, _entity.GetFaction()))
//          //       return false;

//          //    return true;
//          // };         

//          // var     alive_enemy = EntityManager.Instance.Find(Verify_Alive_Enemy);
//          // return (alive_enemy == null);
//       }
//    }

//    public class Scenario_Condition_DefeatSpecificEntity : Scenario_Condition
//    {
//       public override EnumScenarioCondition Condition => EnumScenarioCondition.DefeatSpecificEntity;

//       public override bool Verify()
//       {
//          return false;
//          // var entity_id  = TagHelper.Find_EntityID(Target);
//          // if (entity_id <= 0)
//          //    return false;

//          // var entity = EntityManager.Instance.GetEntity(entity_id);
//          // if (entity == null)
//          //    return false;

//          // return entity.IsDead;
//       }
//    }

//    public class Scenario_Condition_DefeatSpecificFaction : Scenario_Condition
//    {
//       public override EnumScenarioCondition Condition => EnumScenarioCondition.DefeatSpecificFaction;

//       public override bool Verify()
//       {
//          return false;
//          // var faction_id = TagHelper.Find_FactionID(Target);
//          // if (faction_id <= 0)
//          //    return false;

//          // bool Verify_Alive_Enemy(Entity _entity)
//          // {
//          //    if (_entity == null)
//          //       return false;

//          //    if (_entity.IsDead)
//          //       return false;

//          //    return _entity.GetFaction() == faction_id;
//          // }

//          // var     alive_faction_unit = EntityManager.Instance.Find(Verify_Alive_Enemy);
//          // return (alive_faction_unit == null);
//       }
//    }


//    public class Scenario_Condition_TurnCount : Scenario_Condition
//    {
//       public override EnumScenarioCondition Condition => EnumScenarioCondition.TurnCount;

//       //public int TurnValue => (int)Target.TagValue;

//       public override bool Verify()
//       {
//          return false;
//          // var     turn_count  = BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentTurn);
//          // return (turn_count >= TurnValue);
//       }
//    }





// }