﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IBattleSystemParam : ISystemParam
    {
        float DeltaTime { get; }
    }

    public class BattleSystemParam_Test : IBattleSystemParam
    {
        public float DeltaTime => Time.deltaTime;
    }
 
   public abstract class BattleSystem : System<IBattleSystemParam>
    {
        //public override ISystemManager SystemManager => BattleSystemManager.Instance;

        protected BattleSystem(EnumSystem _system_type) : base(_system_type)
        { 
            
        }
    }
}
