using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ Æ¯ï¿½ï¿½
    /// </summary>
    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry   = 1, // ï¿½ï¿½ï¿½ï¿½
      Cavalry    = 2, // ï¿½â¸¶ï¿½ï¿½
      Flyer      = 3, // ï¿½ï¿½ï¿½àº´
      Undead     = 4, // ï¿½ï¿½ï¿½ï¿½ï¿?
      Beast      = 5, // ï¿½ï¿½ï¿½Â·ï¿½
      Large      = 6, // ï¿½ï¿½(ï¿½ï¿½)ï¿½ï¿½ 
      HeavyArmor = 7, // ï¿½ß°ï¿½
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Æ® (HP/EXP ï¿½ï¿½ï¿?)
    /// </summary>
    public enum EnumUnitPoint
    {
        None = 0,

        HP,
        EXP,
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Í½ï¿½
    /// </summary>
    public enum EnumUnitStatus
    {
      None = 0,

      Level      ,  // ï¿½ï¿½ï¿½ï¿½
      Strength   ,  // ï¿½ï¿½
      Magic      ,  // ï¿½ï¿½ï¿½ï¿½
      Skill      ,  // ï¿½ï¿½ï¿?
      Speed      ,  // ï¿½Óµï¿½
      Luck       ,  // ï¿½ï¿½ï¿?
      Defense    ,  // ï¿½ï¿½ï¿½ï¿½
      Resistance ,  // ï¿½ï¿½ï¿½ï¿½
      Movement   ,  // ï¿½Ìµï¿½ï¿½ï¿½
      Weight     ,  // ï¿½ß·ï¿½(Ã¼ï¿½ï¿½)
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ Æ¯ï¿½ï¿½
    /// </summary>
    public enum EnumWeaponAttribute
    {
      None = 0,
  
      Sword        = 1, // ï¿½ï¿½
      Axe          = 2, // ï¿½ï¿½ï¿½ï¿½
      Lance        = 3, // Ã¢
      MartialArts  = 4, // ï¿½ï¿½ï¿½ï¿½
      Bow          = 5, // È°
      Wand         = 6, // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
      Grimoire     = 7, // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
      Dagger       = 8, // ï¿½Ü°ï¿½


      KillInfantry   = 101, // Æ¯È¿, ï¿½ï¿½ï¿½ï¿½     
      KillCavalry    = 102, // Æ¯È¿, ï¿½â¸¶ï¿½ï¿½
      KillFlyer      = 103, // Æ¯È¿, ï¿½ï¿½ï¿½àº´
      KillUndead     = 104, // Æ¯È¿, ï¿½ï¿½ï¿½ï¿½ï¿?
      KillBeast      = 105, // Æ¯È¿, ï¿½ï¿½ï¿½Â·ï¿½
      KillLarge      = 106, // Æ¯È¿, ï¿½ï¿½(ï¿½ï¿½)ï¿½ï¿½ 
      KillHeavyArmor = 107, // Æ¯È¿, ï¿½ß°ï¿½
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Í½ï¿½
    /// </summary>
    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics  , // ï¿½ï¿½ï¿½ï¿½ (ï¿½ï¿½ï¿½ï¿½)
      Might_Magic    , // ï¿½ï¿½ï¿½ï¿½ (ï¿½ï¿½ï¿½ï¿½)
      Hit            , // ï¿½ï¿½ï¿½ï¿½
      Critical       , // ï¿½Ê»ï¿½
      Weight         , // ï¿½ï¿½ï¿½ï¿½
      Dodge          , // È¸ï¿½ï¿½
      Dodge_Critical , // ï¿½Ê»ï¿½ È¸ï¿½ï¿½
      Range          , // ï¿½ï¿½ï¿½ï¿½
    }


    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ 
    /// </summary>
    public enum EnumBlackBoard
    {
        None         = 0,

        CommandType,  // ëª…ë ¹ (?”Œ? ˆ?´?–´/AI)
        CommandState, // ëª…ë ¹ (ê°??Š¥/?™„ë£?)
        Faction,      // ì§„ì˜ (1:?”Œ? ˆ?´?–´, 2:? êµ?)

        IsMoving,
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½/ï¿½Ò¸ï¿½ ï¿½ï¿½
    /// </summary>
    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }


    /// <summary>
    /// ï¿½ï¿½ï¿½ï¿½ Æ¯ï¿½ï¿½ (bitflag)
    /// </summary>
    public enum EnumTerrainAttribute
    {
        Ground,
        Water,

        MAX = 32,
        // bitflag ë¡? ? œ?–´?•˜ê¸? ?•Œë¬¸ì— Maxê°? ?•„?š”, 32 bit or 64 bit ?
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EnumCommandType
    {
        None,   // ¸í·É ¾È ¹ŞÀ½.
        Player, // ÇÃ·¹ÀÌ¾î°¡ ¸í·É
        AI,     // AI ¸í·É
    }

    // /// <summary>
    // /// ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½àµ¿ ï¿½ï¿½ï¿½ï¿½
    // /// </summary>
    // public enum EnumCommandState
    // {
    //     None,
    //     Enable,   // ?–‰?™?´ ê°??Š¥?•œ ?ƒ?ƒœ
    //     Complete, // Çàµ¿ ¿Ï·á.
    // }


    
}
