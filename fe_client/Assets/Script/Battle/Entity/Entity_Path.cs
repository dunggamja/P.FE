using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellPosition(int _x, int _y, bool _setup_path_vehicle = false)
        {
            // ���� ��ġ.
            var from_x = Cell.x;
            var from_y = Cell.y;
            
            // �� ��ġ.
            Cell = (_x, _y);

            // �� ���� ����.
            var terrain_map  = PathNodeManager.TerrainMap;
            if (terrain_map != null)
            {
                // ��ġ ����.
                terrain_map.BlockManager.RefreshEntity(ID, from_x, from_y, _x, _y);                

                // ZOC ����.
                terrain_map.ZOC.DecreaseZOC(PathZOCFaction, from_x, from_y);
                terrain_map.ZOC.IncreaseZOC(PathZOCFaction, _x, _y);
            }

            if (_setup_path_vehicle)
            {
                PathVehicle.Setup(Cell.CellToPosition());
            }
        }

        public void UpdatePathBehavior()
        {
            PathVehicle.Update(this, Time.deltaTime);
            PathNodeManager.Update(this);
        }


        public void SetPathAttribute(EnumPathOwnerAttribute _attribute)
        {
            PathAttribute |= 1 << (int)_attribute;
        }

        public void RemovePathAttribute(EnumPathOwnerAttribute _attribute)
        {
            PathAttribute &= ~(1 << (int)_attribute);
        }
    }
}