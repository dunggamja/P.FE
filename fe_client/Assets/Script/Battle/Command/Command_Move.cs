using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
   public class Command_Move : Command
    {
        
        // List<PathNode>  m_path_node       = new ();
        // int             m_path_node_index = 0;
        (int x, int y)        m_cell_to          = (0, 0); 
        // EnumCellPositionEvent m_cell_event       = EnumCellPositionEvent.Enter;
        bool                  m_visual_immediate = false;
        bool                  m_execute_command  = false;
        bool                  m_is_plan          = false;
        bool                  m_failed_path_find = false;

        public override bool  IsAbortable => true;


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
            m_failed_path_find = false;
        }
        

        protected override void OnEnter()
        {
            if (Owner == null)
                return;

            if (!m_visual_immediate)
            {
                // 경로 생성 & 길찾기
                if (Owner.PathNodeManager.CreatePath(
                    Owner.PathVehicle.Position,
                    m_cell_to.CellToPosition(),
                    Owner) == false)
                {
                    m_failed_path_find = true;
                }
            }
            else
            {
                // 경로 생성 안함.
                Owner.PathNodeManager.ClearPath(); 
            }

            // 카메라 이동 처리.
            Update_CameraPositionEvent();
        }

        protected override bool OnUpdate()
        {
            if (Owner == null || m_failed_path_find)
                return true;

            // 유닛 이동 처리.
            Owner.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);

            //Debug.Log($"Command_Move, OnUpdate, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

            // 카메라 이동 처리.
            Update_CameraPositionEvent();

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
                if (m_failed_path_find == false)
                    Owner.UpdateCellPosition(
                          m_cell_to
                        , (_apply: true, _immediatly: m_visual_immediate)
                        , m_is_plan);

                //Debug.Log($"Command_Move, OnExit, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

                // 행동 플래그 처리.
                if (m_execute_command)
                    Owner.SetCommandDone(EnumCommandFlag.Move);
            }

            // 카메라 이동 처리.
            Update_CameraPositionEvent();
        }

    } 
}