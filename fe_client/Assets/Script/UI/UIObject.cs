using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumUIType
{
    None,

    //
    Screen,  // 전체 화면에 표시되는 UI
    Popup,   // 팝업 형태의 UI
    HUD,     // 월드 스페이스에 표시되는 UI
}


// interface IUIProperty
// {    
//     EnumUIType UIType                { get; }
//     bool       IsEnableMultiInstance { get; }
// }

public abstract class UIBase : MonoBehaviour//, IUIProperty
{

}
public abstract class UIElement : UIBase
{

}

public class UIPage : UIBase
{
    public struct UIOpenParam
    {
        public int    SerialNumber;
        public string UIName;
    }

    public int                 SerialNumber          { get; private set; } = 0;
    public EnumUIType          UIType                => m_ui_type;
    public string              UIName                => m_ui_name;
    public bool                IsEnableMultiInstance => m_is_enable_multi_instance;

    public bool                IsInitialized { get; private set;} = false;

    
    [SerializeField]
    private EnumUIType m_ui_type                  = EnumUIType.None;
    [SerializeField]
    private string     m_ui_name                  = string.Empty;
    [SerializeField]
    private bool       m_is_enable_multi_instance = false;
    // public abstract bool       IsModal               { get; }


    

    bool OnPreProcess_Open(UIOpenParam _param)
    {
        // if (_param == null)
        // {
        //     Debug.LogError($"UIPage: Open failed. _param is null.");
        //     return false;
        // }

        SerialNumber  = _param.SerialNumber;
        IsInitialized = true;

        return true;
    }

    void OnPostProcess_Close()
    {
        IsInitialized = false;
    }


    public void Open(UIOpenParam _param)
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

    }
    

}
