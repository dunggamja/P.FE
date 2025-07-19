using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseContainer
{
    Dictionary<int, int>        m_real_values        = new Dictionary<int, int>();

    // 계획을 세우기 위한 값들. Stack 형태로 관리...!
    Stack<Dictionary<int, int>> m_stack_for_planning = new Stack<Dictionary<int, int>>();


    public int GetValue(int _key, bool _is_plan = false)
    {
        if (_is_plan && 0 < m_stack_for_planning.Count)
        {
            // 계획용 값
            return m_stack_for_planning.Peek().TryGetValue(_key, out var value) ? value : 0;
        }
        else
        {
            // 진짜 값
            return m_real_values.TryGetValue(_key, out var value) ? value : 0;
        }
    }

    public bool HasValue(int _key, bool _is_plan = false)
    {
        return 0 != GetValue(_key, _is_plan);
    }

    public void SetValue(int _key, int _value, bool _is_plan = false)
    {
        if (_is_plan)
        {
            // 계획용 값 셋팅.
            PushPlanValue(_key, _value);
        }
        else
        {
            // 진짜 값 셋팅.
            if (m_real_values.ContainsKey(_key))
            {
                m_real_values[_key] = _value;
            }
            else
            {
                m_real_values.Add(_key, _value);
            }
        }
    }

    public void SetValue(int _key, bool _value, bool _is_plan = false) => SetValue(_key, _value ? 1 : 0, _is_plan);


    public void PushPlanValue(int _key, int _value)
    {
        var original  = 0 < m_stack_for_planning.Count ? m_stack_for_planning.Peek() : m_real_values;
        var new_stack = new Dictionary<int, int>(original);
        if (new_stack.ContainsKey(_key))
        {
            new_stack[_key] = _value;
        }
        else
        {
            new_stack.Add(_key, _value);
        }

        m_stack_for_planning.Push(new_stack);
    }

    public void PushPlanValue(int _key, bool _value) => PushPlanValue(_key, _value ? 1 : 0);

    public void PopPlanValue()
    {
        if (m_stack_for_planning.Count == 0)
        {
            Debug.LogError("m_stack_for_planning is already empty!!");
            return;
        }

        m_stack_for_planning.Pop();
    }




    public void ResetPlan()
    {
        m_stack_for_planning.Clear();
    }

    public void Reset()
    {
        m_real_values.Clear();
        m_stack_for_planning.Clear();
    }
        
}
