using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class InputHandler_UI_Menu : InputHandler
{
    struct InputParam_Result : IPoolObject
    {
        public bool                             IsSelect      { get; set; }
        public bool                             IsCancel      { get; set; }
        public (bool changed, Vector2    value) MoveDirection { get; set; }
        public (bool changed, Vector2Int value) SelectTile    { get; set; }

        public void Reset()
        {
            IsSelect      = false;
            IsCancel      = false;
            MoveDirection = default;
            SelectTile    = default;
        }
    }


    public class HandlerContext : InputHandlerContext
    {
        private HandlerContext() { }

        public Int64 GUI_ID { get; private set; } = 0;

        public HandlerContext SetGUI(Int64 _gui_id)
        {
            GUI_ID = _gui_id;
            return this;
        }

        public static HandlerContext Create(Int64 _gui_id)
        {
            return new HandlerContext()
                .SetGUI(_gui_id);
        }
    }

    public InputHandler_UI_Menu(InputHandlerContext _context) 
        : base(_context)
    {
    }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.UI_Menu;



    public bool IsFinish      { get; private set; } = false;
    Vector2Int  MoveDirection { get; set; }         = Vector2Int.zero; 
    Int64       GUI_ID        { get; set; }         = 0;

    void Reset()
    {
        IsFinish      = false;
        MoveDirection = Vector2Int.zero;
        GUI_ID        = 0;
    }

    protected override void OnStart()
    {
        Reset();

        var context = Context as HandlerContext;
        if (context == null)
        {
            Debug.LogError("InputHandlerContext is not HandlerContext");
            return;
        }

        GUI_ID = context.GUI_ID;
        Debug.LogWarning($"InputHandler_UI_Menu OnStart, GUI_ID: {GUI_ID}");
    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);
        OnUpdate_Input_Process(input_result);


        ObjectPool<InputParam_Result>.Return(input_result);

        return IsFinish;
    }


    private void OnUpdate_Input_Compute(Queue<InputParam> inputParamQueue, ref InputParam_Result input_result)
    {
        while (inputParamQueue.Count > 0)
        {
            var input_param = inputParamQueue.Dequeue();

            switch (input_param)
            {
                case InputParam_UI_Move move:
                {
                    input_result.MoveDirection = (true, move.Direction);
                }
                break;  


                case InputParam_UI_Select _:
                {
                    input_result.IsSelect = true;
                }
                break;
                
                case InputParam_UI_Cancel _:
                {
                    input_result.IsCancel = true;
                }
                break;
            }
        }
    }

    private void OnUpdate_Input_Process(InputParam_Result input_result)
    {
        if (input_result.IsSelect)
        {
            OnUpdate_Input_Process_Select();
        }
        else if (input_result.IsCancel)
        {
            OnUpdate_Input_Process_Cancel();
        }
        else if (input_result.MoveDirection.changed)
        {
            OnUpdate_Input_Process_Move(input_result.MoveDirection.value);
        }
    }

    private void OnUpdate_Input_Process_Select()
    {
        // TODO: 메뉴가 생기면 선택 처리.
        IsFinish = true;
    }

    private void OnUpdate_Input_Process_Cancel()
    {
        IsFinish = true;
    }

    private void OnUpdate_Input_Process_Move(Vector2 _move_direction)
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

        // 메뉴가 생기면 이동 처리.
        MoveDirection = new Vector2Int(input_direction_x, input_direction_y);
    }

    protected override void OnFinish()
    {
        Debug.LogWarning($"InputHandler_UI_Menu OnFinish, GUI_ID: {GUI_ID}");

        // 메뉴 닫기.
        GUIManager.Instance.CloseUI(GUI_ID);

        // 초기화.
        Reset();

    }
}
