using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISystemParam
{

}

/// <summary>
/// 시스템
/// </summary>
public interface ISystem
{
    EnumSystem      SystemType  { get; }
    EnumState       State       { get; }
    
    bool IsProgress { get; }
    bool IsFinished { get; }
}

/// <summary>
/// 시스템 매니저
/// </summary>
public interface ISystemManager
{
    bool IsProgress { get; }
    bool IsFinished { get; }

    ISystem GetSystem(EnumSystem _system_type);
}




/// <summary>
/// 소유자
/// </summary>
public interface IOwner
{
    Int64   ID     { get; }
    // ITarget Target { get; }    
}

/// <summary>
/// 타겟 정보
/// </summary>
public interface ITarget
{
    Int64              MainTargetID   { get; }
    IEnumerable<Int64> OtherTargetIDs { get; }
}


/// <summary>
/// 버프/스킬/아이템 발동 조건
/// </summary>
public interface ICondition
{
    bool IsValid(IOwner _owner);
}

/// <summary>
/// 버프/스킬/아이템 효과
/// </summary>
public interface IEffect
{
    void Apply(IOwner _owner);
}

/// <summary>
/// 버프 연산
/// </summary>
public interface IBuff
{
    BuffValue Collect(EnumSituationType _situation, IOwner _owner, EnumBuffStatus _status);
    BuffValue CollectTarget(EnumSituationType _situation, IOwner _owner, EnumBuffStatus _status);
}


/// <summary>
/// 스킬
/// </summary>
public interface ISkill
{
    bool UseSkill(EnumSituationType _situation, IOwner _owner);
}


public interface ISensor 
{
    void Update(IOwner _owner);
}

// public interface IPathNodeManager
// {
//     PathNode Peek();
//     void     Update();
// }

public interface IPathOwner
{
    PathNodeManager  PathNodeManager { get; }
    PathVehicle      PathVehicle     { get; }
    int              PathAttribute   { get; }
    int              PathZOCFaction  { get; }
}

// public interface IItem
// {
//     Int64        ID           { get; }
//     Int32        Kind         { get; }
//     EnumItemType ItemType     { get; }
//     Int32        CurCount     { get; }
//     Int32        MaxCount     { get; }
//     bool         IsDisposable { get; }
// }