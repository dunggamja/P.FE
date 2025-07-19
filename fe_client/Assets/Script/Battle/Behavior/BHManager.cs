using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{
    public interface IBHParam
    {
        public bool    IsPlan { get; }
        public IOwner  Owner  { get; }
        public ITarget Target { get; }
    }

    public class BHManager
    {

        private List<BHCondition> m_conditions = new List<BHCondition>();
        private List<BHEffect>    m_effects    = new List<BHEffect>();

        public void AddCondition(BHCondition _condition)
        {
            m_conditions.Add(_condition);
        }

        public void AddEffect(BHEffect _effect)
        {
            m_effects.Add(_effect);
        }

        public bool IsValid(IBHParam _param)
        {
            foreach (var condition in m_conditions)
            {
                if (!condition.IsValid(_param))
                    return false;
            }

            return true;
        }

        public void Apply(IBHParam _param)
        {
            foreach (var effect in m_effects)
            {
                effect.Apply(_param);
            }
        }
    }
}