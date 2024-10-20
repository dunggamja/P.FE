using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Command_Dispatch : BattleSystem//, IEventReceiver
    {
        public BattleSystem_Command_Dispatch() : base(EnumSystem.BattleSystem_Command_Dispatch)
        {
        }

        public override void Init()
        {
            
        }

        public override void Release()
        {
            // m_list_command.Clear();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // turn 정보가 없으면 뭘 할수가 없음.
            var turn_system = BattleSystemManager.Instance.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return true;

            // 현재 행동 진영.
            var faction = turn_system.Faction_Cur;

            // 일단은 행동 가능한 유닛들을 모아봅니다.
            var list_commandable = new Dictionary<EnumCommandProgressState, List<Entity>>(2);
            EntityManager.Instance.Loop(e => 
            { 
                if (e.IsEnableCommandProgress(faction)) 
                {
                    var progress_state = e.GetCommandProgressState(faction);
                    if (!list_commandable.TryGetValue(progress_state, out var list_units))
                    {
                        list_units = new List<Entity>(10);
                        list_commandable.Add(progress_state, list_units); 
                    }

                    list_units.Add(e);
                }                    
            });

            // TODO: 유저가 명령을 내려야 하는 경우에 대한 처리 필요.



            // TODO: 행동중인 유닛이 있다면 그것을 우선처리 해야함.
            if (list_commandable.TryGetValue(EnumCommandProgressState.Progress, out var list_progress))
            {
                // TODO: 뭔가 명령을 내려야 겠지...?
                return true;
            }
            else if(list_commandable.TryGetValue(EnumCommandProgressState.None, out var list_none))
            {
                // 

                // sensor 갱신.
                // TODO: sensor system에 대해서 정리 필요함...;;;;;
                // 현재는 그냥 공용함수로 돌리는 거랑 차이가 없는 상태임....
                list_none.ForEach(e => e.SensorManager.Update());

                // 
                var best_score = (EntityID:(Int64)0, ScoreResult:(Sensor_Target_Score.ScoreResult)new());

                // 가장 점수가 높은 행동을 할 수 있는 유닛을 1명 고릅시다.
                foreach(var e in list_none)
                {
                    var sensor_score = e.SensorManager.GetSensor<Sensor_Target_Score>();
                    if (sensor_score == null)
                        continue;

                    if (best_score.EntityID == 0
                     || best_score.ScoreResult.Calculate_Total_Score() < sensor_score.BestScore.Calculate_Total_Score())
                    {
                        best_score = (e.ID, sensor_score.BestScore);
                    }
                }

                // 공격 명령 셋팅.
                if (best_score.EntityID > 0)
                {                    
                    CommandManager.Instance.PushCommand(
                        new Command_Attack
                        (
                            best_score.EntityID,
                            best_score.ScoreResult.TargetID,
                            best_score.ScoreResult.WeaponID
                        )
                    );
                }

                return true;
            }
            else
            {
                // nothing to do...
                return true;
            }
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

    }
}