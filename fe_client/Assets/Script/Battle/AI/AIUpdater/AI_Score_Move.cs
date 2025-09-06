using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
  public class AI_Score_Move : IAIUpdater
  {

    public EnumAIAggressiveType AggressiveType { get; private set; } = EnumAIAggressiveType.Aggressive;

    public EnumAITargetType     TargetType     { get; private set; } = EnumAITargetType.None;


    public void Update(IAIDataManager _owner)
    {
      if (_owner == null)
        return;
      
      // 구현을 어떻게 할까???;;; 
      // 공격은 모든 경우의 수를 체크했었는데... 이동도 그렇게 하는게 좋겠지???;;;

      // 각 좌표 점수가 필요하다. 

      //  적에게 가까워질때의 점수
      //  적에게 위험을 받을때의 점수.
      //  지형 이득 점.수

      // update cell position 이 있을때마다 영향도 맵에 마킹을 해둔다. (블록단위로 마킹)
      // 영향도 맵을 사용할 일이 있을때 마킹해둔 블록들을 갱신하고 사용하도록 한다. 

      switch(TargetType)
      {
        case EnumAITargetType.Target:
          Process_Target(_owner);
          break;
        case EnumAITargetType.Position:
          Process_TargetPosition(_owner);
          break;

        case EnumAITargetType.None:
          Process_NoneTarget(_owner);
          break;
      }

      //   score = w1 * (1f - distToNearestEnemyNorm) // 적에게 가까워질때의 점수
      // + w2 * (1f - threatExposure) // 적에게 위험을 받을때의 점수.
      // + w3 * terrainAdvantage      // 지형 이득 점수.
      // - w4 * moveCost;             // 이동 비용 점수.?? <- 필요한가?

        // 음... 어찌구분할지 잘 모르겠으니.. EnumAIType에 따라서 동작을 분류해봅시다.
        // 뭔가 

        // switch(_owner.AIType)
        // {
        //   case EnumAIType.Attack:
        //     Process_AIType_Attack(_owner);
        //     break;
        // }
        
    }


    void Process_Target(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

        var target_list = _owner.AIData?.Targets?.AllTargetIDList;
        if (target_list == null)
          return;



        // 이미 공격에 실패한 경우에 여기로 들어올 것..
        // 가장 가까운 적에게 최대한 가까운 위치로 이동한다. 
        
    }

    void Process_TargetPosition(IAIDataManager _owner)
    {
      if (_owner == null)
        return;
        
    }

    void Process_NoneTarget(IAIDataManager _owner)
    {
      if (_owner == null)
        return;
        
    }

    
  }
}