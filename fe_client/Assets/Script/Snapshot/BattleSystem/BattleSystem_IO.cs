using System;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem_Turn_IO
{
  public int   TurnUpdateCount { get; set; } = 0;
  public int[] QueueTurn       { get; set; } = new int[2];
  public int[] QueueFaction    { get; set; } = new int[2];
}

public class BattleSystemManager_IO
{
  public BattleSystem_Turn_IO Turn             { get; set; } = null;
  public BlackBoard_IO        BlackBoard       { get; set; } = null;

  public List<(int, Battle.EnumCommanderType)> 
                              FactionCommander { get; set; } = null;

                              
}