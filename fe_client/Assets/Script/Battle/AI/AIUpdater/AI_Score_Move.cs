using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
  public class AI_Score_Move : IAIUpdater
  {



    public bool IsAggressive      { get; set; } = false; // 적을 공격할지 여부
    public bool IsEvassive        { get; set; } = false; // 적을 피할지 여부
    public bool IsBasePosition    { get; set; } = false; // 거점으로 이동할지 여부
    public bool HasTarget         { get; set; } = false; // 특정 목표를 향해 이동할지 여부
    public bool HasTargetPosition { get; set; } = false; // 특정 위치로 이동할지 여부


    public void Update(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

        // 음... 어찌구분할지 잘 모르겠으니.. EnumAIType에 따라서 동작을 분류해봅시다.
        // 뭔가 

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

        // 이미 공격에 실패한 경우에 여기로 들어올 것..
        // 가장 가까운 적에게 최대한 가까운 위치로 이동한다. 
        
    }
  }
}