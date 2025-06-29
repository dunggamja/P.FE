using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class GUIOpenParam
{
    public Int64             ID             { get; private set; }
    public string            GUIName        { get; private set; }
    public EnumGUIType       GUIType        { get; private set; }
    public bool              IsInputEnabled { get; private set; } = false;

    protected GUIOpenParam(Int64 _id, string _gui_name, EnumGUIType _gui_type, bool _is_input_enabled = false)
    {
        ID             = _id;
        GUIName        = _gui_name;
        GUIType        = _gui_type;
        IsInputEnabled = _is_input_enabled;
    }
}

public abstract class GUIPage : GUIBase
{
    
    public Int64        ID            { get; private set; } = 0;
    public bool         IsInitialized { get; private set; } = false;
    public EnumGUIType  GUIType       { get; private set; } = EnumGUIType.None;
    public string       GUIName       { get; private set; } = "";
    public bool         IsInputEnabled { get; private set; } = false;

    static private Int64 s_id_generator = 0;
    static public Int64 GenerateID()
    {
        return ++s_id_generator;
    }

    CanvasGroup m_root_canvas_group = null;
    CanvasGroup RootCanvasGroup
    {
        get
        {
            if (m_root_canvas_group == null)
                m_root_canvas_group = GetComponent<CanvasGroup>();

            return m_root_canvas_group;
        }
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

        if (this is IEventReceiver)
        {
            EventDispatchManager.Instance.AttachReceiver(this as IEventReceiver);
        }
        



        gameObject.SetActive(false);

        ID             = _param.ID;
        GUIType        = _param.GUIType;
        GUIName        = _param.GUIName;
        IsInputEnabled = _param.IsInputEnabled;
        

        IsInitialized = true;

        gameObject.name = $"[{ID}] {GUIName}";
        gameObject.SetActive(true);

        Show();

        return true;
    }




    public void Open(GUIOpenParam _param)
    {        
        OnPreProcess_Open(_param);
        
        OnOpen(_param);
    }

    public void Close()
    {
        OnCloseAsync(this.GetCancellationTokenOnDestroy()).Forget();

        if (this is IEventReceiver)
        {
            EventDispatchManager.Instance.DetachReceiver(this as IEventReceiver);
        }
    }

    public void Show()
    {
        if (RootCanvasGroup != null)
        {
            RootCanvasGroup
                .DOFade(1, 0.1f)
                .From(0f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject)
                .SetUpdate(true);
        }
    }

    public void Hide()
    {
        if (RootCanvasGroup != null)
        {
            RootCanvasGroup
                .DOFade(0f, 0.1f)                
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject)
                .SetUpdate(true);
        }

        // 비활성화도 할것인지는 아직 미정.
    }

    protected abstract void OnOpen(GUIOpenParam _param);

    protected abstract void OnClose();

    protected abstract void OnPostProcess_Close();

    // public abstract void Process_InputEvent(IEventParam _event);

    async UniTask OnCloseAsync(CancellationToken _token)
    {
        try
        {
            OnClose();

            Hide();
            
            // TODO: 이것은 임시로 추후 비동기 처리를 위해 넣은 코드임.
            await UniTask.Delay(100, cancellationToken: _token);

            if (this != null && this.gameObject != null)
            {
                gameObject.SetActive(false);    
            }

            OnPostProcess_Close();

            IsInitialized = false;

            // TODO: GUI를 파괴 할지 재활용할지는 나중에 다시 고민해보자.
            // 일단은 파괴 처리.
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
