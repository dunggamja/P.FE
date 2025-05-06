using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor.Build.Pipeline;
using UnityEngine;


struct VFXAttributeVector3
{
    public Vector3 value_start;
    public Vector3 value_end;
    public float time_start;
    public float time_end;

    public VFXAttributeVector3(Vector3 _value_start, Vector3 _value_end, float _time_start, float _time_end)
    {
        value_start = _value_start;
        value_end = _value_end;
        time_start = _time_start;
        time_end = _time_end;
    }

    public void SetData(Vector3 _value_start, Vector3 _value_end, float _duration)
    {
        value_start = _value_start;
        value_end = _value_end;

        if (_duration > 0f)
        {
            time_start = Time.time;
            time_end = time_start + _duration;
        }
        else
        {
            time_start = 0f;
            time_end = 0f;
        }
    }

    public float Ratio =>
        Mathf.Clamp01(
                (time_end - time_start > 0f)
            ? (Time.time - time_start) / (time_end - time_start) : 1f
        );

    public Vector3 CurrentValue
    {
        get
        {
            return Vector3.Lerp(value_start, value_end, Ratio);
        }
    }
}

struct VFXAttributeQuaternion
{
    public Quaternion value_start;
    public Quaternion value_end;
    public float time_start;
    public float time_end;

    public VFXAttributeQuaternion(Quaternion _value_start, Quaternion _value_end, float _time_start, float _time_end)
    {
        value_start = _value_start;
        value_end = _value_end;
        time_start = _time_start;
        time_end = _time_end;
    }

    public void SetData(Quaternion _value_start, Quaternion _value_end, float _duration)
    {
        value_start = _value_start;
        value_end = _value_end;

        if (_duration > 0f)
        {
            time_start = Time.time;
            time_end = time_start + _duration;
        }
        else
        {
            time_start = 0f;
            time_end = 0f;
        }
    }

    public float Ratio =>
        Mathf.Clamp01(
                (time_end - time_start > 0f)
            ? (Time.time - time_start) / (time_end - time_start) : 1f
        );
    public Quaternion CurrentValue
    {
        get
        {
            return Quaternion.Slerp(value_start, value_end, Ratio);
        }
    }
}

struct VFXAttributeFloat
{
    public float value_start;
    public float value_end;
    public float time_start;
    public float time_end;

    public VFXAttributeFloat(float _value_start, float _value_end, float _time_start, float _time_end)
    {
        value_start = _value_start;
        value_end = _value_end;
        time_start = _time_start;
        time_end = _time_end;
    }

    public void SetData(float _value_start, float _value_end, float _duration)
    {
        value_start = _value_start;
        value_end = _value_end;

        if (_duration > 0f)
        {
            time_start = Time.time;
            time_end = time_start + _duration;
        }
        else
        {
            time_start = 0f;
            time_end = 0f;
        }
    }

    public float Ratio =>
        Mathf.Clamp01(
                (time_end - time_start > 0f)
            ? (Time.time - time_start) / (time_end - time_start) : 1f
        );
    


    public float CurrentValue
    {
        get
        {
            return Mathf.Lerp(value_start, value_end, Ratio);
        }
    }
}


public class VFXObject : MonoBehaviour
{
    public Int64              
    SerialNumber   { get; private set; } = 0;
    
    public string             
    VFXName        { get; private set; } = string.Empty;



    public Transform  
    Parent   { get; private set; } = null;


    VFXAttributeVector3      m_position = new();
    VFXAttributeQuaternion   m_rotation = new();
    VFXAttributeFloat        m_scale    = new();
    public Vector3           Position   => m_position.CurrentValue;
    public Quaternion        Rotation   => m_rotation.CurrentValue;    
    public float             Scale      => m_scale.CurrentValue;

    
    public EnumVFXAttachmentType 
    AttachmentType { get; private set; } = EnumVFXAttachmentType.World;


    

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
        transform.localPosition = Position;
        transform.localRotation = Rotation;
        transform.localScale    = Vector3.one * Scale;


        if (_on_create)
            gameObject.SetActive(true);

    }

    public void SetParent(Transform _parent)
    {
        Parent = _parent;
    }


    public void SetPosition(Vector3 _position, float _time = 0f)
    {
        m_position.SetData(Position,_position, _time);
    }

    public void SetRotation(Quaternion _rotation, float _time = 0f)
    {        
        m_rotation.SetData(Rotation, _rotation, _time);
    }
    

    public void SetScale(float _scale, float _time = 0f)
    {
        m_scale.SetData(Scale, _scale, _time);
    }
    
}
