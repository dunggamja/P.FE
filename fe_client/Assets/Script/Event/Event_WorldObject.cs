using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace Battle
// {
    public class WorldObject_PositionEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

        public Int64   ID            { get; private set; } = 0;
        public Vector3 Position      { get; private set; } = Vector3.zero;
        public Vector3 Position_Prev { get; private set; } = Vector3.zero;
        // public float   Position_Time { get; private set; } = 0f;
        
        public WorldObject_PositionEvent Set(Int64 _id, Vector3 _position, Vector3 _position_prev)
        {
            ID            = _id;
            Position      = _position;
            Position_Prev = _position_prev;
            // Position_Time = _position_time;
            return this;
        }

        public void Reset()
        {
            ID            = 0;
            Position      = Vector3.zero;
            Position_Prev = Vector3.zero;
        }

        public void Release()
        {
            var temp = this;
            ObjectPool<WorldObject_PositionEvent>.Return( temp);
        }
    }
// }