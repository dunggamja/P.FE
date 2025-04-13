using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
    
    // [InputBinding("Grid_Move")]
    public void OnGrid_Move(InputAction.CallbackContext _context)
    {
        if (_context.started || _context.performed)
        {
            var direction   = _context.ReadValue<Vector2>();
            var input_param = new InputParam_Grid_Move(direction);
            
            m_input_handler_context.InputParamQueue.Enqueue(input_param);
            // Debug.Log($"{_context.action.actionMap.name}:Grid_Move");

            // Debug.Log($"OnGrid_Move performed: {_context.ReadValue<Vector2>()}");
        }

        // Debug.Log($"OnGrid_Move anything: {_context.ReadValue<Vector2>()}");
        
        
    }


    // [InputBinding("Grid_Pointer")]
    public void OnGrid_Pointer(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_Pointer");
    }

    // [InputBinding("Grid_Delta")]
    public void OnGrid_Delta(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_Delta");
    }
    
    // [InputBinding("Grid_Click")]
    public void OnGrid_Click(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_Click");
    }

    public void OnGrid_Select(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_Select");
    }

    // [InputBinding("Grid_Cancel")]
    public void OnGrid_Cancel(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_Cancel");
    }

    public void OnGrid_AnyButton(InputAction.CallbackContext _context)
    {
        // Debug.Log($"{_context.action.actionMap.name}:Grid_AnyButton");
    }
    
}
