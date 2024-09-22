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
      Flyer      = 3, // 비병
      Undead     = 4, // 좀비
      Beast      = 5, // 맹수
      Large      = 6, // 거대
      HeavyArmor = 7, // 중갑
    }

    /// <summary>
    /// 유닛 포인트. (cur / max) 형태의 구조.
    /// </summary>
    public enum EnumUnitPoint
    {
        None = 0,

        HP,
        EXP,
    }

    /// <summary>
    /// 유닛 능력치
    /// </summary>
    public enum EnumUnitStatus
    {
      None = 0,

      Level      ,  // 레벨
      Strength   ,  // 힘
      Magic      ,  // 마력
      Skill      ,  // 기술
      Speed      ,  // 속도
      Luck       ,  // 행운
      Defense    ,  // 수비력
      Resistance ,  // 마방력
      Movement   ,  // 이동력
      Weight     ,  // 무게. <= 사용안할듯.
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
      MartialArts  = 4, // 격투
      Bow          = 5, // 활
      Wand         = 6, // 지팡이
      Grimoire     = 7, // 마법서
      Dagger       = 8, // 단검


      KillInfantry   = 101, // 특효, 보병
      KillCavalry    = 102, // 특효, 기병
      KillFlyer      = 103, // 특효, 비병
      KillUndead     = 104, // 특효, 좀비
      KillBeast      = 105, // 특효, 맹수
      KillLarge      = 106, // 특효, 거대
      KillHeavyArmor = 107, // 특효, 중갑
    }

    /// <summary>
    /// 무기 능력치
    /// </summary>
    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics  , // 힘
      Might_Magic    , // 마력
      Hit            , // 명중
      Critical       , // 치명
      Weight         , // 무게
      Dodge          , // 회피
      Dodge_Critical , // 필살회피
      Range          , // 사거리
      Range_Min      , // 사거리. (최소)
    }


    /// <summary>
    /// 블랙보드
    /// </summary>
    public enum EnumBlackBoard
    {
        None         = 0,

        CommandType,  // 명령 타입
        CommandState, // 명령 상태
        Faction,      // 진영

        IsMoving,
    }

    /// <summary>
    /// 유리/불리 명중률에 영향.
    /// </summary>
    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }


    /// <summary>
    /// 지형 속성. bitflag로 생각중. 32bit?
    /// </summary>
    public enum EnumTerrainAttribute
    {
        Invalid  = 0, // 이동 불가
        FlyerOnly, // 비행 유닛만 가능.
        Water,     // 물 지형
        WaterSide, // 물가,
        Ground,    // 땅        
        Forest,   // 숲 지형        
        Slope,    // (산)비탈, 경사면

        MAX = 32,
        // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumPathOwnerAttribute
    {
      Ground, // 지상 이동 가능
      Flyer, // 비행 이동 가능
      Water, // 물 이동 가능
      Slope, // (산) 비탈, 경사면 이동가능



      
      MAX = 32,
      // bitflag.. 32 bit or 64 bit ?
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EnumCommandType
    {
        None,   // 명령 안 받음.
        Player, // 플레이어가 명령
        AI,     // AI 명령
    }

    // /// <summary>
    // /// 占쏙옙占쏙옙占쏙옙 占썅동 占쏙옙占쏙옙
    // /// </summary>
    // public enum EnumCommandState
    // {
    //     None,
    //     Enable,   // ?뻾?룞?씠 媛??뒫?븳 ?긽?깭
    //     Complete, // 행동 완료.
    // }


    
}
