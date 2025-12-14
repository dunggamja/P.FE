using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {

        // public void RefreshCellOccupied()
        // {

        // }

        public void UpdateCellOccupied(bool _is_occupy)
        {
            if (Cell_Occupied == _is_occupy)
                return;

            Cell_Occupied = _is_occupy;

            // Debug.Log($"UpdateCellOccupied: {ID}, {Cell}, {_is_occupy}");

            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Cell_OccupyEvent>.Acquire().Set(
                ID,
                PathZOCFaction,
                Cell,
                _is_occupy: Cell_Occupied));
        }

        
        public void UpdateCellPosition(
            (int x, int y)                  _cell,
            (bool _apply, bool _immediatly) _visual_update,
            bool                            _is_plan)
        {
            // 기존 셀.            
            Cell_Prev = Cell;


            if (_is_plan == false)
            {
                // 선택 셀 점유 해제.
                UpdateCellOccupied(false);
            }            
            
            // 선택 셀.
            Cell        = _cell;

            // 
            if (_is_plan == false)
            {
                // 선택 셀 점유 셋팅.
                UpdateCellOccupied(true);

                // 선택 셀 기준 위치.
                PathBasePosition = Cell;

                // Debug.Log($"PathBasePosition: {ID}, {Cell}");
            }

            // 선택 셀 위치.
            var position_cur  = Cell.CellToPosition();
            var position_prev = PathVehicle.Position;

            PathVehicle.Setup(position_cur, position_prev);

            // visual 선택 효과 생성.
            if (_visual_update._apply)
            {
                var visual_position_to   = PathVehicle.Position;
                var visual_position_from = (_visual_update._immediatly) 
                                        ? PathVehicle.Position 
                                        : PathVehicle.PositionPrev;                

                // 선택 셀 위치 이벤트 생성.
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<WorldObject_PositionEvent>.Acquire().Set(
                    ID,
                    visual_position_to,
                    visual_position_from
                ));
            }

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


        // public void SetPathAttribute(EnumPathOwnerAttribute _attribute)
        // {
        //     PathAttribute |= 1 << (int)_attribute;
        // }

        // public void RemovePathAttribute(EnumPathOwnerAttribute _attribute)
        // {
        //     PathAttribute &= ~(1 << (int)_attribute);
        // }

        public bool IsIgnoreZOC(int _faction)
        {
            // 아군인 경우 제외.
            return BattleSystemManager
                .Instance
                .IsAlly(GetFaction(), _faction);
        }

    }
}