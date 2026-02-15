using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;



public class Cutscene_Trigger : Cutscene
{
    // public override EnumCutsceneType Type => EnumCutsceneType.Trigger;

    public int      TriggerID { get; private set; } = 0;
    public bool     IsWait    { get; private set; } = false;

    protected int BlackBoardKey
    {
        get
        {
            var key = (int)EnumCutsceneGlobalMemory.Trigger_Begin + TriggerID;
            if (key < (int)EnumCutsceneGlobalMemory.Trigger_Begin
            ||  key > (int)EnumCutsceneGlobalMemory.Trigger_End)
            {
                Debug.LogError($"Cutscene_Trigger: BlackBoard_Index is out of range: {key}");
                return 0;
            }

            return key;
        }
    }

    public Cutscene_Trigger(CutsceneSequence _sequence, int _trigger_id, bool _is_wait) : base(_sequence)
    {
        TriggerID = _trigger_id;
        IsWait    = _is_wait;
    }



    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }
    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        if (IsWait)
        {
            // 트리거가 실행될때까지 대기.
            await UniTask.WaitUntil(() => CutsceneManager.Instance.Memory.HasValue(BlackBoardKey), cancellationToken: _skip_token);
        }
        else
        {
            // 트리거 실행.
            CutsceneManager.Instance.Memory.SetValue(BlackBoardKey, true);
        }        
    }

    protected override void OnExit()
    {
        // throw new NotImplementedException();
    }

}
