using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;


public class GameSnapshot
{
    public Int64  ID             { get; private set; } = 0;
    
    // 엔티티 상태들
    public EntityManager_IO       EntityManager       { get; private set; } = new();

    public BattleSystemManager_IO BattleSystemManager { get; private set; } = new();
    
    public TerrainMapManager_IO   TerrainMapManager { get; private set; } = new();
    public MyRandom_IO            Random { get; private set; } = new();

    public static GameSnapshot Save()
    {
        var snapshot                 = new GameSnapshot();
        snapshot.ID                  = Util.GenerateID();
        snapshot.EntityManager       = Battle.EntityManager.Instance.Save();
        snapshot.BattleSystemManager = Battle.BattleSystemManager.Instance.Save();
        snapshot.TerrainMapManager   = Battle.TerrainMapManager.Instance.Save();
        snapshot.Random              = Util.RandomSave();

        return snapshot;
    }

    public static void Load(GameSnapshot _snapshot)
    {
        Battle.EntityManager.Instance.Load(_snapshot.EntityManager);
        Battle.BattleSystemManager.Instance.Load(_snapshot.BattleSystemManager);
        Battle.TerrainMapManager.Instance.Load(_snapshot.TerrainMapManager);
        Util.RandomLoad(_snapshot.Random);
    }
}