using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
    }

    public class EntityBlackBoard : BlackBoard<EnumEntityBlackBoard>
    {
        public AI_Attack.ScoreResult aiscore_attack { get; private set; } = new();

        public Int64 command_progress_entity_id = 0;

        public override void Reset()
        {
            base.Reset();
            aiscore_attack.Reset();
            command_progress_entity_id = 0;
        }
    }

    public class BattleBlackBoard : BlackBoard<EnumBattleBlackBoard>
    {
        public  Int64            aiscore_top_entity_id      { get; set; } = 0;
        private HashSet<Int64>   command_progress_entity_id { get; set; } = new ();

        public void SetCommandProgressEntityID(Int64 _entity_id)
        {
            if (!command_progress_entity_id.Contains(_entity_id))
                 command_progress_entity_id.Add(_entity_id);
        }

        public void RemoveCommandProgressEntityID(Int64 _entity_id)
        {
            command_progress_entity_id.Remove(_entity_id);
        }

        public Int64 PeekCommandProgressEntityID()
        {
            return 0 < command_progress_entity_id.Count ? command_progress_entity_id.First() : 0;
        }

        

        public override void Reset()
        {
            base.Reset();
            aiscore_top_entity_id = 0;
            command_progress_entity_id.Clear();
        }
    }
}

