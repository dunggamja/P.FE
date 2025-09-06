using System;
using System.Collections.Generic;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
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
    Int64                   VFX_Select                { get; set; } = 0;
    Int64                   CommandEntityID           { get; set; } = 0; 
    // (int x, int y)          CommandEntityBasePosition { get; set; } = (0, 0);


    


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

        // 명령이 가능한지 확인.
        OnUpdate_Verify_CommandEntity();

        // 입력의 결과값을 생성.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // 입력의 결과값을 처리.
        OnUpdate_Input_Process(input_result);

        // 타일 이동.
        OnUpdate_Tile_Move();

        // 이동 범위 표시.
        Update_DrawMoveRange();

        // 
        // if (GUIManager.Instance.GetInputFocusGUI() > 0)
        // {
        //     var input_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());
        //     SetChildHandler(input_handler);
        // }


        ObjectPool<InputParam_Result>.Return(ref input_result);
        
        
        return IsFinish;
    }

    protected override void OnFinish()
    {
        ReleaseVFX();
        
        IsFinish                  = false;
        VFX_Select                = 0;
        MoveTile_LastTime         = 0f; 
        MoveDirection             = Vector2Int.zero;
        CommandEntityID           = 0;
        // SelectTile_X      = 0;
        // SelectTile_Y      = 0;
    }

    protected override void OnPause()
    {
        // 타일 커서 제거.
        ReleaseTileSelectVFX();
    }

    protected override void OnResume()
    {
        // 타일 커서 생성
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
        // 입력 파람 처리.
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
                    // // 타일 위치 설정.
                    // var tile_pos      = new Vector2Int((int)input_param_pointer.Position.x, (int)input_param_pointer.Position.y);
                    // _result.SelectTile = (true, tile_pos);
                }
                break;

                case InputParam_Grid_Select:
                {
                    // 선택 처리.
                    _result.IsSelect = true;
                }
                break;

                case InputParam_Grid_Cancel:
                {
                    // 취소 처리.
                    _result.IsCancel = true;
                }
                break;  

                case InputParam_Grid_Forward:
                {
                    // 포워드 처리.
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
            // 선택 처리.
            OnUpdate_Input_Process_Select();
        }
        else if (_result.IsCancel)
        {
            OnUpdate_Input_Process_Cancel();
            // 취소 처리. (종료)
            // IsFinish = true;
        }
        else if (_result.IsForward)
        {
            // 옵션 버튼임. (적 공격 범위 표시 등)
            OnUpdate_Input_Process_Forward();
        }
        else if (_result.MoveDirection.changed)
        {
            // 이동 방향 처리.
            OnUpdate_Input_Process_Move(_result.MoveDirection.value);
        }
        // else if (_result.SelectTile.changed)
        // {
        //     // 타일 선택.
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
            // 조작중인 캐릭터가 있을 경우.
            var entity = EntityManager.Instance.GetEntity(CommandEntityID);
            if (entity != null && entity.Cell == SelectCursor)
            {
                // 유닛 커맨드 UI 열기.
                GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));        
            }
            else
            {
                // TODO: 즉시 이동이라도 시켜주면 되려나.?
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
                // TODO: 시스템 메뉴 열어야 할듯?
            }
        }
    }

    void OnUpdate_Input_Process_Cancel()
    {
        if (CommandEntityID > 0)
        {
            // 조작중인 캐릭터가 있을 경우.
            Process_Cancel_CommandEntity();
        }
        else
        {
            // 조작중인 캐릭터가 없을 경우.
        }
    }

    void OnUpdate_Input_Process_Forward()
    {
        // TODO: 포워드 처리.
        // 적 공격 범위 ON/OFF 처리
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
        // 이동 방향이 없으면 종료.
        if (MoveDirection == Vector2Int.zero)
            return;
        
        // 이동 시간이 지났는지 확인.
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_INTERVAL);
        if (is_time_passed == false)
            return;

        // 타일 이동.
        var new_position_x = SelectCursor.x + MoveDirection.x;
        var new_position_y = SelectCursor.y + MoveDirection.y;

        MoveSelcectCursor(new_position_x, new_position_y);

        // 선택된 유닛 이동 처리.
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
            Debug.LogError("TerrainMapManager.Instance.TerrainMap is null");
            return;
        }

        // 선택된 타일의 엔티티 ID.
        Int64 tile_entity_id = terrain_map.EntityManager.GetCellData(SelectCursor.x, SelectCursor.y);
        
        // 이동 범위 표시 엔티티 ID.
        Int64 draw_entity_id = CommandEntityID > 0 ? CommandEntityID : tile_entity_id;

        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: 
                  (int)Battle.MoveRange.EnumDrawFlag.MoveRange 
                | (int)Battle.MoveRange.EnumDrawFlag.AttackRange,

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

        // 이동 시간 갱신.
        MoveTile_LastTime = Time.time;

        // 이펙트 위치 이동.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<VFX_TransformEvent>.Acquire()
            .SetID(VFX_Select)
            .SetPosition(SelectCursor.CellToPosition())                
        );       
    }

    // 조작중일때의 이동 처리, 실제 이동력 소모 없음.
    private void MoveSelectedEntity(
        Int64          _entity_id,
        (int x, int y) _cell,
        bool           _is_immediate)
    {
        if (_entity_id == 0)
            return;


        // 기존 이동명령 중단.
        BattleSystemManager.Instance.PushCommand(
            new Command_Abort
            (
                _entity_id,
                _is_pending_only: true
            )
        );

        // 이동 명령 추가.
        BattleSystemManager.Instance.PushCommand(
            new Command_Move
            (
                _entity_id,
                _cell,
                // EnumCellPositionEvent.Enter,

                _visual_immediate:    _is_immediate, // 즉시 이동 여부.
                _execute_command: false,         // 명령 상태 처리. 
                _is_plan:         true       // 좌표 점유 처리.
            )
        );
    }


    void ReleaseVFX()
    {
        // 선택 이펙트 삭제.
        ReleaseTileSelectVFX();

        // 이동 범위 표시 초기화.
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
            // 명령을 내릴 유닛 선택.
            SetCommandEntity(_entity_id);
        }
        else
        {
            // TODO: 공격범위 표시할 유닛에 등록.
        }
    }


    void SetCommandEntity(Int64 _entity_id)
    {
        var entity  = EntityManager.Instance.GetEntity(_entity_id);                
        if (entity == null)
            return;

        // 명령을 내릴 유닛 선택.
        CommandEntityID = _entity_id;

        // 명령을 내리는 유닛은 셀 점유 상태를 해제합니다.
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
                // 원래 위치로 복귀.
                entity.UpdateCellPosition(
                    base_position,
                    (_apply: true, _immediatly: true),
                    _is_plan: false);

                CommandEntityID = 0;
            }
            else
            {
                // 커서 이동.
                MoveSelcectCursor(base_position.x, base_position.y);

                // 명령 중이던 캐릭터 원래 위치로 복귀.    
                MoveSelectedEntity(
                    CommandEntityID, 
                    base_position, 
                    _is_immediate: true);
                    // _is_plan: true);
            }
        }
        else
        {
            // 에러 상황
            Debug.LogError($"Process_Cancel_CommandEntity: entity is null: {CommandEntityID}");
            CommandEntityID = 0;
        }
    }



    void CreateTileSelectVFX()
    {
        // 이펙트 생성.
        var vfx_param = ObjectPool<VFXObject.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetPosition(SelectCursor.CellToPosition())
            .SetVFXName(AssetName.TILE_SELECTION);

        VFX_Select = VFXManager.Instance.CreateVFXAsync(vfx_param);
    }

    void ReleaseTileSelectVFX()
    {
        VFXManager.Instance.ReserveReleaseVFX(VFX_Select);
        VFX_Select = 0;
    }



}
