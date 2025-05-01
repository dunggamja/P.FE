using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class GUIOpenParam
{
    public Int64             ID      { get; private set; }
    public string            GUIName { get; private set; }
    public EnumGUIType       GUIType { get; private set; }

    protected GUIOpenParam(Int64 _id, string _gui_name, EnumGUIType _gui_type)
    {
        ID      = _id;
        GUIName = _gui_name;
        GUIType = _gui_type;
    }
}

public abstract class GUIPage : GUIBase
{
    
    public Int64        ID            { get; private set; } = 0;
    public bool         IsInitialized { get; private set; } = false;
    public EnumGUIType  GUIType       { get; private set; } = EnumGUIType.None;
    public string       GUIName       { get; private set; } = "";

    static private Int64 s_id_generator = 0;
    static public Int64 GenerateID()
    {
        return ++s_id_generator;
    }
    

    bool OnPreProcess_Open(GUIOpenParam _param)
    {
        if (IsInitialized)
        {
            Debug.Log($"GUIPage: OnPreProcess_Open failed. {GUIName} is already initialized.");
            return false;
        }

        if (_param == null)
        {
            Debug.LogError($"GUIPage: OnPreProcess_Open failed. _param is null.");
            return false;
        }



        gameObject.SetActive(false);

        ID            = _param.ID;
        GUIType       = _param.GUIType;
        GUIName       = _param.GUIName;
        

        IsInitialized = true;

        gameObject.name = $"[{ID}] {GUIName}";
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
        OnCloseAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    protected virtual void OnOpen()
    {

    }

    protected virtual void OnClose()
    {
    }

    async UniTask OnCloseAsync(CancellationToken _token)
    {
        try
        {
            OnClose();
            
            // TODO: 이것은 임시로 추후 비동기 처리를 위해 넣은 코드임.
            await UniTask.Delay(100, cancellationToken: _token);

            if (this != null && this.gameObject != null)
            {
                gameObject.SetActive(false);    
            }

            
            OnPostProcess_Close();


            // 파괴 처리.
            if (this != null && this.gameObject != null)
            {
                Destroy(this.gameObject);
            }            
        }
        catch (System.Exception)
        {
            
        }
    }
    

}
