using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellPosition(
            int                 _x, 
            int                 _y,
            EnumCellOccupyEvent _cell_occupy_event,
            bool                _is_immediatly_move = false)
        {
            // 이전 위치.            
            Cell_Prev = Cell;
            
            // 새 위치.
            Cell        = (_x, _y);

            

            var position_cur  = Cell.CellToPosition();
            var position_prev = PathVehicle.Position;

            PathVehicle.Setup(position_cur, position_prev);


            if (_cell_occupy_event == EnumCellOccupyEvent.Leave
            ||  _cell_occupy_event == EnumCellOccupyEvent.Change)
            {
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Cell_OccupyEvent>.Acquire().Set(
                    ID,
                    PathZOCFaction,
                    Cell_Prev,
                    _is_enter: false));
            }

            if (_cell_occupy_event == EnumCellOccupyEvent.Enter
            ||  _cell_occupy_event == EnumCellOccupyEvent.Change)
            {
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Cell_OccupyEvent>.Acquire().Set(
                    ID,
                    PathZOCFaction,
                    Cell,
                    _is_enter: true));
            }


            var visual_position_to   = PathVehicle.Position;
            var visual_position_from = (_is_immediatly_move) 
                                     ? PathVehicle.Position 
                                     : PathVehicle.PositionPrev;
            

            // 이벤트 : 렌더링
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