using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IBattleSystemParam : ISystemParam
    {

    }

    public abstract class BattleSystem : System<IBattleSystemParam>
    {
        public override ISystemManager SystemManager => BattleSystemManager.Instance;

        protected BattleSystem(EnumSystem _system_type) : base(_system_type)
        { }
    }
}
