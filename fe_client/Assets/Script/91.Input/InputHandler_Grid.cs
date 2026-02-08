using System;
using System.Collections.Generic;
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


public class InputHandler_Grid_Select : InputHandler
{   

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    (int x, int y)          SelectCursor              { get; set; } = (0, 0);  
         
    bool                    IsFinish                  { get; set; } = false;                
    float                   MoveTile_LastTime         { get; set; } = 0f;
    Vector2Int              MoveDirection             { get; set; } = Vector2Int.zero;    
    Int64                   CommandEntityID           { get; set; } = 0; 
    // (int x, int y)          CommandEntityBasePosition { get; set; } = (0, 0);
    Int64                   m_vfx_select  = 0;


    


    public InputHandler_Grid_Select(InputHandlerContext _context) 
        : base(_context)
    {
    }




    protected override void OnStart()
    {
        CreateTileSelectVFX();
    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        // 선택 엔티티 확인.
        OnUpdate_Verify_CommandEntity();

        // 입력 파라미터 계산.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // 입력 파라미터 처리.
        OnUpdate_Input_Process(input_result);

        // 선택 셀 이동.
        OnUpdate_Tile_Move();

        // 이동 범위 그리기.
        Update_DrawMoveRange();

        // 
        // if (GUIManager.Instance.GetInputFocusGUI() > 0)
        // {
        //     var input_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());
        //     SetChildHandler(input_handler);
        // }


        ObjectPool<InputParam_Result>.Return( input_result);
        
        
        return IsFinish;
    }

    protected override void OnFinish()
    {
        ReleaseVFX();
        
        IsFinish                  = false;
        m_vfx_select                = 0;
        MoveTile_LastTime         = 0f; 
        MoveDirection             = Vector2Int.zero;
        CommandEntityID           = 0;
        // SelectTile_X      = 0;
        // SelectTile_Y      = 0;
    }

    protected override void OnPause()
    {
        // 선택 효과 해제.
        ReleaseTileSelectVFX();
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
            CommandEntityID = 0;
            return;
        }

        var faction           = entity.GetFaction();
        var commander_type    = BattleSystemManager.Instance.GetFactionCommanderType(faction);
        var is_enable_command = entity.HasCommandEnable() && commander_type == EnumCommanderType.Player;
        if (is_enable_command == false)
        {
            CommandEntityID = 0;
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

        if (CommandEntityID > 0)
        {
            // 선택 엔티티 확인.
            var entity = EntityManager.Instance.GetEntity(CommandEntityID);
            if (entity != null && entity.Cell == SelectCursor)
            {
                // 선택 엔티티 UI 확인.
                GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));        
            }
            else
            {
                // TODO: 선택 엔티티 이동.
            }
        }
        else
        {
            var entity_id = terrain_map.EntityManager.GetCellData(SelectCursor.x, SelectCursor.y);
            if (entity_id > 0)
            {
                SelectEntity(entity_id);
            }
            else
            {
                // TODO: 선택 셀 엔티티 확인.
            }
        }
    }

    void OnUpdate_Input_Process_Cancel()
    {
        if (CommandEntityID > 0)
        {
            // 선택 엔티티 취소.
            Process_Cancel_CommandEntity();
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

        // 선택 엔티티 이동.
        MoveSelectedEntity(
            CommandEntityID,
            SelectCursor,
            _is_immediate: false);
            // _is_plan: true);
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


        // FixedObject는 이동범위를 그리지 않는다. 
        var entity = EntityManager.Instance.GetEntity(draw_entity_id);
        if (entity == null)
            draw_entity_id = 0;


        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: 
                  (int)Battle.MoveRange.EnumDrawFlag.MoveRange 
                | (int)Battle.MoveRange.EnumDrawFlag.AttackRange
                | (int)Battle.MoveRange.EnumDrawFlag.WandRange
                | (int)Battle.MoveRange.EnumDrawFlag.VisitOccupyCell,

            _entityID:    
                draw_entity_id,
                
            _use_base_position: 
                true);

        if (draw_entity_id == 0)
        {
            BattleSystemManager.Instance.DrawRange.Clear();
        }
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


        VFXHelper.UpdateCursorVFX(m_vfx_select, SelectCursor);
   

    }

    // 선택 엔티티 이동, 기존에 예약된 명령들 취소 (진행중인 것은 냅둔다.)
    private void MoveSelectedEntity(
        Int64          _entity_id,
        (int x, int y) _cell,
        bool           _is_immediate)
    {
        if (_entity_id == 0)
            return;

        var entity = EntityManager.Instance.GetEntity(_entity_id);
        if (entity == null)
            return;


        // 기존에 예약된 명령들 취소 (진행중인 것은 냅둔다.)
        ServiceLocator<CommandQueueHandler>.Get(ServiceLocator.GLOBAL).PushCommand(
            new Command_Abort
            (
                _entity_id,
                _is_pending_only: true
            )
        );

        // 이동 명령 예약.
        ServiceLocator<CommandQueueHandler>.Get(ServiceLocator.GLOBAL).PushCommand(
            new Command_Move
            (
                _entity_id,
                _cell,
                // EnumCellPositionEvent.Enter,

                _visual_immediate:    _is_immediate, // 즉시 이동.
                _execute_command: false,             // 명령 실행 여부. 
                _is_plan:         true               // 계획 이동 여부.
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
            SetCommandEntity(_entity_id);
        }
        else
        {
            // TODO: 명령 엔티티 설정.
        }
    }


    void SetCommandEntity(Int64 _entity_id)
    {
        var entity  = EntityManager.Instance.GetEntity(_entity_id);                
        if (entity == null)
            return;

        // 선택 엔티티 설정.
        CommandEntityID = _entity_id;

        // 명령을 내렸을 때 시작시...! 셀 점유 해제.
        entity.UpdateCellOccupied(false);
    }

    void Process_Cancel_CommandEntity()
    {
        if (CommandEntityID == 0)
            return;

        var entity = EntityManager.Instance.GetEntity(CommandEntityID);
        if (entity != null)
        {
            var base_position  = entity.PathBasePosition;
            if (base_position == SelectCursor)
            {
                // 진행한 명령이 하나도 없을 경우에만 취소 처리.
                if (entity.IsAnyCommandDone() == false)
                {
                    // 선택 셀 이동.
                    entity.UpdateCellPosition(
                        base_position,
                        (_apply: true, _immediatly: true),
                        _is_plan: false);

                    CommandEntityID = 0;
                }
            }
            else
            {
                // 이동이 가능한 상태일 경우, 기존 위치로 이동 처리.
                if (entity.HasCommandEnable(EnumCommandFlag.Move))
                {
                    // 선택 셀 이동.
                    MoveSelcectCursor(base_position.x, base_position.y);

                    // 선택 엔티티 이동.    
                    MoveSelectedEntity(
                        CommandEntityID, 
                        base_position, 
                        _is_immediate: true);
                }
            }
        }
        else
        {
            // 선택 엔티티 없음.
            Debug.LogError($"Process_Cancel_CommandEntity: entity is null: {CommandEntityID}");
            CommandEntityID = 0;
        }
    }



    void CreateTileSelectVFX()
    {
        // 선택 효과 생성.
        m_vfx_select = VFXHelper.CreateCursorVFX(SelectCursor);
    } 

    void ReleaseTileSelectVFX()
    {
        VFXHelper.ReleaseCursorVFX(ref m_vfx_select);
    }



}
