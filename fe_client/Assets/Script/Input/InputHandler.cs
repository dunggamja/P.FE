using System.Collections.Generic;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid,           // 타일 선택
    Grid_Command,   // 유닛 행동 

    UI,             // UI 기본.
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
    protected abstract bool OnUpdate(InputParam _input_param);
    protected abstract void OnFinish();

    protected abstract void OnPause();
    protected abstract void OnResume();


    public bool Update(InputParam _input_param)
    {
        if (State != EnumState.Update)
        {
            OnStart();
            State = EnumState.Update;
        }

        if (OnUpdate(_input_param) == true)
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

public class InputHandler_UI : InputHandler
{
    // public InputHandler_UI(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
    // {
    // }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.UI;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate(InputParam _input_param)
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

public class InputHandler_Grid : InputHandler
{
    // public InputHandler_Grid(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
    // {

    // }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate(InputParam _input_param)
    {
        switch(_input_param)
        {
            case InputParam_Grid_Move input_param_move:
            {
                Debug.LogWarning(input_param_move.Direction);
            }
                break;
        }

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

public class InputHandler_Grid_Command : InputHandler
{
    // public InputHandler_Grid_Command(Queue<InputParam> _queue_input_param) : base(_queue_input_param)
    // {

    // }

    public override EnumInputHandlerType HandlerType => EnumInputHandlerType.Grid_Command;

    protected override void OnStart()
    {
    }

    protected override bool OnUpdate(InputParam _input_param)
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
    