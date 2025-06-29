using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
    // [InputBinding("UI_Move")]
    public void OnMenu_Move(InputAction.CallbackContext _context)
    {
        // Debug.Log($"OnMenu_Move: {_context.ReadValue<Vector2>()}");
        var input_handler_context = GetFocusInputHandlerContext();
        if (input_handler_context == null)
        {
            Debug.LogError("InputHandlerContext is null");
            return;
        }

        if (_context.started || _context.performed)
        {
            var direction = _context.ReadValue<Vector2>();
            var input_param = new InputParam_UI_Move(direction);
            input_handler_context.InputParamQueue.Enqueue(input_param);

            // Debug.Log($"OnUI_Move performed, direction: {direction}, {_context.phase}");
        }
    }

    // [InputBinding("UI_Pointer")]
    // public void OnMenu_Pointer(InputAction.CallbackContext _context)
    // {
    //     var input_handler_context = GetCurrentInputHandlerContext();
    //     if (input_handler_context == null)
    //     {
    //         Debug.LogError("InputHandlerContext is null");
    //         return;
    //     }

    //     if (_context.started || _context.performed)
    //     {
    //         var position = _context.ReadValue<Vector2>();
    //         var input_param = new InputParam_UI_Pointer(position);
    //         input_handler_context.InputParamQueue.Enqueue(input_param);

    //         Debug.Log($"OnUI_Pointer performed, position: {position}, {_context.phase}");
    //     }
    // }

    // [InputBinding("UI_Delta")]
    // public void OnUI_Delta(InputAction.CallbackContext _context)
    // {
    //     var input_handler_context = GetCurrentInputHandlerContext();
    //     if (input_handler_context == null)
    //     {
    //         Debug.LogError("InputHandlerContext is null");
    //         return;
    //     }

    //     if (_context.started || _context.performed)
    //     {
    //         var input_param = new InputParam_UI_Delta();
    //         input_handler_context.InputParamQueue.Enqueue(input_param);

    //         Debug.Log($"OnUI_Delta performed, {_context.phase}");
    //     }
    // }

    // [InputBinding("UI_Select")]
    public void OnMenu_Select(InputAction.CallbackContext _context)
    {
        var input_handler_context = GetFocusInputHandlerContext();
        if (input_handler_context == null)
        {
            Debug.LogError("InputHandlerContext is null");
            return;
        }

        if (_context.started || _context.performed)
        {
            var input_param = new InputParam_UI_Select();
            input_handler_context.InputParamQueue.Enqueue(input_param);

            // Debug.Log($"OnUI_Select performed, {_context.phase}");
        }
    }

    // [InputBinding("UI_Cancel")]
    public void OnMenu_Cancel(InputAction.CallbackContext _context)
    {
        var input_handler_context = GetFocusInputHandlerContext();
        if (input_handler_context == null)
        {
            Debug.LogError("InputHandlerContext is null");
            return;
        }

        if (_context.started || _context.performed)
        {
            var input_param = new InputParam_UI_Cancel();
            input_handler_context.InputParamQueue.Enqueue(input_param);

            // Debug.Log($"OnUI_Cancel performed, {_context.phase}");
        }
    }
}