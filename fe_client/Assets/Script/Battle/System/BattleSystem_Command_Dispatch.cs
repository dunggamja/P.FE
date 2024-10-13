using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

namespace Battle
{
    public partial class BattleSystem_Command_Dispatch : BattleSystem, IEventReceiver
    {
        // 紐낅졊?쓣 泥섎━?빐蹂댁옄.
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

        // 泥섎━?븷 紐낅졊 ?뜲?씠?꽣. ?닚李⑥쟻?쑝濡? 泥섎━?븳?떎.
        // Queue<CommandData> m_list_command;       


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
            // TODO: 나중에 유저 입력 받는거 처리하고...
            // 일단은 AI 명령 내리는 코드를 작성해봅세.
            var turn_system = BattleSystemManager.Instance.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return false;

            var faction    = turn_system.Faction_Cur;



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



            // TODO: 행동중인 유닛이 있다면 그것을 우선처리 해야함.
            if (list_commandable.TryGetValue(EnumCommandProgressState.Progress, out var list_progress))
            {
                // TODO: 뭔가 행동할게 있을지 탐색해야 겠지?
                return true;
            }
            else if(list_commandable.TryGetValue(EnumCommandProgressState.None, out var list_none))
            {
                // sensor 갱신.
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

                // 명령을 내릴 유닛이 없음.
                if (best_score.EntityID == 0)
                    return true;
                
                var entity = EntityManager.Instance.GetEntity(best_score.EntityID);
                if (entity == null)
                    return true;

                

                

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


        public void OnReceiveEvent(IEventParam _param)
        {
           // EventReceiver를 굳이 무슨 이유로 추가했을까???
        }
    }
}