using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public class  CutsceneCondition_Blackboard : CutsceneCondition
{

    public int                   BlackboardKey { get; private set; } = 0;
    public EnumBlackBoardCompare CompareType   { get; private set; } = EnumBlackBoardCompare.Equal;
    public Int64                 Value         { get; private set; } = 0;

    public CutsceneCondition_Blackboard(int _blackboard_key, EnumBlackBoardCompare _compare_type, Int64 _value = 0)
    {
        BlackboardKey = _blackboard_key;
        CompareType   = _compare_type;
        Value         = _value;
    }

    public override bool Verify(CutsceneSequence _sequence)
    {
       return BattleSystemManager.Instance.BlackBoard.CompareValue(EnumBattleBlackBoard.CommandEntityID_Input, CompareType, Value);
    }
}