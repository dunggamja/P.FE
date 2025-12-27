using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    // public enum EnumUnitSex
    // {
    //     None   = 0,
    //     Male   = 1,
    //     Female = 2
    // }


    /// <summary>
    /// 유닛 속성   
    /// </summary>
    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry      = 1, // 보병
      Cavalry       = 2, // 기병
      Flyer         = 3, // 비행
      Undead        = 4, // 언데드
      Beast         = 5, // 비스트
      Large         = 6, // 대형
      HeavyArmor    = 7, // 중형
      LightArmor    = 8, // 경갑
      Structure     = 9, // 건물


      Male   = 100, // 남성 
      Female = 101, // 여성 
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

      Level       = 1,  // 레벨
      Strength    = 2,  // 힘
      Magic       = 3,  // 마력
      Skill       = 4,  // 스킬
      Speed       = 5,  // 속도
      Luck        = 6,  // 행운
      Defense     = 7,  // 방어
      Resistance  = 8,  // 마방
      Movement    = 9,  // 이동력
      Weight      = 10,  // 중량. 
      Proficiency = 11, // 숙련도
    }

    /// <summary>
    /// 무기 속성.
    /// </summary>
    public enum EnumWeaponCategory
    {
      None = 0,
  
      Sword        = 1, // 검
      Axe          = 2, // 도끼
      Spear        = 3, // 창
      MartialArts  = 4, // 격투술
      Bow          = 5, // 활
      Wand         = 6, // 지팡이 (힐, 보조) : 지팡이는 ItemType을 따로 뺄까?
      Grimoire     = 7, // 마법서 (마법 공격))
      Dagger       = 8, // 단검
    }

    public enum EnumItemConsumeCategory
    {
      None      = 0,
      HPPotion  = 1, // 물약
    }

    public enum EnumResourceCategory
    {
      None = 0,

      Gold = 1, // 골드.
    }

    

    public enum EnumItemAttribute
    {
      None = 0,

      // 장착, 적용, 사용 시 제한 조건.
      ExclusiveClass     = 1, // 전용 클래스  ( Class KIND)
      ExclusiveCharacter = 2, // 전용 캐릭터. ( ID )
      ExclusiveAttribute = 3, // 전용 속성.


      // 장착, 적용, 사용 시 효과
      KillUnitAttribute  = 11, // 보병 (EnumUnitAttribute)
      UnitStatusBuff     = 21, // 유닛 상태 보너스 (EnumUnitStatus)      
      SkillKind          = 31, // 스킬 종류 (Skill KIND)
      ExpBonus           = 41, // 경험치 획득량.   
      

      BuffBonus          = 100, // 버프 보너스 (EnumBuffStatus)


      TargetType           = 200, // 아이템 대상 타입 (EnumTargetType)
      Heal                 = 201, // 회복 (고정)
      HealBonus_UnitStatus = 202, // 회복 보너스 (유닛 스탯 비례, 만분율)



    //   HitBonus_UnitStatus    = 302, // 명중 보너스 (유닛 스탯 비례, 만분율)
      
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
      Proficiency     = 10, // 숙련도
      MaxCount        = 11, // 최대 개수
      Price           = 12, // 가격

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

        

    }

    public enum EnumAIBlackBoard
    {
        None   = 0,
        Begin  = 1,
        
        Attack = Begin, // [AI] 공격
        Done,           // [AI] 완료
        Move,           // [AI] 이동


        Max,

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




        // SelectCursor_X = 100,
        // SelectCursor_Y,
        // CommandProgress_EntityID,


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
        Invalid   = 0, // 무효
        Ground    = 1, // 땅        
        Ground_Forest, // 숲        
        Ground_Dirt,   // 거친 지형 
        Ground_Climb,  // 등반 (경보병 위주?)       

        // Building  = 10, // 건물
                        // 


        Water     = 20, // 물 
        Water_Shallow,  // 얕은 물

        FlyerOnly = 30, // 비행 전용.

        MAX       = 32,
        // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumPathOwnerAttribute
    {
      Ground = 0,        // 땅 이동 가능
      Flyer = 1,         // 비행 이동 가능
      Water = 2,         // 물 이동 가능
      Water_Shallow = 3, // 얕은 물 이동 가능
      Climber = 4,       // 등산 가능.
      
      MAX = 32,
      // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumTargetType
    {
        None  = 0,
        Owner = 1, // 소유자
        Ally  = 2, // 아군
        Enemy = 3, // 적

        // Both  = 4, // 아군 + 적군 구분 없음.
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
        Action,   // 공격.
        Exchange, // 교환
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


    public enum EnumCellPositionEvent
    {
        Enter,  // 입장
        Exit,  // 퇴장
        Move, // 이동

        // MAX = 32,
    }



    public enum EnumUnitCommandType
    {
        None,
        Attack,   // 공격
        Wand,     // 지팡이
        Skill,    // 스킬
        Exchange, // 교환
        Item,     // 아이템
        Wait,     // 대기.
    }

    public enum EnumTagType
    {
       None = 0, 

       Entity         = 1,    // 엔티티.
       Entity_Faction = 2,    // 진영.
       Entity_All     = 3,    // 구분 없이 모든 엔티티.   // All > Faction > Entity, 이것은 일일이 문서화하기 힘드므로 시스템으로 처리한다.

       Position       = 100, // 위치 - 점.
       Position_Rect  = 101, // 위치 - 사각형. 12자리 사용. (XXXYYYXXXYYY) (min,max)

       Trigger        = 10000, // 트리거.


    }

    public enum EnumTagAttributeType
    {
       None = 0,

       TARGET_FOCUS   = 1, // 타겟팅 집중.
       TARGET_IGNORE  = 2, // 타겟팅 제외.
       TARGET_CONTAIN = 3, // 타겟팅 포함.


       POSITION_VISIT = 11, // 위치 - 방문 가능       
       POSITION_EXIT  = 12, // 위치 - 이탈 

       TALK_COMMAND   = 21,  // 대화 명령 가능

       TAG_PARENT     = 100, // 태그 - 부모. (그룹핑을 위한 값...)

       FromScenario   = 999,
    }


    public enum EnumScenarioType
    {
       None = 0,

       Victory = 1, // 시나리오 승리
       Defeat  = 2, // 시나리오 패배
       Event   = 3, // 시나리오 중간 이벤트
    }


    public enum EnumScenarioTrigger
    {
       None = 0,

       OnTurnStart      = 1, // 턴 시작 시 트리거
       OnTurnEnd        = 2, // 턴 종료 시 트리거

       OnEntityDefeat   = 11, // 엔티티 사망 시 트리거

       OnPositionArrive = 21, // 위치 도착 시 트리거
       OnPositionVisit  = 22, // 위치 방문 시 트리거
       
       OnEveryUpdate     = 999, // 매 프레임 트리거
    }

    public enum EnumScenarioCondition
    {
       None = 0,

       DefeatAllEnemies      = 1, // 모든 적을 사망시키면 조건 충족
       DefeatSpecificEntity  = 2, // 특정 엔티티를 사망시키면 조건 충족
       DefeatSpecificFaction = 3, // 특정 진영을 사망시키면 조건 충족


       TurnCount             = 11, // 특정 턴수가 되면 조건 충족


       PositionArrive        = 21, // 위치 도착 시 조건 충족
       PositionVisit         = 22, // 위치 방문 시 조건 충족


       EntityHP_Below        = 31, // 유닛 체력이 특정 이하일 경우 

       HasTagID              = 41, // 특정 태그 ID가 있을 경우 조건 충족
       HasTagName            = 42, // 특정 태그 이름이 있을 경우 조건 충족
       HasItem               = 43, // 특정 아이템을 보유할 경우 조건 충족
    }

    public enum EnumScenarioAction
    {
       None = 0,

       Victory    = 1, // 시나리오 승리
       Defeat     = 2, // 시나리오 패배
       Event      = 3, // 시나리오 중간 이벤트

       ShowDialog = 11, // 대화

       AddTag     = 21, // 태그 추가
       RemoveTag  = 22, // 태그 제거

       AddItem    = 31, // 아이템 추가
       RemoveItem = 32, // 아이템 제거
    }


}
