using System;
using System.Collections.Generic;
using UnityEngine;


public class GameSnapshot
{
    public Int64  ID          { get; private set; }
    public string Description { get; private set; }
    public float  Timestamp   { get; private set; }
    
    // ��ƼƼ ���µ�
    public Dictionary<Int64, EntitySnapshot> EntityStates { get; private set; }
    
    // �ý��� ���µ�
    // public BattleSystemSnapshot BattleSystemState { get; private set; }
    // public CombatSystemSnapshot CombatSystemState { get; private set; }
    
    public GameSnapshot(Int64 id, string description)
    {
        ID = id;
        Description = description;
        Timestamp = Time.time;
        EntityStates = new Dictionary<Int64, EntitySnapshot>();
        // BattleSystemState = new BattleSystemSnapshot();
        // CombatSystemState = new CombatSystemSnapshot();
    }
}