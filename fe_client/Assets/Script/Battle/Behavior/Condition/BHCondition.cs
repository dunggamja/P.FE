// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// namespace Battle
// {
//     public enum EnumBHConditionType
//     {
//         None,

//         Equal,
//         NotEqual,
//         GreaterThan,
//         LessThan,
//         GreaterThanOrEqual,
//         LessThanOrEqual,
//         True,
//         False,
//     }




//     public struct BHCondition_Status : IBHCondition
//     {
//         public EnumBHConditionType ConditionType { get; private set; }
//         public EnumUnitStatus      StatusType    { get; private set; }
//         public int                 Value         { get; private set; }

//         public BHCondition_Status(EnumBHConditionType _condition_type, EnumUnitStatus _status_type, int _value)
//         {
//             ConditionType = _condition_type;
//             StatusType    = _status_type;
//             Value         = _value;
//         }

//         public bool IsValid(IBHParam _param)
//         {
//             if (_param == null)
//                 return false;

            

//             // 메인 타겟 체크
//             if (CheckCondition(_param.Target.MainTargetID, _param.IsPlan) == false)
//                 return false;

//             // 부가 타겟 체크
//             foreach (var target_id in _param.Target.OtherTargetIDs)
//             {
//                 if (CheckCondition(target_id, _param.IsPlan) == false)
//                     return false;
//             }

//             return true;
//         }

//         bool CheckCondition(Int64 _target_id, bool _is_plan)
//         {
//             var target = EntityManager.Instance.GetEntity(_target_id);
//             if (target == null)
//                 return false;

//             var status_value = target.StatusManager.Status.GetStatus(StatusType, _is_plan);

//             switch (ConditionType)
//             {
//                 case EnumBHConditionType.Equal:              return status_value == Value;
//                 case EnumBHConditionType.NotEqual:           return status_value != Value;
//                 case EnumBHConditionType.GreaterThan:        return status_value >  Value;
//                 case EnumBHConditionType.LessThan:           return status_value <  Value;
//                 case EnumBHConditionType.GreaterThanOrEqual: return status_value >= Value;
//                 case EnumBHConditionType.LessThanOrEqual:    return status_value <= Value;
//                 case EnumBHConditionType.True:               return status_value >  0;
//                 case EnumBHConditionType.False:              return status_value == 0;
//             }

//             return false;

//         }
//     }

//     public struct BHCondition_OR : IBHCondition
//     {
//         public List<IBHCondition> Conditions { get; private set; }

//         public BHCondition_OR(List<IBHCondition> _conditions)
//         {
//             Conditions = _conditions;
//         }

//         public bool IsValid(IBHParam _param)
//         {
//             if (Conditions.Count == 0)
//                 return false;

//             foreach (var condition in Conditions)
//             {
//                 if (condition.IsValid(_param))
//                     return true;
//             }

//             return false;
//         }
//     }


//     public struct BHCondition_NOT : IBHCondition
//     {   
//         public IBHCondition Condition { get; private set; }

//         public BHCondition_NOT(IBHCondition _condition)
//         {
//             Condition = _condition;
//         }   

//         public bool IsValid(IBHParam _param)
//         {
//             return !Condition.IsValid(_param);
//         }
//     }
// }
