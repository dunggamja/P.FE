using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Command_Visit : Command
    {
        public override EnumCommandType CommandType => EnumCommandType.Visit;
        
        public Command_Visit(Int64 _owner_id) : base(_owner_id)
        {

        }

        protected override void OnEnter()
        {
            // 카메라 이동 처리.
            // Update_CameraPositionEvent();
            Owner.UpdateCellOccupied(true);
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
                Owner.UpdateCellOccupied(true);


               // 맵 오브젝트 방문 처리.
               using var list_map_object = ListPool<MapObject>.AcquireWrapper();
               MapObjectManager.Instance.Collect_By_Cell(Owner.Cell, list_map_object.Value);
               foreach(var e in list_map_object.Value)
               {
                  CutsceneManager.Instance.OnPlayEvent(
                     CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnMapObjectVisit, e.ID));
               }
            }
        }
    }

    public class Command_Talk : Command
    {
        public override EnumCommandType CommandType => EnumCommandType.Talk;

        public Int64          TargetID { get; private set; } = 0;
        
        public Command_Talk(Int64 _owner_id, Int64 _target_id) : base(_owner_id)
        {
            TargetID = _target_id;
        }

        protected override void OnEnter()
        {
            if (Owner != null)
            {
                Owner.UpdateCellOccupied(true);
            }
        }
        
        
        protected override bool OnUpdate()
        {
            CutsceneManager.Instance.OnPlayEvent(
                CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnTalkCommand, TargetID)
            );

            return true;
        }

        protected override void OnExit(bool _is_abort)
        {
            if (_is_abort)
                return;

            if (Owner != null)
            {
                // 대화하면 행동 종료처리.
                Owner.SetAllCommandDone();
            }
        }
    }

    public class Command_Exit : Command
    {
        public override EnumCommandType CommandType => EnumCommandType.Exit;

        public Command_Exit(Int64 _owner_id) : base(_owner_id)
        {
        }

        protected override void OnEnter()
        {
            Owner.UpdateCellOccupied(true);
        }

        protected override bool OnUpdate()
        {
            return true;
        }

        protected override void OnExit(bool _is_abort)
        {
            if (_is_abort)
                return;

            if (Owner == null)
                return;
          
            // 맵에서 이탈 처리.
            Owner.ApplyExit();
        }
    }

}

