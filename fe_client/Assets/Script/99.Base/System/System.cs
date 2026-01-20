using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class System<T> : ISystem where T : ISystemParam
{
    public EnumSystem      SystemType  { get; private set;   } = EnumSystem.None;
    public EnumState       State       { get; private set;   } = EnumState.None;
    // public EnumSystemTiming SkillTiming { get; protected set; } = EnumSystemTiming.None; 

    //public abstract ISystemManager SystemManager { get; }

    public bool IsProgress => State == EnumState.Progress;
    public bool IsFinished => State == EnumState.Finished;


    protected System(EnumSystem _system_type)
    {
        SystemType = _system_type;
    }

    public    void Init()
    {
        State = EnumState.None;
        OnInit();
    }
    public    void Release()
    {
        State = EnumState.None;  
        OnRelease();
    }

    protected abstract void OnInit();
    protected abstract void OnRelease();

    protected abstract void OnEnter(T _param);
    protected abstract bool OnUpdate(T _param);
    protected abstract void OnExit(T _param);

    public EnumState Update(T _param)
    {
        if (State != EnumState.Progress)
        {
            State = EnumState.Progress;
            OnEnter(_param);
        }

        if (OnUpdate(_param))
        {
            State = EnumState.Finished;
        }

        if (State != EnumState.Progress)
        {
            OnExit(_param);
        }

        return State;
    }
}
