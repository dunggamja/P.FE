using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoard_IO
{
    public BaseContainer_IO Values { get; set; } = new();

}

// public class EntityBlackBoardSnapshot
// {
//     public BlackBoardSnapshot BlackBoard { get; set; } = new();

//     // ai_attack.score_result

//     public static EntityBlackBoardSnapshot Create(
//         BlackBoardSnapshot _black_board)
//     {
//         var snapshot        = new EntityBlackBoardSnapshot();
//         snapshot.BlackBoard = _black_board;
//         return snapshot;
//     }
// }




public class UnitStatus_IO
{
    public BaseContainer_IO Point     { get; set; } = new();
    public BaseContainer_IO Status    { get; set; } = new();
    public BaseContainer_IO Attribute { get; set; } = new();
}

public class BuffManager_IO
{
    public List<Buff> Buffs { get; set; } = new();
}

public class Weapon_IO
{
    public Int64 OwnerID { get; set; }
    public Int64 ItemID  { get; set; }

}

public class StatusManager_IO
{
    public UnitStatus_IO  UnitStatus  { get; set; } = new();
    public BuffManager_IO BuffManager { get; set; } = new();
    public Weapon_IO      Weapon      { get; set; } = new();

}