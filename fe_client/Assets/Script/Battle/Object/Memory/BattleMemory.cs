using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumBlackBoard
    {
        None = 0,

        IsAttacker   = 1,   // 공격자,피격자 체크.
        MoveDistance = 2,   // 이동 거리.
    }

    /// <summary> BlackBoard </summary>
    public interface IBlackBoard
    {
        int  GetValue(EnumBlackBoard _type);
        bool HasValue(EnumBlackBoard _type);
        void SetValue(EnumBlackBoard _type, int _value);
        void SetValue(EnumBlackBoard _type, bool _value);
    }

}

