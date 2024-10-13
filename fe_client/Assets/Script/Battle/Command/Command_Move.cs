using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
   public class Command_Move : Command
    {
        (int x, int y)        m_cell_from       = (0, 0);
        (int x, int y)        m_cell_to         = (0, 0); 
        // List<(int x, int y)>  m_path_node       = new List<(int x, int y)>();
        // int                   m_path_node_index = 0;

        public Command_Move(Int64 _unit_id) : base(_unit_id)
        {
            
        }
        

        protected override void OnEnter()
        {
            var unit_object = EntityManager.Instance.GetEntity(UnitID);
            if (unit_object == null)
                return;

            // ½ΓΐΫ ΐ§Δ‘.
            m_cell_from = unit_object.Cell;
        }

        protected override bool OnUpdate()
        {
            var unit_object = EntityManager.Instance.GetEntity(UnitID);
            if (unit_object == null)
                return true;

            // for(; m_path_node_index < m_path_node.Count; ++m_path_node_index)
            // {
            //     var path_node = m_path_node[m_path_node_index];
            //     var diff_x    = Math.Clamp(path_node.x - unit_object.Cell.x, -1, 1);
            //     var diff_y    = Math.Clamp(path_node.y - unit_object.Cell.y, -1, 1);

            //     // TODO: ?Ό?¨ Update ?λ²μ 1μΉΈμ© ???μ§μ΄κ²? ?΄??€. ?μ€μ ?? .
            //     // if (diff_x != 0)
                    

            //     //if (diff_x != 0)
            //     //    unit_object

            // }

            return true;
        }

        protected override void OnExit()
        {
            
        }

    } 
}