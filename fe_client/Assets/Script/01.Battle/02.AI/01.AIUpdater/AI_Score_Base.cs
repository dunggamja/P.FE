using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{

    public abstract class AI_Score_Base //: IAIUpdater
    {
        public ConditionHandler ConditionChecker { get; private set; } = new();
        public ApplierHandler   ApplierHandler   { get; private set; } = new();


        public bool Verify_Condition(IAIDataManager _owner)
        {
            if (_owner == null)
                return false;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return false;

            return ConditionChecker.Verify_Condition(owner_entity);
        }

        public void Apply_Effect(IAIDataManager _owner, bool _is_success)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            ApplierHandler.Apply_Effect(owner_entity, _is_success);
        }   

        public AI_Score_Base AddCondition(ICondition _condition)
        {
            ConditionChecker.AddCondition(_condition);
            return this;
        }

        public AI_Score_Base Add_OnSuccess(IApplier _applier)
        {
            ApplierHandler.Add_OnSuccess(_applier);
            return this;
        }
        
        public AI_Score_Base Add_OnFailure(IApplier _applier)
        {
            ApplierHandler.Add_OnFailure(_applier);
            return this;
        }
        
        protected abstract bool OnUpdate(IAIDataManager _owner);


        public void Update(IAIDataManager _param)
        {
           var success = false;

           // 조건 체크.
           if (Verify_Condition(_param) == false)
               return;
                    
            // 업데이트 실행
            success = OnUpdate(_param);

            // 성공/실패에 대한 효과 적용.
            Apply_Effect(_param, success);
        }
    }
}