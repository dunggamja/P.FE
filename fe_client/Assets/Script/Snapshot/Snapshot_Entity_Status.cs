using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardSnapshot
{
    public ContainerSnapshot Values { get; set; } = new();

    public static BlackBoardSnapshot Create(
        ContainerSnapshot _values
        )
    {
        var snapshot    = new BlackBoardSnapshot();
        snapshot.Values = _values;
        return snapshot;
    }
}

public class EntityBlackBoardSnapshot
{
    public BlackBoardSnapshot BlackBoard { get; set; } = new();

    // ai_attack.score_result

    public static EntityBlackBoardSnapshot Create(
        BlackBoardSnapshot _black_board)
    {
        var snapshot        = new EntityBlackBoardSnapshot();
        snapshot.BlackBoard = _black_board;
        return snapshot;
    }
}




public class UnitStatusSnapshot
{
    public ContainerSnapshot Point     { get; set; } = new();
    public ContainerSnapshot Status    { get; set; } = new();
    public ContainerSnapshot Attribute { get; set; } = new();


    public static UnitStatusSnapshot Create(
        IEnumerable<(int key, int value)> _point,
        IEnumerable<(int key, int value)> _status,
        IEnumerable<(int key, int value)> _attribute)
    {
        var snapshot       = new UnitStatusSnapshot();
        snapshot.Point     = ContainerSnapshot.Create(_point);
        snapshot.Status    = ContainerSnapshot.Create(_status);
        snapshot.Attribute = ContainerSnapshot.Create(_attribute);
        return snapshot;
    }
}

public class BuffManagerSnapshot
{
    public List<Buff> Buffs { get; set; } = new();
}

public class WeaponSnapshot
{
    public Int64 OwnerID { get; set; }
    public Int64 ItemID  { get; set; }

    public static WeaponSnapshot Create(Int64 _owner_id, Int64 _item_id)
    {
        var snapshot     = new WeaponSnapshot();
        snapshot.OwnerID = _owner_id;
        snapshot.ItemID  = _item_id;
        return snapshot;
    }
}

public class StatusManagerSnapshot
{
    public UnitStatusSnapshot  UnitStatus  { get; set; } = new();
    public BuffManagerSnapshot BuffManager { get; set; } = new();
    public WeaponSnapshot      Weapon      { get; set; } = new();

    public static StatusManagerSnapshot Create(
        UnitStatusSnapshot  _unit_status,
        BuffManagerSnapshot _buff_manager,
        WeaponSnapshot      _weapon)
    {
        var snapshot         = new StatusManagerSnapshot();
        snapshot.UnitStatus  = _unit_status;
        snapshot.BuffManager = _buff_manager;
        snapshot.Weapon      = _weapon;
        return snapshot;
    }
}