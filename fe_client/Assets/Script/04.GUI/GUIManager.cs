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
    public enum EnumGUIOpenState
    {
        None,
        Instancing, // 생성중.
        Open,
    }

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;

    [SerializeField]
    private Transform m_gui_pool_root = null;



    private Dictionary<string, Queue<GUIPage>> m_gui_pools        = new();



    Dictionary<Int64,  GUIPage>        m_active_gui              = new (); // serial number, GUIObject
    Dictionary<string, HashSet<Int64>> m_active_gui_by_name      = new (); // 이름, serial number
    List<Int64>                        m_focus_gui_stack         = new();



    AsyncOperationTracker              m_async_operation_tracker = new();


    public Camera HUDCamera 
        => (m_canvas_root_hud) ? m_canvas_root_hud.worldCamera : null;



       
    private void Awake()
    {
        var main_camera = Camera.main;
        if (main_camera == null)
        {
            Debug.LogError("UIManager: Main Camera is not found.");
        }
        else
        {
            // HUD 카메라 설정. (추후 필요시 추가)
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }


    protected override void OnLoop()
    {
        base.OnLoop();
    }

    public Int64 OpenUI(GUIOpenParam _param)
    {   
        if (_param == null)
        {
            Debug.LogError("GUIManager: OpenUI param is null.");
            return 0;
        }



        if (_param.IsMultipleOpen == false && EnumGUIOpenState.None != IsOpenUI(_param.GUIName))
        {
            Debug.Log($"GUIManager: OpenUI failed. {_param.GUIName} is already open.");
            return 0;
        }

        var cts           = new CancellationTokenSource();

        // 비동기 처리 추적.  
        m_async_operation_tracker.TrackOperation(_param.ID, cts);

        
        {
            // UI 이전 포커스 GUI 처리.
            var prev_focus_gui = GetInputFocusGUI();

            // 활성 UI 목록에 추가.
            m_active_gui.Add(_param.ID, null);

            // UI 이름별 목록에 추가.
            if (m_active_gui_by_name.TryGetValue(_param.GUIName, out var list_gui_id) == false)
            {
                list_gui_id = new HashSet<Int64>(); 
                m_active_gui_by_name.Add(_param.GUIName, list_gui_id);
            }
            list_gui_id.Add(_param.ID);

            // 입력 가능한 UI 추가.
            if (_param.IsInputEnabled)
            {
                m_focus_gui_stack.Add(_param.ID);

                Process_ChangeFocusGUI(_param.ID, _param.GUIType);
            }
            
        }

        // UI 오픈.
        OpenUIAsync(_param, cts.Token).Forget();

        return _param.ID;
    }


    async UniTask OpenUIAsync(GUIOpenParam _param, CancellationToken _cancel_token = default)
    {
        // try
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

            // 풀링 오브젝트 찾기.
            GameObject gui_object = null;
            GUIPage    gui_page   = AcquireFromPool(_param.GUIName);


            if (gui_page == null)
            {
                gui_object = await AssetManager.Instance.InstantiateAsync(_param.GUIName, _ui_root, _cancel_token);

                if (gui_object == null)
                {
                    Debug.LogError($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");
                    //throw new Exception($"UIManager: OpenUIAsync failed to instantiate {_param.GUIName}");
                }

                // GUIPage 컴포넌트 찾기.
                if (gui_object != null && gui_object.TryGetComponent<GUIPage>(out gui_page) == false)
                {
                    Debug.LogError($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
                    GameObject.Destroy(gui_object);
                    gui_object = null;

                    // throw new Exception($"UIManager: OpenUIAsync failed to get GUIPage component. {_param.GUIName}");
                }            
            }


            // 취소 요청이 있을 경우 종료 처리.
            if (_cancel_token.IsCancellationRequested)
            {
                Debug.Log($"GUIManager: OpenUIAsync canceled. {_param.GUIName}");
                
                if (gui_object != null)
                {
                    GameObject.Destroy(gui_object);
                    gui_object = null;
                }

                CloseUI(_param.ID);
                return;

                // 예외 발생 처리.
                // _cancel_token.ThrowIfCancellationRequested();
            }
            


            // 활성 UI 목록에 추가.
            m_active_gui[_param.ID] = gui_page;

            // UI 오픈.
            gui_page.Open(_param);            

            
        }
        // catch (Exception e)
        // {
        //     Debug.LogError($"GUIManager: OpenUIAsync canceled. {_param.GUIName}, e:{e.Message}");

        //     CloseUI(_param.ID);
        // }
        
    }


    public void CloseUI(string name)
    {
        // UI 이름별 목록에서 찾기.
        var list_gui_id = HashSetPool<Int64>.Acquire();

        if (m_active_gui_by_name.TryGetValue(name, out list_gui_id))
        {
            foreach (var e in list_gui_id)
                CloseUI(e);
        }   

        HashSetPool<Int64>.Return(list_gui_id);
    }

    public void CloseUI(Int64 _id)
    {
        // UI 목록에서 찾기.
        if (m_active_gui.TryGetValue(_id, out var gui_object))
        {
            if (gui_object != null)
            {
                // UI 목록에서 제거.
                m_active_gui.Remove(_id);

                // UI 이름별 목록에서 제거.
                if (m_active_gui_by_name.TryGetValue(gui_object.GUIName, out var list_gui_id))
                    list_gui_id.Remove(_id);

                // UI 종료.
                gui_object.Close();
            }
        }

        // 비동기 처리가 진행중인게 있을 경우 취소 처리.
        m_async_operation_tracker.TryCancelOperation(_id);

        // 포커싱 UI 스택에서 제거.
        if (m_focus_gui_stack.Contains(_id))
        {
            m_focus_gui_stack.Remove(_id);


            var focus_gui_id = GetInputFocusGUI();
            if (focus_gui_id > 0)
            {
                var focus_gui_object = GetGUI(focus_gui_id);
                var focus_gui_type   = (focus_gui_object != null) ? focus_gui_object.GUIType : EnumGUIType.None;

                Process_ChangeFocusGUI(focus_gui_id, focus_gui_type);
            }
        }
    }

    // 포커싱 UI 변경 처리.
    void Process_ChangeFocusGUI(Int64 _focus_gui_id, EnumGUIType _focus_gui_type)
    {
        foreach (var e in m_focus_gui_stack)
        {
            var gui_object = GetGUI(e);
            if (gui_object == null)
                continue;
            
            var is_focus = (_focus_gui_id == e);
            if (is_focus)
            {
                // 포커싱 UI
                gui_object.SetInputFocus(true, _focus_gui_type);
            }
            else
            {
                // 포커싱 되지 않은 UI
                gui_object.SetInputFocus(false, _focus_gui_type);
            }
        }
        
    }

    public GUIPage GetGUI(Int64 _id)
    {
        if (m_active_gui.TryGetValue(_id, out var gui_object))
            return gui_object;
        
        return null;
    }

    public Int64 GetGUIID(string _name)
    {
        if (m_active_gui_by_name.TryGetValue(_name, out var list_gui_id))
        {
            foreach (var e in list_gui_id)
                return e;
        }
        return 0;
    }


    public EnumGUIOpenState IsOpenUI(string _name)
    {
        var open_state = EnumGUIOpenState.None;

        if (m_active_gui_by_name.TryGetValue(_name, out var list_gui_id))
        {
            foreach (var e in list_gui_id)
            {
                var state = IsOpenUI(e);
                if (open_state < state)
                    open_state = state;
            }
        }

        return open_state;
    }

    EnumGUIOpenState IsOpenUI(Int64 _id)
    {
        // UI 객체가 null이면 Opening 상태.
        if (m_active_gui.TryGetValue(_id, out var gui_object))
            return (gui_object != null) ? EnumGUIOpenState.Open : EnumGUIOpenState.Instancing;

        // 레포지토리에 없으면 None 상태.
        return EnumGUIOpenState.None;
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

    GUIPage AcquireFromPool(string _gui_name)
    {
        if (m_gui_pools.TryGetValue(_gui_name, out var pool) && pool.Count > 0)
        {
            return pool.Dequeue();
        }

        return null;
    }



    void ReturnToPool(GUIPage _gui_object)
    {
        if (_gui_object == null)
            return;

        if (!m_gui_pools.TryGetValue(_gui_object.GUIName, out var pool))
        {
            pool = new Queue<GUIPage>();
            m_gui_pools.Add(_gui_object.GUIName, pool);
        }

        pool.Enqueue(_gui_object);

        // 풀링시 초기화 처리.
        _gui_object.gameObject.SetActive(false);
        _gui_object.transform.SetParent(m_gui_pool_root);
        _gui_object.transform.localPosition = Vector3.zero;
        _gui_object.transform.localRotation = Quaternion.identity;
        _gui_object.transform.localScale    = Vector3.one;        
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
