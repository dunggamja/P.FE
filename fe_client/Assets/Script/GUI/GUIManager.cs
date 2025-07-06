using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class GUIManager : SingletonMono<GUIManager>
{
    // 

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;



    Dictionary<Int64,  GUIPage>        m_active_gui              = new (); // serial number, GUIObject
    Dictionary<string, HashSet<Int64>> m_active_gui_by_name      = new (); // �̸�, serial number
    List<Int64>                        m_focus_gui_stack         = new();



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
            // HUD ĵ������ ���� ī�޶� �����ϵ��� ����. (�ӽ�?)
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

        // ��� ��ū ����.  
        m_async_operation_tracker.TrackOperation(_param.ID, cts);

        
        {
            // UI ���� �� ��Ŀ�� �� GUI ID
            var prev_focus_gui = GetInputFocusGUI();

            // Ȱ��ȭ�� UI ��Ͽ� �߰�.
            m_active_gui.Add(_param.ID, null);

            // �̸��� �ش��ϴ� UI ��Ͽ� �߰�.
            if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_gui_id) == false)
            {
                list_gui_id = new HashSet<Int64>(); 
                m_active_gui_by_name.Add(_param.GUIName, list_gui_id);
            }
            list_gui_id.Add(_param.ID);

            // �Է� ������ UI ���ÿ� �߰�.
            if (_param.IsInputEnabled)
            {
                m_focus_gui_stack.Add(_param.ID);
            }

            // UI ���� �� ��Ŀ�̵Ǿ��� GUI ó��.
            Process_ChangeFocusGUI(prev_focus_gui);
        }

        // UI ����.
        OpenUIAsync(_param, cts.Token).Forget();

        return _param.ID;
    }


    async UniTask OpenUIAsync(GUIOpenParam _param, CancellationToken _cancel_token = default)
    {
        try
        {
            
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


            // Ȱ��ȭ�� UI ��Ͽ� �߰�.
            m_active_gui[_param.ID] = gui_page;

            // UI ����.
            gui_page.Open(_param);            

            
        }
        catch (Exception e)
        {
            Debug.LogError($"GUIManager: OpenUIAsync canceled. {_param.GUIName}, e:{e.Message}");

            CloseUI(_param.ID);
        }
        
    }


    public void CloseUI(string name)
    {
        // �̸��� �ش��ϴ� UI ��ü�� ��� �ݴ´�.
        if (m_active_gui_by_name.TryGetValue(name, out var list_gui_id))
        {
            foreach (var e in list_gui_id)
                CloseUI(e);
        }   
    }

    public void CloseUI(Int64 _id)
    {
        // �����̳� ���� ó��.
        if (m_active_gui.TryGetValue(_id, out var gui_object))
        {
            if (gui_object != null)
            {
                // �����̳� ���� 1.
                m_active_gui.Remove(_id);

                // �����̳� ���� 2.
                if (m_active_gui_by_name.TryGetValue(gui_object.GUIName, out var list_gui_id))
                    list_gui_id.Remove(_id);

                // �Է� ������ UI ���ÿ��� ����.
                if (m_focus_gui_stack.Contains(_id))
                    m_focus_gui_stack.Remove(_id);

                // UI �ݱ� ó��.
                gui_object.Close();
            }

        }

        // ��� ��ū�� ���ؼ��� ó��.
        m_async_operation_tracker.TryCancelOperation(_id);

        // 
        Process_ChangeFocusGUI(GetInputFocusGUI());
    }

    // �������丮 ���� �� ó��.
    void Process_ChangeFocusGUI(Int64 _gui_id)
    {
        if (_gui_id == 0)
            return;

        var focus_gui = GetGUIPage(_gui_id);
        if (focus_gui != null)
        {
            var is_focus = (GetInputFocusGUI() == _gui_id);
            if (is_focus)
            {
                // ��Ŀ�� ó��.
                focus_gui.SetInputFocus(true);
            }
            else
            {
                // ��Ŀ�� ����.
                focus_gui.SetInputFocus(false);
            }
        }
    }

    public GUIPage GetGUIPage(Int64 _id)
    {
        if (m_active_gui.TryGetValue(_id, out var gui_object))
            return gui_object;
        
        return null;
    }


    bool IsOpenUI(string _name)
    {
        if (m_active_gui_by_name.TryGetValue(_name, out var list_gui_id))
            return list_gui_id.Count > 0;

        return false;
    }

    public Int64 GetInputFocusGUI()
    {
        if (m_focus_gui_stack.Count == 0)
            return 0;
        
        return m_focus_gui_stack[m_focus_gui_stack.Count - 1];
    }

    public bool  HasInputFocusGUI()
    {
        return GetInputFocusGUI() > 0;
    }






    // bool InsertInputGUI(Int64 _id)
    // {
    //     if (m_input_gui_stack.Contains(_id))
    //         return false;
        
    //     m_input_gui_stack.Add(_id);
    //     return true;
    // }

    // bool RemoveInputGUI(Int64 _id)
    // {
    //     if (m_input_gui_stack.Contains(_id))
    //         return false;
        
    //     m_input_gui_stack.Remove(_id);
    //     return true;
    // }






}
