using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public void UpdateCellOccupied(bool _is_occupy)
        {
            if (Cell_Occupied == _is_occupy)
                return;

            Cell_Occupied = _is_occupy;

            Debug.Log($"UpdateCellOccupied: {ID}, {Cell}, {_is_occupy}");

            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Cell_PositionEvent>.Acquire().Set(
                ID,
                PathZOCFaction,
                Cell,
                _is_occupy: Cell_Occupied));
        }

        
        public void UpdateCellPosition(
            (int x, int y)        _cell,
            bool                  _visual_immediatly,
            bool                  _is_plan)
        {
            // 이전 위치.            
            Cell_Prev = Cell;


            if (_is_plan == false)
            {
                // 기존에 점유하고 있던 좌표 해제.
                UpdateCellOccupied(false);
            }            
            
            // 새 위치.
            Cell        = _cell;

            if (_is_plan == false)
            {
                // 새로운 위치 셀 점유.
                UpdateCellOccupied(true);

                // 베이스 포지션 변경.
                PathBasePosition = Cell;

                Debug.Log($"PathBasePosition: {ID}, {Cell}");
            }

            var position_cur  = Cell.CellToPosition();
            var position_prev = PathVehicle.Position;

            PathVehicle.Setup(position_cur, position_prev);

            var visual_position_to   = PathVehicle.Position;
            var visual_position_from = (_visual_immediatly) 
                                     ? PathVehicle.Position 
                                     : PathVehicle.PositionPrev;
            

            // 월드 객체 위치 변경 이벤트.
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