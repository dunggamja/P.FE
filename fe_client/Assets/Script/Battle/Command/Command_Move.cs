using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
   public class Command_Move : Command
    {
        
        (int x, int y)        m_cell_to         = (0, 0); 
        List<PathNode>  m_path_node       = new ();
        int             m_path_node_index = 0;

        public Command_Move(Int64 _owner_id, (int x, int y) _cell_to) : base(_owner_id)
        {
            m_cell_to = _cell_to;
        }
        

        protected override void OnEnter()
        {
            if (Owner == null)
                return;

            // 경로 생성. 길찾기
            Owner.PathNodeManager.CreatePath(
                Owner.Cell.CellToPosition(),
                m_cell_to.CellToPosition(),
                Owner);

        }

        protected override bool OnUpdate()
        {
            if (Owner == null)
                return true;

            // 유닛 이동 처리.
            Owner.UpdatePathBehavior();

            // 이동이 완료되었으면 완료처리.
            return Owner.PathNodeManager.IsEmpty();
        }

        protected override void OnExit()
        {
            if (Owner != null)
            {
                // 좌표 이동 처리.
                Owner.UpdateCellPosition(m_cell_to.x, m_cell_to.y, true);

                // 행동 플래그 
                Owner.SetCommandFlag(EnumCommandFlag.Move, true);
            }
        }

    } 
}