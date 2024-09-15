using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleBlackBoard : IBlackBoard
    {
        BaseContainer m_repository = new BaseContainer();

        public int GetValue(EnumBlackBoard _type)
        {
            return m_repository.GetValue((int)_type);
        }

        public bool HasValue(EnumBlackBoard _type)
        {
            return m_repository.HasValue((int)_type);
        }

        public void SetValue(EnumBlackBoard _type, int _value)
        {
            m_repository.SetValue((int)_type, _value);
        }

        public void SetValue(EnumBlackBoard _type, bool _value)
        {
            m_repository.SetValue((int)_type, _value);
            
        }
    }
}

