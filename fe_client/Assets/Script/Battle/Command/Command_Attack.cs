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
            // TODO : 무기 장착은 나중에 Command_Equip 으로 변경.?
            if (Owner != null)
            {
                // 무기 장착.
                Owner.StatusManager.Weapon.Equip(WeaponID);
            }

            // 전투 시스템 셋팅.
            CombatParam.Cache.Reset();
            CombatParam.Cache.Set(Owner, EntityManager.Instance.GetEntity(Target.MainTargetID));
            
            CombatSystemManager.Instance.Setup(CombatParam.Cache);
        }

        protected override bool OnUpdate()
        {
            CombatSystemManager.Instance.Update();

            return CombatSystemManager.Instance.State == EnumState.Finished;
        }

        protected override void OnExit()
        {
            if (Owner != null)
            {
                Owner.SetCommandFlag(EnumCommandFlag.Action, true);
            }

            CombatParam.Cache.Reset();
        }

    }

}
