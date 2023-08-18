using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

    public enum EnumUnitAttribute
    {
      None = 0,
  
      Infantry = 1, // ����
      Cavalry  = 2, // �⸶��
      Flyer    = 3, // ���ິ
      Undead   = 4, // �����
      Beast    = 5, // ���·�
      Large    = 6, // ��(��)�� 
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


      IsMagic      = 100,
    }


    public enum EnumUnitStatus
    {
      None = 0,

      HP         = 1,  // ü��
      Strength   = 2,  // ��
      Magic      = 3,  // ����
      Skill      = 4,  // ���
      Speed      = 5,  // �ӵ�
      Luck       = 6,  // ���
      Defense    = 7,  // ����
      Resistance = 8,  // ����
      Movement   = 9,  // �̵���
      Weight     = 10, // �߷�(ü��)
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

        Ground = 1,
        Water  = 2,

    }



}
