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

            // TODO: ������ ����� ������ �ϴ� ��쿡 ���� ó�� �ʿ�.


            // TODO: �ൿ���� ������ �ִٸ� �װ��� �켱 ó�� �ؾ���.
            var list_progress = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.Progress);
            if (list_progress.Count > 0)
            {
                // TODO: �ϴ��� �Ϸ� ����� ������ ������ �ص�. 
                //       �ൿ�� �����Ѱ� �ִ��� Ȯ���ϰ� �����ϵ��� �ٲ�� �Ѵ�.
                foreach (var e in list_progress)
                {
                    CommandManager.Instance.PushCommand(new Command_Done(e.ID));
                }

                return true;
            }
            

            var list_none = EntityManager.Instance.Collect(e => e.GetCommandProgressState(faction) == EnumCommandProgressState.None);
           
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