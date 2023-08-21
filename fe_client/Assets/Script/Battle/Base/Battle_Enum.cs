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

      Health     ,  // 체력
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
    }


    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might          = 1, // 위력
      Hit            = 2, // 명중
      Critical       = 3, // 필살
      Weight         = 4, // 무게
      Dodge          = 5, // 회피
      Dodge_Critical = 6, // 필살 회피
      Range          = 7, // 사정
    }


    public enum EnumTerrainAttribute
    {
        None = 0,

        Ground ,
        Water  ,
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

}
