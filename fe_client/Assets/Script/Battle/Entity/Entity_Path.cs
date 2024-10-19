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
            // 이전 위치.
            var from_x = Cell.x;
            var from_y = Cell.y;
            
            // 새 위치.
            Cell = (_x, _y);

            // 블록 갱신.
            var terrain_map  = PathNodeManager.TerrainMap;
            if (terrain_map != null)
            {
                terrain_map.BlockManager.RefreshEntity(ID, from_x, from_y, _x, _y);                
            }


        }
    }
}