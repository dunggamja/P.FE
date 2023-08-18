using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시스템
/// </summary>
public interface ISystem
{
    EnumSystem     SystemType { get; }
    EnumState      State { get; }
    ISystemManager SystemManager { get; }
}


public interface ISystemManager
{
    bool IsProgress { get; }
    bool IsFinished { get; }

    ISystem GetSystem(EnumSystem _system_type);
}

public interface ITarget
{
    int TargetCount { get; }
    IEnumerable<Int64> TargetIDs { get; }
    Int64 FirstTargetID { get; }
}


/// <summary>
/// 소유자
/// </summary>
public interface IOwner
{
    Int64 ID { get; }
    ITarget Target { get; }
}

/// <summary>
/// 버프/스킬 발동 조건
/// </summary>
public interface ICondition
{
    bool IsValid(ISystem _system, IOwner _owner);
}

/// <summary>
/// 버프/스킬 효과
/// </summary>
public interface IEffect
{
    void Apply(ISystem _system, IOwner _owner);
}

/// <summary>
/// 버프 연산
/// </summary>
public interface IBuff
{
    BuffValue Collect(ISystem _system, IOwner _owner, EnumBuffStatus _status);
}