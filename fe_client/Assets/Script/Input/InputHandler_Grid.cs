using System.Collections.Generic;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class InputHandler_Grid_Select : InputHandler
{    
    
    const float  MOVE_TILE_INTERVAL = 0.15f;
    const float  MOVE_DIRECTION_MIN = 0.4f;
    const string VFX_SELECTION      = "local_base/tile_selection";

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    bool                    IsFinish          { get; set; } = false;                
    int                     SelectTile_X      { get; set; } = 0;
    int                     SelectTile_Y      { get; set; } = 0;
    float                   MoveTile_LastTime { get; set; } = 0f;

    int                     VFX_Selection     { get; set; } = 0;

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
        // CancelTokenSource = new CancellationTokenSource();
        // 이펙트 생성
        // VFXManager.Instance.CreateVFX(VFX_SELECTION, CancelTokenSource.Token)

        VFX_Selection = VFXManager.Instance.CreateVFXAsync(VFX_SELECTION);//.ContinueWith(OnCompleteVFXTask);
    }

    protected override bool OnUpdate()
    {
        // 입력 파람 처리.
        var proc_result = OnUpdate_ProcParam(m_context.InputParamQueue);

       
        if (proc_result.IsSelect)
        {
            // 선택 처리.
            OnUpdate_Select();
        }
        else if (proc_result.IsCancel)
        {
            // 취소 처리. (종료)
            IsFinish = true;
        }
        else if (proc_result.MoveDirection != Vector2.zero)
        {
            // 타일 이동.
            OnUpdate_Move(proc_result.MoveDirection);
        }
        else if (proc_result.SelectTile != Vector2Int.zero)
        {
            // 타일 선택.
            // OnUpdate_Select(proc_result.SelectTile);
        }
        
        return IsFinish;
    }

    protected override void OnFinish()
    {
        VFXManager.Instance.ReserveReleaseVFX(VFX_Selection);

        // if (CancelTokenSource != null)
        // {
        //     CancelTokenSource.Cancel();
        //     CancelTokenSource.Dispose();
        //     CancelTokenSource = null;
        // }
    }

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

    // private void OnCompleteVFXTask(VFXObject _vfx_object)
    // {
    //     // if (CancelTokenSource != null)
    //     // {
    //     //     CancelTokenSource.Dispose();
    //     //     CancelTokenSource = null;
    //     // }
    // }
}
