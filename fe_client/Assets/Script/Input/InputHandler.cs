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

    public abstract EnumInputHandlerType HandlerType { get; }
    public EnumState State { get; private set; } = EnumState.None;


    private InputHandler m_child_handler = null;
    
    protected readonly InputHandlerContext m_context;

    public InputHandler(InputHandlerContext _context)
    {
        m_context = _context;
    }


    protected abstract void OnStart();
    protected abstract bool OnUpdate();
    protected abstract void OnFinish();


    protected void SetChildHandler(InputHandler _child_handler)
    {
        m_child_handler = _child_handler;
    }


    public bool Update()
    {
        if (State != EnumState.Update)
        {
            OnStart();
            State = EnumState.Update;

            // 
            m_context.Clear();
        }

        // �ڽ� �ڵ鷯�� ������ �װ� ���� ó��.        
        if (m_child_handler != null)
        {
            if (m_child_handler.Update())
                m_child_handler = null;
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
            m_context.Clear();
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

}
