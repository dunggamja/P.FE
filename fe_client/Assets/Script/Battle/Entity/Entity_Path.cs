using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellPosition(
            int                   _x, 
            int                   _y,
            EnumCellPositionEvent _cell_position_event,
            bool                  _is_immediatly_move = false,
            bool                  _is_path_completed  = false)
        {
            // ���� ��ġ.            
            Cell_Prev = Cell;
            
            // �� ��ġ.
            Cell        = (_x, _y);

            var position_cur  = Cell.CellToPosition();
            var position_prev = PathVehicle.Position;

            PathVehicle.Setup(position_cur, position_prev);

            // ���� ��ġ �� ���� ����. 
            if (_cell_position_event == EnumCellPositionEvent.Exit
            ||  _cell_position_event == EnumCellPositionEvent.Move)
            {
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Cell_PositionEvent>.Acquire().Set(
                    ID,
                    PathZOCFaction,
                    Cell_Prev,
                    _is_occupy: false));
            }


            // ���ο� ��ġ �� ����.
            if (_cell_position_event == EnumCellPositionEvent.Enter
            ||  _cell_position_event == EnumCellPositionEvent.Move)
            {
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Cell_PositionEvent>.Acquire().Set(
                    ID,
                    PathZOCFaction,
                    Cell,
                    _is_occupy: true));
            }

            //
            if (_is_path_completed)
            {
                PathBasePosition = Cell;
            }


            var visual_position_to   = PathVehicle.Position;
            var visual_position_from = (_is_immediatly_move) 
                                     ? PathVehicle.Position 
                                     : PathVehicle.PositionPrev;
            

            // ���� ��ü ��ġ ���� �̺�Ʈ.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObject_PositionEvent>.Acquire().Set(
                ID,
                visual_position_to,
                visual_position_from
            ));
        }

        public void UpdatePathBehavior(float _delta_time)
        {
            PathVehicle.Update(this, _delta_time);
            PathNodeManager.Update(this);

            

            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObject_PositionEvent>.Acquire().Set(
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