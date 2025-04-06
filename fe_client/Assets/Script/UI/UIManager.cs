using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class UIManager : SingletonMono<UIManager>
{
    // canvas를 sort order 순서에 따라 복수개를 둘 필요는 없는가?...

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;



    Dictionary<int,    UIPage>       m_active_ui         = new (); // serial number, UIObject
    Dictionary<string, HashSet<int>> m_active_ui_by_name = new (); // 이름, serial number

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
        } while (m_active_ui.ContainsKey(m_ui_serial));

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

    public int OpenUI(string name)
    {       
        var serial_number = GenerateSerial();
        var cts           = new CancellationTokenSource();

        // 취소 토큰 추적.  
        m_async_operation_tracker.TrackOperation(serial_number, cts);

        // UI 열기.
        OpenUIAsync(new UIPage.UIOpenParam { SerialNumber = serial_number, UIName = name }, cts.Token).Forget();

        return serial_number;
    }


    async UniTask OpenUIAsync(UIPage.UIOpenParam _param, CancellationToken _cancel_token = default)
    {
        try
        {
            var ui_object = await AssetManager.Instance.InstantiateAsync(_param.UIName, _cancel_token);
            if (ui_object == null)
            {
                Debug.LogError($"UIManager: OpenUIAsync failed to instantiate {_param.UIName}");
                return;
            }

            // 취소 토큰 처리.
            _cancel_token.ThrowIfCancellationRequested();

            

            // UI 활성화.
            //ui_object.Open(_param);
            
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"UIManager: OpenUIAsync canceled. {_param.UIName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"UIManager: OpenUIAsync failed. {_param.UIName}, {e.Message}, {e.StackTrace}");
        }

        
        
    }


    public void CloseUI(string name)
    {
        var temp = HashSetPool<int>.Acquire();

        // 이름에 해당하는 UI 객체를 모두 닫는다.
        if (m_active_ui_by_name.TryGetValue(name, out temp))
        {
            foreach (var e in temp)
                CloseUI(e);
        }

        HashSetPool<int>.Return(temp);
    }

    public void CloseUI(int serial_number)
    {
        if (m_active_ui.TryGetValue(serial_number, out var ui_object))
        {
            // 컨테이너 제거 1.
            m_active_ui.Remove(serial_number);

            // 컨테이너 제거 2.
            if (m_active_ui_by_name.TryGetValue(ui_object.UIName, out var temp))
                temp.Remove(serial_number);

            // UI 닫기 처리.
            ui_object.Close();
        }

        // 취소 토큰에 대해서도 처리.
        m_async_operation_tracker.TryCancelOperation(serial_number);
    }


    
    
}
