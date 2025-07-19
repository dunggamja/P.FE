using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    public enum EnumBHConditionType
    {
        None,

        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        True,
        False,
    }


    public abstract class BHCondition
    {
        public abstract bool IsValid(IBHParam _param);
    }

    public class BHCondition_Status : BHCondition
    {
        public EnumBHConditionType ConditionType { get; private set; } = EnumBHConditionType.None;
        public EnumUnitStatus      StatusType    { get; private set; } = EnumUnitStatus.None;
        public int                 Value         { get; private set; } = 0;

        public BHCondition_Status(EnumBHConditionType _condition_type, EnumUnitStatus _status_type, int _value)
            : base()
        {
            ConditionType = _condition_type;
            StatusType    = _status_type;
            Value         = _value;
        }

        public override bool IsValid(IBHParam _param)
        {
            var target_id = _param?.Target?.MainTargetID ?? 0;
            if (target_id == 0)
                return false;

            var target = EntityManager.Instance.GetEntity(target_id);
            if (target == null)
                return false;

            var status_value = target.StatusManager.Status.GetStatus(StatusType, _param.IsPlan);

            switch (ConditionType)
            {
                case EnumBHConditionType.Equal:              return status_value == Value;
                case EnumBHConditionType.NotEqual:           return status_value != Value;
                case EnumBHConditionType.GreaterThan:        return status_value >  Value;
                case EnumBHConditionType.LessThan:           return status_value <  Value;
                case EnumBHConditionType.GreaterThanOrEqual: return status_value >= Value;
                case EnumBHConditionType.LessThanOrEqual:    return status_value <= Value;
                case EnumBHConditionType.True:               return status_value >  0;
                case EnumBHConditionType.False:              return status_value == 0;
            }

            return false;
        }
    }

    public class BHCondition_OR : BHCondition
    {
        public List<BHCondition> Conditions { get; private set; } = new ();

        public BHCondition_OR(List<BHCondition> _conditions)
            : base()
        {
            Conditions = _conditions;
        }

        public override bool IsValid(IBHParam _param)
        {
            if (Conditions.Count == 0)
                return false;

            foreach (var condition in Conditions)
            {
                if (condition.IsValid(_param))
                    return true;
            }

            return false;
        }
    }


    public class BHCondition_NOT : BHCondition
    {   
        public BHCondition Condition { get; private set; } = null;

        public BHCondition_NOT(BHCondition _condition)
            : base()
        {
            Condition = _condition;
        }   

        public override bool IsValid(IBHParam _param)
        {
            return !Condition.IsValid(_param);
        }
    }
}
