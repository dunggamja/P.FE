using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class VFXTransformEvent : IEventParam
    {
        
        public EnumEventProcessTiming EventProcessTiming 
        => EnumEventProcessTiming.OnNextUpdate;

        public int ID { get; private set; }

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
            ObjectPool<VFXTransformEvent>.Return(this);        
        }

        public void Reset()
        {
            ID       = 0;
            Parent   = (false, null);
            Position = (false, Vector3.zero, 0f);
            Rotation = (false, Quaternion.identity, 0f);
            Scale    = (false, 1f, 0f);
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

        public VFXTransformEvent SetPosition(Vector3 _position, float _time = 0f)
        {
            Position = (true, _position, _time);
            return this;
        }

        public VFXTransformEvent SetRotation(Quaternion _rotation, float _time = 0f)
        {
            Rotation = (true, _rotation, _time);
            return this;
        }

        public VFXTransformEvent SetScale(float _scale, float _time = 0f)
        {
            Scale = (true, _scale, _time);
            return this;
        }

    }


