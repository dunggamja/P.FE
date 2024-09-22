using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    /// <summary>
    /// ���� �Ӽ�
    /// </summary>
    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry   = 1, // ����
      Cavalry    = 2, // �⺴
      Flyer      = 3, // ��
      Undead     = 4, // ����
      Beast      = 5, // �ͼ�
      Large      = 6, // �Ŵ�
      HeavyArmor = 7, // �߰�
    }

    /// <summary>
    /// ���� ����Ʈ. (cur / max) ������ ����.
    /// </summary>
    public enum EnumUnitPoint
    {
        None = 0,

        HP,
        EXP,
    }

    /// <summary>
    /// ���� �ɷ�ġ
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
      Defense    ,  // �����
      Resistance ,  // �����
      Movement   ,  // �̵���
      Weight     ,  // ����. <= �����ҵ�.
    }

    /// <summary>
    /// ���� �Ӽ�.
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
      KillCavalry    = 102, // Ưȿ, �⺴
      KillFlyer      = 103, // Ưȿ, ��
      KillUndead     = 104, // Ưȿ, ����
      KillBeast      = 105, // Ưȿ, �ͼ�
      KillLarge      = 106, // Ưȿ, �Ŵ�
      KillHeavyArmor = 107, // Ưȿ, �߰�
    }

    /// <summary>
    /// ���� �ɷ�ġ
    /// </summary>
    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might_Physics  , // ��
      Might_Magic    , // ����
      Hit            , // ����
      Critical       , // ġ��
      Weight         , // ����
      Dodge          , // ȸ��
      Dodge_Critical , // �ʻ�ȸ��
      Range          , // ��Ÿ�
      Range_Min      , // ��Ÿ�. (�ּ�)
    }


    /// <summary>
    /// ������
    /// </summary>
    public enum EnumBlackBoard
    {
        None         = 0,

        CommandType,  // ��� Ÿ��
        CommandState, // ��� ����
        Faction,      // ����

        IsMoving,
    }

    /// <summary>
    /// ����/�Ҹ� ���߷��� ����.
    /// </summary>
    public enum EnumAdvantageState
    {
        None = 0,

        Advantage      ,
        Disadvantage   ,
    }


    /// <summary>
    /// ���� �Ӽ�. bitflag�� ������. 32bit?
    /// </summary>
    public enum EnumTerrainAttribute
    {
        Invalid  = 0, // �̵� �Ұ�
        FlyerOnly, // ���� ���ָ� ����.
        Water,     // �� ����
        WaterSide, // ����,
        Ground,    // ��        
        Forest,   // �� ����        
        Slope,    // (��)��Ż, ����

        MAX = 32,
        // bitflag.. 32 bit or 64 bit ?
    }

    public enum EnumPathOwnerAttribute
    {
      Ground, // ���� �̵� ����
      Flyer, // ���� �̵� ����
      Water, // �� �̵� ����
      Slope, // (��) ��Ż, ���� �̵�����



      
      MAX = 32,
      // bitflag.. 32 bit or 64 bit ?
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EnumCommandType
    {
        None,   // ��� �� ����.
        Player, // �÷��̾ ���
        AI,     // AI ���
    }

    // /// <summary>
    // /// ������ �ൿ ����
    // /// </summary>
    // public enum EnumCommandState
    // {
    //     None,
    //     Enable,   // ?��?��?�� �??��?�� ?��?��
    //     Complete, // �ൿ �Ϸ�.
    // }


    
}
