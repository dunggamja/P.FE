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

            // ��� ����. ��ã��
            Owner.PathNodeManager.CreatePath(
                Owner.Cell.CellToPosition(),
                m_cell_to.CellToPosition(),
                Owner);

        }

        protected override bool OnUpdate()
        {
            if (Owner == null)
                return true;

            // ���� �̵� ó��.
            Owner.UpdatePathBehavior();

            // �̵��� �Ϸ�Ǿ����� �Ϸ�ó��.
            return Owner.PathNodeManager.IsEmpty();
        }

        protected override void OnExit()
        {
            if (Owner != null)
            {
                // ��ǥ �̵� ó��.
                Owner.UpdateCellPosition(m_cell_to.x, m_cell_to.y, true);

                // �ൿ �÷��� 
                Owner.SetCommandFlag(EnumCommandFlag.Move, true);
            }
        }

    } 
}