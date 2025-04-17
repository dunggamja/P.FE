using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumGUIType
{
    None,

    //
    Screen,  // ��ü ȭ�鿡 ǥ�õǴ� UI
    Popup,   // �˾� ������ UI
    HUD,     // ���� �����̽��� ǥ�õǴ� UI
}

public enum EnumGUIState
{
    None,
    Opening, // ������ ��   
    Opened,  // ����
    Closing, // �ݴ� ��
    Closed,  // ����
}


public abstract class GUIBase : MonoBehaviour//, IUIProperty
{
    private Vector3    m_init_local_position = Vector3.zero;
    private Quaternion m_init_local_rotation = Quaternion.identity;
    private Vector3    m_init_local_scale = Vector3.one;

    virtual protected void Awake()
    {
        m_init_local_position = transform.localPosition;
        m_init_local_rotation = transform.localRotation;
        m_init_local_scale    = transform.localScale;
    }

    protected void Init_Transform(Transform _parent)
    {
        transform.SetParent(_parent);
        transform.localPosition = m_init_local_position;
        transform.localRotation = m_init_local_rotation;
        transform.localScale    = m_init_local_scale;
    }
}

public abstract class GUIElement : GUIBase
{
    // button, scrollview, etc.
    // ���� �� ������, GUI ���� ��ü���� Element �� �з��غ� ����.
}
