using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Battle
{
    public class BlackBoard<T> where T : unmanaged
    {
        BaseContainer m_repository = new BaseContainer();

        private unsafe int to_int(T _type)
        {
            T*   temp = &_type;
            int* ptr  = (int*)temp;
            return *ptr;
        }

        public int GetValue(T _type)
        {   
            return m_repository.GetValue(to_int(_type));
        }

        public bool HasValue(T _type)
        {
            return m_repository.HasValue(to_int(_type));
        }

        public void SetValue(T _type, int _value)
        {
            m_repository.SetValue(to_int(_type), _value);
        }

        public void SetValue(T _type, bool _value)
        {
            m_repository.SetValue(to_int(_type), _value);            
        }

        public void SetBPValue(T _type, float _value)
        {
            SetValue(_type, (int)(_value * 10000));            
        }

        public float GetBPValueAsFloat(T _type)
        {
            return GetValue(_type) / 10000f;
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


        public virtual void Reset()
        {
            m_repository.Reset();
        }

        public BlackBoard_IO Save()
        {
            return new BlackBoard_IO()
            {
                Values = m_repository.Save()
            };
        }

        public void Load(BlackBoard_IO _snapshot)
        {
            m_repository.Load(_snapshot.Values);
        }
    }

    public class EntityBlackBoard : BlackBoard<EnumEntityBlackBoard>
    {
        public AI_Attack.ScoreResult Score_Attack { get; private set; } = new();


        public override void Reset()
        {
            base.Reset();
            Score_Attack.Reset();
        }
    }

    public class BattleBlackBoard : BlackBoard<EnumBattleBlackBoard>
    {
        // private HashSet<Int64>   CommandProgress_Entities { get; set; } = new ();

        // public void InsertCommandProgressEntity(Int64 _entity_id)
        // {
        //     if (!CommandProgress_Entities.Contains(_entity_id))
        //          CommandProgress_Entities.Add(_entity_id);
        // }

        // public void RemoveCommandProgressEntity(Int64 _entity_id)
        // {
        //     CommandProgress_Entities.Remove(_entity_id);
        // }

        // public Int64 PeekCommandProgressEntity()
        // {
        //     return 0 < CommandProgress_Entities.Count ? CommandProgress_Entities.First() : 0;
        // }

        

        public override void Reset()
        {
            base.Reset();
            // CommandProgress_Entities.Clear();
        }
    }


    public class BlackBoard_IO
    {
        public BaseContainer_IO Values { get; set; } = new();
    }
}

