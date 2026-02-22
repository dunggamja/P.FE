using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Grid_Cursor : Cutscene
{
    public TAG_INFO Tag {get; private set;} = TAG_INFO.Create(EnumTagType.None, 0);

    public Cutscene_Grid_Cursor(CutsceneSequence _sequence, TAG_INFO _tag) : base(_sequence)
    {
        Tag = _tag;
    }

    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        // Tag 정보를 통해 포지션을 가져옵니다.
        var position = TagHelper.Peek_Position(Tag);

        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<Grid_Cursor_Event>.Acquire()
            .Set(position));

        return UniTask.CompletedTask;
    }

    protected override void OnExit()
    {
    }


}