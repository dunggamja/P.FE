using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
   public class Command_Item : Command
   {
      public override EnumCommandType CommandType => EnumCommandType.Item;

      public Int64              ItemID         { get; private set; } = 0;
      public EnumItemActionType ItemActionType { get; private set; } = EnumItemActionType.None;

      public Command_Item(Int64 _owner_id, Int64 _item_id, EnumItemActionType _item_action_type) : base(_owner_id)
      {
         ItemID         = _item_id;
         ItemActionType = _item_action_type;
      }

        protected override void OnEnter()
        {
            // throw new NotImplementedException();
        }

        protected override bool OnUpdate()
        {
            // throw new NotImplementedException();
            return true;
        }

        protected override void OnExit(bool _is_abort)
        {
           if (_is_abort)
            return;

           var owner_entity = Owner;
           if (owner_entity != null)
           {
               owner_entity.ProcessAction(owner_entity.Inventory.GetItem(ItemID), ItemActionType);

               // 커맨드를 소모하는 아이템 액션인지 체크.
               if (ItemHelper.IsCommandAction(ItemActionType))
               {
                  owner_entity.SetAllCommandDone();
                  // TODO: 모든행동 종료 처리... 기병같은 경우 재이동 기능 구현 필요. 
               }
           }
            
        }
    }
}