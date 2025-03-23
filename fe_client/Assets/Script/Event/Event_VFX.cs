using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class VFXTransformEvent : IEventParam
    {
        
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

        public int                            ID       { get; private set; }
        public (bool apply, Transform  value) Parent   { get; private set; }
        public (bool apply, Vector3    value) Position { get; private set; }
        public (bool apply, Quaternion value) Rotation { get; private set; }
        public (bool apply, float      value) Scale    { get; private set; }       
        public  float                         Time     { get; private set; }


        public void Release()
        {
            ObjectPool<VFXTransformEvent>.Return(this);        
        }

        public void Reset()
        {
            ID       = 0;
            Parent   = (false, null);
            Position = (false, Vector3.zero);
            Rotation = (false, Quaternion.identity);
            Scale    = (false, 1f);
            Time     = 0f;
        }

        public VFXTransformEvent SetID(int _id)
        {
            ID = _id;
            return this;
        }

        public VFXTransformEvent SetParent(Transform _parent)
        {
            Parent = (true, _parent);
            return this;
        }

        public VFXTransformEvent SetPosition(Vector3 _position)
        {
            Position = (true, _position);
            return this;
        }

        public VFXTransformEvent SetRotation(Quaternion _rotation)
        {
            Rotation = (true, _rotation);
            return this;
        }

        public VFXTransformEvent SetScale(float _scale)
        {
            Scale = (true, _scale);
            return this;
        }

        public VFXTransformEvent SetTime(float _time)
        {
            Time = _time;
            return this;
        }
    }


