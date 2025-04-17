using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class GUIManager : SingletonMono<GUIManager>
{
    // canvas�� sort order ������ ���� �������� �� �ʿ�� ���°�?...

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;



    Dictionary<Int64,    GUIPage>      m_active_gui              = new (); // serial number, GUIObject
    Dictionary<string, HashSet<Int64>> m_active_gui_by_name      = new (); // �̸�, serial number
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
            // ���� ī�޶� �����ϵ��� ����.
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }


    protected override void OnLoop()
    {
        base.OnLoop();
    }

    public Int64 OpenUI(GUIOpenParam _param)
    {       
        var cts           = new CancellationTokenSource();

        // ��� ��ū ����.  
        m_async_operation_tracker.TrackOperation(_param.ID, cts);

        // UI ����.
        OpenUIAsync(_param, cts.Token).Forget();

        return _param.ID;
    }


    async UniTask OpenUIAsync(GUIOpenParam _param, CancellationToken _cancel_token = default)
    {
        try
        {
            // Ȱ��ȭ�� UI ��Ͽ� �߰�.
            {
                m_active_gui.Add(_param.ID, null);

                // �̸��� �ش��ϴ� UI ��Ͽ� �߰�.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_sn) == false)
                {
                    list_sn = new HashSet<Int64>(); 
                    m_active_gui_by_name.Add(_param.GUIName, list_sn);
                }
                list_sn.Add(_param.ID);
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

            // ��� ��ū ó��.
            
            var gui_object = await AssetManager.Instance.InstantiateAsync(_param.GUIName, _ui_root, _cancel_token);
            if (gui_object == null)
            {
                Debug.LogError($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");

                throw new Exception($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");
            }

            // GUIPage ������Ʈ üũ.
            if (gui_object.TryGetComponent<GUIPage>(out var gui_page) == false)
            {
                Debug.LogError($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
                GameObject.Destroy(gui_object);

                throw new Exception($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
            }

            // ��� �� gui_object ����.
            if (_cancel_token.IsCancellationRequested)
            {
                Debug.Log($"GUIManager: OpenUIAsync canceled. {_param.GUIName}");
                GameObject.Destroy(gui_object);

                _cancel_token.ThrowIfCancellationRequested();
            }

           

            // UI ����.
            gui_page.Open(_param);            
        }
        catch (Exception e)
        {
            Debug.LogError($"GUIManager: OpenUIAsync canceled. {_param.GUIName}, e:{e.Message}");

            {
                // ��� �� ��� ����.
                m_active_gui.Remove(_param.ID);

                // �̸��� �ش��ϴ� UI ��Ͽ� �߰�.
                if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_sn))
                    list_sn.Remove(_param.ID);
            }
        }
        
    }


    public void CloseUI(string name)
    {
        var temp = HashSetPool<Int64>.Acquire();

        // �̸��� �ش��ϴ� UI ��ü�� ��� �ݴ´�.
        if (m_active_gui_by_name.TryGetValue(name, out temp))
        {
            foreach (var e in temp)
                CloseUI(e);
        }   

        HashSetPool<Int64>.Return(temp);
    }

    public void CloseUI(Int64 id)
    {
        if (m_active_gui.TryGetValue(id, out var gui_object))
        {
            if (gui_object != null)
            {
                // �����̳� ���� 1.
                m_active_gui.Remove(id);

                // �����̳� ���� 2.
                if (m_active_gui_by_name.TryGetValue(gui_object.GUIName, out var temp))
                    temp.Remove(id);

                // UI �ݱ� ó��.
                gui_object.Close();
            }
        }

        // ��� ��ū�� ���ؼ��� ó��.
        m_async_operation_tracker.TryCancelOperation(id);
    }
}
