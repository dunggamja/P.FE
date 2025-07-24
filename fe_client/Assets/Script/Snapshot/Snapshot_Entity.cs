using System;
using System.Collections.Generic;
using UnityEngine;


public class EntitySnapshot
{
    public Int64          ID        { get; private set; } = 0;
    public (int x, int y) Cell      { get; private set; } = (0, 0);
    public (int x, int y) Cell_Prev { get; private set; } = (0, 0);

    public EntityBlackBoardSnapshot BlackBoard    { get; private set; } = new();
    public StatusManagerSnapshot    StatusManager { get; private set; } = new();
    public InventorySnapshot        Inventory     { get; private set; } = new();

    // public static EntitySnapshot Create(Entity _entity)
    // public List<BuffSnapshot> Buffs { get; set; }
    

}