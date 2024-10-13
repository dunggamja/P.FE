using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Command_Attack_Target : ITarget
    {
        public Int64              MainTargetID { get; private set; } = 0;

        public IEnumerable<Int64> OtherTargetIDs => null;



        public Command_Attack_Target(Int64 _target_id)
        {
            MainTargetID = _target_id;
        }

    }

    public class Command_Attack : Command
    {
        public Int64   WeaponID { get; private set; } = 0;
        public ITarget Target   { get; private set; }


        public Command_Attack(Int64 _unit_id, Int64 _target_id, Int64 _weapon_id)
            : base(_unit_id)
        {
            WeaponID = _weapon_id;
            Target   = new Command_Attack_Target(_target_id);
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
            
        }

    }

}
