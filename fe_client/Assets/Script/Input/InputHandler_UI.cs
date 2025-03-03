using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class InputHandler_UI_Menu : InputHandler
{
    public InputHandler_UI_Menu(InputHandlerContext _context) 
        : base(_context)
    {
    }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.UI_Menu;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate()
    {
        return false;
    }

    protected override void OnFinish()
    {
    }

}
