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
        HP_Max,
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
      Weight     ,  // ����. 
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
  
      Might_Physics   = 1, // ��
      Might_Magic     = 2, // ����
      Hit             = 3, // ����
      Critical        = 4, // ġ��
      Weight          = 5, // ����
      Dodge           = 6, // ȸ��
      Dodge_Critical  = 7, // �ʻ�ȸ��
      Range           = 8, // ��Ÿ�
      Range_Min       = 9, // ��Ÿ�. (�ּ�)
    }


    /// <summary>
    /// ������
    /// </summary>
    public enum EnumEntityBlackBoard
    {
        None         = 0,

        CommandOwner = 1,  // [����] ��� Ÿ��
        CommandFlag,       // [����] ��� ���� (0 : ���, 1 : �ൿ �Ϸ�)
        Faction,           // [����] ����

        AIScore_Done,      // [AI] �ൿ�Ϸ�
        AIScore_Attack,    // [AI] ����
        // AIScore_Protect,   // [AI] ��ȣ 
        // AIScore_Survival,  // [AI] ����
        // AIScore_Position,  // [AI] ��ġ Ȯ��
        // AIScore_Advantage, // [AI] ���� Ȱ��
        // AIScore_Custom,    // [AI] ��ũ��Ʈ�� ����Ǵ� ���,
// TODO: AI ������ ��������� �޸�. 
// (http://haruka.saiin.net/~berwick-saga/1/index.php?%C5%A8%A4%CE%B9%D4%C6%B0%A5%D1%A5%BF%A1%BC%A5%F3)
// ��Ī	�ൿ ����	����
// ����	��ó�� �ִ� ���� ���� �̵���, �����Ÿ��� ������ ������ �´�.	��
// ����(������)	Ư�� ��ǥ�� ���� �̵���, �����Ÿ��� ������ ������ �´�.
// ��ǥ �̿ܿ��� �������� �ʴ´�. ��ǥ���� �̵� ��Ʈ�� ������ �����ص� �ٸ� �����Դ� �������� �ʴ´�.
// ��ǥ�� �ʻ󿡼� ������� ������� ���� Ÿ���� �ȴ�.	��
// ����(�̵���)	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ Ư�� ��ǥ�� ���� �̵�.	��
// ���	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ �� �ڸ����� ���.	��
// ���(������)	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ �������� �̵�.	��
// ���	�����Ÿ� ���� ���� ������ ���� ������ ����,
// ���� ��쿡�� ���� �����Ÿ��� ������ �����Ÿ� ������ ����ģ��, �ȿ� ������ �� �ڸ����� ���.	��
// ���(������)	�����Ÿ� ���� ���� ������ ���� ������ ����,
// ���� ���� ���� �����Ÿ��� ������ �����Ÿ� ������ ����ġ��, �� ������ ������ ���� ���� ���� �ƽ��ƽ��ϰ� �ٱ����� �ٰ�����.	��
// ���(�����)	���� �����Ÿ��� ���� �����Ÿ� ������ ����ġ��, ���� ������ �� �ڸ����� ���. ������ ���� �ʴ´�.	��
// ��ȸ	�������� �̵�.	��
// ��ȸ(ȣ����)	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ �������� �̵�.	��
// ��ȸ	�����Ÿ� ���� ���� ������ ���� ������ ����, ������ ������ ��Ʈ�� �̵�.	��
// ����	�� �ڸ��� �������� �ʴ´�.
// �� �ڸ����� ���� �����ϸ� �����Ѵ�.
// ���� �� ���� �������� ������ ������ �� �� ����.	��
// �̵�	Ư�� ��ǥ�� ���� �̵�. ������ ���� �ʴ´�.	��
// ��Ż	��Ż ����Ʈ�� ���Ѵ�. ������ ���� �ʴ´�.	��
// ��Ż(ȣ����)	��Ż ����Ʈ�� ���Ѵ�.
// �̵�ó���� ���� ������ ��밡 ������ ������ �´�.	��


        // IsMoving,
    }

    public enum EnumCombatBlackBoard
    {
        None = 0,
    }

    public enum EnumBattleBlackBoard
    {
        None = 0,

        TurnUpdateCount = 1, // [Turn �ý���] Turn Update Count    
        CurrentTurn,         // [Turn �ý���] Turn Number    
        CurrentFaction,      // [Turn �ý���] Faction Number  
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
        Forest,    // �� ����        
        Slope,     // (��)��Ż, ����

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
    public enum EnumCommanderType
    {
        None,   // ��� �� ����.
        Player, // �÷��̾ ���
        AI,     // AI ���
    }

    public enum EnumCommandFlag : byte
    {
        // None,        
        Move,     // �̵�
        Action,   // ������ ���, ���� ��.
        Exchange, // ������ ��ȯ, ... ��� Action�� �����ϸ� ���� ����?
        // Done,     // �ൿ ����
    }

    // �ൿ ����.
    public enum EnumCommandProgressState 
    {
        None,
        Progress,
        Done,
        Invalid,
    }

    // �ǻ���� �켱����.
    public enum EnumCommandPriority
    {
        None,
                
        // �켱����. �������� �켱������ ����.
        Low,
        Normal,
        High,

        // �ý��� �� �켱�ؼ� ó���ؾ� �ϴ� ���
        Critical, 
    }

    public enum EnumCellPositionEvent
    {
        Enter,  // ����
        Exit,  // �Ҹ�
        Move, // �̵�

        // MAX = 32,
    }
}
