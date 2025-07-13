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
    
    // Dictionary<EnumInputHandlerType, InputHandler> m_list_input_handler         = new();
    // Stack<InputHandler>                            m_stack_input_handler    = new();
    [SerializeField]
    PlayerInput         m_player_input          = null;

    // InputHandlerContext m_input_handler_context = new();

    InputHandler        m_input_handler         = null;     

    public InputHandler FocusInputHandler
    {
        get
        {
            var    input_handler  = m_input_handler;
            while (input_handler != null)
            {   
                if (input_handler.ChildHandler == null)
                    break;

                input_handler = input_handler.ChildHandler;
            }

            return input_handler;
        }
    }

    EnumInputHandlerType FocusInputHandlerType
    {
        get
        {
            
            var    input_handler  = FocusInputHandler;
            return input_handler?.HandlerType ?? EnumInputHandlerType.None;
        }
    }

    InputHandlerContext GetFocusInputHandlerContext()
    {
        var input_handler = FocusInputHandler;
        return input_handler?.Context ?? null;
    }


    string GetInputMapName(EnumInputHandlerType _handler_type)
    {
        switch (_handler_type)
        {
            case EnumInputHandlerType.UI_Menu: 
            case EnumInputHandlerType.UI_Command: 
                return "Menu";
            case EnumInputHandlerType.Grid_Select: 
                return "Grid";
        }

        return string.Empty;
    }

    

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // m_list_input_handler.Add(EnumInputHandlerType.UI,          new InputHandler_UI());
        // m_list_input_handler.Add(EnumInputHandlerType.Grid_Select, new InputHandler_Grid_Select());

        // m_stack_input_handler.Push(new InputHandler_Grid_Select());

        // �ʱ� �Է� �ڵ鷯 ����.
        m_input_handler = new InputHandler_Grid_Select(new InputHandlerContext());

        // �ʱ� �Է� �� ����.
        if (m_player_input != null)
        {
            m_player_input.SwitchCurrentActionMap(m_player_input.defaultActionMap);
        }
    }

    protected override void OnLoop()
    {
        base.OnLoop(); 

        // UI �Է��� �ʿ��� ��Ȳ�� ��� Handler ����.
        if (GUIManager.Instance.HasInputFocusGUI() 
        &&  FocusInputHandlerType != EnumInputHandlerType.UI_Menu)
        {
            // TODO: Child ���ٴ� FSM����?
            var ui_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());
            FocusInputHandler?.SetChildHandler(ui_handler);
        }


        // InputAction�� Map ���� ó��...
        if (m_player_input != null)
        {
            var prev_input_map_name  = m_player_input.currentActionMap?.name ?? string.Empty;
            var cur_input_map_name   = GetInputMapName(FocusInputHandlerType);
            if (prev_input_map_name != cur_input_map_name)
                m_player_input.SwitchCurrentActionMap(cur_input_map_name);
        }

        // InputHandler ������Ʈ ó��...
        if (m_input_handler != null)
            m_input_handler.Update();
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);
    }


    

}
