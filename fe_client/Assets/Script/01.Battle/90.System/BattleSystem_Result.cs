using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Battle
{

    public class BattleSystem_Result : BattleSystem
    {
        private UniTask m_task;


        public BattleSystem_Result() : base(EnumSystem.BattleSystem_Result)
        {
        }
        protected override void OnInit()
        {
            // throw new NotImplementedException();
        }

        protected override void OnRelease()
        {
            // throw new NotImplementedException();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            // throw new NotImplementedException();
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // 승리/패배 조건 체크.
            var battle_result  = (EnumBattleResult)BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.Battle_Result);
            if (battle_result == EnumBattleResult.None && m_task.Status != UniTaskStatus.Pending)
            {           
                // 패배 체크.
                if (CutsceneManager.Instance.VerifyPlayEvent(
                    CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCheckDefeat)))
                {
                    battle_result = EnumBattleResult.Defeat;
                }
                // 승리 체크.
                else if (CutsceneManager.Instance.VerifyPlayEvent(
                         CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCheckVictory)))
                {
                    battle_result = EnumBattleResult.Victory;
                }

                // 승리 / 패배 처리.
                if (battle_result != EnumBattleResult.None)
                    m_task = Process_Result(battle_result);
            }
            

            // 처리 중인 작업이 있는지 체크.
            if (m_task.Status == UniTaskStatus.Pending)
                return false;

            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            // throw new NotImplementedException();
        }

        bool Check_BattleResult()
        {
           // 전투 승패 결과 검사.


           return false;
        }

        async UniTask Process_Result(EnumBattleResult _result)
        {
            if (_result == EnumBattleResult.None)
            {
                // 승/패 처리 할 것이 없으므로 종료처리.
                return;
            }


            // 승리/패배 처리.
            BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.Battle_Result, (int)_result);

            if (_result == EnumBattleResult.Victory)
            {
                // 승리 이벤트 발생.
                CutsceneManager.Instance.OnPlayEvent(
                    CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnVictory));
            }
            else if (_result == EnumBattleResult.Defeat)
            {
                // 패배 이벤트 발생.
                CutsceneManager.Instance.OnPlayEvent(
                    CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnDefeat));
            }


            // 승리/패배 이벤트 컷씬 연출이 완료될때까지 대기.
            await UniTask.WaitUntil(()=> CutsceneManager.Instance.IsPlayingCutscene == false);            


            // 승리 패배 GUI 표시.
            var gui_id = GUIManager.Instance.OpenUI(GUIPage_Battle_Result.PARAM.Create(_result));

            // 승리/패배 GUI 오픈 대기.
            await GUIManager.Instance.WaitForOpenUI(gui_id, CancellationToken.None);

            // 승리/패배 GUI 종료 대기.
            await GUIManager.Instance.WaitForCloseUI(gui_id, CancellationToken.None);   

            // 전투 종료 처리.
            BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.IsBattleFinished, true);        
        }


    }

}