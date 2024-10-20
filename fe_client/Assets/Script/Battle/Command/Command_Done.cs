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

        protected override void OnExit()
        {
            if (Owner != null)
            {
                Owner.SetCommandFlag(EnumCommandFlag.Done, true);
            }
        }

    }

}

