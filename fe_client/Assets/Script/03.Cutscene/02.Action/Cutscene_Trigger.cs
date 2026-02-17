using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;

public class Cutscene_LocalTrigger : Cutscene
{
    public int  TriggerID { get; private set; } = 0;
    public bool IsSet     { get; private set; } = false;    
    
    protected int BlackBoardKey
    {
        get
        {
            var key = (int)EnumCutsceneLocalMemory.Trigger_Begin + TriggerID;
            if (key < (int)EnumCutsceneLocalMemory.Trigger_Begin
            ||  key > (int)EnumCutsceneLocalMemory.Trigger_End)
            {
                Debug.LogError($"Cutscene_LocalTrigger: TriggerID:{TriggerID}, range: {(int)EnumCutsceneLocalMemory.Trigger_End - (int)EnumCutsceneLocalMemory.Trigger_Begin}");
                return -1;
            }

            return key;
        }
    }

    public Cutscene_LocalTrigger(CutsceneSequence _sequence, int _trigger_id, bool _is_wait) : base(_sequence)
    {
        TriggerID = _trigger_id;
        IsSet     = _is_wait;
    }

    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }
    
    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        if (IsSet)
        {
            // 트리거 셋팅
            Sequence.Memory.SetValue(BlackBoardKey, true);
        }
        else
        {
            // 트리거가 실행될때까지 대기.
            await UniTask.WaitUntil(() => Sequence.Memory.HasValue(BlackBoardKey), cancellationToken: _skip_token);
        }  
    }

    protected override void OnExit()
    {
        // throw new NotImplementedException();
    }
}

public class Cutscene_Trigger : Cutscene
{
    // public override EnumCutsceneType Type => EnumCutsceneType.Trigger;

    public int      TriggerID { get; private set; } = 0;
    public bool     IsSet     { get; private set; } = false;    

    protected int BlackBoardKey
    {
        get
        {
            var key = (int)EnumCutsceneGlobalMemory.Trigger_Begin + TriggerID;
            if (key < (int)EnumCutsceneGlobalMemory.Trigger_Begin
            ||  key > (int)EnumCutsceneGlobalMemory.Trigger_End)
            {
                Debug.LogError($"Cutscene_Trigger: TriggerID:{TriggerID}, range: {(int)EnumCutsceneGlobalMemory.Trigger_End - (int)EnumCutsceneGlobalMemory.Trigger_Begin}");
                return -1;
            }

            return key;
        }
    }

    public Cutscene_Trigger(CutsceneSequence _sequence, int _trigger_id, bool _is_wait) : base(_sequence)
    {
        TriggerID = _trigger_id;
        IsSet     = _is_wait;
    }



    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }
    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        if (IsSet)
        {
            // 트리거 실행.
            CutsceneManager.Instance.Memory.SetValue(BlackBoardKey, true);
        }
        else
        {
            // 트리거가 실행될때까지 대기.
            await UniTask.WaitUntil(() => CutsceneManager.Instance.Memory.HasValue(BlackBoardKey), cancellationToken: _skip_token);
        }        
    }

    protected override void OnExit()
    {
        // throw new NotImplementedException();
    }

}
