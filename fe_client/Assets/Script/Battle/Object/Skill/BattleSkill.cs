using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleSkill : ISkill
    {
        public bool UseSkill(ISystem _system, IOwner _owner)
        {
            return false;
        }
    }
    
}

