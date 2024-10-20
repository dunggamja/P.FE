using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Target_Command_Attack : ITarget
    {
        public Int64              MainTargetID { get; private set; } = 0;

        public IEnumerable<Int64> OtherTargetIDs => null;



        public Target_Command_Attack(Int64 _target_id)
        {
            MainTargetID = _target_id;
        }

    }

    public class Command_Attack : Command
    {
        public ITarget        Target   { get; private set; }
        public Int64          WeaponID { get; private set; } 
        public (int x, int y) Position { get; private set; } 


        public Command_Attack(Int64 _owner_id, Int64 _target_id, Int64 _weapon_id, (int x, int y) _position)
            : base(_owner_id)
        {
            Target   = new Target_Command_Attack(_target_id);
            WeaponID = _weapon_id;
            Position = _position;
        }


        protected override void OnEnter()
        {
            if (Owner != null)
            {
                // ���� ����.
                Owner.StatusManager.Weapon.Equip(WeaponID);
            }
            
        }

        protected override bool OnUpdate()
        {
            
            

            return true;   
        }
        protected override void OnExit()
        {
            if (Owner != null)
            {
                Owner.SetCommandFlag(EnumCommandFlag.Action, true);
            }
        }

    }

}
