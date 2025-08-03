using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public static class CombatHelper
    {
        // 전투 예측 실행.
        public static void Run_Plan(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id)
        {
            
            var attacker     = EntityManager.Instance.GetEntity(_attacker_id);
            var target       = EntityManager.Instance.GetEntity(_target_id);  
            var combat_param = ObjectPool<CombatParam_Plan>
                                .Acquire()
                                .Set(attacker, target);

            var snapshot = GameSnapshot.Save();

            // 전투 예측 실행.
            CombatSystemManager.Instance.Setup(combat_param);

            while (CombatSystemManager.Instance.IsFinished == false)
            {
                CombatSystemManager.Instance.Update();

                // 전투를 예측해야 하는데... 데미지/힐 수치까지는 알수 있을듯하고...
                // 데미지가 막혔는지 까지도 
            }

            // // 전투 예측 결과 복사.
            // combat_param.CopyLogs(ref _logs);

            ObjectPool<CombatParam_Plan>.Return(combat_param);

            GameSnapshot.Load(snapshot);
        }
    }
}