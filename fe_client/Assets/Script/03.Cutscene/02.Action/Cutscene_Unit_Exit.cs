using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;



public class Cutscene_Unit_Exit : Cutscene
{
    private List<Int64> UnitID { get; set; } = new();

    public Cutscene_Unit_Exit(CutsceneSequence _sequence, List<Int64> _unit_id) : base(_sequence)
    {
        UnitID.AddRange(_unit_id);
    }

    protected override void OnEnter()
    {
        
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        foreach (var unit_id in UnitID)
        {
            var entity = EntityManager.Instance.GetEntity(unit_id);
            if (entity == null)
                continue;

            // TODO: 이탈 관련 연출 처리 필요.
            entity.ApplyExit();
        }

        

        return UniTask.CompletedTask;        
    }
    
    protected override void OnExit()
    {
        
    }

}
