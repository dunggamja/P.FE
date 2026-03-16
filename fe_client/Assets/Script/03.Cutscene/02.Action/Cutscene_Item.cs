using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Item : Cutscene
{
    public Cutscene_Item(CutsceneSequence _sequence) : base(_sequence)
    {
    }

    protected override void OnEnter()
    {
        throw new NotImplementedException();
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        throw new NotImplementedException();
    }

    protected override void OnExit()
    {
        throw new NotImplementedException();
    }


}