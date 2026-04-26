using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;


class InputParam_Result : IPoolObject
{
    public bool                             IsSelect      { get; set; }
    public bool                             IsCancel      { get; set; }
    public bool                             IsForward     { get; set; }
    public (bool changed, Vector2    value) MoveDirection { get; set; }
    // public (bool changed, Vector2Int value) SelectTile    { get; set; }

    public void Reset()
    {
        IsSelect      = false;
        IsCancel      = false;
        IsForward     = false;
        MoveDirection = default;
        // SelectTile    = default;
    }
}

[EventReceiver(
    typeof(Grid_Cursor_Event),
    typeof(Battle_Command_Event),
    typeof(GUI_Unit_Command_Event))]
public class InputHandler_Grid_Select : InputHandler, IEventReceiver
{   
    // public enum EnumMode
    // {
    //     Select,
    //     Move,
    // }


    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    //  EnumMode         Mode               { get; set; } = EnumMode.Select;
    (int x, int y)    SelectCursor       { get; set; } = (0, 0);               
    float             MoveTile_LastTime  { get; set; } = 0f;
    Vector2Int        MoveDirection      { get; set; } = Vector2Int.zero; 

    

  
    Int64             CommandEntityID 
    { 
        get
        {
            // return m_command_entity_id;
            return BattleSystemManager.Instance.BlackBoard
                .GetValue(EnumBattleBlackBoard.CommandEntityID_Input);
        }         
        set
        {
            // m_command_entity_id = value;

            // 명령 입력 중인 엔티티 ID 설정.
            BattleSystemManager.Instance.BlackBoard.SetValue(
                EnumBattleBlackBoard.CommandEntityID_Input, value);
        }
    } 

    // 이동할 타일을 선택중인 상태.
    public bool    IsSelectMoveTile => CommandEntityID > 0;

    VFXHelper_Path DrawPath { get; set; } = new();
    
    Int64          m_vfx_select = 0;


    public InputHandler_Grid_Select(InputHandlerContext _context) 
        : base(_context)
    {
        
    }

    protected override void OnStart()
    {
        CreateTileSelectVFX();

        EventDispatchManager.Instance.AttachReceiver(this);
        //Debug.Log("InputHandler_Grid_Select OnStart");
    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        // 선택 엔티티 확인.
        OnUpdate_Verify_CommandEntity();

        // SyncMovePickingEntryMove();

        // 입력 파라미터 계산.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // 입력 파라미터 처리.
        OnUpdate_Input_Process(input_result);

        // 선택 셀 이동.
        OnUpdate_Tile_Move();

        // RefreshMovePathVfxIfPicking();

        // 이동 범위 그리기.
        Update_DrawMoveRange();

        // 이동 경로 그리기.
        Update_DrawMovePath();

        // 
        // if (GUIManager.Instance.GetInputFocusGUI() > 0)
        // {
        //     var input_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());
        //     SetChildHandler(input_handler);
        // }


        ObjectPool<InputParam_Result>.Return( input_result);
        
        return false;
        // return IsFinish;
    }

    protected override void OnFinish()
    {
        ReleaseVFX();
        
        // IsFinish                  = false;
        m_vfx_select              = 0;
        MoveTile_LastTime         = 0f; 
        MoveDirection             = Vector2Int.zero;
        CommandEntityID           = 0;
        // SelectTile_X      = 0;
        // SelectTile_Y      = 0;

        // BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.Grid_MovePicking_EntityID, 0);
        // m_prevMovePickingBlackboard      = false;
        // m_wantReopenCommandMenuAfterMove = false;
        // m_reopenMenuAfterMoveEntityId    = 0;

        // VFXHelper_Path.ReleaseTilePathVfx(ref m_vfx_paths);

        EventDispatchManager.Instance.DetachReceiver(this);
        //Debug.Log("InputHandler_Grid_Select OnFinish");
    }

    protected override void OnPause()
    {
        // 선택 효과 해제.
        ReleaseTileSelectVFX();

        // 이동 경로 해제.
        DrawPath.Clear();
    }

    protected override void OnResume()
    {
        // 선택 효과 생성.
        CreateTileSelectVFX();
    }

    void OnUpdate_Verify_CommandEntity()
    {
        if (CommandEntityID <= 0)
            return;

        var entity = EntityManager.Instance.GetEntity(CommandEntityID);
        if (entity == null)
        {
            DeselectCommandEntity();
            return;
        }

        var faction           = entity.GetFaction();
        var commander_type    = BattleSystemManager.Instance.GetFactionCommanderType(faction);
        var is_enable_command = entity.HasCommandEnable() && commander_type == EnumCommanderType.Player;
        if (is_enable_command == false)
        {
            DeselectCommandEntity();
        }
    }

    void OnUpdate_Input_Compute(Queue<InputParam> _queue_input_param, ref InputParam_Result _result)
    {     
        // 입력 파라미터 처리.
        while (_queue_input_param.Count > 0)
        {
            var input_param = _queue_input_param.Dequeue();

            switch(input_param)
            {
                case InputParam_Grid_Move input_param_move: 
                {
                    // 이동 방향 설정.
                    _result.MoveDirection = (true, input_param_move.Direction);
                }
                break;

                case InputParam_Grid_Pointer input_param_pointer:
                {
                    // // 선택 셀 설정.
                    // var tile_pos      = new Vector2Int((int)input_param_pointer.Position.x, (int)input_param_pointer.Position.y);
                    // _result.SelectTile = (true, tile_pos);
                }
                break;

                case InputParam_Grid_Select:
                {
                    // 선택 여부 설정.
                    _result.IsSelect = true;
                }
                break;

                case InputParam_Grid_Cancel:
                {
                    // 취소 여부 설정.
                    _result.IsCancel = true;
                }
                break;  

                case InputParam_Grid_Forward:
                {
                    // 전진 여부 설정.
                    _result.IsForward = true;
                }
                break;
            }
        }
    }


    void OnUpdate_Input_Process(InputParam_Result _result)
    {
       // 명령 진행중일 때는 입력 처리하지 않는다.
       if (BattleSystemManager.Instance.BlackBoard
            .HasValue(EnumBattleBlackBoard.CommandEntityID_Progress))
       {
           return;
       }
        


        if (_result.IsSelect)
        {
            // 선택 여부 처리.
            OnUpdate_Input_Process_Select();
        }
        else if (_result.IsCancel)
        {
            OnUpdate_Input_Process_Cancel();
            // 취소 여부 처리. (선택)
            // IsFinish = true;
        }
        else if (_result.IsForward)
        {
            // 전진 여부 처리. (선택)
            OnUpdate_Input_Process_Forward();
        }
        else if (_result.MoveDirection.changed)
        {
            // 이동 방향 처리.
            OnUpdate_Input_Process_Move(_result.MoveDirection.value);
        }
        // else if (_result.SelectTile.changed)
        // {
        //     // 선택 셀 처리.
        //     // OnUpdate_Select(proc_result.SelectTile);
        // }
    }

    void OnUpdate_Input_Process_Select()
    {
        // Debug.LogWarning(_input_param_select.Position);
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
        {
            Debug.LogError("TerrainMapManager.Instance.TerrainMap is null");
            return;        
        }

        // 유닛 선택.
        if (CommandEntityID == 0)
        {
            var entity_id = terrain_map.EntityManager.GetCellData(SelectCursor.x, SelectCursor.y);
            if (entity_id > 0)
            {
                SelectEntity(entity_id);
            }
        }
        else 
        {
            // 유닛 이동 명령.
            MoveCommandEntity(SelectCursor, _is_immediate: false);
        }

    }

    void OnUpdate_Input_Process_Cancel()
    {
        if (CommandEntityID > 0)
        {
            // 원래 위치로 롤백.
            Process_CommandEntity_ReturnToBasePosition();

            // 행동 메뉴 UI 오픈.
            GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));        
        }
        else
        {
            // 선택 셀 취소.
        }
    }

    void OnUpdate_Input_Process_Forward()
    {
        // TODO: 전진 여부 처리.
        // 전진 여부 ON/OFF 처리
    }

    void OnUpdate_Input_Process_Move(Vector2 _move_direction)
    {           
        var input_direction_x = 0;
        var input_direction_y = 0;

        if (Mathf.Abs(_move_direction.x) > MOVE_DIRECTION_MIN)
        {
            input_direction_x = _move_direction.x > 0 ? 1 : -1;        
        }
        
        if (Mathf.Abs(_move_direction.y) > MOVE_DIRECTION_MIN)
        {
            input_direction_y = _move_direction.y > 0 ? 1 : -1;
        }

        MoveDirection = new Vector2Int(input_direction_x, input_direction_y);
    }

      

    void OnUpdate_Tile_Move()
    {
        //  아군 턴 차례가 맞는지 체크.
        var faction_cur       = (int)BattleSystemManager.Instance.BlackBoard.GetValue(EnumBattleBlackBoard.CurrentFaction);
        var is_player_faction = BattleSystemManager.Instance.GetFactionCommanderType(faction_cur) == EnumCommanderType.Player;        
        if (is_player_faction == false)
            return;

        // 이동 방향 확인.
        if (MoveDirection == Vector2Int.zero)
            return;
        
        // 이동 시간 확인.
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_INTERVAL);
        if (is_time_passed == false)
            return;

        // 선택 셀 이동.
        var new_position_x = SelectCursor.x + MoveDirection.x;
        var new_position_y = SelectCursor.y + MoveDirection.y;

        MoveSelcectCursor(new_position_x, new_position_y);
    }

    void Update_DrawMoveRange()
    {
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
        {
            //Debug.LogError("TerrainMapManager.Instance.TerrainMap is null");
            return;
        }

        // 선택 셀 엔티티 ID.
        Int64 tile_entity_id = terrain_map.EntityManager.GetCellData(SelectCursor.x, SelectCursor.y);
        
        // 이동 범위 엔티티 ID.
        Int64 draw_entity_id = CommandEntityID > 0 ? CommandEntityID : tile_entity_id;

        
        var entity = EntityManager.Instance.GetEntity(draw_entity_id);
        if (entity == null)
            draw_entity_id = 0;


        var draw_flag = (int)Battle.MoveRange.EnumDrawFlag.MoveRange 
                      | (int)Battle.MoveRange.EnumDrawFlag.AttackRange
                      | (int)Battle.MoveRange.EnumDrawFlag.WandRange
                      | (int)Battle.MoveRange.EnumDrawFlag.VisitOccupyCell;

        // 현재 선택중인 유닛의 범위 표시의 경우. 가능한 것들만 표시해줍시다.
        if (CommandEntityID > 0 && CommandEntityID == draw_entity_id)
        {
            if (entity != null)
            {
                if (entity.HasCommandEnable(EnumCommandFlag.Move) == false)
                    draw_flag &= ~(int)Battle.MoveRange.EnumDrawFlag.MoveRange;

                if (entity.HasCommandEnable(EnumCommandFlag.Action) == false)
                {
                    draw_flag &= ~(int)Battle.MoveRange.EnumDrawFlag.AttackRange;
                    draw_flag &= ~(int)Battle.MoveRange.EnumDrawFlag.WandRange;
                    draw_flag &= ~(int)Battle.MoveRange.EnumDrawFlag.ExchangeRange;
                }

                if (entity.HasCommandEnable(EnumCommandFlag.Exchange) == false)
                    draw_flag &= ~(int)Battle.MoveRange.EnumDrawFlag.ExchangeRange;
            }                         
        }

        


        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: draw_flag,

            _entityID:    
                draw_entity_id,
                
            _use_base_position: 
                true);

        if (draw_entity_id == 0)
        {
            BattleSystemManager.Instance.DrawRange.Clear();
        }
    }

    void Update_DrawMovePath()
    {
        // 이동 타일을 선택중인 상황이 아닐 경우는 경로를 그리지 않는다.
        if (IsSelectMoveTile == false || CommandEntityID == 0)
        {
            DrawPath.Clear();
            return;            
        }

        // 도착 셀. 
        var end_cell = SelectCursor;       
        
        // 길찾기 진행.
        using var path_nodes = ListPool<PathNode>.AcquireWrapper();
        if (PathFind(CommandEntityID, end_cell, true, path_nodes.Value))
        {
            DrawPath.DrawPath(path_nodes.Value);
        }
    }

    bool PathFind(
        Int64          _entity_id, 
        (int x, int y) _cell_to, 
        bool           _ignore_occupancy = false, 
        List<PathNode> _path_nodes = null)
    {
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
            return false;

        var entity = EntityManager.Instance.GetEntity(_entity_id);
        if (entity == null)
            return false;


        var path_find_option = PathAlgorithm.PathFindOption.Create()
                        // 이동 가능한 범위를 벗어나지 않기위해 (범위,위치) 셋팅
                        .SetMoveLimitRange(entity.PathMoveRange, entity.PathBasePosition);

        // 목표지점이 점유되어있는지 체크하지 않음.
        if (_ignore_occupancy)
            path_find_option.SetIgnoreOccupancy();


        // entity.PathNodeManager.CreatePath() 와 거의 동일한 로직으로 작동
        return PathAlgorithm.PathFind(terrain_map, entity, 
                entity.PathBasePosition, _cell_to, 

                _option: path_find_option,


                _path_nodes: _path_nodes).result;
    }

    private void MoveSelcectCursor(int _x, int _y)
    {
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map != null && terrain_map.IsInBound(_x, _y) == false)
        {
            return;
        }

        SelectCursor = (_x, _y);

        // 이동 시간 설정.
        MoveTile_LastTime = Time.time;


        VFXHelper.UpdateTileSelectVFX(m_vfx_select, SelectCursor);

    }

    // 명령 엔티티 이동
    private void MoveCommandEntity(
        (int x, int y) _cell,
        bool           _is_immediate)
    {
        if (CommandEntityID == 0)
            return;

        
        // 이동 가능한지 체크.
        if (PathFind(CommandEntityID, _cell, _ignore_occupancy: false) == false)
            return;

        // 이동 명령 예약.
        ServiceLocator<CommandQueueHandler>.Get(ServiceLocator.GLOBAL).PushCommand(
            new Command_Move
            (
                CommandEntityID,
                _cell,

                _visual_immediate: _is_immediate // 즉시 이동.
                // _is_plan:          true           // 실제 이동 점유 처리 여부.
                // _execute_command:  true,          // 명령 실행 여부. 
            )
        );
    }


    void ReleaseVFX()
    {
        // 선택 효과 해제.
        ReleaseTileSelectVFX();

        // 이동 범위 해제.
        BattleSystemManager.Instance.DrawRange.Clear();   
    }

    void SelectEntity(Int64 _entity_id)
    {
        var entity  = EntityManager.Instance.GetEntity(_entity_id);                
        if (entity == null)
            return;

        var faction            = entity.GetFaction();
        var commander_type     = BattleSystemManager.Instance.GetFactionCommanderType(faction);
        bool is_enable_command = entity.HasCommandEnable() && commander_type == EnumCommanderType.Player;
        if  (is_enable_command)
        {
            // 선택 엔티티 설정.
            CommandEntityID = _entity_id;

            // 셀 점유 상태 해제.
            //entity.UpdateCellOccupied(false);  

            // 행동 메뉴 UI 오픈.
            GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));    
        }
        else
        {
            // TODO: 명령을 내릴수 없는 유닛일 때의 대한 처리 필요.
        }
    }

    bool CheckCommandEntityIsInCommandProgress()
    {
        if (CommandEntityID == 0)
            return false;

        var entity = EntityManager.Instance.GetEntity(CommandEntityID);
        if (entity == null)
            return false;


        return entity.GetCommandProgressState() == EnumCommandProgressState.Progress;        
    }

    void DeselectCommandEntity()
    {
        if (CommandEntityID > 0)
        {
            // var entity = EntityManager.Instance.GetEntity(CommandEntityID);
            // if (entity != null && entity.IsActive)
            // {
            //     // 셀 점유 상태 셋팅.
            //     entity.UpdateCellOccupied(true);  
            // }


            CommandEntityID = 0;
        }
    }


    // void SetCommandEntity(Int64 _entity_id)
    // {
    //     var entity  = EntityManager.Instance.GetEntity(_entity_id);                
    //     if (entity == null)
    //         return;
    //     // 선택 엔티티 설정.
    //     CommandEntityID = _entity_id;
    //     // 명령을 내렸을 때 시작시...! 셀 점유 해제.
    //     // entity.UpdateCellOccupied(false);
    // }

    bool Process_CommandEntity_ReturnToBasePosition()
    {
        if (CommandEntityID == 0)
            return false;

        var entity = EntityManager.Instance.GetEntity(CommandEntityID);
        if (entity != null)
        {
            // 이미 원래 위치에 있으면 처리하지 않는다.
            if (entity.PathBasePosition == entity.Cell)
                return false;

            // 좌표 점유 여부.
            var occupy_cell = (entity.Cell_Occupied == false);

            // 위치 상태 원상 복구.
            entity.UpdateCellPosition(
                entity.PathBasePosition,
                (_apply: true, _immediatly: true));
                // _is_plan: occupy_cell);

            // 이동 가능한 상태로 셋팅.
            entity.TryCommand_MoveAgain(false);

            // 커서 좌표도 이동.
            MoveSelcectCursor(entity.PathBasePosition.x, entity.PathBasePosition.y);
            
            return true;
        }
        else
        {
            // 선택 엔티티 없음.
            Debug.LogError($"Process_Cancel_CommandEntity: entity is null: {CommandEntityID}");
            return false;
        }
    }



    void CreateTileSelectVFX()
    {
        // 선택 효과 생성.
        m_vfx_select = VFXHelper.CreateTileSelctVFX(SelectCursor);
    } 

    void ReleaseTileSelectVFX()
    {
        VFXHelper.ReleaseTileSelectVFX(ref m_vfx_select);
    }


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case Grid_Cursor_Event grid_cursor_event:
                OnReceiveEvent_GridCursorEvent(grid_cursor_event);
                break;

            case Battle_Command_Event battle_command_event:
                OnReceiveEvent_Battle_CommandEvent(battle_command_event);
                break;

            case GUI_Unit_Command_Event grid_command_event:
                OnReceiveEvent_Control_Event(grid_command_event);
                break;
        }
    }

    void OnReceiveEvent_GridCursorEvent(Grid_Cursor_Event _event)
    {
        if (_event == null)
            return;

        MoveSelcectCursor(_event.Cell.x, _event.Cell.y);        
    }

    private void OnReceiveEvent_Battle_CommandEvent(Battle_Command_Event _battle_command_event)
    {
        if (_battle_command_event == null)
            return;

        if (_battle_command_event.EntityID == CommandEntityID && CommandEntityID > 0)
        {
            // 명령이 완료되었고,
            // 가능한 행동이 있으면 행동 메뉴 UI 오픈.
            var entity = EntityManager.Instance.GetEntity(CommandEntityID);
            if (entity != null && entity.CommandManager.IsEmpty() && entity.HasCommandEnable())
            {
                GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));
            }                                
        }
    }

    private void OnReceiveEvent_Control_Event(GUI_Unit_Command_Event _gui_command_event)
    {
        if (_gui_command_event == null)
            return;

        switch (_gui_command_event.Event)
        {
            case GUI_Unit_Command_Event.EnumEvent.Move:
            {
                // if (CommandEntityID > 0)
                // {
                //     // // 이동을 위해 셀 점유 상태 해제.
                //     // var entity = EntityManager.Instance.GetEntity(CommandEntityID);
                //     // if (entity != null)
                //     // {
                //     //     entity.UpdateCellOccupied(false); 
                //     // }
                // }
            }
            break;


            // 선택한 엔티티 취소 메시지.
            case GUI_Unit_Command_Event.EnumEvent.Cancel:
            {
                if (CommandEntityID > 0)
                {
                    var entity = EntityManager.Instance.GetEntity(CommandEntityID);
                    if (entity != null)
                    {
                        var is_moved_entity = entity.PathBasePosition != entity.Cell;
                        if (is_moved_entity)
                        {
                            // 위치가 다를 경우 원래 위치로 롤백.
                            Process_CommandEntity_ReturnToBasePosition();
                        }
                        else
                        {
                            // 위치가 동일할 경우 선택 취소. (이미 행동을 진행하고 있었다면 취소 불가)
                            if (CheckCommandEntityIsInCommandProgress() == false)
                                DeselectCommandEntity();
                        }
                    }
                }
            }                
            break;
        }
    }





}
