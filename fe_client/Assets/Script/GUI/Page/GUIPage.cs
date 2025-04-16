using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUIPage : GUIBase
{
    // GUI 


    public struct GUIOpenParam
    {
        public int               SerialNumber;
        public string            GUIName;
        public EnumGUIType       GUIType;        
    }

    public int                   SerialNumber  { get; private set; } = 0;
    public bool                  IsInitialized { get; private set; } = false;
    public abstract EnumGUIType  GUIType       { get; }
    public abstract string       GUIName       { get; }
    

    

    
    // [SerializeField]
    // private EnumGUIType m_gui_type                 = EnumGUIType.None;
    // [SerializeField]
    // private string      m_gui_name                 = string.Empty;
    // [SerializeField]
    // private bool        m_is_enable_multi_instance = false;
    // public abstract bool       IsModal               { get; }


    

    bool OnPreProcess_Open(GUIOpenParam _param)
    {
        if (IsInitialized)
        {
            Debug.Log($"GUIPage: OnPreProcess_Open failed. {GUIName} is already initialized.");
            return false;
        }



        gameObject.SetActive(false);

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
