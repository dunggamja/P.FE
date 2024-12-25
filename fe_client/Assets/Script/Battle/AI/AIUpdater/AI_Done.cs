using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Done : IAIUpdater
    {
        // TODO: 1000f�� �ӽ÷� ���� �ϵ��ڵ� ��...
        const float AI_SCORE = 1000f;

        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            // TODO: �ൿ�� �������� �Ұ������� üũ�ؾ� �Ѵ�.
            var is_acted = owner_entity.HasCommandFlag(EnumCommandFlag.Action);
            var score    = (is_acted) ? AI_SCORE : 0f;

            //             
            owner_entity.BlackBoard.SetBPValue(EnumEntityBlackBoard.AIScore_Done, score);
            
        }
    }
}