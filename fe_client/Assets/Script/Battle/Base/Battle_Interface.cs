using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    


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
