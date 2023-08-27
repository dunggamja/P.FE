using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleObject 
    {
        public void OnReceiveEvent(IEventParam _param)
        {
            if (_param is BattleSystem_Turn.EventParam battle_system_turn)
            {
                // 진영 / 턴 관련 


            }
        }
    }
}
