using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid_Select,    // 타일 선택
    // Grid_Command,   // 유닛에게 명령을 내릴때 (이동,공격,스킬 등) 

    UI_Menu,        // UI 메뉴
    UI_Command,     // 유닛 행동 메뉴 (이동, 공격, 스킬)
    
}

public class InputHandlerContext
{
    public Queue<InputParam> InputParamQueue { get; set; } = new Queue<InputParam>();
    // public bool IsFinish { get; set; } = false;

    public void Clear()
    {
        InputParamQueue.Clear();
        // IsFinish = false;
    }
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

    protected const float  MOVE_INTERVAL      = 0.15f;
    protected const float  MOVE_DIRECTION_MIN = 0.4f;

    public abstract EnumInputHandlerType HandlerType { get; }
    
    public EnumState    State        { get; private set; } = EnumState.None;
    public InputHandler ChildHandler { get; private set; } = null; 
    
    public readonly InputHandlerContext Context = null;

    public InputHandler(InputHandlerContext _context)
    {
        Context = _context;
    }


    protected abstract void OnStart();
    protected abstract bool OnUpdate();
    protected abstract void OnFinish();


    protected void SetChildHandler(InputHandler _child_handler)
    {
        ChildHandler = _child_handler;
    }


    public bool Update()
    {
        if (State != EnumState.Update)
        {
            OnStart();
            State = EnumState.Update;

            // 
            Context.Clear();
        }

        // 자식 핸들러가 있으면 그걸 먼저 처리.        
        if (ChildHandler != null)
        {
            if (ChildHandler.Update())
                ChildHandler = null;
        }
        else
        {
            if (OnUpdate())
            {
                State = EnumState.Finish;
            }
        }

        

        if (State != EnumState.Update)
        {
            OnFinish();
            State = EnumState.Finish;

            // 
            Context.Clear();
        }

        return State == EnumState.Finish;
    }

    public void Abort()
    {
        if (State == EnumState.Update)
        {
            if (ChildHandler != null)
            {
                ChildHandler.Abort();
                ChildHandler = null;
            }

            OnFinish();

            State = EnumState.Finish;
        }
    }

}
