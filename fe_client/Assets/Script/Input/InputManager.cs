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

    [InputBinding("Move")]
    void OnMove(InputAction.CallbackContext _context)
    {
        Debug.Log($"{_context.action.actionMap.name}:OnMove");
    }

    
}
