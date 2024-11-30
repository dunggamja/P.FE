using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BlackBoard<T> where T : Enum
    {
        BaseContainer m_repository = new BaseContainer();

        public int GetValue(T _type)
        {
            return m_repository.GetValue(Convert.ToInt32(_type));
        }

        public bool HasValue(T _type)
        {
            return m_repository.HasValue(Convert.ToInt32(_type));
        }

        public void SetValue(T _type, int _value)
        {
            m_repository.SetValue(Convert.ToInt32(_type), _value);
        }

        public void SetValue(T _type, bool _value)
        {
            m_repository.SetValue(Convert.ToInt32(_type), _value);            
        }

        public void SetBitFlag(T _type, byte _bit_index)
        {
            SetValue(_type, GetValue(_type) | (1 << _bit_index));
        }

        public void ResetBitFlag(T _type, byte _bit_index)
        {
            SetValue(_type, GetValue(_type) & ~(1 << _bit_index));
        }

        public bool HasBitFlag(T _type, byte _bit_index)
        {
            return 0 != (GetValue(_type) & (1 << _bit_index));
        }
        
    }

    public class EntityBlackBoard : BlackBoard<EnumEntityBlackBoard>
    {
        
    }

    public class BattleBlackBoard : BlackBoard<EnumBattleBlackBoard>
    {

    }
}

