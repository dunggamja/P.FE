using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Command_Attack : Command
    {
        public Int64   WeaponID { get; private set; } = 0;
        public ITarget Target   { get; private set; }

        protected override void OnEnter()
        {
            
        }

        protected override bool OnUpdate()
        {

            return true;   
        }
        protected override void OnExit()
        {
            
        }

    }

}
