using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellPosition(int _x, int _y)
        {
            // ���� ��ġ.
            var from_x = Cell.x;
            var from_y = Cell.y;
            
            // �� ��ġ.
            Cell = (_x, _y);

            // ��� ����.
            var terrain_map  = PathNodeManager.TerrainMap;
            if (terrain_map != null)
            {
                terrain_map.BlockManager.RefreshEntity(ID, from_x, from_y, _x, _y);                
            }


        }
    }
}