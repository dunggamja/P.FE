using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPoolObject
{
    void Reset();
}


public static class ObjectPool<T> where T : IPoolObject, new()
{
    private static readonly Stack<T>   m_pool  = new();

    private static readonly HashSet<T> m_inuse = new();



    public static T Acquire()
    {
        var pool_object = m_pool.Count > 0 ? m_pool.Pop() : new T();
        
        m_inuse.Add(pool_object);

        return pool_object;
    }

    public static void Release(T _obj)
    {
        if (_obj == null)
            return;

        // Ǯ���� �������� ������ ��ü�� �ƴ϶�� ��ȯ ó�� ���� �ʴ´�.
        if (m_inuse.Contains(_obj) == false)
            return;
        
        
        _obj.Reset();

        m_inuse.Remove(_obj);
        m_pool.Push(_obj);
    }
}