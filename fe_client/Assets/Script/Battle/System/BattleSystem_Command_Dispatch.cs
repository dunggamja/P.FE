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

            // TODO: 유저가 명령을 내려야 하는 경우에 대한 처리 필요.


            // TODO: 행동중인 유닛이 있다면 그것을 우선 처리 해야함.
            var list_progress = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.Progress);
            if (list_progress.Count > 0)
            {
                // TODO: 일단은 완료 명령을 내리는 것으로 해둠. 
                //       행동이 가능한게 있는지 확인하고 종료하도록 바꿔야 한다.
                foreach (var e in list_progress)
                {
                    CommandManager.Instance.PushCommand(new Command_Done(e.ID));
                }

                return true;
            }
            

            var list_none = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.None);
           
            // sensor 갱신.
            // TODO: sensor system에 대해서 정리 필요함...;;;;;
            // 현재는 그냥 공용함수로 돌리는 거랑 차이가 없는 상태임....
            list_none.ForEach(e => e.SensorManager.Update());

            // TODO: 지금은 공격 스코어가 가장 높은 타겟을 고르고 있는데...
            //       추후에 다른 스코어를 추가할 수 있도록 변경 필요.   (전술적 목표에 따라서.)
            var best_score = (EntityID:(Int64)0, ScoreResult:(Sensor_DamageScore.ScoreResult)new());

            // 가장 점수가 높은 행동을 할 수 있는 유닛을 1명 고릅시다.
            foreach(var e in list_none)
            {
                var sensor_score = e.SensorManager.GetSensor<Sensor_DamageScore>();
                if (sensor_score == null)
                    continue;

                if (best_score.ScoreResult.Calculate_Total_Score() < sensor_score.BestScore.Calculate_Total_Score())
                {
                    best_score = (e.ID, sensor_score.BestScore);
                }
            }

            // 가장 점수가 높은 행동을 할 수 있는 유닛이 있으면 명령을 내립니다.    
            if (best_score.EntityID > 0)
            {   
                Command_By_DamageScore(best_score.EntityID, best_score.ScoreResult);
            }

                
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

        void Command_By_DamageScore(Int64 _entity_id, Sensor_DamageScore.ScoreResult _damage_score)
        {
            // 공격 명령 셋팅.                
            CommandManager.Instance.PushCommand(
                new Command[]
                {
                    // 이동 명령
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        _is_immediate: true // 일단은 즉시 이동.    
                    ),

                    // 공격 명령
                    new Command_Attack
                    (
                        _entity_id,
                        _damage_score.TargetID,
                        _damage_score.WeaponID,
                        _damage_score.Position
                    )

                }
            );
        }

    }
}