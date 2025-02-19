using System;
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

public class InputManager : SingletonMono<InputManager>
{
    
    

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnLoop()
    {
        base.OnLoop();
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);
    }

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
