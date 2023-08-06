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

    /// <summary>
    /// BlackBoard
    /// </summary>
    public interface IBlackBoard
    {
        int  GetValue(EnumBlackBoard _type);
        bool HasValue(EnumBlackBoard _type);
        void SetValue(EnumBlackBoard _type, int _value);
        void SetValue(EnumBlackBoard _type, bool _value);
    }

    public class BlackBoardManager
    {
        public IBlackBoard    BlackBoard    { get; }
        public IActionCounter ActionCounter { get; }
    }
}


//BattleState  = 3,   // 공방 상태.
//public enum EnumBattleState
//{
//    None = 0,

//    Attack_Start     = 1,
//    Attack_Hit       = 2,
//    Attack_Dodged    = 3,
//    Attack_Blocked   = 4,
//    Attack_Finished  = 5,

//    Defense_Start    = 11,
//    Defense_Hit      = 12,
//    Defense_Dodged   = 13,
//    Defense_Blocked  = 14,
//    Defense_Finished = 15,
//}


