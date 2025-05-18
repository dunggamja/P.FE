using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_Menu_MoveEvent : IEventParam
{
    public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

    public Int64      GUI_ID        { get; private set; }
    public Vector2Int MoveDirection { get; private set; }



    public void Release()
    {
        ObjectPool<GUI_Menu_MoveEvent>.Return(this);
    }

    public GUI_Menu_MoveEvent Set(Int64 _gui_id, Vector2Int _move_direction)
    {
        GUI_ID        = _gui_id;
        MoveDirection = _move_direction;

        return this;
    }

    public void Reset()
    {
        GUI_ID        = 0;
        MoveDirection = Vector2Int.zero;
    }
}