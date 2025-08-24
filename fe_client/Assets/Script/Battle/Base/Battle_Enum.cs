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
        HP_Max,
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
      Weight     ,  // 무게. 
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
  
      Might_Physics   = 1, // 힘
      Might_Magic     = 2, // 마력
      Hit             = 3, // 명중
      Critical        = 4, // 치명
      Weight          = 5, // 무게
      Dodge           = 6, // 회피
      Dodge_Critical  = 7, // 필살회피
      Range           = 8, // 사거리
      Range_Min       = 9, // 사거리. (최소)
    }


    /// <summary>
    /// 블랙보드
    /// </summary>
    public enum EnumEntityBlackBoard
    {
        None         = 0,

        CommandOwner = 1,  // [유닛] 명령 타입
        CommandFlag,       // [유닛] 명령 상태 (0 : 대기, 1 : 행동 완료)
        Faction,           // [유닛] 진영

        AIScore_Done,      // [AI] 행동완료
        AIScore_Attack,    // [AI] 공격
        // AIScore_Protect,   // [AI] 보호 
        // AIScore_Survival,  // [AI] 생존
        // AIScore_Position,  // [AI] 위치 확보
        // AIScore_Advantage, // [AI] 이점 활용
        // AIScore_Custom,    // [AI] 스크립트로 제어되는 경우,
// TODO: AI 구현시 참고용으로 메모. 
// (http://haruka.saiin.net/~berwick-saga/1/index.php?%C5%A8%A4%CE%B9%D4%C6%B0%A5%D1%A5%BF%A1%BC%A5%F3)
// 명칭	행동 내용	도발
// 진격	근처에 있는 적을 향해 이동해, 사정거리에 잡으면 공격해 온다.	○
// 진격(한정형)	특정 목표를 향해 이동해, 사정거리에 잡으면 공격해 온다.
// 목표 이외에는 공격하지 않는다. 목표로의 이동 루트를 완전히 봉쇄해도 다른 적에게는 공격하지 않는다.
// 목표가 맵상에서 사라지면 통상적인 진격 타입이 된다.	○
// 진격(이동형)	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 특정 목표를 향해 이동.	○
// 요격	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 그 자리에서 대기.	○
// 요격(거점형)	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 거점으로 이동.	○
// 경계	사정거리 내에 공격 가능한 적이 있으면 공격,
// 없는 경우에는 적의 사정거리에 있으면 사정거리 밖으로 도망친다, 안에 없으면 그 자리에서 대기.	○
// 경계(접근형)	사정거리 내에 공격 가능한 적이 있으면 공격,
// 없는 경우는 적의 사정거리에 있으면 사정거리 밖으로 도망치고, 안 들어오지 않으면 적의 공격 범위 아슬아슬하게 바깥까지 다가간다.	○
// 경계(대기형)	적의 사정거리에 들어가면 사정거리 밖으로 도망치고, 들어가지 않으면 그 자리에서 대기. 공격해 오지 않는다.	×
// 배회	랜덤으로 이동.	○
// 배회(호전형)	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 랜덤으로 이동.	○
// 순회	사정거리 내에 공격 가능한 적이 있으면 공격, 없으면 일정한 루트를 이동.	○
// 고정	그 자리를 움직이지 않는다.
// 그 자리에서 공격 가능하면 공격한다.
// 인접 등 공격 가능하지 않으면 도발은 할 수 없다.	△
// 이동	특정 목표를 향해 이동. 공격해 오지 않는다.	×
// 이탈	이탈 포인트로 향한다. 공격해 오지 않는다.	×
// 이탈(호전형)	이탈 포인트로 향한다.
// 이동처에서 공격 가능한 상대가 있으면 공격해 온다.	×


        // IsMoving,
    }

    public enum EnumCombatBlackBoard
    {
        None = 0,
    }

    public enum EnumBattleBlackBoard
    {
        None = 0,

        TurnUpdateCount = 1, // [Turn 시스템] Turn Update Count    
        CurrentTurn,         // [Turn 시스템] Turn Number    
        CurrentFaction,      // [Turn 시스템] Faction Number  
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
        Forest,    // 숲 지형        
        Slope,     // (산)비탈, 경사면

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
    public enum EnumCommanderType
    {
        None,   // 명령 안 받음.
        Player, // 플레이어가 명령
        AI,     // AI 명령
    }

    public enum EnumCommandFlag : byte
    {
        // None,        
        Move,     // 이동
        Action,   // 아이템 사용, 공격 등.
        Exchange, // 아이템 교환, ... 요건 Action이 가능하면 냅둘 예정?
        // Done,     // 행동 종료
    }

    // 행동 상태.
    public enum EnumCommandProgressState 
    {
        None,
        Progress,
        Done,
        Invalid,
    }

    // 의사결정 우선순위.
    public enum EnumCommandPriority
    {
        None,
                
        // 우선순위. 높을수록 우선순위가 높음.
        Low,
        Normal,
        High,

        // 시스템 상 우선해서 처리해야 하는 경우
        Critical, 
    }

    public enum EnumCellPositionEvent
    {
        Enter,  // 등장
        Exit,  // 소멸
        Move, // 이동

        // MAX = 32,
    }
}
