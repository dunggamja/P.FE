using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    public struct Skill
    {
        List<ICondition> Conditions;
        List<IEffect>    Effects;

        public bool IsValidCondition(BattleStatusManager _param)
        {
            if (Conditions != null)
            {
                foreach(var e in Conditions)
                {
                    if (e != null && !e.IsValid(_param))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Apply(BattleStatusManager _param)
        {
            foreach(var e in Effects)
            {
                if (e != null)
                {
                    e.Apply(_param);
                }
            }
        }

    }
}
