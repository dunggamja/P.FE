using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumSystem
{
    None,
    CombatSystem_Turn,        // (전투씬), 공/방 턴 관리
    CombatSystem_Damage,      // (전투씬), 데미지 처리.
     
    BattleSystem_Turn,        // (전투), 턴 진행    
    BattleSystem_Command,     // (전투), 명령 처리
    BattleSystem_Move,        // (전투) 유닛 이동
    BattleSystem_Combat,      // (전투) 유닛 전투
    BattleSystem_Interaction, // (전투) 유닛 상호작용 (보물상자, 대화)
}

public enum EnumState
{
    None,
    //Init,
    Progress,
    Finished,
}

/// <summary>
/// 버프/스킬 연산 등의 타이밍 정의용
/// </summary>
public enum EnumSituationType
{
    None,

    CombatSystem_Turn_Start,       // (전투씬) 턴 시작시 스킬 사용.

    CombatSystem_Damage_Start,     // (전투씬) 공격 전 스킬 사용
    //CombatSystem_Damage_Calculate, // (전투씬) 공격 데미지 적용 직전 마지막 보정.
    //CombatSystem_Damage_Appy,      // (전투씬) 공격 데미지 적용 후 처리
    CombatSystem_Damage_Finish,    // (전투씬) 공격 데미지 적용 완료 후 마지막 처리.

    BattleSystem_Turn_Changed,     // (전투) 턴 변경
    BattleSystem_Faction_Changed,  // (전투) 진영 변경
}

