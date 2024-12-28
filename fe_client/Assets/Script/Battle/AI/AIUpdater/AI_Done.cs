using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Done : IAIUpdater
    {
        // TODO: 0.001f는 그냥 임시로 적은 하드코딩 값.
        //       정말 할만한 행동이 없으면 Done 을 하도록 해둔 것임.
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