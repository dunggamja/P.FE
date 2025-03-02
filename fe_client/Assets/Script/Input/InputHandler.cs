using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid_Select,    // Ÿ�� ����
    // Grid_Command,   // ���ֿ��� ����� ������ (�̵�,����,��ų ��) 

    UI_Menu,        // UI �޴�
    UI_Command,     // ���� �ൿ �޴� (�̵�, ����, ��ų)
    
}

public class InputHandlerContext
{
    public Queue<InputParam> InputParamQueue { get; set; } = new Queue<InputParam>();

    public void ClearInputParams()
    {
        InputParamQueue.Clear();
    }
}


public abstract class InputHandler
{
    public enum EnumState
    {
        None,
        Start,   // ����
        Update,  // �ݺ�
        Finish,  // ����
    }

    public abstract EnumInputHandlerType HandlerType { get; }
    public EnumState State { get; private set; } = EnumState.None;

    private InputHandler m_child_handler = null;
    
    protected readonly InputHandlerContext m_context;

    public InputHandler(InputHandlerContext _context)
    {
        m_context = _context;
    }


    protected abstract void OnStart();
    protected abstract bool OnUpdate();
    protected abstract void OnFinish();

    // protected abstract void OnPause();
    // protected abstract void OnResume();

    protected void SetChildHandler(InputHandler _child_handler)
    {
        m_child_handler = _child_handler;
    }


    public bool Update()
    {
        if (State != EnumState.Update)
        {
            OnStart();
            State = EnumState.Update;

            // 
            m_context.ClearInputParams();
        }

        // �ڽ� �ڵ鷯�� ������ �װ� ���� ó��.        
        if (m_child_handler != null)
        {
            if (m_child_handler.Update())
                m_child_handler = null;
        }
        
        if (m_child_handler == null)
        {
            if (OnUpdate())
            {
                State = EnumState.Finish;
            }
        }

        

        if (State != EnumState.Update)
        {
            OnFinish();
            State = EnumState.Finish;

            // 
            m_context.ClearInputParams();
        }

        return State == EnumState.Finish;
    }

    public void Abort()
    {
        if (State == EnumState.Update)
        {
            OnFinish();
            State = EnumState.Finish;
        }
    }

    // public void Pause()
    // {
    //     if (State == EnumState.Update)
    //     {
    //         OnPause();
    //     }
    // }


    // public void Resume()
    // {
    //     if (State == EnumState.Update)
    //     {
    //         OnResume();
    //     }
    // }

    // public void OnInput()
}

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

    // protected override void OnPause()
    // {
    // }

    // protected override void OnResume()
    // {
    // }
}

public class InputHandler_Grid_Select : InputHandler
{
    
    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    int     SelectTile_X         { get; set; } = 0;
    int     SelectTile_Y         { get; set; } = 0;
    float   MoveTile_LastTime    { get; set; } = 0f;
    const float MOVE_TILE_INTERVAL = 0.15f;
    const float MOVE_DIRECTION_MIN = 0.4f;

    public InputHandler_Grid_Select(InputHandlerContext _context) 
        : base(_context)
    {
    }

    struct ProcParam_Result
    {
        public bool       IsSelect      { get; set; }
        public bool       IsCancel      { get; set; }
        public Vector2    MoveDirection { get; set; }
        public Vector2Int SelectTile    { get; set; }
    }



    protected override void OnStart()
    {

    }

    protected override bool OnUpdate()
    {
        // �Է� �Ķ� ó��.
        var proc_result =OnUpdate_ProcParam(m_context.InputParamQueue);

       
        if (proc_result.IsSelect)
        {
            // ���� ó��.
            OnUpdate_Select();
        }
        else if (proc_result.IsCancel)
        {
            // ��� ó��.
            return true;
        }
        else if (proc_result.MoveDirection != Vector2.zero)
        {
            // Ÿ�� �̵�.
            OnUpdate_Move(proc_result.MoveDirection);
        }
        else if (proc_result.SelectTile != Vector2Int.zero)
        {
            // Ÿ�� ����.
            // OnUpdate_Select(proc_result.SelectTile);
        }
        

        return false;
    }

    protected override void OnFinish()
    {
        // MoveTile_Direction = Vector2.zero;
    }

    // protected override void OnPause()
    // {
    //     // MoveTile_Direction = Vector2.zero;
        
    // }

    // protected override void OnResume()
    // {
    // }

    ProcParam_Result OnUpdate_ProcParam(Queue<InputParam> _queue_input_param)
    {        
        ProcParam_Result result = new ProcParam_Result();
        while (_queue_input_param.Count > 0)
        {
            var input_param = _queue_input_param.Dequeue();

            switch(input_param)
            {
                case InputParam_Grid_Move input_param_move: 
                result.MoveDirection = input_param_move.Direction;
                break;

                case InputParam_Grid_Pointer input_param_pointer:
                result.SelectTile  = new Vector2Int(
                    (int)input_param_pointer.Position.x, 
                    (int)input_param_pointer.Position.y);
                break;

                case InputParam_Grid_Select:
                result.IsSelect = true;
                break;
                case InputParam_Grid_Cancel:
                result.IsCancel = true;
                break;  
            }
        }

        return result;
    }

    void OnUpdate_Move(Vector2 _move_direction)
    {
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_TILE_INTERVAL);
        if (is_time_passed == false)
            return;

        var is_moved = false;

        if (Mathf.Abs(_move_direction.x) > MOVE_DIRECTION_MIN)
        {
            SelectTile_X += _move_direction.x > 0 ? 1 : -1;
            is_moved = true;
        }
        if (Mathf.Abs(_move_direction.y) > MOVE_DIRECTION_MIN)
        {
            SelectTile_Y += _move_direction.y > 0 ? 1 : -1;
            is_moved = true;
        }

        if (is_moved == true)
        {
            MoveTile_LastTime = Time.time;
            //Debug.LogWarning($"SelectTile_X: {SelectTile_X}, SelectTile_Y: {SelectTile_Y}");
        }
    }

  
    void OnUpdate_Select()
    {
        // Debug.LogWarning(_input_param_select.Position);
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
        {
            Debug.LogError("TerrainMapManager.Instance.TerrainMap is null");
            return;
        }

        var entity_id = terrain_map.BlockManager.FindEntityID(SelectTile_X, SelectTile_Y);
        if (entity_id > 0)
        {
            var entity         = EntityManager.Instance.GetEntity(entity_id);
            var faction        = entity.GetFaction();
            var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            switch(commander_type)
            {
                case EnumCommanderType.None:
                break;

                case EnumCommanderType.Player:
                //InputManager.Instance.StackHandler(EnumInputHandlerType.UI_Command);
                break;

                case EnumCommanderType.AI:
                break;
            }
        }
        else
        {
            //InputManager.Instance.StackHandler(EnumInputHandlerType.UI_Menu);
        }

        return;
    }
}

// public class InputHandler_Grid_Command : InputHandler
// {
//     // public InputHandler_Grid_Command(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
//     // {

//     // }

//     public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Command;

//     protected override void OnStart()
//     {
//     }

//     protected override bool OnUpdate(Queue<InputParam> _queue_input_param)
//     {
//         return false;
//     }

//     protected override void OnFinish()
//     {
//     }

//     protected override void OnPause()
//     {
        
//     }

//     protected override void OnResume()
//     {
        
//     }
// }
    