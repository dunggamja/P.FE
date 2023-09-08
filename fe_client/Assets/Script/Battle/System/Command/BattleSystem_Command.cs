using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Command : BattleSystem
    {
        // 명령을 처리해보자.
        public struct CommandData
        {
            public Int64 UnitID     { get; private set; }

            public void Reset()
            {
                UnitID     = 0;
            }
        }

        // 처리할 명령 데이터.
        CommandData m_command_data;       

        public BattleSystem_Command() : base(EnumSystem.BattleSystem_Command)
        {
        }

        public override void Reset()
        {
            m_command_data.Reset();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            // 명령 완료 처리.
            m_command_data.Reset();
        }

    }
}