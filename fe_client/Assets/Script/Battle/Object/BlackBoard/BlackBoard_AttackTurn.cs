using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 전투 공/방 순서
    /// </summary>
    public interface IActionCounter
    {
        int  GetActionCount();
        int  GetAttackCount();

        void AddActionCount(int _value);
        void AddAttackCount(int _value);

        void Reset();
        void ResetAttackCount();

        void ProcessCounter();
    }

    //public struct Counter
    //{
    //    public int Time;
    //    public int Count;
    //}
}
