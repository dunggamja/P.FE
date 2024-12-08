using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Done : IAIUpdater
    {
        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            // 코드가 너무 길어져서 변수들 캐싱.
            var owner_blackboard = owner_entity.BlackBoard;

            // TODO: 행동이 가능한지 불가능한지 체크해야 한다.
            if (owner_entity.HasCommandFlag(EnumCommandFlag.Action))
            {
                // TODO: 1000f는 임시로 적은 하드코딩 값...
                var my_score  = 1000f;
                var top_score = BattleSystemManager.Instance.BlackBoard.GetBPValueAsFloat(EnumBattleBlackBoard.AIScore);

                // 
                owner_blackboard.SetBPValue(EnumEntityBlackBoard.AIScore_Done, my_score);

                if (top_score < my_score)
                {
                    BattleSystemManager.Instance.BlackBoard.SetBPValue(EnumBattleBlackBoard.AIScore, my_score);
                    BattleSystemManager.Instance.BlackBoard.aiscore_top_entity_id = _owner.ID;
                }
            }
        }
    }
}