using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

    /// <summary>
    /// 버프/스킬 발동 조건
    /// </summary>
    public interface ICondition
    {
        bool IsValid(BattleObject _owner);
    }

    /// <summary>
    /// 버프/스킬 효과
    /// </summary>
    public interface IEffect
    {
        void Apply(BattleObject _owner);
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

    /// <summary>
    /// 버프 연산
    /// </summary>
    public interface IBuff
    {
        BuffValue Calculate(BattleObject _owner, EnumBuffStatus _status);
    }

    

    


    public interface ICommand
    {
        void Do();
        void Undo();
    }

}
