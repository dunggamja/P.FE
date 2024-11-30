using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class EntityBlackBoard //: IBlackBoard
    {
        BaseContainer m_repository = new BaseContainer();

        public int GetValue(EnumEntityBlackBoard _type)
        {
            return m_repository.GetValue((int)_type);
        }

        public bool HasValue(EnumEntityBlackBoard _type)
        {
            return m_repository.HasValue((int)_type);
        }

        public void SetValue(EnumEntityBlackBoard _type, int _value)
        {
            m_repository.SetValue((int)_type, _value);
        }

        public void SetValue(EnumEntityBlackBoard _type, bool _value)
        {
            m_repository.SetValue((int)_type, _value);            
        }

        public void SetBitFlag(EnumEntityBlackBoard _type, byte _bit_index)
        {
            SetValue(_type, GetValue(_type) | (1 << _bit_index));
        }

        public void ResetBitFlag(EnumEntityBlackBoard _type, byte _bit_index)
        {
            SetValue(_type, GetValue(_type) & ~(1 << _bit_index));
        }

        public bool HasBitFlag(EnumEntityBlackBoard _type, byte _bit_index)
        {
            return 0 != (GetValue(_type) & (1 << _bit_index));
        }

    }
}

