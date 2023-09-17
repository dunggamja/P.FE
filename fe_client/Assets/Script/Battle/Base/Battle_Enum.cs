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
    /// �������� 
    /// </summary>
    public enum EnumBlackBoard
    {
        None         = 0,

        CommandType,  // 명령 (플레이어/AI)
        CommandState, // 명령 (가능/완료)
        Faction,      // 진영 (1:플레이어, 2:적군)


        IsMoving,
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
        // bitflag 로 제어하기 때문에 Max값 필요, 32 bit or 64 bit ?
    }


    /// <summary>
    /// ������ ���� Ÿ��
    /// </summary>
    public enum EnumCommandType
    {
        None,   // ���� �Ұ���
        Player, // 플레이어가 명령을 내려야 함
        AI,     // AI가 제어 
    }

    /// <summary>
    /// ������ �ൿ ����
    /// </summary>
    public enum EnumCommandState
    {
        None,
        Active, // 행동이 가능한 상태
        Wait,   // 행동 완료 후 대기 상태
    }


    
}
