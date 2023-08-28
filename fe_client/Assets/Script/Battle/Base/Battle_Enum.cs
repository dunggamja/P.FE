using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// ���� Ư��
    /// </summary>
    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry   = 1, // ����
      Cavalry    = 2, // �⸶��
      Flyer      = 3, // ���ິ
      Undead     = 4, // �����
      Beast      = 5, // ���·�
      Large      = 6, // ��(��)�� 
      HeavyArmor = 7, // �߰�
    }

    /// <summary>
    /// ������ ����Ʈ (HP/EXP ���)
    /// </summary>
    public enum EnumUnitPoint
    {
        None = 0,

        HP,
        EXP,
    }

    /// <summary>
    /// ������ �������ͽ�
    /// </summary>
    public enum EnumUnitStatus
    {
      None = 0,

      Level      ,  // ����
      Strength   ,  // ��
      Magic      ,  // ����
      Skill      ,  // ���
      Speed      ,  // �ӵ�
      Luck       ,  // ���
      Defense    ,  // ����
      Resistance ,  // ����
      Movement   ,  // �̵���
      Weight     ,  // �߷�(ü��)
    }

    /// <summary>
    /// ���� Ư��
    /// </summary>
    public enum EnumWeaponAttribute
    {
      None = 0,
  
      Sword        = 1, // ��
      Axe          = 2, // ����
      Lance        = 3, // â
      MartialArts  = 4, // ����
      Bow          = 5, // Ȱ
      Wand         = 6, // ������
      Grimoire     = 7, // ������
      Dagger       = 8, // �ܰ�


      KillInfantry   = 101, // Ưȿ, ����     
      KillCavalry    = 102, // Ưȿ, �⸶��
      KillFlyer      = 103, // Ưȿ, ���ິ
      KillUndead     = 104, // Ưȿ, �����
      KillBeast      = 105, // Ưȿ, ���·�
      KillLarge      = 106, // Ưȿ, ��(��)�� 
      KillHeavyArmor = 107, // Ưȿ, �߰�
    }

    /// <summary>
    /// ������ �������ͽ�
    /// </summary>
    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics  , // ���� (����)
      Might_Magic    , // ���� (����)
      Hit            , // ����
      Critical       , // �ʻ�
      Weight         , // ����
      Dodge          , // ȸ��
      Dodge_Critical , // �ʻ� ȸ��
      Range          , // ����
    }


    /// <summary>
    /// ������ 
    /// </summary>
    public enum EnumBlackBoard
    {
        None         = 0,

        CommandType,  // EnumCommandType
        CommandState, // �ൿ ����
        Faction,      // 
    }

    /// <summary>
    /// ����/�Ҹ� ��
    /// </summary>
    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }


    /// <summary>
    /// ���� Ư�� (bitflag)
    /// </summary>
    public enum EnumTerrainAttribute
    {
        Ground,
        Water,

        MAX = 32,
        // bitflag �� ������ �ҰŶ� MAX �� ����.
    }


    /// <summary>
    /// ������ ���� Ÿ��
    /// </summary>
    public enum EnumCommandType
    {
        None,   // ��� �Ұ���
        Player, // �÷��̾ ���.
        AI,     // AI�� ���.
    }

    /// <summary>
    /// ������ �ൿ ����
    /// </summary>
    public enum EnumCommandState
    {
        None,
        Active, // �ൿ ����
        Wait,   // �ൿ �Ϸ� �� ��� ����
    }


    
}
