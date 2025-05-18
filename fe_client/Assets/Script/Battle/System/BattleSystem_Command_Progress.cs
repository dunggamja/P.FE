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
            // 요거를 각 엔티티별로 돌리도록 변경해야 겠다. 


            // // command queue가 모두 종료될 때 까지 돌아봅시다.
            // var command  = BattleSystemManager.Instance.PeekCommand();
            // if (command != null)
            // {
            //     // 이건 CommandManager 안으로...
            //     // if (command.Update() == EnumState.Finished)
            //     // {
            //     //     // command가 완료되면 pop 처리.
            //     //     CommandManager.Instance.PopCommand();
            //     // }

            //     // // 행동을 진행 중인 상태인지 체크합니다.
            //     // var entity           = EntityManager.Instance.GetEntity(command.OwnerID);
            //     // var command_progress = (entity != null && entity.GetCommandProgressState() == EnumCommandProgressState.Progress);
            //     // if (command_progress)
            //     // {
            //     //     // 행동이 이어지고 있다면 넣어준다.
            //     //     BattleSystemManager.Instance.BlackBoard.InsertCommandProgressEntity(command.OwnerID);                                          
            //     // }
            //     // else
            //     // {
            //     //     // 행동이 완료되었다면 빼준다.
            //     //     BattleSystemManager.Instance.BlackBoard.RemoveCommandProgressEntity(command.OwnerID);
            //     // }
            // }
 
            // queue가 모두 비었으면 종료처리.
            // return CommandManager.Instance.PeekCommand() == null;
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

        }        
    }
}
