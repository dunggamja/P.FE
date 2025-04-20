using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class InputHandler_UI_Menu : InputHandler
{
    public class HandlerContext : InputHandlerContext
    {
        private HandlerContext() { }

        public Int64 GUI_ID { get; private set; } = 0;

        public HandlerContext SetGUI(Int64 _gui_id)
        {
            GUI_ID = _gui_id;
            return this;
        }

        public static HandlerContext Create(Int64 _gui_id)
        {
            return new HandlerContext()
                .SetGUI(_gui_id);
        }
    }

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
