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

        // ����� �������� Ȯ��.
        OnUpdate_Verify_CommandEntity();

        // �Է��� ������� ����.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // �Է��� ������� ó��.
        OnUpdate_Input_Process(input_result);

        // Ÿ�� �̵�.
        OnUpdate_Tile_Move();

        // �̵� ���� ǥ��.
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
        // Ÿ�� Ŀ�� ����.
        ReleaseTileSelectVFX();
    }

    protected override void OnResume()
    {
        // Ÿ�� Ŀ�� ����
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
        // �Է� �Ķ� ó��.
        while (_queue_input_param.Count > 0)
        {
            var input_param = _queue_input_param.Dequeue();

            switch(input_param)
            {
                case InputParam_Grid_Move input_param_move: 
                {
                    // �̵� ���� ����.
                    _result.MoveDirection = (true, input_param_move.Direction);
                }
                break;

                case InputParam_Grid_Pointer input_param_pointer:
                {
                    // // Ÿ�� ��ġ ����.
                    // var tile_pos      = new Vector2Int((int)input_param_pointer.Position.x, (int)input_param_pointer.Position.y);
                    // _result.SelectTile = (true, tile_pos);
                }
                break;

                case InputParam_Grid_Select:
                {
                    // ���� ó��.
                    _result.IsSelect = true;
                }
                break;

                case InputParam_Grid_Cancel:
                {
                    // ��� ó��.
                    _result.IsCancel = true;
                }
                break;  

                case InputParam_Grid_Forward:
                {
                    // ������ ó��.
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
            // ���� ó��.
            OnUpdate_Input_Process_Select();
        }
        else if (_result.IsCancel)
        {
            OnUpdate_Input_Process_Cancel();
            // ��� ó��. (����)
            // IsFinish = true;
        }
        else if (_result.IsForward)
        {
            // �ɼ� ��ư��. (�� ���� ���� ǥ�� ��)
            OnUpdate_Input_Process_Forward();
        }
        else if (_result.MoveDirection.changed)
        {
            // �̵� ���� ó��.
            OnUpdate_Input_Process_Move(_result.MoveDirection.value);
        }
        // else if (_result.SelectTile.changed)
        // {
        //     // Ÿ�� ����.
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
            // �������� ĳ���Ͱ� ���� ���.
            var entity = EntityManager.Instance.GetEntity(CommandEntityID);
            if (entity != null && entity.Cell == SelectCursor)
            {
                // ���� Ŀ�ǵ� UI ����.
                GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(CommandEntityID));        
            }
            else
            {
                // TODO: ��� �̵��̶� �����ָ� �Ƿ���.?
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
                // TODO: �ý��� �޴� ����� �ҵ�?
            }
        }
    }

    void OnUpdate_Input_Process_Cancel()
    {
        if (CommandEntityID > 0)
        {
            // �������� ĳ���Ͱ� ���� ���.
            Process_Cancel_CommandEntity();
        }
        else
        {
            // �������� ĳ���Ͱ� ���� ���.
        }
    }

    void OnUpdate_Input_Process_Forward()
    {
        // TODO: ������ ó��.
        // �� ���� ���� ON/OFF ó��
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
        // �̵� ������ ������ ����.
        if (MoveDirection == Vector2Int.zero)
            return;
        
        // �̵� �ð��� �������� Ȯ��.
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_INTERVAL);
        if (is_time_passed == false)
            return;

        // Ÿ�� �̵�.
        var new_position_x = SelectCursor.x + MoveDirection.x;
        var new_position_y = SelectCursor.y + MoveDirection.y;

        MoveSelcectCursor(new_position_x, new_position_y);

        // ���õ� ���� �̵� ó��.
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

        // ���õ� Ÿ���� ��ƼƼ ID.
        Int64 tile_entity_id = terrain_map.EntityManager.GetCellData(SelectCursor.x, SelectCursor.y);
        
        // �̵� ���� ǥ�� ��ƼƼ ID.
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

        // �̵� �ð� ����.
        MoveTile_LastTime = Time.time;

        // ����Ʈ ��ġ �̵�.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<VFX_TransformEvent>.Acquire()
            .SetID(VFX_Select)
            .SetPosition(SelectCursor.CellToPosition())                
        );       
    }

    // �������϶��� �̵� ó��, ���� �̵��� �Ҹ� ����.
    private void MoveSelectedEntity(
        Int64          _entity_id,
        (int x, int y) _cell,
        bool           _is_immediate)
    {
        if (_entity_id == 0)
            return;


        // ���� �̵���� �ߴ�.
        BattleSystemManager.Instance.PushCommand(
            new Command_Abort
            (
                _entity_id,
                _is_pending_only: true
            )
        );

        // �̵� ��� �߰�.
        BattleSystemManager.Instance.PushCommand(
            new Command_Move
            (
                _entity_id,
                _cell,
                // EnumCellPositionEvent.Enter,

                _visual_immediate:    _is_immediate, // ��� �̵� ����.
                _execute_command: false,         // ��� ���� ó��. 
                _is_plan:         true       // ��ǥ ���� ó��.
            )
        );
    }


    void ReleaseVFX()
    {
        // ���� ����Ʈ ����.
        ReleaseTileSelectVFX();

        // �̵� ���� ǥ�� �ʱ�ȭ.
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
            // ����� ���� ���� ����.
            SetCommandEntity(_entity_id);
        }
        else
        {
            // TODO: ���ݹ��� ǥ���� ���ֿ� ���.
        }
    }


    void SetCommandEntity(Int64 _entity_id)
    {
        var entity  = EntityManager.Instance.GetEntity(_entity_id);                
        if (entity == null)
            return;

        // ����� ���� ���� ����.
        CommandEntityID = _entity_id;

        // ����� ������ ������ �� ���� ���¸� �����մϴ�.
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
                // ���� ��ġ�� ����.
                entity.UpdateCellPosition(
                    base_position,
                    (_apply: true, _immediatly: true),
                    _is_plan: false);

                CommandEntityID = 0;
            }
            else
            {
                // Ŀ�� �̵�.
                MoveSelcectCursor(base_position.x, base_position.y);

                // ��� ���̴� ĳ���� ���� ��ġ�� ����.    
                MoveSelectedEntity(
                    CommandEntityID, 
                    base_position, 
                    _is_immediate: true);
                    // _is_plan: true);
            }
        }
        else
        {
            // ���� ��Ȳ
            Debug.LogError($"Process_Cancel_CommandEntity: entity is null: {CommandEntityID}");
            CommandEntityID = 0;
        }
    }



    void CreateTileSelectVFX()
    {
        // ����Ʈ ����.
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
