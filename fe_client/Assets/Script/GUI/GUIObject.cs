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

}

public abstract class GUIElement : GUIBase
{
    // button, scrollview, etc.
    // 아직 잘 모르지만, GUI 내부 객체들을 Element 로 분류해볼 예정.
}

public class GUIPage : GUIBase
{
    // GUI 


    public struct GUIOpenParam
    {
        public int       SerialNumber;
        public string    GUIName;
        public Transform Parent;
    }

    public int                 SerialNumber          { get; private set; } = 0;
    public EnumGUIType         GUIType               => m_gui_type;
    public string              GUIName               => m_gui_name;
    public bool                IsEnableMultiInstance => m_is_enable_multi_instance;

    public bool                IsInitialized { get; private set;} = false;

    
    [SerializeField]
    private EnumGUIType m_gui_type                 = EnumGUIType.None;
    [SerializeField]
    private string      m_gui_name                 = string.Empty;
    [SerializeField]
    private bool        m_is_enable_multi_instance = false;
    // public abstract bool       IsModal               { get; }


    

    bool OnPreProcess_Open(GUIOpenParam _param)
    {
        if (IsInitialized)
        {
            Debug.Log($"GUIPage: OnPreProcess_Open failed. {GUIName} is already initialized.");
            return false;
        }

        gameObject.SetActive(false);

        transform.SetParent(_param.Parent);
        transform.SetAsFirstSibling();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale    = Vector3.one;

        SerialNumber  = _param.SerialNumber;
        IsInitialized = true;

        gameObject.SetActive(true);

        return true;
    }

    void OnPostProcess_Close()
    {
        IsInitialized = false;
    }


    public void Open(GUIOpenParam _param)
    {        
        OnPreProcess_Open(_param);
        
        OnOpen();
    }

    public void Close()
    {
        OnClose();

        OnPostProcess_Close();
    }

    protected virtual void OnOpen()
    {

    }

    protected virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
    

}
