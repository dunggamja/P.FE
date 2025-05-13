using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class VFX_TransformEvent : IEventParam
    {
        
        public EnumEventProcessTiming EventProcessTiming 
        => EnumEventProcessTiming.OnNextUpdate;

        public Int64 
        ID { get; private set; }

        public (bool apply, Transform  value) 
        Parent   { get; private set; }

        public (bool apply, Vector3    value, float time) 
        Position { get; private set; }

        public (bool apply, Quaternion value, float time) 
        Rotation { get; private set; }

        public (bool apply, float      value, float time) 
        Scale    { get; private set; }       
        


        public void Release()
        {
            // ObjectPooling...
            ObjectPool<VFX_TransformEvent>.Return(this);        
        }

        public void Reset()
        {
            ID       = 0;
            Parent   = (false, null);
            Position = (false, Vector3.zero, 0f);
            Rotation = (false, Quaternion.identity, 0f);
            Scale    = (false, 1f, 0f);
        }

        public VFX_TransformEvent SetID(Int64 _id)
        {
            ID = _id;
            return this;
        }

        public VFX_TransformEvent SetParent(Transform _parent)
        {
            Parent = (true, _parent);
            return this;
        }

        public VFX_TransformEvent SetPosition(Vector3 _position, float _time = 0f)
        {
            Position = (true, _position, _time);
            return this;
        }

        public VFX_TransformEvent SetRotation(Quaternion _rotation, float _time = 0f)
        {
            Rotation = (true, _rotation, _time);
            return this;
        }

        public VFX_TransformEvent SetScale(float _scale, float _time = 0f)
        {
            Scale = (true, _scale, _time);
            return this;
        }

    }


