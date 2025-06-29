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
        Update,  // 반복
        Pause,   // 일시정지
        Finish,  // 종료
    }

    protected const float  MOVE_INTERVAL      = 0.15f;
    protected const float  MOVE_DIRECTION_MIN = 0.4f;

    public abstract EnumInputHandlerType HandlerType { get; }
    
    public EnumState    State        { get; private set; } = EnumState.None;
    public InputHandler ChildHandler { get; private set; } = null; 
    // public bool         IsAbort      { get; protected set; } = false;
    
    public readonly InputHandlerContext Context = null;

    public InputHandler(InputHandlerContext _context)
    {
        Context = _context;
    }


    protected abstract void OnStart();
    protected abstract bool OnUpdate();
    protected abstract void OnFinish();
    protected abstract void OnPause();
    protected abstract void OnResume();

    public bool IsUpdate      => State == EnumState.Update || State == EnumState.Pause;


    public void SetChildHandler(InputHandler _child_handler)
    {
        ChildHandler = _child_handler;
    }


    public bool Update()
    {
        if (IsUpdate == false)
        {
            OnStart();
            State = EnumState.Update;

            // 
            Context.Clear();
        }

        // 자식 핸들러가 있으면 그걸 먼저 처리.        
        if (ChildHandler != null)
        {
            if (State == EnumState.Update)
            {
                // 일시정지.
                State = EnumState.Pause;
                OnPause();
            }

            if (ChildHandler.Update())
            {
                // 자식 핸들러 종료 시                
                
                // 재개.
                State = EnumState.Update;
                OnResume();        

                ChildHandler = null;        
            }           
        }
        else
        {
            if (OnUpdate())
            {
                State = EnumState.Finish;
            }
        }
        

        if (IsUpdate == false)
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
        if (IsUpdate)
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
