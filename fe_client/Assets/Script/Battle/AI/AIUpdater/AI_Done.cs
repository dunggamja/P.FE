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

            // �ڵ尡 �ʹ� ������� ������ ĳ��.
            var owner_blackboard = owner_entity.BlackBoard;

            // TODO: �ൿ�� �������� �Ұ������� üũ�ؾ� �Ѵ�.
            if (owner_entity.HasCommandFlag(EnumCommandFlag.Action))
            {
                // TODO: 1000f�� �ӽ÷� ���� �ϵ��ڵ� ��...
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