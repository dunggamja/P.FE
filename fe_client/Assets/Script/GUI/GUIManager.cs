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



    Dictionary<int,    GUIPage>      m_active_gui              = new (); // serial number, GUIObject
    Dictionary<string, HashSet<int>> m_active_gui_by_name      = new (); // 이름, serial number
    AsyncOperationTracker            m_async_operation_tracker = new();

    // TODO: Open/Close AsyncOperationTracker로 관리해도 될 것 같음.

    private int m_ui_serial = 0;

    private int GenerateSerial()
    {
        do
        { 
            // 시리얼 넘버 증가.
            ++m_ui_serial;

            // 시리얼 넘버가 0이면 1로 변경.
            if (m_ui_serial <= 0)
                m_ui_serial = 1;
            
            // 중복 체크.
        } while (m_active_gui.ContainsKey(m_ui_serial));

        return m_ui_serial;
    }

       
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

    public int OpenUI(string name)
    {       
        var serial_number = GenerateSerial();
        var cts           = new CancellationTokenSource();

        // 취소 토큰 추적.  
        m_async_operation_tracker.TrackOperation(serial_number, cts);

        // UI 열기.
        OpenUIAsync(new GUIPage.GUIOpenParam 
        { 
            SerialNumber = serial_number, 
            GUIName      = name,
            GUIType      = EnumGUIType.Screen
        }, cts.Token).Forget();

        return serial_number;
    }


    async UniTask OpenUIAsync(GUIPage.GUIOpenParam _param, CancellationToken _cancel_token = default)
    {
        try
        {
            // 활성화된 UI 목록에 추가.
            {
                m_active_gui.Add(_param.SerialNumber, null);

                // 이름에 해당하는 UI 목록에 추가.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_sn) == false)
                {
                    list_sn = new HashSet<int>(); 
                    m_active_gui_by_name.Add(_param.GUIName, list_sn);
                }
                list_sn.Add(_param.SerialNumber);
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

           

            // UI 열기.
            gui_page.Open(_param);            
        }
        catch (Exception e)
        {
            Debug.LogError($"GUIManager: OpenUIAsync canceled. {_param.GUIName}, e:{e.Message}");

            {
                // 취소 시 목록 제거.
                m_active_gui.Remove(_param.SerialNumber);

                // 이름에 해당하는 UI 목록에 추가.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_sn))
                    list_sn.Remove(_param.SerialNumber);
            }
        }
        
    }


    public void CloseUI(string name)
    {
        var temp = HashSetPool<int>.Acquire();

        // 이름에 해당하는 UI 객체를 모두 닫는다.
        if (m_active_gui_by_name.TryGetValue(name, out temp))
        {
            foreach (var e in temp)
                CloseUI(e);
        }

        HashSetPool<int>.Return(temp);
    }

    public void CloseUI(int serial_number)
    {
        if (m_active_gui.TryGetValue(serial_number, out var gui_object))
        {
            if (gui_object != null)
            {
                // 컨테이너 제거 1.
                m_active_gui.Remove(serial_number);

                // 컨테이너 제거 2.
                if (m_active_gui_by_name.TryGetValue(gui_object.GUIName, out var temp))
                    temp.Remove(serial_number);

                // UI 닫기 처리.
                gui_object.Close();
            }
        }

        // 취소 토큰에 대해서도 처리.
        m_async_operation_tracker.TryCancelOperation(serial_number);
    }


    
    
}
