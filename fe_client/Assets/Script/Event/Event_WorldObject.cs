using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class WorldPositionEvent : IEventParam
    {
        public Int64   ID         { get; private set; } = 0;
        public Vector3 Position   { get; private set; } = Vector3.zero;
        public Vector3 Velocity   { get; private set; } = Vector3.zero;
        public bool    SyncVisual { get; private set; } = false;
        
        public WorldPositionEvent(Int64 _id, Vector3 _position, Vector3 _velocity, bool _sync_visual)
        {
            ID         = _id;
            Position   = _position;
            Velocity   = _velocity;
            SyncVisual = _sync_visual;
        }
    }
}