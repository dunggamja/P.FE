using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Delay : Cutscene
{
    public float WaitTime { get; private set; } = 0f;

    public Cutscene_Delay(CutsceneSequence _sequence, float _wait_time) : base(_sequence)
    {
        WaitTime = _wait_time;
    }

    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }
    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime), cancellationToken: _skip_token);
    }

    protected override void OnExit()
    {
        
    }

}