using System.Collections.Generic;
using Battle;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid_Select,    // 타일 선택
    Grid_Command,   // 유닛 행동 

    UI_Menu,        // UI 메뉴
    UI_Command,     // 유닛 행동 메뉴 (이동, 공격, 스킬)
    
}




public abstract class InputHandler
{
    public enum EnumState
    {
        None,
        Start,   // 시작
        Update,  // 반복
        Finish,  // 종료
    }

    public abstract EnumInputHandlerType HandlerType { get; }
    public EnumState State { get; private set; } = EnumState.None;
    
    // protected readonly Queue<InputParam> m_queue_input_param;

    // public InputHandler(Queue<InputParam> _queue_input_param)
    // {
    //     m_queue_input_param = _queue_input_param;
    // }


    protected abstract void OnStart();
    protected abstract bool OnUpdate(Queue<InputParam> _queue_input_param);
    protected abstract void OnFinish();

    protected abstract void OnPause();
    protected abstract void OnResume();


    public bool Update(Queue<InputParam> _queue_input_param)
    {
        if (State != EnumState.Update)
        {
            OnStart();
            State = EnumState.Update;
        }

        if (OnUpdate(_queue_input_param) == true)
        {
            State = EnumState.Finish;
        }

        if (State != EnumState.Update)
        {
            OnFinish();
            State = EnumState.Finish;
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

    public void Pause()
    {
        if (State == EnumState.Update)
        {
            OnPause();
        }
    }


    public void Resume()
    {
        if (State == EnumState.Update)
        {
            OnResume();
        }
    }

    // public void OnInput()
}

public class InputHandler_UI_Menu : InputHandler
{
    // public InputHandler_UI(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
    // {
    // }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.UI_Menu;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate(Queue<InputParam> _queue_input_param)
    {
        return false;
    }

    protected override void OnFinish()
    {
    }

    protected override void OnPause()
    {
    }

    protected override void OnResume()
    {
    }
}

public class InputHandler_Grid_Select : InputHandler
{
    
    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    int     SelectTile_X         { get; set; } = 0;
    int     SelectTile_Y         { get; set; } = 0;

    Vector2 MoveTile_Direction { get; set; } = Vector2.zero;
    float   MoveTile_LastTime  { get; set; } = 0f;
    const float MOVE_TILE_INTERVAL = 0.15f;
    const float MOVE_DIRECTION_MIN = 0.4f;


    bool IsSelect { get; set; } = false;
    bool IsCancel { get; set; } = false;



    protected override void OnStart()
    {

    }

    protected override bool OnUpdate(Queue<InputParam> _queue_input_param)
    {
        // 입력 파람 처리.
        OnUpdate_ProcParam(_queue_input_param);

        if (IsSelect)
        {
            // 선택 처리.
            //return true;

            OnUpdate_Select();

        }
        else if (IsCancel)
        {
            // 취소 처리.
            return true;
        }
        else 
        {
            // 타일 이동.
            OnUpdate_Move();
        }
        

        return false;
    }

    protected override void OnFinish()
    {
        MoveTile_Direction = Vector2.zero;
    }

    protected override void OnPause()
    {
        MoveTile_Direction = Vector2.zero;
        
    }

    protected override void OnResume()
    {
    }

    void OnUpdate_ProcParam(Queue<InputParam> _queue_input_param)
    {        
        while (_queue_input_param.Count > 0)
        {
            var input_param = _queue_input_param.Dequeue();

            switch(input_param)
            {
                case InputParam_Grid_Move input_param_move: 
                MoveTile_Direction = input_param_move.Direction;
                break;

                case InputParam_Grid_Pointer input_param_pointer:
                SelectTile_X = (int)input_param_pointer.Position.x;
                SelectTile_Y = (int)input_param_pointer.Position.y;
                break;

                case InputParam_Grid_Select input_param_select:
                IsSelect = true;
                break;
                case InputParam_Grid_Cancel input_param_cancel:
                IsCancel = true;
                break;  
            }
        }
    }

    void OnUpdate_Move()
    {
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_TILE_INTERVAL);
        if (is_time_passed == false)
            return;

        var is_moved = false;

        if (Mathf.Abs(MoveTile_Direction.x) > MOVE_DIRECTION_MIN)
        {
            SelectTile_X += MoveTile_Direction.x > 0 ? 1 : -1;
            is_moved = true;
        }
        if (Mathf.Abs(MoveTile_Direction.y) > MOVE_DIRECTION_MIN)
        {
            SelectTile_Y += MoveTile_Direction.y > 0 ? 1 : -1;
            is_moved = true;
        }

        if (is_moved == true)
        {
            MoveTile_LastTime = Time.time;
            Debug.LogWarning($"SelectTile_X: {SelectTile_X}, SelectTile_Y: {SelectTile_Y}");
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

    }
    
}

public class InputHandler_Grid_Command : InputHandler
{
    // public InputHandler_Grid_Command(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
    // {

    // }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Command;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate(Queue<InputParam> _queue_input_param)
    {
        return false;
    }

    protected override void OnFinish()
    {
    }

    protected override void OnPause()
    {
        
    }

    protected override void OnResume()
    {
        
    }
}
    