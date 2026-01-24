using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Command_Done : Command
    {
        public override EnumCommandType CommandType => EnumCommandType.Done;
        
        public Command_Done(Int64 _owner_id) : base(_owner_id)
        {

        }

        protected override void OnEnter()
        {
            // 카메라 이동 처리.
            // Update_CameraPositionEvent();
        }

        protected override bool OnUpdate()
        {
            return true;
        }

        protected override void OnExit(bool _is_abort)
        {

            if (_is_abort)
                return;

            if (Owner != null)
            {
                // Owner.SetCommandDone(EnumCommandFlag.Done);
                Owner.SetAllCommandDone();

                // 좌표 처리.
                Owner.UpdateCellPosition(
                    Owner.Cell,
                    (_apply: true, _immediatly: true),
                    _is_plan: false
                );
            }
        }

    }

}

