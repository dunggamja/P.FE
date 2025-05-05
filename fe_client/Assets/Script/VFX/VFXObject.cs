using System;
using UnityEngine;

public class VFXObject : MonoBehaviour
{
    public Int64  SerialNumber { get; private set; } = 0;
    public string VFXName      { get; private set; } = string.Empty;

    public struct VFXAttribute<T> 
    {
        public T     value_start;
        public T     value_end;
        public float time_start;
        public float time_end;

        public VFXAttribute(T _value_start, T _value_end, float _time_start, float _time_end)
        {
            value_start = _value_start;
            value_end   = _value_end;
            time_start  = _time_start;
            time_end    = _time_end;
        }

        public void SetData(T _value_start, T _value_end, float _duration)       
        {
            value_start = _value_start;
            value_end   = _value_end;

            if (_duration > 0f) 
            {
                time_start  = Time.time;
                time_end    = time_start + _duration;
            }
            else
            {
                time_start  = 0f;
                time_end    = 0f;
            }
        }

        

        public float Ratio => 
              (time_end  - time_start  > 0f) 
            ? (Time.time - time_start) / (time_end - time_start) : 1f; 
    }


    // TODO: Parent 밑에 속할 것인지, Follow 할 것인지 기능 필요.
    public Transform  
    Parent   { get; private set; } = null;
   
    public (Vector3 value, float time)    
    Position { get; private set; } = (Vector3.zero, 0f);
    
    public (Quaternion value, float time)    
    Rotation { get; private set; } = (Quaternion.identity, 0f);
    
    public (float value, float time)      
    Scale    { get; private set; } = (1f, 0f);


    

    public void OnCreate(
        Int64      _serial_number,
        string     _vfx_name, 
        Transform  _parent,
        Vector3    _position,
        Quaternion _rotation,
        float      _scale)
    {
        SerialNumber = _serial_number;
        VFXName      = _vfx_name;


        SetParent(_parent);
        SetPosition(_position);
        SetRotation(_rotation);
        SetScale(_scale);

        UpdateTransform(true);
    }

    public void OnRelease()
    {
        SerialNumber = 0;

        gameObject.SetActive(false);

        // transform.SetParent(_parent);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale    = Vector3.one;
    }


    void Update()
    {
        UpdateTransform();
    }

    void LateUpdate()
    {
        // 
    }


    void UpdateTransform(bool _on_create = false)
    {
        if (_on_create)
            gameObject.SetActive(false);


        transform.SetParent(Parent);
        transform.localPosition = Position.value;
        transform.localRotation = Rotation.value;
        transform.localScale    = Vector3.one * Scale.value;


        if (_on_create)
            gameObject.SetActive(true);

    }

    public void SetParent(Transform _parent)
    {
        Parent = _parent;
    }


    public void SetPosition(Vector3 _position, float _time = 0f)
    {
        Position = (_position, _time);
    }

    public void SetRotation(Quaternion _rotation, float _time = 0f)
    {
        Rotation = (_rotation, _time);
    }
    

    public void SetScale(float _scale, float _time = 0f)
    {
        Scale = (_scale, _time);
    }
    
}
