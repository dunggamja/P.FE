using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

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

    public enum EnumUnitPoint
    {
        None = 0,

        HP,
    }

    public enum EnumUnitStatus
    {
      None = 0,

      Health     ,  // ü��
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
    }


    public enum EnumWeaponStatus
    {
      None = 0,
  
      Might          = 1, // ����
      Hit            = 2, // ����
      Critical       = 3, // �ʻ�
      Weight         = 4, // ����
      Dodge          = 5, // ȸ��
      Dodge_Critical = 6, // �ʻ� ȸ��
      Range          = 7, // ����
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
