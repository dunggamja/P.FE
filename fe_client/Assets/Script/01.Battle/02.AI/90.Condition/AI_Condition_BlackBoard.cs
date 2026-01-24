using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class AI_Condition_BlackBoard : ICondition
    { 
        public enum EnumOperator
        {
           Equal,
           NotEqual,

           True,
           False,

           GreaterThan,
           LessThan,
           GreaterThanOrEqual,
           LessThanOrEqual,
        }

        public EnumOperator         OperatorType { get; private set; } = EnumOperator.Equal;
        public EnumEntityBlackBoard Key          { get; private set; } = EnumEntityBlackBoard.None;
        public int                  Value        { get; private set; } = 0;

        public AI_Condition_BlackBoard(EnumOperator _operator_type, EnumEntityBlackBoard _key, int _value = 0)
        {
            OperatorType = _operator_type;
            Key          = _key;
            Value        = _value;
        }


        public bool Verify_Condition(IOwner _owner)
        {
            if (_owner == null)
                return false;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return false;

            var black_board = owner_entity.BlackBoard;

            switch(OperatorType)
            {
                case EnumOperator.Equal:
                    return black_board.GetValue(Key) == Value;
                case EnumOperator.NotEqual:
                    return black_board.GetValue(Key) != Value;
                case EnumOperator.True:
                    return black_board.HasValue(Key) == true;
                case EnumOperator.False:
                    return black_board.HasValue(Key) == false;
                case EnumOperator.GreaterThan:
                    return black_board.GetValue(Key) > Value;
                case EnumOperator.LessThan:
                    return black_board.GetValue(Key) < Value;
                case EnumOperator.GreaterThanOrEqual:
                    return black_board.GetValue(Key) >= Value;
                case EnumOperator.LessThanOrEqual:
                    return black_board.GetValue(Key) <= Value;
            }

            return false;
        }
    }


    public class AI_Condition_AIBlackBoard : ICondition
    { 
        public enum EnumOperator
        {
           Equal,
           NotEqual,

           True,
           False,

           GreaterThan,
           LessThan,
           GreaterThanOrEqual,
           LessThanOrEqual,
        }

        public EnumOperator         OperatorType { get; private set; } = EnumOperator.Equal;
        public EnumAIBlackBoard Key          { get; private set; } = EnumAIBlackBoard.None;
        public int                  Value        { get; private set; } = 0;

        public AI_Condition_AIBlackBoard(EnumOperator _operator_type, EnumAIBlackBoard _key, int _value = 0)
        {
            OperatorType = _operator_type;
            Key          = _key;
            Value        = _value;
        }


        public bool Verify_Condition(IOwner _owner)
        {
            if (_owner == null)
                return false;

            var owner_entity = EntityManager.Instance.GetEntity(_owner.ID);
            if (owner_entity == null)
                return false;

            var ai_black_board = owner_entity.AIManager.AIBlackBoard;

            switch(OperatorType)
            {
                case EnumOperator.Equal:
                    return ai_black_board.GetValue(Key) == Value;
                case EnumOperator.NotEqual:
                    return ai_black_board.GetValue(Key) != Value;
                case EnumOperator.True:
                    return ai_black_board.HasValue(Key) == true;
                case EnumOperator.False:
                    return ai_black_board.HasValue(Key) == false;
                case EnumOperator.GreaterThan:
                    return ai_black_board.GetValue(Key) > Value;
                case EnumOperator.LessThan:
                    return ai_black_board.GetValue(Key) < Value;
                case EnumOperator.GreaterThanOrEqual:
                    return ai_black_board.GetValue(Key) >= Value;
                case EnumOperator.LessThanOrEqual:
                    return ai_black_board.GetValue(Key) <= Value;
            }

            return false;
        }
    }
}