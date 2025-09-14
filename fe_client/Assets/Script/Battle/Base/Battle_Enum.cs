using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// 유닛 속성   
    /// </summary>
    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry   = 1, // 보병
      Cavalry    = 2, // 기병
      Flyer      = 3, // 비행
      Undead     = 4, // 언데드
      Beast      = 5, // 비스트
      Large      = 6, // 대형
      HeavyArmor = 7, // 중형
    }

    /// <summary>
    /// 유닛 포인트. (cur / max) 현재 / 최대
    /// </summary>
    public enum EnumUnitPoint
    {
        None = 0,

        HP,
        HP_Max,
        EXP,
    }

    /// <summary>
    /// 유닛 능력치.
    /// </summary>
    public enum EnumUnitStatus
    {
      None = 0,

      Level      ,  // 레벨
      Strength   ,  // 힘
      Magic      ,  // 마력
      Skill      ,  // 스킬
      Speed      ,  // 속도
      Luck       ,  // 행운
      Defense    ,  // 방어
      Resistance ,  // 마방
      Movement   ,  // 이동력
      Weight     ,  // 중량. 
    }

    /// <summary>
    /// 무기 속성.
    /// </summary>
    public enum EnumWeaponAttribute
    {
      None = 0,
  
      Sword        = 1, // 검
      Axe          = 2, // 도끼
      Lance        = 3, // 창
      MartialArts  = 4, // 격투술
      Bow          = 5, // 활
      Wand         = 6, // 마법봉
      Grimoire     = 7, // 마법서
      Dagger       = 8, // 단검


      KillInfantry   = 101, // 보병
      KillCavalry    = 102, // 보병
      KillFlyer      = 103, // 비행
      KillUndead     = 104, // 언데드
      KillBeast      = 105, // 비스트
      KillLarge      = 106, // 대형
      KillHeavyArmor = 107, // 중형
    }

    /// <summary>
    /// 무기 상태.
    /// </summary>
    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics   = 1, // 물리 위력
      Might_Magic     = 2, // 마법 위력
      Hit             = 3, // 명중
      Critical        = 4, // 필살
      Weight          = 5, // 무게
      Dodge           = 6, // 회피
      Dodge_Critical  = 7, // 필살 회피
      Range           = 8, // 사거리
      Range_Min       = 9, // 사거리. (최소)
    }


    /// <summary>
    /// 엔티티 블랙보드.
    /// </summary>
    public enum EnumEntityBlackBoard
    {
        None         = 0,

        CommandOwner = 1,  // [명령] 소유자
        CommandFlag,       // [명령] 명령 (0 : 이동, 1 : 공격)
        Faction,           // [명령] 진영

        AIScore_Begin  = 1000,
        
        AIScore_Attack = AIScore_Begin, // [AI] 공격
        AIScore_Done,          // [AI] 완료
        AIScore_Move,          // [AI] 이동


        AIScore_Max,

    }

    public enum EnumCombatBlackBoard
    {
        None = 0,
    }

    public enum EnumBattleBlackBoard
    {
        None = 0,

        TurnUpdateCount = 1, // [Turn 우선순위] Turn Update Count    
        CurrentTurn,         // [Turn 우선순위] Turn Number    
        CurrentFaction,      // [Turn 우선순위] Faction Number  
    }

    /// <summary>
    /// 우선순위/후순위 우선순위.
    /// </summary>
    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }


    /// <summary>
    /// 지형 속성. bitflag. 32bit?
    /// </summary>
    public enum EnumTerrainAttribute
    {
        Invalid  = 0, // 무효
        FlyerOnly, // 비행 전용.
        Water,     // 물 
        WaterSide, // 물 옆,
        Ground,    // 땅        
        Forest,    // 숲        
        Slope,     // (경사) 경사, 경사

        MAX = 32,
        // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumPathOwnerAttribute
    {
      Ground, // 땅 이동 가능
      Flyer, // 비행 이동 가능
      Water, // 물 이동 가능
      Slope, // (경사) 경사, 경사 이동 가능
      
      MAX = 32,
      // bitflag.. 32 bit or 64 bit ?
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EnumCommanderType
    {
        None,   // 명령 없음.
        Player, // 플레이어
        AI,     // AI
    }

    public enum EnumCommandFlag : byte
    {
        // None,        
        Move,     // 이동
        Action,   // 행동, 공격.
        Exchange, // 행동 교환, ... 행동이 있으면 행동 취소?
        // Done,     // �ൿ ����
    }

    // 명령 진행 상태.
    public enum EnumCommandProgressState 
    {
        None,
        Progress,
        Done,
        Invalid,
    }

    // 명령 우선순위.
    // public enum EnumCommandPriority
    // {
    //     None,
                
    //     // 
    //     Low,
    //     Normal,
    //     High,

    //     // 
    //     Critical, 
    // }

    public enum EnumCellPositionEvent
    {
        Enter,  // 입장
        Exit,  // 퇴장
        Move, // 이동

        // MAX = 32,
    }
}
