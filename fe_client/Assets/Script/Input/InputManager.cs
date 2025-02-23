using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[AttributeUsage(AttributeTargets.Method)]
public class InputBindingAttribute : PropertyAttribute
{
    public string ActionName { get; private set; } = string.Empty;

    public InputBindingAttribute(string _action_name)
    {
        ActionName = _action_name;
    }
}

public partial class InputManager : SingletonMono<InputManager>
{
    [SerializeField]
    PlayerInput                                    m_player_input               = null;

    
    Dictionary<EnumInputHandlerType, InputHandler> m_list_input_handler         = new();
    Stack<EnumInputHandlerType>                    m_stack_input_handler_type   = new();
    Queue<InputParam>                              m_queue_input_param          = new();

    InputHandler                                   m_current_input_handler      = null;     

    

    protected override void OnInitialize()
    {
        base.OnInitialize();

        m_list_input_handler.Add(EnumInputHandlerType.UI,   new InputHandler_UI());
        m_list_input_handler.Add(EnumInputHandlerType.Grid, new InputHandler_Grid());

        m_stack_input_handler_type.Push(EnumInputHandlerType.Grid);
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // ���� �Է� �ڵ鷯�� ���ų�, ���� �Է� �ڵ鷯�� Ÿ���� ������ �ֻ�� Ÿ�԰� �ٸ��� �Է� �ڵ鷯 ����...
        var peek_handler_type = m_stack_input_handler_type.Peek();
        var change_hander     = (m_current_input_handler == null ||  m_current_input_handler.HandlerType != peek_handler_type);
        if (change_hander)
        {
            // ���� �Է� �ڵ鷯�� ������ ��� ó��...
            if (m_current_input_handler != null)
                m_current_input_handler.Pause();            
            
            // // �Է� �Ķ���� ť �ʱ�ȭ...
            // foreach (var e in m_queue_input_param)
            //     e.Reset();
            
            m_queue_input_param.Clear();

            // ���ο� �Է� �ڵ鷯 ����...
            m_current_input_handler = m_list_input_handler[peek_handler_type];

            // ���ο� �Է� �ڵ鷯�� ������ �簳 ó��...
            if (m_current_input_handler != null)
                m_current_input_handler.Resume();
        }

        if (m_current_input_handler != null)
        {
            var input_param = (m_queue_input_param.Count > 0) ? m_queue_input_param.Dequeue() : null;  

            // �ڵ鷯 ������Ʈ
            if (m_current_input_handler.Update(input_param))
            {
                // ���ÿ��� �ڵ鷯 ����
                m_stack_input_handler_type.Pop();
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

    public void StackHandler(EnumInputHandlerType _input_handler_type)
    {
        if (m_stack_input_handler_type.TryPeek(out EnumInputHandlerType _peek))
        {
            // �̹� �ش� �Է� �ڵ鷯�� �ֻ�ܿ� �� ������ ����...
            if (_peek == _input_handler_type)
                return;
        }

        m_stack_input_handler_type.Push(_input_handler_type);
    }

    // public void PopHandler()
    // {
    //     m_stack_input_handler_type.Pop();
    // }

}
