using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {
        public class EventParam : IEventParam
        {
            public BattleSystem_Turn System { get; private set; }

            public EventParam(BattleSystem_Turn _system)
            {
                System = _system;
            }
        }
    }
}
