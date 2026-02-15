using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Unit_Move : Cutscene
{
    // public override EnumCutsceneType Type => EnumCutsceneType.Unit_Move;

    public Int64          UnitID             { get; private set; } = 0;
    public (int x, int y) StartPosition      { get; private set; } = (0, 0);
    public (int x, int y) EndPosition        { get; private set; } = (0, 0);
    public bool           UpdateCellPosition { get; private set; } = false;
    // public bool           IsMoveForced     { get; private set; } = false;
    public Cutscene_Unit_Move(CutsceneSequence _sequence,
    Int64          _unit_id,
    (int x, int y) _start_position,
    (int x, int y) _end_position) : base(_sequence)
    {
        UnitID        = _unit_id;
        StartPosition = _start_position;
        EndPosition   = _end_position;
    }


    protected override void OnEnter()
    {
        var unit = EntityManager.Instance.GetEntity(UnitID);
        if (unit == null)
            return;

        // 길찾기 경로 생성.
        unit.PathNodeManager.CreatePath(
            StartPosition.CellToPosition(), 
            EndPosition.CellToPosition(), 
            unit,
            _is_force_move: true);
    }

    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        var unit = EntityManager.Instance.GetEntity(UnitID);
        if (unit == null)
            return;

        // 유닛 이동 실행.
        while(unit.PathNodeManager.IsEmpty() == false)
        {
            unit.UpdatePathBehavior(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL);
            await UniTask.Delay((int)(Constants.BATTLE_SYSTEM_UPDATE_INTERVAL * 1000f), cancellationToken: _skip_token);
        }
    }

    protected override void OnExit()
    {
        var unit = EntityManager.Instance.GetEntity(UnitID);
        if (unit == null)
            return;


        if (UpdateCellPosition)
        {
            // 유닛 위치 적용.
            unit.UpdateCellPosition(
                EndPosition,
                (_apply: true, _immediatly: true),
                _is_plan: false);

        }

    }
}