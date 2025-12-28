using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Applier_BlackBoard : IApplier
    {
        public EnumEntityBlackBoard Key       { get; private set; } = EnumEntityBlackBoard.None;
        public int                  Value     { get; private set; } = 0;
      //   public EnumValue            ValueType { get; private set; } = EnumValue.Integer;

        public AI_Applier_BlackBoard(EnumEntityBlackBoard _key, int _value)//, EnumValue _value_type = EnumValue.Integer)
        {
            Key       = _key;
            Value     = _value;
            // ValueType = _value_type;
        }


        public void Apply_Effect(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            owner_entity.BlackBoard.SetValue(Key, Value);
        }
    }

    public class AI_Applier_AIBlackBoard : IApplier
    {
        public EnumAIBlackBoard Key       { get; private set; } = EnumAIBlackBoard.None;
        public int                  Value     { get; private set; } = 0;
      //   public EnumValue            ValueType { get; private set; } = EnumValue.Integer;

        public AI_Applier_AIBlackBoard(EnumAIBlackBoard _key, int _value)//, EnumValue _value_type = EnumValue.Integer)
        {
            Key       = _key;
            Value     = _value;
            // ValueType = _value_type;
        }


        public void Apply_Effect(IOwner _owner)
        {
            if (_owner == null)
                return;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return;

            owner_entity.AIManager.AIBlackBoard.SetValue(Key, Value);
        }
    }
}