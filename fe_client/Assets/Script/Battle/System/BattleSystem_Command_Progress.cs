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
            // command queue�� ��� ����� �� ���� ���ƺ��ô�.
            var command = CommandManager.Instance.PeekCommand();
            if (command != null && command.Update() == EnumState.Finished)
            {
                // command�� �Ϸ�Ǹ� pop ó��.
                CommandManager.Instance.PopCommand();
            }
 
            // queue�� ��� ������� ����ó��.
            return CommandManager.Instance.PeekCommand() == null;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

        }        
    }
}
