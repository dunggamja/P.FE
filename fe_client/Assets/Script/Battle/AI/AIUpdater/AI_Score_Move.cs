using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
  public class AI_Score_Move : IAIUpdater
  {



    public bool IsAggressive      { get; set; } = false; // ���� �������� ����
    public bool IsEvassive        { get; set; } = false; // ���� ������ ����
    public bool IsBasePosition    { get; set; } = false; // �������� �̵����� ����
    public bool HasTarget         { get; set; } = false; // Ư�� ��ǥ�� ���� �̵����� ����
    public bool HasTargetPosition { get; set; } = false; // Ư�� ��ġ�� �̵����� ����


    public void Update(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

        // ��... ��������� �� �𸣰�����.. EnumAIType�� ���� ������ �з��غ��ô�.
        // ���� 

        switch(_owner.AIType)
        {
          case EnumAIType.Attack:
            Process_AIType_Attack(_owner);
            break;
        }
        
    }


    void Process_AIType_Attack(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

        // �̹� ���ݿ� ������ ��쿡 ����� ���� ��..
        // ���� ����� ������ �ִ��� ����� ��ġ�� �̵��Ѵ�. 
        
    }
  }
}