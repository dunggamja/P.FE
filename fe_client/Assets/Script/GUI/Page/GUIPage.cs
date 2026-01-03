using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public abstract class GUIOpenParam
{
    public Int64                ID             { get; private set; }
    public string               GUIName        { get; private set; }
    public abstract EnumGUIType GUIType        { get; }
    public bool                 IsInputEnabled { get; private set; } = false;
    public virtual bool         IsMultipleOpen { get; private set; } = false;

    protected GUIOpenParam(
        Int64  _id,
        string _gui_name,
        bool   _is_input_enabled = false,
        bool   _is_multiple_open = false)
    {
        ID             = _id;
        GUIName        = _gui_name;
        IsInputEnabled = _is_input_enabled;
        IsMultipleOpen = _is_multiple_open;
    }
}

public abstract class GUIPage : GUIBase
{
    
    public Int64        ID             { get; private set; } = 0;
    public bool         IsInitialized  { get; private set; } = false;
    public EnumGUIType  GUIType        { get; private set; } = EnumGUIType.None;
    public string       GUIName        { get; private set; } = "";
    public bool         IsInputEnabled { get; private set; } = false;
    public bool         IsInputFocused { get; private set; } = false;
    public bool         IsMultipleOpen { get; private set; } = false;

    public bool         IsVisible      { get; private set; } = false;


    public bool         IsInputFocusing
    {
        get
        {
            return ID == GUIManager.Instance.GetInputFocusGUI();
        }
    }


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

    private Tween m_do_show = null;
    private Tween m_do_hide = null;

    // public  bool  IsVisible { get; private set; } = false; // 필요할경우 다시 살리자.

    private bool  IsShowing => m_do_show != null && m_do_show.IsPlaying();
    private bool  IsHiding  => m_do_hide != null && m_do_hide.IsPlaying();


    
    

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
        // GUIType        = _param.GUIType;
        GUIName        = _param.GUIName;
        IsInputEnabled = _param.IsInputEnabled;
        IsMultipleOpen = _param.IsMultipleOpen;
        

        IsInitialized = true;

        gameObject.name = $"[{ID}] {GUIName}";
        gameObject.SetActive(true);

        Show();

        return true;
    }

    public void Open(GUIOpenParam _param)
    { 
        IsClosing = false;

        OnPreProcess_Open(_param);
        
        OnOpen(_param);


        // // 포커스 켜기.
        // if (IsInputFocusing)
        // {
        //     SetInputFocus(true, GUIType);
        // }
    }

    public void Close()
    {
        IsClosing = true;

        OnCloseAsync(this.GetCancellationTokenOnDestroy()).Forget();

        if (this is IEventReceiver)
        {
            EventDispatchManager.Instance.DetachReceiver(this as IEventReceiver);
        }
    }

    public void Show()
    {
        // 포커스 켜기.
        if (IsVisible)
        {
            return;
        }

        SetVisible(true);

        // 포커스 끄기.
        if (IsHiding)
        {
            m_do_hide.Kill();
            m_do_hide = null;
        }


        if (RootCanvasGroup != null)
        {
            m_do_show = 
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
        // 이미 숨기는 처리중.
        if (IsVisible == false)
        {
            return;
        }

        SetVisible(false);

        // 현재 보이는 처리중이면 종료 처리.
        if (IsShowing)
        {
            m_do_show.Kill();
            m_do_show = null;
        }


        if (RootCanvasGroup != null)
        {
            m_do_hide = 
            RootCanvasGroup
                .DOFade(0f, 0.1f)                
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject)
                .SetUpdate(true);
        }
    }

    protected abstract void OnOpen(GUIOpenParam _param);

    protected abstract void OnClose();

    protected abstract void OnPostProcess_Close();


    async UniTask OnCloseAsync(CancellationToken _token)
    {
        try
        {
            // 포커스 끄기.
            // SetInputFocus(false, GUIType);

            

            // 종료 처리.
            OnClose();

            // UI 숨기기. (알파값 Dotween)
            Hide();
            
            // TODO: UI마다 닫히는 시간 제어 필요.
            await UniTask.Delay(100, cancellationToken: _token);

            if (this != null && this.gameObject != null)
            {
                gameObject.SetActive(false);    
            }

            // TODO: ??? 이건 아직 사용하는 곳이 없군.
            OnPostProcess_Close();

            // TODO: GUIElement, GUIPage OnClear 처리 어떻게 할지 고민중.
            OnClear();

            IsInitialized = false;

            // TODO: GUI 풀링 처리.
            if (this != null && this.gameObject != null)
            {
                Destroy(this.gameObject);
            }            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GUIPage: OnCloseAsync failed. {GUIName}, e:{e.Message}");
        }
    }

    public void SetInputFocus(bool _focused, EnumGUIType _focus_gui_type)
    {
        // 포커스 상태가 동일할경우.
        var is_changed_focus = (IsInputFocused != _focused);
        
        // 포커싱 상태.
        IsInputFocused       = _focused;

        if (_focused)
        {
            // 포커싱을 받았을 경우 UI 보이기.
            Show();
        }
        else
        {
            // 전체 화면 UI가 포커싱을 받았을 경우 UI 숨기기.
            if (_focus_gui_type == EnumGUIType.Screen)
                Hide();
        }

        if (is_changed_focus)
            OnInputFocusChanged(_focused);
    }

    protected virtual void OnInputFocusChanged(bool _focused) { }


    private void SetVisible(bool _visible)
    {
        if (IsVisible == _visible)
            return;

        IsVisible = _visible;
        OnVisibleChanged(_visible);
    }

    protected virtual void OnVisibleChanged(bool _visible) { }

    
    protected void CloseSelf() => GUIManager.Instance.CloseUI(ID);


}
