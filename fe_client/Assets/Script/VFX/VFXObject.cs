using UnityEngine;

public class VFXObject : MonoBehaviour
{
    public int    SerialNumber { get; private set; } = 0;
    public string VFXName      { get; private set; } = string.Empty;


    // TODO: Parent 밑에 속할 것인지, Follow 할 것인지 기능 필요.
    public Transform  Parent   { get; private set; } = null;
    public Vector3    Position { get; private set; } = Vector3.zero;
    public Quaternion Rotation { get; private set; } = Quaternion.identity;
    public float      Scale    { get; private set; } = 1f;

    bool m_dirty_transform = false;

    public void OnCreate(
        int        _serial_number,
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
        if (m_dirty_transform)
            UpdateTransform();
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

        m_dirty_transform = false;
    }

    public void SetParent(Transform _parent)
    {
        Parent            = _parent;
        m_dirty_transform = true;
    }


    public void SetPosition(Vector3 _position)
    {
        Position          = _position;
        m_dirty_transform = true;
    }

    public void SetRotation(Quaternion _rotation)
    {
        Rotation          = _rotation;
        m_dirty_transform = true;
    }
    

    public void SetScale(float _scale)
    {
        Scale             = _scale;
        m_dirty_transform = true;
    }
    
}
