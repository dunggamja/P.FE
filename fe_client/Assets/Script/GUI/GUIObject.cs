using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumGUIType
{
    None,

    //
    Screen,  // 전체 화면에 표시되는 UI
    Popup,   // 팝업 형태의 UI
    HUD,     // 월드 스페이스에 표시되는 UI
}

public enum EnumGUIState
{
    None,
    Opening, // 열리는 중   
    Opened,  // 열림
    Closing, // 닫는 중
    Closed,  // 닫힘
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
    // 아직 잘 모르지만, GUI 내부 객체들을 Element 로 분류해볼 예정.
}
