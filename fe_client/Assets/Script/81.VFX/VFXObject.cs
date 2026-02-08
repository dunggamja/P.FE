using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Battle;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;




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


struct VFXAttributeTerrain
{
    public bool  snap_to_terrain;
    
    public float snap_offset;
}



public class VFXObject : MonoBehaviour
{
    public class Param : IPoolObject
    {
        public string     VFXName           { get; set; } = string.Empty;
        public Transform  VFXRoot           { get; set; } = null;
        public Vector3    Position          { get; set; } = Vector3.zero;
        public Quaternion Rotation          { get; set; } = Quaternion.identity;       
        public float      Scale             { get; set; } = 1f;
        public (bool snap, float snap_offset) SnapToTerrain { get; set; } = (false, 0f);

        public (Transform target, EnumVFXAttachmentType attachment_type) 
        FollowTarget { get; set; } = (null, EnumVFXAttachmentType.World);

        

        public virtual void Reset()
        {
            VFXName       = string.Empty;
            VFXRoot       = null;
            Position      = Vector3.zero;
            Rotation      = Quaternion.identity;
            Scale         = 1f;
            SnapToTerrain = (false, 0f);
            FollowTarget  = (null, EnumVFXAttachmentType.World);
        }

        public Param SetVFXName(string _vfx_name)
        {
            VFXName = _vfx_name;
            return this;
        }

        public Param SetVFXRoot_Default()
        {
            // TODO: 오브젝트 생성 위치 셋팅.
            // 오브젝트 생성 위치 셋팅.
            VFXRoot = WorldObjectManager.Instance.transform;
            return this;
        }

        public Param SetVFXRoot(Transform _vfx_root)
        {
            VFXRoot = _vfx_root;
            return this;
        }

        public Param SetPosition(Vector3 _position)
        {
            Position = _position;
            return this;
        }

        public Param SetRotation(Quaternion _rotation)
        {
            Rotation = _rotation;
            return this;
        }

        public Param SetScale(float _scale)
        {
            Scale = _scale;
            return this;
        }

        public Param SetFollowTarget(Transform _target, EnumVFXAttachmentType _attachment_type)
        {
            FollowTarget = (_target, _attachment_type);
            return this;
        }

        public Param SetSnapToTerrain(bool _snap_to_terrain, float _snap_offset)
        {
            SnapToTerrain = (_snap_to_terrain, _snap_offset);
            return this;
        }

        public virtual void Apply(VFXObject _vfx_object)
        {
            _vfx_object.SetParent(VFXRoot);
            _vfx_object.SetPosition(Position);
            _vfx_object.SetRotation(Rotation);
            _vfx_object.SetScale(Scale);
            _vfx_object.SetSnapToTerrain(SnapToTerrain.snap, SnapToTerrain.snap_offset);
        }
    }

    public Int64              
    SerialNumber   { get; private set; } = 0;
    
    public string             
    VFXName        { get; private set; } = string.Empty;



    public Transform  
    Parent   { get; private set; } = null;


    VFXAttributeVector3      m_position = new();
    VFXAttributeQuaternion   m_rotation = new();
    VFXAttributeFloat        m_scale    = new();
    VFXAttributeTerrain      m_terrain  = new();


    public Vector3           Position   => m_position.CurrentValue;
    public Quaternion        Rotation   => m_rotation.CurrentValue;    
    public float             Scale      => m_scale.CurrentValue;

    
    public EnumVFXAttachmentType 
    AttachmentType { get; private set; } = EnumVFXAttachmentType.World;


    

    public void OnCreate(
        Int64 _serial_number,
        Param _param)
    {
        SerialNumber = _serial_number;
        VFXName      = _param.VFXName;

        _param.Apply(this);

        UpdateTransform(true);

#if UNITY_EDITOR
        // 하이어라키에서 디버깅시에만 사용하는 값.
        gameObject.name = $"[{SerialNumber}] {VFXName}";        
#endif

        OnCreatePostProcess(_param);
    }

    protected virtual void OnCreatePostProcess(VFXObject.Param _param) {}

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
        bool is_transform_changed = false;


        if (_on_create)
        {
            // 처음 생성시 처리하는 부분
            gameObject.SetActive(false);

            is_transform_changed = true;
        }
        else
        {
            // Update시 처리하는 부분.
            is_transform_changed = 
               (transform.parent        != Parent)
            || (transform.localPosition != Position)
            || (transform.localRotation != Rotation)
            || (transform.localScale    != Vector3.one * Scale);
        }

        // Transform 셋팅.
        if (is_transform_changed)
        {
            transform.SetParent(Parent);
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale    = Vector3.one * Scale;
            ApplySnapToTerrain();
        }


        if (_on_create)
        {
            // 오브젝트 활성화
            gameObject.SetActive(true);
        }
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

    public void SetSnapToTerrain(bool _snap_to_terrain, float _snap_offset)
    {
        m_terrain.snap_to_terrain = _snap_to_terrain;
        m_terrain.snap_offset     = _snap_offset;
    }


    protected virtual void ApplySnapToTerrain()
    {
        transform.position = SnapToTerrain(transform.position);
    }


    protected Vector3 SnapToTerrain(Vector3 _position, bool _exclude_fixedobjects = false)
    {
        if (m_terrain.snap_to_terrain == false)
            return _position;

        var new_height = TerrainMapManager.Instance.GetWorldHeight(_position, _exclude_fixedobjects)
                       + m_terrain.snap_offset;

        return new Vector3(_position.x, new_height,_position.z);
    }

    
}
