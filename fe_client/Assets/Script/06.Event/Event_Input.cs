using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid_Cursor_Event : IEventParam
{
    public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

    public (int x, int y) Cell { get; private set; } = (0, 0);

    public Grid_Cursor_Event Set((int x, int y) _cell)
    {
        Cell = _cell;
        return this;
    }

    public void Reset()
    {
        Cell = (0, 0);
    }

    public void Release()
    {
        ObjectPool<Grid_Cursor_Event>.Return(this);
    }

}