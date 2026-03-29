using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CutsceneCondition_Trigger : CutsceneCondition
{
    public int  TriggerID { get; private set; } = 0;
    public bool HasTrigger { get; private set; } = false;


    protected int BlackBoardKey
    {
        get
        {
            var key = (int)EnumCutsceneGlobalMemory.Trigger_Begin + TriggerID;
            if (key < (int)EnumCutsceneGlobalMemory.Trigger_Begin
            ||  key > (int)EnumCutsceneGlobalMemory.Trigger_End)
            {
                Debug.LogError($"CutsceneCondition_Trigger: TriggerID:{TriggerID}, range: {(int)EnumCutsceneGlobalMemory.Trigger_End - (int)EnumCutsceneGlobalMemory.Trigger_Begin}");
                return -1;
            }

            return key;
        }
    }


    public CutsceneCondition_Trigger(int _trigger_id, bool _has_trigger)
    {
        TriggerID  = _trigger_id;
        HasTrigger = _has_trigger;
    }

    public override bool Verify(CutsceneSequence _sequence)
    {
        return HasTrigger == CutsceneManager.Instance.Memory.HasValue(BlackBoardKey);
    }
}