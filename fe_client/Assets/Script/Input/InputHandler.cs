using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid_Select,    // Ÿ�� ����
    // Grid_Command,   // ���ֿ��� ������ ������ (�̵�,����,��ų ��) 

    UI_Menu,        // UI �޴�
    UI_Command,     // ���� �ൿ �޴� (�̵�, ����, ��ų)
    
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
        Update,  // �ݺ�
        Pause,   // �Ͻ�����
        Finish,  // ����
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

        // 자식 입력 처리 중인 경우 입력 처리 진행.        
        if (ChildHandler != null)
        {
            if (State == EnumState.Update)
            {
                // 일시 정지.
                State = EnumState.Pause;
                OnPause();
            }

            if (ChildHandler.Update())
            {
                // 자식 입력 처리 완료.                
                
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
