using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class GUIManager : SingletonMono<GUIManager>
{
    // canvas를 sort order 순서에 따라 복수개를 둘 필요는 없는가?...

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;



    Dictionary<Int64,    GUIPage>      m_active_gui              = new (); // serial number, GUIObject
    Dictionary<string, HashSet<Int64>> m_active_gui_by_name      = new (); // 이름, serial number
    AsyncOperationTracker              m_async_operation_tracker = new();



       
    private void Awake()
    {
        var main_camera = Camera.main;
        if (main_camera == null)
        {
            Debug.LogError("UIManager: Main Camera is not found.");
        }
        else
        {
            // 메인 카메라를 참조하도록 설정.
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }


    protected override void OnLoop()
    {
        base.OnLoop();
    }

    public Int64 OpenUI(GUIOpenParam _param)
    {   
        if (IsOpenUI(_param.GUIName))
        {
            Debug.Log($"GUIManager: OpenUI failed. {_param.GUIName} is already open.");
            return 0;
        }

        var cts           = new CancellationTokenSource();

        // 취소 토큰 추적.  
        m_async_operation_tracker.TrackOperation(_param.ID, cts);

        // UI 열기.
        OpenUIAsync(_param, cts.Token).Forget();

        return _param.ID;
    }


    async UniTask OpenUIAsync(GUIOpenParam _param, CancellationToken _cancel_token = default)
    {
        try
        {
            // 활성화된 UI 목록에 추가.
            {
                m_active_gui.Add(_param.ID, null);

                // 이름에 해당하는 UI 목록에 추가.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_gui_id) == false)
                {
                    list_gui_id = new HashSet<Int64>(); 
                    m_active_gui_by_name.Add(_param.GUIName, list_gui_id);
                }
                list_gui_id.Add(_param.ID);
            }

            Transform _ui_root = null;

             switch(_param.GUIType)
            {
                case EnumGUIType.Screen:
                case EnumGUIType.Popup:
                _ui_root = m_canvas_root_screen.transform;
                break;

                case EnumGUIType.HUD:
                _ui_root = m_canvas_root_hud.transform;
                break;
            }

            // 취소 토큰 처리.
            
            var gui_object = await AssetManager.Instance.InstantiateAsync(_param.GUIName, _ui_root, _cancel_token);
            if (gui_object == null)
            {
                Debug.LogError($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");

                throw new Exception($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");
            }

            // GUIPage 컴포넌트 체크.
            if (gui_object.TryGetComponent<GUIPage>(out var gui_page) == false)
            {
                Debug.LogError($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
                GameObject.Destroy(gui_object);

                throw new Exception($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
            }

            // 취소 시 gui_object 제거.
            if (_cancel_token.IsCancellationRequested)
            {
                Debug.Log($"GUIManager: OpenUIAsync canceled. {_param.GUIName}");
                GameObject.Destroy(gui_object);

                _cancel_token.ThrowIfCancellationRequested();
            }


            // 활성화된 UI 목록에 추가.
            m_active_gui[_param.ID] = gui_page;

            // UI 열기.
            gui_page.Open(_param);            
        }
        catch (Exception e)
        {
            Debug.LogError($"GUIManager: OpenUIAsync canceled. {_param.GUIName}, e:{e.Message}");

            {
                // 취소 시 목록 제거.
                m_active_gui.Remove(_param.ID);

                // 이름에 해당하는 UI 목록에 추가.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_gui_id))
                    list_gui_id.Remove(_param.ID);
            }
        }
        
    }


    public void CloseUI(string name)
    {
        // 이름에 해당하는 UI 객체를 모두 닫는다.
        if (m_active_gui_by_name.TryGetValue(name, out var list_gui_id))
        {
            foreach (var e in list_gui_id)
                CloseUI(e);
        }   
    }

    public void CloseUI(Int64 _id)
    {
        if (m_active_gui.TryGetValue(_id, out var gui_object))
        {
            if (gui_object != null)
            {
                // 컨테이너 제거 1.
                m_active_gui.Remove(_id);

                // 컨테이너 제거 2.
                if (m_active_gui_by_name.TryGetValue(gui_object.GUIName, out var list_gui_id))
                    list_gui_id.Remove(_id);

                // UI 닫기 처리.
                gui_object.Close();
            }
        }

        // 취소 토큰에 대해서도 처리.
        m_async_operation_tracker.TryCancelOperation(_id);
    }


    bool IsOpenUI(string _name)
    {
        if (m_active_gui_by_name.TryGetValue(_name, out var list_gui_id))
            return list_gui_id.Count > 0;

        return false;
    }


}
