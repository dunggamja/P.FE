using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumSystem
{
    None,
    CombatSystem_Turn,   // (전투씬), 공/방 턴 관리
    CombatSystem_Damage, // (전투씬), 데미지 처리.

    BattleSystem_Turn,   // (전투), 턴 진행    
}

public enum EnumState
{
    None,
    //Init,
    Progress,
    Finished,
}

