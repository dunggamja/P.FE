using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {

        public void UpdatePathBasePosition()
        {
            PathBasePosition = Cell;
        }

        public void UpdateCellOccupied(bool _is_occupy)
        {
            // // 점유상태가 동일하면 이벤트 발생하지 않음.
            // if (Cell_Occupied == _is_occupy)
            //     return;

            // 점유상태 변경시 이벤트 발생.
            var prev_occupied = Cell_Occupied;
            var cur_occupied  = _is_occupy;

            var prev_has_zoc  = HasZOC_Last;
            var cur_has_zoc   = PathHasZOC;

            // 이전과 동일한 상태면 처리할 필요 없음.
            if (prev_occupied == cur_occupied && prev_has_zoc == cur_has_zoc)
                return;


            Cell_Occupied = _is_occupy;
            HasZOC_Last   = PathHasZOC;

            // Debug.Log($"UpdateCellOccupied: {ID}, {Cell}, {_is_occupy}");

            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Cell_OccupyEvent>.Acquire().Set(
                ID,
                PathZOCFaction,
                Cell,
                _is_occupy: (prev_occupied, cur_occupied),
                _has_zoc:   (prev_has_zoc, cur_has_zoc))
                );
        }

        
        public void UpdateCellPosition(
            (int x, int y)                  _cell,
            (bool _apply, bool _immediatly) _visual_update)
            // ,            bool                            _is_plan = false)
        {
            // 기존 셀.            
            Cell_Prev = Cell;


            // if (_is_plan == false)
            {
                // 선택 셀 점유 해제.
                UpdateCellOccupied(false);
            }            
            
            // 선택 셀.
            Cell        = _cell;

            // 
            // if (_is_plan == false)
            {
                // 선택 셀 점유 셋팅.
                UpdateCellOccupied(true);
            }

            // 선택 셀 위치.
            var position_cur  = Cell.CellToPosition();
            var position_prev = PathVehicle.Position;

            // PathVehicle 갱신.
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

        public bool PathIgnoreZOC(int _faction)
        {
            // 현재는 비병이 ZOC 체크를 하지 않기 위해서 사용.
            // TODO: 아니면 비병용 ZOC 레이어를 나누는 것이 좋을수도 있을듯...
            if (PathHasZOC == false)
                return true;            

            // 아군인 경우 제외.
            if (BattleSystemManager.Instance.IsAlly(GetFaction(), _faction))
                return true;

            return false;
        }

        /// 맵에서 이탈 처리.
        /// TODO: 이탈 관련 연출 처리 필요
        public void ApplyExit()
        {
            // 이미 이탈한 상태인지 체크.
            if (IsExit)
                return;

            // 모든 행동 종료 처리.
            SetAllCommandDone();

            // 좌표 점유 해제.
            UpdateCellOccupied(false);

            // 이탈 상태 셋팅.
            BlackBoard.SetValue(EnumEntityBlackBoard.Exited, true);

            // HUD 삭제.
            DeleteHUD();

            // 안보이게 표시.
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObject_ShowEvent>.Acquire().Set(ID, false));
        }
    }
}