using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
   public class Command_Move : Command
    {

        public override EnumCommandType CommandType => EnumCommandType.Move;
        public override bool            IsAbortable => true;


        // List<PathNode>  m_path_node       = new ();
        // int             m_path_node_index = 0;
        (int x, int y)        m_cell_to          = (0, 0); 
        // EnumCellPositionEvent m_cell_event       = EnumCellPositionEvent.Enter;
        bool                  m_visual_immediate = false;
        bool                  m_execute_command  = false;
        bool                  m_is_plan          = false;
        bool                  m_move_success = false;



        public Command_Move(
          Int64                 _owner_id
        , (int x, int y)        _cell_to
        // , EnumCellPositionEvent _cell_event
        , bool                  _execute_command
        , bool                  _visual_immediate = false
        , bool                  _is_plan          = false)
            : base(_owner_id)
        {
            m_cell_to          = _cell_to;
            // m_cell_event       = _cell_event;
            m_execute_command  = _execute_command;
            m_visual_immediate = _visual_immediate;
            m_is_plan          = _is_plan;
            m_move_success     = false;
        }
        

        protected override void OnEnter()
        {
            if (Owner == null)
                return;


            // 이동이 가능한 상태인지 체크.
            if (Owner.HasCommandEnable(EnumCommandFlag.Move) == false)
            {
                m_move_success = false;
                return;
            }

            // 즉시 이동여부에 따라서 길찾기 여부 결정.
            if (m_visual_immediate)
            {
                // 경로 생성 안함.
                Owner.PathNodeManager.ClearPath(); 
            }
            else
            {
                // 길찾기 시도.
                m_move_success = Owner.PathNodeManager.CreatePath(
                            Owner.PathVehicle.Position, 
                            m_cell_to.CellToPosition(), 
                            Owner);
            }
        }

        protected override bool OnUpdate()
        {
            if (Owner == null || m_move_success == false)
                return true;

            // 유닛 이동 처리.
            Owner.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);

            //Debug.Log($"Command_Move, OnUpdate, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

            // 카메라 이동 처리.
            // Update_CameraPositionEvent();

            // 이동이 완료되었으면 완료처리.
            return Owner.PathNodeManager.IsEmpty();
        }

        protected override void OnExit(bool _is_abort)
        {
            if (_is_abort)
            {
                return;
            }


            if (Owner != null)
            {
                // Owner.PathVehicle.Position
                
                // 좌표 이동 처리.
                if (m_move_success)
                {
                    Owner.UpdateCellPosition(
                          m_cell_to
                        , (_apply: true, _immediatly: m_visual_immediate)
                        , m_is_plan);
                }

                //Debug.Log($"Command_Move, OnExit, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

                // 행동 플래그 처리.
                if (m_execute_command)
                    Owner.SetCommandDone(EnumCommandFlag.Move);
            }

            // 카메라 이동 처리.
            // Update_CameraPositionEvent();
        }

    } 
}