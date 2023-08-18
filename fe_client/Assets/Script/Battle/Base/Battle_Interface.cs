using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// 시스템
    /// </summary>
    public interface ISystem
    {
        EnumSystem     SystemType    { get; }
        EnumState      State         { get; }
        ISystemManager SystemManager { get; }
    }


    public interface ISystemManager
    {
        bool    IsProgress { get; }
        bool    IsFinished { get; }

        ISystem GetSystem(EnumSystem _system_type);
    }

    public interface ITarget
    {
        int                TargetCount   { get; }
        IEnumerable<Int64> TargetIDs     { get; }
        Int64              FirstTargetID { get; }
    }


    /// <summary>
    /// 소유자
    /// </summary>
    public interface IOwner
    {
        Int64    ID     { get; }
        ITarget  Target { get; }
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
        BuffValue Collect(ISystem _system, BattleObject _owner, EnumBuffStatus _status);
    }


    /// <summary>
    /// 유닛
    /// </summary>
    public interface IUnit
    {
        int  GetStatus(EnumUnitStatus _status_type);
        bool HasAttribute(EnumUnitAttribute _attribute_type);
    }

    /// <summary>
    /// 무기
    /// </summary>
    public interface IWeapon
    {
        int  GetStatus(EnumWeaponStatus _status_type);
        bool HasAttribute(EnumWeaponAttribute _attribute_type);
    }

    /// <summary>
    /// 지형효과
    /// </summary>
    public interface ITerrain
    {
        bool HasAttribute(EnumTerrainAttribute _attribute_type);
    }

    //public interface ICommand
    //{
    //    void Do();
    //    void Undo();
    //}

}
