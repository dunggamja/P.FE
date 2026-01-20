// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// namespace Battle
// {
//     public interface IBHParam
//     {
//         public bool    IsPlan { get; }
//         // public IOwner  Owner  { get; }
//         public ITarget Target { get; }
//     }
//     public interface IBHCondition
//     {
//         public abstract bool IsValid(IBHParam _param);
//     }

//     public interface IBHEffect
//     {
//         public abstract void Apply(IBHParam _param);
//     }

//     public class BHManager
//     {

//         private List<IBHCondition> m_conditions = new List<IBHCondition>();
//         private List<IBHEffect>    m_effects    = new List<IBHEffect>();

//         public void AddCondition(IBHCondition _condition)
//         {
//             m_conditions.Add(_condition);
//         }

//         public void AddEffect(IBHEffect _effect)
//         {
//             m_effects.Add(_effect);
//         }

//         public bool IsValid(IBHParam _param)
//         {
//             foreach (var condition in m_conditions)
//             {
//                 if (!condition.IsValid(_param))
//                     return false;
//             }

//             return true;
//         }

//         public void Apply(IBHParam _param)
//         {
//             foreach (var effect in m_effects)
//             {
//                 effect.Apply(_param);
//             }
//         }

//         public void Clear()
//         {
//             m_conditions.Clear();
//             m_effects.Clear();
//         }
//     }
// }