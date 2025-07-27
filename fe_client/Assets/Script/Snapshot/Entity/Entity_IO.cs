using System;
using System.Collections.Generic;
using UnityEngine;


public class Entity_IO
{
    public Int64               ID               { get; set; } = 0;
    public (int x, int y)      Cell             { get; set; } = (0, 0);
    public (int x, int y)      Cell_Prev        { get; set; } = (0, 0);
   
    public BlackBoard_IO       BlackBoard       { get; set; } = new();
    public StatusManager_IO    StatusManager    { get; set; } = new();
    public Inventory_IO        Inventory        { get; set; } = new();

    public PathVehicle_IO      PathVehicle      { get; set; } = new();
    public int                 PathAttribute    { get; set; } = 0;
    public (int x, int y)      PathBasePosition { get; set; } = (0, 0);

}

public class EntityManager_IO
{
    public List<Entity_IO> Entities { get; private set; } = new();


}