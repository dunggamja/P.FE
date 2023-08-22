using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// 유닛
    /// </summary>
    public interface IStatus
    {
        int  GetPoint(EnumUnitPoint _point_type);
        int  GetStatus(EnumUnitStatus _status_type);
        bool HasAttribute(EnumUnitAttribute _attribute_type);

        void SetPoint(EnumUnitPoint _point_type, int _value);
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
    /// BlackBoard 
    /// </summary>
    public interface IBlackBoard
    {
        int  GetValue(EnumBlackBoard _type);
        bool HasValue(EnumBlackBoard _type);
        void SetValue(EnumBlackBoard _type, int _value);
        void SetValue(EnumBlackBoard _type, bool _value);
    }
}
