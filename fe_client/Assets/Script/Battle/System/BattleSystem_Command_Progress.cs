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
            // ��Ÿ� �� ��ƼƼ���� �������� �����ؾ� �ڴ�. 


            // // command queue�� ��� ����� �� ���� ���ƺ��ô�.
            // var command  = BattleSystemManager.Instance.PeekCommand();
            // if (command != null)
            // {
            //     // �̰� CommandManager ������...
            //     // if (command.Update() == EnumState.Finished)
            //     // {
            //     //     // command�� �Ϸ�Ǹ� pop ó��.
            //     //     CommandManager.Instance.PopCommand();
            //     // }

            //     // // �ൿ�� ���� ���� �������� üũ�մϴ�.
            //     // var entity           = EntityManager.Instance.GetEntity(command.OwnerID);
            //     // var command_progress = (entity != null && entity.GetCommandProgressState() == EnumCommandProgressState.Progress);
            //     // if (command_progress)
            //     // {
            //     //     // �ൿ�� �̾����� �ִٸ� �־��ش�.
            //     //     BattleSystemManager.Instance.BlackBoard.InsertCommandProgressEntity(command.OwnerID);                                          
            //     // }
            //     // else
            //     // {
            //     //     // �ൿ�� �Ϸ�Ǿ��ٸ� ���ش�.
            //     //     BattleSystemManager.Instance.BlackBoard.RemoveCommandProgressEntity(command.OwnerID);
            //     // }
            // }
 
            // queue�� ��� ������� ����ó��.
            // return CommandManager.Instance.PeekCommand() == null;
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

        }        
    }
}
