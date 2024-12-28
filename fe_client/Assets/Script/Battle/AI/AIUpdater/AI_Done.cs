using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Done : IAIUpdater
    {
        // TODO: 0.001f�� �׳� �ӽ÷� ���� �ϵ��ڵ� ��.
        //       ���� �Ҹ��� �ൿ�� ������ Done �� �ϵ��� �ص� ����.
        const float DONE_SCORE = 0.001f;

        public void Update(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;
            //             
            owner_entity.BlackBoard.SetBPValue(EnumEntityBlackBoard.AIScore_Done, DONE_SCORE);            
        }
    }
}