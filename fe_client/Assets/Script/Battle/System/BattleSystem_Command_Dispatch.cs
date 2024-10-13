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
        // 명령?�� 처리?��보자.
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

        // 처리?�� 명령 ?��?��?��. ?��차적?���? 처리?��?��.
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
            // TODO: ���߿� ���� �Է� �޴°� ó���ϰ�...
            // �ϴ��� AI ��� ������ �ڵ带 �ۼ��غ���.
            var turn_system = BattleSystemManager.Instance.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return false;

            var faction    = turn_system.Faction_Cur;



            // �ϴ��� �ൿ ������ ���ֵ��� ��ƺ��ϴ�.
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



            // TODO: �ൿ���� ������ �ִٸ� �װ��� �켱ó�� �ؾ���.
            if (list_commandable.TryGetValue(EnumCommandProgressState.Progress, out var list_progress))
            {
                // TODO: ���� �ൿ�Ұ� ������ Ž���ؾ� ����?
                return true;
            }
            else if(list_commandable.TryGetValue(EnumCommandProgressState.None, out var list_none))
            {
                // sensor ����.
                list_none.ForEach(e => e.SensorManager.Update());

                // 
                var best_score = (EntityID:(Int64)0, ScoreResult:(Sensor_Target_Score.ScoreResult)new());

                // ���� ������ ���� �ൿ�� �� �� �ִ� ������ 1�� ���ô�.
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

                // ����� ���� ������ ����.
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
           // EventReceiver�� ���� ���� ������ �߰�������???
        }
    }
}