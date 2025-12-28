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
    Int64       MainTargetID   { get; }
    List<Int64> AllTargetIDList { get; }
}

public interface ITargetPosition
{
    List<(int x, int y)> Positions { get; }
}


/// <summary>
/// Entity Condition
/// </summary>
public interface ICondition
{
    bool Verify_Condition(IOwner _owner);
}

/// <summary>
/// Entity Effect
/// </summary>
public interface IApplier
{
    void Apply_Effect(IOwner _owner);
}


public class ConditionHandler : IPoolObject
{
    private List<ICondition> m_conditions = new();

    public ConditionHandler AddCondition(ICondition _condition)
    {
        m_conditions.Add(_condition);
        return this;
    }

    public void Reset()
    {
        m_conditions.Clear();
    }

    public bool Verify_Condition(IOwner _owner)
    {
        foreach (var condition in m_conditions)
        {
            if (!condition.Verify_Condition(_owner))
                return false;
        }
        return true;
    }
}

public class ApplierHandler : IPoolObject
{
    private List<IApplier> m_on_success = new();
    private List<IApplier> m_on_failure = new();

    public ApplierHandler Add_OnSuccess(IApplier _applier)
    {
        m_on_success.Add(_applier);
        return this;
    }

    public ApplierHandler Add_OnFailure(IApplier _applier)
    {
        m_on_failure.Add(_applier);
        return this;
    }

    public void Reset()
    {
        m_on_success.Clear();
        m_on_failure.Clear();
    }

    public void Apply_Effect(IOwner _owner, bool _is_success)
    {
        if (_is_success)
        {
            foreach (var applier in m_on_success)
            {
                applier.Apply_Effect(_owner);
            }
        }
        else
        {   
            foreach (var applier in m_on_failure)
            {
                applier.Apply_Effect(_owner);
            }
        }
    }
}




// public interface IAIUpdater 
// {
//     bool Verify_Condition(IAIDataManager _owner);

//     void Apply_Effect(IAIDataManager _owner, bool _is_success);

//     bool OnUpdate(IAIDataManager _owner);
// }

// public interface IAIData
// {
//     // TODO: 각 행동마다 필요로 하는 데이터가 다를수 있을 거 같군...
//     //       일단 그 부분은 나중에 생각해보자. 

//     // // 목표 타겟 / 위치가 지정되어 있을 경우
//     // public ITarget         Targets        { get; } // 목표 타겟
//     // public ITargetPosition TargetPosition { get; } // 목표 위치
//     public (int x, int y)  BasePosition { get; }   // 거점
// }

public interface IAIDataManager
{
    Int64             ID     { get; }

    Battle.EnumAIType AIType { get; }

    // IAIData           AIData { get; }
}



// public interface IPathNodeManager
// {
//     PathNode Peek();
//     void     Update();
// }

public interface IPathOwner 
{
    PathNodeManager  PathNodeManager   { get; }
    PathVehicle      PathVehicle       { get; }
    int              PathAttribute     { get; }
    int              PathZOCFaction    { get; }
    (int x, int y)   PathBasePosition  { get; }
    int              PathMoveRange     { get; }

    bool             IsIgnoreZOC(int _faction);
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