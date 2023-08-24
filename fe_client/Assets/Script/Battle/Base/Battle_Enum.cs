using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry   = 1, // 보병
      Cavalry    = 2, // 기마병
      Flyer      = 3, // 비행병
      Undead     = 4, // 좀비류
      Beast      = 5, // 짐승류
      Large      = 6, // 대(大)형 
      HeavyArmor = 7, // 중갑
    }

    public enum EnumUnitPoint
    {
        None = 0,

        HP,
    }

    public enum EnumUnitStatus
    {
      None = 0,

      Level      ,  // 레벨
      Strength   ,  // 힘
      Magic      ,  // 마력
      Skill      ,  // 기술
      Speed      ,  // 속도
      Luck       ,  // 행운
      Defense    ,  // 수비
      Resistance ,  // 마방
      Movement   ,  // 이동력
      Weight     ,  // 중량(체격)
    }

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
      KillCavalry    = 102, // 특효, 기마병
      KillFlyer      = 103, // 특효, 비행병
      KillUndead     = 104, // 특효, 좀비류
      KillBeast      = 105, // 특효, 짐승류
      KillLarge      = 106, // 특효, 대(大)형 
      KillHeavyArmor = 107, // 특효, 중갑
    }


    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics  , // 위력 (물리)
      Might_Magic    , // 위력 (마법)
      Hit            , // 명중
      Critical       , // 필살
      Weight         , // 무게
      Dodge          , // 회피
      Dodge_Critical , // 필살 회피
      Range          , // 사정
    }


    

    public enum EnumBlackBoard
    {
        None         = 0,


    }


    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }



    public enum EnumTerrainAttribute
    {
        Ground,
        Water,

        MAX = 32,
        // bitflag 로 연산을 할거임...
    }

    

}
