using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
   public class Target_Command_Exchange : ITarget
   {
      public Int64 MainTargetID { get; private set; } = 0;

      public List<Int64> AllTargetIDList => null;

      public Target_Command_Exchange(Int64 _target_id)
      {
         MainTargetID = _target_id;
      }
   }

   public class Command_Exchange : Command
   {
      public ITarget        Target         { get; private set; }
      public List<Item>     ActorItemList  { get; private set; } = new();
      public List<Item>     TargetItemList { get; private set; } = new();
      public bool           ExecuteCommand { get; private set; } = false;
      // public (int x, int y) Position       { get; private set; } 

      public override EnumCommandType CommandType => EnumCommandType.Exchange;

      public Command_Exchange(
         Int64          _owner_id,
         Int64          _target_id,
         List<Item>     _actor_item_list,
         List<Item>     _target_item_list,
         bool           _execute_command
         // (int x, int y) _position
         ) : base(_owner_id)
      {
         Target         = new Target_Command_Exchange(_target_id);

         if (_actor_item_list != null)
            ActorItemList.AddRange(_actor_item_list);

         if (_target_item_list != null)
            TargetItemList.AddRange(_target_item_list);

         ExecuteCommand = _execute_command;

      }

      protected override void OnEnter()
      {
         // if (Owner != null)
         // {
         //    Owner.UpdateCellPosition(
         //        Position,
         //        (_apply: true, _immediatly: true),
         //        _is_plan: false);
         // }
      }

      protected override bool OnUpdate()
      {
         
         return true;
      }

      protected override void OnExit(bool _is_abort)
      {
         if (_is_abort)
            return;


         // 타겟 아이템 셋팅.
         var target_entity = EntityManager.Instance.GetEntity(Target.MainTargetID);
         if (target_entity != null)
         {
            // 소유중인 아이템 갱신
            target_entity.RemoveItemAll();

            if (TargetItemList != null)
            {
               foreach(var e in TargetItemList)
                  target_entity.Inventory.AddItem(e);

               target_entity.Equip_Weapon_Auto();
            }
         }

         // 소유자 아이템 셋팅.
         var owner_entity = Owner;
         if (owner_entity != null)
         {
            // 소유중인 아이템 갱신
            owner_entity.RemoveItemAll();

            if (ActorItemList != null)
            {
               foreach(var e in ActorItemList)
                  owner_entity.Inventory.AddItem(e);

               owner_entity.Equip_Weapon_Auto();
            }

            if (ExecuteCommand)
            {
               owner_entity.SetCommandDone(EnumCommandFlag.Exchange);
            }
         }
      }
      
   }

}
