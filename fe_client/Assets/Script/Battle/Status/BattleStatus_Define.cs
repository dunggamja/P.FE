using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BattleStatus
{

    public enum EnumBattleUnitAttribute
    {
      None = 0,
  
      Infantry = 1, // 보병
      Cavalry  = 2, // 기마병
      Flyer    = 3, // 비행병
      Undead   = 4, // 좀비류
      Beast    = 5, // 짐승류
      Large    = 6, // 대(大)형 
    }


    public enum EnumBattleWeaponAttribute
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


      IsMagic      = 100,
    }


    public enum EnumBattleUnitStatus
    {
      None = 0,

      HP         = 1,  // 체력
      Strength   = 2,  // 힘
      Magic      = 3,  // 마력
      Skill      = 4,  // 기술
      Speed      = 5,  // 속도
      Luck       = 6,  // 행운
      Defense    = 7,  // 수비
      Resistance = 8,  // 마방
      Movement   = 9,  // 이동력
      Weight     = 10, // 중량(체격)
    }

    public enum EnumBattleWeaponStatus
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


    public enum EnumBattleTerrainAttribute
    {
        None = 0,
    }

    public enum EnumBattleTerrainStatus
    {
        None = 0,
    }


    public enum EnumBattleTurn
    {
        None = 0,

        Attacker,
        Defender,
    }

    public enum EnumBattleBlackBoard
    {
        None = 0,

        IsAttacker   = 1, // 공격자,피격자 체크.
        MoveDistance = 2, // 이동 거리.
    }

}
