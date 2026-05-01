using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Condition_Command : ICondition
    {
        public EnumCommandFlag CommandFlag { get; private set; } = EnumCommandFlag.Action;
        public bool            IsEnable    { get; private set; } = false;

        public AI_Condition_Command(EnumCommandFlag _command_flag, bool _is_enable)
        {
            CommandFlag = _command_flag;
            IsEnable    = _is_enable;
        }

        public bool Verify_Condition(IOwner _owner)
        {
            if (_owner == null)
                return false;
                
            var owner_entity = _owner as Entity;
            if (owner_entity == null)
                return false;

            return IsEnable == owner_entity.HasCommandEnable(CommandFlag);
        }
    }
}