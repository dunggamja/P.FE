using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    Grid_Select,    // Ÿ�� ����
    // Grid_Command,   // ���ֿ��� ����� ������ (�̵�,����,��ų ��) 

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
        Start,   // ����
        Update,  // �ݺ�
        Finish,  // ����
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

        // �ڽ� �ڵ鷯�� ������ �װ� ���� ó��.        
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
