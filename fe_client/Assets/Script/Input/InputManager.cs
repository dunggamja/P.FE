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
    // Stack<InputHandler>                            m_stack_input_handler    = new();
    InputHandlerContext m_input_handler_context = new();

    InputHandler        m_input_handler = null;     

    

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // m_list_input_handler.Add(EnumInputHandlerType.UI,          new InputHandler_UI());
        // m_list_input_handler.Add(EnumInputHandlerType.Grid_Select, new InputHandler_Grid_Select());

        // m_stack_input_handler.Push(new InputHandler_Grid_Select());

        // �ʱ� �Է� �ڵ鷯 ����.
        m_input_handler = new InputHandler_Grid_Select(m_input_handler_context);
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        if (m_input_handler != null)
            m_input_handler.Update();

        // ���� �Է� �ڵ鷯�� ���ų�, ���� �Է� �ڵ鷯�� Ÿ���� ������ �ֻ�� Ÿ�԰� �ٸ��� �Է� �ڵ鷯 ����...
        // var peek_handler  = m_stack_input_handler.Peek();
        // var change_hander = (m_current_input_handler == null ||  m_current_input_handler != peek_handler);
        // // if (change_hander)
        // {
        //     // �Է� �Ķ���� ť �ʱ�ȭ...
        //     // m_queue_input_param.Clear();

        //     // // ���� �Է� �ڵ鷯�� ������ ��� ó��...
        //     // if (m_current_input_handler != null)
        //     //     m_current_input_handler.Pause();                        

        //     // ���ο� �Է� �ڵ鷯 ����...
        //     // m_current_input_handler = peek_handler;

        //     // // ���ο� �Է� �ڵ鷯�� ������ �簳 ó��...
        //     // if (m_current_input_handler != null)
        //     //     m_current_input_handler.Resume();
        // }

        // if (m_input_handler != null)
        // {
        //     // �ڵ鷯 ������Ʈ
        //     m_input_handler.Update();
            
        //     // // �ڵ鷯 ���� üũ.
        //     // if (m_current_input_handler.State == InputHandler.EnumState.Finish)
        //     // {
        //     //     // ���ÿ��� �ڵ鷯 ����
        //     //     m_stack_input_handler.Pop();
        //     // }
        // }
        
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);
    }


    // public void AddInputParam(InputParam _input_param)
    // {
    //     m_queue_input_param.Enqueue(_input_param);
    // }

    // public void StackHandler(InputHandler _input_handler)
    // {
    //     m_stack_input_handler.Push(_input_handler);
    // }


    // void PopHandler()
    // {
    //     if (m_stack_input_handler.Count > 1)
    //         m_stack_input_handler.Pop();        
    // }

}
