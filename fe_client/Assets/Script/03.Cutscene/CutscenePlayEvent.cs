using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


public struct CutscenePlayEvent : IEquatable<CutscenePlayEvent>
{
    public EnumCutscenePlayEvent Event;

    public Int64 Value1;
    public Int64 Value2;


    public static CutscenePlayEvent Create(EnumCutscenePlayEvent _event, Int64 _value1 = 0, Int64 _value2 = 0)
    {
        return new CutscenePlayEvent()
        {
            Event  = _event,
            Value1 = _value1,
            Value2 = _value2,
        };
    }

    public static bool operator ==(CutscenePlayEvent _left, CutscenePlayEvent _right)
    {
        return _left.Event == _right.Event && _left.Value1 == _right.Value1 && _left.Value2 == _right.Value2;
    }

    public static bool operator !=(CutscenePlayEvent _left, CutscenePlayEvent _right)
    {
        return !(_left == _right);
    }

    public bool Equals(CutscenePlayEvent other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is CutscenePlayEvent _event)
            return Equals(_event);

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Event, Value1, Value2);
    }
}