using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
    
    [InputBinding("Grid_Move")]
    public void OnGrid_Move(InputAction.CallbackContext _context)
    {

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
