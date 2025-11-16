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
      None    = 0,
      Potion  = 1, // 물약
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



        Water     = 20, // 물 
        Water_Shallow,  // 얕은 물

        FlyerOnly = 30, // 비행 전용.

        MAX       = 32,
        // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumPathOwnerAttribute
    {
      Ground,        // 땅 이동 가능
      Flyer,         // 비행 이동 가능
      Water,         // 물 이동 가능
      Water_Shallow, // 얕은 물 이동 가능
      Climber,       // 등산 가능.
      
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
