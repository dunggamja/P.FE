using System;
using UnityEngine;


namespace Battle
{
    public class Battle_Camera_PositionEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;
        public (int x, int y) Cell { get; private set; } = (0, 0);

        public Battle_Camera_PositionEvent SetCell((int x, int y) _cell)
        {
            Cell = _cell;
            return this;
        }

        public void Release()
        {
            ObjectPool<Battle_Camera_PositionEvent>.Return(this);
        }

        public void Reset()
        {
            Cell = (0, 0);
        }
    }

    public class Battle_Cursor_PositionEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;
        public (int x, int y)         Cell { get; private set; } = (0, 0);


        public void Release()
        {
            ObjectPool<Battle_Cursor_PositionEvent>.Return(this);
        }

        public Battle_Cursor_PositionEvent Set((int x, int y) _cell_position)
        {
            Cell = _cell_position;
            return this;
        }

        public void Reset()
        {
            Cell = (0, 0);
        }
    }


    // public class Battle_Entity_MoveEvent : IEventParam
    // {
    //     public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

    //     public Int64          EntityID { get; private set; } = 0;
    //     public (int x, int y) MoveCell { get; private set; } = (0, 0);

    //     public void Release()
    //     {
    //         ObjectPool<Battle_Entity_MoveEvent>.Return(this);
    //     }

    //     public void Reset()
    //     {
    //         EntityID = 0;
    //         MoveCell = (0, 0);
    //     }

    //     public Battle_Entity_MoveEvent Set(Int64 _entity_id, (int x, int y) _move_cell)
    //     {
    //         EntityID = _entity_id;
    //         MoveCell = _move_cell;
    //         return this;
    //     }        
    // }
}

