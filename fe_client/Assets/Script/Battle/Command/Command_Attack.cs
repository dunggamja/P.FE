using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Target_Command_Attack : ITarget
    {

        public Int64       MainTargetID { get; private set; } = 0;

        public List<Int64> AllTargetIDList => null;



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

        public override EnumCommandType CommandType => EnumCommandType.Attack;

        


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
                // 공격 유닛 위치 셋팅.
                Owner.UpdateCellPosition(
                    Position,
                    (_apply: true, _immediatly: true),
                    _is_plan: false);

                // 무기 장착.
                Owner.ProcessAction(Owner.Inventory.GetItem(WeaponID), EnumItemActionType.Equip);
            }

            // 카메라 이동 처리.
            // Update_CameraPositionEvent();

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

        protected override void OnExit(bool _is_abort)
        {
            CombatParam.Cache.Reset();

            if (_is_abort)
                return;

            if (Owner != null)
            {

                // Owner.SetCommandDone(EnumCommandFlag.Action);
                // TODO: 일단 모든 행동 종료처리로 해둔다.
                Owner.SetAllCommandDone();
            }

        }

    }

}
