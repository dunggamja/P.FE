using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumSystem
{
    None,
    CombatSystem_Turn,                // (전투씬), 공/방 턴  관리
    CombatSystem_Damage,              // (전투씬), 데미지 처리.
     
    BattleSystem_Turn,                // (전투필드),  턴   진행    
    BattleSystem_Command_Dispatch,     // (전투필드),  유닛들 명령 dispatch
    BattleSystem_Command_Progress,     // (전투필드),  유닛들 명령 dispatch
    // BattleSystem_Navigation,  // (전투필드), 유닛들 이동 처리
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

    //BattleSystem_Command_Dispatch_AI_Update,
}


public enum EnumItemType
{
    None,

    Weapon,     // 무기
    Consumable, // 소모품
    Accessary,  // 액세서리
    Treasure,   // 보물
}

public enum EnumItemActionType
{
    None,

    Equip,   // 장비
    Unequip, // 장비 해제
    Consume, // 사용 (무기는 내구도 감소...)
    Dispose, // 버리기/매각
}


public enum EnumConditionAttribute
{
    None,
}

public enum EnumEffectAttribute
{
    None,
    
    //Damage      = 1, // 데미지
    Recovery    = 2, // 회복
    Buff        = 3, // 버프
    Skill       = 4, // 스킬


    // 스킬    
    SkillGet    = 1001, // 스킬 획득    
    ClassUp     = 1002, // 클래스 업
    ClassChange = 1003, // 클래스 체인지
}

public enum EnumEffectTargetAttribute
{
    None,

    Owner       = 1,   // 소유자 대상
    Ally        = 2,   // 아군 대상인가
    Enemy       = 3,   // 적 대상인가

    AreaEffect  = 4, // 범위 효과인가.
    FocusOwner  = 5, // 소유자를 타겟팅
    FocusGround = 6, // 위치를 중점으로
}


