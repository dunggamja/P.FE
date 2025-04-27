using System;
using System.Collections.Generic;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputHandler_Grid_Select : InputHandler
{   
    
    struct InputParam_Result : IPoolObject
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
 
    
    
    const string VFX_SELECT_NAME    = "local_base/tile_selection";

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Select;

    int                     SelectTile_X      { get; set; } = 0;
    int                     SelectTile_Y      { get; set; } = 0;


    bool                    IsFinish          { get; set; } = false;                
    float                   MoveTile_LastTime { get; set; } = 0f;
    Vector2Int              MoveDirection     { get; set; } = Vector2Int.zero;    
    int                     VFX_Select_ID     { get; set; } = 0;
    Int64                   SelectedEntityID  { get; set; } = 0; 

    


    public InputHandler_Grid_Select(InputHandlerContext _context) 
        : base(_context)
    {
    }


    protected override void OnStart()
    {
        // 이펙트 생성.
        VFX_Select_ID = VFXManager.Instance.CreateVFXAsync(VFX_SELECT_NAME); 
        //.ContinueWith(OnCompleteVFXTask);

        // // 이펙트 삭제 예정 목록에 추가.
        // VFXManager.Instance.ReserveReleaseVFX(VFX_Selection);

    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        // 입력의 결과값을 생성.
        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);

        // 입력의 결과값을 처리.
        OnUpdate_Input_Process(input_result);

        // 타일 이동.
        OnUpdate_Tile_Move();


        ObjectPool<InputParam_Result>.Return(input_result);
        
        
        return IsFinish;
    }

    protected override void OnFinish()
    {
        
        VFXManager.Instance.ReserveReleaseVFX(VFX_Select_ID);
        
        IsFinish          = false;
        VFX_Select_ID     = 0;
        MoveTile_LastTime = 0f; 
        MoveDirection     = Vector2Int.zero;
        SelectedEntityID  = 0;


        // SelectTile_X      = 0;
        // SelectTile_Y      = 0;
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

        if (SelectedEntityID > 0)
        {
            //var entity         = EntityManager.Instance.GetEntity(SelectedEntityID);
            //var faction        = entity?.GetFaction() ?? 0;
            //var commander_type = BattleSystemManager.Instance.GetFactionCommanderType(faction);

            // TESTCODE:            
            var gui_id        = GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(SelectedEntityID));
            var input_handler = new InputHandler_UI_Menu(InputHandler_UI_Menu.HandlerContext.Create(gui_id));

            Debug.LogWarning($"InputHandler_Grid_Select OnUpdate_Input_Process_Select, gui_id: {gui_id}");
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

                // // TESTCODE:            
                // GUIManager.Instance.OpenUI(GUIPage_Unit_Command.PARAM.Create(entity_id));


                switch(commander_type)
                {                 
                    case EnumCommanderType.Player:
                    {
                        SelectedEntityID = entity_id;
                    }
                    break;  

                    case EnumCommanderType.None:
                    case EnumCommanderType.AI:
                    {
                        // TODO: 상태창 열기.
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
            SelectedEntityID = 0;
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
        // 이동 방향이 없으면 종료.
        if (MoveDirection == Vector2Int.zero)
            return;
        
        // 이동 시간이 지났는지 확인.
        var is_time_passed = (Time.time - MoveTile_LastTime > MOVE_INTERVAL);
        if (is_time_passed == false)
            return;

        // 이동 시간 갱신.
        MoveTile_LastTime = Time.time;

        // 타일 이동.
        SelectTile_X += MoveDirection.x;
        SelectTile_Y += MoveDirection.y;

        var VFX_Position = new Vector3(SelectTile_X, 0f, SelectTile_Y);

        

        // 이펙트 위치 이동.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<VFXTransformEvent>.Acquire()
            .SetID(VFX_Select_ID)
            .SetPosition(VFX_Position)                
        );

        // 유닛 이동.
        if (SelectedEntityID > 0)
        {
            var entity  = EntityManager.Instance.GetEntity(SelectedEntityID);
            if (entity != null)
            {
                entity.UpdateCellPosition(SelectTile_X, SelectTile_Y);
            }
        }
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
