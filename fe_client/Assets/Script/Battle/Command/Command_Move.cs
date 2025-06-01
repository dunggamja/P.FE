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
        (int x, int y)  m_cell_to          = (0, 0); 
        bool            m_is_immediate     = false;
        bool            m_is_plan          = false;
        bool            m_failed_path_find = false;

        public Command_Move(
          Int64          _owner_id
        , (int x, int y) _cell_to
        , bool           _is_immediate = false
        , bool           _is_plan      = false)
            : base(_owner_id)
        {
            m_cell_to          = _cell_to;
            m_is_immediate     = _is_immediate;
            m_is_plan          = _is_plan;
            m_failed_path_find = false;
        }
        

        protected override void OnEnter()
        {
            if (Owner == null)
                return;

            if (!m_is_immediate)
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
        }

        protected override bool OnUpdate()
        {
            if (Owner == null || m_failed_path_find)
                return true;

            // 유닛 이동 처리.
            Owner.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);

            //Debug.Log($"Command_Move, OnUpdate, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

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
                    Owner.UpdateCellPosition(m_cell_to.x, m_cell_to.y
                    , EnumCellPositionEvent.Move
                    , m_is_immediate);

                //Debug.Log($"Command_Move, OnExit, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

                // 행동 플래그 처리.
                if (m_is_plan == false)
                    Owner.SetCommandDone(EnumCommandFlag.Move);
            }
        }

    } 
}