using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Command : BattleSystem
    {
        // 명령을 처리해보자.
        // public class CommandData
        // {
        //     public Int64     UnitID     { get; private set; }
        //     public EnumState State      { get; private set; } 

        //     public void Reset()
        //     {
        //         UnitID = 0;
        //         State  = EnumState.None;
        //     }
        // }

        // 처리할 명령 데이터. 순차적으로 처리한다.
        // Queue<CommandData> m_list_command;       


        public BattleSystem_Command() : base(EnumSystem.BattleSystem_Command)
        {
        }

        public override void Reset()
        {
            // m_list_command.Clear();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // while(m_list_command.Count > 0)
            // {
            //     if (m_list_command.TryPeek(out var command))
            //     {
            //             
            //     }
            //     else
            //     {
            //         m_list_command.Dequeue();
            //     }
            // }



            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

    }
}