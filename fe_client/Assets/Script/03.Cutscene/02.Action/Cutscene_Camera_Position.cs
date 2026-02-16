using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;



public class Cutscene_Camera_Position : Cutscene
{
    public (int x, int y) Position { get; private set; } = (0, 0);

    public Cutscene_Camera_Position(CutsceneSequence _sequence, (int x, int y) _position) : base(_sequence)
    {
        Position = _position;
    }

    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        EventDispatchManager.Instance.UpdateEvent(
               ObjectPool<Battle_Camera_PositionEvent>.Acquire()
               .SetCell(Position));

        return UniTask.CompletedTask;        
    }

    protected override void OnExit()
    {
        // throw new NotImplementedException();
    }


}