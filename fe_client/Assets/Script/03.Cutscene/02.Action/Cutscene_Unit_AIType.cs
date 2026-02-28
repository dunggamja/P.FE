using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;

public class Cutscene_Unit_AIType : Cutscene
{
    public List<Int64> UnitID { get; private set; } = new();
    public EnumAIType  AIType { get; private set; } = EnumAIType.None;

    public Cutscene_Unit_AIType(CutsceneSequence _sequence, List<Int64> _unit_id, EnumAIType _ai_type) : base(_sequence)
    {
        UnitID.AddRange(_unit_id);
        AIType = _ai_type;
    }

    protected override void OnEnter()
    {
        
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        foreach (var e in UnitID)
        {
            var entity = EntityManager.Instance.GetEntity(e);
            if (entity == null)
                continue;

            entity.SetAIType(AIType);
        }

        return UniTask.CompletedTask;
    }

    protected override void OnExit()
    {
        
    }
}