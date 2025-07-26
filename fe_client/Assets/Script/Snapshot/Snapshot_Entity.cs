using System;
using System.Collections.Generic;
using UnityEngine;


public class EntitySnapshot
{
    public Int64                    ID               { get; private set; } = 0;
    public (int x, int y)           Cell             { get; private set; } = (0, 0);
    public (int x, int y)           Cell_Prev        { get; private set; } = (0, 0);
   
    public BlackBoardSnapshot       BlackBoard       { get; private set; } = new();
    public StatusManagerSnapshot    StatusManager    { get; private set; } = new();
    public InventorySnapshot        Inventory        { get; private set; } = new();

    public PathVehicleSnapshot      PathVehicle      { get; private set; } = new();
    public int                      PathAttribute    { get; private set; } = 0;
    public (int x, int y)           PathBasePosition { get; private set; } = (0, 0);





    public static EntitySnapshot Create(
        Int64                 _id,
        (int x, int y)        _cell,
        (int x, int y)        _cell_prev,
        BlackBoardSnapshot    _black_board,
        StatusManagerSnapshot _status_manager,
        InventorySnapshot     _inventory,
        PathVehicleSnapshot   _path_vehicle,
        int                   _path_attribute,
        (int x, int y)        _path_base_position)
    {
        return new EntitySnapshot
        {
            ID               = _id,
            Cell             = _cell,
            Cell_Prev        = _cell_prev,
            BlackBoard       = _black_board,
            StatusManager    = _status_manager,
            Inventory        = _inventory,
            PathVehicle      = _path_vehicle,
            PathAttribute    = _path_attribute,
            PathBasePosition = _path_base_position
        };
    }
    // public List<BuffSnapshot> Buffs { get; set; }
    

}