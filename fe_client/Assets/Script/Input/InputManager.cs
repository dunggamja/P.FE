using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// [AttributeUsage(AttributeTargets.Method)]
// public class InputBindingAttribute : PropertyAttribute
// {
//     public string ActionName { get; private set; } = string.Empty;

//     public InputBindingAttribute(string _action_name)
//     {
//         ActionName = _action_name;
//     }
// }

public partial class InputManager : SingletonMono<InputManager>
{
    [SerializeField]
    PlayerInput                                    m_player_input               = null;

    
    // Dictionary<EnumInputHandlerType, InputHandler> m_list_input_handler         = new();
    Stack<InputHandler>                            m_stack_input_handler    = new();
    Queue<InputParam>                              m_queue_input_param      = new();

    InputHandler                                   m_current_input_handler  = null;     

    

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // m_list_input_handler.Add(EnumInputHandlerType.UI,          new InputHandler_UI());
        // m_list_input_handler.Add(EnumInputHandlerType.Grid_Select, new InputHandler_Grid_Select());

        m_stack_input_handler.Push(new InputHandler_Grid_Select());
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // 현재 입력 핸들러가 없거나, 현재 입력 핸들러의 타입이 스택의 최상단 타입과 다르면 입력 핸들러 변경...
        var peek_handler  = m_stack_input_handler.Peek();
        var change_hander = (m_current_input_handler == null ||  m_current_input_handler != peek_handler);
        if (change_hander)
        {
            // 이전 입력 핸들러가 있으면 취소 처리...
            if (m_current_input_handler != null)
                m_current_input_handler.Pause();            
            
            // // 입력 파라미터 큐 초기화...
            // foreach (var e in m_queue_input_param)
            //     e.Reset();
            
            m_queue_input_param.Clear();

            // 새로운 입력 핸들러 설정...
            m_current_input_handler = peek_handler;

            // 새로운 입력 핸들러가 있으면 재개 처리...
            if (m_current_input_handler != null)
                m_current_input_handler.Resume();
        }

        if (m_current_input_handler != null)
        {
            // 핸들러 업데이트
            if (m_current_input_handler.Update(m_queue_input_param))
            {
                // 스택에서 핸들러 제거
                m_stack_input_handler.Pop();
            }
        }
        
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);
    }


    // public void AddInputParam(InputParam _input_param)
    // {
    //     m_queue_input_param.Enqueue(_input_param);
    // }

    public void StackHandler(InputHandler _input_handler)
    {
        m_stack_input_handler.Push(_input_handler);
    }

    void PopHandler()
    {
        if (m_stack_input_handler.Count > 1)
            m_stack_input_handler.Pop();        
    }

}
