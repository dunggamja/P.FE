using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class UIManager : SingletonMono<UIManager>
{
    // canvas�� sort order ������ ���� �������� �� �ʿ�� ���°�?...

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;



    Dictionary<int,    UIPage>       m_active_ui         = new (); // serial number, UIObject
    Dictionary<string, HashSet<int>> m_active_ui_by_name = new (); // �̸�, serial number

    AsyncOperationTracker            m_async_operation_tracker = new();

    // TODO: Open/Close AsyncOperationTracker�� �����ص� �� �� ����.

    private int m_ui_serial = 0;

    private int GenerateSerial()
    {
        do
        { 
            // �ø��� �ѹ� ����.
            ++m_ui_serial;

            // �ø��� �ѹ��� 0�̸� 1�� ����.
            if (m_ui_serial <= 0)
                m_ui_serial = 1;
            
            // �ߺ� üũ.
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
            // ���� ī�޶� �����ϵ��� ����.
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }

    public int OpenUI(string name)
    {       
        var serial_number = GenerateSerial();
        var cts           = new CancellationTokenSource();

        // ��� ��ū ����.  
        m_async_operation_tracker.TrackOperation(serial_number, cts);

        // UI ����.
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

            // ��� ��ū ó��.
            _cancel_token.ThrowIfCancellationRequested();

            

            // UI Ȱ��ȭ.
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

        // �̸��� �ش��ϴ� UI ��ü�� ��� �ݴ´�.
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
            // �����̳� ���� 1.
            m_active_ui.Remove(serial_number);

            // �����̳� ���� 2.
            if (m_active_ui_by_name.TryGetValue(ui_object.UIName, out var temp))
                temp.Remove(serial_number);

            // UI �ݱ� ó��.
            ui_object.Close();
        }

        // ��� ��ū�� ���ؼ��� ó��.
        m_async_operation_tracker.TryCancelOperation(serial_number);
    }


    
    
}
