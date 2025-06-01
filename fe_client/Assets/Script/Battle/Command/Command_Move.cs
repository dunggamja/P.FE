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
                // ��� ���� & ��ã��
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
                // ��� ���� ����.
                Owner.PathNodeManager.ClearPath(); 
            }
        }

        protected override bool OnUpdate()
        {
            if (Owner == null || m_failed_path_find)
                return true;

            // ���� �̵� ó��.
            Owner.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);

            //Debug.Log($"Command_Move, OnUpdate, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

            // �̵��� �Ϸ�Ǿ����� �Ϸ�ó��.
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
                
                // ��ǥ �̵� ó��.
                if (m_failed_path_find == false)
                    Owner.UpdateCellPosition(m_cell_to.x, m_cell_to.y
                    , EnumCellPositionEvent.Move
                    , m_is_immediate);

                //Debug.Log($"Command_Move, OnExit, ID:{OwnerID}, Position:{Owner.PathVehicle.Position}");

                // �ൿ �÷��� ó��.
                if (m_is_plan == false)
                    Owner.SetCommandDone(EnumCommandFlag.Move);
            }
        }

    } 
}