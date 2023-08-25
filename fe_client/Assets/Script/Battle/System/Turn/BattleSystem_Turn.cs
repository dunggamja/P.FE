using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleSystem_Turn : BattleSystem
    {
        public int Turn_Current  { get; private set; }
        public int Phase_Current { get; private set; }

        public BattleSystem_Turn() : base(EnumSystem.BattleSystem_Turn)
        { }

        public override void Reset()
        {
            Turn_Current  = 0;
            Phase_Current = 0;
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            throw new NotImplementedException();
        }
    }

}

