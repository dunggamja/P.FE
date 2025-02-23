using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
    
    [InputBinding("Grid_Move")]
    public void OnGrid_Move(InputAction.CallbackContext _context)
    {
        switch(_context)
        {
            case InputAction.CallbackContext _ when _context.started:
                Debug.LogWarning("started");
                break;
            case InputAction.CallbackContext _ when _context.performed:
                Debug.LogWarning("performed");
                break;
            case InputAction.CallbackContext _ when _context.canceled:
                Debug.LogWarning("canceled");
                break;
        }

        {
            var direction   = _context.ReadValue<Vector2>();
            var input_param = new InputParam_Grid_Move(direction);
            
            m_queue_input_param.Enqueue(input_param);
        }
        
        
        Debug.Log($"{_context.action.actionMap.name}:Grid_Move");
    }


    [InputBinding("Grid_Pointer")]
    public void OnGrid_Pointer(InputAction.CallbackContext _context)
    {
        Debug.Log($"{_context.action.actionMap.name}:Grid_Pointer");
    }

    [InputBinding("Grid_Delta")]
    public void OnGrid_Delta(InputAction.CallbackContext _context)
    {
        Debug.Log($"{_context.action.actionMap.name}:Grid_Delta");
    }
    
    [InputBinding("Grid_Click")]
    public void OnGrid_Click(InputAction.CallbackContext _context)
    {
        Debug.Log($"{_context.action.actionMap.name}:Grid_Click");
    }

    [InputBinding("Grid_Cancel")]
    public void OnGrid_Cancel(InputAction.CallbackContext _context)
    {
        Debug.Log($"{_context.action.actionMap.name}:Grid_Cancel");
    }
    
}
