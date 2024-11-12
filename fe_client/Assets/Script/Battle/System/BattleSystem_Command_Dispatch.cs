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
            // turn ������ ������ �� �Ҽ��� ����.
            var turn_system = BattleSystemManager.Instance.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (turn_system == null)
                return true;

            // ���� �ൿ ����.
            var faction = turn_system.Faction_Cur;

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

            // TODO: ������ ����� ������ �ϴ� ��쿡 ���� ó�� �ʿ�.



            // TODO: �ൿ���� ������ �ִٸ� �װ��� �켱ó�� �ؾ���.
            if (list_commandable.TryGetValue(EnumCommandProgressState.Progress, out var list_progress))
            {
                // TODO: ���� ����� ������ ����...?
                //       �ϴ��� �Ϸ� ����� ������ ������ �ص�. 
                foreach (var e in list_progress)
                {
                    CommandManager.Instance.PushCommand(new Command_Done(e.ID));
                }

                return true;
            }
            else if(list_commandable.TryGetValue(EnumCommandProgressState.None, out var list_none))
            {
                // 

                // sensor ����.
                // TODO: sensor system�� ���ؼ� ���� �ʿ���...;;;;;
                // ����� �׳� �����Լ��� ������ �Ŷ� ���̰� ���� ������....
                list_none.ForEach(e => e.SensorManager.Update());

                // TODO: ������ ���� ���ھ ���� ���� Ÿ���� ���� �ִµ�...
                //       ���Ŀ� �ٸ� ���ھ �߰��� �� �ֵ��� ���� �ʿ�.   (������ ��ǥ�� ����.)
                var best_score = (EntityID:(Int64)0, ScoreResult:(Sensor_DamageScore.ScoreResult)new());

                // ���� ������ ���� �ൿ�� �� �� �ִ� ������ 1�� ���ô�.
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

                // ���� ������ ���� �ൿ�� �� �� �ִ� ������ ������ ����� �����ϴ�.    
                if (best_score.EntityID > 0)
                {   
                    Command_By_DamageScore(best_score.EntityID, best_score.ScoreResult);
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

        void Command_By_DamageScore(Int64 _entity_id, Sensor_DamageScore.ScoreResult _damage_score)
        {
            // ���� ��� ����.                
            CommandManager.Instance.PushCommand(
                new Command[]
                {
                    // �̵� ���
                    new Command_Move
                    (
                        _entity_id,
                        _damage_score.Position,
                        _is_immediate: true // �ϴ��� ��� �̵�.    
                    ),

                    // ���� ���
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