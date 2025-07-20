// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TacticsToolkit;
// using UnityEngine;


// namespace Battle
// {
//     public enum EnumBHEffectType
//     {
//         None,
//         Add,
//         Subtract,
//         Set,
//         True,
//         False,
        
//     }

//     public struct BHEffect_Point : IBHEffect
//     {
//         public EnumBHEffectType EffectType { get; private set;  }
//         public EnumUnitPoint    PointType  { get; private set;  }
//         public int              Value      { get; private set;  }

//         public BHEffect_Point(EnumBHEffectType _effect_type, EnumUnitPoint _point_type, int _value)
//         {
//             EffectType = _effect_type;
//             PointType  = _point_type;
//             Value      = _value;
//         }

//         public void Apply(IBHParam _param)
//         {
//             if (_param == null)
//                 return;

//             // 메인 타겟 적용
//             ApplyPoint(_param.Target.MainTargetID, _param.IsPlan);

//             // 부가 타겟 적용
//             foreach (var target_id in _param.Target.OtherTargetIDs)
//             {
//                 ApplyPoint(target_id, _param.IsPlan);
//             }
//         }

//         void ApplyPoint(Int64 _target_id, bool _is_plan)
//         {
//             var target = EntityManager.Instance.GetEntity(_target_id);
//             if (target == null)
//                 return;

//             var prev_value = target.StatusManager.Status.GetPoint(PointType, _is_plan);
//             var new_value  = 0;

//             switch (EffectType)
//             {
//                 case EnumBHEffectType.Add:      new_value = prev_value + Value; break;
//                 case EnumBHEffectType.Subtract: new_value = prev_value - Value; break;
//                 case EnumBHEffectType.Set:      new_value = Value;              break;
//                 case EnumBHEffectType.True:     new_value = 1;                  break;
//                 case EnumBHEffectType.False:    new_value = 0;                  break;
//             }

//             // 포인트 값 보정.
//             new_value = CorrectPoint(_target_id, new_value, _is_plan);

//             target.StatusManager.Status.SetPoint(PointType, new_value, _is_plan);
//         }

//         int CorrectPoint(Int64 _target_id, int _value, bool _is_plan)
//         {
//             var min_value = 0;
//             var max_value = _value;

//             switch (PointType)
//             {
//                 case EnumUnitPoint.HP:
//                 {
//                     var target = EntityManager.Instance.GetEntity(_target_id);
//                     if (target != null)
//                     {
//                         min_value = 0;
//                         max_value = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max, _is_plan);
//                     }
//                 }
//                 break;
//             }

//             return Math.Clamp(_value, min_value, max_value);
//         }



//         static BHEffect_Point Damage(int _damage)
//         {
//             return new BHEffect_Point(EnumBHEffectType.Subtract, EnumUnitPoint.HP, _damage);
//         }

//         static BHEffect_Point Heal(int _heal)
//         {
//             return new BHEffect_Point(EnumBHEffectType.Add, EnumUnitPoint.HP, _heal);
//         }
        
        
//     }





//     public struct BHEffect_Status : IBHEffect
//     {
//         public EnumBHEffectType EffectType { get; private set;  }
//         public EnumUnitStatus   StatusType { get; private set;  }
//         public int              Value      { get; private set;  }

//         public BHEffect_Status(EnumBHEffectType _effect_type, EnumUnitStatus _status_type, int _value)
//         {
//             EffectType = _effect_type;
//             StatusType = _status_type;
//             Value      = _value;
//         }

//         public void Apply(IBHParam _param)
//         {
//             if (_param == null)
//                 return;

//             // 메인 타겟 적용
//             ApplyStatus(_param.Target.MainTargetID, _param.IsPlan);

//             // 부가 타겟 적용
//             foreach (var target_id in _param.Target.OtherTargetIDs)
//             {
//                 ApplyStatus(target_id, _param.IsPlan);
//             }
//         }

//         void ApplyStatus(Int64 _target_id, bool _is_plan)
//         {
//             var target = EntityManager.Instance.GetEntity(_target_id);
//             if (target == null)
//                 return;

//             var prev_value = target.StatusManager.Status.GetStatus(StatusType, _is_plan);
//             var new_value  = 0;

//             switch (EffectType)
//             {
//                 case EnumBHEffectType.Add:      new_value = prev_value + Value; break;
//                 case EnumBHEffectType.Subtract: new_value = prev_value - Value; break;
//                 case EnumBHEffectType.Set:      new_value = Value;              break;
//                 case EnumBHEffectType.True:     new_value = 1;                  break;
//                 case EnumBHEffectType.False:    new_value = 0;                  break;
//             }

//             target.StatusManager.Status.SetStatus(StatusType, new_value, _is_plan);
//         }

//     }
// }