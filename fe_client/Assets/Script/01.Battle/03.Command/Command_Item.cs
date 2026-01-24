using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    public class Target_Command_Item : ITarget
    {
        public Int64 MainTargetID { get; private set; } = 0;

        public List<Int64> AllTargetIDList => null; //TODO:///

        public Target_Command_Item(Int64 _target_id)
        {
            MainTargetID = _target_id;
        }
    }

   public class Command_Item : Command
   {
      public override EnumCommandType CommandType => EnumCommandType.Item;

      public ITarget            Target         { get; private set; } = null;
      public Int64              ItemID         { get; private set; } = 0;
      public EnumItemActionType ItemActionType { get; private set; } = EnumItemActionType.None;


      private bool m_is_sequence_done  = false;
      private bool m_is_command_action = false;
      private int  m_item_kind         = 0;

      public Command_Item(Int64 _owner_id, Int64 _target_id, Int64 _item_id, EnumItemActionType _item_action_type) : base(_owner_id)
      {
         Target         = new Target_Command_Item(_target_id);
         ItemID         = _item_id;
         ItemActionType = _item_action_type;
      }

        protected override void OnEnter()
        {
            m_is_sequence_done  = false;
            m_item_kind         = 0;

            // 커맨드를 소모하는 아이템 액션인지 체크.
            m_is_command_action = ItemHelper.IsCommandAction(ItemActionType);
            

            var owner_entity = Owner;
            if (owner_entity != null)
            {
                if (m_is_command_action)
                {
                    // TODO: 모든행동 종료 처리... 기병같은 경우 재이동 기능 구현 필요. 
                    owner_entity.SetAllCommandDone();

                    // 좌표 처리.
                    Owner.UpdateCellPosition(
                        Owner.Cell,
                        (_apply: true, _immediatly: true),
                        _is_plan: false
                    );
                }

                // 아이템 KIND 체크.
                var item_object = owner_entity.Inventory.GetItem(ItemID);                
                m_item_kind     = item_object?.Kind ?? 0;

                // 아이템 사용 처리.
                owner_entity.ProcessAction(item_object, ItemActionType);
            }

            // 연출 시작.
            PlaySequence().Forget();
        }

        protected override bool OnUpdate()
        {
            // 연출이 종료되었는지 체크.
            return m_is_sequence_done;
        }

        protected override void OnExit(bool _is_abort)
        {

        }


        async UniTask PlaySequence()
        {
            if (m_is_command_action)
            {
                // Battle Scene 시작.
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>.Acquire().Set(true)
                    );    

                
                // 타겟 HP 갱신.
                {
                    var target_entity = EntityManager.Instance.GetEntity(Target.MainTargetID);
                    var target_hp     = target_entity?.StatusManager.Status.GetPoint(EnumUnitPoint.HP) ?? 0;

                    EventDispatchManager.Instance.UpdateEvent(
                            ObjectPool<Battle_Entity_HP_UpdateEvent>
                            .Acquire()
                            .Set(Target.MainTargetID, target_hp)
                            );
                }

                await UniTask.Delay(500); // 연출 시간.

                // Battle Scene 종료.
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>.Acquire().Set(false)
                    );    
            }


            // 아이템 인벤토리 갱신.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Entity_InventoryEvent>.Acquire().Set(Owner.ID)
                );


            // 
            m_is_sequence_done = true;
        }
    }
}