using System;
using System.Collections.Generic;
using Battle;
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

[EventReceiver(typeof(Battle_Scene_ChangeEvent))]
public partial class InputManager : SingletonMono<InputManager>, IEventReceiver
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

        // 입력 처리 시작.
        m_input_handler = new InputHandler_Grid_Select(new InputHandlerContext());

        // 입력 처리 종료.
        if (m_player_input != null)
        {
            m_player_input.SwitchCurrentActionMap(m_player_input.defaultActionMap);
        }
    }

    protected override void OnLoop()
    {
        base.OnLoop(); 

        // UI 입력 처리 중인 경우 입력 처리 종료.
        if (GUIManager.Instance.HasInputFocusGUI() 
        &&  FocusInputHandlerType != EnumInputHandlerType.UI_Menu)
        {
            // TODO: Child 입력 처리 중인 경우 입력 처리 종료?
            var ui_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());
            FocusInputHandler?.SetChildHandler(ui_handler);
        }


        // InputAction Map 전환.
        if (m_player_input != null)
        {
            var prev_input_map_name  = m_player_input.currentActionMap?.name ?? string.Empty;
            var cur_input_map_name   = GetInputMapName(FocusInputHandlerType);
            if (prev_input_map_name != cur_input_map_name)
                m_player_input.SwitchCurrentActionMap(cur_input_map_name);
        }

        // InputHandler 업데이트.
        if (m_input_handler != null)
            m_input_handler.Update();
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);
    }



    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case Battle_Scene_ChangeEvent battle_scene_change_event:
                OnReceiveEvent_Battle_Scene_ChangeEvent(battle_scene_change_event);
                break;
        }
    }

    void OnReceiveEvent_Battle_Scene_ChangeEvent(Battle_Scene_ChangeEvent _event)
    {
        
    }




}
