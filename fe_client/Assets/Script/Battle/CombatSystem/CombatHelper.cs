using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public static class CombatHelper
    {
        // ���� ���� ����.
        public static void Run_Plan(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id)
        {
            
            var attacker     = EntityManager.Instance.GetEntity(_attacker_id);
            var target       = EntityManager.Instance.GetEntity(_target_id);  
            var combat_param = ObjectPool<CombatParam_Plan>
                                .Acquire()
                                .Set(attacker, target);

            Debug.Log("GameSnapshot.Save() Start");
            var snapshot = GameSnapshot.Save();
            Debug.Log("GameSnapshot.Save() End");

            // ���� ���� ����.
            CombatSystemManager.Instance.Setup(combat_param);

            while (CombatSystemManager.Instance.IsFinished == false)
            {
                CombatSystemManager.Instance.Update();

                // ������ �����ؾ� �ϴµ�... ������/�� ��ġ������ �˼� �������ϰ�...
                // �������� �������� ������ 
            }

            // // ���� ���� ��� ����.
            // combat_param.CopyLogs(ref _logs);

            ObjectPool<CombatParam_Plan>.Return(combat_param);

            BattleLogManager.Instance.Logs.ForEach(log => 
            {
                Debug.Log($"{log.LogType} {log.EntityID} {log.Value}");
            });

            Debug.Log("GameSnapshot.Load() Start");
            GameSnapshot.Load(snapshot);
            Debug.Log("GameSnapshot.Load() End");
        }
    }
}