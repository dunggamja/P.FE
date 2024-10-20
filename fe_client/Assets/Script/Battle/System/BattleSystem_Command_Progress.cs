using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Battle
{
    public class BattleSystem_Command_Progress : BattleSystem//, IEventReceiver
    {

        // Command m_command_progress = null;

        public BattleSystem_Command_Progress() : base(EnumSystem.BattleSystem_Command_Progress)
        {
        }

        public override void Init()
        {
            
        }

        public override void Release()
        {
            
        }


        protected override void OnEnter(IBattleSystemParam _param)
        {
           
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // command queue가 모두 종료될 때 까지 돌아봅시다.
            var command = CommandManager.Instance.PeekCommand();
            if (command != null && command.Update() == EnumState.Finished)
            {
                // command가 완료되면 pop 처리.
                CommandManager.Instance.PopCommand();
            }
 
            // queue가 모두 비었으면 종료처리.
            return CommandManager.Instance.PeekCommand() == null;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

        }        
    }
}
