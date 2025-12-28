using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{

   public class AI_Score_Wand : AI_Score_Base
   {
      public class Result : IPoolObject
      {
         public enum EnumHealScore
         {
            TargetHP,       // 타겟 HP (일정 수치 이상 달아있어야 회복)
            HealAmount,     // 회복 양
            TargetPriority, // 타겟 우선순위
            MAX,
         }

         public enum EnumDebuffScore
         {
            TargetStatus,   // 타겟 상태 
            HitRate,        // 명중률
            Max,
         }




         public  Int64          TargetID { get; private set; } = 0;
         public  Int64          WeaponID { get; private set; } = 0;
         public  (int x, int y) Position { get; private set; } = (0, 0);


         public void Reset()
         {
            TargetID = 0;
            WeaponID = 0;
            Position = (0, 0);
         }
      }





      protected override bool OnUpdate(IAIDataManager _param)
      {
         if (_param == null)
               return false;

         var owner_entity = EntityManager.Instance.GetEntity(_param.ID);
         if (owner_entity == null)
               return false;

         return true;
      }
   }
}

