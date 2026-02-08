using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseContainer
{
    Dictionary<int, Int64>        m_real_values = new ();

    public Int64 GetValue(int _key)
    {
        {
            // 진짜 값
            return m_real_values.TryGetValue(_key, out var value) ? value : 0;
        }
    }

    public bool HasValue(int _key)
    {
        return 0 != GetValue(_key);
    }

    public void SetValue(int _key, Int64 _value)
    {
        
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

    public void SetValue(int _key, bool _value) => SetValue(_key, _value ? 1 : 0);


    public void Reset()
    {
        m_real_values.Clear();
    }

    public BaseContainer_IO Save()
    {
        var values = new List<(int, Int64)>();
        foreach (var item in m_real_values)
        {
            values.Add((item.Key, item.Value));
        }


        return new BaseContainer_IO()
        {
            Values = values
        };
    }

    public void Load(BaseContainer_IO _snapshot)
    {
        m_real_values.Clear();
        foreach (var item in _snapshot.Values)
        {
            m_real_values.Add(item.key, item.value);
        }
    }
        
}

public class BaseContainer_IO
{
    public List<(int key, Int64 value)> Values { get; set; } = new();
}
