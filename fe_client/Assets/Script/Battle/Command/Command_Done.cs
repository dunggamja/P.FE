using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Command_Done : Command
    {
        public Command_Done(Int64 _owner_id) : base(_owner_id)
        {

        }

        protected override void OnEnter()
        {
            
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

                // ÁÂÇ¥ Ã³¸®.
                Owner.UpdateCellPosition(
                    Owner.Cell,
                    _visual_immediatly: true,
                    _is_plan: false
                );
            }
        }

    }

}

