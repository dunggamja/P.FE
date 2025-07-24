using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public static class CombatHelper
    {
        // ���� ���� ����.
        public static void Run_Plan(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id, ref List<CombatLog> _logs)
        {
            // 
            var attacker     = EntityManager.Instance.GetEntity(_attacker_id);
            var target       = EntityManager.Instance.GetEntity(_target_id);  
            var combat_param = ObjectPool<CombatParam_Plan>
                                .Acquire()
                                .Set(attacker, target);

            // ���� ���� ����.
            CombatSystemManager.Instance.Setup(combat_param);

            while (CombatSystemManager.Instance.IsFinished == false)
            {
                CombatSystemManager.Instance.Update();
            }

            // ���� ���� ��� ����.
            combat_param.CopyLogs(ref _logs);

            ObjectPool<CombatParam_Plan>.Return(combat_param);
        }
    }
}