using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Score_Done : AI_Score_Base
    {
        // TODO: 0.001f는 그냥 임시로 적은 하드코딩 값.
        //       정말 할만한 행동이 없으면 Done 을 하도록 해둔 것임.
        const float DONE_SCORE = 0.001f;

        protected override bool OnUpdate(IAIDataManager _param)
        {
            if (_param == null)
                return false;

            var owner_entity = EntityManager.Instance.GetEntity(_param.ID);
            if (owner_entity == null)
                return false;
            //             
            owner_entity.AIManager.AIBlackBoard.SetBPValue(EnumAIBlackBoard.Score_Done, DONE_SCORE);            
            return true;
        }
    }
}