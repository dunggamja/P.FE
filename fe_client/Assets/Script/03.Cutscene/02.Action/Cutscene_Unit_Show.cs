using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Unit_Show : Cutscene
{
    private List<Int64> UnitID { get; set; } = new();
    private bool        Show   { get; set; } = false;

    public Cutscene_Unit_Show(CutsceneSequence _sequence, List<Int64> _unit_id, bool _show) : base(_sequence)
    {
        UnitID.AddRange(_unit_id);
        Show   = _show;
    }

    protected override void OnEnter()
    {
        
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        foreach (var unit_id in UnitID)
        {
            EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<WorldObject_ShowEvent>.Acquire()
                .Set(unit_id, Show));
        }

        return UniTask.CompletedTask;        
    }
    
    protected override void OnExit()
    {
        
    }

}
