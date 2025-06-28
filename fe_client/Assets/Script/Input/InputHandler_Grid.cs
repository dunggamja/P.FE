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
    public (bool changed, Vector2    value) MoveDirection { get; set; }
    // public (bool changed, Vector2Int value) SelectTile    { get; set; }

    public void Reset()
    {
        IsSelect      = false;
        IsCancel      = false;
        MoveDirection = default;
        // SelectTile    = default;
    }
}



public class InputHandler_Grid_Select : InputHandler
{   

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    int                     SelectTile_X       { get; set; } = 0;
    int                     SelectTile_Y       { get; set; } = 0;
         
         
    bool                    IsFinish           { get; set; } = false;                
    float                   MoveTile_LastTime  { get; set; } = 0f;
    Vector2Int              MoveDirection      { get; set; } = Vector2Int.zero;    
    Int64                   VFX_Select         { get; set; } = 0;
    Int64                   SelectedEntityID   { get; set; } = 0; 


    


    public InputHandler_Grid_Select(InputHandlerContext _context) 
        : base(_context)
    {
    }


    protected override void OnStart()
    {
        // ����Ʈ ����.
        var vfx_param = ObjectPool<VFXObject.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_SELECTION);

        VFX_Select = VFXManager.Instance.CreateVFXAsync(vfx_param); 
        //.ContinueWith(OnCompleteVFXTask);

        // // ����Ʈ ���� ���� ��Ͽ� �߰�.
        // VFXManager.Instance.ReserveReleaseVFX(VFX_Selection);

    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        // �Է��� ������� ����.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // �Է��� ������� ó��.
        OnUpdate_Input_Process(input_result);

        // Ÿ�� �̵�.
        OnUpdate_Tile_Move();

        // �̵� ���� ǥ��.
        OnUpdate_DrawMoveRange();


        ObjectPool<InputParam_Result>.Return(input_result);
        
        
        return IsFinish;
    }

    protected override void OnFinish()
    {
        ReleaseVFX();
        
        IsFinish          = false;
        VFX_Select        = 0;
        MoveTile_LastTime = 0f; 
        MoveDirection     = Vector2Int.zero;
        SelectedEntityID  = 0;
        // SelectedEntityBasePosition = (0, 0);


        // SelectTile_X      = 0;
        // SelectTile_Y      = 0;
    }

    protected override void OnPause()
    {
        // throw new NotImplementedException();
    }

    protected override void OnResume()
    {
        // throw new NotImplementedException();
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

        if (SelectedEntityID > 0)
        {
            //var entity         = EntityManager.Instance.GetEntity(SelectedEntityID);
            //var faction        = entity?.GetFaction() ?? 0;
            //var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            // TESTCODE:            
            var gui_id        = GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(SelectedEntityID));
            var input_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create());

            input_handler.Push_GUI(gui_id);

            //Debug.LogWarning($"InputHandler_Grid_Select OnUpdate_Input_Process_Select, gui_id: {gui_id}");
            SetChildHandler(input_handler);
        }
        else
        {

            var entity_id = terrain_map.BlockManager.FindEntityID(SelectTile_X, SelectTile_Y);
            if (entity_id > 0)
            {
                var entity         = EntityManager.Instance.GetEntity(entity_id);
                var faction        = entity.GetFaction();
                var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);

                switch(commander_type)
                {                 
                    case EnumCommanderType.Player:
                    {
                        SelectedEntityID           = entity_id;
                    }
                    break;  

                    case EnumCommanderType.None:
                    case EnumCommanderType.AI:
                    {
                        // TODO: ����â ����.
                    }
                    break;
                }
            }
            else
            {
                //InputManager.Instance.StackHandler(EnumInputHandlerType.UI_Menu);
            }

        }


        return;
    }

    void OnUpdate_Input_Process_Cancel()
    {
        if (SelectedEntityID > 0)
        {
            var entity = EntityManager.Instance.GetEntity(SelectedEntityID);
            if (entity != null)
            {
                var select_tile   = (SelectTile_X, SelectTile_Y);
                var base_position = entity.PathBasePosition;

                if (select_tile != base_position)
                {
                    MoveTile_LastTime = Time.time;
                    MoveSelcectedTile(base_position.x, base_position.y);
                    MoveSelectedEntity(
                        SelectedEntityID, 
                        base_position, 
                        _is_immediate: true, 
                        _is_plan: false);
                }
                else
                {
                    SelectedEntityID = 0;
                }
            }
        }
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
        SelectTile_X += MoveDirection.x;
        SelectTile_Y += MoveDirection.y;

        // �̵� �ð� ����.
        MoveTile_LastTime = Time.time;

        MoveSelcectedTile(SelectTile_X, SelectTile_Y);

        MoveSelectedEntity(
            SelectedEntityID, 
            (SelectTile_X, SelectTile_Y), 
            _is_immediate: false, 
            _is_plan: true);

        
    }

    void OnUpdate_DrawMoveRange()
    {
        var terrain_map = TerrainMapManager.Instance.TerrainMap;
        if (terrain_map == null)
        {
            Debug.LogError("TerrainMapManager.Instance.TerrainMap is null");
            return;
        }

        // ���õ� Ÿ���� ��ƼƼ ID.
        Int64 tile_entity_id = terrain_map.BlockManager.FindEntityID(SelectTile_X, SelectTile_Y);
        
        // �̵� ���� ǥ�� ��ƼƼ ID.
        Int64 draw_entity_id = SelectedEntityID > 0 ? SelectedEntityID : tile_entity_id;

        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: 
                  (int)Battle.MoveRange.EnumDrawFlag.MoveRange 
                | (int)Battle.MoveRange.EnumDrawFlag.AttackRange,

            _entityID:    
                draw_entity_id);

        if (draw_entity_id == 0)
        {
            BattleSystemManager.Instance.DrawRange.Clear();
        }
    }

    private void MoveSelcectedTile(int _x, int _y)
    {
        SelectTile_X = _x;
        SelectTile_Y = _y;

        // ����Ʈ ��ġ �̵�.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<VFX_TransformEvent>.Acquire()
            .SetID(VFX_Select)
            .SetPosition((SelectTile_X, SelectTile_Y).CellToPosition())                
        );       
    }

    private void MoveSelectedEntity(Int64 _entity_id, (int x, int y) _cell, bool _is_immediate, bool _is_plan)
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
                _is_immediate,
                _is_plan
            )
        );
    }


    void ReleaseVFX()
    {
        // ���� ����Ʈ ����.
        VFXManager.Instance.ReserveReleaseVFX(VFX_Select);
        VFX_Select = 0;

        // �̵� ���� ǥ�� �ʱ�ȭ.
        BattleSystemManager.Instance.DrawRange.Clear();   
    }

}
