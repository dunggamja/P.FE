using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellPosition(int _x, int _y, bool _ignore_prev_position = false)
        {
            // 이전 위치.
            var from_x = Cell.x;
            var from_y = Cell.y;
            
            // 새 위치.
            Cell = (_x, _y);

            var position_cur  = Cell.CellToPosition();
            var position_prev = (_ignore_prev_position) ? Cell.CellToPosition() : PathVehicle.Position;

            PathVehicle.Setup(position_cur, position_prev);

            

            // 이벤트 : Cell
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<CellPositionEvent>.Acquire().Set(
                ID,
                PathZOCFaction,
                Cell,
                (from_x, from_y),
                _ignore_prev_position
            ));

            // 이벤트 : WorldObejct Position
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObjectPositionEvent>.Acquire().Set(
                ID,
                PathVehicle.Position,
                PathVehicle.PositionPrev
            ));
        }

        public void UpdatePathBehavior(float _delta_time)
        {
            PathVehicle.Update(this, _delta_time);
            PathNodeManager.Update(this);

            

            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObjectPositionEvent>.Acquire().Set(
                    ID,
                    PathVehicle.Position,
                    PathVehicle.PositionPrev
                ));
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