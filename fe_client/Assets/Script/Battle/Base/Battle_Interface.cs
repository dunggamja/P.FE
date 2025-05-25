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
        Int64 OwnerID { get; }
        Int64 ItemID  { get; }
        int  GetStatus(EnumWeaponStatus _status_type);
        bool HasAttribute(EnumWeaponAttribute _attribute_type);
        void Equip(Int64 _item_id);
        void Unequip();
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
    // public interface IBlackBoard
    // {
    //     int  GetValue(EnumEntityBlackBoard _type);
    //     bool HasValue(EnumEntityBlackBoard _type);
    //     void SetValue(EnumEntityBlackBoard _type, int _value);
    //     void SetValue(EnumEntityBlackBoard _type, bool _value);
    //     void SetBitFlag(EnumEntityBlackBoard _type, byte _bit_index);
    //     void ResetBitFlag(EnumEntityBlackBoard _type, byte _bit_index);
    //     bool HasBitFlag(EnumEntityBlackBoard _type, byte _bit_index);
    // }

    /// <summary>
    /// 진영 정보.
    /// </summary>
    public interface IFaction
    {
        int  GetFaction();
        void SetFaction(int _faction);
    }

    /// <summary>
    /// 유닛의 제어
    /// </summary>
    public interface ICommand
    {
        // bool IsCommandDirty { get; }
        // bool IsCommandAbort { get; }

        // void SetCommandDirty(bool _is_dirty);
        // void SetCommandAbort(bool _is_abort);

        // EnumCommandOwner  GetCommandOwner();
        // void              SetCommandOwner(EnumCommandOwner _command_owner);

        // bool             HasCommandFlag(); 
        // void             SetCommandEnable(bool _enable); 
    }


    
}
