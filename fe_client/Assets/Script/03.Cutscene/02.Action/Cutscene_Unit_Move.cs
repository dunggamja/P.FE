using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public struct UNIT_MOVE_DATA
{
    public Int64          UnitID;
    public (int x, int y) StartPosition;
    public (int x, int y) EndPosition;
}


public class Cutscene_Unit_Move : Cutscene
{
    // public override EnumCutsceneType Type => EnumCutsceneType.Unit_Move;

    public List<UNIT_MOVE_DATA> UnitMoveData       { get; private set; } = new();
    public bool                 UpdateCellPosition { get; private set; } = false;
    // public bool           IsMoveForced     { get; private set; } = false;
    public Cutscene_Unit_Move(
        CutsceneSequence     _sequence,
        List<UNIT_MOVE_DATA> _unit_move_data,
        bool                 _update_cell_position) : base(_sequence)
    {
        if (_unit_move_data != null)
            UnitMoveData.AddRange(_unit_move_data);
        else 
            UnitMoveData.Clear();

        UpdateCellPosition = _update_cell_position;
    }


    protected override void OnEnter()
    {
        foreach (var unit_move_data in UnitMoveData)
        {
            var unit = EntityManager.Instance.GetEntity(unit_move_data.UnitID);
            if (unit == null)
                continue;

            // 길찾기 경로 생성.
            unit.PathNodeManager.CreatePath(
                unit_move_data.StartPosition.CellToPosition(), 
                unit_move_data.EndPosition.CellToPosition(), 
                unit, _is_force_move: true);
        }
    }

    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        // 모든 유닛의 이동이 완료될때까지 대기.
        await UniTask.WhenAll(UnitMoveData.Select(async e => 
        {
            var unit = EntityManager.Instance.GetEntity(e.UnitID);
            if (unit == null)
                return;

            while(unit.PathNodeManager.IsEmpty() == false)
            {
                unit.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);
                await UniTask.Delay((int)(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL * 1000f), cancellationToken: _skip_token);
            }
        }));


        // var unit = EntityManager.Instance.GetEntity(UnitID);
        // if (unit == null)
        //     return;

        // // 유닛 이동 실행.
        // while(unit.PathNodeManager.IsEmpty() == false)
        // {
        //     unit.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);
        //     await UniTask.Delay((int)(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL * 1000f), cancellationToken: _skip_token);
        // }
    }

    protected override void OnExit()
    {
        

        if (UpdateCellPosition)
        {
            // 유닛 위치 적용.
            foreach (var unit_move_data in UnitMoveData)
            {
                var unit = EntityManager.Instance.GetEntity(unit_move_data.UnitID);
                if (unit == null)
                    continue;

                unit.UpdateCellPosition(
                    unit_move_data.EndPosition,
                    (_apply: true, _immediatly: true),
                    _is_plan: false);                
            }
            

        }

    }
}