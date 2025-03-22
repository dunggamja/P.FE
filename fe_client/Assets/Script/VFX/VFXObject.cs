using UnityEngine;

public class VFXObject : MonoBehaviour
{
    public int    SerialNumber { get; private set; } = 0;
    public string VFXName      { get; private set; } = string.Empty;

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

        gameObject.SetActive(false);

        transform.SetParent(_parent);
        transform.localPosition = _position;
        transform.localRotation = _rotation;
        transform.localScale    = Vector3.one * _scale;

        gameObject.SetActive(true);
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

}
