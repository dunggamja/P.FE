using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class InputHandler_UI_Menu : InputHandler
{
    class InputParam_Result : IPoolObject
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

        

        public static HandlerContext Create()
        {
            return new HandlerContext();
        }
    }

    public InputHandler_UI_Menu(InputHandlerContext _context) 
        : base(_context)
    {
    }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.UI_Menu;



    public bool  IsFinish           { get; private set; } = false;
    Vector2Int   MoveDirection      { get; set; }         = Vector2Int.zero; 
    float        MoveInput_LastTime { get; set; }         = 0f;
    
    // Stack<Int64> Stack_GUI          { get; set; } = new();

    Int64 FocusGUI
    {
        get
        {
            return GUIManager.Instance.GetInputFocusGUI();
        }
    }


    void Reset()
    {
        IsFinish      = false;
        MoveDirection = Vector2Int.zero;
        // Stack_GUI.Clear();
        // GUI_ID        = 0;
    }

    // public void Push_GUI(Int64 _gui_id)
    // {
    //     Stack_GUI.Push(_gui_id);
    // }

    public bool Close_FocusGUI()
    {
        if (FocusGUI > 0)
        {
            GUIManager.Instance.CloseUI(FocusGUI);        
            return true;
        }

        return false;
    }

    protected override void OnStart()
    {
        var context = Context as HandlerContext;
        if (context == null)
        {
            Debug.LogError("InputHandlerContext is not HandlerContext");
            return;
        }

        // GUI_ID = context.GUI_ID;
        //Debug.LogWarning($"InputHandler_UI_Menu OnStart, GUI_ID: {GUI_ID}");
    }

    protected override bool OnUpdate()
    {
        var input_result = ObjectPool<InputParam_Result>.Acquire();

        OnUpdate_Input_Compute(Context.InputParamQueue, ref input_result);
        OnUpdate_Input_Process(input_result);

        OnUpdate_Menu_Move();
        ObjectPool<InputParam_Result>.Return(input_result);


        // 메뉴가 없으면 종료.
        if (FocusGUI == 0)
        {
            IsFinish = true;
        }      


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
        var gui_id = FocusGUI;
        if (gui_id == 0)
            return;

        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<GUI_Menu_SelectEvent>.Acquire().Set(gui_id));
    }

    private void OnUpdate_Input_Process_Cancel()
    {
        Close_FocusGUI();
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

    void OnUpdate_Menu_Move()
    {
        // 이동 방향이 없으면 종료.
        if (MoveDirection == Vector2Int.zero)
            return;
        
        // 이동 시간이 지났는지 확인.
        var is_time_passed = (Time.time - MoveInput_LastTime > MOVE_INTERVAL);
        if (is_time_passed == false)
            return;

        // 이동 시간 갱신.
        MoveInput_LastTime = Time.time;

        // 이동 방향 이벤트 발생.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<GUI_Menu_MoveEvent>.Acquire().Set(FocusGUI, MoveDirection));
    }

    protected override void OnFinish()
    {
        //Debug.LogWarning($"InputHandler_UI_Menu OnFinish, GUI_ID: {GUI_ID}");

        // 열려있는 GUI 모두 닫아줍시다.
        while(Close_FocusGUI())
        {
            // 루프 돌면서 닫아줍시다.
        }


        // 초기화.
        Reset();
    }

    protected override void OnPause()
    {
        // throw new NotImplementedException();
    }

    protected override void OnResume()
    {
        // throw new NotImplementedException();
    }
}
