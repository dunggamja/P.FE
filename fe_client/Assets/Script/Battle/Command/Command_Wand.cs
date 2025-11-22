using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Target_Command_Wand : ITarget
    {
        public Int64 MainTargetID { get; private set; } = 0;

        public List<Int64> AllTargetIDList => null; //TODO:///

        public Target_Command_Wand(Int64 _target_id)
        {
            MainTargetID = _target_id;
        }
    }

   public class Command_Wand : Command
   {
       public ITarget        Target   { get; private set; }
       public Int64          WeaponID { get; private set; } 
       public (int x, int y) Position { get; private set; } 

       public override EnumCommandType CommandType => EnumCommandType.Wand;

       public Command_Wand(Int64 _owner_id, Int64 _target_id, Int64 _weapon_id, (int x, int y) _position)
          : base(_owner_id)
       {
            Target   = new Target_Command_Wand(_target_id);
            WeaponID = _weapon_id;
            Position = _position;
       }

       protected override void OnEnter()
       {
         if (Owner != null)
         {
            Owner.UpdateCellPosition(
                Position,
                (_apply: true, _immediatly: true),
                _is_plan: false);
         }

         // 무기 장착.
         Owner.ProcessAction(Owner.Inventory.GetItem(WeaponID), EnumItemActionType.Equip);

         // 전투 시스템 셋팅.
         var combat_param = new CombatParam().Set(
            _attacker: Owner, 
            _defender: EntityManager.Instance.GetEntity(Target.MainTargetID), 
            _use_wand: true);
         CombatSystemManager.Instance.Setup(combat_param);
       }

        protected override bool OnUpdate()
        {
          CombatSystemManager.Instance.Update();
          
          return CombatSystemManager.Instance.State == EnumState.Finished;
        }

        protected override void OnExit(bool _is_abort)
        {
            if (_is_abort)
                return;

            if (Owner != null)
            {
                // Owner.SetCommandDone(EnumCommandFlag.Action);
                // TODO: 모든행동 종료 처리... 기병같은 경우 재이동 기능 구현 필요. 
                Owner.SetAllCommandDone();
            }
        }
    }
}
